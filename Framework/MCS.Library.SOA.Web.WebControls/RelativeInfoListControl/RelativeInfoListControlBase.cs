using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using System.Reflection;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
    public class RelativeInfoListControlBase : WebControl
    {
        public RelativeInfoListControlBase()
            : base(HtmlTextWriterTag.Div)
        { }

        #region Properties

        [DefaultValue("")]
        public string Title
        {
            get
            {
                return this.ViewState.GetViewStateValue("Title", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("Title", value);
            }
        }

        [DefaultValue("MoreLinks")]
        public string SeeMoreLinkUrl
        {
            get
            {
                return this.ViewState.GetViewStateValue("SeeMoreLinkUrl", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("SeeMoreLinkUrl", value);
            }
        }

        public virtual string ImgIcon
        {
            get
            {
                return this.ViewState.GetViewStateValue("ImgIcon", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("ImgIcon", value);
            }
        }

        public string LinkTarget
        {
            get
            {
                return this.ViewState.GetViewStateValue("LinkTarget", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("LinkTarget", value);
            }

        }

        [DefaultValue("post")]
        public string MethodType
        {
            get
            {
                return this.ViewState.GetViewStateValue("MethodType", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("MethodType", value);
            }
        }

        #endregion

        protected virtual IWfActivity GetDefaultActivity()
        {
            IWfActivity result = null;

            if (WfClientContext.Current.CurrentActivity != null)
                result = WfClientContext.Current.CurrentActivity.ApprovalRootActivity;

            return result;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
            {
                writer.Write("Relative Link Control");
            }

            base.Render(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegisterCSS();
            string script = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                "MCS.Web.WebControls.RelativeInfoListControl.RelativeInfoListControl.js");

            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RelativeInfoListClient", script, true);

        }

        private string GetFormStr()
        {
            StringBuilder strb = new StringBuilder();
            strb.Append(string.Format("<input id=\"{0}\" name=\"hiddenJsonStr\" value='{1}' type=\"hidden\" />", this.ClientID + "_hidden", PostStr));
            strb.Append(string.Format("<input id=\"{0}\" value='{1}' type=\"hidden\" />", this.ClientID + "_method", string.IsNullOrEmpty(this.MethodType) ? "post" : this.MethodType));
            strb.Append(string.Format("<input id=\"{0}\" value='{1}' type=\"hidden\" />", this.ClientID + "_action", this.LinkTarget));

            return strb.ToString();
        }

        protected HtmlGenericControl WfRelativeHTML()
        {
            HtmlGenericControl tblBorder = new HtmlGenericControl("div");
            tblBorder.Attributes["class"] = "relativeTableContainer";
            tblBorder.ID = this.ClientID + "_div";
            tblBorder.Controls.Add(GetRelativeTable());
            //form.Controls.Add(tblBorder);
            HtmlGenericControl divHiddenContainer = new HtmlGenericControl("div");
            divHiddenContainer.Style["display"] = "none";
            divHiddenContainer.InnerHtml = this.GetFormStr();
            tblBorder.Controls.Add(divHiddenContainer);
            return tblBorder;
        }

        protected override void CreateChildControls()
        {
            this.Controls.Add(WfRelativeHTML());
            base.CreateChildControls();
        }

        private HtmlTable GetRelativeTable()
        {
            HtmlTable tbl = new HtmlTable();
            tbl.Attributes["class"] = "relativeTable";
            HtmlTableRow headerRow = new HtmlTableRow();
            HtmlTableCell headerCell = new HtmlTableCell();
            HtmlGenericControl imgSpan = new HtmlGenericControl("span");
            HtmlImage image = new HtmlImage();

            image.Src = ImgIcon;
            imgSpan.Controls.Add(image);
            HtmlGenericControl textSpan = new HtmlGenericControl("span");
            //headerCell.InnerText = wfRelativeKey;
            headerCell.Attributes["colspan"] = ColSpan;
            textSpan.InnerText = this.Title;
            textSpan.Attributes["class"] = "titleSpan";
            headerCell.Controls.Add(imgSpan);
            headerCell.Controls.Add(textSpan);
            headerRow.Controls.Add(headerCell);
            tbl.Rows.Add(headerRow);

            foreach (HtmlTableRow row in GetRelativeRows())
            {
                tbl.Rows.Add(row);
            }
            foreach (HtmlTableRow row in GetMoreLinksRow())
            {
                tbl.Rows.Add(row);
            }

            //HtmlTableRow footerRow = new HtmlTableRow();
            //HtmlTableCell footerCell = new HtmlTableCell();
            //footerCell.Attributes["class"] = "footerCell";
            //HyperLink link = new HyperLink();
            //link.NavigateUrl = "#";//string.IsNullOrEmpty(SeeMoreLinkUrl) ? "" : SeeMoreLinkUrl;
            //link.Text = "更多";
            ////link.Target = "_blank";
            //link.Style["TEXT-DECORATION"] = "underline";
            //link.Attributes["onclick"] = string.Format("onOpenMoreClick('{0}','{1}','{2}')", this.ClientID + "_hidden", string.IsNullOrEmpty(this.MethodType) ? "post" : this.MethodType, this.LinkTarget);
            //footerCell.Attributes["colspan"] = "2";
            //footerCell.Controls.Add(link);
            //footerRow.Controls.Add(footerCell);
            //tbl.Controls.Add(footerRow);
            return tbl;

        }

        private void RegisterCSS()
        {
            string css = ResourceHelper.LoadStringFromResource(
                Assembly.GetExecutingAssembly(),
                "MCS.Web.WebControls.RelativeInfoListControl.base.css");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StatusCSS",
                string.Format("<style type='text/css'>{0}</style>", css));
        }


        protected virtual List<HtmlTableRow> GetRelativeRows()
        {
            return new List<HtmlTableRow>();
        }

        protected virtual List<HtmlTableRow> GetMoreLinksRow()
        {
            return new List<HtmlTableRow>();
        }


        public virtual string PostStr
        {
            get
            {
                return string.Empty;
            }
        }

        [DefaultValue("1")]
        protected virtual string ColSpan
        {
            get
            {
                return "1";
            }
        }
    }
}
