#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
* Copyright � Matthew Curland. All rights reserved.                        *
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Neumont.Tools.Modeling.Design;
using Neumont.Tools.ORM;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.Modeling;
using Neumont.Tools.Modeling.Diagrams;
using Neumont.Tools.ORM.Shell;
namespace Neumont.Tools.ORM.ShapeModel
{
	public partial class ObjectTypeShape : IModelErrorActivation
	{
		#region Member Variables
		private static AutoSizeTextField myTextShapeField;
		private static AutoSizeTextField myReferenceModeShapeField;
		private const double HorizontalMargin = 0.060;
		private const double VerticalMargin = 0.050;
		private static readonly StyleSetResourceId DashedShapeOutlinePen = new StyleSetResourceId("Neumont", "DashedShapeOutlinePen");
		#endregion // Member Variables
		#region Customize appearance
		/// <summary>
		/// Show a shadow if this <see cref="ObjectTypeShape"/> represents an <see cref="ObjectType"/> that appears
		/// in more than one location.
		/// </summary>
		public override bool HasShadow
		{
			get
			{
				return ORMBaseShape.ElementHasMultiplePresentations(this);
			}
		}
		/// <summary>
		/// Support automatic appearance updating when multiple presentations are present.
		/// </summary>
		public override bool DisplaysMultiplePresentations
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// Switch between the standard solid pen and
		/// a dashed pen depending on the objectification settings
		/// </summary>
		public override StyleSetResourceId OutlinePenId
		{
			get
			{
				ObjectType associatedObjectType = ModelElement as ObjectType;
				return (associatedObjectType != null && associatedObjectType.IsValueType) ? DashedShapeOutlinePen : DiagramPens.ShapeOutline;
			}
		}
		/// <summary>
		/// Add a dashed pen to the class resource set
		/// </summary>
		/// <param name="classStyleSet">Shared class styleset instance</param>
		protected override void InitializeResources(StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);
			PenSettings settings = new PenSettings();
			settings.Color = Color.DarkBlue;
			settings.Width = 1.2f / 72f; // 1.2 point
			classStyleSet.OverridePen(DiagramPens.ShapeOutline, settings);
			settings.DashStyle = DashStyle.Dash;
			classStyleSet.AddPen(DashedShapeOutlinePen, DiagramPens.ShapeOutline, settings);
		}
		/// <summary>
		/// Get the shape of an object type. Controllable via the ORM Designer
		/// tab on the options page.
		/// </summary>
		public override ShapeGeometry ShapeGeometry
		{
			get
			{
				ShapeGeometry useShape;
				switch (Shell.OptionsPage.CurrentObjectTypeDisplayShape)
				{
					case Shell.ObjectTypeDisplayShape.Ellipse:
						useShape = CustomFoldEllipseShapeGeometry.ShapeGeometry;
						break;
					case Shell.ObjectTypeDisplayShape.HardRectangle:
						useShape = CustomFoldRectangleShapeGeometry.ShapeGeometry;
						break;
					case Shell.ObjectTypeDisplayShape.SoftRectangle:
					default:
						useShape = CustomFoldRoundedRectangleShapeGeometry.ShapeGeometry;
						break;
				}
				return useShape;
			}
		}
		/// <summary>
		/// Size to ContentSize plus some margin padding.
		/// </summary>
		public override void AutoResize()
		{
			SizeD contentSize = ContentSize;
			if (!contentSize.IsEmpty)
			{
				contentSize.Width += HorizontalMargin + HorizontalMargin;
				contentSize.Height += VerticalMargin + VerticalMargin;
				Size = contentSize;
			}
		}
		/// <summary>
		/// Set the content size to the text size
		/// </summary>
		protected override SizeD ContentSize
		{
			get
			{
				SizeD retVal = SizeD.Empty;
				TextField textShape = TextShapeField;
				TextField referenceShape = ReferenceModeShapeField;
				if (textShape != null)
				{
					SizeD textSize = textShape.GetBounds(this).Size;
					SizeD referenceSize = referenceShape.GetBounds(this).Size;
					retVal.Width = (textSize.Width > referenceSize.Width) ? textSize.Width : referenceSize.Width;
					retVal.Height = textSize.Height + referenceSize.Height;
				}
				return retVal;
			}
		}
		/// <summary>
		/// Retrieve the (singleton) shape field for the text
		/// </summary>
		protected static TextField TextShapeField
		{
			get
			{
				return myTextShapeField;
			}
		}
		/// <summary>
		/// Retrieve the (singleton) shape field for the reference mode text
		/// </summary>
		protected static TextField ReferenceModeShapeField
		{
			get
			{
				return myReferenceModeShapeField;
			}
		}
		/// <summary>
		/// Creates and adds shape fields to this shape type. Called once per type.
		/// </summary>
		/// <param name="shapeFields">The shape fields collection for this shape type.</param>
		protected override void InitializeShapeFields(IList<ShapeField> shapeFields)
		{
			base.InitializeShapeFields(shapeFields);

			// Initialize field
			AutoSizeTextField field = CreateObjectNameTextField("ObjectNameTextField");
			field.DrawBorder = false;
			field.FillBackground = false;
			field.DefaultTextBrushId = DiagramBrushes.ShapeTitleText;
			field.DefaultPenId = DiagramPens.ShapeOutline;
			field.DefaultFontId = DiagramFonts.ShapeTitle;
			field.DefaultFocusable = true;
			field.DefaultText = "Object";

			StringFormat fieldFormat = new StringFormat(StringFormatFlags.NoClip);
			fieldFormat.Alignment = StringAlignment.Center;
			field.DefaultStringFormat = fieldFormat;
			field.AssociateValueWith(Store, ORMNamedElement.NameDomainPropertyId);

			// Initialize reference mode field
			AutoSizeTextField referenceModeField = CreateReferenceModeTextField("RefModeTextField");
			referenceModeField.DrawBorder = false;
			referenceModeField.DefaultTextBrushId = DiagramBrushes.ShapeTitleText;
			referenceModeField.DefaultPenId = DiagramPens.ShapeOutline;
			referenceModeField.DefaultFontId = DiagramFonts.ShapeTitle;
			referenceModeField.DefaultText = string.Empty;
			referenceModeField.DefaultStringFormat = fieldFormat;
			// Note that the reference mode field is associated with the ReferenceModeDecoratedString
			// property, not ReferenceModeDisplay. The field will only activate for editing
			// if it is a string property.
			referenceModeField.AssociateValueWith(Store, ObjectType.ReferenceModeDecoratedStringDomainPropertyId);

			// Add all shapes before modifying anchoring behavior
			shapeFields.Add(field);
			shapeFields.Add(referenceModeField);

			// Modify field anchoring behavior
			AnchoringBehavior anchor = field.AnchoringBehavior;
			anchor.SetTopAnchor(AnchoringBehavior.Edge.Top, VerticalMargin);
			anchor.CenterHorizontally();

			// Modify reference mode field anchoring behavior
			AnchoringBehavior referenceModeAnchor = referenceModeField.AnchoringBehavior;
			referenceModeAnchor.CenterHorizontally();
			referenceModeAnchor.SetTopAnchor(field, AnchoringBehavior.Edge.Bottom, 0);

			Debug.Assert(myTextShapeField == null); // Only called once
			myTextShapeField = field;
			Debug.Assert(myReferenceModeShapeField == null); // Only called once
			myReferenceModeShapeField = referenceModeField;
		}

