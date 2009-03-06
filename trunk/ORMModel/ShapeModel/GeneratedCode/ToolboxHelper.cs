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

namespace ORMSolutions.ORMArchitect.Core.ShapeModel
{
	/// <summary>
	/// Helper class used to create and initialize toolbox items for this DSL.
	/// </summary>
	/// <remarks>
	/// Double-derived class to allow easier code customization.
	/// </remarks>
	public partial class ORMShapeToolboxHelper : ORMShapeToolboxHelperBase 
	{
		/// <summary>
		/// Constructs a new ORMShapeToolboxHelper.
		/// </summary>
		public ORMShapeToolboxHelper(global::System.IServiceProvider serviceProvider)
			: base(serviceProvider) { }
	}
	
	/// <summary>
	/// Helper class used to create and initialize toolbox items for this DSL.
	/// </summary>
	public abstract class ORMShapeToolboxHelperBase
	{
		/// <summary>
		/// Toolbox item filter string used to identify ORMShape toolbox items.  
		/// </summary>
		/// <remarks>
		/// See the MSDN documentation for the ToolboxItemFilterAttribute class for more information on toolbox
		/// item filters.
		/// </remarks>
		public const string ToolboxFilterString = "ORMShape.1.0";
		/// <summary>
		/// Toolbox item filter string used to identify RoleConnector connector tool.
		/// </summary>
		public const string RoleConnectorFilterString = "RoleConnector.1.0";
		/// <summary>
		/// Toolbox item filter string used to identify SubtypeConnector connector tool.
		/// </summary>
		public const string SubtypeConnectorFilterString = "SubtypeConnector.1.0";
		/// <summary>
		/// Toolbox item filter string used to identify ExternalConstraintConnector connector tool.
		/// </summary>
		public const string ExternalConstraintConnectorFilterString = "ExternalConstraintConnector.1.0";
		/// <summary>
		/// Toolbox item filter string used to identify ModelNoteConnector connector tool.
		/// </summary>
		public const string ModelNoteConnectorFilterString = "ModelNoteConnector.1.0";

		private global::System.IServiceProvider sp;
		
		/// <summary>
		/// Constructs a new ORMShapeToolboxHelperBase.
		/// </summary>
		protected ORMShapeToolboxHelperBase(global::System.IServiceProvider serviceProvider)
		{
			if(serviceProvider == null) throw new global::System.ArgumentNullException("serviceProvider");
			
			this.sp = serviceProvider;
		}
		
		/// <summary>
		/// Serivce provider used to access services from the hosting environment.
		/// </summary>
		protected global::System.IServiceProvider ServiceProvider
		{
			get
			{
				return this.sp;
			}
		}

		/// <summary>
		/// Returns the display name of the tab that should be opened by default when the editor is opened.
		/// </summary>
		public static string DefaultToolboxTabName
		{
			get
			{
				return global::ORMSolutions.ORMArchitect.Core.ShapeModel.ORMShapeDomainModel.SingletonResourceManager.GetString("ORM DesignerToolboxTab", global::System.Globalization.CultureInfo.CurrentUICulture);
			}
		}
		
		
		/// <summary>
		/// Returns the toolbox items count in the default tool box tab.
		/// </summary>
		public static int DefaultToolboxTabToolboxItemsCount
		{
			get
			{
				return 20;
			}
		}
		

