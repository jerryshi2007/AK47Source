using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WcfExtensions
{
    /// <summary>
    /// 通过WfClientChannel调用远程服务返回的异常
    /// </summary>
    public class WfClientChannelException : System.Exception
    {
        private string _Detail = string.Empty;

        public WfClientChannelException()
        {
        }

        public WfClientChannelException(string message)
            : base(message)
        {
        }

        public WfClientChannelException(string message, System.Exception innerException)
        {
        }

        public string Detail
        {
            get
            {
                return this._Detail;
            }
            set
            {
                this._Detail = value;
            }
        }
    }
}
