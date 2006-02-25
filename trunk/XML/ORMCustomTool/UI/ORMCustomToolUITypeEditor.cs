using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio.VirtualTreeGrid;

namespace Neumont.Tools.ORM.ORMCustomTool
{
	public sealed partial class ORMCustomTool
	{
		private sealed partial class ORMCustomToolPropertyDescriptor : PropertyDescriptor
		{
			private sealed class ORMCustomToolUITypeEditor : UITypeEditor
			{
				public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
				{
					return UITypeEditorEditStyle.Modal;
				}

				public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
				{
					IWindowsFormsEditorService windowsFormsEditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if (windowsFormsEditorService != null)
					{
						/*ORMCustomTool ormCustomTool = ORMCustomTool.Singleton;
						if (ormCustomTool != null)
						{
							EnvDTE.ProjectItem projectItem = ormCustomTool.GetService<EnvDTE.ProjectItem>();
							Project project = Engine.GlobalEngine.GetLoadedProject(projectItem.ContainingProject.FullName);
							BuildItemGroup buildItemGroup = ORMCustomTool.GetBuildItemGroup(project, projectItem.Name);
							windowsFormsEditorService.ShowDialog(new ORMGeneratorSelectionControl(ormCustomTool, project, buildItemGroup));
						}*/
					}
					return null;
				}

				public override bool GetPaintValueSupported(ITypeDescriptorContext context)
				{
					return false;
				}

				public override void PaintValue(PaintValueEventArgs e)
				{
					// Don't do anything, since we don't have a visual representation.
				}
			}
		}
	}
}
