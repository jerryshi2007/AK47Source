using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;
using MCS.Library.Data;
using MCS.Library.Core;

namespace MCS.Library.WcfExtensions
{
    internal class WfServerContextInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _InnerInvoker = null;

        public WfServerContextInvoker(IOperationInvoker innerInvoker)
        {
            this._InnerInvoker = innerInvoker;
        }

        public object[] AllocateInputs()
        {
            return this._InnerInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            Message messge = OperationContext.Current.RequestContext.RequestMessage;

            object mappingObject = null;

            Dictionary<string, string> connectionMappings = null;

            if (messge.Properties.TryGetValue("ConnectionMappings", out mappingObject) == false)
                connectionMappings = new Dictionary<string, string>();
            else
                connectionMappings = (Dictionary<string, string>)mappingObject;

            InitConnectionMappings(connectionMappings);

            object contextObject = null;

            if (messge.Properties.TryGetValue("Context", out contextObject))
            {
                Dictionary<string, object> context = (Dictionary<string, object>)contextObject;

                if (context.ContainsKey("TenantCode"))
                    TenantContext.Current.TenantCode = (string)context["TenantCode"];

                if (context.ContainsKey("Culture"))
                    ExceptionHelper.DoSilentAction(() => Thread.CurrentThread.CurrentCulture = new CultureInfo((string)context["Culture"]));
            }

            return this._InnerInvoker.Invoke(instance, inputs, out outputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return this._InnerInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return this._InnerInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get
            {
                return this._InnerInvoker.IsSynchronous;
            }
        }

        private void InitConnectionMappings(Dictionary<string, string> connectionMappings)
        {
            foreach (KeyValuePair<string, string> kp in connectionMappings)
            {
                DbConnectionMappingContext.CreateMapping(kp.Key, kp.Value);
            }
        }
    }
}
