﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Neumont.Tools.Modeling.Shell.DynamicSurveyTreeGrid;
namespace Neumont.Tools.RelationalModels.ConceptualDatabase
{
	partial class ConceptualDatabaseDomainModel : ISurveyQuestionProvider
	{
		private static readonly ISurveyQuestionTypeInfo[] mySurveyQuestionTypeInfo1 = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForSurveySchemaType.Instance};
		private static readonly ISurveyQuestionTypeInfo[] mySurveyQuestionTypeInfo2 = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForSurveySchemaChildType.Instance};
		private static readonly ISurveyQuestionTypeInfo[] mySurveyQuestionTypeInfo3 = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForSurveyTableChildType.Instance,
			ProvideSurveyQuestionForSurveyTableChildGlyphType.Instance,
			ProvideSurveyQuestionForSurveyColumnClassificationType.Instance};
		private static readonly ISurveyQuestionTypeInfo[] mySurveyQuestionTypeInfo4 = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForSurveyReferenceConstraintChildType.Instance};
		private static readonly ISurveyQuestionTypeInfo[] mySurveyQuestionTypeInfo5 = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForSurveyColumnReferenceChildType.Instance};
		private static readonly ISurveyQuestionTypeInfo[] mySurveyQuestionTypeInfo6 = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForSurveyUniquenessConstraintChildType.Instance};
		/// <summary>Implements <see cref="ISurveyQuestionProvider.GetSurveyQuestions"/></summary>
		protected static IEnumerable<ISurveyQuestionTypeInfo> GetSurveyQuestions(object expansionKey)
		{
			if (expansionKey == null)
			{
				return mySurveyQuestionTypeInfo1;
			}
			else if (expansionKey == Schema.SurveyExpansionKey)
			{
				return mySurveyQuestionTypeInfo2;
			}
			else if (expansionKey == Table.SurveyExpansionKey)
			{
				return mySurveyQuestionTypeInfo3;
			}
			else if (expansionKey == ReferenceConstraint.SurveyExpansionKey)
			{
				return mySurveyQuestionTypeInfo4;
			}
			else if (expansionKey == ColumnReference.SurveyExpansionKey)
			{
				return mySurveyQuestionTypeInfo5;
			}
			else if (expansionKey == UniquenessConstraint.SurveyExpansionKey)
			{
				return mySurveyQuestionTypeInfo6;
			}
			return null;
		}
		IEnumerable<ISurveyQuestionTypeInfo> ISurveyQuestionProvider.GetSurveyQuestions(object expansionKey)
		{
			return GetSurveyQuestions(expansionKey);
		}
		/// <summary>Implements <see cref="ISurveyQuestionProvider.SurveyQuestionImageList"/></summary>
		protected ImageList SurveyQuestionImageList
		{
			get
			{
				return Resources.SurveyTreeImageList;
			}
		}
		ImageList ISurveyQuestionProvider.SurveyQuestionImageList
		{
			get
			{
				return this.SurveyQuestionImageList;
			}
		}
		/// <summary>Implements <see cref="ISurveyQuestionProvider.GetErrorDisplayTypes"/></summary>
		protected static IEnumerable<Type> GetErrorDisplayTypes()
		{
			return null;
		}
		IEnumerable<Type> ISurveyQuestionProvider.GetErrorDisplayTypes()
		{
			return GetErrorDisplayTypes();
		}
		private sealed class ProvideSurveyQuestionForSurveySchemaType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveySchemaType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveySchemaType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveySchemaType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveySchemaType> typedData = data as IAnswerSurveyQuestion<SurveySchemaType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return answer;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Grouping | SurveyQuestionUISupport.Sorting | SurveyQuestionUISupport.Glyph;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 200;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveySchemaChildType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveySchemaChildType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveySchemaChildType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveySchemaChildType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveySchemaChildType> typedData = data as IAnswerSurveyQuestion<SurveySchemaChildType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return (int)SurveySchemaType.Last + 1 + answer;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Grouping | SurveyQuestionUISupport.Sorting | SurveyQuestionUISupport.Glyph;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveyTableChildType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveyTableChildType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveyTableChildType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveyTableChildType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveyTableChildType> typedData = data as IAnswerSurveyQuestion<SurveyTableChildType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return -1;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Grouping | SurveyQuestionUISupport.Sorting;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveyTableChildGlyphType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveyTableChildGlyphType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveyTableChildGlyphType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveyTableChildGlyphType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveyTableChildGlyphType> typedData = data as IAnswerSurveyQuestion<SurveyTableChildGlyphType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return (int)SurveySchemaType.Last + 1 + (int)SurveySchemaChildType.Last + 1 + answer;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Glyph;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveyColumnClassificationType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveyColumnClassificationType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveyColumnClassificationType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveyColumnClassificationType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveyColumnClassificationType> typedData = data as IAnswerSurveyQuestion<SurveyColumnClassificationType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				int retVal;
				switch ((SurveyColumnClassificationType)answer)
				{
					case SurveyColumnClassificationType.PrimaryRequired:
					case SurveyColumnClassificationType.PrimaryNullable:
						retVal = (int)SurveySchemaType.Last + 1 + (int)SurveySchemaChildType.Last + 1 + (int)SurveyTableChildGlyphType.Last + 1 + (int)SurveyReferenceConstraintChildType.Last + 1 + (int)SurveyUniquenessConstraintChildType.Last + 1 + 0;
						break;
					default:
						retVal = -1;
						break;
				}
				return retVal;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				switch ((SurveyColumnClassificationType)answer)
				{
					case SurveyColumnClassificationType.Required:
					case SurveyColumnClassificationType.PrimaryRequired:
						return new SurveyQuestionDisplayData(true, false);
				}
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Overlay | SurveyQuestionUISupport.DisplayData;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveyReferenceConstraintChildType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveyReferenceConstraintChildType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveyReferenceConstraintChildType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveyReferenceConstraintChildType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveyReferenceConstraintChildType> typedData = data as IAnswerSurveyQuestion<SurveyReferenceConstraintChildType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return (int)SurveySchemaType.Last + 1 + (int)SurveySchemaChildType.Last + 1 + (int)SurveyTableChildGlyphType.Last + 1 + answer;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Sorting | SurveyQuestionUISupport.Glyph;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveyColumnReferenceChildType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveyColumnReferenceChildType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveyColumnReferenceChildType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveyColumnReferenceChildType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveyColumnReferenceChildType> typedData = data as IAnswerSurveyQuestion<SurveyColumnReferenceChildType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return -1;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Sorting;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForSurveyUniquenessConstraintChildType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForSurveyUniquenessConstraintChildType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForSurveyUniquenessConstraintChildType();
			public Type QuestionType
			{
				get
				{
					return typeof(SurveyUniquenessConstraintChildType);
				}
			}
			public ISurveyDynamicValues DynamicQuestionValues
			{
				get
				{
					return null;
				}
			}
			public int AskQuestion(object data, object contextElement)
			{
				IAnswerSurveyQuestion<SurveyUniquenessConstraintChildType> typedData = data as IAnswerSurveyQuestion<SurveyUniquenessConstraintChildType>;
				if (typedData != null)
				{
					return typedData.AskQuestion(contextElement);
				}
				return -1;
			}
			public int MapAnswerToImageIndex(int answer)
			{
				return (int)SurveySchemaType.Last + 1 + (int)SurveySchemaChildType.Last + 1 + (int)SurveyTableChildGlyphType.Last + 1 + (int)SurveyReferenceConstraintChildType.Last + 1 + answer;
			}
			public SurveyQuestionDisplayData GetDisplayData(int answer)
			{
				return SurveyQuestionDisplayData.Default;
			}
			public SurveyQuestionUISupport UISupport
			{
				get
				{
					return SurveyQuestionUISupport.Sorting | SurveyQuestionUISupport.Glyph;
				}
			}
			public static int QuestionPriority
			{
				get
				{
					return 0;
				}
			}
			int ISurveyQuestionTypeInfo.QuestionPriority
			{
				get
				{
					return QuestionPriority;
				}
			}
		}
	}
}
