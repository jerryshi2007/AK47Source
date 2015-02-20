using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.Utility;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Builders;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixStorageManager : IWfMatrixStorageManager
    {
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="process">流程信息</param>
        public void Save(IWfMatrixProcess process)
        {
            WfClientProcessDescriptor clientDescriptor = WfMatrixDescriptorTransformation.Instance.Transform(process);

            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = process.TenantCode;
            WfClientProcessDescriptorServiceProxy.Instance.SaveDescriptor(clientDescriptor);
        }

        /// <summary>
        /// 根据流程Key加载流程
        /// </summary>
        /// <param name="key">流程Key</param>
        /// <returns>流程对象</returns>
        public IWfMatrixProcess Load(string key)
        {
            var clientProcessDescriptor = WfClientProcessDescriptorServiceProxy.Instance.LoadDescriptor(key);

            IWfMatrixProcess dynamicProcessParams = WfMatrixDescriptorTransformation.Instance.TransformBack(clientProcessDescriptor);

            dynamicProcessParams.InitGlobalParameter();

            return dynamicProcessParams;
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="key">流程Key</param>
        public void Delete(string key)
        {
            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = GetTenantCode();
            WfClientProcessDescriptorServiceProxy.Instance.DeleteDescriptor(key);
        }

        /// <summary>
        /// 创建两个节点的流程
        /// </summary>
        /// <param name="key">流程KEY</param>
        /// <param name="name">流程名称</param>
        /// <param name="process">创建的流程</param>
        /// <param name="msg">报错提醒信息</param>
        /// <returns>执行成功</returns>
        public bool Build(string key, string name, out IWfMatrixProcess process, out string msg)
        {
            bool isok = true;
            process = null;
            msg = string.Empty;

            if (ProcessKeyIsNull(key, out msg))
            {
                return false;
            }

            process = WfMatrixEmptyProcessBuilder.Instance.BuildProcess(key);
            process.Name = name;

            try
            {
                //check数据 key   
                bool haveKey = WfClientProcessDescriptorServiceProxy.Instance.ExsitsProcessKey(key);
                if (haveKey)
                {
                    msg = CIIC.HSR.TSP.Resource.Common.Message.EmptyProcessKeyIsNotAllowed;
                    return false;
                }

                Save(process);
            }
            catch (Exception ex)
            {
                Logging.Loggers.Default.Error("WfMatrixEmptyProcessBuilder.Instance.BuildProcess", ex);
                msg = ex.Message;
                isok = false;
            }

            return isok;
        }

        public PagedCollection<WfClientProcessDescriptorInfo> QueryProcessDescriptorListPaged(QueryModel queryModel, int pageIndex, int pageSize, int? totalCount = default(int?))
        {
            queryModel.NullCheck("queryModel");

            int startIndex = Math.Max((pageIndex - 1) * pageSize, 0);

            string where = GetWhereClause<WfClientProcessDescriptorInfo>(queryModel);
            string orderby = GetOrderBy<WfClientProcessDescriptorInfo>(queryModel);

            int total = totalCount.HasValue ? totalCount.Value : -1;

            WfClientServiceBrokerContext.Current.Context[Consts.TenantCode] = GetTenantCode();
            WfClientProcessDescriptorInfoPageQueryResult qResult =
                WfClientProcessDescriptorServiceProxy.Instance.QueryProcessDescriptorInfo(startIndex, pageSize, where, orderby, total);

            PagedCollection<WfClientProcessDescriptorInfo> result =
                new PagedCollection<WfClientProcessDescriptorInfo>() { Items = qResult.QueryResult.ToList(), TotalItems = qResult.TotalCount };

            return result;
        }

        #region 私有方法

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private string GetWhereClause<T>(QueryModel queryModel)
        {
            IConnectiveSqlClause container = InnerGetWhereSqlClauseBuilder<T>(queryModel.Criteria);

            return container.ToSqlString(TSqlBuilder.Instance);

            //StringBuilder strB = new StringBuilder();
            //WhereSqlClauseBuilder builder = GetWhereSqlClauseBuilder<T>(queryModel.Criteria, strB);

            //return strB.ToString();
        }

        //private WhereSqlClauseBuilder GetWhereSqlClauseBuilder<T>(LogicalQueryCriteriaGroup group, StringBuilder strB)
        //{
        //    LogicOperatorDefine lop = ToLogicOperatorDefine(group.LogicalComputeOperator);
        //    WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder(lop);

        //    ConnectiveSqlClauseCollection collection = new ConnectiveSqlClauseCollection(lop);

        //    foreach (QueryCriteriaBase criteria in group.Criterias)
        //    {
        //        if (criteria is ValueCompareQueryCriteria)
        //        {
        //            ValueCompareQueryCriteria item = (ValueCompareQueryCriteria)criteria;
        //            string op = GetOp(item.CompareOperator);
        //            string name = GetPathField<T>(item.Path);

        //            builder.AppendItem(name, item.CompareValue, op);
        //        }
        //        else if (criteria is LogicalQueryCriteriaGroup)
        //        {
        //            WhereSqlClauseBuilder subBuilder = GetWhereSqlClauseBuilder<T>((LogicalQueryCriteriaGroup)criteria, strB);
        //            collection.Add(subBuilder);
        //        }
        //    }

        //    if (collection.IsEmpty)
        //    {
        //        strB.Append(builder.ToSqlString(TSqlBuilder.Instance));
        //    }
        //    else
        //    {
        //        collection.Add(builder);
        //        strB.Append(collection.ToSqlString(TSqlBuilder.Instance));
        //    }

        //    return builder;
        //}

        private IConnectiveSqlClause InnerGetWhereSqlClauseBuilder<T>(LogicalQueryCriteriaGroup group)
        {
            LogicOperatorDefine lop = ToLogicOperatorDefine(group.LogicalComputeOperator);
            ConnectiveSqlClauseCollection container = new ConnectiveSqlClauseCollection(lop);

            foreach (QueryCriteriaBase criteria in group.Criterias)
            {
                if (criteria is ValueCompareQueryCriteria)
                {
                    ValueCompareQueryCriteria item = (ValueCompareQueryCriteria)criteria;
                    WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

                    builder.AppendItem(GetPathField<T>(item.Path), item.CompareValue, GetOp(item.CompareOperator));

                    container.Add(builder);
                }
                else if (criteria is LogicalQueryCriteriaGroup)
                {
                    container.Add(InnerGetWhereSqlClauseBuilder<T>((LogicalQueryCriteriaGroup)criteria));
                }
            }

            return container;
        }

        /// <summary>
        /// 连接符号
        /// </summary>
        /// <param name="lop"></param>
        /// <returns></returns>
        private static LogicOperatorDefine ToLogicOperatorDefine(LogicalComputeOperator lop)
        {
            LogicOperatorDefine result = LogicOperatorDefine.And;

            if (lop == LogicalComputeOperator.Or)
                result = LogicOperatorDefine.Or;

            return result;
        }

        private static string GetPathField<T>(string path)
        {
            string result = path;

            ConditionMappingItem item = ConditionMapping.GetMappingInfo(typeof(T)).FirstOrDefault(p => p.PropertyName == path);

            if (item != null)
                result = item.DataFieldName;

            return result;
        }

        /// <summary>
        /// 操作符号
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private static string GetOp(CompareQueryOperator op)
        {
            return CompareQueryOperatorHelper.ToSqlOperator(op);
        }


        /// <summary>
        /// 排序字段
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static string GetOrderBy<T>(QueryModel queryModel)
        {
            OrderBySqlClauseBuilder orderByBuilder = new OrderBySqlClauseBuilder();

            foreach (SortDirection sd in queryModel.ToOrderBy())
            {
                FieldSortDirection direction = FieldSortDirection.Ascending;

                if (sd.Direction == Direction.Descending)
                    direction = FieldSortDirection.Descending;

                orderByBuilder.AppendItem(GetPathField<T>(sd.Member), direction);
            }

            return orderByBuilder.ToSqlString(TSqlBuilder.Instance);
            //StringBuilder sbSort = new StringBuilder();
            //List<SortDirection> orderBy = queryModel.ToOrderBy();

            //if (orderBy != null)
            //{
            //    int i = 0;
            //    foreach (var item in orderBy)
            //    {
            //        if (i != 0)
            //            sbSort.Append(", ");

            //        string name = GetPathField<T>(item.Member);
            //        sbSort.Append(SecurityHelper.FilterSqlInjectChart(name));

            //        if (item.Direction == Direction.Descending)
            //        {
            //            sbSort.Append(" ").Append("DESC");
            //        }

            //        i++;
            //    }
            //}

            //return sbSort.ToString();
        }

        private bool ProcessKeyIsNull(string key, out string msg)
        {
            bool isok = false;
            msg = string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                isok = true;
                msg = CIIC.HSR.TSP.Resource.Common.Message.EmptyProcessKeyIsNotAllowed;
            }
            return isok;
        }

        private string GetTenantCode()
        {
            if (Context != null)
            {
                return Context.TenantCode;
            }

            return string.Empty;
        }
        #endregion

        /// <summary>
        /// 运行时上下文
        /// </summary>
        public ServiceContext Context
        {
            set;
            get;
        }
    }
}
