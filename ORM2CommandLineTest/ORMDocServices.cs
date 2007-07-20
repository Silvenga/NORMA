﻿//#define TESTDUMP
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.ORM.ShapeModel;
using Neumont.Tools.ORM;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Runtime.Serialization;
using Neumont.Tools.ORM.Shell;
using Neumont.Tools.Modeling;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.XmlDiffPatch;
using Neumont.Tools.Modeling.Shell.DynamicSurveyTreeGrid;
using Neumont.Tools.Modeling.Diagrams;

namespace Neumont.Tools.ORM.SDK.TestEngine
{
	public partial struct Suite
	{
		/// <summary>
		/// Create a new Services object
		/// </summary>
		/// <returns>IORMToolServices implementation</returns>
		public static IORMToolServices CreateServices()
		{
			return new ORMDocServices();
		}
		private class ORMDocServices : IORMToolServices, IORMFontAndColorService, IORMToolTestServices, IORMToolTestSuiteReportFactory, IORMPropertyProviderService, IServiceProvider
		{
			#region IORMToolServices Implementation
			IORMPropertyProviderService IORMToolServices.PropertyProviderService
			{
				get
				{
					return this;
				}
			}
			IORMFontAndColorService IORMToolServices.FontAndColorService
			{
				get
				{
					return this;
				}
			}

