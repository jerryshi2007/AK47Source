using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Web.Library.Script;

//[assembly: WebResource("MCS.Web.WebControls.Workflow.WfActivitiesSelector.WfActivitiesSelectorControl.htm", "text/html")]
[assembly: WebResource("MCS.Web.WebControls.Workflow.WfActivitiesSelector.WfActivitiesSelectorControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 流程的活动点选择对话框控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.WfActivitiesSelectorControl", "MCS.Web.WebControls.Workflow.WfActivitiesSelector.WfActivitiesSelectorControl.js")]
    [DialogContent("MCS.Web.WebControls.Workflow.WfActivitiesSelector.WfActivitiesSelectorControl.htm", "MCS.Library.SOA.Web.WebControls")]
    [ToolboxData("<{0}:WfActivitiesSelectorControl runat=server></{0}:WfActivitiesSelectorControl>")]
    public class WfActivitiesSelectorControl : WfProcessDialogControlBase
    {
        private WfProcessActivitiesListControl activityListControl = new WfProcessActivitiesListControl();

        #region Protected
        protected override void InitDialogContent(Control container)
        {
            base.InitDialogContent(container);

            container.Controls.Add(this.activityListControl);
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.activityListControl.Process = ControlParams.Process;

            base.OnPreRender(e);
        }

        protected override void InitConfirmButton(HtmlInputButton confirmButton)
        {
            confirmButton.Attributes["onclick"] = "onConfirmClick();";
        }
        #endregion
    }
}
