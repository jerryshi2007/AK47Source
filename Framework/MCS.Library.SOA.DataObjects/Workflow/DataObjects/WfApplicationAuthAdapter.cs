using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Data;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfApplicationAuthAdapter : UpdatableAndLoadableAdapterBase<WfApplicationAuth, WfApplicationAuthCollection>
    {
        public static readonly WfApplicationAuthAdapter Instance = new WfApplicationAuthAdapter();

        private WfApplicationAuthAdapter()
        {
        }

        public WfApplicationAuth Load(string appName, string progName, WfApplicationAuthType authType)
        {
            appName.CheckStringIsNullOrEmpty("appName");
            progName.CheckStringIsNullOrEmpty("progName");

            return WfApplicationAuthAdapter.Instance.Load(builder =>
            {
                builder.AppendItem("APPLICATION_NAME", appName);
                builder.AppendItem("PROGRAM_NAME", progName);
                builder.AppendItem("AUTH_TYPE", authType.ToString());
            }).FirstOrDefault();
        }

        public WfApplicationAuthCollection Load(string appName, WfApplicationAuthType authType)
        {
            appName.CheckStringIsNullOrEmpty("appName");

            return WfApplicationAuthAdapter.Instance.Load(builder =>
            {
                builder.AppendItem("APPLICATION_NAME", appName).AppendItem("AUTH_TYPE", authType.ToString());
            });
        }

        /// <summary>
        /// 读取用户对于各应用的管理权限。这个信息是被缓存的
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public WfApplicationAuthCollection GetUserApplicationAuthInfo(IUser user)
        {
            user.NullCheck("user");

            return WfApplicationAuthCache.Instance.GetOrAddNewValue(user.ID, (cache, key) =>
                {
                    WfApplicationAuthCollection result = LoadUserApplicationAuthInfo(user);

                    cache.Add(key, result);

                    return result;
                });
        }

        /// <summary>
        /// 读取用户对于各应用的管理权限，不通过缓存
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public WfApplicationAuthCollection LoadUserApplicationAuthInfo(IUser user)
        {
            user.NullCheck("user");

            List<IRole> roles = user.Roles.GetAllRoles();

            List<string> roleIDs = new List<string>();

            foreach (IRole role in roles)
                roleIDs.Add(role.ID);

            string sql = "SELECT * FROM WF.UNION_APP_AUTH_WITH_ALIAS WHERE {0}";

            WfApplicationAuthCollection result = new WfApplicationAuthCollection();

            if (roleIDs.Count > 0)
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ROLE_ID");

                inBuilder.AppendItem(roleIDs.ToArray());

                ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And,
                        inBuilder, new WhereSqlClauseBuilder().AppendTenantCode(typeof(WfApplicationAuth)));

                sql = string.Format(sql, inBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

                DataView view = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0].DefaultView;

                ORMapping.DataViewToCollection(result, view);
            }

            return result;
        }

        protected override string GetConnectionName()
        {
            return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
        }
    }
}
