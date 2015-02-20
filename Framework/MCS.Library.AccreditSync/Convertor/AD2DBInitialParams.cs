using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library;
using MCS.Library.Services;

namespace MCS.Library.Accredit
{
	public class AD2DBInitialParams : IDisposable
	{
		private ServerInfo serverInfo = new ServerInfo();
		private DirectoryEntry root = null;
		private ServiceLog log = null;
		private ADHelper adHelper = null;

		private string accreditAdminConnectionName = string.Empty;
		private string userInfoExtend = string.Empty;

		public event BeforeProcessADObjectDelegate BeforeProcessADObject;

		public static AD2DBInitialParams GetParams()
		{
			AD2DBInitialParams result = new AD2DBInitialParams();

			ExceptionHelper.FalseThrow(ServerInfoConfigSettings.GetConfig().ServerInfos.ContainsKey("dc"),
				"不能在配制节serverInfoConfigSettings中找到名称为dc的配置项");

			result.serverInfo = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"].ToServerInfo();
			result.adHelper = ADHelper.GetInstance(result.serverInfo);
			result.userInfoExtend = ADToDBConfigSettings.GetConfig().UserInfoExtendConnectionName;

			return result;
		}

		private AD2DBInitialParams()
		{
		}

		public ADHelper DirectoryHelper
		{
			get { return this.adHelper; }
		}

		public string AccreditAdminConnectionName
		{
			get
			{
				if (string.IsNullOrEmpty(this.accreditAdminConnectionName))
					this.accreditAdminConnectionName = "AccreditAdmin";

				return this.accreditAdminConnectionName;
			}
			set
			{
				this.accreditAdminConnectionName = value;
			}
		}

		public string UserInfoExtend
		{
			get
			{
				if (string.IsNullOrEmpty(this.userInfoExtend))
					this.userInfoExtend = "HB2008";

				return this.userInfoExtend;
			}
			set
			{
				this.userInfoExtend = value;
			}
		}

		public ServiceLog Log
		{
			get
			{
				if (this.log == null)
					this.log = new ServiceLog(ServiceMainSettings.SERVICE_NAME);

				return this.log;
			}
			set
			{
				this.log = value;
			}
		}

		public DirectoryEntry Root
		{
			get
			{
				if (this.root == null)
					this.root = ADHelper.GetInstance(this.serverInfo).NewEntry(ADToDBConfigSettings.GetConfig().RootOUPath);

				return this.root;
			}
		}

		internal void OnBeforeProcessADObject(SearchResult sr, AD2DBInitialParams initParams, ref bool bContinue)
		{
			if (BeforeProcessADObject != null)
				BeforeProcessADObject(sr, initParams, ref bContinue);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (root != null)
				root.Dispose();
		}

		#endregion
	}
}
