using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class SecurityCheckServiceBizlet : ISecurityCheckServiceBizlet
    {
        public System.Data.DataSet GetApplications()
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            return arpService.GetApplications();
        }

        public System.Data.DataSet GetRoles(string appCodeName, Common.RightMaskType rightMask)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            return arpService.GetRoles();
        }

        public System.Data.DataSet GetFunctions(string appCodeName, Common.RightMaskType rightMask)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            return arpService.GetResource();
        }

        public System.Data.DataSet GetFunctionsRoles(string appCodeName, string funcCodeNames)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            return arpService.GetResourceRoles(funcCodeNames);
        }

        public System.Data.DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, Common.DelegationMaskType delegationMask, Common.SidelineMaskType sidelineMask, string extAttr)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            return arpService.GetUsersInRoles(roleCodeNames);
        }

        public System.Data.DataSet GetUserRoles(string userValue, string appCodeName, Common.UserValueType userValueType, Common.RightMaskType rightMask, Common.DelegationMaskType delegationMask)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            string userId = userValue;
            if (userValueType.Equals(UserValueType.LogonName))
            {
                TranslatorUser user = arpService.GetUserByLogonName(userValue);
                if (null != user)
                {
                    userId = user.UserId.ToString();
                }
            }
            else if (userValueType.Equals(UserValueType.PersonID))
            {
                TranslatorUser user = arpService.GetUserByEmpCode(userValue);
                if (null != user)
                {
                    userId = user.UserId.ToString();
                }
            }

            return arpService.GetRolesByUserId(userId);
        }

        public System.Data.DataSet GetUserPermissions(string userValue, string appCodeName, Common.UserValueType userValueType, Common.RightMaskType rightMask, Common.DelegationMaskType delegationMask)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            string userId = userValue;
            if (userValueType.Equals(UserValueType.LogonName))
            {
                TranslatorUser user = arpService.GetUserByLogonName(userValue);
                if (null != user)
                {
                    userId = user.UserId.ToString();
                }
            }
            else if (userValueType.Equals(UserValueType.PersonID))
            {
                TranslatorUser user = arpService.GetUserByEmpCode(userValue);
                if (null != user)
                {
                    userId = user.UserId.ToString();
                }
            }

            return arpService.GetResourcesByUserId(userId);
        }

        public System.Data.DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank, bool includeDelegate)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            return arpService.GetUsersInRoles(roleCodeNames);
        }

        public ServiceContext Context
        {
            get;
            set;
        }
    }
}
