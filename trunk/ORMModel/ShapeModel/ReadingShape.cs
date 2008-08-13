﻿#region Common Public License Copyright Notice
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
using Microsoft.VisualStudio.Modeling.Shell;
using Neumont.Tools.Modeling;
using Neumont.Tools.ORM.Shell;
using System.Drawing.Drawing2D;

#endregion

namespace Neumont.Tools.ORM.ShapeModel
{
	public partial class ReadingShape : IModelErrorActivation, ISelectionContainerFilter
	{
		#region Size Constants
		/// <summary>
		/// The height of reading indicator with the arrow pointing up
		/// </summary>
		private const double ReadingIndicatorArrowHeight = .05d;
		/// <summary>
		/// The width of reading indicator arrow with the arrow pointing up
		/// </summary>
		private const double ReadingIndicatorArrowWidth = .055d;
		/// <summary>
		/// Returns the size of a reading indicator arrow associated with a vertically <see cref="FactTypeShape"/>
		/// </summary>
		private static readonly SizeD ReadingIndicatorArrowVerticalSize = new SizeD(ReadingIndicatorArrowWidth, ReadingIndicatorArrowHeight);
		/// <summary>
		/// Returns the size of a reading indicator arrow associated with a horizontal <see cref="FactTypeShape"/>
		/// </summary>
		private static readonly SizeD ReadingIndicatorArrowHorizontalSize = new SizeD(ReadingIndicatorArrowHeight, ReadingIndicatorArrowWidth);
		#endregion // Size Constants
		#region Member Variables
		private static AutoSizeTextField myTextShapeField;
		private static DirectionIndicatorField myLeftDirectionIndicator;
		private static DirectionIndicatorField myRightDirectionIndicator;
		private static readonly string ellipsis = ResourceStrings.ReadingShapeEllipsis;
		private static readonly char c_ellipsis = ellipsis[0];
		private string myDisplayText;
		#endregion // Member Variables
		#region Model Event Hookup and Handlers
		#region Event Hookup
		/// <summary>
		/// Manages <see cref="EventHandler{TEventArgs}"/>s in the <see cref="Store"/> for the purpose of notifying the
		/// <see cref="ReadingShape"/>s to invalidate their cached data.
		/// </summary>
		/// <param name="store">The <see cref="Store"/> for which the <see cref="EventHandler{TEventArgs}"/>s should be managed.</param>
		/// <param name="eventManager">The <see cref="ModelingEventManager"/> used to manage the <see cref="EventHandler{TEventArgs}"/>s.</param>
		/// <param name="action">The <see cref="EventHandlerAction"/> that should be taken for the <see cref="EventHandler{TEventArgs}"/>s.</param>
		public static new void ManageEventHandlers(Store store, ModelingEventManager eventManager, EventHandlerAction action)
		{
			DomainDataDirectory dataDirectory = store.DomainDataDirectory;

			// Track ElementLink changes
			DomainClassInfo classInfo = dataDirectory.FindDomainRelationship(ReadingOrderHasReading.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(ReadingAddedEvent), action);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(ReadingRemovedEvent), action);

			classInfo = dataDirectory.FindDomainClass(Reading.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementPropertyChangedEventArgs>(ReadingAttributeChangedEvent), action);

			classInfo = dataDirectory.FindDomainRelationship(ReadingOrderHasRole.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(RoleAddedEvent), action);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(RoleRemovedEvent), action);

			classInfo = dataDirectory.FindDomainRelationship(FactTypeShapeHasRoleDisplayOrder.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<RolePlayerOrderChangedEventArgs>(RolePlayerOrderChangedHandler), action);
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
					LinkedElementCollection<ReadingOrder> readings = factType.ReadingOrderCollection;
					int readingCount = readings.Count;
					for (int i = 0; i < readingCount; ++i)
					{
						RefreshPresentationElements(PresentationViewsSubject.GetPresentation(readings[i]));
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
			if (!link.IsDeleted && order.ReadingCollection[0] == link.Reading)
			{
				RefreshPresentationElements(order);
			}
		}

		/// <summary>
		/// Event handler that listens for when ReadingOrderHasReading link is being removed
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		private static void ReadingRemovedEvent(object sender, ElementDeletedEventArgs e)
		{
			ReadingOrder ord = (e.ModelElement as ReadingOrderHasReading).ReadingOrder;
			if (!ord.IsDeleted)
			{
				// There is no way to test for primary after the element is removed, so
				// always attempt a refresh on a delete
				RefreshPresentationElements(ord);
			}
		}

