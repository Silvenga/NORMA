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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Diagrams.GraphObject;
using Neumont.Tools.Modeling.Design;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.ORM.Shell;
using Neumont.Tools.Modeling;

namespace Neumont.Tools.ORM.ShapeModel
{
	[DebuggerDisplay("{System.String.Concat(ToString(), \": \",(ModelElement != null) ? ModelElement.ToString() : \"null\")}")]
	public partial class ORMBaseShape
	{
		#region Public token values
		/// <summary>
		/// Set this context key in the current transaction to
		/// force all child shapes to be explicitly placed.
		/// </summary>
		public static readonly object PlaceAllChildShapes = new object();
		#endregion // Public token values
		#region Virtual extensions
		/// <summary>
		/// Called during the OnChildConfiguring from the parent shape.
		/// The default implementation does nothing.
		/// </summary>
		/// <param name="parent">The parent shape. May be a diagram.</param>
		/// <param name="createdDuringViewFixup">Whether this shape was created as part of a view fixup</param>
		public virtual void ConfiguringAsChildOf(NodeShape parent, bool createdDuringViewFixup)
		{
		}
		/// <summary>
		/// Place the child shape the first time it is placed
		/// on the diagram
		/// </summary>
		/// <param name="parent">The parent shape</param>
		/// <param name="createdDuringViewFixup">Whether this shape was created as part of a view fixup</param>
		public virtual void PlaceAsChildOf(NodeShape parent, bool createdDuringViewFixup)
		{
		}
		/// <summary>
		/// Override to modify the content size of an item.
		/// By default, the AutoResize function will use this
		/// size (if it is set) with no margins.
		/// </summary>
		protected virtual SizeD ContentSize
		{
			get
			{
				return SizeD.Empty;
			}
		}
		/// <summary>
		/// Sizes to object to the size of the contents.
		/// The default implementation uses the ContentSize
		/// if it is set and does not do any margin adjustments.
		/// </summary>
		public virtual void AutoResize()
		{
			SizeD contentSize = ContentSize;
			if (!contentSize.IsEmpty)
			{
				Size = contentSize;
			}
		}
		#endregion // Virtual extensions
		#region Customize appearance
		/// <summary>
		/// Determines if a the model element backing the shape element
		/// is represented by a shape of the same type elsewhere in the presentation
		/// layer. If a derived shape displays multiple presentations, they should
		/// also override the <see cref="DisplaysMultiplePresentations"/> property.
		/// </summary>
		public static bool ElementHasMultiplePresentations(ShapeElement shapeElement)
		{
			ModelElement modelElement;
			if (null != shapeElement &&
				null != (modelElement = shapeElement.ModelElement))
			{
				LinkedElementCollection<PresentationElement> presentationElements = PresentationViewsSubject.GetPresentation(modelElement);
				int pelCount = presentationElements.Count;
				if (pelCount != 0)
				{
					Partition shapePartition = shapeElement.Partition;
					Type thisType = shapeElement.GetType();
					for (int i = 0; i < pelCount; ++i)
					{
						PresentationElement pel = presentationElements[i];
						if (shapeElement != pel && shapePartition == pel.Partition && thisType.IsAssignableFrom(pel.GetType()))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		/// <summary>
		/// A callback delegate for the <see cref="VisitAssociatedShapes"/> method
		/// </summary>
		/// <param name="shape">The shape element to visit</param>
		/// <returns>Return <see langword="true"/> to continue iteration</returns>
		public delegate bool VisitShape(ShapeElement shape);
		/// <summary>
		/// Iterate shapes associated with a given element
		/// </summary>
		/// <param name="modelElement">The parent element to iterate. Can be <see langword="null"/> if <paramref name="shapeElement"/> is specified.</param>
		/// <param name="shapeElement">The shape to reference. Can be <see langword="null"/> is <paramref name="modelElement"/> is specified.</param>
		/// <param name="visitor">A <see cref="VisitShape"/> callback delegate</param>
		public static void VisitAssociatedShapes(ModelElement modelElement, ShapeElement shapeElement, VisitShape visitor)
		{
			if (modelElement == null && shapeElement != null)
			{
				modelElement = shapeElement.ModelElement;
			}
			if (modelElement != null)
			{
				LinkedElementCollection<PresentationElement> presentationElements = PresentationViewsSubject.GetPresentation(modelElement);
				int pelCount = presentationElements.Count;
				if (pelCount != 0)
				{
					Partition shapePartition = (shapeElement != null) ? shapeElement.Partition : modelElement.Partition;
					Type thisType = (shapeElement != null) ? shapeElement.GetType() : typeof(ShapeElement);
					for (int i = 0; i < pelCount; ++i)
					{
						PresentationElement pel = presentationElements[i];
						if (shapePartition == pel.Partition && thisType.IsAssignableFrom(pel.GetType()))
						{
							if (!visitor(pel as ShapeElement))
							{
								return;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Turn off the shadow by default
		/// </summary>
		/// <value>false</value>
		public override bool HasShadow
		{
			get { return false; }
		}
		/// <summary>
		/// Indicate that the shape changes its display when more
		/// than one presentation is visible. Derived shapes should
		/// use the <see cref="ElementHasMultiplePresentations"/> method
		/// to determine when multiple presentations should be displayed.
		/// Default is <see langword="false"/>
		/// </summary>
		public virtual bool DisplaysMultiplePresentations
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Default to no sides being resizable.
		/// </summary>
		public override NodeSides ResizableSides
		{
			get
			{
				return NodeSides.None;
			}
		}
		/// <summary>
		/// Add error brushes to the styleSet
		/// </summary>
		protected override void InitializeResources(StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);

			IORMFontAndColorService colorService = (Store as IORMToolServices).FontAndColorService;
			BrushSettings brushSettings = new BrushSettings();
			//UNDONE: This color isn't permanent. probably want a better color for the errors.
			brushSettings.ForeColor = Color.LightPink;
			//brushSettings.ForeColor = colorService.GetForeColor(ORMDesignerColor.ConstraintError);
			brushSettings.HatchStyle = HatchStyle.LightDownwardDiagonal;
			brushSettings.BrushType = typeof(HatchBrush);
			classStyleSet.AddBrush(ORMDiagram.ErrorBackgroundResource, DiagramBrushes.DiagramBackground, brushSettings);
			brushSettings.ForeColor = ORMDiagram.ModifyLuminosity(brushSettings.ForeColor);
			brushSettings.BackColor = ORMDiagram.ModifyLuminosity(((SolidBrush)classStyleSet.GetBrush(DiagramBrushes.DiagramBackground)).Color);
			classStyleSet.AddBrush(ORMDiagram.HighlightedErrorBackgroundResource, DiagramBrushes.DiagramBackground, brushSettings);

			BrushSettings transBrush = new BrushSettings();
			transBrush.ForeColor = Color.Transparent;
			classStyleSet.AddBrush(ORMDiagram.TransparentBrushResource, DiagramBrushes.DiagramBackground, transBrush);
		}
		/// <summary>
		/// Use a different background brush if we have errors
		/// </summary>
		public override StyleSetResourceId BackgroundBrushId
		{
			get
			{
				if (ModelError.HasErrors(ModelElement, ModelErrorUses.DisplayPrimary))
				{
					ORMDiagram diagram;
					DiagramView view;
					DiagramClientView clientView;
					HighlightedShapesCollection highlightedShapes;
					if (null != (diagram = Diagram as ORMDiagram) &&
						null != (view = diagram.ActiveDiagramView) &&
						null != (clientView = view.DiagramClientView) &&
						null != (highlightedShapes = clientView.HighlightedShapes) &&
						highlightedShapes.Count != 0 &&
						highlightedShapes.Contains(new DiagramItem(this)))
					{
						return ORMDiagram.HighlightedErrorBackgroundResource;
					}
					else
					{
						return ORMDiagram.ErrorBackgroundResource;
					}
				}
				else
				{
					return DiagramBrushes.DiagramBackground;
				}
			}
		}
		/// <summary>
		/// Size the object appropriately
		/// </summary>
		public override void OnBoundsFixup(BoundsFixupState fixupState, int iteration, bool createdDuringViewFixup)
		{
			base.OnBoundsFixup(fixupState, iteration, createdDuringViewFixup);
			if (fixupState == BoundsFixupState.ViewFixup)
			{
				AutoResize();
			}
		}
		/// <summary>
		/// Allow relative child shapes to move above and to the left of their parent
		/// </summary>
		public override BoundsRules BoundsRules
		{
			get
			{
				return NoBoundsRules.Instance;
			}
		}
		/// <summary>
		/// Defer to ConfiguringAsChildOf for ORMBaseShape children
		/// </summary>
		/// <param name="child">The child being configured</param>
		/// <param name="createdDuringViewFixup">Whether this shape was created as part of a view fixup</param>
		protected override void OnChildConfiguring(ShapeElement child, bool createdDuringViewFixup)
		{
			ORMBaseShape baseShape;
			if (null != (baseShape = child as ORMBaseShape))
			{
				baseShape.ConfiguringAsChildOf(this, createdDuringViewFixup);
			}
		}
		/// <summary>
		/// If a new child shape was not placed, then defer to
		/// PlaceAsChildOf on the child. Note that AutoResize
		/// has not been called on the child prior to this call.
		/// </summary>
		/// <param name="child">A new child shape element</param>
		/// <param name="childWasPlaced">false if the child element was not previously placed.</param>
		/// <param name="createdDuringViewFixup">Whether this shape was created as part of a view fixup</param>
		protected override void OnChildConfigured(ShapeElement child, bool childWasPlaced, bool createdDuringViewFixup)
		{
			// Don't check childWasPlaced here during drag-drop. In this case,
			// childWasPlace will be true if the parent shape was placed, so is
			// only relevant for our code base for the top-level shapes, in which
			// case the OnChildConfigured for the dropped shape is called on the
			// diagram, not a base shape. Any derived shape that allows placed
			// drag from the toolbox (etc) onto child shapes (which none of
			// the shapes in ORMShapeDomainModel allow), then childWasPlaced should
			// be tested.
			if (childWasPlaced)
			{
				Transaction transaction = Store.TransactionManager.CurrentTransaction.TopLevelTransaction;
				if (transaction != null &&
					(DropTargetContext.HasDropTargetContext(transaction) ||
					transaction.Context.ContextInfo.ContainsKey(ORMBaseShape.PlaceAllChildShapes)))
				{
					childWasPlaced = false;
				}
			}
			if (!childWasPlaced)
			{
				ORMBaseShape baseShape;
				if (null != (baseShape = child as ORMBaseShape))
				{
					baseShape.PlaceAsChildOf(this, createdDuringViewFixup);
				}
			}
		}
		#endregion // Customize appearance
		#region Auto-activate associated errors
		/// <summary>
		/// Attempt to activate an activatable error.
		/// </summary>
		/// <param name="e">DiagramPointEventArgs. Indicates the element to activate.</param>
		/// <returns>true if an error was activated</returns>
		public static bool AttemptErrorActivation(DiagramPointEventArgs e)
		{
			bool retVal = false;
			if (!e.Handled)
			{
				DiagramClientView clientView = e.DiagramClientView;
				MouseAction action = clientView.ActiveMouseAction;
				if ((action == null || !(action is ConnectAction || action is ToolboxAction)) &&
					clientView.Selection.Count == 1)
				{
					DiagramItem diagramItem = e.DiagramHitTestInfo.HitDiagramItem;
					IModelErrorActivation activator = diagramItem.Shape as IModelErrorActivation;
					if (activator != null)
					{
						IModelErrorOwner errorOwner = null;
						foreach (ModelElement mel in diagramItem.RepresentedElements)
						{
							errorOwner = EditorUtility.ResolveContextInstance(mel, false) as IModelErrorOwner;
							break;
						}
						if (errorOwner != null)
						{
							foreach (ModelError error in errorOwner.GetErrorCollection(ModelErrorUses.DisplayPrimary))
							{
								if (activator.ActivateModelError(error))
								{
									// UNDONE: MSBUG Report Microsoft bug DiagramClientView.OnDoubleClick is checking
									// for an active mouse action after the double click and clearing it if it is set.
									// This may be appropriate if the mouse action was set before the subfield double
									// click and did not change during the callback, but is definitely not appropriate
									// if the double click activated the mouse action.
									// Note that this bug makes it impossible to override OnDoubleClick and OnSubFieldDoubleClick
									// because e.Handled cannot be reliably checked, so there is no way to call base.OnDoubleClick
									// of base.OnSubFieldDoubleClick from a more derived class without attempting error activation
									// twice. If this is fixed, then any OnDoubleClick/OnSubFieldDoubleClick implementation
									// that simply defers to this method then calls the base can be eliminated in favor
									// of the same methods here.
									//e.Handled = true;
									retVal = true;
									break;
								}
							}
						}
					}
				}
			}
			return retVal;
		}
		#endregion // Auto-activate associated errors
		#region Accessibility Properties
		/// <summary>
		/// Return the class name as the accessible name
		/// </summary>
		public override string AccessibleName
		{
			get
			{
				return TypeDescriptor.GetClassName(this);
			}
		}
		/// <summary>
		/// Return the component name as the accessible value
		/// </summary>
		public override string AccessibleValue
		{
			get
			{
				return TypeDescriptor.GetComponentName(this);
			}
		}
		#endregion // Accessibility Properties
		#region Auto-invalidate tracking
		/// <summary>
		/// Call to automatically invalidate the shape during events.
		/// Invalidates during the original event sequence as well as undo and redo.
		/// </summary>
		public void InvalidateRequired()
		{
			InvalidateRequired(false);
		}
		/// <summary>
		/// Call to automatically invalidate the shape during events.
		/// Invalidates during the original event sequence as well as undo and redo.
		/// </summary>
		/// <param name="refreshBitmap">Value to forward to the Invalidate method's refreshBitmap property during event playback</param>
		public void InvalidateRequired(bool refreshBitmap)
		{
			TransactionManager tmgr = Store.TransactionManager;
			if (tmgr.InTransaction)
			{
				UpdateCounter = unchecked(tmgr.CurrentTransaction.SequenceNumber - (refreshBitmap ? 0L : 1L));
			}
		}
		/// <summary>
		/// Called during event playback before an <see cref="ShapeElement.Invalidate()"/> call triggered
		/// via the <see cref="InvalidateRequired()"/> mechanism is called. The default implementation
		/// does nothing.
		/// </summary>
		protected virtual void BeforeInvalidate()
		{
		}
		private long GetUpdateCounterValue()
		{
			TransactionManager tmgr = Store.TransactionManager;
			if (tmgr.InTransaction)
			{
				// Using subtract 2 and set to 1 under to indicate
				// the difference between an Invalidate(true) and
				// and Invalidate(false)
				return unchecked(tmgr.CurrentTransaction.SequenceNumber - 2);
			}
			else
			{
				return 0L;
			}
		}
		private void SetUpdateCounterValue(long newValue)
		{
			// Nothing to do, we're just trying to create a transaction log
		}
		/// <summary>
		/// Manages <see cref="EventHandler{TEventArgs}"/>s in the <see cref="Store"/> for <see cref="ORMBaseShape"/>s.
		/// </summary>
		/// <param name="store">The <see cref="Store"/> for which the <see cref="EventHandler{TEventArgs}"/>s should be managed.</param>
		/// <param name="eventManager">The <see cref="ModelingEventManager"/> used to manage the <see cref="EventHandler{TEventArgs}"/>s.</param>
		/// <param name="action">The <see cref="EventHandlerAction"/> that should be taken for the <see cref="EventHandler{TEventArgs}"/>s.</param>
		public static void ManageEventHandlers(Store store, ModelingEventManager eventManager, EventHandlerAction action)
		{
			DomainDataDirectory dataDirectory = store.DomainDataDirectory;
			DomainClassInfo classInfo = dataDirectory.FindDomainClass(ORMBaseShape.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementPropertyChangedEventArgs>(PropertyChangedEvent), action);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(ShapeAddedEvent), action);
			classInfo = dataDirectory.FindDomainClass(PresentationViewsSubject.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(ShapeDeletedEvent), action);
		}
		private static void PropertyChangedEvent(object sender, ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id.Equals(UpdateCounterDomainPropertyId))
			{
				ORMBaseShape shape = (ORMBaseShape)e.ModelElement;
				if (!shape.IsDeleted)
				{
					shape.BeforeInvalidate();
					shape.Invalidate(Math.Abs(unchecked((long)e.OldValue - (long)e.NewValue)) != 1L);
				}
			}
		}
		private static void ShapeAddedEvent(object sender, ElementAddedEventArgs e)
		{
			ORMBaseShape shape = e.ModelElement as ORMBaseShape;
			ModelElement backingElement;
			if (shape.DisplaysMultiplePresentations &&
				null != (backingElement = shape.ModelElement))
			{
				InvalidateRemainingShapes(backingElement, shape);
			}
		}
		private static void ShapeDeletedEvent(object sender, ElementDeletedEventArgs e)
		{
			PresentationViewsSubject link = e.ModelElement as PresentationViewsSubject;
			ORMBaseShape shape;
			ModelElement backingElement;
			if (!(backingElement = link.Subject).IsDeleted &&
				null != (shape = link.Presentation as ORMBaseShape) &&
				shape.DisplaysMultiplePresentations)
			{
				InvalidateRemainingShapes(backingElement, null);
			}
		}
		private static void InvalidateRemainingShapes(ModelElement backingElement, ORMBaseShape ignoreShape)
		{
			LinkedElementCollection<PresentationElement> pels = PresentationViewsSubject.GetPresentation(backingElement);
			int pelCount = pels.Count;
			if (ignoreShape == null || pelCount > 1)
			{
				for (int i = 0; i < pelCount; ++i)
				{
					PresentationElement pel = pels[i];
					ORMBaseShape updateShape;
					if (pel != ignoreShape &&
						null != (updateShape = pel as ORMBaseShape))
					{
						updateShape.Invalidate();
					}
				}
			}
		}
		#endregion // Auto-invalidate tracking
		#region DuplicateNameError Activation Helper
		/// <summary>
		/// Activate the Name property in the Properties Window
		/// for the specified element
		/// </summary>
		/// <param name="targetElement">The underlying model element with a name property</param>
		protected void ActivateNameProperty(ModelElement targetElement)
		{
			Store store = Store;
			EditorUtility.ActivatePropertyEditor(
				(store as IORMToolServices).ServiceProvider,
				DomainTypeDescriptor.CreateNamePropertyDescriptor(targetElement),
				false);
		}
		#endregion // DuplicateNameError Activation Helper
		#region Update shapes on ModelError added/removed
		[RuleOn(typeof(ModelHasError))] // AddRule
		private sealed partial class ModelErrorAdded : AddRule
		{
			public sealed override void ElementAdded(ElementAddedEventArgs e)
			{
				ProcessModelErrorChange(e.ModelElement as ModelHasError);
			}
		}
		[RuleOn(typeof(ModelHasError))] // DeletingRule
		private sealed partial class ModelErrorDeleting : DeletingRule
		{
			public sealed override void ElementDeleting(ElementDeletingEventArgs e)
			{
				ModelHasError link = e.ModelElement as ModelHasError;
				if (!link.Model.IsDeleting)
				{
					ProcessModelErrorChange(link);
				}
			}
		}
		private static void ProcessModelErrorChange(ModelHasError errorLink)
		{
			ModelError error = errorLink.Error;
			// Give the error itself a change to have an indirect owner.
			// A ModelError can own itself.
			InvalidateIndirectErrorOwnerDisplay(error, null);
			DomainClassInfo classInfo = error.GetDomainClass();
			ReadOnlyCollection<DomainRoleInfo> playedMetaRoles = classInfo.AllDomainRolesPlayed;
			int playedMetaRoleCount = playedMetaRoles.Count;
			for (int i = 0; i < playedMetaRoleCount; ++i)
			{
				DomainRoleInfo roleInfo = playedMetaRoles[i];
				if (roleInfo.Id != ModelHasError.ErrorDomainRoleId)
				{
					LinkedElementCollection<ModelElement> rolePlayers = roleInfo.GetLinkedElements(error);
					int rolePlayerCount = rolePlayers.Count;
					for (int j = 0; j < rolePlayerCount; ++j)
					{
						ModelElement rolePlayer = rolePlayers[j];
						InvalidateErrorOwnerDisplay(rolePlayer);
						InvalidateIndirectErrorOwnerDisplay(rolePlayer, null);
					}
				}
			}
		}
		private static void InvalidateErrorOwnerDisplay(ModelElement element)
		{
			if (!element.IsDeleting)
			{
				LinkedElementCollection<PresentationElement> pels = PresentationViewsSubject.GetPresentation(element);
				int pelCount = pels.Count;
				for (int i = 0; i < pelCount; ++i)
				{
					ORMBaseShape shape = pels[i] as ORMBaseShape;
					if (shape != null && !shape.IsDeleting)
					{
						shape.InvalidateRequired();
					}
				}
			}
		}
		private static void InvalidateIndirectErrorOwnerDisplay(ModelElement element, DomainDataDirectory domainDataDirectory)
		{
			IHasIndirectModelErrorOwner indirectOwner = element as IHasIndirectModelErrorOwner;
			if (indirectOwner != null)
			{
				Guid[] metaRoles = indirectOwner.GetIndirectModelErrorOwnerLinkRoles();
				int roleCount;
				if (metaRoles != null &&
					0 != (roleCount = metaRoles.Length))
				{
					if (domainDataDirectory == null)
					{
						domainDataDirectory = element.Store.DomainDataDirectory;
					}
					for (int i = 0; i < roleCount; ++i)
					{
						Debug.Assert(metaRoles[i] != Guid.Empty);
						DomainRoleInfo metaRole = domainDataDirectory.FindDomainRole(metaRoles[i]);
						if (metaRole != null)
						{
							LinkedElementCollection<ModelElement> counterparts = metaRole.GetLinkedElements(element);
							int counterpartCount = counterparts.Count;
							for (int j = 0; j < counterpartCount; ++j)
							{
								ModelElement counterpart = counterparts[j];
								if (counterpart is IModelErrorOwner)
								{
									InvalidateErrorOwnerDisplay(counterpart);
								}
								InvalidateIndirectErrorOwnerDisplay(counterpart, domainDataDirectory);
							}
						}
					}
				}
			}
			ElementLink elementLink;
			IElementLinkRoleHasIndirectModelErrorOwner indirectLinkRoleOwner;
			if (null != (indirectLinkRoleOwner = element as IElementLinkRoleHasIndirectModelErrorOwner) &&
				null != (elementLink = element as ElementLink))
			{
				Guid[] metaRoles = indirectLinkRoleOwner.GetIndirectModelErrorOwnerElementLinkRoles();
				int roleCount;
				if (metaRoles != null &&
					0 != (roleCount = metaRoles.Length))
				{
					if (domainDataDirectory == null)
					{
						domainDataDirectory = element.Store.DomainDataDirectory;
					}
					for (int i = 0; i < roleCount; ++i)
					{
						Debug.Assert(metaRoles[i] != Guid.Empty);
						DomainRoleInfo metaRole = domainDataDirectory.FindDomainRole(metaRoles[i]);
						if (metaRole != null)
						{
							ModelElement rolePlayer = metaRole.GetRolePlayer(elementLink);
							if (rolePlayer is IModelErrorOwner)
							{
								InvalidateErrorOwnerDisplay(rolePlayer);
							}
							InvalidateIndirectErrorOwnerDisplay(rolePlayer, domainDataDirectory);
						}
					}
				}
			}
		}
		#endregion // Update shapes on ModelError added/removed
		#region Relative shape anchoring
		/// <summary>
		/// Helper function to keep all relative shapes equidistant from
		/// a shape when the shape bounds change.
		/// </summary>
		/// <param name="e">ElementPropertyChangedEventArgs</param>
		protected static void MaintainRelativeShapeOffsetsForBoundsChange(ElementPropertyChangedEventArgs e)
		{
			Guid attributeId = e.DomainProperty.Id;
			if (attributeId == ORMBaseShape.AbsoluteBoundsDomainPropertyId)
			{
				ORMBaseShape parentShape = e.ModelElement as ORMBaseShape;
				RectangleD oldBounds = (RectangleD)e.OldValue;
				if (oldBounds.IsEmpty ||
					e.ModelElement.Store.TransactionManager.CurrentTransaction.TopLevelTransaction.Context.ContextInfo.ContainsKey(ORMBaseShape.PlaceAllChildShapes))
				{
					// Initializing, let normal placement win
					return;
				}
				RectangleD newBounds = (RectangleD)e.NewValue;
				SizeD oldSize = oldBounds.Size;
				SizeD newSize = newBounds.Size;
				double xChange = newSize.Width - oldSize.Width;
				double yChange = newSize.Height - oldSize.Height;
				bool checkX = !VGConstants.FuzzZero(xChange, VGConstants.FuzzDistance);
				bool checkY = !VGConstants.FuzzZero(yChange, VGConstants.FuzzDistance);
				if (checkX || checkY)
				{
					LinkedElementCollection<ShapeElement> childShapes = parentShape.RelativeChildShapes;
					int childCount = childShapes.Count;
					if (childCount != 0)
					{
						for (int i = 0; i < childCount; ++i)
						{
							bool changeBounds = false;
							PointD change = default(PointD);
							NodeShape childShape = childShapes[i] as NodeShape;
							if (childShape != null)
							{
								RectangleD childBounds = childShape.AbsoluteBounds;
								if (checkX)
								{
									double newRight = newBounds.Right - xChange;
									double childLeft = childBounds.Left;
									if (childLeft > newRight || // Completely to the right
										(childBounds.Right > newRight && childLeft > newBounds.Left)) // Straddles right edge
									{
										change.X = xChange;
										changeBounds = true;
									}
								}
								if (checkY)
								{
									double newBottom = newBounds.Bottom - yChange;
									double childTop = childBounds.Top;
									if (childTop > newBottom || // Completely below
										(childBounds.Bottom > newBottom && childTop > newBounds.Top)) // Straddles bottom edge
									{
										change.Y = yChange;
										changeBounds = true;
									}
								}
								if (changeBounds)
								{
									childBounds.Offset(change);
									childShape.AbsoluteBounds = childBounds;
								}
							}
						}
					}
				}
			}
		}
		#endregion // Relative shape anchoring
		#region Luminosity Modification
		/// <summary>
		/// Redirect all luminosity modification to the ORMDiagram.ModifyLuminosity
		/// algorithm
		/// </summary>
		/// <param name="currentLuminosity">The luminosity to modify</param>
		/// <param name="view">The view containing this item</param>
		/// <returns>Modified luminosity value</returns>
		protected override int ModifyLuminosity(int currentLuminosity, DiagramClientView view)
		{
			if (view.HighlightedShapes.Contains(new DiagramItem(this)))
			{
				return ORMDiagram.ModifyLuminosity(currentLuminosity);
			}
			return currentLuminosity;
		}
		#endregion // Luminosity Modification
	}
}
