using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    public enum ClientObjectSchemaType
    {
        // Summary:
        //     未指定
        Unspecified = 0,

        //
        // Summary:
        //     组织机构
        Organizations = 1,

        //
        // Summary:
        //     用户
        Users = 2,

        //
        // Summary:
        //     组
        Groups = 4,

        //
        // Summary:
        //     兼职
        Sideline = 8,

        //
        // Summary:
        //     角色中的机构
        OrganizationsInRole = 65,

        //
        // Summary:
        //     所有条件
        All = 65535,
    }

    public enum ClientUserRankType
    {
        // Summary:
        //     敏感级别
        Unspecified = 0,

        //
        // Summary:
        //     敏感级别
        MinGanJiBie = 1,

        //
        // Summary:
        //     工人
        GongRen = 8,

        //
        // Summary:
        //     一般人员
        YiBanRenYuan = 10,

        //
        // Summary:
        //     副科级
        FuKeji = 20,

        //
        // Summary:
        //     正科级
        ZhengKeJi = 30,

        //
        // Summary:
        //     副处级
        FuChuJi = 40,

        //
        // Summary:
        //     正处级
        ZhengChuJi = 50,

        //
        // Summary:
        //     副局级
        FuJuJi = 60,

        //
        // Summary:
        //     正局级
        ZhengJuJi = 70,

        //
        // Summary:
        //     副部级
        FuBuJi = 80,

        //
        // Summary:
        //     正部级
        ZhengBuJi = 90,
    }

    public enum ClientSearchOUIDType
    {
        // Summary:
        //     空设置
        None = 0,

        //
        // Summary:
        //     按照ID来进行查询(Guid)
        Guid = 1,

        //
        // Summary:
        //     按照对象的全路径来进行查询
        FullPath = 5,

        //
        // Summary:
        //     按照用户的登录名称来进行查询
        LogOnName = 6,
    }

    // Summary:
    //     排序对象的升降序
    public enum SortDirectionType
    {
        // Summary:
        //     空设置
        None = 0,

        //
        // Summary:
        //     升序
        Ascending = 1,

        //
        // Summary:
        //     降序
        Descending = 2,
    }

    // Summary:
    //     排序对象的排序条件
    public enum OrderByPropertyType
    {
        // Summary:
        //     空设置
        None = 0,

        //
        // Summary:
        //     按照GlobalSortID属性排序
        GlobalSortID = 1,

        //
        // Summary:
        //     按照FullPath排序
        FullPath = 2,

        //
        // Summary:
        //     按照名字排序
        Name = 3,
    }
}
