using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 带读取和更新功能Adapter的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TCollection"></typeparam>
    public abstract class UpdatableAndLoadableAdapterBase<T, TCollection> : UpdatableAdapterBase<T>
        where TCollection : EditableDataObjectCollectionBase<T>, new()
    {
        private static readonly Dictionary<string, object> _FixedContext = new Dictionary<string, object>();

        public virtual T CreateNewData()
        {
            return (T)TypeCreator.CreateInstance(typeof(T));
        }

        public virtual T CreateNewData(DataRow row)
        {
            return CreateNewData();
        }

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="whereAction"></param>
        /// <returns></returns>
        public virtual bool Exists(Action<WhereSqlClauseBuilder> whereAction)
        {
            return this.Exists(whereAction, this.GetQueryMappingInfo());
        }

        public bool Exists(Action<WhereSqlClauseBuilder> whereAction, ORMappingItemCollection mappings)
        {
            whereAction.NullCheck("whereAction");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            whereAction(builder);
            builder.AppendTenantCode(typeof(T));

            string sql = string.Format("SELECT TOP 1 * FROM {0}", mappings.TableName);

            if (builder.Count > 0)
                sql = sql + string.Format(" WHERE {0}", builder.ToSqlString(TSqlBuilder.Instance));

            return (int)DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0].Rows.Count > 0;
        }

        /// <summary>
        /// 按照In条件加载对象
        /// </summary>
        /// <param name="inAction"></param>
        /// <returns></returns>
        public virtual TCollection LoadByInBuilder(Action<InSqlClauseBuilder> inAction)
        {
            return LoadByInBuilder(inAction, null);
        }

        /// <summary>
        /// 按照In条件加载对象
        /// </summary>
        /// <param name="inAction"></param>
        /// <param name="orderByAction"></param>
        /// <returns></returns>
        public virtual TCollection LoadByInBuilder(Action<InSqlClauseBuilder> inAction, Action<OrderBySqlClauseBuilder> orderByAction)
        {
            inAction.NullCheck("whereAction");

            return this.LoadByInBuilder(inAction, orderByAction, this.GetQueryMappingInfo());
        }

        public TCollection LoadByInBuilder(Action<InSqlClauseBuilder> inAction, Action<OrderBySqlClauseBuilder> orderByAction, ORMappingItemCollection mappings)
        {
            inAction.NullCheck("whereAction");

            TCollection result = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("LoadByInBuilder({0})", this.GetType().FullName), () =>
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder();

                inAction(inBuilder);

                string condition = string.Empty;

                if (inBuilder.IsEmpty == false)
                {
                    ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And,
                        inBuilder, new WhereSqlClauseBuilder().AppendTenantCode(typeof(T)));

                    condition = builder.ToSqlString(TSqlBuilder.Instance);

                    OrderBySqlClauseBuilder orderByBuilder = null;

                    if (orderByAction != null)
                    {
                        orderByBuilder = new OrderBySqlClauseBuilder();

                        orderByAction(orderByBuilder);
                    }

                    result = InnerLoadByBuilder(condition, orderByBuilder, mappings);
                }
                else
                    result = new TCollection();
            });

            return result;
        }

        /// <summary>
        /// 根据外界的builder加载数据
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual TCollection LoadByBuilder(IConnectiveSqlClause builder)
        {
            return LoadByBuilder(builder, null);
        }

        /// <summary>
        /// 根据外界的builder加载数据
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="orderByBuilder"></param>
        /// <returns></returns>
        public virtual TCollection LoadByBuilder(IConnectiveSqlClause builder, OrderBySqlClauseBuilder orderByBuilder)
        {
            builder.NullCheck("builder");

            TCollection result = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("LoadByBuilder({0})", this.GetType().FullName), () =>
            {
                result = InnerLoadByBuilder(builder.ToSqlString(TSqlBuilder.Instance),
                    orderByBuilder,
                    this.GetQueryMappingInfo());
            });

            return result;
        }

        /// <summary>
        /// 按照条件加载对象
        /// </summary>
        /// <param name="whereAction">条件</param>
        /// <returns>对象集合</returns>
        public virtual TCollection Load(Action<WhereSqlClauseBuilder> whereAction)
        {
            return Load(whereAction, null);
        }

        /// <summary>
        /// 按照条件加载对象
        /// </summary>
        /// <param name="whereAction">筛选条件</param>
        /// <param name="orderByAction">排序条件</param>
        /// <returns>对象集合</returns>
        public TCollection Load(Action<WhereSqlClauseBuilder> whereAction, Action<OrderBySqlClauseBuilder> orderByAction)
        {
            return Load(whereAction, orderByAction, this.GetQueryMappingInfo());
        }

        /// <summary>
        /// 按照条件加载对象
        /// </summary>
        /// <param name="whereAction">筛选条件</param>
        /// <param name="orderByAction">排序条件</param>
        /// <param name="mappings"></param>
        /// <returns>对象集合</returns>
        public TCollection Load(Action<WhereSqlClauseBuilder> whereAction, Action<OrderBySqlClauseBuilder> orderByAction, ORMappingItemCollection mappings)
        {
            TCollection result = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("Load({0})", this.GetType().FullName), () =>
            {
                whereAction.NullCheck("whereAction");

                WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

                whereAction(builder);

                builder.AppendTenantCode(typeof(T));

                OrderBySqlClauseBuilder orderByBuilder = null;

                if (orderByAction != null)
                {
                    orderByBuilder = new OrderBySqlClauseBuilder();

                    orderByAction(orderByBuilder);
                }

                result = InnerLoadByBuilder(builder.ToSqlString(TSqlBuilder.Instance), orderByBuilder, mappings);
            });

            return result;
        }

        protected TCollection InnerLoadByBuilder(string condition, OrderBySqlClauseBuilder orderByBuilder, ORMappingItemCollection mappings)
        {
            string sql = string.Format("SELECT * FROM {0}", mappings.TableName);

            if (condition.IsNotEmpty())
                sql = sql + string.Format(" WHERE {0}", condition);

            if (orderByBuilder != null)
                sql = sql + string.Format(" ORDER BY {0}", orderByBuilder.ToSqlString(TSqlBuilder.Instance));

            TCollection result = QueryData(sql);

            AfterLoad(result);

            return result;
        }

        /// <summary>
        /// 加载数据之后
        /// </summary>
        /// <param name="data"></param>
        protected virtual void AfterLoad(TCollection data)
        {
        }

        protected TDataCollection QueryData<TData, TDataCollection>(ORMappingItemCollection mapping, string sql)
            where TData : new()
            where TDataCollection : EditableDataObjectCollectionBase<TData>, new()
        {
            TDataCollection result = new TDataCollection();

            DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            foreach (DataRow row in table.Rows)
            {
                TData data = new TData();

                ORMapping.DataRowToObject(row, mapping, data);

                result.Add(data);
            }

            return result;
        }

        protected TCollection QueryData(string sql)
        {
            ORMappingItemCollection mapping = this.GetQueryMappingInfo();

            return QueryData(mapping, sql);
        }

        protected TCollection QueryData(ORMappingItemCollection mapping, string sql)
        {
            TCollection result = new TCollection();

            DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            foreach (DataRow row in table.Rows)
            {
                T data = CreateNewData(row);

                ORMapping.DataRowToObject(row, mapping, data);

                if (data is ILoadableDataEntity)
                    ((ILoadableDataEntity)data).Loaded = true;

                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// 得到查询数据时的ORMapping信息
        /// </summary>
        /// <returns></returns>
        protected virtual ORMappingItemCollection GetQueryMappingInfo()
        {
            return base.GetMappingInfo(_FixedContext);
        }
    }
}
