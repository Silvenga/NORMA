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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MSOLE = Microsoft.VisualStudio.OLE.Interop;
using Neumont.Tools.Modeling.Design;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.Modeling;
using Neumont.Tools.Modeling.Shell;

namespace Neumont.Tools.ORM.Shell
{
	#region ORMNotesToolWindow class
	/// <summary>
	/// The ToolWindow which is responsible for displaying and allowing
	/// the update of notes on elements implementing <see cref="INoteOwner{Note}"/>.
	/// </summary>
	[Guid("A7C9E14E-9EEE-4D79-A7F4-9E9D1A567498")]
	[CLSCompliant(false)]
	public class ORMNotesToolWindow : ORMBaseNoteToolWindow<Note>
	{
		#region Constructor
		/// <summary>
		/// Returns a new <see cref="ORMNotesToolWindow"/>
		/// </summary>
		public ORMNotesToolWindow(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}
		#endregion // Constructor
		#region Base overrides
		private static readonly Guid[] myNoteRoleIdentifiers = { ObjectTypeHasNote.NoteDomainRoleId, FactTypeHasNote.NoteDomainRoleId, ModelHasModelNote.NoteDomainRoleId };
		/// <summary>
		/// Return role identifiers for <see cref="ObjectType"/>, <see cref="FactType"/> and <see cref="ModelNote"/> <see cref="Note"/>s
		/// </summary>
		protected override Guid[] GetNoteRoleIdentifiers()
		{
			return myNoteRoleIdentifiers;
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
				return PackageResources.Id.ToolWindowIcons;
			}
		}
		/// <summary>
		/// See <see cref="ToolWindow.BitmapIndex"/>.
		/// </summary>
		protected override int BitmapIndex
		{
			get
			{
				return PackageResources.ToolWindowIconIndex.NotesEditor;
			}
		}
		#endregion // Base overrides
	}
	#endregion // ORMNotesToolWindow class
	#region ORMDefinitionToolWindow class
	/// <summary>
	/// The ToolWindow which is responsible for displaying and allowing
	/// the update of definitions on elements implementing <see cref="INoteOwner{Definition}"/>.
	/// </summary>
	[Guid("FC6D8343-48D1-4294-915F-01B6350E0E12")]
	[CLSCompliant(false)]
	public class ORMDefinitionToolWindow : ORMBaseNoteToolWindow<Definition>
	{
		#region Constructor
		/// <summary>
		/// Returns a new <see cref="ORMDefinitionToolWindow"/>
		/// </summary>
		public ORMDefinitionToolWindow(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}
		#endregion // Constructor
		#region Base overrides
		private static readonly Guid[] myNoteRoleIdentifiers = { ObjectTypeHasDefinition.DefinitionDomainRoleId, FactTypeHasDefinition.DefinitionDomainRoleId };
		/// <summary>
		/// Return role identifiers for <see cref="ObjectType"/> and <see cref="FactType"/> <see cref="Definition"/>s
		/// </summary>
		protected override Guid[] GetNoteRoleIdentifiers()
		{
			return myNoteRoleIdentifiers;
		}
		/// <summary>
		/// Returns the title of the window.
		/// </summary>
		public override string WindowTitle
		{
			get
			{
				return ResourceStrings.ModelDefinitionWindowTitle;
			}
		}
		/// <summary>
		/// See <see cref="ToolWindow.BitmapResource"/>.
		/// </summary>
		protected override int BitmapResource
		{
			get
			{
				return PackageResources.Id.ToolWindowIcons;
			}
		}
		/// <summary>
		/// See <see cref="ToolWindow.BitmapIndex"/>.
		/// </summary>
		protected override int BitmapIndex
		{
			get
			{
				return PackageResources.ToolWindowIconIndex.DefinitionEditor;
			}
		}
		#endregion // Base overrides
	}
	#endregion // ORMDefinitionToolWindow class
	#region ORMBaseNoteToolWindow class
	/// <summary>
	/// A base class for any ToolWindow that is used to display a note type field. Notes
	/// are assumed to be multiline string fields.
	/// </summary>
	/// <typeparam name="NoteType">The type of a DomainClass with a domain property called Text.</typeparam>
	[CLSCompliant(false)]
	public abstract class ORMBaseNoteToolWindow<NoteType> : ORMToolWindow, MSOLE.IOleCommandTarget where NoteType : ModelElement
	{
		#region Private data members
		private TextBox myTextBox;
		private List<INoteOwner<NoteType>> mySelectedNoteOwners;
		#endregion // Private data members
		#region Construction
		/// <summary>
		/// Returns a new <see cref="ORMBaseNoteToolWindow{NoteType}"/>
		/// </summary>
		protected ORMBaseNoteToolWindow(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
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
		/// Handles note change.
		/// </summary>
		private void NoteAlteredEventHandler(object sender, ElementPropertyChangedEventArgs e)
		{
			ModelElement element = e.ModelElement;
			Store store = element.Store;
			if (e.DomainProperty.Id == GetNoteTextPropertyId(store))
			{
				// First, see if the note element implements INoteOwner directly
				INoteOwner<NoteType> noteOwner = element as INoteOwner<NoteType>;
				if (noteOwner != null)
				{
					NoteAlteredEventHandler(noteOwner);
				}
				else
				{
					// If note, find the owning element
					Guid[] ownerRoles = GetNoteRoleIdentifiers();
					for (int i = 0; i < ownerRoles.Length; ++i)
					{
						noteOwner = DomainRoleInfo.GetLinkedElement(element, ownerRoles[i]) as INoteOwner<NoteType>;
						if (noteOwner != null)
						{
							NoteAlteredEventHandler(noteOwner);
							break;
						}
					}
				}
			}
		}
		/// <summary>
		/// Helper function to handler a note change event
		/// </summary>
		private void NoteAlteredEventHandler(INoteOwner<NoteType> noteOwner)
		{
			List<INoteOwner<NoteType>> currentOwners;
			if (null != noteOwner &&
				null != (currentOwners = mySelectedNoteOwners) &&
				currentOwners.Count != 0 &&
				currentOwners.Contains(noteOwner))
			{
				DisplayNotes();
			}
		}
		/// <summary>
		/// Event handler for the note textbox.  Forces the note to be saved
		/// if the textbox is enabled.
		/// </summary>
		void myTextBox_LostFocus(object sender, EventArgs e)
		{
			List<INoteOwner<NoteType>> owners = mySelectedNoteOwners;
			if (owners != null && owners.Count == 1 && myTextBox.Enabled)	// If we only have one selected note and the textbox is enabled,
			{
				SetNote(owners[0], myTextBox.Text);	// try to set the note.
			}
		}
		#endregion // Event handlers
		#region Helper functions
		/// <summary>
		/// Sets the NoteText property on an INoteOwner instance.
		/// </summary>
		/// <param name="owner">The INoteOwner on which the note should be set.</param>
		/// <param name="text">The text of the note.</param>
		private void SetNote(INoteOwner<NoteType> owner, string text)
		{
			PropertyDescriptor descriptor;
			ORMDesignerDocData currentDoc;
			IORMToolServices currentStore;
			if (null != owner &&
				null != (currentDoc = CurrentDocument) &&
				null != (currentStore = currentDoc.Store as IORMToolServices) &&
				currentStore.CanAddTransaction &&
				null != (descriptor = owner.NoteTextPropertyDescriptor))	// Be really defensive.
			{
				if (0 != string.CompareOrdinal((string)descriptor.GetValue(owner), text))
				{
					descriptor.SetValue(owner, text);
				}
			}
		}
		/// <summary>
		/// Populates mySelectedNoteOwners with all currently selected note
		/// owners in the passed in ORMDesignerDocView.
		/// </summary>
		private void PopulateSelectedNoteOwners()
		{
			List<INoteOwner<NoteType>> selectedTypes = mySelectedNoteOwners;
			if (selectedTypes == null)
			{
				selectedTypes = new List<INoteOwner<NoteType>>();
				mySelectedNoteOwners = selectedTypes;
			}
			selectedTypes.Clear();	// Clear the list of selected root types.
			IORMSelectionContainer selectionContainer = CurrentORMSelectionContainer;
			if (selectionContainer != null)
			{

				ICollection objects = base.GetSelectedComponents();
				if (objects != null)
				{
					foreach (object o in objects)
					{
						INoteOwner<NoteType> owner = EditorUtility.ResolveContextInstance(o, false) as INoteOwner<NoteType>;	// and if they are an INoteOwner,
						if (owner != null)
						{
							selectedTypes.Add(owner);	// add them to the list of selected owners.
						}
					}
				}
			}
			DisplayNotes();	// Display the notes for all selected owners.
		}
		/// <summary>
		/// Displays the notes for all selected note owners in the Notes Window.
		/// </summary>
		private void DisplayNotes()
		{
			List<INoteOwner<NoteType>> selectedNoteOwners = mySelectedNoteOwners; // Cache the list of selected note owners.
			int selectedNoteOwnersCount = selectedNoteOwners.Count; // Cache the count of selected note owners.
			TextBox textBox = myTextBox; // Cache the text box.
			if (textBox != null) // If the text box is not null,
			{
				textBox.Clear(); // clear it's text
				textBox.ReadOnly = false; // and set it to read only.
				if (selectedNoteOwnersCount == 0) // If there aren't any selected note owners,
				{
					return; // bail out.
				}
				else if (selectedNoteOwnersCount == 1) // If there is only one selected note owner,
				{
					textBox.AppendText(selectedNoteOwners[0].NoteText); // add its note text to the text box.
				}
				else
				{
					textBox.ReadOnly = true; // Otherwise, the textbox should be read-only because multiple
					StringBuilder sb = new StringBuilder();
					string formatString = ResourceStrings.ModelNotesWindowRootTypeNameNotesSeparatorFormatString;
					// root types are selected.
					bool first = true;
					for (int i = 0; i < selectedNoteOwnersCount; ++i) // Loop through the selected root types,
					{
						INoteOwner<NoteType> noteOwner = selectedNoteOwners[i]; // cache them locally,
						string noteText = noteOwner.NoteText; // and cache their note text.
						if (!string.IsNullOrEmpty(noteText)) // If there is note text on the note owner,
						{
							if (!first)
							{
								sb.AppendLine();
							}
							else
							{
								first = false;
							}
							sb.AppendFormat(CultureInfo.InvariantCulture, formatString, noteOwner.Name, noteText);
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
				ModelingDocData docData = CurrentDocument;
				Microsoft.VisualStudio.Modeling.Shell.UndoManager undoManager;
				MSOLE.IOleCommandTarget forwardTo;
				if ((docData != null &&
					null != (undoManager = docData.UndoManager) &&
					null != (forwardTo = undoManager.VSUndoManager as MSOLE.IOleCommandTarget)) ||
					null != (forwardTo = GetService(typeof(MSOLE.IOleCommandTarget)) as MSOLE.IOleCommandTarget))
				{
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
				ModelingDocData docData = CurrentDocument;
				Microsoft.VisualStudio.Modeling.Shell.UndoManager undoManager;
				MSOLE.IOleCommandTarget forwardTo;
				if ((docData != null &&
					null != (undoManager = docData.UndoManager) &&
					null != (forwardTo = undoManager.VSUndoManager as MSOLE.IOleCommandTarget)) ||
					null != (forwardTo = GetService(typeof(MSOLE.IOleCommandTarget)) as MSOLE.IOleCommandTarget))
				{
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
		/// Populates the tool window when the selection changes
		/// </summary>
		protected override void OnORMSelectionContainerChanged()
		{
			PopulateSelectedNoteOwners();
		}
		/// <summary>
		/// Clear a covered window when the document changes and when the selection changes.
		/// </summary>
		protected override CoveredFrameContentActions CoveredFrameContentActions
		{
			get
			{
				return CoveredFrameContentActions.ClearContentsOnSelectionChanged | CoveredFrameContentActions.ClearContentsOnDocumentChanged;
			}
		}
		/// <summary>
		/// Return the role ids of the note end of all relationship that
		/// own this kind of note. This must return the same set of identifiers
		/// regardless of store instance.
		/// </summary>
		protected abstract Guid[] GetNoteRoleIdentifiers();
		private static EventHandler<ElementEventArgs>[] myOwningRelationshipHandlers = null;
		/// <summary>
		/// We cannot use anonymous delegates in ManageEventHandlers to respond to add/remove
		/// of relationships because the instance would change each time, making it impossible
		/// to remove the event handler from the watch list on a store. So, we build these
		/// once here. Note that this assumes that GetNoteRoleIdentifiers() returns a fixed
		/// set of identifiers and cannot change from store to store.
		/// </summary>
		private EventHandler<ElementEventArgs>[] GetOwningRelationshipHandlers(Store store)
		{
			EventHandler<ElementEventArgs>[] handlers = myOwningRelationshipHandlers;
			if (handlers == null)
			{
				Guid[] noteRoleIdentifiers = GetNoteRoleIdentifiers();
				handlers = new EventHandler<ElementEventArgs>[noteRoleIdentifiers.Length];
				for (int i = 0; i < noteRoleIdentifiers.Length; ++i)
				{
					DomainDataDirectory dataDirectory = store.DomainDataDirectory;
					DomainRoleInfo role = dataDirectory.FindDomainRole(noteRoleIdentifiers[i]);
					Guid oppositeRoleId = role.OppositeDomainRole.Id;
					handlers[i] = delegate(object sender, ElementEventArgs e)
					{
						NoteAlteredEventHandler(DomainRoleInfo.GetRolePlayer((ElementLink)e.ModelElement, oppositeRoleId) as INoteOwner<NoteType>);
					};
				}
				System.Threading.Interlocked.CompareExchange<EventHandler<ElementEventArgs>[]>(ref myOwningRelationshipHandlers, handlers, null);
				handlers = myOwningRelationshipHandlers;
			}
			return handlers;
		}
		private static Guid[] myNoteTextPropertyId; // Defined as an array so we can statically initialize without locking
		private Guid GetNoteTextPropertyId(Store store)
		{
			Guid[] propertyId = myNoteTextPropertyId;
			if (propertyId == null)
			{
				DomainDataDirectory dataDirectory = store.DomainDataDirectory;
				propertyId = new Guid[] { store.DomainDataDirectory.GetDomainClass(typeof(NoteType)).FindDomainProperty("Text", false).Id };
				System.Threading.Interlocked.CompareExchange<Guid[]>(ref myNoteTextPropertyId, propertyId, null);
				propertyId = myNoteTextPropertyId;
			}
			return propertyId[0];
		}
		/// <summary>
		/// Manages <see cref="EventHandler{TEventArgs}"/>s in the <see cref="Store"/> so that the <see cref="ORMNotesToolWindow"/>
		/// contents can be updated to reflect any model changes.
		/// </summary>
		/// <param name="store">The <see cref="Store"/> for which the <see cref="EventHandler{TEventArgs}"/>s should be managed.</param>
		/// <param name="eventManager">The <see cref="ModelingEventManager"/> used to manage the <see cref="EventHandler{TEventArgs}"/>s.</param>
		/// <param name="action">The <see cref="EventHandlerAction"/> that should be taken for the <see cref="EventHandler{TEventArgs}"/>s.</param>
		protected override void ManageEventHandlers(Store store, ModelingEventManager eventManager, EventHandlerAction action)
		{
			if (store == null || store.Disposed)
			{
				return; // Bail out
			}
			DomainDataDirectory dataDirectory = store.DomainDataDirectory;

			// Track Note additions and deletions changes
			Guid[] noteRoleIdentifiers = GetNoteRoleIdentifiers();
			EventHandler<ElementEventArgs>[] handlers = GetOwningRelationshipHandlers(store);
			for (int i = 0; i < noteRoleIdentifiers.Length; ++i)
			{
				EventHandler<ElementEventArgs> handler = handlers[i];
				DomainClassInfo classInfo = dataDirectory.FindDomainRole(noteRoleIdentifiers[i]).DomainRelationship;
				eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(handler), action);
				eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(handler), action);
			}

			// Track Note.Text changes
			eventManager.AddOrRemoveHandler(dataDirectory.FindDomainProperty(GetNoteTextPropertyId(store)), new EventHandler<ElementPropertyChangedEventArgs>(NoteAlteredEventHandler), action);
		}
		#endregion // ORMToolWindow Implementation
	}
	#endregion // ORMBaseNoteToolWindow class
}
