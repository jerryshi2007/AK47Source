using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
    public class OriginaUserOUControlQueryImpl : IUserOUControlQuery
    {
        public static readonly IUserOUControlQuery Instance = new OriginaUserOUControlQueryImpl();

        public Dictionary<string, IEnumerable<IOrganization>> QueryObjectsParents(params string[] ids)
        {
            Dictionary<string, IEnumerable<IOrganization>> result = new Dictionary<string, IEnumerable<IOrganization>>();

            OguObjectCollection<IOguObject> objs = OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, ids);

            foreach (IOguObject obj in objs)
                result[obj.ID] = GetAllParents(obj);

            return result;
        }

        public UserInfoExtendCollection QueryUsersExtendedInfo(params string[] ids)
        {
            return UserInfoExtendDataObjectAdapter.Instance.GetUserInfoExtendInfoCollectionByUsers(ids);
        }

        public UserIMAddressCollection QueryUsersIMAddress(params string[] ids)
        {
            //新实现，从权限中心获取用户SIP地址
            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ids);

            UserIMAddressCollection result = new UserIMAddressCollection();

            foreach (IUser user in users)
            {
                if (result.ContainsKey(user.ID) == false)
                {
                    UserIMAddress item = new UserIMAddress(
                        user.ID,
                        DictionaryHelper.GetValue(user.Properties, "Sip", string.Empty));

                    result.Add(item);
                }
            }

            return result;
        }

        private static IEnumerable<IOrganization> GetAllParents(IOguObject obj)
        {
            List<IOrganization> orgs = new List<IOrganization>();

            IOrganization parent = obj.Parent;

            while (parent != null)
            {
                orgs.Insert(0, parent);
                parent = parent.Parent;
            }

            return orgs;
        }

        public IOrganization GetOrganizationByPath(string rootPath)
        {
            IOrganization result = null;

            if (rootPath.IsNotEmpty())
                result = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.FullPath, rootPath).FirstOrDefault();
            else
                result = OguMechanismFactory.GetMechanism().GetRoot();

            if (result == null)
                throw new ArgumentException(string.Format("不能找到路径为\"{0}\"的根组织", rootPath));

            return result;
        }

        public OguObjectCollection<IOguObject> GetObjects(params string[] ids)
        {
            return OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, ids);
        }

        public IEnumerable<IOguObject> GetChildren(IOrganization parent)
        {
            return parent.Children;
        }

        public OguObjectCollection<T> QueryDescendants<T>(SchemaQueryType type, IOrganization parent, string prefix, int maxCount) where T : IOguObject
        {
            return parent.QueryChildren<T>(prefix, true, SearchLevel.SubTree, maxCount);
        }

        /// <summary>
        /// 校验已知类型的参数规则
        /// </summary>
        /// <typeparam name="TAct">实际的类型</typeparam>
        /// <typeparam name="TExp">预期的类型</typeparam>
        /// <param name="actual">实际的类型</param>
        /// <param name="mask">位掩码</param>
        private void CheckSchema<TAct, TExp>(SchemaQueryType actual, SchemaQueryType mask)
        {
            if (((actual & mask) == mask))
            {
                if (typeof(TAct).IsAssignableFrom(typeof(TExp)) == false)
                {
                    throw new InvalidCastException(string.Format("指定的类型和转换之间存在明显不兼容，{0}与{1}不兼容", typeof(TAct).Name, actual.ToString()));
                }
            }
        }



    }
}
