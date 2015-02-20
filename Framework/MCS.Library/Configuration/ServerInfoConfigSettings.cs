using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// ������������Ϣ
	/// </summary>
	public class ServerInfoConfigSettings : ConfigurationSection
    {
        private ServerInfoConfigSettings()
        {
        }

		/// <summary>
		/// �������ļ��еõ�������������Ϣ
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
		/// ������������Ϣ����
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
	/// ÿһ��������������Ϣ��������
	/// </summary>
    public class ServerInfoConfigureElement : NamedConfigurationElement
    {
        /// <summary>
        /// ����������
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
        /// ��¼��������Identity�����������
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
        /// ʹ�÷������Ķ˿ں�
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
        /// ����������֤��ʽ
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
		/// ת��ΪServerInfo����
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
					"������identityConfigSettings���ý����ҵ�ServerInfo��������{0}�����õ�Identity: {1}", this.Name, this.IdentityName);

				info.Identity = idElem.ToLogOnIdentity();
			}

			return info;
		}
    }

    /// <summary>
    /// ����������֤��ʽ
    /// </summary>
    public enum AuthenticateType
    {
		/// <summary>
		/// ����
		/// </summary>
        [EnumItemDescription("����")]
        Anonymous = 0,

		/// <summary>
		/// ������֤
		/// </summary>
        [EnumItemDescription("������֤")]
        Basic = 1,

		/// <summary>
		/// NTLM��֤
		/// </summary>
        [EnumItemDescription("NTLM��֤")]
        NTLM = 2
    }

	/// <summary>
	/// ������������ļ���
	/// </summary>
    public class ServerInfoConfigureElementCollection : NamedConfigurationElementCollection<ServerInfoConfigureElement>
    {
    }
}
