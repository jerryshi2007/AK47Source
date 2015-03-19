using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Common.Test
{
    public static class Consts
    {
        public static Dictionary<string, WfClientUser> Users = new Dictionary<string, WfClientUser>(StringComparer.OrdinalIgnoreCase)
        {
            { "Requestor", new WfClientUser("f4048590-feec-4c15-990d-2f7693146937", "刘闽辉")},
            { "Approver1", new WfClientUser("80f4464f-e912-40c9-9502-c369a0d935ee", "樊海云")},
            { "CEO", new WfClientUser("5e03356b-0d68-4f58-82c3-900d2cb55feb", "李明")},
            { "CFO", new WfClientUser("0b7390ac-5578-44bc-a4dd-c4155ba5cca2", "曲毅民")},
            { "InvalidUser", new WfClientUser("11111-5578-44bc-a4dd-c4155ba5cca2", "非法用户")},

            { "OMP", new WfClientUser("D1ACF323-F725-4C5C-ACDA-D25C6F32A880", "黄揽")}
        };

        public static Dictionary<string, WfClientOrganization> Departments = new Dictionary<string, WfClientOrganization>(StringComparer.OrdinalIgnoreCase)
        {
            { "RequestorOrg", new WfClientOrganization("f53e880d-b191-4788-8477-b0ddaa6d3a57", "流程管理部")},
        };

        public static string TenantCode
        {
            get
            {
                return "TenantCode";
            }
        }
    }
}
