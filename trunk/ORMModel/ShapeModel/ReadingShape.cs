#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Neumont.Tools.ORM.ObjectModel;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Drawing;
using Microsoft.VisualStudio.EnterpriseTools.Shell;

#endregion

namespace Neumont.Tools.ORM.ShapeModel
{
	public partial class ReadingShape : IModelErrorActivation
	{
		#region Member Variables and Constants
		private static AutoSizeTextField myTextShapeField;
		private static readonly Regex regCountPlaces = new Regex(@"{(?<placeHolderNr>\d+)}", RegexOptions.Compiled);
		private static readonly string ellipsis = ResourceStrings.ReadingShapeEllipsis;
		private static readonly char c_ellipsis = ellipsis[0];
		private string myDisplayText;
		#endregion // Member Variables and Constants
		#region Model Event Hookup and Handlers
		#region Event Hookup
		/// <summary>
		/// Attaches event listeners for the purpose of notifying the
		/// ReadingShape to invalidate its cached data.
		/// </summary>
		public static new void AttachEventHandlers(Store store)
		{
			MetaDataDirectory dataDirectory = store.MetaDataDirectory;
			EventManagerDirectory eventDirectory = store.EventManagerDirectory;

			// Track ElementLink changes
			MetaClassInfo classInfo = dataDirectory.FindMetaRelationship(ReadingOrderHasReading.MetaRelationshipGuid);
			eventDirectory.ElementAdded.Add(classInfo, new ElementAddedEventHandler(ReadingAddedEvent));
			eventDirectory.ElementRemoved.Add(classInfo, new ElementRemovedEventHandler(ReadingRemovedEvent));

			classInfo = dataDirectory.FindMetaClass(Reading.MetaClassGuid);
			eventDirectory.ElementAttributeChanged.Add(classInfo, new ElementAttributeChangedEventHandler(ReadingAttributeChangedEvent));

			classInfo = dataDirectory.FindMetaRelationship(ReadingOrderHasRole.MetaRelationshipGuid);
			eventDirectory.ElementAdded.Add(classInfo, new ElementAddedEventHandler(RoleAddedEvent));
			eventDirectory.ElementRemoved.Add(classInfo, new ElementRemovedEventHandler(RoleRemovedEvent));

			classInfo = dataDirectory.FindMetaRelationship(FactTypeShapeHasRoleDisplayOrder.MetaRelationshipGuid);
			eventDirectory.RolePlayerOrderChanged.Add(classInfo, new RolePlayerOrderChangedEventHandler(RolePlayerOrderChangedHandler));
		}
		/// <summary>
		/// Detaches event listeners for the purpose of notifying the
		/// ReadingShape to invalidate its cached data.
		/// </summary>
		public static new void DetachEventHandlers(Store store)
		{
			if (store == null || store.Disposed)
			{
				return;
			}
			MetaDataDirectory dataDirectory = store.MetaDataDirectory;
			EventManagerDirectory eventDirectory = store.EventManagerDirectory;

			// Track ElementLink changes
			MetaClassInfo classInfo = dataDirectory.FindMetaRelationship(ReadingOrderHasReading.MetaRelationshipGuid);
			eventDirectory.ElementAdded.Remove(classInfo, new ElementAddedEventHandler(ReadingAddedEvent));
			eventDirectory.ElementRemoved.Remove(classInfo, new ElementRemovedEventHandler(ReadingRemovedEvent));

			classInfo = dataDirectory.FindMetaClass(Reading.MetaClassGuid);
			eventDirectory.ElementAttributeChanged.Remove(classInfo, new ElementAttributeChangedEventHandler(ReadingAttributeChangedEvent));

			classInfo = dataDirectory.FindMetaRelationship(ReadingOrderHasRole.MetaRelationshipGuid);
			eventDirectory.ElementAdded.Remove(classInfo, new ElementAddedEventHandler(RoleAddedEvent));
			eventDirectory.ElementRemoved.Remove(classInfo, new ElementRemovedEventHandler(RoleRemovedEvent));

			classInfo = dataDirectory.FindMetaRelationship(FactTypeShapeHasRoleDisplayOrder.MetaRelationshipGuid);
			eventDirectory.RolePlayerOrderChanged.Remove(classInfo, new RolePlayerOrderChangedEventHandler(RolePlayerOrderChangedHandler));
		}
		#endregion // Event Hookup
		#region Reading Events
		/// <summary>
		/// Handles when the roleplayer order position has changed.
		/// </summary>
		private static void RolePlayerOrderChangedHandler(object sender, RolePlayerOrderChangedEventArgs e)
		{
			FactTypeShape factShape = e.SourceElement as FactTypeShape;
			if (factShape != null)
			{
				FactType factType = factShape.ModelElement as FactType;
				if (factType != null)
				{
					ReadingOrderMoveableCollection readings = factType.ReadingOrderCollection;
					int readingCount = readings.Count;
					for (int i = 0; i < readingCount; ++i)
					{
						RefreshPresentationElements(readings[i].PresentationRolePlayers);
					}
				}
			}
		}
		/// <summary>
		/// Event handler that listens for when ReadingOrderHasReading link is being added
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		private static void ReadingAddedEvent(object sender, ElementAddedEventArgs e)
		{
			ReadingOrderHasReading link = e.ModelElement as ReadingOrderHasReading;
			ReadingOrder order = link.ReadingOrder;
			// The primary reading is the first reading in the reading order.
			// Note: this is done inline here because we already have the link,
			// which would need to be requeried in the IsPrimaryForReadingOrder property
			if (object.ReferenceEquals(order.ReadingCollection[0], link.ReadingCollection))
			{
				RefreshPresentationElements(order);
			}
		}

