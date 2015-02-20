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
using MCS.OA.Portal.Common;

namespace MCS.OA.Portal
{
    /// <summary>
    /// 全文检索应用名下拉框
    /// </summary>
    public class FullTextSearchApplicationDropDownList : PortalDropDownListBase
    {
        public override void InitData()
        {
            this.SelectAllText = "全部应用";
            this.SelectAllValue = string.Empty;
            this.InsertSelectAllOption();
            //ApplicationInfoCollection apps = ApplicationInfoConfig.GetConfig().Applications;
            //foreach (ApplicationInfo app in apps)
            //{
            //    if (app.EnableFullTextSearch)
            //        this.Items.Add(new ListItem(app.Description, app.Name));
            //}//Note:全部应用注销
        }
    }
}
