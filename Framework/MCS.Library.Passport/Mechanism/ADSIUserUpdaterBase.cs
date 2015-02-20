using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using System.DirectoryServices;
using MCS.Library.Configuration;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ADSIUserUpdaterBase : IUserInfoUpdater
	{
		#region IUserInfoUpdater Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strUserID"></param>
		/// <param name="oldPassword"></param>
		/// <param name="newPassword"></param>
		public void ChangePassword(string strUserID, string oldPassword, string newPassword)
		{
			using (DirectoryEntry entry = GetUserDirectoryEntry(strUserID))
			{
				entry.Invoke("ChangePassword", oldPassword, newPassword);
			}
		}

		#endregion

		/// <summary>
		/// 得到ServerInfo的配置信息
		/// </summary>
		/// <returns></returns>
		protected virtual ServerInfo GetServerInfo()
		{
			ExceptionHelper.FalseThrow(ServerInfoConfigSettings.GetConfig().ServerInfos.ContainsKey("userInfo"),
				"没有在serverInfoConfigSettings/serverInfos配置userInfo项");

			return ServerInfoConfigSettings.GetConfig().ServerInfos["userInfo"].ToServerInfo();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strUserID"></param>
		/// <returns></returns>
		protected abstract DirectoryEntry GetUserDirectoryEntry(string strUserID);
	}
}
