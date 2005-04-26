using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Modeling;
using System.Globalization;

namespace Northface.Tools.ORM.ObjectModel
{
	#region IFactConstraint interface
	/// <summary>
	/// A constraint is defined such that it can have
	/// roles that span multiple fact types. The core
	/// model makes it difficult to determine which roles
	/// on a fact are used by a given constraint. ExternalFactConstraint
	/// and InternalFactConstraint relationships are generated
	/// automatically, but these have significantly different
	/// mechanisms for getting from the fact type to the constraint
	/// and its roles. The IFactConstraint interface is defined to
	/// smooth over this difference.
	/// </summary>
	public interface IFactConstraint
	{
		/// <summary>
		/// Get the constraint instance bound
		/// to the context fact
		/// </summary>
		IConstraint Constraint { get;}
		/// <summary>
		/// Get the roles associated with both the
		/// constraint and the fact.
		/// </summary>
		IList<Role> RoleCollection { get;}
		/// <summary>
		/// Get the fact type instance associated
		/// with this constraint. All roles in RoleCollection
		/// will be parented to this fact.
		/// </summary>
		FactType FactType { get;}
	}
	#endregion // IFactConstraint interface
	public partial class FactType : INamedElementDictionaryChild, IModelErrorOwner
	{
		#region ReadingOrder acquisition
		/// <summary>
		/// Gets a reading order, first by trying to find it, if one doesn't exist
		/// it will then create a new ReadingOrder. It operates under the assumption
		/// that a transaction has already been started.
		/// </summary>
		/// <returns></returns>
		public static ReadingOrder GetReadingOrder(FactType theFact, Role[] roleOrder)
		{
			ReadingOrder retval = FindMatchingReadingOrder(theFact, roleOrder);
			if (retval == null)
			{
				retval = CreateReadingOrder(theFact, roleOrder);
			}
			return retval;
		}

		/// <summary>
		/// Lookes for a ReadingOrder that has the roles in the same order
		/// as the currently selected role order.
		/// </summary>
		/// <returns>The reading order if found, null if it was not.</returns>
		public static ReadingOrder FindMatchingReadingOrder(FactType theFact, Role[] roleOrder)
		{
			ReadingOrder retval = null;
			ReadingOrderMoveableCollection readingOrders = theFact.ReadingOrderCollection;
			foreach (ReadingOrder order in readingOrders)
			{
				RoleMoveableCollection roles = order.RoleCollection;
				int numRoles = roles.Count;
				if (numRoles == roleOrder.Length)
				{
					bool match = true;
					for (int i = 0; i < numRoles; ++i)
					{
						if (roles[i] != roleOrder[i])
						{
							match = false;
							break;
						}
					}
					if (match)
					{
						retval = order;
						break;
					}
				}
			}
			return retval;
		}

		/// <summary>
		/// Gets the reading order that matches the currently displayed order of the
		/// fact that is passed in.
		/// </summary>
		/// <returns>The matching ReadingOrder or null if one does not exist.</returns>
		public static ReadingOrder FindMatchingReadingOrder(FactType theFact)
		{
			RoleMoveableCollection factRoles = theFact.RoleCollection;
			Role[] roleOrder = new Role[factRoles.Count];
			factRoles.CopyTo(roleOrder, 0);
			return FindMatchingReadingOrder(theFact, roleOrder);
		}

