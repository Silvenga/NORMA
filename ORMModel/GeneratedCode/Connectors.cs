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

using DslModeling = Microsoft.VisualStudio.Modeling;
using DslDesign = Microsoft.VisualStudio.Modeling.Design;
using DslDiagrams = Microsoft.VisualStudio.Modeling.Diagrams;

namespace Neumont.Tools.ORM.ShapeModel
{
	/// <summary>
	/// DomainClass ORMBaseBinaryLinkShape
	/// Description for Neumont.Tools.ORM.ShapeModel.ORMBaseBinaryLinkShape
	/// </summary>
	[DslDesign::DisplayNameResource("Neumont.Tools.ORM.ShapeModel.ORMBaseBinaryLinkShape.DisplayName", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[DslDesign::DescriptionResource("Neumont.Tools.ORM.ShapeModel.ORMBaseBinaryLinkShape.Description", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("ceff4339-48d0-4ffe-b052-2f9da167b1db")]
	public abstract partial class ORMBaseBinaryLinkShape : DslDiagrams::BinaryLinkShape
	{
		#region Connector styles
		#endregion
		#region Constructors, domain class Id
	
		/// <summary>
		/// ORMBaseBinaryLinkShape domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0xceff4339, 0x48d0, 0x4ffe, 0xb0, 0x52, 0x2f, 0x9d, 0xa1, 0x67, 0xb1, 0xdb);
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		protected ORMBaseBinaryLinkShape(DslModeling::Partition partition, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}
namespace Neumont.Tools.ORM.ShapeModel
{
	/// <summary>
	/// DomainClass RolePlayerLink
	/// Description for Neumont.Tools.ORM.ShapeModel.RolePlayerLink
	/// </summary>
	[DslDesign::DisplayNameResource("Neumont.Tools.ORM.ShapeModel.RolePlayerLink.DisplayName", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[DslDesign::DescriptionResource("Neumont.Tools.ORM.ShapeModel.RolePlayerLink.Description", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("2b3f0aae-b1b1-4727-8862-5c34b494b499")]
	public partial class RolePlayerLink : ORMBaseBinaryLinkShape
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with RolePlayerLink.
		/// </summary>
		public static DslDiagrams::Decorator FindRolePlayerLinkDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		#region Connector styles
		#endregion
		#region Constructors, domain class Id
	
		/// <summary>
		/// RolePlayerLink domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x2b3f0aae, 0xb1b1, 0x4727, 0x88, 0x62, 0x5c, 0x34, 0xb4, 0x94, 0xb4, 0x99);
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public RolePlayerLink(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public RolePlayerLink(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}
namespace Neumont.Tools.ORM.ShapeModel
{
	/// <summary>
	/// DomainClass ExternalConstraintLink
	/// Description for Neumont.Tools.ORM.ShapeModel.ExternalConstraintLink
	/// </summary>
	[DslDesign::DisplayNameResource("Neumont.Tools.ORM.ShapeModel.ExternalConstraintLink.DisplayName", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[DslDesign::DescriptionResource("Neumont.Tools.ORM.ShapeModel.ExternalConstraintLink.Description", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("8815e6d8-238b-422c-a4b3-29fdc8de9ea5")]
	public partial class ExternalConstraintLink : ORMBaseBinaryLinkShape
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with ExternalConstraintLink.
		/// </summary>
		public static DslDiagrams::Decorator FindExternalConstraintLinkDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		#region Connector styles
		#endregion
		#region Constructors, domain class Id
	
		/// <summary>
		/// ExternalConstraintLink domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x8815e6d8, 0x238b, 0x422c, 0xa4, 0xb3, 0x29, 0xfd, 0xc8, 0xde, 0x9e, 0xa5);
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public ExternalConstraintLink(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public ExternalConstraintLink(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}
namespace Neumont.Tools.ORM.ShapeModel
{
	/// <summary>
	/// DomainClass ValueRangeLink
	/// Description for Neumont.Tools.ORM.ShapeModel.ValueRangeLink
	/// </summary>
	[DslDesign::DisplayNameResource("Neumont.Tools.ORM.ShapeModel.ValueRangeLink.DisplayName", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[DslDesign::DescriptionResource("Neumont.Tools.ORM.ShapeModel.ValueRangeLink.Description", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("374e43c3-c294-49c4-8a61-3c3ca5fc86e8")]
	public partial class ValueRangeLink : ORMBaseBinaryLinkShape
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with ValueRangeLink.
		/// </summary>
		public static DslDiagrams::Decorator FindValueRangeLinkDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		#region Connector styles
		#endregion
		#region Constructors, domain class Id
	
		/// <summary>
		/// ValueRangeLink domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x374e43c3, 0xc294, 0x49c4, 0x8a, 0x61, 0x3c, 0x3c, 0xa5, 0xfc, 0x86, 0xe8);
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public ValueRangeLink(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public ValueRangeLink(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}
namespace Neumont.Tools.ORM.ShapeModel
{
	/// <summary>
	/// DomainClass ModelNoteLink
	/// Description for Neumont.Tools.ORM.ShapeModel.ModelNoteLink
	/// </summary>
	[DslDesign::DisplayNameResource("Neumont.Tools.ORM.ShapeModel.ModelNoteLink.DisplayName", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[DslDesign::DescriptionResource("Neumont.Tools.ORM.ShapeModel.ModelNoteLink.Description", typeof(global::Neumont.Tools.ORM.ShapeModel.ORMShapeModel), "Neumont.Tools.ORM.GeneratedCode.ShapeDomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DomainObjectId("21e7c585-bc80-446f-8517-bc4fd465971f")]
	public partial class ModelNoteLink : ORMBaseBinaryLinkShape
	{
		#region DiagramElement boilerplate
		private static DslDiagrams::StyleSet classStyleSet;
		private static global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields;
		private static global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators;
		
		/// <summary>
		/// Per-class style set for this shape.
		/// </summary>
		protected override DslDiagrams::StyleSet ClassStyleSet
		{
			get
			{
				if (classStyleSet == null)
				{
					classStyleSet = CreateClassStyleSet();
				}
				return classStyleSet;
			}
		}
		
		/// <summary>
		/// Per-class ShapeFields for this shape.
		/// </summary>
		public override global::System.Collections.Generic.IList<DslDiagrams::ShapeField> ShapeFields
		{
			get
			{
				if (shapeFields == null)
				{
					shapeFields = CreateShapeFields();
				}
				return shapeFields;
			}
		}
		
		/// <summary>
		/// Event fired when decorator initialization is complete for this shape type.
		/// </summary>
		public static event global::System.EventHandler DecoratorsInitialized;
		
		/// <summary>
		/// List containing decorators used by this type.
		/// </summary>
		public override System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
		{
			get 
			{
				if(decorators == null)
				{
					decorators = CreateDecorators();
					
					// fire this event to allow the diagram to initialize decorator mappings for this shape type.
					if(DecoratorsInitialized != null)
					{
						DecoratorsInitialized(this, global::System.EventArgs.Empty);
					}
				}
				
				return decorators; 
			}
		}
		
		/// <summary>
		/// Finds a decorator associated with ModelNoteLink.
		/// </summary>
		public static DslDiagrams::Decorator FindModelNoteLinkDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		#endregion
		#region Connector styles
		#endregion
		#region Constructors, domain class Id
	
		/// <summary>
		/// ModelNoteLink domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x21e7c585, 0xbc80, 0x446f, 0x85, 0x17, 0xbc, 0x4f, 0xd4, 0x65, 0x97, 0x1f);
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public ModelNoteLink(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public ModelNoteLink(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}
