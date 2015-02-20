#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	PermissionObjBaseImpl.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
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
    /// ��Ȩϵͳ����Ļ���
    /// </summary>
    public abstract class PermissionObjBaseImpl : IPermissionObject, MCS.Library.OGUPermission.IPermissionPropertyAccessible
    {
        private string id = string.Empty;
        private string name = string.Empty;
        private string codeName = string.Empty;
        private string description = string.Empty;

        #region IPermissionObject ��Ա
        /// <summary>
        /// �����ID
        /// </summary>
        public string ID
        {
            get { return this.id; }
            internal set { this.id = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Name
        {
            get { return this.name; }
            internal set { this.name = value; }
        }

        /// <summary>
        /// �����Ӣ������
        /// </summary>
        public string CodeName
        {
            get { return this.codeName; }
            internal set { this.codeName = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Description
        {
            get { return this.description; }
            internal set { this.description = value; }
        }

        #endregion

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="row"></param>
        public virtual void InitProperties(DataRow row)
        {
            this.id = Common.GetDataRowTextValue(row, "ID");
            this.name = Common.GetDataRowTextValue(row, "NAME");
            this.codeName = Common.GetDataRowTextValue(row, "CODE_NAME");
            this.description = Common.GetDataRowTextValue(row, "DESCRIPTION");
        }

        #region ��ʽ�ӿ�ʵ��
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
    /// Ӧ����صĶ���Ļ��࣬�����ɫ��Role����Ȩ�ޣ�Permission��
    /// </summary>
    public class AppMemberPermissionObjBaseImpl : PermissionObjBaseImpl
    {
        private string appID = string.Empty;
        private IApplication application = null;

        #region Sync Objects
        private object applicationSyncObj = new object();
        #endregion

        /// <summary>
        /// Ӧ��ID
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
        /// ������Ӧ�ó���
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
        /// ��ʼ������
        /// </summary>
        /// <param name="row"></param>
        public override void InitProperties(DataRow row)
        {
            base.InitProperties(row);

            this.appID = Common.GetDataRowTextValue(row, "APP_ID");
        }
    }

    /// <summary>
    /// Ӧ�ó������ʵ��
    /// </summary>
    public class ApplicationImpl : PermissionObjBaseImpl, IApplication, IApplicationPropertyAccessible
    {
        private string resourceLevel = string.Empty;

        #region Sync Objects
        private object rolesSyncObj = new object();
        private object permissionsSyncObj = new object();
        #endregion

        /// <summary>
        /// ��ɫ����
        /// </summary>
        protected RoleCollection roles = null;

        /// <summary>
        /// ���ܣ�Ȩ�޼��ϣ�
        /// </summary>
        protected PermissionCollection permissions = null;

        /// <summary>
        /// ���췽��
        /// </summary>
        public ApplicationImpl()
        {
        }

        #region IApplication ��Ա

        /// <summary>
        /// Ӧ�õļ���
        /// </summary>
        public string ResourceLevel
        {
            get { return this.resourceLevel; }
            internal set { this.resourceLevel = value; }
        }

        /// <summary>
        /// �����Ľ�ɫ
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
        /// �����Ĺ��ܣ�Ȩ�ޣ�
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
        /// ��ʼ������
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
    /// ��ɫ�����ʵ��
    /// </summary>
    public class RoleImpl : AppMemberPermissionObjBaseImpl, IRole, IApplicationMemberPropertyAccessible
    {
        private OguObjectCollection<IOguObject> objectsInRole = null;

        #region Sync Objects
        private object objectsInRoleSyncObj = new object();
        #endregion

        /// <summary>
        /// ���췽��
        /// </summary>
        public RoleImpl()
        {
        }

        #region IApplicationObject ��Ա
        /// <summary>
        /// ȫ·����CodeName��AppCodeName\RoleCodeName��
        /// </summary>
        public string FullCodeName
        {
            get
            {
                return this.Application.CodeName + ":" + this.CodeName;
            }
        }

        /// <summary>
        /// ��ɫ�еĶ���
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
        /// ��ѯָ�����ŷ�Χ�£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�µ�������Ա
        /// </summary>
        /// <param name="orgRoot">���ŷ�Χ��ȫ·�����մ�ʱ�������ƣ����ʱ�ö��ŷָ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <param name="sidelineMask">��Աְλ����</param>
        /// <param name="extAttr">Ҫ���ȡ����չ����</param>
        /// <returns>ָ�����ŷ�Χ�£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�µ�������Ա</returns>
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
    /// Ȩ�޵�ʵ����
    /// </summary>
    public class PermissionImpl : AppMemberPermissionObjBaseImpl, IPermission, IApplicationMemberPropertyAccessible
    {
        private RoleCollection relativeRoles = null;

        #region Sync Objects
        private object relativeRolesSyncObj = new object();
        #endregion

        /// <summary>
        /// ���췽��
        /// </summary>
        public PermissionImpl()
        {
        }

        #region IApplicationObject ��Ա

        /// <summary>
        /// ȫ·����CodeName��AppCodeName\PermissionCodeName��
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
        /// �õ���صĽ�ɫ
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
        /// �õ���صĽ�ɫ
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
    /// ȱʡ��OguObjectFactory
    /// </summary>
    internal class DefaultPermissionObjectFactory : IPermissionObjectFactory
    {
        public static readonly IPermissionObjectFactory Instance = new DefaultPermissionObjectFactory();

        private DefaultPermissionObjectFactory()
        {
        }

        #region IPermissionObjectFactory Members

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="type">��Ȩ����Ľӿ�����</param>
        /// <returns>��Ȩ����</returns>
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
