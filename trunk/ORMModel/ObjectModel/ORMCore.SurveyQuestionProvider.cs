﻿using System;
using Neumont.Tools.ORM.Framework.DynamicSurveyTreeGrid;
namespace Neumont.Tools.ORM.ObjectModel
{
	public partial class ORMCoreModel : ISurveyQuestionProvider
	{
		private static readonly ISurveyQuestionTypeInfo[] SurveyQuestionTypeInfo = new ISurveyQuestionTypeInfo[]{
			ProvideSurveyQuestionForElementType.Instance,
			ProvideSurveyQuestionForErrorState.Instance};
		/// <summary>
		/// Returns an array of ISurveyQuestionTypeInfo representing the questions that can be asked of objects in this DomainModel
		/// </summary>
		protected static ISurveyQuestionTypeInfo[] GetSurveyQuestionTypeInfo()
		{
			return (ISurveyQuestionTypeInfo[])ORMCoreModel.SurveyQuestionTypeInfo.Clone();
		}
		ISurveyQuestionTypeInfo[] ISurveyQuestionProvider.GetSurveyQuestionTypeInfo()
		{
			return GetSurveyQuestionTypeInfo();
		}
		private sealed class ProvideSurveyQuestionForElementType : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForElementType()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForElementType();
			public Type QuestionType
			{
				get
				{
					return typeof(ElementType);
				}
			}
			public int AskQuestion(object data)
			{
				IAnswerSurveyQuestion<ElementType> typedData = data as IAnswerSurveyQuestion<ElementType>;
				if (typedData != null)
				{
					return typedData.AskQuestion();
				}
				else
				{
					return -1;
				}
			}
		}
		private sealed class ProvideSurveyQuestionForErrorState : ISurveyQuestionTypeInfo
		{
			private ProvideSurveyQuestionForErrorState()
			{
			}
			public static readonly ISurveyQuestionTypeInfo Instance = new ProvideSurveyQuestionForErrorState();
			public Type QuestionType
			{
				get
				{
					return typeof(ErrorState);
				}
			}
			public int AskQuestion(object data)
			{
				IAnswerSurveyQuestion<ErrorState> typedData = data as IAnswerSurveyQuestion<ErrorState>;
				if (typedData != null)
				{
					return typedData.AskQuestion();
				}
				else
				{
					return -1;
				}
			}
		}
	}
}
