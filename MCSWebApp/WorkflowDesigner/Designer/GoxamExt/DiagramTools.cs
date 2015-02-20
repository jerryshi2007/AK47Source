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
using Northwoods.GoXam;
using Designer.Views;
using System.Windows.Browser;
using Designer.Models;

namespace Designer
{
	[Obsolete]
	public class DiagramTools : DiagramTool
	{
		//public DiagramMouseDownTool()
		//{
		//    this.Web = new WebInterAction();
		//}

		//public IWebInterAction Web;

		public override bool CanStart()
		{
			if (!base.CanStart()) return false;

			Northwoods.GoXam.Diagram diagram = this.Diagram;

			// heed Diagram.AllowSelect
			if (diagram == null) return false;

			// any mouse button will do, not just the left button
			return true;
		}

		public override void DoMouseDown()
		{
			if (this.Active)
			{
				new WebInterAction().LoadProperty(WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW,
					this.Diagram.Tag.ToString(),
					WorkflowUtils.ExtractWorkflowInfoJson(this.Diagram));

				var form = this.Diagram.PartsModel.FindNodeByKey("menuNode");
				if (form == null) return;

				form.Visible = false;
			}

			// all done!
			StopTool();
		}
	}

	public class WfToolManager : ToolManager
	{
		public override void DoMouseDown()
		{
			base.DoMouseDown();

			Diagram diagram = this.Diagram;
			if (diagram == null) return;

			Part currentPart = FindPartAt(diagram.LastMousePointInModel, true);
			if (currentPart == null)
			{
				var form = this.Diagram.PartsModel.FindNodeByKey(DiagramPage.ACTCONTEXT_MENU);
				if (form != null) form.Visible = false;

				form = this.Diagram.PartsModel.FindNodeByKey(DiagramPage.WFCONTEXT_MENU);
				if (form != null) form.Visible = false;

				if (string.Compare(diagram.Tag.ToString(), WorkflowUtils.CurrentKey) != 0)
				{
					HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_WORKFLOW, diagram.Tag.ToString(), WorkflowUtils.ExtractWorkflowInfoJson(diagram));
					WorkflowUtils.CurrentKey = diagram.Tag.ToString();
				}
			}
			else
			{
				if (currentPart.DataContext is Northwoods.GoXam.PartManager.PartBinding)
				{
					Northwoods.GoXam.PartManager.PartBinding currentBinding = (Northwoods.GoXam.PartManager.PartBinding)((currentPart as FrameworkElement).DataContext);

					if (currentBinding.Data == null)
						return;
					if (currentBinding.Data is ActivityNode)
					{
						ActivityNode nodeData = currentBinding.Data as ActivityNode;
						if (nodeData == null)
							return;

						string strKey = string.Format("{0}@{1}", diagram.Tag.ToString(), nodeData.Key);
						if (string.Compare(strKey, WorkflowUtils.CurrentKey) != 0)
						{
							HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY, diagram.Tag.ToString(), WorkflowUtils.ExtractActivityInfoJson(nodeData));
							WorkflowUtils.CurrentKey = strKey;
						}
					}
				}
			}
		}

		private bool IsBlankArea(Diagram diagram)
		{
			Part currentPart = FindPartAt(diagram.LastMousePointInModel, true);

			if (currentPart == null) return true;
			return false;
		}
	}
}
