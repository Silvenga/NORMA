#region Common Public License Copyright Notice
/**************************************************************************\
* Natural Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
* Copyright � ORM Solutions, LLC. All rights reserved.                     *
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

// Work items:
// 1) The property change events on the links are firing too late to find the parent. Switch to remove on the ModelElement itself. (not working)
// 2) RolePlayerChanged needs to be handled
// 3) Check earlier if the element has a string or not. We go a very long way before bailing for lack of a name.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling;

namespace ORMSolutions.ORMArchitect.Framework
{
	#region LocatedElement structure
	/// <summary>
	/// A structure to return a located element, or a collection
	/// of elements with the same name.
	/// </summary>
	public struct LocatedElement
	{
		/// <summary>
		/// An empty LocatedElement structure
		/// </summary>
		public static readonly LocatedElement Empty = new LocatedElement();
		private object myElement;
		/// <summary>
		/// Construct with a single element
		/// </summary>
		/// <param name="singleElement">A ModelElement object</param>
		public LocatedElement(ModelElement singleElement)
		{
			myElement = singleElement;
		}
		/// <summary>
		/// Construct with multiple elements
		/// </summary>
		/// <param name="multipleElements">A collection of ModelElement objects</param>
		public LocatedElement(ICollection multipleElements)
		{
			myElement = multipleElements;
		}
		/// <summary>
		/// Construct with single or multiple elements
		/// </summary>
		/// <param name="element">A ModelElement or a collection of NamedElements</param>
		public LocatedElement(object element)
		{
			Debug.Assert(element is ModelElement || element is ICollection);
			myElement = element;
		}
		/// <summary>
		/// Get a single element. Returns null
		/// if multiple elements were found with the same name.
		/// </summary>
		public ModelElement SingleElement
		{
			get
			{
				return myElement as ModelElement;
			}
		}
		/// <summary>
		/// Get a collection of elements. Returns null if only a
		/// single element was found.
		/// </summary>
		public ICollection MultipleElements
		{
			get
			{
				return myElement as ICollection;
			}
		}
		/// <summary>
		/// Is the element empty?
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return myElement == null;
			}
		}
		/// <summary>
		/// Get either the SingleElement or the MultipleElements
		/// value.
		/// </summary>
		public object AnyElement
		{
			get
			{
				return myElement;
			}
		}
		/// <summary>
		/// Get either the SingleElement or the first of the multiple elements
		/// </summary>
		public ModelElement FirstElement
		{
			get
			{
				ModelElement retVal = null;
				object element = myElement;
				if (element != null)
				{
					retVal = element as ModelElement;
					if (retVal == null)
					{
						foreach (ModelElement multiElement in (ICollection)element)
						{
							retVal = multiElement;
							break;
						}
					}
				}
				return retVal;
			}
		}
		/// <summary>
		/// Return true if the element is already represented
		/// by this LocatedElement
		/// </summary>
		public bool ContainsElement(ModelElement element)
		{
			object testElement = myElement;
			ICollection collection;
			if (testElement == element)
			{
				return true;
			}
			else if (null != (collection = testElement as ICollection))
			{
				foreach (object test in collection)
				{
					if (element == test)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
	#endregion // LocatedElement structure
	#region IDuplicateNameCollectionManager interface
	/// <summary>
	/// A callback interface to construct a collection of
	/// elements with the same name. A concrete implementation
	/// can either collection the elements into an array-like
	/// collection, or create IMS objects for tracking the errors
	/// in the object model.
	/// </summary>
	public interface IDuplicateNameCollectionManager
	{
		/// <summary>
		/// An element was added whose name conflicted with one or
		/// more existing elements. This method should create or add
		/// to a collection of these elements. OnDuplicateElementAdded
		/// will be called twice when the first duplicate is found. The
		/// first call will have elementCollection == null and should be
		/// used to create the collection and add the first item. The second
		/// call will return the created collection with the second element.
		/// </summary>
		/// <param name="elementCollection">An existing collection, or null
		/// to initialize the collection and populate it with the first element.</param>
		/// <param name="element">The duplicate element</param>
		/// <param name="afterTransaction">The store is not in a transaction, so store
		/// modifications should not be made. This parameter will be true during
		/// events, which only fire during undo/redo. In this state, a collection that
		/// is implemented through the IMS will be extant in the desired state in
		/// the store and needs to be located and returned.</param>
		/// <param name="notifyAdded">Used during deserialization fixup to
		/// track elements that are added while rules are disabled</param>
		/// <returns>A new (or modified) collection containing all elements.</returns>
		ICollection OnDuplicateElementAdded(ICollection elementCollection, ModelElement element, bool afterTransaction, INotifyElementAdded notifyAdded);
		/// <summary>
		/// A duplicate element has been removed. This method is also responsible
		/// for destroying the collection as the last element is removed.
		/// </summary>
		/// <param name="elementCollection">An existing collection containing the
		/// removed element and other duplicates.</param>
		/// <param name="element">The element to remove.</param>
		/// <param name="afterTransaction">The store is not in a transaction, so store
		/// modifications should not be made. This parameter will be true during
		/// events, which only interaction with the name dictionaries during undo/redo.
		/// In this state, a collection implemented through the IMS should not be modified.</param>
		/// <returns>A new (or modified) collection containing all elements.</returns>
		ICollection OnDuplicateElementRemoved(ICollection elementCollection, ModelElement element, bool afterTransaction);
		/// <summary>
		/// After a rollback has occurred, any collection returned by the other two methods
		/// that was changed without a snapshot may be in an inconsistent state. This gives the
		/// duplicate name manager the chance to resynchronize these collections with the current
		/// state of a backing collection, which will be correct after the rollback if it is
		/// implemented using the backing store.
		/// </summary>
		/// <param name="collection">The collection to update</param>
		void AfterCollectionRollback(ICollection collection);
	}
	#endregion // IDuplicateNameCollectionManager
	#region DuplicateNameAction enum
	/// <summary>
	/// Specifies how like-named elements should be
	/// handled by the INamedElementDictionary implementation.
	/// </summary>
	public enum DuplicateNameAction
	{
		/// <summary>
		/// Create or modify a duplicate element collection when a duplicate
		/// is added or removed. Maps to afterTransaction=true parameter values
		/// in calls to the IDuplicateNameCollectionManager interface.
		/// </summary>
		ModifyDuplicateCollection,
		/// <summary>
		/// Retrieve a duplicate element collection when a duplicate
		/// is added or removed. Maps to afterTransaction=false parameter values
		/// in calls to the IDuplicateNameCollectionManager interface.
		/// </summary>
		RetrieveDuplicateCollection,
		/// <summary>
		/// Disallow a new duplicate.
		/// </summary>
		ThrowOnDuplicateName,
	}
	#endregion // DuplicateNameAction enum
	#region INamedElementDictionary interface
	/// <summary>
	/// An interface used to manage names on an object
	/// and provide a quick lookup mechanism for retrieving them.
	/// </summary>
	public interface INamedElementDictionary
	{
		/// <summary>
		/// An element has been added. Generate a unique name
		/// if the name is empty, or ensure a unique name if
		/// the Name property is not set.
		/// </summary>
		/// <param name="element">The element to add.</param>
		/// <param name="duplicateAction">Specify the action
		/// to take if the name is already in use in the dictionary.</param>
		/// <param name="notifyAdded">The listener to notify if elements are added during fixup</param>
		void AddElement(ModelElement element, DuplicateNameAction duplicateAction, INotifyElementAdded notifyAdded);
		/// <summary>
		/// An element is being removed.
		/// </summary>
		/// <param name="element">The element to remove</param>
		/// <param name="alternateElementName">If specified, a name to use instead of
		/// the current element name value</param>
		/// <param name="duplicateAction">Specify the action
		/// to take if the name is already in use in the dictionary.</param>
		/// <returns>true if the element was successfully removed</returns>
		bool RemoveElement(ModelElement element, string alternateElementName, DuplicateNameAction duplicateAction);
		/// <summary>
		/// An element is being replaced with another element.
		/// </summary>
		/// <param name="originalElement">The element to remove</param>
		/// <param name="replacementElement">The element to replace it with</param>
		/// <param name="duplicateAction">Specify the action
		/// to take if the name is already in use in the dictionary.</param>
		void ReplaceElement(ModelElement originalElement, ModelElement replacementElement, DuplicateNameAction duplicateAction);
		/// <summary>
		/// An element has been renamed.
		/// </summary>
		/// <param name="element">The element being renamed</param>
		/// <param name="oldName">The old name for the element</param>
		/// <param name="newName">The new name for the element</param>
		/// <param name="duplicateAction">Specify the action
		/// to take if the name is already in use in the dictionary.</param>
		void RenameElement(ModelElement element, string oldName, string newName, DuplicateNameAction duplicateAction);
		/// <summary>
		/// Find elements matching the given name 
		/// </summary>
		/// <param name="elementName">The name to locate</param>
		/// <returns>A LocatedElement structure, indicating no, 1, or
		/// multiple matches.</returns>
		LocatedElement GetElement(string elementName);
		/// <summary>
		/// If the dictionary is remote and cannot be reached due
		/// to deleted links, then the rest of these methods are
		/// vacant and this returns a type. The type is sent to a
		/// resolved ancestor element that implements <see cref="INamedElementDictionaryOwner"/>
		/// to get the dictionary. This is used only during event
		/// resolution and will be ignored during transacted processing,
		/// where the path is still fully navigable during 'deleting' events.
		/// </summary>
		Type RemoteDictionaryType { get;}
	}
	#endregion // INamedElementDictionary interface
	#region INamedElementDictionaryParentNode interface
	/// <summary>
	/// An empty interface used to represent a parent node in
	/// a named element dictionary link. This is implicitly
	/// implemented by <see cref="INamedElementDictionaryParent"/>
	/// and must be explicitly implemented by any object that
	/// implements <see cref="INamedElementDictionaryRemoteChild"/>
	/// where that child also acts as a parent. This occurs only
	/// if the remote dictionary chain passes multiple intermediate
	/// steps.
	/// </summary>
	public interface INamedElementDictionaryParentNode
	{
		// No contents
	}
	#endregion // INamedElementDictionaryParentNode interface
	#region INamedElementDictionaryChildNode interface
	/// <summary>
	/// An empty interface used to represent a child node in
	/// a named element dictionary link. This is implicitly
	/// implemented by <see cref="INamedElementDictionaryChild"/>
	/// and <see cref="INamedElementDictionaryRemoteChild"/>.
	/// </summary>
	public interface INamedElementDictionaryChildNode
	{
		// No contents
	}
	#endregion // INamedElementDictionaryChildNode interface
	#region INamedElementDictionaryParent interface
	/// <summary>
	/// An interface implemented on the parent role player
	/// in a name dictionary setup.
	/// </summary>
	public interface INamedElementDictionaryParent : INamedElementDictionaryParentNode
	{
		/// <summary>
		/// Get the dictionary corresponding to the elements
		/// at the counterpart end of the relationship.
		/// </summary>
		/// <param name="parentDomainRoleId">The role played by
		/// the implementing object. Generally, this will
		/// be called for an aggregating role.</param>
		/// <param name="childDomainRoleId">The opposite role,
		/// representing the many end of the set.</param>
		/// <returns>INamedElementDictionary implementation</returns>
		INamedElementDictionary GetCounterpartRoleDictionary(Guid parentDomainRoleId, Guid childDomainRoleId);
		/// <summary>
		/// A key to test against the transaction context to determine if
		/// duplicate names should be allowed or not. Three values are treated
		/// specially:
		/// NamedElementDictionary.AllowDuplicateNamesKey allows duplicates with
		///    checking the ContextInfo of the current transaction.
		/// NamedElementDictionary.BlockDuplicateNamesKey disallows duplicates with
		///    checking the ContextInfo of the current transaction.
		/// null is interpreted as ModelingDocData.Loading. A null return enables
		///    duplicates during document load, but disallows duplicates in subsequent editing
		///
		/// If the returned key implements <see cref="INamedElementDictionaryContextKeyBlocksDuplicates"/>,
		/// then the key is looked for in the context information, but the meaning of the key reverses,
		/// so no key in the dictionary means to allow, and a key in the dictionary means to block.
		/// </summary>
		/// <param name="parentDomainRoleId"></param>
		/// <param name="childDomainRoleId"></param>
		/// <returns></returns>
		object GetAllowDuplicateNamesContextKey(Guid parentDomainRoleId, Guid childDomainRoleId);
	}
	#endregion // INamedElementDictionaryOwner interface
	#region INamedElementDictionaryChild interface
	/// <summary>
	/// An interface to mark a child element as a participant
	/// in a named element dictionary. This interface should
	/// only be specified on ModelElement-derived classes.
	/// </summary>
	public interface INamedElementDictionaryChild : INamedElementDictionaryChildNode
	{
		/// <summary>
		/// Return the role guids to get from a child element
		/// to its containing parent set.
		/// </summary>
		/// <param name="parentDomainRoleId"></param>
		/// <param name="childDomainRoleId"></param>
		void GetRoleGuids(out Guid parentDomainRoleId, out Guid childDomainRoleId);
	}
	#endregion // INamedElementDictionaryChild interface
	#region NamedElementDictionaryLinkUse enum
	/// <summary>
	/// Specify how the parent and child information in
	/// a <see cref="INamedElementDictionaryLink"/> is used.
	/// Links can be used for both direct naming relationships,
	/// where the parent owns the dictionary, or for indirect
	/// dictionary ownership, where the dictionary specified
	/// by the direct parent is owned by a more distant ancestor.
	/// </summary>
	[Flags]
	public enum NamedElementDictionaryLinkUse
	{
		/// <summary>
		/// The parent element has an <see cref="INamedElementDictionaryParent"/>
		/// implementation to retrieve a dictionary that manages
		/// names for the child element.
		/// </summary>
		DirectDictionary = 1,
		/// <summary>
		/// The child element has an <see cref="INamedElementDictionaryRemoteChild"/>
		/// implementation pointing towards the managed elements, and the
		/// parent element moves up the chain to the true dictionary owner.
		/// </summary>
		DictionaryConnector = 2,
	}
	#endregion // NamedElementDictionaryLinkUse enum
	#region INamedElementDictionaryLink interface
	/// <summary>
	/// An interface to mark an ElementLink as the relationship
	/// that controls a set of named elements. Technically, this
	/// information can be derived by investigating the role players,
	/// but this is much more work than we want to do whenever an
	/// ElementLink is added.
	/// </summary>
	public interface INamedElementDictionaryLink
	{
		/// <summary>
		/// Get the parent role player. This will either be a
		/// <see cref="INamedElementDictionaryParent"/> implementation
		/// or a <see cref="INamedElementDictionaryRemoteChild"/> implementation
		/// that is part of a multi-step remote dictionary.
		/// </summary>
		INamedElementDictionaryParentNode ParentRolePlayer{get;}
		/// <summary>
		/// Get the child role player. This will either be a
		/// <see cref="INamedElementDictionaryChild"/> or a
		/// <see cref="INamedElementDictionaryRemoteChild"/> implementation.
		/// </summary>
		INamedElementDictionaryChildNode ChildRolePlayer { get;}
		/// <summary>
		/// Indicate whether the link is used for direct dictionary
		/// ownership or as a path to a remote dictionary, or both.
		/// </summary>
		NamedElementDictionaryLinkUse DictionaryLinkUse { get;}
	}
	#endregion // INamedElementDictionaryLink interface
	#region INamedElementDictionaryRemoteChild interface
	/// <summary>
	/// By default, the assumption is made that the object implementing
	/// INamedElementDictionaryParent also owns the dictionary. However, this
	/// does not allow for dictionaries to be shared across different levels
	/// of the metamodel. If names must be unique across a set of both
	/// child and grandchild elements and the dictionary is owned by the
	/// parent/grandparent, then the dictionary cannot be reached if the grandchildren
	/// are associated with the child before the child is associated with the
	/// parent. Implementing this interface on an object returned by the
	/// <see cref="INamedElementDictionaryLink.ChildRolePlayer"/> property of a link with
	/// a <see cref="INamedElementDictionaryLink.DictionaryLinkUse"/> that includes the
	/// to <see cref="NamedElementDictionaryLinkUse.DictionaryConnector"/> flag enables the
	/// dictionary implementation to include remote objects in the dictionary.
	/// </summary>
	/// <remarks>If there is more than one remote step between the named element
	/// and the object that owns the dictionary, then the object implementing
	/// this interface should also implement the empty <see cref="INamedElementDictionaryParentNode"/>
	/// interface so that it can be returned by the <see cref="INamedElementDictionaryLink.ParentRolePlayer"/>
	/// property in an intermediate link.</remarks>
	public interface INamedElementDictionaryRemoteChild : INamedElementDictionaryChildNode
	{
		/// <summary>
		/// Get the meta-role guids for all roles on this object that
		/// attach to a link that implements <see cref="INamedElementDictionaryLink"/>
		/// and move from this element towards the named element stored in
		/// the dictionary.
		/// </summary>
		/// <returns>An array of supported guids, or null.</returns>
		Guid[] GetNamedElementDictionaryChildRoles();
		/// <summary>
		/// Get the parent role that leads to the named element dictionaries
		/// associated with this element. Returns <see cref="Guid.Empty"/>
		/// if no parent role is available.
		/// </summary>
		Guid NamedElementDictionaryParentRole { get;}
	}
	#endregion // INamedElementDictionaryRemoteChild interface
	#region INamedElementDictionaryOwner interface
	/// <summary>
	/// Retrieve a named element dictionary based on the type of
	/// child element. Unlike <see cref="INamedElementDictionaryParent"/>,
	/// which can be implemented at multiple levels in the same model
	/// to route nested child elements to a root element, this interface
	/// should be implemented only on the objects that directly own
	/// one or more dictionaries.
	/// </summary>
	public interface INamedElementDictionaryOwner
	{
		/// <summary>
		/// Find the owned <see cref="INamedElementDictionary"/> based
		/// on the type of child object.
		/// </summary>
		/// <param name="childType">The type of a child element.</param>
		/// <returns>The owned dictionary if the child type is recognized.</returns>
		INamedElementDictionary FindNamedElementDictionary(Type childType);
	}
	#endregion // INamedElementDictionaryOwner interface
	#region IDefaultNamePattern interface
	/// <summary>
	/// Provide a custom pattern for a default name.
	/// </summary>
	public interface IDefaultNamePattern
	{
		/// <summary>
		/// Return a name pattern with a replacement field to
		/// hold a generated number. If no replacement field
		/// is provided, the number is appended.
		/// </summary>
		string DefaultNamePattern { get;}
		/// <summary>
		/// Return <see langword="true"/> if the default name
		/// can be automatically reset.
		/// </summary>
		bool DefaultNameResettable { get;}
	}
	#endregion // IDefaultNamePattern interface
	#region INamedElementDictionaryContextKeyBlocksDuplicates interface
	/// <summary>
	/// An interface to implement on a context key returned from
	/// <see cref="INamedElementDictionaryParent.GetAllowDuplicateNamesContextKey"/>.
	/// If the returned key implements this interface then it reverses the
	/// meaning of placing this key is present in the context dictionary.
	/// With this interface set on a key, a duplicate name exception is thrown only
	/// if the key is present in the dictionary, opposite to the default behavior
	/// which throws only when the key is not present.
	/// </summary>
	public interface INamedElementDictionaryContextKeyBlocksDuplicates
	{
		// Intentionally empty
	}
	#endregion // INamedElementDictionaryContextKeyBlocksDuplicates interface
	#region NamedElementDictionary class
	/// <summary>
	/// A class used to enforce naming across a relationship
	/// representing a collection of ModelElement children.
	/// Duplicate element collection creation, name generation,
	/// and exception handling can all be controlled by derived classes.
	/// </summary>
	public partial class NamedElementDictionary : INamedElementDictionary
	{
		#region Public token values
		/// <summary>
		/// A large negative number used to make sure the name generation
		/// rules fire very early.
		/// </summary>
		public const int RulePriority = -500000;
		/// <summary>
		/// A key to return from INamedElementDictionaryParent.GetAllowDuplicateNamesContextKey
		/// if duplicate names should be allowed.
		/// </summary>
		public static readonly object AllowDuplicateNamesKey = new object();
		/// <summary>
		/// A key to return from INamedElementDictionaryParent.GetAllowDuplicateNamesContextKey
		/// if duplicate names should not be allowed.
		/// </summary>
		public static readonly object BlockDuplicateNamesKey = new object();
		/// <summary>
		/// The default key used by the named element dictionary to control whether
		/// duplicate names should be allowed. A null return from
		/// INamedElementDictionaryParent.GetAllowDuplicateNamesContextKey will use
		/// this key by default
		/// </summary>
		public static readonly object DefaultAllowDuplicateNamesKey = new object();
		#endregion // Public token values
		#region Default duplicate collection manager
		/// <summary>
		/// A simple collection implementation using arrays. Does not participate
		/// with the state of a <see cref="Store"/> in any way.
		/// </summary>
		private sealed class SimpleDuplicateCollectionManager : IDuplicateNameCollectionManager
		{
			/// <summary>
			/// The singleton instance of the collection manager
			/// </summary>
			public static readonly SimpleDuplicateCollectionManager Singleton = new SimpleDuplicateCollectionManager();
			private SimpleDuplicateCollectionManager()
			{
			}
			ICollection IDuplicateNameCollectionManager.OnDuplicateElementAdded(ICollection elementCollection, ModelElement element, bool afterTransaction, INotifyElementAdded notifyAdded)
			{
				ModelElement[] elements = (ModelElement[])elementCollection;
				ModelElement[] retVal = null;
				if (elements == null)
				{
					// Create a new collection and prepare for a second call
					retVal = new ModelElement[] { element, null };
				}
				else
				{
					int elementCount = elements.Length;
					if (elementCount == 2 && elements[1] == null)
					{
						// Second half of initial creation
						elements[1] = element;
						retVal = elements;
					}
					else
					{
						// Copy the existing elements and add a new one.
						// Obviously, this makes the assumption that >2 duplicates
						// is unusual. Otherwise, we would use a growable collection
						// instead of reallocating arrays each time.
						retVal = new ModelElement[elementCount + 1];
						elements.CopyTo(retVal, 0);
						retVal[elementCount] = element;
					}
				}
				return retVal;
			}
			ICollection IDuplicateNameCollectionManager.OnDuplicateElementRemoved(ICollection elementCollection, ModelElement element, bool afterTransaction)
			{
				ModelElement[] elements = (ModelElement[])elementCollection;
				int elementCount = elements.Length;
				ModelElement[] retVal = null;
				// There is nothing to do when there are 1 or 2 elements. A call
				// with 2 elements will immediately be followed by a call with
				// 1 element, at which point the collection will be abandoned.
				if (elementCount <= 2)
				{
					retVal = elements;
				}
				else
				{
					retVal = new ModelElement[elementCount - 1];
					int newIndex = 0;
					for (int i = 0; i < elementCount; ++i)
					{
						ModelElement testElement = elements[i];
						if (testElement == element)
						{
							for (int j = i + 1; j < elementCount; ++j)
							{
								// Assume elements are distinct. Continue copy without the test.
								retVal[newIndex] = elements[j];
								++newIndex;
							}
							break;
						}
						else
						{
							retVal[newIndex] = testElement;
							++newIndex;
						}
					}
				}
				return retVal;
			}
			void IDuplicateNameCollectionManager.AfterCollectionRollback(ICollection collection)
			{
				// Intentionally empty. The simple duplicate collection manager does not use
				// state from a Store.
			}
		}
		#endregion // Default duplicate collection manager
		#region Member variables
		private Dictionary<string, object> myDictionary;
		private IDuplicateNameCollectionManager myDuplicateManager;
		/// <summary>
		/// A stack of changes that need to be reverted if the
		/// transaction is rolled back. Managed by methods in the
		/// EntryStateChange structure. Note that each dictionary
		/// will be contained within a single store, and a store can
		/// only have one top-level transaction at a time, so storing
		/// this with the object is sufficient.
		/// </summary>
		private Stack<EntryStateChange> myChangeStack;
		#endregion // Member variables
		#region Constructors
		/// <summary>
		/// Construct a NamedElementDictionary with the specified duplicate
		/// collection manager
		/// </summary>
		/// <param name="duplicateManager">IDuplicateNameCollectionManager implementation</param>
		public NamedElementDictionary(IDuplicateNameCollectionManager duplicateManager)
		{
			myDuplicateManager = duplicateManager;
			myDictionary = new Dictionary<string, object>();
		}
		/// <summary>
		/// Construct a NamedElementDictionary with the specified duplicate
		/// collection manager
		/// </summary>
		/// <param name="duplicateManager">IDuplicateNameCollectionManager implementation</param>
		/// <param name="comparer">String comparison algorithm used to determine duplicate names</param>
		public NamedElementDictionary(IDuplicateNameCollectionManager duplicateManager, IEqualityComparer<string> comparer)
		{
			myDuplicateManager = duplicateManager;
			myDictionary = new Dictionary<string, object>(comparer);
		}
		/// <summary>
		/// Default constructor. Uses a basic collection manager with no IMS ties
		/// </summary>
		public NamedElementDictionary()
			: this(SimpleDuplicateCollectionManager.Singleton)
		{
		}
		/// <summary>
		/// Default constructor. Uses a basic collection manager with no IMS ties
		/// </summary>
		/// <param name="comparer">String comparison algorithm used to determine duplicate names</param>
		public NamedElementDictionary(IEqualityComparer<string> comparer)
			: this(SimpleDuplicateCollectionManager.Singleton, comparer)
		{
		}
		#endregion // Constructors
		#region Virtual methods
		/// <summary>
		/// Get the root name for generating a unique name for this
		/// element. A unique name is generated by default by appending
		/// numbers to the end of the string. However, the numbers can
		/// be placed elsewhere in the string by including the formatting
		/// sequence '{0}' anywhere in the returned string. The default
		/// implementation defers to <see cref="IDefaultNamePattern"/>
		/// with a fallback on <see cref="TypeDescriptor.GetClassName(System.Object)"/>.
		/// </summary>
		/// <param name="element">The element to generate a name for</param>
		/// <returns>A string to use as the base name. Can include {0} to
		/// indicate where the unique numbers should be included in the string.</returns>
		protected virtual string GetRootNamePattern(ModelElement element)
		{
			IDefaultNamePattern patternProvider = element as IDefaultNamePattern;
			string retVal;
			if (null != (patternProvider = element as IDefaultNamePattern) &&
				!string.IsNullOrEmpty(retVal = patternProvider.DefaultNamePattern))
			{
				return retVal;
			}
			return TypeDescriptor.GetClassName(element);
		}
		/// <summary>
		/// Get the default name for the element. If a default name is provided
		/// then there is no attempt made to get a root name pattern and the dictionary
		/// is temporarily switched into 'allow duplicates' mode
		/// </summary>
		/// <param name="element">The element to get a name for</param>
		/// <returns>null, or a default name for the element</returns>
		protected virtual string GetDefaultName(ModelElement element)
		{
			return null;
		}
		/// <summary>
		/// If a duplicate name is caught during load, then determine if the name
		/// is an auto-generated name that should be regenerated instead of tracked
		/// as a duplicate name error. The default implementation returns <see langword="false"/>.
		/// </summary>
		/// <param name="element">The element with the duplicate name</param>
		/// <param name="elementName">The pre-fetched element name</param>
		/// <returns>Return <see langword="true"/> to reset the name.</returns>
		protected virtual bool ShouldResetDuplicateName(ModelElement element, string elementName)
		{
			IDefaultNamePattern patternProvider;
			string pattern;
			return null != (patternProvider = element as IDefaultNamePattern) &&
				patternProvider.DefaultNameResettable &&
				Utility.IsNumberDecoratedName(elementName, string.IsNullOrEmpty(pattern = patternProvider.DefaultNamePattern) ? TypeDescriptor.GetClassName(element) : pattern);
		}
		/// <summary>
		/// Override to throw a custom exception when
		/// adding duplicate names is not allowed.
		/// </summary>
		/// <param name="element">The element that could not be added due to the duplicate name</param>
		/// <param name="requestedName"></param>
		protected virtual void ThrowDuplicateNameException(ModelElement element, string requestedName)
		{
			// UNDONE: Localize
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The name '{0}' is already used in this context.", requestedName));
		}
		#endregion // Virtual methods
		#region Unique name generation
		/// <summary>
		/// Generate a unique name for this element. Defers to
		/// GetRootNamePattern to get a starting name pattern
		/// </summary>
		/// <param name="element">ModelElement</param>
		/// <param name="forceAllowDuplicateName">Set to true if the GetDefaultName returns a non-empty string</param>
		/// <returns>A name that is not currently in the dictionary.</returns>
		private string GenerateUniqueName(ModelElement element, out bool forceAllowDuplicateName)
		{
			string rootName = GetDefaultName(element);
			if (!string.IsNullOrEmpty(rootName))
			{
				forceAllowDuplicateName = true;
				return rootName;
			}
			forceAllowDuplicateName = false;
			if (string.IsNullOrEmpty(rootName = GetRootNamePattern(element)))
			{
				// Indicates unique names are not supported
				return null;
			}
			if (!rootName.Contains("{0}"))
			{
				rootName += "{0}";
			}
			int i = 0;
			string newKey = null;
			Dictionary<string, object> dic = myDictionary;
			do
			{
				++i;
				newKey = string.Format(CultureInfo.InvariantCulture, rootName, i.ToString(CultureInfo.InvariantCulture));
			} while(dic.ContainsKey(newKey));
			return newKey;
		}
		#endregion // Unique name generation
		#region INamedElementDictionary Members
		void INamedElementDictionary.AddElement(ModelElement element, DuplicateNameAction duplicateAction, INotifyElementAdded notifyAdded)
		{
			AddElement(element, duplicateAction, notifyAdded);
		}
		/// <summary>
		/// Implements <see cref="INamedElementDictionary.AddElement"/>
		/// </summary>
		/// <param name="element">ModelElement</param>
		/// <param name="duplicateAction">DuplicateNameAction</param>
		/// <param name="notifyAdded">Set if a callback is required when an element is added
		/// during the IDuplicateNameCollectionManager.OnDuplicateElementAdded. Used
		/// during deserialization fixup</param>
		protected void AddElement(ModelElement element, DuplicateNameAction duplicateAction, INotifyElementAdded notifyAdded)
		{
			AddElement(element, duplicateAction, DomainClassInfo.GetName(element), notifyAdded);
		}
		/// <summary>
		/// Add an element with a provided name (ignores the current element name).
		/// Helper function for AddElement and ReplaceElement
		/// </summary>
		/// <param name="element">ModelElement</param>
		/// <param name="duplicateAction">DuplicateNameAction</param>
		/// <param name="elementName">Name to use as the remove key</param>
		/// <param name="notifyAdded">Set if a callback is required when an element is added
		/// during the IDuplicateNameCollectionManager.OnDuplicateElementAdded. Used
		/// during deserialization fixup</param>
		private void AddElement(ModelElement element, DuplicateNameAction duplicateAction, string elementName, INotifyElementAdded notifyAdded)
		{
			if (elementName.Length == 0)
			{
				// This will fire a name change rule, which will reenter this instance
				// in RenameElement. Just set the name and get out. Don't do this
				// unless we're inside a transaction.
				if (duplicateAction != DuplicateNameAction.RetrieveDuplicateCollection)
				{
					Debug.Assert(element.Store.TransactionManager.InTransaction);
					bool forceAllowDuplicateName;
					elementName = GenerateUniqueName(element, out forceAllowDuplicateName);
					if (elementName != null && elementName.Length != 0)
					{
						if (forceAllowDuplicateName && duplicateAction == DuplicateNameAction.ThrowOnDuplicateName)
						{
							duplicateAction = DuplicateNameAction.ModifyDuplicateCollection;
							bool ruleDisabled = false;
							RuleManager ruleManager = null;
							try
							{
								if (notifyAdded == null)
								{
									ruleManager = element.Store.RuleManager;
									ruleManager.DisableRule(typeof(NamedElementChangedRuleClass));
									ruleDisabled = true;
								}
								DomainClassInfo.SetName(element, elementName);
							}
							finally
							{
								if (ruleDisabled)
								{
									ruleManager.EnableRule(typeof(NamedElementChangedRuleClass));
								}
							}
						}
						else
						{
							DomainClassInfo.SetName(element, elementName);
							if (notifyAdded == null)
							{
								// The name change rule will not fire during deserialization, just fall through here
								// if we're deserializing
								return;
							}
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}
			}
			LocatedElement locateData = GetElement(elementName);
			bool afterTransaction = duplicateAction == DuplicateNameAction.RetrieveDuplicateCollection;
			Debug.Assert(afterTransaction != element.Store.TransactionManager.InTransaction);
			bool handleRollback = !afterTransaction && notifyAdded == null; // Don't log during loads
			if (locateData.IsEmpty)
			{
				if (handleRollback)
				{
					EntryStateChange.OnEntryChange(element, this, elementName, null);
				}
				myDictionary.Add(elementName, element);
			}
			else if (locateData.ContainsElement(element))
			{
				// Unusual case, but it does happen if changing the element
				// name triggers a name change for an element that has an
				// add rule pending for a named element dictionary link.
			}
			else if (duplicateAction == DuplicateNameAction.ThrowOnDuplicateName)
			{
				ThrowDuplicateNameException(element, elementName);
				// The return will only be hit if a derived class chooses
				// to not throw an exception
				return;
			}
			else if (notifyAdded != null && ShouldResetDuplicateName(element, elementName))
			{
				AddElement(element, duplicateAction, "", notifyAdded);
			}
			else
			{
				ModelElement singleElement = locateData.SingleElement;
				ICollection newCollection = null;
				if (singleElement == null)
				{
					// We already have a collection, just add to it
					ICollection existingCollection = locateData.MultipleElements;
					newCollection = myDuplicateManager.OnDuplicateElementAdded(existingCollection, element, afterTransaction, notifyAdded);
					if (existingCollection == newCollection)
					{
						// No need to replace
						newCollection = null;
					}
				}
				else
				{
					if (element != singleElement)
					{
						// Call OnDuplicateElementAdded twice. The first time creates the collection,
						// the second one adds to it.
						newCollection = myDuplicateManager.OnDuplicateElementAdded(
							myDuplicateManager.OnDuplicateElementAdded(null, singleElement, afterTransaction, notifyAdded),
							element,
							afterTransaction,
							notifyAdded);
					}
				}
				if (newCollection != null)
				{
					if (handleRollback)
					{
						EntryStateChange.OnEntryChange(element, this, elementName, locateData.AnyElement);
					}
					myDictionary[elementName] = newCollection;
				}
			}
		}
		bool INamedElementDictionary.RemoveElement(ModelElement element, string alternateElementName, DuplicateNameAction duplicateAction)
		{
			return RemoveElement(element, alternateElementName, duplicateAction);
		}
		/// <summary>
		/// Implements <see cref="INamedElementDictionary.RemoveElement"/>, and helper function
		/// for <see cref="INamedElementDictionary.ReplaceElement"/>.
		/// Remove an element with a provided name. The current element name is
		/// used if the alternate is not provided.
		/// </summary>
		/// <param name="element">ModelElement</param>
		/// <param name="alternateElementName">If specified, a name to use instead of
		/// the current element name value</param>
		/// <param name="duplicateAction">DuplicateNameAction</param>
		/// <returns>true if the element was successfully removed</returns>
		protected bool RemoveElement(ModelElement element, string alternateElementName, DuplicateNameAction duplicateAction)
		{
			string elementName = alternateElementName;
			// Use null here. An empty name is natural during a rename (DSL initial properties are empty, not null), but there
			// will be no corresponding entry to remove.
			if (elementName == null)
			{
				elementName = DomainClassInfo.GetName(element);
			}
			if (!string.IsNullOrEmpty(elementName))
			{
				LocatedElement locateData = GetElement(elementName);
				if (!locateData.IsEmpty)
				{
					bool afterTransaction = duplicateAction == DuplicateNameAction.RetrieveDuplicateCollection;
					ModelElement singleElement = locateData.SingleElement;
					if (singleElement != null)
					{
						if (singleElement == element)
						{
							if (!afterTransaction)
							{
								EntryStateChange.OnEntryChange(element, this, elementName, singleElement);
							}
							myDictionary.Remove(elementName);
						}
					}
					else
					{
						ICollection existingCollection = locateData.MultipleElements;
						int elementCount = existingCollection.Count;
						Debug.Assert(elementCount >= 2);
						if (elementCount == 2)
						{
							ModelElement oppositeElement = null;
							bool seenElement = false;
							foreach (ModelElement otherElement in existingCollection)
							{
								if (element == otherElement)
								{
									seenElement = true;
								}
								else
								{
									oppositeElement = otherElement;
								}
							}
							if (seenElement &&
								oppositeElement != null)
							{
								myDuplicateManager.OnDuplicateElementRemoved(
									myDuplicateManager.OnDuplicateElementRemoved(existingCollection, element, afterTransaction),
									oppositeElement,
									afterTransaction);
								if (!afterTransaction)
								{
									EntryStateChange.OnEntryChange(element, this, elementName, existingCollection);
								}
								myDictionary[elementName] = oppositeElement;
								return true;
							}
						}
						else
						{
							foreach (ModelElement testElement in existingCollection)
							{
								if (testElement == element)
								{
									ICollection newCollection = myDuplicateManager.OnDuplicateElementRemoved(existingCollection, element, afterTransaction);
									if (newCollection != existingCollection)
									{
										if (!afterTransaction)
										{
											EntryStateChange.OnEntryChange(element, this, elementName, existingCollection);
										}
										myDictionary[elementName] = newCollection;
									}
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
		void INamedElementDictionary.ReplaceElement(ModelElement originalElement, ModelElement replacementElement, DuplicateNameAction duplicateAction)
		{
			ReplaceElement(originalElement, replacementElement, duplicateAction);
		}
		/// <summary>
		/// Implements <see cref="INamedElementDictionary.ReplaceElement"/>
		/// </summary>
		/// <param name="originalElement">ModelElement</param>
		/// <param name="replacementElement">ModelElement</param>
		/// <param name="duplicateAction">DuplicateNameAction</param>
		protected void ReplaceElement(ModelElement originalElement, ModelElement replacementElement, DuplicateNameAction duplicateAction)
		{
			// Consider optimizing if the old/new names are the same
			RemoveElement(originalElement, null, duplicateAction);
			AddElement(replacementElement, duplicateAction, null);
		}
		void INamedElementDictionary.RenameElement(ModelElement element, string oldName, string newName, DuplicateNameAction duplicateAction)
		{
			RenameElement(element, oldName, newName, duplicateAction);
		}
		/// <summary>
		/// Implements <see cref="INamedElementDictionary.RenameElement"/>
		/// </summary>
		/// <param name="element">ModelElement</param>
		/// <param name="oldName">string</param>
		/// <param name="newName">string</param>
		/// <param name="duplicateAction">duplicateAction</param>
		protected void RenameElement(ModelElement element, string oldName, string newName, DuplicateNameAction duplicateAction)
		{
			// UNDONE: If AddElement fails, this does not readd the
			// removed element. Delay the remove until we're relatively
			// sure the add will work correctly.
			RemoveElement(element, oldName, duplicateAction);
			AddElement(element, duplicateAction, newName, null);
		}
		LocatedElement INamedElementDictionary.GetElement(string elementName)
		{
			return GetElement(elementName);
		}
		/// <summary>
		/// Return element(s) with the given name
		/// </summary>
		/// <param name="elementName">string</param>
		/// <returns>LocatedElement structure</returns>
		protected LocatedElement GetElement(string elementName)
		{
			object element;
			return myDictionary.TryGetValue(elementName, out element) ? new LocatedElement(element) : LocatedElement.Empty;
		}
		Type INamedElementDictionary.RemoteDictionaryType
		{
			get
			{
				return RemoteDictionaryType;
			}
		}
		/// <summary>
		/// Implements <see cref="INamedElementDictionary.RemoteDictionaryType"/>
		/// If this object is in use, then the dictionary has been found and
		/// there is no remote type.
		/// </summary>
		protected static Type RemoteDictionaryType
		{
			get
			{
				return null;
			}
		}
		#endregion // INamedElementDictionary Members
		#region Deserialization Fixup
		/// <summary>
		/// Return a deserialization fixup listener. The listener
		/// ensures that the name dictionaries are correctly populated
		/// after a model deserialization is completed.
		/// </summary>
		/// <param name="fixupPhase">A fixup phase for adding implicitly
		/// created elements and populating the name dictionaries</param>
		/// <param name="elementDomainModel">The required domain model type for processed elements</param>
		public static IDeserializationFixupListener GetFixupListener(int fixupPhase, DomainModelInfo elementDomainModel)
		{
			return new DeserializationFixupListener(fixupPhase, elementDomainModel);
		}
		/// <summary>
		/// A listener class to validate and/or populate the ModelError
		/// collection on load, as well as populating the task list.
		/// </summary>
		private sealed class DeserializationFixupListener : DeserializationFixupListener<INamedElementDictionaryLink>
		{
			private DomainModelInfo myDomainModelFilter;
			/// <summary>
			/// Create a new NamedElementDictionary.DeserializationFixupListener
			/// </summary>
			/// <param name="fixupPhase">The phase number for this listener</param>
			/// <param name="elementDomainModel">The required domain model type for processed elements</param>
			public DeserializationFixupListener(int fixupPhase, DomainModelInfo elementDomainModel) : base(fixupPhase)
			{
				myDomainModelFilter = elementDomainModel;
			}
			protected override bool VerifyElementType(ModelElement element)
			{
				DomainModelInfo modelFilter = myDomainModelFilter;
				return (modelFilter != null) ? element.GetDomainClass().DomainModel == modelFilter : true;
			}
			/// <summary>
			/// Add this element to the appropriate dictionary, and allow
			/// IDuplicateNameCollectionManager implementations to validate
			/// their current content.
			/// </summary>
			/// <param name="element">An IModelErrorOwner instance</param>
			/// <param name="store">The context store</param>
			/// <param name="notifyAdded">The listener to notify if elements are added during fixup</param>
			protected sealed override void ProcessElement(INamedElementDictionaryLink element, Store store, INotifyElementAdded notifyAdded)
			{
				HandleDeserializationAdd(element, notifyAdded);
			}
		}
		#endregion Deserialization Fixup
		#region Store integration
		/// <summary>
		/// Translate the current store context settings into
		/// a duplicate name action setting
		/// </summary>
		/// <param name="element">The element to add</param>
		/// <param name="contextKey">The key to look for</param>
		/// <returns>DuplicateNameAction (ModifyDuplicateCollection or ThrowOnDuplicateName)</returns>
		private static DuplicateNameAction GetDuplicateNameActionForRule(ModelElement element, object contextKey)
		{
			DuplicateNameAction duplicateAction = DuplicateNameAction.ThrowOnDuplicateName;
			bool reverseKeyDefault = false;
			Store store = element.Store;
			if (contextKey == null)
			{
				contextKey = DefaultAllowDuplicateNamesKey;
			}
			else if (contextKey == AllowDuplicateNamesKey)
			{
				contextKey = null;
				duplicateAction = DuplicateNameAction.ModifyDuplicateCollection;
			}
			else if (contextKey == BlockDuplicateNamesKey)
			{
				contextKey = null;
			}
			else if (contextKey is INamedElementDictionaryContextKeyBlocksDuplicates)
			{
				reverseKeyDefault = true;
				duplicateAction = DuplicateNameAction.ModifyDuplicateCollection;
			}

			if (contextKey != null)
			{
				if (element.Store.TransactionManager.CurrentTransaction.TopLevelTransaction.Context.ContextInfo.ContainsKey(contextKey))
				{
					duplicateAction = reverseKeyDefault ? DuplicateNameAction.ThrowOnDuplicateName : DuplicateNameAction.ModifyDuplicateCollection;
				}
			}
			return duplicateAction;
		}
		/// <summary>
		/// AddRule: typeof(ElementLink), Priority=NamedElementDictionary.RulePriority;
		/// </summary>
		private static void ElementLinkAddedRule(ElementAddedEventArgs e)
		{
			HandleAddRemove(e.ModelElement, false, false);
		}
		/// <summary>
		/// DeletingRule: typeof(ElementLink), Priority=NamedElementDictionary.RulePriority;
		/// </summary>
		private static void ElementLinkDeletingRule(ElementDeletingEventArgs e)
		{
			HandleAddRemove(e.ModelElement, true, false);
		}
		private static void ElementLinkAddedEvent(object sender, ElementAddedEventArgs e)
		{
			HandleAddRemove(e.ModelElement, false, true);
		}
		private static void ElementLinkRemovedEvent(object sender, ElementDeletedEventArgs e)
		{
			HandleAddRemove(e.ModelElement, true, true);
		}
		/// <summary>
		/// A simple structure used for rolling back changes to the
		/// dictionary when a transaction rolls back. The log entry
		/// are only used for rollbacks, there is not enough information
		/// to replay a change.
		/// </summary>
		private struct EntryStateChange
		{
			/// <summary>
			/// A dictionary of either single NamedElementDictionary or
			/// collections of NamedElementDictionary implementations that
			/// have changes during a live transaction.
			/// </summary>
			private static Dictionary<Transaction, object> myTransactions = new Dictionary<Transaction, object>();
			/// <summary>
			/// Called before an entry is added/removed/modified in the
			/// dictionary.
			/// </summary>
			/// <param name="element">The element being modified. Used to get the current transaction.</param>
			/// <param name="dictionary">The dictionary being modified</param>
			/// <param name="name">The name of the element</param>
			/// <param name="value">The element value before the change</param>
			public static void OnEntryChange(ModelElement element, NamedElementDictionary dictionary, string name, object value)
			{
				Transaction transaction = element.Store.TransactionManager.CurrentTransaction;
				if (transaction.IsNested)
				{
					transaction = transaction.TopLevelTransaction;
				}
				// First, make sure this dictionary is logged for this transaction
				// in our transaction-keyed dictionary.
				object newEntry = null;
				object currentValue;
				if (myTransactions.TryGetValue(transaction, out currentValue))
				{
					// Check for simple case first (only one dictionary has
					// changes in this transaction)
					if (currentValue != dictionary)
					{
						List<NamedElementDictionary> list = currentValue as List<NamedElementDictionary>;
						if (list == null)
						{
							list = new List<NamedElementDictionary>();
							list.Add((NamedElementDictionary)currentValue);
							list.Add(dictionary);
							newEntry = list;
						}
						else if (!list.Contains(dictionary))
						{
							list.Add(dictionary);
						}
					}
				}
				else
				{
					// Add as a single. This will be the most common case as the
					// majority of transactions are small.
					newEntry = dictionary;
				}
				if (newEntry != null)
				{
					myTransactions[transaction] = newEntry;
				}
				EnsureStack(ref dictionary.myChangeStack).Push(new EntryStateChange(name, value));
			}
			/// <summary>
			/// The transaction was successful. Clear all associated change logs and
			/// stop watching this transaction.
			/// </summary>
			/// <param name="transaction">The transaction being committed</param>
			public static void TransactionCommitted(Transaction transaction)
			{
				if (!transaction.IsNested)
				{
					object dictionaryList;
					if (myTransactions.TryGetValue(transaction, out dictionaryList))
					{
						NamedElementDictionary singleDictionary = dictionaryList as NamedElementDictionary;
						if (singleDictionary != null)
						{
							singleDictionary.myChangeStack = null;
						}
						else
						{
							foreach (NamedElementDictionary dictionary in (List<NamedElementDictionary>)dictionaryList)
							{
								dictionary.myChangeStack = null;
							}
						}
						myTransactions.Remove(transaction);
					}
				}
			}
			/// <summary>
			/// Return all dictionaries with changes during this transaction back to their
			/// initial states.
			/// </summary>
			/// <param name="transaction"></param>
			public static void TransactionRolledBack(Transaction transaction)
			{
				if (!transaction.IsNested)
				{
					object dictionaryList;
					if (myTransactions.TryGetValue(transaction, out dictionaryList))
					{
						NamedElementDictionary singleDictionary = dictionaryList as NamedElementDictionary;
						if (singleDictionary != null)
						{
							RollBackDictionary(singleDictionary);
						}
						else
						{
							foreach (NamedElementDictionary dictionary in (List<NamedElementDictionary>)dictionaryList)
							{
								RollBackDictionary(dictionary);
							}
						}
						myTransactions.Remove(transaction);
					}
				}
			}
			private static void RollBackDictionary(NamedElementDictionary elementDictionary)
			{
				Stack<EntryStateChange> stack = elementDictionary.myChangeStack;
				if (stack.Count != 0)
				{
					Dictionary<string, object> dic = elementDictionary.myDictionary;
					// Even if we restore the original objects, the objects themselves
					// may not be in the original state they were in when they were added
					// to the dictionary. This happens specifically when a native backing
					// collection is maintained by a collection that stores multiple elements,
					// and a collection snapshot is not returned by the duplicate name collection
					// manager. In this case, we need to give the dictionary a chance to restore
					// the state on its objects after the dictionary itself is restored.
					Dictionary<string, object> trackingDictionary = null;
					while (stack.Count != 0)
					{
						EntryStateChange change = stack.Pop();
						string changeName = change.Name;
						if (change.Value == null)
						{
							dic.Remove(changeName);
							if (trackingDictionary != null &&
								trackingDictionary.ContainsKey(changeName))
							{
								trackingDictionary.Remove(changeName);
							}
						}
						else
						{
							object changeValue = change.Value;
							dic[changeName] = changeValue;
							(trackingDictionary ?? (trackingDictionary = new Dictionary<string, object>()))[changeName] = changeValue;
						}
					}
					if (trackingDictionary != null)
					{
						IDuplicateNameCollectionManager duplicateNameManager = elementDictionary.myDuplicateManager;
						foreach (object latestValue in trackingDictionary.Values)
						{
							ICollection collection;
							if (null != (collection = latestValue as ICollection))
							{
								duplicateNameManager.AfterCollectionRollback(collection);
							}
						}
					}
				}
				elementDictionary.myChangeStack = null;
			}
			private static Stack<EntryStateChange> EnsureStack(ref Stack<EntryStateChange> changeStack)
			{
				if (changeStack == null)
				{
					changeStack = new Stack<EntryStateChange>();
				}
				return changeStack;
			}
			private EntryStateChange(string name, object value)
			{
				Name = name;
				Value = value;
			}
			/// <summary>
			/// The new name. Set for a name change, null otherwise.
			/// </summary>
			public string Name;
			/// <summary>
			/// The element or collection stored in this named slot prior to the change.
			/// A null value here indicates that the named slot did not exist
			/// prior to the change and should be removed on rollback.
			/// </summary>
			public object Value;
		}
		private static void TransactionCommittedEvent(object sender, TransactionCommitEventArgs e)
		{
			EntryStateChange.TransactionCommitted(e.Transaction);
		}
		private static void TransactionRolledBackEvent(object sender, TransactionRollbackEventArgs e)
		{
			EntryStateChange.TransactionRolledBack(e.Transaction);
		}
		/// <summary>
		/// See discussion of the need for DetachedElementRecord in the HandleElementChanged
		/// routine.
		/// </summary>
		private struct DetachedElementRecord
		{
			/// <summary>
			/// Create a new record, recording the old and
			/// new names for the element
			/// </summary>
			public DetachedElementRecord(string oldName, string newName)
			{
				OldName = oldName;
				NewName = newName;
				SingleDictionary = null;
				Dictionaries = null;
			}
			/// <summary>
			/// Create a new record, recording the dictionary
			/// it should be removed from
			/// </summary>
			public DetachedElementRecord(INamedElementDictionary singleDictionary)
			{
				SingleDictionary = singleDictionary;
				Dictionaries = null;
				OldName = null;
				NewName = null;
			}
			public string OldName;
			public string NewName;
			public INamedElementDictionary SingleDictionary;
			public List<INamedElementDictionary> Dictionaries;
			/// <summary>
			/// Merge an existing record into this one. The dictionaries
			/// and old name are pulled from the existing record, while the
			/// new name of this record is preserved
			/// </summary>
			public void MergeExisting(ref DetachedElementRecord existingRecord)
			{
				if (existingRecord.OldName != null)
				{
					Debug.Assert(existingRecord.NewName == OldName);
					OldName = existingRecord.OldName;
				}
				SingleDictionary = existingRecord.SingleDictionary;
				Dictionaries = existingRecord.Dictionaries;
			}
			/// <summary>
			/// Add a tracked dictionary to this record
			/// </summary>
			/// <param name="dictionary"></param>
			public void AddDictionary(INamedElementDictionary dictionary)
			{
				if (SingleDictionary != null)
				{
					Debug.Assert(Dictionaries == null); // Have either a single or multiple
					if (SingleDictionary != dictionary)
					{
						Dictionaries = new List<INamedElementDictionary>();
						Dictionaries.Add(SingleDictionary);
						Dictionaries.Add(dictionary);
						SingleDictionary = null;
					}
				}
				else if (Dictionaries != null)
				{
					if (!Dictionaries.Contains(dictionary))
					{
						Dictionaries.Add(dictionary);
					}
				}
				else
				{
					SingleDictionary = dictionary;
				}
			}
		}
		private static Dictionary<ModelElement, DetachedElementRecord> myDetachedElementRecords;
		// Track direct connections for link elements with a remote dictionary type.
		// This item is keyed off the parent element of the link. This parent key
		// will be a child in the immediate connector parent, which is what the
		// remote links are keyed off. The remote links are then followed up the
		// remote chain until the parent is an owner of the remote dictionary key
		// provided by the parent object, or the parent is not deleted and can be
		// directly followed.
		private struct LinkAndDictionaryType
		{
			public readonly INamedElementDictionaryLink Link;
			public readonly Type DictionaryType;
			public LinkAndDictionaryType(INamedElementDictionaryLink link, Type dictionaryType)
			{
				Link = link;
				DictionaryType = dictionaryType;
			}
		}
		private static Dictionary<ModelElement, LinkedNode<LinkAndDictionaryType>> myDetachedLinksWithRemoteDictionary;
		private static Dictionary<ModelElement, INamedElementDictionaryLink> myDetachedRemoteDictionaryConnectorLinks;
		private static void ElementEventsEndedEvent(object sender, ElementEventsEndedEventArgs e)
		{
			Dictionary<ModelElement, DetachedElementRecord> changes = myDetachedElementRecords;
			Dictionary<ModelElement, LinkedNode<LinkAndDictionaryType>> detachedRemotePrimaryLinks = myDetachedLinksWithRemoteDictionary;
			Dictionary<ModelElement, INamedElementDictionaryLink> detachedRemoteConnectorLinks = myDetachedRemoteDictionaryConnectorLinks;

			// Toss unused tracked changes when events are finished
			myDetachedElementRecords = null;
			myDetachedLinksWithRemoteDictionary = null;
			myDetachedRemoteDictionaryConnectorLinks = null;

			if (detachedRemotePrimaryLinks != null &&
				detachedRemoteConnectorLinks != null)
			{
				foreach (KeyValuePair<ModelElement, LinkedNode<LinkAndDictionaryType>> primaryLinkPair in detachedRemotePrimaryLinks)
				{
					ModelElement primaryParent = primaryLinkPair.Key;
					LinkedNode<LinkAndDictionaryType> linkAndTypeNode = primaryLinkPair.Value;
					INamedElementDictionary resolvedDictionary = null;
					Type previousDictionaryType = null;
					while (linkAndTypeNode != null)
					{
						LinkAndDictionaryType linkAndType = linkAndTypeNode.Value;
						Type dictionaryType = linkAndType.DictionaryType;
						if (dictionaryType == previousDictionaryType)
						{
							if (resolvedDictionary == null)
							{
								continue;
							}
						}
						else
						{
							// The dictionary types are usually the same, do some basic optimization
							// so we only retrieve the dictionary one time.
							previousDictionaryType = dictionaryType;
							resolvedDictionary = null;
						}

						if (resolvedDictionary == null)
						{
							// Go up the deleted chain as far as we can
							ModelElement resolvedParent = primaryParent;
							INamedElementDictionaryOwner dictionaryOwner;
							for (; ; )
							{
								INamedElementDictionaryLink deletedConnectingLink;
								if (detachedRemoteConnectorLinks.TryGetValue(resolvedParent, out deletedConnectingLink))
								{
									resolvedParent = (ModelElement)deletedConnectingLink.ParentRolePlayer;
									if (null != (dictionaryOwner = resolvedParent as INamedElementDictionaryOwner) &&
										null != (resolvedDictionary = dictionaryOwner.FindNamedElementDictionary(dictionaryType)))
									{
										break;
									}
									resolvedParent = (ModelElement)deletedConnectingLink.ParentRolePlayer;
									if (!(resolvedParent is INamedElementDictionaryRemoteChild))
									{
										resolvedParent = null; // The next loop won't resolve, so don't bother.
										break;
									}
									continue;
								}
								else if (null != (dictionaryOwner = resolvedParent as INamedElementDictionaryOwner))
								{
									// Check current parent for a dictionary before going into the loop below.
									resolvedDictionary = dictionaryOwner.FindNamedElementDictionary(dictionaryType);
								}
								break;
							}

							if (null == resolvedDictionary &&
								null != resolvedParent)
							{
								// We got as far up the parent stack as we could get using
								// cached detached objects. We can now assume that the remainder
								// of the links are still in the model. Use the available information
								// on the named element dictionary interfaces to find the dictionary.
								INamedElementDictionaryRemoteChild remoteChildInfo;
								Guid parentRoleId;
								DomainRoleInfo parentRoleInfo;
								DomainDataDirectory dataDirectory = resolvedParent.Store.DomainDataDirectory;
								while (null != resolvedParent &&
									!resolvedParent.IsDeleted &&
									null != (remoteChildInfo = resolvedParent as INamedElementDictionaryRemoteChild) &&
									(parentRoleId = remoteChildInfo.NamedElementDictionaryParentRole) != Guid.Empty &&
									null != (parentRoleInfo = dataDirectory.FindDomainRole(parentRoleId)) &&
									parentRoleInfo.IsOne &&
									null != (resolvedParent = parentRoleInfo.GetLinkedElement(resolvedParent)))
								{
									if (null != (dictionaryOwner = resolvedParent as INamedElementDictionaryOwner) &&
										null != (resolvedDictionary = dictionaryOwner.FindNamedElementDictionary(dictionaryType)))
									{
										resolvedParent = null;
									}
								}
							}
						}
						if (null != resolvedDictionary)
						{
							ModelElement namedChild = (ModelElement)linkAndType.Link.ChildRolePlayer;
							DetachedElementRecord changeRecord;
							if (changes != null &&
								changes.TryGetValue(namedChild, out changeRecord))
							{
								changeRecord.AddDictionary(resolvedDictionary);
								changes[namedChild] = changeRecord;
							}
							else
							{
								resolvedDictionary.RemoveElement(namedChild, null, DuplicateNameAction.RetrieveDuplicateCollection);
							}
						}
						linkAndTypeNode = linkAndTypeNode.Next;
					}
				}
			}

			// The name will have stabilized at this point, remove it
			if (changes != null)
			{
				foreach (KeyValuePair<ModelElement, DetachedElementRecord> keyAndValue in changes)
				{
					DetachedElementRecord changeRecord = keyAndValue.Value;
					string startingName = changeRecord.OldName;
					ModelElement element = keyAndValue.Key;
					if (changeRecord.SingleDictionary != null)
					{
						changeRecord.SingleDictionary.RemoveElement(element, startingName, DuplicateNameAction.RetrieveDuplicateCollection);
					}
					else if (changeRecord.Dictionaries != null)
					{
						foreach (INamedElementDictionary dictionary in changeRecord.Dictionaries)
						{
							dictionary.RemoveElement(element, startingName, DuplicateNameAction.RetrieveDuplicateCollection);
						}
					}
				}
			}
		}
		/// <summary>
		/// Add or remove elements to associated named element
		/// dictionaries when a link is added
		/// </summary>
		/// <param name="element">ModelElement to add or remove</param>
		/// <param name="remove">true to remove, false to add</param>
		/// <param name="forEvent">This call is handling an event</param>
		private static void HandleAddRemove(ModelElement element, bool remove, bool forEvent)
		{
			if (forEvent)
			{
				UndoManager undoMgr = element.Store.UndoManager;
				if (!(undoMgr.InUndo || undoMgr.InRedo))
				{
					return;
				}
			}
			INamedElementDictionaryLink link = element as INamedElementDictionaryLink;
			if (link != null)
			{
				HandleAddRemove(link, element, remove, forEvent, null);
			}
		}
		/// <summary>
		/// Add an element resulting from deserialization fixup
		/// </summary>
		/// <param name="link">The link to add</param>
		/// <param name="notifyAdded">A notification interface</param>
		private static void HandleDeserializationAdd(INamedElementDictionaryLink link, INotifyElementAdded notifyAdded)
		{
			ModelElement mel;
			if (null == (mel = link as ModelElement) || !mel.IsDeleted)
			{
				HandleAddRemove(link, null, false, false, notifyAdded);
			}
		}
		/// <summary>
		/// Add or remove elements to associated named element
		/// dictionaries when a link is added
		/// </summary>
		/// <param name="link">INamedElementDictionaryLink to add</param>
		/// <param name="element">The ModelElement (same object as link). Not required if forEvent is false</param>
		/// <param name="remove">true to remove, false to add</param>
		/// <param name="forEvent">This call is handling an event</param>
		/// <param name="notifyAdded">The listener to notify if elements are added during fixup.
		/// Passed through the INamedElementDictionary.AddElement</param>
		private static void HandleAddRemove(INamedElementDictionaryLink link, ModelElement element, bool remove, bool forEvent, INotifyElementAdded notifyAdded)
		{
			Debug.Assert(element != null || !forEvent);
			INamedElementDictionaryChildNode childNode;
			INamedElementDictionaryParentNode parentNode;
			if (null != link &&
				null != (childNode = link.ChildRolePlayer) &&
				null != (parentNode = link.ParentRolePlayer))
			{
				INamedElementDictionaryParent parent;
				INamedElementDictionaryChild child;
				ModelElement namedChild;
				NamedElementDictionaryLinkUse linkUse = link.DictionaryLinkUse;
				if (0 != (linkUse & NamedElementDictionaryLinkUse.DirectDictionary) &&
					(null != (parent = parentNode as INamedElementDictionaryParent)) &&
					(null != (child = childNode as INamedElementDictionaryChild)) &&
					(null != (namedChild = child as ModelElement)))
				{
					Guid parentRoleGuid;
					Guid childRoleGuid;
					child.GetRoleGuids(out parentRoleGuid, out childRoleGuid);
					INamedElementDictionary dictionary = parent.GetCounterpartRoleDictionary(parentRoleGuid, childRoleGuid);
					if (dictionary != null)
					{
						Type remoteType = dictionary.RemoteDictionaryType;
						if (remoteType == null) // The dictionary is reachable from the connected objects
						{
							DuplicateNameAction duplicateAction;
							if (forEvent)
							{
								duplicateAction = DuplicateNameAction.RetrieveDuplicateCollection;
								if (remove && element.IsDeleted && myDetachedElementRecords != null)
								{
									DetachedElementRecord changeRecord;
									if (myDetachedElementRecords.TryGetValue(namedChild, out changeRecord))
									{
										changeRecord.AddDictionary(dictionary);
										myDetachedElementRecords[namedChild] = changeRecord;
										return; // Handle all of these at the end of the events
									}
								}
							}
							else
							{
								duplicateAction = GetDuplicateNameActionForRule(
									namedChild,
									parent.GetAllowDuplicateNamesContextKey(parentRoleGuid, childRoleGuid));
							}
							if (remove)
							{
								if (!dictionary.RemoveElement(namedChild, null, duplicateAction) &&
									forEvent)
								{
									string elementName = DomainClassInfo.GetName(namedChild);
									if (elementName != null)
									{
										DetachedElementRecord changeRecord;
										if (myDetachedElementRecords == null)
										{
											myDetachedElementRecords = new Dictionary<ModelElement, DetachedElementRecord>();
											changeRecord = new DetachedElementRecord(dictionary);
										}
										else
										{
											DetachedElementRecord existingRecord;
											if (myDetachedElementRecords.TryGetValue(namedChild, out existingRecord))
											{
												existingRecord.AddDictionary(dictionary);
												changeRecord = existingRecord;
											}
											else
											{
												changeRecord = new DetachedElementRecord(dictionary);
											}
										}
										myDetachedElementRecords[namedChild] = changeRecord;
									}
								}
							}
							else
							{
								dictionary.AddElement(namedChild, duplicateAction, notifyAdded);
							}
						}
						else if (forEvent)
						{
							// Cache the link keyed off the parent element. We also cache remote
							// connectors for events, which cache of the child element. The child
							// element in those cases matches the primary element here.
							Dictionary<ModelElement, LinkedNode<LinkAndDictionaryType>> remoteLinks = myDetachedLinksWithRemoteDictionary;
							if (remoteLinks == null)
							{
								myDetachedLinksWithRemoteDictionary = remoteLinks = new Dictionary<ModelElement, LinkedNode<LinkAndDictionaryType>>();
							}
							ModelElement linkKey = (ModelElement)parent;
							LinkedNode<LinkAndDictionaryType> newNode = new LinkedNode<LinkAndDictionaryType>(new LinkAndDictionaryType(link, remoteType));
							LinkedNode<LinkAndDictionaryType> prevNode;
							if (remoteLinks.TryGetValue(linkKey, out prevNode))
							{
								newNode.SetNext(prevNode, ref newNode);
							}
							remoteLinks[linkKey] = newNode;
						}
					}
				}

				// Now handle any remote parents associated with this link. Remote
				// work only needs to be done for adds inside a transaction. The rest
				// handles itself automatically.
				INamedElementDictionaryRemoteChild remoteChild;
				if (forEvent)
				{
					if (remove &&
						0 != (linkUse & NamedElementDictionaryLinkUse.DictionaryConnector) &&
						null != (remoteChild = childNode as INamedElementDictionaryRemoteChild))
					{
						// Cache the link keyed off the child element to allow us to reconstruct
						// the remote ownership chain.
						Dictionary<ModelElement, INamedElementDictionaryLink> remoteConnectors = myDetachedRemoteDictionaryConnectorLinks;
						if (remoteConnectors == null)
						{
							myDetachedRemoteDictionaryConnectorLinks = remoteConnectors = new Dictionary<ModelElement, INamedElementDictionaryLink>();
						}
						remoteConnectors[(ModelElement)childNode] = link;
					}
				}
				else if (!remove &&
					notifyAdded == null &&
					0 != (linkUse & NamedElementDictionaryLinkUse.DictionaryConnector) &&
					null != (remoteChild = childNode as INamedElementDictionaryRemoteChild))
				{
					Guid[] remoteRoleGuids = remoteChild.GetNamedElementDictionaryChildRoles();
					int remoteRoleGuidsCount;
					if (remoteRoleGuids != null && 0 != (remoteRoleGuidsCount = remoteRoleGuids.Length))
					{
						ModelElement parentElement = (ModelElement)remoteChild;
						for (int i = 0; i < remoteRoleGuidsCount; ++i)
						{

							ReadOnlyCollection<ElementLink> remoteLinks = DomainRoleInfo.GetElementLinks<ElementLink>(parentElement, remoteRoleGuids[i]);
							int remoteLinksCount = remoteLinks.Count;
							for (int j = 0; j < remoteLinksCount; ++j)
							{
								ElementLink remoteLinkElement = remoteLinks[j];
								INamedElementDictionaryLink remoteLink = remoteLinkElement as INamedElementDictionaryLink;
								if (remoteLink != null)
								{
									HandleAddRemove(remoteLink, remoteLinkElement, false, false, null);
								}
							}
						}
					}
				}
			}
		}
		// UNDONE: RolePlayerChange
		/// <summary>
		/// ChangeRule: typeof(ModelElement), Priority=NamedElementDictionary.RulePriority;
		/// </summary>
		private static void NamedElementChangedRule(ElementPropertyChangedEventArgs e)
		{
			HandleElementChanged(e, false);
		}
		private static void NamedElementChangedEvent(object sender, ElementPropertyChangedEventArgs e)
		{
			HandleElementChanged(e, true);
		}
		private static void HandleElementChanged(ElementPropertyChangedEventArgs e, bool forEvent)
		{
			ModelElement element = e.ModelElement;
			if (forEvent)
			{
				UndoManager undoMgr = element.Store.UndoManager;
				if (!(undoMgr.InUndo || undoMgr.InRedo))
				{
					return;
				}
			}
			INamedElementDictionaryChild child = element as INamedElementDictionaryChild;
			if (child != null)
			{
				if (e.DomainProperty == e.DomainClass.NameDomainProperty)
				{
					Guid parentRoleGuid;
					Guid childRoleGuid;
					child.GetRoleGuids(out parentRoleGuid, out childRoleGuid);
					ModelElement namedChild = child as ModelElement;
					LinkedElementCollection<ModelElement> parents = namedChild.Store.DomainDataDirectory.GetDomainRole(childRoleGuid).GetLinkedElements(namedChild);
					int parentsCount = parents.Count;
					if (parentsCount == 0 && forEvent && element.IsDeleted)
					{
						// Handle a problem with events. The counterpart collection is always empty
						// when we get here during events, so there is no way to get back to the parent object
						// until we get the remove event for the element. However, the remove event will get
						// the element with the new value, not the old. Track this change.
						DetachedElementRecord changeRecord = new DetachedElementRecord(e.OldValue as string, e.NewValue as string);
						if (changeRecord.OldName != null)
						{
							if (myDetachedElementRecords == null)
							{
								myDetachedElementRecords = new Dictionary<ModelElement, DetachedElementRecord>();
							}
							else
							{
								DetachedElementRecord existingRecord;
								if (myDetachedElementRecords.TryGetValue(namedChild, out existingRecord))
								{
									changeRecord.MergeExisting(ref existingRecord);
								}
							}
							myDetachedElementRecords[namedChild] = changeRecord;
						}
					}
					for (int i = 0; i < parentsCount; ++i)
					{
						INamedElementDictionaryParent parent;
						INamedElementDictionary dictionary;
						if (null != (parent = parents[i] as INamedElementDictionaryParent) &&
							null != (dictionary = parent.GetCounterpartRoleDictionary(parentRoleGuid, childRoleGuid)) &&
							null == dictionary.RemoteDictionaryType)
						{
							DuplicateNameAction duplicateAction;
							if (forEvent)
							{
								duplicateAction = DuplicateNameAction.RetrieveDuplicateCollection;
							}
							else
							{
								duplicateAction = GetDuplicateNameActionForRule(
									namedChild,
									parent.GetAllowDuplicateNamesContextKey(parentRoleGuid, childRoleGuid));
							}
							dictionary.RenameElement(namedChild, e.OldValue as string, e.NewValue as string, duplicateAction);
						}
					}
				}
			}
		}
		/// <summary>
		/// Call from <see cref="IModelingEventSubscriber.ManageModelingEventHandlers"/>
		/// implementations to attach <see cref="EventHandler{TEventArgs}"/>s that correctly deal with undo and redo scenarios.
		/// </summary>
		/// <param name="store">The <see cref="Store"/> for which the <see cref="EventHandler{TEventArgs}"/>s should be managed.</param>
		/// <param name="eventManager">The <see cref="ModelingEventManager"/> used to manage the <see cref="EventHandler{TEventArgs}"/>s.</param>
		/// <param name="action">The <see cref="EventHandlerAction"/> that should be taken for the <see cref="EventHandler{TEventArgs}"/>s.</param>
		public static void ManageModelStateEventHandlers(Store store, ModelingEventManager eventManager, EventHandlerAction action)
		{
			
			DomainDataDirectory dataDirectory = store.DomainDataDirectory;
			DomainClassInfo classInfo = dataDirectory.FindDomainRelationship(ElementLink.DomainClassId);

			// Track ElementLink changes
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(ElementLinkAddedEvent), action);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(ElementLinkRemovedEvent), action);
			// UNDONE: RolePlayerChanged

			// Track ModelElement 
			classInfo = dataDirectory.FindDomainClass(ModelElement.DomainClassId);
			eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementPropertyChangedEventArgs>(NamedElementChangedEvent), action);

			eventManager.AddOrRemoveHandler(new EventHandler<ElementEventsEndedEventArgs>(ElementEventsEndedEvent), action);

			// Track commit and rollback events so we can rollback/abandon a change log as needed.
			eventManager.AddOrRemoveHandler(new EventHandler<TransactionCommitEventArgs>(TransactionCommittedEvent), action);
			eventManager.AddOrRemoveHandler(new EventHandler<TransactionRollbackEventArgs>(TransactionRolledBackEvent), action);
		}
		#endregion // Store integration
		#region Remote handling
		#region RemoteDictionaryToken class
		private sealed class RemoteDictionaryToken : INamedElementDictionary
		{
			#region Member Variables
			private Type myDictionaryType;
			#endregion // Member Variables
			#region Constructor
			/// <summary>
			/// Create a dictionary token for a given type
			/// </summary>
			public RemoteDictionaryToken(Type dictionaryType)
			{
				myDictionaryType = dictionaryType;
			}
			#endregion // Constructor
			#region INamedElementDictionary Implementation
			void INamedElementDictionary.AddElement(ModelElement element, DuplicateNameAction duplicateAction, INotifyElementAdded notifyAdded)
			{
			}
			bool INamedElementDictionary.RemoveElement(ModelElement element, string alternateElementName, DuplicateNameAction duplicateAction)
			{
				return false;
			}
			void INamedElementDictionary.ReplaceElement(ModelElement originalElement, ModelElement replacementElement, DuplicateNameAction duplicateAction)
			{
			}
			void INamedElementDictionary.RenameElement(ModelElement element, string oldName, string newName, DuplicateNameAction duplicateAction)
			{
			}
			LocatedElement INamedElementDictionary.GetElement(string elementName)
			{
				return LocatedElement.Empty;
			}
			Type INamedElementDictionary.RemoteDictionaryType
			{
				get
				{
					return myDictionaryType;
				}
			}
			#endregion // INamedElementDictionary Implementation
		}
		#endregion // RemoteDictionaryToken class
		private static Dictionary<Type, RemoteDictionaryToken> myDictionaryTokens;
		/// <summary>
		/// Get a token representing a remote dictionary that is
		/// not currently reachable by a direct path.
		/// </summary>
		/// <param name="dictionaryType">A type tha will retrieve the dictionary
		/// from the resolve <see cref="INamedElementDictionaryOwner"/></param>
		/// <returns>A bare <see cref="INamedElementDictionary"/> implementation
		/// setting the remote dictionary type.</returns>
		public static INamedElementDictionary GetRemoteDictionaryToken(Type dictionaryType)
		{
			Dictionary<Type, RemoteDictionaryToken> tokens;
			RemoteDictionaryToken retVal;
			if (null == (tokens = myDictionaryTokens))
			{
				System.Threading.Interlocked.CompareExchange<Dictionary<Type, RemoteDictionaryToken>>(ref myDictionaryTokens, new Dictionary<Type, RemoteDictionaryToken>(), null);
				tokens = myDictionaryTokens;
			}
			else if (tokens.TryGetValue(dictionaryType, out retVal))
			{
				return retVal;
			}
			lock (tokens)
			{
				if (tokens.TryGetValue(dictionaryType, out retVal))
				{
					return retVal;
				}
				retVal = new RemoteDictionaryToken(dictionaryType);
				tokens.Add(dictionaryType, retVal);
			}
			return retVal;
		}
		#endregion // Remote handling
	}
	#endregion // NamedElementDictionary class
}
