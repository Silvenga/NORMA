﻿#region Common Public License Copyright Notice
/**************************************************************************\
* Natural Object-Role Modeling Architect for Visual Studio                 *
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
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;

namespace ORMSolutions.ORMArchitect.Views.RelationalView
{
	/// <summary>
	/// Double-derived base class for DomainClass TableShape
	/// </summary>
	[global::System.ComponentModel.TypeDescriptionProvider(typeof(global::ORMSolutions.ORMArchitect.Framework.Diagrams.Design.PresentationElementTypeDescriptionProvider<TableShape, global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Table, global::ORMSolutions.ORMArchitect.Framework.Diagrams.Design.PresentationElementTypeDescriptor<TableShape, global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Table>>))]
	[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Views.RelationalView.TableShape.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Views.RelationalView.RelationalShapeDomainModel), "ORMSolutions.ORMArchitect.Views.RelationalView.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Views.RelationalView.TableShape.Description", typeof(global::ORMSolutions.ORMArchitect.Views.RelationalView.RelationalShapeDomainModel), "ORMSolutions.ORMArchitect.Views.RelationalView.GeneratedCode.DomainModelResx")]
	[DslModeling::DomainObjectId("50dabfcd-909c-418a-8895-172aadaad4fb")]
	internal abstract partial class TableShapeBase : DslDiagrams::CompartmentShape
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
		public override global::System.Collections.Generic.IList<DslDiagrams::Decorator> Decorators
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
		/// Finds a decorator associated with TableShape.
		/// </summary>
		public static DslDiagrams::Decorator FindTableShapeDecorator(string decoratorName)
		{	
			if(decorators == null) return null;
			return DslDiagrams::ShapeElement.FindDecorator(decorators, decoratorName);
		}
		
		
		/// <summary>
		/// Shape instance initialization.
		/// </summary>
		public override void OnInitialize()
		{
			base.OnInitialize();
			
			// Create host shapes for outer decorators.
			foreach(DslDiagrams::Decorator decorator in this.Decorators)
			{
				if(decorator.RequiresHost)
				{
					decorator.ConfigureHostShape(this);
				}
			}
			
		}
		#endregion
		#region Shape size
		
		/// <summary>
		/// Default size for this shape.
		/// </summary>
		public override DslDiagrams::SizeD DefaultSize
		{
			get
			{
				return new DslDiagrams::SizeD(1, 0.25);
			}
		}
		#endregion
		#region Shape styles
		/// <summary>
		/// Initializes style set resources for this shape type
		/// </summary>
		/// <param name="classStyleSet">The style set for this shape class</param>
		protected override void InitializeResources(DslDiagrams::StyleSet classStyleSet)
		{
			base.InitializeResources(classStyleSet);
			
			// Outline pen settings for this shape.
			DslDiagrams::PenSettings outlinePen = new DslDiagrams::PenSettings();
			outlinePen.Width = 0.015625F;
			classStyleSet.OverridePen(DslDiagrams::DiagramPens.ShapeOutline, outlinePen);
			// Custom font styles
			DslDiagrams::FontSettings fontSettings;
			fontSettings = new DslDiagrams::FontSettings();
			fontSettings.Style =  global::System.Drawing.FontStyle.Bold ;
			fontSettings.Size = 10/72.0F;
			classStyleSet.AddFont(new DslDiagrams::StyleSetResourceId(string.Empty, "ShapeTextBold10"), DslDiagrams::DiagramFonts.ShapeText, fontSettings);
		}
		
		/// <summary>
		/// Indicates whether this shape displays a background gradient.
		/// </summary>
		public override bool HasBackgroundGradient
		{
			get
			{
				return true;
			}
		}
		
		/// <summary>
		/// Indicates the direction of the gradient.
		/// </summary>
		public override global::System.Drawing.Drawing2D.LinearGradientMode BackgroundGradientMode
		{
			get
			{
				return global::System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			}
		}
		#endregion
		#region Decorators
		/// <summary>
		/// Initialize the collection of shape fields associated with this shape type.
		/// </summary>
		protected override void InitializeShapeFields(global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields)
		{
			base.InitializeShapeFields(shapeFields);
			DslDiagrams::TextField field1 = new DslDiagrams::TextField("TableNameDecorator");
			field1.DefaultText = global::ORMSolutions.ORMArchitect.Views.RelationalView.RelationalShapeDomainModel.SingletonResourceManager.GetString("TableShapeTableNameDecoratorDefaultText");
			field1.DefaultFocusable = true;
			field1.DefaultAutoSize = true;
			field1.AnchoringBehavior.MinimumHeightInLines = 1;
			field1.AnchoringBehavior.MinimumWidthInCharacters = 1;
			field1.DefaultAccessibleState = System.Windows.Forms.AccessibleStates.Invisible;
			field1.DefaultFontId = new DslDiagrams::StyleSetResourceId(string.Empty, "ShapeTextBold10");			
			shapeFields.Add(field1);
			
		}
		
		/// <summary>
		/// Initialize the collection of decorators associated with this shape type.  This method also
		/// creates shape fields for outer decorators, because these are not part of the shape fields collection
		/// associated with the shape, so they must be created here rather than in InitializeShapeFields.
		/// </summary>
		protected override void InitializeDecorators(global::System.Collections.Generic.IList<DslDiagrams::ShapeField> shapeFields, global::System.Collections.Generic.IList<DslDiagrams::Decorator> decorators)
		{
			base.InitializeDecorators(shapeFields, decorators);
			
			DslDiagrams::ShapeField field1 = DslDiagrams::ShapeElement.FindShapeField(shapeFields, "TableNameDecorator");
			DslDiagrams::Decorator decorator1 = new DslDiagrams::ShapeDecorator(field1, DslDiagrams::ShapeDecoratorPosition.InnerTopCenter, DslDiagrams::PointD.Empty);
			decorators.Add(decorator1);
				
		}
		
		/// <summary>
		/// Ensure outer decorators are placed appropriately.  This is called during view fixup,
		/// after the shape has been associated with the model element.
		/// </summary>
		public override void OnBoundsFixup(DslDiagrams::BoundsFixupState fixupState, int iteration, bool createdDuringViewFixup)
		{
			base.OnBoundsFixup(fixupState, iteration, createdDuringViewFixup);
			
			if(iteration == 0)
			{
				foreach(DslDiagrams::Decorator decorator in this.Decorators)
				{
					if(decorator.RequiresHost)
					{
						decorator.RepositionHostShape(decorator.GetHostShape(this));
					}
				}
			}
		}
		#endregion
		#region ComparmentShape code
		/// <summary>
		/// Returns a value indicating whether compartment header should be visible if there is only one of them.
		/// </summary>
		public override bool IsSingleCompartmentHeaderVisible
		{
			get { return true; }
		}
		
		private static DslDiagrams::CompartmentDescription[] compartmentDescriptions;
		
		/// <summary>
		/// Gets an array of CompartmentDescription for all compartments shown on this shape
		/// (including compartments defined on base shapes).
		/// </summary>
		/// <returns></returns>
		public override DslDiagrams::CompartmentDescription[] GetCompartmentDescriptions()
		{
			if(compartmentDescriptions == null)
			{
				// Initialize the array of compartment descriptions if we haven't done so already. 
				// First we get any compartment descriptions in base shapes, and add on any compartments
				// that are defined on this shape. 
				DslDiagrams::CompartmentDescription[] baseCompartmentDescriptions = base.GetCompartmentDescriptions();
				
				int localCompartmentsOffset = 0;
				if(baseCompartmentDescriptions!=null)
				{
					localCompartmentsOffset = baseCompartmentDescriptions.Length;
				}
				compartmentDescriptions = new DslDiagrams::ElementListCompartmentDescription[1+localCompartmentsOffset];
				
				if(baseCompartmentDescriptions!=null)
				{
					baseCompartmentDescriptions.CopyTo(compartmentDescriptions, 0);	
				}
				{
					string title = global::ORMSolutions.ORMArchitect.Views.RelationalView.RelationalShapeDomainModel.SingletonResourceManager.GetString("TableShapeColumnsCompartmentTitle");
					compartmentDescriptions[localCompartmentsOffset+0] = new DslDiagrams::ElementListCompartmentDescription("ColumnsCompartment", title, 
						global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.LightGray), false, 
						global::System.Drawing.Color.FromKnownColor(global::System.Drawing.KnownColor.White), false,
						null, null,
						false);
				}
			}
			
			return TableShape.compartmentDescriptions;
		}
		
		private static global::System.Collections.Generic.Dictionary<global::System.Type, DslDiagrams::CompartmentMapping[]> compartmentMappings;
		
		/// <summary>
		/// Gets an array of CompartmentMappings for all compartments displayed on this shape
		/// (including compartment maps defined on base shapes). 
		/// </summary>
		/// <param name="melType">The type of the DomainClass that this shape is mapped to</param>
		/// <returns></returns>
		protected override DslDiagrams::CompartmentMapping[] GetCompartmentMappings(global::System.Type melType)
		{
			if(melType==null) throw new global::System.ArgumentNullException("melType");
			
			if(compartmentMappings==null)
			{
				// Initialize the table of compartment mappings if we haven't done so already. 
				// The table contains an array of CompartmentMapping for every Type that this
				// shape can be mapped to. 
				compartmentMappings = new global::System.Collections.Generic.Dictionary<global::System.Type, DslDiagrams::CompartmentMapping[]>();
				{
					// First we get the mappings defined for the base shape, and add on any mappings defined for this
					// shape. 
					DslDiagrams::CompartmentMapping[] baseMappings = base.GetCompartmentMappings(typeof(global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Table));
					int localCompartmentMappingsOffset = 0;
					if(baseMappings!=null)
					{
						localCompartmentMappingsOffset = baseMappings.Length;
					}
					DslDiagrams::CompartmentMapping[] mappings = new DslDiagrams::CompartmentMapping[1+localCompartmentMappingsOffset];
					
					if(baseMappings!=null)
					{
						baseMappings.CopyTo(mappings, 0);
					}
					mappings[localCompartmentMappingsOffset+0] = new DslDiagrams::ElementListCompartmentMapping(
																				"ColumnsCompartment", 
																				global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Column.EditNameDomainPropertyId, 
																				global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Column.DomainClassId, 
																				GetElementsFromTableForColumnsCompartment,
																				null,
																				null,
																				null);
					compartmentMappings.Add(typeof(global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Table), mappings);
				}
			}
			
			// See if we can find the mapping being requested directly in the table. 
			DslDiagrams::CompartmentMapping[] returnValue;
			if(compartmentMappings.TryGetValue(melType, out returnValue))
			{
				return returnValue;
			}
			
			// If not, loop through the types in the table, and find the 'most derived' base
			// class of melType. 
			global::System.Type selectedMappedType = null;
			foreach(global::System.Type mappedType in compartmentMappings.Keys)
			{
				if(mappedType.IsAssignableFrom(melType) && (selectedMappedType==null || selectedMappedType.IsAssignableFrom(mappedType)))
				{
					selectedMappedType = mappedType;
				}
			}
			if(selectedMappedType!=null)
			{
				return compartmentMappings[selectedMappedType];
			}
			return new DslDiagrams::CompartmentMapping[] {};
		}
		
			#region DomainPath traversal methods to get the list of elements to display in a compartment.
			internal static global::System.Collections.IList GetElementsFromTableForColumnsCompartment(DslModeling::ModelElement element)
			{
				global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Table root = (global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Table)element;
					// Segments 0 and 1
					DslModeling::LinkedElementCollection<global::ORMSolutions.ORMArchitect.RelationalModels.ConceptualDatabase.Column> result = root.ColumnCollection;
				return result;
			}
			#endregion
		
		#endregion
		#region Constructors, domain class Id
	
		/// <summary>
		/// TableShape domain class Id.
		/// </summary>
		public static readonly new global::System.Guid DomainClassId = new global::System.Guid(0x50dabfcd, 0x909c, 0x418a, 0x88, 0x95, 0x17, 0x2a, 0xad, 0xaa, 0xd4, 0xfb);
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		protected TableShapeBase(DslModeling::Partition partition, DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
		#region UpdateCounter domain property code
		
		/// <summary>
		/// UpdateCounter domain property Id.
		/// </summary>
		public static readonly global::System.Guid UpdateCounterDomainPropertyId = new global::System.Guid(0x9d49ffbb, 0x3bb3, 0x4711, 0xb8, 0x8e, 0x9d, 0xbb, 0xd9, 0x84, 0xc6, 0x3d);
		
		/// <summary>
		/// Gets or sets the value of UpdateCounter domain property.
		/// Description for ORMSolutions.ORMArchitect.Views.RelationalView.TableShape.Update
		/// Counter
		/// </summary>
		[DslDesign::DisplayNameResource("ORMSolutions.ORMArchitect.Views.RelationalView.TableShape/UpdateCounter.DisplayName", typeof(global::ORMSolutions.ORMArchitect.Views.RelationalView.RelationalShapeDomainModel), "ORMSolutions.ORMArchitect.Views.RelationalView.GeneratedCode.DomainModelResx")]
		[DslDesign::DescriptionResource("ORMSolutions.ORMArchitect.Views.RelationalView.TableShape/UpdateCounter.Description", typeof(global::ORMSolutions.ORMArchitect.Views.RelationalView.RelationalShapeDomainModel), "ORMSolutions.ORMArchitect.Views.RelationalView.GeneratedCode.DomainModelResx")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.ReadOnly(true)]
		[DslModeling::DomainProperty(Kind = DslModeling::DomainPropertyKind.CustomStorage)]
		[DslModeling::DomainObjectId("9d49ffbb-3bb3-4711-b88e-9dbbd984c63d")]
		private global::System.Int64 UpdateCounter
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return UpdateCounterPropertyHandler.Instance.GetValue(this);
			}
			[global::System.Diagnostics.DebuggerStepThrough]
			set
			{
				UpdateCounterPropertyHandler.Instance.SetValue(this, value);
			}
		}
		/// <summary>
		/// Value handler for the TableShape.UpdateCounter domain property.
		/// </summary>
		internal sealed partial class UpdateCounterPropertyHandler : DslModeling::DomainPropertyValueHandler<TableShapeBase, global::System.Int64>
		{
			private UpdateCounterPropertyHandler() { }
		
			/// <summary>
			/// Gets the singleton instance of the TableShape.UpdateCounter domain property value handler.
			/// </summary>
			public static readonly UpdateCounterPropertyHandler Instance = new UpdateCounterPropertyHandler();
		
			/// <summary>
			/// Gets the Id of the TableShape.UpdateCounter domain property.
			/// </summary>
			public sealed override global::System.Guid DomainPropertyId
			{
				[global::System.Diagnostics.DebuggerStepThrough]
				get
				{
					return UpdateCounterDomainPropertyId;
				}
			}
			
			/// <summary>
			/// Gets a strongly-typed value of the property on specified element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <returns>Property value.</returns>
			public override sealed global::System.Int64 GetValue(TableShapeBase element)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
				// There is no storage for UpdateCounter because its Kind is
				// set to CustomStorage. Please provide the GetUpdateCounterValue()
				// method on the domain class.
				return element.GetUpdateCounterValue();
			}
		
			/// <summary>
			/// Sets property value on an element.
			/// </summary>
			/// <param name="element">Element which owns the property.</param>
			/// <param name="newValue">New property value.</param>
			public override sealed void SetValue(TableShapeBase element, global::System.Int64 newValue)
			{
				if (element == null) throw new global::System.ArgumentNullException("element");
		
				global::System.Int64 oldValue = GetValue(element);
				if (newValue != oldValue)
				{
					ValueChanging(element, oldValue, newValue);
					// There is no storage for UpdateCounter because its Kind is
					// set to CustomStorage. Please provide the SetUpdateCounterValue()
					// method on the domain class.
					element.SetUpdateCounterValue(newValue);
					//ValueChanged(element, oldValue, GetValue(element));
					ValueChanged(element, oldValue, newValue);
				}
			}
		}
		
		#endregion
	}
	/// <summary>
	/// DomainClass TableShape
	/// Description for ORMSolutions.ORMArchitect.Views.RelationalView.TableShape
	/// </summary>
			
	internal partial class TableShape : TableShapeBase
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public TableShape(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public TableShape(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
			: base(partition, propertyAssignments)
		{
		}
		#endregion
	}
}