		/// <summary>
		/// Event handler that listens for when ReadingOrderHasReading link is being removed
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		private static void ReadingRemovedEvent(object sender, ElementRemovedEventArgs e)
		{
			ReadingOrder ord = (e.ModelElement as ReadingOrderHasReading).ReadingOrder;
			if (!ord.IsRemoved)
			{
				// There is no way to test for primary after the element is removed, so
				// always attempt a refresh on a delete
				RefreshPresentationElements(ord);
			}
		}

		/// <summary>
		/// Event handler that listens for when a Reading attribute is changed
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		private static void ReadingAttributeChangedEvent(object sender, ElementAttributeChangedEventArgs e)
		{
			Guid attributeId = e.MetaAttribute.Id;

			if (attributeId == Reading.TextMetaAttributeGuid)
			{
				Reading reading = e.ModelElement as Reading;
				if (!reading.IsRemoved &&
					reading.IsPrimaryForReadingOrder)
				{
					RefreshPresentationElements(reading.ReadingOrder);
				}
			}
		}
		#endregion // Reading Events
		#region Role Events
		/// <summary>
		/// Event handler that listens for when ReadingOrderHasRole link is being added
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		public static void RoleAddedEvent(object sender, ElementAddedEventArgs e)
		{
			ReadingOrderHasRole link = e.ModelElement as ReadingOrderHasRole;
			ReadingOrder ord = link.ReadingOrder;

			RefreshPresentationElements(ord);
		}

		/// <summary>
		/// Event handler that listens for when ReadingOrderHasRole link is being removed
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		public static void RoleRemovedEvent(object sender, ElementRemovedEventArgs e)
		{
			ReadingOrderHasRole link = e.ModelElement as ReadingOrderHasRole;
			ReadingOrder ord = link.ReadingOrder;

			if (!ord.IsRemoved)
			{
				RefreshPresentationElements(ord);
			}
		}

