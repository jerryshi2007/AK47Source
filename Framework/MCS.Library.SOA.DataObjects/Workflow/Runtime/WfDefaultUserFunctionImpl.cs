using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    internal class WfDefaultUserFunctionImpl : IWfCalculateUserFunction
    {
        public object CalculateUserFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
        {
            object result = null;

            switch (funcName.ToLower())
            {
                case "currentuserinrole":   //CurrentUserInRole
                    CheckParamsCount(funcName, arrParams, 1);
                    result = CurrentUserInRole((string)arrParams[0].Value, callerContext);
                    break;
                case "userinrole":  //UserInRole
                    CheckParamsCount(funcName, arrParams, 2);

                    if (arrParams[1].Value == null)
                        result = false;
                    else
                    {
                        (arrParams[1].Value is IUser).FalseThrow("用户自定义函数{0}的第二个参数值必须是IUser类型", funcName);
                        result = UserInRole((string)arrParams[0].Value, (IUser)arrParams[1].Value, callerContext);
                    }
                    break;
                case "aresamecandidates":   //AreSameCandidates
                    (arrParams.Count == 0 || arrParams.Count == 2).FalseThrow("用户自定义函数{0}必须没有参数或者有两个参数", funcName);

                    switch (arrParams.Count)
                    {
                        case 0:
                            result = AreSameCandidates(callerContext);
                            break;
                        case 2:
                            result = AreSameCandidates((string)arrParams[0].Value, (string)arrParams[1].Value, callerContext);
                            break;
                    }
                    break;
                case "branchesreturnvalue": //BranchesReturnValue
                    result = BranchesReturnValue(callerContext);
                    break;
                case "alltrue": //AllTrue
                    result = BranchProcessReturnType.AllTrue.ToString();
                    break;
                case "allfalse":    //AllFalse
                    result = BranchProcessReturnType.AllFalse.ToString();
                    break;
                case "partialtrue": //PartialTrue
                    result = BranchProcessReturnType.PartialTrue.ToString();
                    break;
                case "containsuser":    //ContainsUser
                    result = ContainsUserImpl(funcName, arrParams, callerContext);
                    break;
                case "allelapsedoperators": //AllElapsedOperators
                    result = AllElapsedOperators(callerContext);
                    break;
            }

            return result;
        }

        private static bool ContainsUserImpl(string funcName, ParamObjectCollection arrParams, object callerContext)
        {
            CheckParamsCount(funcName, arrParams, 2);

            (arrParams[0].Value is IEnumerable<IUser>).FalseThrow("用户自定义函数{0}的第一个参数值必须是IEnumerable<IUser>类型", funcName);
            (arrParams[1].Value is IUser).FalseThrow("用户自定义函数{0}的第二个参数值必须是IUser类型", funcName);

            IEnumerable<IUser> users = (IEnumerable<IUser>)arrParams[0].Value;
            IUser target = (IUser)arrParams[1].Value;

            return users.Contains(target, OguObjectIDEqualityComparer<IUser>.Default);
        }

        private static bool AreSameCandidates(object callerContext)
        {
            bool result = false;

            if (callerContext is WfConditionDescriptor)
            {
                WfConditionDescriptor condition = (WfConditionDescriptor)callerContext;

                if (condition.Owner != null && condition.Owner is IWfTransitionDescriptor)
                {
                    IWfTransitionDescriptor transitionDesp = (IWfTransitionDescriptor)condition.Owner;
                    result = AreSameCandidates(transitionDesp.FromActivity, transitionDesp.ToActivity);
                }
            }

            return result;
        }

        private static bool AreSameCandidates(string actDesp1, string actDesp2, object callerContext)
        {
            bool result = false;

            if (callerContext is WfConditionDescriptor && actDesp1.IsNotEmpty() && actDesp2.IsNotEmpty())
            {
                WfConditionDescriptor condition = (WfConditionDescriptor)callerContext;

                if (condition.Owner != null)
                {
                    IWfProcess process = condition.Owner.ProcessInstance;

                    result = AreSameCandidates(process.Descriptor.Activities[actDesp1], process.Descriptor.Activities[actDesp2]);
                }
            }

            return result;
        }

        private static string BranchesReturnValue(object callerContext)
        {
            string result = BranchProcessReturnType.AllTrue.ToString();

            if (callerContext is WfConditionDescriptor)
            {
                WfConditionDescriptor condition = (WfConditionDescriptor)callerContext;

                if (condition.Owner != null)
                {
                    IWfProcess process = condition.Owner.ProcessInstance;

                    result = condition.Owner.ProcessInstance.CurrentActivity.BranchProcessReturnValue.ToString();
                }
            }

            return result;
        }

        private static bool AreSameCandidates(IWfActivityDescriptor fromActDesp, IWfActivityDescriptor toActDesp)
        {
            bool result = false;

            if (fromActDesp.Instance != null && toActDesp.Instance != null)
                result = fromActDesp.Instance.Candidates.AreSameAssignees(toActDesp.Instance.Candidates);

            return result;
        }

        private static bool UserInRole(string rolesNames, IUser user, object callerContext)
        {
            bool result = DeluxePrincipal.IsInRole(user, rolesNames);

            if (result == false)
                result = IsInSOARoles(user, rolesNames, callerContext);

            return result;
        }

        private static bool CurrentUserInRole(string rolesNames, object callerContext)
        {
            bool result = false;

            if (DeluxePrincipal.IsAuthenticated)
            {
                result = DeluxePrincipal.Current.IsInRole(rolesNames);

                if (result == false)
                    result = IsInSOARoles(DeluxeIdentity.CurrentUser, rolesNames, callerContext);
            }

            return result;
        }

        private static IEnumerable<IUser> AllElapsedOperators(object callerContext)
        {
            List<IUser> result = new List<IUser>();

            if (callerContext is WfConditionDescriptor)
            {
                WfConditionDescriptor condition = (WfConditionDescriptor)callerContext;

                if (condition.Owner != null)
                {
                    foreach (IWfActivity activity in condition.Owner.ProcessInstance.Activities)
                    {
                        if (activity.Status == WfActivityStatus.Completed)
                        {
                            if (OguUser.IsNotNullOrEmpty(activity.Operator))
                            {
                                if (result.NotExists(u => string.Compare(u.ID, activity.Operator.ID, true) == 0))
                                    result.Add(activity.Operator);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static bool IsInSOARoles(IUser user, string rolesNames, object callerContext)
        {
            bool result = false;

            IRole[] roles = DeluxePrincipal.GetRoles(rolesNames);

            for (int i = 0; i < roles.Length; i++)
            {
                result = IsInSOARole(user, SOARole.CreateWrapperObject(roles[i]), callerContext);

                if (result)
                    break;
            }

            return result;
        }

        private static bool IsInSOARole(IUser user, IRole role, object callerContext)
        {
            bool result = false;

            SOARoleContext context = PrepareRoleContext(role, callerContext);

            try
            {
                result = role.ObjectsInRole.Contains(user, new OguObjectIDEqualityComparer<IOguObject>());
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }

            return result;
        }

        private static SOARoleContext PrepareRoleContext(IRole role, object callerContext)
        {
            SOARoleContext result = null;

            if (callerContext is WfConditionDescriptor)
            {
                WfConditionDescriptor condition = (WfConditionDescriptor)callerContext;
                IWfProcess process = null;

                if (condition.Owner != null)
                    process = condition.Owner.ProcessInstance;

                result = SOARoleContext.CreateContext(role, process);
            }

            return result;
        }

        private static void CheckParamsCount(string funcName, ParamObjectCollection arrParams, int count)
        {
            (arrParams.Count == count).FalseThrow("用户自定义函数\"{0}\"必须有{1}个参数", funcName, count);
        }
    }
}
