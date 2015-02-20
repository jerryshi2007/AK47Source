using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 邮件参数搜集器
    /// </summary>
    public class MailCollector
    {
        private MailArguments _MailTaskArguments = new MailArguments();
        private MailArguments _MailCompletedArguments = new MailArguments();
        /// <summary>
        /// 待办预警设置
        /// </summary>
        public MailArguments MailCompletedArguments
        {
            get { return _MailCompletedArguments; }
            set { _MailCompletedArguments = value; }
        }
        /// <summary>
        /// 流程完成的预警设置
        /// </summary>
        public MailArguments MailTaskArguments
        {
            get { return _MailTaskArguments; }
            set { _MailTaskArguments = value; }
        }
        /// <summary>
        /// 租户编码
        /// </summary>
        public string TenantCode { get; set; }
        /// <summary>
        /// 是否为多租户模式
        /// </summary>
        public bool IsTenantMode { get; set; }
        /// <summary>
        /// 反序列化邮件参数
        /// </summary>
        /// <param name="json">被序列化的数据</param>
        /// <returns>邮件参数</returns>
        public static MailCollector Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<MailCollector>(json);
        }
        /// <summary>
        /// 序列化邮件参数
        /// </summary>
        /// <param name="mailCollector">邮件参数</param>
        /// <returns>json</returns>
        public static string Serialize(MailCollector mailCollector)
        {
            if (null == mailCollector)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(mailCollector);
        }
    }
}
