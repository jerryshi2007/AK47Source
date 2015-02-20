#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	FlatFileTraceListener.cs
// Remark	��	��־���ȱʡʵ�ֵ����������ɸ�ʽ�����ı��ļ����������������ļ���·����
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using MCS.Library.Core;

namespace MCS.Library.Logging
{
    /// <summary>
    /// �ı��ļ���������Listerner��
    /// </summary>
    /// <remarks>
    /// FormattedTextWriterTraceListener�����࣬���ӱ�ͷ�ͽ�ע
    /// </remarks>
    public sealed class FlatFileTraceListener : FormattedTextWriterTraceListener
    {
        private string header = string.Empty;
        private string footer = string.Empty;
        private string formatterName = string.Empty;

        /// <summary>
        /// �ı���ʽ����
        /// </summary>
        /// <remarks>
        /// ��Listener������ĸ�ʽ����
        /// </remarks>
        public override ILogFormatter Formatter
        {
            get
            {
                ILogFormatter formatter = base.Formatter;
                if (false == string.IsNullOrEmpty(this.formatterName))// != string.Empty)
                {
					LoggerFormatterConfigurationElement formatterelement = LoggingSection.GetConfig().Formatters[this.formatterName];
                    
                    formatter = LogFormatterFactory.GetFormatter(formatterelement);
                }

                return formatter;
            }
        }

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public FlatFileTraceListener()
            : base()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="element">LogListenerElement����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Logging\ListenerTest.cs"
        /// lang="cs" region="FlatFileTraceListener Test" tittle="����Listener����"></code>
        /// </remarks>
		public FlatFileTraceListener(LoggerListenerConfigurationElement element)
        {
            this.Name = element.Name;

			string filename = element.FileName;

			if (filename.IsNullOrEmpty())
                filename = element.Name + ".log";

            filename = RootFileNameAndEnsureTargetFolderExists(filename);
    
			FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TextWriterTraceListener textlistener = new TextWriterTraceListener(fs, this.Name);

            this.SlaveListener = textlistener;

			this.header = element.Header;

			if (this.header.IsNullOrEmpty())
                this.header = string.Empty;

			this.footer = element.Footer;

			if (this.footer.IsNullOrEmpty())
				this.footer = string.Empty;

            this.formatterName = element.LogFormatterName;

            //string filename;
            //element.ExtendedAttributes.TryGetValue("fileName", out filename);
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="formatter">ILogFormatterʵ��</param>
        /// <remarks>
        /// ��Formatter��ʼ��һ��ʵ������
        /// </remarks>
        public FlatFileTraceListener(ILogFormatter formatter)
            : base(formatter)
        {

        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stream">FileStream����</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(FileStream stream, ILogFormatter formatter)
            : base(stream, formatter)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stream">FileStream����</param>
        public FlatFileTraceListener(FileStream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">StreamWriter����.</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(StreamWriter writer, ILogFormatter formatter)
            : base(writer, formatter)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">StreamWriter����</param>
        public FlatFileTraceListener(StreamWriter writer)
            : base(writer)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">TextWriter����</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(TextWriter writer, ILogFormatter formatter)
            : base(writer, formatter)
        {

        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">��־�ļ���</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(string fileName, ILogFormatter formatter)
            : base(fileName, formatter)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">��־�ļ���</param>
        public FlatFileTraceListener(string fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">��־�ļ���</param>
        /// <param name="header">��־��¼��ͷ</param>
        /// <param name="footer">��־��¼��ע</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(string fileName, string header, string footer, ILogFormatter formatter)
            : base(fileName, formatter)
        {
            this.header = header;
            this.footer = footer;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">��־�ļ���</param>
        /// <param name="header">��־��¼��ͷ</param>
        /// <param name="footer">��־��¼��ע</param>
        public FlatFileTraceListener(string fileName, string header, string footer)
            : base(fileName)
        {
            this.header = header;
            this.footer = footer;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stream">FileStream����</param>
        /// <param name="name">Listener������</param>
        /// <param name="formatter">ILogFormatterʵ��.</param>
        public FlatFileTraceListener(FileStream stream, string name, ILogFormatter formatter)
            : base(stream, formatter)
        {
            this.Name = name;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stream">FileStream����</param>
        /// <param name="name">Listener������</param>
        public FlatFileTraceListener(FileStream stream, string name)
            : base(stream)
        {
            this.Name = name;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">StreamWriter����</param>
        /// <param name="name">Listener������</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(StreamWriter writer, string name, ILogFormatter formatter)
            : base(writer, formatter)
        {
            this.Name = name;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">StreamWriter����</param>
        /// <param name="name">Listener������</param>
        public FlatFileTraceListener(StreamWriter writer, string name)
            : base(writer)
        {
            this.Name = name;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">��־�ļ���</param>
        /// <param name="name">Listener������</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FlatFileTraceListener(string fileName, string name, ILogFormatter formatter)
            : base(fileName, formatter)
        {
            this.Name = name;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileName">��־�ļ���</param>
        /// <param name="name">Listener������</param>
        public FlatFileTraceListener(string fileName, string name)
            : base(fileName)
        {
            this.Name = name;
        }

        /// <summary>
        /// ���ط�����д���ļ�
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="eventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="id">�¼�����ֵ��ʶ��</param>
		/// <param name="data">Ҫ��¼����־����</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (this.header.Length > 0)
                WriteLine(this.header);

			if (data is LogEntity)
            {
				LogEntity logData = data as LogEntity;

                if (this.Formatter != null)
                {
					base.WriteLine(this.Formatter.Format(logData));
                }
                else
                {
					base.TraceData(eventCache, source, eventType, id, logData);
                }
            }
            else
            {
				base.TraceData(eventCache, source, eventType, id, data);
            }

            if (this.footer.Length > 0)
                WriteLine(this.footer);
        }

        /// <summary>
        /// FlatFileTraceListener֧�ֵ�Attibutes
        /// </summary>
        protected override string[] GetSupportedAttributes()
        {
            return new string[4] { "formatter", "fileName", "header", "footer" };
        }
    }
}
