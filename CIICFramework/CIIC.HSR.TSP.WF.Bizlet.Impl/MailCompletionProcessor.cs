using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.WF.Bizlet.Common;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class MailCompletionProcessor:MailProcessorBase
    {
        private ProcessBO _ProcessBO=null;
        public MailCompletionProcessor(MailCollector mailCollector, ProcessBO process)
            : base(mailCollector)
        {
            _ProcessBO=process;
        }
        public override MailArguments GetMailArguments(MailCollector mailCollector)
        {
            return mailCollector.MailCompletedArguments;
        }
        public override List<ReceiverDetail> GetReceiverIds(MailCollector mailCollector)
        {
            mailCollector.MailTaskArguments.TemplateKeyValues.ExistsSkip(MailArgKeys.CreatorName, _ProcessBO.CreatorName);
            return new List<ReceiverDetail>() { new ReceiverDetail() { 
                Id = _ProcessBO.CreatorId, Args = mailCollector.MailTaskArguments.TemplateKeyValues } };
        }
    }
}
