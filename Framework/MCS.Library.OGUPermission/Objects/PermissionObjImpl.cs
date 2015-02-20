#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	PermissionObjBaseImpl.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission.Properties;

namespace MCS.Library.OGUPermission
{
    #region PermissionObjBaseImpl
    /// <summary>
    /// 授权系统对象的基类
    /// </summary>
    public abstract class PermissionObjBaseImpl : IPermissionObject, MCS.Library.OGUPermission.IPermissionPropertyAccessible
    {
        private string id = string.Empty;
        private string name = string.Empty;
        private string codeName = string.Empty;
        private string description = string.Empty;

        #region IPermissionObject 成员
        /// <summary>
        /// 对象的ID
        /// </summary>
        public string ID
        {
            get { return this.id; }
            internal set { this.id = value; }
        }

        /// <summary>
        /// 对象的名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            internal set { this.name = value; }
        }

        /// <summary>
        /// 对象的英文名称
        /// </summary>
        public string CodeName
        {
            get { return this.codeName; }
            internal set { this.codeName = value; }
        }

        /// <summary>
        /// 对象的描述
        /// </summary>
        public string Description
        {
            get { return this.description; }
            internal set { this.description = value; }
        }

        #endregion

        /// <summary>
        /// 初始化属性
        /// </summary>
        /// <param name="row"></param>
        public virtual void InitProperties(DataRow row)
        {
            this.id = Common.GetDataRowTextValue(row, "ID");
            this.name = Common.GetDataRowTextValue(row, "NAME");
            this.codeName = Common.GetDataRowTextValue(row, "CODE_NAME");
            this.description = Common.GetDataRowTextValue(row, "DESCRIPTION");
        }

        #region 显式接口实现
        string IPermissionPropertyAccessible.CodeName
        {
            get
            {
                return this.CodeName;
            }
            set
            {
                this.CodeName = value;
            }
        }

        string IPermissionPropertyAccessible.Description
        {
            get
            {
                return this.Description;
            }
            set
            {
                this.Description = value;
            }
        }

        string IPermissionPropertyAccessible.ID
        {
            get
            {
                return this.ID;
            }
            set
            {
                this.ID = value;
            }
        }