		/// <summary>
		/// Creates a new ReadingOrder with the same role sequence as the currently selected one.
		/// A transaction should have been pushed before calling this method. It operates under
		/// the assumption that a transaction has already been started.
		/// </summary>
		/// <returns>Should always return a value unless there was an error creating the ReadingOrder</returns>
		public static ReadingOrder CreateReadingOrder(FactType theFact, Role[] roleOrder)
		{
			ReadingOrder retval = null;
			if (roleOrder.Length > 0)
			{
				retval = ReadingOrder.CreateReadingOrder(theFact.Store);
				RoleMoveableCollection readingRoles = retval.RoleCollection;
				int numRoles = roleOrder.Length;
				for (int i = 0; i < numRoles; ++i)
				{
					readingRoles.Add(roleOrder[i]);
				}
				theFact.ReadingOrderCollection.Add(retval);
			}
			return retval;
		}
		#endregion
		#region FactType Specific
		/// <summary>
		/// Get a read-only collection of FactConstraint links. Use the
		/// appropriate methods on IFactConstraint to get to the Constraint
		/// and RoleCollection values for each returned constraint.
		/// </summary>
		[CLSCompliant(false)]
		public ICollection<IFactConstraint> ExternalFactConstraintCollection
		{
			get
			{
				return new FactConstraintCollectionImpl(this, false, true);
			}
		}
		/// <summary>
		/// Get a collection of all constraints associated with this fact,
		/// along with the roles on this fact that are used by each fact
		/// constraint.
		/// </summary>
		/// <value></value>
		[CLSCompliant(false)]
		public ICollection<IFactConstraint> FactConstraintCollection
		{
			get
			{
				return new FactConstraintCollectionImpl(this, true, true);
			}
		}
		/// <summary>
		/// Get an enumeration of constraints of the given type
		/// </summary>
		/// <param name="filterType">The type of constraint to return</param>
		/// <returns>IEnumerable</returns>
		[CLSCompliant(false)]
		public IEnumerable<InternalConstraint> GetInternalConstraints(ConstraintType filterType)
		{
			IList constraints = InternalConstraintCollection;
			int constraintCount = constraints.Count;
			for (int i = 0; i < constraintCount; ++i)
			{
				InternalConstraint ic = (InternalConstraint)constraints[i];
				if (ic.Constraint.ConstraintType == filterType)
				{
					yield return ic;
				}
			}
		}
		/// <summary>
		/// Get the number of internal constraints of the specified constraint type
		/// </summary>
		/// <param name="filterType">The type of constraint to count</param>
		/// <returns>int</returns>
		public int GetInternalConstraintsCount(ConstraintType filterType)
		{
			int retVal = 0;
			// Count the enumerator without foreach to satisfy FxCop
			IEnumerator<InternalConstraint> ienum = GetInternalConstraints(filterType).GetEnumerator();
			while (ienum.MoveNext())
			{
				++retVal;
			}
			return retVal;
		}
		#endregion // FactType Specific
		#region FactConstraintCollection implementation
		private class FactConstraintCollectionImpl : ICollection<IFactConstraint>
		{
			#region Member Variables
			private IList[] myLists;
			#endregion // Member Variables
			#region Constructors
			/// <summary>
			/// Create a FactConstraint collection for the given fact type. Fact constraints
			/// come from multiple links, this puts them all together.
			/// </summary>
			/// <param name="factType">The parent fact type</param>
			/// <param name="includeInternalConstraints">true to include internal fact constraints</param>
			/// <param name="includeExternalConstraints">true to include external fact constraints</param>
			public FactConstraintCollectionImpl(FactType factType, bool includeInternalConstraints, bool includeExternalConstraints)
			{
				Debug.Assert(includeExternalConstraints || includeExternalConstraints);
				int total = 0;
				if (includeInternalConstraints)
				{
					++total;
				}
				if (includeExternalConstraints)
				{
					total += 2;
				}
				myLists = new IList[total];
				int externalIndex = 0;
				if (includeInternalConstraints)
				{
					myLists[0] = factType.InternalConstraintCollection;
					++externalIndex;
				}
				if (includeExternalConstraints)
				{
					myLists[externalIndex] = factType.GetElementLinks(SingleColumnExternalFactConstraint.FactTypeCollectionMetaRoleGuid);
					myLists[externalIndex + 1] = factType.GetElementLinks(MultiColumnExternalFactConstraint.FactTypeCollectionMetaRoleGuid);
				}
			}
			#endregion // Constructors
			#region ICollection<IFactConstraint> Implementation
			bool ICollection<IFactConstraint>.Contains(IFactConstraint item)
			{
				IList[] lists = myLists;
				int listCount = lists.Length;
				for (int i = 0; i < listCount; ++i)
				{
					if (lists[i].Contains(item))
					{
						return true;
					}
				}
				return false;
			}
			void ICollection<IFactConstraint>.CopyTo(IFactConstraint[] array, int arrayIndex)
			{
				IList[] lists = myLists;
				int listCount = lists.Length;
				int prevTotal = 0;
				for (int i = 0; i < listCount; ++i)
				{
					IList curList = lists[i];
					int curTotal = curList.Count;
					if (curTotal != 0)
					{
						curList.CopyTo(array, prevTotal);
						prevTotal += curTotal;
					}
				}
			}
			int ICollection<IFactConstraint>.Count
			{
				get
				{
					IList[] lists = myLists;
					int listCount = lists.Length;
					int total = 0;
					for (int i = 0; i < listCount; ++i)
					{
						total += lists[i].Count;
					}
					return total;
				}
			}
			bool ICollection<IFactConstraint>.IsReadOnly
			{
				get { return true; }
			}
			bool ICollection<IFactConstraint>.Remove(IFactConstraint item)
			{
				// Not supported for read-only
				throw new InvalidOperationException();
			}
			void ICollection<IFactConstraint>.Add(IFactConstraint item)
			{
				// Not supported for read-only
				throw new InvalidOperationException();
			}
			void ICollection<IFactConstraint>.Clear()
			{
				// Not supported for read-only
				throw new InvalidOperationException();
			}
			#endregion // ICollection<IFactConstraint> Implementation
			#region IEnumerable<IFactConstraint> Implementation
			IEnumerator<IFactConstraint> IEnumerable<IFactConstraint>.GetEnumerator()
			{
				IList[] lists = myLists;
				int listCount = lists.Length;
				for (int i = 0; i < listCount; ++i)
				{
					IList curList = lists[i];
					int curTotal = curList.Count;
					for (int j = 0; j < curTotal; ++j)
					{
						yield return (IFactConstraint)curList[j];
					}
				}
			}
			#endregion // IEnumerable<IFactConstraint> Implementation
		}
		#endregion // FactConstraintCollection implementation
		#region Customize property display
		/// <summary>
		/// Distinguish between an objectified and a
		/// normal fact type.
		/// </summary>
		public override string GetClassName()
		{
			return (NestingType == null) ? ResourceStrings.FactType : ResourceStrings.ObjectifiedFactType;
		}
		#endregion // Customize property display
		#region MergeContext functions
		/// <summary>
		/// Support adding root elements and constraints directly to the design surface
		/// </summary>
		/// <param name="elementGroupPrototype">The object representing the serialized data being added to this FactType.</param>
		/// <param name="protoElement">The element to add.</param>
		/// <returns>True if addition is allowed; otherwise, false.</returns>
		protected override bool CanAddChildElement(ElementGroupPrototype elementGroupPrototype, ProtoElementBase protoElement)
		{
			if (protoElement != null)
			{
				MetaClassInfo classInfo = Store.MetaDataDirectory.FindMetaClass(protoElement.MetaClassId);
				return (classInfo.IsDerivedFrom(InternalUniquenessConstraint.MetaClassGuid));
			}
			return false;
		}

