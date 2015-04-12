using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 角色的上下文信息。如果根据角色属性（矩阵）判断时，依据此上下文中提供的属性值
    /// </summary>
    public class SOARoleContext : IDisposable
    {
        private SOARolePropertiesQueryParamCollection _QueryParams = null;
        private IWfProcess _Process = null;
        private SOARolePropertyDefinitionCollection _PropertyDefinitions = null;

        private static string CacheKey
        {
            get
            {
                return typeof(SOARoleContext).Name;
            }
        }

        public SOARolePropertiesQueryParamCollection QueryParams
        {
            get
            {
                if (this._QueryParams == null)
                    this._QueryParams = new SOARolePropertiesQueryParamCollection();

                return this._QueryParams;
            }
        }

        public IWfProcess Process
        {
            get
            {
                return this._Process;
            }
        }

        public SOARolePropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                if (this._PropertyDefinitions == null)
                    this._PropertyDefinitions = new SOARolePropertyDefinitionCollection();

                return this._PropertyDefinitions;
            }
            private set
            {
                this._PropertyDefinitions = value;
            }
        }

        /// <summary>
        /// 根据流程和角色创建SOARoleContext
        /// </summary>
        /// <param name="role"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public static SOARoleContext CreateContext(IRole role, IWfProcess process)
        {
            role.NullCheck("role");

            SOARole soaRole = (SOARole)SOARole.CreateWrapperObject(role);

            return CreateContext(soaRole.PropertyDefinitions, process);
        }

        public static SOARoleContext CreateContext(SOARolePropertyDefinitionCollection propertyDefines, IWfProcess process)
        {
            propertyDefines.NullCheck("propertyDefines");

            SOARolePropertiesQueryParamCollection queryParams = CreateQueryParams(propertyDefines, process);

            SOARoleContext context = SOARoleContext.CreateContext(queryParams, process);

            context.PropertyDefinitions = propertyDefines;

            return context;
        }

        private static SOARolePropertiesQueryParamCollection CreateQueryParams(SOARolePropertyDefinitionCollection propertyDefines, IWfProcess process)
        {
            SOARolePropertiesQueryParamCollection queryParams = new SOARolePropertiesQueryParamCollection();

            WfApplicationParametersContext apContext = WfApplicationParametersContext.Current;

            //如果是审批矩阵，只取第一列作为条件
            if (propertyDefines.MatrixType == WfMatrixType.ApprovalMatrix)
            {
                if (propertyDefines.Count > 0)
                {
                    SOARolePropertyDefinition pd = propertyDefines[0];

                    object arpValue = GetQueryValueFromContext(pd, process);

                    queryParams.Add(new SOARolePropertiesQueryParam()
                    {
                        QueryName = pd.Name,
                        QueryValue = arpValue
                    });
                }
            }
            else
            {
                //活动矩阵和角色矩阵，排除保留字
                foreach (SOARolePropertyDefinition pd in propertyDefines)
                {
                    //是否是可查询的列
                    if (IsQueryableColumn(pd, propertyDefines))
                    {
                        object arpValue = GetQueryValueFromContext(pd, process);

                        queryParams.Add(new SOARolePropertiesQueryParam()
                        {
                            QueryName = pd.Name,
                            QueryValue = arpValue
                        });
                    }

                }
            }

            return queryParams;
        }

        /// <summary>
        /// 从上下文和流程中得到值
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        private static object GetQueryValueFromContext(SOARolePropertyDefinition pd, IWfProcess process)
        {
            WfApplicationParametersContext apContext = WfApplicationParametersContext.Current;

            object arpValue = null;

            if (apContext != null)
                arpValue = apContext.ApplicationRuntimeParameters.GetValue(pd.Name, (string)null);

            if (arpValue == null && process != null)
                arpValue = process.ApplicationRuntimeParameters.GetValueRecursively(pd.Name, (string)null);

            return arpValue;
        }

        private static bool IsQueryableColumn(SOARolePropertyDefinition pd, SOARolePropertyDefinitionCollection propertyDefines)
        {
            bool isReservered = true;

            switch (propertyDefines.MatrixType)
            {
                case WfMatrixType.RoleMatrix:
                    isReservered = SOARolePropertyDefinition.IsRoleMatrixReservedPropertyName(pd.Name);
                    break;
                case WfMatrixType.ActivityMatrix:
                    isReservered = SOARolePropertyDefinition.IsActivityMatrixReservedPropertyName(pd.Name);
                    break;
            }

            return isReservered == false;
        }

        public static SOARoleContext CreateContext(IEnumerable<SOARolePropertiesQueryParam> queryParams)
        {
            return CreateContext(queryParams, null);
        }

        public static SOARoleContext CreateContext(IEnumerable<SOARolePropertiesQueryParam> queryParams, IWfProcess process)
        {
            queryParams.NullCheck("queryParams");

            ObjectContextCache.Instance.ContainsKey(CacheKey).TrueThrow("SOARoleContext已经在使用中，不能嵌套使用");

            SOARoleContext context = new SOARoleContext();

            context._Process = process;
            context.QueryParams.CopyFrom(queryParams);

            ObjectContextCache.Instance.Add(CacheKey, context);

            return context;
        }

        public static SOARoleContext Current
        {
            get
            {
                return GetCurrentContext();
            }
            set
            {
                if (value == null)
                    ObjectContextCache.Instance.Remove(CacheKey);
                else
                    ObjectContextCache.Instance[CacheKey] = value;
            }
        }

        public static IWfProcess CurrentProcess
        {
            get
            {
                IWfProcess process = null;

                if (SOARoleContext.Current != null)
                    process = SOARoleContext.Current.Process;

                return process;
            }
        }

        /// <summary>
        /// 使用当前角色或流程构造上下文，执行相应的操作。完成后，还原上下文。
        /// 如果没有当前上下文，则创建一个新的上下文。否则沿用旧的上下文。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="process"></param>
        /// <param name="action"></param>
        public static void DoAction(IRole role, IWfProcess process, Action<SOARoleContext> action)
        {
            if (action != null)
            {
                SOARoleContext originalRoleContext = SOARoleContext.Current;

                try
                {
                    if (originalRoleContext == null)
                        SOARoleContext.CreateContext(role, process);

                    action(SOARoleContext.Current);
                }
                finally
                {
                    SOARoleContext.Current = originalRoleContext;
                }
            }
        }

        /// <summary>
        /// 创建一个新的上下文，并且使用它进行计算。计算完成后恢复上下文。
        /// </summary>
        /// <param name="role"></param>
        /// <param name="process"></param>
        /// <param name="action"></param>
        public static void DoNewContextAction(IRole role, IWfProcess process, Action<SOARoleContext> action)
        {
            if (action != null)
            {
                SOARoleContext originalRoleContext = SOARoleContext.Current;

                try
                {
                    SOARoleContext.Current = null;
                    SOARoleContext.CreateContext(role, process);

                    action(SOARoleContext.Current);
                }
                finally
                {
                    SOARoleContext.Current = originalRoleContext;
                }
            }
        }

        /// <summary>
        /// 使用当前角色或流程构造上下文，执行相应的操作。完成后，还原上下文。
        /// 如果没有当前上下文，则创建一个新的上下文。否则沿用旧的上下文。
        /// </summary>
        /// <param name="propertyDefines"></param>
        /// <param name="process"></param>
        /// <param name="action"></param>
        public static void DoAction(SOARolePropertyDefinitionCollection propertyDefines, IWfProcess process, Action<SOARoleContext> action)
        {
            if (action != null)
            {
                SOARoleContext originalRoleContext = SOARoleContext.Current;

                try
                {
                    if (originalRoleContext == null)
                        SOARoleContext.CreateContext(propertyDefines, process);

                    action(SOARoleContext.Current);
                }
                finally
                {
                    SOARoleContext.Current = originalRoleContext;
                }
            }
        }

        /// <summary>
        /// 创建一个新的上下文，并且使用它进行计算。计算完成后恢复上下文。
        /// </summary>
        /// <param name="propertyDefines"></param>
        /// <param name="process"></param>
        /// <param name="action"></param>
        public static void DoNewContextAction(SOARolePropertyDefinitionCollection propertyDefines, IWfProcess process, Action<SOARoleContext> action)
        {
            if (action != null)
            {
                SOARoleContext originalRoleContext = SOARoleContext.Current;

                try
                {
                    SOARoleContext.Current = null;
                    SOARoleContext.CreateContext(propertyDefines, process);

                    action(SOARoleContext.Current);
                }
                finally
                {
                    SOARoleContext.Current = originalRoleContext;
                }
            }
        }

        private static SOARoleContext GetCurrentContext()
        {
            object context = null;

            ObjectContextCache.Instance.TryGetValue(CacheKey, out context);

            return (SOARoleContext)context;
        }

        public void Dispose()
        {
            object context = null;

            if (ObjectContextCache.Instance.TryGetValue(CacheKey, out context))
            {
                ObjectContextCache.Instance.Remove(CacheKey);
            }
        }
    }
}
