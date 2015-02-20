using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace MCS.Library.Services
{
    /// <summary>
    /// TextBoxWriter translates a series of writes to a TextBox display. 
    /// Note that no locking is performed because this class is only written 
    /// to by BufferedStringTextWriter which does locking. However the 
    /// class does do an extra level of buffering in the case where the 
    /// control has not yet been created at the time of the Write().
    /// </summary>
    public class TextBoxWriter : TextWriter
    {
        private TextBoxBase textBox;
        private StringBuilder stringBuffer;
        private delegate void AppendTextDelegate(string s);

        public TextBoxWriter(TextBox textBox)
        {
            this.textBox = textBox;
            this.textBox.HandleCreated += new EventHandler(OnHandleCreated);
        }

        public TextBoxWriter(RichTextBox richTextBox)
        {
            this.textBox = richTextBox;

            this.textBox.HandleCreated += new EventHandler(OnHandleCreated);
        }

        public override Encoding Encoding
        {
            get 
            {
                return Encoding.Default;
            }
        }

        public override void Write(string s)
        {
            if (this.textBox.IsHandleCreated)
                AppendText(s);
            else
                BufferText(s);
        }

        public override void Write(char c)
        {
            Write(c.ToString());
        }

        public override void WriteLine(string s)
        {
            Write(s + "\r\n");
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void AppendText(string s)
        {
            if (stringBuffer != null)
            {
                InvokeAppendText(stringBuffer.ToString());
                stringBuffer = null;
            }

            InvokeAppendText(s);
        }

        private void BufferText(string s)
        {
            if (stringBuffer == null)
                stringBuffer = new StringBuilder(1024);

            stringBuffer.Append(s);
        }

        private void OnHandleCreated(object sender, EventArgs e)
        {
            if (stringBuffer != null)
                InvokeAppendText(stringBuffer.ToString());

            stringBuffer = null;
        }

        private void InvokeAppendText(string s)
        {
            //this.textBox.AppendText(s);

            //AbortRequestedʱ���ܵ���invoke����д��־��ԭ���ǣ���ʱ��TextWriterд��Ϣ������invoke�������Ļ�������
            //��Ϊinvoke���õĺ����ǵ�ǰ�̣߳�ServiceThread.CurrentThread����ִ����ί�з��������ǽ������̣߳�MainForm�����̣߳���ִ�У�
            //�����ʱ�����̣߳�MainForm�����̣߳�������Join״̬���ȴ������̵߳Ľ������Ӷ�����������߳��໥�ȴ���������
            //this.param.Log.Write("������" + ex.Message);

            if (Thread.CurrentThread.ThreadState != System.Threading.ThreadState.AbortRequested )
                this.textBox.Invoke(new AppendTextDelegate(this.textBox.AppendText), s);
        }
    }
}
