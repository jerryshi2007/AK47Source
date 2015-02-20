#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	PassportEncryptionSettings.cs
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
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
    /// <summary>
    /// ��Passport�йصļ��ܽӿ�
    /// </summary>
    public class PassportEncryptionSettings : ConfigurationSection
    {
        private ITicketEncryption ticketEncryption = null;
        private IStringEncryption stringEncryption = null;

        /// <summary>
        /// �����¼����������Ϣ
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="PassportEncryptionSettingsTest" lang="cs" title="��ȡ�ӽ���Ticket��������Ϣ" />
        /// </remarks>
        public static PassportEncryptionSettings GetConfig()
        {
            PassportEncryptionSettings result =
                (PassportEncryptionSettings)ConfigurationBroker.GetSection("passportEncryptionSettings");

            if (result == null)
                result = new PassportEncryptionSettings();

            return result;
        }

        /// <summary>
        /// Ticket������
        /// </summary>
        public ITicketEncryption TicketEncryption
        {
            get
            {
                if (this.ticketEncryption == null)
                    if (TypeFactories.ContainsKey("ticketEncryption"))
                        this.ticketEncryption = (ITicketEncryption)TypeFactories["ticketEncryption"].CreateInstance();
                    else
                        this.ticketEncryption = new TicketEncryption();

                return this.ticketEncryption;
            }
        }

        /// <summary>
        /// �ַ���������
        /// </summary>
        public IStringEncryption StringEncryption
        {
            get
            {
                if (this.stringEncryption == null)
                    if (TypeFactories.ContainsKey("stringEncryption"))
                        this.stringEncryption = (IStringEncryption)TypeFactories["stringEncryption"].CreateInstance();
                    else
                        this.stringEncryption = new StringEncryption();

                return this.stringEncryption;
            }
        }

        [ConfigurationProperty("typeFactories", IsRequired = true)]
        private TypeConfigurationCollection TypeFactories
        {
            get
            {
                return (TypeConfigurationCollection)this["typeFactories"];
            }
        }
    }
}