		/// <summary>
		/// Used to invalidate caches on presentation elements.
		/// </summary>
		/// <param name="order">The reading order being changed</param>
		private static void RefreshPresentationElements(ReadingOrder order)
		{
			// We're displaying multiple reading orders in a single
			// presentation element, so we need to look across pels
			// on all reading orders associated with this fact, not
			// just the one passed in.
			if (RefreshPresentationElements(order.PresentationRolePlayers))
			{
				return;
			}
			FactType fact = order.FactType;
			if (fact != null && !fact.IsRemoved)
			{
				ReadingOrderMoveableCollection orders = fact.ReadingOrderCollection;
				int orderCount = orders.Count;
				for (int i = 0; i < orderCount; ++i)
				{
					ReadingOrder currentOrder = orders[i];
					if (!object.ReferenceEquals(currentOrder, order))
					{
						if (RefreshPresentationElements(currentOrder.PresentationRolePlayers))
						{
							break;
						}
					}
				}
			}
		}
		/// <summary>
		/// Helper function for previous function. Return true if a ReadingShape pel was found for
		/// this reading order
		/// </summary>
		/// <param name="pels"></param>
		/// <returns>true if shape invalidated</returns>
		private static bool RefreshPresentationElements(PresentationElementMoveableCollection pels)
		{
			bool retVal = false;
			ReadingShape rs;
			int numPels = pels.Count;
			for (int i = 0; i < numPels; ++i)
			{
				rs = pels[i] as ReadingShape;
				if (rs != null)
				{
					rs.InvalidateDisplayText();
					retVal = true;
					// Don't return, allow for multiples on different diagrams. However,
					// they should all be attached to the same ReadingOrder
				}
			}
			return retVal;
		}
		#endregion // Role Events
		#endregion // Model Event Hookup and Handlers
		#region overrides
		/// <summary>
		/// Associate the reading text with this shape
		/// </summary>
		protected override Guid AssociatedShapeMetaAttributeGuid
		{
			get { return ReadingTextMetaAttributeGuid; }
		}

		/// <summary>
		/// Associate to the readints text attribute
		/// </summary>
		protected override Guid AssociatedModelMetaAttributeGuid
		{
			get { return ReadingOrder.ReadingTextMetaAttributeGuid; }
		}

		/// <summary>
		/// Store per-type value for the base class
		/// </summary>
		protected override AutoSizeTextField TextShapeField
		{
			get
			{
				return myTextShapeField;
			}
			set
			{
				Debug.Assert(myTextShapeField == null); // This should only be called once per type
				myTextShapeField = value;
			}
		}
		/// <summary>
		/// Place a newly added reading shape under the fact
		/// </summary>
		/// <param name="parent">FactTypeShape parent element</param>
		public override void PlaceAsChildOf(NodeShape parent)
		{
			FactTypeShape factShape = (FactTypeShape)parent;
			AutoResize();
			SizeD size = Size;
			double yOffset;
			if (factShape.ConstraintDisplayPosition == ConstraintDisplayPosition.Top)
			{
				yOffset = factShape.Size.Height + .5 * size.Height;
			}
			else
			{
				yOffset = -1.5 * size.Height;
			}
			Location = new PointD(0, yOffset);
		}
		/// <summary>
		/// Changed to allow resizing of the label
		/// </summary>
		public override NodeSides ResizableSides
		{
			get { return NodeSides.All; }
		}

		/// <summary>
		/// Overrides default implemenation to instantiate an Reading specific one.
		/// </summary>
		protected override AutoSizeTextField CreateAutoSizeTextField()
		{
			return new ReadingAutoSizeTextField();
		}
		#endregion
		#region Helper methods
		/// <summary>
		/// Notifies the shape that the currently cached display text may no longer
		/// be accurate, so it needs to be recreated.
		/// </summary>
		private void InvalidateDisplayText()
		{
			BeforeInvalidate();
			//this is triggering code that needs a transaction
			if (Store.TransactionManager.InTransaction)
			{
				this.AutoResize();
				InvalidateRequired(true);
			}
		}
		/// <summary>
		/// Clear the cached display text before invalidation
		/// </summary>
		protected override void BeforeInvalidate()
		{
			myDisplayText = null;
		}
		/// <summary>
		/// Check to see if FactType Derivation symbols should be appended to the display text
		/// </summary>
		private void AppendDerivation(StringBuilder stringBuilder)
		{
			FactType ft = this.ParentShape.ModelElement as FactType;

			if (ft != null)
			{
				FactTypeDerivationExpression derivation = ft.DerivationRule;
				if (derivation != null && !derivation.IsRemoved)
				{
					// UNDONE: Localize the derived fact marks. This should probably be a format expression, not just an append
					string decorator = null;
					DerivationStorageType storage = ft.DerivationStorageDisplay;
					switch (storage)
					{
						case DerivationStorageType.Derived:
							decorator = " *";
							break;
						case DerivationStorageType.DerivedAndStored:
							decorator = " **";
							break;
						case DerivationStorageType.PartiallyDerived:
							decorator = " *—";
							break;
						default:
							Debug.Fail("Unknown derivation storage type");
							break;
					}
					if (decorator != null)
					{
						stringBuilder.Append(decorator);
					}
				}
			}
		}

