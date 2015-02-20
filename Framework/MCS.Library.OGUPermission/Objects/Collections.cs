#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	Collection.cs
// Remark	��	��Ա����Ķ��󼯺�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission.Properties;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// ��������Ա����Ķ��󼯺�
    /// </summary>
    /// <typeparam name="T">���������͡�</typeparam>
    [Serializable]
    public sealed class OguObjectCollection<T> : ReadOnlyCollection<T> where T : IOguObject
    {
        /// <summary>
        /// ��ʼ�������ʵ��
        /// </summary>
        /// <param name="list"></param>
        public OguObjectCollection(IList<T> list)
            : base(list)
        {
        }
        /// <summary>
        /// ��ʼ�������ʵ����
        /// </summary>
        /// <param name="objs">��������Ա����Ķ������顣</param>
        public OguObjectCollection(params T[] objs)
            : base(new List<T>())
        {
            foreach (T obj in objs)
                Items.Add(obj);
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="index">������</param>
        /// <returns>��������ָ������ƥ��Ķ���</returns>
        public new T this[int index]
        {
            get
            {
                return base[index];
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="orderByProperty">�������ԣ�<seealso cref="OrderByPropertyType"/>��</param>
        /// <param name="direction">����ʽ��<seealso cref="SortDirectionType"/>��</param>
        public void Sort(OrderByPropertyType orderByProperty, SortDirectionType direction)
        {
            T[] objArray = new T[this.Count];

            Items.CopyTo(objArray, 0);

            Array.Sort(objArray, delegate(T x, T y)
            {
                int result = 0;

                switch (orderByProperty)
                {
                    case OrderByPropertyType.FullPath:
                        result = x.FullPath.CompareTo(y.FullPath);
                        break;
                    case OrderByPropertyType.GlobalSortID:
                        result = x.GlobalSortID.CompareTo(y.GlobalSortID);
                        break;
                    case OrderByPropertyType.Name:
                        result = x.Name.CompareTo(y.Name);
                        break;
                }

                if (direction == SortDirectionType.Descending)
                    result = -result;

                return result;
            });

            Items.Clear();
            for (int i = 0; i < objArray.Length; i++)
                Items.Add(objArray[i]);
        }

        /// <summary>
        /// ���ݻ���������Ա�ļ�ְ��Ϣ�����ͬһ�����к��ж����Ա��Ϣ����ô���ݴ���Ļ�������ɸѡ�������ʣ�¶�����¼��������ְ���е�����
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public OguObjectCollection<IUser> FilterUniqueSidelineUsers(IOrganization parent)
        {
            Dictionary<string, List<T>> groupedData = GroupOguObjects(this);

            MakeUniqueUserInGroup(groupedData, parent);

            List<IUser> resultList = new List<IUser>();

            foreach (KeyValuePair<string, List<T>> kp in groupedData)
            {
                foreach (T obj in kp.Value)
                {
                    if (obj is IUser)
                        resultList.Add((IUser)obj);
                }
            }

            return new OguObjectCollection<IUser>(resultList);
        }

        /// <summary>
        /// ���Ҷ���
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T Find(Predicate<T> match)
        {
            T result = default(T);

            if (match != null)
            {
                foreach (T obj in this)
                {
                    if (match(obj))
                    {
                        result = obj;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// �����ж����IDת����ID�ļ���
        /// </summary>
        /// <returns></returns>
        public IList<string> ToIDList()
        {
            List<string> result = new List<string>();

            foreach (T obj in this)
            {
                result.Add(obj.ID);
            }

            return result;
        }

        private static void MakeUniqueUserInGroup(Dictionary<string, List<T>> groupedData, IOrganization parent)
        {
            foreach (KeyValuePair<string, List<T>> kp in groupedData)
            {
                List<T> list = kp.Value;

                if (list.Count > 1)
                {
                    List<T> filterdObjs = list.FindAll(obj => parent == null || obj.IsChildrenOf(parent));

                    if (filterdObjs.Count == 0)
                        filterdObjs = list.FindAll(obj => (obj is IUser) && ((IUser)obj).IsSideline == false);

                    if (filterdObjs.Count == 0)
                        filterdObjs.Add(list[0]);

                    list.Clear();
                    filterdObjs.ForEach(obj => list.Add(obj));
                }
            }
        }

        private static Dictionary<string, List<TObj>> GroupOguObjects<TObj>(OguObjectCollection<TObj> objs) where TObj : IOguObject
        {
            Dictionary<string, List<TObj>> result = new Dictionary<string, List<TObj>>(StringComparer.OrdinalIgnoreCase);

            foreach (TObj obj in objs)
            {
                List<TObj> list = null;

                if (result.TryGetValue(obj.ID, out list) == false)
                {
                    list = new List<TObj>();
                    result.Add(obj.ID, list);
                }

                list.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// �õ�IDΨһ�ļ�¼���ϡ�ͬһ�����������һ�Ρ�������ֵ�ǰ�ĺ���ɾ���Ĳ��棬��������ǰ�ġ����ֻ��
        /// �Ѿ�ɾ���ģ�������ɾ���ġ�
        /// </summary>
        /// <returns></returns>
        internal OguObjectCollection<T> GetRemovedDuplicateDeletedObjectCollection()
        {
            OguObjectCollection<T> result = new OguObjectCollection<T>();

            foreach (T obj in Items)
            {
                if ((int)obj.Properties["STATUS"] == 3 &&
                    ExistsNotDeletedID(obj.ID, Items))
                {
                    continue;
                }

                result.Items.Add(obj);
            }

            return result;
        }

        private static bool ExistsNotDeletedID(string id, IList<T> list)
        {
            bool result = false;

            foreach (T obj in list)
            {
                if ((int)obj.Properties["STATUS"] != 3 && string.Compare(obj.ID, id, true) == 0)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Ȩ�޼��ϡ�
    /// </summary>
    /// <typeparam name="T">���������͡�</typeparam>
    [Serializable]
    public class PermissionObjectCollection<T> : ReadOnlyCollection<T> where T : IPermissionObject
    {
        /// <summary>
        /// �ڲ��ֵ䡣
        /// </summary>
        protected Dictionary<string, T> innerDict = null;

        internal PermissionObjectCollection(IList<T> list)
            : base(list)
        {
            InitDictionary();
        }

        /// <summary>
        /// ��ʼ�������ʵ����
        /// </summary>
        /// <param name="objs">ָ����Ȩ�޶������顣</param>
        public PermissionObjectCollection(params T[] objs)
            : base(new List<T>())
        {
            foreach (T obj in objs)
                Items.Add(obj);

            InitDictionary();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="codeName">�������ơ�</param>
        /// <returns>ָ��������Ӧ��Ȩ�޶���</returns>
        public virtual T this[string codeName]
        {
            get
            {
                return innerDict[codeName];
            }
        }

        /// <summary>
        /// �ж��Ƿ���ָ���Ĵ��롣
        /// </summary>
        /// <param name="codeName">ָ���Ĵ���</param>
        /// <returns>����ָ���Ĵ��룬���� True�����򣬷��� False��</returns>
        public bool ContainsKey(string codeName)
        {
            return innerDict.ContainsKey(codeName);
        }

        /// <summary>
        /// ���Ի�ȡָ�������Ȩ�ޡ�
        /// </summary>
        /// <param name="codeName">ָ���Ĵ���</param>
        /// <param name="app">Ȩ�޶���</param>
        /// <returns>�Ƿ�ɹ���ȡ��</returns>
        public bool TryGetValue(string codeName, out T app)
        {
            return innerDict.TryGetValue(codeName, out app);
        }

        /// <summary>
        /// ��������ÿһ��Ԫ��
        /// </summary>
        /// <param name="action"></param>
        public virtual void ForEach(Action<T> action)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

            foreach (T item in this)
                action(item);
        }

        /// <summary>
        /// �жϼ������Ƿ����ĳԪ��
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual bool Exists(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            bool result = false;

            foreach (T item in this)
            {
                if (match(item))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// �жϼ�����ÿ��Ԫ���Ƿ�����ĳ����
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual bool TrueForAll(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            bool result = true;

            foreach (T item in this)
            {
                if (match(item) == false)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// �ڼ����в�������ƥ�������ĵ�һ��Ԫ��
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual T Find(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            T result = default(T);

            foreach (T item in this)
            {
                if (match(item))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// �Ӻ���ǰ���ң��ҵ���һ��ƥ���Ԫ��
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual T FindLast(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            T result = default(T);

            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (match((T)this[i]))
                {
                    result = (T)this[i];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// �ҵ�����ƥ������������Ԫ��
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual IList<T> FindAll(Predicate<T> match)
        {
            IList<T> result = new List<T>();

            foreach (T item in this)
            {
                if (match(item))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// ���ݼ��ϵ����ݳ�ʼ��CodeName���ֵ�
        /// </summary>
        protected void InitDictionary()
        {
            this.innerDict = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

            foreach (T obj in this)
            {
                if (innerDict.ContainsKey(obj.CodeName) == false)
                    innerDict.Add(obj.CodeName, obj);
            }
        }
    }

    /// <summary>
    /// Ӧ�õļ��ϡ�
    /// </summary>
    /// <typeparam name="T">Ӧ�õ����͡�</typeparam>
    public class ApplicationCollection<T> : PermissionObjectCollection<T> where T : IApplication
    {
        internal ApplicationCollection(IList<T> list)
            : base(list)
        {
        }
    }

    /// <summary>
    /// Ӧ�õļ���
    /// </summary>
    [Serializable]
    public class ApplicationCollection : ApplicationCollection<IApplication>
    {
        internal ApplicationCollection(IList<IApplication> list)
            : base(list)
        {
        }

        /// <summary>
        /// ��ʼ�� <see cref="ApplicationCollection"/> �����ʵ����
        /// </summary>
        /// <param name="objs"><see cref="IApplication"/>����</param>
        public ApplicationCollection(params IApplication[] objs)
            : base(objs)
        {
        }
    }

    /// <summary>
    /// �û��Ľ�ɫ��Ȩ����Ϣ�Ļ���
    /// </summary>
    [Serializable]
    public abstract class UserPermissionObjectCollectionBase
    {
        private IUser user = null;
        private Dictionary<string, IApplication> applicationDict = new Dictionary<string, IApplication>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="user"><see cref="IUser"/>����</param>
        protected UserPermissionObjectCollectionBase(IUser user)
        {
            this.user = user;
        }

        /// <summary>
        /// �û�����
        /// </summary>
        protected IUser User
        {
            get
            {
                return this.user;
            }
        }

        /// <summary>
        /// ����codeName�õ�Ӧ�ö���
        /// </summary>
        /// <param name="codeName">Ӧ�õ�codeName</param>
        /// <returns>Ӧ�ö���</returns>
        protected IApplication GetApplication(string codeName)
        {
            return GetApplication(codeName, true);
        }

        /// <summary>
        /// ����codeName�õ�Ӧ�ö���
        /// </summary>
        /// <param name="codeName">Ӧ�õ�codeName</param>
        /// <param name="throwNotExistsApp">������Appʱ���Ƿ��׳��쳣</param>
        /// <returns>Ӧ�ö���</returns>
        protected IApplication GetApplication(string codeName, bool throwNotExistsApp)
        {
            IApplication app = null;

            lock (applicationDict)
            {
                if (applicationDict.TryGetValue(codeName, out app) == false)
                {
                    ApplicationCollection apps = OguPermissionSettings.GetConfig().PermissionFactory.GetApplications(codeName);

                    if (throwNotExistsApp)
                        ExceptionHelper.FalseThrow(apps.Count > 0, Resource.CanNotFindObject, codeName);

                    if (apps.Count == 0)
                        app = null;
                    else
                    {
                        app = apps[0];

                        applicationDict.Add(codeName, app);
                    }
                }
            }

            return app;
        }
    }

    /// <summary>
    /// �û��Ľ�ɫ����
    /// </summary>
    [Serializable]
    public class UserRoleCollection : UserPermissionObjectCollectionBase
    {
        private Dictionary<IApplication, RoleCollection> userAppRoles = new Dictionary<IApplication, RoleCollection>();

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="user"></param>
        public UserRoleCollection(IUser user)
            : base(user)
        {
        }

        /// <summary>
        /// ����Ӧ�õ�codeName�ͽ�ɫ��codeName�õ���ɫ����
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�codeName</param>
        /// <param name="roleCodeName">��ɫ��codeName</param>
        /// <returns>��ɫ����</returns>
        public IRole this[string appCodeName, string roleCodeName]
        {
            get
            {
                return GetUserAppRoles(GetApplication(appCodeName))[roleCodeName];
            }
        }

        /// <summary>
        /// ����Ӧ�õ�codeName�ͽ�ɫ��codeName�õ���ɫ����
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�codeName</param>
        /// <returns>��ɫ����</returns>
        public RoleCollection this[string appCodeName]
        {
            get
            {
                return GetUserAppRoles(GetApplication(appCodeName));
            }
        }

        /// <summary>
        /// �Ƿ����ĳ��Ӧ��
        /// </summary>
        /// <param name="appCodeName"></param>
        /// <returns></returns>
        public bool ContainsApp(string appCodeName)
        {
            IApplication app = GetApplication(appCodeName, false);

            return app != null;
        }

        /// <summary>
        /// �õ����е�Ӧ�ý�ɫ
        /// </summary>
        /// <returns></returns>
        public Dictionary<IApplication, RoleCollection> GetAllAppRoles()
        {
            ApplicationCollection allApps = OguPermissionSettings.GetConfig().PermissionFactory.GetAllApplications();

            lock (this.userAppRoles)
            {
                foreach (IApplication app in allApps)
                {
                    if (this.userAppRoles.ContainsKey(app) == false)
                    {
                        RoleCollection roles = OguPermissionSettings.GetConfig().PermissionObjectImpls.GetUserRoles(app, this.User);
                        this.userAppRoles.Add(app, roles);
                    }
                }
            }

            return this.userAppRoles;
        }

        /// <summary>
        /// �õ����еĽ�ɫ���������ͺ�GetAllAppRoles��һ������һ���Բ�ѯ�����н�ɫ������ͨ��Application���Ի�ȡӦ��ʱ����һ��ȡ��
        /// </summary>
        /// <returns></returns>
        public List<IRole> GetAllRoles()
        {
            return AppAdminMechanism.Instance.GetAllUserRoles(this.User);
        }

        private RoleCollection GetUserAppRoles(IApplication app)
        {
            RoleCollection roles = null;

            lock (this.userAppRoles)
            {
                if (this.userAppRoles.TryGetValue(app, out roles) == false)
                {
                    roles = OguPermissionSettings.GetConfig().PermissionObjectImpls.GetUserRoles(app, User);
                    this.userAppRoles.Add(app, roles);
                }
            }

            return roles;
        }
    }

    /// <summary>
    /// �û���Ȩ�޼���
    /// </summary>
    [Serializable]
    public class UserPermissionCollection : UserPermissionObjectCollectionBase
    {
        private Dictionary<IApplication, PermissionCollection> userAppPermissions = new Dictionary<IApplication, PermissionCollection>();

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="user"></param>
        public UserPermissionCollection(IUser user)
            : base(user)
        {
        }

        /// <summary>
        /// ����Ӧ�õ�codeName��Ȩ�޵�codeName�õ�Ȩ�޶���
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�codeName</param>
        /// <param name="permissionCodeName">Ȩ�޵�codeName</param>
        /// <returns>Ȩ�޶���</returns>
        public IPermission this[string appCodeName, string permissionCodeName]
        {
            get
            {
                return GetUserAppPermissions(GetApplication(appCodeName))[permissionCodeName];
            }
        }

        /// <summary>
        /// ����Ӧ�õ�codeName��Ȩ�޵�codeName�õ�Ȩ�޼���
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�codeName</param>
        /// <returns>Ȩ�޼���</returns>
        public PermissionCollection this[string appCodeName]
        {
            get
            {
                return GetUserAppPermissions(GetApplication(appCodeName));
            }
        }

        /// <summary>
        /// �õ�����Ӧ�õ�Ȩ�޼���
        /// </summary>
        /// <returns></returns>
        public Dictionary<IApplication, PermissionCollection> GetAllAppPermissions()
        {
            ApplicationCollection allApps = OguPermissionSettings.GetConfig().PermissionFactory.GetAllApplications();

            lock (this.userAppPermissions)
            {
                foreach (IApplication app in allApps)
                {
                    if (this.userAppPermissions.ContainsKey(app) == false)
                    {
                        PermissionCollection permissions = OguPermissionSettings.GetConfig().PermissionObjectImpls.GetUserPermissions(app, User);
                        this.userAppPermissions.Add(app, permissions);
                    }
                }
            }

            return this.userAppPermissions;
        }

        private PermissionCollection GetUserAppPermissions(IApplication app)
        {
            PermissionCollection permissions = null;

            lock (userAppPermissions)
            {
                if (userAppPermissions.TryGetValue(app, out permissions) == false)
                {
                    permissions = OguPermissionSettings.GetConfig().PermissionObjectImpls.GetUserPermissions(app, User);
                    userAppPermissions.Add(app, permissions);
                }
            }

            return permissions;
        }
    }

    /// <summary>
    /// ��ɫ�ļ���
    /// </summary>
    [Serializable]
    public class RoleCollection : PermissionObjectCollection<IRole>
    {
        internal RoleCollection(IList<IRole> list)
            : base(list)
        {
        }

        /// <summary>
        /// ��ʼ�� <see cref="RoleCollection"/> �����ʵ����
        /// </summary>
        /// <param name="objs">����� <see cref="IRole"/> �������顣</param>
        public RoleCollection(params IRole[] objs)
            : base(objs)
        {
        }

        /// <summary>
        /// ��ȡָ���Ľ�ɫ��
        /// </summary>
        /// <param name="codeNames">��������</param>
        /// <returns><see cref="RoleCollection"/>����</returns>
        public RoleCollection GetSpecifiedRoles(params string[] codeNames)
        {
            List<IRole> list = new List<IRole>();

            foreach (IRole role in this)
            {
                if (Array.Exists<string>(codeNames, delegate(string name)
                {
                    return string.Compare(role.CodeName, name) == 0;
                }))
                {
                    list.Add(role);
                }
            }

            return new RoleCollection(list);
        }
    }

    /// <summary>
    /// Ȩ�޼��ϡ�
    /// </summary>
    [Serializable]
    public class PermissionCollection : PermissionObjectCollection<IPermission>
    {
        internal PermissionCollection(IList<IPermission> list)
            : base(list)
        {
        }
        /// <summary>
        /// ��ʼ�� <see cref="PermissionCollection"/> �����ʵ����
        /// </summary>
        /// <param name="objs">����� <see cref="IPermission"/> �������顣</param>
        public PermissionCollection(params IPermission[] objs)
            : base(objs)
        {
        }

        /// <summary>
        /// ��ȡָ����Ȩ�ޡ�
        /// </summary>
        /// <param name="codeNames">�ƶ��Ĵ���</param>
        /// <returns><see cref="PermissionCollection"/>����</returns>
        public PermissionCollection GetSpecifiedPermissions(params string[] codeNames)
        {
            List<IPermission> list = new List<IPermission>();

            foreach (IPermission permission in this)
            {
                if (Array.Exists<string>(codeNames, delegate(string name)
                {
                    return string.Compare(permission.CodeName, name) == 0;
                }))
                {
                    list.Add(permission);
                }
            }

            return new PermissionCollection(list);
        }
    }
}
