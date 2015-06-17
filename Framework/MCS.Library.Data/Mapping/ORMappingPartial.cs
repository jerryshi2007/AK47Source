using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// ORMapping的partial class，主要封装了公有方法
    /// </summary>
    public static partial class ORMapping
    {
        #region Mapping Info
        /// <summary>
        /// 获取对象和数据字段之间的映射关系
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <returns>映射关系集合</returns>
        /// <remarks>获取对象和数据字段之间的映射关系
        /// <see cref="MCS.Library.Data.Mapping.ORMappingItemCollection"/>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetMappingInfo" lang="cs" title="获取对象和数据字段之间的映射关系"/>
        /// </remarks>
        public static ORMappingItemCollection GetMappingInfo<T>()
        {
            return InnerGetMappingInfo(typeof(T));
        }

        /// <summary>
        ///  获取对象和数据字段之间的映射关系
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <returns>获取对象和数据字段之间的映射关系</returns>
        public static ORMappingItemCollection GetMappingInfo(Type type)
        {
            return InnerGetMappingInfo(type);
        }

        /// <summary>
        /// 根据映射关系，得到Select语句中返回的字段名称数组
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static string[] GetSelectFieldsName(ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            mapping.NullCheck("mapping");
            ignoreProperties.NullCheck("ignoreProperties");

            List<string> result = new List<string>();

            foreach (ORMappingItem item in mapping)
            {
                if ((item.BindingFlags & ClauseBindingFlags.Select) != ClauseBindingFlags.None)
                {
                    if (Array.Exists<string>(ignoreProperties, target => (string.Compare(target, item.PropertyName, true) == 0)
                                            ) == false)
                        result.Add(item.DataFieldName);
                }
            }

            return result.ToArray();

        }

        /// <summary>
        /// 根据映射关系，得到Select语句中返回的字段名称的SQL语句部分
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static string GetSelectFieldsNameSql(ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            string[] fields = GetSelectFieldsName(mapping, ignoreProperties);

            return string.Join(", ", fields);
        }

        /// <summary>
        /// 根据类型，得到Select语句中返回的字段名称的SQL语句部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static string GetSelectFieldsNameSql<T>(params string[] ignoreProperties)
        {
            string[] fields = GetSelectFieldsName<T>(ignoreProperties);

            return string.Join(", ", fields);
        }

        /// <summary>
        /// 根据数据类型，得到Select语句中返回的字段名称数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static string[] GetSelectFieldsName<T>(params string[] ignoreProperties)
        {
            ignoreProperties.NullCheck("ignoreProperties");

            ORMappingItemCollection mapping = GetMappingInfo<T>();

            return GetSelectFieldsName(mapping, ignoreProperties);
        }

        #endregion Mapping Info

        #region Object To Sql
        /// <summary>
        /// 根据对象拼Insert语句
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="builder">生成Sql语句类型的Builder如TSqlBuilder或PlSqlBuilder</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>根据传入的对象和对象映射时需要忽略的字段以及类定义上的表名，生成完整的Insert语句</returns>
        public static string GetInsertSql<T>(T graph, ISqlBuilder builder, params string[] ignoreProperties)
        {
            ORMappingItemCollection mapping = InnerGetMappingInfoByObject(graph);

            return GetInsertSql<T>(graph, mapping, builder, ignoreProperties);
        }

        /// <summary>
        /// 根据对象拼Insert语句
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="mapping">映射关系</param>
        /// <param name="builder">生成Sql语句类型的Builder如TSqlBuilder或PlSqlBuilder</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>根据传入的对象和对象映射时需要忽略的字段以及类定义上的表名，生成完整的Insert语句</returns>
        public static string GetInsertSql<T>(T graph, ORMappingItemCollection mapping, ISqlBuilder builder, params string[] ignoreProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");
            ExceptionHelper.FalseThrow<ArgumentNullException>(builder != null, "builder");

            InsertSqlClauseBuilder insertBuilder = GetInsertSqlClauseBuilder(graph, mapping, ignoreProperties);

            return string.Format("INSERT INTO {0} {1}", mapping.TableName, insertBuilder.ToSqlString(builder));
        }

        /// <summary>
        /// 根据对象拼Insert语句时的方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>InsertSqlClauseBuilder对象，供拼Insert语句使用</returns>
        /// <remarks>
        /// 根据传入的对象和对象映射时需要忽略的字段，返回InsertSqlClauseBuilder对象，以供后续拼Insert语句的字段名称和Values部分
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetInsertSqlClauseBuilder" lang="cs" title="拼Insert语句"/>
        /// <see cref="MCS.Library.Data.Builder.InsertSqlClauseBuilder"/>
        /// </remarks>
        public static InsertSqlClauseBuilder GetInsertSqlClauseBuilder<T>(T graph, params string[] ignoreProperties)
        {
            return GetInsertSqlClauseBuilder<T>(graph, InnerGetMappingInfoByObject(graph), ignoreProperties);
        }

        /// <summary>
        /// 根据对象拼Insert语句时的方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="mapping">映射关系</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>InsertSqlClauseBuilder对象，供拼Insert语句使用</returns>
        /// <remarks>
        /// 根据传入的对象和对象映射时需要忽略的字段，返回InsertSqlClauseBuilder对象，以供后续拼Insert语句的字段名称和Values部分
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetInsertSqlClauseBuilder" lang="cs" title="拼Insert语句"/>
        /// <see cref="MCS.Library.Data.Builder.InsertSqlClauseBuilder"/>
        /// </remarks>
        public static InsertSqlClauseBuilder GetInsertSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

            InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

            FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Insert,
                new DoSqlClauseBuilder<T>(DoInsertUpdateSqlClauseBuilder<T>), ignoreProperties);

            builder.AppendTenantCode(typeof(T));

            return builder;
        }

        /// <summary>
        /// 根据对象拼Update语句
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="builder">生成Sql语句类型的Builder如TSqlBuilder或PlSqlBuilder</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>根据传入的对象和对象映射时需要忽略的字段以及类定义上的表名，生成完整的Update语句</returns>
        public static string GetUpdateSql<T>(T graph, ISqlBuilder builder, params string[] ignoreProperties)
        {
            ORMappingItemCollection mapping = InnerGetMappingInfoByObject(graph);

            return GetUpdateSql<T>(graph, mapping, builder, ignoreProperties);
        }

        /// <summary>
        /// 根据对象拼Insert语句
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="mapping">映射关系</param>
        /// <param name="builder">生成Sql语句类型的Builder如TSqlBuilder或PlSqlBuilder</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>根据传入的对象和对象映射时需要忽略的字段以及类定义上的表名，生成完整的Insert语句</returns>
        public static string GetUpdateSql<T>(T graph, ORMappingItemCollection mapping, ISqlBuilder builder, params string[] ignoreProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");
            ExceptionHelper.FalseThrow<ArgumentNullException>(builder != null, "builder");

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                UpdateSqlClauseBuilder updateBuilder = GetUpdateSqlClauseBuilder(graph, mapping, ignoreProperties);
                WhereSqlClauseBuilder whereBuilder = GetWhereSqlClauseBuilderByPrimaryKey(graph, mapping);

                return string.Format("UPDATE {0} SET {1} WHERE {2}",
                    mapping.TableName,
                    updateBuilder.ToSqlString(builder),
                    whereBuilder.ToSqlString(builder));
            }
        }

        /// <summary>
        /// 根据对象拼Update语句时的方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>UpdateSqlClauseBuilder对象，供拼Update语句使用</returns>
        /// <remarks>
        /// 根据传入的对象和对象映射时需要忽略的字段，返回UpdateSqlClauseBuilder对象，以供后续拼Update语句的字段名称和Values部分
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetUpdateSqlClauseBuilder" lang="cs" title="拼Update语句"/>
        /// <see cref="MCS.Library.Data.Builder.UpdateSqlClauseBuilder"/>
        /// </remarks>
        public static UpdateSqlClauseBuilder GetUpdateSqlClauseBuilder<T>(T graph, params string[] ignoreProperties)
        {
            return GetUpdateSqlClauseBuilder<T>(graph, InnerGetMappingInfoByObject(graph), ignoreProperties);
        }

        /// <summary>
        /// 根据对象拼Update语句时的方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="mapping">映射关系</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>UpdateSqlClauseBuilder对象，供拼Update语句使用</returns>
        /// <remarks>
        /// 根据传入的对象和对象映射时需要忽略的字段，返回UpdateSqlClauseBuilder对象，以供后续拼Update语句的字段名称和Values部分
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetUpdateSqlClauseBuilder" lang="cs" title="拼Update语句"/>
        /// <see cref="MCS.Library.Data.Builder.UpdateSqlClauseBuilder"/>
        /// </remarks>
        public static UpdateSqlClauseBuilder GetUpdateSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

            UpdateSqlClauseBuilder builder = new UpdateSqlClauseBuilder();

            FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Update,
                new DoSqlClauseBuilder<T>(DoInsertUpdateSqlClauseBuilder<T>), ignoreProperties);

            //builder.AppendTenantCode(typeof(T));

            return builder;
        }

        /// <summary>
        /// 根据对象拼Where子句的方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>WhereSqlClauseBuilder，供拼Where子句使用</returns>
        /// <remarks>
        /// 根据传入的对象和对象映射时需要忽略的字段，返回WhereSqlClauseBuilder对象，以供后续拼Where子句使用
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetWhereSqlClauseBuilder" lang="cs" title="根据传入的对象拼Where子句"/>
        /// <see cref="MCS.Library.Data.Builder.WhereSqlClauseBuilder"/>
        /// </remarks>
        public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder<T>(T graph, params string[] ignoreProperties)
        {
            return GetWhereSqlClauseBuilder<T>(graph, InnerGetMappingInfoByObject(graph), ignoreProperties);
        }

        /// <summary>
        /// 根据对象拼Where子句的方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="mapping">映射关系</param>
        /// <param name="ignoreProperties">忽略的字段</param>
        /// <returns>WhereSqlClauseBuilder，供拼Where子句使用</returns>
        /// <remarks>
        /// 根据传入的对象和对象映射时需要忽略的字段，返回WhereSqlClauseBuilder对象，以供后续拼Where子句使用
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetWhereSqlClauseBuilder" lang="cs" title="根据传入的对象拼Where子句"/>
        /// <see cref="MCS.Library.Data.Builder.WhereSqlClauseBuilder"/>
        /// </remarks>
        public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Where,
                new DoSqlClauseBuilder<T>(DoWhereSqlClauseBuilder<T>), ignoreProperties);

            return builder;
        }

        /// <summary>
        /// 根据对象、主键、忽略的属性，生成WhereSqlClauseBuilder对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="ignoreProperties">忽略的字段属性</param>
        /// <returns>WhereSqlClauseBuilder，供拼Where子句使用</returns>
        /// <remarks>
        /// 根据传入的对象、主键、忽略的属性，返回WhereSqlClauseBuilder对象，以供后续拼Where子句使用
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetWhereSqlClauseBuilderByPrimaryKey" lang="cs" title="根据主键生成WhereSqlClauseBuilder对象"/>
        /// <see cref="MCS.Library.Data.Builder.WhereSqlClauseBuilder"/>
        /// </remarks>
        public static WhereSqlClauseBuilder GetWhereSqlClauseBuilderByPrimaryKey<T>(T graph, params string[] ignoreProperties)
        {
            return GetWhereSqlClauseBuilderByPrimaryKey(graph, InnerGetMappingInfoByObject(graph), ignoreProperties);
        }

        /// <summary>
        /// 根据对象、主键、忽略的属性，生成WhereSqlClauseBuilder对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="graph">对象</param>
        /// <param name="mapping">映射关系</param>
        /// <param name="ignoreProperties">忽略的字段属性</param>
        /// <returns>WhereSqlClauseBuilder，供拼Where子句使用</returns>
        /// <remarks>
        /// 根据传入的对象、主键、忽略的属性，返回WhereSqlClauseBuilder对象，以供后续拼Where子句使用
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="GetWhereSqlClauseBuilderByPrimaryKey" lang="cs" title="根据主键生成WhereSqlClauseBuilder对象"/>
        /// <see cref="MCS.Library.Data.Builder.WhereSqlClauseBuilder"/>
        /// </remarks>
        public static WhereSqlClauseBuilder GetWhereSqlClauseBuilderByPrimaryKey<T>(T graph, ORMappingItemCollection mapping, params string[] ignoreProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Where,
                new DoSqlClauseBuilder<T>(DoWhereSqlClauseBuilderByPrimaryKey<T>), ignoreProperties);

            return builder;
        }

        /// <summary>
        /// 从Tenant上下文中获取TenantCode并且添加到Builder中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="tenantCodeFieldName"></param>
        /// <returns></returns>
        public static T AppendTenantCode<T>(this T builder, string tenantCodeFieldName = "TENANT_CODE") where T : SqlClauseBuilderIUW
        {
            if (builder != null)
            {
                if (TenantContext.Current.Enabled)
                {
                    if (builder.ContainsDataField(tenantCodeFieldName) == false)
                        builder.AppendItem(tenantCodeFieldName, TenantContext.Current.TenantCode);
                }
            }

            return builder;
        }

        /// <summary>
        /// 根据类型上的TenantRelativeObjectAttribute以及TenantContext.Current.Enabled决定是否在builder上添加租户编码字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T AppendTenantCode<T>(this T builder, Type type) where T : SqlClauseBuilderIUW
        {
            if (builder != null)
            {
                TenantRelativeObjectAttribute tenantAttr = AttributeHelper.GetCustomAttribute<TenantRelativeObjectAttribute>(type);

                if (tenantAttr != null)
                    builder.AppendTenantCode(tenantAttr.TenantCodeFieldName);
            }

            return builder;
        }

        /// <summary>
        /// 从Tenant上下文中获取TenantCode并且在某个条件子句添加租户编码的过滤字段
        /// </summary>
        /// <param name="connective"></param>
        /// <param name="tenantCodeFieldName"></param>
        /// <returns></returns>
        public static ConnectiveSqlClauseCollection AppendTenantCodeSqlClause(this IConnectiveSqlClause connective, string tenantCodeFieldName = "TENANT_CODE")
        {
            ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);

            if (connective != null)
            {
                WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

                wBuilder.AppendTenantCode(tenantCodeFieldName);

                result.Add(connective);
                result.Add(wBuilder);
            }

            return result;
        }

        /// <summary>
        /// 根据类型上的TenantRelativeObjectAttribute以及TenantContext.Current.Enabled决定是否在某个条件子句添加租户编码的过滤字段
        /// </summary>
        /// <param name="connective"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConnectiveSqlClauseCollection AppendTenantCodeSqlClause(this IConnectiveSqlClause connective, Type type)
        {
            ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);

            if (connective != null)
            {
                WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

                wBuilder.AppendTenantCode(type);

                result.Add(connective);
                result.Add(wBuilder);
            }

            return result;
        }
        #endregion Object To Sql

        #region DataRow or DataRead to Object
        /// <summary>
        /// 将DataRow的值写入到对象中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="graph">对象</param>
        /// <remarks>
        /// 将传入的DataRow中的数值写入到对象graph中
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="DataRowToObject" lang="cs" title="将DataRow的值写入到对象中"/>
        /// </remarks>
        public static void DataRowToObject<T>(DataRow row, T graph)
        {
            DataRowToObject(row, InnerGetMappingInfoByObject(graph), graph);
        }

        /// <summary>
        /// DataView的数据转换到集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="view"></param>
        public static void DataViewToCollection<T>(EditableDataObjectCollectionBase<T> collection, DataView view) where T : new()
        {
            DataViewToCollection(collection, view, null);
        }

        /// <summary>
        /// DataView的数据转换到集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <param name="view"></param>
        /// <param name="dod"></param>
        public static void DataViewToCollection<T>(EditableDataObjectCollectionBase<T> collection, ORMappingItemCollection items, DataView view, DataToObjectDeligations dod) where T : new()
        {
            collection.NullCheck("collection");
            items.NullCheck("items");
            view.NullCheck("view");

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                foreach (DataRowView drv in view)
                {
                    T graph = new T();

                    DataRowToObject(drv.Row, items, graph, dod);

                    collection.Add(graph);
                }
            }
        }

        /// <summary>
        /// DataView的数据转换到集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <param name="view"></param>
        public static void DataViewToCollection<T>(EditableDataObjectCollectionBase<T> collection, ORMappingItemCollection items, DataView view) where T : new()
        {
            DataViewToCollection(collection, items, view, null);
        }

        /// <summary>
        /// DataView的数据转换到集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="view"></param>
        /// <param name="dod"></param>
        public static void DataViewToCollection<T>(EditableDataObjectCollectionBase<T> collection, DataView view, DataToObjectDeligations dod) where T : new()
        {
            collection.NullCheck("collection");
            view.NullCheck("view");

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                foreach (DataRowView drv in view)
                {
                    T graph = new T();

                    DataRowToObject(drv.Row, graph, dod);

                    collection.Add(graph);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="graph"></param>
        /// <param name="dod"></param>
        public static void DataRowToObject<T>(DataRow row, T graph, DataToObjectDeligations dod)
        {
            DataRowToObject(row, InnerGetMappingInfoByObject<T>(graph), graph, dod);
        }

        /// <summary>
        /// 将DataRow的值写入到对象中
        /// </summary>
        /// <param name="row">DataRow对象</param>
        /// <param name="items">映射关系</param>
        /// <param name="graph">对象</param>
        public static void DataRowToObject(DataRow row, ORMappingItemCollection items, object graph)
        {
            DataRowToObject(row, items, graph, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">DataRow对象</param>
        /// <param name="items">映射关系</param>
        /// <param name="graph">对象</param>
        /// <param name="dod"></param>
        public static void DataRowToObject(DataRow row, ORMappingItemCollection items, object graph, DataToObjectDeligations dod)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(row != null, "row");
            ExceptionHelper.FalseThrow<ArgumentNullException>(items != null, "items");
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(row.Table != null, "row.Table");

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (items.ContainsKey(column.ColumnName))
                    {
                        ORMappingItem item = items[column.ColumnName];

                        System.Type realType = GetRealType(item.MemberInfo);

                        object data = row[column];

                        if (item.EncryptProperty)
                            data = DecryptPropertyValue(item, data);

                        if (Convertible(realType, data))
                            SetValueToObject(item, graph, ConvertData(item, data), row, dod);
                    }
                }
            }
        }
        /// <summary>
        /// 将DataReader的值写入到对象中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dr">IDataReader对象</param>
        /// <param name="graph">对象</param>
        /// <remarks>
        /// 将传入的DataReader中的数值写入到对象graph中
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Data.SqlBuilder.Test\ORMappingTest.cs" region="DataReaderToObject" lang="cs" title="将DataRow的值写入到对象中"/>
        /// </remarks>
        public static void DataReaderToObject<T>(IDataReader dr, T graph)
        {
            DataReaderToObject(dr, graph, null);
        }

        /// <summary>
        /// 将DataReader的值写入到对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="graph"></param>
        /// <param name="dod"></param>
        public static void DataReaderToObject<T>(IDataReader dr, T graph, DataToObjectDeligations dod)
        {
            DataReaderToObject(dr, InnerGetMappingInfoByObject<T>(graph), graph, dod);
        }

        /// <summary>
        /// 将DataReader的值写入到对象中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dr">IDataReader对象</param>
        /// <param name="items">映射关系</param>
        /// <param name="graph">对象</param>
        public static void DataReaderToObject<T>(IDataReader dr, ORMappingItemCollection items, T graph)
        {
            DataReaderToObject(dr, items, graph, null);
        }

        /// <summary>
        /// 将DataReader的值写入到对象中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dr">IDataReader对象</param>
        /// <param name="items">映射关系</param>
        /// <param name="graph">对象</param>
        /// <param name="dod"></param>
        public static void DataReaderToObject<T>(IDataReader dr, ORMappingItemCollection items, T graph, DataToObjectDeligations dod)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dr != null, "dr");
            ExceptionHelper.FalseThrow<ArgumentNullException>(items != null, "items");
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            DataTable schemaTable = dr.GetSchemaTable();

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    string columnName = row["ColumnName"].ToString();
                    if (items.ContainsKey(columnName))
                    {
                        ORMappingItem item = items[row["ColumnName"].ToString()];
                        System.Type realType = GetRealType(item.MemberInfo);

                        object data = dr[columnName];

                        if (item.EncryptProperty)
                            data = DecryptPropertyValue(item, data);

                        if (Convertible(realType, data))
                            SetValueToObject(item, graph, ConvertData(item, data), dr, dod);
                    }
                }
            }
        }

        /// <summary>
        /// DataReader到Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="dr"></param>
        public static void DataReaderToCollection<T>(EditableDataObjectCollectionBase<T> collection, IDataReader dr) where T : new()
        {
            DataReaderToCollection<T>(collection, dr, (DataToObjectDeligations)null);
        }

        /// <summary>
        /// DataReader到Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="dr"></param>
        /// <param name="dod"></param>
        public static void DataReaderToCollection<T>(EditableDataObjectCollectionBase<T> collection, IDataReader dr, DataToObjectDeligations dod) where T : new()
        {
            dr.NullCheck("dr");

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                while (dr.Read())
                {
                    T graph = new T();

                    DataReaderToObject(dr, graph, dod);

                    collection.Add(graph);
                }
            }
        }

        /// <summary>
        /// DataReader到Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="dr"></param>
        /// <param name="items"></param>
        public static void DataReaderToCollection<T>(EditableDataObjectCollectionBase<T> collection, IDataReader dr, ORMappingItemCollection items) where T : new()
        {
            DataReaderToCollection<T>(collection, dr, items, null);
        }

        /// <summary>
        /// DataReader到Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="dr"></param>
        /// <param name="items"></param>
        /// <param name="dod"></param>
        public static void DataReaderToCollection<T>(EditableDataObjectCollectionBase<T> collection, IDataReader dr, ORMappingItemCollection items, DataToObjectDeligations dod) where T : new()
        {
            dr.NullCheck("dr");
            items.NullCheck("items");
            dod.NullCheck("dod");

            using (ORMappingContext context = ORMappingContext.GetContext())
            {
                while (dr.Read())
                {
                    T graph = new T();

                    DataReaderToObject(dr, items, graph, dod);

                    collection.Add(graph);
                }
            }
        }
        #endregion
    }
}
