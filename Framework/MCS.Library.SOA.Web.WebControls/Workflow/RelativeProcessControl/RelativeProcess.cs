using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
    public class RelativeProcess : WebControl, INamingContainer
    {
        public RelativeProcess()
            : base(System.Web.UI.HtmlTextWriterTag.Div)
        { }

        #region private
        private GridView RelativeProcessGrid = new GridView() { AutoGenerateColumns = false };
        private UpdatePanel updatepanel = new UpdatePanel();
        private SubmitButton refreshBtn = new SubmitButton();
        UpdatePanel uPanel = new UpdatePanel();
        #endregion private

        #region Properties
        [Browsable(false)]
        public WfRelativeProcessCollection RelativeProcesses
        {
            get 
            {
                WfRelativeProcessCollection relProces;
                string relativeID = string.Empty;
                relativeID = string.IsNullOrEmpty(RelativeID) ?  WfClientContext.Current.CurrentActivity.Process.RelativeID: RelativeID;
                relProces =
                        WfRelativeProcessAdapter.Instance.Load((p) => p.AppendItem("RELATIVE_ID", relativeID, "="));

                return relProces;
            }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public string RelativeID
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "RelativeID", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "RelativeID", value);
            }
        }

        [DefaultValue("blue")]
        public string HeaderColor
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "HeaderColor", "blue");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "HeaderColor", value);
            }
        }
        #endregion Properties

        private string CurrentPageUrl
        {
            get
            {
                Page page = (Page)HttpContext.Current.CurrentHandler;

                return page.ResolveUrl(page.AppRelativeVirtualPath);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (ProcessOpenProcessRequest())
                return;
            
            uPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
            uPanel.ID = "RelProcUpdatePanel";
            uPanel.ChildrenAsTriggers = true;

            AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
            refreshBtn.ID = "regreshID";
            trigger.ControlID = refreshBtn.ID;
            this.updatepanel.Triggers.Add(trigger);
            InitGridView();

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        private bool ProcessOpenProcessRequest()
        {
            string processID = WebUtility.GetRequestQueryValue<string>("relProcCtlProcessID", "");
            HttpContext.Current.Response.Clear();
            bool result = processID.IsNotEmpty();
            if (result)
            {
                IWfProcess proc = WfRuntime.GetProcessByProcessID(processID);
                string url = string.Format("{0}?resourceID={1}&activityID={2}",proc.Descriptor.Url,proc.ResourceID,proc.CurrentActivity.ID);
                Page.Response.Redirect(url);
            }
            return result;
        }

        private void InitGridView()
        {
            RelativeProcessGrid.Width = this.Width;
            RelativeProcessGrid.CssClass = "dataList";
            //RelativeProcessGrid.Height = this.Height;

            //BoundField RelativeCol = new BoundField();
            //RelativeCol.HeaderStyle.Height = 15;
            //RelativeCol.ItemStyle.Width = 60;
            //RelativeCol.HeaderText = "打开流程";
            
            //RelativeCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            //RelativeProcessGrid.Columns.Insert(0, RelativeCol);

            BoundField ProcessDescCol = new BoundField();
            ProcessDescCol.DataField = "Description";
            ProcessDescCol.HeaderStyle.Height = 15;
            ProcessDescCol.HeaderText = "流程描述";
            ProcessDescCol.ItemStyle.Width = Unit.Percentage(50);
            RelativeProcessGrid.Columns.Insert(0, ProcessDescCol);

            TemplateField ProcessStatusCol = new TemplateField();
            ProcessStatusCol.HeaderText = "流程状态";
            ProcessStatusCol.HeaderStyle.Height = 15;
            ProcessStatusCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            ProcessStatusCol.ItemTemplate = new ProcessStatusTemplate(DataControlRowType.DataRow);
            RelativeProcessGrid.Columns.Insert(1, ProcessStatusCol);
        }

        void RelativeProcesses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowType == DataControlRowType.Header))
            {
                TableItemStyle headerStyle = new TableItemStyle() { CssClass = "head" };
                e.Row.ApplyStyle(headerStyle);
            }
            else if(e.Row.RowType == DataControlRowType.DataRow)
            {
                
                WfRelativeProcess currData = e.Row.DataItem as WfRelativeProcess;
                e.Row.Cells[0].Text = string.Format("<a style=\"TEXT-DECORATION:underline \" href=\"{0}\" target=\"blank\">" + e.Row.Cells[0].Text + "</>", CurrentPageUrl + "?relProcCtlProcessID=" + currData.ProcessID);
                
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            ScriptManager sm = null;
            ScriptControlHelper.EnsureScriptManager(ref sm, this.Page);

            //UpdatePanel uPanel = new UpdatePanel();
                
            RelativeProcessGrid.RowDataBound += new GridViewRowEventHandler(RelativeProcesses_RowDataBound);

            refreshBtn.Text = "刷新";
            refreshBtn.PopupCaption = "正在刷新......";
            
            refreshBtn.Click += new EventHandler(refreshBtn_Click);

            RelativeProcessGrid.DataSource = RelativeProcesses;
            RelativeProcessGrid.DataBind();

            this.Controls.Add(refreshBtn);
            updatepanel.ContentTemplateContainer.Controls.Add(RelativeProcessGrid);

            this.Controls.Add(updatepanel);
            


        }

        void refreshBtn_Click(object sender, EventArgs e)
        {
			this.RelativeProcessGrid.DataSource = this.RelativeProcesses;
			this.RelativeProcessGrid.DataBind();

			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "RefreshRelativeProcess",
				"SubmitButton.resetAllStates();", true);
        }
    }

    public class ProcessStatusTemplate : ITemplate
    {
        private DataControlRowType templateType;
        public ProcessStatusTemplate(DataControlRowType type)
        {
            templateType = type;
        }

        public void InstantiateIn(Control container)
        {
            switch (templateType)
            {
                case DataControlRowType.DataRow:
                    WfStatusControl statusCtl = new WfStatusControl();
                    statusCtl.ID = "ProcessStatus";
                    statusCtl.DataBinding += new EventHandler(statusCtl_DataBinding);
                    container.Controls.Add(statusCtl);
                    break;
                case DataControlRowType.EmptyDataRow:
                    break;
                case DataControlRowType.Footer:
                    break;
                case DataControlRowType.Header:
                    break;
                case DataControlRowType.Pager:
                    break;
                case DataControlRowType.Separator:
                    break;
                default:
                    break;
            }
        }

        void statusCtl_DataBinding(object sender, EventArgs e)
        {
            WfStatusControl statusCtl = (WfStatusControl)sender;
            GridViewRow row = (GridViewRow)statusCtl.NamingContainer;

            statusCtl.ResourceID = DataBinder.Eval(row.DataItem, "RelativeID").ToString();

        }
    }

}