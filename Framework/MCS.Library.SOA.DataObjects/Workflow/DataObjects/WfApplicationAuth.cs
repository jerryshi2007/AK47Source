using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 应用授权类型
    /// </summary>
    public enum WfApplicationAuthType
    {
        [EnumItemDescription("")]
        None = 0,

        [EnumItemDescription("表单管理员")]
        FormAdmin = 1,

        [EnumItemDescription("表单查看者")]
        FormViewer = 2
    }

    /// <summary>
    /// 应用的分类授权信息
    /// </summary>
    [Serializable]
    [ORTableMapping("WF.APP_PROGRAM_AUTH")]
    [TenantRelativeObject]
    public class WfApplicationAuth
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        [ORFieldMapping("APPLICATION_NAME", PrimaryKey = true)]
        public string ApplicationName
        {
            get;
            set;
        }

        /// <summary>
        /// 应用模块名称
        /// </summary>
        [ORFieldMapping("PROGRAM_NAME", PrimaryKey = true)]
        public string ProgramName
        {
            get;
            set;
        }

        /// <summary>
        /// 授权类型
        /// </summary>
        [ORFieldMapping("AUTH_TYPE", PrimaryKey = true)]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
        public WfApplicationAuthType AuthType
        {
            get;
            set;
        }

        /// <summary>
        /// 角色ID
        /// </summary>
        [ORFieldMapping("ROLE_ID")]
        public string RoleID
        {
            get;
            set;
        }

        /// <summary>
        /// 角色描述
        /// </summary>
        [ORFieldMapping("ROLE_DESCRIPTION")]
        public string RoleDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CREATE_TIME")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public DateTime CreateTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 应用授权类型的集合类
    /// </summary>
    [Serializable]
    public class WfApplicationAuthCollection : EditableDataObjectCollectionBase<WfApplicationAuth>
    {
        /// <summary>
        /// 是否包含某一应用下的某一类权限
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="progName"></param>
        /// <returns></returns>
        public bool Contains(string appName, string progName, WfApplicationAuthType authType)
        {
            bool result = false;

            foreach (WfApplicationAuth authData in this)
            {
                if (string.Compare(appName, authData.ApplicationName, true) == 0 &&
                    string.Compare(progName, authData.ProgramName, true) == 0 &&
                    authData.AuthType == authType)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 扫描整个集合，生成带所有APPLICATION_NAME和PROGRAM_NAME的的ConnectiveSqlClauseCollection，条件之间使用运算符OR。用于筛选出用户可以看的到的列表
        /// </summary>
        /// <param name="appFieldName">APPLICATION_NAME的字段名</param>
        /// <param name="progFieldName">PROGRAM_NAME的字段名</param>
        /// <returns></returns>
        public ConnectiveSqlClauseCollection GetApplicationAndProgramBuilder(string appFieldName, string progFieldName)
        {
            ConnectiveSqlClauseCollection result = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);

            Dictionary<string, string> addedKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (WfApplicationAuth authData in this)
            {
                string key = CalculateKey(authData.ApplicationName, authData.ProgramName);

                if (addedKey.ContainsKey(key) == false)
                {
                    WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder(LogicOperatorDefine.And);

                    builder.AppendItem(appFieldName, authData.ApplicationName);
                    builder.AppendItem(progFieldName, authData.ProgramName);

                    addedKey.Add(key, key);

                    result.Add(builder);
                }
            }

            result = result.AppendTenantCodeSqlClause(typeof(WfApplicationAuth));

            return result;
        }

        private static string CalculateKey(string appName, string progName)
        {
            return appName + "~" + progName;
        }
    }
}
