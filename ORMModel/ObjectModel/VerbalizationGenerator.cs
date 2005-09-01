﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50215.44
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Neumont.Tools.ORM.ObjectModel
{
	using System;
	using System.Text;
	using System.Globalization;
	
	/// <summary>
	///</summary>
	public enum VerbalizationTextSnippetType
	{
		/// <summary>
		///</summary>
		AtMostOneQuantifier,
		/// <summary>
		///</summary>
		EachInstanceQuantifier,
		/// <summary>
		///</summary>
		ExistentialQuantifier,
		/// <summary>
		///</summary>
		ForEachCompactQuantifier,
		/// <summary>
		///</summary>
		ForEachQuantifier,
		/// <summary>
		///</summary>
		IdentityReferenceQuantifier,
		/// <summary>
		///</summary>
		ImpersonalPronoun,
		/// <summary>
		///</summary>
		IndentedListClose,
		/// <summary>
		///</summary>
		IndentedListFinalSeparator,
		/// <summary>
		///</summary>
		IndentedListOpen,
		/// <summary>
		///</summary>
		IndentedListPairSeparator,
		/// <summary>
		///</summary>
		IndentedListSeparator,
		/// <summary>
		///</summary>
		ModalNecessityOperator,
		/// <summary>
		///</summary>
		ModalPossibilityOperator,
		/// <summary>
		///</summary>
		MoreThanOneQuantifier,
		/// <summary>
		///</summary>
		SimpleListClose,
		/// <summary>
		///</summary>
		SimpleListFinalSeparator,
		/// <summary>
		///</summary>
		SimpleListOpen,
		/// <summary>
		///</summary>
		SimpleListPairSeparator,
		/// <summary>
		///</summary>
		SimpleListSeparator,
		/// <summary>
		///</summary>
		UniversalQuantifier,
	}
	/// <summary>
	///</summary>
	public struct VerbalizationSet
	{
		private string[] mySnippets;
		/// <summary>
		///</summary>
		public VerbalizationSet(string[] snippets)
		{
			this.mySnippets = snippets;
		}
		/// <summary>
		///</summary>
		public string GetSnippet(VerbalizationTextSnippetType snippetType)
		{
			return this.mySnippets[((int)(snippetType))];
		}
	}
	/// <summary>
	///</summary>
	public class VerbalizationSets
	{
		/// <summary>
		///</summary>
		public static VerbalizationSets Default = VerbalizationSets.CreateDefaultVerbalizationSets();
		private VerbalizationSet[] mySets;
		private VerbalizationSets()
		{
		}
		/// <summary>
		///</summary>
		public string GetSnippet(VerbalizationTextSnippetType snippetType, bool isDeontic, bool isNegative)
		{
			int setIndex = 0;
			if (isDeontic)
			{
				setIndex = (setIndex + 1);
			}
			if (isNegative)
			{
				setIndex = (setIndex + 2);
			}
			return this.mySets[setIndex].GetSnippet(snippetType);
		}
		private static VerbalizationSets CreateDefaultVerbalizationSets()
		{
			VerbalizationSets retVal = new VerbalizationSets();
			retVal.mySets = new VerbalizationSet[] {
					new VerbalizationSet(new string[] {
								"at most one {0}",
								"each instance of {0} occurs only once",
								"some {0}",
								"for each {0} {1}",
								"for each {0},\r\n\t{1}",
								"the same {0}",
								"that {0}",
								"",
								" and\r\n\t",
								"\r\n\t",
								" and\r\n\t",
								" and\r\n\t",
								"it is necessary that {0}",
								"it is possible that {0}",
								"more than one {0}",
								"",
								", and ",
								"",
								" and ",
								", ",
								"each {0}"}),
					new VerbalizationSet(new string[] {
								"at most one {0}",
								"each instance of {0} occurs only once",
								"some {0}",
								"for each {0} {1}",
								"for each {0},\r\n\t{1}",
								"the same {0}",
								"that {0}",
								"",
								" and\r\n\t",
								"\r\n\t",
								" and\r\n\t",
								" and\r\n\t",
								"it is obligatory that {0}",
								"it is permitted that {0}",
								"more than one {0}",
								"",
								", and ",
								"",
								" and ",
								", ",
								"each {0}"}),
					new VerbalizationSet(new string[] {
								"at most one {0}",
								"each instance of {0} occurs only once",
								"some {0}",
								"for each {0} {1}",
								"for each {0},\r\n\t{1}",
								"the same {0}",
								"that {0}",
								"",
								" and\r\n\t",
								"\r\n\t",
								" and\r\n\t",
								" and\r\n\t",
								"it is necessary that {0}",
								"it is impossible that {0}",
								"more than one {0}",
								"",
								", and ",
								"",
								" and ",
								", ",
								"each {0}"}),
					new VerbalizationSet(new string[] {
								"at most one {0}",
								"each instance of {0} occurs only once",
								"some {0}",
								"for each {0} {1}",
								"for each {0},\r\n\t{1}",
								"the same {0}",
								"that {0}",
								"",
								" and\r\n\t",
								"\r\n\t",
								" and\r\n\t",
								" and\r\n\t",
								"it is obligatory that {0}",
								"it is forbidden that {0}",
								"more than one {0}",
								"",
								", and ",
								"",
								" and ",
								", ",
								"each {0}"})};
			return retVal;
		}
	}
	/// <summary>
	///</summary>
	public partial class InternalUniquenessConstraint : IVerbalize
	{
		/// <summary>
		///</summary>
		protected string GetVerbalization(bool isNegative)
		{
			StringBuilder sbMain = null;
			StringBuilder sbTemp = null;
			IModelErrorOwner errorOwner = (this) as IModelErrorOwner;
			if ((errorOwner != null))
			{
				foreach (ModelError error in errorOwner.ErrorCollection)
				{
					if ((sbMain == null))
					{
						sbMain = new StringBuilder();
					}
					else
					{
						sbMain.AppendLine();
					}
					sbMain.Append(error.Name);
				}
				if ((sbMain != null))
				{
					return sbMain.ToString();
				}
			}
			VerbalizationSets snippets = VerbalizationSets.Default;
			bool isDeontic = false;
			FactType parentFact = this.FactType;
			RoleMoveableCollection includedRoles = this.RoleCollection;
			RoleMoveableCollection allRoles = parentFact.RoleCollection;
			ReadingOrderMoveableCollection allReadingOrders = parentFact.ReadingOrderCollection;
			if ((allReadingOrders.Count == 0))
			{
				return "";
			}
			int includedArity = includedRoles.Count;
			int fullArity = allRoles.Count;
			string[] basicRoleReplacements = new string[fullArity];
			string[] roleReplacements = new string[fullArity];
			sbMain = new StringBuilder();
			ReadingOrder readingOrder;
			int i = 0;
			for (; (i < fullArity); i = (i + 1))
			{
				ObjectType rolePlayer = allRoles[i].RolePlayer;
				string basicReplacement;
				if ((rolePlayer != null))
				{
					basicReplacement = rolePlayer.Name;
				}
				else
				{
					basicReplacement = ("Role" + ((i + 1)).ToString(CultureInfo.CurrentUICulture));
				}
				basicRoleReplacements[i] = basicReplacement;
			}
			if (((fullArity == includedArity) 
						&& (fullArity == 2)))
			{
				string snippet1 = snippets.GetSnippet(VerbalizationTextSnippetType.EachInstanceQuantifier, isDeontic, isNegative);
				string snippet1replace1 = null;
				readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, allRoles[0], null, allRoles, true);
				snippet1replace1 = FactType.PopulatePredicateText(readingOrder, allRoles, basicRoleReplacements);
				sbMain.AppendFormat(CultureInfo.CurrentUICulture, snippet1, snippet1replace1);
				sbMain.AppendLine();
				string snippet2 = snippets.GetSnippet(VerbalizationTextSnippetType.ModalPossibilityOperator, isDeontic, isNegative);
				string snippet2replace1 = null;
				if ((sbTemp == null))
				{
					sbTemp = new StringBuilder();
				}
				else
				{
					sbTemp.Length = 0;
				}
				int snippet2replaceroleIter1 = 0;
				for (; (snippet2replaceroleIter1 < fullArity); snippet2replaceroleIter1 = (snippet2replaceroleIter1 + 1))
				{
					Role primaryRole = allRoles[snippet2replaceroleIter1];
					VerbalizationTextSnippetType listSnippet;
					if ((snippet2replaceroleIter1 == 0))
					{
						listSnippet = VerbalizationTextSnippetType.IndentedListOpen;
					}
					else
					{
						if ((snippet2replaceroleIter1 
									== (fullArity - 1)))
						{
							if ((snippet2replaceroleIter1 == 1))
							{
								listSnippet = VerbalizationTextSnippetType.IndentedListPairSeparator;
							}
							else
							{
								listSnippet = VerbalizationTextSnippetType.IndentedListFinalSeparator;
							}
						}
						else
						{
							listSnippet = VerbalizationTextSnippetType.IndentedListSeparator;
						}
					}
					sbTemp.Append(snippets.GetSnippet(listSnippet, isDeontic, isNegative));
					snippet2replace1 = null;
					readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, primaryRole, null, allRoles, true);
					int snippet2replacefactRoleIter1 = 0;
					for (; (snippet2replacefactRoleIter1 < fullArity); snippet2replacefactRoleIter1 = (snippet2replacefactRoleIter1 + 1))
					{
						Role currentRole = allRoles[snippet2replacefactRoleIter1];
						string roleReplacement = null;
						string basicReplacement = basicRoleReplacements[snippet2replacefactRoleIter1];
						if ((primaryRole == currentRole))
						{
							roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.IdentityReferenceQuantifier, isDeontic, isNegative), basicReplacement);
						}
						else
						{
							if ((primaryRole != currentRole))
							{
								roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.MoreThanOneQuantifier, isDeontic, isNegative), basicReplacement);
							}
						}
						if ((roleReplacement == null))
						{
							roleReplacement = basicReplacement;
						}
						roleReplacements[snippet2replacefactRoleIter1] = roleReplacement;
					}
					snippet2replace1 = FactType.PopulatePredicateText(readingOrder, allRoles, roleReplacements);
					sbTemp.Append(snippet2replace1);
					if ((snippet2replaceroleIter1 
								== (fullArity - 1)))
					{
						sbTemp.Append(snippets.GetSnippet(VerbalizationTextSnippetType.IndentedListClose, isDeontic, isNegative));
					}
				}
				snippet2replace1 = sbTemp.ToString();
				sbMain.AppendFormat(CultureInfo.CurrentUICulture, snippet2, snippet2replace1);
			}
			else
			{
				if ((fullArity == 2))
				{
					readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, null, includedRoles, allRoles, false);
					if ((readingOrder != null))
					{
						int factTextfactRoleIter1 = 0;
						for (; (factTextfactRoleIter1 < fullArity); factTextfactRoleIter1 = (factTextfactRoleIter1 + 1))
						{
							Role currentRole = allRoles[factTextfactRoleIter1];
							string roleReplacement = null;
							string basicReplacement = basicRoleReplacements[factTextfactRoleIter1];
							if (includedRoles.Contains(currentRole))
							{
								roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.UniversalQuantifier, isDeontic, isNegative), basicReplacement);
							}
							else
							{
								roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.AtMostOneQuantifier, isDeontic, isNegative), basicReplacement);
							}
							if ((roleReplacement == null))
							{
								roleReplacement = basicReplacement;
							}
							roleReplacements[factTextfactRoleIter1] = roleReplacement;
						}
						sbMain.Append(FactType.PopulatePredicateText(readingOrder, allRoles, roleReplacements));
					}
					else
					{
						readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, allRoles[0], null, allRoles, true);
						if ((readingOrder != null))
						{
							string snippet1 = snippets.GetSnippet(VerbalizationTextSnippetType.ForEachCompactQuantifier, isDeontic, isNegative);
							string snippet1replace1 = null;
							if ((sbTemp == null))
							{
								sbTemp = new StringBuilder();
							}
							else
							{
								sbTemp.Length = 0;
							}
							int snippet1replaceroleIter1 = 0;
							for (; (snippet1replaceroleIter1 < includedArity); snippet1replaceroleIter1 = (snippet1replaceroleIter1 + 1))
							{
								Role primaryRole = includedRoles[snippet1replaceroleIter1];
								VerbalizationTextSnippetType listSnippet;
								if ((snippet1replaceroleIter1 == 0))
								{
									listSnippet = VerbalizationTextSnippetType.SimpleListOpen;
								}
								else
								{
									if ((snippet1replaceroleIter1 
												== (includedArity - 1)))
									{
										if ((snippet1replaceroleIter1 == 1))
										{
											listSnippet = VerbalizationTextSnippetType.SimpleListPairSeparator;
										}
										else
										{
											listSnippet = VerbalizationTextSnippetType.SimpleListFinalSeparator;
										}
									}
									else
									{
										listSnippet = VerbalizationTextSnippetType.SimpleListSeparator;
									}
								}
								sbTemp.Append(snippets.GetSnippet(listSnippet, isDeontic, isNegative));
								sbTemp.Append(basicRoleReplacements[allRoles.IndexOf(includedRoles[snippet1replaceroleIter1])]);
								if ((snippet1replaceroleIter1 
											== (includedArity - 1)))
								{
									sbTemp.Append(snippets.GetSnippet(VerbalizationTextSnippetType.SimpleListClose, isDeontic, isNegative));
								}
							}
							snippet1replace1 = sbTemp.ToString();
							string snippet1replace2 = null;
							readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, allRoles[0], null, allRoles, true);
							int snippet1replacefactRoleIter2 = 0;
							for (; (snippet1replacefactRoleIter2 < fullArity); snippet1replacefactRoleIter2 = (snippet1replacefactRoleIter2 + 1))
							{
								Role currentRole = allRoles[snippet1replacefactRoleIter2];
								string roleReplacement = null;
								string basicReplacement = basicRoleReplacements[snippet1replacefactRoleIter2];
								if (includedRoles.Contains(currentRole))
								{
									roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.ImpersonalPronoun, isDeontic, isNegative), basicReplacement);
								}
								else
								{
									roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.AtMostOneQuantifier, isDeontic, isNegative), basicReplacement);
								}
								if ((roleReplacement == null))
								{
									roleReplacement = basicReplacement;
								}
								roleReplacements[snippet1replacefactRoleIter2] = roleReplacement;
							}
							snippet1replace2 = FactType.PopulatePredicateText(readingOrder, allRoles, roleReplacements);
							sbMain.AppendFormat(CultureInfo.CurrentUICulture, snippet1, snippet1replace1, snippet1replace2);
						}
					}
				}
				else
				{
					if ((fullArity == includedArity))
					{
						string snippet1 = snippets.GetSnippet(VerbalizationTextSnippetType.EachInstanceQuantifier, isDeontic, isNegative);
						string snippet1replace1 = null;
						readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, allRoles[0], null, allRoles, true);
						snippet1replace1 = FactType.PopulatePredicateText(readingOrder, allRoles, basicRoleReplacements);
						sbMain.AppendFormat(CultureInfo.CurrentUICulture, snippet1, snippet1replace1);
					}
					else
					{
						readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, null, includedRoles, allRoles, false);
						if ((readingOrder != null))
						{
							string snippet1 = snippets.GetSnippet(VerbalizationTextSnippetType.ForEachQuantifier, isDeontic, isNegative);
							string snippet1replace1 = null;
							if ((sbTemp == null))
							{
								sbTemp = new StringBuilder();
							}
							else
							{
								sbTemp.Length = 0;
							}
							int snippet1replaceroleIter1 = 0;
							for (; (snippet1replaceroleIter1 < includedArity); snippet1replaceroleIter1 = (snippet1replaceroleIter1 + 1))
							{
								Role primaryRole = includedRoles[snippet1replaceroleIter1];
								VerbalizationTextSnippetType listSnippet;
								if ((snippet1replaceroleIter1 == 0))
								{
									listSnippet = VerbalizationTextSnippetType.SimpleListOpen;
								}
								else
								{
									if ((snippet1replaceroleIter1 
												== (includedArity - 1)))
									{
										if ((snippet1replaceroleIter1 == 1))
										{
											listSnippet = VerbalizationTextSnippetType.SimpleListPairSeparator;
										}
										else
										{
											listSnippet = VerbalizationTextSnippetType.SimpleListFinalSeparator;
										}
									}
									else
									{
										listSnippet = VerbalizationTextSnippetType.SimpleListSeparator;
									}
								}
								sbTemp.Append(snippets.GetSnippet(listSnippet, isDeontic, isNegative));
								sbTemp.Append(basicRoleReplacements[allRoles.IndexOf(includedRoles[snippet1replaceroleIter1])]);
								if ((snippet1replaceroleIter1 
											== (includedArity - 1)))
								{
									sbTemp.Append(snippets.GetSnippet(VerbalizationTextSnippetType.SimpleListClose, isDeontic, isNegative));
								}
							}
							snippet1replace1 = sbTemp.ToString();
							string snippet1replace2 = null;
							int snippet1replacefactRoleIter2 = 0;
							for (; (snippet1replacefactRoleIter2 < fullArity); snippet1replacefactRoleIter2 = (snippet1replacefactRoleIter2 + 1))
							{
								Role currentRole = allRoles[snippet1replacefactRoleIter2];
								string roleReplacement = null;
								string basicReplacement = basicRoleReplacements[snippet1replacefactRoleIter2];
								if (includedRoles.Contains(currentRole))
								{
									roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.ImpersonalPronoun, isDeontic, isNegative), basicReplacement);
								}
								else
								{
									roleReplacement = string.Format(CultureInfo.CurrentUICulture, snippets.GetSnippet(VerbalizationTextSnippetType.AtMostOneQuantifier, isDeontic, isNegative), basicReplacement);
								}
								if ((roleReplacement == null))
								{
									roleReplacement = basicReplacement;
								}
								roleReplacements[snippet1replacefactRoleIter2] = roleReplacement;
							}
							snippet1replace2 = FactType.PopulatePredicateText(readingOrder, allRoles, roleReplacements);
							sbMain.AppendFormat(CultureInfo.CurrentUICulture, snippet1, snippet1replace1, snippet1replace2);
						}
						else
						{
							readingOrder = FactType.GetMatchingReadingOrder(allReadingOrders, allRoles[0], null, allRoles, true);
							if ((readingOrder != null))
							{
							}
						}
					}
				}
			}
			return sbMain.ToString();
		}
		string IVerbalize.GetVerbalization(bool isNegative)
		{
			return this.GetVerbalization(isNegative);
		}
	}
}