		/// <summary>
		/// Add a shape element linked to this parent to display the value range
		/// </summary>
		/// <param name="element">ModelElement of type ObjectType</param>
		/// <returns>true</returns>
		protected override bool ShouldAddShapeForElement(ModelElement element)
		{
#if DEBUG
			//RoleValueConstraints added should be for a role opposite of
			//this object's role player. The test below allows a RoleValueConstraint
			//to be added on the opposite role even if that opposite role is played
			//by the same ObjectType as the RoleValueConstraint's role (i.e. allows
			//for ring constraints).
			bool isRoleValueRangeDefn = false;
			RoleValueConstraint roleDefn;
			if (null != (roleDefn = element as RoleValueConstraint))
			{
				Role roleInDefn = roleDefn.Role;
				FactType factType = roleInDefn.FactType;
				foreach (RoleBase rBase in factType.RoleCollection)
				{
					Role r = rBase.Role;
					if (roleInDefn != r)
					{
						if (r.RolePlayer == AssociatedObjectType)
						{
							isRoleValueRangeDefn = true;
							break;
						}
					}
				}
			}
			Debug.Assert(
				(element is ValueTypeValueConstraint && ((ValueTypeValueConstraint)element).ValueType == AssociatedObjectType) ||
				isRoleValueRangeDefn
			);
#endif // DEBUG
			return true;
		}
		/// <summary>
		/// Indicate that we support tool tips. Used for showing
		/// definition information.
		/// </summary>
		public override bool HasToolTip
		{
			get
			{
				return OptionsPage.CurrentDisplayDefinitionTooltips;
			}
		}
		/// <summary>
		/// Show a tooltip containing the element definition text.
		/// </summary>
		public override string GetToolTipText(DiagramItem item)
		{
			string retVal = null;
			// Show for all shapes and fields in item
			retVal = AssociatedObjectType.DefinitionText;
			if (retVal.Length == 0)
			{
				retVal = null;
			}
			return retVal;
		}
		#endregion // Customize appearance
		#region ObjectTypeShape specific
		/// <summary>
		/// Get the ObjectTypeType associated with this shape
		/// </summary>
		public ObjectType AssociatedObjectType
		{
			get
			{
				return ModelElement as ObjectType;
			}
		}
		/// <summary>
		/// Makes a shape a relative child element.
		/// </summary>
		/// <param name="childShape">The ShapeElement to get the ReleationshipType for.</param>
		/// <returns>RelationshipType.Relative</returns>
		protected override RelationshipType ChooseRelationship(ShapeElement childShape)
		{
			Debug.Assert(childShape is ValueConstraintShape);
			return RelationshipType.Relative;
		}

