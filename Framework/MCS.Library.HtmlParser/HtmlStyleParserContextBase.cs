using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HtmlStyleParserContextBase<T> : IDisposable
    {
        private StringBuilder _Buffer = new StringBuilder();
        private TextWriter _Writer = null;
        private T _Stage;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlStyleParserContextBase()
        {
            this._Writer = new StringWriter(this._Buffer);
            this.Index = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Stage
        {
            get
            {
                return this._Stage;
            }
            set
            {
                this._Stage = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextWriter Writer
        {
            get
            {
                return this._Writer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool BufferIsEmpty
        {
            get
            {
                return this._Buffer.Length == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public string ChangeStage(T stage)
        {
            this._Stage = stage;

            return this.ResetWriter().Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string ResetWriter()
        {
            string result = this._Buffer.ToString();

            this._Buffer.Clear();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._Writer != null)
                    this._Writer.Dispose();
            }
        }
    }
}
