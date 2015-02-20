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
    /// �Ż�Ӧ����������
    /// </summary>
    public class ApplicationNameDropDownList : PortalDropDownListBase
    {
        public override void InitData()
        {
            this.SelectAllText = "ȫ��Ӧ��";
            this.SelectAllValue = string.Empty;
            this.InsertSelectAllOption();
            //ApplicationInfoCollection apps = ApplicationInfoConfig.GetConfig().Applications;
            //foreach (ApplicationInfo app in apps)
            //{
            //    this.Items.Add(new ListItem(app.Description, app.Name));
            //}//Note:ȫ��Ӧ��ע��
        }
    }
}
