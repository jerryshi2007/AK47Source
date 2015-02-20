using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls.RatingControl
{

    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.RelatedPersonInputControl", "MCS.Web.WebControls.WorkItemControl.PersonInput.RelatedPersonInputControl.js")]
    [DialogContent("MCS.Web.WebControls.WorkItemControl.PersonInput.RelatedPersonInputControlTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
    [ToolboxData("<{0}:RelatedPersonInputControl runat=server></{0}:RelatedPersonInputControl>")]
    public class RatingControl : ScriptControlBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RatingControl()
        {
        }

        #region 重写的方法
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            base.OnPagePreLoad(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        #endregion
    }
}
