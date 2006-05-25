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

// Turn this on to block hiding of shapes implied by an objectification pattern
//#define SHOW_IMPLIED_SHAPES
// Turn this on to show a fact shape instead of a subtype link for all SubtypeFacts
//#define SHOW_FACTSHAPE_FOR_SUBTYPE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Diagrams.GraphObject;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.ORM.Shell;
using Microsoft.VisualStudio.EnterpriseTools.Shell;
namespace Neumont.Tools.ORM.ShapeModel
{
	#region IStickyObject interface
	/// <summary>
	/// Interface for implementing "Sticky" selections.  Presentation elements that are sticky
	/// will maintain their selected status when compatible objects are clicked.
	/// </summary>
	[CLSCompliant(true)]
	public interface IStickyObject
	{
		/// <summary>
		/// Call this on an object when you're setting it as a StickyObject.  This method
		/// will go through the object's associated elements to perform any actions needed
		/// such as calling Invalidate().
		/// </summary>
		void StickyInitialize();
		/// <summary>
		/// Returns whether the Presentation Element that was passed in is selectable in the
		/// context of this StickyObject.  For example, when an external constraint is the
		/// active StickyObject, roles are selectable and objects are not.
		/// </summary>
		/// <returns>Whether the PresentationElement passed in is selectable in this StickyObject's context</returns>
		bool StickySelectable(ModelElement mel);
		/// <summary>
		/// Needed to allow outside entities to tell the StickyObject to redraw itself and its children.
		/// </summary>
		void StickyRedraw();
	}
	#endregion // IStickyObject interface
	public partial class ORMDiagram : IProxyDisplayProvider
	{
		# region DragDrop overrides
		/// <summary>
		/// Check to see if <see cref="DiagramDragEventArgs.Data">dragged object</see> is a type that can be dropped on the <see cref="Diagram"/>,
		/// if so change <see cref="DiagramDragEventArgs.Effect"/>.
		/// </summary>
		public override void OnDragOver(DiagramDragEventArgs e)
		{
			string[] dataFormats = e.Data.GetFormats();
			if (Array.IndexOf(dataFormats, typeof(ObjectType).FullName) >= 0 ||
				Array.IndexOf(dataFormats, typeof(FactType).FullName) >= 0 ||
				Array.IndexOf(dataFormats, typeof(SetComparisonConstraint).FullName) >= 0 ||
				Array.IndexOf(dataFormats, typeof(SetConstraint).FullName) >= 0)
			{
				e.Effect = DragDropEffects.All;
				e.Handled = true;
			}
			base.OnDragOver(e);
		}
		/// <summary>
		/// check to see if dragged object is a type that can be dropped on the diagram, if so allow it to be added to form by calling FixUpDiagram
		/// </summary>
		/// <param name="e"></param>
		public override void OnDragDrop(DiagramDragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			if (dataObject == null)
			{
				return;
			}
			ObjectType objectType = null;
			FactType factType = null;
			SetComparisonConstraint multiCol = null;
			SetConstraint singleCol = null;
			ModelElement element = null;
			bool[] factsContained;
			int factsRemaining;
			if (null != (objectType = dataObject.GetData(typeof(ObjectType)) as ObjectType))
			{
				element = objectType;
			}
			else if (null != (factType = dataObject.GetData(typeof(FactType)) as FactType))
			{
				element = factType;
			}
			else if (null != (multiCol = dataObject.GetData(typeof(SetComparisonConstraint)) as SetComparisonConstraint))
			{
				FactTypeMoveableCollection factTypeList = multiCol.FactTypeCollection;
				factsContained = new bool[factTypeList.Count];
				factsRemaining = factTypeList.Count;
				FactType fact;
				foreach (ShapeElement shape in NestedChildShapes)
				{
					if (null != (fact = shape.ModelElement as FactType))
					{
						int index = factTypeList.IndexOf(fact);
						if (index != -1)
						{
							if (!factsContained[index])
							{
								factsContained[index] = true;
								--factsRemaining;
								if (factsRemaining == 0)
								{
									element = multiCol;
									break;
								}
							}
						}
					}
				}
			}
			else if (null != (singleCol = dataObject.GetData(typeof(SetConstraint)) as SetConstraint))
			{
				FactTypeMoveableCollection factTypeList = singleCol.FactTypeCollection;
				factsContained = new bool[factTypeList.Count];
				factsRemaining = factTypeList.Count;
				FactType fact;
				foreach (ShapeElement shape in NestedChildShapes)
				{
					if (null != (fact = shape.ModelElement as FactType))
					{
						int index = factTypeList.IndexOf(fact);
						if (index != -1)
						{
							if (!factsContained[index])
							{
								factsContained[index] = true;
								--factsRemaining;
								if (factsRemaining == 0)
								{
									element = singleCol;
									break;
								}
							}
						}
					}
				}
			}
			if (element != null)
			{
				e.Effect = DragDropEffects.All;
				e.Handled = true;
				using (Transaction transaction = this.TransactionManager.BeginTransaction(ResourceStrings.DropShapeTransactionName))
				{
					ModelElement droppedOnElement = this.ModelElement;
					DropTargetContext.Set(transaction, Id, e.MousePosition, null);
					Diagram.FixUpDiagram(droppedOnElement, element);
					if (factType != null)
					{
						foreach (RoleBase roleBase in factType.RoleCollection)
						{
							Role role = roleBase.Role;
							FixupRelatedLinks(droppedOnElement, role.GetElementLinks(ObjectTypePlaysRole.PlayedRoleCollectionMetaRoleGuid));
							FixupRelatedLinks(droppedOnElement, role.GetElementLinks(FactSetConstraint.FactTypeCollectionMetaRoleGuid));
							FixupRelatedLinks(droppedOnElement, role.GetElementLinks(FactSetComparisonConstraint.FactTypeCollectionMetaRoleGuid));
						}
					}
					else if (objectType != null)
					{
						IList rolePlayerLinks = objectType.GetElementLinks(ObjectTypePlaysRole.RolePlayerMetaRoleGuid);
						int linksCount = rolePlayerLinks.Count;
						for (int i = 0; i < linksCount; ++i)
						{
							ObjectTypePlaysRole link = rolePlayerLinks[i] as ObjectTypePlaysRole;
							Role role = link.PlayedRoleCollection;
							SubtypeMetaRole subRole;
							SupertypeMetaRole superRole;
							FactType subtypeFact = null;
							if (null != (subRole = role as SubtypeMetaRole))
							{
								subtypeFact = role.FactType;
							}
							else if (null != (superRole = role as SupertypeMetaRole))
							{
								subtypeFact = role.FactType;
							}
							if (subtypeFact != null)
							{
								FixUpDiagram(droppedOnElement, subtypeFact);
							}
							else
							{
								FixUpDiagram(droppedOnElement, link);
							}
						}
					}
					else if (singleCol != null)
					{
						FixupRelatedLinks(droppedOnElement, singleCol.GetElementLinks(FactSetConstraint.SetConstraintCollectionMetaRoleGuid));
					}
					else if (multiCol != null)
					{
						FixupRelatedLinks(droppedOnElement, multiCol.GetElementLinks(FactSetComparisonConstraint.SetComparisonConstraintCollectionMetaRoleGuid));
					}
					transaction.Commit();
				}
			}
			base.OnDragDrop(e);
		}
		private static void FixupRelatedLinks(ModelElement droppedOnElement, IList links)
		{
			int linksCount = links.Count;
			for (int i = 0; i < linksCount; ++i)
			{
				FixUpDiagram(droppedOnElement, (ModelElement)links[i]);
			}
		}
		# endregion // DragDrop overrides
		#region Hack to block child shape moving twice
		/// <summary>
		/// Key used to add a DiagramItemCollection to the current top-level
		/// transaction's ContextInfo when elements are being moved on the diagram.
		/// </summary>
		public static readonly string MovingDiagramItemsContextKey = Guid.NewGuid().ToString("B", CultureInfo.InvariantCulture);
		/// <summary>
		/// Hack override to handle MSBUG that moves child shapes twice on the diagram. Combined
		/// with ORMBaseShape.CanMove to limit moving of child shapes during a transaction.
		/// </summary>
		public override void MoveByRepositioning(ElementGroupPrototype elementGroupPrototype, DiagramItemCollection topLevelItems, PointD moveDelta)
		{
			Store store = Store;
			IDictionary contextInfo = null;
			if (store.TransactionActive)
			{
				contextInfo = store.TransactionManager.CurrentTransaction.TopLevelTransaction.Context.ContextInfo;
				contextInfo[MovingDiagramItemsContextKey] = topLevelItems;
			}
			try
			{
				base.MoveByRepositioning(elementGroupPrototype, topLevelItems, moveDelta);
			}
			finally
			{
				if (contextInfo != null)
				{
					contextInfo.Remove(MovingDiagramItemsContextKey);
				}
			}
		}
		#endregion // Hack to block child shape moving twice
		#region Toolbox filter strings
		/// <summary>
		/// The filter string used for simple actions
		/// </summary>
		public const string ORMDiagramDefaultFilterString = "ORMDiagramDefaultFilterString";
		/// <summary>
		/// The filter string used to create an external constraint. Very similar to a
		/// normal action, except the external constraint connector is activated on completion
		/// of the action.
		/// </summary>
		public const string ORMDiagramExternalConstraintFilterString = "ORMDiagramExternalConstraintFilterString";
		/// <summary>
		/// The filter string used to connect role sequences to external constraints
		/// </summary>
		public const string ORMDiagramConnectExternalConstraintFilterString = "ORMDiagramConnectExternalConstraintFilterString";
		/// <summary>
		/// The filter string used to create subtype relationships between object types
		/// </summary>
		public const string ORMDiagramCreateSubtypeFilterString = "ORMDiagramCreateSubtypeFilterString";
		/// <summary>
		/// The filter string used to create an internal constraint. Very similar to a
		/// normal action, except the internal constraint connector is activated on completion
		/// of the action.
		/// </summary>
		public const string ORMDiagramInternalUniquenessConstraintFilterString = "ORMDiagramInternalUniquenessConstraintFilterString";
		/// <summary>
		/// The filter string used to connect role sequences to internal uniqueness constraints
		/// </summary>
		public const string ORMDiagramConnectInternalUniquenessConstraintFilterString = "ORMDiagramConnectInternalUniquenessConstraintFilterString";
		/// <summary>
		/// The filter string used to connect a role to its role player object type
		/// </summary>
		public const string ORMDiagramConnectRoleFilterString = "ORMDiagramConnectRoleFilterString";
		#endregion // Toolbox filter strings
		#region StickyEditObject
		/// <summary>
		/// The StickyObject associated with this diagram.  
		/// </summary>
		[NonSerialized]
		private IStickyObject mySticky;
		/// <summary>
		/// Get access to the diagram's StickyObject
		/// </summary>
		/// <value>StickyObject</value>
		public IStickyObject StickyObject
		{
			get
			{
				return mySticky;
			}
			set
			{
				// If the previous StickyObject was a ShapeElement, invalidate it so that it can redraw.
				// This is because a sticky ShapeElement should give a visual indicator that it's active.

				// Need to account for: going from null to ShapeElement, ShapeElement to null, ShapeElement to ShapeElement
				IStickyObject currentStickyShape;
				IStickyObject incomingStickyShape;

				currentStickyShape = mySticky;
				incomingStickyShape = value;
				if (currentStickyShape != null)
				{
					mySticky = null;
					currentStickyShape.StickyRedraw();
				}

				if (incomingStickyShape != null)
				{
					mySticky = value;
					mySticky.StickyInitialize();
				}
			}
		}
		#endregion //StickyEditObject
		#region View Fixup Methods
		/// <summary>
		/// Called as a result of the FixUpDiagram calls
		/// with the diagram as the first element.
		/// </summary>
		/// <param name="element">Added element</param>
		/// <returns>True for items displayed directly on the
		/// surface. Nesting object types are not displayed.</returns>
		protected override bool ShouldAddShapeForElement(ModelElement element)
		{
			ElementLink link = element as ElementLink;
			SubtypeFact subtypeFact = null;
			ModelElement element1 = null;
			ModelElement element2 = null;
			if (link != null)
			{
				element1 = link.GetRolePlayer(0);
				Role role1 = element1 as Role;
				if (role1 != null)
				{
					element1 = role1.FactType;
				}
				element2 = link.GetRolePlayer(1);
				Role role2 = element2 as Role;
				if (role2 != null)
				{
					element2 = role2.FactType;
				}
			}
			else if ((subtypeFact = element as SubtypeFact) != null)
			{
				element1 = subtypeFact.Subtype;
				element2 = subtypeFact.Supertype;
			}

			bool isLink = link != null || subtypeFact != null;
			if (isLink && (element1 == null || element2 == null || FindShapeForElement(element1) == null || FindShapeForElement(element2) == null))
			{
				return false;
			}
			else if (!isLink && !this.AutoPopulateShapes)
			{
				// Note that this used to be the following, but we can't rely on ActiveDiagramView
				// to be null when the diagram is visible in an inactive window.
				// MSBUG ActiveDiagramView should be null if the containing window is not active.
				//else if (!isLink && (!this.AutoPopulateShapes && this.ActiveDiagramView == null))
				DiagramView activeDiagramView = this.ActiveDiagramView;
				if (activeDiagramView == null)
				{
					return false;
				}
				else
				{
					// If the diagram has an active view on it, then we
					// need to make sure that the view belongs to the currently
					// active document. Otherwise, with multiple windows open
					// on the document and different diagrams open in each document,
					// dropping on one document will also add the shape to the
					// diagram in the non-active window.
					IServiceProvider serviceProvider;
					IMonitorSelectionService selectionService;
					DiagramDocView currentView;
					if (null == (serviceProvider = (Store as IORMToolServices).ServiceProvider) ||
						null == (selectionService = (IMonitorSelectionService)serviceProvider.GetService(typeof(IMonitorSelectionService))) ||
						null == (currentView = selectionService.CurrentDocumentView as DiagramDocView) ||
						!object.ReferenceEquals(currentView.CurrentDesigner, activeDiagramView))
					{
						return false;
					}
				}
			}
			ObjectType objType;
			FactType factType;
			ObjectTypePlaysRole objectTypePlaysRole;
			SetConstraint setConstraint;
			FactConstraint factConstraint;
			if (null != (factType = element as FactType))
			{
				if (factType is SubtypeFact)
				{
					return true;
				}
#if !SHOW_IMPLIED_SHAPES
				else if (factType.ImpliedByObjectification != null)
				{
					return false;
				}
#endif // !SHOW_IMPLIED_SHAPES
				return ShouldDisplayPartOfReferenceMode(factType);
			}
			else if (null != (objectTypePlaysRole = element as ObjectTypePlaysRole))
			{
#if SHOW_IMPLIED_SHAPES
#if !SHOW_FACTSHAPE_FOR_SUBTYPE
				FactType fact = objectTypePlaysRole.PlayedRoleCollection.FactType;
				if (fact is SubtypeFact)
				{
					return false;
				}
#endif // !SHOW_FACTSHAPE_FOR_SUBTYPE
#elif SHOW_FACTSHAPE_FOR_SUBTYPE
				FactType fact = objectTypePlaysRole.PlayedRoleCollection.FactType;
				if (fact.ImpliedByObjectification != null)
				{
					return false;
				}
#else
				FactType fact = objectTypePlaysRole.PlayedRoleCollection.FactType;
				if (fact is SubtypeFact || fact.ImpliedByObjectification != null)
				{
					return false;
				}
#endif
				return ShouldDisplayPartOfReferenceMode(objectTypePlaysRole);
			}
			else if (null != (factConstraint = element as FactConstraint))
			{
				return true;
			}
			else if (null != (setConstraint = element as SetConstraint))
			{
				return !setConstraint.Constraint.ConstraintIsInternal;
			}
			else if (element is SetComparisonConstraint ||
					 element is RoleHasValueConstraint)
			{
				return true;
			}
			else if (null != (objType = element as ObjectType))
			{
				return ShouldDisplayObjectType(objType);
			}
			return base.ShouldAddShapeForElement(element);
		}
		/// <summary>
		/// Determine if an ObjectType element should be displayed on
		/// the diagram.
		/// </summary>
		/// <param name="typeElement">The element to test</param>
		/// <returns>true to display, false to not display</returns>
		public bool ShouldDisplayObjectType(ObjectType typeElement)
		{
			// We don't ever display a nesting ObjectType, even if the Objectification is not drawn.
			if (typeElement.NestedFactType == null)
			{
				return ShouldDisplayPartOfReferenceMode(typeElement);
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// Function to determine if a fact type, which may be participating
		/// in a reference mode pattern, should be displayed.
		/// </summary>
		private bool ShouldDisplayPartOfReferenceMode(FactType factType)
		{
			foreach (UniquenessConstraint constraint in factType.GetInternalConstraints<UniquenessConstraint>())
			{
				ObjectType entity = constraint.PreferredIdentifierFor;
				// We only consider this to be a collapsible ref mode if its roleplayer is a value type
				if (entity != null && constraint.RoleCollection[0].RolePlayer.IsValueType)
				{
					return !ShouldCollapseReferenceMode(entity);
				}
			}
			return true;
		}
		/// <summary>
		/// Function to determine if a role player link, which may be participating
		/// in a reference mode pattern, should be displayed. Defers to test for
		/// the corresponding fact type.
		/// </summary>
		private bool ShouldDisplayPartOfReferenceMode(ObjectTypePlaysRole objectTypePlaysRole)
		{
			Role role = objectTypePlaysRole.PlayedRoleCollection;
			FactType factType = role.FactType;
			return (factType != null) ? ShouldDisplayPartOfReferenceMode(factType) : true;
		}
		/// <summary>
		/// Function to determine if an object type, which may be participating
		/// as the value type in the reference mode pattern, should be displayed. The
		/// object type needs to be displayed if any of the reference modes using the
		/// value type has a true ExpandRefMode property or if the object type is
		/// a role player for any other visible role.
		/// </summary>
		private bool ShouldDisplayPartOfReferenceMode(ObjectType objectType)
		{
			if (objectType.IsValueType)
			{
				RoleMoveableCollection playedRoles = objectType.PlayedRoleCollection;
				int playedRoleCount = playedRoles.Count;
				if (playedRoleCount > 0)
				{
					bool partOfCollapsedRefMode = false;
					for (int i = 0; i < playedRoleCount; ++i)
					{
						FactType factType = playedRoles[i].FactType;
						if (factType != null)
						{
							if (ShouldDisplayPartOfReferenceMode(factType))
							{
								partOfCollapsedRefMode = false;
								break;
							}
							else
							{
								partOfCollapsedRefMode = true;
								// Keep going. We may be part of a
								// non-collapsed relationship as well, in
								// which case we need to be visible.
							}
						}
					}
					if (partOfCollapsedRefMode)
					{
						return false;
					}
				}
			}
			return true;
		}
		/// <summary>
		/// Test if the reference mode should be collapsed. Helper function for
		/// ShouldDisplayPartOfReferenceMode implementations.
		/// </summary>
		/// <param name="objectType"></param>
		/// <returns>True if the object type has a collapsed reference mode</returns>
		private bool ShouldCollapseReferenceMode(ObjectType objectType)
		{
			ObjectTypeShape objectTypeShape = FindShapeForElement<ObjectTypeShape>(objectType);
			if (objectTypeShape != null)
			{
				if (objectType.HasReferenceMode)
				{
					return !objectTypeShape.ExpandRefMode;
				}
			}
			return objectType.HasReferenceMode;
		}
		/// <summary>
		/// An object type is displayed as an ObjectTypeShape unless it is
		/// objectified, in which case we display it as an ObjectifiedFactTypeNameShape
		/// </summary>
		/// <param name="element">The element to test. Expecting an ObjectType.</param>
		/// <param name="shapeTypes">The choice of shape types</param>
		/// <returns></returns>
		protected override MetaClassInfo ChooseShape(ModelElement element, IList shapeTypes)
		{
			Guid classId = element.MetaClassId;
			if (classId == ObjectType.MetaClassGuid)
			{
				return ChooseShapeTypeForObjectType((ObjectType)element, shapeTypes);
			}
#if SHOW_FACTSHAPE_FOR_SUBTYPE
			if (classId == SubtypeFact.MetaClassGuid)
			{
				foreach (MetaClassInfo classInfo in shapeTypes)
				{
					if (classInfo.Id == FactTypeShape.MetaClassGuid)
					{
						return classInfo;
					}
				}
			}
#endif // SHOW_FACTSHAPE_FOR_SUBTYPE
			Debug.Assert(false); // We're only expecting an ObjectType here
			return base.ChooseShape(element, shapeTypes);
		}
		/// <summary>
		/// Helper function to choose the appropriate shape for an ObjectType
		/// UNDONE: The original plan was to override ChooseParentShape here to switch the
		/// parent to a FactType. However, the childShape passed to ChooseParentShape is not
		/// yet attached to its backing ModelElement, so the override is essentially useless,
		/// and we need to duplicate the ChooseShape code on FactTypeShape itself.
		/// </summary>
		/// <param name="element">ObjectType</param>
		/// <param name="shapeTypes">IList of MetaClassInfo</param>
		/// <returns></returns>
		public static MetaClassInfo ChooseShapeTypeForObjectType(ObjectType element, IList shapeTypes)
		{
			Guid shapeClassId = FactTypeShape.ShouldDrawObjectification(element.NestedFactType) ? ObjectifiedFactTypeNameShape.MetaClassGuid : ObjectTypeShape.MetaClassGuid;
			foreach (MetaClassInfo classInfo in shapeTypes)
			{
				if (classInfo.Id == shapeClassId)
				{
					return classInfo;
				}
			}
			Debug.Assert(false); // Missed a shape associated with ObjectType
			return null;
		}
		/// <summary>
		/// Defer to ConfiguringAsChildOf for ORMBaseShape and ORMBaseBinaryLinkShape children
		/// </summary>
		/// <param name="child">The child being configured</param>
		protected override void OnChildConfiguring(ShapeElement child)
		{
			ORMBaseShape baseShape;
			ORMBaseBinaryLinkShape baseLinkShape;
			if (null != (baseShape = child as ORMBaseShape))
			{
				baseShape.ConfiguringAsChildOf(this);
			}
			else if (null != (baseLinkShape = child as ORMBaseBinaryLinkShape))
			{
				// ORM lines cross, they don't jump. However, the RouteJumpType cannot
				// be set before the diagram is in place, so this property cannot be set
				// from initialization code in the shape itself.
				baseLinkShape.RouteJumpType = VGObjectLineJumpCode.VGObjectJumpCodeNever;
				baseLinkShape.ConfiguringAsChildOf(this);
			}
		}
		/// <summary>
		/// Locate an existing shape on this diagram corresponding to this element
		/// </summary>
		/// <param name="element">The element to search</param>
		/// <returns>An existing shape, or null if not found</returns>
		public ShapeElement FindShapeForElement(ModelElement element)
		{
			return FindShapeForElement<ShapeElement>(element);
		}
		/// <summary>
		/// Locate an existing typed shape on this diagram corresponding to this element
		/// </summary>
		/// <typeparam name="TShape">The type of the shape to return</typeparam>
		/// <param name="element">The element to search</param>
		/// <returns>An existing shape, or null if not found</returns>
		public TShape FindShapeForElement<TShape>(ModelElement element) where TShape : ShapeElement
		{
			foreach (PresentationElement pel in element.PresentationRolePlayers)
			{
				TShape shape = pel as TShape;
				if (shape != null && shape.Diagram == this)
				{
					return shape;
				}
			}
			return null;
		}
		#endregion // View Fixup Methods
		#region Customize appearance
		/// <summary>
		/// The Brush to use when drawing the background of a sticky object.
		/// </summary>
		public static readonly StyleSetResourceId StickyBackgroundResource = new StyleSetResourceId("Neumont", "StickyBackgroundResource");
		/// <summary>
		/// The Brush to use when drawing the foreground of a sticky object.
		/// </summary>
		public static readonly StyleSetResourceId StickyForegroundResource = new StyleSetResourceId("Neumont", "StickyForegroundResource");
		/// <summary>
		/// The brush or pen used to draw a link decorator as sticky
		/// </summary>
		public static readonly StyleSetResourceId StickyConnectionLineDecoratorResource = new StyleSetResourceId("Neumont", "StickyConnectionLineDecorator");
		/// <summary>
		/// The brush or pen used to draw a link decorator as active. Generally corresponds to the role picker color
		/// </summary>
		public static readonly StyleSetResourceId ActiveConnectionLineDecoratorResource = new StyleSetResourceId("Neumont", "ActiveConnectionLineDecorator");
		/// <summary>
		/// The brush used to draw a link as active. Generally corresponds to the role picker color.
		/// </summary>
		public static readonly StyleSetResourceId ActiveBackgroundResource = new StyleSetResourceId("Neumont", "ActiveBackgroundResource");
		/// <summary>
		/// The brush used to draw the background for an item with errors.
		/// </summary>
		public static readonly StyleSetResourceId ErrorBackgroundResource = new StyleSetResourceId("Neumont", "ErrorBackgroundResource");
		/// <summary>
		/// The brush used to draw the background for an item with errors when the shape is highlighted.
		/// </summary>
		public static readonly StyleSetResourceId HighlightedErrorBackgroundResource = new StyleSetResourceId("Neumont", "HighlightedErrorBackgroundResource");

		/// <summary>
		/// Standard override to populate the style set for the shape type
		/// </summary>
		protected override void InitializeResources(StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);

			IORMFontAndColorService colorService = (Store as IORMToolServices).FontAndColorService;
			Color stickyBackColor = colorService.GetBackColor(ORMDesignerColor.ActiveConstraint);
			Color stickyForeColor = colorService.GetForeColor(ORMDesignerColor.ActiveConstraint);
			BrushSettings brushSettings = new BrushSettings();
			brushSettings.Color = stickyBackColor;
			classStyleSet.AddBrush(StickyBackgroundResource, DiagramBrushes.ShapeBackgroundSelected, brushSettings);

			brushSettings.Color = stickyForeColor;
			classStyleSet.AddBrush(StickyForegroundResource, DiagramBrushes.ShapeText, brushSettings);

			PenSettings penSettings = new PenSettings();
			penSettings.Color = stickyBackColor;
			penSettings.Width = 1.0F / 72.0F; // 1 Point. 0 Means 1 pixel, but should only be used for non-printed items
			penSettings.Alignment = PenAlignment.Center;
			classStyleSet.AddPen(StickyBackgroundResource, DiagramPens.ShapeHighlightOutline, penSettings);

			penSettings.Color = stickyForeColor;
			classStyleSet.AddPen(StickyForegroundResource, DiagramPens.ShapeHighlightOutline, penSettings);
		}
		/// <summary>
		/// Drop the grid size to make positioning easier.
		/// </summary>
		public override double DefaultGridSize
		{
			get
			{
				return .05;
			}
		}
		/// <summary>
		/// Overrides the OnCreated to set the snaptogrid to false;
		/// </summary>
		public override void OnCreated()
		{
			base.OnCreated();
			//turned snap to grid off because we are aligning the facttypes based
			//on the center of the roles. Since the center of the roles is not necessarily
			//going to be located in alignment on the grid we had to turn this off so facttypes
			//would get properly aligned with other objects.
			SnapToGrid = false;  
		}
		#endregion // Customize appearance
		#region Toolbox initialization
		/// <summary>
		/// Initialize toolbox items. All items are thrown on the diagram (it doesn't
		/// really matter what object we put them on).
		/// </summary>
		/// <param name="toolboxItem"></param>
		/// <returns></returns>
		public override ElementGroupPrototype InitializeToolboxItem(ModelingToolboxItem toolboxItem)
		{
			Store store = Store;
			string itemId = toolboxItem.Id;
			ElementGroup group = new ElementGroup(store);
			ElementGroupPrototype retVal = null;
			int roleArity = 0;
			switch (itemId)
			{
				case ResourceStrings.ToolboxEntityTypeItemId:
					ObjectType entityType = ObjectType.CreateObjectType(store);
					group.AddGraph(entityType);
					retVal = group.CreatePrototype(entityType);
					break;
				case ResourceStrings.ToolboxValueTypeItemId:
					ObjectType valueType = ObjectType.CreateObjectType(store);
					group.AddGraph(valueType);
					// Do not try to set the IsValueType property here. IsValueType picks
					// up the default data type for the model, which can only be done
					// when the model is known. Instead, flag the element so that it
					// can be set during MergeRelate on the model.
					group.UserData = "VALUETYPE";
					retVal = group.CreatePrototype(valueType);
					break;
				case ResourceStrings.ToolboxObjectifiedFactTypeItemId:
					roleArity = 2;
					break;
				case ResourceStrings.ToolboxUnaryFactTypeItemId:
					roleArity = 1;
					break;
				case ResourceStrings.ToolboxBinaryFactTypeItemId:
					roleArity = 2;
					break;
				case ResourceStrings.ToolboxTernaryFactTypeItemId:
					roleArity = 3;
					break;
				case ResourceStrings.ToolboxInternalUniquenessConstraintItemId:
					UniquenessConstraint iuc = UniquenessConstraint.CreateInternalUniquenessConstraint(store);
					group.AddGraph(iuc);
					// Add this here so that we can distinguish between internal and external uniqueness
					// constraints without unpacking the model. We want to merge internals into a fact
					// and externals into the model.
					group.UserData = "INTERNALUNIQUENESSCONSTRAINT";
					retVal = group.CreatePrototype(iuc);
					break;
				case ResourceStrings.ToolboxExternalUniquenessConstraintItemId:
					UniquenessConstraint euc = UniquenessConstraint.CreateUniquenessConstraint(store);
					group.AddGraph(euc);
					retVal = group.CreatePrototype(euc);
					break;
				case ResourceStrings.ToolboxEqualityConstraintItemId:
					EqualityConstraint eqc = EqualityConstraint.CreateEqualityConstraint(store);
					group.AddGraph(eqc);
					retVal = group.CreatePrototype(eqc);
					break;
				case ResourceStrings.ToolboxExclusionConstraintItemId:
					ExclusionConstraint exc = ExclusionConstraint.CreateExclusionConstraint(store);
					group.AddGraph(exc);
					retVal = group.CreatePrototype(exc);
					break;
				case ResourceStrings.ToolboxInclusiveOrConstraintItemId:
					MandatoryConstraint dmc = MandatoryConstraint.CreateMandatoryConstraint(store);
					group.AddGraph(dmc);
					retVal = group.CreatePrototype(dmc);
					break;
				case ResourceStrings.ToolboxExclusiveOrConstraintItemId:
					// Intentionally unprototyped item (for now)
					break;
				case ResourceStrings.ToolboxSubsetConstraintItemId:
					SubsetConstraint sc = SubsetConstraint.CreateSubsetConstraint(store);
					group.AddGraph(sc);
					retVal = group.CreatePrototype(sc);
					break;
				case ResourceStrings.ToolboxFrequencyConstraintItemId:
					FrequencyConstraint fc = FrequencyConstraint.CreateFrequencyConstraint(store);
					group.AddGraph(fc);
					retVal = group.CreatePrototype(fc);
					break;
				case ResourceStrings.ToolboxRingConstraintItemId:
					RingConstraint ring = RingConstraint.CreateRingConstraint(store);
					group.AddGraph(ring);
					retVal = group.CreatePrototype(ring);
					break;
				case ResourceStrings.ToolboxRoleConnectorItemId:
				case ResourceStrings.ToolboxExternalConstraintConnectorItemId:
				case ResourceStrings.ToolboxSubtypeConnectorItemId:
					// Intentionally unprototyped item
					break;
				default:
					Debug.Assert(false, "Unknown ResourceString Id"); // Unknown Id
					break;
			}
			if (retVal == null)
			{
				if (roleArity != 0)
				{
					FactType factType = FactType.CreateFactType(store);
					RoleBaseMoveableCollection roles = factType.RoleCollection;
					for (int i = 0; i < roleArity; ++i)
					{
						Role role = Role.CreateRole(store);
						roles.Add(role);
						group.AddGraph(role);
					}
					group.AddGraph(factType);
					if (itemId == ResourceStrings.ToolboxObjectifiedFactTypeItemId)
					{
						// Create the relationship between the FactType and ObjectType for Objectified Fact Types
						ObjectType objectifiedObjectType = ObjectType.CreateObjectType(store);
						group.AddGraph(objectifiedObjectType);
						Objectification objectification = Objectification.CreateObjectification(store, new RoleAssignment[]
							{
								new RoleAssignment(Objectification.NestingTypeMetaRoleGuid, objectifiedObjectType),
								new RoleAssignment(Objectification.NestedFactTypeMetaRoleGuid, factType)
							});
						group.AddGraph(objectification);
						retVal = group.CreatePrototype(new ModelElement[] { factType, objectifiedObjectType });
					}
					else
					{
						retVal = group.CreatePrototype(factType);
					}
				}
			}
			return retVal;
		}
		#endregion // Toolbox initialization
		#region Toolbox support
		/// <summary>
		/// Enable our toolbox actions. Additional filters recognized in this
		/// routine are added in ORMEditorFactory.GetToolboxItems.
		/// </summary>
		public override void OnViewMouseEnter(DiagramPointEventArgs pointArgs)
		{
			DiagramView activeView = ActiveDiagramView;
			MouseAction action = null;

			if (activeView != null)
			{
				if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramConnectExternalConstraintFilterString))
				{
					action = ExternalConstraintConnectAction;
				}
				else if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramExternalConstraintFilterString))
				{
					action = ExternalConstraintAction;
				}
				else if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramConnectInternalUniquenessConstraintFilterString))
				{
					action = InternalUniquenessConstraintConnectAction;
				}
				else if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramInternalUniquenessConstraintFilterString))
				{
					action = InternalUniquenessConstraintAction;
				}
				else if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramConnectRoleFilterString))
				{
					action = RoleConnectAction;
				}
				else if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramCreateSubtypeFilterString))
				{
					action = SubtypeConnectAction;
				}
				else if (activeView.SelectedToolboxItemSupportsFilterString(ORMDiagram.ORMDiagramDefaultFilterString))
				{
					action = ToolboxAction;
				}
			}

			DiagramClientView clientView = pointArgs.DiagramClientView;
			if (clientView.ActiveMouseAction != action &&
				// UNDONE: We should not need the following line because the current mouse action
				// should correspond to the current toolbox action. However, Toolbox.SetSelectedToolboxItem
				// always crashes, so there is no way to reset the action when we explicitly chain.
				// The result of not doing this is that moving the mouse off and back on the diagram
				// during a chained mouse action cancels the action.
				// See corresponding code in ExternalConstraintConnectAction.ChainMouseAction and
				// InternalUniquenessConstraintConnectAction.ChainMouseAction.
				(action != null || activeView.Toolbox.GetSelectedToolboxItem() != null))
			{
				clientView.ActiveMouseAction = action;
			}
		}
		/// <summary>
		/// Select the given item on the default tab
		/// </summary>
		/// <param name="activeView">DiagramView</param>
		/// <param name="itemId">Name of the item id</param>
		public static void SelectToolboxItem(DiagramView activeView, string itemId)
		{
			SelectToolboxItem(activeView, itemId, ResourceStrings.ToolboxDefaultTabName);
		}
		/// <summary>
		/// Select the given item on the specified toolbox tab
		/// UNDONE: The critical point of this routine crashes VS, so
		/// it is currently a noop
		/// </summary>
		/// <param name="activeView">DiagramView</param>
		/// <param name="itemId">Name of the item id</param>
		/// <param name="tabName">The tab name to select</param>
		public static void SelectToolboxItem(DiagramView activeView, string itemId, string tabName)
		{
			IToolboxService toolbox = activeView.Toolbox;
			if (toolbox != null)
			{
				// Select the connector action on the toolbox
				Debug.Assert(toolbox.GetSelectedToolboxItem() == null); // Should be turned off during MouseActionDeactivated
				ToolboxItemCollection items = toolbox.GetToolboxItems(tabName);
				foreach (ToolboxItem item in items)
				{
					ModelingToolboxItem modelingItem = item as ModelingToolboxItem;
					if (modelingItem != null && modelingItem.Id == itemId)
					{
						// UNDONE: See comments on side effect in ORMDiagram.OnViewMouseEnter
						//toolbox.SetSelectedToolboxItem(item); // UNDONE: MSBUG Gives 'Value does not fall within expected range' error message, not sure why
						break;
					}
				}
			}
		}
		#region External constraint action
		[NonSerialized]
		private ExternalConstraintConnectAction myExternalConstraintConnectAction;
		[NonSerialized]
		private ExternalConstraintAction myExternalConstraintAction;
		/// <summary>
		/// The connect action used to connect an external constraint to its role sequences
		/// </summary>
		public ExternalConstraintConnectAction ExternalConstraintConnectAction
		{
			get
			{
				if (myExternalConstraintConnectAction == null)
				{
					myExternalConstraintConnectAction = new ExternalConstraintConnectAction(this);
				}
				return myExternalConstraintConnectAction;
			}
		}
		/// <summary>
		/// Create the action used to connect an external constraint to its role sequences
		/// </summary>
		/// <returns>ExternalConstraintConnectAction instance</returns>
		protected virtual ExternalConstraintConnectAction CreateExternalConstraintConnectAction()
		{
			return new ExternalConstraintConnectAction(this);
		}
		/// <summary>
		/// The action used to drop an external constraint from the toolbox
		/// </summary>
		public ExternalConstraintAction ExternalConstraintAction
		{
			get
			{
				if (myExternalConstraintAction == null)
				{
					myExternalConstraintAction = CreateExternalConstraintAction();
					myExternalConstraintAction.AfterMouseActionDeactivated += delegate(object sender, DiagramEventArgs e)
					{
						ExternalConstraintAction action = sender as ExternalConstraintAction;
						if (action.ActionCompleted)
						{
							ExternalConstraintShape addedShape = action.AddedConstraintShape;
							Debug.Assert(addedShape != null); // ActionCompleted should be false otherwise
							ExternalConstraintConnectAction.ChainMouseAction(addedShape, e.DiagramClientView);
						}
					};
				}
				return myExternalConstraintAction;
			}
		}
		/// <summary>
		/// Create the action used to add an external constraint from the toolbox
		/// </summary>
		/// <returns>ExternalConstraintAction instance</returns>
		protected virtual ExternalConstraintAction CreateExternalConstraintAction()
		{
			return new ExternalConstraintAction(this);
		}
		#endregion // External constraint action
		#region Internal uniqueness constraint action
		[NonSerialized]
		private InternalUniquenessConstraintAction myInternalUniquenessConstraintAction;
		[NonSerialized]
		private InternalUniquenessConstraintConnectAction myInternalUniquenessConstraintConnectAction;
		/// <summary>
		/// The connect action used to connect an internal uniqueness constraint
		/// to its roles.
		/// </summary>
		public InternalUniquenessConstraintConnectAction InternalUniquenessConstraintConnectAction
		{
			get
			{
				if (myInternalUniquenessConstraintConnectAction == null)
				{
					myInternalUniquenessConstraintConnectAction = CreateInternalUniquenessConstraintConnectAction();
				}
				return myInternalUniquenessConstraintConnectAction;
			}
		}
		/// <summary>
		/// Create the connect action used to add an internal uniqueness constraint from the toolbox
		/// </summary>
		/// <returns>InternalUniquenssConstraintAction instance</returns>
		protected virtual InternalUniquenessConstraintConnectAction CreateInternalUniquenessConstraintConnectAction()
		{
			return new InternalUniquenessConstraintConnectAction(this);
		}
		/// <summary>
		/// The action used to add an internal uniqueness constraint from the toolbox
		/// </summary>
		public InternalUniquenessConstraintAction InternalUniquenessConstraintAction
		{
			get
			{
				if (myInternalUniquenessConstraintAction == null)
				{
					myInternalUniquenessConstraintAction = CreateInternalUniquenessConstraintAction();
					myInternalUniquenessConstraintAction.AfterMouseActionDeactivated += delegate(object sender, DiagramEventArgs e)
					{
						InternalUniquenessConstraintAction action = sender as InternalUniquenessConstraintAction;
						if (action.ActionCompleted)
						{
							UniquenessConstraint constraint = action.AddedConstraint;
							FactTypeShape addedToShape = action.DropTargetShape;
							DiagramClientView view = e.DiagramClientView;
							Debug.Assert(constraint != null); // ActionCompleted should be false otherwise
							view.Selection.Set(addedToShape.GetDiagramItem(constraint));
							InternalUniquenessConstraintConnectAction.ChainMouseAction(addedToShape, constraint, view);
						}
					};
				}
				return myInternalUniquenessConstraintAction;
			}
		}
		/// <summary>
		/// Create the connect action used to connect internal uniqueness constrant roles
		/// </summary>
		/// <returns>InternalUniquenssConstraintAction instance</returns>
		protected virtual InternalUniquenessConstraintAction CreateInternalUniquenessConstraintAction()
		{
			return new InternalUniquenessConstraintAction(this);
		}
		#endregion Internal uniqueness constraint action
		#region Role drag action
		[NonSerialized]
		private RoleDragPendingAction myRoleDragPendingAction;
		[NonSerialized]
		private RoleConnectAction myRoleConnectAction;
		/// <summary>
		/// The drag action used by a role box to begin dragging.
		/// The default implementation chains to a RoleConnectAction
		/// when dragging begins.
		/// </summary>
		public RoleDragPendingAction RoleDragPendingAction
		{
			get
			{
				RoleDragPendingAction retVal = myRoleDragPendingAction;
				if (retVal == null)
				{
					myRoleDragPendingAction = retVal = CreateRoleDragPendingAction();
				}
				return retVal;
			}
		}
		/// <summary>
		/// Create the drag action used for the RoleDragPendingAction property
		/// </summary>
		/// <returns>RoleDragPendingAction instance</returns>
		protected virtual RoleDragPendingAction CreateRoleDragPendingAction()
		{
			return new RoleDragPendingAction(this);
		}
		/// <summary>
		/// The connect action used to connect a role and
		/// its role player (an object type)
		/// </summary>
		public RoleConnectAction RoleConnectAction
		{
			get
			{
				RoleConnectAction retVal = myRoleConnectAction;
				if (retVal == null)
				{
					myRoleConnectAction = retVal = CreateRoleConnectAction();
				}
				return retVal;
			}
		}
		/// <summary>
		/// Create the connect action used to connect roles to their role players
		/// </summary>
		/// <returns>RoleConnectAction instance</returns>
		protected virtual RoleConnectAction CreateRoleConnectAction()
		{
			return new RoleConnectAction(this);
		}
		#endregion // Role drag action
		#region Subtype create action
		[NonSerialized]
		private SubtypeConnectAction mySubtypeConnectAction;
		/// <summary>
		/// The connect action used to connect a base type to a derived type
		/// </summary>
		public SubtypeConnectAction SubtypeConnectAction
		{
			get
			{
				SubtypeConnectAction retVal = mySubtypeConnectAction;
				if (retVal == null)
				{
					mySubtypeConnectAction = retVal = CreateSubtypeConnectAction();
				}
				return retVal;
			}
		}
		/// <summary>
		/// Create the connect action used to connect roles to their role players
		/// </summary>
		/// <returns>SubtypeConnectAction instance</returns>
		protected virtual SubtypeConnectAction CreateSubtypeConnectAction()
		{
			return new SubtypeConnectAction(this);
		}
		#endregion // Subtype create action
		#endregion // Toolbox support
		#region Other base overrides
		/// <summary>
		/// Set the initial diagram name. We always set it to 'New Page' for now
		/// </summary>
		public override void OnInitialized()
		{
			base.OnInitialized();
			Name = ResourceStrings.DiagramDefaultNewPageName;
		}
		/// <summary>
		/// Clean up disposable members (connection actions)
		/// </summary>
		/// <param name="disposing">Do stuff if true</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					// Use a somewhat paranoid pattern here to protect against reentrancy
					IDisposable disposeMe;
					disposeMe = myExternalConstraintAction as IDisposable;
					myExternalConstraintAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}

					disposeMe = myExternalConstraintConnectAction as IDisposable;
					myExternalConstraintConnectAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}

					disposeMe = myInternalUniquenessConstraintAction as IDisposable;
					myInternalUniquenessConstraintAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}

					disposeMe = myInternalUniquenessConstraintConnectAction as IDisposable;
					myInternalUniquenessConstraintConnectAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}

					disposeMe = myRoleDragPendingAction as IDisposable;
					myRoleDragPendingAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}

					disposeMe = myRoleConnectAction as IDisposable;
					myRoleConnectAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}

					disposeMe = mySubtypeConnectAction as IDisposable;
					mySubtypeConnectAction = null;
					if (disposeMe != null)
					{
						disposeMe.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		/// <summary>
		/// Set the base font based on the font and color settings.
		/// UNDONE: This affects the size right now, but not the font name
		/// </summary>
		protected override Font BaseFontFromEnvironment
		{
			get
			{
				return (this.Store as ObjectModel.IORMToolServices).FontAndColorService.GetFont(ORMDesignerColorCategory.Editor);
			}
		}
		/// <summary>
		/// Stop all auto shape selection on transaction commit except when
		/// the item is being dropped.
		/// </summary>
		public override IList FixUpDiagramSelection(ShapeElement newChildShape)
		{
			if (DropTargetContext.HasDropTargetContext(Store.TransactionManager.CurrentTransaction))
			{
				return base.FixUpDiagramSelection(newChildShape);
			}
			return null;
		}
		#endregion // Other base overrides
		#region Display Properties
		/// <summary>
		/// Retrieve the component name for the property grid. The
		/// component name is displayed bolded in the property grid dropdown
		/// before the class name (retrieved from GetClassName)
		/// </summary>
		/// <returns></returns>
		public override string GetComponentName()
		{
			ModelElement element = ModelElement;
			return (element != null) ? element.GetComponentName() : base.GetComponentName();
		}
		/// <summary>
		/// Crash fix, the shell is calling back after the store is disposed. Catch the case.
		/// </summary>
		public override string GetClassName()
		{
			return Store.Disposed ? GetType().Name : base.GetClassName();
		}
		/// <summary>
		/// Block display of the diagram's name, which is displayed beside the
		/// Name for the underlying model if we let it through
		/// </summary>
		/// <param name="metaAttrInfo">MetaAttributeInfo</param>
		/// <returns>false for Diagram.Name, defers to base for all other attributes</returns>
		public override bool ShouldCreatePropertyDescriptor(MetaAttributeInfo metaAttrInfo)
		{
			Guid attributeGuid = metaAttrInfo.Id;
			if (attributeGuid == Diagram.NameMetaAttributeGuid)
			{
				return false;
			}
			return base.ShouldCreatePropertyDescriptor(metaAttrInfo);
		}
		#endregion // Display Properties
		#region Accessibility Properties
		/// <summary>
		/// Return the class name as the accessible name
		/// </summary>
		public override string AccessibleName
		{
			get
			{
				return GetClassName();
			}
		}
		/// <summary>
		/// Return the component name as the accessible value
		/// </summary>
		public override string AccessibleValue
		{
			get
			{
				return GetComponentName();
			}
		}
		#endregion // Accessibility Properties
		#region Utility Methods
		/// <summary>
		/// Modify the luminosity for a given color. This
		/// duplicates the algorithm in ShapeElement.GetShapeLuminosity,
		/// which is not available because it is only run for shape elements
		/// in the DiagramClientView.HighlightedShapes collection.
		/// </summary>
		/// <param name="startColor">The original color</param>
		/// <returns>The modified color</returns>
		public static Color ModifyLuminosity(Color startColor)
		{
			HslColor hslColor = HslColor.FromRgbColor(startColor);
			hslColor.Luminosity = ModifyLuminosity(hslColor.Luminosity);
			return hslColor.ToRgbColor();
		}
		/// <summary>
		/// Modify a specific luminosity. 
		/// </summary>
		/// <param name="startLuminosity">Beginning luminosity value</param>
		/// <returns>modified luminosity</returns>
		public static int ModifyLuminosity(int startLuminosity)
		{
			// Base framework algorithm for reference
			//const int luminosityCheck = 160;
			//const int luminosityDelta = 40;
			//const double luminosityFactor = 0.9;
			//return (startLuminosity >= luminosityCheck) ?
			//	(int)(startLuminosity * luminosityFactor) :
			//	(startLuminosity + luminosityDelta);

			// Use a sliding scale to brighten/darken colors
			const int maxLuminosity = 255;
			const int luminosityCheck = 160;
			const int luminosityFixedDelta = 60;
			const int luminosityIncrementalDelta = 30;
			const double luminosityFixedFactor = 0.93;
			const double luminosityIncrementalFactor = -0.06;
			return (startLuminosity >= luminosityCheck) ?
				(int)(startLuminosity * (luminosityFixedFactor + (luminosityIncrementalFactor * (double)(maxLuminosity - startLuminosity) / (maxLuminosity - luminosityCheck)))) :
				(startLuminosity + luminosityFixedDelta + (int)((double)(luminosityCheck - startLuminosity) / luminosityCheck * luminosityIncrementalDelta));
		}
		#endregion // Utility Methods
		#region IProxyDisplayProvider Implementation
		/// <summary>
		/// Implements IProxyDisplayProvider.ElementDisplayedAs
		/// </summary>
		protected ModelElement ElementDisplayedAs(ModelElement element)
		{
			ObjectType objectElement;
			if (null != (objectElement = element as ObjectType))
			{
				if (!ShouldDisplayObjectType(objectElement))
				{
					FactType nestedFact = objectElement.NestedFactType;
					if (nestedFact != null)
					{
						return nestedFact;
					}
					// Otherwise, every fact type we're a role player for is
					// part of a collapsed reference mode. Grab the first fact, and
					// find the corresponding object type.
					foreach (ConstraintRoleSequence constraintSequence in objectElement.PlayedRoleCollection[0].ConstraintRoleSequenceCollection)
					{
						UniquenessConstraint iuc = constraintSequence as UniquenessConstraint;
						if (iuc != null && iuc.IsInternal)
						{
							ObjectType displayedType = iuc.PreferredIdentifierFor;
							if (displayedType != null)
							{
								return displayedType;
							}
						}
					}
				}
			}
			return null;
		}
		ModelElement IProxyDisplayProvider.ElementDisplayedAs(ModelElement element)
		{
			return ElementDisplayedAs(element);
		}
		#endregion // IProxyDisplayProvider Implementation
	}
	#region Extra conditional attribute on FactTypeShape to show fact shape instead of subtype link
#if SHOW_FACTSHAPE_FOR_SUBTYPE
	// This forces a call to ORMDiagram.ChooseShape
	[ShapeForAttribute(typeof(SubtypeFact))]
	public partial class FactTypeShape
	{
	}
#endif // SHOW_FACTSHAPE_FOR_SUBTYPE
	#endregion // Extra conditional attribute on FactTypeShape to show fact shape instead of subtype link
}