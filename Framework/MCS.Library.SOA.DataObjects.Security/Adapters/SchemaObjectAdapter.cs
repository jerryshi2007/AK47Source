using System;
using System.Data;
using System.Linq;
using System.Transactions;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
    /// <summary>
    /// 最基本的对象适配器。从SchemaObjectAdapterBase派生，是权限中心所有对象的读写器。
    /// 基类SchemaObjectAdapterBase中主要实现了更新操作，而SchemaObjectAdapter主要实现了对象的读（Load）操作。
    /// </summary>
    public class SchemaObjectAdapter : SchemaObjectAdapterBase<SchemaObjectBase>
    {
        /// <summary>
        /// <see cref="SchemaObjectAdapter"/>的实例，此字段为只读
        /// </summary>
        public static readonly SchemaObjectAdapter Instance = new SchemaObjectAdapter();

        private SchemaObjectAdapter()
        {
        }

        /// <summary>
        /// 根据ID载入数据
        /// </summary>
        /// <param name="id">对象的ID</param>
        /// <returns><see cref="SchemaObjectBase"/>的派生类型的实例</returns>
        public SchemaObjectBase Load(string id)
        {
            return Load(id, DateTime.MinValue);
        }

        /// <summary>
        /// 根据ID和时间载入对象
        /// </summary>
        /// <param name="id">对象的ID</param>
        /// <param name="timePoint">表示时间点的<see cref="DateTime"/> 或 <see cref="DateTime.MinValue"/>表示当前时间</param>
        /// <returns><see cref="SchemaObjectBase"/>的派生类型的实例</returns>
        public SchemaObjectBase Load(string id, DateTime timePoint)
        {
            id.NullCheck("id");

            SchemaObjectBase result = null;

            if (id == SCOrganization.RootOrganizationID)
            {
                result = SCOrganization.GetRoot();
            }
            else
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");

                inBuilder.AppendItem(id);

                SchemaObjectCollection objs = Load(inBuilder, timePoint);

                result = objs.FirstOrDefault();
            }

            return result;
        }

        /// <summary>
        /// 根据<see cref="IConnectiveSqlClause"/>指定的条件载入对象
        /// </summary>
        /// <param name="condition">表示条件的<see cref="IConnectiveSqlClause"/></param>
        /// <returns>一个<see cref="SchemaObjectCollection"/>，包含条件指定的对象。</returns>
        public SchemaObjectCollection Load(IConnectiveSqlClause condition)
        {
            return Load(condition, DateTime.MinValue);
        }

        /// <summary>
        /// 根据<see cref="IConnectiveSqlClause"/>指定的条件和时间点载入对象
        /// </summary>
        /// <param name="condition">表示条件的<see cref="IConnectiveSqlClause"/></param>
        /// <param name="timePoint">时间点 - 或 - <see cref="DateTime.MinValue"/>表示当前时间</param>
        /// <returns>一个<see cref="SchemaObjectCollection"/>，包含条件指定的对象。</returns>
        public SchemaObjectCollection Load(IConnectiveSqlClause condition, DateTime timePoint)
        {
            var timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

            SchemaObjectCollection result = new SchemaObjectCollection();

            if (condition.IsEmpty == false)
            {
                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(condition, timePointBuilder);

                using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
                {
                    VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, connectiveBuilder, this.GetConnectionName(),
                        view =>
                        {
                            result.LoadFromDataView(view);
                        });
                }
            }

            return result;
        }

        /// <summary>
        /// 根据代码名称，<see cref="IConnectiveSqlClause"/>指定的条件和时间点载入对象
        /// </summary>
        /// <param name="conditionAction">要在构造条件时执行的操作</param>
        /// <param name="timePoint">时间点</param>
        /// <param name="codeNames">代码名称</param>
        /// <returns>一个<see cref="SchemaObjectCollection"/>，包含条件指定的对象。</returns>
        public SchemaObjectCollection LoadByCodeName(Action<ConnectiveSqlClauseCollection> conditionAction, DateTime timePoint, params string[] codeNames)
        {
            var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

            var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("S.CodeName");

            inBuilder.AppendItem(codeNames);

            SchemaObjectCollection result = null;

            if (inBuilder.IsEmpty == false)
            {
                var conditions = new ConnectiveSqlClauseCollection(inBuilder, timeCondition, timeCondition2);

                if (conditionAction != null)
                    conditionAction(conditions);

                result = this.LoadByCodeNameInner(conditions);
            }
            else
                result = new SchemaObjectCollection();

            return result;
        }

        private SchemaObjectCollection LoadByCodeNameInner(ConnectiveSqlClauseCollection conditions)
        {
            string sql = "SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.SchemaObjectSnapshot S ON O.ID = S.ID AND S.VersionStartTime = O.VersionStartTime  WHERE ";
            sql += conditions.ToSqlString(TSqlBuilder.Instance);

            var result = new SchemaObjectCollection();
            var dt = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

            result.LoadFromDataView(dt.DefaultView);

            return result;
        }

        [Obsolete("此方法已经被废弃，请使用带Status的重载方法")]
        public SchemaObjectBase LoadByCodeName(string schemaType, string codeName, DateTime timePoint)
        {
            string.IsNullOrEmpty(schemaType).TrueThrow("schemaType不得为空");
            string.IsNullOrEmpty(codeName).TrueThrow("codeName不得为空");

            return LoadByCodeName(c =>
            {
                WhereSqlClauseBuilder condition = new WhereSqlClauseBuilder();
                condition.AppendItem("O.SchemaType", schemaType);
                c.Add(condition);
            }, timePoint, codeName).FirstOrDefault();
        }

        /// <summary>
        /// 根据codename和status进行查询
        /// </summary>
        /// <param name="schemaType"></param>
        /// <param name="codeName"></param>
        /// <param name="status"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectBase LoadByCodeName(string schemaType, string codeName, SchemaObjectStatus status, DateTime timePoint)
        {
            string.IsNullOrEmpty(schemaType).TrueThrow("schemaType不得为空");
            string.IsNullOrEmpty(codeName).TrueThrow("codeName不得为空");

            return LoadByCodeName(c =>
            {
                WhereSqlClauseBuilder condition = new WhereSqlClauseBuilder();
                condition.AppendItem("O.SchemaType", schemaType);
                condition.AppendItem("O.Status", (int)status);
                c.Add(condition);
            }, timePoint, codeName).FirstOrDefault();
        }

        /// <summary>
        /// 按照SchemaType加载对象
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection LoadBySchemaType(string[] schemaTypes, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");

            InSqlClauseBuilder builder = new InSqlClauseBuilder("SchemaType");

            builder.AppendItem(schemaTypes);

            return Load(builder, timePoint);
        }

        /// <summary>
        /// 根据XPath查询SchemaObject
        /// </summary>
        /// <param name="xPath">符合条件的xPath语句</param>
        /// <param name="schemaTypes">SchemaType。如果为空数组，则查询所有类型的对象</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection LoadByXPath(string xPath, string[] schemaTypes, DateTime timePoint)
        {
            return LoadByXPath(xPath, schemaTypes, true, timePoint);
        }

        /// <summary>
        /// 根据XPath查询SchemaObject
        /// </summary>
        /// <param name="xPath">符合条件的xPath语句</param>
        /// <param name="schemaTypes">SchemaType。如果为空数组，则查询所有类型的对象</param>
        /// <param name="includingDeleted">为true时，包含已删除的对象。</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection LoadByXPath(string xPath, string[] schemaTypes, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");

            SchemaObjectCollection result = new SchemaObjectCollection();

            if (xPath.IsNotEmpty())
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SchemaType");

                inBuilder.AppendItem(schemaTypes);

                WhereSqlClauseBuilder pathBuilder = new WhereSqlClauseBuilder();

                pathBuilder.AppendItem("Data", xPath, string.Empty, "Data.exist(${Data}$) > 0");

                if (includingDeleted == false)
                    pathBuilder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

                ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection(inBuilder, pathBuilder);

                result = Load(builder, timePoint);
            }
            else
                result = new SchemaObjectCollection();

            return result;
        }

        /// <summary>
        /// 从Cache中获取图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectPhoto GetObjectPhoto(string id, string propertyName, DateTime timePoint)
        {
            SchemaObjectPhotoKey cacheKey = new SchemaObjectPhotoKey() { ObjectID = id, PropertyName = propertyName, TimePoint = timePoint };

            return SchemaObjectPhotoCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
                {
                    SchemaObjectPhoto photo = LoadObjectPhoto(id, propertyName, timePoint);

                    MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

                    cache.Add(cacheKey, photo, dependency);

                    return photo;
                });
        }

        /// <summary>
        /// 从数据库得到对象的图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectPhoto LoadObjectPhoto(string id, string propertyName, DateTime timePoint)
        {
            SchemaObjectPhoto result = null;
            SchemaObjectBase obj = SchemaObjectAdapter.Instance.Load(id, timePoint);

            if (obj != null)
            {
                if (obj.Properties.ContainsKey(propertyName))
                {
                    ImageProperty imgInfo = JSONSerializerExecute.Deserialize<ImageProperty>(obj.Properties[propertyName].StringValue);

                    if (imgInfo != null)
                    {
                        MaterialContent mc = MaterialContentAdapter.Instance.Load(builder => builder.AppendItem("CONTENT_ID", imgInfo.ID)).FirstOrDefault();

                        if (mc != null)
                            result = new SchemaObjectPhoto() { ImageInfo = imgInfo, ContentData = mc.ContentData };
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 载入无上级对象的对象
        /// </summary>
        /// <param name="childSchemas">要返回的对象类型</param>
        /// <returns></returns>
        public SchemaObjectCollection LoadObjectsOfParentAbsense(string[] childSchemas, string[] parentSchemas, DateTime timePoint)
        {
            var timeConditionOut = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");

            var timeConditionIn = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");

            WhereSqlClauseBuilder outWhere = new WhereSqlClauseBuilder();
            outWhere.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);

            WhereSqlClauseBuilder innerWhere = new WhereSqlClauseBuilder();
            innerWhere.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);

            InSqlClauseBuilder innerInChild = new InSqlClauseBuilder("R.ChildSchemaType");
            innerInChild.AppendItem(childSchemas);

            InSqlClauseBuilder innerInParent = new InSqlClauseBuilder("R.ParentSchemaType");
            innerInParent.AppendItem(parentSchemas);

            InSqlClauseBuilder outterIn = new InSqlClauseBuilder("O.SchemaType");
            outterIn.AppendItem(childSchemas);

            ConnectiveSqlClauseCollection innerConditions = new ConnectiveSqlClauseCollection(innerWhere, innerInParent, innerInChild, timeConditionIn);

            ConnectiveSqlClauseCollection outterConditions = new ConnectiveSqlClauseCollection(outWhere, outterIn, timeConditionOut);

            string sql = string.Format(@"SELECT O.* FROM SC.SchemaObject O 
WHERE NOT EXISTS(
SELECT R.ParentID FROM SC.SchemaRelationObjects R WHERE 
R.ObjectID = O.ID AND R.ChildSchemaType = O.SchemaType AND {0}
) AND {1}
", innerConditions.ToSqlString(TSqlBuilder.Instance), outterConditions.ToSqlString(TSqlBuilder.Instance));

            var result = new SchemaObjectCollection();
            var dt = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

            result.LoadFromDataView(dt.DefaultView);

            return result;
        }

        /// <summary>
        /// 清除所有数据。慎用，仅测试使用
        /// </summary>
        public void ClearAllData()
        {
            DbHelper.RunSql("EXEC SC.ClearAllData", this.GetConnectionName());
        }

        /// <summary>
        /// 根据指定的CodeName和Schema类型载入对象
        /// </summary>
        /// <param name="schemas">指定包含的schemas</param>
        /// <param name="codeNames">指定包含的codeName</param>
        /// <param name="normalOnly">表示是否查找所有状态的对象</param>
        /// <param name="ignoreVersions">表示是否忽略时间版本因素</param>
        /// <param name="timePoint">当<paramref name="ignoreVersions"/>为<see langword="true"/>时，表示指定的时间点版本</param>
        /// <returns>如果为<see langword="true"/>表示存在，或<see langword="true"/>表示不存在</returns>
        public SchemaObjectCollection LoadByCodeNameAndSchema(string[] schemas, string[] codeNames, bool normalOnly, bool ignoreVersions, DateTime timePoint)
        {
            if (schemas == null || schemas.Length == 0)
                throw new ArgumentNullException("schemas");
            if (codeNames == null || codeNames.Length == 0)
                throw new ArgumentNullException("codeNames");

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            where.AppendItem("1", (int)1);

            if (normalOnly)
            {
                where.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
            }

            ConnectiveSqlClauseCollection conditions = new ConnectiveSqlClauseCollection(where);

            if (ignoreVersions == false)
            {
                conditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S."));
                conditions.Add(VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O."));
            }

            if (schemas.Length > 1)
            {
                var inSchemas = new InSqlClauseBuilder("S.SchemaType");
                inSchemas.AppendItem(schemas);
            }
            else
            {
                where.AppendItem("S.SchemaType", schemas[0]);
            }

            if (codeNames.Length > 1)
            {
                var inCodeName = new InSqlClauseBuilder("S.CodeName");
                inCodeName.AppendItem(codeNames);
                conditions.Add(inCodeName);
            }
            else
            {
                where.AppendItem("S.CodeName", codeNames[0]);
            }

            return this.LoadByCodeNameInner(conditions);
        }

        /// <summary>
        /// 清除时间点之后的数据，并恢复之前的数据（慎用，仅限测试）
        /// </summary>
        /// <param name="time"></param>
        public void HistoryMoveBack(DateTime time)
        {
            using (HistoryMoveBackAdapter adapter = new HistoryMoveBackAdapter(this.GetConnectionName()))
            {
                adapter.MoveBack("SC.Acl", new string[] { "ContainerID", "MemberID", "ContainerPermission" }, time);
                adapter.MoveBack("SC.Acl_Current", new string[] { "ContainerID", "MemberID", "ContainerPermission" }, time);
                adapter.MoveBack("SC.Conditions", new string[] { "OwnerID", "Type", "SortID" }, time);
                adapter.MoveBack("SC.Conditions_Current", new string[] { "OwnerID", "Type", "SortID" }, time);
                adapter.MoveBack("SC.SchemaApplicationSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaApplicationSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaGroupSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaGroupSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaMembers", new string[] { "ContainerID", "MemberID" }, time);
                adapter.MoveBack("SC.SchemaMembersSnapshot", new string[] { "ContainerID", "MemberID" }, time);
                adapter.MoveBack("SC.SchemaMembersSnapshot_Current", new string[] { "ContainerID", "MemberID" }, time);
                adapter.MoveBack("SC.SchemaObject", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaObjectSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaObjectSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaOrganizationSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaOrganizationSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaPermissionSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaPermissionSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaRelationObjects", new string[] { "ParentID", "ObjectID" }, time);
                adapter.MoveBack("SC.SchemaRelationObjectsSnapshot", new string[] { "ParentID", "ObjectID" }, time);
                adapter.MoveBack("SC.SchemaRelationObjectsSnapshot_Current", new string[] { "ParentID", "ObjectID" }, time);
                adapter.MoveBack("SC.SchemaRoleSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaRoleSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaUserSnapshot", new string[] { "ID" }, time);
                adapter.MoveBack("SC.SchemaUserSnapshot_Current", new string[] { "ID" }, time);
                adapter.MoveBack("SC.UserAndContainerSnapshot", new string[] { "ContainerID", "UserID" }, time);
                adapter.MoveBack("SC.UserAndContainerSnapshot_Current", new string[] { "ContainerID", "UserID" }, time);
            }
        }

        class HistoryMoveBackAdapter : IDisposable
        {
            private Database db;
            private DateTime timeEnd = new DateTime(9999, 9, 9);
            private DbContext dbContext;

            public HistoryMoveBackAdapter(string connectionName)
            {
                this.dbContext = DbContext.GetContext(connectionName);
                this.db = DatabaseFactory.Create(this.dbContext);
            }

            internal void MoveBack(string tableName, string[] keyColumns, DateTime timeToMove)
            {
                if (this.db == null || this.dbContext == null)
                    throw new ObjectDisposedException(this.ToString());

                var cmd = this.db.GetSqlStringCommand("DELETE FROM " + tableName + " WHERE VersionStartTime>@time");
                this.db.AddInParameter(cmd, "time", DbType.DateTime, timeToMove);
                this.db.ExecuteNonQuery(cmd);

                string columns = string.Empty;
                for (int i = 0; i < keyColumns.Length - 1; i++)
                {
                    columns += keyColumns[i] + ",";
                }

                columns += keyColumns[keyColumns.Length - 1];

                string conditions = string.Empty;

                for (int i = 0; i < keyColumns.Length - 1; i++)
                {
                    conditions += "DEST_TBL." + keyColumns[i] + " = SRC_TBL." + keyColumns[i] + " AND ";
                }

                conditions += " DEST_TBL." + keyColumns[keyColumns.Length - 1] + " = SRC_TBL." + keyColumns[keyColumns.Length - 1] + "";

                string sql = string.Format("UPDATE {0} SET VersionEndTime = @endtime FROM {1} DEST_TBL INNER JOIN (SELECT max(VersionStartTime) AS VST,{2} FROM {3} GROUP BY {4}) SRC_TBL ON {5}", tableName, tableName, columns, tableName, columns, conditions);

                cmd = this.db.GetSqlStringCommand(sql);

                this.db.AddInParameter(cmd, "endtime", DbType.DateTime, this.timeEnd);

                this.db.ExecuteNonQuery(cmd);
            }

            public void Dispose()
            {
                if (this.dbContext != null)
                {
                    this.dbContext.Dispose();
                    this.dbContext = null;
                    this.db = null;
                }
            }
        }
    }
}
