using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Common.Test
{
    public class ActivityDescriptorHelper
    {
        /// <summary>
        /// 创建一个不带资源的简单活动
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="actType"></param>
        /// <returns></returns>
        public static WfClientActivityDescriptor CreateSimpleClientActivity(string key, string name, WfClientActivityType actType)
        {
            WfClientActivityDescriptor actDesp = new WfClientActivityDescriptor(actType);

            actDesp.Key = key;
            actDesp.Name = name;

            actDesp.RelativeLinks.Add(new WfClientRelativeLinkDescriptor("AR1") { Category = "Activity", Url = "http://www.ak47.com" });

            return actDesp;
        }

        public static WfClientActivityDescriptor CreateSimpleClientActivityWithUser(string key, string name, string userKey, WfClientActivityType actType)
        {
            WfClientActivityDescriptor actDesp = CreateSimpleClientActivity(key, name, actType);

            actDesp.Resources.Add(new WfClientUserResourceDescriptor(Consts.Users[userKey]));

            return actDesp;
        }

        public static WfActivityDescriptor CreateSimpleServerActivity(string key, string name, WfActivityType actType)
        {
            WfActivityDescriptor actDesp = new WfActivityDescriptor(key, actType);

            actDesp.Name = name;

            actDesp.RelativeLinks.Add(new WfRelativeLinkDescriptor("AR1") { Category = "Activity", Url = "http://www.ak47.com" });

            return actDesp;
        }
    }
}
