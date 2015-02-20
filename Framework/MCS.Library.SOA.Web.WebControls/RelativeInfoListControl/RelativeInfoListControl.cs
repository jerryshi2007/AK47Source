using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using System.Web.UI.WebControls;

[assembly: WebResource("MCS.Web.WebControls.RelativeInfoListControl.relativeLinkLogo.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.RelativeInfoListControl.relativeUserLogo.gif", "image/gif")]

namespace MCS.Web.WebControls
{
    public class RelativeLinkControl : RelativeInfoListControlBase
    {
        private WfRelativeLinkDescriptorCollection _RelativeLinks = null;
        private WfRelativeLinkDescriptorCollection _moreLinks = null;

        [Browsable(false)]
        public WfRelativeLinkDescriptorCollection RelativeLinks
        {
            get
            {
                if (this._RelativeLinks == null)
                {
                    IWfActivity currentActivity = GetDefaultActivity();

                    if (currentActivity != null)
                    {
                        this._RelativeLinks = currentActivity.Descriptor.RelativeLinks;

						//如果没有链接，则查找对应的分支流程模版
						if (this._RelativeLinks.Count == 0 && currentActivity.Process.EntryInfo != null)
							this._RelativeLinks = currentActivity.Process.EntryInfo.ProcessTemplate.RelativeLinks;

                        if (this._RelativeLinks.Count == 0)
							this._RelativeLinks = currentActivity.SameResourceRootActivity.Process.Descriptor.RelativeLinks;
                    }
                    else
                    {
                        this._RelativeLinks = new WfRelativeLinkDescriptorCollection();
                    }

                    if (this.Category.IsNotEmpty())
                        this._RelativeLinks = this._RelativeLinks.FilterByCategory(this.Category);
                }

                return this._RelativeLinks;
            }
        }

        [Browsable(false)]
        public WfRelativeLinkDescriptorCollection MoreLinks
        {
            get
            {
                if (this._moreLinks == null)
                {
                    IWfActivity currentActivity = GetDefaultActivity();

                    if (currentActivity != null)
                    {
                        this._moreLinks = currentActivity.Descriptor.RelativeLinks;

                        if (this._moreLinks.Count == 0)
							this._moreLinks = currentActivity.SameResourceRootActivity.Process.Descriptor.RelativeLinks;
                    }
                    else
                    {
                        this._moreLinks = new WfRelativeLinkDescriptorCollection();
                    }

                    if (this.MoreLinkCategory != "")
                        this._moreLinks = this._moreLinks.FilterByCategory(this.MoreLinkCategory);
                    else
                        this._moreLinks = new WfRelativeLinkDescriptorCollection();
                }
                return this._moreLinks;
            }
        }


