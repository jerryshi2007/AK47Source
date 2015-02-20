using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceBrokerSoapHeader : SoapHeader
    {
        private bool useServerCache = true;
        private DateTime timePoint = DateTime.MinValue;

        private List<SoapHeaderConnectionMappingItem> connectionMappings = new List<SoapHeaderConnectionMappingItem>();
        private List<SoapHeaderContextItem> context = new List<SoapHeaderContextItem>();

        /// <summary>
        /// 是否使用服务端缓存
        /// </summary>
        public bool UseServerCache
        {
            get
            {
                return this.useServerCache;
            }
            set
            {
                this.useServerCache = value;
            }
        }

        /// <summary>
        /// 是否使用时光穿梭
        /// </summary>
        public DateTime TimePoint
        {
            get
            {
                return this.timePoint;
            }
            set
            {
                this.timePoint = value;
            }
        }

        /// <summary>
        /// 连接映射
        /// </summary>
        public List<SoapHeaderConnectionMappingItem> ConnectionMappings
        {
            get
            {
                return this.connectionMappings;
            }
        }

        /// <summary>
        /// 上下文信息
        /// </summary>
        public List<SoapHeaderContextItem> Context
        {
            get
            {
                return this.context;
            }
        }
    }
}
