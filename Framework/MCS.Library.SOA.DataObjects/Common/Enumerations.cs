using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 文件的紧急程度
	/// </summary>
	public enum FileEmergency
	{
		/// <summary>
		/// 无
		/// </summary>
		[EnumItemDescription("－", "", 0)]
		None = 0,

		/// <summary>
		/// 急件
		/// </summary>
		[EnumItemDescription("急件", "!", 1)]
		JiJian = 1,

		/// <summary>
		/// 平急
		/// </summary>
		[EnumItemDescription("平急", "!!", 2)]
		PingJi = 2,

		/// <summary>
		/// 加急
		/// </summary>
		[EnumItemDescription("加急", "!!", 3)]
		JiaJi = 3,

		/// <summary>
		/// 特急
		/// </summary>
		[EnumItemDescription("特急", "!!!", 4)]
		TeJi = 4,

		/// <summary>
		/// 特提
		/// </summary>
		[EnumItemDescription("特提", "!!!", 5)]
		TeTi = 5
	}

	/// <summary>
	/// 文件的密级
	/// </summary>
	public enum FileSecret
	{
		/// <summary>
		/// 无
		/// </summary>
		[EnumItemDescription("－", 0)]
		None = 0,

		/// <summary>
		/// 秘密
		/// </summary>
		[EnumItemDescription("秘密", 1)]
		MiMi = 1,

		/// <summary>
		/// 机密
		/// </summary>
		[EnumItemDescription("机密", 2)]
		JiMi = 2,

		/// <summary>
		/// 绝密
		/// </summary>
		[EnumItemDescription("绝密", 3)]
		JueMi = 3

	}

	public enum ArchiveStatus
	{
		/// <summary>
		/// 未归档
		/// </summary>
		[EnumItemDescription("未归档", 0)]
		UnArchive = 0,

		/// <summary>
		/// 已归档
		/// </summary>
		[EnumItemDescription("已归档", 1)]
		Archived = 1,

		/// <summary>
		/// 不归档
		/// </summary>
		[EnumItemDescription("不归档", 2)]
		NeedNotArchive = 2
	}

	/// <summary>
	/// 分发部门是否已经办结
	/// </summary>
	public enum CompleteFlag
	{
		/// <summary>
		/// 未完成
		/// </summary>
		[EnumItemDescription("未办结", 0)]
		NoComplete = 0,

		/// <summary>
		/// 已经完成
		/// </summary>
		[EnumItemDescription("已办结", 1)]
		IsComplete = 1,

		/// <summary>
		/// 已经取消
		/// </summary>
		[EnumItemDescription("已取消", 2)]
		IsCancelled = 2
	}

	/// <summary>
	/// 性别
	/// </summary>
	public enum Gender
	{
		/// <summary>
		/// 男
		/// </summary>
		[EnumItemDescription("男", 0, Define.DefaultCulture)]
		Male = 0,

		/// <summary>
		/// 女
		/// </summary>
		[EnumItemDescription("女", 1, Define.DefaultCulture)]
		Female = 1
	}

    //从MCS.OA.ExtDataObjects迁移
    /// <summary>
    /// 审批分数
    /// </summary>
    public enum ApprovalScore
    {
        /// <summary>
        /// 未评分
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// 重点奖励
        /// </summary>
        [EnumItemDescription("重点奖励", 1, Define.DefaultCulture)]
        HighReward = 1,

        /// <summary>
        /// 一般奖励
        /// </summary>
        [EnumItemDescription("一般奖励", 1, Define.DefaultCulture)]
        Reward = 2,

        /// <summary>
        /// 好
        /// </summary>
        [EnumItemDescription("好", 1, Define.DefaultCulture)]
        Good = 3,

        /// <summary>
        /// 合格
        /// </summary>
        [EnumItemDescription("合格", 1, Define.DefaultCulture)]
        Soso = 4,

        /// <summary>
        /// 差
        /// </summary>
        [EnumItemDescription("差", 1, Define.DefaultCulture)]
        Poor = 5,

        /// <summary>
        /// 重点处罚
        /// </summary>
        [EnumItemDescription("重点处罚", 1, Define.DefaultCulture)]
        Punish = 6,

        /// <summary>
        /// 一般处罚
        /// </summary>
        [EnumItemDescription("一般处罚", 1, Define.DefaultCulture)]
        HighPunish = 7
    }

    /// <summary>
    /// 评审结果
    /// </summary>
    public enum ApprovalResult
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumItemDescription("同意", 1, Define.DefaultCulture)]
        Agree = 1,
        /// <summary>
        /// 
        /// </summary>
        [EnumItemDescription("不同意", 2, Define.DefaultCulture)]
        Disagree = 0
    }

}
