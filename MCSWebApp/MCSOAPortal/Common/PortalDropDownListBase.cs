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
        /// ȫѡ��ʾ�����֣����Ϊ�գ�����ʾȫѡ��Ĭ��Ϊstring.Empty
        /// </summary>
        [WebDescription("ȫѡ��ʾ������(���Ϊ�գ�����ʾȫѡ��Ĭ��Ϊstring.Empty)")]
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
        /// ȫѡ��ʾ��ֵ��Ĭ��Ϊstring.Empty
        /// </summary>
        [WebDescription("ȫѡ��ʾ��ֵ��Ĭ��Ϊstring.Empty)")]
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
        /// ׼������
        /// </summary>
        public virtual void InitData()
        {
        }

        /// <summary>
        /// ʹ���б��EnumItemDescription���Ե�ö�����Ͱ󶨶���
        /// </summary>
        /// <param name="hbEnum">ö������</param>
        protected virtual void BindHBEnum(Type hbEnum)
        {
            ExceptionHelper.FalseThrow(hbEnum.IsEnum, "hbEnum");
            this.BindSource(EnumItemDescriptionAttribute.GetDescriptionList(hbEnum), "Description", "EnumValue");
        }

        /// <summary>
        /// �󶨶���
        /// </summary>
        /// <param name="dataSource">����Դ</param>
        /// <param name="dataTextField">��ʾ������</param>
        /// <param name="dataValueField">value������</param>
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
        /// ����ȫѡ��
        /// </summary>
        protected virtual void InsertSelectAllOption()
        {
            this.Items.Insert(0, new ListItem(this.selectAllText, this.selectAllValue));
        }
    }
}
