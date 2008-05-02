using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.ORMAbstraction;
using Neumont.Tools.ORMToORMAbstractionBridge;
using Neumont.Tools.RelationalModels.ConceptualDatabase;
using ORMUniquenessConstraint = Neumont.Tools.ORM.ObjectModel.UniquenessConstraint;
using Neumont.Tools.Modeling;

namespace Neumont.Tools.ORMAbstractionToConceptualDatabaseBridge
{
	#region IDatabaseNameGenerator interface
	/// <summary>
	/// Generate relational names for elements contained in a <see cref="Schema"/>
	/// </summary>
	public interface IDatabaseNameGenerator
	{
		/// <summary>
		/// Generate a name for the provided <paramref name="table"/>
		/// </summary>
		/// <param name="table">A <see cref="Table"/> element</param>
		/// <param name="phase">The current phase of the name to generate. As the phase number goes
		/// higher the returned name should be more complex. The initial request will be 0, with additional
		/// requested incremented 1 from the previous name request.</param>
		/// <returns>A name for <paramref name="table"/>.</returns>
		string GenerateTableName(Table table, int phase);
		/// <summary>
		/// Generate a name for the provided <paramref name="column"/>
		/// </summary>
		/// <param name="column">A <see cref="Column"/> element</param>
		/// <param name="phase">The current phase of the name to generate. As the phase number goes
		/// higher the returned name should be more complex. The initial request will be 0, with additional
		/// requested incremented 1 from the previous name request.</param>
		/// <returns>A name for <paramref name="column"/>.</returns>
		string GenerateColumnName(Column column, int phase);
		/// <summary>
		/// Generate a name for the provided <paramref name="constraint"/>
		/// </summary>
		/// <param name="constraint">A <see cref="Constraint"/> element</param>
		/// <param name="phase">The current phase of the name to generate. As the phase number goes
		/// higher the returned name should be more complex. The initial request will be 0, with additional
		/// requested incremented 1 from the previous name request.</param>
		/// <returns>A name for <paramref name="constraint"/>.</returns>
		string GenerateConstraintName(Constraint constraint, int phase);
	}
	#endregion // IDatabaseNameGenerator interface
	#region NamePart struct
	/// <summary>
	/// Options used with the <see cref="NamePart"/> structure
	/// </summary>
	[Flags]
	public enum NamePartOptions
	{
		/// <summary>
		/// No special options
		/// </summary>
		None = 0,
		/// <summary>
		/// The element should not be cased
		/// </summary>
		ExplicitCasing = 1,
		/// <summary>
		/// Stop a name part that was added as a single-word expansion
		/// </summary>
		ReplacementOfSelf = 2,
	}
	/// <summary>
	/// A callback delegate for adding a <see cref="NamePart"/>
	/// </summary>
	/// <param name="part">The <see cref="NamePart"/> to add</param>
	/// <param name="insertIndex">The index to insert the name part at.</param>
	public delegate void AddNamePart(NamePart part, int? insertIndex);
	/// <summary>
	/// Represent a single string with options
	/// </summary>
	public struct NamePart
	{
		private string myString;
		private NamePartOptions myOptions;
		/// <summary>
		/// Create a new NamePart with default options
		/// </summary>
		/// <param name="value">The string value for this <see cref="NamePart"/></param>
		public NamePart(string value)
		{
			myString = value;
			myOptions = NamePartOptions.None;
		}
		/// <summary>
		/// Create a new NamePart with explicit options
		/// </summary>
		/// <param name="value">The string value for this <see cref="NamePart"/></param>
		/// <param name="options">Values from <see cref="NamePartOptions"/></param>
		public NamePart(string value, NamePartOptions options)
		{
			myString = value;
			myOptions = options;
		}
		/// <summary>
		/// Is the structure populated?
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(myString);
			}
		}
		/// <summary>
		/// Return the <see cref="NamePartOptions"/> passed to the constructor
		/// </summary>
		public NamePartOptions Options
		{
			get
			{
				return myOptions;
			}
		}
		/// <summary>
		/// If <see langword="true"/>, then the casing of the string should not be changed
		/// </summary>
		public bool ExplicitCasing
		{
			get
			{
				return 0 != (myOptions & NamePartOptions.ExplicitCasing);
			}
		}
		/// <summary>
		/// Implicitly cast the <see cref="NamePart"/> to its string value
		/// </summary>
		public static implicit operator string(NamePart part)
		{
			return part.myString;
		}
		/// <summary>
		/// Implicitly cast the <see cref="NamePart"/> to its string value
		/// </summary>
		public static implicit operator NamePart(string value)
		{
			return new NamePart(value);
		}
	}
	#endregion // NamePart struct
	#region ORMAbstractionToConceptualDatabaseBridgeDomainModel.NameGeneration class
	partial class ORMAbstractionToConceptualDatabaseBridgeDomainModel
	{
		private static class NameGeneration
		{
			#region GenerateAllNames method
			/// <summary>
			/// Generate names for the provided <see cref="Schema"/>
			/// </summary>
			public static void GenerateAllNames(Schema schema)
			{
				IDatabaseNameGenerator nameGenerator = new DefaultDatabaseNameGenerator(schema.Store);
				UniqueNameGenerator uniqueChecker = new UniqueNameGenerator();

				LinkedElementCollection<Table> tables = schema.TableCollection;

				// Generate table names
				uniqueChecker.GenerateUniqueElementNames(
					tables,
					delegate(object element, int phase)
					{
						return nameGenerator.GenerateTableName((Table)element, phase);
					},
					delegate(object element, string elementName)
					{
						((Table)element).Name = elementName;
					});

				foreach (Table table in tables)
				{
					//column names
					uniqueChecker.GenerateUniqueElementNames(
						table.ColumnCollection,
						delegate(object element, int phase)
						{
							return nameGenerator.GenerateColumnName((Column)element, phase);
						},
						delegate(object element, string elementName)
						{
							((Column)element).Name = elementName;
						});

					//constraint names
					LinkedElementCollection<ReferenceConstraint> constraints;
					if (0 != (constraints = table.ReferenceConstraintCollection).Count)
					{
						uniqueChecker.GenerateUniqueElementNames(
							constraints,
							delegate(object element, int phase)
							{
								return nameGenerator.GenerateConstraintName((Constraint)element, phase);
							},
							delegate(object element, string elementName)
							{
								((Constraint)element).Name = elementName;
							});
					}
				}
			}
			#endregion // GenerateAllNames method
			#region LinkedNode class
			/// <summary>
			/// A linked list node class. LinkedList{} is too hard to modify during iteration,
			/// and a LinkedListNode{} requires a LinkedList, so we rolled our own.
			/// </summary>
			private class LinkedNode<T>
			{
				private T myValue;
				private LinkedNode<T> myNext;
				private LinkedNode<T> myPrev;
				public LinkedNode(T value)
				{
					myValue = value;
				}
				/// <summary>
				/// Set the next element
				/// </summary>
				/// <param name="next">Next element. If next has a previous element, then the head of the next element is inserted.</param>
				/// <param name="head">Reference to head node</param>
				public void SetNext(LinkedNode<T> next, ref LinkedNode<T> head)
				{
					Debug.Assert(next != null);
					if (next.myPrev != null)
					{
						next.myPrev.SetNext(GetHead(), ref head);
						return;
					}
					if (myNext != null)
					{
						myNext.myPrev = next.GetTail();
					}
					if (myPrev == null)
					{
						head = this;
					}
					myNext = next;
					next.myPrev = this;
				}
				/// <summary>
				/// The value passed to the constructor or set directly
				/// </summary>
				public T Value
				{
					get
					{
						return myValue;
					}
					set
					{
						myValue = value;
					}
				}
				/// <summary>
				/// Get the next node
				/// </summary>
				public LinkedNode<T> Next
				{
					get
					{
						return myNext;
					}
				}
				/// <summary>
				/// Get the previous node
				/// </summary>
				public LinkedNode<T> Previous
				{
					get
					{
						return myPrev;
					}
				}
				/// <summary>
				/// Get the head element in the linked list
				/// </summary>
				public LinkedNode<T> GetHead()
				{
					LinkedNode<T> retVal = this;
					LinkedNode<T> prev;
					while (null != (prev = retVal.myPrev))
					{
						retVal = prev;
					}
					return retVal;
				}
				/// <summary>
				/// Get the tail element in the linked list
				/// </summary>
				public LinkedNode<T> GetTail()
				{
					LinkedNode<T> retVal = this;
					LinkedNode<T> next;
					while (null != (next = retVal.myNext))
					{
						retVal = next;
					}
					return retVal;
				}
				/// <summary>
				/// Detach the current node
				/// </summary>
				/// <param name="headNode"></param>
				public void Detach(ref LinkedNode<T> headNode)
				{
					if (myPrev == null)
					{
						headNode = myNext;
					}
					else
					{
						myPrev.myNext = myNext;
					}
					if (myNext != null)
					{
						myNext.myPrev = myPrev;
					}
					myNext = null;
					myPrev = null;
				}
			}
			#endregion // LinkedNode class
			#region Unique name generation algorithm
			/// <summary>
			/// Generate a candidate name for the given <paramref name="element"/>
			/// </summary>
			/// <param name="element">The element to generate a candidate name for</param>
			/// <param name="phase">The current phase of the name to generate. As the phase number goes
			/// higher the returned name should be more complex. The initial request will be 0, with additional
			/// requested incremented 1 from the previous name request.</param>
			/// <returns>The candidate name, or <see langword="null"/> if a name is not available for the specified phase.</returns>
			private delegate string GenerateCandidateElementNameCallback(object element, int phase);
			/// <summary>
			/// Set the name for the given element. Used by GenerateUniqueElementNames
			/// </summary>
			private delegate void SetElementNameCallback(object element, string elementName);
			private struct UniqueNameGenerator
			{
				#region ElementPhase structure
				/// <summary>
				/// A structure to hold an element coupled with a phase number.
				/// Used to determine the phase that was used to generate a name.
				/// </summary>
				private struct ElementPhase
				{
					private readonly object myElement;
					private readonly int myPhase;
					/// <summary>
					/// Create a new <see cref="ElementPhase"/>
					/// </summary>
					/// <param name="element">The element involved</param>
					/// <param name="phase">The phase the name was generated with</param>
					public ElementPhase(object element, int phase)
					{
						myElement = element;
						myPhase = phase;
					}
					/// <summary>
					/// The element passed to the constructor
					/// </summary>
					public object Element
					{
						get
						{
							return myElement;
						}
					}
					/// <summary>
					/// The phase passed to the constructor
					/// </summary>
					public int Phase
					{
						get
						{
							return myPhase;
						}
					}
				}
				#endregion // ElementPhase structure
				#region Fields
				/// <summary>
				/// Map already generated names into a dictionary that contains either one of the element
				/// objects or a linked list of objects. Linked lists contain duplicate nodes
				/// </summary>
				private Dictionary<string, object> myNameMappingDictionary;
				/// <summary>
				/// A dictionary of unresolved names, corresponds to keys in the nameMappingDictionary
				/// </summary>
				Dictionary<string, string> myUnresolvedNames;
				#endregion // Fields
				#region Public methods
				public void GenerateUniqueElementNames(IEnumerable elements, GenerateCandidateElementNameCallback generateName, SetElementNameCallback setName)
				{
					if (myNameMappingDictionary != null)
					{
						myNameMappingDictionary.Clear();
					}
					else
					{
						myNameMappingDictionary = new Dictionary<string, object>();
					}
					if (myUnresolvedNames != null)
					{
						myUnresolvedNames.Clear();
					}
					// Generate initial names
					foreach (object element in elements)
					{
						string elementName = generateName(element, 0);
						if (elementName != null)
						{
							AddElement(element, elementName, 0);
						}
					}

					Dictionary<string, object> nameMappingDictionary = myNameMappingDictionary;
					while (myUnresolvedNames != null && 0 != myUnresolvedNames.Count)
					{
						// Walk the existing unresolved names and attempt to resolve them further.
						// Iterate until we can't resolve any more
						Dictionary<string, string> unresolvedNames = myUnresolvedNames;
						myUnresolvedNames = null;

						foreach (string currentName in unresolvedNames.Values)
						{
							// If we've added this name as unresolved during this pass, then take it back out
							// We'll pick it up again if it doesn't resolve
							if (myUnresolvedNames != null && myUnresolvedNames.ContainsKey(currentName))
							{
								myUnresolvedNames.Remove(currentName);
							}
							LinkedNode<ElementPhase> startHeadNode = (LinkedNode<ElementPhase>)nameMappingDictionary[currentName];
							LinkedNode<ElementPhase> headNode = startHeadNode;
							LinkedNode<ElementPhase> nextNode = headNode;
							while (nextNode != null)
							{
								LinkedNode<ElementPhase> currentNode = nextNode;
								nextNode = currentNode.Next;

								ElementPhase elementPhase = currentNode.Value;
								object element = elementPhase.Element;
								// The next phase to request is based on the last phase requested for this element,
								// not the number of times we've passed through the loop
								int phase = elementPhase.Phase + 1;
								string newName = generateName(element, phase);
								// Name generation can return null if the phase is not supported be satisfied
								if (newName != null)
								{
									if (0 == string.CompareOrdinal(newName, currentName))
									{
										currentNode.Value = new ElementPhase(element, phase);
									}
									else
									{
										currentNode.Detach(ref headNode);
										AddElement(element, newName, phase);
									}
								}
							}

							// Manage the remains of the list in the dictionary
							if (headNode == null)
							{
								// Everything detached from this name, remove the key
								nameMappingDictionary.Remove(currentName);
							}
							else if (headNode != startHeadNode)
							{
								if (headNode.Next == null)
								{
									nameMappingDictionary[currentName] = headNode.Value;
								}
								else
								{
									nameMappingDictionary[currentName] = headNode;
									Dictionary<string, string> currentUnresolvedNames = myUnresolvedNames;
									if (currentUnresolvedNames == null)
									{
										myUnresolvedNames = currentUnresolvedNames = new Dictionary<string, string>();
									}
									currentUnresolvedNames[currentName] = currentName;
								}
							}
						}
					}

					// Walk the set, appending additional numbers as needed, and set the names
					foreach (KeyValuePair<string, object> pair in nameMappingDictionary)
					{
						object element = pair.Value;
						LinkedNode<ElementPhase> node = element as LinkedNode<ElementPhase>;
						if (node != null)
						{
							// We added these in reverse order, so walk backwards to number them
							LinkedNode<ElementPhase> tail = node.GetTail();
							if (node == tail)
							{
								setName(node.Value.Element, pair.Key);
							}
							else
							{
								// We need to resolve farther
								string baseName = pair.Key;
								int currentIndex = 0;
								LinkedNode<ElementPhase> nextNode = tail;
								while (nextNode != null)
								{
									element = nextNode.Value.Element;
									nextNode = nextNode.Previous; // We started at the tail, walk backwards

									string candidateName;
									do
									{
										++currentIndex;
										candidateName = baseName + currentIndex.ToString();
									} while (nameMappingDictionary.ContainsKey(candidateName));

									// If we get out of the loop, then we finally have a unique name
									setName(element, candidateName);
								}
							}
						}
						else
						{
							setName(((ElementPhase)element).Element, pair.Key);
						}
					}
				}
				#endregion // Public methods
				#region Helper methods
				private void AddElement(object element, string elementName, int phase)
				{
					object existing;
					Dictionary<string, object> nameMappingDictionary = myNameMappingDictionary;
					if (nameMappingDictionary.TryGetValue(elementName, out existing))
					{
						// Note: We use LinkedListNode here directly instead of a LinkedList
						// to facilitate dynamically adding/removing elements during iteration
						LinkedNode<ElementPhase> node = existing as LinkedNode<ElementPhase>;
						if (node == null)
						{
							// Record the unresolvedName
							if (myUnresolvedNames == null)
							{
								myUnresolvedNames = new Dictionary<string, string>();
							}
							myUnresolvedNames[elementName] = elementName;

							// Create a node for the original element
							node = new LinkedNode<ElementPhase>((ElementPhase)existing);
						}

						LinkedNode<ElementPhase> newNode = new LinkedNode<ElementPhase>(new ElementPhase(element, phase));
						newNode.SetNext(node, ref node);
						nameMappingDictionary[elementName] = newNode;
					}
					else
					{
						nameMappingDictionary[elementName] = new ElementPhase(element, phase);
					}
				}
				#endregion // Helper methods
			}
			#endregion // Unique name generation algorithm
			#region DefaultDatabaseNameGenerator class
			private class DefaultDatabaseNameGenerator : IDatabaseNameGenerator
			{
				#region Member variables
				private NameGenerator myTableGenerator;
				private NameGenerator myColumnGenerator;
				private ORMModel myORMModel;
				private Store myStore;
				#endregion // Member variables
				#region Constructor
				/// <summary>
				/// Create a new <see cref="DefaultDatabaseNameGenerator"/>
				/// </summary>
				/// <param name="store">The context <see cref="Store"/></param>
				public DefaultDatabaseNameGenerator(Store store)
				{
					myStore = store;
				}
				#endregion // Constructor
				#region IDatabaseNameGenerator Members
				string IDatabaseNameGenerator.GenerateTableName(Table table, int phase)
				{
					return GenerateTableName(table, phase);
				}
				string IDatabaseNameGenerator.GenerateColumnName(Column column, int phase)
				{
					return GenerateColumnName(column, phase);
				}
				string IDatabaseNameGenerator.GenerateConstraintName(Constraint constraint, int phase)
				{
					return GenerateConstraintName(constraint, phase);
				}
				#endregion
				#region Accessor properties
				private NameGenerator ColumnNameGenerator
				{
					get
					{
						NameGenerator retVal = myColumnGenerator;
						if (retVal == null)
						{
							myColumnGenerator = retVal = NameGenerator.GetGenerator(myStore, typeof(RelationalNameGenerator), typeof(ColumnNameUsage));
						}
						return retVal;
					}
				}
				private NameGenerator TableNameGenerator
				{
					get
					{
						NameGenerator retVal = myTableGenerator;
						if (retVal == null)
						{
							myTableGenerator = retVal = NameGenerator.GetGenerator(myStore, typeof(RelationalNameGenerator), typeof(TableNameUsage));
						}
						return retVal;
					}
				}
				private ORMModel ContextModel
				{
					get
					{
						ORMModel retVal = myORMModel;
						if (retVal == null)
						{
							foreach (ORMModel model in myStore.ElementDirectory.FindElements<ORMModel>(false))
							{
								myORMModel = retVal = model;
								break;
							}
						}
						return retVal;
					}
				}
				private static string GetSpacingReplacement(NameGenerator generator)
				{
					string retVal;
					switch (generator.SpacingFormat)
					{
						case NameGeneratorSpacingFormat.ReplaceWith:
							retVal = generator.SpacingReplacement;
							break;
						case NameGeneratorSpacingFormat.Retain:
							retVal = " ";
							break;
						// case NameGeneratorSpacingFormat.Remove:
						default:
							retVal = "";
							break;
					}
					return retVal;
				}
				#endregion // Accessor properties
				#region Regex patterns
				private static Regex myReplaceFieldsPattern;
				private Regex ReplaceFieldsPattern
				{
					get
					{
						Regex retVal = myReplaceFieldsPattern;
						if (retVal == null)
						{
							System.Threading.Interlocked.CompareExchange<Regex>(
								ref myReplaceFieldsPattern,
								new Regex(
									@"{\d+}",
									RegexOptions.Compiled),
								null);
							retVal = myReplaceFieldsPattern;
						}
						return retVal;
					}
				}
				#endregion // Regex patterns
				#region Name generation methods
				private string GenerateTableName(Table table, int phase)
				{
					if (phase != 0)
					{
						return null;
					}
					NamePart singleName = default(NamePart);
					List<NamePart> nameCollection = null;
					NameGenerator nameGenerator = TableNameGenerator;

					ReferenceModeNaming.SeparateObjectTypeParts(
						ConceptTypeIsForObjectType.GetObjectType(TableIsPrimarilyForConceptType.GetConceptType(table)),
						nameGenerator,
						delegate(NamePart newPart, int? insertIndex)
						{
							AddToNameCollection(ref singleName, ref nameCollection, newPart, insertIndex.HasValue ? insertIndex.Value : -1);
						});

					string finalName = GetFinalName(singleName, nameCollection, nameGenerator);
					return string.IsNullOrEmpty(finalName) ? "TABLE" : finalName;
				}
				private string GenerateColumnName(Column column, int phase)
				{
					if (phase > 1)
					{
						return null;
					}
					bool decorateWithPredicateText = phase == 1;
					LinkedNode<ColumnPathStep> currentNode = GetColumnPath(column);

					NameGenerator generator = ColumnNameGenerator;

					if (currentNode == null)
					{
						return GetFinalName(ResourceStrings.NameGenerationValueTypeValueColumn, null, generator);
					}

					// Prepare for adding name parts. The single NamePart string is used when
					// possible to avoid using the list for a single entry
					NamePart singleName = default(NamePart);
					List<NamePart> nameCollection = null;
					AddNamePart addPart = delegate(NamePart newPart, int? insertIndex)
						{
							AddToNameCollection(ref singleName, ref nameCollection, newPart, insertIndex.HasValue ? insertIndex.Value : -1);
						};
					ObjectType previousObjectType = null;
					ConceptType primaryConceptType = TableIsPrimarilyForConceptType.GetConceptType(column.Table);
					if (primaryConceptType != null)
					{
						previousObjectType = ConceptTypeIsForObjectType.GetObjectType(primaryConceptType);
					}
					ObjectType previousPreviousObjectType = null;
					bool firstPass = true;
					bool treatNextIdentifierAsFirstStep = false;
					bool lastStepConsumedNextNode = false;
					bool lastStepUsedExplicitRoleName = false;
					do
					{
						LinkedNode<ColumnPathStep> nextLoopNode = currentNode.Next;
						ColumnPathStep step = currentNode.Value;
						ColumnPathStepFlags stepFlags = step.Flags;
						if (treatNextIdentifierAsFirstStep)
						{
							treatNextIdentifierAsFirstStep = false;
							if (0 != (stepFlags & ColumnPathStepFlags.IsIdentifier))
							{
								firstPass = true;
							}
						}
						if (0 != (stepFlags & ColumnPathStepFlags.ForwardSubtype))
						{
							addPart(step.ObjectType.GetAbbreviatedName(generator, true), null);
							lastStepConsumedNextNode = false;
							lastStepUsedExplicitRoleName = false;
						}
						else if (0 != (stepFlags & ColumnPathStepFlags.ReverseSubtype))
						{
							// Don't add names for reverse path types
							lastStepConsumedNextNode = false;
							lastStepUsedExplicitRoleName = false;
							treatNextIdentifierAsFirstStep = true;
						}
						else
						{
							bool decorate = 0 != (stepFlags & ColumnPathStepFlags.RequiresDecoration);
							if (decorate || (nextLoopNode == null && (0 == (stepFlags & ColumnPathStepFlags.IsIdentifier) || !(lastStepConsumedNextNode || lastStepUsedExplicitRoleName))))
							{
								LinkedNode<ColumnPathStep> nextNode = nextLoopNode;
								if (!decorate &&
									nextNode == null &&
									0 != (stepFlags & ColumnPathStepFlags.IsIdentifier))
								{
									LinkedNode<ColumnPathStep> previousNode = currentNode.Previous;
									if (previousNode != null)
									{
										ColumnPathStep previousStep = previousNode.Value;
										ColumnPathStepFlags previousFlags = previousStep.Flags;
										if (0 == (previousFlags & (ColumnPathStepFlags.RequiresDecoration | ColumnPathStepFlags.ForwardSubtype | ColumnPathStepFlags.ReverseSubtype)) &&
											previousObjectType.ReferenceModePattern != null)
										{
											// Special condition when the last node is a simple identifier:
											// Back up one step to enable picking up the role or hyphen-bound
											// name instead of just the predicate text.
											stepFlags = previousFlags;
											step = previousStep;
											nextNode = currentNode;
											previousObjectType = previousPreviousObjectType;
										}
									}
								}
								Role nearRole = step.FromRole;
								Role farRole = nearRole.OppositeRoleAlwaysResolveProxy.Role;
								string explicitFarRoleName = farRole.Name;
								FactType factType = nearRole.FactType;
								LinkedElementCollection<RoleBase> factTypeRoles = factType.RoleCollection;
								int? unaryRoleIndex = FactType.GetUnaryRoleIndex(factTypeRoles);
								bool isUnary = unaryRoleIndex.HasValue;
								LinkedElementCollection<ReadingOrder> readingOrders = null;
								IReading reading = null;
								if (decorate && decorateWithPredicateText)
								{
									readingOrders = factType.ReadingOrderCollection;
									reading = factType.GetMatchingReading(readingOrders, null, nearRole, null, false, true, factTypeRoles, isUnary);
								}
								lastStepConsumedNextNode = false;
								lastStepUsedExplicitRoleName = false;
								if (reading != null && !reading.IsDefault)
								{
									string readingText = string.Format(CultureInfo.CurrentCulture, reading.Text, isUnary ? "\x1" : "", "\x1");
									int splitPosition = readingText.LastIndexOf('\x1');
									addPart(readingText.Substring(0, splitPosition).Replace("- ", " ").Replace(" -", " "), null);
									if (!string.IsNullOrEmpty(explicitFarRoleName))
									{
										addPart(explicitFarRoleName, null);
									}
									else if (nextNode != null)
									{
										lastStepConsumedNextNode = ReferenceModeNaming.ResolveObjectTypeName(
											step.ObjectType,
											nextNode.Value.ObjectType,
											true,
											ReferenceModeNamingUse.ReferenceToEntityType,
											generator,
											addPart);
									}
									else
									{
										ReferenceModeNaming.ResolveObjectTypeName(
											(!firstPass || 0 != (stepFlags & ColumnPathStepFlags.IsIdentifier)) ? previousObjectType : null,
											step.ObjectType,
											false,
											firstPass ? ReferenceModeNamingUse.PrimaryIdentifier : ReferenceModeNamingUse.ReferenceToEntityType, // Ignored if first parameter is null
											generator,
											addPart);
									}
									if ((readingText.Length - splitPosition) > 1)
									{
										addPart(readingText.Substring(splitPosition + 1).Replace("- ", " ").Replace(" -", " "), null);
									}
								}
								else if (!string.IsNullOrEmpty(explicitFarRoleName))
								{
									addPart(explicitFarRoleName, null);
									lastStepUsedExplicitRoleName = true;
								}
								else
								{
									// Find an appropriate hyphen bind in the available readings
									string hyphenBoundFormatString = null;
									if (readingOrders == null)
									{
										readingOrders = factType.ReadingOrderCollection;
									}
									ReadingOrder readingOrder;
									if (0 != (readingOrders ?? (readingOrders = factType.ReadingOrderCollection)).Count &&
										null != (readingOrder = isUnary ? readingOrders[0] : FactType.FindMatchingReadingOrder(readingOrders, new RoleBase[] { nearRole, farRole })))
									{
										foreach (Reading testReading in readingOrder.ReadingCollection)
										{
											hyphenBoundFormatString = VerbalizationHyphenBinder.GetFormatStringForHyphenBoundRole(testReading, farRole, unaryRoleIndex);
											if (hyphenBoundFormatString != null)
											{
												break;
											}
										}
									}
									int splitPosition = 0;
									if (hyphenBoundFormatString != null)
									{
										hyphenBoundFormatString = string.Format(CultureInfo.CurrentCulture, hyphenBoundFormatString, "\x1");
										splitPosition = hyphenBoundFormatString.IndexOf('\x1');
										if (splitPosition != 0)
										{
											addPart(hyphenBoundFormatString.Substring(0, splitPosition), null);
										}
									}
									if (nextNode != null)
									{
										lastStepConsumedNextNode = ReferenceModeNaming.ResolveObjectTypeName(
											step.ObjectType,
											nextNode.Value.ObjectType,
											true,
											ReferenceModeNamingUse.ReferenceToEntityType,
											generator,
											addPart);
									}
									else
									{
										ReferenceModeNaming.ResolveObjectTypeName(
											(!firstPass || 0 != (stepFlags & ColumnPathStepFlags.IsIdentifier)) ? previousObjectType : null,
											step.ObjectType,
											false,
											firstPass ? ReferenceModeNamingUse.PrimaryIdentifier : ReferenceModeNamingUse.ReferenceToEntityType, // Ignored if first parameter is null
											generator,
											addPart);
									}
									if (hyphenBoundFormatString != null &&
										(hyphenBoundFormatString.Length - splitPosition) > 1)
									{
										addPart(hyphenBoundFormatString.Substring(splitPosition + 1), null);
									}
								}
							}
							else
							{
								lastStepConsumedNextNode = false;
								lastStepUsedExplicitRoleName = false;
							}
						}
						previousPreviousObjectType = previousObjectType;
						previousObjectType = step.ResolvedSupertype;
						currentNode = nextLoopNode;
						firstPass = false;
					} while (currentNode != null);
					string finalName = GetFinalName(singleName, nameCollection, generator);
					if (string.IsNullOrEmpty(finalName))
					{
						return (phase == 0) ? "COLUMN" : null;
					}
					return finalName;
				}
				private string GenerateConstraintName(Constraint constraint, int phase)
				{
					if (phase != 0)
					{
						return null;
					}
					ReferenceConstraint refConstraint = constraint as ReferenceConstraint;
					if (null != refConstraint)
					{
						return refConstraint.SourceTable.Name + "_FK";
					}
					return constraint.Name;
				}
				#endregion // Name generation methods
				#region NameCollection helpers
				private void AddToNameCollection(ref NamePart singleName, ref List<NamePart> nameCollection, string newName)
				{
					AddToNameCollection(ref singleName, ref  nameCollection, new NamePart(newName), -1);
				}
				private void AddToNameCollection(ref NamePart singleName, ref List<NamePart> nameCollection, string newName, int index)
				{
					AddToNameCollection(ref singleName, ref nameCollection, new NamePart(newName), index);
				}
				private void AddToNameCollection(ref NamePart singleName, ref List<NamePart> nameCollection, NamePart newNamePart, int index)
				{
					string newName = newNamePart;
					newName = newName.Trim();
					NamePartOptions options = newNamePart.Options;
					Debug.Assert(newName != null);
					if (newName.Contains(" "))
					{
						string[] individualEntries = newName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < individualEntries.Length; ++i)
						{
							//add each space separated name individually
							AddToNameCollection(ref singleName, ref nameCollection, individualEntries[i], index == -1 ? -1 : index + i);
						}
						return;
					}

					if (singleName.IsEmpty)
					{
						//we only have one name so far, so just use the string
						singleName = new NamePart(newName, options);
					}
					else
					{
						//we need to now use the collection
						if (null == nameCollection)
						{
							nameCollection = new List<NamePart>();
							//first add to the actual collection the element that had previosly been added
							nameCollection.Add(singleName);
						}
						int count;
						if (index == -1)
						{
							index = nameCollection.Count;
							count = index + 1;
							nameCollection.Add(new NamePart(newName, options));
						}
						else
						{
							nameCollection.Insert(index, new NamePart(newName, options));
							count = nameCollection.Count;
						}
						//remove duplicate information
						int nextIndex;
						if ((index > 0 && ((string)nameCollection[index - 1]).EndsWith(newName, StringComparison.CurrentCultureIgnoreCase))
							|| ((nextIndex = index + 1) < count && ((string)nameCollection[nextIndex]).StartsWith(newName, StringComparison.CurrentCultureIgnoreCase)))
						{
							//we don't need the name that was just added
							// UNDONE: Possiblye kill this check? Name scrubbing should be handled by the current algorithm
							nameCollection.RemoveAt(index);
						}
						else
						{
							//check if we need the following name
							while (nextIndex < count)
							{
								if (newName.EndsWith(nameCollection[nextIndex], StringComparison.CurrentCultureIgnoreCase))
								{
									nameCollection.RemoveAt(nextIndex);
									--count;
								}
								else
								{
									break;
								}
							}
							//check the preceding name
							nextIndex = index - 1;
							while (nextIndex > -1)
							{
								if (newName.StartsWith(nameCollection[nextIndex], StringComparison.CurrentCultureIgnoreCase))
								{
									nameCollection.RemoveAt(nextIndex--);
								}
								else
								{
									break;
								}
							}
						}
					}
				}
				private string GetFinalName(NamePart singleName, List<NamePart> nameCollection, NameGenerator generator)
				{
					ResolveRecognizedPhrases(ref singleName, ref nameCollection, generator);
					NameGeneratorCasingOption casing = generator.CasingOption;
					string space = GetSpacingReplacement(generator);
					string finalName;
					if (null == nameCollection)
					{
						if (singleName.IsEmpty)
						{
							return "";
						}
						if (casing == NameGeneratorCasingOption.None)
						{
							finalName = singleName;
						}
						else
						{
							finalName = DoFirstWordCasing(singleName, casing, CultureInfo.CurrentCulture.TextInfo);
						}
					}
					else
					{
						TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

						string name;
						if (casing == NameGeneratorCasingOption.None)
						{
							name = nameCollection[0];
						}
						else
						{
							name = DoFirstWordCasing(nameCollection[0], casing, textInfo);
						}

						//we already know there are at least two name entries, so use a string builder
						StringBuilder builder = new StringBuilder(name);

						//we already have the first entry, so mark camel as pascal
						NameGeneratorCasingOption tempCasing = casing;
						if (tempCasing == NameGeneratorCasingOption.Camel)
						{
							tempCasing = NameGeneratorCasingOption.Pascal;
						}

						//add each entry with proper spaces and casing
						int count = nameCollection.Count;
						for (int i = 1; i < count; ++i)
						{
							builder.Append(space);
							if (casing == NameGeneratorCasingOption.None)
							{
								name = nameCollection[i];
							}
							else
							{
								name = DoFirstWordCasing(nameCollection[i], tempCasing, textInfo);
							}
							builder.Append(name);
						}
						finalName = builder.ToString();
					}

					return finalName;
				}
				private struct RecognizedPhraseData
				{
					private static readonly char[] SpaceCharArray = new char[] { ' ' };
					private string[] myOriginalNames;
					private string myUnparsedReplacement;
					public static bool Populate(NameAlias alias, int remainingParts, out RecognizedPhraseData phraseData)
					{
						phraseData = new RecognizedPhraseData();
						string matchPhrase = ((RecognizedPhrase)alias.Element).Name;
						string replacePhrase = alias.Name;
						if (0 == string.Compare(matchPhrase, replacePhrase, StringComparison.CurrentCultureIgnoreCase))
						{
							// Sanity check, don't process these
							return false;
						}
						if (matchPhrase.IndexOf(' ') != -1)
						{
							if (replacePhrase.IndexOf(matchPhrase, StringComparison.CurrentCultureIgnoreCase) != -1)
							{
								// UNDONE: We handle expanding single words to a phrase containing the word, but not
								// multi-word phrases doing the same thing. However, make sure we don't recurse in this
								// situation.
								return false;
							}
							string[] parts = matchPhrase.Split(SpaceCharArray, StringSplitOptions.RemoveEmptyEntries);
							if (parts.Length > remainingParts)
							{
								return false;
							}
							phraseData.myOriginalNames = parts;
						}
						else
						{
							phraseData.myOriginalNames = new string[] { matchPhrase };
						}
						phraseData.myUnparsedReplacement = replacePhrase;
						return true;
					}
					public bool IsEmpty
					{
						get
						{
							return myOriginalNames == null;
						}
					}
					public string[] OriginalNames
					{
						get
						{
							return myOriginalNames;
						}
					}
					/// <summary>
					/// Get the replacement names. The assumption is that this is rarely called,
					/// and the results are not cached.
					/// </summary>
					public string[] ReplacementNames
					{
						get
						{
							string name = myUnparsedReplacement;
							return string.IsNullOrEmpty(name) ?
								new string[0] :
								(name.IndexOf(' ') != -1) ? name.Split(SpaceCharArray, StringSplitOptions.RemoveEmptyEntries) : new string[] { name };
						}
					}
				}
				private void ResolveRecognizedPhrases(ref NamePart singleName, ref List<NamePart> nameCollection, NameGenerator generator)
				{
					ORMModel model = ContextModel;
					if (model != null)
					{
						if (nameCollection != null)
						{
							int nameCount = nameCollection.Count;
							int remainingParts = nameCount;
							for (int i = 0; i < nameCount; ++i, --remainingParts)
							{
								// For each part, collection possible replacement phrases beginning with that name
								NamePart currentPart = nameCollection[i];
								RecognizedPhraseData singlePhrase = new RecognizedPhraseData();
								List<RecognizedPhraseData> phraseList = null;
								bool possibleReplacement = false;
								foreach (NameAlias alias in model.GetRecognizedPhrasesStartingWith(currentPart, generator))
								{
									RecognizedPhraseData phraseData;
									if (RecognizedPhraseData.Populate(alias, remainingParts, out phraseData))
									{
										if (phraseList == null)
										{
											possibleReplacement = true;
											if (singlePhrase.IsEmpty)
											{
												singlePhrase = phraseData;
											}
											else
											{
												phraseList = new List<RecognizedPhraseData>();
												phraseList.Add(singlePhrase);
												phraseList.Add(phraseData);
												singlePhrase = new RecognizedPhraseData();
											}
										}
										else
										{
											phraseList.Add(phraseData);
										}
									}
								}
								// If we have possible replacements, then look farther to see
								// if the multi-part phrases match. Start by searching the longest
								// match possible.
								if (possibleReplacement)
								{
									if (phraseList != null)
									{
										phraseList.Sort(delegate(RecognizedPhraseData left, RecognizedPhraseData right)
										{
											return right.OriginalNames.Length.CompareTo(left.OriginalNames.Length);
										});
										int phraseCount = phraseList.Count;
										for (int j = 0; j < phraseCount; ++j)
										{
											if (TestResolvePhraseDataForCollection(phraseList[j], ref singleName, ref nameCollection, i, generator))
											{
												return;
											}
										}
									}
									else
									{
										if (TestResolvePhraseDataForCollection(singlePhrase, ref singleName, ref nameCollection, i, generator))
										{
											return;
										}
									}
								}
							}
						}
						else if (!singleName.IsEmpty)
						{
							LocatedElement element = model.RecognizedPhrasesDictionary.GetElement(singleName);
							RecognizedPhrase phrase;
							NameAlias alias;
							if (null != (phrase = element.SingleElement as RecognizedPhrase) &&
								null != (alias = generator.FindMatchingAlias(phrase.AbbreviationCollection)))
							{
								RecognizedPhraseData phraseData;
								if (RecognizedPhraseData.Populate(alias, 1, out phraseData))
								{
									string[] replacements = phraseData.ReplacementNames;
									int replacementLength = replacements.Length;
									NamePart startingPart = singleName;
									singleName = new NamePart();
									if (replacementLength == 0)
									{
										// Highly unusual, but possible with collapsing phrases and omitted readings
										singleName = new NamePart();
									}
									else
									{
										string testForEqual = singleName;
										bool caseIfEqual = 0 != (singleName.Options & NamePartOptions.ExplicitCasing);
										singleName = new NamePart();
										if (replacementLength == 1)
										{
											string replacement = replacements[0];
											NamePartOptions options = NamePartOptions.None;
											if ((caseIfEqual && 0 == string.Compare(testForEqual, replacement, StringComparison.CurrentCulture)) ||
												(0 == string.Compare(testForEqual, replacement, StringComparison.CurrentCultureIgnoreCase)))
											{
												// Single replacement for same string
												return;
											}
											AddToNameCollection(ref singleName, ref nameCollection, new NamePart(replacement, options));
										}
										else
										{
											for (int i = 0; i < replacementLength; ++i)
											{
												string replacement = replacements[i];
												NamePartOptions options = NamePartOptions.None;
												if (caseIfEqual && 0 == string.Compare(testForEqual, replacement, StringComparison.CurrentCulture))
												{
													options |= NamePartOptions.ExplicitCasing | NamePartOptions.ReplacementOfSelf;
												}
												else if (0 == string.Compare(testForEqual, replacement, StringComparison.CurrentCultureIgnoreCase))
												{
													options |= NamePartOptions.ReplacementOfSelf;
												}
												AddToNameCollection(ref singleName, ref nameCollection, new NamePart(replacement, options));
											}
										}
										ResolveRecognizedPhrases(ref singleName, ref nameCollection, generator);
									}
									return;
								}
							}
						}
					}
				}
				/// <summary>
				/// Helper for ResolveRecognizedPhrases. Returns true is parent processing is complete.
				/// </summary>
				private bool TestResolvePhraseDataForCollection(RecognizedPhraseData phraseData, ref NamePart singleName, ref List<NamePart> nameCollection, int collectionIndex, NameGenerator generator)
				{
					Debug.Assert(nameCollection != null);
					string[] matchNames = phraseData.OriginalNames;
					int matchLength = matchNames.Length;
					int i = 0;
					int firstExplicitPart = -1;
					int explicitPartCount = 0;
					for (; i < matchLength; ++i) // Note the bound on this is already verified by RecognizedPhraseData.Populate
					{
						NamePart testPart = nameCollection[collectionIndex + i];
						bool currentPartExplicit = 0 != (testPart.Options & NamePartOptions.ExplicitCasing);
						if (0 != string.Compare(testPart, matchNames[i], currentPartExplicit ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase) ||
							(matchLength == 1 && 0 != (testPart.Options & NamePartOptions.ReplacementOfSelf)))
						{
							break;
						}
						if (currentPartExplicit && firstExplicitPart == -1)
						{
							++explicitPartCount;
							firstExplicitPart = i;
						}
					}
					if (i == matchLength)
					{
						// We have a valid replacement, apply it and recurse
						string[] explicitlyCasedNames = null;
						string singleMatchName = (matchLength == 1) ? matchNames[0] : null;
						if (explicitPartCount != 0)
						{
							explicitlyCasedNames = new string[explicitPartCount];
							int nextExplicitName = 0;
							for (int j = collectionIndex + firstExplicitPart; ; ++j)
							{
								NamePart testPart = nameCollection[j];
								if (0 != (testPart.Options & NamePartOptions.ExplicitCasing))
								{
									explicitlyCasedNames[nextExplicitName] = testPart;
									if (++nextExplicitName == explicitPartCount)
									{
										break;
									}
								}
							}
							if (explicitPartCount > 1)
							{
								Array.Sort<string>(explicitlyCasedNames, StringComparer.CurrentCulture);
							}
						}
						nameCollection.RemoveRange(collectionIndex, matchLength);
						int startingCollectionSize = nameCollection.Count;
						string[] replacements = phraseData.ReplacementNames;
						for (i = 0; i < replacements.Length; ++i)
						{
							// Recognized phrases do not record casing priority and phrases are
							// generally treated as case insensitive. However, if any replacement
							// word exactly matches an explicitly cased word in the original names
							// then case the replacement as well.
							NamePartOptions options = NamePartOptions.None;
							string replacement = replacements[i];
							if (explicitlyCasedNames != null && 0 <= Array.BinarySearch<string>(explicitlyCasedNames, replacement, StringComparer.CurrentCulture))
							{
								options |= NamePartOptions.ExplicitCasing;
								if (matchLength == 1)
								{
									options |= NamePartOptions.ReplacementOfSelf;
								}
							}
							else if (singleMatchName != null && 0 == string.Compare(singleMatchName, replacement, StringComparison.CurrentCultureIgnoreCase))
							{
								options |= NamePartOptions.ExplicitCasing;
							}
							AddToNameCollection(ref singleName, ref nameCollection, new NamePart(replacement, options), collectionIndex + nameCollection.Count - startingCollectionSize);
						}
						ResolveRecognizedPhrases(ref singleName, ref nameCollection, generator);
						return true;
					}
					return false;
				}
				#endregion // NameCollection helpers
				#region Casing helpers
				private string DoFirstWordCasing(NamePart name, NameGeneratorCasingOption casing, TextInfo textInfo)
				{
					if (name.ExplicitCasing) return name;
					switch (casing)
					{
						case NameGeneratorCasingOption.Camel:
							return TestHasAdjacentUpperCase(name) ? (string)name : DoFirstLetterCase(name, false, textInfo);
						case NameGeneratorCasingOption.Pascal:
							return TestHasAdjacentUpperCase(name) ? (string)name : DoFirstLetterCase(name, true, textInfo);
						case NameGeneratorCasingOption.Lower:
							return TestHasAdjacentUpperCase(name) ? (string)name : textInfo.ToLower(name);
						case NameGeneratorCasingOption.Upper:
							return textInfo.ToUpper(name);
					}

					return null;
				}
				private bool TestHasAdjacentUpperCase(string name)
				{
					if (!string.IsNullOrEmpty(name))
					{
						int length = name.Length;
						bool previousCharUpper = false;
						for (int i = 0; i < length; ++i)
						{
							if (Char.IsUpper(name, i))
							{
								if (previousCharUpper)
								{
									return true;
								}
								previousCharUpper = true;
							}
							else
							{
								previousCharUpper = false;
							}
						}
					}
					return false;
				}
				private string DoFirstLetterCase(NamePart name, bool upper, TextInfo textInfo)
				{
					string nameValue = name;
					char c = nameValue[0];
					if (upper)
					{
						c = textInfo.ToUpper(c);
					}
					else
					{
						c = textInfo.ToLower(c);
					}
					if (nameValue.Length > 1)
					{
						nameValue = c.ToString() + nameValue.Substring(1);
					}
					else
					{
						nameValue = c.ToString();
					}
					return nameValue;
				}
				#endregion // Casing helpers
				#region Column analysis
				/// <summary>
				/// Flag values for <see cref="ColumnPathStep"/> elements
				/// </summary>
				[Flags]
				private enum ColumnPathStepFlags
				{
					/// <summary>
					/// Flags not set
					/// </summary>
					None = 0,
					/// <summary>
					/// A subtype that has not been separated or partitioned
					/// </summary>
					ForwardSubtype = 1,
					/// <summary>
					/// A subtype that has been separated or partitioned
					/// </summary>
					ReverseSubtype = 2,
					/// <summary>
					/// A FactType that is part of an identifier
					/// </summary>
					IsIdentifier = 4,
					/// <summary>
					/// The step is passed the identification stage
					/// </summary>
					PassedIdentifier = 8,
					/// <summary>
					/// This step needs special name generation
					/// </summary>
					RequiresDecoration = 0x10,
				}
				[DebuggerDisplay("{System.String.Concat(ObjectType.Name, (FromRole != null) ? System.String.Concat(\", \", FromRole.FactType.Name) : \"\", \" Flags=\", Flags.ToString(\"g\"))}")]
				private struct ColumnPathStep
				{
					private ColumnPathStepFlags myFlags;
					private ObjectType myObjectType;
					private ObjectType myResolvedSupertype;
					private Role myFromRole;
					public ColumnPathStep(Role fromRole, ObjectType targetObjectType, ObjectType resolvedSupertype, ColumnPathStepFlags flags)
					{
						myFromRole = fromRole;
						myObjectType = targetObjectType;
						myResolvedSupertype = resolvedSupertype;
						myFlags = flags;
					}
					public Role FromRole
					{
						get
						{
							return myFromRole;
						}
					}
					public ObjectType ObjectType
					{
						get
						{
							return myObjectType;
						}
					}
					public ObjectType ResolvedSupertype
					{
						get
						{
							return myResolvedSupertype ?? myObjectType;
						}
					}
					public ColumnPathStepFlags Flags
					{
						get
						{
							return myFlags;
						}
						set
						{
							myFlags = value;
						}
					}
					/// <summary>
					/// Test for equality without checking the state of the flags or the resolved supertype
					/// </summary>
					public bool CoreEquals(ColumnPathStep otherStep)
					{
						return myObjectType == otherStep.myObjectType && myFromRole == otherStep.myFromRole;
					}
				}
				private Table myColumnStepTable;
				private Dictionary<Column, LinkedNode<ColumnPathStep>> myColumnSteps;
				private LinkedNode<ColumnPathStep> GetColumnPath(Column column)
				{
					Table table = column.Table;
					if (myColumnStepTable == table)
					{
						return myColumnSteps[column];
					}
					Dictionary<Column, LinkedNode<ColumnPathStep>> columnSteps = myColumnSteps;
					if (myColumnStepTable == null)
					{
						myColumnSteps = columnSteps = new Dictionary<Column, LinkedNode<ColumnPathStep>>();
					}
					else
					{
						columnSteps.Clear();
					}
					myColumnStepTable = table;

					// Seed the cross-cutting dictionary and seed it with the main represented
					// ObjectTypes to force decorations on columns that loop back into this table.
					Dictionary<ObjectType, LinkedNode<LinkedNode<ColumnPathStep>>> objectTypeToSteps = new Dictionary<ObjectType, LinkedNode<LinkedNode<ColumnPathStep>>>();
					ConceptType primaryConceptType = TableIsPrimarilyForConceptType.GetConceptType(table);
					if (primaryConceptType != null)
					{
						ObjectType targetObjectType = ConceptTypeIsForObjectType.GetObjectType(primaryConceptType);
						objectTypeToSteps[targetObjectType] = new LinkedNode<LinkedNode<ColumnPathStep>>(new LinkedNode<ColumnPathStep>(new ColumnPathStep(null, targetObjectType, null, ColumnPathStepFlags.None)));
					}
					foreach (ConceptType secondaryConceptType in TableIsAlsoForConceptType.GetConceptType(table))
					{
						ObjectType targetObjectType = ConceptTypeIsForObjectType.GetObjectType(secondaryConceptType);
						objectTypeToSteps[targetObjectType] = new LinkedNode<LinkedNode<ColumnPathStep>>(new LinkedNode<ColumnPathStep>(new ColumnPathStep(null, targetObjectType, null, ColumnPathStepFlags.None)));
					}
					LinkedElementCollection<Column> columns = table.ColumnCollection;
					int columnCount = columns.Count;
					for (int iColumn = 0; iColumn < columnCount; ++iColumn)
					{
						Column currentColumn = columns[iColumn];
						LinkedElementCollection<ConceptTypeChild> childPath = ColumnHasConceptTypeChild.GetConceptTypeChildPath(currentColumn);
						int childPathCount = childPath.Count;
						LinkedNode<ColumnPathStep> headNode = null;
						LinkedNode<ColumnPathStep> tailNode = null;
						bool passedIdentifier = false;
						bool processTailDelayed = false;
						for (int iChild = 0; iChild < childPathCount; ++iChild)
						{
							ConceptTypeChild child = childPath[iChild];
							ConceptTypeAssimilatesConceptType assimilation = child as ConceptTypeAssimilatesConceptType;
							bool reverseAssimilation = assimilation != null && AssimilationMapping.GetAbsorptionChoiceFromAssimilation(assimilation) != AssimilationAbsorptionChoice.Absorb;
							LinkedElementCollection<FactType> factTypes = ConceptTypeChildHasPathFactType.GetPathFactTypeCollection(child);
							int factTypeCount = factTypes.Count;
							for (int iFactType = 0; iFactType < factTypeCount; ++iFactType)
							{
								FactType factType = factTypes[iFactType];
								Role targetRole = FactTypeMapsTowardsRole.GetTowardsRole(factType).Role;
								if (assimilation != null && !reverseAssimilation)
								{
									targetRole = targetRole.OppositeRoleAlwaysResolveProxy.Role;
								}
								ColumnPathStepFlags flags = passedIdentifier ? ColumnPathStepFlags.PassedIdentifier : 0;
								ColumnPathStep pathStep;
								bool processPreviousTail = false;
								if (reverseAssimilation)
								{
									if (tailNode != null && 0 != (tailNode.Value.Flags & ColumnPathStepFlags.ReverseSubtype))
									{
										continue;
									}
									flags |= ColumnPathStepFlags.ReverseSubtype;
									pathStep = new ColumnPathStep(null, targetRole.RolePlayer, null, flags);
									processPreviousTail = processTailDelayed;
									processTailDelayed = false;
								}
								else if (assimilation != null)
								{
									flags |= ColumnPathStepFlags.ForwardSubtype;
									pathStep = new ColumnPathStep(null, targetRole.RolePlayer, null, flags);
									if (tailNode != null && 0 != (tailNode.Value.Flags & ColumnPathStepFlags.ForwardSubtype))
									{
										tailNode.Value = pathStep;
										continue;
									}
									processTailDelayed = true;
								}
								else
								{
									if (tailNode != null && 0 != (tailNode.Value.Flags & (ColumnPathStepFlags.ForwardSubtype | ColumnPathStepFlags.ReverseSubtype)))
									{
										// Add a resolved supertype to the reverse subtype to
										// allow later steps to be compared to this one.
										pathStep = tailNode.Value;
										tailNode.Value = new ColumnPathStep(pathStep.FromRole, pathStep.ObjectType, targetRole.RolePlayer, pathStep.Flags);
									}
									Role oppositeRole = targetRole.OppositeRoleAlwaysResolveProxy.Role;
									ORMUniquenessConstraint pid = targetRole.RolePlayer.PreferredIdentifier;
									if (pid != null && pid.RoleCollection.Contains(oppositeRole))
									{
										flags |= ColumnPathStepFlags.IsIdentifier;
										passedIdentifier = true;
									}
									pathStep = new ColumnPathStep(targetRole, oppositeRole.RolePlayer, null, flags);
									processPreviousTail = processTailDelayed;
									processTailDelayed = false;
								}
								if (processPreviousTail && tailNode != null)
								{
									ProcessTailNode(tailNode, objectTypeToSteps);
								}
								LinkedNode<ColumnPathStep> newNode = new LinkedNode<ColumnPathStep>(pathStep);
								if (tailNode == null)
								{
									headNode = tailNode = newNode;
								}
								else
								{
									tailNode.SetNext(newNode, ref headNode);
									tailNode = newNode;
								}
								if (!processTailDelayed)
								{
									ProcessTailNode(tailNode, objectTypeToSteps);
								}
							}
						}
						if (processTailDelayed)
						{
							ProcessTailNode(tailNode, objectTypeToSteps);
						}
						columnSteps.Add(currentColumn, headNode);
					}
					return columnSteps[column];
				}
				private static void ProcessTailNode(LinkedNode<ColumnPathStep> tailNode, Dictionary<ObjectType, LinkedNode<LinkedNode<ColumnPathStep>>> objectTypeToSteps)
				{
					ColumnPathStep step = tailNode.Value;
					ObjectType objectType = step.ObjectType;
					Role fromRole = step.FromRole;
					LinkedNode<LinkedNode<ColumnPathStep>> existingNodesHead;
					LinkedNode<LinkedNode<ColumnPathStep>> newNode = new LinkedNode<LinkedNode<ColumnPathStep>>(tailNode);
					if (objectTypeToSteps.TryGetValue(objectType, out existingNodesHead))
					{
						LinkedNode<LinkedNode<ColumnPathStep>> crossCutNode = existingNodesHead;
						do
						{
							LinkedNode<ColumnPathStep> pathNode = crossCutNode.Value;
							ColumnPathStep currentStep = pathNode.Value;
							if (currentStep.FromRole != fromRole)
							{
								// If there is any path divergence coming into the ObjectType
								// then all paths into the node need decorating directly, or
								// an early node needs decorating if this is part of the preferred
								// identifier of the earlier node but not the current one.
								bool decorateStep = false;
								bool decorateStepPathNonIdentifier = false;
								LinkedNode<ColumnPathStep> stepPathNonIdentifierNode = GetPreviousNonIdentifierNode(tailNode);
								LinkedNode<LinkedNode<ColumnPathStep>> crossCutNode2 = existingNodesHead;
								do
								{
									LinkedNode<ColumnPathStep> pathNode2 = crossCutNode2.Value;
									LinkedNode<ColumnPathStep> pathNonIdentifierNode = GetPreviousNonIdentifierNode(pathNode2);
									if (pathNonIdentifierNode != stepPathNonIdentifierNode &&
										((pathNonIdentifierNode == null ^ stepPathNonIdentifierNode == null) ||
										(!pathNonIdentifierNode.Value.CoreEquals(stepPathNonIdentifierNode.Value))))
									{
										if (pathNonIdentifierNode != null)
										{
											if (pathNode2 != pathNonIdentifierNode)
											{
												pathNode2 = null;
												DecorateCrossCutNonIdentifiers(objectTypeToSteps[pathNonIdentifierNode.Value.ObjectType]);
											}
										}
										decorateStepPathNonIdentifier = true;
									}
									else
									{
										decorateStep = true;
									}
									if (pathNode2 != null)
									{
										ColumnPathStep pathStep2 = pathNode2.Value;
										if (0 == (pathStep2.Flags & ColumnPathStepFlags.RequiresDecoration))
										{
											pathStep2.Flags |= ColumnPathStepFlags.RequiresDecoration;
											pathNode2.Value = pathStep2;
										}
									}
									crossCutNode2 = crossCutNode2.Next;
								} while (crossCutNode2 != null);
								if (decorateStepPathNonIdentifier && stepPathNonIdentifierNode != null)
								{
									if (stepPathNonIdentifierNode == tailNode)
									{
										decorateStep = true;
									}
									else
									{
										DecorateCrossCutNonIdentifiers(objectTypeToSteps[stepPathNonIdentifierNode.Value.ObjectType]);
									}
								}
								if (decorateStep)
								{
									step.Flags |= ColumnPathStepFlags.RequiresDecoration;
									tailNode.Value = step;
								}
								break;
							}
							crossCutNode = crossCutNode.Next;
						} while (crossCutNode != null);
						newNode.SetNext(existingNodesHead, ref newNode);
					}
					objectTypeToSteps[objectType] = newNode;
				}
				/// <summary>
				/// Helper for <see cref="ProcessTailNode"/>
				/// </summary>
				private static void DecorateCrossCutNonIdentifiers(LinkedNode<LinkedNode<ColumnPathStep>> nonIdentifierCrossCutNode)
				{
					do
					{
						LinkedNode<ColumnPathStep> nonIdentifierNode = nonIdentifierCrossCutNode.Value;
						ColumnPathStep nonIdentifierStep = nonIdentifierNode.Value;
						ColumnPathStepFlags nonIdentifierStepFlags = nonIdentifierStep.Flags;
						if (0 == (nonIdentifierStepFlags & (ColumnPathStepFlags.IsIdentifier | ColumnPathStepFlags.PassedIdentifier | ColumnPathStepFlags.RequiresDecoration)))
						{
							nonIdentifierStep.Flags |= ColumnPathStepFlags.RequiresDecoration;
							nonIdentifierNode.Value = nonIdentifierStep;
						}
						nonIdentifierCrossCutNode = nonIdentifierCrossCutNode.Next;
					} while (nonIdentifierCrossCutNode != null);
				}
				/// <summary>
				/// Helper for <see cref="ProcessTailNode"/>
				/// </summary>
				private static LinkedNode<ColumnPathStep> GetPreviousNonIdentifierNode(LinkedNode<ColumnPathStep> stepNode)
				{
					LinkedNode<ColumnPathStep> currentNode = stepNode;
					do
					{
						ColumnPathStepFlags flags = currentNode.Value.Flags;
						if (0 == (flags & (ColumnPathStepFlags.IsIdentifier | ColumnPathStepFlags.PassedIdentifier)))
						{
							return currentNode;
						}
						currentNode = currentNode.Previous;
					} while (currentNode != null);
					return null;
				}
				#endregion // Column analysis
			}
			#endregion // DefaultDatabaseNameGenerator class
		}
	}
	#endregion // ORMAbstractionToConceptualDatabaseBridgeDomainModel.NameGeneration class

	#region ORMAbstractionToConceptualDatabaseBridgeDomainModel.RelationalNameGenerator Class
	partial class RelationalNameGenerator
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="store">Store where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public RelationalNameGenerator(Store store, params PropertyAssignment[] propertyAssignments)
			: this(store != null ? store.DefaultPartition : null, propertyAssignments)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="partition">Partition where new element is to be created.</param>
		/// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
		public RelationalNameGenerator(Partition partition, params PropertyAssignment[] propertyAssignments)
			: base(partition, GenerateDefaultValues(propertyAssignments))
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyAssignments"></param>
		/// <returns></returns>
		private static PropertyAssignment[] GenerateDefaultValues(params PropertyAssignment[] propertyAssignments)
		{
			PropertyAssignment[] properties = propertyAssignments;
			string nameUsage = null;
			int casingOptionIndex = -1;
			int spacingOptionIndex = -1;
			for (int i = 0; i < properties.Length; ++i)
			{
				PropertyAssignment assignment = properties[i];
				Guid propertyId = assignment.PropertyId;
				if (propertyId == NameGenerator.CasingOptionDomainPropertyId)
				{
					casingOptionIndex = i;
				}
				else if (propertyId == NameGenerator.NameUsageDomainPropertyId)
				{
					nameUsage = (string)assignment.Value;
				}
				else if (propertyId == NameGenerator.SpacingFormatDomainPropertyId)
				{
					spacingOptionIndex = i;
				}
			}
			if (nameUsage == null)
			{
				if (spacingOptionIndex != -1)
				{
					properties[spacingOptionIndex] = new PropertyAssignment(NameGenerator.SpacingFormatDomainPropertyId, NameGeneratorSpacingFormat.Remove);
				}
			}
			else if (casingOptionIndex != -1)
			{
				if (nameUsage == "RelationalColumn")
				{
					properties[casingOptionIndex] = new PropertyAssignment(NameGenerator.CasingOptionDomainPropertyId, NameGeneratorCasingOption.Camel);
				}
				else if (nameUsage == "RelationalTable")
				{
					properties[casingOptionIndex] = new PropertyAssignment(NameGenerator.CasingOptionDomainPropertyId, NameGeneratorCasingOption.Pascal);
				}
			}
			return properties;
		}
	}
	#endregion // ORMAbstractionToConceptualDatabaseBridgeDomainModel.RelationalNameGenerator Class
}
