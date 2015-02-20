using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCS.Library.Globalization
{
    /// <summary>
    /// 基于Cookie的用户信息存储
    /// </summary>
    public class SessionCookieUserCultureInfoAccessor : IUserCultureInfoAccessor
    {
        /// <summary>
        /// 从Cookie中读取用户的文种信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="defaultLanguageID"></param>
        /// <returns></returns>
        public string GetCurrentUserLanguageID(string userID, string defaultLanguageID)
        {
            return CookieHelper.GetRequestCookieValue("SessionUserCulture", defaultLanguageID);
        }

        /// <summary>
        /// 保存用户文种信息到Cookie中
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="languageID"></param>
        public void SaveUserLanguageID(string userID, string languageID)
        {
            CookieHelper.SetResponseCookieValue("SessionUserCulture", languageID);
        }
    }
}
