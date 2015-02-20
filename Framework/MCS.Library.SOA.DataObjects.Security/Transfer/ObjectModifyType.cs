using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	[Flags]
	public enum ObjectModifyType
	{
		/// <summary>
		/// 没修改
		/// </summary>
		None = 0,

		/// <summary>
		/// 添加
		/// </summary>
		Add = 1,

		/// <summary>
		/// 删除
		/// </summary>
		Delete = 2,

		/// <summary>
		/// 内容修改
		/// </summary>
		PropertyModified = 4,

		/// <summary>
		/// 子成员修改(没用)
		/// </summary>
		ChildrenModified = 8,

		/// <summary>
		/// 属性和子成员均修改
		/// </summary>
		PropertyAndChildrenModified = PropertyModified | ChildrenModified,

		/// <summary>
		/// 对现有群组增加缺失标记
		/// </summary>
		MissingMarker = 16,


	}
}
