#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	Collection.cs
// Remark	：	人员和组的对象集合
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
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
    /// 机构、人员和组的对象集合
    /// </summary>
    /// <typeparam name="T">期望的类型。</typeparam>
    [Serializable]
    public sealed class OguObjectCollection<T> : ReadOnlyCollection<T> where T : IOguObject
    {
        /// <summary>
        /// 初始化类的新实例
        /// </summary>
        /// <param name="list"></param>
        public OguObjectCollection(IList<T> list)
            : base(list)
        {
        }
        /// <summary>
        /// 初始化类的新实例。
        /// </summary>
        /// <param name="objs">机构、人员和组的对象数组。</param>
        public OguObjectCollection(params T[] objs)
            : base(new List<T>())
        {
            foreach (T obj in objs)
                Items.Add(obj);
        }
        /// <summary>
        /// 索引器。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns>集合中与指定索引匹配的对象。</returns>
        public new T this[int index]
        {
            get
            {
                return base[index];
            }
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderByProperty">排序属性，<seealso cref="OrderByPropertyType"/>。</param>
        /// <param name="direction">排序方式，<seealso cref="SortDirectionType"/>。</param>
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
        /// 根据机构过滤人员的兼职信息。如果同一集合中含有多个人员信息，那么根据传入的机构进行筛选，如果还剩下多条记录，根据主职进行调整。
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
        /// 查找对象
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
        /// 将所有对象的ID转换成ID的集合
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
        /// 得到ID唯一的记录集合。同一个对象仅出现一次。如果出现当前的和已删除的并存，仅保留当前的。如果只有
        /// 已经删除的，则保留已删除的。
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
    /// 权限集合。
    /// </summary>
    /// <typeparam name="T">期望的类型。</typeparam>
    [Serializable]
    public class PermissionObjectCollection<T> : ReadOnlyCollection<T> where T : IPermissionObject
    {
        /// <summary>
        /// 内部字典。
        /// </summary>
        protected Dictionary<string, T> innerDict = null;

        internal PermissionObjectCollection(IList<T> list)
            : base(list)
        {
            InitDictionary();
        }

        /// <summary>
        /// 初始化类的新实例。
        /// </summary>
        /// <param name="objs">指定的权限对象数组。</param>
        public PermissionObjectCollection(params T[] objs)
            : base(new List<T>())
        {
            foreach (T obj in objs)
                Items.Add(obj);

            InitDictionary();
        }

        /// <summary>
        /// 索引器。
        /// </summary>
        /// <param name="codeName">索引名称。</param>
        /// <returns>指定索引对应的权限对象。</returns>
        public virtual T this[string codeName]
        {
            get
            {
                return innerDict[codeName];
            }
        }

        /// <summary>
        /// 判断是否含有指定的代码。
        /// </summary>
        /// <param name="codeName">指定的代码</param>
        /// <returns>含有指定的代码，返回 True；否则，返回 False。</returns>
        public bool ContainsKey(string codeName)
        {
            return innerDict.ContainsKey(codeName);
        }

        /// <summary>
        /// 尝试获取指定代码的权限。
        /// </summary>
        /// <param name="codeName">指定的代码</param>
        /// <param name="app">权限对象</param>
        /// <returns>是否成功获取。</returns>
        public bool TryGetValue(string codeName, out T app)
        {
            return innerDict.TryGetValue(codeName, out app);
        }

        /// <summary>
        /// 迭代处理每一个元素
        /// </summary>
        /// <param name="action"></param>
        public virtual void ForEach(Action<T> action)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

            foreach (T item in this)
                action(item);
        }

        /// <summary>
        /// 判断集合中是否存在某元素
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
        /// 判断集合中每个元素是否都满足某条件
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
        /// 在集合中查找满足匹配条件的第一个元素
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
        /// 从后向前查找，找到第一个匹配的元素
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
        /// 找到满足匹配条件的所有元素
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
        /// 根据集合的内容初始化CodeName的字典
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
    /// 应用的集合。
    /// </summary>
    /// <typeparam name="T">应用的类型。</typeparam>
    public class ApplicationCollection<T> : PermissionObjectCollection<T> where T : IApplication
    {
        internal ApplicationCollection(IList<T> list)
            : base(list)
        {
        }
    }

    /// <summary>
    /// 应用的集合
    /// </summary>
    [Serializable]
    public class ApplicationCollection : ApplicationCollection<IApplication>
    {
        internal ApplicationCollection(IList<IApplication> list)
            : base(list)
        {
        }

        /// <summary>
        /// 初始化 <see cref="ApplicationCollection"/> 类的新实例。
        /// </summary>
        /// <param name="objs"><see cref="IApplication"/>对象。</param>
        public ApplicationCollection(params IApplication[] objs)
            : base(objs)
        {
        }
    }

    /// <summary>
    /// 用户的角色和权限信息的基类
    /// </summary>
    [Serializable]
    public abstract class UserPermissionObjectCollectionBase
    {
        private IUser user = null;
        private Dictionary<string, IApplication> applicationDict = new Dictionary<string, IApplication>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="user"><see cref="IUser"/>对象。</param>
        protected UserPermissionObjectCollectionBase(IUser user)
        {
            this.user = user;
        }

        /// <summary>
        /// 用户对象
        /// </summary>
        protected IUser User
        {
            get
            {
                return this.user;
            }
        }

        /// <summary>
        /// 根据codeName得到应用对象
        /// </summary>
        /// <param name="codeName">应用的codeName</param>
        /// <returns>应用对象</returns>
        protected IApplication GetApplication(string codeName)
        {
            return GetApplication(codeName, true);
        }

        /// <summary>
        /// 根据codeName得到应用对象
        /// </summary>
        /// <param name="codeName">应用的codeName</param>
        /// <param name="throwNotExistsApp">不存在App时，是否抛出异常</param>
        /// <returns>应用对象</returns>
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
    /// 用户的角色集合
    /// </summary>
    [Serializable]
    public class UserRoleCollection : UserPermissionObjectCollectionBase
    {
        private Dictionary<IApplication, RoleCollection> userAppRoles = new Dictionary<IApplication, RoleCollection>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="user"></param>
        public UserRoleCollection(IUser user)
            : base(user)
        {
        }

        /// <summary>
        /// 根据应用的codeName和角色的codeName得到角色对象
        /// </summary>
        /// <param name="appCodeName">应用的codeName</param>
        /// <param name="roleCodeName">角色的codeName</param>
        /// <returns>角色对象</returns>
        public IRole this[string appCodeName, string roleCodeName]
        {
            get
            {
                return GetUserAppRoles(GetApplication(appCodeName))[roleCodeName];
            }
        }

        /// <summary>
        /// 根据应用的codeName和角色的codeName得到角色集合
        /// </summary>
        /// <param name="appCodeName">应用的codeName</param>
        /// <returns>角色集合</returns>
        public RoleCollection this[string appCodeName]
        {
            get
            {
                return GetUserAppRoles(GetApplication(appCodeName));
            }
        }

        /// <summary>
        /// 是否包含某个应用
        /// </summary>
        /// <param name="appCodeName"></param>
        /// <returns></returns>
        public bool ContainsApp(string appCodeName)
        {
            IApplication app = GetApplication(appCodeName, false);

            return app != null;
        }

        /// <summary>
        /// 得到所有的应用角色
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
        /// 得到所有的角色。返回类型和GetAllAppRoles不一样。是一次性查询出所有角色，但是通过Application属性获取应用时，逐一获取。
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
    /// 用户的权限集合
    /// </summary>
    [Serializable]
    public class UserPermissionCollection : UserPermissionObjectCollectionBase
    {
        private Dictionary<IApplication, PermissionCollection> userAppPermissions = new Dictionary<IApplication, PermissionCollection>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="user"></param>
        public UserPermissionCollection(IUser user)
            : base(user)
        {
        }

        /// <summary>
        /// 根据应用的codeName和权限的codeName得到权限对象
        /// </summary>
        /// <param name="appCodeName">应用的codeName</param>
        /// <param name="permissionCodeName">权限的codeName</param>
        /// <returns>权限对象</returns>
        public IPermission this[string appCodeName, string permissionCodeName]
        {
            get
            {
                return GetUserAppPermissions(GetApplication(appCodeName))[permissionCodeName];
            }
        }

        /// <summary>
        /// 根据应用的codeName和权限的codeName得到权限集合
        /// </summary>
        /// <param name="appCodeName">应用的codeName</param>
        /// <returns>权限集合</returns>
        public PermissionCollection this[string appCodeName]
        {
            get
            {
                return GetUserAppPermissions(GetApplication(appCodeName));
            }
        }

        /// <summary>
        /// 得到所有应用的权限集合
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
    /// 角色的集合
    /// </summary>
    [Serializable]
    public class RoleCollection : PermissionObjectCollection<IRole>
    {
        internal RoleCollection(IList<IRole> list)
            : base(list)
        {
        }

        /// <summary>
        /// 初始化 <see cref="RoleCollection"/> 类的新实例。
        /// </summary>
        /// <param name="objs">传入的 <see cref="IRole"/> 对象数组。</param>
        public RoleCollection(params IRole[] objs)
            : base(objs)
        {
        }

        /// <summary>
        /// 获取指定的角色。
        /// </summary>
        /// <param name="codeNames">代码名称</param>
        /// <returns><see cref="RoleCollection"/>对象。</returns>
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
    /// 权限集合。
    /// </summary>
    [Serializable]
    public class PermissionCollection : PermissionObjectCollection<IPermission>
    {
        internal PermissionCollection(IList<IPermission> list)
            : base(list)
        {
        }
        /// <summary>
        /// 初始化 <see cref="PermissionCollection"/> 类的新实例。
        /// </summary>
        /// <param name="objs">传入的 <see cref="IPermission"/> 对象数组。</param>
        public PermissionCollection(params IPermission[] objs)
            : base(objs)
        {
        }

        /// <summary>
        /// 获取指定的权限。
        /// </summary>
        /// <param name="codeNames">制定的代码</param>
        /// <returns><see cref="PermissionCollection"/>对象。</returns>
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