		#endregion // ObjectTypeShape specific
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
		#region IModelErrorActivation Implementation
		/// <summary>
		/// Implements IModelErrorActivation.ActivateModelError for DataTypeNotSpecifiedError
		/// </summary>
		protected bool ActivateModelError(ModelError error)
		{
			DataTypeNotSpecifiedError dataTypeError;
			EntityTypeRequiresReferenceSchemeError requiresReferenceSchemeError;
			ObjectTypeDuplicateNameError duplicateName;
			PopulationMandatoryError populationMandatory;
			bool retVal = true;
			if (null != (dataTypeError = error as DataTypeNotSpecifiedError))
			{
				Store store = Store;
				ObjectType valueType = dataTypeError.ValueTypeHasDataType.ValueType;
				EditorUtility.ActivatePropertyEditor(
					(store as IORMToolServices).ServiceProvider,
					DomainTypeDescriptor.CreatePropertyDescriptor(valueType, ObjectType.DataTypeDisplayDomainPropertyId),
					true);
			}
			else if (null != (requiresReferenceSchemeError = error as EntityTypeRequiresReferenceSchemeError))
			{
				Store store = Store;
				EditorUtility.ActivatePropertyEditor(
					(store as IORMToolServices).ServiceProvider,
					DomainTypeDescriptor.CreatePropertyDescriptor(requiresReferenceSchemeError.ObjectType, ObjectType.ReferenceModeDisplayDomainPropertyId),
					true);
			}
			else if (null != (duplicateName = error as ObjectTypeDuplicateNameError))
			{
				ActivateNameProperty(duplicateName.ObjectTypeCollection[0]);
			}
			else if (null != (populationMandatory = error as PopulationMandatoryError))
			{
				ORMDesignerPackage.SamplePopulationEditorWindow.AutoCorrectMandatoryError(populationMandatory);
			}
			else if (error is TooFewEntityTypeRoleInstancesError)
			{
				retVal = ORMDesignerPackage.SamplePopulationEditorWindow.ActivateModelError(error);
			}
			else
			{
				retVal = false;
			}
			return retVal;
		}
		bool IModelErrorActivation.ActivateModelError(ModelError error)
		{
			return ActivateModelError(error);
		}
		#endregion // IModelErrorActivation Implementation
		#region Shape display update rules
		/// <summary>
		/// ChangeRule: typeof(Neumont.Tools.ORM.ObjectModel.ObjectType), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// Iif primaryidentifier for a Entity type is 
		/// removed and ref mode is not expanded, then resize the entity type
		/// </summary>
		private static void ObjectTypeChangeRule(ElementPropertyChangedEventArgs e)
		{
			Guid attributeGuid = e.DomainProperty.Id;
			ObjectType changeType = null;
			if (attributeGuid == ORMNamedElement.NameDomainPropertyId)
			{
				// Figure out if we need to resize related object shapes. This happens
				// when we've renamed a value type that is bound to the reference scheme
				// of an entity type.
				changeType = e.ModelElement as ObjectType;

				if (changeType.IsValueType)
				{
					LinkedElementCollection<Role> playedRoles = changeType.PlayedRoleCollection;
					int roleCount = playedRoles.Count;
					for (int i = 0; i < roleCount; ++i)
					{
						Role currentRole = playedRoles[i];
						LinkedElementCollection<ConstraintRoleSequence> roleConstraints = currentRole.ConstraintRoleSequenceCollection;
						int constraintCount = roleConstraints.Count;
						for (int j = 0; j < constraintCount; ++j)
						{
							ConstraintRoleSequence currentConstraintRoleSequence = roleConstraints[j];
							IConstraint associatedConstraint = currentConstraintRoleSequence.Constraint;
							if (associatedConstraint.ConstraintType == ConstraintType.InternalUniqueness)
							{
								ResizeAssociatedShapes(associatedConstraint.PreferredIdentifierFor);
							}
						}
					}
				}
			}
			else if (attributeGuid == ObjectType.IsIndependentDomainPropertyId)
			{
				changeType = e.ModelElement as ObjectType;
			}
			if (changeType != null)
			{
				ResizeAssociatedShapes(changeType);
			}
		}
		#region ObjectTypeShapeChangeRule
		/// <summary>
		/// ChangeRule: typeof(ObjectTypeShape), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// Keep relative child elements a fixed distance away from the object type shape
		/// when the shape changes.
		/// </summary>
		private static void ObjectTypeShapeChangeRule(ElementPropertyChangedEventArgs e)
		{
			MaintainRelativeShapeOffsetsForBoundsChange(e);
		}
		#endregion // ObjectTypeShapeChangeRule
		/// <summary>
		/// DeleteRule: typeof(Neumont.Tools.ORM.ObjectModel.EntityTypeHasPreferredIdentifier), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// </summary>
		private static void PreferredIdentifierDeleteRule(ElementDeletedEventArgs e)
		{
			ProcessPreferredIdentifierDelete(e.ModelElement as EntityTypeHasPreferredIdentifier, null);
		}
		/// <summary>
		/// Rule helper method
		/// </summary>
		private static void ProcessPreferredIdentifierDelete(EntityTypeHasPreferredIdentifier link, ObjectType objectType)
		{
			if (objectType == null)
			{
				objectType = link.PreferredIdentifierFor;
			}
			if (!objectType.IsDeleted)
			{
				ResizeAssociatedShapes(objectType);
			}
		}
		/// <summary>
		/// RolePlayerChangeRule: typeof(Neumont.Tools.ORM.ObjectModel.EntityTypeHasPreferredIdentifier), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// </summary>
		private static void PreferredIdentifierRolePlayerChangeRuleForResizeRule(RolePlayerChangedEventArgs e)
		{
			ObjectType oldObjectType = null;
			if (e.DomainRole.Id == EntityTypeHasPreferredIdentifier.PreferredIdentifierForDomainRoleId)
			{
				oldObjectType = (ObjectType)e.OldRolePlayer;
			}
			ProcessPreferredIdentifierDelete(e.ElementLink as EntityTypeHasPreferredIdentifier, oldObjectType);
		}
		/// <summary>
		/// Resize shapes for the provided object type
		/// </summary>
		/// <param name="objectType">The associated model element</param>
		private static void ResizeAssociatedShapes(ObjectType objectType)
		{
			if (objectType != null && !objectType.IsDeleted)
			{
				LinkedElementCollection<PresentationElement> pels = PresentationViewsSubject.GetPresentation(objectType);
				int pelCount = pels.Count;
				for (int i = 0; i < pelCount; ++i)
				{
					// ORMBaseShape Picks up ObjectTypeShape, ObjectifiedFactTypeNameShape
					ORMBaseShape objectShape = pels[i] as ORMBaseShape;
					if (null != objectShape)
					{
						SizeD oldSize = objectShape.Size;
						objectShape.AutoResize();
						if (oldSize == objectShape.Size)
						{
							objectShape.InvalidateRequired(true);
						}
					}
				}
			}
		}
		/// <summary>
		/// Ensure that the ExpandRefMode property is set. Called when a
		/// reference mode pattern is added to the model manually on
		/// shapes that are already expanded.
		/// </summary>
		/// <param name="constraint">The constraint providing a preferred identifier</param>
		/// <param name="preferredIdentifierFor">The ObjectType from the core model</param>
		private static void EnsureRefModeExpanded(UniquenessConstraint constraint, ObjectType preferredIdentifierFor)
		{
			Debug.Assert(constraint != null); // Check before call
			Debug.Assert(preferredIdentifierFor != null); // Check before call
			//Get the object that represents the item with the preferred identifier. 
			foreach (PresentationElement pel in PresentationViewsSubject.GetPresentation(preferredIdentifierFor))
			{
				ObjectTypeShape objectShape;
				ObjectifiedFactTypeNameShape objectifiedShape;
				if (null != (objectShape = pel as ObjectTypeShape))
				{
					//If there is a fact shape and it is visible then we need to 
					//set ExpandRefMode to true, otherwise set it to false.
					FactTypeShape factShape = (objectShape.Diagram as ORMDiagram).FindShapeForElement<FactTypeShape>(constraint.FactTypeCollection[0]);
					bool newValue = factShape != null && factShape.IsVisible;
					if (objectShape.ExpandRefMode != newValue)
					{
						objectShape.ExpandRefMode = newValue;
					}
					else
					{
						SizeD oldSize = objectShape.Size;
						objectShape.AutoResize();
						if (oldSize == objectShape.Size)
						{
							objectShape.InvalidateRequired(true);
						}
					}
				}
				else if (null != (objectifiedShape = pel as ObjectifiedFactTypeNameShape))
				{
					FactTypeShape factShape = (objectifiedShape.Diagram as ORMDiagram).FindShapeForElement<FactTypeShape>(constraint.FactTypeCollection[0]);
					bool newValue = factShape != null && factShape.IsVisible;
					if (objectifiedShape.ExpandRefMode != newValue)
					{
						objectifiedShape.ExpandRefMode = newValue;
					}
					else
					{
						SizeD oldSize = objectifiedShape.Size;
						objectifiedShape.AutoResize();
						if (oldSize == objectifiedShape.Size)
						{
							objectifiedShape.InvalidateRequired(true);
						}
					}
				}
			}
		}
		/// <summary>
		/// AddRule: typeof(Neumont.Tools.ORM.ObjectModel.EntityTypeHasPreferredIdentifier)
		/// </summary>
		private static void PreferredIdentifierAddedRule(ElementAddedEventArgs e)
		{
			ProcessPreferredIdentifierAdded(e.ModelElement as EntityTypeHasPreferredIdentifier);
		}
		/// <summary>
		/// Rule helper method
		/// </summary>
		private static void ProcessPreferredIdentifierAdded(EntityTypeHasPreferredIdentifier link)
		{
			ObjectType rolePlayer;
			LinkedElementCollection<Role> roles;
			UniquenessConstraint constraint = link.PreferredIdentifier;
			if (constraint.IsInternal &&
				1 == (roles = constraint.RoleCollection).Count &&
				null != (rolePlayer = roles[0].RolePlayer) &&
				rolePlayer.IsValueType)
			{
				EnsureRefModeExpanded(constraint, link.PreferredIdentifierFor);
			}
		}
		/// <summary>
		/// RolePlayerChangeRule: typeof(Neumont.Tools.ORM.ObjectModel.EntityTypeHasPreferredIdentifier)
		/// Verify that all preconditions hold for adding a primary
		/// identifier and extend modifiable conditions as needed.
		/// Defers to <see cref="ProcessPreferredIdentifierAdded"/>.
		/// </summary>
		private static void PreferredIdentifierRolePlayerChangeRule(RolePlayerChangedEventArgs e)
		{
			ProcessPreferredIdentifierAdded(e.ElementLink as EntityTypeHasPreferredIdentifier);
		}
		/// <summary>
		/// AddRule: typeof(Neumont.Tools.ORM.ObjectModel.ConstraintRoleSequenceHasRole), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AddShapeRulePriority;
		/// </summary>
		private static void PreferredIdentifierLengthenedRule(ElementAddedEventArgs e)
		{
			ConstraintRoleSequenceHasRole link = e.ModelElement as ConstraintRoleSequenceHasRole;
			UniquenessConstraint constraint;
			ObjectType preferredIdentifierFor;
			LinkedElementCollection<Role> constraintRoles;
			if (null != (constraint = link.ConstraintRoleSequence as UniquenessConstraint) &&
				!constraint.IsDeleted &&
				null != (preferredIdentifierFor = constraint.PreferredIdentifierFor) &&
				null != preferredIdentifierFor.Objectification &&
				(constraintRoles = constraint.RoleCollection).Count != 1)
			{
				ResizeAssociatedShapes(preferredIdentifierFor);
			}
		}
		/// <summary>
		/// DeleteRule: typeof(Neumont.Tools.ORM.ObjectModel.ConstraintRoleSequenceHasRole), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AddShapeRulePriority;
		/// </summary>
		private static void PreferredIdentifierShortenedRule(ElementDeletedEventArgs e)
		{
			ConstraintRoleSequenceHasRole link = e.ModelElement as ConstraintRoleSequenceHasRole;
			UniquenessConstraint constraint;
			ObjectType preferredIdentifierFor;
			LinkedElementCollection<Role> constraintRoles;
			if (null != (constraint = link.ConstraintRoleSequence as UniquenessConstraint) &&
				!constraint.IsDeleted &&
				null != (preferredIdentifierFor = constraint.PreferredIdentifierFor) &&
				null != preferredIdentifierFor.Objectification &&
				(constraintRoles = constraint.RoleCollection).Count == 1)
			{
				ResizeAssociatedShapes(preferredIdentifierFor);
			}
		}

