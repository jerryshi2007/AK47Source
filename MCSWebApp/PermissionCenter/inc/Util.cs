using System;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Web.Library.Script;
using PermissionCenter.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
    public static class Util
    {
        internal static readonly char[] CommaSpliter = new char[] { ',' };

        private static readonly object TheDicKey = new object();

        private static System.Collections.Specialized.HybridDictionary dicIco16 = new System.Collections.Specialized.HybridDictionary();

        internal static IUser CurrentUser
        {
            get
            {
                return MCS.Library.Principal.DeluxeIdentity.CurrentUser;
            }
        }

        public static bool SuperVisiorMode
        {
            get
            {
                return SCPrincipalExtension.IsSupervisor(DeluxePrincipal.Current);
            }
        }

        public static bool IsRoleEmpty(string roleId)
        {
            return PC.Adapters.UserAndContainerSnapshotAdapter.Instance.CountAliveUsersByContainer(roleId, DateTime.MinValue) == 0;
        }

        /// <summary>
        /// 编码用于HTML元素属性的字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string HtmlAttributeEncode(string val)
        {
            return System.Web.HttpUtility.HtmlAttributeEncode(val);
        }

        /// <summary>
        /// 提供
        /// </summary>
        /// <param name="schemaObj">一个<see cref="MCS.Library.SOA.DataObjects.SCSimpleObject"/>，或一个<see cref="System.Data.DataRowView"/></param>
        /// <returns></returns>
        public static string CssSpritesFor(object schemaObj, IconSizeType size)
        {
            SCSimpleObject obj = schemaObj as SCSimpleObject;
            System.Data.DataRowView view = schemaObj as System.Data.DataRowView;
            string css = string.Empty;
            string schemaType = null;
            int status = 0;
            if (obj != null)
            {
                schemaType = obj.SchemaType;
                status = obj.Status == SchemaObjectStatus.Normal ? 0 : 1;
            }
            else if (view != null)
            {
                schemaType = (string)view["SchemaType"];
                status = 0;
            }

            if (schemaType != null)
            {
                switch (size)
                {
                    case IconSizeType.Size16:
                        css = "pc-icon16";
                        break;
                    case IconSizeType.Size32:
                        css = "pc-icon32";
                        break;
                    default:
                        break;
                }

                if (status != 0)
                {
                    css += " pc-status-deleted";
                }

                css += " " + schemaType;
            }

            return css;
        }

        /// <summary>
        /// 确定一个Schema是否属于人员
        /// </summary>
        /// <param name="schemaType">Schema的类型</param>
        /// <returns><see langword="true"/>表示是，<see langword="true"/>表示不是。</returns>
        public static bool IsUser(string schemaType)
        {
            return Util.GetSchemaCatgoryDictionary()[schemaType].Equals("Users");
        }

        /// <summary>
        /// 确定一个Schema是否属于群组
        /// </summary>
        /// <param name="schemaType">Schema的类型</param>
        /// <returns><see langword="true"/>表示是，<see langword="true"/>表示不是。</returns>
        public static bool IsGroup(string schemaType)
        {
            return Util.GetSchemaCatgoryDictionary()[schemaType].Equals("Groups");
        }

        /// <summary>
        /// 确定一个Schema是否属于角色
        /// </summary>
        /// <param name="schemaType">Schema的类型</param>
        /// <returns><see langword="true"/>表示是，<see langword="true"/>表示不是。</returns>
        public static bool IsRole(string schemaType)
        {
            return Util.GetSchemaCatgoryDictionary()[schemaType].Equals("Roles");
        }

        /// <summary>
        /// 确定一个Schema是否属于角色或权限
        /// </summary>
        /// <param name="schemaType">Schema的类型</param>
        /// <returns><see langword="true"/>表示是，<see langword="true"/>表示不是。</returns>
        public static bool IsRoleOrPermission(string schemaType)
        {
            string category = (string)Util.GetSchemaCatgoryDictionary()[schemaType];

            return category.Equals("Roles") || category.Equals("Permissions");
        }

        /// <summary>
        /// 确定一个Schema是否属于组织
        /// </summary>
        /// <param name="schemaType">Schema的类型</param>
        /// <returns><see langword="true"/>表示是，<see langword="true"/>表示不是。</returns>
        public static bool IsOrganization(string schemaType)
        {
            return Util.GetSchemaCatgoryDictionary()[schemaType].Equals("Organizations");
        }

        public static bool IsAclContainer(string schemaType)
        {
            var cate = (string)Util.GetSchemaCatgoryDictionary()[schemaType];

            switch (cate)
            {
                case "Organizations":
                case "Applications":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 确定当前上下文下进行的更新操作是安全操作
        /// </summary>
        internal static void EnsureOperationSafe()
        {
            if (CheckOperationSafe() == false)
            {
                throw new InvalidOperationException("当前使用模拟时间进行数据操作是被禁止的。" + Environment.NewLine + "如果要启用，请修改appSettings配置节中的enableSimulatedOperation项。");
            }
        }

        /// <summary>
        /// 确定当前上下文下进行的更新操作是否是允许
        /// </summary>
        /// <returns><see langword="true"/>表示操作安全，否则表示操作禁止。</returns>
        internal static bool CheckOperationSafe()
        {
            if (TimePointContext.Current.UseCurrentTime == false)
            {
                if ("true" != System.Configuration.ConfigurationManager.AppSettings["enableSimulatedOperation"])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 根据当前时间上下文来产生一个时间。
        /// </summary>
        /// <returns>当使用当前时间时为<see cref="DateTime.MinValue"/> - 或 - 表示当前模拟时间的<see cref="DateTime"/></returns>
        internal static DateTime GetTime()
        {
            if (TimePointContext.Current.UseCurrentTime)
                return DateTime.MinValue;
            else
                return TimePointContext.Current.SimulatedTime;
        }

        /// <summary>
        /// 将脚本放在块中
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        internal static string SurroundScriptBlock(string script)
        {
            return "<script type=\"text/javascript\">" + script + "</script>";
        }

        internal static StringBuilder BeginScriptBlock()
        {
            StringBuilder strB = new StringBuilder("<script type=\"text/javascript\">");
            strB.AppendLine();
            return strB;
        }

        internal static string EndScriptBlock(StringBuilder builder)
        {
            builder.AppendLine("</script>");
            return builder.ToString();
        }

        /// <summary>
        /// 生成没有冲突的CodeName
        /// </summary>
        /// <param name="suggestName">建议的名称</param>
        /// <param name="category">模式类型名</param>
        /// <returns>可用的名称</returns>
        internal static string MakeNoConflictCodeName(string suggestName, string schemaType)
        {
            string newName;
            int i = 0;
            bool success = false;
            do
            {
                newName = suggestName + "(" + i++ + ")";
                success = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(schemaType, newName, SchemaObjectStatus.Normal, DateTime.MinValue) == null;
            }
            while (success == false);

            return newName;
        }

        /// <summary>
        /// 获取可用的操作类型
        /// </summary>
        /// <param name="keys"></param>
        /// <returns><see cref="TransferObjectType"/>的组合值</returns>
        internal static TransferObjectType GetAvailableOperation(string[] keys)
        {
            TransferObjectType result = TransferObjectType.None;
            if (keys != null)
            {
                var relations = DbUtil.LoadCurrentParentRelations(keys, SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());
                bool existRootOrg = false;

                foreach (var r in relations)
                {
                    if (Util.IsOrganization(r.ChildSchemaType) && r.ParentID == PC.SCOrganization.RootOrganizationID)
                    {
                        existRootOrg = true; // 一级组织原则上不允许转移，如果含有一级组织，则什么都做不成
                        break;
                    }
                }

                if (existRootOrg == false)
                {
                    foreach (var item in DbUtil.LoadObjects(keys))
                    {
                        if (item is SCUser)
                            result |= TransferObjectType.Members;
                        else if (item is SCGroup)
                            result |= TransferObjectType.Groups;
                        else if (item is SCOrganization)
                            result |= TransferObjectType.Orgnizations;
                    }
                }
            }

            return result;
        }

        /*
		internal static void ValidateAdminRole(MCS.Library.OGUPermission.IRole role)
		{
			string[] parts = role.FullCodeName.Split(':');

			if (parts.Length != 2 || parts[0].Length == 0 || parts[1].Length == 0)
				throw new FormatException("角色的全名格式有误");

			var app = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName("Applications", parts[0], DateTime.MinValue) as PC.SCApplication;

			var role1 = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName("Roles", parts[1], DateTime.MinValue) as PC.SCRole;

			if (app == null)
				throw new ManageRoleNotExistException(parts[0], parts[1]);

			if (PC.Adapters.SCMemberRelationAdapter.Instance.Load(app.ID, role1.ID) == null)
				throw new ManageRoleNotExistException(parts[0], parts[1], string.Format("存在无效的配置，指定的角色全名对应无效的对象{0}", parts[0], parts[1]));
		}
        */

        internal static ManageAclStatus GetAdminRoleStatus(BannerNotice notice)
        {
            ManageAclStatus result;

            var adminRole = ObjectSchemaSettings.GetConfig().GetAdminRole();
            if (adminRole != null)
            {
                // 检查授权
                try
                {
                    // Util.ValidateAdminRole(adminRole);
                    // 经过这步，基本确定配置和当前对象无误
                    string[] parts = adminRole.FullCodeName.Split(':');

                    string adminRoleID;
                    if (parts.Length != 2)
                    {
                        throw new FormatException("配置文件中的管理角色路径格式错误。");
                    }

                    try
                    {
                        adminRoleID = adminRole.ID; // 有可能抛异常
                    }
                    catch (Exception ex)
                    {
                        throw new ManageRoleNotExistException(parts[0], parts[1], ex);
                    }

                    if (Util.IsRoleEmpty(adminRoleID))
                    {
                        notice.Text = string.Format("管理角色{0}尚无任何人员，权限已完全开放，请联系管理员立即处理此安全风险。", adminRole.FullCodeName);
                        notice.RenderType = WebControls.NoticeType.Warning;
                        result = ManageAclStatus.NobodyIn;
                    }
                    else
                    {
                        result = ManageAclStatus.Ready;
                    }
                }
                catch (ManageRoleNotExistException m)
                {
                    notice.Text = m.Message + " 请联系管理员";
                    notice.RenderType = WebControls.NoticeType.Error;
                    result = ManageAclStatus.RoleNotExists;
                }
            }
            else
            {
                notice.RenderType = WebControls.NoticeType.Error;
                notice.Text = "系统尚未配置管理应用和角色，授权管理操作对所有用户开放，为了防范安全风险请联系管理员立即修改配置并重启服务。";
                result = ManageAclStatus.NoConfig;
            }

            return result;
        }

        internal static void InitSecurityContext(WebControls.BannerNotice notice)
        {
            if (TimePointContext.Current.UseCurrentTime)
            {
                // switch (MCS.Library.PCPassport.Principal.PCPrincipal.PCCurrent.KeyRoleStatus)
                // {
                //    case MCS.Library.PCPassport.Principal.SecurityStatus.Ready:
                //        break;
                //    case MCS.Library.PCPassport.Principal.SecurityStatus.ConfigNotReady:
                //        notice.Text = "尚未配置管理应用或管理角色，操作将不受ACL控制，请联系管理员处理此安全风险。";
                //        break;
                //    case MCS.Library.PCPassport.Principal.SecurityStatus.KeyRoleEmpty:
                //        notice.Text = "管理角色中无任何人员，操作将不受ACL控制，请联系管理员处理此安全风险。";
                //        break;
                //    default:
                //        break;
                // } 
            }
        }

        internal static bool ContainsPermission(PC.Permissions.SCContainerAndPermissionCollection permissions, string containerID, string permission)
        {
            if (permissions == null || string.IsNullOrEmpty(containerID))
                return false;
            else
                return permissions.ContainsKey(containerID, permission);
        }

        internal static UserCustomSearchCondition NewSearchCondition(string resourceKey, string conditionType, string conditionName)
        {
            UserCustomSearchCondition condition = new UserCustomSearchCondition()
            {
                ID = Guid.NewGuid().ToString(),
                UserID = Util.CurrentUser.ID,
                ResourceID = resourceKey,
                ConditionName = conditionName,
                ConditiontType = conditionType,
                CreateTime = DateTime.Now
            };

            return condition;
        }

        internal static void SaveSearchCondition(MCS.Web.WebControls.SearchEventArgs e, MCS.Web.WebControls.DeluxeSearch control, string pageKey, object dataToSave)
        {
            if (e.IsSaveCondition && string.IsNullOrEmpty(e.ConditionName) == false)
            {
                UserCustomSearchCondition condition = Util.NewSearchCondition(pageKey, "Default", e.ConditionName);

                condition.ConditionContent = JSONSerializerExecute.Serialize(dataToSave);

                UserCustomSearchConditionAdapter.Instance.Update(condition);

                control.UserCustomSearchConditions = DbUtil.LoadSearchCondition(pageKey, "Default");
            }
        }

        internal static void UpdateSearchTip(MCS.Web.WebControls.DeluxeSearch deluxeSearch)
        {
        }

        public static DateTime FromJavascriptTime(long point)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddTicks(point * 10000).ToLocalTime(); // JavaScript返回UTC时刻从1970年1月1日 0点开始到目前为止的毫秒数
            return dt;
        }

        public static System.Collections.Specialized.HybridDictionary GetSchemaCatgoryDictionary()
        {
            System.Collections.Specialized.HybridDictionary dic = null;

            if (HttpContext.Current.Items.Contains(Util.TheDicKey) == false)
            {
                dic = new System.Collections.Specialized.HybridDictionary();

                foreach (ObjectSchemaConfigurationElement schema in ObjectSchemaSettings.GetConfig().Schemas)
                {
                    dic.Add(schema.Name, schema.Category);
                }

                HttpContext.Current.Items.Add(Util.TheDicKey, dic);
            }
            else
            {
                dic = (System.Collections.Specialized.HybridDictionary)HttpContext.Current.Items[Util.TheDicKey];
            }

            return dic;
        }

        public static bool IsNullOrDeleted(SchemaObjectBase obj)
        {
            return obj == null || obj.Status != SchemaObjectStatus.Normal;
        }

        public static string AutoName(string args, params string[] args1)
        {
            if (string.IsNullOrEmpty(args) == false)
                return args;
            else
            {
                for (int i = 0; i < args1.Length; i++)
                {
                    if (string.IsNullOrEmpty(args1[i]) == false)
                    {
                        return args1[i];
                    }
                }
            }

            return "（空白）";
        }

        /// <summary>
        /// 交换两个Grid的状态
        /// </summary>
        /// <param name="CurrentGrid"></param>
        /// <param name="gridMain"></param>
        internal static void SwapGrid(System.Web.UI.WebControls.MultiView views, int viewIndex, MCS.Web.WebControls.DeluxeGrid src, MCS.Web.WebControls.DeluxeGrid target)
        {
            if (src != target)
            {
                target.PageIndex = src.PageIndex;

                // if (src.SortDirection != target.SortDirection || src.SortExpression != target.SortExpression)
                {
                    target.Sort(src.SortExpression, src.SortDirection);
                }

                views.ActiveViewIndex = viewIndex;

                var arr = new string[src.SelectedKeys.Count];
                src.SelectedKeys.CopyTo(arr);
                target.SelectedKeys.Clear();
                target.SelectedKeys.AddRange(arr);
            }
        }

        internal static void ConfigToggleViewButton(int viewIndex, System.Web.UI.WebControls.HyperLink lnkViewMode, System.Web.UI.HtmlControls.HtmlGenericControl lblViewMode)
        {
            if (viewIndex == 0)
            {
                lnkViewMode.CssClass = "pc-toggler-dd list-cmd shortcut";
                lblViewMode.InnerText = "常规列表";
            }
            else
            {
                lnkViewMode.CssClass = "pc-toggler-dt list-cmd shortcut";
                lblViewMode.InnerText = "精简表格";
            }
        }

        internal static InvokeWebServiceJob CreateImmediateJob(string name, string category, string url, string methodName, WfServiceOperationParameterCollection parameters)
        {
            return CreateImmediateJob(UuidHelper.NewUuidString(), name, category, url, methodName, parameters);
        }

        internal static InvokeWebServiceJob CreateImmediateJob(string jobID, string name, string category, string url, string methodName, WfServiceOperationParameterCollection parameters)
        {
            InvokeWebServiceJob job = new InvokeWebServiceJob();

            job.JobID = jobID;
            job.Category = category;

            job.Name = name;
            job.SvcOperationDefs = new WfServiceOperationDefinitionCollection();

            WfServiceAddressDefinition address = new WfServiceAddressDefinition(WfServiceRequestMethod.Post, null, url);

            WfServiceOperationDefinition serviceDef = new WfServiceOperationDefinition(address, methodName, parameters, string.Empty);

            serviceDef.TimeOut = 24 * 60 * 60 * 1000;
            job.SvcOperationDefs.Add(serviceDef);

            return job;
        }

        /// <summary>
        /// 创建立即调度的计划
        /// </summary>
        /// <returns></returns>
        internal static JobSchedule CreateImmediateSchedule()
        {
            DateTime now = DateTime.Now;
            DateTime execTime = now.AddSeconds(2);

            FixedTimeFrequency timeFrequency = new FixedTimeFrequency(new TimeSpan(execTime.Hour, execTime.Minute, execTime.Second));
            DailyJobScheduleFrequency frequency = new DailyJobScheduleFrequency(1, timeFrequency);
            JobSchedule schedule = new JobSchedule(UuidHelper.NewUuidString(), "立即执行", DateTime.Now, frequency);

            schedule.SchduleType = JobSchduleType.Temporary;
            schedule.StartTime = now.AddHours(-1);
            schedule.EndTime = now.AddDays(1);
            return schedule;
        }
    }
}