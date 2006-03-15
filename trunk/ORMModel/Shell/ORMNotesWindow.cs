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
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.EnterpriseTools.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using Neumont.Tools.ORM.ObjectModel;
using Microsoft.VisualStudio.Modeling;
using Neumont.Tools.ORM.ObjectModel.Editors;
using MSOLE = Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections;

namespace Neumont.Tools.ORM.Shell
{
	/// <summary>
	/// The ToolWindow which is responsible for displaying and allowing
	/// the update of notes on RootTypes.
	/// </summary>
	[Guid("A7C9E14E-9EEE-4D79-A7F4-9E9D1A567498")]
	[CLSCompliant(false)]
	public class ORMNotesToolWindow : ORMToolWindow, MSOLE.IOleCommandTarget
	{
		#region Private data members
		private TextBox myTextBox;
		private List<RootType> mySelectedRootTypes;
		#endregion // Private data members
		#region Construction
		/// <summary>
		/// Returns the ORM Notes Window.
		/// </summary>
		public ORMNotesToolWindow(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			mySelectedRootTypes = new List<RootType>();
			PopulateSelectedRootTypes();
		}
		/// <summary>
		/// Returns the parent of myTextBox.
		/// </summary>
		public override System.Windows.Forms.IWin32Window Window
		{
			get
			{
				TextBox textBox = myTextBox;	// Cache the textbox.
				if (textBox == null)	// If it's null,
				{
					myTextBox = textBox = new TextBox();	// instantiate it and set the properties, wire events,
					textBox.Dock = DockStyle.Fill;
					textBox.WordWrap = true;
					textBox.Multiline = true;
					textBox.ScrollBars = ScrollBars.Vertical;
					textBox.LostFocus += new EventHandler(myTextBox_LostFocus);
					ContainerControl container = new ContainerControl();	// and set up a parent container.
					container.Controls.Add(textBox);
				}
				return textBox.Parent;	// Finally, pass the parent container back as the window.
			}
		}
		#endregion // Construction
		#region Event handlers
		/// <summary>
		/// Handles note added, changed, or removed events.
		/// </summary>
		private void NoteAlteredEventHandler(object sender, EventArgs e)
		{
			DisplayNotes();
		}
		/// <summary>
		/// Event handler for the note textbox.  Forces the note to be saved
		/// if the textbox is enabled.
		/// </summary>
		void myTextBox_LostFocus(object sender, EventArgs e)
		{
			if (mySelectedRootTypes.Count == 1 && myTextBox.Enabled)	// If we only have one selected RootType and the textbox is enabled,
			{
				SetNote(mySelectedRootTypes[0], myTextBox.Text);	// try to set the note.
			}
		}
		#endregion // Event handlers
		#region Helper functions
		/// <summary>
		/// Sets the NoteText property on a RootType.
		/// </summary>
		/// <param name="owner">The RootType on which the note should be set.</param>
		/// <param name="text">The text of the note.</param>
		private void SetNote(RootType owner, string text)
		{
			if (owner != null && owner.Store != null)	// Be really defensive.
			{
				owner.CreatePropertyDescriptor(owner.Store.MetaDataDirectory.FindMetaAttribute(RootType.NoteTextMetaAttributeGuid), owner).SetValue(owner, text);
			}
		}
		/// <summary>
		/// Populates mySelectedRootTypes with all currently selected root
		/// types in the passed in ORMDesignerDocView.
		/// </summary>
		private void PopulateSelectedRootTypes()
		{
			if (mySelectedRootTypes == null)
			{
				mySelectedRootTypes = new List<RootType>();
			}
			mySelectedRootTypes.Clear();	// Clear the list of selected root types.
			IORMSelectionContainer selectionContainer = myCurrentORMSelectionContainer;
			if (selectionContainer != null)
			{

				ICollection objects = base.GetSelectedComponents();
				foreach (object o in objects)
				{
					RootType rootType = EditorUtility.ResolveContextInstance(o, false) as RootType;	// and if they are a RootType,
					if (rootType != null)
					{
						mySelectedRootTypes.Add(rootType);	// add them to the list of selected RootTypes.
					}
				}
			}
			DisplayNotes();	// Display the notes for all selected RootTypes.
		}
		/// <summary>
		/// Displays the notes for all selected RootTypes in the Notes Window.
		/// </summary>
		private void DisplayNotes()
		{
			List<RootType> selectedRootTypes = mySelectedRootTypes; // Cache the list of selected root types.
			int selectedRootTypesCount = selectedRootTypes.Count; // Cache the count of selected root types.
			TextBox textBox = myTextBox; // Cache the text box.
			if (textBox != null) // If the text box is not null,
			{
				textBox.Clear(); // clear it's text
				textBox.ReadOnly = false; // and set it to read only.
				if (selectedRootTypesCount == 0) // If there aren't any selected root types,
				{
					return; // bail out.
				}
				else if (selectedRootTypesCount == 1) // If there is only one selected root type,
				{
					textBox.AppendText(selectedRootTypes[0].NoteText); // add its note text to the text box.
				}
				else
				{
					textBox.ReadOnly = true; // Otherwise, the textbox should be read-only because multiple
					StringBuilder sb = new StringBuilder();
					string formatString = ResourceStrings.ModelNotesWindowRootTypeNameNotesSeparatorFormatString;
					// root types are selected.
					bool first = true;
					for (int i = 0; i < selectedRootTypesCount; ++i) // Loop through the selected root types,
					{
						RootType rootType = selectedRootTypes[i]; // cache them locally,
						string noteText = rootType.NoteText; // and cache their note text.
						if (!string.IsNullOrEmpty(noteText)) // If there is note text on the root type,
						{
							if (!first)
							{
								sb.AppendLine();
							}
							else
							{
								first = false;
							}
							sb.AppendFormat(CultureInfo.InvariantCulture, formatString, rootType.Name, noteText);
						}
					}
					textBox.Text = sb.ToString();
				}
			}
		}
		#endregion // Helper functions
		#region IOleCommandTarget Members
		/// <summary>
		/// Provides a first chance to tell the shell that this window is capable
		/// of handling certain commands. Implements IOleCommandTarget.QueryStatus
		/// </summary>
		protected int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, MSOLE.OLECMD[] prgCmds, IntPtr pCmdText)
		{
			int hr = VSConstants.S_OK;
			bool handled = true;
			if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)	// Only handle commands from the Office 97
			// Command Set (aka VSStandardCommandSet97).
			{
				// There typically is only one command passed in to this array - in any case, we only care
				// about the first command.
				MSOLE.OLECMD cmd = prgCmds[0];
				switch ((VSConstants.VSStd97CmdID)cmd.cmdID)
				{
					case VSConstants.VSStd97CmdID.Delete:
						// Inform the shell that we should have a chance to handle the delete command.
						MSOLE.IOleCommandTarget forwardTo = myTextBox as MSOLE.IOleCommandTarget;
						if (forwardTo != null)
						{
							hr = forwardTo.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
						}
						else
						{
							goto default;
						}
						break;

					default:
						// Inform the shell that we don't support any other commands.
						handled = false;
						hr = (int)MSOLE.Constants.OLECMDERR_E_NOTSUPPORTED;
						break;
				}
			}
			else
			{
				// Inform the shell that we don't recognize this command group.
				handled = false;
				hr = (int)MSOLE.Constants.OLECMDERR_E_UNKNOWNGROUP;
			}
			if (!handled)
			{
				Debug.Assert(ErrorHandler.Failed(hr));
				ModelingDocData docData = myCurrentDocument;
				if (docData != null)
				{
					MSOLE.IOleCommandTarget forwardTo = docData.UndoManager.VSUndoManager as MSOLE.IOleCommandTarget;
					// If the command wasn't handled already, forward it to the undo manager.
					hr = forwardTo.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
				}
				else
				{
					hr = (int)MSOLE.Constants.MSOCMDERR_E_NOTSUPPORTED;
				}
			}
			return hr;
		}
		int MSOLE.IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, MSOLE.OLECMD[] prgCmds, IntPtr pCmdText)
		{
			return QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
		}

		/// <summary>
		/// Provides a first chance to handle any command that MSOLE.IOleCommandTarget.QueryStatus
		/// informed the shell to pass to this window. Implements IOleCommandTarget.Exec
		/// </summary>
		protected int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			int hr = 0;
			bool handled = true;
			// Only handle commands from the Office 97 Command Set (aka VSStandardCommandSet97).
			if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
			{
				// Default to a not-supported status.
				hr = (int)MSOLE.Constants.OLECMDERR_E_NOTSUPPORTED;
				switch ((VSConstants.VSStd97CmdID)nCmdID)
				{
					case VSConstants.VSStd97CmdID.Delete:
						// If we aren't in label edit (in which case the commands should be passed down to the
						// VirtualTreeView control), handle the delete command and set the hresult to a handled status.
						MSOLE.IOleCommandTarget forwardTo = myTextBox as MSOLE.IOleCommandTarget;
						if (forwardTo != null)
						{
							forwardTo.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
							// We enabled the command, so we say we handled it regardless of the further conditions
							hr = VSConstants.S_OK;
						}
						else
						{
							goto default;
						}
						break;

					default:
						// If the command is from our command set, but not explicitly handled, inform the shell
						// that we didn't handle the command.
						handled = false;
						hr = (int)MSOLE.Constants.OLECMDERR_E_NOTSUPPORTED;
						break;
				}
			}
			// The command is from an unknown group.
			else
			{
				handled = false;
				hr = (int)MSOLE.Constants.OLECMDERR_E_UNKNOWNGROUP;
			}

			if (!handled)
			{
				Debug.Assert(ErrorHandler.Failed(hr));
				ModelingDocData docData = myCurrentDocument;
				if (docData != null)
				{
					MSOLE.IOleCommandTarget forwardTo = docData.UndoManager.VSUndoManager as MSOLE.IOleCommandTarget;
					// If the command wasn't handled already, give the undo manager a chance to handle the command.
					hr = forwardTo.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
				}
			}
			return hr;
		}
		int MSOLE.IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			return Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
		}
		#endregion
		#region ORMToolWindow Implementation
		/// <summary>
		/// Retains base CurrentModelElementSelectionContainer implementation and adds a
		/// method call to PopulateSelectedRootTypes.
		/// </summary>
		protected override IORMSelectionContainer CurrentModelElementSelectionContainer
		{
			set
			{
				base.CurrentModelElementSelectionContainer = value;
				PopulateSelectedRootTypes();
			}
		}
		/// <summary>
		/// Attaches event handlers to the store.
		/// </summary>
		protected override void AttachEventHandlers(Store store)
		{
			if (store == null || store.Disposed)
			{
				return; // Bail out
			}
			MetaDataDirectory dataDirectory = store.MetaDataDirectory;
			EventManagerDirectory eventDirectory = store.EventManagerDirectory;

			// Track Note changes
			MetaClassInfo classInfo = dataDirectory.FindMetaRelationship(RootTypeHasNote.MetaRelationshipGuid);
			eventDirectory.ElementAdded.Add(classInfo, new ElementAddedEventHandler(NoteAlteredEventHandler));
			eventDirectory.ElementRemoved.Add(classInfo, new ElementRemovedEventHandler(NoteAlteredEventHandler));

			// Track Note.Text changes
			MetaAttributeInfo attributeInfo = dataDirectory.FindMetaAttribute(Note.TextMetaAttributeGuid);
			eventDirectory.ElementAttributeChanged.Add(attributeInfo, new ElementAttributeChangedEventHandler(NoteAlteredEventHandler));
		}

		/// <summary>
		/// Detaches event handlers from the store.
		/// </summary>
		protected override void DetachEventHandlers(Store store)
		{
			if (store == null || store.Disposed)
			{
				return; // Bail out
			}
			MetaDataDirectory dataDirectory = store.MetaDataDirectory;
			EventManagerDirectory eventDirectory = store.EventManagerDirectory;

			// Track Note changes
			MetaClassInfo classInfo = dataDirectory.FindMetaRelationship(RootTypeHasNote.MetaRelationshipGuid);
			eventDirectory.ElementAdded.Remove(classInfo, new ElementAddedEventHandler(NoteAlteredEventHandler));
			eventDirectory.ElementRemoved.Remove(classInfo, new ElementRemovedEventHandler(NoteAlteredEventHandler));

			// Track Note.Text changes
			MetaAttributeInfo attributeInfo = dataDirectory.FindMetaAttribute(Note.TextMetaAttributeGuid);
			eventDirectory.ElementAttributeChanged.Remove(attributeInfo, new ElementAttributeChangedEventHandler(NoteAlteredEventHandler));
		}
		/// <summary>
		/// Returns the title of the window.
		/// </summary>
		public override string WindowTitle
		{
			get
			{
				return ResourceStrings.ModelNotesWindowTitle;
			}
		}
		/// <summary>
		/// See <see cref="ToolWindow.BitmapResource"/>.
		/// </summary>
		protected override int BitmapResource
		{
			get
			{
				return 125;
			}
		}
		/// <summary>
		/// See <see cref="ToolWindow.BitmapIndex"/>.
		/// </summary>
		protected override int BitmapIndex
		{
			get
			{
				return 5;
			}
		}
		#endregion // ORMToolWindow Implementation
	}
}
