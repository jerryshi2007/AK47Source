using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Globalization;

namespace MCS.OA.CommonPages.UserOperationLog
{
    public partial class UploadFileLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack == false)
            {
                InitUploadFileLogDetailInfoById(this.Request.Params["id"]);
                //InitDetailInfo(Request.QueryString["id"]);
            }
        }

        private void InitUploadFileLogDetailInfoById(string ID)
        {
            int id;
            if (int.TryParse(ID, out id))
            {
                UploadFileHistory uploadFilelog = UploadFileHistoryAdapter.Instance.Load(id);
                if(uploadFilelog !=null)
                    this.SetPageControlValue(uploadFilelog);
            }
        }

        /// <summary>
        /// 设置页面显示控件值
        /// </summary>
        /// <param name="log"></param>
        private void SetPageControlValue(UploadFileHistory log)
        {
            this.lb_ApplicationName.Text = HttpUtility.HtmlEncode((log.ApplicationName == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : log.ApplicationName);
            this.lb_CreateTime.Text = HttpUtility.HtmlEncode((log.CreateTime.ToString() == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : string.Format("{0:yyyy-MM-dd HH:mm:ss}", log.CreateTime));
            this.lb_CurrentFileName.Text = HttpUtility.HtmlEncode((log.CurrentFileName == string.Empty) ? Translator.Translate(Define.DefaultCulture, "无") : log.CurrentFileName);
            this.lb_Operator.Text = HttpUtility.HtmlEncode(string.IsNullOrEmpty(log.Operator.DisplayName) ? Translator.Translate(Define.DefaultCulture, "无") : log.Operator.DisplayName);
            this.lb_OriginalFileName.Text = HttpUtility.HtmlEncode(string.IsNullOrEmpty(log.OriginalFileName) ? Translator.Translate(Define.DefaultCulture, "无") : log.OriginalFileName);
            this.lb_ProgramName.Text = HttpUtility.HtmlEncode(string.IsNullOrEmpty(log.ProgramName) ? Translator.Translate(Define.DefaultCulture, "无") : log.ProgramName);
            this.lb_StatusText.Text = HttpUtility.HtmlEncode(string.IsNullOrEmpty(log.StatusText) ? Translator.Translate(Define.DefaultCulture, "无") : log.StatusText);
        }

    }
}