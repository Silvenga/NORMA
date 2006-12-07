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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio.Modeling;
using Neumont.Tools.Modeling;
using Neumont.Tools.Modeling.Shell.DynamicSurveyTreeGrid;

namespace Neumont.Tools.ORM.ObjectModel
{
	/// <summary>
	/// A delegate callback to use with the <see cref="ORMCoreDomainModel.DelayValidateElement"/> method.
	/// The delegate will be called when the current transaction finishes committing.
	/// </summary>
	/// <param name="element">The <see cref="ModelElement"/> to validate</param>
	public delegate void ElementValidation(ModelElement element);
	[VerbalizationSnippetsProvider("VerbalizationSnippets")]
	public partial class ORMCoreDomainModel : IORMModelEventSubscriber, ISurveyNodeProvider
	{
		#region InitializingToolboxItems property
		private static bool myInitializingToolboxItems;
		/// <summary>
		/// Static property to disable rule reflection for fast
		/// model load. This needs to be on a meta model, not the
		/// package, so that the models also load from the command line.
		/// </summary>
		public static bool InitializingToolboxItems
		{
			get
			{
				return myInitializingToolboxItems;
			}
			set
			{
				Debug.Assert(value || myInitializingToolboxItems, "InitializingToolboxItems already turned off");
				myInitializingToolboxItems = value;
			}
		}
		#endregion // InitializingToolboxItems property
		#region TransactionRulesFixupHack class
		[RuleOn(typeof(CoreDomainModel))]
		private sealed class TransactionRulesFixupHack : TransactionBeginningRule
		{
			// UNDONE: There are still a few situations that this doesn't handle:
			//
			// If one or more new DomainModels are loaded during a transaction (before TransactionCommitting but after TransactionBeginning),
			// we will miss resorting the rules since we won't get called until the next transaction, and by then the 'committingRules' field
			// will no longer be null. We could partially work around this by resorting the rules at the start of every transaction (whether
			// or not the field is null), but the extra overhead that would cause isn't really worth it since for our purposes we don't load
			// new DomainModels in the middle of transactions anyway.
			// 
			// The fact that StoreDiagramMappingData is a static singleton and the way in which it is used leads to serious race conditions
			// when multiple Stores are being used at the same time (e.g. if two documents are open). ElementEventArgs from one transaction
			// can and do end up being processed by LineRoutingRule during the commit of completely unrelated transactions in separate Stores.
			// This has lead to some really bizarre behavior and obscure bugs. Unfortunately, we haven't been able to come up with anything
			// that we can do about it, since all of this is happening deep in the bowels of Store and Diagram.

			private static readonly RuntimeFieldHandle CommittingRulesFieldHandle = InitializeCommittingRulesFieldHandle();
			private static RuntimeFieldHandle InitializeCommittingRulesFieldHandle()
			{
				const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;
				Type ruleManagerType = typeof(RuleManager);
				FieldInfo committingRulesFieldInfo = ruleManagerType.GetField("committingRules", bindingFlags);
				return (committingRulesFieldInfo != null && committingRulesFieldInfo.FieldType == typeof(List<TransactionCommittingRule>)) ? committingRulesFieldInfo.FieldHandle : default(RuntimeFieldHandle);
			}
			private delegate void GetCommittingRulesDelegate(RuleManager @this);
			private static readonly GetCommittingRulesDelegate GetCommittingRules = InitializeGetCommittingRules();
			private static GetCommittingRulesDelegate InitializeGetCommittingRules()
			{
				const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;
				Type ruleManagerType = typeof(RuleManager);
				MethodInfo getCommittingRulesMethodInfo = ruleManagerType.GetMethod("GetCommittingRules", bindingFlags, null, CallingConventions.HasThis, Type.EmptyTypes, null);
				return (getCommittingRulesMethodInfo != null) ?
					(GetCommittingRulesDelegate)Delegate.CreateDelegate(typeof(GetCommittingRulesDelegate), getCommittingRulesMethodInfo, false) : null;
			}

			public TransactionRulesFixupHack()
				: base()
			{
				if (CommittingRulesFieldHandle.Value == IntPtr.Zero || (object)GetCommittingRules == null)
				{
					base.IsEnabled = false;
				}
			}

