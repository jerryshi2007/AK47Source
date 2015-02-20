using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CIIC.HSR.TSP.Services;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    public interface IWFUserContext
    {
        WFUserContext GetUser();
    }
    public class DefaultUserContext : IWFUserContext
    {
        public WFUserContext GetUser()
        {
            if (null != HttpContext.Current)
            {
                IUserInfo userInfo= HttpContext.Current.GetCurrentUserInfo();
                if (null != userInfo)
                {
                    WFUserContext clientUser = new WFUserContext();
                    clientUser.TenantCode = userInfo.TenentCode;
                    clientUser.WfClientUser.ID = userInfo.UserID.ToString();
                    clientUser.WfClientUser.Name = userInfo.UserName;
                    return clientUser;
                }
            }
            return null;
        }
    }
}
