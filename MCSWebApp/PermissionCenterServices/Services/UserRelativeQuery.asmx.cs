using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using PermissionCenter.Adapters;
using MCS.Library.SOA.DataObjects.Workflow;

namespace PermissionCenter.Services
{
	/// <summary>
	/// UserRelativeQuery 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	public class UserRelativeQuery : System.Web.Services.WebService
	{
		[WebMethod(Description = "查询人员的直接角色，不包括矩阵中的角色和条件计算的角色")]
		public DataTable QueryRolesByUser(string userID)
		{
			return QueryByUserAdapter.Instance.QueryRoles(userID);
		}

		[WebMethod(Description = "查询出现在其中的用户的角色矩阵")]
		public DataTable QueryRoleMatricesByUser(string userID)
		{
			return QueryByUserAdapter.Instance.QueryMatrices(userID);
		}

		[WebMethod(Description = "查询人员的流程矩阵，返回矩阵相关的流程定义的信息")]
		public DataTable QueryProcessMatricesByUser(string userID)
		{
			return WfMatrixAdapter.Instance.QueryUserRelativeProcessMatrices(userID);
		}

		[WebMethod(Description = "查询人员的流程定义，返回相关的流程定义的信息")]
		public DataTable QueryProcessDescriptorsByUser(string userID)
		{
			return WfProcessDescriptorDimensionAdapter.Instance.QueryUserRelativeProcessDescriptors(userID);
		}

		[WebMethod(Description = "查询人员相关的流程实例，返回相关的流程实例和待办的信息")]
		public DataTable QueryProcessInstancesByUser(string userID)
		{
			return WfProcessCurrentInfoAdapter.Instance.QueryUserRelativeRunningProcesses(userID);
		}
	}
}