		/// <summary>
		/// Returns a list of toolbox items for use with this DSL.
		/// </summary>
		public virtual global::System.Collections.Generic.IList<DslDesign::ModelingToolboxItem> CreateToolboxItems()
		{
			global::System.Collections.Generic.List<DslDesign::ModelingToolboxItem> toolboxItems = new global::System.Collections.Generic.List<DslDesign::ModelingToolboxItem>();
			
			// Create store and load domain models.
			using(DslModeling::Store store = new DslModeling::Store(this.ServiceProvider))
			{
				store.LoadDomainModels(typeof(DslDiagrams::CoreDesignSurfaceDomainModel),
					typeof(global::ORMSolutions.ORMArchitect.Framework.FrameworkDomainModel),
					typeof(global::ORMSolutions.ORMArchitect.Core.ObjectModel.ORMCoreDomainModel),
					typeof(global::Microsoft.VisualStudio.Modeling.Diagrams.CoreDesignSurfaceDomainModel),
					typeof(global::ORMSolutions.ORMArchitect.Core.ShapeModel.ORMShapeDomainModel));
				global::System.Resources.ResourceManager resourceManager = global::ORMSolutions.ORMArchitect.Core.ShapeModel.ORMShapeDomainModel.SingletonResourceManager;
				global::System.Globalization.CultureInfo resourceCulture = global::System.Globalization.CultureInfo.CurrentUICulture;
			
				// Open transaction so we can create model elements corresponding to toolbox items.
				using(DslModeling::Transaction t = store.TransactionManager.BeginTransaction("CreateToolboxItems"))
				{

					// Add EntityType shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"EntityTypeToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						1, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("EntityTypeToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("EntityTypeToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"EntityType", // F1 help keyword for the toolbox item.
						resourceManager.GetString("EntityTypeToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.ObjectType.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ValueType shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ValueTypeToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						2, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ValueTypeToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ValueTypeToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ValueType", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ValueTypeToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.ObjectType.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ObjectifiedFactType shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ObjectifiedFactTypeToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						3, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ObjectifiedFactTypeToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ObjectifiedFactTypeToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ObjectifiedFactType", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ObjectifiedFactTypeToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.ObjectType.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add UnaryFactType shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"UnaryFactTypeToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						4, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("UnaryFactTypeToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("UnaryFactTypeToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"UnaryFactType", // F1 help keyword for the toolbox item.
						resourceManager.GetString("UnaryFactTypeToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.FactType.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add BinaryFactType shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"BinaryFactTypeToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						5, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("BinaryFactTypeToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("BinaryFactTypeToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"BinaryFactType", // F1 help keyword for the toolbox item.
						resourceManager.GetString("BinaryFactTypeToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.FactType.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add TernaryFactType shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"TernaryFactTypeToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						6, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("TernaryFactTypeToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("TernaryFactTypeToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"TernaryFactType", // F1 help keyword for the toolbox item.
						resourceManager.GetString("TernaryFactTypeToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.FactType.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add RoleConnector connector tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"RoleConnectorToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						7, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("RoleConnectorToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("RoleConnectorToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.				
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"RoleConnector", // F1 help keyword for the toolbox item.
						resourceManager.GetString("RoleConnectorToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						null, // Connector toolbox items do not have an underlying data object.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require), 
							new global::System.ComponentModel.ToolboxItemFilterAttribute(RoleConnectorFilterString)
						}));

					// Add SubtypeConnector connector tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"SubtypeConnectorToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						8, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("SubtypeConnectorToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("SubtypeConnectorToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.				
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"SubtypeConnector", // F1 help keyword for the toolbox item.
						resourceManager.GetString("SubtypeConnectorToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						null, // Connector toolbox items do not have an underlying data object.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require), 
							new global::System.ComponentModel.ToolboxItemFilterAttribute(SubtypeConnectorFilterString)
						}));

					// Add InternalUniquenessConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"InternalUniquenessConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						9, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("InternalUniquenessConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("InternalUniquenessConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"InternalUniquenessConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("InternalUniquenessConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.UniquenessConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ExternalUniquenessConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ExternalUniquenessConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						10, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ExternalUniquenessConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ExternalUniquenessConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ExternalUniquenessConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ExternalUniquenessConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.UniquenessConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add EqualityConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"EqualityConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						11, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("EqualityConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("EqualityConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"EqualityConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("EqualityConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.EqualityConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ExclusionConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ExclusionConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						12, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ExclusionConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ExclusionConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ExclusionConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ExclusionConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.ExclusionConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add InclusiveOrConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"InclusiveOrConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						13, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("InclusiveOrConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("InclusiveOrConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"InclusiveOrConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("InclusiveOrConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.MandatoryConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ExclusiveOrConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ExclusiveOrConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						14, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ExclusiveOrConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ExclusiveOrConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ExclusiveOrConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ExclusiveOrConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.ExclusiveOrConstraintCoupler.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add SubsetConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"SubsetConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						15, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("SubsetConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("SubsetConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"SubsetConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("SubsetConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.SubsetConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add FrequencyConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"FrequencyConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						16, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("FrequencyConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("FrequencyConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"FrequencyConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("FrequencyConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.FrequencyConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add RingConstraint shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"RingConstraintToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						17, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("RingConstraintToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("RingConstraintToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"RingConstraint", // F1 help keyword for the toolbox item.
						resourceManager.GetString("RingConstraintToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.RingConstraint.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ExternalConstraintConnector connector tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ExternalConstraintConnectorToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						18, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ExternalConstraintConnectorToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ExternalConstraintConnectorToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.				
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ExternalConstraintConnector", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ExternalConstraintConnectorToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						null, // Connector toolbox items do not have an underlying data object.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require), 
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ExternalConstraintConnectorFilterString)
						}));

					// Add ModelNote shape tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ModelNoteToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						19, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ModelNoteToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ModelNoteToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ModelNote", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ModelNoteToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						CreateElementToolPrototype(store, global::ORMSolutions.ORMArchitect.Core.ObjectModel.ModelNote.DomainClassId), // ElementGroupPrototype (data object) representing model element on the toolbox.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require) 
						}));

					// Add ModelNoteConnector connector tool.
					toolboxItems.Add(new DslDesign::ModelingToolboxItem(
						"ModelNoteConnectorToolboxItem", // Unique identifier (non-localized) for the toolbox item.
						20, // Position relative to other items in the same toolbox tab.
						resourceManager.GetString("ModelNoteConnectorToolboxItem", resourceCulture), // Localized display name for the item.
						(global::System.Drawing.Bitmap)DslDiagrams::ImageHelper.GetImage(resourceManager.GetObject("ModelNoteConnectorToolboxBitmap", resourceCulture)), // Image displayed next to the toolbox item.				
						"ORM DesignerToolboxTab", // Unique identifier (non-localized) for the toolbox item tab.
						resourceManager.GetString("ORM DesignerToolboxTab", resourceCulture), // Localized display name for the toolbox tab.
						"ModelNoteConnector", // F1 help keyword for the toolbox item.
						resourceManager.GetString("ModelNoteConnectorToolboxTooltip", resourceCulture), // Localized tooltip text for the toolbox item.
						null, // Connector toolbox items do not have an underlying data object.
						new global::System.ComponentModel.ToolboxItemFilterAttribute[] { // Collection of ToolboxItemFilterAttribute objects that determine visibility of the toolbox item.
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ToolboxFilterString, global::System.ComponentModel.ToolboxItemFilterType.Require), 
							new global::System.ComponentModel.ToolboxItemFilterAttribute(ModelNoteConnectorFilterString)
						}));

					t.Rollback();
				}
			}

			return toolboxItems;
		}
		
		/// <summary>
		/// Creates an ElementGroupPrototype for the element tool corresponding to the given domain class id.
		/// Default behavior is to create a prototype containing an instance of the domain class.
		/// Derived classes may override this to add additional information to the prototype.
		/// </summary>
		protected virtual DslModeling::ElementGroupPrototype CreateElementToolPrototype(DslModeling::Store store, global::System.Guid domainClassId)
		{
			DslModeling::ModelElement element = store.ElementFactory.CreateElement(domainClassId);
			DslModeling::ElementGroup elementGroup = new DslModeling::ElementGroup(store.DefaultPartition);
			elementGroup.AddGraph(element, true);
			return elementGroup.CreatePrototype();
		}
	}
}
