using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 票据检查阶段
	/// </summary>
	[Flags]
	public enum AccessTicketCheckPhase
	{
		/// <summary>
		/// 空
		/// </summary>
		None = 0,

		/// <summary>
		/// 在OnInit阶段
		/// </summary>
		Init = 1,

		/// <summary>
		/// 在OnLoad阶段
		/// </summary>
		Load = 2,

		/// <summary>
		/// 在PreRender阶段
		/// </summary>
		PreRender = 4,

		/// <summary>
		/// POST时是否校验
		/// </summary>
		Post = 8,

		/// <summary>
		/// CallBack时是否校验
		/// </summary>
		CallBack = 16,
	}
}
