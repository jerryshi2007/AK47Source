#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	PassportSectionGroup.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
	/// <summary>
	/// ��Passport���йص�������
	/// </summary>
	internal sealed class PassportSectionGroup : ConfigurationSectionGroup
	{
		/// <summary>
		/// Passport�Ŀͻ���Ӧ�����ý�
		/// </summary>
		[ConfigurationProperty("passportClientSettings")]
		public PassportClientSettings ClientSettings
		{
			get
			{
				return base.Sections["passportClientSettings"] as PassportClientSettings; 
			}
		}

		/// <summary>
		/// Passport��֤��������Ҫ�����ý�
		/// </summary>
		[ConfigurationProperty("passportSignInSettings")]
		public PassportSignInSettings SignInSettings
		{
			get
			{
				return base.Sections["passportSignInSettings"] as PassportSignInSettings;
			}
		}

		/// <summary>
		/// Passport�ӽ��ܵ����ý�
		/// </summary>
		[ConfigurationProperty("passportEncryptionSettings")]
		public PassportEncryptionSettings EncryptionSettings
		{
			get
			{
				return base.Sections["passportEncryptionSettings"] as PassportEncryptionSettings;
			}
		}

		/// <summary>
		/// ���ģ���йص����ý�
		/// </summary>
		[ConfigurationProperty("impersonateSettings")]
		public ImpersonateSettings ImpersonateSettings
		{
			get
			{
				return base.Sections["impersonateSettings"] as ImpersonateSettings;
			}
		}

		/// <summary>
		/// ��֤Ŀ¼�������йص����ý�
		/// </summary>
		[ConfigurationProperty("authenticateDirSettings")]
		public AuthenticateDirSettings AuthenticateDirSettings
		{
			get
			{
				return base.Sections["authenticateDirSettings"] as AuthenticateDirSettings;
			}
		}

		/// <summary>
		/// ����ӳ���йص����ý�
		/// </summary>
		[ConfigurationProperty("domainMappingSettings")]
		public DomainMappingSettings DomainMappingSettings
		{
			get
			{
				return base.Sections["domainMappingSettings"] as DomainMappingSettings;
			}
		}
	}
}
