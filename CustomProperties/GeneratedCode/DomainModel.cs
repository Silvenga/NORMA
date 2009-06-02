﻿#region Common Public License Copyright Notice
/**************************************************************************\
* Natural Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright © Neumont University. All rights reserved.                     *
* Copyright © ORM Solutions, LLC. All rights reserved.                     *
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
namespace ORMSolutions.ORMArchitect.CustomProperties
{
	/// <summary>
	/// DomainModel CustomPropertiesDomainModel
	/// Add custom properties to ORM model elements
	/// </summary>
	[DslModeling::ExtendsDomainModel("3EAE649F-E654-4D04-8289-C25D2C0322D8"/*ORMSolutions.ORMArchitect.Core.ObjectModel.ORMCoreDomainModel*/)]
	[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel.DisplayName", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
	[DslModeling::DomainObjectId("b1430d25-af34-47e3-bfff-561deef0a2b1")]
	internal partial class CustomPropertiesDomainModel : DslModeling::DomainModel
	{
		#region Constructor, domain model Id
	
		/// <summary>
		/// CustomPropertiesDomainModel domain model Id.
		/// </summary>
		public static readonly global::System.Guid DomainModelId = new global::System.Guid(0xb1430d25, 0xaf34, 0x47e3, 0xbf, 0xff, 0x56, 0x1d, 0xee, 0xf0, 0xa2, 0xb1);
	
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="store">Store containing the domain model.</param>
		public CustomPropertiesDomainModel(DslModeling::Store store)
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
				typeof(CustomPropertyGroup),
				typeof(CustomPropertyDefinition),
				typeof(CustomProperty),
				typeof(CustomPropertyHasCustomPropertyDefinition),
				typeof(CustomPropertyGroupContainsCustomPropertyDefinition),
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
				new DomainMemberInfo(typeof(CustomPropertyGroup), "Name", CustomPropertyGroup.NameDomainPropertyId, typeof(CustomPropertyGroup.NamePropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyGroup), "IsDefault", CustomPropertyGroup.IsDefaultDomainPropertyId, typeof(CustomPropertyGroup.IsDefaultPropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyGroup), "Description", CustomPropertyGroup.DescriptionDomainPropertyId, typeof(CustomPropertyGroup.DescriptionPropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "Name", CustomPropertyDefinition.NameDomainPropertyId, typeof(CustomPropertyDefinition.NamePropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "Description", CustomPropertyDefinition.DescriptionDomainPropertyId, typeof(CustomPropertyDefinition.DescriptionPropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "Category", CustomPropertyDefinition.CategoryDomainPropertyId, typeof(CustomPropertyDefinition.CategoryPropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "DataType", CustomPropertyDefinition.DataTypeDomainPropertyId, typeof(CustomPropertyDefinition.DataTypePropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "DefaultValue", CustomPropertyDefinition.DefaultValueDomainPropertyId, typeof(CustomPropertyDefinition.DefaultValuePropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "VerbalizeDefaultValue", CustomPropertyDefinition.VerbalizeDefaultValueDomainPropertyId, typeof(CustomPropertyDefinition.VerbalizeDefaultValuePropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "ORMTypes", CustomPropertyDefinition.ORMTypesDomainPropertyId, typeof(CustomPropertyDefinition.ORMTypesPropertyHandler)),
				new DomainMemberInfo(typeof(CustomPropertyDefinition), "CustomEnumValue", CustomPropertyDefinition.CustomEnumValueDomainPropertyId, typeof(CustomPropertyDefinition.CustomEnumValuePropertyHandler)),
				new DomainMemberInfo(typeof(CustomProperty), "Value", CustomProperty.ValueDomainPropertyId, typeof(CustomProperty.ValuePropertyHandler)),
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
				new DomainRolePlayerInfo(typeof(CustomPropertyHasCustomPropertyDefinition), "CustomProperty", CustomPropertyHasCustomPropertyDefinition.CustomPropertyDomainRoleId),
				new DomainRolePlayerInfo(typeof(CustomPropertyHasCustomPropertyDefinition), "CustomPropertyDefinition", CustomPropertyHasCustomPropertyDefinition.CustomPropertyDefinitionDomainRoleId),
				new DomainRolePlayerInfo(typeof(CustomPropertyGroupContainsCustomPropertyDefinition), "CustomPropertyGroup", CustomPropertyGroupContainsCustomPropertyDefinition.CustomPropertyGroupDomainRoleId),
				new DomainRolePlayerInfo(typeof(CustomPropertyGroupContainsCustomPropertyDefinition), "CustomPropertyDefinition", CustomPropertyGroupContainsCustomPropertyDefinition.CustomPropertyDefinitionDomainRoleId),
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
				createElementMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(3);
				createElementMap.Add(typeof(CustomPropertyGroup), 0);
				createElementMap.Add(typeof(CustomPropertyDefinition), 1);
				createElementMap.Add(typeof(CustomProperty), 2);
			}
			int index;
			if (!createElementMap.TryGetValue(elementType, out index))
			{
				throw new global::System.ArgumentException("elementType is not recognized as a type of domain class which belongs to this domain model.");
			}
			switch (index)
			{
				case 0: return new CustomPropertyGroup(partition, propertyAssignments);
				case 1: return new CustomPropertyDefinition(partition, propertyAssignments);
				case 2: return new CustomProperty(partition, propertyAssignments);
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
				createElementLinkMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(2);
				createElementLinkMap.Add(typeof(CustomPropertyHasCustomPropertyDefinition), 0);
				createElementLinkMap.Add(typeof(CustomPropertyGroupContainsCustomPropertyDefinition), 1);
			}
			int index;
			if (!createElementLinkMap.TryGetValue(elementLinkType, out index))
			{
				throw new global::System.ArgumentException("elementLinkType is not recognized as a type of domain relationship which belongs to this domain model.");
			}
			switch (index)
			{
				case 0: return new CustomPropertyHasCustomPropertyDefinition(partition, roleAssignments, propertyAssignments);
				case 1: return new CustomPropertyGroupContainsCustomPropertyDefinition(partition, roleAssignments, propertyAssignments);
				default: return null;
			}
		}
		#endregion
		#region Resource manager
		
		private static global::System.Resources.ResourceManager resourceManager;
		
		/// <summary>
		/// The base name of this model's resources.
		/// </summary>
		public const string ResourceBaseName = "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx";
		
		/// <summary>
		/// Gets the DomainModel's ResourceManager. If the ResourceManager does not already exist, then it is created.
		/// </summary>
		public override global::System.Resources.ResourceManager ResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return CustomPropertiesDomainModel.SingletonResourceManager;
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
				if (CustomPropertiesDomainModel.resourceManager == null)
				{
					CustomPropertiesDomainModel.resourceManager = new global::System.Resources.ResourceManager(ResourceBaseName, typeof(CustomPropertiesDomainModel).Assembly);
				}
				return CustomPropertiesDomainModel.resourceManager;
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
					return CustomPropertiesDomainModel.CopyClosure;
				case DslModeling::ClosureType.DeleteClosure:
					return CustomPropertiesDomainModel.DeleteClosure;
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
				if (CustomPropertiesDomainModel.copyClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter copyFilter = new DslModeling::ChainingElementVisitorFilter();
					copyFilter.AddFilter(new CustomPropertiesCopyClosure());
					
					CustomPropertiesDomainModel.copyClosure = copyFilter;
				}
				return CustomPropertiesDomainModel.copyClosure;
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
				if (CustomPropertiesDomainModel.removeClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter removeFilter = new DslModeling::ChainingElementVisitorFilter();
					removeFilter.AddFilter(new CustomPropertiesDeleteClosure());
		
					CustomPropertiesDomainModel.removeClosure = removeFilter;
				}
				return CustomPropertiesDomainModel.removeClosure;
			}
		}
		#endregion
	}
		
	#region Copy/Remove closure classes
	/// <summary>
	/// Remove closure visitor filter
	/// </summary>
	internal partial class CustomPropertiesDeleteClosure : CustomPropertiesDeleteClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CustomPropertiesDeleteClosure() : base()
		{
		}
	}
	
	/// <summary>
	/// Base class for remove closure visitor filter
	/// </summary>
	internal partial class CustomPropertiesDeleteClosureBase : DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Generic.Dictionary<global::System.Guid, bool> domainRoles;
		/// <summary>
		/// Constructor
		/// </summary>
		public CustomPropertiesDeleteClosureBase()
		{
			#region Initialize DomainData Table
			DomainRoles.Add(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyHasCustomPropertyDefinition.CustomPropertyDomainRoleId, true);
			DomainRoles.Add(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyGroupContainsCustomPropertyDefinition.CustomPropertyDefinitionDomainRoleId, true);
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
	internal partial class CustomPropertiesCopyClosure : CustomPropertiesCopyClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CustomPropertiesCopyClosure() : base()
		{
		}
	}
	/// <summary>
	/// Base class for copy closure visitor filter
	/// </summary>
	internal partial class CustomPropertiesCopyClosureBase : DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Generic.Dictionary<global::System.Guid, bool> domainRoles;
		/// <summary>
		/// Constructor
		/// </summary>
		public CustomPropertiesCopyClosureBase()
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
namespace ORMSolutions.ORMArchitect.CustomProperties
{
	/// <summary>
	/// DomainEnumeration: ORMTypes
	/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes
	/// </summary>
	[global::System.ComponentModel.TypeConverter(typeof(global::ORMSolutions.ORMArchitect.Framework.Design.EnumConverter<ORMTypes, global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel>))]
	[global::System.Flags]
	internal enum ORMTypes
	{
		/// <summary>
		/// None
		/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.None
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/None.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		None = 0,
		/// <summary>
		/// EntityType
		/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.EntityType
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/EntityType.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		EntityType = 1,
		/// <summary>
		/// ValueType
		/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.ValueType
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/ValueType.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		ValueType = 2,
		/// <summary>
		/// FactType
		/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.FactType
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/FactType.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		FactType = 4,
		/// <summary>
		/// SubtypeFact
		/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.SubtypeFact
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/SubtypeFact.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		SubtypeFact = 8,
		/// <summary>
		/// Role
		/// Description for ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.Role
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/Role.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		Role = 16,
		/// <summary>
		/// FrequencyConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.FrequencyConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/FrequencyConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		FrequencyConstraint = 32,
		/// <summary>
		/// MandatoryConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.MandatoryConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/MandatoryConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		MandatoryConstraint = 64,
		/// <summary>
		/// RingConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.RingConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/RingConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		RingConstraint = 128,
		/// <summary>
		/// UniquenessConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.UniquenessConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/UniquenessConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		UniquenessConstraint = 256,
		/// <summary>
		/// EqualityConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.EqualityConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/EqualityConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		EqualityConstraint = 512,
		/// <summary>
		/// ExclusionConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.ExclusionConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/ExclusionConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		ExclusionConstraint = 1024,
		/// <summary>
		/// SubsetConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.SubsetConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/SubsetConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		SubsetConstraint = 2048,
		/// <summary>
		/// ValueConstraint
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.ValueConstraint
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/ValueConstraint.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		ValueConstraint = 4096,
		/// <summary>
		/// AllConstraints
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.ORMTypes.AllConstraints
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.ORMTypes/AllConstraints.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		AllConstraints = 7904,
	}
}
namespace ORMSolutions.ORMArchitect.CustomProperties
{
	/// <summary>
	/// DomainEnumeration: CustomPropertyDataType
	/// Description for
	/// ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType
	/// </summary>
	[global::System.ComponentModel.TypeConverter(typeof(global::ORMSolutions.ORMArchitect.Framework.Design.EnumConverter<CustomPropertyDataType, global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel>))]
	internal enum CustomPropertyDataType
	{
		/// <summary>
		/// String
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType.String
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType/String.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		String = 0,
		/// <summary>
		/// Integer
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType.Integer
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType/Integer.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		Integer = 1,
		/// <summary>
		/// Decimal
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType.Decimal
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType/Decimal.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		Decimal = 2,
		/// <summary>
		/// DateTime
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType.DateTime
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType/DateTime.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		DateTime = 3,
		/// <summary>
		/// CustomEnumeration
		/// Description for
		/// ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType.CustomEnumeration
		/// </summary>
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.CustomProperties.CustomPropertyDataType/CustomEnumeration.Description", typeof(global::ORMSolutions.ORMArchitect.CustomProperties.CustomPropertiesDomainModel), "ORMSolutions.ORMArchitect.CustomProperties.GeneratedCode.DomainModelResx")]
		CustomEnumeration = 4,
	}
}

