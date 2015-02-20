using System;
using System.Text;
using System.Collections.Generic;
using System.DirectoryServices;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 本地用户的修改器
	/// </summary>
	public class LocalUserUpdater : ADSIUserUpdaterBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strUserID"></param>
		/// <returns></returns>
		protected override DirectoryEntry GetUserDirectoryEntry(string strUserID)
		{
			ServerInfo si = GetServerInfo();

			DirectoryEntry entry = ADHelper.GetInstance(si).CreateEntry(string.Format("WinNT://{0}/{1}, user", si.ServerName, strUserID),
									si.Identity.LogOnNameWithDomain, si.Identity.Password);

			return entry;
		}
	}
}
