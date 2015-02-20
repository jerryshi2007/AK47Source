using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Text;
using System.Data.SqlClient;

using MCS.Library.Accredit.AppAdmin;

namespace MCS.Applications.AppAdmin.exports
{
	/// <summary>
	/// AppSecurityCheckService 的摘要说明。
	/// </summary>
	public class AppSecurityCheckService : System.Web.Services.WebService
	{
		public AppSecurityCheckService()
		{
			//CODEGEN: 该调用是 ASP.NET Web 服务设计器所必需的
			InitializeComponent();
		}

		#region 组件设计器生成的代码		
		//Web 服务设计器所必需的
		private IContainer components = null;				
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
		}
		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}		
		#endregion

		#region 应用系统基本的授权信息
		#region GetApplications
		/// <summary>
		/// 获得所有应用的信息
		/// </summary>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetApplications()
		{
			return SecurityCheck.GetApplications();
		}		
		#endregion GetApplications		
		
		#region GetRoles		
		/// <summary>
		/// 查询指定应用中，指定类别的所有角色
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetRoles(string appCodeName, RightMaskType rightMask)
		{
			return SecurityCheck.GetRoles(appCodeName, rightMask);
		}		
		#endregion GetRoles

		#region GetFunctions
		/// <summary>
		/// 查询指定应用中，指定类别的所有功能
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetFunctions(string appCodeName, RightMaskType rightMask)
		{
			return SecurityCheck.GetFunctions(appCodeName, rightMask);
		}
		#endregion GetFunctions
		#endregion 

		#region 角色、功能、被授权对象（人员）之间的对应关系
		#region GetRolesUsers		
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <param name="sidelineMask">人员职位类型</param>
		/// <param name="extAttr">要求获取的扩展属性</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetRolesUsers(string orgRoot, 
			string appCodeName, 
			string roleCodeNames, 
			DelegationMaskType delegationMask, 
			SidelineMaskType sidelineMask, 
			string extAttr)
		{
			return SecurityCheck.GetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, sidelineMask, extAttr);
		}
		
		#region GetDepartmentAndUserInRoles
		/// <summary>
		/// 查询属于指定部门，特定应用中某些角色下的所有机构和人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetDepartmentAndUserInRoles(string orgRoot, 
			string appCodeName, 
			string roleCodeNames, 
			bool doesMixSort, 
			bool doesSortRank, 
			bool includeDelegate)
		{
			return SecurityCheck.GetDepartmentAndUserInRoles(orgRoot, appCodeName, 
				roleCodeNames, doesMixSort, doesSortRank, includeDelegate);
		}
		#endregion		
		#endregion GetRolesUsers

		#region GetChildrenInRoles
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetChildrenInRoles(string orgRoot, 
			string appCodeName, 
			string roleCodeNames, 
			bool doesMixSort,
			bool doesSortRank, 
			bool includeDelegate)
		{
			return SecurityCheck.GetChildrenInRoles(orgRoot, appCodeName,
				roleCodeNames, doesMixSort, doesSortRank, includeDelegate);
		}
		#endregion GetChildrenInRoles

		#region GetFunctionsRoles
		/// <summary>
		/// 查找指定应用中，具有指定功能的角色。
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetFunctionsRoles(string appCodeName, string funcCodeNames)
		{
			return SecurityCheck.GetFunctionsRoles(appCodeName, funcCodeNames);
		}
		#endregion GetFunctionsRoles		

		#region GetFunctionsUsers		
		/// <summary>
		/// 查询指定部门下，指定应用中，拥有指定功能的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <param name="sidelineMask">人员职位类型</param>
		/// <param name="extAttr">要求获取的扩展属性</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetFunctionsUsers(string orgRoot, 
			string appCodeName, 
			string funcCodeNames, 
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask, 
			string extAttr)
		{
			return SecurityCheck.GetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, delegationMask, sidelineMask, extAttr);
		}
		#endregion GetFunctionsUsers
		#endregion

		#region 权限判定
		#region IsUserInRoles		
		/// <summary>
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public bool IsUserInRoles(string userValue, 
			string appCodeName, 
			string roleCodeNames, 
			UserValueType userValueType, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.IsUserInRoles(userValue, appCodeName, roleCodeNames, userValueType, delegationMask);
		}
		#endregion IsUserInRoles

		#region IsUserInAllRoles		
		/// <summary>
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public bool IsUserInAllRoles(string userValue, 
			string appCodeName, 
			string roleCodeNames,
			UserValueType userValueType, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.IsUserInAllRoles(userValue, appCodeName, roleCodeNames, userValueType, delegationMask);
		}
		#endregion IsUserInAllRoles

		#region DoesUserHasPermissions		
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public bool DoesUserHasPermissions(string userValue, 
			string appCodeName,
			string funcCodeNames, 
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.DoesUserHasPermissions(userValue, appCodeName, funcCodeNames, userValueType, delegationMask);
		}
		#endregion DoesUserHasPermissions

		#region DoesUserHasAllPermissions		
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public bool DoesUserHasAllPermissions(string userValue, 
			string appCodeName, 
			string funcCodeNames, 
			UserValueType userValueType, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.DoesUserHasAllPermissions(userValue, appCodeName, funcCodeNames, userValueType, delegationMask);
		}
		#endregion DoesUserHasAllPermissions
		#endregion
		
		#region 获得指定人员的权限信息
		#region GetUserRoles
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserRoles(string userValue, 
			string appCodeName, 
			UserValueType userValueType, 
			RightMaskType rightMask, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.GetUserRoles(userValue, appCodeName, userValueType, rightMask, delegationMask);
		}
		#endregion GetUserRoles

		#region GetUserPermissions
		/// <summary>
		/// 查询指定人员，在指定应用具有的权限（功能）
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserPermissions(string userValue,
			string appCodeName, 
			UserValueType userValueType, 
			RightMaskType rightMask, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.GetUserPermissions(userValue, appCodeName, userValueType, rightMask, delegationMask);
		}
		#endregion GetUserPermissions

		#region GetUserApplicationsRoles
		/// <summary>
		/// 查询指定人员的应用角色信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserApplicationsRoles(string userValue, 
			UserValueType userValueType, 
			RightMaskType rightMask, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.GetUserApplicationsRoles(userValue, userValueType, rightMask, delegationMask);
		}
		#endregion GetUserApplicationsRoles

		#region GetUserApplications
		/// <summary>
		/// 查询指定人员所拥有的所有应用信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserApplications(string userValue,
			UserValueType userValueType, 
			RightMaskType rightMask, 
			DelegationMaskType delegationMask)
		{
			return SecurityCheck.GetUserApplications(userValue, userValueType, rightMask, delegationMask);
		}
		#endregion GetUserApplications

		#region GetUserRolesScopes
		/// <summary>
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <param name="scopeMask">服务范围类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserRolesScopes(string userValue, 
			string appCodeName, 
			string roleCodeNames, 
			UserValueType userValueType,
			DelegationMaskType delegationMask, 
			ScopeMaskType scopeMask)
		{
			return SecurityCheck.GetUserRolesScopes(userValue, appCodeName, roleCodeNames, userValueType, delegationMask, scopeMask);
		}
		#endregion GetUserRolesScopes

		#region GetUserFunctionsScopes
		/// <summary>
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <param name="scopeMask">服务范围类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserFunctionsScopes(string userValue, 
			string appCodeName, 
			string funcCodeNames, 
			UserValueType userValueType,
			DelegationMaskType delegationMask, 
			ScopeMaskType scopeMask)
		{
			return SecurityCheck.GetUserFunctionsScopes(userValue, appCodeName, funcCodeNames, userValueType, delegationMask, scopeMask);
		}
		#endregion GetUserFunctionsScopes
		#endregion

		#region 获得指定人员的委派信息
		#region GetOriginalUser
		/// <summary>
		/// 查询指定人员在指定应用中，指定角色的原有委派者
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetOriginalUser(string userValue, 
			string appCodeName, 
			string roleCodeNames,
			UserValueType userValueType,
			bool includeDisabled)
		{
			return SecurityCheck.GetOriginalUser(userValue, appCodeName, roleCodeNames, userValueType, includeDisabled);
		}
		#endregion GetOriginalUser

		#region GetAllOriginalUser		
		/// <summary>
		/// 查询指定人员在指定所有应用中所有角色的原有委派者
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetAllOriginalUser(string userValue, 
			UserValueType userValueType, 
			bool includeDisabled)
		{
			return SecurityCheck.GetAllOriginalUser(userValue, userValueType, includeDisabled);
		}
		#endregion GetAllOriginalUser		

		#region GetDelegatedUser
		/// <summary>
		/// 查询指定人员在指定应用中，指定角色的被委派者
		/// </summary>
		/// <param name="userValues">用户身份标识（由userValueType参数指定类型，多个时用逗号分隔）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetDelegatedUser(string userValues, 
			string appCodeName, 
			string roleCodeNames, 
			UserValueType userValueType, 
			bool includeDisabled)
		{
			return SecurityCheck.GetDelegatedUser(userValues, appCodeName, roleCodeNames, userValueType, includeDisabled);
		}		
		#endregion GetDelegatedUser		

		#region GetAllDelegatedUser
		/// <summary>
		/// 查询指定人员在所有应用中、所有角色的被委派者
		/// </summary>
		/// <param name="userValues">用户登录名</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetAllDelegatedUser(string userValues, UserValueType userValueType, bool includeDisabled)
		{
			return SecurityCheck.GetAllDelegatedUser(userValues, userValueType, includeDisabled);
		}
		#endregion GetAllDelegatedUser

		#region GetUserAllowDelegteRoles
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的，可进行委派的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限授权类型</param>
		/// <param name="delegationMask">权限委派类型</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetUserAllowDelegteRoles(string userValue,
			string appCodeName, 
			UserValueType userValueType, 
			RightMaskType rightMask)
		{
			return SecurityCheck.GetUserAllowDelegteRoles(userValue, appCodeName, userValueType, rightMask);
		}
		#endregion GetUserAllowDelegteRoles
		#endregion

		#region RemoveAllCache
		/// <summary>
		/// 清理数据缓存
		/// </summary>
		[WebMethod]
		public void RemoveAllCache()
		{
			SecurityCheck.RemoveAllCache();
		}
		#endregion
	}
}
