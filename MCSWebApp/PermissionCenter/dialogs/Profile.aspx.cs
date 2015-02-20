using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;

namespace PermissionCenter.Dialogs
{
	public partial class Profile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false)
			{
				foreach (var att in typeof(Profile).Assembly.GetCustomAttributes(true))
				{
					if (att is System.Reflection.AssemblyFileVersionAttribute)
					{
						this.lbVersion.Text = ((System.Reflection.AssemblyFileVersionAttribute)att).Version;
						break;
					}
				}

				var cate = UserSettings.GetSettings(Util.CurrentUser.ID).Categories["PermissionCenter"];

				this.BindToView(cate);

				tabMaintain.Visible = Util.SuperVisiorMode;

				this.lnkPassword.NavigateUrl = System.Configuration.ConfigurationManager.AppSettings["passworControlUrl"];
			}
		}

		private void BindToView(UserSettingsCategory cate)
		{
			var ouMode = (OuBrowseViewMode)cate.Properties.GetValue("OuBrowseView", 0);
			var perPageSize = (int)cate.Properties.GetValue("PerPageSizeOption", 0);
			var userViewFlags = (UserBrowseViewMode)cate.Properties.GetValue("UserBrowseView", 0);
			var generalViewFlags = (GeneralViewMode)cate.Properties.GetValue("GeneralBrowseView", 0);

			this.ddlSizePerPage.SelectedIndex = perPageSize;

			if ((ouMode & OuBrowseViewMode.Fixed) == OuBrowseViewMode.Fixed)
			{
				if ((ouMode & OuBrowseViewMode.Hierarchical) == OuBrowseViewMode.Hierarchical)
				{
					this.orgModeHierarchical.Checked = true;
				}
				else
				{
					this.orgModeList.Checked = true;
				}
			}
			else
			{
				this.orgModeAuto.Checked = true;
			}

			if ((userViewFlags & UserBrowseViewMode.Fixed) == UserBrowseViewMode.Fixed)
			{
				if ((userViewFlags & UserBrowseViewMode.ReducedTable) == UserBrowseViewMode.ReducedTable)
				{
					this.userViewTable.Checked = true;
				}
				else if ((userViewFlags & UserBrowseViewMode.ReducedList) == UserBrowseViewMode.ReducedList)
				{
					this.userViewReduced.Checked = true;
				}
				else
				{
					this.userViewDetail.Checked = true;
				}
			}
			else
			{
				this.userViewAuto.Checked = true;
			}

			if ((generalViewFlags & GeneralViewMode.Fixed) == GeneralViewMode.Fixed)
			{
				if ((generalViewFlags & GeneralViewMode.Table) == GeneralViewMode.Table)
				{
					this.generalViewTable.Checked = true;
				}
				else
				{
					this.generalViewList.Checked = true;
				}
			}
			else
			{
				this.generalViewAuto.Checked = true;
			}
		}

		protected void HandleSave(object sender, EventArgs e)
		{
			try
			{
				var settings = UserSettings.GetSettings(Util.CurrentUser.ID);

				this.SaveSettings(settings.Categories["PermissionCenter"]);

				settings.Update();

				this.notice.Text = "保存成功";
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
			}
		}

		private void SaveSettings(UserSettingsCategory cate)
		{
			var ouMode = (OuBrowseViewMode)cate.Properties.GetValue("OuBrowseView", 0);
			var userViewFlags = (UserBrowseViewMode)cate.Properties.GetValue("UserBrowseView", 0);
			var generalViewFlags = (GeneralViewMode)cate.Properties.GetValue("GeneralBrowseView", 0);

			if (this.orgModeAuto.Checked)
			{
				ouMode = ouMode & (~OuBrowseViewMode.Fixed);
			}
			else if (this.orgModeHierarchical.Checked)
			{
				ouMode = OuBrowseViewMode.Hierarchical | OuBrowseViewMode.Fixed;
			}
			else
			{
				ouMode = OuBrowseViewMode.List | OuBrowseViewMode.Fixed;
			}

			if (this.userViewAuto.Checked)
			{
				userViewFlags = userViewFlags & (~UserBrowseViewMode.Fixed);
			}
			else if (userViewTable.Checked)
			{
				userViewFlags = UserBrowseViewMode.ReducedTable | UserBrowseViewMode.Fixed;
			}
			else if (userViewReduced.Checked)
			{
				userViewFlags = UserBrowseViewMode.ReducedList | UserBrowseViewMode.Fixed;
			}
			else
			{
				userViewFlags = UserBrowseViewMode.DetailList | UserBrowseViewMode.Fixed;
			}

			if (this.generalViewAuto.Checked)
			{
				generalViewFlags = generalViewFlags & (~GeneralViewMode.Fixed);
			}
			else if (this.generalViewList.Checked)
			{
				generalViewFlags = GeneralViewMode.Fixed | GeneralViewMode.List;
			}
			else
			{
				generalViewFlags = GeneralViewMode.Fixed | GeneralViewMode.Table;
			}

			cate.Properties.SetValue("OuBrowseView", (int)ouMode);
			cate.Properties.SetValue("PerPageSizeOption", this.ddlSizePerPage.SelectedIndex);
			cate.Properties.SetValue("UserBrowseView", (int)userViewFlags);
			cate.Properties.SetValue("GeneralBrowseView", (int)generalViewFlags);
		}
	}
}