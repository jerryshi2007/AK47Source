using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MCS.Library.WcfExtensions
{
    public class WfClientChannelFactory<T> : ChannelFactory<T>
    {
        /// <summary>
        /// 使用WfRawContentWebHttpBinding初始化客户端
        /// </summary>
        /// <param name="address"></param>
        public WfClientChannelFactory(EndpointAddress address)
            : this(new WfRawContentWebHttpBinding(), address)
        {

        }

        public WfClientChannelFactory(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
#if DEBUG
            //Debug版设置一个长超时，便于断点调试
            binding.ReceiveTimeout = TimeSpan.FromSeconds(600);
            binding.SendTimeout = TimeSpan.FromSeconds(600);
#endif

            if (binding is WfRawContentWebHttpBinding)
            {
                ((WfRawContentWebHttpBinding)binding).MaxReceivedMessageSize = int.MaxValue;

                if (!this.Endpoint.Behaviors.Contains(typeof(WfJsonWebHttpBehavior)))
                    this.Endpoint.Behaviors.Add(new WfJsonWebHttpBehavior());
            }
        }
    }
}
