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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VirtualTreeGrid;
using Neumont.Tools.Modeling.Shell;

#if VISUALSTUDIO_9_0
using VirtualTreeInPlaceControlFlags = Microsoft.VisualStudio.VirtualTreeGrid.VirtualTreeInPlaceControls;
#endif //VISUALSTUDIO_9_0

namespace Neumont.Tools.Modeling.Design
{
	/// <summary>
	/// A base class used to display a list of elements in a
	/// VirtualTreeGrid control. Override the GetTree method to
	/// populate the constrol, and alternately the LastControlSize property.
	/// </summary>
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public abstract class TreePicker<T> : SizePreservingEditor<T>
		where T : TreePicker<T>
	{
		#region DropDownTreeControl class. Handles Escape key for ListBox
		private sealed class DropDownTreeControl : StandardVirtualTreeControl
		{
			private bool myEscapePressed;
			private int myLastSelectedRow = -1;
			private int myLastSelectedColumn = -1;
			public event DoubleClickEventHandler AfterDoubleClick;
			protected sealed override bool IsInputKey(Keys keyData)
			{
				if ((keyData & Keys.KeyCode) == Keys.Escape)
				{
					myEscapePressed = true;
				}
				return base.IsInputKey(keyData);
			}
			protected sealed override CreateParams CreateParams
			{
				[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
				get
				{
					CreateParams @params = base.CreateParams;
					@params.ExStyle &= ~0x200; // Turn off Fixed3D border style
					return @params;
				}
			}
			protected sealed override void OnDoubleClick(DoubleClickEventArgs e)
			{
				base.OnDoubleClick(e);
				if (AfterDoubleClick != null)
				{
					AfterDoubleClick(this, e);
				}
			}
			public bool EscapePressed
			{
				get
				{
					return myEscapePressed;
				}
			}
			public int LastSelectedRow
			{
				get
				{
					return myLastSelectedRow;
				}
			}
			public int LastSelectedColumn
			{
				get
				{
					return myLastSelectedColumn;
				}
			}
			protected sealed override void OnSelectionChanged(EventArgs e)
			{
				myLastSelectedColumn = CurrentColumn;
				myLastSelectedRow = CurrentIndex;
				base.OnSelectionChanged(e);
			}
		}
		#endregion // DropDownTreeControl class. Handles Escape key for VirtualTreeControl
		#region UITypeEditor overrides
		private object myInitialSelectionValue;
		/// <summary>
		/// Required UITypeEditor override. Opens dropdown modally
		/// and waits for user input.
		/// </summary>
		/// <param name="context">The descriptor context. Used to retrieve
		/// the live instance and other data.</param>
		/// <param name="provider">The service provider for the given context.</param>
		/// <param name="value">The current property value</param>
		/// <returns>The updated property value, or the orignal value to effect a cancel</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editor = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (editor != null)
			{
				object newObject = value;
				// Get the tree contents
				ITree tree = GetTree(context, value);
				// Proceed if there is anything to show
				// Don't check tree.VisibleItemCount. Allows the derived class to display an empty dropdown
				// by returning a tree with no visible elements.
				if (tree != null)
				{
					// Create a listbox with its events
					using (DropDownTreeControl treeControl = new DropDownTreeControl())
					{
						if (UseStandardCheckBoxes)
						{
							ImageList images = new ImageList();
							images.ImageSize = new Size(16, 16);
							treeControl.ImageList = images;
							treeControl.StandardCheckBoxes = true;
						}
						treeControl.BindingContextChanged += new EventHandler(HandleBindingContextChanged);
						treeControl.AfterDoubleClick += delegate(object sender, DoubleClickEventArgs e)
						{
							if (e.Button == MouseButtons.Left)
							{
								editor.CloseDropDown();
							}
						};

						// Manage the size of the control
						Size lastSize = LastControlSize;
						if (!lastSize.IsEmpty)
						{
							treeControl.Size = lastSize;
						}
						myInitialSelectionValue = value;

						// Show the dropdown. This is modal.
						IMultiColumnTree multiTree = tree as IMultiColumnTree;
						if (multiTree != null)
						{
							treeControl.MultiColumnTree = multiTree;
						}
						else
						{
#if VISUALSTUDIO_9_0 // MSBUG: Hack workaround crashing bug in VirtualTreeControl.OnToggleExpansion
							treeControl.ColumnPermutation = new ColumnPermutation(1, new int[]{0}, false);
#endif
							treeControl.Tree = tree;
						}

						// Make sure keystrokes are forwarded while the modal dropdown is open
						IVirtualTreeInPlaceControl virtualTreeInPlaceControl = editor as IVirtualTreeInPlaceControl;
						VirtualTreeInPlaceControlFlags flags = virtualTreeInPlaceControl != null ? virtualTreeInPlaceControl.Flags : 0;
						if (0 != (flags & VirtualTreeInPlaceControlFlags.ForwardKeyEvents))
						{
							virtualTreeInPlaceControl.Flags = flags & ~VirtualTreeInPlaceControlFlags.ForwardKeyEvents;
						}

						editor.DropDownControl(treeControl);

						// Restore keystroke forwarding
						if (0 != (flags & VirtualTreeInPlaceControlFlags.ForwardKeyEvents))
						{
							virtualTreeInPlaceControl.Flags = flags;
						}

						// Record the final size, we'll use it next time for this type of control
						LastControlSize = treeControl.Size;

						// Make sure the user didn't cancel, and give derived classes a chance
						// to translate the value displayed in the tree to an appropriately
						// typed value for the associated property.
						if (!treeControl.EscapePressed)
						{
							int lastRow = treeControl.LastSelectedRow;
							int lastColumn = treeControl.LastSelectedColumn;
							if (lastRow != -1 || AlwaysTranslateToValue)
							{
								newObject = TranslateToValue(context, value, tree, lastRow, lastColumn);
							}
						}
					}
				}
				return newObject;
			}
			return value;
		}
		#endregion // UITypeEditor overrides
		#region TreePicker Specifics
		private void HandleBindingContextChanged(object sender, EventArgs e)
		{
			Control listBox = (Control)sender;
			if (myInitialSelectionValue != null)
			{
				listBox.BindingContextChanged -= new EventHandler(HandleBindingContextChanged);
				object value = myInitialSelectionValue;
				myInitialSelectionValue = null;
				SelectInitialValue(value, (VirtualTreeControl)sender);
			}
		}

		/// <summary>
		/// Generate the tree to display in the tree control. If the
		/// return tree also implements IMultiColumnTree then it will be
		/// shown as a multi-column tree.
		/// </summary>
		/// <param name="context">ITypeDescriptorContext passed in by the system</param>
		/// <param name="value">The current value</param>
		/// <returns>A list. An empty list will</returns>
		protected abstract ITree GetTree(ITypeDescriptorContext context, object value);
		/// <summary>
		/// Translate the current state of the tree to a new value for the property
		/// </summary>
		/// <param name="context">ITypeDescriptorContext passed in by the system</param>
		/// <param name="oldValue">The starting value</param>
		/// <param name="tree">The tree returned by GetTree</param>
		/// <param name="selectedRow">The last selected row in the tree</param>
		/// <param name="selectedColumn">The last selected column in the tree</param>
		/// <returns>Default implementation returns oldValue</returns>
		protected virtual object TranslateToValue(ITypeDescriptorContext context, object oldValue, ITree tree, int selectedRow, int selectedColumn)
		{
			return oldValue;
		}
		/// <summary>
		/// By default, TranslateToValue is not called if no item is
		/// selected in the tree. However, if the value of the tree
		/// is based on checkbox state and not selection, then it is
		/// possible to change the value displayed in the tree without
		/// selecting an item. If TranslateToValue is not dependent
		/// on the currently selected item, or if 'no selected item'
		/// has a semantic meaning, then return true from this property
		/// to force TranslateToValue to be called for an empty
		/// selection state. You can combine override this property
		/// and the <see cref="SelectInitialValue"/> method to
		/// have no initial selection in the tree.
		/// </summary>
		protected virtual bool AlwaysTranslateToValue
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Select a value in the provided tree control. If overriden,
		/// this method should expand the control.Tree to the appropriate
		/// state and set the CurrentColumn and CurrentIndex properties on the control.
		/// The default behavior defers to VirtualTreeControl.SelectObject.
		/// Failing to initialize the selection can result in modifications
		/// made to the check state of a control to be ignored. See <see cref="AlwaysTranslateToValue"/>
		/// for additional information.
		/// </summary>
		/// <param name="value">The initial value</param>
		/// <param name="control">The dropped down tree control</param>
		protected virtual void SelectInitialValue(object value, VirtualTreeControl control)
		{
			IBranch branch = control.Tree.Root;
			if (0 != (branch.Features & BranchFeatures.PositionTracking))
			{
				control.SelectObject(branch, value, (int)ObjectStyle.TrackingObject, 0);
			}
		}
		/// <summary>
		/// Should standard checkboxes be enabled on the parent tree control. Defaults to true.
		/// </summary>
		protected virtual bool UseStandardCheckBoxes
		{
			get
			{
				return true;
			}
		}
		#endregion // TreePicker Specifics
	}
}
