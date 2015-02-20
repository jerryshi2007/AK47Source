using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.ModalDialog
{
	public partial class ExtGlobalParametersEditor : System.Web.UI.Page
	{
		private const string TreeRootText = "应用";

		protected void Page_Load(object sender, EventArgs e)
		{
			PropertyEditorRegister();
		}

		private void InitTreeNodes(WfApplicationCollection applications)
		{
			DeluxeTreeNode rootNode = new DeluxeTreeNode(TreeRootText, TreeRootText)
			{
				NodeOpenImg = "../images/computer.gif",
				NodeCloseImg = "../images/computer.gif",
				Expanded = true
			};

			AddDefaultTreeNode(rootNode);

			foreach (WfApplication app in applications)
			{
				DeluxeTreeNode node = new DeluxeTreeNode(app.Name, app.CodeName)
				{
					ToolTip = app.CodeName,
					NodeOpenImg = "../images/accomplished.gif",
					NodeCloseImg = "../images/accomplished.gif",
					ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading
				};

				rootNode.Nodes.Add(node);
			}

			tree.Nodes.Add(rootNode);
		}

		private void AddDefaultTreeNode(DeluxeTreeNode rootNode)
		{
			DeluxeTreeNode node = new DeluxeTreeNode("全局参数", "Default")
			{
				ToolTip = "Default",
				Selected = true,
				NodeOpenImg = "../images/accomplished.gif",
				NodeCloseImg = "../images/accomplished.gif",
				ExtendedData = "Program"
			};

			rootNode.Nodes.Add(node);
		}

		protected override void OnPreRender(EventArgs e)
		{
			InitTreeNodes(WfApplicationAdapter.Instance.LoadAll());

			var initPropeties = new { Key = "Default", Name = "全局参数", Properties = WfGlobalParameters.LoadDefault().Properties };

			this.ClientScript.RegisterHiddenField("initProperties",
				JSONSerializerExecute.Serialize(initPropeties));

			base.OnPreRender(e);
		}

		protected void tree_GetChildrenData(DeluxeTreeNode parentNode, DeluxeTreeNodeCollection result, string callBackContext)
		{
			WfProgramInApplicationCollection programs = WfApplicationAdapter.Instance.LoadProgramsByApplication(parentNode.Value);

			foreach (WfProgram program in programs)
			{
				DeluxeTreeNode node = new DeluxeTreeNode(program.Name, program.ApplicationCodeName + "~" + program.CodeName)
				{
					ToolTip = program.CodeName,
					NodeOpenImg = "../images/edit.gif",
					NodeCloseImg = "../images/edit.gif",
					ExtendedData = "Program"
				};

				result.Add(node);
			}
		}

		private static void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
		}
	}
}