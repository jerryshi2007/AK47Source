using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//意见列表控件所涉及到的枚举类型

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 显示左侧意见的选项
	/// </summary>
	[Flags]
	public enum ShowStepUsersDefine
	{
		/// <summary>
		/// 显示所有人员
		/// </summary>
		All = 0,

		/// <summary>
		/// 显示下一步的人员
		/// </summary>
		OnlyShowNextStepUsers = 1,
	}

	/// <summary>
	/// 环节的编辑选项
	/// </summary>
	[Flags]
	public enum ActivityEditMode
	{
		/// <summary>
		/// 不允许编辑
		/// </summary>
		None = 0,

		/// <summary>
		/// 允许添加
		/// </summary>
		Add = 1,

		/// <summary>
		/// 允许编辑
		/// </summary>
		Edit = 2,

		/// <summary>
		/// 允许删除
		/// </summary>
		Delete = 4
	}

	/// <summary>
	/// 左侧用户Title的显示模式
	/// </summary>
	public enum UserTitleShowMode
	{
		None = 0,
		ShowUserTitleWhenActivityNameEmpty = 1,
		ShowActivityName = 2,
		ShowActivityNameAndTitle = 3,
		ShowDepartmentName = 4,
		ShowActivityNameAndDeptNameAndTitle = 7
	}
}