			private IORMToolTaskProvider myTaskProvider;
			IORMToolTaskProvider IORMToolServices.TaskProvider
			{
				get
				{
					IORMToolTaskProvider provider = myTaskProvider;
					if (provider == null)
					{
						myTaskProvider = provider = new ORMTaskProvider();
					}
					return provider;
				}
			}
			IServiceProvider IORMToolServices.ServiceProvider
			{
				get
				{
					return this;
				}
			}
			IDictionary<string, VerbalizationTargetData> IORMToolServices.VerbalizationTargets
			{
				get
				{
					return null;
				}
			}
			IDictionary<Type, IVerbalizationSets> IORMToolServices.GetVerbalizationSnippetsDictionary(string target)
			{
				return null;
			}
			INotifySurveyElementChanged IORMToolServices.NotifySurveyElementChanged
			{
				get
				{
					return null;
				}
			}
			bool IORMToolServices.CanAddTransaction
			{
				get
				{
					return true;
				}
				set
				{
				}
			}
			LayoutEngine IORMToolServices.GetLayoutEngine(Type engineType)
			{
				return null;
			}
			bool IORMToolServices.ActivateShape(ShapeElement shape)
			{
				return false;
			}
			#endregion // IORMToolServices Implementation
			#region IORMFontAndColorService Implementation
			Color IORMFontAndColorService.GetBackColor(ORMDesignerColor colorIndex)
			{
				return Color.Black;
			}
			Font IORMFontAndColorService.GetFont(ORMDesignerColorCategory fontCategory)
			{
				Font font = new Font(new FontFamily("Times New Roman"), 10, FontStyle.Regular);
				return font;
			}
			FontStyle IORMFontAndColorService.GetFontStyle(ORMDesignerColor colorIndex)
			{
				return FontStyle.Bold;
			}
			Color IORMFontAndColorService.GetForeColor(ORMDesignerColor colorIndex)
			{
				return Color.White;
			}
			#endregion // IORMFontAndColorService Implementation
			#region IORMPropertyProviderService Implementation
			void IORMPropertyProviderService.GetProvidedProperties(IORMExtendableElement extendableElement, System.ComponentModel.PropertyDescriptorCollection properties)
			{
				// We don't yet need to support this for testing.
			}
			void IORMPropertyProviderService.AddOrRemovePropertyProvider<TExtendableElement>(ORMPropertyProvisioning propertyProvisioning, bool includeSubtypes, EventHandlerAction action)
			{
				// We don't yet need to support this for testing.
			}
			#endregion // IORMPropertyProviderService Implementation
			#region IServiceProvider Implementation
			object IServiceProvider.GetService(Type serviceType)
			{
				if (serviceType == typeof(IORMToolTestServices) ||
					serviceType == typeof(IORMToolTestSuiteReportFactory))
				{
					return this;
				}
				return null;
			}
			#endregion // IServiceProvider Implementation
			#region IORMToolTestServices Implementation
			private static XslCompiledTransform myIdMapTransform;
			private XslCompiledTransform IdMapTransform
			{
				get
				{
					XslCompiledTransform retVal = myIdMapTransform;
					if (retVal == null)
					{
						lock (LockObject)
						{
							retVal = myIdMapTransform;
							if (retVal == null)
							{
								retVal = new XslCompiledTransform();
								Type resourceType = typeof(Suite);
								using (Stream transformStream = resourceType.Assembly.GetManifestResourceStream(resourceType, "MapIds.xslt"))
								{
									using (StreamReader reader = new StreamReader(transformStream))
									{
										using (XmlReader xmlReader = new XmlTextReader(reader))
										{
											retVal.Load(xmlReader, null, null);
										}
									}
								}
								myIdMapTransform = retVal;
							}
						}
					}
					return retVal;
				}
			}
			Store IORMToolTestServices.Load(MethodInfo testMethod, string referenceName)
			{
				Store retVal = null;
				Type testType = testMethod.ReflectedType;
				Assembly testAssembly = testType.Assembly;
				if (referenceName == null)
				{
					referenceName = "";
				}
				string loadString = (referenceName.Length == 0) ? ".Load" : ".Load.";
				string resourceName = string.Concat(testType.FullName, ".", testMethod.Name, loadString, referenceName, ".orm");
				Stream resourceStream = null;
				try
				{
					if (null != testAssembly.GetManifestResourceInfo(resourceName))
					{
						resourceStream = testAssembly.GetManifestResourceStream(resourceName);
					}
					else if (referenceName.Length == 0)
					{
						// Default to the new orm model file template if a referenceName
						// is not specified.
						Type resourceType = typeof(Suite);
						resourceStream = resourceType.Assembly.GetManifestResourceStream(resourceType, "ORMModel.orm");
					}
					if (resourceStream != null)
					{
						retVal = LoadFileStream(resourceStream);
					}
				}
				finally
				{
					if (resourceStream != null)
					{
						((IDisposable)resourceStream).Dispose();
					}
				}
				return retVal;
			}
			private Store LoadFileStream(Stream stream)
			{
				if (stream == null)
				{
					return null;
				}
				ORMStore store = new ORMStore(this);
				store.UndoManager.UndoState = UndoState.Disabled;
				Type[] domainModels = new Type[5] { typeof(CoreDomainModel), typeof(CoreDesignSurfaceDomainModel), typeof(FrameworkDomainModel), typeof(ORMCoreDomainModel), typeof(ORMShapeDomainModel) };
				store.LoadDomainModels(domainModels);
				using (Transaction t = store.TransactionManager.BeginTransaction("File load and fixup"))
				{
					if (stream.Length > 1)
					{
						(new ORMSerializationEngine(store)).Load(stream);
					}
					t.Commit();
				}
				AddErrorReportingEvents(store);
				store.UndoManager.UndoState = UndoState.Enabled;
				return store;
			}
			void IORMToolTestServices.Compare(Store store, MethodInfo testMethod, string referenceName)
			{
				// See if we have anything to compare it to
				Type testType = testMethod.ReflectedType;
				Assembly testAssembly = testType.Assembly;
				if (referenceName == null)
				{
					referenceName = "";
				}
				try
				{
					myReportWriter.WriteStartElement("Compare");
					if (referenceName.Length != 0)
					{
						myReportWriter.WriteAttributeString("name", referenceName);
					}
					using (MemoryStream currentStream = new MemoryStream())
					{
						// Get the current data into a stream
						(new ORMSerializationEngine(store)).Save(currentStream);
						currentStream.Seek(0, SeekOrigin.Begin);

						string compareString = (referenceName.Length == 0) ? ".Compare" : ".Compare.";
						string resourceName = string.Concat(testType.FullName, ".", testMethod.Name, compareString, referenceName, ".orm");
						Stream baselineStream = null;
						try
						{
							// Get the baseline that we're comparing to
							if (null != testAssembly.GetManifestResourceInfo(resourceName))
							{
								baselineStream = testAssembly.GetManifestResourceStream(resourceName);
							}
							if (baselineStream != null)
							{
								bool hasDiff = false;

								// See if the data is different. If it is different, we'll
								// first attempt to clean up any GUID differences, then compare
								// again to see if differences remain.
								XmlDiff diff = DiffEngine;
								XmlReaderSettings readerSettings = DetachableReaderSettings;
								XmlWriterSettings writerSettings = DetachableWriterSettings;
								using (MemoryStream diffStream = new MemoryStream())
								{
									using (XmlReader baselineReader = XmlReader.Create(baselineStream, readerSettings))
									{
										using (XmlReader currentReader = XmlTextReader.Create(currentStream, readerSettings))
										{
											using (XmlWriter diffWriter = XmlWriter.Create(diffStream, writerSettings))
											{
												hasDiff = !diff.Compare(baselineReader, currentReader, diffWriter);
											}
										}
									}
									if (hasDiff)
									{
										// Clean up Guid differences
										diffStream.Seek(0, SeekOrigin.Begin);

#if TESTDUMP
										// Code for testing purposes, currentDumpPreGuidScrub will contain the current xml
										currentStream.Seek(0, SeekOrigin.Begin);
										string currentDumpPreGuidScrub = null;
										using (XmlReader debugReader = XmlReader.Create(currentStream, readerSettings))
										{
											StringBuilder sb = new StringBuilder();
											using (XmlWriter debugWriter = XmlWriter.Create(sb))
											{
												debugWriter.Settings.Encoding = Encoding.UTF8;
												FormatXml(debugReader, debugWriter);
											}
											currentDumpPreGuidScrub = sb.ToString();
										}
#endif // TESTDUMP

										baselineStream.Seek(0, SeekOrigin.Begin);
										StringMapDictionary idMap = null;

										// Get a list of recognized id values from the base document.
										// This allows the GetIdMap function to distinguish a mapped
										// id (one created for a new element that is otherwise equivalent
										// to the new element in the base) from a reordered element.
										Dictionary<string, object> knownIds = new Dictionary<string, object>();
										using (XmlReader docReader = XmlReader.Create(baselineStream, readerSettings))
										{
											while (docReader.Read())
											{
												if (docReader.NodeType == XmlNodeType.Element)
												{
													string attr = docReader.GetAttribute("id");
													if (attr != null && attr.Length != 0)
													{
														knownIds[attr] = null;
													}
												}
											}
										}
										baselineStream.Seek(0, SeekOrigin.Begin);

										// Get a map for all changed ids that are not
										// known in the base file to the values they
										// are replacing.
										using (XmlReader docReader = XmlReader.Create(baselineStream, readerSettings))
										{
											using (XmlReader diffReader = XmlTextReader.Create(diffStream, readerSettings))
											{
												idMap = GetIdMap(docReader, diffReader, knownIds);
											}
										}
										diffStream.Seek(0, SeekOrigin.Begin);

										if (idMap != null)
										{
											XsltArgumentList args = new XsltArgumentList();
											args.AddExtensionObject("id-map-extension", idMap);
											using (MemoryStream modifiedCurrentStream = new MemoryStream())
											{
												// Transform the current stream to resolve guid differences
												currentStream.Seek(0, SeekOrigin.Begin);
												using (XmlReader currentReader = XmlTextReader.Create(currentStream, readerSettings))
												{
													using (XmlWriter modifiedCurrentWriter = XmlWriter.Create(modifiedCurrentStream, writerSettings))
													{
														IdMapTransform.Transform(currentReader, args, modifiedCurrentWriter);
													}
												}

#if TESTDUMP
												// Code for testing purposes, currentDumpPostGuidScrub will contain the current xml
												modifiedCurrentStream.Seek(0, SeekOrigin.Begin);
												string currentDumpPostGuidScrub = null;
												using (XmlReader debugReader = XmlReader.Create(modifiedCurrentStream, readerSettings))
												{
													StringBuilder sb = new StringBuilder();
													using (XmlWriter debugWriter = XmlWriter.Create(sb))
													{
														debugWriter.Settings.Encoding = Encoding.UTF8;
														FormatXml(debugReader, debugWriter);
													}
													currentDumpPostGuidScrub = sb.ToString();
												}
#endif // TESTDUMP
												
												modifiedCurrentStream.Seek(0, SeekOrigin.Begin);

												// Repeat the comparison to get a new diffgram
												baselineStream.Seek(0, SeekOrigin.Begin);
												using (XmlReader baselineReader = XmlReader.Create(baselineStream, readerSettings))
												{
													using (XmlReader currentReader = XmlTextReader.Create(modifiedCurrentStream, readerSettings))
													{
														using (XmlWriter diffWriter = XmlWriter.Create(diffStream, writerSettings))
														{
															hasDiff = !diff.Compare(baselineReader, currentReader, diffWriter);
														}
													}
												}
												if (hasDiff)
												{
													// The original diff will be longer than the current diff,
													// but we're using the same stream. Truncate the stream.
													diffStream.SetLength(diffStream.Position);
													diffStream.Seek(0, SeekOrigin.Begin);
												}
											}
										}
									}
									myReportWriter.WriteAttributeString("result", hasDiff ? "failDiffgram" : "pass");
									if (hasDiff)
									{
										using (XmlReader reader = XmlTextReader.Create(diffStream, DetachableReaderSettings))
										{
											reader.MoveToContent();
											myReportWriter.WriteNode(reader, false);
										}
									}
								}
							} // baselineStream != null
							else
							{
								// There is no base line, so we can't establish whether or not
								// the test succeeded. Write a fail report with the current settings.
								myReportWriter.WriteAttributeString("result", "failMissingBaseline");
								using (XmlReader reader = XmlTextReader.Create(currentStream, DetachableReaderSettings))
								{
									reader.MoveToContent();
									myReportWriter.WriteNode(reader, false);
								}
							}
						}
						finally
						{
							if (baselineStream != null)
							{
								((IDisposable)baselineStream).Dispose();
							}
						}
					}
				}
				finally
				{
					myReportWriter.WriteEndElement();
				}
			}
			/// <summary>
			/// Used in a stack to track position and value
			/// while walking a an Xml Diff map.
			/// </summary>
			private struct DiffMatchRecord
			{
				public DiffMatchRecord(int level, int match)
				{
					Level = level;
					Match = match;
				}
				/// <summary>
				/// The recursion level
				/// </summary>
				public int Level;
				/// <summary>
				/// The match value from the diff xml
				/// </summary>
				public int Match;
			}
			/// <summary>
			/// Provide a dictionary class suitable to be used as
			/// an extension object from an xslt transform
			/// </summary>
			private class StringMapDictionary : Dictionary<string, string>
			{
				/// <summary>
				/// Map the current value to an alternate
				/// value if one is specified. Otherwise, leave
				/// return the value.
				/// </summary>
				/// <param name="value">Original value</param>
				/// <returns>Mapped value, or original value</returns>
				public string Map(string value)
				{
					string retVal;
					return TryGetValue(value, out retVal) ? retVal : value;
				}
			}
			/// <summary>
			/// The XmlDiffPatch engine spits the new data, but not the old. We need to
			/// reexamine the original document to retrieve the old data for the id attributes.
			/// Note that this routine assumes all ids are globally unique values.
			/// </summary>
			/// <param name="docReader">An initialized reader for the original document</param>
			/// <param name="diffReader">An initialized reader for the xml diffgram</param>
			/// <param name="knownIds">A dictionary of recognized id values. Any changed id that
			/// is known in the original document is not considered mappable.</param>
			/// <returns>Dictionary of replacement id values keyed off the original values</returns>
			private static StringMapDictionary GetIdMap(XmlReader docReader, XmlReader diffReader, Dictionary<string, object> knownIds)
			{
				StringMapDictionary retVal = null;
				Stack<DiffMatchRecord> diffMatchStack = new Stack<DiffMatchRecord>();
				diffMatchStack.Push(new DiffMatchRecord(0, 0)); // Seed so we can Peek safely
				int currentLevel = 0;
				int docPassPending = 0;
				diffReader.MoveToContent(); // Jump to root element
				while (diffReader.Read())
				{
					XmlNodeType diffNodeType = diffReader.NodeType;
					switch (diffNodeType)
					{
						case XmlNodeType.Element:
							if (docPassPending != 0)
							{
								PassEndElement(docReader, docPassPending);
								docPassPending = 0;
							}
							string localName = diffReader.LocalName;
							if (localName == "node")
							{
								// Pull the match value from the Xml document
								int newDiffMatch = XmlConvert.ToInt32(diffReader.GetAttribute("match"));

								// Increment our level
								++currentLevel;
								int lastDiffMatch = 0;

								// Manage the stack to reflect the current level and value.
								// We keep the stack as small as possible by never having
								// more than one record on the stack for a level with the same parent.
								DiffMatchRecord lastRecord = diffMatchStack.Pop();
								if (currentLevel < lastRecord.Level)
								{
									do
									{
										lastRecord = diffMatchStack.Pop();
									} while (currentLevel < lastRecord.Level);
								}
								if (lastRecord.Level == currentLevel)
								{
									// Update the value for this level and repush
									lastDiffMatch = lastRecord.Match;
									lastRecord.Match = newDiffMatch;
									diffMatchStack.Push(lastRecord);
								}
								else
								{
									// We popped one too many, push it back on the
									// stack and add a new entry for this level
									diffMatchStack.Push(lastRecord);
									diffMatchStack.Push(new DiffMatchRecord(currentLevel, newDiffMatch));
								}

								// Advance docReader the specified number of elements
								int stopDocAfter = newDiffMatch - lastDiffMatch;
								while (stopDocAfter != 0 && docReader.Read())
								{
									XmlNodeType docNodeType = docReader.NodeType;
									switch (docNodeType)
									{
										case XmlNodeType.XmlDeclaration:
											--stopDocAfter;
											break;
										case XmlNodeType.Element:
											if (0 != --stopDocAfter)
											{
												PassEndElement(docReader);
											}
											break;
									}
								}

								if (diffReader.IsEmptyElement)
								{
									// Treat this just like an end element
									--currentLevel;
									++docPassPending;
								}
							}
							else if (localName == "change")
							{
								// Note that this is the only part of this
								// routine that is id-specific. The bulk of the
								// routine could be moved to C:\Documents and Settings\MCurland\My Documents\Visual Studio 2005\Projects\ORMPackage\ORMModel\ObjectModel\ORMCore.dsldma helper function, and
								// this could be a delegate callback
								string attr = diffReader.GetAttribute("match");
								bool readString = false;
								if (attr != null && attr == "@id")
								{
									string newId = diffReader.ReadString();
									readString = true;
									if (!knownIds.ContainsKey(newId))
									{
										string originalId = docReader.GetAttribute("id");
										if (originalId != null && originalId.Length != 0)
										{
											if (retVal == null)
											{
												retVal = new StringMapDictionary();
											}
											retVal[newId] = originalId;
										}
									}
								}
								if (!readString)
								{
									// ReadString causes the reader to advance, don't advance it again.
									PassEndElement(diffReader);
								}
							}
							else
							{
								PassEndElement(diffReader);
							}
							break;
						case XmlNodeType.EndElement:
							if (diffReader.LocalName == "node")
							{
								--currentLevel;
								++docPassPending;
							}
							break;
					}
				}
				return retVal;
			}
			void IORMToolTestServices.LogValidationErrors(string referenceName)
			{
				ICollection<IORMToolTaskItem> tasks = null;
				int taskCount = 0;
				string[] taskStrings = null;
				ORMTaskProvider taskProvider = (ORMTaskProvider)myTaskProvider;
				if (taskProvider != null)
				{
					tasks = taskProvider.TaskItems;
					taskCount = tasks.Count;
				}
				if (taskCount != 0)
				{
					taskStrings = new string[taskCount];
					int i = 0;
					foreach (IORMToolTaskItem taskItem in tasks)
					{
						taskStrings[i] = taskItem.Text;
						++i;
					}
					Array.Sort<string>(taskStrings);
				}
				bool hasRefName = referenceName != null && referenceName.Length != 0;
				if (hasRefName || taskStrings != null)
				{
					XmlWriter writer = myReportWriter;
					writer.WriteStartElement("ValidationErrors");
					if (hasRefName)
					{
						writer.WriteAttributeString("name", referenceName);
					}
					if (taskStrings != null)
					{
						for (int i = 0; i < taskCount; ++i)
						{
							writer.WriteElementString("ValidationError", taskStrings[i]);
						}
					}
					writer.WriteEndElement();
				}
			}
			void IORMToolTestServices.LogMessage(string message)
			{
				myReportWriter.WriteElementString("Message", message);
			}
			void IORMToolTestServices.LogException(Exception exception)
			{
				LogException(myReportWriter, exception);
			}