        string IPermissionPropertyAccessible.Name
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }
        #endregion
    }
    #endregion PermissionObjBaseImpl

    /// <summary>
    /// 应用相关的对象的基类，例如角色（Role）和权限（Permission）
    /// </summary>
    public class AppMemberPermissionObjBaseImpl : PermissionObjBaseImpl
    {
        private string appID = string.Empty;
        private IApplication application = null;

        #region Sync Objects
        private object applicationSyncObj = new object();
        #endregion

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID
        {
            get
            {
                return this.appID;
            }
            internal set
            {
                this.appID = value;
            }
        }

        /// <summary>
        /// 所属的应用程序
        /// </summary>
        public IApplication Application
        {
            get
            {
                if (this.application == null)
                {
                    lock (this.applicationSyncObj)
                    {
                        if (this.application == null)
                        {
                            ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetAllApplications();

                            IApplication app = apps.Find(a => a.ID == this.AppID);

                            this.application = app;
                        }
                    }
                }

                return this.application;
            }
            internal set
            {
                this.application = value;
            }
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        /// <param name="row"></param>
        public override void InitProperties(DataRow row)
        {
            base.InitProperties(row);

            this.appID = Common.GetDataRowTextValue(row, "APP_ID");
        }
    }

    /// <summary>
    /// 应用程序类的实现
    /// </summary>
    public class ApplicationImpl : PermissionObjBaseImpl, IApplication, IApplicationPropertyAccessible
    {
        private string resourceLevel = string.Empty;

        #region Sync Objects
        private object rolesSyncObj = new object();
        private object permissionsSyncObj = new object();
        #endregion

        /// <summary>
        /// 角色集合
        /// </summary>
        protected RoleCollection roles = null;

        /// <summary>
        /// 功能（权限集合）
        /// </summary>
        protected PermissionCollection permissions = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ApplicationImpl()
        {
        }

        #region IApplication 成员

        /// <summary>
        /// 应用的级别
        /// </summary>
        public string ResourceLevel
        {
            get { return this.resourceLevel; }
            internal set { this.resourceLevel = value; }
        }

        /// <summary>
        /// 包含的角色
        /// </summary>
        public virtual RoleCollection Roles
        {
            get
            {
                if (this.roles == null)
                {
                    lock (this.rolesSyncObj)
                    {
                        if (this.roles == null)
                            this.roles = OguPermissionSettings.GetConfig().PermissionObjectImpls.GetRoles(this);
                    }
                }

                return this.roles;
            }
        }

        /// <summary>
        /// 包含的功能（权限）
        /// </summary>
        public virtual PermissionCollection Permissions
        {
            get
            {
                if (this.permissions == null)
                {
                    lock (this.permissionsSyncObj)
                    {
                        if (this.permissions == null)
                            this.permissions = OguPermissionSettings.GetConfig().PermissionObjectImpls.GetPermissions(this);
                    }
                }

                return this.permissions;
            }
        }
        #endregion

        /// <summary>
        /// 初始化属性
        /// </summary>
        /// <param name="row"></param>
        public override void InitProperties(DataRow row)
        {
            base.InitProperties(row);

            this.resourceLevel = Common.GetDataRowTextValue(row, "RESOURCE_LEVEL");
        }

        string IApplicationPropertyAccessible.ResourceLevel
        {
            get
            {
                return this.ResourceLevel;
            }
            set
            {
                this.ResourceLevel = value;
            }
        }
    }

    /// <summary>
    /// 角色对象的实现
    /// </summary>
    public class RoleImpl : AppMemberPermissionObjBaseImpl, IRole, IApplicationMemberPropertyAccessible
    {
        private OguObjectCollection<IOguObject> objectsInRole = null;

        #region Sync Objects
        private object objectsInRoleSyncObj = new object();
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public RoleImpl()
        {
        }

        #region IApplicationObject 成员
        /// <summary>
        /// 全路径的CodeName（AppCodeName\RoleCodeName）
        /// </summary>
        public string FullCodeName
        {
            get
            {
                return this.Application.CodeName + ":" + this.CodeName;
            }
        }

        /// <summary>
        /// 角色中的对象
        /// </summary>
        public OguObjectCollection<IOguObject> ObjectsInRole
        {
            get
            {
                if (this.objectsInRole == null)
                {
                    lock (this.objectsInRoleSyncObj)
                    {
                        if (this.objectsInRole == null)
                            this.objectsInRole = PermissionMechanismFactory.GetMechanism().GetRolesObjects(
                                                    new RoleCollection(this),
                                                    new OguObjectCollection<IOrganization>(OguMechanismFactory.GetMechanism().GetRoot()),
                                                    false);
                    }
                }

                return this.objectsInRole;
            }
        }

        /// <summary>
        /// 查询指定部门范围下，指定应用系统中，指定角色下的所有人员
        /// </summary>
        /// <param name="orgRoot">部门范围的全路径，空串时不做限制，多个时用逗号分隔</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <param name="sidelineMask">人员职位类型</param>
        /// <param name="extAttr">要求获取的扩展属性</param>
        /// <returns>指定部门范围下，指定应用系统中，指定角色下的所有人员</returns>
        public static OguObjectCollection<IUser> GetUsersFromRoles(string orgRoot, string appCodeName, string roleCodeNames,
            DelegationMaskType delegationMask, SidelineMaskType sidelineMask, string extAttr)
        {
            DataSet ds = AppAdminServiceBroker.Instance.GetRolesUsers(orgRoot, appCodeName, roleCodeNames,
                    delegationMask, SidelineMaskType.All, extAttr);

            var users = Common.BuildObjectsFromTable<IUser>(ds.Tables[0]);
            return new OguObjectCollection<IUser>(users);
        }
        #endregion

        IApplication IApplicationMemberPropertyAccessible.Application
        {
            get
            {
                return this.Application;
            }
            set
            {
                this.Application = value;
            }
        }
    }

    /// <summary>
    /// 权限的实现类
    /// </summary>
    public class PermissionImpl : AppMemberPermissionObjBaseImpl, IPermission, IApplicationMemberPropertyAccessible
    {
        private RoleCollection relativeRoles = null;

        #region Sync Objects
        private object relativeRolesSyncObj = new object();
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public PermissionImpl()
        {
        }

        #region IApplicationObject 成员

        /// <summary>
        /// 全路径的CodeName（AppCodeName\PermissionCodeName）
        /// </summary>
        public string FullCodeName
        {
            get
            {
                return this.Application.CodeName + ":" + this.CodeName;
            }
        }
        #endregion

        IApplication IApplicationMemberPropertyAccessible.Application
        {
            get
            {
                return this.Application;
            }
            set
            {
                this.Application = value;
            }
        }

        /// <summary>
        /// 得到相关的角色
        /// </summary>
        public RoleCollection RelativeRoles
        {
            get
            {
                if (this.relativeRoles == null)
                {
                    lock (this.relativeRolesSyncObj)
                    {
                        if (this.relativeRoles == null)
                            this.relativeRoles = GetRelativeRoles();
                    }
                }

                return this.relativeRoles;
            }
        }

        /// <summary>
        /// 得到相关的角色
        /// </summary>
        /// <returns></returns>
        private RoleCollection GetRelativeRoles()
        {
            DataTable table = AppAdminServiceBroker.Instance.GetFunctionsRoles(this.Application.CodeName, this.CodeName).Tables[0];

            List<IRole> relativeRoleList = new List<IRole>();

            foreach (DataRow row in table.Rows)
            {
                IRole role = null;

                if (this.Application.Roles.TryGetValue(row["CODE_NAME"].ToString(), out role))
                    relativeRoleList.Add(role);
            }

            return new RoleCollection(relativeRoleList);
        }
    }

    /// <summary>
    /// 缺省的OguObjectFactory
    /// </summary>
    internal class DefaultPermissionObjectFactory : IPermissionObjectFactory
    {
        public static readonly IPermissionObjectFactory Instance = new DefaultPermissionObjectFactory();

        private DefaultPermissionObjectFactory()
        {
        }

        #region IPermissionObjectFactory Members

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="type">授权对象的接口类型</param>
        /// <returns>授权对象</returns>
        public IPermissionObject CreateObject(System.Type type)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

            PermissionObjBaseImpl result = null;

            if (type == typeof(IApplication))
                result = new ApplicationImpl();
            else
                if (type == typeof(IRole))
                    result = new RoleImpl();
                else
                    if (type == typeof(IPermission))
                        result = new PermissionImpl();
                    else
                        throw new InvalidOperationException(string.Format(Resource.InvalidObjectTypeCreation, type.ToString()));

            return result;
        }

        #endregion
    }
}
