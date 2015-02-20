using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.UI.Control.Controls;
using MCS.Library.Core;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Proxies;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
	public abstract class WFParameterWithResponseBase : WFParameterBase, IWFOperation<ResponseData>
	{
        public WFParameterWithResponseBase()
        {
            
        }
		public ResponseData Execute()
		{
			return this.Execute(null);
		}
       
		/// <summary>
		/// 执行操作，如果出现异常，可以调用补偿的操作。
		/// </summary>
		/// <param name="compensation"></param>
		/// <returns></returns>
		public ResponseData Execute(Func<Exception, bool> compensation)
		{
            string tenantCode = TenantContext.Current.TenantCode;

            if (this.TenantCode.IsNotEmpty())
                tenantCode = this.TenantCode;

            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = tenantCode;
            this.RuntimeContext.ApplicationRuntimeParameters[Consts.TenantCode] = tenantCode;

			ResponseData response = new ResponseData();

			try
			{
                //流程意见默认赋值
                if (string.IsNullOrEmpty(this.ClientOpinionComments))
                {
                    this.ClientOpinionComments = "{{**string.Empty**}}";
                }
                //添加意见
                if (!(string.IsNullOrEmpty(this.ClientOpinionId) && string.IsNullOrEmpty(this.ClientOpinionComments)))
                {
                    WfClientOpinion wfClientOpinion = new WfClientOpinion();
                    this.RuntimeContext.Opinion = wfClientOpinion;
                    wfClientOpinion.Content = this.ClientOpinionComments;
                    wfClientOpinion.ID = this.ClientOpinionId;
                }

                this.EMailCollector.TenantCode = tenantCode;
                
                this.InternalExecute(response);
			}
			catch (WfClientChannelException ex)
			{
				bool autoThrow = true;

				if (compensation != null)
					autoThrow = compensation(ex);

				if (autoThrow)
					throw;
			}

			return response;
		}

		protected abstract void InternalExecute(ResponseData response);
        protected virtual MailCollectorArg GetMailCollectorArgs()
        {
            var process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessByID(this.ProcessId, this.RuntimeContext.Operator);

            MailCollectorArg arg = new MailCollectorArg();
            if (process.Descriptor.Properties.ContainsKey(Consts.CompletedAlarmTypeCode))
            {
                arg.CompletedAlarmTypeCode = process.Descriptor.Properties[Consts.CompletedAlarmTypeCode].StringValue;
            }
            if (process.Descriptor.Properties.ContainsKey(Consts.TaskAlarmTypeCode))
            {
                arg.TaskAlarmTypeCode = process.Descriptor.Properties[Consts.TaskAlarmTypeCode].StringValue;
            }

            return arg;
        }
        protected void DefaultFillEmailCollector()
        {
            Dictionary<string, object> dic = this.RuntimeContext.ApplicationRuntimeParameters;
            DefaultFillEmailCollector(this.RuntimeContext.ApplicationRuntimeParameters);
        }
        protected void DefaultFillEmailCollector(Dictionary<string, object> dic)
        {
            dic.Keys.ForEach(p =>
            {
                if (!p.Equals(Consts.EmailCollector, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (this.EMailCollector.MailCompletedArguments.TemplateKeyValues.ContainsKey(p))
                    {
                        this.EMailCollector.MailCompletedArguments.TemplateKeyValues.Remove(p);
                    }
                    if (this.EMailCollector.MailTaskArguments.TemplateKeyValues.ContainsKey(p))
                    {
                        this.EMailCollector.MailTaskArguments.TemplateKeyValues.Remove(p);
                    }

                    string value = dic[p] == null ? string.Empty : dic[p].ToString();
                    this.EMailCollector.MailCompletedArguments.TemplateKeyValues.Add(p, value);
                    this.EMailCollector.MailTaskArguments.TemplateKeyValues.Add(p, value);
                }
            });

            var collectorArgs = this.GetMailCollectorArgs();
            this.EMailCollector.MailCompletedArguments.AlarmTypeCode = collectorArgs.CompletedAlarmTypeCode;
            this.EMailCollector.MailTaskArguments.AlarmTypeCode = collectorArgs.TaskAlarmTypeCode;

            string emailArgs = MailCollector.Serialize(this.EMailCollector);
            dic[Consts.EmailCollector] = emailArgs;
        }
	}
}
