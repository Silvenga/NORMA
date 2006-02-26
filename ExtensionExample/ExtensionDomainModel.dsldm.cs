﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// Common Public License Copyright Notice
// /**************************************************************************\
// * Neumont Object-Role Modeling Architect for Visual Studio                 *
// *                                                                          *
// * Copyright © Neumont University. All rights reserved.                     *
// *                                                                          *
// * The use and distribution terms for this software are covered by the      *
// * Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
// * can be found in the file CPL.txt at the root of this distribution.       *
// * By using this software in any fashion, you are agreeing to be bound by   *
// * the terms of this license.                                               *
// *                                                                          *
// * You must not remove this notice, or any other, from this software.       *
// \**************************************************************************/
namespace ExtensionExample
{
	/// <summary>
	/// 
	/// </summary>
	[Microsoft.VisualStudio.Modeling.InheritBaseModel("2b131234-7959-458d-834f-2dc0769ce683")]
	[System.CLSCompliant(true)]
	[System.Serializable]
	[Microsoft.VisualStudio.Modeling.MetaObject(ExtensionExample.ExtensionDomainModel.MetaModelGuidString, "ExtensionExample.ExtensionDomainModel")]
	public  partial class ExtensionDomainModel : Microsoft.VisualStudio.Modeling.SubStore
	{
		#region ExtensionDomainModel's Generated MetaClass Code
		/// <summary>
		/// MetaModel Guid String
		/// </summary>
		public const System.String MetaModelGuidString = "9f620b5a-9a99-45a4-a022-c9ed95ce85d6";
		/// <summary>
		/// MetaModel Guid
		/// </summary>
		public static readonly System.Guid MetaModelGuid = new System.Guid(ExtensionExample.ExtensionDomainModel.MetaModelGuidString);
		/// <summary>
		/// Default Constructor called by the IMS -- do not call directly
		/// </summary>
		public ExtensionDomainModel() : base()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public ExtensionDomainModel(Microsoft.VisualStudio.Modeling.Store store) : base(store.DefaultPartition, ExtensionExample.ExtensionDomainModel.MetaModelGuid)
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public ExtensionDomainModel(Microsoft.VisualStudio.Modeling.Partition partition) : base(partition, ExtensionExample.ExtensionDomainModel.MetaModelGuid)
		{
		}
		#endregion

	}
	#region ExtensionDomainModel's ResourceManager Code
	public  partial class ExtensionDomainModel
	{
		private static System.Resources.ResourceManager resourceManager = null;
		/// <summary>
		/// The base name of this models resources.
		/// </summary>
		public const string ResourceBaseName = "ExtensionExample.ExtensionDomainModel";
		/// <summary>
		/// Returns the SubStore's ResourceManager. If the ResourceManager does not already exist, then it is created.
		/// </summary>
		public override System.Resources.ResourceManager ResourceManager
		{
			get
			{
				return ExtensionExample.ExtensionDomainModel.SingletonResourceManager;
			}
		}
		/// <summary>
		/// A internal object used for synchronization.
		/// </summary>
		private static object internalSyncObject;
		/// <summary>
		/// Gets the internal object used for synchronization.
		/// </summary>
		private static object InternalSyncObject 
		{
			get 
			{
				if (internalSyncObject == null) 
				{
					object o = new object();
					System.Threading.Interlocked.CompareExchange(ref internalSyncObject, o, null);
				}
				return internalSyncObject;
			}
		}
		/// <summary>
		/// Gets the Singleton ResourceManager for this SubStore
		/// </summary>
		public static System.Resources.ResourceManager SingletonResourceManager
		{
			get
			{
				if (ExtensionExample.ExtensionDomainModel.resourceManager == null)
				{
					lock (ExtensionExample.ExtensionDomainModel.InternalSyncObject)
					{
						if (ExtensionExample.ExtensionDomainModel.resourceManager == null)
						{
							ExtensionExample.ExtensionDomainModel.resourceManager = new System.Resources.ResourceManager(ResourceBaseName, typeof(ExtensionExample.ExtensionDomainModel).Assembly);
						}
					}
				}
				return ExtensionExample.ExtensionDomainModel.resourceManager;
			}
		}
	}
	#endregion
	/// <summary>
	/// Copy closure visitor filter
	/// </summary>
	[System.CLSCompliant(true)]
	[System.Serializable]
	public  class ExtensionDomainModelCopyClosure : Microsoft.VisualStudio.Modeling.IElementVisitorFilter
	{
		/// <summary>
		/// MetaRoles
		/// </summary>
		private System.Collections.Generic.Dictionary<System.Guid, System.Guid> metaRolesMember;
		/// <summary>
		/// Constructor
		/// </summary>
		public ExtensionDomainModelCopyClosure()
		{
			#region Initialize MetaData Table
			#endregion
		}
		/// <summary>
		/// Called to ask the filter if a particular relationship from a source element should be included in the traversal
		/// </summary>
		/// <param name="walker">ElementWalker traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="sourceRoleInfo">MetaRoleInfo of the role that the source element is playing in the relationship</param>
		/// <param name="metaRelationshipInfo">MetaRelationshipInfo for the ElementLink in question</param>
		/// <param name="targetRelationship">Relationship in question</param>
		/// <returns>Yes if the relationship should be traversed</returns>
		public virtual Microsoft.VisualStudio.Modeling.VisitorFilterResult ShouldVisitRelationship(Microsoft.VisualStudio.Modeling.ElementWalker walker, Microsoft.VisualStudio.Modeling.ModelElement sourceElement, Microsoft.VisualStudio.Modeling.MetaRoleInfo sourceRoleInfo, Microsoft.VisualStudio.Modeling.MetaRelationshipInfo metaRelationshipInfo, Microsoft.VisualStudio.Modeling.ElementLink targetRelationship)
		{
			return this.MetaRoles.ContainsKey(sourceRoleInfo.Id) ? Microsoft.VisualStudio.Modeling.VisitorFilterResult.Yes : Microsoft.VisualStudio.Modeling.VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// Called to ask the filter if a particular role player should be Visited during traversal
		/// </summary>
		/// <param name="walker">ElementWalker traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="elementLink">Element Link that forms the relationship to the role player in question</param>
		/// <param name="targetRoleInfo">MetaRoleInfo of the target role</param>
		/// <param name="targetRolePlayer">Model Element that plays the target role in the relationship</param>
		/// <returns></returns>
		public virtual Microsoft.VisualStudio.Modeling.VisitorFilterResult ShouldVisitRolePlayer(Microsoft.VisualStudio.Modeling.ElementWalker walker, Microsoft.VisualStudio.Modeling.ModelElement sourceElement, Microsoft.VisualStudio.Modeling.ElementLink elementLink, Microsoft.VisualStudio.Modeling.MetaRoleInfo targetRoleInfo, Microsoft.VisualStudio.Modeling.ModelElement targetRolePlayer)
		{
			foreach (Microsoft.VisualStudio.Modeling.MetaRoleInfo metaRoleInfo in elementLink.MetaRelationship.MetaRoles)
			{
				if (metaRoleInfo != targetRoleInfo && this.MetaRoles.ContainsKey(metaRoleInfo.Id))
				{
					return Microsoft.VisualStudio.Modeling.VisitorFilterResult.Yes;
				}
			}
			return Microsoft.VisualStudio.Modeling.VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// MetaRoles
		/// </summary>
		private System.Collections.Generic.Dictionary<System.Guid, System.Guid> MetaRoles
		{
			get
			{
				if (this.metaRolesMember == null)
				{
					this.metaRolesMember = new System.Collections.Generic.Dictionary<System.Guid, System.Guid>();
				}
				return this.metaRolesMember;
			}
		}

	}
	/// <summary>
	/// Remove closure visitor filter
	/// </summary>
	[System.CLSCompliant(true)]
	[System.Serializable]
	public  class ExtensionDomainModelRemoveClosure : Microsoft.VisualStudio.Modeling.IElementVisitorFilter
	{
		/// <summary>
		/// MetaRoles
		/// </summary>
		private System.Collections.Generic.Dictionary<System.Guid, System.Guid> metaRolesMember;
		/// <summary>
		/// Constructor
		/// </summary>
		public ExtensionDomainModelRemoveClosure()
		{
			#region Initialize MetaData Table
			#endregion
		}
		/// <summary>
		/// Called to ask the filter if a particular relationship from a source element should be included in the traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="sourceRoleInfo">MetaRoleInfo of the role that the source element is playing in the relationship</param>
		/// <param name="metaRelationshipInfo">MetaRelationshipInfo for the ElementLink in question</param>
		/// <param name="targetRelationship">Relationship in question</param>
		/// <returns>Yes if the relationship should be traversed</returns>
		public virtual Microsoft.VisualStudio.Modeling.VisitorFilterResult ShouldVisitRelationship(Microsoft.VisualStudio.Modeling.ElementWalker walker, Microsoft.VisualStudio.Modeling.ModelElement sourceElement, Microsoft.VisualStudio.Modeling.MetaRoleInfo sourceRoleInfo, Microsoft.VisualStudio.Modeling.MetaRelationshipInfo metaRelationshipInfo, Microsoft.VisualStudio.Modeling.ElementLink targetRelationship)
		{
			return Microsoft.VisualStudio.Modeling.VisitorFilterResult.Yes;
		}
		/// <summary>
		/// Called to ask the filter if a particular role player should be Visited during traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="elementLink">Element Link that forms the relationship to the role player in question</param>
		/// <param name="targetRoleInfo">MetaRoleInfo of the target role</param>
		/// <param name="targetRolePlayer">Model Element that plays the target role in the relationship</param>
		/// <returns></returns>
		public virtual Microsoft.VisualStudio.Modeling.VisitorFilterResult ShouldVisitRolePlayer(Microsoft.VisualStudio.Modeling.ElementWalker walker, Microsoft.VisualStudio.Modeling.ModelElement sourceElement, Microsoft.VisualStudio.Modeling.ElementLink elementLink, Microsoft.VisualStudio.Modeling.MetaRoleInfo targetRoleInfo, Microsoft.VisualStudio.Modeling.ModelElement targetRolePlayer)
		{
			return this.MetaRoles.ContainsKey(targetRoleInfo.Id) ? Microsoft.VisualStudio.Modeling.VisitorFilterResult.Yes : Microsoft.VisualStudio.Modeling.VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// MetaRoles
		/// </summary>
		private System.Collections.Generic.Dictionary<System.Guid, System.Guid> MetaRoles
		{
			get
			{
				if (this.metaRolesMember == null)
				{
					this.metaRolesMember = new System.Collections.Generic.Dictionary<System.Guid, System.Guid>();
				}
				return this.metaRolesMember;
			}
		}

	}
	/// <summary>
	/// 
	/// </summary>
	public  partial class ExtensionDomainModel : Microsoft.VisualStudio.Modeling.SubStore
	{
		#region ExtensionDomainModel's AllGeneratedMetaModelTypes Code
		/// <summary>
		/// Virtual method that returns a collection of all types for metamodel 
		/// </summary>
		protected sealed override System.Type[] AllGeneratedMetaModelTypes()
		{
			System.Type[] typeArray = new System.Type[]
			{
				typeof(ExtensionExample.ExtensionDomainModel),
				typeof(ExtensionExample.MyCustomExtensionElement),
				typeof(ExtensionExample.MyCustomExtensionElementElementFactoryCreator),

			};
			return typeArray;
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public  partial class ExtensionDomainModel : Microsoft.VisualStudio.Modeling.SubStore
	{
		#region ExtensionDomainModel's GeneratedReflectedMetaAttributes Code
		/// <summary>
		/// Virtual method that returns a collection of all fields for all types in metamodel ExtensionDomainModel
		/// </summary>
		protected sealed override Microsoft.VisualStudio.Modeling.MetaFieldInfo[] GeneratedReflectedMetaAttributes()
		{
			Microsoft.VisualStudio.Modeling.MetaFieldInfo[] typeArray = new Microsoft.VisualStudio.Modeling.MetaFieldInfo[]
			{
				new Microsoft.VisualStudio.Modeling.MetaFieldInfo(typeof(ExtensionExample.MyCustomExtensionElement), "TestProperty", ExtensionExample.MyCustomExtensionElement.TestPropertyMetaAttributeGuid, typeof(ExtensionExample.MyCustomExtensionElement.MyCustomExtensionElementTestPropertyFieldHandler)),
				new Microsoft.VisualStudio.Modeling.MetaFieldInfo(typeof(ExtensionExample.MyCustomExtensionElement), "CustomEnum", ExtensionExample.MyCustomExtensionElement.CustomEnumMetaAttributeGuid, typeof(ExtensionExample.MyCustomExtensionElement.MyCustomExtensionElementCustomEnumFieldHandler)),

			};
			return typeArray;
		}
		#endregion
	}
}
namespace ExtensionExample
{
	/// <summary>
	/// 
	/// </summary>
	[System.CLSCompliant(true)]
	[System.Serializable]
	[Microsoft.VisualStudio.Modeling.MetaClass("9f620b5a-9a99-45a4-a022-c9ed95ce85d6")]
	[Microsoft.VisualStudio.Modeling.MetaObject(ExtensionExample.MyCustomExtensionElement.MetaClassGuidString, "ExtensionExample.MyCustomExtensionElement")]
	public  partial class MyCustomExtensionElement : Microsoft.VisualStudio.Modeling.ModelElement
	{
		#region MyCustomExtensionElement's Generated MetaClass Code
		/// <summary>
		/// MetaClass Guid String
		/// </summary>
		public new const System.String MetaClassGuidString = "14db7e59-72e3-441f-9993-88fb3e3c01b3";
		/// <summary>
		/// MetaClass Guid
		/// </summary>
		public static readonly new System.Guid MetaClassGuid = new System.Guid(ExtensionExample.MyCustomExtensionElement.MetaClassGuidString);
		#endregion

		#region TestProperty's Generated  Field Code
		#region TestProperty's Generated  MetaAttribute Code
		/// <summary>
		/// MetaAttribute Guid String
		/// </summary>
		public const System.String TestPropertyMetaAttributeGuidString = "6825c613-7e2a-4d14-8277-0db3b86b1210";

		/// <summary>
		/// MetaAttribute Guid
		/// </summary>
		public static readonly System.Guid TestPropertyMetaAttributeGuid = new System.Guid(ExtensionExample.MyCustomExtensionElement.TestPropertyMetaAttributeGuidString);
		#endregion

		#region TestProperty's Generated Property Code

		private System.String testPropertyPropertyStorage = "Default value";
		
		/// <summary>
		/// 
		/// </summary>
		[System.ComponentModel.Editor(typeof(TestElementPicker), typeof(System.Drawing.Design.UITypeEditor))]
		[Microsoft.VisualStudio.Modeling.StringDomainAttribute(DefaultString="Default value")]
		[Microsoft.VisualStudio.Modeling.MetaAttributeAttribute(FieldHandlerType=typeof(MyCustomExtensionElementTestPropertyFieldHandler))]
		[Microsoft.VisualStudio.Modeling.MetaObject(ExtensionExample.MyCustomExtensionElement.TestPropertyMetaAttributeGuidString, "ExtensionExample.MyCustomExtensionElement.TestProperty")]
		public  System.String TestProperty
		{
			get
			{
				return testPropertyPropertyStorage;
			}
		
			set
			{
				myCustomExtensionElementTestPropertyFieldHandler.SetFieldValue(this, value, false, Microsoft.VisualStudio.Modeling.TransactionManager.CommandFactory);
			}
		}
		#endregion

		#region MyCustomExtensionElementTestPropertyFieldHandler Generated Code
		/// <summary>
		/// FieldHandler for MyCustomExtensionElement.TestProperty field
		/// </summary>
		private static MyCustomExtensionElementTestPropertyFieldHandler	myCustomExtensionElementTestPropertyFieldHandler	= MyCustomExtensionElementTestPropertyFieldHandler.Instance;

		/// <summary>
		/// Implement the field handler for MyCustomExtensionElement.TestProperty
		/// </summary>
		[System.CLSCompliant(false)]
		public sealed partial class MyCustomExtensionElementTestPropertyFieldHandler : Microsoft.VisualStudio.Modeling.TypedModelElementInlineFieldHandler<ExtensionExample.MyCustomExtensionElement,System.String>
		{
			/// <summary>
			/// Constructor
			/// </summary>
			private MyCustomExtensionElementTestPropertyFieldHandler() { }

			/// <summary>
			/// Returns the singleton instance of the MyCustomExtensionElement.TestProperty field handler
			/// </summary>
			/// <value>MyCustomExtensionElementTestPropertyFieldHandler</value>
			public static MyCustomExtensionElementTestPropertyFieldHandler Instance
			{
				get
				{
					if (ExtensionExample.MyCustomExtensionElement.myCustomExtensionElementTestPropertyFieldHandler != null)
					{
						return ExtensionExample.MyCustomExtensionElement.myCustomExtensionElementTestPropertyFieldHandler;
					}
					else
					{
						// The static constructor in MyCustomExtensionElement will assign this value to
						// ExtensionExample.MyCustomExtensionElement.myCustomExtensionElementTestPropertyFieldHandler, so just instantiate one and return it
						return new MyCustomExtensionElementTestPropertyFieldHandler();
					}
				}
			}

			/// <summary>
			/// Returns the meta attribute id for the MyCustomExtensionElement.TestProperty field handler
			/// </summary>
			/// <value>Guid</value>
			public sealed override System.Guid Id
			{
				get
				{
					return ExtensionExample.MyCustomExtensionElement.TestPropertyMetaAttributeGuid;
				}
			}
			/// <summary>
			/// Gets the value of the attribute as it exists in the element
			/// </summary>
			/// <param name="element">the MyCustomExtensionElement</param>
			protected sealed override System.String GetValue(ExtensionExample.MyCustomExtensionElement element)
			{
				return element.testPropertyPropertyStorage;
			}

			/// <summary>
			/// Sets the value into the element
			/// </summary>
			/// <param name="element">the element</param>
			/// <param name="value">new value</param>
			/// <param name="commandFactory">the command factory for this change</param>
			/// <param name="allowDuplicates">allow duplicate value to continue to fire rules and events</param>
			/// <param name="oldValue">the old value before the change</param>
			/// <returns>true if the value actually changed</returns>
			protected sealed override bool SetValue(ExtensionExample.MyCustomExtensionElement element, System.String value, Microsoft.VisualStudio.Modeling.CommandFactory commandFactory, bool allowDuplicates, ref System.String oldValue)
			{
				oldValue = element.testPropertyPropertyStorage;
				if (allowDuplicates || oldValue != value)
				{
					OnValueChanging(element, oldValue, value);
					element.testPropertyPropertyStorage = value;
					OnValueChanged(element, oldValue, value);
					return true;
				}
				return false;
			}
		
		}
		#endregion
		#endregion
		
		#region CustomEnum's Generated  Field Code
		#region CustomEnum's Generated  MetaAttribute Code
		/// <summary>
		/// MetaAttribute Guid String
		/// </summary>
		public const System.String CustomEnumMetaAttributeGuidString = "26aaf88d-2051-4d87-b863-bf330d7123bb";

		/// <summary>
		/// MetaAttribute Guid
		/// </summary>
		public static readonly System.Guid CustomEnumMetaAttributeGuid = new System.Guid(ExtensionExample.MyCustomExtensionElement.CustomEnumMetaAttributeGuidString);
		#endregion

		#region CustomEnum's Generated Property Code

		private ExtensionExample.TestEnumeration customEnumPropertyStorage = ExtensionExample.TestEnumeration.None;
		
		/// <summary>
		/// 
		/// </summary>
		[Microsoft.VisualStudio.Modeling.EnumerationDomainAttribute(EnumerationType=typeof(ExtensionExample.TestEnumeration),DefaultEnumerationValueName="None")]
		[Microsoft.VisualStudio.Modeling.MetaAttributeAttribute(FieldHandlerType=typeof(MyCustomExtensionElementCustomEnumFieldHandler))]
		[Microsoft.VisualStudio.Modeling.MetaObject(ExtensionExample.MyCustomExtensionElement.CustomEnumMetaAttributeGuidString, "ExtensionExample.MyCustomExtensionElement.CustomEnum")]
		public  ExtensionExample.TestEnumeration CustomEnum
		{
			get
			{
				return customEnumPropertyStorage;
			}
		
			set
			{
				myCustomExtensionElementCustomEnumFieldHandler.SetFieldValue(this, value, false, Microsoft.VisualStudio.Modeling.TransactionManager.CommandFactory);
			}
		}
		#endregion

		#region MyCustomExtensionElementCustomEnumFieldHandler Generated Code
		/// <summary>
		/// FieldHandler for MyCustomExtensionElement.CustomEnum field
		/// </summary>
		private static MyCustomExtensionElementCustomEnumFieldHandler	myCustomExtensionElementCustomEnumFieldHandler	= MyCustomExtensionElementCustomEnumFieldHandler.Instance;

		/// <summary>
		/// Implement the field handler for MyCustomExtensionElement.CustomEnum
		/// </summary>
		[System.CLSCompliant(false)]
		public sealed partial class MyCustomExtensionElementCustomEnumFieldHandler : Microsoft.VisualStudio.Modeling.TypedModelElementInlineFieldHandler<ExtensionExample.MyCustomExtensionElement,ExtensionExample.TestEnumeration>
		{
			/// <summary>
			/// Constructor
			/// </summary>
			private MyCustomExtensionElementCustomEnumFieldHandler() { }

			/// <summary>
			/// Returns the singleton instance of the MyCustomExtensionElement.CustomEnum field handler
			/// </summary>
			/// <value>MyCustomExtensionElementCustomEnumFieldHandler</value>
			public static MyCustomExtensionElementCustomEnumFieldHandler Instance
			{
				get
				{
					if (ExtensionExample.MyCustomExtensionElement.myCustomExtensionElementCustomEnumFieldHandler != null)
					{
						return ExtensionExample.MyCustomExtensionElement.myCustomExtensionElementCustomEnumFieldHandler;
					}
					else
					{
						// The static constructor in MyCustomExtensionElement will assign this value to
						// ExtensionExample.MyCustomExtensionElement.myCustomExtensionElementCustomEnumFieldHandler, so just instantiate one and return it
						return new MyCustomExtensionElementCustomEnumFieldHandler();
					}
				}
			}

			/// <summary>
			/// Returns the meta attribute id for the MyCustomExtensionElement.CustomEnum field handler
			/// </summary>
			/// <value>Guid</value>
			public sealed override System.Guid Id
			{
				get
				{
					return ExtensionExample.MyCustomExtensionElement.CustomEnumMetaAttributeGuid;
				}
			}
			/// <summary>
			/// Gets the value of the attribute as it exists in the element
			/// </summary>
			/// <param name="element">the MyCustomExtensionElement</param>
			protected sealed override ExtensionExample.TestEnumeration GetValue(ExtensionExample.MyCustomExtensionElement element)
			{
				return element.customEnumPropertyStorage;
			}

			/// <summary>
			/// Sets the value into the element
			/// </summary>
			/// <param name="element">the element</param>
			/// <param name="value">new value</param>
			/// <param name="commandFactory">the command factory for this change</param>
			/// <param name="allowDuplicates">allow duplicate value to continue to fire rules and events</param>
			/// <param name="oldValue">the old value before the change</param>
			/// <returns>true if the value actually changed</returns>
			protected sealed override bool SetValue(ExtensionExample.MyCustomExtensionElement element, ExtensionExample.TestEnumeration value, Microsoft.VisualStudio.Modeling.CommandFactory commandFactory, bool allowDuplicates, ref ExtensionExample.TestEnumeration oldValue)
			{
				oldValue = element.customEnumPropertyStorage;
				if (allowDuplicates || oldValue != value)
				{
					OnValueChanging(element, oldValue, value);
					element.customEnumPropertyStorage = value;
					OnValueChanged(element, oldValue, value);
					return true;
				}
				return false;
			}
		
		}
		#endregion
		#endregion
		
	}
	#region MyCustomExtensionElement's Generated Constructor Code
	public  partial class MyCustomExtensionElement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MyCustomExtensionElement(Microsoft.VisualStudio.Modeling.Store store, Microsoft.VisualStudio.Modeling.ModelDataBag bag) : base(store.DefaultPartition, bag)
		{
		}
		/// <summary>
		/// Class Factory
		/// </summary>
		public static MyCustomExtensionElement CreateMyCustomExtensionElement(Microsoft.VisualStudio.Modeling.Store store)
		{
			return CreateMyCustomExtensionElement(store.DefaultPartition);
		}
		/// <summary>
		/// Class Factory
		/// </summary>
		public static MyCustomExtensionElement CreateAndInitializeMyCustomExtensionElement(Microsoft.VisualStudio.Modeling.Store store, Microsoft.VisualStudio.Modeling.AttributeAssignment[] assignments)
		{
			return CreateAndInitializeMyCustomExtensionElement(store.DefaultPartition, assignments);
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public MyCustomExtensionElement(Microsoft.VisualStudio.Modeling.Partition partition, Microsoft.VisualStudio.Modeling.ModelDataBag bag)
			: base(partition, bag)
		{
		}
		/// <summary>
		/// Class Factory
		/// </summary>
		public static MyCustomExtensionElement CreateMyCustomExtensionElement(Microsoft.VisualStudio.Modeling.Partition partition)
		{
			return (MyCustomExtensionElement)partition.ElementFactory.CreateElement(typeof(MyCustomExtensionElement));
		}
		/// <summary>
		/// Class Factory
		/// </summary>
		public static MyCustomExtensionElement CreateAndInitializeMyCustomExtensionElement(Microsoft.VisualStudio.Modeling.Partition partition, Microsoft.VisualStudio.Modeling.AttributeAssignment[] assignments)
		{
			return (MyCustomExtensionElement)partition.ElementFactory.CreateElement(typeof(MyCustomExtensionElement), assignments);
		}
	}
	#endregion
	#region Class Factory Creator for MyCustomExtensionElement
	/// <summary>
	/// MyCustomExtensionElement Class Factory Creator
	/// </summary>
	[Microsoft.VisualStudio.Modeling.ElementFactoryCreatorFor(typeof(ExtensionExample.MyCustomExtensionElement))]
	public sealed class MyCustomExtensionElementElementFactoryCreator : Microsoft.VisualStudio.Modeling.ElementFactoryCreator
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MyCustomExtensionElementElementFactoryCreator()
		{
		}
		/// <summary>
		/// Class Factory Create Method
		/// </summary>
		public override Microsoft.VisualStudio.Modeling.ModelElement Create(Microsoft.VisualStudio.Modeling.Partition partition, Microsoft.VisualStudio.Modeling.ModelDataBag bag)
		{
			return new ExtensionExample.MyCustomExtensionElement( partition, bag );
		}
		/// <summary>
		/// Create an instance of the createor object
		/// </summary>
		public static MyCustomExtensionElementElementFactoryCreator Instance
		{
			get
			{
				return new MyCustomExtensionElementElementFactoryCreator();
			}
		}
	}
	#endregion

}


