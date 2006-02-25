﻿#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright © Neumont University. All rights reserved.                     *
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

#region Using directives
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.VirtualTreeGrid; 
#endregion

namespace Neumont.Tools.ORM.ObjectModel
{
	/// <summary>
	/// Represents the Reference Mode Header Branch
	/// </summary>
	public class ReferenceModeHeaderBranch : IBranch 
	{
		private ReferenceModeKindsBranch myReferenceModeKindsBranch;
		private CustomReferenceModesBranch myCustomBranch;
		private IntrinsicReferenceModesBranch myIntrinsicBranch;

		private enum Headers
		{
			ReferenceModeKinds = 0,
			CustomReferenceModes = 1,
			IntrinsicReferenceModes = 2
		}

		private static string myIntrinsicReferenceModesHeader = ResourceStrings.ModelReferenceModeEditorIntrinsicReferenceModesHeader;
		private static string myCustomReferenceModesHeader = ResourceStrings.ModelReferenceModeEditorCustomReferenceModesHeader;
		private static string myReferenceModeKindHeader = ResourceStrings.ModelReferenceModeEditorReferenceModeKindHeader;

		private string[] myHeaderNames = { myReferenceModeKindHeader, myCustomReferenceModesHeader, myIntrinsicReferenceModesHeader }; 

		/// <summary>
		/// Default constructor
		/// </summary>
		public ReferenceModeHeaderBranch()
		{
			this.myReferenceModeKindsBranch = new ReferenceModeKindsBranch();
			myCustomBranch = new CustomReferenceModesBranch();
			myIntrinsicBranch = new IntrinsicReferenceModesBranch();			
		}