		/// <summary>
		/// Attach a deserialized InternalUniquenessConstraint to this FactType.
		/// Called after prototypes for these items are dropped onto the diagram
		/// from the toolbox.
		/// </summary>
		/// <param name="sourceElement">The element being added</param>
		/// <param name="elementGroup">The element describing all of the created elements</param>
		public override void MergeRelate(ModelElement sourceElement, ElementGroup elementGroup)
		{
			base.MergeRelate(sourceElement, elementGroup);
			InternalUniquenessConstraint internalConstraint;
			if (null != (internalConstraint = sourceElement as InternalUniquenessConstraint))
			{
				internalConstraint.FactType = this;
			}
		}
		#endregion // MergeContext functions
		#region CustomStorage handlers
		/// <summary>
		/// Standard override. All custom storage properties are derived, not
		/// stored. Actual changes are handled in FactTypeChangeRule.
		/// </summary>
		/// <param name="attribute">MetaAttributeInfo</param>
		/// <param name="newValue">object</param>
		public override void SetValueForCustomStoredAttribute(MetaAttributeInfo attribute, object newValue)
		{
			Guid attributeGuid = attribute.Id;
			if (attributeGuid == NestingTypeDisplayMetaAttributeGuid)
			{
				// Handled by FactTypeChangeRule
				return;
			}
			base.SetValueForCustomStoredAttribute(attribute, newValue);
		}
		/// <summary>
		/// Standard override. Retrieve values for calculated properties.
		/// </summary>
		/// <param name="attribute">MetaAttributeInfo</param>
		/// <returns></returns>
		public override object GetValueForCustomStoredAttribute(MetaAttributeInfo attribute)
		{
			Guid attributeGuid = attribute.Id;
			if (attributeGuid == NestingTypeDisplayMetaAttributeGuid)
			{
				return NestingType;
			}
			return base.GetValueForCustomStoredAttribute(attribute);
		}
		/// <summary>
		/// Standard override. Defer to GetValueForCustomStoredAttribute.
		/// </summary>
		/// <param name="attribute">MetaAttributeInfo</param>
		/// <returns></returns>
		protected override object GetOldValueForCustomStoredAttribute(MetaAttributeInfo attribute)
		{
			return GetValueForCustomStoredAttribute(attribute);
		}
		#endregion // CustomStorage handlers
		#region INamedElementDictionaryChild implementation
		void INamedElementDictionaryChild.GetRoleGuids(out Guid parentMetaRoleGuid, out Guid childMetaRoleGuid)
		{
			GetRoleGuids(out parentMetaRoleGuid, out childMetaRoleGuid);
		}
		/// <summary>
		/// Implementation of INamedElementDictionaryChild.GetRoleGuids. Identifies
		/// this child as participating in the 'ModelHasFactType' naming set.
		/// </summary>
		/// <param name="parentMetaRoleGuid">Guid</param>
		/// <param name="childMetaRoleGuid">Guid</param>
		protected void GetRoleGuids(out Guid parentMetaRoleGuid, out Guid childMetaRoleGuid)
		{
			parentMetaRoleGuid = ModelHasFactType.ModelMetaRoleGuid;
			childMetaRoleGuid = ModelHasFactType.FactTypeCollectionMetaRoleGuid;
		}
		#endregion // INamedElementDictionaryChild implementation
		#region RoleChangeRule class
		[RuleOn(typeof(FactType))]
		private class FactTypeChangeRule : ChangeRule
		{
			/// <summary>
			/// Forward through the property grid property to the underlying
			/// nesting type property
			/// </summary>
			/// <param name="e"></param>
			public override void ElementAttributeChanged(ElementAttributeChangedEventArgs e)
			{
				Guid attributeGuid = e.MetaAttribute.Id;
				if (attributeGuid == FactType.NestingTypeDisplayMetaAttributeGuid)
				{
					(e.ModelElement as FactType).NestingType = e.NewValue as ObjectType;
				}
			}
		}
		#endregion // RoleChangeRule class
		#region IModelErrorOwner Members

