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
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
namespace Neumont.Tools.ORM.OIALModel
{
	/// <summary>
	/// DomainModel OIALDomainModel
	/// Extension rules and elements used to perform dynamic object absorption. Used by
	/// more compact views on the ORM model.
	/// </summary>
	[DslModeling::ExtendsDomainModel("3EAE649F-E654-4D04-8289-C25D2C0322D8"/*Neumont.Tools.ORM.ObjectModel.ORMCoreDomainModel*/)]
	[DslDesign::DisplayNameResource("Neumont.Tools.ORM.OIALModel.OIALDomainModel.DisplayName", typeof(global::Neumont.Tools.ORM.OIALModel.OIALDomainModel), "Neumont.Tools.ORM.OIALModel.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("Neumont.Tools.ORM.OIALModel.OIALDomainModel.Description", typeof(global::Neumont.Tools.ORM.OIALModel.OIALDomainModel), "Neumont.Tools.ORM.OIALModel.GeneratedCode.DomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("cd96aa55-fcbc-47d0-93f8-30d3dacc5ff7")]
	public partial class OIALDomainModel : DslModeling::DomainModel
	{
		#region Constructor, domain model Id
	
		/// <summary>
		/// OIALDomainModel domain model Id.
		/// </summary>
		public static readonly global::System.Guid DomainModelId = new global::System.Guid(0xcd96aa55, 0xfcbc, 0x47d0, 0x93, 0xf8, 0x30, 0xd3, 0xda, 0xcc, 0x5f, 0xf7);
	
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="store">Store containing the domain model.</param>
		public OIALDomainModel(DslModeling::Store store)
			: base(store, DomainModelId)
		{
		}
		
		#endregion
		#region Domain model reflection
			
		/// <summary>
		/// Gets the list of generated domain model types (classes, rules, relationships).
		/// </summary>
		/// <returns>List of types.</returns>
		protected sealed override global::System.Type[] GetGeneratedDomainModelTypes()
		{
			return new global::System.Type[]
			{
				typeof(OIALNamedElement),
				typeof(OIALModel),
				typeof(ConceptType),
				typeof(InformationTypeFormat),
				typeof(ChildSequence),
				typeof(Constraint),
				typeof(SingleChildConstraint),
				typeof(ChildSequenceConstraint),
				typeof(SingleChildUniquenessConstraint),
				typeof(SingleChildSequenceConstraint),
				typeof(MultiChildSequenceConstraint),
				typeof(SingleChildFrequencyConstraint),
				typeof(ValueConstraint),
				typeof(ChildSequenceFrequencyConstraint),
				typeof(RingConstraint),
				typeof(DisjunctiveMandatoryConstraint),
				typeof(ChildSequenceUniquenessConstraint),
				typeof(MinTwoChildrenChildSequence),
				typeof(SubsetConstraint),
				typeof(TwoOrMoreChildSequenceConstraint),
				typeof(ExclusionConstraint),
				typeof(EqualityConstraint),
				typeof(OIALModelHasORMModel),
				typeof(OIALModelHasConceptType),
				typeof(ConceptTypeChild),
				typeof(ConceptTypeAbsorbedConceptType),
				typeof(InformationType),
				typeof(OIALHasInformationTypeFormat),
				typeof(ConceptTypeRef),
				typeof(InformationTypeFormatHasObjectType),
				typeof(ConceptTypeHasObjectType),
				typeof(ChildSequenceConstraintHasChildSequence),
				typeof(SingleChildSequenceConstraintHasMinTwoChildrenChildSequence),
				typeof(SubsetConstraintHasSubChildSequence),
				typeof(SubsetConstraintHasSuperChildSequence),
				typeof(TwoOrMoreChildSequenceConstraintHasChildSequence),
				typeof(ChildHasSingleChildConstraint),
				typeof(OIALModelHasChildSequenceConstraint),
				typeof(ConceptTypeChildHasPathRole),
				typeof(ChildSequenceHasConceptTypeChild),
			};
		}
		/// <summary>
		/// Gets the list of generated domain properties.
		/// </summary>
		/// <returns>List of property data.</returns>
		protected sealed override DomainMemberInfo[] GetGeneratedDomainProperties()
		{
			return new DomainMemberInfo[]
			{
				new DomainMemberInfo(typeof(OIALNamedElement), "Name", OIALNamedElement.NameDomainPropertyId, typeof(OIALNamedElement.NamePropertyHandler)),
				new DomainMemberInfo(typeof(Constraint), "Modality", Constraint.ModalityDomainPropertyId, typeof(Constraint.ModalityPropertyHandler)),
				new DomainMemberInfo(typeof(SingleChildUniquenessConstraint), "IsPreferred", SingleChildUniquenessConstraint.IsPreferredDomainPropertyId, typeof(SingleChildUniquenessConstraint.IsPreferredPropertyHandler)),
				new DomainMemberInfo(typeof(ChildSequenceUniquenessConstraint), "ShouldIgnore", ChildSequenceUniquenessConstraint.ShouldIgnoreDomainPropertyId, typeof(ChildSequenceUniquenessConstraint.ShouldIgnorePropertyHandler)),
				new DomainMemberInfo(typeof(ChildSequenceUniquenessConstraint), "IsPreferred", ChildSequenceUniquenessConstraint.IsPreferredDomainPropertyId, typeof(ChildSequenceUniquenessConstraint.IsPreferredPropertyHandler)),
				new DomainMemberInfo(typeof(ConceptTypeChild), "Mandatory", ConceptTypeChild.MandatoryDomainPropertyId, typeof(ConceptTypeChild.MandatoryPropertyHandler)),
				new DomainMemberInfo(typeof(ConceptTypeChild), "Name", ConceptTypeChild.NameDomainPropertyId, typeof(ConceptTypeChild.NamePropertyHandler)),
				new DomainMemberInfo(typeof(ConceptTypeRef), "OppositeName", ConceptTypeRef.OppositeNameDomainPropertyId, typeof(ConceptTypeRef.OppositeNamePropertyHandler)),
			};
		}
		/// <summary>
		/// Gets the list of generated domain roles.
		/// </summary>
		/// <returns>List of role data.</returns>
		protected sealed override DomainRolePlayerInfo[] GetGeneratedDomainRoles()
		{
			return new DomainRolePlayerInfo[]
			{
				new DomainRolePlayerInfo(typeof(OIALModelHasORMModel), "OIALModel", OIALModelHasORMModel.OIALModelDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALModelHasORMModel), "ORMModel", OIALModelHasORMModel.ORMModelDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALModelHasConceptType), "Model", OIALModelHasConceptType.ModelDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALModelHasConceptType), "ConceptType", OIALModelHasConceptType.ConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeChild), "Parent", ConceptTypeChild.ParentDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeChild), "Target", ConceptTypeChild.TargetDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeAbsorbedConceptType), "AbsorbingConceptType", ConceptTypeAbsorbedConceptType.AbsorbingConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeAbsorbedConceptType), "AbsorbedConceptType", ConceptTypeAbsorbedConceptType.AbsorbedConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(InformationType), "ConceptType", InformationType.ConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(InformationType), "InformationTypeFormat", InformationType.InformationTypeFormatDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALHasInformationTypeFormat), "Model", OIALHasInformationTypeFormat.ModelDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALHasInformationTypeFormat), "InformationTypeFormat", OIALHasInformationTypeFormat.InformationTypeFormatDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeRef), "ReferencingConceptType", ConceptTypeRef.ReferencingConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeRef), "ReferencedConceptType", ConceptTypeRef.ReferencedConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(InformationTypeFormatHasObjectType), "InformationTypeFormat", InformationTypeFormatHasObjectType.InformationTypeFormatDomainRoleId),
				new DomainRolePlayerInfo(typeof(InformationTypeFormatHasObjectType), "ValueType", InformationTypeFormatHasObjectType.ValueTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeHasObjectType), "ConceptType", ConceptTypeHasObjectType.ConceptTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeHasObjectType), "ObjectType", ConceptTypeHasObjectType.ObjectTypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ChildSequenceConstraintHasChildSequence), "ChildSequenceConstraint", ChildSequenceConstraintHasChildSequence.ChildSequenceConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(ChildSequenceConstraintHasChildSequence), "ChildSequence", ChildSequenceConstraintHasChildSequence.ChildSequenceDomainRoleId),
				new DomainRolePlayerInfo(typeof(SingleChildSequenceConstraintHasMinTwoChildrenChildSequence), "SingleChildSequenceConstraint", SingleChildSequenceConstraintHasMinTwoChildrenChildSequence.SingleChildSequenceConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(SingleChildSequenceConstraintHasMinTwoChildrenChildSequence), "ChildSequence", SingleChildSequenceConstraintHasMinTwoChildrenChildSequence.ChildSequenceDomainRoleId),
				new DomainRolePlayerInfo(typeof(SubsetConstraintHasSubChildSequence), "SubsetConstraint", SubsetConstraintHasSubChildSequence.SubsetConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(SubsetConstraintHasSubChildSequence), "SubChildSequence", SubsetConstraintHasSubChildSequence.SubChildSequenceDomainRoleId),
				new DomainRolePlayerInfo(typeof(SubsetConstraintHasSuperChildSequence), "SubsetConstraint", SubsetConstraintHasSuperChildSequence.SubsetConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(SubsetConstraintHasSuperChildSequence), "SuperChildSequence", SubsetConstraintHasSuperChildSequence.SuperChildSequenceDomainRoleId),
				new DomainRolePlayerInfo(typeof(TwoOrMoreChildSequenceConstraintHasChildSequence), "TwoOrMoreChildSequenceConstraint", TwoOrMoreChildSequenceConstraintHasChildSequence.TwoOrMoreChildSequenceConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(TwoOrMoreChildSequenceConstraintHasChildSequence), "ChildSequence", TwoOrMoreChildSequenceConstraintHasChildSequence.ChildSequenceDomainRoleId),
				new DomainRolePlayerInfo(typeof(ChildHasSingleChildConstraint), "ConceptTypeChild", ChildHasSingleChildConstraint.ConceptTypeChildDomainRoleId),
				new DomainRolePlayerInfo(typeof(ChildHasSingleChildConstraint), "SingleChildConstraint", ChildHasSingleChildConstraint.SingleChildConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALModelHasChildSequenceConstraint), "OIALModel", OIALModelHasChildSequenceConstraint.OIALModelDomainRoleId),
				new DomainRolePlayerInfo(typeof(OIALModelHasChildSequenceConstraint), "ChildSequenceConstraint", OIALModelHasChildSequenceConstraint.ChildSequenceConstraintDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeChildHasPathRole), "ConceptTypeChild", ConceptTypeChildHasPathRole.ConceptTypeChildDomainRoleId),
				new DomainRolePlayerInfo(typeof(ConceptTypeChildHasPathRole), "PathRole", ConceptTypeChildHasPathRole.PathRoleDomainRoleId),
				new DomainRolePlayerInfo(typeof(ChildSequenceHasConceptTypeChild), "ChildSequence", ChildSequenceHasConceptTypeChild.ChildSequenceDomainRoleId),
				new DomainRolePlayerInfo(typeof(ChildSequenceHasConceptTypeChild), "ConceptTypeChild", ChildSequenceHasConceptTypeChild.ConceptTypeChildDomainRoleId),
			};
		}
		#endregion
		#region Factory methods
		private static global::System.Collections.Generic.Dictionary<global::System.Type, int> createElementMap;
	
		/// <summary>
		/// Creates an element of specified type.
		/// </summary>
		/// <param name="partition">Partition where element is to be created.</param>
		/// <param name="elementType">Element type which belongs to this domain model.</param>
		/// <param name="propertyAssignments">New element property assignments.</param>
		/// <returns>Created element.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public sealed override DslModeling::ModelElement CreateElement(DslModeling::Partition partition, global::System.Type elementType, DslModeling::PropertyAssignment[] propertyAssignments)
		{
			if (elementType == null) throw new global::System.ArgumentNullException("elementType");
	
			if (createElementMap == null)
			{
				createElementMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(22);
				createElementMap.Add(typeof(OIALModel), 0);
				createElementMap.Add(typeof(ConceptType), 1);
				createElementMap.Add(typeof(InformationTypeFormat), 2);
				createElementMap.Add(typeof(ChildSequence), 3);
				createElementMap.Add(typeof(SingleChildUniquenessConstraint), 4);
				createElementMap.Add(typeof(SingleChildFrequencyConstraint), 5);
				createElementMap.Add(typeof(ValueConstraint), 6);
				createElementMap.Add(typeof(ChildSequenceFrequencyConstraint), 7);
				createElementMap.Add(typeof(RingConstraint), 8);
				createElementMap.Add(typeof(DisjunctiveMandatoryConstraint), 9);
				createElementMap.Add(typeof(ChildSequenceUniquenessConstraint), 10);
				createElementMap.Add(typeof(MinTwoChildrenChildSequence), 11);
				createElementMap.Add(typeof(SubsetConstraint), 12);
				createElementMap.Add(typeof(ExclusionConstraint), 13);
				createElementMap.Add(typeof(EqualityConstraint), 14);
			}
			int index;
			if (!createElementMap.TryGetValue(elementType, out index))
			{
				throw new global::System.ArgumentException("elementType is not recognized as a type of domain class which belongs to this domain model.");
			}
			switch (index)
			{
				case 0: return new OIALModel(partition, propertyAssignments);
				case 1: return new ConceptType(partition, propertyAssignments);
				case 2: return new InformationTypeFormat(partition, propertyAssignments);
				case 3: return new ChildSequence(partition, propertyAssignments);
				case 4: return new SingleChildUniquenessConstraint(partition, propertyAssignments);
				case 5: return new SingleChildFrequencyConstraint(partition, propertyAssignments);
				case 6: return new ValueConstraint(partition, propertyAssignments);
				case 7: return new ChildSequenceFrequencyConstraint(partition, propertyAssignments);
				case 8: return new RingConstraint(partition, propertyAssignments);
				case 9: return new DisjunctiveMandatoryConstraint(partition, propertyAssignments);
				case 10: return new ChildSequenceUniquenessConstraint(partition, propertyAssignments);
				case 11: return new MinTwoChildrenChildSequence(partition, propertyAssignments);
				case 12: return new SubsetConstraint(partition, propertyAssignments);
				case 13: return new ExclusionConstraint(partition, propertyAssignments);
				case 14: return new EqualityConstraint(partition, propertyAssignments);
				default: return null;
			}
		}
	
		private static global::System.Collections.Generic.Dictionary<global::System.Type, int> createElementLinkMap;
	
		/// <summary>
		/// Creates an element link of specified type.
		/// </summary>
		/// <param name="partition">Partition where element is to be created.</param>
		/// <param name="elementLinkType">Element link type which belongs to this domain model.</param>
		/// <param name="roleAssignments">List of relationship role assignments for the new link.</param>
		/// <param name="propertyAssignments">New element property assignments.</param>
		/// <returns>Created element link.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public sealed override DslModeling::ElementLink CreateElementLink(DslModeling::Partition partition, global::System.Type elementLinkType, DslModeling::RoleAssignment[] roleAssignments, DslModeling::PropertyAssignment[] propertyAssignments)
		{
			if (elementLinkType == null) throw new global::System.ArgumentNullException("elementType");
			if (roleAssignments == null) throw new global::System.ArgumentNullException("roleAssignments");
	
			if (createElementLinkMap == null)
			{
				createElementLinkMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(18);
				createElementLinkMap.Add(typeof(OIALModelHasORMModel), 0);
				createElementLinkMap.Add(typeof(OIALModelHasConceptType), 1);
				createElementLinkMap.Add(typeof(ConceptTypeAbsorbedConceptType), 2);
				createElementLinkMap.Add(typeof(InformationType), 3);
				createElementLinkMap.Add(typeof(OIALHasInformationTypeFormat), 4);
				createElementLinkMap.Add(typeof(ConceptTypeRef), 5);
				createElementLinkMap.Add(typeof(InformationTypeFormatHasObjectType), 6);
				createElementLinkMap.Add(typeof(ConceptTypeHasObjectType), 7);
				createElementLinkMap.Add(typeof(SingleChildSequenceConstraintHasMinTwoChildrenChildSequence), 8);
				createElementLinkMap.Add(typeof(SubsetConstraintHasSubChildSequence), 9);
				createElementLinkMap.Add(typeof(SubsetConstraintHasSuperChildSequence), 10);
				createElementLinkMap.Add(typeof(TwoOrMoreChildSequenceConstraintHasChildSequence), 11);
				createElementLinkMap.Add(typeof(ChildHasSingleChildConstraint), 12);
				createElementLinkMap.Add(typeof(OIALModelHasChildSequenceConstraint), 13);
				createElementLinkMap.Add(typeof(ConceptTypeChildHasPathRole), 14);
				createElementLinkMap.Add(typeof(ChildSequenceHasConceptTypeChild), 15);
			}
			int index;
			if (!createElementLinkMap.TryGetValue(elementLinkType, out index))
			{
				throw new global::System.ArgumentException("elementLinkType is not recognized as a type of domain relationship which belongs to this domain model.");
			}
			switch (index)
			{
				case 0: return new OIALModelHasORMModel(partition, roleAssignments, propertyAssignments);
				case 1: return new OIALModelHasConceptType(partition, roleAssignments, propertyAssignments);
				case 2: return new ConceptTypeAbsorbedConceptType(partition, roleAssignments, propertyAssignments);
				case 3: return new InformationType(partition, roleAssignments, propertyAssignments);
				case 4: return new OIALHasInformationTypeFormat(partition, roleAssignments, propertyAssignments);
				case 5: return new ConceptTypeRef(partition, roleAssignments, propertyAssignments);
				case 6: return new InformationTypeFormatHasObjectType(partition, roleAssignments, propertyAssignments);
				case 7: return new ConceptTypeHasObjectType(partition, roleAssignments, propertyAssignments);
				case 8: return new SingleChildSequenceConstraintHasMinTwoChildrenChildSequence(partition, roleAssignments, propertyAssignments);
				case 9: return new SubsetConstraintHasSubChildSequence(partition, roleAssignments, propertyAssignments);
				case 10: return new SubsetConstraintHasSuperChildSequence(partition, roleAssignments, propertyAssignments);
				case 11: return new TwoOrMoreChildSequenceConstraintHasChildSequence(partition, roleAssignments, propertyAssignments);
				case 12: return new ChildHasSingleChildConstraint(partition, roleAssignments, propertyAssignments);
				case 13: return new OIALModelHasChildSequenceConstraint(partition, roleAssignments, propertyAssignments);
				case 14: return new ConceptTypeChildHasPathRole(partition, roleAssignments, propertyAssignments);
				case 15: return new ChildSequenceHasConceptTypeChild(partition, roleAssignments, propertyAssignments);
				default: return null;
			}
		}
		#endregion
		#region Resource manager
		
		private static global::System.Resources.ResourceManager resourceManager;
		
		/// <summary>
		/// The base name of this model's resources.
		/// </summary>
		public const string ResourceBaseName = "Neumont.Tools.ORM.OIALModel.GeneratedCode.DomainModelResx";
		
		/// <summary>
		/// Gets the DomainModel's ResourceManager. If the ResourceManager does not already exist, then it is created.
		/// </summary>
		public override global::System.Resources.ResourceManager ResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return OIALDomainModel.SingletonResourceManager;
			}
		}
	
		/// <summary>
		/// Gets the Singleton ResourceManager for this domain model.
		/// </summary>
		public static global::System.Resources.ResourceManager SingletonResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				if (OIALDomainModel.resourceManager == null)
				{
					OIALDomainModel.resourceManager = new global::System.Resources.ResourceManager(ResourceBaseName, typeof(OIALDomainModel).Assembly);
				}
				return OIALDomainModel.resourceManager;
			}
		}
		#endregion
		#region Copy/Remove closures
		/// <summary>
		/// CopyClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter copyClosure;
		/// <summary>
		/// DeleteClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter removeClosure;
		/// <summary>
		/// Returns an IElementVisitorFilter that corresponds to the ClosureType.
		/// </summary>
		/// <param name="type">closure type</param>
		/// <param name="rootElements">collection of root elements</param>
		/// <returns>IElementVisitorFilter or null</returns>
		public override DslModeling::IElementVisitorFilter GetClosureFilter(DslModeling::ClosureType type, global::System.Collections.Generic.ICollection<DslModeling::ModelElement> rootElements)
		{
			switch (type)
			{
				case DslModeling::ClosureType.CopyClosure:
					return OIALDomainModel.CopyClosure;
				case DslModeling::ClosureType.DeleteClosure:
					return OIALDomainModel.DeleteClosure;
			}
			return base.GetClosureFilter(type, rootElements);
		}
		/// <summary>
		/// CopyClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter CopyClosure
		{
			get
			{
				// Incorporate all of the closures from the models we extend
				if (OIALDomainModel.copyClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter copyFilter = new DslModeling::ChainingElementVisitorFilter();
					copyFilter.AddFilter(new OIALCopyClosure());
					
					OIALDomainModel.copyClosure = copyFilter;
				}
				return OIALDomainModel.copyClosure;
			}
		}
		/// <summary>
		/// DeleteClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter DeleteClosure
		{
			get
			{
				// Incorporate all of the closures from the models we extend
				if (OIALDomainModel.removeClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter removeFilter = new DslModeling::ChainingElementVisitorFilter();
					removeFilter.AddFilter(new OIALDeleteClosure());
		
					OIALDomainModel.removeClosure = removeFilter;
				}
				return OIALDomainModel.removeClosure;
			}
		}
		#endregion
	}
		
	#region Copy/Remove closure classes
	/// <summary>
	/// Remove closure visitor filter
	/// </summary>
	[global::System.CLSCompliant(true)]
	public partial class OIALDeleteClosure : OIALDeleteClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OIALDeleteClosure() : base()
		{
		}
	}
	
	/// <summary>
	/// Base class for remove closure visitor filter
	/// </summary>
	public partial class OIALDeleteClosureBase : DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Generic.Dictionary<global::System.Guid, bool> domainRoles;
		/// <summary>
		/// Constructor
		/// </summary>
		public OIALDeleteClosureBase()
		{
			#region Initialize DomainData Table
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.OIALModelHasConceptType.ConceptTypeDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.ConceptTypeAbsorbedConceptType.AbsorbedConceptTypeDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.InformationType.InformationTypeFormatDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.OIALHasInformationTypeFormat.InformationTypeFormatDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.ChildSequenceConstraintHasChildSequence.ChildSequenceDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.SingleChildSequenceConstraintHasMinTwoChildrenChildSequence.ChildSequenceDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.SubsetConstraintHasSubChildSequence.SubChildSequenceDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.SubsetConstraintHasSuperChildSequence.SuperChildSequenceDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.TwoOrMoreChildSequenceConstraintHasChildSequence.ChildSequenceDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.ChildHasSingleChildConstraint.SingleChildConstraintDomainRoleId, true);
			DomainRoles.Add(global::Neumont.Tools.ORM.OIALModel.OIALModelHasChildSequenceConstraint.ChildSequenceConstraintDomainRoleId, true);
			#endregion
		}
		/// <summary>
		/// Called to ask the filter if a particular relationship from a source element should be included in the traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="sourceRoleInfo">DomainRoleInfo of the role that the source element is playing in the relationship</param>
		/// <param name="domainRelationshipInfo">DomainRelationshipInfo for the ElementLink in question</param>
		/// <param name="targetRelationship">Relationship in question</param>
		/// <returns>Yes if the relationship should be traversed</returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRelationship(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::DomainRoleInfo sourceRoleInfo, DslModeling::DomainRelationshipInfo domainRelationshipInfo, DslModeling::ElementLink targetRelationship)
		{
			return DslModeling::VisitorFilterResult.Yes;
		}
		/// <summary>
		/// Called to ask the filter if a particular role player should be Visited during traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="elementLink">Element Link that forms the relationship to the role player in question</param>
		/// <param name="targetDomainRole">DomainRoleInfo of the target role</param>
		/// <param name="targetRolePlayer">Model Element that plays the target role in the relationship</param>
		/// <returns></returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRolePlayer(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::ElementLink elementLink, DslModeling::DomainRoleInfo targetDomainRole, DslModeling::ModelElement targetRolePlayer)
		{
			return this.DomainRoles.ContainsKey(targetDomainRole.Id) ? DslModeling::VisitorFilterResult.Yes : DslModeling::VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Generic.Dictionary<global::System.Guid, bool> DomainRoles
		{
			get
			{
				if (this.domainRoles == null)
				{
					this.domainRoles = new global::System.Collections.Generic.Dictionary<global::System.Guid, bool>();
				}
				return this.domainRoles;
			}
		}
	
	}
	/// <summary>
	/// Copy closure visitor filter
	/// </summary>
	[global::System.CLSCompliant(true)]
	public partial class OIALCopyClosure : OIALCopyClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OIALCopyClosure() : base()
		{
		}
	}
	/// <summary>
	/// Base class for copy closure visitor filter
	/// </summary>
	public partial class OIALCopyClosureBase : DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Generic.Dictionary<global::System.Guid, bool> domainRoles;
		/// <summary>
		/// Constructor
		/// </summary>
		public OIALCopyClosureBase()
		{
			#region Initialize DomainData Table
			#endregion
		}
		/// <summary>
		/// Called to ask the filter if a particular relationship from a source element should be included in the traversal
		/// </summary>
		/// <param name="walker">ElementWalker traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="sourceRoleInfo">DomainRoleInfo of the role that the source element is playing in the relationship</param>
		/// <param name="domainRelationshipInfo">DomainRelationshipInfo for the ElementLink in question</param>
		/// <param name="targetRelationship">Relationship in question</param>
		/// <returns>Yes if the relationship should be traversed</returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRelationship(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::DomainRoleInfo sourceRoleInfo, DslModeling::DomainRelationshipInfo domainRelationshipInfo, DslModeling::ElementLink targetRelationship)
		{
			return this.DomainRoles.ContainsKey(sourceRoleInfo.Id) ? DslModeling::VisitorFilterResult.Yes : DslModeling::VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// Called to ask the filter if a particular role player should be Visited during traversal
		/// </summary>
		/// <param name="walker">ElementWalker traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="elementLink">Element Link that forms the relationship to the role player in question</param>
		/// <param name="targetDomainRole">DomainRoleInfo of the target role</param>
		/// <param name="targetRolePlayer">Model Element that plays the target role in the relationship</param>
		/// <returns></returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRolePlayer(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::ElementLink elementLink, DslModeling::DomainRoleInfo targetDomainRole, DslModeling::ModelElement targetRolePlayer)
		{
			return this.DomainRoles.ContainsKey(targetDomainRole.Id) ? DslModeling::VisitorFilterResult.Yes : DslModeling::VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Generic.Dictionary<global::System.Guid, bool> DomainRoles
		{
			get
			{
				if (this.domainRoles == null)
				{
					this.domainRoles = new global::System.Collections.Generic.Dictionary<global::System.Guid, bool>();
				}
				return this.domainRoles;
			}
		}
	
	}
	#endregion
		
}
namespace Neumont.Tools.ORM.OIALModel
{
	/// <summary>
	/// DomainEnumeration: MandatoryConstraintModality
	/// A list of constraint modalities for simple mandatory role constraints used in
	/// <see cref="ConceptTypeChild"/> relationships.
	/// </summary>
	[global::System.ComponentModel.TypeConverter(typeof(global::Neumont.Tools.Modeling.Design.EnumConverter<MandatoryConstraintModality, global::Neumont.Tools.ORM.OIALModel.OIALDomainModel>))]
	[global::System.CLSCompliant(true)]
	public enum MandatoryConstraintModality
	{
		/// <summary>
		/// NotMandatory
		/// See <see langword="null"/>.
		/// </summary>
		[DslDesign::DescriptionResource("Neumont.Tools.ORM.OIALModel.MandatoryConstraintModality/NotMandatory.Description", typeof(global::Neumont.Tools.ORM.OIALModel.OIALDomainModel), "Neumont.Tools.ORM.OIALModel.GeneratedCode.DomainModelResx")]
		NotMandatory = 0,
		/// <summary>
		/// Alethic
		/// See <see cref="Neumont.Tools.ORM.ObjectModel.ConstraintModality.Alethic"/>.
		/// </summary>
		[DslDesign::DescriptionResource("Neumont.Tools.ORM.OIALModel.MandatoryConstraintModality/Alethic.Description", typeof(global::Neumont.Tools.ORM.OIALModel.OIALDomainModel), "Neumont.Tools.ORM.OIALModel.GeneratedCode.DomainModelResx")]
		Alethic = 1,
		/// <summary>
		/// Deontic
		/// See <see cref="Neumont.Tools.ORM.ObjectModel.ConstraintModality.Deontic"/>.
		/// </summary>
		[DslDesign::DescriptionResource("Neumont.Tools.ORM.OIALModel.MandatoryConstraintModality/Deontic.Description", typeof(global::Neumont.Tools.ORM.OIALModel.OIALDomainModel), "Neumont.Tools.ORM.OIALModel.GeneratedCode.DomainModelResx")]
		Deontic = 2,
	}
}
