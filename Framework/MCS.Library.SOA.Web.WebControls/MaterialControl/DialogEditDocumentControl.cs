#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	DialogEditDocument.cs
// Remark	：	编辑文档
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			冯立雷		20111128		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.MaterialControl.DialogEditDocumentControl.js", "application/x-javascript")]
namespace MCS.Web.WebControls
{
    /// <summary>
    /// 编辑文档
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [RequiredScript(typeof(MaterialScript), 4)]
    [ClientScriptResource("MCS.Web.WebControls.DialogEditDocumentControl",
        "MCS.Web.WebControls.MaterialControl.DialogEditDocumentControl.js")]
    [ParseChildren(true), PersistChildren(true),]
    internal class DialogEditDocumentControl : ScriptControlBase
    {
        public DialogEditDocumentControl()
			: base(true, System.Web.UI.HtmlTextWriterTag.Div)
		{
		}

        #region 私有变量

        private OfficeViewerWrapper officeViewerWrapper = new OfficeViewerWrapper();

        #endregion

        #region 属性

        /// <summary>
        /// OfficeViewerWrapper控件ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("officeViewerWrapperID")]
        private string OfficeViewerWrapperID
        {
            get
            {
                return this.officeViewerWrapper.ClientID;
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            this.Controls.Add(this.officeViewerWrapper);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterOfficeViewerClientEventHandler("BeforeDocumentSaved()", GetBeforDocumentSavedScript());
        }

        private void RegisterOfficeViewerClientEventHandler(string activeXEventName, string script)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("<script language=\"javascript\" for=\"{0}\" event=\"{1}\">\n",
                                this.OfficeViewerWrapperID + "_Viewer",
                                activeXEventName);

            result.Append(script);
            result.Append("\n</script>\n");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.OfficeViewerWrapperID + "_beforeSave",
                                                        result.ToString());
        }

        private string GetBeforDocumentSavedScript()
        {
            string result = string.Format("$find(\"{0}\").raiseBeforDocumentSaved();", this.ClientID);
            return result;
        }
    }
}
