using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.UI.Control.Controls;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Library.WcfExtensions;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using System.Threading;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    /// <summary>
    /// 流程上下文创建帮助
    /// </summary>
    public static class WFContextHelper
    {
        /// <summary>
        /// 根据请求上下文创建流程上下文
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns>流程上下文</returns>
        public static WFUIRuntimeContext GetWFContext(this HttpRequestBase request)
        {
            WFUIRuntimeContext result = null;

            if (request.RequestContext.HttpContext.Items.Contains("WFContext") == false)
            {
                result = WFUIRuntimeContext.InitByHttpRequest(request);

                request.RequestContext.HttpContext.Items["WFContext"] = result;
            }
            else
                result = (WFUIRuntimeContext)request.RequestContext.HttpContext.Items["WFContext"];

            return result;
        }

        /// <summary>
        /// 如果已经存在流程上下文，则重新刷新流程上下文
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static WFUIRuntimeContext ReloadWFContext(this HttpRequestBase request)
        {
            WFUIRuntimeContext currentContext = GetWFContext(request);

            if (currentContext.Process != null)
            {
                if (currentContext.Process.CurrentActivity != null)
                    currentContext = WFUIRuntimeContext.InitByActivityID(currentContext.Process.CurrentActivity.ID);
                else
                    currentContext = WFUIRuntimeContext.InitByProcessID(currentContext.Process.ID);

                request.RequestContext.HttpContext.Items["WFContext"] = currentContext;
            }

            return currentContext;
        }
    }

    /// <summary>
    /// 流程上下文
    /// </summary>
    public class WFUIRuntimeContext
    {
        private WFUIRuntimeContext()
        {
        }

        private WFUIRuntimeContext(string activityID)
        {
            this.ActivityID = activityID;
        }

        internal static WFUIRuntimeContext InitByHttpRequest(HttpRequestBase request)
        {
            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = GetCurrentTenantCode();
            WfClientServiceBrokerContext.Current.Context[Consts.Culture] = Thread.CurrentThread.CurrentCulture.Name;

            WFUIRuntimeContext result = null;

            if (request.QueryString["activityID"] != null)
                result = InitByActivityID(request.QueryString["activityID"]);
            else
                if (request.QueryString["processID"] != null)
                    result = InitByProcessID(request.QueryString["processID"]);
                else
                    if (request.QueryString["resourceID"] != null)
                        result = InitByResourceID(request.QueryString["resourceID"]);
                    else
                        result = InitWithoutProcessInfo();

            return result;
        }

        internal static WFUIRuntimeContext InitByActivityID(string activityID)
        {
            WFUIRuntimeContext result = new WFUIRuntimeContext(activityID);

            result.CurrentUser = GetCurrentUser();
            result.TenantCode = GetCurrentTenantCode();
            result.Process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByActivityID(activityID, result.CurrentUser);

            return result;
        }

        internal static WFUIRuntimeContext InitByProcessID(string processID)
        {
            WfClientUser user = GetCurrentUser();

            WfClientProcessInfo processInfo = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByID(processID, user);

            WFUIRuntimeContext result = new WFUIRuntimeContext(processInfo.CurrentActivity.ID);

            result.TenantCode = GetCurrentTenantCode();
            result.Process = processInfo;
            result.CurrentUser = user;

            return result;
        }

        private static WFUIRuntimeContext InitByResourceID(string resourceID)
        {
            WfClientUser user = GetCurrentUser();

            WfClientProcessInfoCollection processesInfo = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByResourceID(resourceID, user);

            (processesInfo.Count > 0).FalseThrow("不能根据'{0}'找到ResourceID对应的流程", resourceID);

            WfClientProcessInfo processInfo = processesInfo.Find(p =>
            {
                return p.HasParentProcess == false;
            });

            if (processInfo == null)
                processInfo = processesInfo[0];

            WFUIRuntimeContext result = new WFUIRuntimeContext(processInfo.CurrentActivity.ID);

            result.TenantCode = GetCurrentTenantCode();
            result.Process = processInfo;
            result.CurrentUser = user;

            return result;
        }

        private static WFUIRuntimeContext InitWithoutProcessInfo()
        {
            WFUIRuntimeContext result = new WFUIRuntimeContext();

            result.CurrentUser = GetCurrentUser();
            result.TenantCode = GetCurrentTenantCode();

            return result;
        }

        private static WfClientUser GetCurrentUser()
        {
            WfClientUser user = null;

            IWFUserContext userContext = Containers.Global.Singleton.Resolve<IWFUserContext>();

            if (null != userContext)
                user = userContext.GetUser().WfClientUser;

            return user;
        }

        private static string GetCurrentTenantCode()
        {
            string tenantCode = null;

            IWFUserContext userContext = Containers.Global.Singleton.Resolve<IWFUserContext>();

            if (null != userContext)
                tenantCode = userContext.GetUser().TenantCode;

            return tenantCode;
        }

        public WfClientUser CurrentUser
        {
            get;
            private set;
        }

        public WfClientProcessInfo Process
        {
            get;
            private set;
        }

        public string TenantCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 节点ID
        /// </summary>
        public string ActivityID
        {
            get;
            set;
        }
    }
}
