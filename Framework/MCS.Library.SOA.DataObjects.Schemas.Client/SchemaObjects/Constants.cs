using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	/// <summary>
	/// 表示对象的状态
	/// </summary>
	public enum ClientSchemaObjectStatus
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
}