		/// <summary>
		/// Sets the reference modes for the class
		/// </summary>
		/// <param name="model"></param>
		public void SetModel(ORMModel model)
		{
			myCustomBranch.SetModel(model);
			myIntrinsicBranch.SetModel(model);
			myReferenceModeKindsBranch.SetModel(model);
		}
		#region IBranch Members
		/// <summary>
		/// Implements IBranch.BeginLabelEdit
		/// </summary>
		protected static VirtualTreeLabelEditData BeginLabelEdit(int row, int column, VirtualTreeLabelEditActivationStyles activationStyle)
		{
			return VirtualTreeLabelEditData.Invalid;
		}
		VirtualTreeLabelEditData IBranch.BeginLabelEdit(int row, int column, VirtualTreeLabelEditActivationStyles activationStyle)
		{
			return BeginLabelEdit(row, column, activationStyle);
		}
		/// <summary>
		/// Implements IBranch.CommitLabelEdit
		/// </summary>
		protected static LabelEditResult CommitLabelEdit(int row, int column, string newText)
		{
			return LabelEditResult.CancelEdit;
		}
		LabelEditResult IBranch.CommitLabelEdit(int row, int column, string newText)
		{
			return CommitLabelEdit(row, column, newText);
		}
		/// <summary>
		/// Implements IBranch.Features
		/// </summary>
		protected static BranchFeatures Features
		{
			get { return BranchFeatures.Expansions | BranchFeatures.Realigns; }
		}
		BranchFeatures IBranch.Features
		{
			get { return Features; }
		}
		/// <summary>
		/// Implements IBranch.GetAccssibilityData
		/// </summary>
		protected static VirtualTreeAccessibilityData GetAccessibilityData(int row, int column)
		{
			return VirtualTreeAccessibilityData.Empty;
		}
		VirtualTreeAccessibilityData IBranch.GetAccessibilityData(int row, int column)
		{
			return GetAccessibilityData(row, column);
		}
		/// <summary>
		/// Implements IBranch.GetDisplayData
		/// </summary>
		protected static VirtualTreeDisplayData GetDisplayData(int row, int column, VirtualTreeDisplayDataMasks requiredData)
		{
			VirtualTreeDisplayData retVal = new VirtualTreeDisplayData();
			retVal.BackColor = SystemColors.ControlLight;
			return retVal;
		}
		VirtualTreeDisplayData IBranch.GetDisplayData(int row, int column, VirtualTreeDisplayDataMasks requiredData)
		{
			return GetDisplayData(row, column, requiredData);
		}
		/// <summary>
		/// Implements IBranch.GetObject
		/// </summary>
		protected object GetObject(int row, int column, ObjectStyle style, ref int options)
		{
			if (style == ObjectStyle.ExpandedBranch)
			{
				switch ((Headers)row)
				{
					case Headers.ReferenceModeKinds:
						return this.myReferenceModeKindsBranch;

					case Headers.CustomReferenceModes:
						return this.myCustomBranch;

					case Headers.IntrinsicReferenceModes:
						return myIntrinsicBranch;
				}
			}
			return null;
		}
		object IBranch.GetObject(int row, int column, ObjectStyle style, ref int options)
		{
			return GetObject(row, column, style, ref options);
		}
		/// <summary>
		/// Implements IBranch.GetText
		/// </summary>
		protected string GetText(int row, int column)
		{
			return myHeaderNames[row];
		}
		string IBranch.GetText(int row, int column)
		{
			return GetText(row, column);
		}
		/// <summary>
		/// Implements IBranch.GetTipText
		/// </summary>
		protected static string GetTipText(int row, int column, ToolTipType tipType)
		{
			return null;
		}
		string IBranch.GetTipText(int row, int column, ToolTipType tipType)
		{
			return GetTipText(row, column, tipType);
		}
		/// <summary>
		/// Implements IBranch.IsExpandable
		/// </summary>
		protected static bool IsExpandable(int row, int column)
		{
			return true;
		}
		bool IBranch.IsExpandable(int row, int column)
		{
			return IsExpandable(row, column);
		}
		/// <summary>
		/// Implements IBranch.LocateObject
		/// </summary>
		protected static LocateObjectData LocateObject(object obj, ObjectStyle style, int locateOptions)
		{
			switch (style)
			{
				case ObjectStyle.ExpandedBranch:
					if (obj is ReferenceModeKindsBranch)
					{
						return new LocateObjectData(0, 0, (int)BranchLocationAction.KeepBranch);
					}
					return new LocateObjectData(0, 0, (int)BranchLocationAction.DiscardBranch);
				default:
					Debug.Assert(false); // Shouldn't be here
					return new LocateObjectData();
			}
		}
		LocateObjectData IBranch.LocateObject(object obj, ObjectStyle style, int locateOptions)
		{
			return LocateObject(obj, style, locateOptions);
		}
		event BranchModificationEventHandler IBranch.OnBranchModification
		{
			add { }
			remove { }
		}
		/// <summary>
		/// Implements IBranch.OnDragEvent
		/// </summary>
		protected static void OnDragEvent(object sender, int row, int column, DragEventType eventType, DragEventArgs args)
		{
		}
		void IBranch.OnDragEvent(object sender, int row, int column, DragEventType eventType, DragEventArgs args)
		{
			OnDragEvent(sender, row, column, eventType, args);
		}
		/// <summary>
		/// Implements IBranch.OnGiveFeedback
		/// </summary>
		protected static void OnGiveFeedback(GiveFeedbackEventArgs args, int row, int column)
		{
		}
		void IBranch.OnGiveFeedback(GiveFeedbackEventArgs args, int row, int column)
		{
			OnGiveFeedback(args, row, column);
		}
		/// <summary>
		/// Implements IBranch.OnQueryContinueDrag
		/// </summary>
		protected static void OnQueryContinueDrag(QueryContinueDragEventArgs args, int row, int column)
		{
		}
		void IBranch.OnQueryContinueDrag(QueryContinueDragEventArgs args, int row, int column)
		{
			OnQueryContinueDrag(args, row, column);
		}
		/// <summary>
		/// Implements IBranch.OnStartDrag
		/// </summary>
		protected static VirtualTreeStartDragData OnStartDrag(object sender, int row, int column, DragReason reason)
		{
			return VirtualTreeStartDragData.Empty;
		}
		VirtualTreeStartDragData IBranch.OnStartDrag(object sender, int row, int column, DragReason reason)
		{
			return OnStartDrag(sender, row, column, reason);
		}
		/// <summary>
		/// Implements IBranch.ToggleState
		/// </summary>
		protected static StateRefreshChanges ToggleState(int row, int column)
		{
			return StateRefreshChanges.None;
		}
		StateRefreshChanges IBranch.ToggleState(int row, int column)
		{
			return ToggleState(row, column);
		}
		/// <summary>
		/// Implements IBranch.SynchronizeState
		/// </summary>
		protected static StateRefreshChanges SynchronizeState(int row, int column, IBranch matchBranch, int matchRow, int matchColumn)
		{
			return StateRefreshChanges.None;
		}
		StateRefreshChanges IBranch.SynchronizeState(int row, int column, IBranch matchBranch, int matchRow, int matchColumn)
		{
			return SynchronizeState(row, column, matchBranch, matchRow, matchColumn);
		}
		/// <summary>
		/// Implements IBranch.UpdateCounter
		/// </summary>
		protected static int UpdateCounter
		{
			get
			{
				return 0;
			}
		}
		int IBranch.UpdateCounter
		{
			get
			{
				return UpdateCounter;
			}
		}
		/// <summary>
		/// Implements IBranch.VisibleItemCount
		/// </summary>
		protected int VisibleItemCount
		{
			get { return myHeaderNames.Length; }
		}
		int IBranch.VisibleItemCount
		{
			get { return VisibleItemCount; }
		}
		#endregion
	}
}