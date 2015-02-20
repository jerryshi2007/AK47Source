using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Globalization;
using MCS.Web.Library;

namespace MCS.Web.WebControls
{
	public partial class OpinionListView
	{
		private void ChangeProcessButton_Click(object sender, EventArgs e)
		{
			try
			{
				WfActivityDescriptorCreateParams createParams = null;

				WfMoveToControl.DoActionAfterRegisterContextConverter(() =>
					createParams = JSONSerializerExecute.Deserialize<WfActivityDescriptorCreateParams>(_HiddenData.Value));

				WfExecutorBase executor = null;

				IWfActivity targetActivity = FindCurrentActivity(createParams);
				IWfActivity changedActivity = targetActivity;

				switch (createParams.Operation.ToLower())
				{
					case "add":
						executor = new WfAddActivityExecutor(CurrentActivity, targetActivity, createParams);
						break;
					case "update":
						executor = new WfEditActivityExecutor(CurrentActivity, targetActivity, createParams);
						break;
					case "delete":
						executor = new WfDeleteActivityExecutor(CurrentActivity, targetActivity);
						break;
					default:
						throw new SystemSupportException(string.Format("Invalid operation name: {0}", createParams.Operation));
				}

				WfMoveToControl.Current.OnAfterCreateModifyProcessExecutor(executor);
				WfClientContext.Current.Execute(executor);

				WfMoveToControl.Current.OnProcessChanged(targetActivity.Process);

				RecreateProcesses(WfMoveToControl.Current.NextSteps, false, false);

				if (executor != null)
					WfMoveToControl.Current.OnAfterProcessChanged(targetActivity.Process);
			}
			catch (System.Exception ex)
			{
				RegisterPostBackException(ex);
			}
		}

		private void RefreshProcessButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (RefreshProcess != null)
					RefreshProcess(sender, e);

				WfMoveToControl.Current.OnProcessChanged(WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process);

				WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process.GenerateCandidatesFromResources();

				RecreateProcesses(true);

				WfMoveToControl.Current.OnAfterProcessChanged(WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process);
			}
			catch (System.Exception ex)
			{
				RegisterPostBackException(ex);
			}
		}

		private void RefreshCurrentProcessButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (RefreshProcess != null)
					RefreshProcess(sender, e);

				WfMoveToControl.Current.OnProcessChanged(WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process);

				WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process.GenerateCandidatesFromResources();

				RecreateProcesses(true, true);

				WfMoveToControl.Current.OnAfterProcessChanged(WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process);
			}
			catch (System.Exception ex)
			{
				RegisterPostBackException(ex);
			}
		}

		private void ChangeTransitionButton_Click(object sender, EventArgs e)
		{
			try
			{
				WfControlNextStep nextStep = null;

				if (_EnabledNextStepSelectorControl != null)
				{
					nextStep = _EnabledNextStepSelectorControl.SelectedNextStep;

					if (nextStep != null)
					{
						IWfActivityDescriptor originalDesp = WfClientContext.Current.OriginalActivity.Descriptor;

						/* YDZ 2012/11/23 为了记录克隆前线的状态
						foreach (IWfTransitionDescriptor t in originalDesp.ToTransitions)
						{
							t.Properties.SetValue("DefineDefaultSelect", t.DefaultSelect); //记录改变值前的记录

							t.DefaultSelect = false;

							t.DefaultSelect = t.Key == nextStep.TransitionDescriptor.Key;

							if (t.DefaultSelect)
							{
								if (t.AffectProcessReturnValue)
									WfClientContext.Current.OriginalActivity.Descriptor.Process.DefaultReturnValue = t.AffectedProcessReturnValue;
							}
						} */

						//originalDesp.ToTransitions.ForEach(t => t.DefaultSelect = false);

						originalDesp.ToTransitions.ForEach(t =>
						{
							t.DefaultSelect = false;
							t.DefaultSelect = t.Key == nextStep.TransitionDescriptor.Key;

							if (t.DefaultSelect)
							{
								if (t.AffectProcessReturnValue)
									WfClientContext.Current.OriginalActivity.Descriptor.Process.DefaultReturnValue = t.AffectedProcessReturnValue;
							}
						});
					}
				}

				WfControlNextStepCollection nextSteps = WfMoveToControl.Current.NextSteps;

				if (nextStep != null)
				{
					nextSteps = new WfControlNextStepCollection();
					nextSteps.Add(nextStep);
				}

				WfMoveToControl.Current.OnProcessChanged(WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process);

				WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process.GenerateCandidatesFromResources();

				RecreateProcesses(nextSteps, true, false);

				WfMoveToControl.Current.OnAfterProcessChanged(WfClientContext.Current.CurrentActivity.OpinionRootActivity.Process);
			}
			catch (System.Exception ex)
			{
				RegisterPostBackException(ex);
			}
		}

		private void RecreateProcesses(bool saveAndReload, bool isCurrent = false)
		{
			RecreateProcesses(null, saveAndReload, isCurrent);
		}

		private void RecreateProcesses(WfControlNextStepCollection nextSteps, bool saveAndReload, bool isCurrent)
		{
			_Table.Rows.Clear();

			if (saveAndReload)
			{
				WfSaveDataExecutor executor;

				if (isCurrent)
				{
					executor = new WfSaveDataExecutor(WfClientContext.Current.OriginalCurrentActivity, WfClientContext.Current.OriginalCurrentActivity);
				}
				else
				{
					executor = new WfSaveDataExecutor(OriginalActivity, OriginalActivity);
				}

				executor.AutoCommit = false;
				executor.SaveUserTasks = false;

				executor.Execute();

				ReloadProcessContext();
			}

			CreateProcessSteps(GetAllMainStreamActivities());

			if (WfMoveToControl.Current != null)
			{
				if (nextSteps == null)
					nextSteps = WfMoveToControl.Current.NextSteps;

				WfMoveToControl.DoActionAfterRegisterContextConverter(() =>
					AddControlToTemplate(_RootPanel, new LiteralControl(
						string.Format("<input type='hidden' id='adjustNextStepsHidden' name='adjustNextStepsHidden' value='{0}'/>",
							HttpUtility.HtmlAttributeEncode(JSONSerializerExecute.Serialize(nextSteps)))
						))
					);
			}
		}

		private void ReloadProcessContext()
		{
			/*
			ProcessContext.Current.BeginProcessChanges();

			try
			{
				WfRuntime.ClearProcessCache();
			}
			finally
			{
				ProcessContext.Current.RollBackProcessChanges();
			}*/
		}

		private void RegisterPostBackException(System.Exception ex)
		{
			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ModifyProcessError",
					WebUtility.GetShowClientErrorScript(ex.Message, ex.StackTrace,
					Translator.Translate(Define.DefaultCulture, "错误")), true);
		}
	}
}
