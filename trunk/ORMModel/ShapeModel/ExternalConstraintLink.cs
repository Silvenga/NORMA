using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Diagrams.GraphObject;
using Northface.Tools.ORM.ObjectModel;
namespace Northface.Tools.ORM.ShapeModel
{
	public partial class ExternalConstraintLink
	{
		#region Customize appearance
		/// <summary>
		/// Override the connection line pen with a dashed pen
		/// </summary>
		/// <param name="classStyleSet"></param>
		protected override void InitializeResources(StyleSet classStyleSet)
		{
			PenSettings settings = new PenSettings();
			settings.Color = Color.Violet;
			settings.DashStyle = DashStyle.Dash;
			settings.Width = 1.0F/72.0F; // 1 Point. 0 Means 1 pixel, but should only be used for non-printed items
			classStyleSet.OverridePen(DiagramPens.ConnectionLine, settings);
		}
		/// <summary>
		/// Use a straight line routing style
		/// </summary>
		[CLSCompliant(false)]
		protected override VGRoutingStyle DefaultRoutingStyle
		{
			get
			{
				return VGRoutingStyle.VGRouteStraight; // VGRouteCenterToCenter;
			}
		}
		/// <summary>
		/// Selecting external constraint links gets in the way of selecting other primary
		/// objects. It is best just to turn them off. This also eliminates a bunch of unnamed
		/// constraint links from the property grid element picker.
		/// </summary>
		public override bool CanSelect
		{
			get
			{
				return false;
			}
		}
		#endregion // Customize appearance
		#region ExternalConstraintLink specific
		/// <summary>
		/// Stop the user from manually routine link lines
		/// </summary>
		/// <value>false</value>
		public override bool CanManuallyRoute
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Get the FactConstraint link associated with this link shape. The
		/// fact constraint link can be used to get the associated roles.
		/// </summary>
		public ExternalFactConstraint AssociatedFactConstraint
		{
			get
			{
				return ModelElement as ExternalFactConstraint;
			}
		}
		#endregion // ExternalConstraintLink specific
	}
}