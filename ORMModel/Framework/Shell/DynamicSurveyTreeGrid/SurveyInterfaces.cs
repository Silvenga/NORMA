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
using System.Text;
using Microsoft.VisualStudio.Modeling;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Neumont.Tools.Modeling.Shell.DynamicSurveyTreeGrid
{
	#region SurveyQuestionUISupport enum
	/// <summary>
	/// Survey question ui support. Indicates how answers to this
	/// questions may be used when presenting elements that answer
	/// the associated <see cref="ISurveyQuestionTypeInfo"/>
	/// </summary>
	[Flags]
	public enum SurveyQuestionUISupport
	{
		/// <summary>
		/// If nothing is supported by question
		/// </summary>
		None = 0,
		/// <summary>
		/// If sorting is supported by question
		/// </summary>
		Sorting = 1,
		/// <summary>
		/// If grouping is supported, implies that sorting is also supported
		/// </summary>
		Grouping = 2,
		/// <summary>
		/// Answers to the question can be resolved to an image index that is
		/// used for a primary glyph. Index resolution is performed with the
		/// <see cref="ISurveyQuestionTypeInfo.MapAnswerToImageIndex"/> method.
		/// </summary>
		Glyph = 4,
		/// <summary>
		/// Answers to the question can be resolved to an image index that is
		/// used as an overlay glyph. Index resolution is performed with the
		/// <see cref="ISurveyQuestionTypeInfo.MapAnswerToImageIndex"/> method.
		/// </summary>
		Overlay = 8,
		/// <summary>
		/// Question supports additional display settings with the
		/// <see cref="ISurveyQuestionTypeInfo.GetDisplayData"/> method.
		/// </summary>
		DisplayData = 0x10,
	}
	#endregion // SurveyQuestionUISupport enum
	#region ISurveyQuestionProvider interface
	/// <summary>
	/// An ISurveyQuestionProvider can return an array of ISurveyQuestionTypeInfo
	/// </summary>
	public interface ISurveyQuestionProvider
	{
		/// <summary>
		/// Retrieve the supported <see cref="ISurveyQuestionTypeInfo"/> instances
		/// </summary>
		/// <param name="expansionKey">The expansion key indicating the type of expansion
		/// data to retrieve for the provided context. Expansion keys are also used by
		/// <see cref="ISurveyNode.SurveyNodeExpansionKey"/> and <see cref="ISurveyNodeProvider.GetSurveyNodes"/></param>
		/// <returns><see cref="IEnumerable{ISurveyQuestionTypeInfo}"/> representing questions supported by this provider</returns>
		IEnumerable<ISurveyQuestionTypeInfo> GetSurveyQuestions(object expansionKey);
		/// <summary>
		/// The <see cref="ImageList"/> associated with answers to all supported questions
		/// </summary>
		ImageList SurveyQuestionImageList { get;}
        /// <summary>
        /// Get the list of types for this survey provider that correspond to
        /// error state display changes. Each returned type should correspond to one
        /// of the <see cref="ISurveyQuestionTypeInfo.QuestionType"/> types returned
        /// by the <see cref="GetSurveyQuestions"/>
        /// </summary>
        /// <returns><see cref="IEnumerable{Type}"/> or null</returns>
        IEnumerable<Type> GetErrorDisplayTypes();
	}
	#endregion // ISurveyQuestionProvider interface
	#region SurveyQuestionDisplayData structure
	/// <summary>
	/// Associate additional display information with this question.
	/// Used by the <see cref="ISurveyQuestionTypeInfo.GetDisplayData"/> method.
	/// </summary>
	public struct SurveyQuestionDisplayData
	{
		private bool myIsBold;
		private bool myIsGrayText;
		/// <summary>
		/// Default display settings
		/// </summary>
		public static readonly SurveyQuestionDisplayData Default = new SurveyQuestionDisplayData();
		/// <summary>
		/// Create a new <see cref="SurveyQuestionDisplayData"/>
		/// </summary>
		/// <param name="isBold">Display text as bold</param>
		/// <param name="isGrayText">Display text as gray text</param>
		public SurveyQuestionDisplayData(bool isBold, bool isGrayText)
		{
			myIsBold = isBold;
			myIsGrayText = isGrayText;
		}
		/// <summary>
		/// Return <see langword="true"/> if these are the default settings
		/// </summary>
		public bool IsDefault
		{
			get
			{
				return !(myIsBold || myIsGrayText);
			}
		}
		/// <summary>
		/// Should the text display bold?
		/// </summary>
		public bool Bold
		{
			get
			{
				return myIsBold;
			}
		}
		/// <summary>
		/// Should the text display gray?
		/// </summary>
		public bool GrayText
		{
			get
			{
				return myIsGrayText;
			}
		}
	}
	#endregion // SurveyQuestionDisplayData structure
	#region ISurveyQuestionTypeInfo interface
	/// <summary>
	/// Holds a reference to a specific type of question and method for asking question of objects
	/// </summary>
	public interface ISurveyQuestionTypeInfo
	{
		/// <summary>
		/// The type of question that this ISurveyQuestionTypeInfo represents.
		/// The return type must be an <see cref="Enum"/> or implement <see cref="ISurveyDynamicValues"/>
		/// </summary>
		Type QuestionType { get;}
		/// <summary>
		/// If the <see cref="QuestionType"/> represents a set of dynamic values,
		/// then return the dynamic value instant.
		/// </summary>
		ISurveyDynamicValues DynamicQuestionValues { get;}
		/// <summary>
		/// Retrieve the answer of any object to my question
		/// </summary>
		/// <param name="data">The data object to query for an answer to this question.</param>
		/// <param name="contextElement">The context container element for this data. This information
		/// should be derivable for a primary element, but not for references to primary elements, which
		/// may provide different answers for different context elements.</param>
		/// <returns>Answer to question, or -1 if the provided <paramref name="data"/>
		/// object does not implement <see cref="IAnswerSurveyQuestion{T}"/> or returns
		/// a not applicable answer for the <see cref="QuestionType"/> associated with
		/// this implementation of <see cref="ISurveyQuestionTypeInfo"/></returns>
		int AskQuestion(object data, object contextElement);
		/// <summary>
		/// Maps the index of the answer to an image in the <see cref="ImageList"/> provided
		/// by the <see cref="ISurveyQuestionProvider.SurveyQuestionImageList"/> property.
		/// </summary>
		/// <param name="answer">A value from the enum type returned by the <see cref="QuestionType"/> property.</param>
		/// <returns>0-based index into the image list.</returns>
		int MapAnswerToImageIndex(int answer);
		/// <summary>
		/// Retrieves additional display information for a given answer. Used only if
		/// <see cref="UISupport"/> indicates support for <see cref="SurveyQuestionUISupport.DisplayData"/>
		/// </summary>
		/// <param name="answer">A value from the enum type returned by the <see cref="QuestionType"/> property.</param>
		/// <returns><see cref="SurveyQuestionDisplayData"/> settings</returns>
		SurveyQuestionDisplayData GetDisplayData(int answer);
		/// <summary>
		/// UISupport for Question
		/// </summary>
		SurveyQuestionUISupport UISupport { get;}
		/// <summary>
		/// Retrieve a priority to allow questions to be sorted relative
		/// to other questions. Lower priority sorts first in the list.
		/// </summary>
		int QuestionPriority { get;}
	}
	#endregion // ISurveyQuestionTypeInfo interface
	#region IAnswerSurveyQuestion<T> interface
	/// <summary>
	/// Implement this interface to answer a question by
	/// providing a value from an enum.
	/// </summary>
	/// <typeparam name="TAnswerEnum">an enum representing the potential answers to this question</typeparam>
	public interface IAnswerSurveyQuestion<TAnswerEnum>
		where TAnswerEnum : struct, IFormattable, IComparable
	{
		/// <summary>
		/// Called by survey tree to get the answer for an enum question
		/// </summary>
		/// <param name="contextElement">The context element for this instance.</param>
		/// <returns>int representing the answer to the enum question, or -1 for a 'not applicable' answer.</returns>
		int AskQuestion(object contextElement);
	}
	#endregion // IAnswerSurveyQuestion<T> interface
	#region IAnswerSurveyDynamicQuestion<T> interface
	/// <summary>
	/// Implement this interface to answer a question for
	/// a dynamic question.
	/// </summary>
	/// <typeparam name="TAnswerValues">an enum representing the potential answers to this question</typeparam>
	public interface IAnswerSurveyDynamicQuestion<TAnswerValues>
		where TAnswerValues : class, ISurveyDynamicValues
	{
		/// <summary>
		/// Called by survey tree to get the answer for a dynamic value
		/// </summary>
		/// <param name="answerValues">The set of dynamic answers.</param>
		/// <param name="contextElement">The context element for this instance.</param>
		/// <returns>int representing the answer to the enum question, or -1 for a 'not applicable' answer.</returns>
		int AskQuestion(TAnswerValues answerValues, object contextElement);
	}
	#endregion // IAnswerSurveyDynamicQuestion<T> interface
	#region ISurveyDynamicValues interface
	/// <summary>
	/// Represent a set of dynamic values to be used in place of
	/// a fixed <see cref="T:System.Enum"/>. The set of available
	/// values is more dynamic than an enum, but it is still fixed
	/// within the context of a single <see cref="Store"/> instance.
	/// </summary>
	public interface ISurveyDynamicValues
	{
		/// <summary>
		/// Get the total number of dynamic values supported
		/// </summary>
		int ValueCount { get;}
		/// <summary>
		/// Get the localized name of a value. This name will
		/// only be used if a dynamic question applied associated
		/// with this value set supports grouping.
		/// </summary>
		string GetValueName(int value);
	}
	#endregion // ISurveyDynamicValues interface
	#region ISurveyNode interface
	/// <summary>
	/// An optional interface to provide detailed information about nodes displayed in
	/// a survey tree. If an element implements both <see cref="ISurveyNode"/> and <see cref="ISurveyNodeReference"/>,
	/// then no <see cref="ISurveyNode"/> settings from the referenced element are called, so it is
	/// the developer's responsibility to forward requests as appropriate.
	/// </summary>
	public interface ISurveyNode
	{
		/// <summary>
		/// Set if the name is editable.
		/// </summary>
		bool IsSurveyNameEditable { get; }
		/// <summary>
		/// A custom display name for the element. If this returns <see langword="null"/> then
		/// the element's <see cref="M:Object.ToString"/> method is called.
		/// </summary>
		string SurveyName { get; }
		/// <summary>
		/// If <see cref="P:IsSurveyNameEditable"/> is true, then this property can be used to
		/// modify the edited name. This allows text decoration to be displayed with <see cref="P:SurveyName"/>
		/// but not display the name for editing.
		/// </summary>
		string EditableSurveyName { get; set; }
		/// <summary>
		/// Get a data object representing the survey node. Used for drag-drop operations.
		/// </summary>
		object SurveyNodeDataObject { get;}
		/// <summary>
		/// Return an object to use as the identifier for elements used
		/// as the expansion for this element. Elements returned as part
		/// of this expansion should implement <see cref="ISurveyNodeContext"/>
		/// to be able to find this element. If this element implements
		/// <see cref="ISurveyNodeReference"/> and returns an expansion key, then
		/// the combination of <see cref="P:ISurveyNodeReference.ReferencedSurveyNode"/> and
		/// <see cref="P:ISurveyNodeReference.SurveyNodeReferenceReason"/> must be unique
		/// across all elements. Normally, this combination is only required to be unique within
		/// a single context element.
		/// </summary>
		object SurveyNodeExpansionKey { get;}
	}
	#endregion // ISurveyNode interface
	#region SurveyNodeReferenceOptions enum
	/// <summary>
	/// Options for display of node references. Returned by <see cref="P:ISurveyNodeReference.SurveyNodeReferenceOptions"/>.
	/// </summary>
	[Flags]
	public enum SurveyNodeReferenceOptions
	{
		/// <summary>
		/// Use default link options (the node does not support expansion, a link decoration is shown, the selection is treated the same as the main element, target answers are used for all presentation elements)
		/// </summary>
		None = 0,
		/// <summary>
		/// The link should be presented to indicate that the primary information
		/// is presented elsewhere
		/// </summary>
		BlockLinkDisplay = 1,
		/// <summary>
		/// The referencing node should offer the same expansion as the primary element
		/// </summary>
		InlineExpansion = 2,
		/// <summary>
		/// Use the <see cref="ISurveyNodeReference"/> instance as the selected node
		/// </summary>
		SelectSelf = 4,
		/// <summary>
		/// Use the <see cref="ISurveyNodeReference.SurveyNodeReferenceReason"/> as the selected node
		/// </summary>
		SelectReferenceReason = 8,
		/// <summary>
		/// Apply the <see cref="ISurveyNodeReference.UseSurveyNodeReferenceAnswer"/> method if set
		/// </summary>
		FilterReferencedAnswers = 0x10,
	}
	#endregion // SurveyNodeReferenceOptions enum
	#region ISurveyNodeReference interface
	/// <summary>
	/// An element may only have a single primary location in the survey
	/// tree, but the developer can have as many references to that element
	/// as they like. A reference to an element has the same display information
	/// as the primary element except for the addition of a 'link' overlay. Nodes
	/// of this type may answer survey questions relating to the context node
	/// by implementing the <see cref="IAnswerSurveyQuestion{T}"/>,
	/// <see cref=" IAnswerSurveyDynamicQuestion{T}"/>, and <see cref="ICustomComparableSurveyNode"/>
	/// interfaces. If this element does not independently implement <see cref="ISurveyNode"/>, then the
	/// framework will automatically defer to the settings specified on the reference
	/// element.
	/// </summary>
	public interface ISurveyNodeReference
	{
		/// <summary>
		/// The primary node location referenced by this element
		/// </summary>
		object ReferencedSurveyNode { get;}
		/// <summary>
		/// The reason for referencing the primary element. This element
		/// must be set, but it may return the same value for all instances
		/// of an element that implements this interface. Only on SurveyNodeReference/SurveyNodeReferenceReason
		/// pairing is allowed per context element.
		/// </summary>
		object SurveyNodeReferenceReason { get;}
		/// <summary>
		/// Specify behavior options for the referenced node. These
		/// settings must be fixed over the life of the reference.
		/// </summary>
		SurveyNodeReferenceOptions SurveyNodeReferenceOptions { get;}
		/// <summary>
		/// Filters whether answers provided by the referenced node should be used in
		/// the presentation of the link node. This method is called only if the <see cref="F:SurveyNodeReferenceOptions.FilterReferencedAnswers"/>
		/// option is set.
		/// </summary>
		/// <param name="questionType">Specifies the question type</param>
		/// <param name="dynamicValues">The values for a dynamic question, otherwise <see langword="null"/></param>
		/// <param name="answer">The answer provided by the target</param>
		/// <returns><see langword="true"/> to use the answer</returns>
		bool UseSurveyNodeReferenceAnswer(Type questionType, ISurveyDynamicValues dynamicValues, int answer);
	}
	#endregion // ISurveyNodeReference interface
	#region ISurveyNodeContext interface
	/// <summary>
	/// Implement on elements that are displayed as expansions in
	/// the survey tree. Used to locate context elements when an
	/// item has not yet been expanded and needs to be located in the tree.
	/// </summary>
	public interface ISurveyNodeContext
	{
		/// <summary>
		/// Return the survey context object for this object.
		/// A context object will return this element from
		/// <see cref="ISurveyNodeProvider.GetSurveyNodes"/>.
		/// </summary>
		object SurveyNodeContext { get;}
	}
	#endregion // ISurveyNodeContext interface
	#region ICustomComparableSurveyNode interface
	/// <summary>
	/// Provide a custom comparison to do before the sort falls back
	/// on comparing the survey names of the objects. A class that implements
	/// the <see cref="ISurveyNode"/> interface can also implement ICustomComparableSurveyNode
	/// to support custom sorting. Any grouping semantics will already have been supplied before
	/// this comparison is called.
	/// </summary>
	public interface ICustomComparableSurveyNode
	{
		/// <summary>
		/// Determine the ordering for this <see cref="ISurveyNode"/> object
		/// as compared to another ISurveyNode.
		/// </summary>
		/// <param name="contextElement">The context container for the elements being compared.</param>
		/// <param name="other">The opposite object to compare to</param>
		/// <param name="customSortData">Data returned by the <see cref="ResetCustomSortData"/>
		/// method representing a snapshot of the data used to sort an element.</param>
		/// <param name="otherCustomSortData">Sort data returned by the other
		/// element's <see cref="ResetCustomSortData"/> method.</param>
		/// <returns>Normal comparison semantics apply, except that a 0
		/// return here applies no information, not equality. Survey nodes
		/// must have a fully deterministic order.</returns>
		int CompareToSurveyNode(object contextElement, object other, object customSortData, object otherCustomSortData);
		/// <summary>
		/// Custom sorting nodes requires data that cannot be represented by
		/// the survey categorization and string representations of the element.
		/// Generally, this data is based on the current node state. However,
		/// the old data is required to find an element in a custom sorted
		/// state at a time when the old custom sort information is no longer
		/// available. The data returned here (potentially null if the custom sort
		/// is based on state that does not change over time) is passed to the
		/// <see cref="CompareToSurveyNode"/> method, which can interpret the
		/// data to compare to elements.
		/// </summary>
		/// <param name="contextElement">The context container for this element.</param>
		/// <param name="customSortData">A reference to existing data. If this
		/// is not null, then the data should only be recreated if it has changed.</param>
		/// <returns><see langword="true"/> if the data has changed</returns>
		bool ResetCustomSortData(object contextElement, ref object customSortData);
	}
	#endregion // ICustomComparableSurveyNode interface
	#region ISurveyNodeProvider interface
	/// <summary>
	/// Interface for a <see cref="DomainModel"/> to provide a list of objects for the <see cref="SurveyTreeContainer"/>.
	/// </summary>
	public interface ISurveyNodeProvider
	{
		/// <summary>
		/// Retrieve survey elements for this <see cref="DomainModel"/>. A <paramref name="context"/>
		/// object and <paramref name="expansionKey"/> are provided to provide detail information for
		/// each type of object. Requesting expansion nodes from the original provider as opposed to
		/// the context node enables providers that are unknown to the context node to provide
		/// expansion information.
		/// </summary>
		/// <param name="context">The parent object. If the context is <see langword="null"/>, then
		/// the nodes are being requested for the top-level list.</param>
		/// <param name="expansionKey">The expansion key indicating the type of expansion
		/// data to retrieve for the provided context. Expansion keys are also used by
		/// <see cref="ISurveyNode.SurveyNodeExpansionKey"/> and <see cref="ISurveyQuestionProvider.GetSurveyQuestions"/></param>
		/// <returns><see cref="IEnumerable{Object}"/> for all nodes returned by the provider</returns>
		IEnumerable<object> GetSurveyNodes(object context, object expansionKey);
	}
	#endregion //ISurveyNodeProvider interface
	#region INotifySurveyElementChanged interface
	/// <summary>
	///defines behavior for a container in the survey tree to recieve events from it's contained elements
	/// </summary>
	public interface INotifySurveyElementChanged
	{
		/// <summary>
		/// Called when an element is added to the container's node provider
		/// </summary>
		/// <param name="element">The object that has been added. If the element implements
		/// the <see cref="ISurveyNodeReference"/> interface then this will be added
		/// as a reference to the primary element.</param>
		/// <param name="contextElement">If the object is added as part of a detail expansion, the context element
		/// is the element's parent object.</param>
		void ElementAdded(object element, object contextElement);
		/// <summary>
		/// Called if the answers provided by a node have been changed.
		/// </summary>
		/// <param name="element">the object that has been changed</param>
		/// <param name="questionTypes">The question types.</param>
		void ElementChanged(object element, params Type[] questionTypes);
		/// <summary>
		/// Called if the answers provided by a node reference have been changed.
		/// </summary>
		/// <param name="element">the object that has been changed</param>
		/// <param name="referenceReason">The reference reason. Corresponds to the
		/// reason provided by <see cref="P:ISurveyNodeReference.SurveyNodeReferenceReason"/></param>
		/// <param name="contextElement">The context container of the referenced element.</param>
		/// <param name="questionTypes">The question types.</param>
		void ElementReferenceChanged(object element, object referenceReason, object contextElement, params Type[] questionTypes);
		/// <summary>
		/// Called if element is removed from the container's node provider
		/// </summary>
		/// <param name="element">The object that was removed from the node provider</param>
		void ElementDeleted(object element);
		/// <summary>
		/// Called if element is removed from the container's node provider
		/// </summary>
		/// <param name="element">The object that was removed from the node provider</param>
		/// <param name="referenceReason">The reference reason. Corresponds to the
		/// reason provided by <see cref="P:ISurveyNodeReference.SurveyNodeReferenceReason"/></param>
		/// <param name="contextElement">The context container of the referenced element.</param>
		void ElementReferenceDeleted(object element, object referenceReason, object contextElement);
		/// <summary>
		/// Called if an element in the container's node provider has been renamed
		/// </summary>
		/// <param name="element">The object that has been renamed</param>
		void ElementRenamed(object element);
		/// <summary>
		/// Called if an element reference has been renamed. This is needed only if the
		/// <see cref="ISurveyNodeReference"/> instance also implements <see cref="ISurveyNode"/>
		/// and provides a custom name.
		/// </summary>
		/// <param name="element">The object that has been renamed</param>
		/// <param name="referenceReason">The reference reason. Corresponds to the
		/// reason provided by <see cref="P:ISurveyNodeReference.SurveyNodeReferenceReason"/></param>
		/// <param name="contextElement">The context container of the referenced element.</param>
		void ElementReferenceRenamed(object element, object referenceReason, object contextElement);
		/// <summary>
		/// Called if the custom sort data has changed for an element in the container's node provider
		/// </summary>
		/// <param name="element">The object that has been modified</param>
		void ElementCustomSortChanged(object element);
		/// <summary>
		/// Called if the custom sort data has changed for an element in the container's node provider
		/// </summary>
		/// <param name="element">The object that has been modified</param>
		/// <param name="referenceReason">The reference reason. Corresponds to the
		/// reason provided by <see cref="P:ISurveyNodeReference.SurveyNodeReferenceReason"/></param>
		/// <param name="contextElement">The context container of the referenced element.</param>
		void ElementReferenceCustomSortChanged(object element, object referenceReason, object contextElement);
	}
	#endregion // INotifySurveyElementChanged interface
}
