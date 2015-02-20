using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using MCS.Library.Core;
using MCS.Library.WcfExtensions.Configuration;
using System.ServiceModel.Channels;

namespace MCS.Library.WcfExtensions
{
    /// <summary>
    /// 提供一个服务宿主，宿主内的服务已包含Metadata结点，服务地址绑定一个平台定制JSON格式的结点
    /// </summary>
    public class WfServiceHost : ServiceHost
    {
        private readonly string _ImplementedContract;
        private readonly bool _AtlasEnabled;
        private readonly WfServiceBindingMode _BindingMode = WfServiceBindingMode.WfRawContentWebHttpBinding;

        public WfServiceHost(Type serviceType, Uri[] baseAddress, string implementedContract)
            : base(serviceType, baseAddress)
        {
            this._ImplementedContract = implementedContract;
        }

        public WfServiceHost(Type serviceType, Uri[] baseAddress, string implementedContract, bool atlasEnabled)
            : this(serviceType, baseAddress, implementedContract)
        {
            this._AtlasEnabled = atlasEnabled;
        }

        public WfServiceHost(Type serviceType, Uri[] baseAddress, string implementedContract, bool atlasEnabled, WfServiceBindingMode bindingMode)
            : this(serviceType, baseAddress, implementedContract)
        {
            this._AtlasEnabled = atlasEnabled;
            this._BindingMode = bindingMode;
        }

        protected override void InitializeRuntime()
        {
            ServiceMetadataBehavior metaData = this.Description.Behaviors.GetBehavior<ServiceMetadataBehavior>();

            metaData.HttpGetEnabled = true;

            ServiceEndpoint endPoint = this.Description.Endpoints.FindByContractName(this._ImplementedContract);

            if (endPoint == null)
            {
                endPoint = this.AddServiceEndpoint(this._ImplementedContract, PrepareServiceEndPoint(this._BindingMode), "");
            }

            if (endPoint.Behaviors.Contains(typeof(WfJsonWebHttpBehavior)) == false)
            {
                if (this._BindingMode == WfServiceBindingMode.WfRawContentWebHttpBinding)
                    endPoint.Behaviors.Add(new WfJsonWebHttpBehavior());
            }

            if (this._AtlasEnabled && endPoint.Behaviors.Contains(typeof(WfWebScriptBehavior)) == false)
            {
                //platform script proxy 
                endPoint.Behaviors.Add(new WfWebScriptBehavior());
            }

            base.InitializeRuntime();
        }

        private static Binding PrepareServiceEndPoint(WfServiceBindingMode bindingMode)
        {
            Binding result = null;

            switch (bindingMode)
            {
                case WfServiceBindingMode.WfRawContentWebHttpBinding:
                    result = new WfRawContentWebHttpBinding();
                    break;
                case WfServiceBindingMode.BasicHttpBinding:
                    result = new BasicHttpBinding();
                    break;
                case WfServiceBindingMode.WSHttpBinding:
                    result = new WSHttpBinding();
                    break;
                default:
                    throw new SystemSupportException(string.Format("不能支持的服务绑定模式:{0}", bindingMode));
            }

            return result;
        }
    }
}
