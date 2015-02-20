using System;
using System.Collections;
using System.ComponentModel;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.WebControls
{
	[Designer("MCS.Web.WebControls.GenericControlDesigner, MCS.Web.WebControls")]
	[ToolboxData(@"<{0}:Banner runat=""server"" />")]
	public partial class Banner : System.Web.UI.UserControl
	{
		private HyperLink[] menuitems = null;

		public event EventHandler TimePointChanged;

		public int ActiveMenuIndex { get; set; }

		protected override void OnInit(EventArgs e)
		{
			this.menuitems = new HyperLink[]
			{
				this.bannerBtnAllMembers,
				this.bannerBtnRootSchemas,
				this.bannerBtnOrgs,
				this.bannerBtnAllGroups,
				this.bannerBtnApps,
				this.bannerBtnLogs
			};

			base.OnInit(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			HyperLink hy = null;

			if (this.ActiveMenuIndex >= 0 && this.ActiveMenuIndex < this.menuitems.Length)
				hy = this.menuitems[this.ActiveMenuIndex];

			if (hy != null)
				hy.CssClass += " focus";

			this.btnPickTime.Attributes.Add("data-rlctl", this.bannerCustomTime.ClientID);

			this.lnkPassword.NavigateUrl = System.Configuration.ConfigurationManager.AppSettings["passworControlUrl"];

			this.BindOrganizationList();

			this.lnkSysMan.Visible = Util.SuperVisiorMode;

			this.RefreshTimeShuttle();
		}

		protected void HandleItemCommand(object source, RepeaterCommandEventArgs e)
		{
			DateTime timePoint = DateTime.Parse(e.CommandArgument.ToString());

			switch (e.CommandName)
			{
				case "TimeShuttle":
					this.ShuttleToTimePoint(timePoint);
					break;
				case "Delete":
					this.DeleteTimePoint(timePoint);
					break;
			}
		}

		/// <summary>
		/// 引发<see cref="TimePointChanged"/>事件
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnTimePointChanged(EventArgs e)
		{
			if (this.TimePointChanged != null)
				this.TimePointChanged(this, e);
		}

		protected void ShuttleNow(object sender, EventArgs e)
		{
			ChangeAndSaveTimePoint(DateTime.MinValue);

			this.OnTimePointChanged(EventArgs.Empty);
		}

		protected void ShuttleAny(object sender, EventArgs e)
		{
			long timespan = long.Parse(this.bannerCustomTime.Value);
			DateTime dt = Util.FromJavascriptTime(timespan);

			this.ShuttleToTimePoint(dt);
		}

		/// <summary>
		/// 清除所有条目
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ClearRecent(object sender, EventArgs e)
		{
			var category = UserRecentData.GetSettings(DeluxeIdentity.CurrentUser.ID, "recentTimepoints");

			category.Items.Clear();

			category.SaveChanges(DeluxeIdentity.CurrentUser.ID);
		}

		protected void bannerOrgList_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			HyperLink lnk = e.Item.FindControl("banOrgItem") as HyperLink;
			if (lnk != null)
			{
				lnk.NavigateUrl = "~/lists/OUExplorer.aspx?ou=" + Server.UrlEncode(((SCSimpleObject)e.Item.DataItem).ID);
			}
		}
		#region Private
		internal static void ChangeAndSaveTimePoint(DateTime timePoint)
		{
			TimePointContext.Current.SimulatedTime = timePoint;
			TimePointContext.Current.UseCurrentTime = timePoint == DateTime.MinValue;

			IPersistTimePoint persister = TimePointSimulationSettings.GetConfig().Persister;

			if (persister != null)
				persister.SaveTimePoint(DeluxeIdentity.CurrentUser.ID, timePoint);
			else
				throw new SystemSupportException("未获取IPersistTimePoint,无法保存时间点，时间穿梭失败。");
		}

		private static void AddOrReplace(DateTime dt, UserRecentDataCategoryItemCollection recentTimePoints)
		{
			int index = -1; // 查找是否是使用过的日期

			for (int i = 0; i < recentTimePoints.Count; i++)
			{
				if (recentTimePoints[i].GetValue("timePoint", DateTime.MinValue) == dt)
				{
					index = i;
					break;
				}
			}

			// 如果是使用过的数据
			if (index >= 0)
			{
				recentTimePoints.Advance(index);
			}
			else
			{
				// 新的日子
				var newItem = recentTimePoints.CreateItem();

				newItem.SetValue("timePoint", dt);

				recentTimePoints.Add(newItem);
			}
		}

		private void RefreshTimeShuttle()
		{
			this.timemark.InnerText = TimePointContext.Current.UseCurrentTime ? "现在" : TimePointContext.Current.SimulatedTime.ToString("yyyy-MM-dd HH:mm:ss");

			IUser curUser = DeluxeIdentity.CurrentUser;

			this.userLogonName.InnerText = curUser.DisplayName;

			this.userPresence.UserID = curUser.ID;
			this.userPresence.UserIconUrl = "~/Handlers/UserPhoto.ashx?id=" + Server.UrlEncode(curUser.ID) + "&time=now";

			UserRecentDataCategory category = UserRecentData.GetSettings(curUser.ID, "recentTimepoints");

			var recentTimePoints = category.Items;

			this.recentList.DataSource = this.GetRecentTimeView(recentTimePoints, category.DefaultSize);

			this.recentList.DataBind();
		}

		private IEnumerable GetRecentTimeView(UserRecentDataCategoryItemCollection recentTimePoints, int limit)
		{
			if (recentTimePoints != null)
			{
				int count = 0;
				foreach (PropertyValueCollection item in recentTimePoints)
				{
					yield return new { TimePoint = (DateTime)item["timePoint"].GetRealValue(), LastAccessData = (DateTime)item["lastAccessDate"].GetRealValue() };
					count++;
					if (count > limit)
						break;
				}
			}

			yield break;
		}

		private void DeleteTimePoint(DateTime timePoint)
		{
			this.DoChnageTimePointAction(
				timePoint, recentTimePoints => recentTimePoints.Items.Remove(item => item.GetValue("timePoint", DateTime.MinValue) == timePoint));
		}

		private void ShuttleToTimePoint(DateTime timePoint)
		{
			this.DoChnageTimePointAction(timePoint, recentTimePoints => AddOrReplace(timePoint, recentTimePoints.Items));
		}

		private void DoChnageTimePointAction(DateTime timePoint, Action<UserRecentDataCategory> action)
		{
			UserRecentDataCategory recentTimes = UserRecentData.LoadSettings(DeluxeIdentity.CurrentUser.ID, "recentTimepoints");

			action(recentTimes);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				recentTimes.SaveChanges(DeluxeIdentity.CurrentUser.ID);

				ChangeAndSaveTimePoint(timePoint);

				scope.Complete();
			}

			this.OnTimePointChanged(EventArgs.Empty);
		}

		/// <summary>
		/// 绑定一般管理架构
		/// </summary>
		private void BindOrganizationList()
		{
			this.bannerOrgList.DataBind();
		}
		#endregion Private
	}
}