		/// <summary>
		/// Causes the ReadingShape on a FactTypeShape to invalidate
		/// </summary>
		public static void InvalidateReadingShape(FactType factType)
		{
			foreach (ShapeElement se in factType.PresentationRolePlayers)
			{
				FactTypeShape factShape = se as FactTypeShape;
				if (factShape != null)
				{
					foreach (ShapeElement se2 in se.RelativeChildShapes)
					{
						ReadingShape readShape = se2 as ReadingShape;
						if (readShape != null)
						{
							readShape.myDisplayText = null;
							readShape.InvalidateRequired(true);
							readShape.AutoResize();
						}
					}
				}
			}
		}
		#endregion // Helper methods
		#region properties
		/// <summary>
		/// Constructs how the reading text should be displayed.
		/// </summary>
		public String DisplayText
		{
			get
			{
				if (myDisplayText == null)
				{
					StringBuilder retval = new StringBuilder();
					ReadingOrder readingOrd = this.ModelElement as ReadingOrder;
					Debug.Assert(readingOrd != null);

					FactType factType = readingOrd.FactType;
					FactTypeShape factShape = this.ParentShape as FactTypeShape;
					if (factType == null || factType.IsRemoved)
					{
						return "";
					}
					ReadingOrderMoveableCollection readingOrderCollection = factType.ReadingOrderCollection;
					ReadingOrder primaryReadingOrder = FactTypeShape.FindMatchingReadingOrder(factShape);
					int numReadingOrders = readingOrderCollection.Count;
					for (int i = 0; i < numReadingOrders; ++i)
					{
						if (i > 0)
						{
							retval.Append(ResourceStrings.ReadingShapeReadingSeparator);
							if (numReadingOrders > 2)
							{
								retval.Append("\u000A\u000D");
							}
						}
						ReadingOrder readingOrder = readingOrderCollection[i];
						string aReading = readingOrder.ReadingText;
						RoleBaseMoveableCollection roleCollection = readingOrder.RoleCollection;
						int roleCount = roleCollection.Count;
						if (roleCount <= 2 || (numReadingOrders > 1 && i == 0))
						{
							aReading = regCountPlaces.Replace(aReading, ellipsis).Trim();
							if (i == 0 && roleCollection[0] != factShape.DisplayedRoleOrder[0])
							{
								//Terry's preffered character to append is \u25C4 which can
								//be found in the "Arial Unicode MS" font
								retval.Append(ResourceStrings.ReadingShapeInverseReading);
							}
							if (numReadingOrders <= 2 && roleCount <= 2 &&
								aReading.IndexOf(c_ellipsis) == 0 &&
								(roleCount == 1 || aReading.LastIndexOf(c_ellipsis) == aReading.Length - 1))
							{
								aReading = aReading.Replace(ellipsis, String.Empty).Trim();
							}
						}
						else
						{
							RoleBaseMoveableCollection factRoleCollection = factShape.DisplayedRoleOrder;
							//RoleMoveableCollection factRoleCollection = factType.RoleCollection;
							bool primaryOrder = object.ReferenceEquals(primaryReadingOrder, readingOrder);
							//UNDONE: the roleCount should be factRoleCollection.Count. However, this causes
							//an error when a role is added to a factType because the factType attempts to
							//update the ReadingShape before the ReadingOrders have had the role added to them.
							//Check the order of execution to see if the ReadingOrders can have the role added
							//to them before the ReadingShape is updated.
							string[] roleTranslator = new string[roleCount];
							if (primaryOrder)
							{
								for (int readRoleNum = 0; readRoleNum < roleCount; ++readRoleNum)
								{
									roleTranslator[readRoleNum] = ellipsis;
								}
							}
							else
							{
								for (int readRoleNum = 0; readRoleNum < roleCount; ++readRoleNum)
								{
									RoleBase currentRole = roleCollection[readRoleNum];
									ObjectType rolePlayer = currentRole.Role.RolePlayer;
									string formatString;
									string replacementField;
									if (rolePlayer == null)
									{
										replacementField = (factRoleCollection.IndexOf(currentRole) + 1).ToString(CultureInfo.InvariantCulture);
										formatString = ResourceStrings.ReadingShapeUnattachedRoleDisplay;
									}
									else
									{
										replacementField = rolePlayer.Name;
										formatString = ResourceStrings.ReadingShapeAttachedRoleDisplay;
									}
									roleTranslator[readRoleNum] = string.Format(CultureInfo.InvariantCulture, formatString, replacementField);
								}
							}
							aReading = string.Format(CultureInfo.InvariantCulture, aReading, roleTranslator);
						}
						retval.Append(aReading);
					}
					AppendDerivation(retval);
					myDisplayText = retval.ToString();
				}
				return myDisplayText;
			}
		}
		#endregion // properties
		#region Reading text display update rules
		// Note that the corresponding add rule for [RuleOn(typeof(FactTypeHasReadingOrder))] is in the ORMShapeModel
		// for easy sharing with the deserialization fixup process
		[RuleOn(typeof(FactTypeHasReadingOrder), FireTime = TimeToFire.TopLevelCommit, Priority = DiagramFixupConstants.AddShapeRulePriority)]
		private class ReadingOrderRemoved : RemoveRule
		{
			public override void ElementRemoved(ElementRemovedEventArgs e)
			{
				FactTypeHasReadingOrder link = e.ModelElement as FactTypeHasReadingOrder;
				FactType factType = link.FactType;
				foreach (PresentationElement pel in factType.PresentationRolePlayers)
				{
					FactTypeShape factShape = pel as FactTypeShape;
					if (factShape != null)
					{
						foreach (ShapeElement shape in factShape.RelativeChildShapes)
						{
							ReadingShape readingShape = shape as ReadingShape;
							if (readingShape != null)
							{
								readingShape.InvalidateDisplayText();
							}
						}
					}
				}
			}
		}
		#endregion // Reading text display update rules
		#region nested class ReadingAutoSizeTextField
		/// <summary>
		/// Contains code to replace RolePlayer place holders with an ellipsis.
		/// </summary>
		private class ReadingAutoSizeTextField : AutoSizeTextField
		{
			/// <summary>
			/// Initialize a ReadingAutoSizeTextField
			/// </summary>
			public ReadingAutoSizeTextField()
			{
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Near;
				DefaultStringFormat = format;
			}
			/// <summary>
			/// Code that handles the displaying of ellipsis in place of place holders and also
			/// their suppression if the are on the outside of a binary fact.
			/// </summary>
			public override string GetDisplayText(ShapeElement parentShape)
			{
				string retval = null;
				ReadingShape parentReading = parentShape as ReadingShape;

				if (parentReading == null)
				{
					retval = base.GetDisplayText(parentShape);
				}
				else
				{
					retval = parentReading.DisplayText;
				}

				return retval;
			}

