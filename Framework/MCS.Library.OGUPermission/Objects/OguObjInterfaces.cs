#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	OguObjInterfaces.cs
// Remark	：	人员和部门接口的基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Passport;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// 最简单的机构人员对象
    /// </summary>
    public interface IOguSimpleObject
    {
        /// <summary>
        /// 对象的ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 对象的显示名称
        /// </summary>
        string DisplayName
        {
            get;
        }

        /// <summary>
        /// 对象在系统中的全部路径
        /// </summary>
        string FullPath
        {
            get;
        }
    }

    /// <summary>
    /// 人员和部门接口的基类
    /// </summary>
    public interface IOguObject: ITicketToken
    {
        ///// <summary>
        ///// 对象的ID
        ///// </summary>
        //string ID
        //{
        //    get;
        //}

        ///// <summary>
        ///// 对象的名称
        ///// </summary>
        //string Name
        //{
        //    get;
        //}

        ///// <summary>
        ///// 对象的显示名称
        ///// </summary>
        //string DisplayName
        //{
        //    get;
        //}

        /// <summary>
        /// 描述信息
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// 对象在系统中的全部路径
        /// </summary>
        string FullPath
        {
            get;
        }

        /// <summary>
        /// 对象的类型
        /// </summary>
        SchemaType ObjectType
        {
            get;
        }

        /// <summary>
        /// 父部门
        /// </summary>
        IOrganization Parent
        {
            get;
        }

        /// <summary>
        /// 在部门内的排序号
        /// </summary>
        string SortID
        {
            get;
        }

        /// <summary>
        /// 在整个机构中的排序号
        /// </summary>
        string GlobalSortID
        {
            get;
        }

        /// <summary>
        /// 用户的缺省顶级部门
        /// </summary>
        IOrganization TopOU
        {
            get;
        }

        /// <summary>
        /// 对象在组织机构树上的层次
        /// </summary>
        int Levels
        {
            get;
        }

        /// <summary>
        /// 判断当前对象是否是parent对象的子孙对象
        /// </summary>
        /// <param name="parent">某个机构对象</param>
        /// <returns>是否属于某机构</returns>
        bool IsChildrenOf(IOrganization parent);

        /// <summary>
        /// 属性集合
        /// </summary>
        IDictionary Properties
        {
            get;
        }
    }

    /// <summary>
    /// 用户的接口
    /// </summary>
    public interface IUser : IOguObject
    {
        /// <summary>
        /// 登录名称
        /// </summary>
        string LogOnName
        {
            get;
        }

        /// <summary>
        /// 用户的邮件地址
        /// </summary>
        string Email
        {
            get;
        }

        /// <summary>
        /// 用户的职位
        /// </summary>
        string Occupation
        {
            get;
        }

        /// <summary>
        /// 人员的级别
        /// </summary>
        UserRankType Rank
        {
            get;
        }

        /// <summary>
        /// 用户的属性
        /// </summary>
        UserAttributesType Attributes
        {
            get;
        }

        /// <summary>
        /// 用户所属的组
        /// </summary>
        OguObjectCollection<IGroup> MemberOf
        {
            get;
        }

        /// <summary>
        /// 用户是否属于某一个或几个组
        /// </summary>
        /// <param name="groups">一个或几个组</param>
        /// <returns>是否属于某一个或几个组</returns>
        bool IsInGroups(params IGroup[] groups);

        /// <summary>
        /// 是否是兼职的用户信息
        /// </summary>
        bool IsSideline
        {
            get;
        }

        /// <summary>
        /// 所有相关的，包括兼职和主职的用户信息
        /// </summary>
        OguObjectCollection<IUser> AllRelativeUserInfo
        {
            get;
        }

        /// <summary>
        /// 判断用户是否是某个机构对象的子孙对象，用户信息可以包含所有兼职信息
        /// </summary>
        /// <param name="parent">机构对象</param>
        /// <param name="includeSideline">是否判断兼职对象</param>
        /// <returns>是否属于某机构</returns>
        bool IsChildrenOf(IOrganization parent, bool includeSideline);

        /// <summary>
        /// 当前用户的秘书
        /// </summary>
        OguObjectCollection<IUser> Secretaries
        {
            get;
        }

        /// <summary>
        /// 当前用户是谁的秘书
        /// </summary>
        OguObjectCollection<IUser> SecretaryOf
        {
            get;
        }
        #region 下面是授权的接口方法
        /// <summary>
        /// 用户的角色
        /// </summary>
        UserRoleCollection Roles
        {
            get;
        }

        /// <summary>
        /// 用户的权限
        /// </summary>
        UserPermissionCollection Permissions
        {
            get;
        }
        #endregion
    }

    /// <summary>
    /// 组织机构的接口
    /// </summary>
    public interface IOrganization : IOguObject
    {
        /// <summary>
        /// 关区代码
        /// </summary>
        string CustomsCode
        {
            get;
        }
        /// <summary>
        /// 部门的类型
        /// </summary>
        DepartmentTypeDefine DepartmentType
        {
            get;
        }

        /// <summary>
        /// 部门的类别
        /// </summary>
        DepartmentClassType DepartmentClass
        {
            get;
        }

        /// <summary>
        /// 部门的级别
        /// </summary>
        DepartmentRankType Rank
        {
            get;
        }

        /// <summary>
        /// 该部门是否是顶级部门
        /// </summary>
        bool IsTopOU
        {
            get;
        }

        /// <summary>
        /// 该部门的下一级子对象
        /// </summary>
        OguObjectCollection<IOguObject> Children
        {
            get;
        }

        /// <summary>
        /// 得到所有的子对象（所有级别深度）
        /// </summary>
        /// <typeparam name="T">期望的类型</typeparam>
        /// <param name="includeSideLine">是否包含兼职的人员</param>
        /// <returns>得到所有的子对象（所有级别深度）</returns>
        OguObjectCollection<T> GetAllChildren<T>(bool includeSideLine) where T : IOguObject;

        /// <summary>
        /// 在子对象进行查询（所有级别深度）
        /// </summary>
        /// <typeparam name="T">期望的类型</typeparam>
        /// <param name="matchString">模糊查询的字符串</param>
        /// <param name="includeSideLine">是否包含兼职的人员</param>
        /// <param name="level">查询的深度</param>
        /// <param name="returnCount">返回的行数</param>
        /// <returns>得到查询的子对象</returns>
        OguObjectCollection<T> QueryChildren<T>(string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject;
    }

    /// <summary>
    /// 虚拟组织应该提供的功能
    /// </summary>
    public interface IVirtualOrganization
    {
        /// <summary>
        /// 获取或设置一个值，表示子部门是否排除虚拟部门
        /// </summary>
        bool ExcludeVirtualDepartment { get; set; }
    }

    /// <summary>
    /// 在角色中组织机构的接口
    /// </summary>
    public interface IOrganizationInRole : IOrganization
    {
        /// <summary>
        /// 机构中的人员级别限制
        /// </summary>
        UserRankType AccessLevel
        {
            get;
        }
    }

    /// <summary>
    /// 用户组的接口
    /// </summary>
    public interface IGroup : IOguObject
    {
        /// <summary>
        /// 组内的人员
        /// </summary>
        OguObjectCollection<IUser> Members
        {
            get;
        }
    }

    /// <summary>
    /// 机构人员、组对象的实例创建的工厂接口
    /// </summary>
    public interface IOguObjectFactory
    {
        /// <summary>
        /// 根据接口类型创建对象
        /// </summary>
        /// <param name="type">需要创建的对象类型</param>
        /// <returns>对象实例</returns>
        IOguObject CreateObject(SchemaType type);
    }
}
