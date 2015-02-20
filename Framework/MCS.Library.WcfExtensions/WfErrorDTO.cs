using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.WcfExtensions
{
    [Serializable]
    public class WfErrorDTO : MarshalByRefObject
    {
        /// <summary>
        /// 异常名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 异常编号
        /// 100 平台WCF服务产生的异常,由WfErrorHandler引发
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            StringBuilder strB = new StringBuilder();

            strB.AppendLine(this.Number.ToString());
            strB.AppendLine(this.Name);
            strB.AppendLine(this.Message);
            strB.AppendLine(this.Description);

            return strB.ToString();
        }
    }
}
