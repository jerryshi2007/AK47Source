using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using System.Xml.Linq;
using MCS.Library.Data;
using System.Transactions;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户的个人设置
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public sealed class UserSettings
	{
		private UserSettingsCategoryCollection _Categories = null;

		public UserSettings()
		{
		}

		public string UserID
		{
			get;
			internal set;
		}

		/// <summary>
		/// 得到属性值，如果类别或属性名称不存在，则返回缺省值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="category">类别名称</param>
		/// <param name="propName">属性名称</param>
		/// <param name="defaultValue">缺省值</param>
		/// <returns></returns>
		public T GetPropertyValue<T>(string categoryName, string propName, T defaultValue)
		{
			categoryName.CheckStringIsNullOrEmpty("categoryName");
			propName.CheckStringIsNullOrEmpty("propName");

			T result = defaultValue;

			UserSettingsCategory category = this.Categories[categoryName];

			if (category != null)
				result = category.Properties.GetValue(propName, defaultValue);

			return result;
		}

		/// <summary>
		/// 个人设置的类别
		/// </summary>
		public UserSettingsCategoryCollection Categories
		{
			get
			{
				if (this._Categories == null)
					this._Categories = new UserSettingsCategoryCollection();

				return this._Categories;
			}
			internal set { this._Categories = value; }
		}

		internal void InitFromConfiguration()
		{
			this._Categories = new UserSettingsCategoryCollection();

			foreach (PropertyGroupConfigurationElement elem in UserSettingsConfig.GetConfig().Categories)
			{
				this._Categories.Add(new UserSettingsCategory(elem));
			}
		}

		/// <summary>
		/// 得到用户的个人设置
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static UserSettings LoadSettings(string userID)
		{
			UserSettings settings = GetDefaultSettings(userID);

            UserSettings settingsFromDB = LoadFromDB(userID);

            if (settingsFromDB != null)
                settings.ImportProperties(settingsFromDB);

			return settings;
		}

		public static UserSettings GetSettings(string userID)
		{
			userID.CheckStringIsNullOrEmpty("userID");

			UserSettings result = UserSettingsCache.Instance.GetOrAddNewValue(userID, (cache, key) =>
			{
				UserSettings settings = LoadSettings(userID);

                MixedDependency mixedDependency = new MixedDependency(
                    new UdpNotifierCacheDependency(),
                    new UserRecentDataCacheItemDependency(),
                    new MemoryMappedFileNotifierCacheDependency());

				cache.Add(key, settings, mixedDependency);

				return settings;
			});

			return result;
		}

		/// <summary>
		/// 得到某个用户的个人设置的属性值，如果userID不存在、categoryName或propName不存在，都返回缺省值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="userID"></param>
		/// <param name="categoryName"></param>
		/// <param name="propName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T GetPropertyValue<T>(string userID, string categoryName, string propName, T defaultValue)
		{
			UserSettings settings = GetSettings(userID);

			return settings.GetPropertyValue(categoryName, propName, defaultValue);
		}

		/// <summary>
		/// 得到缺省的设置
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static UserSettings GetDefaultSettings(string userID)
		{
			userID.CheckStringIsNullOrEmpty("userID");

			UserSettings settings = new UserSettings();

			settings.UserID = userID;
			settings.InitFromConfiguration();

			return settings;
		}

		/// <summary>
		/// 从数据库中加载
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		private static UserSettings LoadFromDB(string userID)
		{
			string sql = string.Format("SELECT SETTINGS FROM USER_SETTINGS WHERE USER_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(userID, true));

			UserSettings result = null;

			string settings = (string)DbHelper.RunSqlReturnScalar(sql, ConnectionDefine.UserRelativeInfoConnectionName);

			if (settings.IsNotEmpty())
			{
				XElementFormatter formatter = new XElementFormatter();

				formatter.OutputShortType = false;

				XElement root = XElement.Parse(settings);

				result = (UserSettings)formatter.Deserialize(root);
			}

			return result;
		}

		public void Update()
		{
			XElementFormatter formatter = new XElementFormatter();

			formatter.OutputShortType = false;

			string settings = formatter.Serialize(this).ToString();

			string updateSQL = string.Format("UPDATE USER_SETTINGS SET SETTINGS = {0} WHERE USER_ID = {1}",
				TSqlBuilder.Instance.CheckQuotationMark(settings, true),
				TSqlBuilder.Instance.CheckQuotationMark(this.UserID, true));

			using (DbContext context = DbContext.GetContext(ConnectionDefine.UserRelativeInfoConnectionName))
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					if (DbHelper.RunSql(updateSQL, ConnectionDefine.UserRelativeInfoConnectionName) == 0)
					{
						string insertSQL = string.Format("INSERT INTO USER_SETTINGS(USER_ID, SETTINGS) VALUES({0}, {1})",
							TSqlBuilder.Instance.CheckQuotationMark(this.UserID, true),
							TSqlBuilder.Instance.CheckQuotationMark(settings, true));

						DbHelper.RunSql(insertSQL, ConnectionDefine.UserRelativeInfoConnectionName);
					}

					scope.Complete();
				}
			}

			CacheNotifyData notifyData = new CacheNotifyData(typeof(UserSettingsCache), this.UserID, CacheNotifyType.Invalid);
			UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
		}

		/// <summary>
		/// 导入别的UserSettings的属性值，如果类别或属性名称不存在，则忽略
		/// </summary>
		/// <param name="srcSettings"></param>
		private void ImportProperties(UserSettings srcSettings)
		{
			foreach (UserSettingsCategory category in this.Categories)
			{
				UserSettingsCategory srcCategory = srcSettings.Categories[category.Name];

				if (srcCategory != null)
					category.ImportProperties(srcCategory);
			}
		}
	}
}
