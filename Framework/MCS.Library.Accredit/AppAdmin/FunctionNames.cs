#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	FunctionNames.cs
// Remark	：		自授权管理系统中默认管理功能项的枚举定义
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		新创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// 自授权功能的所有功能CODE_NAME
	/// </summary>
	/// <remarks>自授权功能的所有功能CODE_NAME</remarks>
	public enum FunctionNames
	{
		/// <summary>
		/// 添加应用功能
		/// </summary>
		/// <remarks>添加应用功能</remarks>
		ADD_APPLICATION_FUNC,
		/// <summary>
		/// 删除应用功能
		/// </summary>
		/// <remarks>删除应用功能</remarks>
		DELETE_APPLICATION_FUNC,
		/// <summary>
		/// 修改应用功能
		/// </summary>
		/// <remarks>修改应用功能</remarks>
		MODIFY_APPLICATION_FUNC,
		/// <summary>
		/// 添加范围功能
		/// </summary>
		/// <remarks>添加范围功能</remarks>
		ADD_SCOPE_FUNC,	
		/// <summary>
		/// 删除范围功能
		/// </summary>
		/// <remarks>删除范围功能</remarks>
		DELETE_SCOPE_FUNC,
		/// <summary>
		/// 修改范围功能
		/// </summary>
		/// <remarks>修改范围功能</remarks>
		MODIFY_SCOPE_FUNC,
		/// <summary>
		/// 添加角色功能
		/// </summary>
		/// <remarks>添加角色功能</remarks>
		ADD_ROLE_FUNC,
		/// <summary>
		/// 删除角色功能
		/// </summary>
		/// <remarks>删除角色功能</remarks>
		DELETE_ROLE_FUNC,
		/// <summary>
		/// 修改角色功能
		/// </summary>
		/// <remarks>修改角色功能</remarks>
		MODIFY_ROLE_FUNC,
		/// <summary>
		/// 添加对象功能
		/// </summary>
		/// <remarks>添加对象功能</remarks>
		ADD_OBJECT_FUNC,
		/// <summary>
		/// 删除对象功能
		/// </summary>
		/// <remarks>删除对象功能</remarks>
		DELETE_OBJECT_FUNC,
		/// <summary>
		/// 修改对象功能
		/// </summary>
		/// <remarks>修改对象功能</remarks>
		MODIFY_OBJECT_FUNC,
		/// <summary>
		/// 添加功能、功能集合功能
		/// </summary>
		/// <remarks>添加功能、功能集合功能</remarks>
		ADD_FUNCTION_FUNC,
		/// <summary>
		/// 删除功能、功能集合功能
		/// </summary>
		/// <remarks>删除功能、功能集合功能</remarks>
		DELETE_FUNCTION_FUNC,
		/// <summary>
		/// 修改功能、功能集合功能
		/// </summary>
		/// <remarks>修改功能、功能集合功能</remarks>
		MODIFY_FUNCTION_FUNC,
		/// <summary>
		/// 维护功能-功能集合关系功能
		/// </summary>
		/// <remarks>维护功能-功能集合关系功能</remarks>
		FSTF_MAINTAIN_FUNC,
		/// <summary>
		/// 维护角色功能关系功能
		/// </summary>
		/// <remarks>维护角色功能关系功能</remarks>
		RTF_MAINTAIN_FUNC,
		/// <summary>
		/// 自授权角色功能关系功能
		/// </summary>
		/// <remarks>自授权角色功能关系功能</remarks>
		SELF_MAINTAIN_FUNC,
		/// <summary>
		/// 记录日志功能
		/// </summary>
		/// <remarks>记录日志功能</remarks>
		LOG_MAINTAIN_FUNC,
		/// <summary>
		/// 查询日志功能
		/// </summary>
		/// <remarks>查询日志功能</remarks>
		QUERY_LOG_FUNC
	}
}
