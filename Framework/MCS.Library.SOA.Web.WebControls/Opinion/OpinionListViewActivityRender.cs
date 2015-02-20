using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;

//意见列表控件Activity的Render部分

namespace MCS.Web.WebControls
{
	public partial class OpinionListView
	{
		private void RenderActivity(HtmlTableCell actCell, IWfMainStreamActivityDescriptor msActDesp)
		{
			HtmlGenericControl actName = new HtmlGenericControl("div");

			actName.Attributes["class"] = "actName";
			actName.InnerText = msActDesp.Activity.Name;

			actCell.Controls.Add(actName);

			bool bGenerateUsers = ShowStepUsers == ShowStepUsersDefine.All;

			if (!bGenerateUsers)
				bGenerateUsers = (CurrentStepIsDrafter || WfClientContext.Current.InAdminMode);

			if (bGenerateUsers)
				GenerateStepUsers(msActDesp, actCell);
		}

		private void GenerateStepUsers(IWfMainStreamActivityDescriptor msActivity, Control container)
		{
			IWfProcess process = msActivity.Activity.Process.ProcessInstance;

			if (process != null)
			{
				IWfActivity activity = process.Activities.FindActivityByDescriptorKey(msActivity.Activity.Key);

				RenderActivityUsers(activity.Candidates.GetSelectedAssignees().ToUsers(), msActivity, container);
			}
		}

		private void RenderActivityUsers(IEnumerable<IUser> users, IWfMainStreamActivityDescriptor msActivity, Control container)
		{
			foreach (IUser user in users)
			{
				bool showUserInfo = false;

				if (TitleShowMode == UserTitleShowMode.ShowActivityNameAndTitle)
					showUserInfo = true;
				else
				{
					if (TitleShowMode == UserTitleShowMode.ShowUserTitleWhenActivityNameEmpty)
						showUserInfo = string.IsNullOrEmpty(msActivity.Activity.Name);
				}

				if (showUserInfo)
				{
					HtmlGenericControl userTitle = new HtmlGenericControl("div");

					try
					{
						string titleText = string.Empty;

						string deptName = string.Empty;

						if ((TitleShowMode & UserTitleShowMode.ShowDepartmentName) != UserTitleShowMode.None)
							deptName = GetUserDepartmentName(user);

						if (string.IsNullOrEmpty(user.Occupation))
							titleText = deptName;
						else
							titleText = deptName.IsNotEmpty() ? deptName + "/" + user.Occupation : user.Occupation;

						userTitle.InnerText = titleText;

						if ((int)user.Properties.GetValue("STATUS", 1) != 1)
						{
							userTitle.Attributes["title"] = string.Format("{0} {1}",
								titleText,
								Translator.Translate(Define.DefaultCulture, "帐号已注销") + ". " +
								Translator.Translate(Define.DefaultCulture, "请联系管理员调整流程"));
						}
						else
						{
							userTitle.Attributes["class"] = "opDept";
						}
					}
					catch (System.Exception)
					{
						userTitle.Attributes["title"] = Translator.Translate(Define.DefaultCulture, "人员信息有误") + "." +
							Translator.Translate(Define.DefaultCulture, "请联系管理员调整流程");
					}

					container.Controls.Add(userTitle);
				}

				HtmlGenericControl userName = new HtmlGenericControl("div");
				userName.Attributes["class"] = "opName";

				if (this.EnableUserPresence)
				{
					UserPresence presence = new UserPresence();

					presence.UserID = user.ID;
					presence.UserDisplayName = user.DisplayName;

					userName.Controls.Add(presence);
				}
				else
					userName.InnerText = user.DisplayName;

				container.Controls.Add(userName);
			}
		}

		/// <summary>
		/// 发生在PreRender环节，生成流程编辑界面
		/// </summary>
		/// <param name="levels"></param>
		private void RenderActivitiesEditor(WfMainStreamActivityDescriptorCollection msActivities)
		{
			if (CurrentActivity.Process.Status == WfProcessStatus.Running)
			{
				Dictionary<string, Dictionary<string, string>> changeParams = GetActivityKeysCanBeChanged(CurrentActivity.Descriptor);

				if (CurrentActivity.Descriptor.ActivityType == WfActivityType.InitialActivity &&
					CurrentActivity.OpinionRootActivity == CurrentActivity)
				{
					if (_Table.Rows.Count > 0)
					{
						Control container = _Table.Rows[0].Cells[0];

						HtmlGenericControl div = new HtmlGenericControl("div");

						container.Controls.Add(div);

						IWfActivityDescriptor actDesp = GetCanModifiedActivityDescriptorFromGroup(CurrentActivity.Descriptor);

						ActivityEditMode editMode = GetActivityDespEditMode(actDesp, msActivities, changeParams);
						RenderOneActivityEditor(div, CurrentActivity.Descriptor, editMode);

						div.Attributes["class"] = "command";
					}
				}

				foreach (IWfMainStreamActivityDescriptor msActivity in msActivities)
				{
					Control container = null;

					if (_activityEditorPlaceHolders.TryGetValue(msActivity.Activity.Key, out container))
					{
						HtmlGenericControl div = new HtmlGenericControl("div");

						container.Controls.Add(div);

						if (ReadOnly == false || WfClientContext.Current.InAdminMode)
						{
							IWfActivityDescriptor actDesp = GetCanModifiedActivityDescriptorFromGroup(msActivity.Activity);

							ActivityEditMode editMode = GetActivityDespEditMode(actDesp, msActivities, changeParams);

							if (editMode != ActivityEditMode.None)
							{
								RenderOneActivityEditor(div, actDesp, editMode);

								div.Attributes["class"] = "command";
							}
						}
					}
				}
			}
		}

