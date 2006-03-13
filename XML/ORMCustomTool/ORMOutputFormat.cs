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

namespace Neumont.Tools.ORM.ORMCustomTool
{
	/// <summary>
	/// The non-localized, official names of several predefined output formats.
	/// </summary>
	public static class ORMOutputFormat
	{
		/// <summary>
		/// ORM - Object-Role Modeling Output Format
		/// </summary>
		public const string ORM = "ORM";
		/// <summary>
		/// OIAL - ORM Intermediate Abstraction Language Output Format
		/// </summary>
		public const string OIAL = "OIAL";
		/// <summary>
		/// DCIL - Database Conceptual Intermediate Language Output Format
		/// </summary>
		public const string DCIL = "DCIL";
		/// <summary>
		/// PLiX - Programming Language in XML Output Format
		/// </summary>
		public const string PLiX = "PLiX";
	}
}
