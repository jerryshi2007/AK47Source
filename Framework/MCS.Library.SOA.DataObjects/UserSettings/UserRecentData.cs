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
    /// 表示用户最近的数据
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class UserRecentData
    {
        private UserRecentDataCategoryCollection _Categories = null;

        public UserRecentData()
        {
        }

        public string UserID
        {
            get;
            internal set;
        }


        /// <summary>
        /// 获取一个<see cref="UserRecentDataCategoryCollection"/>，表示用户最近数据的类别的集合。
        /// </summary>
        public UserRecentDataCategoryCollection Categories
        {
            get
            {
                if (this._Categories == null)
                    this._Categories = new UserRecentDataCategoryCollection();

                return this._Categories;
            }
            internal set { this._Categories = value; }
        }

        internal void InitFromConfiguration()
        {
            this._Categories = new UserRecentDataCategoryCollection();

            foreach (UserRecentDataCategoryConfigurationElement elem in UserRecentDataConfigurationSection.GetConfig().Categories)
            {
                this._Categories.Add(new UserRecentDataCategory(elem));
            }
        }

        /// <summary>
        /// 载入用户的最近数据
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static UserRecentData LoadSettings(string userID)
        {
            UserRecentData settings = GetDefaultSettings(userID);

            UserRecentData settingsFromDB = LoadFromDB(userID);

            if (settingsFromDB != null)
                settings.ImportProperties(settingsFromDB);

            return settings;
        }

        /// <summary>
        /// 根据用户ID获取指定的配置
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static UserRecentData GetSettings(string userID)
        {
            userID.CheckStringIsNullOrEmpty("userID");

            UserRecentData result = UserRecentDataCache.Instance.GetOrAddNewValue(userID, (cache, key) =>
            {
                UserRecentData settings = LoadSettings(userID);
                
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
        /// 根据用户ID和Category获取指定的配置
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static UserRecentDataCategory GetSettings(string userID, string category)
        {
            userID.CheckStringIsNullOrEmpty("userID");
            category.CheckStringIsNullOrEmpty("category");

            var result = UserRecentDataCategoryCache.Instance.GetOrAddNewValue(new UserRecentDataCategoryCacheKey(userID, category), (cache, key) =>
            {
                var categoryData = LoadSettings(key.UserId, key.Category);

                MixedDependency mixedDependency = new MixedDependency(
                    new UdpNotifierCacheDependency(),
                    new UserRecentDataCacheItemDependency(),
                    new MemoryMappedFileNotifierCacheDependency());

                cache.Add(key, categoryData, mixedDependency);

                return categoryData;
            });

            return result;
        }

        /// <summary>
        /// 从数据库获取指定类别数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static UserRecentDataCategory LoadSettings(string userId, string category)
        {
            UserRecentDataCategory settings = GetDefaultCategorySettings(category);

            UserRecentDataCategory settingsFromDB = LoadFromDB(userId, category);

            if (settingsFromDB != null)
                settings.ImportValues(settingsFromDB);

            return settings;
        }

        private static UserRecentDataCategory GetDefaultCategorySettings(string category)
        {
            category.CheckStringIsNullOrEmpty("category");

            UserRecentDataCategory settings = new UserRecentDataCategory();

            settings.InitFromConfiguration(UserRecentDataConfigurationSection.GetConfig().Categories[category]);

            return settings;
        }

        /// <summary>
        /// 得到缺省的设置
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static UserRecentData GetDefaultSettings(string userID)
        {
            userID.CheckStringIsNullOrEmpty("userID");

            UserRecentData settings = new UserRecentData();

            settings.UserID = userID;
            settings.InitFromConfiguration();

            return settings;
        }



        /// <summary>
        /// 从数据库中加载
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private static UserRecentData LoadFromDB(string userID)
        {
            string sql = string.Format("SELECT CATEGORY,DATA FROM USER_RECENT_DATA WHERE USER_ID = {0}",
                TSqlBuilder.Instance.CheckQuotationMark(userID, true));

            UserRecentData result = new UserRecentData() { UserID = userID };
            result.InitFromConfiguration();

            using (DbContext dbi = DbContext.GetContext(ConnectionDefine.UserRelativeInfoConnectionName))
            {
                Database db = DatabaseFactory.Create(dbi);

                using (var dr = db.ExecuteReader(System.Data.CommandType.Text, sql))
                {
                    while (dr.Read())
                    {
                        var catDataString = dr.GetString(1);

                        if (catDataString != null)
                        {
                            XElementFormatter formatter = new XElementFormatter();

                            formatter.OutputShortType = false;

                            XElement root = XElement.Parse(catDataString);

                            var loadedCatData = (UserRecentDataCategory)formatter.Deserialize(root);

                            result.Categories[(string)dr.GetString(0)].ImportValues(loadedCatData);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 从数据库中加载
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private static UserRecentDataCategory LoadFromDB(string userID, string category)
        {
            userID.CheckStringIsNullOrEmpty("userID");
            category.CheckStringIsNullOrEmpty("category");

            string sql = string.Format("SELECT DATA FROM USER_RECENT_DATA WHERE USER_ID = {0} AND CATEGORY = {1}",
                TSqlBuilder.Instance.CheckQuotationMark(userID, true),
                TSqlBuilder.Instance.CheckQuotationMark(category, true)
                );

            UserRecentDataCategory result = null;

            string settings = (string)DbHelper.RunSqlReturnScalar(sql, ConnectionDefine.UserRelativeInfoConnectionName);

            if (settings.IsNotEmpty())
            {
                XElementFormatter formatter = new XElementFormatter();

                formatter.OutputShortType = false;

                XElement root = XElement.Parse(settings);

                result = (UserRecentDataCategory)formatter.Deserialize(root);
            }

            return result;
        }


        public void Update(string category)
        {
            category.CheckStringIsNullOrEmpty("category");

            this.Categories.ContainsKey(category).FalseThrow<KeyNotFoundException>("category not found"); ;
            this.Categories[category].SaveChanges(this.UserID);

        }

        /// <summary>
        /// 导入别的UserSettings的属性值，如果类别或属性名称不存在，则忽略
        /// </summary>
        /// <param name="srcSettings"></param>
        private void ImportProperties(UserRecentData srcSettings)
        {
            foreach (UserRecentDataCategory category in this.Categories)
            {
                if (srcSettings.Categories.ContainsKey(category.Name))
                {
                    UserRecentDataCategory srcCategory = srcSettings.Categories[category.Name];

                    if (srcCategory != null)
                        category.ImportValues(srcCategory);
                }
            }
        }
    }

    /// <summary>
    /// 表示类别的缓存键
    /// </summary>
    [Serializable]
    internal struct UserRecentDataCategoryCacheKey
    {
        public UserRecentDataCategoryCacheKey(string userId, string category)
            : this()
        {
            userId.NullCheck("userId");
            category.NullCheck("category");
            this.UserId = userId;
            this.Category = category;
        }
        public string UserId { get; set; }
        public string Category { get; set; }
        public override bool Equals(object obj)
        {
            var other = (UserRecentDataCategoryCacheKey)obj;
            return other.Category.Equals(this.Category) && other.UserId.Equals(this.UserId);

        }
        public override int GetHashCode()
        {
            return UserId.GetHashCode() + Category.GetHashCode();
        }
    }
}
