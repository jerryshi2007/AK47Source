using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// 属性输出到数据库的快照模式定义
	/// </summary>
	[Flags]
	public enum SnapshotModeDefinition
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		/// <summary>
		/// 在快照中
		/// </summary>
		[EnumItemDescription("是否在快照中")]
		IsInSnapshot = 1,

		/// <summary>
		/// 可以搜索
		/// </summary>
		[EnumItemDescription("是否可以检索")]
		IsSearchable = 2,

		/// <summary>
		/// 可以排序
		/// </summary>
		[EnumItemDescription("是否可以排序")]
		IsSortable = 4,

		/// <summary>
		/// 可以全文检索
		/// </summary>
		[EnumItemDescription("是否全文检索")]
		IsFullTextIndexed = 8,

		/// <summary>
		/// 在通用快照中保存
		/// </summary>
		[EnumItemDescription("是否在通用快照中")]
		IsInCommonSnapshot = 16,

		/// <summary>
		/// 全部，但不保存在通用快照中
		/// </summary>
		[EnumItemDescription("仅仅不在通用快照中，其它都在")]
		AllButNotInCommonSnapshot = All & ~IsInCommonSnapshot,

		/// <summary>
		/// 全部
		/// </summary>
		All = 31
	}

	/// <summary>
	/// 表示属性的批量模式
	/// </summary>
	public enum PropertyBatchMode
	{
		/// <summary>
		/// 正常模式，此属性不依赖于对象或其他属性，可以批量修改。
		/// </summary>
		Normal = 0,
		/// <summary>
		/// 独立模式，此属性依赖于对象自身或其他属性，不可以批量修改。
		/// </summary>
		Standalone = 1,
	}

	/// <summary>
	/// 对象的CodeName唯一性校验逻辑
	/// </summary>
	public enum SchemaObjectCodeNameValidationMethod
	{
		/// <summary>
		/// 按照CodeNameKey来确定唯一性
		/// </summary>
		ByCodeNameKey,

		/// <summary>
		/// 按照容器和CodeNameKey来确定唯一性
		/// </summary>
		ByContainerAndCodeNameKey
	}

	/// <summary>
	/// 容器内的对象的校验规则
	/// </summary>
	public enum SCRelationFullPathValidationMethod
	{
		/// <summary>
		/// 不作控制
		/// </summary>
		None,

		/// <summary>
		/// 在父亲中唯一
		/// </summary>
		UniqueInParent
	}

	/// <summary>
	/// 表示对象的状态
	/// </summary>
	public enum SchemaObjectStatus
	{
		/// <summary>
		/// 未指定
		/// </summary>
		[EnumItemDescription("未指定")]
		Unspecified = 0,

		/// <summary>
		/// 正常
		/// </summary>
		[EnumItemDescription("正常")]
		Normal = 1,

		/// <summary>
		/// 被删除
		/// </summary>
		[EnumItemDescription("被删除")]
		Deleted = 3,

		/// <summary>
		/// 容器删除时的被动删除
		/// </summary>
		[EnumItemDescription("容器删除时的被动删除")]
		DeletedByContainer = 4
	}

	/// <summary>
	/// 对象的操作类型
	/// </summary>
	public enum SCObjectOperationMode
	{
		Add,
		Update,
		Delete
	}
}