			/// <summary>
			/// Changed to return true to get multiple line support.
			/// </summary>
			public override bool GetMultipleLine(ShapeElement parentShape)
			{
				return true;
			}
		}
		#endregion // nested class ReadingAutoSizeTextField
		#region change rules
		/// <summary>
		/// Changing the position of a Reading in a ReadingOrder changes the
		/// primary reading for that order, requiring a redraw
		/// </summary>
		[RuleOn(typeof(ReadingOrderHasReading), FireTime = TimeToFire.LocalCommit, Priority = DiagramFixupConstants.ResizeParentRulePriority)]
		private class ReadingPositionChanged : RolePlayerPositionChangeRule
		{
			public override void RolePlayerPositionChanged(RolePlayerOrderChangedEventArgs e)
			{
				if (e.OldOrdinal == 0 || e.NewOrdinal == 0)
				{
					ReadingOrder order = e.SourceElement as ReadingOrder;
					FactType factType = order.FactType;
					foreach (PresentationElement pel in factType.PresentationRolePlayers)
					{
						FactTypeShape factShape = pel as FactTypeShape;
						if (factShape != null)
						{
							foreach (ShapeElement shape in factShape.RelativeChildShapes)
							{
								ReadingShape readingShape = shape as ReadingShape;
								if (readingShape != null)
								{
									readingShape.InvalidateDisplayText();
								}
							}
						}
					}
				}
			}
		}
		[RuleOn(typeof(FactTypeShapeHasRoleDisplayOrder), FireTime = TimeToFire.LocalCommit, Priority = DiagramFixupConstants.ResizeParentRulePriority)]
		private class RoleDisplayOrderAdded : AddRule
		{
			public override void ElementAdded(ElementAddedEventArgs e)
			{
				FactTypeShape factShape = (e.ModelElement as FactTypeShapeHasRoleDisplayOrder).FactTypeShape;
				foreach (ShapeElement shape in factShape.RelativeChildShapes)
				{
					ReadingShape readingShape = shape as ReadingShape;
					if (readingShape != null)
					{
						readingShape.InvalidateDisplayText();
					}
				}
			}
		}
		[RuleOn(typeof(FactTypeShapeHasRoleDisplayOrder), FireTime = TimeToFire.LocalCommit, Priority = DiagramFixupConstants.ResizeParentRulePriority)]
		private class RoleDisplayOrderPositionChanged : RolePlayerPositionChangeRule
		{
			public override void  RolePlayerPositionChanged(RolePlayerOrderChangedEventArgs e)
			{

				FactTypeShape factShape = e.SourceElement as FactTypeShape;
				foreach (ShapeElement shape in factShape.RelativeChildShapes)
				{
					ReadingShape readingShape = shape as ReadingShape;
					if (readingShape != null)
					{
						readingShape.InvalidateDisplayText();
					}
				}
			}
		}