		private ActivityEditMode GetActivityDespEditMode(IWfActivityDescriptor actDesp, WfMainStreamActivityDescriptorCollection msActivities, Dictionary<string, Dictionary<string, string>> changeParams)
		{
			ActivityEditMode result = ActivityEditMode.None;

			if (actDesp != null)
			{
				if (actDesp.Properties.GetValue("AllowToBeAppended", true) &&
					WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowAppendActivityAfterUnstartActivities", true) &&
					TargetActivityCanBeChangedByCurrentActivity(actDesp, "CanAppendActivityKeys", changeParams) &&
					IsExceedMaximizeMainStreamActivityCount(msActivities.Count) == false
					|| WfClientContext.Current.InAdminMode)
					result |= ActivityEditMode.Add;

				if (ActivityGroupIsRunningOrPassed(actDesp) == false)
				{
					if (actDesp.Properties.GetValue("AllowToBeModified", true) &&
							WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowModifyUnstartActivities", true) &&
							TargetActivityCanBeChangedByCurrentActivity(actDesp, "CanModifyActivityKeys", changeParams) || WfClientContext.Current.InAdminMode)
						result |= ActivityEditMode.Edit;

					if (actDesp.Properties.GetValue("AllowToBeDeleted", true) &&
							WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowDeleteUnstartActivities", true) &&
							TargetActivityCanBeChangedByCurrentActivity(actDesp, "CanDeleteActivityKeys", changeParams) || WfClientContext.Current.InAdminMode)
						result |= ActivityEditMode.Delete;
				}

				RenderOneActivityEventArgs eventArgs = new RenderOneActivityEventArgs();
				eventArgs.CurrentActivity = CurrentActivity;
				eventArgs.ActivityDescriptor = actDesp;
				eventArgs.EditMode = result;

				OnRenderOneActivity(CurrentActivity, actDesp, eventArgs);

				result = eventArgs.EditMode;
			}

			return result;
		}

		private void RenderOneActivityEditor(Control parent, IWfActivityDescriptor actDesp, ActivityEditMode editMode)
		{
			if ((editMode & ActivityEditMode.Add) != ActivityEditMode.None)
			{
				HtmlAnchor addAnchor = CreateEditActivityAnchor(Translator.Translate(Define.DefaultCulture, "增加"), "add", false, actDesp);
				parent.Controls.Add(addAnchor);
			}

			if ((editMode & ActivityEditMode.Edit) != ActivityEditMode.None)
			{
				HtmlAnchor editAnchor = CreateEditActivityAnchor(Translator.Translate(Define.DefaultCulture, "修改"), "update",
					WfVariableDefine.UseSmallEditModeDescriptor(actDesp), actDesp);
				parent.Controls.Add(editAnchor);
			}

			if ((editMode & ActivityEditMode.Delete) != ActivityEditMode.None)
			{
				HtmlAnchor deleteAnchor = CreateEditActivityAnchor(Translator.Translate(Define.DefaultCulture, "删除"), "delete", false, actDesp);
				parent.Controls.Add(deleteAnchor);
			}
		}

		private HtmlAnchor CreateEditActivityAnchor(string text, string op, bool smallMode, IWfActivityDescriptor actDesp)
		{
			HtmlAnchor anchor = new HtmlAnchor();

			anchor.HRef = "#";
			anchor.InnerText = text;
			anchor.Attributes["processID"] = OriginalActivity.Process.ID;
			anchor.Attributes["resourceID"] = OriginalActivity.Process.ResourceID;
			anchor.Attributes["currentActivityKey"] = actDesp.Key;
			anchor.Attributes["class"] = "btn";

			anchor.Attributes["onclick"] = string.Format("onEditActivityClick(\"{0}\", \"{1}\", \"{2}\", \"{3}\", {4})",
				GetActivityEditorClientID(actDesp),
				this._ChangeProcessButton.ClientID,
				_HiddenData.ClientID,
				op,
				smallMode ? "true" : "false");

			return anchor;
		}

