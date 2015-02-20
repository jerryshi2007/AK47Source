using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Globalization;
using System.Globalization;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 从用户设置中获取得到默认语言
    /// </summary>
    public class UserSettingsCultureInfoAccessor : IUserCultureInfoAccessor
    {
        public static readonly UserSettingsCultureInfoAccessor Instance = new UserSettingsCultureInfoAccessor();

        private UserSettingsCultureInfoAccessor()
        {
        }

        /// <summary>
        /// 得到用户的语言ID（zh-CN，或者en-US）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="defaultLanguageID">默认的语言ID</param>
        /// <returns></returns>
        public string GetCurrentUserLanguageID(string userID, string defaultLanguageID)
        {
            //得到当前登录用户的ID
            if (DeluxePrincipal.IsAuthenticated && DeluxeIdentity.Current != null)
                userID = DeluxeIdentity.CurrentUser.ID;

            return UserSettings.GetPropertyValue(userID, "CommonSettings", "Language", defaultLanguageID);
        }

        public void SaveUserLanguageID(string userID, string languageID)
        {
            //得到当前登录用户的ID
            if (DeluxePrincipal.IsAuthenticated && DeluxeIdentity.Current != null)
                userID = DeluxeIdentity.CurrentUser.ID;

            UserSettings settings = UserSettings.LoadSettings(userID);

            settings.Categories["CommonSettings"].Properties["Language"].StringValue = languageID;
        }
    }
}