		/// <summary>
		/// Event handler that listens for when a Reading property is changed
		/// and then tells associated model elements to invalidate their cache
		/// </summary>
		private static void ReadingAttributeChangedEvent(object sender, ElementPropertyChangedEventArgs e)
		{
			Guid attributeId = e.DomainProperty.Id;

			if (attributeId == Reading.TextDomainPropertyId)
			{
				Reading reading = e.ModelElement as Reading;
				if (!reading.IsDeleted &&
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
		public static void RoleRemovedEvent(object sender, ElementDeletedEventArgs e)
		{
			ReadingOrderHasRole link = e.ModelElement as ReadingOrderHasRole;
			ReadingOrder ord = link.ReadingOrder;

			if (!ord.IsDeleted)
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
			if (RefreshPresentationElements(PresentationViewsSubject.GetPresentation(order)))
			{
				return;
			}
			FactType fact = order.FactType;
			if (fact != null && !fact.IsDeleted)
			{
				LinkedElementCollection<ReadingOrder> orders = fact.ReadingOrderCollection;
				int orderCount = orders.Count;
				for (int i = 0; i < orderCount; ++i)
				{
					ReadingOrder currentOrder = orders[i];
					if (currentOrder != order)
					{
						if (RefreshPresentationElements(PresentationViewsSubject.GetPresentation(currentOrder)))
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
		private static bool RefreshPresentationElements(LinkedElementCollection<PresentationElement> pels)
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
		#region Base Overrides
		/// <summary>
		/// Associate to the reading's text property
		/// </summary>
		protected override Guid AssociatedModelDomainPropertyId
		{
			get { return ReadingOrder.ReadingTextDomainPropertyId; }
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
		/// <param name="createdDuringViewFixup">Whether this shape was created as part of a view fixup</param>
		public override void PlaceAsChildOf(NodeShape parent, bool createdDuringViewFixup)
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
		/// Overrides default implemenation to instantiate an Reading specific one.
		/// </summary>
		/// <param name="fieldName">Non-localized name for the field</param>
		protected override AutoSizeTextField CreateAutoSizeTextField(string fieldName)
		{
			return new ReadingAutoSizeTextField(fieldName);
		}
		/// <summary>
		/// Create a new reading shape for a remaining reading order if this reading order
		/// is deleted.
		/// </summary>
		protected override void OnDeleting()
		{
			Store store = Store;
			if (!store.InUndoRedoOrRollback)
			{
				FactTypeShape parentShape;
				FactType factType;
				ReadingOrder oldOrder;
				if (null != (oldOrder = ModelElement as ReadingOrder) &&
					oldOrder.IsDeleting &&
					null != (parentShape = ParentShape as FactTypeShape) &&
					!parentShape.IsDeleting &&
					null != (factType = parentShape.ModelElement as FactType))
				{
					LinkedElementCollection<ReadingOrder> remainingOrders = factType.ReadingOrderCollection;
					if (remainingOrders.Count != 0)
					{
						LinkedElementCollection<RoleBase> roles = factType.RoleCollection;
						Reading newReading = factType.GetMatchingReading(remainingOrders, oldOrder, roles[0], null, false, false, false, roles, true) as Reading;
						if (newReading != null)
						{
							ReadingOrder newOrder = newReading.ReadingOrder;
							if (newOrder != null)
							{
								ReadingShape newShape = new ReadingShape(Partition, new PropertyAssignment[] { new PropertyAssignment(NodeShape.AbsoluteBoundsDomainPropertyId, AbsoluteBounds) });
								newShape.Associate(newOrder);
								parentShape.RelativeChildShapes.Add(newShape);
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Add reading indicator shape fields
		/// </summary>
		protected override void InitializeShapeFields(IList<ShapeField> shapeFields)
		{
			base.InitializeShapeFields(shapeFields);
			// UNDONE: Localize field names, used only for accessibility display
			DirectionIndicatorField leftField = new DirectionIndicatorField("LeftDirectionIndicator", true);
			DirectionIndicatorField rightField = new DirectionIndicatorField("RightDirectionIndicator", false);
			shapeFields.Add(leftField);
			shapeFields.Add(rightField);
			AnchoringBehavior behavior = leftField.AnchoringBehavior;
			behavior.SetRightAnchor(TextShapeField, AnchoringBehavior.Edge.Left, 0d);
			behavior.CenterVertically();
			behavior = rightField.AnchoringBehavior;
			behavior.SetLeftAnchor(TextShapeField, AnchoringBehavior.Edge.Right, 0d);
			behavior.CenterVertically();
			Debug.Assert(myLeftDirectionIndicator == null && myRightDirectionIndicator == null);
			myLeftDirectionIndicator = leftField;
			myRightDirectionIndicator = rightField;
		}
		/// <summary>
		/// Set the content size of the ReadingShape
		/// </summary>
		protected override SizeD ContentSize
		{
			get
			{
				SizeD retVal = SizeD.Empty;
				ShapeField textField = TextShapeField;
				if (textField != null)
				{
					retVal = textField.GetMinimumSize(this);
					retVal = new SizeD(retVal.Width + myLeftDirectionIndicator.GetMinimumSize(this).Width + myRightDirectionIndicator.GetMinimumSize(this).Width, retVal.Height);
				}
				return retVal;
			}
		}
		/// <summary>
		/// Get the accessible name for a shape field
		/// </summary>
		public override string GetFieldAccessibleName(ShapeField field)
		{
			return field.Name;
		}
		#endregion // Base Overrides
		#region Helper methods
		/// <summary>
		/// Notifies the shape that the currently cached display text may no longer
		/// be accurate, so it needs to be recreated.
		/// </summary>
		public void InvalidateDisplayText()
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
		private static string GetDerivationDecorator(FactType factType)
		{
			string retVal = null;
			if (factType != null)
			{
				FactTypeDerivationExpression derivation = factType.DerivationRule;
				if (derivation != null && !derivation.IsDeleted)
				{
					// UNDONE: Localize the derived fact marks. This should probably be a format expression, not just an append
					DerivationStorageType storage = factType.DerivationStorageDisplay;
					switch (factType.DerivationStorageDisplay)
					{
						case DerivationStorageType.Derived:
							retVal = " *";
							break;
						case DerivationStorageType.DerivedAndStored:
							retVal = " **";
							break;
						case DerivationStorageType.PartiallyDerived:
							retVal = " *—";
							break;
						default:
							Debug.Fail("Unknown derivation storage type");
							break;
					}
				}
			}
			return retVal;
		}

		/// <summary>
		/// Causes the ReadingShape on a FactTypeShape to invalidate
		/// </summary>
		public static void InvalidateReadingShape(FactType factType)
		{
			foreach (ShapeElement se in PresentationViewsSubject.GetPresentation(factType))
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
					FactTypeShape factShape;
					FactType factType;
					LinkedElementCollection<ReadingOrder> readingOrders;
					int readingOrderCount;
					if (null == (factShape = ParentShape as FactTypeShape) ||
						null == (factType = factShape.AssociatedFactType) ||
						0 == (readingOrderCount = (readingOrders = factType.ReadingOrderCollection).Count))
					{
						return "";
					}
					string derivationDecorator = GetDerivationDecorator(factType);
					string retVal = null;
					bool doNamedReplacement = false;
					ReadingOrder defaultOrder = readingOrders[0];
					LinkedElementCollection<RoleBase> orderedRoles = defaultOrder.RoleCollection;
					int roleCount = orderedRoles.Count;
					string readingFormatString = null;
					if (readingOrderCount == 1)
					{
						readingFormatString = defaultOrder.ReadingText;
						if (roleCount > 2 &&
							defaultOrder != factShape.FindMatchingReadingOrder(false) &&
							defaultOrder != factShape.FindMatchingReadingOrder(true))
						{
							doNamedReplacement = true;
						}
						else if (factType.UnaryRole != null)
						{
							// Do not ellipsize unary reading role placeholders when the role is at the beginning
							retVal = Reading.ReplaceFields(
								readingFormatString,
								delegate(int index, Match match)
								{
									return (match.Index == 0) ? string.Empty : ellipsis;
								}).Trim();
						}
						else
						{
							retVal = EllipsizeReadingFormatString(readingFormatString, roleCount);
						}
					}
					else if (roleCount > 2)
					{
						ReadingOrder matchingOrder;
						if (null != (matchingOrder = factShape.FindMatchingReadingOrder(false)) ||
							null != (matchingOrder = factShape.FindMatchingReadingOrder(true)))
						{
							retVal = Reading.ReplaceFields(matchingOrder.ReadingText, ellipsis).Trim();
						}
						else
						{
							// CONSIDER: Do we want to attempt a better match, such as a leading role
							// match if we don't get the full match.
							doNamedReplacement = true;
						}
					}
					else
					{
						// UNDONE: Unary binarization is likely to hit this assert
						Debug.Assert(roleCount == 2, "A unary fact should not have more than one reading order.");
						ReadingOrder firstOrder;
						ReadingOrder secondOrder;
						if (defaultOrder == factShape.FindMatchingReadingOrder())
						{
							firstOrder = defaultOrder;
							secondOrder = readingOrders[1];
						}
						else
						{
							firstOrder = readingOrders[1];
							secondOrder = defaultOrder;
						}
						retVal = string.Concat(
							EllipsizeReadingFormatString(firstOrder.ReadingText, roleCount),
							ResourceStrings.ReadingShapeReadingSeparator,
							EllipsizeReadingFormatString(secondOrder.ReadingText, roleCount),
							derivationDecorator);
						derivationDecorator = null;
					}
					if (doNamedReplacement)
					{
						LinkedElementCollection<RoleBase> factRoles = factShape.DisplayedRoleOrder;
						string[] roleTranslator = new string[roleCount];
						for (int readRoleNum = 0; readRoleNum < roleCount; ++readRoleNum)
						{
							RoleBase currentRole = orderedRoles[readRoleNum];
							ObjectType rolePlayer = currentRole.Role.RolePlayer;
							string formatString;
							string replacementField;
							if (rolePlayer == null)
							{
								replacementField = (factRoles.IndexOf(currentRole) + 1).ToString(CultureInfo.InvariantCulture);
								formatString = ResourceStrings.ReadingShapeUnattachedRoleDisplay;
							}
							else
							{
								replacementField = rolePlayer.Name;
								formatString = ResourceStrings.ReadingShapeAttachedRoleDisplay;
							}
							roleTranslator[readRoleNum] = string.Format(CultureInfo.InvariantCulture, formatString, replacementField);
						}
						retVal = string.Format(CultureInfo.InvariantCulture, (readingFormatString == null) ? defaultOrder.ReadingText : readingFormatString, roleTranslator);
					}
					if (derivationDecorator != null)
					{
						retVal += derivationDecorator;
					}
					myDisplayText = retVal;
				}
				return myDisplayText;
			}
		}
		/// <summary>
		/// Replace replacement fields with ellipsis and trim leading/trailing ellipsis
		/// as appropriate for unary and binary format strings
		/// </summary>
		private static string EllipsizeReadingFormatString(string formatString, int roleCount)
		{
			string retVal = Reading.ReplaceFields(formatString, ellipsis).Trim();
			int retValLength = retVal.Length;
			if (retValLength != 0)
			{
				switch (roleCount)
				{
					case 1:
						if (retVal[0] == c_ellipsis)
						{
							retVal = retVal.Substring(1).TrimStart();
						}
						break;
					case 2:
						if (retValLength > 1 &&
							retVal[0] == c_ellipsis &&
							retVal[retVal.Length - 1] == c_ellipsis)
						{
							retVal = retVal.Substring(1, retValLength - 2).Trim();
						}
						break;
				}
			}
			return retVal;
		}
		#endregion // properties
		#region Reading text display update rules
		// Note that the corresponding add rule for [RuleOn(typeof(FactTypeHasReadingOrderRuleClass))] is in ORMShapeDomainModel
		// for easy sharing with the deserialization fixup process
		/// <summary>
		/// DeleteRule: typeof(Neumont.Tools.ORM.ObjectModel.FactTypeHasReadingOrder), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AddShapeRulePriority;
		/// </summary>
		private static void ReadingOrderDeletedRule(ElementDeletedEventArgs e)
		{
			FactTypeHasReadingOrder link = e.ModelElement as FactTypeHasReadingOrder;
			FactType factType = link.FactType;
			foreach (PresentationElement pel in PresentationViewsSubject.GetPresentation(factType))
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
		#endregion // Reading text display update rules
		#region nested class ReadingAutoSizeTextField
		/// <summary>
		/// Contains code to replace RolePlayer place holders with an ellipsis.
		/// </summary>
		private sealed class ReadingAutoSizeTextField : AutoSizeTextField
		{
			/// <summary>
			/// Initialize a ReadingAutoSizeTextField
			/// </summary>
			/// <param name="fieldName">Non-localized name for the field</param>
			public ReadingAutoSizeTextField(string fieldName)
				: base(fieldName)
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
			/// Redirect all editing to the reading editor until we get the inplace editing with locked
			/// replacement fields live inline in the document.
			/// </summary>
			public override bool CanEditValue(ShapeElement parentShape, DiagramClientView view)
			{
				return true;
			}
			/// <summary>
			/// Redirect all editing to the reading editor until we get the inplace editing with locked
			/// replacement fields live inline in the document.
			/// </summary>
			public override void EditValue(ShapeElement parentShape, DiagramClientView view)
			{
				Neumont.Tools.ORM.Shell.ORMDesignerPackage.ReadingEditorWindow.Show();
			}
			/// <summary>
			/// Redirect all editing to the reading editor until we get the inplace editing with locked
			/// replacement fields live inline in the document.
			/// </summary>
			public override void EditValue(ShapeElement parentShape, DiagramClientView view, PointD mousePosition)
			{
				Neumont.Tools.ORM.Shell.ORMDesignerPackage.ReadingEditorWindow.Show();
			}
			/// <summary>
			/// Redirect all editing to the reading editor until we get the inplace editing with locked
			/// replacement fields live inline in the document.
			/// </summary>
			public override void OnKeyPress(DiagramKeyPressEventArgs e)
			{
				// UNDONE: This is a flagrant reflector ripoff of Microsoft.VisualStudio.Modeling.Diagrams.TextField.OnKeyPress
				// and Microsoft.VisualStudio.Modeling.Diagrams.ShapeField.OnKeyPress required because
				// a third EditValue function is not virtual.
				if (this.IsNavigationKey(e.KeyChar))
				{
					e.Handled = true;
				}
				else if (((e.KeyChar != '\r')) && (((char.IsLetterOrDigit(e.KeyChar) || char.IsNumber(e.KeyChar)) || (char.IsPunctuation(e.KeyChar) || char.IsSeparator(e.KeyChar))) || (char.IsSymbol(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))))
				{
					e.Handled = true;
					// UNDONE: Consider moving forwarding the key press to the reading editor
					// Probably not worth the trouble given that this will all go away when
					// we get in-place activation working.
					System.Media.SystemSounds.Beep.Play();
				}
				else if (e.TargetItem == null)
				{
					e.Handled = false;
				}
			}
		}
		#endregion // nested class ReadingAutoSizeTextField
		#region DirectionIndicatorField class
		/// <summary>
		/// Creates a shape to properly align the other shapefields within the FactTypeShape.
		/// </summary>
		private sealed class DirectionIndicatorField : ShapeField
		{
			/// <summary>
			/// Determine the required direction for the arrow to display
			/// </summary>
			private enum ArrowDirection
			{
				None,
				Up,
				Down,
				Left,
				Right,
			}
			private readonly bool myIsLeft;
			/// <summary>
			/// A shape field to display reading direction indicators depending
			/// on the direction of the current reading.
			/// </summary>
			/// <param name="fieldName">Non-localized name for the field</param>
			/// <param name="isLeft">Reading direction indicator is displayed on the left side of the reading</param>
			public DirectionIndicatorField(string fieldName, bool isLeft)
				: base(fieldName)
			{
				DefaultFocusable = false;
				DefaultSelectable = false;
				DefaultVisibility = true;
				myIsLeft = isLeft;
			}

			/// <summary>
			/// Returns <see cref="ReadingIndicatorArrowHorizontalSize"/> if <see cref="FactTypeShape.DisplayOrientation"/> is
			/// <see cref="DisplayOrientation.Horizontal"/>. Otherwise, returns <see cref="ReadingIndicatorArrowVerticalSize"/>
			/// </summary>
			public sealed override SizeD GetMinimumSize(ShapeElement parentShape)
			{
				switch (GetDirection(parentShape))
				{
					case ArrowDirection.Up:
					case ArrowDirection.Down:
						return ReadingIndicatorArrowVerticalSize;
					case ArrowDirection.Left:
					case ArrowDirection.Right:
						return ReadingIndicatorArrowHorizontalSize;
				}
				return SizeD.Empty;
			}
			private ArrowDirection GetDirection(ShapeElement parentShape)
			{
				ArrowDirection retVal = ArrowDirection.None;
				FactTypeShape factTypeShape = (FactTypeShape)parentShape.ParentShape;
				FactType factType = factTypeShape.AssociatedFactType;
				LinkedElementCollection<RoleBase> roles = factType.RoleCollection;
				int roleCount = roles.Count;
				if (roleCount > 1)
				{
					DisplayOrientation orientation = factTypeShape.DisplayOrientation;
					bool isVertical = orientation != DisplayOrientation.Horizontal;
					// Vertical indicators are displayed on the right
					bool isRight = !myIsLeft;
					if (!isVertical || isRight)
					{
						bool continueArrowCheck = false;
						bool isReverseReading = false;
						if (roleCount == 2)
						{
							LinkedElementCollection<ReadingOrder> orders = factType.ReadingOrderCollection;
							if (orders.Count == 1)
							{
								roles = orders[0].RoleCollection;
								isReverseReading = (orientation == DisplayOrientation.VerticalRotatedLeft) ? roles[0] == factTypeShape.DisplayedRoleOrder[0] : roles[0] != factTypeShape.DisplayedRoleOrder[0];
								continueArrowCheck = true;
							}
						}
						else
						{
							// Display direction indicators if either the forward or reverse orders are
							// an exact match. Other orders are displayed with named replacement fields
							bool haveForwardReading = null != factTypeShape.FindMatchingReadingOrder(false);
							isReverseReading = !haveForwardReading && null != factTypeShape.FindMatchingReadingOrder(true);
							continueArrowCheck = haveForwardReading | isReverseReading;
						}
						if (continueArrowCheck)
						{
							bool showForward = false;
							if (!isReverseReading)
							{
								ReadingDirectionIndicatorDisplay displayOption = OptionsPage.CurrentReadingDirectionIndicatorDisplay;
								switch (displayOption)
								{
									//case ReadingDirectionIndicatorDisplay.Reversed:
									//case ReadingDirectionIndicatorDisplay.Separated:
									case ReadingDirectionIndicatorDisplay.Rotated:
										showForward = isVertical;
										break;
									case ReadingDirectionIndicatorDisplay.Always:
										showForward = isVertical || isRight;
										break;
								}
							}
							if (isReverseReading || showForward)
							{
								if (isVertical)
								{
									retVal = isReverseReading ? ArrowDirection.Up : ArrowDirection.Down;
								}
								else
								{
									retVal = isReverseReading ? (isRight ? ArrowDirection.None : ArrowDirection.Left) : ArrowDirection.Right;
								}
							}
						}
					}
				}
				return retVal;
			}
			/// <summary>
			/// Paint the direction arrow in the provided rectangle
			/// </summary>
			public override void DoPaint(DiagramPaintEventArgs e, ShapeElement parentShape)
			{
				RectangleD bounds = this.GetBounds(parentShape);
				PointD startPoint;
				PointD midPoint;
				PointD endPoint;
				switch (GetDirection(parentShape))
				{
					case ArrowDirection.Up:
						startPoint = new PointD(bounds.Left, bounds.Bottom);
						midPoint = new PointD(bounds.Left + bounds.Width / 2, bounds.Top);
						endPoint = new PointD(bounds.Right, bounds.Bottom);
						break;
					case ArrowDirection.Down:
						startPoint = new PointD(bounds.Left, bounds.Top);
						midPoint = new PointD(bounds.Left + bounds.Width / 2, bounds.Bottom);
						endPoint = new PointD(bounds.Right, bounds.Top);
						break;
					case ArrowDirection.Left:
						startPoint = new PointD(bounds.Right, bounds.Top);
						midPoint = new PointD(bounds.Left, bounds.Top + bounds.Height / 2);
						endPoint = new PointD(bounds.Right, bounds.Bottom);
						break;
					case ArrowDirection.Right:
						startPoint = new PointD(bounds.Left, bounds.Top);
						midPoint = new PointD(bounds.Right, bounds.Top + bounds.Height / 2);
						endPoint = new PointD(bounds.Left, bounds.Bottom);
						break;
					default:
						return;
				}
				using (GraphicsPath path = new GraphicsPath())
				{
					path.AddPolygon(new PointF[] {
							PointD.ToPointF(startPoint),
							PointD.ToPointF(midPoint),
							PointD.ToPointF(endPoint)});
					e.Graphics.FillPath(SystemBrushes.WindowText, path);
				}
			}
		}
		#endregion // DirectionIndicatorField class
		#region change rules
		/// <summary>
		/// RolePlayerPositionChangeRule: typeof(Neumont.Tools.ORM.ObjectModel.ReadingOrderHasReading), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// Changing the position of a Reading in a ReadingOrder changes the
		/// primary reading for that order, requiring a redraw
		/// </summary>
		private static void ReadingPositionChangedRule(RolePlayerOrderChangedEventArgs e)
		{
			if (e.OldOrdinal == 0 || e.NewOrdinal == 0)
			{
				ReadingOrder order = e.SourceElement as ReadingOrder;
				FactType factType = order.FactType;
				foreach (PresentationElement pel in PresentationViewsSubject.GetPresentation(factType))
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
		/// <summary>
		/// AddRule: typeof(Neumont.Tools.ORM.ObjectModel.ReadingOrderHasReading), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// Adding a reading at the 0 position of the ReadingOrder
		/// changes the primary reading for that order
		/// </summary>
		private static void ReadingAddedRule(ElementAddedEventArgs e)
		{
			ReadingOrderHasReading link = e.ModelElement as ReadingOrderHasReading;
			if (!link.IsDeleted)
			{
				ReadingOrder order = link.ReadingOrder;
				FactType factType;
				if (order.ReadingCollection[0] == link.Reading &&
					null != (factType = order.FactType))
				{
					foreach (PresentationElement pel in PresentationViewsSubject.GetPresentation(factType))
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
		/// <summary>
		/// AddRule: typeof(FactTypeShapeHasRoleDisplayOrder), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// </summary>
		private static void RoleDisplayOrderAddedRule(ElementAddedEventArgs e)
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
		/// <summary>
		/// RolePlayerPositionChangeRule: typeof(FactTypeShapeHasRoleDisplayOrder), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// </summary>
		private static void RoleDisplayOrderPositionChangedRule(RolePlayerOrderChangedEventArgs e)
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
		/// <summary>
		/// ChangeRule: typeof(FactTypeShape), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// </summary>
		private static void DisplayOrientationChangedRule(ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id == FactTypeShape.DisplayOrientationDomainPropertyId)
			{
				FactTypeShape factShape = e.ModelElement as FactTypeShape;
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
		/// ChangeRule: typeof(Neumont.Tools.ORM.ObjectModel.Reading), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AddShapeRulePriority;
		/// Rule to notice changes to Reading.Text properties so that the
		/// reading shapes can have their display text invalidated.
		/// </summary>
		private static void ReadingTextChangedRule(ElementPropertyChangedEventArgs e)
		{
			Guid attributeId = e.DomainProperty.Id;
			if (attributeId == Reading.TextDomainPropertyId)
			{
				Reading reading = e.ModelElement as Reading;
				ReadingOrder readingOrder;
				FactType factType;
				if (!reading.IsDeleted &&
					null != (readingOrder = reading.ReadingOrder) &&
					null != (factType = readingOrder.FactType))
				{
					// UNDONE: We're using this and similar foreach constructs all over this
					// file. Put some clean helper functions together and start using them.
					LinkedElementCollection<PresentationElement> pelList = PresentationViewsSubject.GetPresentation(factType);
					int pelCount = pelList.Count;
					for (int i = 0; i < pelCount; ++i)
					{
						FactTypeShape factShape = pelList[i] as FactTypeShape;
						if (factShape != null)
						{
							LinkedElementCollection<ShapeElement> childShapes = factShape.RelativeChildShapes;
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
		#region ISelectionContainerFilter Implementation
		/// <summary>
		/// Implements ISelectionContainerFilter.IncludeInSelectionContainer
		/// </summary>
		protected static bool IncludeInSelectionContainer
		{
			get
			{
				return false;
			}
		}
		bool ISelectionContainerFilter.IncludeInSelectionContainer
		{
			get
			{
				return IncludeInSelectionContainer;
			}
		}
		#endregion // ISelectionContainerFilter Implementation
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
