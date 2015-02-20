using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.PermissionBridge.Adapters
{
    internal abstract class OguPropertyAdapterBase
    {
        // static Dictionary<Type, OguPropertyAdapterBase> adapters;

        // static OguPropertyAdapterBase()
        // {
        //    adapters = new Dictionary<Type, OguPropertyAdapterBase>();
        //    adapters.Add(typeof(IUser), UserPropertyAdapter.Instance);
        //    adapters.Add(typeof(IGroup), GroupPropertyAdapter.Instance);
        //    adapters.Add(typeof(IOrganization), OrganizationPropertyAdapter.Instance);
        //    adapters.Add(typeof(IOrganizationInRole), OrganizationInRolePropertyAdapter.Instance);
        // }

        internal static OguPropertyAdapterBase GetConverter(IOguObject obj)
        {
            //Type[] types = type.GetInterfaces();
            //return adapters[type];
            if (obj is IOrganization)
                return OrganizationPropertyAdapter.Instance;
            else if (obj is IGroup)
                return GroupPropertyAdapter.Instance;
            else if (obj is IUser)
                return UserPropertyAdapter.Instance;
            else
                return null;
        }

        protected virtual void Fill(IOguObject target, SchemaObjectBase src, SCObjectAndRelation relation, SCSimpleObjectCollection parentsInfo)
        {
            Fill(target, src, relation);

            var wrapper = target as IOguPropertyAccessible;

            if (parentsInfo != null && parentsInfo.Count > 0)
                wrapper.FullPath = parentsInfo.JoinNameToFullPath() + "\\" + wrapper.Name;
        }

        public virtual void Fill(IOguObject target, SchemaObjectBase src, SCObjectAndRelation relation)
        {
            var wrapper = target as IOguPropertyAccessible;

            (wrapper != null).FalseThrow("工厂创建的对象未实现IOguPropertyAccessible");

            wrapper.Description = src.Properties.GetValue<string>("Description", string.Empty);
            wrapper.DisplayName = src.Properties.GetValue<string>("DisplayName", string.Empty);
            wrapper.ID = src.ID;
            wrapper.Name = src.Properties.GetValue<string>("Name", string.Empty);

            wrapper.Name.IsNullOrEmpty().TrueThrow<InvalidCastException>("名称不可省略");

            if (relation != null)
            {
                wrapper.SortID = relation.InnerSort.ToString();
                wrapper.Properties["ParentID"] = relation.ParentID;
            }
        }

    }

    sealed class UserPropertyAdapter : OguPropertyAdapterBase
    {
        UserPropertyAdapter()
        {
        }

        public readonly static UserPropertyAdapter Instance = new UserPropertyAdapter();

        public override void Fill(IOguObject target, SchemaObjectBase src, SCObjectAndRelation relation)
        {
            base.Fill(target, src, relation);

            if (src.SchemaType != "Users")
                throw new ArgumentException(string.Format("SchemaType不匹配", src), "src");
            var wrapper = target as OGUPermission.IUserPropertyAccessible;
            if (wrapper == null)
                throw new InvalidCastException("工厂创建的对象未实现IUserPropertyAccessible");

            //wrapper.Levels = src.Properties.GetValue<int>("Levels", 0);

            wrapper.Attributes = src.Properties.GetValue<UserAttributesType>("CadreType", UserAttributesType.Unspecified);
            wrapper.Email = src.Properties.GetValue<string>("Mail", string.Empty);
            wrapper.LogOnName = src.Properties.GetValue<string>("CodeName", string.Empty);
            wrapper.ObjectType = SchemaType.Users;
            wrapper.Occupation = src.Properties.GetValue<string>("Occupation", string.Empty);
            wrapper.Rank = src.Properties.GetValue<UserRankType>("UserRank", UserRankType.Unspecified);
            if (relation != null)
                wrapper.IsSideline = !relation.Default;
        }
    }

    class GroupPropertyAdapter : OguPropertyAdapterBase
    {
        GroupPropertyAdapter()
        {

        }

        public readonly static GroupPropertyAdapter Instance = new GroupPropertyAdapter();

        public override void Fill(IOguObject target, SchemaObjectBase src, SCObjectAndRelation relation)
        {
            base.Fill(target, src, relation);

            if (src.SchemaType != "Groups")
                throw new ArgumentException(string.Format("SchemaType不匹配", src), "src");
            var wrapper = target as OGUPermission.IOguPropertyAccessible;

            if (wrapper == null)
                throw new InvalidCastException("工厂创建的对象未实现IOguPropertyAccessible");

            wrapper.ObjectType = SchemaType.Groups;
        }
    }

    class OrganizationPropertyAdapter : OguPropertyAdapterBase
    {
        OrganizationPropertyAdapter()
        {

        }

        public readonly static OrganizationPropertyAdapter Instance = new OrganizationPropertyAdapter();

        public override void Fill(IOguObject target, SchemaObjectBase src, SCObjectAndRelation relation)
        {
            base.Fill(target, src, relation);

            if (src.SchemaType != "Organizations")
                throw new ArgumentException(string.Format("SchemaType {0}不匹配", src), "src");
            var wrapper = target as OGUPermission.IOrganizationPropertyAccessible;
            if (wrapper == null)
                throw new InvalidCastException("工厂创建的对象未实现IOrganizationPropertyAccessible");

            wrapper.CustomsCode = src.Properties.GetValue<string>("CustomsCode", string.Empty); // 关区号
            wrapper.DepartmentClass = src.Properties.GetValue<DepartmentClassType>("DepartmentClass", DepartmentClassType.Unspecified);
            wrapper.DepartmentType = src.Properties.GetValue<DepartmentTypeDefine>("DepartmentType", DepartmentTypeDefine.Unspecified);
            //wrapper.Levels = src.Properties.GetValue<int>("Levels", 0);
            wrapper.ObjectType = SchemaType.Organizations;
            wrapper.Rank = src.Properties.GetValue<DepartmentRankType>("DepartmentRank", DepartmentRankType.None);
        }


    }

    class OrganizationInRolePropertyAdapter : OguPropertyAdapterBase
    {
        OrganizationInRolePropertyAdapter()
        {

        }

        public readonly static OrganizationInRolePropertyAdapter Instance = new OrganizationInRolePropertyAdapter();

        public override void Fill(IOguObject target, SchemaObjectBase src, SCObjectAndRelation relation)
        {
            base.Fill(target, src, relation);
        }
    }

}
