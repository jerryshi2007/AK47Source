using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MCS.Applications.AppAdmin.Common
{
	/// <summary>
	/// LOG系统中需要定义的操作类型
	/// </summary>
	public enum LogOpType
	{
		INSERT_ROLE,
		UPDATE_ROLE,
		DELETE_ROLE,
		INSERT_FUNCTION,
		UPDATE_FUNCTION,
		DELETE_FUNCTION,
		MODIFY_ROLE_TO_FUNC,
		INSERT_DELEGATION,
		DELETE_DELEGATION,
		UPDATE_DELEGATION,
		ADD_APPLICATION_FUNC,//描述：增加应用系统
		DELETE_APPLICATION_FUNC,//描述：删除应用系统
		MODIFY_APPLICATION_FUNC,//修改应用系统的属性
		ADD_SCOPE_FUNC,//描述：增加服务范围
		DELETE_SCOPE_FUNC,//描述：删除服务范围
		MODIFY_SCOPE_FUNC,//描述：修改服务范围-被授权对象之间的关系
		ADD_OBJECT_FUNC,//描述：给角色增加被授权对象
		DELETE_OBJECT_FUNC,//描述：删除角色中的被授权对象
	}
}
