using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.OA.Portal;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.Portal.Common
{
    /// <summary>
    /// 门户应用名下拉框
    /// </summary>
    public class ApplicationNameDropDownList : PortalDropDownListBase
    {
        public override void InitData()
        {
            this.SelectAllText = "全部应用";
            this.SelectAllValue = string.Empty;
            this.InsertSelectAllOption();
            //ApplicationInfoCollection apps = ApplicationInfoConfig.GetConfig().Applications;
            //foreach (ApplicationInfo app in apps)
            //{
            //    this.Items.Add(new ListItem(app.Description, app.Name));
            //}//Note:全部应用注销
        }
    }
}
