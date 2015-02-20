using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;

namespace PermissionCenter
{
	public static class ProfileUtil
	{
		public static int PageSize
		{
			get
			{
				var pageSizeConfig = (int)GetCategory().Properties.GetValue("PerPageSizeOption", 0);
				switch (pageSizeConfig)
				{
					case 0:
						return 10;
					case 1:
						return 20;
					case 2:
						return 50;
					default:
						return 100;
				}
			}
		}

		public static UserBrowseViewMode UserBrowseMode
		{
			get
			{
				return (UserBrowseViewMode)GetCategory().Properties.GetValue("UserBrowseView", 0);
			}
		}

		public static GeneralViewMode GeneralViewMode
		{
			get
			{
				return (GeneralViewMode)GetCategory().Properties.GetValue("GeneralBrowseView", 0);
			}
		}

		public static void ToggleUserBrowseMode(int index)
		{
			var current = ProfileUtil.UserBrowseMode & UserBrowseViewMode.Fixed;
			if (current != UserBrowseViewMode.Fixed)
			{
				switch (index)
				{
					case 0:
						current |= UserBrowseViewMode.DetailList; // 常规列表

						break;
					case 1:
						current |= UserBrowseViewMode.ReducedList; // 精简列表

						break;
					case 2:
						current |= UserBrowseViewMode.ReducedTable; // 精简表格

						break;
					default:
						throw new ArgumentOutOfRangeException("index");
				}

				var settings = UserSettings.LoadSettings(Util.CurrentUser.ID);
				var cate = settings.Categories["PermissionCenter"];
				cate.Properties.SetValue("UserBrowseView", (int)current);
				settings.Update();
			}
		}

		public static void ToggleGeneralBrowseMode(int index)
		{
			var current = ProfileUtil.GeneralViewMode & GeneralViewMode.Fixed;
			if (current != GeneralViewMode.Fixed)
			{
				switch (index)
				{
					case 0:
						current |= GeneralViewMode.List; // 常规列表

						break;
					case 1:
						current |= GeneralViewMode.Table; // 精简表格

						break;
					default:
						throw new ArgumentOutOfRangeException("index");
				}

				var settings = UserSettings.LoadSettings(Util.CurrentUser.ID);
				var cate = settings.Categories["PermissionCenter"];
				cate.Properties.SetValue("GeneralBrowseView", (int)current);
				settings.Update();
			}
		}

		private static UserSettingsCategory GetCategory()
		{
			var settings = UserSettings.GetSettings(Util.CurrentUser.ID).Categories["PermissionCenter"];
			return settings;
		}

		public static int UserBrowseModeIndex
		{
			get
			{
				var mode = ProfileUtil.UserBrowseMode;
				if ((mode & UserBrowseViewMode.ReducedTable) == UserBrowseViewMode.ReducedTable)
				{
					return 2;
				}
				else if ((mode & UserBrowseViewMode.ReducedList) == UserBrowseViewMode.ReducedList)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}

		public static int GeneralViewModeIndex
		{
			get
			{
				var mode = ProfileUtil.GeneralViewMode;
				if ((mode & GeneralViewMode.Table) == PermissionCenter.GeneralViewMode.Table)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}