        [DefaultValue("")]
        public string Category
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "Category", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "Category", value);
            }
        }

        [DefaultValue("MoreLinks")]
        public string MoreLinkCategory
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "MoreLinkCategory", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "MoreLinkCategory", value);
            }
        }

        public override string ImgIcon
        {
            get
            {
                return this.ViewState.GetViewStateValue("ImgIcon", string.Empty) == string.Empty ?
                    Page.ClientScript.GetWebResourceUrl(typeof(RelativeLinkControl),
                    "MCS.Web.WebControls.RelativeInfoListControl.relativeLinkLogo.gif") :
                    this.ViewState.GetViewStateValue("ImgIcon", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("ImgIcon", value);
            }
        }

        public override string PostStr
        {
            get
            {
                return JSONSerializerExecute.Serialize(this.RelativeLinks);
            }
        }

        protected override List<HtmlTableRow> GetRelativeRows()
        {
            List<HtmlTableRow> rows = new List<HtmlTableRow>();
            foreach (var item in RelativeLinks.Take(5))
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell = new HtmlTableCell();
                cell.Attributes["class"] = "linkCell";
                HtmlAnchor anchor = new HtmlAnchor();
                anchor.InnerText = item.Name;
                anchor.HRef = item.Url;
                anchor.Title = item.Description;
                anchor.Target = string.IsNullOrEmpty(LinkTarget) ? "_blank" : LinkTarget;
                cell.Controls.Add(anchor);
                row.Controls.Add(cell);
                rows.Add(row);
            }
            return rows;
        }

        protected override List<HtmlTableRow> GetMoreLinksRow()
        {
            List<HtmlTableRow> rows = new List<HtmlTableRow>();
            foreach (var item in MoreLinks.Take(5))
            {
                HtmlTableRow footerRow = new HtmlTableRow();
                HtmlTableCell footerCell = new HtmlTableCell();
                footerCell.Attributes["class"] = "footerCell";
                HyperLink link = new HyperLink();
                link.NavigateUrl = item.Url;
                link.Text = item.Name;
                link.Target = "_blank";
                link.Style["TEXT-DECORATION"] = "underline";
                //link.Attributes["onclick"] = string.Format("onOpenMoreClick('{0}','{1}','{2}')", this.ClientID + "_hidden", string.IsNullOrEmpty(this.MethodType) ? "post" : this.MethodType, this.LinkTarget);
                footerCell.Attributes["colspan"] = "2";
                footerCell.Controls.Add(link);
                footerRow.Controls.Add(footerCell);
                rows.Add(footerRow);
            }

            return rows;

        }
    }

    public class RelativeUserControl : RelativeInfoListControlBase
    {
        private WfExternalUserCollection _ExternalUser = null;
        private WfResourceDescriptorCollection _InternalUsers = null;

        /// <summary>
        /// 外部相关人员集合
        /// </summary>
        [Browsable(false)]
        public WfExternalUserCollection ExtrenalUsers
        {
            get
            {
                if (this._ExternalUser == null)
                {
                    IWfActivity currentActivity = GetDefaultActivity();

                    if (currentActivity != null)
                        this._ExternalUser = currentActivity.Descriptor.ExternalUsers;
                    else
                        this._ExternalUser = new WfExternalUserCollection();

                }

                return this._ExternalUser;
            }
        }

        public override string ImgIcon
        {
            get
            {
                return this.ViewState.GetViewStateValue("ImgIcon", string.Empty) == string.Empty ?
                    Page.ClientScript.GetWebResourceUrl(typeof(RelativeLinkControl),
                    "MCS.Web.WebControls.RelativeInfoListControl.relativeUserLogo.gif") :
                    this.ViewState.GetViewStateValue("ImgIcon", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("ImgIcon", value);

            }
        }

        public override string PostStr
        {
            get
            {
                WfConverterHelper.RegisterConverters();

                return JSONSerializerExecute.Serialize(this.ExtrenalUsers);
            }
        }

        protected override string ColSpan
        {
            get
            {
                return "2";
            }
        }

        [Browsable(false)]
        public WfResourceDescriptorCollection InternalRelativeUsers
        {
            get
            {
                if (this._InternalUsers == null)
                {
                    IWfActivity currentActivity = GetDefaultActivity();

                    if (currentActivity != null)
                        this._InternalUsers = currentActivity.Descriptor.InternalRelativeUsers;
                    else
                        this._InternalUsers = new WfResourceDescriptorCollection();

                }

                return this._InternalUsers;
            }
        }

        protected override List<HtmlTableRow> GetRelativeRows()
        {
            List<HtmlTableRow> rows = new List<HtmlTableRow>();
            foreach (WfUserResourceDescriptor item in InternalRelativeUsers)
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell = new HtmlTableCell();
                cell.Attributes["class"] = "userCell";

                UserPresence userPresence = new UserPresence();
                userPresence.UserID = item.User.ID;
                userPresence.UserDisplayName = item.User.DisplayName;

                cell.Controls.Add(userPresence);
                HtmlTableCell userTitleCell = new HtmlTableCell();
                userTitleCell.InnerText = item.User.Occupation;
                userTitleCell.Attributes["class"] = "userTitleCell";
                row.Controls.Add(userTitleCell);
                row.Controls.Add(cell);
                rows.Add(row);
            }

            if (rows.Count >= 5)
                return rows;

            foreach (WfExternalUser item in ExtrenalUsers.Take(5 - InternalRelativeUsers.Count))
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell = new HtmlTableCell();
                cell.Attributes["class"] = "userCell";

                HtmlTableCell userTitleCell = new HtmlTableCell();
                userTitleCell.InnerText = item.Title;
                userTitleCell.Attributes["class"] = "userTitleCell";
                row.Controls.Add(userTitleCell);

                string interRelativeUserHTML = string.Empty;
                interRelativeUserHTML += item.Name;
                cell.InnerHtml = interRelativeUserHTML;
                row.Controls.Add(cell);
                rows.Add(row);
            }

            return rows;
        }

    }
    /*
    public class RelativePlanTimeControl : RelativeInfoListControlBase
    {
        [DefaultValue("")]
        public DateTime? CompleteTime
        {
            get
            {
                DateTime? result = this.ViewState.GetViewStateValue("completeTime", (DateTime?)null);

                if (result.HasValue == false)
                {
                    IWfActivity currentActivity = GetDefaultActivity();

                    if (currentActivity != null)
                        result = currentActivity.Descriptor.EstimateEndTime;
                }

                return result;
            }
            set
            {
                this.ViewState.SetViewStateValue("completeTime", value);
            }

        }
        public override string ImgIcon
        {
            get
            {
                return this.ViewState.GetViewStateValue("ImgIcon", string.Empty) == string.Empty ?
                    Page.ClientScript.GetWebResourceUrl(typeof(RelativeLinkControl),
                    "MCS.Web.WebControls.RelativeInfoListControl.relativeUserLogo.gif") :
                    this.ViewState.GetViewStateValue("ImgIcon", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("ImgIcon", value);

            }
        }

        protected override List<HtmlTableRow> GetRelativeRows()
        {

            List<HtmlTableRow> rows = new List<HtmlTableRow>();
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();

            if (CompleteTime.HasValue && CompleteTime.Value != DateTime.MinValue)
                cell.InnerText = CompleteTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

            row.Controls.Add(cell);
            rows.Add(row);

            return rows;
        }
    }
    */
}
