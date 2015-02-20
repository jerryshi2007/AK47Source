#region
// -------------------------------------------------
// Assembly	：	HB.DataAdapters
// FileName	：	Define.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070724		创建
// -------------------------------------------------
#endregion

using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using MCS.Library;
using MCS.Library.Data;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	internal static class Define
	{
		public const string DefaultCulture = "DataObjects";
	}

	public static class ConnectionDefine
	{
		private const string DefaultDBConnectionName = "HB2008";
		private const string DefaultSearchConnectionName = "FullTextSearch";
		private const string DefaultUserRelativeInfoConnectionName = "UserRelativeInfo";
		private const string DefaultAccreditConnectionName = "AccreditAdmin";
	    private const string DefaultKBConnectionName = "MCSKB";

		public static readonly DateTime MaxVersionEndTime = new DateTime(9999, 9, 9);

		public static string DBConnectionName
		{
			get
			{
				return ConnectionNameMappingSettings.GetConfig().GetConnectionName("HBDataObjects", ConnectionDefine.DefaultDBConnectionName);
			}
		}

		public static string SearchConnectionName
		{
			get
			{
				return ConnectionNameMappingSettings.GetConfig().GetConnectionName("SearchDataObjects", ConnectionDefine.DefaultSearchConnectionName);
			}
		}

		/// <summary>
		/// 用户相关信息的配置节
		/// </summary>
		public static string UserRelativeInfoConnectionName
		{
			get
			{
				string connectionName = ConnectionNameMappingSettings.GetConfig().GetConnectionName("UserRelativeDataObjects", ConnectionDefine.DefaultUserRelativeInfoConnectionName);

				if (DbConnectionManager.ConnectionNameIsConfiged(connectionName) == false)
					connectionName = DBConnectionName;

				return connectionName;
			}
		}

		public static string DefaultAccreditInfoConnectionName
		{

			get
			{
				return ConnectionNameMappingSettings.GetConfig().GetConnectionName(DefaultAccreditConnectionName, ConnectionDefine.DefaultAccreditConnectionName);
			}
		}

	    public static string KBConnectionName
	    {
	        get
	        {
                return ConnectionNameMappingSettings.GetConfig().GetConnectionName(DefaultKBConnectionName,
                                                                                   ConnectionDefine.DefaultDBConnectionName);	            
	        }
	    }
	}
}