			public sealed override void TransactionBeginning(TransactionBeginningEventArgs e)
			{
				if (!base.IsEnabled)
				{
					// We have to check this ourselves since the RuleManager doesn't bother to...
					return;
				}

				RuleManager ruleManager = e.Transaction.Store.RuleManager;

				FieldInfo committingRulesFieldInfo = FieldInfo.GetFieldFromHandle(CommittingRulesFieldHandle);
				if (committingRulesFieldInfo.GetValue(ruleManager) == null)
				{
					// The committingRules field will only be null if a new DomainModel has been added since the last time this ran,
					// in which case, we need to resort the rules.
					GetCommittingRules(ruleManager);
					List<TransactionCommittingRule> committingRules = (List<TransactionCommittingRule>)committingRulesFieldInfo.GetValue(ruleManager);
					Debug.Assert(committingRules != null);
					committingRules.Sort(RulePriorityComparer<TransactionCommittingRule>.Instance);
				}
			}

			#region RulePriorityComparer class
			private sealed class RulePriorityComparer<TRule> : IComparer<TRule>
				where TRule : Rule
			{
				private RulePriorityComparer()
					: base()
				{
				}
				public static readonly RulePriorityComparer<TRule> Instance = new RulePriorityComparer<TRule>();
				public int Compare(TRule x, TRule y)
				{
					if ((object)x == (object)y)
					{
						return 0;
					}
					else if ((object)x == null)
					{
						return -1;
					}
					else if ((object)y == null)
					{
						return 1;
					}
					else
					{
						int diff = ((int)x.FireTime).CompareTo((int)y.FireTime);
						if (diff == 0)
						{
							diff = x.Priority.CompareTo(y.Priority);
							if (diff == 0)
							{
								diff = x.CompareTo(y);
							}
						}
						return diff;
					}
				}
			}
			#endregion // RulePriorityComparer class
		}
		#endregion // TransactionRulesFixupHack class
		#region Delayed Model Validation
		private static readonly object DelayedValidationContextKey = new object();
		/// <summary>
		/// Class to delay validate rules when a transaction is committing.
		/// </summary>
		[RuleOn(typeof(ORMCoreDomainModel), Priority = Int32.MinValue)] // TransactionCommittingRule
		private sealed partial class DelayValidateElements : TransactionCommittingRule
		{
			public sealed override void TransactionCommitting(TransactionCommitEventArgs e)
			{
				Dictionary<object, object> contextInfo = e.Transaction.Context.ContextInfo;
				object validatorsObject;
				while (contextInfo.TryGetValue(DelayedValidationContextKey, out validatorsObject)) // Let's do a while in case a new validator is added by the other validators
				{
					Dictionary<ElementValidator, object> validators = (Dictionary<ElementValidator, object>)validatorsObject;
					contextInfo.Remove(DelayedValidationContextKey);
					foreach (ElementValidator key in validators.Keys)
					{
						key.Validate();
					}
				}
			}
		}
		/// <summary>
		/// A structure to use for <see cref="ModelElement"/> validation.
		/// </summary>
		private struct ElementValidator : IEquatable<ElementValidator>
		{
			/// <summary>
			/// Initializes a new instance of <see cref="ElementValidator"/>.
			/// </summary>
			public ElementValidator(ModelElement element, ElementValidation validation)
			{
				this.Element = element;
				this.Validation = validation;
			}
			/// <summary>
			/// Invokes <see cref="Validation"/>, passing it <see cref="Element"/>.
			/// </summary>
			public void Validate()
			{
				this.Validation(this.Element);
			}
			/// <summary>
			/// The <see cref="ModelElement"/> to validate.
			/// </summary>
			private readonly ModelElement Element;
			/// <summary>
			/// The callback valdiation function.
			/// </summary>
			private readonly ElementValidation Validation;
			/// <summary>See <see cref="Object.GetHashCode()"/>.</summary>
			public override int GetHashCode()
			{
				return this.Element.GetHashCode() ^ this.Validation.GetHashCode();
			}
			/// <summary>See <see cref="Object.Equals(Object)"/>.</summary>
			public override bool Equals(object obj)
			{
				return obj is ElementValidator && this.Equals((ElementValidator)obj);
			}
			/// <summary>See <see cref="IEquatable{ElementValidator}.Equals"/>.</summary>
			public bool Equals(ElementValidator other)
			{
				return this.Element.Equals(other.Element) && this.Validation.Equals(other.Validation);
			}
		}
		/// <summary>
		/// Called inside a transaction register a callback function that validates an
		/// element when the transaction completes. There are multiple model changes
		/// that can trigger most validation rules. Registering validation rules for delayed
		/// validation ensures that the validation code only runs once for any given object.
		/// </summary>
		/// <param name="element">The element to validate</param>
		/// <param name="validation">The validation function to run</param>
		/// <returns>true if this element/validator pair is being added for the first time in this transaction</returns>
		public static bool DelayValidateElement(ModelElement element, ElementValidation validation)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if ((object)validation == null)
			{
				throw new ArgumentNullException("validation");
			}
			Store store = element.Store;
			Debug.Assert(store.TransactionActive);
			Dictionary<object, object> contextDictionary = store.TransactionManager.CurrentTransaction.TopLevelTransaction.Context.ContextInfo;
			object dictionaryObject;
			Dictionary<ElementValidator, object> dictionary;
			ElementValidator key = new ElementValidator(element, validation);
			if (contextDictionary.TryGetValue(DelayedValidationContextKey, out dictionaryObject))
			{
				dictionary = (Dictionary<ElementValidator, object>)dictionaryObject;
				if (dictionary.ContainsKey(key))
				{
					return false; // We're already validating this one
				}
			}
			else
			{
				contextDictionary[DelayedValidationContextKey] = dictionary = new Dictionary<ElementValidator, object>();
			}
			dictionary[key] = null;
			return true;
		}
		#endregion // Delayed Model Validation
		#region IORMModelEventSubscriber Implementation
		/// <summary>
		/// Implements <see cref="IORMModelEventSubscriber.ManagePreLoadModelingEventHandlers"/>.
		/// This implementation does nothing and does not need to be called.
		/// </summary>
		void IORMModelEventSubscriber.ManagePreLoadModelingEventHandlers(ModelingEventManager eventManager, EventHandlerAction action)
		{
		}
		/// <summary>
		/// Implements <see cref="IORMModelEventSubscriber.ManagePostLoadModelingEventHandlers"/>.
		/// </summary>
		protected void ManagePostLoadModelingEventHandlers(ModelingEventManager eventManager, EventHandlerAction action)
		{
			NamedElementDictionary.ManageEventHandlers(Store, eventManager, action);
		}
		void IORMModelEventSubscriber.ManagePostLoadModelingEventHandlers(ModelingEventManager eventManager, EventHandlerAction action)
		{
			this.ManagePostLoadModelingEventHandlers(eventManager, action);
		}
		/// <summary>
		/// Implementes <see cref="IORMModelEventSubscriber.ManageSurveyQuestionModelingEventHandlers"/>.
		/// </summary>
		protected void ManageSurveyQuestionModelingEventHandlers(ModelingEventManager eventManager, EventHandlerAction action)
		{
			DomainDataDirectory directory = this.Store.DomainDataDirectory;
			DomainClassInfo classInfo = directory.FindDomainRelationship(ModelHasObjectType.DomainClassId);

			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(ModelElementAdded), action);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(ModelElementRemoved), action);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementPropertyChangedEventArgs>(ModelElementNameChanged), action);
			classInfo = directory.FindDomainClass(FactType.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementPropertyChangedEventArgs>(FactTypeNameChanged), action);
		}
		void IORMModelEventSubscriber.ManageSurveyQuestionModelingEventHandlers(ModelingEventManager eventManager, EventHandlerAction action)
		{
			this.ManageSurveyQuestionModelingEventHandlers(eventManager, action);
		}
		#endregion // IORMModelEventSubscriber Implementation
		#region IVerbalizationSnippetsProvider Implementation
		private class VerbalizationSnippets : IVerbalizationSnippetsProvider
		{
			/// <summary>
			/// IVerbalizationSnippetsProvider.ProvideVerbalizationSnippets
			/// </summary>
			protected VerbalizationSnippetsData[] ProvideVerbalizationSnippets()
			{
				return new VerbalizationSnippetsData[]
				{
					new VerbalizationSnippetsData(
						typeof(CoreVerbalizationSnippetType),
						CoreVerbalizationSets.Default,
						"Core",
						ResourceStrings.CoreVerbalizationSnippetsTypeDescription,
						ResourceStrings.CoreVerbalizationSnippetsDefaultDescription)
				};
			}
			VerbalizationSnippetsData[] IVerbalizationSnippetsProvider.ProvideVerbalizationSnippets()
			{
				return ProvideVerbalizationSnippets();
			}
		}
		#endregion // IVerbalizationSnippetsProvider Implementation
		#region ISurveyNodeProvider Members
		IEnumerable<SampleDataElementNode> ISurveyNodeProvider.GetSurveyNodes()
		{
			return this.GetSurveyNodes();
		}
		/// <summary>
		/// Provides an <see cref="IEnumerable{SampleDataElementNode}"/> for the <see cref="SurveyTreeControl"/>.
		/// </summary>
		protected IEnumerable<SampleDataElementNode> GetSurveyNodes()
		{
			foreach (ModelElement element in this.Store.ElementDirectory.FindElements(FactType.DomainClassId, true))
			{
				yield return new SampleDataElementNode(element);
			}
			foreach (ModelElement element in this.Store.ElementDirectory.FindElements(ObjectType.DomainClassId, true))
			{
				yield return new SampleDataElementNode(element);
			}
		}
		#endregion
		#region SurveyEventHandling
		/// <summary>
		/// wired on SurveyQuestionLoad as event handler for ElementAdded events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ModelElementAdded(object sender, ElementAddedEventArgs e)
		{	
			INotifySurveyElementChanged eventNotify;
			ModelElement element = e.ModelElement;
			if (null != (eventNotify = (element.Store as IORMToolServices).NotifySurveyElementChanged))
			{
				eventNotify.ElementAdded((object)element);
			}
		}
		/// <summary>
		/// wired on SurveyQuestionLoad as event handler for ElementDeleted events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ModelElementRemoved(object sender, ElementDeletedEventArgs e)
		{
			INotifySurveyElementChanged eventNotify;
			ModelElement element = e.ModelElement;
			if (null != (eventNotify = (element.Store as IORMToolServices).NotifySurveyElementChanged))
			{
				eventNotify.ElementDeleted((object)element);
			}
		}
		/// <summary>
		/// wired on SurveyQuestionLoad as event handler for ElementChanged events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ModelElementChanged(object sender, ElementPropertyChangedEventArgs e)
		{
			INotifySurveyElementChanged eventNotify;
			ModelElement element = e.ModelElement;
			if (null != (eventNotify = (element.Store as IORMToolServices).NotifySurveyElementChanged))
			{
				ISurveyQuestionTypeInfo[] effectedQuestions = (this as ISurveyQuestionProvider).GetSurveyQuestionTypeInfo();
				eventNotify.ElementChanged((object)element, effectedQuestions);
			}
		}
		/// <summary>
		/// wired on SurveyQuestionLoad as event handler for ElementPropertyChanged events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ModelElementNameChanged(object sender, ElementPropertyChangedEventArgs e)
		{
			INotifySurveyElementChanged eventNotify;
			ModelElement element = e.ModelElement;
			if (null != (eventNotify = (element.Store as IORMToolServices).NotifySurveyElementChanged))
			{
				eventNotify.ElementRenamed((object)element);
			}
		}
		/// <summary>
		/// wired on SurveyQuestionLoad as event handler for FactType Name change events (custom events)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void FactTypeNameChanged(object sender, ElementPropertyChangedEventArgs e)
		{
			INotifySurveyElementChanged eventNotify = (e.ModelElement.Store as IORMToolServices).NotifySurveyElementChanged;
			if (eventNotify != null)
			{
				//TODO: find a way to get the changed model element off of CustomModelEventArgs or the sender
				eventNotify.ElementRenamed(sender);
			}
		}
		#endregion //SurveyEventHandling
	}
}
