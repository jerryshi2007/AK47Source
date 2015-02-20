using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MCS.Library.Globalization
{
	/// <summary>
	/// 用户全球化的参数设置
	/// </summary>
	public interface IUserCultureInfoAccessor
	{
		/// <summary>
		/// 得到当前用户的语言ID（zh-CN，或者en-US）
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="defaultLanguageID">默认的语言ID</param>
		/// <returns></returns>
		string GetCurrentUserLanguageID(string userID, string defaultLanguageID);

        /// <summary>
        /// 保存当前用户的语言ID（zh-CN，或者en-US）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="languageID"></param>
        void SaveUserLanguageID(string userID, string languageID);
	}
}