		/// <summary>
		/// Returns the error associated with the fact.
		/// </summary>
		[CLSCompliant(false)]
		protected IEnumerable<ModelError> ErrorCollection
		{
			get
			{
				FactTypeRequiresReadingError error = this.ReadingRequiredError;
				if (error != null)
				{
					yield return error;
				}
			}
		}
		IEnumerable<ModelError> IModelErrorOwner.ErrorCollection
		{
			get 
			{
				return ErrorCollection;
			}
		}

		/// <summary>
		/// Implements IModelErrorOwner.ValidateErrors
		/// </summary>
		protected void ValidateErrors(INotifyElementAdded notifyAdded)
		{
			ValidateRequiresReading(notifyAdded);
		}
		void IModelErrorOwner.ValidateErrors(INotifyElementAdded notifyAdded)
		{
			ValidateErrors(notifyAdded);
		}

		#region Validation Methods
		private void ValidateRequiresReading(INotifyElementAdded notifyAdded)
		{
			if (!IsRemoved)
			{
				bool hasError = true;
				Store theStore = Store;
				ORMModel theModel = Model;
				ReadingOrderMoveableCollection readingOrders = ReadingOrderCollection;
				if (readingOrders.Count > 0)
				{
					foreach (ReadingOrder order in readingOrders)
					{
						if (order.ReadingCollection.Count > 0)
						{
							hasError = false;
							break;
						}
					}
				}

				FactTypeRequiresReadingError noReadingError = ReadingRequiredError;
				if (hasError)
				{
					if (noReadingError == null)
					{
						noReadingError = FactTypeRequiresReadingError.CreateFactTypeRequiresReadingError(theStore);
						noReadingError.Model = theModel;
						noReadingError.FactType = this;
						noReadingError.GenerateErrorText();
						if (notifyAdded != null)
						{
							notifyAdded.ElementAdded(noReadingError, true);
						}
					}
				}
				else
				{
					if (noReadingError != null)
					{
						noReadingError.Remove();
					}
				}
			}
		}
		#endregion
		#region Reading Required Rules
		[RuleOn(typeof(ModelHasFactType))]
		private class ModelHasFactTypeAddRuleModelValidation : AddRule
		{
			public override void ElementAdded(ElementAddedEventArgs e)
			{
				ModelHasFactType link = e.ModelElement as ModelHasFactType;
				FactType fact = link.FactTypeCollection;
				fact.ValidateErrors(null);
			}
		}

