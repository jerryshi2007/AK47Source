using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 表示应用的成员对象所须有的方法
	/// </summary>
	public interface IApplicationMemberPropertyAccessible : IPermissionPropertyAccessible
	{
		/// <summary>
		/// 获取应用
		/// </summary>
		IApplication Application
		{
			get;
			set;
		}

		///// <summary>
		///// 包含AppCode的全CodeName，格式如AppCode:CodeName
		///// </summary>
		//string FullCodeName
		//{
		//    get;
		//    set;
		//}
	}
}
