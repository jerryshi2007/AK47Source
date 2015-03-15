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
using Designer.Models;
using System.Windows.Browser;
using Designer.Utils;

namespace Designer
{
	public class WorkflowTextEditingTool : TextEditingTool
	{
		public WorkflowTextEditingTool() : this(new WebInterAction()) { }
		public WorkflowTextEditingTool(IWebInterAction client)
			: base()
		{
			_webMethod = client;
		}

		private IWebInterAction _webMethod;

		protected override bool DoAcceptText()
		{
			//先保留一个编辑元素的对象引用
			var part = this.AdornedPart.Data;
			//完成text编辑
			bool result = base.DoAcceptText();

			//将编辑后的元素更新到property grid控件
			var node = part as ActivityNode;
			if (node != null)
			{
				_webMethod.UpdateProcess(WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY,
					this.Diagram.Tag.ToString(), node.Key,
					WorkflowUtils.ExtractActivityInfoJson(node));
				return result;
			}

			var link = part as ActivityLink;
			if (link != null)
			{
				_webMethod.UpdateProcess(WorkflowUtils.CLIENTSCRIPT_PARAM_TRANSITION,
					this.Diagram.Tag.ToString(), link.Key,
					WorkflowUtils.ExtractTransitionInfoJson(link));
			}

			return result;
		}

		public override void DoActivate()
		{
			base.DoActivate();
			var part = this.AdornedPart.Data;
			ActivityNode nodeData = part as ActivityNode;
			string strKey = string.Format("{0}@{1}", this.Diagram.Tag.ToString(), nodeData.Key);
			if (string.Compare(this.Diagram.Tag.ToString(), WorkflowUtils.CurrentKey) != 0)
			{
				HtmlPage.Window.Invoke("LoadProperty", WorkflowUtils.CLIENTSCRIPT_PARAM_ACTIVITY, this.Diagram.Tag.ToString(), WorkflowUtils.ExtractActivityInfoJson(nodeData));

				WorkflowUtils.CurrentKey = strKey;
			}
		}
	}
}
