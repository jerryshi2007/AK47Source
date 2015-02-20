#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LoggerFactory.cs
// Remark	��	����Logger�Ĺ����࣬�����ڲ�ά���ö���Ķ��С�
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Logging
{
    /// <summary>
    /// Logger������
    /// </summary>
    /// <remarks>
    /// ���ڴ���Logger����Ĺ�����
    /// </remarks>
    public sealed class LoggerFactory
    {
        private static IDictionary loggers = new Dictionary<string, Logger>();

        private LoggerFactory()
        {
        }

        /// <summary>
        /// ����Name, �������ļ���ȡ��������Logger����
        /// </summary>
        /// <param name="name">Logger������</param>
        /// <returns>��ȡ��Logger����</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LoggerTest.cs"
        /// lang="cs" region="Create Logger Test" tittle="��ȡLogger����"></code>
        /// </remarks>
        public static Logger Create(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "���ݵ�Logger���������Ϊ��");
            Logger logger = null;
            lock (loggers)
            {
                if (loggers[name] != null)
                    logger = (Logger)loggers[name];
                else
                {
                    logger = GetLoggerFromConfig(name);
                    if (loggers.Contains(name))
                        loggers[name] = logger;
                    else
                        loggers.Add(logger.Name, logger);
                }
            }

            return logger;
        }

        /// <summary>
        /// ����Logger����
        /// </summary>
        /// <returns>ֻ��ȱʡֵ��Logger����</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LoggerTest.cs"
        /// lang="cs" region="Create Logger Test" tittle="��ȡLogger����"></code>
        /// </remarks>
        public static Logger Create()
        {
            return new Logger();
        }

        private static Logger GetLoggerFromConfig(string name)
        {
			LoggerConfigurationElement logelement = LoggingSection.GetConfig().Loggers[name];

            ExceptionHelper.FalseThrow(logelement != null, "δ���ҵ�����Ϊ��{0}��Logger�����ý�", name);
            return new Logger(logelement.Name, logelement.Enabled);
        }
    }
}
