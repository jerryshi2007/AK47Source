using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 服务器配置信息
	/// </summary>
	public class ServerInfoConfigSettings : ConfigurationSection
    {
        private ServerInfoConfigSettings()
        {
        }

		/// <summary>
		/// 从配置文件中得到服务器配置信息
		/// </summary>
		/// <returns></returns>
        public static ServerInfoConfigSettings GetConfig()
        {
            ServerInfoConfigSettings result = (ServerInfoConfigSettings)ConfigurationBroker.GetSection("serverInfoConfigSettings");

            if (result == null)
                result = new ServerInfoConfigSettings();

            return result;
        }

		/// <summary>
		/// 服务器配置信息集合
		/// </summary>
        [ConfigurationProperty("serverInfos")]
        public ServerInfoConfigureElementCollection ServerInfos
        {
            get
            {
                return (ServerInfoConfigureElementCollection)this["serverInfos"];
            }
        }
    }

	/// <summary>
	/// 每一个服务器配置信息的配置项
	/// </summary>
    public class ServerInfoConfigureElement : NamedConfigurationElement
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        [ConfigurationProperty("serverName", IsRequired = true)]
        public string ServerName
        {
            get
            {
                return (string)this["serverName"];
            }
        }

        /// <summary>
        /// 登录服务器的Identity配置项的名称
        /// </summary>
        [ConfigurationProperty("identityName")]
        public string IdentityName
        {
            get
            {
                return (string)this["identityName"];
            }
        }

        /// <summary>
        /// 使用服务器的端口号
        /// </summary>
        [ConfigurationProperty("port")]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
        }

        /// <summary>
        /// 服务器的认证方式
        /// </summary>
        [ConfigurationProperty("authenticateType", DefaultValue = AuthenticateType.Anonymous)]
        public AuthenticateType AuthenticateType
        {
            get
            {
                return (AuthenticateType)this["authenticateType"];
            }
        }

		/// <summary>
		/// 转换为ServerInfo对象
		/// </summary>
		/// <returns></returns>
		public ServerInfo ToServerInfo()
		{
			ServerInfo info = new ServerInfo();

			info.ServerName = this.ServerName;
			info.AuthenticateType = this.AuthenticateType;
			info.Port = this.Port;

			if (string.IsNullOrEmpty(this.IdentityName) == false)
			{
				IdentityConfigurationElement idElem = IdentityConfigSettings.GetConfig().Identities[this.IdentityName];

				ExceptionHelper.FalseThrow(idElem != null,
					"不能在identityConfigSettings配置节中找到ServerInfo的配置项{0}中配置的Identity: {1}", this.Name, this.IdentityName);

				info.Identity = idElem.ToLogOnIdentity();
			}

			return info;
		}
    }

    /// <summary>
    /// 服务器的认证方式
    /// </summary>
    public enum AuthenticateType
    {
		/// <summary>
		/// 匿名
		/// </summary>
        [EnumItemDescription("匿名")]
        Anonymous = 0,

		/// <summary>
		/// 基本认证
		/// </summary>
        [EnumItemDescription("基本认证")]
        Basic = 1,

		/// <summary>
		/// NTLM认证
		/// </summary>
        [EnumItemDescription("NTLM认证")]
        NTLM = 2
    }

	/// <summary>
	/// 服务器配置项的集合
	/// </summary>
    public class ServerInfoConfigureElementCollection : NamedConfigurationElementCollection<ServerInfoConfigureElement>
    {
    }
}