		// Note that we do not need a RolePlayerChangeRule for ValueTypeHasDataType as
		// this will not change whether a given type is used as a refmode or not

		/// <summary>
		/// AddRule: typeof(Neumont.Tools.ORM.ObjectModel.ValueTypeHasDataType)
		/// An object type can be a preferred identifier. Changing it to a value
		/// type makes it a refmode. Make sure that the ExpandRefMode property is in sync.
		/// </summary>
		private static void DataTypeAddedRule(ElementAddedEventArgs e)
		{
			ValueTypeHasDataType dataTypeLink = e.ModelElement as ValueTypeHasDataType;
			LinkedElementCollection<Role> playedRoles = dataTypeLink.ValueType.PlayedRoleCollection;
			int playedRolesCount = playedRoles.Count;
			for (int i = 0; i < playedRolesCount; ++i)
			{
				LinkedElementCollection<ConstraintRoleSequence> sequences = playedRoles[i].ConstraintRoleSequenceCollection;
				int constraintsCount = sequences.Count;
				for (int j = 0; j < constraintsCount; ++j)
				{
					UniquenessConstraint iuc = sequences[j] as UniquenessConstraint;
					if (iuc != null && iuc.IsInternal)
					{
						ObjectType preferredFor = iuc.PreferredIdentifierFor;
						if (preferredFor != null)
						{
							EnsureRefModeExpanded(iuc, preferredFor);
						}
					}
				}
			}
		}
		/// <summary>
		/// DeleteRule: typeof(Neumont.Tools.ORM.ObjectModel.ValueTypeHasDataType), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AddShapeRulePriority;
		/// An object type can be the role player for a single-role preferred identifier. Changing it to a value
		/// type makes it a refmode. Make sure that the ExpandRefMode property is in sync.
		/// </summary>
		private static void DataTypeDeleteRule(ElementDeletedEventArgs e)
		{
			ValueTypeHasDataType dataTypeLink = e.ModelElement as ValueTypeHasDataType;
			ObjectType valueType = dataTypeLink.ValueType;
			if (!valueType.IsDeleted)
			{
				LinkedElementCollection<Role> playedRoles = valueType.PlayedRoleCollection;
				int playedRolesCount = playedRoles.Count;
				for (int i = 0; i < playedRolesCount; ++i)
				{
					LinkedElementCollection<ConstraintRoleSequence> sequences = playedRoles[i].ConstraintRoleSequenceCollection;
					int constraintsCount = sequences.Count;
					for (int j = 0; j < constraintsCount; ++j)
					{
						UniquenessConstraint iuc = sequences[j] as UniquenessConstraint;
						if (iuc != null && iuc.IsInternal)
						{
							ObjectType preferredFor = iuc.PreferredIdentifierFor;
							if (preferredFor != null)
							{
								ResizeAssociatedShapes(preferredFor);
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// AddRule: typeof(Neumont.Tools.ORM.ObjectModel.ObjectTypePlaysRole)
		/// A preferred identifier internal uniqueness constraint can be attached to a role with no
		/// role player. Attaching a role player will match the reference mode pattern, which then needs
		/// to ensure that the ExpandRefMode property is correct. Also, resize shapes if the first subtype role
		/// is added where this is an existing derivation rule.
		/// </summary>
		private static void RolePlayerAddedRule(ElementAddedEventArgs e)
		{
			ObjectTypePlaysRole link = e.ModelElement as ObjectTypePlaysRole;
			ObjectType rolePlayer = link.RolePlayer;
			if (rolePlayer.IsValueType)
			{
				LinkedElementCollection<ConstraintRoleSequence> sequences = link.PlayedRole.ConstraintRoleSequenceCollection;
				int constraintsCount = sequences.Count;
				for (int i = 0; i < constraintsCount; ++i)
				{
					UniquenessConstraint iuc = sequences[i] as UniquenessConstraint;
					if (iuc != null && iuc.IsInternal)
					{
						ObjectType preferredFor = iuc.PreferredIdentifierFor;
						if (preferredFor != null)
						{
							EnsureRefModeExpanded(iuc, preferredFor);
						}
					}
				}
			}
			if (link.PlayedRole is SubtypeMetaRole &&
				null != rolePlayer.DerivationRule)
			{
				int subtypeCount = 0;
				ObjectType.WalkSupertypeRelationships(
					rolePlayer,
					delegate(SubtypeFact subtypeFact, ObjectType type, int depth)
					{
						++subtypeCount;
						return (subtypeCount == 1) ? ObjectTypeVisitorResult.SkipChildren : ObjectTypeVisitorResult.Stop;
					});
				if (subtypeCount == 1)
				{
					ResizeAssociatedShapes(rolePlayer);
				}
			}
		}
		/// <summary>
		/// DeleteRule: typeof(Neumont.Tools.ORM.ObjectModel.ObjectTypePlaysRole), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.ResizeParentRulePriority;
		/// Deleting a value type that participates in a refmode pattern does not remove the
		/// preferred identifier, so there is no notification to the shape that the refmode is gone.
		/// This forces the opposite ObjectTypeShape to resize in case it lost its refmode.
		/// Also, remove the derivation decorator if the last Subtype is removed.
		/// </summary>
		private static void RolePlayerDeleteRule(ElementDeletedEventArgs e)
		{
			ObjectTypePlaysRole link = (ObjectTypePlaysRole)e.ModelElement;
			Role role = link.PlayedRole;
			if (!role.IsDeleted)
			{
				foreach (ConstraintRoleSequence sequence in role.ConstraintRoleSequenceCollection)
				{
					UniquenessConstraint iuc = sequence as UniquenessConstraint;
					if (iuc != null && iuc.IsInternal)
					{
						ResizeAssociatedShapes(iuc.PreferredIdentifierFor);
					}
				}
			}
			ObjectType rolePlayer = link.RolePlayer;
			if (!rolePlayer.IsDeleted &&
				role is SubtypeMetaRole &&
				rolePlayer.DerivationRule != null &&
				!rolePlayer.IsSubtype)
			{
				ResizeAssociatedShapes(rolePlayer);
			}
		}
		/// <summary>
		/// RolePlayerChangeRule: typeof(Neumont.Tools.ORM.ObjectModel.ObjectTypePlaysRole)
		/// Changing the identifying role player of a FactType that participates in the refmode
		/// pattern needs to appropriately update the ExpandRefMode property and to resize
		/// shapes for the identified object types.
		/// </summary>
		private static void RolePlayerRolePlayerChangedRule(RolePlayerChangedEventArgs e)
		{
			ObjectTypePlaysRole link = e.ElementLink as ObjectTypePlaysRole;
			if (e.DomainRole.Id == ObjectTypePlaysRole.RolePlayerDomainRoleId)
			{
				bool newPlayerIsValueType = link.RolePlayer.IsValueType;
				bool oldPlayerIsValueType = ((ObjectType)e.OldRolePlayer).IsValueType;
				if ((newPlayerIsValueType || oldPlayerIsValueType) &&
					(newPlayerIsValueType != oldPlayerIsValueType))
				{
					LinkedElementCollection<ConstraintRoleSequence> sequences = link.PlayedRole.ConstraintRoleSequenceCollection;
					int constraintsCount = sequences.Count;
					for (int i = 0; i < constraintsCount; ++i)
					{
						UniquenessConstraint iuc = sequences[i] as UniquenessConstraint;
						if (iuc != null && iuc.IsInternal)
						{
							ObjectType preferredFor = iuc.PreferredIdentifierFor;
							if (preferredFor != null)
							{
								if (newPlayerIsValueType)
								{
									EnsureRefModeExpanded(iuc, preferredFor);
								}
								else
								{
									ResizeAssociatedShapes(preferredFor);
								}
							}
						}
					}
				}
			}
			else
			{
				// This can add an expanded reference mode for an object type associated with the new
				// role player and remove the reference mode for the old player
				if (link.RolePlayer.IsValueType)
				{
					LinkedElementCollection<ConstraintRoleSequence> sequences = link.PlayedRole.ConstraintRoleSequenceCollection;
					int constraintsCount = sequences.Count;
					for (int i = 0; i < constraintsCount; ++i)
					{
						UniquenessConstraint iuc = sequences[i] as UniquenessConstraint;
						if (iuc != null && iuc.IsInternal)
						{
							ObjectType preferredFor = iuc.PreferredIdentifierFor;
							if (preferredFor != null)
							{
								EnsureRefModeExpanded(iuc, preferredFor);
							}
						}
					}
					foreach (ConstraintRoleSequence sequence in ((Role)e.OldRolePlayer).ConstraintRoleSequenceCollection)
					{
						UniquenessConstraint iuc = sequence as UniquenessConstraint;
						if (iuc != null && iuc.IsInternal)
						{
							ResizeAssociatedShapes(iuc.PreferredIdentifierFor);
						}
					}
				}
			}
		}
		/// <summary>
		/// AddRule: typeof(Neumont.Tools.ORM.ObjectModel.SubtypeHasDerivationExpression), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AutoLayoutShapesRulePriority;
		/// Resize associated shapes when a derivation rule is added
		/// </summary>
		private static void SubtypeDerivationAddedRule(ElementAddedEventArgs e)
		{
			ResizeAssociatedShapes(((SubtypeHasDerivationExpression)e.ModelElement).Subtype);
		}
		/// <summary>
		/// DeleteRule: typeof(Neumont.Tools.ORM.ObjectModel.SubtypeHasDerivationExpression), FireTime=TopLevelCommit, Priority=DiagramFixupConstants.AutoLayoutShapesRulePriority;
		/// </summary>
		private static void SubtypeDerivationDeletedRule(ElementDeletedEventArgs e)
		{
			ResizeAssociatedShapes(((SubtypeHasDerivationExpression)e.ModelElement).Subtype);
		}
		#endregion // Shape display update rules
		#region Store Event Handlers
		/// <summary>
		///  Helper function to update the mandatory dot in response to events
		/// </summary>
		private static void UpdateObjectTypeDisplay(ObjectType objectType)
		{
			foreach (PresentationElement pel in PresentationViewsSubject.GetPresentation(objectType))
			{
				ShapeElement shape = pel as ShapeElement;
				if (shape != null)
				{
					shape.Invalidate();
				}
			}
		}
		/// <summary>
		/// Manages <see cref="EventHandler{TEventArgs}"/>s in the <see cref="Store"/> for <see cref="ObjectTypeShape"/>s.
		/// </summary>
		/// <param name="store">The <see cref="Store"/> for which the <see cref="EventHandler{TEventArgs}"/>s should be managed.</param>
		/// <param name="eventManager">The <see cref="ModelingEventManager"/> used to manage the <see cref="EventHandler{TEventArgs}"/>s.</param>
		/// <param name="action">The <see cref="EventHandlerAction"/> that should be taken for the <see cref="EventHandler{TEventArgs}"/>s.</param>
		public static new void ManageEventHandlers(Store store, ModelingEventManager eventManager, EventHandlerAction action)
		{
			DomainDataDirectory dataDirectory = store.DomainDataDirectory;

			DomainRelationshipInfo relInfo = dataDirectory.FindDomainRelationship(ValueTypeHasDataType.DomainClassId);
			eventManager.AddOrRemoveHandler(relInfo, new EventHandler<ElementAddedEventArgs>(DataTypeAddedEvent), action);
			eventManager.AddOrRemoveHandler(relInfo, new EventHandler<ElementDeletedEventArgs>(DataTypeRemovedEvent), action);
		}
		/// <summary>
		/// Update the shape when a data type is added from the ObjecType.
		/// </summary>
		private static void DataTypeAddedEvent(object sender, ElementAddedEventArgs e)
		{
			ValueTypeHasDataType link = e.ModelElement as ValueTypeHasDataType;
			ObjectType ot = link.ValueType;
			if (!ot.IsDeleted)
			{
				UpdateObjectTypeDisplay(ot);
			}
		}
		/// <summary>
		/// Update the shape when a data type is removed from the ObjecType.
		/// </summary>
		private static void DataTypeRemovedEvent(object sender, ElementDeletedEventArgs e)
		{
			ValueTypeHasDataType link = e.ModelElement as ValueTypeHasDataType;
			ObjectType ot = link.ValueType;
			if (!ot.IsDeleted)
			{
				UpdateObjectTypeDisplay(ot);
			}
		}
		#endregion // Store Event Handlers
		#region ReferenceModeTextField class
		/// <summary>
		/// Create the text field used for displaying the reference mode
		/// </summary>
		/// <param name="fieldName">Non-localized name for the field</param>
		/// <returns>ReferenceModeTextField</returns>
		protected virtual ReferenceModeTextField CreateReferenceModeTextField(string fieldName)
		{
			return new ReferenceModeTextField(fieldName);
		}
		/// <summary>
		/// Class to show reference mode
		/// </summary>
		protected class ReferenceModeTextField : AutoSizeTextField
		{
			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="fieldName">Non-localized name for the field</param>
			public ReferenceModeTextField(string fieldName)
				: base(fieldName)
			{
				DefaultFocusable = true;
			}

			/// <summary>
			/// Gets the minimum <see cref="SizeD"/> of this <see cref="ReferenceModeTextField"/>.
			/// </summary>
			/// <param name="parentShape">
			/// The <see cref="ObjectTypeShape"/> that this <see cref="ReferenceModeTextField"/> is associated with.
			/// </param>
			/// <returns>The minimum <see cref="SizeD"/> of this <see cref="ReferenceModeTextField"/>.</returns>
			public override SizeD GetMinimumSize(ShapeElement parentShape)
			{
				if (GetVisible(parentShape))
				{
					return base.GetMinimumSize(parentShape);
				}
				return SizeD.Empty;
			}

			/// <summary>
			/// Returns whether or not the text field is visible.
			/// </summary>
			public override bool GetVisible(ShapeElement parentShape)
			{
				ObjectTypeShape objectTypeShape = parentShape as ObjectTypeShape;
				ObjectType objectType = parentShape.ModelElement as ObjectType;
				return (objectType != null && objectTypeShape != null && !objectTypeShape.ExpandRefMode && !objectType.IsValueType);
			}

			/// <summary>
			/// Overrides the display text to add parenthesis
			/// </summary>
			public override string GetDisplayText(ShapeElement parentShape)
			{
				ObjectType objectType = parentShape.ModelElement as ObjectType;
				if (objectType != null && objectType.HasReferenceMode)
				{
					return string.Format(CultureInfo.InvariantCulture, ResourceStrings.ObjectTypeShapeReferenceModeFormatString, base.GetDisplayText(parentShape));
				}
				return base.GetDisplayText(parentShape);
			}
		}
		#endregion // ReferenceModeTextField class
		#region ObjectNameTextField class
		/// <summary>
		/// Create the text field used for displaying the object name
		/// </summary>
		/// <param name="fieldName">Non-localized name for the field</param>
		/// <returns>ObjectNameTextField</returns>
		protected virtual ObjectNameTextField CreateObjectNameTextField(string fieldName)
		{
			return new ObjectNameTextField(fieldName);
		}
		/// <summary>
		/// Class to show a decorated object name
		/// </summary>
		protected class ObjectNameTextField : AutoSizeTextField
		{
			/// <summary>
			/// Create a new ObjectNameTextField
			/// </summary>
			/// <param name="fieldName">Non-localized name for the field</param>
			public ObjectNameTextField(string fieldName)
				: base(fieldName)
			{
			}			/// <summary>
			/// Modify the display text for independent object types.
			/// </summary>
			/// <param name="parentShape">The ShapeElement to get the display text for.</param>
			/// <returns>The text to display.</returns>
			public override string GetDisplayText(ShapeElement parentShape)
			{
				string retVal = base.GetDisplayText(parentShape);
				ObjectType objectType = parentShape.ModelElement as ObjectType;
				if (objectType != null)
				{
					if (objectType.IsIndependent)
					{
						retVal = string.Format(CultureInfo.InvariantCulture, ResourceStrings.ObjectTypeShapeIndependentFormatString, retVal);
					}
					else if (objectType.DerivationRule != null && objectType.IsSubtype) // Note that subtypes are never independent
					{
						retVal = string.Format(CultureInfo.InvariantCulture, ResourceStrings.ObjectTypeShapeDerivedSubtypeFormatString, retVal);
					}
				}
				return retVal;
			}
		}
		#endregion // ObjectNameTextField class
		#region Deserialization Fixup
		/// <summary>
		/// Return a deserialization fixup listener. The listener ensures
		/// the correct size for an <see cref="ObjectTypeShape"/>.
		/// </summary>
		public static IDeserializationFixupListener FixupListener
		{
			get
			{
				return new ObjectTypeShapeFixupListener();
			}
		}
		/// <summary>
		/// A listener to reset the size of an ObjectTypeShape.
		/// </summary>
		private sealed class ObjectTypeShapeFixupListener : DeserializationFixupListener<ObjectTypeShape>
		{
			/// <summary>
			/// Create a new ObjectTypeShapeFixupListener
			/// </summary>
			public ObjectTypeShapeFixupListener()
				: base((int)ORMDeserializationFixupPhase.ValidateStoredPresentationElements)
			{
			}
			/// <summary>
			/// Update the shape size if it is independent or if the derivation status is set.
			/// The size for these strings depends on localized values, which may change when
			/// loading the file on a different machine.
			/// Also, the SubtypeFact fixup rule may dynamically create a derivation rule if
			/// a rule is specified on the SubtypeFact instead of on the Subtype itself.
			/// </summary>
			protected sealed override void ProcessElement(ObjectTypeShape element, Store store, INotifyElementAdded notifyAdded)
			{
				ObjectType objectType;
				if (!element.IsDeleted &&
					null != (objectType = element.AssociatedObjectType) &&
					(objectType.IsIndependent ||
					(objectType.DerivationRule != null && objectType.IsSubtype)))
				{
					// Note that technically the size may be wrong if a derivation rule or an independent
					// setting has been removed. However, in this case, the length will be a little too long,
					// which is harmless. This fixes the situation where it is too short.
					element.AutoResize();
				}
			}
		}
		#endregion // Deserialization Fixup
	}
}
