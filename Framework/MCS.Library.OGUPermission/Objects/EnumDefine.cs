#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	EnumDefine.cs
// Remark	：	进行机构人员查询时，提供的ID的类型
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// 进行机构人员查询时，提供的ID的类型
    /// </summary>
    public enum SearchOUIDType
    {
		/// <summary>
		/// 空设置
		/// </summary>
		None = 0,

        /// <summary>
        /// 按照ID来进行查询(Guid)
        /// </summary>
        Guid = 1,

        /// <summary>
        /// 按照对象的全路径来进行查询
        /// </summary>
        FullPath = 5,

        /// <summary>
        /// 按照用户的登录名称来进行查询
        /// </summary>
        LogOnName = 6,
    }

    /// <summary>
    /// 查询子对象的深度
    /// </summary>
    public enum SearchLevel
    {
        /// <summary>
        /// 查询所有子对象
        /// </summary>
        SubTree = 0,

        /// <summary>
        /// 仅查询一级
        /// </summary>
        OneLevel = 1
    }

	/// <summary>
	/// 列出对象的条件掩码
	/// </summary>
	[Flags]
	public enum ListObjectMask
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		/// <summary>
		/// 查询正常使用的数据对象
		/// </summary>
		Common = 1,

		/// <summary>
		/// 直接删除的对象
		/// </summary>
		DirectDeleted = 2,

		/// <summary>
		/// 因部门导致数据逻辑删除对象
		/// </summary>
		DeletedByOrganization = 4,

		/// <summary>
		/// 因人员导致数据逻辑删除对象
		/// </summary>
		DeletedByUser = 8,

		/// <summary>
		/// 全部条件
		/// </summary>
		All = 15
	}

    /// <summary>
    /// 排序对象的排序条件
    /// </summary>
    public enum OrderByPropertyType
    {
		/// <summary>
		/// 空设置
		/// </summary>
		None = 0,

        /// <summary>
        /// 按照GlobalSortID属性排序
        /// </summary>
        GlobalSortID = 1,

        /// <summary>
        /// 按照FullPath排序
        /// </summary>
        FullPath = 2,

        /// <summary>
        /// 按照名字排序
        /// </summary>
        Name = 3
    }

    /// <summary>
    /// 排序对象的升降序
    /// </summary>
    public enum SortDirectionType
    {
		/// <summary>
		/// 空设置
		/// </summary>
		None = 0,

        /// <summary>
        /// 升序
        /// </summary>
        Ascending = 1,

        /// <summary>
        /// 降序
        /// </summary>
        Descending = 2
    }

    /// <summary>
    /// 对象的类型
    /// </summary>
    [Flags]
    public enum SchemaType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// 组织机构
        /// </summary>
        Organizations = 1,

        /// <summary>
        /// 用户
        /// </summary>
        Users = 2,

        /// <summary>
        /// 组
        /// </summary>
        Groups = 4,

        /// <summary>
        /// 兼职
        /// </summary>
        Sideline = 8,

        /// <summary>
        /// 角色中的机构
        /// </summary>
        OrganizationsInRole = 65,

        /// <summary>
        /// 所有条件
        /// </summary>
        All = 65535
    }

    /// <summary>
    /// 人员和部门的级别
    /// </summary>
    public enum UserRankType
    {
        /// <summary>
        /// 敏感级别
        /// </summary>
        [EnumItemDescription("(未指定)", SortId = 99)]
        Unspecified = 0,

        /// <summary>
        /// 敏感级别
        /// </summary>
        [EnumItemDescription("特殊级别", SortId = 98)]
        MinGanJiBie = 1,

        /// <summary>
        /// 工人
        /// </summary>
        [EnumItemDescription("工人", SortId = 95)]
        GongRen = 8,

        /// <summary>
        /// 一般人员
        /// </summary>
        [EnumItemDescription("一般人员", SortId = 90)]
        YiBanRenYuan = 10,

        /// <summary>
        /// 副科级
        /// </summary>
        [EnumItemDescription("副科级", SortId = 80)]
        FuKeji = 20,

        /// <summary>
        /// 正科级
        /// </summary>
        [EnumItemDescription("正科级", SortId = 70)]
        ZhengKeJi = 30,

        /// <summary>
        /// 副处级
        /// </summary>
        [EnumItemDescription("副处级", SortId = 60)]
        FuChuJi = 40,

        /// <summary>
        /// 正处级
        /// </summary>
        /// 
        [EnumItemDescription("正处级", SortId = 50)]
        ZhengChuJi = 50,

        /// <summary>
        /// 副局级
        /// </summary>
        [EnumItemDescription("副局级", SortId = 40)]
        FuJuJi = 60,

        /// <summary>
        /// 正局级
        /// </summary>
        [EnumItemDescription("正局级", SortId = 30)]
        ZhengJuJi = 70,

        /// <summary>
        /// 副部级
        /// </summary>
        [EnumItemDescription("副部级", SortId = 20)]
        FuBuJi = 80,

        /// <summary>
        /// 正部级
        /// </summary>
        [EnumItemDescription("正部级", SortId = 10)]
        ZhengBuJi = 90
    }

    /// <summary>
    /// 部门的级别
    /// </summary>
    public enum DepartmentRankType
    {
		/// <summary>
		/// 空设置
		/// </summary>
		[EnumItemDescription("(未指定)", SortId = 99)]
		None = 0,

        /// <summary>
        /// 敏感级别
        /// </summary>
		 [EnumItemDescription("敏感级别", SortId = 95)]
        MinGanJiBie = 1,

        /// <summary>
        /// 一般部门
        /// </summary>
		 [EnumItemDescription("一般部门", SortId = 90)]
        YiBanBuMen = 10,

        /// <summary>
        /// 副科级
        /// </summary>
		 [EnumItemDescription("副科级", SortId = 80)]
        FuKeji = 20,

        /// <summary>
        /// 正科级
        /// </summary>
		 [EnumItemDescription("正科级", SortId = 70)]
        ZhengKeJi = 30,

        /// <summary>
        /// 副处级
        /// </summary>
		 [EnumItemDescription("副处级", SortId = 60)]
        FuChuJi = 40,

        /// <summary>
        /// 正处级
        /// </summary>
		 [EnumItemDescription("正处级", SortId = 50)]
        ZhengChuJi = 50,

        /// <summary>
        /// 副局级
        /// </summary>
		 [EnumItemDescription("副局级", SortId = 40)]
        FuJuJi = 60,

        /// <summary>
        /// 正局级
        /// </summary>
		 [EnumItemDescription("正局级", SortId = 30)]
        ZhengJuJi = 70,

        /// <summary>
        /// 副部级
        /// </summary>
		 [EnumItemDescription("副部级", SortId = 20)]
        FuBuJi = 80,

        /// <summary>
        /// 正部级
        /// </summary>
		[EnumItemDescription("正部级", SortId = 10)]
        ZhengBuJi = 90
    }

    /// <summary>
    /// 部门的分类，如:32隶属海关、64派驻机构...
    /// </summary>
    [Flags]
    public enum DepartmentClassType
    {
        /// <summary>
        /// 未指定
        /// </summary>
		[EnumItemDescription("未指定")]
        Unspecified = 0,

        /// <summary>
        /// 隶属海关
        /// </summary>
		[EnumItemDescription("隶属组织")]
        LiShuHaiGuan = 32,

        /// <summary>
        /// 派驻机构
        /// </summary>
		[EnumItemDescription("派驻组织")]
        PaiZhuJiGou = 64,

        /// <summary>
        /// 内设机构
        /// </summary>
		[EnumItemDescription("内设组织")]
        NeiSheJiGou = 128,

        /// <summary>
        /// 其它机构
        /// </summary>
		[EnumItemDescription("其它组织")]
        QiTaJiGou = 256,
    }

    /// <summary>
    /// 部门的一些特殊属性（1虚拟机构、2一般部门、4办公室（厅）、8综合处）
    /// </summary>
    [Flags]
    public enum DepartmentTypeDefine
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [EnumItemDescription("(未指定)")]
        Unspecified = 0,

        /// <summary>
        /// 虚拟机构
        /// </summary>
        [EnumItemDescription("虚拟机构")]
        XuNiJiGou = 1,

        /// <summary>
        /// 一般部门
        /// </summary>
        [EnumItemDescription("一般部门")]
        YiBanBuMen = 2,

        /// <summary>
        /// 办公室（厅）
        /// </summary>
        [EnumItemDescription("办公室(厅)")]
        BanGongShi = 4,

        /// <summary>
        /// 综合处
        /// </summary>
        [EnumItemDescription("综合处")]
        ZongHeChu = 8,

        /// <summary>
        /// 缉私局
        /// </summary>
        [EnumItemDescription("缉私局")]
        JiSiJu = 16,
    }

    /// <summary>
    /// 用户的一些特殊属性（党组成员1、署管干部2、交流干部4、借调干部8）
    /// </summary>
    [Flags]
    public enum UserAttributesType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [EnumItemDescription("(未指定)")]
        Unspecified = 0,

        /// <summary>
        /// 党组成员
        /// </summary>
        [EnumItemDescription("党组成员")]
        DangZuChengYuan = 1,

        /// <summary>
        /// 署管干部
        /// </summary>
        [EnumItemDescription("署管干部")]
        ShuGuanGanBu = 2,

        /// <summary>
        /// 交流干部
        /// </summary>
        [EnumItemDescription("交流干部")]
        JiaoLiuGanBu = 4,

        /// <summary>
        /// 借调干部
        /// </summary>
        [EnumItemDescription("借调干部")]
        JieDiaoGanBu = 8
    }

    /// <summary>
    /// 在消除重复用户对象时，消除主职，还是兼职
    /// </summary>
    public enum DistinctReserveType
    {
		/// <summary>
		/// 空设置
		/// </summary>
		None = 0, 

        /// <summary>
        /// 保留主职
        /// </summary>
        KeepMasterOccupation = 1,

        /// <summary>
        /// 保留兼职
        /// </summary>
        KeepSidelineOccupation = 2
    }
}
