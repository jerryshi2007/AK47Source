using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.RightSideNavigator.RightSideNavigator.js", "text/javascript")]
[assembly: WebResource("MCS.Web.WebControls.RightSideNavigator.RightSideNavigator.css", "text/css")]

namespace MCS.Web.WebControls
{
	[ClientCssResource("MCS.Web.WebControls.RightSideNavigator.RightSideNavigator.css")]
	[ClientScriptResource("MCS.Web.WebControls.RightSideNavigator", "MCS.Web.WebControls.RightSideNavigator.RightSideNavigator.js")]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[Designer(typeof(RightSideNavigatorDesigner))]
	public class RightSideNavigator : ScriptControlBase
	{
		private static readonly string ControlKey = "RightSideNavigator";
		private RelativeLinkCategoryCollection _RelativeLinkCategories = new RelativeLinkCategoryCollection();
		private WfRelativeLinkDescriptorCollection _relativeLinks = null;

		public RightSideNavigator()
			: base(HtmlTextWriterTag.Div)
		{
			base.CssClass = "mui-mbar";
		}

		[Category("Default")]
		[Description("分类")]
		public string CustomerServiceCategory
		{
			get { return GetPropertyValue("CustomerServiceCategory", string.Empty); }
			set { SetPropertyValue("CustomerServiceCategory", value); }
		}

		[PersistenceMode(PersistenceMode.InnerProperty), Description("相关链接类别")]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue((string)null)]
		[Browsable(false)]
		public RelativeLinkCategoryCollection RelativeLinkCategories
		{
			get { return this._RelativeLinkCategories; }
		}

		[Browsable(false)]
		public WfRelativeLinkDescriptorCollection RelativeLinks
		{
			get
			{
				if (_relativeLinks == null)
				{
					var currentActivity = GetDefaultActivity();

					if (currentActivity != null)
					{
						this._relativeLinks = currentActivity.Descriptor.RelativeLinks;

						if (this._relativeLinks.Count == 0)
						{
							this._relativeLinks = currentActivity.Process.Descriptor.RelativeLinks;

							if (this._relativeLinks.Count == 0)
							{
								if (currentActivity.Process.EntryInfo != null)
									this._relativeLinks = currentActivity.Process.EntryInfo.ProcessTemplate.RelativeLinks;

								if (this._relativeLinks.Count == 0)
									this._relativeLinks = currentActivity.SameResourceRootActivity.Process.Descriptor.RelativeLinks;
							}
						}
					}
					else
					{
						this._relativeLinks = new WfRelativeLinkDescriptorCollection();
					}
				}

				return this._relativeLinks;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			ExceptionHelper.TrueThrow(HttpContext.Current.Items.Contains(ControlKey), "页面中只能有一个RightSideNavigator控件！");
			HttpContext.Current.Items.Add(ControlKey, true);
			base.OnInit(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!DesignMode)
			{
				RenderCustomerService();
				RenderRelativeLinks();
			}

			base.OnPreRender(e);
		}

		[Browsable(false)]
		protected IWfActivity GetDefaultActivity()
		{
			IWfActivity result = null;

			if (WfClientContext.Current.OriginalActivity != null)
				result = WfClientContext.Current.OriginalActivity.ApprovalRootActivity;

			return result;
		}

		private void RenderCustomerService()
		{
			var users = CutomerServiceExecutiveSetting.GetConfig().CutomerServiceExecutiveQuery.QueryCutomerServiceExecutive(CustomerServiceCategory.Split(','));

			if (users.Any())
			{
				var divCustomerServiceControl = new HtmlGenericControl("div") { ID = "divCustomerService" };
				divCustomerServiceControl.Attributes["class"] = "muiFirst mui-msg";
				divCustomerServiceControl.Controls.Add(new HtmlGenericControl("div") { InnerText = "在线客服" });
				this.Controls.Add(divCustomerServiceControl);

				var divPanel = new HtmlGenericControl("div") { ID = divCustomerServiceControl.ID + "_panel" };
				divPanel.Attributes["class"] = "mui";
				divPanel.Style[HtmlTextWriterStyle.Display] = "none";
				divCustomerServiceControl.Controls.Add(divPanel);

				var divTitle = new HtmlGenericControl("div") { InnerText = "◆ 在线客服" };
				divTitle.Attributes["class"] = "mui-title";
				divPanel.Controls.Add(divTitle);

				var ul = new HtmlGenericControl("ul");
				ul.Attributes["class"] = "mui-content text-left ";
				divPanel.Controls.Add(ul);

				foreach (var u in users)
				{
					var li = new HtmlGenericControl("li");
					UserPresence presence = new UserPresence();

					presence.UserID = u.ID;
					presence.UserDisplayName = u.DisplayName;

					li.Controls.Add(presence);
					ul.Controls.Add(li);

					presence.EnsureInUserList();
				}
			}
		}

		private void RenderRelativeLinks()
		{
			if (this.RelativeLinkCategories.Count > 0)
			{
				var i = 0;
				foreach (var linkCategory in this.RelativeLinkCategories)
				{
					var divLinkCategory = new HtmlGenericControl("div") { ID = "divLinkCategory_" + i++ };
					divLinkCategory.Attributes["class"] = "mui-msg";
					divLinkCategory.Controls.Add(new HtmlGenericControl("div") { InnerText = linkCategory.Title });
					this.Controls.Add(divLinkCategory);

					if (linkCategory.Links.Count > 0)
					{
						var divPanel = new HtmlGenericControl("div") { ID = divLinkCategory.ID + "_panel" };
						divPanel.Attributes["class"] = "mui";
						divPanel.Style[HtmlTextWriterStyle.Display] = "none";
						divLinkCategory.Controls.Add(divPanel);

						foreach (var link in linkCategory.Links)
						{
							var divTitle = new HtmlGenericControl("div") { InnerText = "◆ " + link.Title };
							divTitle.Attributes["class"] = "mui-title";
							divPanel.Controls.Add(divTitle);

							var ul = new HtmlGenericControl("ul");
							ul.Attributes["class"] = "mui-content text-left ";
							divPanel.Controls.Add(ul);

							if (this.RelativeLinks.Count > 0)
							{
								if (link.CategoryName.IsNotEmpty())
								{
									var links = this.RelativeLinks.FilterByCategory(link.CategoryName);

									foreach (var relativeLink in links)
									{
										var li = new HtmlGenericControl("li");
										ul.Controls.Add(li);
										var a = new HtmlAnchor();
										a.InnerText = relativeLink.Name;
										a.HRef = relativeLink.Url;
										a.Target = "_blank";
										li.Controls.Add(a);
									}
								}

								if (link.MoreCategoryName.IsNotEmpty())
								{
									var moreLinks = this.RelativeLinks.FilterByCategory(link.MoreCategoryName);
									foreach (var moreLink in moreLinks)
									{
										var li = new HtmlGenericControl("li");
										li.Style["text-align"] = "right";
										li.Style["font-size"] = "11px";
										ul.Controls.Add(li);

										var a = new HtmlAnchor();
										a.InnerText = moreLink.Name;
										a.HRef = moreLink.Url;
										a.Target = "_blank";
										li.Controls.Add(a);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
