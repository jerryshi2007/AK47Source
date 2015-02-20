using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Northwoods.GoXam.Tool;
using Designer.Utils;
using Designer.Models;

namespace Designer
{
	public class WorkflowRelinkingTool : RelinkingTool
	{
		public override void DoDeactivate()
		{
			base.DoDeactivate();
			var link = this.Diagram.SelectedLink;

			ActivityLink linkData = (ActivityLink)link.Data;
			linkData.WfReturnLine = false;

			if (linkData.From == linkData.To || linkData.To == "N0")
			{
				linkData.WfReturnLine = true;
			}

			DiagramUtils.WebInterAct.LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION,
				this.Diagram.Tag.ToString(),
				WorkflowUtils.ExtractTransitionInfoJson(linkData));
		}
	}
}
