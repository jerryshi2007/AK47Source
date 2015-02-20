#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogEntity.cs
// Remark	��	��־��¼���ͣ���ʾ������־��¼
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Security;
using System.Security.Permissions;
using MCS.Library.Core;
using System.Reflection;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ��־��¼
    /// </summary>
    /// <remarks>
    /// ��������־��¼��
    /// </remarks>
    [Serializable]
    [XmlRoot("LogEntity")]
    public sealed class LogEntity : ICloneable
    {
        private LogPriority defaultPriority = LogPriority.Normal;
        private TraceEventType logEventType = TraceEventType.Information;
        private int defaultEventId = 0;
        private Guid activityID = Guid.Empty;
        private string defaultTitle = string.Empty;
        private string message = string.Empty;
        private string stackTrace = string.Empty;
        private string source = string.Empty;
        private DateTime timeStamp = DateTime.MinValue;
        private string machineName = string.Empty;
        private IDictionary<string, object> extendedProperties = null;

        /// <summary>
        /// ���캯����������Ϣ��������־��¼����
        /// </summary>
        /// <param name="message">��־��¼��Ϣ</param>
        /// <remarks>
        /// message��������Ϊ�գ��������쳣
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="������Ϣ������־��¼����"></code>
        /// </remarks>
        public LogEntity(string message)
        {
            ExceptionHelper.TrueThrow<LogException>(string.IsNullOrEmpty(message), "��־��¼��Ϣ����Ϊ��");

            this.message = message;
            BuildUpInitialProperties();
        }

        /// <summary>
        /// ���캯���������쳣������־��¼����
        /// </summary>
        /// <param name="ex">�쳣����</param>
        /// <remarks>
        /// ex��������Ϊ�գ��������쳣
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="�����쳣���󴴽���־��¼����"></code>
        /// </remarks>
        public LogEntity(Exception ex)
        {
            ExceptionHelper.TrueThrow<LogException>(ex == null, "���ݵ��쳣����Ϊ��");

            BuildUpInitialProperties();

            Exception realEx = ExceptionHelper.GetRealException(ex);

            this.defaultTitle = ex.Message;
            this.message = realEx.Message;
            this.stackTrace = realEx.StackTrace;
            this.source = realEx.Source;
            this.logEventType = TraceEventType.Error;
        }

        /// <summary>
        /// ���캯�������ݲ�������LogEntity����
        /// </summary>
        /// <param name="title">��־����</param>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="eventID">��־�¼�ID</param>
        /// <remarks>
        /// ���������ݵĲ�������LogEntity������������Ϊȱʡֵ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="���ݲ�������LogEntity����"></code>
        /// </remarks>
        public LogEntity(string title, string message, int eventID)
            : this(title, message, eventID, LogPriority.Normal, TraceEventType.Information, string.Empty, string.Empty, null)
        {
           
        }

        /// <summary>
        /// ���캯�������ݲ�������LogEntity����
        /// </summary>
        /// <param name="title">��־����</param>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="eventID">��־�¼�ID</param>
        /// <param name="priority">��־��¼���ȼ�</param>
        /// <param name="logEventType">��־�¼�����</param>
        /// <param name="source">��־��Դ</param>
        /// <param name="stackTrace">����ջ</param>
        /// <param name="propterties">��չ��Ϣ</param>
        /// <remarks>
        /// ���������ݵĲ�������LogEntity����
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="Create LogEntity Test" tittle="���ݲ�������LogEntity����"></code>
        /// </remarks>
        public LogEntity(string title, string message, int eventID, LogPriority priority, TraceEventType logEventType, string source,
            string stackTrace, IDictionary<string, Object> propterties)
        {
            ExceptionHelper.TrueThrow<LogException>(string.IsNullOrEmpty(message), "��־��¼��Ϣ����Ϊ��");

            this.message = message;
            this.defaultTitle = title;
            this.defaultEventId = eventID;
            this.logEventType = logEventType;
            this.defaultPriority = priority;
            this.source = source;
            this.stackTrace = stackTrace;
            this.extendedProperties = propterties;

            BuildUpInitialProperties();
        }

        #region ��������
        /// <summary>
        /// ��־�¼�ID
        /// </summary>
        public int EventID
        {
            get
            {
                return this.defaultEventId;
            }
            set
            {
                this.defaultEventId = value;
            }
        }

        /// <summary>
        /// ��־����
        /// </summary>
        public string Title
        {
            get
            {
                return this.defaultTitle;
            }
            set
            {
                this.defaultTitle = value;
            }
        }

        /// <summary>
        /// ��־��Ϣ
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        /// <summary>
        /// ����ջ
        /// </summary>
        public string StackTrace
        {
            get
            {
                //return Environment.StackTrace;
                return this.stackTrace;
            }
            set
            {
                this.stackTrace = value;
            }
        }

        /// <summary>
        /// ��־��Դ
        /// </summary>
        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        /// <summary>
        /// ʱ���
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return this.timeStamp;
            }
            set
            {
                this.timeStamp = value;
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public string MachineName
        {
            get
            {
                return this.machineName;
            }
            set
            {
                this.machineName = value;
            }
        }

        /// <summary>
        /// ���������ID
        /// </summary>
        public Guid ActivityID
        {
            get
            {
                return this.activityID;
            }
            set
            {
                this.activityID = value;
            }
        }

        /// <summary>
        /// ��־��¼���ȼ�
        /// </summary>
        public LogPriority Priority
        {
            get
            {
                return this.defaultPriority;
            }
            set
            {
                this.defaultPriority = value;
            }
        }

        /// <summary>
        /// ��־�¼�����
        /// </summary>
        public TraceEventType LogEventType
        {
            get 
            {
                return this.logEventType;
            }
            set 
            {
                this.logEventType = value;
            }
        }

        /// <summary>
        /// ��չ��Ϣ
        /// </summary>
        public IDictionary<string, object> ExtendedProperties
        {
            get
            {
                if (this.extendedProperties == null)
                    this.extendedProperties = new Dictionary<string, object>();

                return this.extendedProperties;
            }
            //set
            //{
            //    extendedProperties = value;
            //}
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// ʵ��ICloneable�ӿ�
        /// </summary>
        /// <returns>
        /// ��¡����LogEntity����
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogEntityTest.cs"
        /// lang="cs" region="LogEntityClone Test" tittle="��¡LogEntity����"></code>
        /// </returns>
        public object Clone()
        {
            LogEntity log = new LogEntity(this.Message);

            log.EventID = this.EventID;
            log.Title = this.Title;
            log.Priority = this.Priority;
            log.LogEventType = this.LogEventType;
            log.MachineName = this.MachineName;
            log.StackTrace = this.StackTrace;
            log.ActivityID = this.ActivityID;
            log.Source = this.Source;

            if (this.extendedProperties != null)
                log.extendedProperties = new Dictionary<string, object>(this.extendedProperties);

            return log;
        }

        #endregion

        private void BuildUpInitialProperties()
        {
            this.timeStamp = DateTime.Now;
            
            if (IsTracingAvailable)
            {
                try
                {
                    this.ActivityID = Trace.CorrelationManager.ActivityId;
                }
                catch (Exception)
                {
                    this.ActivityID = Guid.Empty;
                }
            }

            try
            {
                this.machineName = Environment.MachineName;
            }
            catch (Exception ex)
            {
                this.machineName = String.Format(Properties.Resource.Culture,
                    Properties.Resource.InitialPropertyError, ex.Message);
            }

            //try
            //{
            //    stackTrace = Environment.StackTrace;
            //}
            //catch (Exception ex)
            //{
            //    stackTrace = String.Format(Properties.Resource.Culture,
            //        Properties.Resource.InitialPropertyError, ex.Message);
            //}
        }

        private bool IsTracingAvailable
        {
			get
			{
				bool tracingAvailable = false;

				try
				{
					tracingAvailable = true;
					//tracingAvailable = SecurityManager.IsGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
				}
				catch (SecurityException)
				{
				}

				return tracingAvailable;
			}
        }
    }
}
