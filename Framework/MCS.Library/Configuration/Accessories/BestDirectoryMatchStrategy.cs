#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	BestDirectoryMatchStrategy.cs
// Remark	��	Ѱ��Ŀ¼�������ƥ��һ����㷨
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

using System.Web;
using System.Web.Hosting;

using MCS.Library.Core;
using MCS.Library.Accessories;
using MCS.Library.Properties;
#endregion

namespace MCS.Library.Configuration.Accessories
{
    /// <summary>
    /// Ѱ��Ŀ¼�������ƥ��һ����㷨
    /// </summary>
    internal sealed class BestDirectoryMatchStrategy : BestPathMatchStrategyBase
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="instances">MetaConfigurationSourceInstanceElementCollection���󣨼��ϣ�</param>
        public BestDirectoryMatchStrategy(MetaConfigurationSourceInstanceElementCollection instances)
        {
            instances.NullCheck("instances");

            if (EnvironmentHelper.IsUsingWebConfig)
                this.path = HostingEnvironment.ApplicationVirtualPath;
            else
                this.path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            this.path = FormatPath(path);
            this.candidates = FileterPath(instances, true);
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

            int maxMatchLength = -1;
            string metaConfig = string.Empty;

            foreach (KeyValuePair<string, string> item in items)
            {
                // ����·��ƥ��
                if (path.StartsWith(item.Key))
                {
                    if (item.Key.Length > maxMatchLength)
                    {
                        maxMatchLength = item.Key.Length;
                        metaConfig = item.Value;
                    }
                    else
                    {
                        if ((item.Key.Length == maxMatchLength) && !string.Equals(metaConfig, item.Value))
                        {
                            string message = string.Format(Resource.ExceptionConflictPathDefinition,
                                item.Key, metaConfig, item.Value);

                            throw new SystemSupportException(message);
                        }
                    }
                }
            }

            return metaConfig;
        }
    }
}