		/// <summary>
		/// 得到当前环节中，能够被编辑的Activity
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		private IWfActivityDescriptor GetCanModifiedActivityDescriptorFromGroup(IWfActivityDescriptor actDesp)
		{
			IWfActivityDescriptor result = null;

			if (ActivityCanModified(actDesp))
				result = actDesp;

			return result;
		}

		private bool ActivityCanModified(IWfActivityDescriptor actDesp)
		{
			return (WfClientContext.Current.InMoveToMode || WfClientContext.Current.InAdminMode) &&
				(CurrentStepIsDrafter || (ActivityIsPassed(actDesp) == false));
		}

		private static bool ActivityIsPassed(IWfActivityDescriptor actDesp)
		{
			return (actDesp.Instance != null &&
				(actDesp.Instance.Status == WfActivityStatus.Completed || actDesp.Instance.Status == WfActivityStatus.Aborted));
		}

		private static bool ActivityGroupIsRunningOrPassed(IWfActivityDescriptor actDesp)
		{
			IEnumerable<IWfActivityDescriptor> groupActivities = actDesp.GetSameGroupActivities();

			return groupActivities.Exists(a => a.Instance != null && a.Instance.Status != WfActivityStatus.NotRunning);
		}

		private void CreateInitialActivityEditorRow(HtmlTable parent, IWfActivityDescriptor actDesp)
		{
			HtmlTableRow row = new HtmlTableRow();
			parent.Rows.Add(row);

			CreateActivityCell(row);

			//如果起始点可以编辑意见，那么缺省的第一行“添加”、“删除”、“修改”流程的行不显示
			if (actDesp.Properties.GetValue("ShowingInOpinionList", false) == true)
				row.Style["display"] = "none";
		}

		private HtmlTableCell CreateActivityCell(Control parent)
		{
			HtmlTableCell actCell = new HtmlTableCell();
			parent.Controls.Add(actCell);

			actCell.Attributes["class"] = "activity";
			actCell.Controls.Add(new HtmlGenericControl("span"));

			return actCell;
		}

		private bool TargetActivityCanBeChangedByCurrentActivity(IWfActivityDescriptor targetActDesp, string propertyName, Dictionary<string, Dictionary<string, string>> changeParames)
		{
			bool result = true;

			Dictionary<string, string> activityKeys = changeParames[propertyName];

			if (activityKeys.Count > 0)
				result = activityKeys.ContainsKey(targetActDesp.Key);

			return result;
		}

		private static Dictionary<string, Dictionary<string, string>> GetActivityKeysCanBeChanged(IWfActivityDescriptor actDesp)
		{
			Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

			FillActivityKeysCanBeChanged(actDesp, "CanAppendActivityKeys", result);
			FillActivityKeysCanBeChanged(actDesp, "CanModifyActivityKeys", result);
			FillActivityKeysCanBeChanged(actDesp, "CanDeleteActivityKeys", result);

			return result;
		}

		private static void FillActivityKeysCanBeChanged(IWfActivityDescriptor actDesp, string propertyName, Dictionary<string, Dictionary<string, string>> result)
		{
			Dictionary<string, string> activityKeys = null;

			if (result.TryGetValue(propertyName, out activityKeys) == false)
			{
				activityKeys = new Dictionary<string, string>();

				result.Add(propertyName, activityKeys);
			}

			string strKeys = actDesp.Properties.GetValue(propertyName, string.Empty);
			if (!string.IsNullOrEmpty(strKeys))
			{
				foreach (string key in strKeys.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
				{
					activityKeys[key] = key.Trim();
				}
			}
		}

		/// <summary>
		/// 当前环节是否是起草人
		/// </summary>
		private bool CurrentStepIsDrafter
		{
			get
			{
				return CurrentActivity.Descriptor.ActivityType == WfActivityType.InitialActivity &&
							CurrentActivity.OpinionRootActivity == CurrentActivity;
			}
		}

		/// <summary>
		/// 是否超出了最大流程主环节的限制
		/// </summary>
		/// <param name="currentMSActivitiesCount"></param>
		/// <returns></returns>
		private bool IsExceedMaximizeMainStreamActivityCount(int currentMSActivitiesCount)
		{
			bool result = false;

			int acitivitiesLimit = CurrentProcess.Descriptor.Properties.GetValue("MaximizeMainStreamActivityCount", -1);

			if (acitivitiesLimit == -1)
				acitivitiesLimit = WfGlobalParameters.GetValueRecursively(
					CurrentProcess.Descriptor.ApplicationName,
					CurrentProcess.Descriptor.ProgramName,
					"MaximizeMainStreamActivityCount", -1);

			if (acitivitiesLimit >= 0)
				result = currentMSActivitiesCount > acitivitiesLimit;

			return result;
		}
	}
}