			private static void LogException(XmlWriter writer, Exception exception)
			{
				writer.WriteStartElement("Exception");
				writer.WriteAttributeString("type", exception.GetType().FullName);
				writer.WriteAttributeString("message", exception.Message);
				Exception nextException = exception.InnerException;
				if (nextException != null)
				{
					LogException(writer, nextException);
				}
				writer.WriteEndElement();
			}
			private MemoryStream myReportStream;
			private XmlWriter myReportWriter;
			void IORMToolTestServices.OpenReport()
			{
				if (myTaskProvider != null)
				{
					myTaskProvider.RemoveAllTasks();
				}
				if (myReportStream == null)
				{
					myReportStream = new MemoryStream();
				}
				else
				{
					myReportStream.Seek(0, SeekOrigin.Begin);
				}
				CloseReportWriter();
				XmlWriter writer = XmlTextWriter.Create(myReportStream, DetachableWriterSettings);
				writer.WriteStartDocument();
				writer.WriteStartElement("TestReport", "http://schemas.neumont.edu/ORM/SDK/TestReport");
				myReportWriter = writer;
			}
			private void CloseReportWriter()
			{
				if (myReportWriter != null)
				{
					((IDisposable)myReportWriter).Dispose();
					myReportWriter = null;
				}
			}
			XmlReader IORMToolTestServices.CloseReport(MethodInfo testMethod)
			{

				myReportWriter.WriteEndDocument();
				CloseReportWriter();
				// Make sure the stream length corresponds to the current position
				// before reading.
				myReportStream.SetLength(myReportStream.Position);
				myReportStream.Seek(0, SeekOrigin.Begin);
				return XmlTextReader.Create(myReportStream, DetachableReaderSettings);
			}
			#endregion // IORMToolTestServices Implementation
			#region IORMToolTestSuiteReportFactory Implementation
			private class ORMToolTestSuiteReport : IORMToolTestSuiteReport
			{
				#region Member Variables
				const int TestAssemblyLevel = 1;
				const int TestClassLevel = 2;
				private XmlWriter myWriter;
				private string myPendingTestAssembly;
				private int myOpenElements;
				private ORMSuiteReportResult myResult;
				#endregion // Member Variables
				#region Constructor
				public ORMToolTestSuiteReport(XmlWriter writer)
				{
					myWriter = writer;
					writer.WriteStartDocument();
					writer.WriteStartElement("Suites", "http://schemas.neumont.edu/ORM/SDK/TestSuiteReport");
				}
				#endregion // Constructor
				#region Console interaction
				private void WriteProgressMessage(string message, bool fail)
				{
					if (fail)
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}
					Console.WriteLine(message);
					if (fail)
					{
						Console.ResetColor();
					}
				}
				#endregion // Console interaction
				#region IORMToolTestSuiteReport Implementation
				private void CloseOpenElements(int leave)
				{
					int total = myOpenElements;
					if (myOpenElements > leave)
					{
						myOpenElements = leave;
					}
					for (int i = leave; i < total; ++i)
					{
						myWriter.WriteEndElement();
					}
				}
				void IORMToolTestSuiteReport.BeginSuite(string suiteName)
				{
					CloseOpenElements(0);
					++myOpenElements;
					myWriter.WriteStartElement("Suite");
					myWriter.WriteAttributeString("name", suiteName);
					WriteProgressMessage(string.Format("Beginning suite '{0}'", suiteName), false);
				}
				void IORMToolTestSuiteReport.BeginTestAssembly(string path, bool loadFailure)
				{
					CloseOpenElements(TestAssemblyLevel);
					myPendingTestAssembly = path;
					if (loadFailure)
					{
						myResult |= ORMSuiteReportResult.AssemblyLoadFailure;
						myPendingTestAssembly = path;
						WritePendingTestAssembly(true);
						myPendingTestAssembly = null;
						myWriter.WriteAttributeString("loadFailure", "true");
						myWriter.WriteEndElement();
						--myOpenElements;
					}
				}
				private void WritePendingTestAssembly(bool forLoadFailure)
				{
					if (myPendingTestAssembly != null)
					{
						string path = myPendingTestAssembly;
						myPendingTestAssembly = null;
						myWriter.WriteStartElement("TestAssembly");
						myWriter.WriteAttributeString("location", path);
						++myOpenElements;
						WriteProgressMessage(string.Format(forLoadFailure ? "  Assembly '{0}' failed to load" : "  Running tests in assembly '{0}'", path), forLoadFailure);
					}
				}
				void IORMToolTestSuiteReport.BeginTestClass(string testClassNamespace, string testClassName, bool loadFailure)
				{
					WritePendingTestAssembly(false);
					CloseOpenElements(TestClassLevel);
					myWriter.WriteStartElement("TestClass");
					myWriter.WriteAttributeString("namespace", testClassNamespace);
					myWriter.WriteAttributeString("name", testClassName);
					string progressFormat;
					if (loadFailure)
					{
						myResult |= ORMSuiteReportResult.ClassLoadFailure;
						myWriter.WriteAttributeString("loadFailure", "true");
						myWriter.WriteEndElement();
						progressFormat = "    Test class '{0}.{1}' could not be created";
					}
					else
					{
						++myOpenElements;
						progressFormat = "    Running tests in class '{0}.{1}'";
					}
					WriteProgressMessage(string.Format(progressFormat, testClassNamespace, testClassName), loadFailure);
				}
				void IORMToolTestSuiteReport.ReportTestResults(string testMethodName, ORMTestResult result, XmlReader report)
				{
					myWriter.WriteStartElement("Test");
					myWriter.WriteAttributeString("name", testMethodName);
					string resultString = null;
					bool fail = true;
					switch (result)
					{
						case ORMTestResult.Pass:
							resultString = "pass";
							fail = false;
							break;
						case ORMTestResult.FailBind:
							resultString = "failBind";
							break;
						case ORMTestResult.FailReportBaseline:
							resultString = "failReportMissingBaseline";
							break;
						case ORMTestResult.FailReportDiffgram:
							resultString = "failReportDiffgram";
							break;
						default:
							Debug.Assert(false, "Unknown ORMTestResult");
							break;
					}
					if (resultString != null)
					{
						myWriter.WriteAttributeString("result", resultString);
						if (fail)
						{
							myResult |= ORMSuiteReportResult.TestFailure;
						}
						WriteProgressMessage(string.Format(fail ? "      Test '{0}' failed" : "      Test '{0}' passed", testMethodName), fail);
					}
					if (report != null)
					{
						FormatXml(report, myWriter);
					}
					myWriter.WriteEndElement();
				}
				ORMSuiteReportResult IORMToolTestSuiteReport.CloseSuiteReport()
				{
					myOpenElements = 0;
					myWriter.WriteEndDocument();
					return myResult;
				}
				#endregion // IORMToolTestSuiteReport Implementation
			}
			IORMToolTestSuiteReport IORMToolTestSuiteReportFactory.Create(XmlWriter writer)
			{
				return new ORMToolTestSuiteReport(writer);
			}
			#endregion // IORMToolTestSuiteReportFactory Implementation
			#region Model Event Manipulation
			private void AddErrorReportingEvents(Store store)
			{
				DomainDataDirectory dataDirectory = store.DomainDataDirectory;
				ModelingEventManager eventManager = ModelingEventManager.GetModelingEventManager(store);

				DomainClassInfo classInfo = dataDirectory.FindDomainRelationship(ModelHasError.DomainClassId);
				eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(ErrorAddedEvent), EventHandlerAction.Add);

				classInfo = dataDirectory.FindDomainClass(ModelError.DomainClassId);
				eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(ErrorRemovedEvent), EventHandlerAction.Add);
				eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementPropertyChangedEventArgs>(ErrorChangedEvent), EventHandlerAction.Add);
			}

			private void ErrorAddedEvent(object sender, ElementAddedEventArgs e)
			{
				ModelError.AddToTaskProvider(e.ModelElement as ModelHasError);
			}
			private void ErrorRemovedEvent(object sender, ElementDeletedEventArgs e)
			{
				ModelError error = e.ModelElement as ModelError;
				IORMToolTaskItem taskData = error.TaskData as IORMToolTaskItem;
				if (taskData != null)
				{
					error.TaskData = null;
					(this as IORMToolServices).TaskProvider.RemoveTask(taskData);
				}
			}
			private void ErrorChangedEvent(object sender, ElementPropertyChangedEventArgs e)
			{
				ModelError error = e.ModelElement as ModelError;
				IORMToolTaskItem taskData = error.TaskData as IORMToolTaskItem;
				if (taskData != null)
				{
					taskData.Text = error.ErrorText;
				}
			}
			#endregion //Model Event Manipulation
		}
	}
}
