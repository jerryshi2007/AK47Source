#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	FormattedTextWriterTraceListener.cs
// Remark	��	��־���ȱʡʵ�ֵ����������ɸ�ʽ�����ı���д��������Listerner������װ��TextWriterTraceListener
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MCS.Library.Logging
{
    /// <summary>
    /// �ɸ�ʽ�����ı���д��������Listerner��
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase�������࣬��װTextWriterTraceListener��
    /// </remarks>
    public class FormattedTextWriterTraceListener : FormattedTraceListenerWrapperBase
    {
        private string fileName = string.Empty;

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public FormattedTextWriterTraceListener()
            : base(new TextWriterTraceListener())
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FormattedTextWriterTraceListener(ILogFormatter formatter)
            : this()
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="filename">��־�ļ���</param>
        /// <remarks>
        /// ����־��¼д��filenameָ�����ļ���
        /// </remarks>
        public FormattedTextWriterTraceListener(string filename)
        {
            this.fileName = RootFileNameAndEnsureTargetFolderExists(filename);
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TextWriterTraceListener textlistener = new TextWriterTraceListener(fs, base.Name);

            base.SlaveListener = textlistener;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="filename">��־�ļ���</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        /// <remarks>
        /// ����־��¼����formatter��ʽ����д��filenameָ�����ļ���
        /// </remarks>
        public FormattedTextWriterTraceListener(string filename, ILogFormatter formatter)
            : this(filename)
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stream">Stream����</param>
        /// <remarks>
        /// ����־��¼��д��stream����
        /// </remarks>
        public FormattedTextWriterTraceListener(Stream stream)
        {
            TextWriterTraceListener textlistener = new TextWriterTraceListener(stream, base.Name);

            base.SlaveListener = textlistener;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stream">Stream����</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        /// <remarks>
        /// ����־��¼����formatter��ʽ����д��stream����
        /// </remarks>
        public FormattedTextWriterTraceListener(Stream stream, ILogFormatter formatter)
            : this(stream)
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">TextWriter����</param>
        /// <remarks>
        /// ����־��¼��д��writer��
        /// </remarks>
        public FormattedTextWriterTraceListener(TextWriter writer)
            : base(new TextWriterTraceListener(writer))
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="writer">TextWriter����</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        /// <remarks>
        /// ����־��¼����formatter��ʽ����д��writer��
        /// </remarks>
        public FormattedTextWriterTraceListener(TextWriter writer, ILogFormatter formatter)
            : this(writer)
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// �Ƿ��̰߳�ȫ����TextWriterTraceListenerΪtrue
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// ���ط�����д������
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
            if (data is LogEntity)
            {
				LogEntity logData = data as LogEntity;
                if (this.Formatter != null)
                {
					base.Write(this.Formatter.Format(logData));
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
        }

        /// <summary>
        ///֧�ֵ�����
        /// </summary>
        /// <returns>��������</returns>
        protected override string[] GetSupportedAttributes()
        {
            return new string[1] { "formatter" };
        }

        /// <summary>
        /// �ļ�·������
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns>��׼�������ļ�·��</returns>
        protected static string RootFileNameAndEnsureTargetFolderExists(string fileName)
        {
            string rootedFileName = fileName;
            if (false == Path.IsPathRooted(rootedFileName))
            {
                rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedFileName);
            }

            string directory = Path.GetDirectoryName(rootedFileName);
            if (false == string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return rootedFileName;
        }
    }

    #region Obsoleted Implementation of FormattedTextWriterTraceListener Class
    //public class FormattedTextWriterTraceListener : TextWriterTraceListener
   // {
   //     private ILogFormatter formatter;
 
   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     public FormattedTextWriterTraceListener()
   //         : base()
   //     {
            
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(ILogFormatter formatter)
   //         : this()
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(Stream stream, ILogFormatter formatter)
   //         : this(stream)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     public FormattedTextWriterTraceListener(Stream stream)
   //         : base(stream)
   //     {
          
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer, ILogFormatter formatter)
   //         : this(writer)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer)
   //         : base(writer)
   //     {
           
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(string fileName, ILogFormatter formatter)
   //         : this(fileName)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     public FormattedTextWriterTraceListener(string fileName)
   //         : base(RootFileNameAndEnsureTargetFolderExists(fileName))
   //     {
          
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     /// <param name="name">The name.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(Stream stream, string name, ILogFormatter formatter)
   //         : this(stream, name)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     /// <param name="name">The name.</param>
   //     public FormattedTextWriterTraceListener(Stream stream, string name)
   //         : base(stream, name)
   //     {
           
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     /// <param name="name">The name.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer, string name, ILogFormatter formatter)
   //         : this(writer, name)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     /// <param name="name">The name.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer, string name)
   //         : base(writer, name)
   //     {
           
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     /// <param name="name">The name.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(string fileName, string name, ILogFormatter formatter)
   //         : this(fileName, name)
   //     {
   //     }

   //     /// <summary>
   //     /// ���캯��
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     /// <param name="name">The name.</param>
   //     public FormattedTextWriterTraceListener(string fileName, string name)
   //         : base(RootFileNameAndEnsureTargetFolderExists(fileName), name)
   //     {
   //         this.Formatter = formatter;
           
   //     }

   //     /// <summary>
   //     /// д����
   //     /// </summary>
   //     /// <remarks>
   //     /// Formatting is only performed if the object to trace is a <see cref="LogEntry"/> and the formatter is set.
   //     /// </remarks>
   //     /// <param name="eventCache">The context information.</param>
   //     /// <param name="source">The trace source.</param>
   //     /// <param name="eventType">The severity.</param>
   //     /// <param name="id">The event id.</param>
   //     /// <param name="data">The object to trace.</param>
   //     public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
   //     {
   //         if (data is LogEntity)
   //         {
   //             if (this.Formatter != null)
   //             {
   //                 base.Write(this.Formatter.Format(data as LogEntity));
   //             }
   //             else
   //             {
   //                 base.TraceData(eventCache, source, eventType, id, data);
   //             }
                
   //         }
   //         else
   //         {
   //             base.TraceData(eventCache, source, eventType, id, data);
   //         }
   //     }

   //     /// <summary>
   //     /// �ı���ʽ����
   //     /// </summary>
   //     public virtual ILogFormatter Formatter
   //     {
   //         get
   //         {
   //             return this.formatter;
   //         }

   //         set
   //         {
   //             this.formatter = value;
   //         }
   //     }

   //     /// <summary>
   //     ///֧�ֵ�����
   //     /// </summary>
   //     /// <returns></returns>
   //     protected override string[] GetSupportedAttributes()
   //     {
   //         return new string[1] { "formatter" };
   //     }
  
   //     private static string RootFileNameAndEnsureTargetFolderExists(string fileName)
   //     {
   //         string rootedFileName = fileName;
   //         if (!Path.IsPathRooted(rootedFileName))
   //         {
   //             rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedFileName);
   //         }

   //         string directory = Path.GetDirectoryName(rootedFileName);
   //         if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
   //         {
   //             Directory.CreateDirectory(directory);
   //         }

   //         return rootedFileName;
   //     }

    // }
    #endregion
}
