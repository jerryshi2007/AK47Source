using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    /// <summary>
    /// 文件的紧急程度
    /// </summary>
    public enum ClientFileEmergency
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 急件
        /// </summary>
        JiJian = 1,

        /// <summary>
        /// 平急
        /// </summary>
        PingJi = 2,

        /// <summary>
        /// 加急
        /// </summary>
        JiaJi = 3,

        /// <summary>
        /// 特急
        /// </summary>
        TeJi = 4,

        /// <summary>
        /// 特提
        /// </summary>
        TeTi = 5
    }

    /// <summary>
    /// 文件的密级
    /// </summary>
    public enum ClientFileSecret
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 秘密
        /// </summary>
        MiMi = 1,

        /// <summary>
        /// 机密
        /// </summary>
        JiMi = 2,

        /// <summary>
        /// 绝密
        /// </summary>
        JueMi = 3
    }

    public enum ClientArchiveStatus
    {
        /// <summary>
        /// 未归档
        /// </summary>
        UnArchive = 0,

        /// <summary>
        /// 已归档
        /// </summary>
        Archived = 1,

        /// <summary>
        /// 不归档
        /// </summary>
        NeedNotArchive = 2
    }

    /// <summary>
    /// 分发部门是否已经办结
    /// </summary>
    public enum ClientCompleteFlag
    {
        /// <summary>
        /// 未完成
        /// </summary>
        NoComplete = 0,

        /// <summary>
        /// 已经完成
        /// </summary>
        IsComplete = 1,

        /// <summary>
        /// 已经取消
        /// </summary>
        IsCancelled = 2
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum ClientGender
    {
        /// <summary>
        /// 男
        /// </summary>
        Male = 0,

        /// <summary>
        /// 女
        /// </summary>
        Female = 1
    }

    //从MCS.OA.ExtDataObjects迁移
    /// <summary>
    /// 审批分数
    /// </summary>
    public enum ClientApprovalScore
    {
        /// <summary>
        /// 未评分
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// 重点奖励
        /// </summary>
        HighReward = 1,

        /// <summary>
        /// 一般奖励
        /// </summary>
        Reward = 2,

        /// <summary>
        /// 好
        /// </summary>
        Good = 3,

        /// <summary>
        /// 合格
        /// </summary>
        Soso = 4,

        /// <summary>
        /// 差
        /// </summary>
        Poor = 5,

        /// <summary>
        /// 重点处罚
        /// </summary>
        Punish = 6,

        /// <summary>
        /// 一般处罚
        /// </summary>
        HighPunish = 7
    }

    /// <summary>
    /// 评审结果
    /// </summary>
    public enum ClientApprovalResult
    {
        /// <summary>
        /// 同意
        /// </summary>
        Agree = 1,
        /// <summary>
        /// 不同意
        /// </summary>
        Disagree = 0
    }

    // Summary:
    //     部门的分类，如:32隶属海关、64派驻机构...
    [Flags]
    public enum ClientDepartmentClassType
    {
        // Summary:
        //     未指定
        Unspecified = 0,
        //
        // Summary:
        //     隶属海关
        LiShuHaiGuan = 32,
        //
        // Summary:
        //     派驻机构
        PaiZhuJiGou = 64,
        //
        // Summary:
        //     内设机构
        NeiSheJiGou = 128,
        //
        // Summary:
        //     其它机构
        QiTaJiGou = 256,
    }

    // Summary:
    //     部门的一些特殊属性（1虚拟机构、2一般部门、4办公室（厅）、8综合处）
    [Flags]
    public enum ClientDepartmentTypeDefine
    {
        // Summary:
        //     未指定
        Unspecified = 0,
        //
        // Summary:
        //     虚拟机构
        XuNiJiGou = 1,
        //
        // Summary:
        //     一般部门
        YiBanBuMen = 2,
        //
        // Summary:
        //     办公室（厅）
        BanGongShi = 4,
        //
        // Summary:
        //     综合处
        ZongHeChu = 8,
        //
        // Summary:
        //     缉私局
        JiSiJu = 16,
    }

    // Summary:
    //     部门的级别
    public enum ClientDepartmentRankType
    {
        // Summary:
        //     空设置
        None = 0,
        //
        // Summary:
        //     敏感级别
        MinGanJiBie = 1,
        //
        // Summary:
        //     一般人员
        YiBanBuMen = 10,
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

   
}
