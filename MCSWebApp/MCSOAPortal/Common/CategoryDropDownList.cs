using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using MCS.OA.Portal;
using MCS.Library.Principal;

namespace MCS.OA.Portal.Common
{
    public class CategoryDropDownList : PortalDropDownListBase
    {
		/// <summary>
		/// 门户任务类别下拉框
		/// </summary>
        public override void InitData()
        {
            this.Items.Clear();
            this.SelectAllText = "全部类别";
            this.SelectAllValue = string.Empty;
            this.InsertSelectAllOption();
            TaskCategoryCollection taskCategories = TaskCategoryAdapter.Instance.GetCategoriesByUserID(DeluxeIdentity.CurrentUser.ID);
           
            foreach (TaskCategory category in taskCategories)
            {
                this.Items.Add(new ListItem(category.CategoryName, category.CategoryID));
            }
			this.Items.Add(new ListItem("未分类", "Others"));
            /*
            this.Items.Add(new ListItem("通用流程", "Others"));
            this.Items.Add(new ListItem("连锁开发系统", "1"));
            this.Items.Add(new ListItem("门店管理系统", "2"));
             */ 
        }
    }
}
