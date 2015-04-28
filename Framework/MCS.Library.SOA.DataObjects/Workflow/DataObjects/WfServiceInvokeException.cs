using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;


namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 服务调用的异常
    /// </summary>
    [Serializable]
    public class WfServiceInvokeException : Exception
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public WfServiceInvokeException()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message"></param>
        public WfServiceInvokeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WfServiceInvokeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public WfServiceInvokeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// 状态文本
        /// </summary>
        public string StatusDescription
        {
            get;
            internal set;
        }

        /// <summary>
        /// 从Response获取错误信息
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static WfServiceInvokeException FromWebResponse(WebResponse response)
        {
            StringBuilder strB = new StringBuilder();

            string content = string.Empty;

            using (Stream stream = response.GetResponseStream())
            {
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                content = streamReader.ReadToEnd();
            }

            if (response is HttpWebResponse)
            {
                HttpWebResponse hwr = (HttpWebResponse)response;

                strB.AppendFormat("Status Code: {0}, Description: {1}\n", hwr.StatusCode, hwr.StatusDescription);
            }

            strB.AppendLine(content);

            WfServiceInvokeException result = new WfServiceInvokeException(strB.ToString());

            if (response is HttpWebResponse)
            {
                HttpWebResponse hwr = (HttpWebResponse)response;

                result.StatusCode = hwr.StatusCode;
                result.StatusDescription = hwr.StatusDescription;
            }

            return result;
        }
    }
}
