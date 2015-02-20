#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	BestFileNameMatchStrategy.cs
// Remark	��	Ѱ����ƥ����ļ��㷨
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MCS.Library.Core;
using MCS.Library.Accessories;
using MCS.Library.Properties;
#endregion
namespace MCS.Library.Configuration.Accessories
{
    /// <summary>
    /// Ѱ����ƥ����ļ��㷨
    /// </summary>
    internal sealed class BestFileNameMatchStrategy : BestPathMatchStrategyBase
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="instances">MetaConfigurationSourceInstanceElementCollection���󣨼��ϣ�</param>
        public BestFileNameMatchStrategy(MetaConfigurationSourceInstanceElementCollection instances)
        {
            if (instances == null) throw new NullReferenceException("instances");

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            path = FormatPath(path);
            candidates = FileterPath(instances, false);
        }

        /// <summary>
        /// ��·���б���ѡ����Ŀ��·����������һ�
        /// </summary>
        /// <param name="items">·���б�</param>
        /// <returns>��������Ŀ</returns>
        public override string Calculate(IList<KeyValuePair<string, string>> items)
        {
            if ((items == null) || (items.Count <= 0) || string.IsNullOrEmpty(path))
                return string.Empty;

            string metaConfig = string.Empty;

            foreach (KeyValuePair<string, string> item in items)
            {
                if (string.Equals(item.Key, path))
                {
                    if (string.IsNullOrEmpty(metaConfig))
                        metaConfig = item.Value;
                    #region confilit dispose
                    else
                    {
                        if (false == string.Equals(metaConfig, item.Value))
                        {
                            string message = string.Format(Resource.ExceptionConflictPathDefinition,
                                item.Key, metaConfig, item.Value);

                            throw new SystemSupportException(message);
                        }
                    }
                    #endregion
                }
            }

            return metaConfig;
        }
    }
}
