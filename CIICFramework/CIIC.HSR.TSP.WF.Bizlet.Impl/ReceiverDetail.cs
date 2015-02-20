using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class ReceiverDetail
    {
        private IDictionary<string, string> _Args = new Dictionary<string, string>();
        /// <summary>
        /// 接收人待办中的系统参数
        /// </summary>
        public IDictionary<string, string> Args
        {
            get { return _Args; }
            set { _Args = value; }
        }
        /// <summary>
        /// 接收人Id
        /// </summary>
        public string Id { get; set; }
    }
}
