using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;

namespace MCS.OA.Portal
{
    public class PortalDropDownListBase : DropDownList
    {
        private string selectAllText = string.Empty;
        private string selectAllValue = string.Empty;

        /// <summary>
        /// 全选显示的文字，如果为空，则不显示全选。默认为string.Empty
        /// </summary>
        [WebDescription("全选显示的文字(如果为空，则不显示全选。默认为string.Empty)")]
        public string SelectAllText
        {
            get
            {
                return this.selectAllText;
            }
            set
            {
                this.selectAllText = value;
            }
        }

        /// <summary>
        /// 全选显示的值。默认为string.Empty
        /// </summary>
        [WebDescription("全选显示的值。默认为string.Empty)")]
        public string SelectAllValue
        {
            get
            {
                return this.selectAllValue;
            }
            set
            {
                this.selectAllValue = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.Page.IsPostBack)
                this.InitData();

            base.OnLoad(e);
        }

        /// <summary>
        /// 准备数据
        /// </summary>
        public virtual void InitData()
        {
        }

        /// <summary>
        /// 使用有标记EnumItemDescription属性的枚举类型绑定对象
        /// </summary>
        /// <param name="hbEnum">枚举类型</param>
        protected virtual void BindHBEnum(Type hbEnum)
        {
            ExceptionHelper.FalseThrow(hbEnum.IsEnum, "hbEnum");
            this.BindSource(EnumItemDescriptionAttribute.GetDescriptionList(hbEnum), "Description", "EnumValue");
        }

        /// <summary>
        /// 绑定对象
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataTextField">显示的属性</param>
        /// <param name="dataValueField">value的属性</param>
        protected virtual void BindSource(object dataSource, string dataTextField, string dataValueField)
        {
            this.DataSource = dataSource;
            this.DataTextField = dataTextField;
            this.DataValueField = dataValueField;
            this.DataBind();

            if (string.IsNullOrEmpty(this.selectAllText) == false)
                this.InsertSelectAllOption();
        }

        /// <summary>
        /// 插入全选行
        /// </summary>
        protected virtual void InsertSelectAllOption()
        {
            this.Items.Insert(0, new ListItem(this.selectAllText, this.selectAllValue));
        }
    }
}
