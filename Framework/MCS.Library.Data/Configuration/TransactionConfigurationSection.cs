using System;
using System.Collections.Generic;
using System.Transactions;
using System.Configuration;
namespace MCS.Library.Data.Configuration
{
    /// <summary>
    /// �����й�TransactionScope�Ĳ���
    /// ִ���и��ײ�������ΪĬ�����ã�Ӧ�ÿ��Ը�����Ҫͨ����������������޸���ز�����
    /// </summary>
    class TransactionConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("timeout", IsRequired=false, DefaultValue="00:10:00")]
        public virtual TimeSpan Timeout
        {
            get
            {
                string val = Convert.ToString(this["timeout"]);
                return TimeSpan.Parse(val);
            }
        }

        [ConfigurationProperty("isolationLevel", IsRequired = false, DefaultValue="ReadCommitted")]
        public virtual IsolationLevel IsolationLevel
        {
            get
            {
                string val = Convert.ToString(this["isolationLevel"]);
                return (IsolationLevel)Enum.Parse(typeof(IsolationLevel), val);
            }
        }

    }
}
