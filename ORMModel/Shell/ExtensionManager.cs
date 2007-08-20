#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
*                                                                          *
* The use and distribution terms for this software are covered by the      *
* Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
* can be found in the file CPL.txt at the root of this distribution.       *
* By using this software in any fashion, you are agreeing to be bound by   *
* the terms of this license.                                               *
*                                                                          *
* You must not remove this notice, or any other, from this software.       *
\**************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Shell.Interop;
using Neumont.Tools.Modeling.Design;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.Modeling.Shell;
using System.Collections;
using System.Globalization;

namespace Neumont.Tools.ORM.Shell
{
	public sealed partial class ExtensionManager : Form
	{
		private readonly Store _store;
		private readonly List<Type> _loadedDomainModelTypes;
		private readonly IList<ORMExtensionType> _allExtensionTypes;

		/// <summary>
		/// Initialize the ExtensionManager form
		/// </summary>
		private ExtensionManager(Store store)
		{
			InitializeComponent();
			this._store = store;
			this._allExtensionTypes = ORMDesignerPackage.GetAvailableCustomExtensions();
			ICollection<DomainModel> domainModels = store.DomainModels;
			this._loadedDomainModelTypes = new List<Type>(domainModels.Count);
			foreach (DomainModel domainModel in domainModels)
			{
				this._loadedDomainModelTypes.Add(domainModel.GetType());
			}
		}
		/// <summary>
		/// This method shows the ExtensionManager form.
		/// </summary>
		/// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
		/// <param name="docData">The docData retrieved from the current document.</param>
		[CLSCompliant(false)]
		public static void ShowDialog(IServiceProvider serviceProvider, ORMDesignerDocData docData)
		{
			ExtensionManager extensionManager = new ExtensionManager(docData.Store);
			if (extensionManager.ShowDialog(new OwnerWindow(serviceProvider)) == DialogResult.OK)
			{
				// TODO: Prompt the user to make sure they really want us to start deleting stuff...

				ListView.CheckedListViewItemCollection checkedItems = extensionManager.lvExtensions.CheckedItems;
				List<ORMExtensionType> checkedTypes = new List<ORMExtensionType>(checkedItems.Count);
				foreach (ListViewItem listViewItem in checkedItems)
				{
					checkedTypes.Add((ORMExtensionType)listViewItem.Tag);
				}

				IList<ORMExtensionType> allExtensionTypes = extensionManager._allExtensionTypes;

				List<Guid> loadedDomainModelIds = new List<Guid>(4 + checkedTypes.Count);
				loadedDomainModelIds.Add(CoreDomainModel.DomainModelId);
				loadedDomainModelIds.Add(Microsoft.VisualStudio.Modeling.Diagrams.CoreDesignSurfaceDomainModel.DomainModelId);
				loadedDomainModelIds.Add(ObjectModel.ORMCoreDomainModel.DomainModelId);
				loadedDomainModelIds.Add(ShapeModel.ORMShapeDomainModel.DomainModelId);
				foreach (ORMExtensionType extensionType in checkedTypes)
				{
					loadedDomainModelIds.Add(extensionType.Type.GUID);
				}

				for (int i = 0; i < checkedTypes.Count; i++)
				{
					foreach (ExtendsDomainModelAttribute extendsDomainModelAttribute in checkedTypes[i].Type.GetCustomAttributes(typeof(ExtendsDomainModelAttribute), false))
					{
						Guid extendedModelId = extendsDomainModelAttribute.ExtendedModelId;
						if (!loadedDomainModelIds.Contains(extendedModelId))
						{
							// Find the missing domain model
							foreach (ORMExtensionType candidateExtensionType in allExtensionTypes)
							{
								object[] domainObjectIdAttributes = candidateExtensionType.Type.GetCustomAttributes(typeof(DomainObjectIdAttribute), false);
								if (domainObjectIdAttributes.Length <= 0)
								{
									continue;
								}
								Guid candidateModelId = ((DomainObjectIdAttribute)domainObjectIdAttributes[0]).Id;
								if (extendedModelId.Equals(candidateModelId))
								{
									loadedDomainModelIds.Add(candidateModelId);
									checkedTypes.Add(candidateExtensionType);
									break;
								}
							}
							// If we didn't find the requested domain model, we don't need to worry about doing anything here,
							// since the Store will throw later when they try to load the requesting domain model.
						}
					}
				}

				Stream stream = null;
				try
				{
					Object streamObj;
					(docData as EnvDTE.IExtensibleObject).GetAutomationObject("ORMXmlStream", null, out streamObj);
					stream = streamObj as Stream;

					Debug.Assert(stream != null);

					stream = CleanupStream(stream, checkedTypes);
					docData.ReloadFromStream(stream);
				}
				finally
				{
					if (stream != null)
					{
						stream.Dispose();
					}
				}
			}
		}
		/// <summary>
		/// This is a custom callback class for the XSLT file that is
		/// responsible for adding or removing the custom extension namespaces to the ORM document.
		/// </summary>
		private sealed class NamespaceUtility
		{
			private readonly string[] _namespaces;
			private readonly List<string> _addedNamespaces;
			private bool _hasEnumerator;
			private IEnumerator<string> _enumerator;
			private bool _hasCurrent;
			private static readonly Random random = new Random();
			/// <summary>
			/// Default Constructor for the <see cref="NamespaceUtility"/>.
			/// </summary>
			/// <param name="namespaces">An array of available namespaces.</param>
			public NamespaceUtility(string[] namespaces)
			{
				this._namespaces = namespaces;
				Array.Sort<string>(namespaces);
				this._addedNamespaces = new List<string>(namespaces.Length);
			}
			/// <summary>
			/// This method checks to see if the namespace was added via the ExtensionManager Dialogue
			/// </summary>
			/// <param name="namespaceUri">The namespace to check if it was added,</param>
			/// <returns>true if the namespace was added false if it was not.</returns>
			public bool wasNamespaceAdded(string namespaceUri)
			{
				return this._addedNamespaces.BinarySearch(namespaceUri) >= 0;
			}
			/// <summary>
			/// This method adds the namespace to the selected list. for future reference.
			/// </summary>
			/// <param name="namespaceUri">the namespace you want to add.</param>
			public void addedNamespace(string namespaceUri)
			{
				this._addedNamespaces.Add(namespaceUri);
			}
			/// <summary>
			/// This is an Iterator helper class to move accross the available namespaces
			/// you wish to add.
			/// </summary>
			/// <returns>The current namespace position if there is a next one. an empty string if there is not.</returns>
			public string getNextSelectedNamespace()
			{
				if (!this._hasEnumerator)
				{
					this._enumerator = (this._namespaces as IEnumerable<string>).GetEnumerator();
					this._hasEnumerator = true;
				}
				this._hasCurrent = this._enumerator.MoveNext();
				if (this._hasCurrent)
				{
					return this._enumerator.Current;
				}
				else
				{
					return string.Empty;
				}
			}
			/// <summary>
			/// This method checks if the namespace was selected in the ExtensionManager Dialogue.
			/// </summary>
			/// <param name="namespaceUri">The namespace you wish to check for checked status.</param>
			/// <returns>true if the namespace was selected false if it was not.</returns>
			public bool isNamespaceSelected(string namespaceUri)
			{
				return Array.BinarySearch<string>(this._namespaces, namespaceUri) >= 0;
			}
			/// <summary>
			/// This is a Randomizer to get around the fact that we do not have unique identifiers
			/// for the different namespaces.
			/// </summary>
			/// <returns>the a psuedo random namespace extension to be used as the prefix.</returns>
			public static string getRandomPrefix()
			{
				return "ormExtension" + random.Next();
			}
		}
		/// <summary>
		/// This method is responsible for cleaning the streamed ORM file.
		/// </summary>
		/// <param name="stream">The file stream that contains the ORM file.</param>
		/// <param name="extensionTypes">A list of extension types.</param>
		/// <returns>The cleaned stream.</returns>
		private static Stream CleanupStream(Stream stream, IList<ORMExtensionType> extensionTypes)
		{
			MemoryStream outputStream = new MemoryStream((int)stream.Length);
			XsltArgumentList argList = new XsltArgumentList();

			int extensionsCount = extensionTypes.Count;
			CustomSerializedXmlNamespacesAttribute[] namespaceAttributes = new CustomSerializedXmlNamespacesAttribute[extensionsCount];
			int totalNamespaceCount = 0;
			for (int i = 0; i < extensionsCount; ++i)
			{
				object[] attributes = extensionTypes[i].Type.GetCustomAttributes(typeof(CustomSerializedXmlNamespacesAttribute), false);
				CustomSerializedXmlNamespacesAttribute currentAttribute;
				int currentNamespaceCount;
				if (attributes != null &&
					attributes.Length != 0 &&
					0 != (currentNamespaceCount = (currentAttribute = (CustomSerializedXmlNamespacesAttribute)attributes[0]).Count))
				{
					totalNamespaceCount += currentNamespaceCount;
					namespaceAttributes[i] = currentAttribute;
				}
				else
				{
					// Just use the default one provided
					totalNamespaceCount += 1;
				}
			}
			string[] namespaces = new string[totalNamespaceCount];
			int nextNamespaceIndex = 0;
			for (int i = 0; i < extensionsCount; ++i)
			{
				CustomSerializedXmlNamespacesAttribute currentAttribute;
				if (null != (currentAttribute = namespaceAttributes[i]))
				{
					int attributeCount = currentAttribute.Count;
					for (int j = 0; j < attributeCount; ++j)
					{
						namespaces[nextNamespaceIndex] = currentAttribute[j];
						++nextNamespaceIndex;
					}
				}
				else
				{
					namespaces[nextNamespaceIndex] = extensionTypes[i].NamespaceUri;
					++nextNamespaceIndex;
				}
			}
			argList.AddExtensionObject("urn:schemas-neumont-edu:ORM:NamespacesUtility", new NamespaceUtility(namespaces));
			XslCompiledTransform transform = GetExtensionStripperTransform();

			stream.Position = 0;
			using (XmlReader reader = XmlReader.Create(stream))
			{
				transform.Transform(reader, argList, outputStream);
			}
			outputStream.Position = 0;
			return outputStream;
		}
		/// <summary>
		/// The onload method.  first we gather a list of available custom
		/// extension then we populate the <see cref="ListView"/> with the list.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected sealed override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			lvExtensions.Items.Clear();
			foreach (ORMExtensionType type in this._allExtensionTypes)
			{
				AddItemToListView(type);
			}
			lvExtensions.ListViewItemSorter = ItemComparer.Instance;
		}

