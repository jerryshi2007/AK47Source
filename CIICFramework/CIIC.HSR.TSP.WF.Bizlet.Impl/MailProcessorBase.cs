using CIIC.HSR.TSP.AN.Bizlet.Contract;
using CIIC.HSR.TSP.AN.Bizlet.Impl;
using CIIC.HSR.TSP.AN.BizObject;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public abstract class MailProcessorBase
    {
        private MailCollector _MailCollector = null;
        public MailProcessorBase(MailCollector mailCollector)
        {
            _MailCollector = mailCollector;
        }
        public void Send()
        {
            List<ReceiverDetail> receiverIds = GetReceiverIds(_MailCollector);
            MailArguments templateArgs = GetMailArguments(_MailCollector);

            if (receiverIds.Count == 0 || string.IsNullOrEmpty(templateArgs.AlarmTypeCode))
            {
                return;
            }

            ServiceAdapterBizlet serviceAdapter = ServiceAdapterBizlet.CreateInstance(_MailCollector.IsTenantMode);
            serviceAdapter.Runtime.User.UserId = "system";
            serviceAdapter.Runtime.TenantCode = _MailCollector.TenantCode;
            IAlarmBizlet ialarm = serviceAdapter.GetAlarmService();

            try
            {
                receiverIds.ForEach(p =>
                {
                    ReceiverDescription receiver = FillEmailAddress(p);
                    if (null != receiver)
                    {
                        ialarm.SendAlarm(templateArgs.AlarmTypeCode, p.Args, new List<ReceiverDescription>() { receiver });
                    }
                });
            }
            catch (Exception ex)
            {
                Logging.Loggers.Default.Error("发送邮件通知时错误", ex);
            }
            
        }
        public abstract List<ReceiverDetail> GetReceiverIds(MailCollector mailCollector);
        public abstract MailArguments GetMailArguments(MailCollector mailCollector);
        /// <summary>
        /// 填充邮件地址
        /// </summary>
        /// <param name="receiverIds">为填充邮件地址的用户列表</param>
        /// <returns>填充邮件地址之后的邮件列表</returns>
        private ReceiverDescription FillEmailAddress(ReceiverDetail receiverIds)
        {
            ReceiverDescription result = null;
            var userBizlet = Containers.Global.Singleton.Resolve<IAAUserBizlet>();
            AAUserBO user = userBizlet.GetAAUserByUserID(new Guid(receiverIds.Id));
            if (null != user)
            {
                result = new ReceiverDescription()
                {
                    CustomerCode = string.IsNullOrEmpty(user.CustEmpCode) ? user.UserName : user.CustEmpCode,
                    PeopleEmail = user.Email,
                    PeopleMobile = user.Mobile,
                    PeopleName = user.UserName,
                    UserCode = user.UserId.ToString()
                };
            }

            return result;
        }
    }
}
