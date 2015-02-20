using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
    /// <summary>
    /// 与快照表相关的查询。
    /// 这里有很多方法返回SCObjectAndRelation集合，这个对象SCObjectAndRelation包含了对象及其指定的关系信息。
    /// 在一定的上下文环境中，例如某个组织，一个对象肯定有唯一的关系对象。SCObjectAndRelation就是表示了这种信息。
    /// SCObjectAndRelation的FillDetail会返回SchmemaObjectBase信息
    /// </summary>
    public partial class SCSnapshotAdapter
    {
        /// <summary>
        /// <see cref="SCSnapshotAdapter"/>的实例，此字段为只读
        /// </summary>
        public static readonly SCSnapshotAdapter Instance = new SCSnapshotAdapter();

        private SCSnapshotAdapter()
        {
        }

        /// <summary>
        /// 根据多个FullPath进行查询
        /// </summary>
        /// <param name="fullPaths">一个或多个全路径</param>
        /// <returns></returns>
        [Obsolete]
        public SCObjectAndRelationCollection QueryByMultiFullPaths(params string[] fullPaths)
        {
            fullPaths.NullCheck("fullPaths");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (fullPaths.Length > 0)
            {
                MarkupFullPaths(fullPaths);

                string combinedPaths = string.Join(";", fullPaths);

                DataTable table = DbHelper.RunSqlReturnDS("EXEC SC.QueryObjectsByMultiFullPath " + TSqlBuilder.Instance.CheckUnicodeQuotationMark(combinedPaths), this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }

        #region QueryCount
        /// <summary>
        /// 查询每一种类型对象的个数（状态为正常的）
        /// </summary>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public Dictionary<string, int> QueryCountGroupBySchema(DateTime timePoint)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            IConnectiveSqlClause connectiveBuilder = CreateStatusAndTimePointBuilder(false, timePoint, "SC.");

            string tableName = "SC.SchemaObjectSnapshot";

            if (timePoint == DateTime.MinValue)
                tableName = "SC.SchemaObjectSnapshot_Current";

            string sql = string.Format("SELECT SC.SchemaType, COUNT(*) AS Count FROM {0} SC WHERE {1} GROUP BY SC.SchemaType",
                tableName, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

            DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

            foreach (DataRow row in table.Rows)
                result.Add(row["SchemaType"].ToString(), (int)row["Count"]);

            return result;
        }

        #endregion QueryCount

        #region QueryObjectAndRelation
        /// <summary>
        /// 根据对象类型，FullPath和时间点查询数据
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="fullPaths"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByFullPaths(string[] schemaTypes, string[] fullPaths, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");
            fullPaths.NullCheck("fullPaths");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SR.FullPath");

            inBuilder.AppendItem(fullPaths);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

                wBuilder.AppendItem(schemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryObjectAndRelationByBuilder(connectiveBuilder, includingDeleted, timePoint, -1);

                //由于Root在数据库中不存在，因此如果查询条件中包含Root，则添加进来
                SCObjectAndRelation root = SCObjectAndRelation.GetRoot();

                if (Array.Exists(fullPaths, s => s == root.FullPath))
                    result.Add(root);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据对象类型，ID和时间点查询数据
        /// </summary>
        /// <param name="schemaTypes">字符串的数组，每个字符串代表一种模式类型</param>
        /// <param name="ids">字符串的数组，每个字符串代表一个ID</param>
        /// <param name="includingDeleted">是否包含已删除的对象</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByIDs(string[] schemaTypes, string[] ids, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");
            ids.NullCheck("ids");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SC.ID");

            inBuilder.AppendItem(ids);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

                wBuilder.AppendItem(schemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryObjectAndRelationByBuilder(connectiveBuilder, includingDeleted, timePoint, -1);

                //由于Root在数据库中不存在，因此如果查询条件中包含Root，则添加进来
                SCObjectAndRelation root = SCObjectAndRelation.GetRoot();

                if (Array.Exists(ids, s => s == root.ID))
                    result.Add(root);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据CodeName查询对象关系
        /// </summary>
        /// <param name="schemaTypes">字符串的数组，每个字符串代表一种模式类型</param>
        /// <param name="codeNames">字符串的数组，每个字符串表示一个代码名称</param>
        /// <param name="includingDeleted">是否包含已删除的对象</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByCodeNames(string[] schemaTypes, string[] codeNames, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");
            codeNames.NullCheck("codeNames");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SC.CodeName");

            inBuilder.AppendItem(codeNames);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

                wBuilder.AppendItem(schemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryObjectAndRelationByBuilder(connectiveBuilder, includingDeleted, timePoint, -1);

                //由于Root在数据库中不存在，因此如果查询条件中包含Root，则添加进来
                SCObjectAndRelation root = SCObjectAndRelation.GetRoot();

                if (Array.Exists(codeNames, s => s == root.CodeName))
                    result.Add(root);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据指定的条件检索对象关系
        /// </summary>
        /// <param name="builder">包含条件的<see cref="IConnectiveSqlClause"/></param>
        /// <param name="timePoint"></param>
        /// <param name="includingDeleted">表示是否包含值</param>
        /// <param name="maxSize">当>0时，限制返回的结果行数，否则忽略此参数</param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint, int maxSize)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                var timeConditionC = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SC.");
                var timeConditionR = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SR.");

                WhereSqlClauseBuilder builderC = new WhereSqlClauseBuilder();

                if (includingDeleted == false)
                    builderC.AppendItem("SC.Status", (int)SchemaObjectStatus.Normal);

                WhereSqlClauseBuilder builderR = new WhereSqlClauseBuilder();

                if (includingDeleted == false)
                    builderR.AppendItem("SR.Status", (int)SchemaObjectStatus.Normal);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, builderC, builderR, timeConditionC, timeConditionR);

                string resourcePath = "QueryObjectAndRelation_Current.sql";

                if (TimePointContext.Current.UseCurrentTime == false || includingDeleted)
                    resourcePath = "QueryObjectAndRelation.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, (maxSize > 0 ? ("TOP " + maxSize + " ") : ""), connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }
        #endregion QueryObjectAndRelation

        #region QueryObjectAndRelationByParents
        /// <summary>
        /// 查询子对象
        /// </summary>
        /// <param name="schemaTypes">子对象的类型</param>
        /// <param name="parentIDs"></param>
        /// <param name="recursively">是否包含所有子对象</param>
        /// <param name="includingNonDefault">是否包含兼职的对象</param>
        /// <param name="includingDeleted">是否包含删除的对象</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByParentIDs(string[] schemaTypes, string[] parentIDs, bool recursively, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");
            parentIDs.NullCheck("parentIDs");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SR.ParentID");

            inBuilder.AppendItem(parentIDs);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

                wBuilder.AppendItem(schemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                //if (parentIDs.Contains(SCOrganization.RootOrganizationID))
                //    result = QueryObjectAndRelationByRoot(schemaTypes, recursively, includingNonDefault, includingDeleted, timePoint);
                //else
                //    result = new SCObjectAndRelationCollection();

                IEnumerable<SCObjectAndRelation> tempResult = QueryObjectAndRelationByParentIDBuilder(connectiveBuilder, recursively, includingNonDefault, includingDeleted, timePoint);

                result.CopyFrom(tempResult);
            }

            return result;
        }

        private IEnumerable<SCObjectAndRelation> QueryObjectAndRelationByParentIDBuilder(IConnectiveSqlClause builder, bool recursively, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                WhereSqlClauseBuilder relationBuilder = new WhereSqlClauseBuilder();

                if (includingNonDefault == false)
                    relationBuilder.AppendItem("SR.IsDefault", 1);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "SR.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, relationBuilder, extraBuilder);

                string resourcePath = "QueryObjectAndRelationByParentID_Current.sql";

                if (timePoint != DateTime.MinValue || includingDeleted)
                    resourcePath = "QueryObjectAndRelationByParentID.sql";

                string sqlTemplate = Assembly.GetExecutingAssembly().LoadStringFromResource(string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, true);
            }

            return result;
        }

        private SCObjectAndRelationCollection QueryObjectAndRelationByRoot(string[] schemaTypes, bool recursively, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

            wBuilder.AppendItem(schemaTypes);

            WhereSqlClauseBuilder relationBuilder = new WhereSqlClauseBuilder();

            if (includingNonDefault == false)
                relationBuilder.AppendItem("SR.IsDefault", 1);

            string likeExp = "______";

            if (recursively)
                likeExp = "_%";

            relationBuilder.AppendItem("SR.GlobalSort", likeExp, "LIKE");

            IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                "SC.", "SR.");

            ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(relationBuilder, extraBuilder);

            string resourcePath = "QueryObjectAndRelationByRoot_Current.sql";

            if (timePoint != DateTime.MinValue)
                resourcePath = "QueryObjectAndRelationByRoot.sql";

            string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

            string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

            DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            result.LoadFromDataView(table.DefaultView, true);

            return result;
        }

        /// <summary>
        /// 查询某个CodeName下的所有子对象。
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="codeNames"></param>
        /// <param name="includingNonDefault"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByParentCodeNames(string[] schemaTypes, string[] codeNames, bool recursively, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder relationInBuilder = new InSqlClauseBuilder("SCContainer.CodeName");

            relationInBuilder.AppendItem(codeNames);

            if (relationInBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

                wBuilder.AppendItem(schemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, relationInBuilder);

                result = QueryObjectAndRelationByParentBuilder(connectiveBuilder, recursively, includingNonDefault, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 查询某个FullPath下的所有子对象。
        /// </summary>
        /// <param name="fullPaths"></param>
        /// <param name="schemaTypes"></param>
        /// <param name="includingNonDefault"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByParentFullPath(string[] schemaTypes, string[] fullPaths, bool recursively, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            schemaTypes.NullCheck("schemaTypes");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder relationInBuilder = new InSqlClauseBuilder("SRContainer.FullPath");

            relationInBuilder.AppendItem(fullPaths);

            if (relationInBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("SC.SchemaType");

                wBuilder.AppendItem(schemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, relationInBuilder);

                result = QueryObjectAndRelationByParentBuilder(connectiveBuilder, recursively, includingNonDefault, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 构造查询子对象的SQL
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="recursively"></param>
        /// <param name="includingNonDefault"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByParentBuilder(IConnectiveSqlClause builder, bool recursively, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                WhereSqlClauseBuilder relationBuilder = new WhereSqlClauseBuilder();

                if (includingNonDefault == false)
                    relationBuilder.AppendItem("SR.IsDefault", 1);

                string likeExp = "______";

                if (recursively)
                    likeExp = "_%";

                string expression = string.Format("SRContainer.GlobalSort + {0}", TSqlBuilder.Instance.CheckUnicodeQuotationMark(likeExp));

                relationBuilder.AppendItem("SR.GlobalSort", expression, "LIKE", true);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "SR.", "SRContainer.", "SCContainer.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, relationBuilder, extraBuilder);

                string resourcePath = "QueryObjectAndRelationByParent_Current.sql";

                if (timePoint != DateTime.MinValue || includingDeleted)
                    resourcePath = "QueryObjectAndRelationByParent.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, true);
            }

            return result;
        }

        /// <summary>
        /// 在父对象下根据关键字查找子对象
        /// </summary>
        /// <param name="schemaTypes">子对象的类型</param>
        /// <param name="ids">父对象的ID</param>
        /// <param name="keyword">搜索的关键字</param>
        /// <param name="maxCount">最大返回行数</param>
        /// <param name="includeDescendent">是否递归查找子对象</param>
        /// <param name="includingNonDefault">是否包含兼职的</param>
        /// <param name="includingDeleted">是否包含已删除的</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByKeywordAndParentIDs(string[] schemaTypes, string[] ids, string keyword, int maxCount, bool includeDescendent, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection parents = QueryObjectAndRelationByIDs(new string[0], ids, includingDeleted, timePoint);

            IConnectiveSqlClause builder = GetFullPathBuilder(schemaTypes, parents, keyword, includeDescendent, includingNonDefault);

            SCObjectAndRelationCollection result = null;

            if (keyword.IsNotEmpty())
                result = QueryObjectAndRelationByBuilder(builder, includingDeleted, timePoint, maxCount);
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 在父对象下根据关键字查找子对象
        /// </summary>
        /// <param name="schemaTypes">子对象的类型</param>
        /// <param name="fullPaths">父对象的FullPath</param>
        /// <param name="keyword">搜索的关键字</param>
        /// <param name="maxCount">最大返回行数</param>
        /// <param name="includeDescendent">是否递归查找子对象</param>
        /// <param name="includingNonDefault">是否包含兼职的</param>
        /// <param name="includingDeleted">是否包含已删除的</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByKeywordAndParentFullPaths(string[] schemaTypes, string[] fullPaths, string keyword, int maxCount, bool includeDescendent, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection parents = QueryObjectAndRelationByFullPaths(new string[0], fullPaths, includingDeleted, timePoint);

            IConnectiveSqlClause builder = GetFullPathBuilder(schemaTypes, parents, keyword, includeDescendent, includingNonDefault);

            SCObjectAndRelationCollection result = null;

            if (keyword.IsNotEmpty())
                result = QueryObjectAndRelationByBuilder(builder, includingDeleted, timePoint, maxCount);
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 在父对象下根据关键字查找子对象
        /// </summary>
        /// <param name="schemaTypes">子对象的类型</param>
        /// <param name="codeNames">父对象的CodeName</param>
        /// <param name="keyword">搜索的关键字</param>
        /// <param name="maxCount">最大返回行数</param>
        /// <param name="includeDescendent">是否递归查找子对象</param>
        /// <param name="includingNonDefault">是否包含兼职的</param>
        /// <param name="includingDeleted">是否包含已删除的</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryObjectAndRelationByKeywordAndParentCodeNames(string[] schemaTypes, string[] codeNames, string keyword, int maxCount, bool includeDescendent, bool includingNonDefault, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection parents = QueryObjectAndRelationByCodeNames(new string[0], codeNames, includingDeleted, timePoint);

            IConnectiveSqlClause builder = GetFullPathBuilder(schemaTypes, parents, keyword, includeDescendent, includingNonDefault);

            SCObjectAndRelationCollection result = null;

            if (keyword.IsNotEmpty())
                result = QueryObjectAndRelationByBuilder(builder, includingDeleted, timePoint, maxCount);
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        private static IConnectiveSqlClause GetFullPathBuilder(string[] schemaTypes, SCObjectAndRelationCollection parents, string keyword, bool includeDescendent, bool includingNonDefault)
        {
            WhereSqlClauseBuilder fullPathBuilder = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);

            foreach (SCObjectAndRelation p in parents)
            {
                if (includeDescendent)
                    fullPathBuilder.AppendItem("SR.FullPath", p.FullPath + "%", "LIKE");
                else
                    fullPathBuilder.AppendItem("SR.ParentID", p.ID);
            }

            InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("SC.SchemaType");

            schemaBuilder.AppendItem(schemaTypes);

            WhereSqlClauseBuilder relationBuilder = new WhereSqlClauseBuilder();

            if (includingNonDefault == false)
                relationBuilder.AppendItem("SR.IsDefault", 1);

            if (keyword != "@SearchAll@")	//@SearchAll@是默认搜索所有数据的标志，因此不加约束
                relationBuilder.AppendItem("SC.SearchContent", TSqlBuilder.Instance.FormatFullTextString(LogicOperatorDefine.And, keyword), string.Empty, "CONTAINS(${DataField}$, ${Data}$)");

            ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection(fullPathBuilder, schemaBuilder, relationBuilder);

            return result;
        }
        #endregion QueryObjectAndRelationByParents

        #region QueryUserAndContainers
        /// <summary>
        /// 根据用户ID查询其所属的容器
        /// </summary>
        /// <param name="containerSchemaTypes"></param>
        /// <param name="userIDs"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryUserBelongToContainersByIDs(string[] containerSchemaTypes, string[] userIDs, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            userIDs.NullCheck("userIDs");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("UC.UserID");

            inBuilder.AppendItem(userIDs);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryUserBelongToContainersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryUserBelongToContainersByFullPaths(string[] containerSchemaTypes, string[] fullPaths, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            fullPaths.NullCheck("fullPaths");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SR.FullPath");

            inBuilder.AppendItem(fullPaths);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryUserBelongToContainersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryUserBelongToContainersByCodeNames(string[] containerSchemaTypes, string[] codeNames, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            codeNames.NullCheck("codeNames");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SC.CodeName");

            inBuilder.AppendItem(codeNames);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryUserBelongToContainersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据Builder进行用户属于某容器的查询
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryUserBelongToContainersByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "UC.", "SR.", "SRContainer.", "SCContainer.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, extraBuilder);

                string resourcePath = "QueryUserContainers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryUserContainers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, true);
            }

            return result;
        }

        /// <summary>
        /// 根据容器的ID查询出包含的用户
        /// </summary>
        /// <param name="containerSchemaTypes">容器的SchemaType</param>
        /// <param name="containerIDs">容器的ID</param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryContainerContainsUsersByIDs(string[] containerSchemaTypes, string[] containerIDs, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            containerIDs.NullCheck("containerIDs");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("UC.ContainerID");

            inBuilder.AppendItem(containerIDs);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryContainerContainsUsersBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryContainerContainsUsersByFullPaths(string[] containerSchemaTypes, string[] containerFullPaths, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            containerFullPaths.NullCheck("containerFullPaths");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SRContainer.FullPath");

            inBuilder.AppendItem(containerFullPaths);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryContainerContainsUsersBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryContainerContainsUsersByCodeNames(string[] containerSchemaTypes, string[] containerCodeNames, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            containerCodeNames.NullCheck("containerCodeNames");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SCContainer.CodeName");

            inBuilder.AppendItem(containerCodeNames);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryContainerContainsUsersBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据Builder进行某容器包含用户的查询
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryContainerContainsUsersBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "UC.", "SR.", "SRContainer.", "SCContainer.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, extraBuilder);

                string resourcePath = "QueryContainsUsers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryContainsUsers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, true);
            }

            return result;
        }
        #endregion QueryUserAndContainers

        #region QueryMemberAndContainers
        public SCObjectAndRelationCollection QueryMemberBelongToContainersByIDs(string[] containerSchemaTypes, string[] userIDs, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            userIDs.NullCheck("userIDs");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("UC.MemberID");

            inBuilder.AppendItem(userIDs);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryMemberBelongToContainersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryMemberBelongToContainersByFullPaths(string[] containerSchemaTypes, string[] fullPaths, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            fullPaths.NullCheck("fullPaths");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SR.FullPath");

            inBuilder.AppendItem(fullPaths);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryMemberBelongToContainersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryMemberBelongToContainersByCodeNames(string[] containerSchemaTypes, string[] codeNames, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            codeNames.NullCheck("codeNames");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SC.CodeName");

            inBuilder.AppendItem(codeNames);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryMemberBelongToContainersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据Builder进行用户属于某容器的查询
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryMemberBelongToContainersByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "UC.", "SR.", "SRContainer.", "SCContainer.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, extraBuilder);

                string resourcePath = "QueryMemberContainers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryMemberContainers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, true);
            }

            return result;
        }

        public SCObjectAndRelationCollection QueryContainerContainsMembersByIDs(string[] containerSchemaTypes, string[] containerIDs, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            containerIDs.NullCheck("containerIDs");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("UC.ContainerID");

            inBuilder.AppendItem(containerIDs);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryContainerContainsMembersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryContainerContainsMembersByFullPaths(string[] containerSchemaTypes, string[] containerFullPaths, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            containerFullPaths.NullCheck("containerFullPaths");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SRContainer.FullPath");

            inBuilder.AppendItem(containerFullPaths);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryContainerContainsMembersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        public SCObjectAndRelationCollection QueryContainerContainsMembersByCodeNames(string[] containerSchemaTypes, string[] containerCodeNames, bool includingDeleted, DateTime timePoint)
        {
            containerSchemaTypes.NullCheck("schemaTypes");
            containerCodeNames.NullCheck("containerCodeNames");

            SCObjectAndRelationCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SCContainer.CodeName");

            inBuilder.AppendItem(containerCodeNames);

            if (inBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder wBuilder = new InSqlClauseBuilder("UC.ContainerSchemaType");

                wBuilder.AppendItem(containerSchemaTypes);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(wBuilder, inBuilder);

                result = QueryContainerContainsMembersByBuilder(connectiveBuilder, includingDeleted, timePoint);
            }
            else
                result = new SCObjectAndRelationCollection();

            return result;
        }

        /// <summary>
        /// 根据Builder进行某容器包含用户的查询
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryContainerContainsMembersByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            if (builder.IsEmpty == false)
            {
                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "UC.", "SR.", "SRContainer.", "SCContainer.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, extraBuilder);

                string resourcePath = "QueryContainsMembers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryContainsMembers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, true);
            }

            return result;
        }
        #endregion QueryMemberAndContainers

        #region QueryPermissions
        /// <summary>
        /// 根据用户ID查询权限
        /// </summary>
        /// <param name="userIDs"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryPermissionsByUserIDs(string[] userIDs, bool includingDeleted, DateTime timePoint)
        {
            userIDs.NullCheck("userIDs");

            SchemaObjectCollection result = null;

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("UC.UserID");

            inBuilder.AppendItem(userIDs);

            if (inBuilder.IsEmpty == false)
                result = QueryPermissionsByBuilder(inBuilder, includingDeleted, timePoint);
            else
                result = new SchemaObjectCollection();

            return result;
        }

        /// <summary>
        /// 根据Builder查询用户所属权限信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryPermissionsByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SchemaObjectCollection result = new SchemaObjectCollection();

            if (builder.IsEmpty == false)
            {
                var timeConditionC = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SC.");

                WhereSqlClauseBuilder builderC = new WhereSqlClauseBuilder();

                if (includingDeleted == false)
                    builderC.AppendItem("SC.Status", (int)SchemaObjectStatus.Normal);

                var timeConditionU = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "UC.");

                WhereSqlClauseBuilder builderU = new WhereSqlClauseBuilder();

                if (includingDeleted == false)
                    builderU.AppendItem("UC.Status", (int)SchemaObjectStatus.Normal);

                var timeConditionR = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SR.");

                WhereSqlClauseBuilder builderR = new WhereSqlClauseBuilder();

                if (includingDeleted == false)
                    builderR.AppendItem("UC.Status", (int)SchemaObjectStatus.Normal);

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, builderC, builderU, builderR, timeConditionC, timeConditionR, timeConditionU);

                string sql = string.Format("SELECT SC.*" +
                    "\nFROM SC.UserAndContainerSnapshot UC INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON UC.ContainerID = SR.ParentID INNER JOIN SC.SchemaObject SC ON SC.ID = SR.ObjectID" +
                    "\nWHERE {0}", connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }
        #endregion QueryPermissions
    }
}