		private sealed class ItemComparer : IComparer
		{
			public static readonly IComparer Instance = new ItemComparer();
			private ItemComparer()
			{
			}

			public int Compare(object obj1, object obj2)
			{
				ListViewItem item1 = (ListViewItem) obj1;
				ListViewItem item2 = (ListViewItem) obj2;
				return string.Compare(item1.SubItems[1].Text, item2.SubItems[1].Text, false, CultureInfo.CurrentCulture);
			}
		}
		/// <summary>
		/// This method adds the passed in ORMExtensionType to the ListView on the ExtensionManager dialogue.
		/// </summary>
		/// <param name="ormExtensionType">The extension you want to add.</param>
		private void AddItemToListView(ORMExtensionType ormExtensionType)
		{
			Type type = ormExtensionType.Type;
			ListViewItem lvi = new ListViewItem();
			lvi.Tag = ormExtensionType;
			if (this._loadedDomainModelTypes.Contains(type))
			{
				lvi.Checked = true;
			}

			//Add the DisplayName
			lvi.SubItems.Add(DomainTypeDescriptor.GetDisplayName(type));
			
			//Add the Description
			lvi.SubItems.Add(DomainTypeDescriptor.GetDescription(type));

			lvExtensions.Items.Add(lvi);
		}
		private sealed class OwnerWindow : IWin32Window
		{
			private readonly IntPtr myHandle;
			[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
			private static extern IntPtr GetDesktopWindow();
			public OwnerWindow(IServiceProvider serviceProvider)
			{
				IVsUIShell shell = (IVsUIShell)serviceProvider.GetService(typeof(IVsUIShell));
				if (shell == null || ErrorHandler.Failed(shell.GetDialogOwnerHwnd(out myHandle)) || myHandle == IntPtr.Zero)
				{
					myHandle = GetDesktopWindow();
				}
			}
			public IntPtr Handle
			{
				get
				{
					return myHandle;
				}
			}
		}
		/// <summary>
		/// This method grabs and compiles the XSLT transform that strips or adds custom extension to the ORM file.
		/// </summary>
		/// <returns>The compiled XSLT tranform.</returns>
		private static XslCompiledTransform GetExtensionStripperTransform()
		{
			XslCompiledTransform retVal = new XslCompiledTransform(false);
			Type resourceType = typeof(ExtensionManager);
			using (Stream transformStream = resourceType.Assembly.GetManifestResourceStream(resourceType, "ExtensionStripper.xslt"))
			{
				using (XmlReader reader = XmlReader.Create(transformStream))
				{
					retVal.Load(reader, XsltSettings.TrustedXslt, new XmlUrlResolver());
				}
			}
			return retVal;
		}
	}
}
