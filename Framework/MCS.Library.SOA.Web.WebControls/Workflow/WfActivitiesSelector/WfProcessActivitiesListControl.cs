using System;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
    [ToolboxData("<{0}:WfProcessActivitiesListControl runat=server></{0}:WfProcessActivitiesListControl>")]
    public class WfProcessActivitiesListControl : Control
    {
        private IWfProcess process = null;

        [Browsable(false)]
        public IWfProcess Process
        {
            get
            {
                return this.process;
            }
            set
            {
                this.process = value;
            }
        }

        [DefaultValue("")]
        public string ProcessDespKey
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "ProcessDespKey", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "ProcessDespKey", value);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            IWfProcess currentProcess = this.process;

            //if (currentProcess == null && ProcessContext.Current.OriginalActivity != null) //12-29
            //    currentProcess = ProcessContext.Current.OriginalActivity.Process;          //12-29
            if (currentProcess == null && WfClientContext.Current.OriginalActivity != null)
                currentProcess = WfClientContext.Current.OriginalActivity.Process;

            if (currentProcess != null)
            {
                IWfProcessDescriptor processDesp = currentProcess.CurrentActivity.Descriptor.Process;

                if (string.IsNullOrEmpty(ProcessDespKey) == false)
                {
                    //processDesp = WorkflowSettings.GetConfig().ProcessDescriptorManager.GetProcessDescriptor(ProcessDespKey);
					processDesp = WfProcessDescriptorManager.GetDescriptor(ProcessDespKey);
                }

                RenderActivities(this, currentProcess, processDesp);
            }

            base.OnPreRender(e);
        }

        #region Private
        private void RenderActivities(Control container, IWfProcess process, IWfProcessDescriptor processDesp)
        {
            HtmlTable table = new HtmlTable();

            container.Controls.Add(table);

            RednerActivity(table, processDesp.InitialActivity, process, -1);

            for (int i = 0; i < processDesp.Activities.Count; i++)
            {
                IWfActivityDescriptor actDesp = processDesp.Activities[i];

                //if (actDesp is IWfInitialActivityDescriptor == false &&
                //    actDesp is IWfCompletedActivityDescriptor == false)
                //    RednerActivity(table, actDesp, process, i);           //12-29

                if (actDesp.ActivityType == WfActivityType.InitialActivity == false &&
                    actDesp.ActivityType == WfActivityType.CompletedActivity == false)
                    RednerActivity(table, actDesp, process, i);          
            }

            RednerActivity(table, processDesp.CompletedActivity, process, -2);
        }

        private void RednerActivity(Control parent, IWfActivityDescriptor actDesp, IWfProcess process, int index)
        {
            if (actDesp != null)
            {
                bool enabled = true;
                AdjustProcessAllowSelectFlagType aType = AdjustProcessAllowSelectFlagType.Auto;

                //if (actDesp is IOAWfActivityDescriptor)
                //    aType = ((IOAWfActivityDescriptor)actDesp).AdjustProcessAllowSelectFlag;   //12-29 先注释掉             

                //如果是Anchor点或者不能走到结束点，则disabled掉
                //if (actDesp is IWfAnchorActivityDescriptor ||   //12-29

                if (actDesp.ActivityType == WfActivityType.NormalActivity ||   
                    (aType == AdjustProcessAllowSelectFlagType.Auto &&
                    actDesp.CanReachTo(actDesp.Process.CompletedActivity) == false) ||
                    aType == AdjustProcessAllowSelectFlagType.Unallowed)
                    enabled = false;

                #region
                HtmlTableRow row = new HtmlTableRow();
                parent.Controls.Add(row);

                HtmlTableCell cellRadio = new HtmlTableCell();
                row.Controls.Add(cellRadio);
                cellRadio.Attributes["class"] = "cellRadio";

                HtmlGenericControl radio = new HtmlGenericControl("input");
                cellRadio.Controls.Add(radio);
                radio.Attributes["type"] = "radio";
                radio.Attributes["value"] = actDesp.Process.Key + ":" + actDesp.Key + ":" + actDesp.Name;
                radio.Attributes["name"] = "selectedActivity";
                radio.Attributes["processKey"] = actDesp.Process.Key;
                radio.Attributes["activityKey"] = actDesp.Key;
                radio.ID = "selected" + index;
                radio.Disabled = !enabled;

                HtmlTableCell cellImage = new HtmlTableCell();
                row.Controls.Add(cellImage);
                cellImage.Attributes["class"] = "cellImage";

                HtmlImage logo = new HtmlImage();
                cellImage.Controls.Add(logo);
                logo.Disabled = !enabled;
                logo.Src = GetActivityLogoByType(actDesp);

                HtmlTableCell cellText = new HtmlTableCell();
                row.Controls.Add(cellText);
                cellText.Attributes["class"] = "cellText";

                HtmlGenericControl content = new HtmlGenericControl("label");
                cellText.Controls.Add(content);
                content.Disabled = !enabled;
                content.Attributes["for"] = radio.ClientID;
                content.InnerText = actDesp.Name;
                content.Attributes["title"] = actDesp.Name;

                if (process.CurrentActivity.Descriptor == actDesp)
                {
                    content.Attributes["title"] = actDesp.Name + "(当前环节)";
                    content.Style["color"] = "green";
                    content.Style["font-weight"] = "bold";
                }
                #endregion
            }
        }

        private string GetActivityLogoByType(IWfActivityDescriptor actDesp)
        {
            string result = ControlResources.ActivityLogoUrl;

            //if (actDesp is IWfAnchorActivityDescriptor)
            //    result = WfControlResources.AncnhorActivityLogoUrl;
            //else
            //    if (actDesp is IWfInitialActivityDescriptor)
            //        result = WfControlResources.InitialActivityLogoUrl;
            //    else
            //        if (actDesp is IWfCompletedActivityDescriptor)
            //            result = WfControlResources.CompletedActivityLogoUrl;  12-29

            if (actDesp.ActivityType == WfActivityType.NormalActivity)
                result = ControlResources.AncnhorActivityLogoUrl;
            else
                if (actDesp.ActivityType == WfActivityType.InitialActivity)
                    result = ControlResources.InitialActivityLogoUrl;
                else
                    if (actDesp.ActivityType == WfActivityType.CompletedActivity)
                        result = ControlResources.CompletedActivityLogoUrl;

            return result;
        }
        #endregion Private
    }

    //12-29 添加 临时
    public enum AdjustProcessAllowSelectFlagType
    {
        Allowed = 0,
        Unallowed = 1,
        Auto = 2,
    }
}
