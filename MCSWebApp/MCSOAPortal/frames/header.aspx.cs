using System;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.OA.Portal.frames
{
    public partial class header : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RenderPersonalInfo();
                DateTime now = DateTime.Now;
                string weekstr = DateTime.Now.DayOfWeek.ToString();
                switch (weekstr)
                {
                    case "Monday": weekstr = "星期一"; break;
                    case "Tuesday": weekstr = "星期二"; break;
                    case "Wednesday": weekstr = "星期三"; break;
                    case "Thursday": weekstr = "星期四"; break;
                    case "Friday": weekstr = "星期五"; break;
                    case "Saturday": weekstr = "星期六"; break;
                    case "Sunday": weekstr = "星期日"; break;
                }
                lblDateTime.Text = now.Year + "年" + now.Month + "月" + now.Day + "日" + " " + weekstr;
            }
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        private void RenderPersonalInfo()
        {
            lblUserName.Text = Server.HtmlEncode("您好！" + DeluxeIdentity.CurrentUser.DisplayName);
            WfDelegationCollection delegationCollection = WfDelegationAdapter.Instance.GetUserActiveDelegations(DeluxeIdentity.CurrentUser.ID);
            if (delegationCollection.Count > 0)
            {
                lblUserName.Style["color"] = "#ddd";
                imgDelegate.Style["display"] = "";
            }
        }
    }
}