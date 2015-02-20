using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	public class UserReportInfoAdapter : UpdatableAndLoadableAdapterBase<UserReportInfo, UserReportInfoCollection>
	{
		public static UserReportInfoAdapter Instance = new UserReportInfoAdapter();

		private UserReportInfoAdapter()
		{
		}

		public IUser GetUserReportTo(string userID)
		{
			IUser result = null;

			UserReportInfo reportInfo = GetUserReportToInfo(userID);

			if (reportInfo != null)
				result = reportInfo.ReportTo;

			return result;
		}

		public IUser GetUserReportTo(IUser user)
		{
			user.NullCheck("user");

			IUser result = null;

			UserReportInfo reportInfo = GetUserReportToInfo(user);

			if (reportInfo != null)
				result = reportInfo.ReportTo;

			return result;
		}

		public UserReportInfo GetUserReportToInfo(IUser user)
		{
			user.NullCheck("user");

			return GetUserReportToInfo(user.ID);
		}

		public UserReportInfo GetUserReportToInfo(string userID)
		{
			userID.CheckStringIsNullOrEmpty("userID");

			UserReportInfo result = UserReportInfoCache.Instance.GetOrAddNewValue(userID, (cache, key) =>
				{
					UserReportInfo reportInfo = LoadUserReportToInfo(key);

                    MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

					cache.Add(userID, reportInfo, dependency);

					return reportInfo;
				});

			return result;
		}

		public UserReportInfo LoadUserReportToInfo(IUser user)
		{
			user.NullCheck("user");

			return LoadUserReportToInfo(user.ID);
		}

		public UserReportInfo LoadUserReportToInfo(string userID)
		{
			userID.CheckStringIsNullOrEmpty("userID");

			UserReportInfo reportInfo = null;

			UserReportInfoCollection reportInfoList = Load(builder => builder.AppendItem("USER_ID", userID));

			if (reportInfoList.Count > 0)
				reportInfo = reportInfoList[0];
            
			return reportInfo;
		}

		public UserReportInfoCollection LoadUsersReportInfo(IEnumerable<IUser> users)
		{
			ExceptionHelper.FalseThrow(users != null, "用户集合不能为null");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			builder.AppendItem(ConvertUsersToUserIDs(users));

			UserReportInfoCollection result = new UserReportInfoCollection();

			if (builder.Count > 0)
			{
				string sql = string.Format("SELECT * FROM USERS_REPORT_LINE WHERE USER_ID {0}",
					builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = null;
				DbHelper.RunSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], ConnectionDefine.UserRelativeInfoConnectionName);

				result.LoadFromDataView(table.DefaultView);
			}

			return result;
		}

		protected override void AfterInnerDelete(UserReportInfo data, Dictionary<string, object> context)
		{
			CacheNotifyData notifyData = new CacheNotifyData(typeof(UserReportInfoCache), data.User.ID, CacheNotifyType.Invalid);
			UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
		}

		protected override void AfterInnerUpdate(UserReportInfo data, Dictionary<string, object> context)
		{
			CacheNotifyData notifyData = new CacheNotifyData(typeof(UserReportInfoCache), data.User.ID, CacheNotifyType.Invalid);
			UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
		}

		protected override string GetConnectionName()
		{
			return ConnectionDefine.UserRelativeInfoConnectionName;
		}

		private static string[] ConvertUsersToUserIDs(IEnumerable<IUser> users)
		{
			List<string> result = new List<string>();

			foreach (IUser user in users)
				result.Add(user.ID);

			return result.ToArray();
		}
	}

	public class UserReportInfoCache : CacheQueue<string, UserReportInfo>
	{
		public static readonly UserReportInfoCache Instance = CacheManager.GetInstance<UserReportInfoCache>();

		private UserReportInfoCache()
		{
		}
	}
}
