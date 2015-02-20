using CIIC.HSR.TSP.AN.Bizlet.Contract;
using CIIC.HSR.TSP.AN.Bizlet.Impl;
using CIIC.HSR.TSP.AN.BizObject;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class MailTaskProcessor:MailProcessorBase
    {
        private List<USER_TASKBO> _Tasks = new List<USER_TASKBO>();
        public MailTaskProcessor(MailCollector mailCollector, List<USER_TASKBO> tasks)
            : base(mailCollector)
        {
            _Tasks = tasks;
        }
        public override MailArguments GetMailArguments(MailCollector mailCollector)
        {
            return mailCollector.MailTaskArguments;
        }
        public override List<ReceiverDetail> GetReceiverIds(MailCollector mailCollector)
        {
            List<ReceiverDetail> ids = new List<ReceiverDetail>();

            if (null != _Tasks && _Tasks.Count > 0)
            {
                _Tasks.ForEach(p =>
                    {
                        IDictionary<string, string> copiedArgs = new Dictionary<string, string>();
                        foreach (var key in mailCollector.MailTaskArguments.TemplateKeyValues.Keys)
                        {
                            copiedArgs.Add(key, mailCollector.MailTaskArguments.TemplateKeyValues[key]);
                        }

                        string deliverTime = p.DELIVER_TIME == null ?
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : p.DELIVER_TIME.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

                        copiedArgs.ExistsSkip(MailArgKeys.ApplicationName, p.APPLICATION_NAME);
                        copiedArgs.ExistsSkip(MailArgKeys.ApplicationName, p.APPLICATION_NAME);
                        copiedArgs.ExistsSkip(MailArgKeys.ProgramName, p.PROGRAM_NAME);
                        copiedArgs.ExistsSkip(MailArgKeys.CreatorName, p.DRAFT_USER_NAME);
                        copiedArgs.ExistsSkip(MailArgKeys.DeliverTime, deliverTime);
                        copiedArgs.ExistsSkip(MailArgKeys.ReceiverName, p.SEND_TO_USER);
                        copiedArgs.ExistsSkip(MailArgKeys.TaskTitle, p.TASK_TITLE);
                        copiedArgs.ExistsSkip(MailArgKeys.Url, p.URL);

                        ids.Add(new ReceiverDetail()
                        {
                            Id = p.SEND_TO_USER,
                            Args = copiedArgs
                        });
                    });
            }

            return ids;
        }
    }
}
