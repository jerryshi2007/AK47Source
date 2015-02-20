using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class MailArguments
    {
        private IDictionary<string, string> _TemplateKeyValues = new Dictionary<string, string>();
        private string _AlarmTypeCode = string.Empty;
        /// <summary>
        /// 预警类型编码
        /// </summary>
        public string AlarmTypeCode
        {
            get { return _AlarmTypeCode; }
            set { _AlarmTypeCode = value; }
        }
        /// <summary>
        /// Mail中相关的参数
        /// </summary>
        public IDictionary<string, string> TemplateKeyValues
        {
            get { return _TemplateKeyValues; }
            set { _TemplateKeyValues = value; }
        }
    }
}
