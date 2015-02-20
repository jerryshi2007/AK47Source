using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfActivityDescriptorGroupResourceEditor.WfActivityDescriptorGroupResourceEditor.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// Activity的组资源选择器
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfActivityDescriptorGroupResourceEditor",
		"MCS.Web.WebControls.Workflow.WfActivityDescriptorGroupResourceEditor.WfActivityDescriptorGroupResourceEditor.js")]
	[DialogContent("MCS.Web.WebControls.Workflow.WfActivityDescriptorGroupResourceEditor.WfActivityDescriptorGroupResourceEditor.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:WfActivityDescriptorGroupResourceEditor runat=server></{0}:WfActivityDescriptorGroupResourceEditor>")]
	public class WfActivityDescriptorGroupResourceEditor : WfActivityDescriptorEditorBase
	{
		private UserOUGraphControl groupControl = new UserOUGraphControl() { InvokeWithoutViewState = true, ID = "groupControl" };

		protected override void OnInit(EventArgs e)
		{
			if (this.Page.IsCallback)
				EnsureChildControls();

			base.OnInit(e);
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("userInputClientID")]
		protected string UserInputClientID
		{
			get
			{
				return groupControl.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("actNameClientID")]
		protected override string ActNameClientID
		{
			get
			{
				return base.ActNameClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("allAgreeWhenConsignCheckboxClientID")]
		protected override string AllAgreeWhenConsignCheckboxClientID
		{
			get
			{
				return base.AllAgreeWhenConsignCheckboxClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("operation")]
		protected override string Operation
		{
			get
			{
				return ControlParams.Operation;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("currentActivityKey")]
		protected override string CurrentActivityKey
		{
			get
			{
				return base.CurrentActivityKey;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("smallDialogUrl")]
		private string SmallDialogUrl
		{
			get
			{
				return GetSmallDialogUrl();
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("smallDialogFeature")]
		private string SmallDialogFeature
		{
			get
			{
				return GetSmallDialogFeature();
			}
		}

		protected override void InitUserInputArea(Control container)
		{
			InitUserInput(WebControlUtility.FindControlByHtmlIDProperty(container, "resourceContainer", true));
		}

		protected override void VariablesInitialized(List<VariableObject> variables)
		{
			if (variables.Exists(v => v.Key == WfVariableDefine.AllAgreeWhenConsignVariableName == false))
				variables.Add(new VariableObject(WfVariableDefine.AllAgreeWhenConsignVariableName, "会签时，所有用户通过后流转到下一步", true));
		}

		protected override string GetSmallDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 480;
			feature.Height = 420;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 480;
			feature.Height = 420;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		private void InitUserInput(Control container)
		{
			groupControl.ID = "groupInput";
			groupControl.Width = Unit.Percentage(100);
			groupControl.Height = Unit.Percentage(100);
			groupControl.ShowingMode = ControlShowingMode.Normal;
			groupControl.RootPath = WfVariableDefine.GroupSelectorRootPath(CurrentActivityDescriptor);
			groupControl.ListMask = UserControlObjectMask.All;
			groupControl.SelectMask = UserControlObjectMask.Group | UserControlObjectMask.User;
			groupControl.MultiSelect = true;
			groupControl.MergeSelectResult = true;
			groupControl.LoadingObjectToTreeNode += new LoadingObjectToTreeNodeDelegate(groupControl_LoadingObjectToTreeNode);
			groupControl.GetChildren += new GetChildrenDelegate(groupControl_GetChildren);

			if (this.Operation == "update")
			{
				OguDataCollection<IUser> users = CurrentActivityDescriptor.Resources.ToUsers();

				users.ForEach(u => groupControl.SelectedOuUserData.Add(u));
			}

			container.Controls.Add(groupControl);
		}

		private IEnumerable<IOguObject> groupControl_GetChildren(UserOUGraphControl treeControl, IOguObject parent)
		{
			IEnumerable<IOguObject> result = null;

			if (parent is IOrganization)
				result = ((IOrganization)parent).Children;
			else
			{
				List<IOguObject> resultList = new List<IOguObject>();

				if (parent is IGroup)
				{
					foreach (IUser user in ((IGroup)parent).Members)
					{
						resultList.Add(OguBase.CreateWrapperObject(user));
					}

					result = resultList;
				}

				result = resultList;
			}

			return result;
		}

		private void groupControl_LoadingObjectToTreeNode(UserOUGraphControl treeControl, IOguObject oguObj, DeluxeTreeNode newTreeNode, ref bool cancel)
		{
			if ((oguObj is IUser) == false)
			{
				newTreeNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;

				if (oguObj is IGroup)
				{
					IGroup group = (IGroup)oguObj;

					int count = GroupMemberSelectedCount(group, treeControl.SelectedOuUserData);

					if (count > 0 && group.Members.Count == count)
					{
						newTreeNode.Checked = true;
						treeControl.SelectedOuUserData.Add(group);
					}
				}
			}
			else
			{
				OguUser user = (OguUser)oguObj;

				StringBuilder strB = new StringBuilder();

				foreach (IGroup group in user.MemberOf)
				{
					if (strB.Length > 0)
						strB.Append(",");

					strB.Append(group.ID);
				}

				user.Tag = strB.ToString();
			}

			newTreeNode.ShowCheckBox = true;
		}

		private static int GroupMemberSelectedCount(IGroup group, IEnumerable<IOguObject> selectedObjs)
		{
			int count = 0;

			foreach (IUser user in group.Members)
			{
				if (IsSelected(user.ID, selectedObjs))
					count++;
			}

			return count;
		}

		private static bool IsSelected(string id, IEnumerable<IOguObject> selectedObjs)
		{
			bool result = false;

			foreach (IOguObject obj in selectedObjs)
			{
				if (string.Compare(obj.ID, id, true) == 0)
				{
					result = true;
					break;
				}
			}

			return result;
		}
	}
}