		[RuleOn(typeof(FactTypeHasReadingOrder))]
		private class FactTypeHasReadingOrderAddRuleModelValidation : AddRule
		{
			public override void ElementAdded(ElementAddedEventArgs e)
			{
				FactTypeHasReadingOrder link = e.ModelElement as FactTypeHasReadingOrder;
				FactType fact = link.FactType;
				if (fact.ReadingRequiredError != null)
				{
					fact.ValidateErrors(null);
				}
			}
		}
		[RuleOn(typeof(FactTypeHasReadingOrder), FireTime = TimeToFire.LocalCommit)]
		private class FactTypeHasReadingOrderRemovedRuleModelValidation : RemoveRule
		{
			public override void ElementRemoved(ElementRemovedEventArgs e)
			{
				FactTypeHasReadingOrder link = e.ModelElement as FactTypeHasReadingOrder;
				FactType fact = link.FactType;
				if (!fact.IsRemoved)
				{
					fact.ValidateErrors(null);
				}
			}
		}

		[RuleOn(typeof(ReadingOrderHasReading))]
		private class ReadingOrderHasReadingAddRuleModelValidation : AddRule
		{
			public override void ElementAdded(ElementAddedEventArgs e)
			{
				ReadingOrderHasReading link = e.ModelElement as ReadingOrderHasReading;
				ReadingOrder ord = link.ReadingOrder;
				FactType fact = ord.FactType;
				if (fact != null)
				{
					fact.ValidateErrors(null);
				}
			}
		}
		[RuleOn(typeof(ReadingOrderHasReading), FireTime = TimeToFire.LocalCommit)]
		private class ReadingOrderHasReadingRemoveRuleModelValidation : RemoveRule
		{
			public override void ElementRemoved(ElementRemovedEventArgs e)
			{
				ReadingOrderHasReading link = e.ModelElement as ReadingOrderHasReading;
				ReadingOrder ord = link.ReadingOrder;
				FactType fact;
				if (!ord.IsRemoved &&
					null != (fact = ord.FactType) &&
					!fact.IsRemoved)
				{
					fact.ValidateErrors(null);
				}
			}
		}
		#endregion

		#endregion
	}

	#region FactType Model Validation Errors
	
	#region class FactTypeRequiresReadingError
	partial class FactTypeRequiresReadingError : IRepresentModelElements
	{
		#region overrides

		/// <summary>
		/// Creates error text for when a fact has no readings.
		/// </summary>
		public override void GenerateErrorText()
		{
			string newText = String.Format(CultureInfo.InvariantCulture, ResourceStrings.ModelErrorFactTypeRequiresReadingMessage, FactType.Name, Model.Name);
			if (Name != newText)
			{
				Name = newText;
			}
		}

		/// <summary>
		/// Sets regernate to ModelNameChange | OwnerNameChange
		/// </summary>
		public override RegenerateErrorTextEvents RegenerateEvents
		{
			get 
			{
				return RegenerateErrorTextEvents.ModelNameChange | RegenerateErrorTextEvents.OwnerNameChange;
			}
		}

		#endregion

		#region IRepresentModelElements Members

		/// <summary>
		/// The fact the error belongs to
		/// </summary>
		protected ModelElement[] GetRepresentedElements()
		{
			return new ModelElement[] { this.FactType };
		}

		ModelElement[] IRepresentModelElements.GetRepresentedElements()
		{
			return GetRepresentedElements();
		}
		#endregion
	}
	#endregion // class FactTypeRequiresReadingError

	#endregion // FactType Model Validation Errors
}
