using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;
using System.ComponentModel;
using System.Web;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow.DTO;
using MCS.Library.Core;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfRuntimeViewer.WfRuntimeViewerDialog.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Workflow.WfRuntimeViewer.WfRuntimeViewerDialogTemplate.htm", "text/html")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 分支流程选择对话框
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfRuntimeViewerDialog", "MCS.Web.WebControls.Workflow.WfRuntimeViewer.WfRuntimeViewerDialog.js")]
	[DialogContent("MCS.Web.WebControls.Workflow.WfRuntimeViewer.WfRuntimeViewerDialogTemplate.htm", true)]
	[ToolboxData("<{0}:WfRuntimeViewerDialog runat=server></{0}:WfRuntimeViewerDialog>")]
	public class WfRuntimeViewerDialog : DialogControlBase<WfRuntimeViewerParams>
	{
		private Button serverConfirmButton = null;
		private DeluxeGrid processGrid = null;
		private HtmlGenericControl statistics = null;

		public WfRuntimeViewerDialog()
		{
			WfConverterHelper.RegisterConverters();
		}

		#region properties
		[Bindable(true), Category("ActivityID"), Description("查看分支流程ID")]
		private string ActivityID
		{
			get
			{
				return ControlParams.ActivityID;
			}
			set
			{
				ControlParams.ActivityID = value;
			}
		}

		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		private Dictionary<WfProcessStatus, int> StatisticsData
		{
			get
			{
				Dictionary<WfProcessStatus, int> result = (Dictionary<WfProcessStatus, int>)this.ViewState["StatisticsData"];

				if (result == null)
				{
					result = WfProcessCurrentInfoAdapter.Instance.LoadStatisticsDataByOwnerActivityID(this.ActivityID, string.Empty);

					this.ViewState["StatisticsData"] = result;
				}

				return result;
			}
		}
		#endregion

		#region override

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Center = true;
			feature.Width = 720;
			feature.Height = 520;
			feature.Resizable = true;
			feature.ShowScrollBars = false;
			feature.ShowToolBar = false;
			feature.ShowStatusBar = false;

			return feature.ToDialogFeatureClientString();
		}

		protected override Control LoadDialogTemplate()
		{
			ScriptManager sm = null;
			ScriptControlHelper.EnsureScriptManager(ref sm, this.Page);

			return base.LoadDialogTemplate();
		}

		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);

			this.serverConfirmButton = (Button)this.FindControlByID("serverConfirmButton", true);
			this.processGrid = (DeluxeGrid)this.FindControlByID("ProcessDescInfoDeluxeGrid", true);
			this.statistics = (HtmlGenericControl)this.FindControlByID("statistics", true);

			if (this.statistics != null)
				this.statistics.InnerText = this.GetStatisticsText();

			processGrid.RowDataBound += DeluxeGridRowDataBound;
			this.serverConfirmButton.Click += new EventHandler(serverConfirmButton_Click);
			ObjectDataSource objectDataSource = container.FindControl<ObjectDataSource>(true);

			objectDataSource.Selecting += new ObjectDataSourceSelectingEventHandler(objectDataSource_Selecting);
			objectDataSource.Selected += new ObjectDataSourceStatusEventHandler(objectDataSource_Selected);

			if (Page.IsPostBack == false)
			{
				ExecuteQuery();
			}
		}

		private string GetStatisticsText()
		{
			StringBuilder strB = new StringBuilder();

			if (this.ActivityID.IsNotEmpty())
			{
				Dictionary<WfProcessStatus, int> statData = this.StatisticsData;

				int total = 0;
				foreach (KeyValuePair<WfProcessStatus, int> kp in statData)
					total += kp.Value;

				strB.AppendFormat("总共{0}条流程", total);

				EnumItemDescriptionList statusList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(WfProcessStatus));

				foreach (EnumItemDescription item in statusList)
				{
					int count = 0;

					if (statData.TryGetValue((WfProcessStatus)item.EnumValue, out count))
					{
						strB.AppendFormat(", {0}{1}条", item.Description, count);
					}
				}
			}

			return strB.ToString();
		}

		private void serverConfirmButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.processGrid != null)
				{
					List<WorkflowInfo> processInfoList = new List<WorkflowInfo>();

					foreach (string key in this.processGrid.SelectedKeys)
					{
						IWfProcess process = WfRuntime.GetProcessByProcessID(key);
						var processInfo = WorkflowInfo.ProcessAdapter(process);
						processInfoList.Add(processInfo);

					}

					HtmlInputHidden resultData = (HtmlInputHidden)WebControlUtility.FindControlByID(this, "resultData", true);

					if (resultData != null)
						resultData.Value = JSONSerializerExecute.Serialize(processInfoList);

					ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "Close",
						string.Format("window.returnValue = $get('{0}').value; top.close()", resultData.ClientID),
						true);
				}
			}
			catch (System.Exception ex)
			{
				string errorScript = WebUtility.GetShowClientErrorScript(ex.Message, ex.StackTrace, "错误");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ShowError",
					"SubmitButton.resetAllStates();" + errorScript,
					true);
			}
		}

		private void DeluxeGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			var process = e.Row.DataItem as WfProcessCurrentInfo;

			if (process == null)
				return;

			var enumDesc = EnumItemDescriptionAttribute.GetDescriptionList(typeof(WfProcessStatus));

			e.Row.Cells[3].Text = enumDesc.Single(p => p.Name == process.Status.ToString()).Description;

			if (process.StartTime == DateTime.MinValue)
				e.Row.Cells[4].Text = string.Empty;

			if (process.EndTime == DateTime.MinValue)
				e.Row.Cells[5].Text = string.Empty;
		}

		private void ExecuteQuery()
		{
			LastQueryRowCount = -1;
			var whereCondition = (HtmlInputHidden)this.FindControlByID("whereCondition", true);
			var activityID = this.ActivityID;

			if (activityID.IsNotEmpty())
			{
				WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
				builder.AppendItem<string>("OWNER_ACTIVITY_ID", activityID);
				whereCondition.Value = builder.ToSqlString(TSqlBuilder.Instance);
			}

			LastQueryRowCount = -1;
			this.processGrid.SelectedKeys.Clear();
			this.processGrid.PageIndex = 0;
		}

		protected void objectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		protected void objectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			base.InitConfirmButton(confirmButton);

			confirmButton.Attributes["onclick"] = string.Format("{0}.click()", serverConfirmButton.ClientID);
		}
		#endregion
	}
}
