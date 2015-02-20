using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
    public partial class SCSnapshotAdapter
    {
        #region Query Application
        public SchemaObjectCollection QueryApplications(string[] schemaTypes, bool includingDeleted, DateTime timePoint)
        {
            SchemaObjectCollection result = new SchemaObjectCollection();

            ConnectiveSqlClauseCollection timeConditionC = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SC.");

            InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("SC.SchemaType");

            schemaBuilder.AppendItem(schemaTypes);

            WhereSqlClauseBuilder builderC = new WhereSqlClauseBuilder();

            if (includingDeleted == false)
                builderC.AppendItem("SC.Status", (int)SchemaObjectStatus.Normal);

            ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(schemaBuilder, builderC, timeConditionC);

            string sql = string.Format("SELECT SC.*" +
                "\nFROM SC.SchemaObject SC" +
                "\nWHERE {0}", connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

            DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

            result.LoadFromDataView(table.DefaultView);

            return result;
        }
        #endregion Query Application

        #region QueryApplicationObjects
        /// <summary>
        /// 查询应用中的角色
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="appCodeName"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryApplicationObjectsByCodeName(string[] schemaTypes, string appCodeName, bool includingDeleted, DateTime timePoint)
        {
            InSqlClauseBuilder schemaTypeBuilder = new InSqlClauseBuilder("M.MemberSchemaType");

            schemaTypeBuilder.AppendItem(schemaTypes);

            WhereSqlClauseBuilder codeBuilder = new WhereSqlClauseBuilder();

            codeBuilder.AppendItem("A.CodeName", appCodeName);

            return QueryApplicationObjectsByBuilder(new ConnectiveSqlClauseCollection(schemaTypeBuilder, codeBuilder), includingDeleted, timePoint);
        }

        /// <summary>
        /// 按照Builder查询应用所包含的元素（角色和权限）
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryApplicationObjectsByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SchemaObjectCollection result = new SchemaObjectCollection();

            if (builder.IsEmpty == false)
            {
                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "A.", "M.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, extraBuilder);

                string resourcePath = "QueryApplicationObjects_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryApplicationObjects.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }

        /// <summary>
        /// 根据AppCodeName和Permission的CodeName查询出对应的角色
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="appCodeName"></param>
        /// <param name="permissionCodeNames"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryPermissionRolesByCodeName(string[] schemaTypes, string appCodeName, string[] permissionCodeNames, bool includingDeleted, DateTime timePoint)
        {
            SchemaObjectCollection result = null;

            if (appCodeName.IsNotEmpty() && permissionCodeNames != null && permissionCodeNames.Length > 0)
            {
                InSqlClauseBuilder schemaTypeBuilder = new InSqlClauseBuilder("M.MemberSchemaType");

                schemaTypeBuilder.AppendItem(schemaTypes);

                WhereSqlClauseBuilder codeBuilder = new WhereSqlClauseBuilder();

                codeBuilder.AppendItem("A.CodeName", appCodeName);

                InSqlClauseBuilder permissionCodeTypeBuilder = new InSqlClauseBuilder("P.CodeName");

                permissionCodeTypeBuilder.AppendItem(permissionCodeNames);

                result = QueryPermissionRolesByBuilder(new ConnectiveSqlClauseCollection(schemaTypeBuilder, codeBuilder, permissionCodeTypeBuilder), includingDeleted, timePoint);
            }
            else
                result = new SchemaObjectCollection();

            return result;
        }

        public SchemaObjectCollection QueryPermissionRolesByBuilder(IConnectiveSqlClause builder, bool includingDeleted, DateTime timePoint)
        {
            builder.NullCheck("builder");

            SchemaObjectCollection result = new SchemaObjectCollection();

            if (builder.IsEmpty == false)
            {
                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "A.", "M.", "P.", "R.");

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, extraBuilder);

                string resourcePath = "QueryPermissionsRoles_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryPermissionsRoles.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }

        /// <summary>
        /// 查询用户所属的角色
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="appCodeName"></param>
        /// <param name="userIDs"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryUserBelongToRoles(string[] schemaTypes, string appCodeName, string[] userIDs, bool includingDeleted, DateTime timePoint)
        {
            SchemaObjectCollection result = new SchemaObjectCollection();

            InSqlClauseBuilder userBuilder = new InSqlClauseBuilder("UC.UserID");

            userBuilder.AppendItem(userIDs);

            if (userBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("MA.MemberSchemaType");
                schemaBuilder.AppendItem(schemaTypes);

                WhereSqlClauseBuilder appBuilder = new WhereSqlClauseBuilder();

                if (appCodeName.IsNotEmpty())
                    appBuilder.AppendItem("A.CodeName", appCodeName);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "A.", "MA.", "UC.");

                string resourcePath = "QueryUserBelongToRoles_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryUserBelongToRoles.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(userBuilder, schemaBuilder, appBuilder, extraBuilder);

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, (row, obj) =>
                {
                    obj.Tag = row["AppID"].ToString();
                });
            }

            return result;
        }

        /// <summary>
        /// 查询用户所属的权限
        /// </summary>
        /// <param name="schemaTypes">角色的SchemaType。权限是从角色推导出来的</param>
        /// <param name="appCodeName"></param>
        /// <param name="userIDs"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SchemaObjectCollection QueryUserBelongToPermissions(string[] schemaTypes, string appCodeName, string[] userIDs, bool includingDeleted, DateTime timePoint)
        {
            SchemaObjectCollection result = new SchemaObjectCollection();

            InSqlClauseBuilder userBuilder = new InSqlClauseBuilder("UC.UserID");

            userBuilder.AppendItem(userIDs);

            if (userBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("MA.MemberSchemaType");
                schemaBuilder.AppendItem(schemaTypes);

                WhereSqlClauseBuilder appBuilder = new WhereSqlClauseBuilder();
                appBuilder.AppendItem("A.CodeName", appCodeName);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "SC.", "A.", "MA.", "UC.", "R.", "SR.");

                string resourcePath = "QueryUserBelongToPermissions_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryUserBelongToPermissions.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(userBuilder, schemaBuilder, appBuilder, extraBuilder);

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, (row, obj) =>
                {
                    obj.Tag = row["AppID"].ToString();
                });
            }

            return result;
        }

        /// <summary>
        /// 查询角色中的用户
        /// </summary>
        /// <param name="schemaTypes">角色的Schema</param>
        /// <param name="appCodeName"></param>
        /// <param name="roleCodeNames"></param>
        /// <param name="removeDuplicateData">是否删除重复的数据</param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryRolesContainsUsers(string[] schemaTypes, string appCodeName, string[] roleCodeNames, bool removeDuplicateData, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            InSqlClauseBuilder roleBuilder = new InSqlClauseBuilder("R.CodeName");
            roleBuilder.AppendItem(roleCodeNames);

            if (roleBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("MA.MemberSchemaType");
                schemaBuilder.AppendItem(schemaTypes);

                WhereSqlClauseBuilder appBuilder = new WhereSqlClauseBuilder();
                appBuilder.AppendItem("A.CodeName", appCodeName);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "A.", "MA.", "R.", "UC.", "SR.", "SC.");

                string resourcePath = "QueryRolesContainsUsers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryRolesContainsUsers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(roleBuilder, schemaBuilder, appBuilder, extraBuilder);

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, removeDuplicateData);
            }

            return result;
        }

        public SCObjectAndRelationCollection QueryRolesContainsUsers(string[] schemaTypes, string[] roleIDs, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            InSqlClauseBuilder roleBuilder = new InSqlClauseBuilder("R.ID");
            roleBuilder.AppendItem(roleIDs);

            if (roleBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("MA.MemberSchemaType");
                schemaBuilder.AppendItem(schemaTypes);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "A.", "MA.", "R.", "UC.", "SR.", "SC.");

                string resourcePath = "QueryRolesContainsUsers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryRolesContainsUsers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(roleBuilder, schemaBuilder, extraBuilder);

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }

        /// <summary>
        /// 查询角色所包含的成员
        /// </summary>
        /// <param name="schemaTypes">成员的SchemaType</param>
        /// <param name="appCodeName"></param>
        /// <param name="roleCodeNames"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryRolesContainsMembers(string[] schemaTypes, string appCodeName, string[] roleCodeNames, bool removeDuplicateData, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            InSqlClauseBuilder roleBuilder = new InSqlClauseBuilder("R.CodeName");
            roleBuilder.AppendItem(roleCodeNames);

            if (roleBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("MA.MemberSchemaType");
                schemaBuilder.AppendItem(schemaTypes);

                WhereSqlClauseBuilder appBuilder = new WhereSqlClauseBuilder();
                appBuilder.AppendItem("A.CodeName", appCodeName);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "A.", "MA.", "R.", "UC.", "SR.", "SC.");

                string resourcePath = "QueryRolesContainsMembers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryRolesContainsMembers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(roleBuilder, schemaBuilder, appBuilder, extraBuilder);

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView, removeDuplicateData);
            }

            return result;
        }

        /// <summary>
        /// 根据角色ID查询出它的内容
        /// </summary>
        /// <param name="schemaTypes">角色的SchemaType</param>
        /// <param name="roleIDs"></param>
        /// <param name="includingDeleted"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        public SCObjectAndRelationCollection QueryRolesContainsMembers(string[] schemaTypes, string[] roleIDs, bool includingDeleted, DateTime timePoint)
        {
            SCObjectAndRelationCollection result = new SCObjectAndRelationCollection();

            InSqlClauseBuilder roleBuilder = new InSqlClauseBuilder("R.ID");
            roleBuilder.AppendItem(roleIDs);

            if (roleBuilder.IsEmpty == false)
            {
                InSqlClauseBuilder schemaBuilder = new InSqlClauseBuilder("MA.MemberSchemaType");
                schemaBuilder.AppendItem(schemaTypes);

                IConnectiveSqlClause extraBuilder = CreateStatusAndTimePointBuilder(includingDeleted, timePoint,
                    "A.", "MA.", "R.", "UC.", "SR.", "SC.");

                string resourcePath = "QueryRolesContainsMembers_Current.sql";

                if (timePoint != DateTime.MinValue && includingDeleted == true)
                    resourcePath = "QueryRolesContainsMembers.sql";

                string sqlTemplate = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Concat("MCS.Library.SOA.DataObjects.Security.Adapters.Templates.", resourcePath));

                ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(roleBuilder, schemaBuilder, extraBuilder);

                string sql = string.Format(sqlTemplate, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

                DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

                result.LoadFromDataView(table.DefaultView);
            }

            return result;
        }
        #endregion QueryApplicationObjects
    }
}