		/// <summary>
		/// Rule to notice changes to Reading.Text properties so that the
		/// reading shapes can have their display text invalidated.
		/// </summary>
		[RuleOn(typeof(Reading))]
		private class ReadingTextChanged : ChangeRule
		{
			/// <summary>
			/// Notice when Text attribute is changed and invalidate display text of the ReadingShapes
			/// </summary>
			public override void ElementAttributeChanged(ElementAttributeChangedEventArgs e)
			{
				Guid attributeId = e.MetaAttribute.Id;
				if (attributeId == Reading.TextMetaAttributeGuid)
				{
					Reading reading = e.ModelElement as Reading;
					ReadingOrder readingOrder;
					FactType factType;
					if (!reading.IsRemoved &&
						null != (readingOrder = reading.ReadingOrder) &&
						null != (factType = readingOrder.FactType))
					{
						// UNDONE: We're using this and similar foreach constructs all over this
						// file. Put some clean helper functions together and start using them.
						PresentationElementMoveableCollection pelList = factType.PresentationRolePlayers;
						int pelCount = pelList.Count;
						for (int i = 0; i < pelCount; ++i)
						{
							FactTypeShape factShape = pelList[i] as FactTypeShape;
							if (factShape != null)
							{
								ShapeElementMoveableCollection childShapes = factShape.RelativeChildShapes;
								int childPelCount = childShapes.Count;
								for (int j = 0; j < childPelCount; ++j)
								{
									ReadingShape readingShape = childShapes[j] as ReadingShape;
									if (readingShape != null)
									{
										readingShape.InvalidateDisplayText();
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion // change rules
		#region IModelErrorActivation Implementation
		/// <summary>
		/// Implements IModelErrorActivation.ActivateModelError. Forwards errors to
		/// associated fact type
		/// </summary>
		protected bool ActivateModelError(ModelError error)
		{
			IModelErrorActivation parent = ParentShape as IModelErrorActivation;
			if (parent != null)
			{
				return parent.ActivateModelError(error);
			}
			return false;
		}
		bool IModelErrorActivation.ActivateModelError(ModelError error)
		{
			return ActivateModelError(error);
		}
		#endregion // IModelErrorActivation Implementation
		#region Mouse handling
		/// <summary>
		/// Attempt model error activation
		/// </summary>
		public override void OnDoubleClick(DiagramPointEventArgs e)
		{
			ORMBaseShape.AttemptErrorActivation(e);
			base.OnDoubleClick(e);
		}
		#endregion // Mouse handling
	}
}