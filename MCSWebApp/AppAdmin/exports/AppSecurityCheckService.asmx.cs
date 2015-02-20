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
	/// AppSecurityCheckService ��ժҪ˵����
	/// </summary>
	public class AppSecurityCheckService : System.Web.Services.WebService
	{
		public AppSecurityCheckService()
		{
			//CODEGEN: �õ����� ASP.NET Web ����������������
			InitializeComponent();
		}

		#region �����������ɵĴ���		
		//Web ����������������
		private IContainer components = null;				
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
		}
		/// <summary>
		/// ������������ʹ�õ���Դ��
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

		#region Ӧ��ϵͳ��������Ȩ��Ϣ
		#region GetApplications
		/// <summary>
		/// �������Ӧ�õ���Ϣ
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
		/// ��ѯָ��Ӧ���У�ָ���������н�ɫ
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetRoles(string appCodeName, RightMaskType rightMask)
		{
			return SecurityCheck.GetRoles(appCodeName, rightMask);
		}		
		#endregion GetRoles

		#region GetFunctions
		/// <summary>
		/// ��ѯָ��Ӧ���У�ָ���������й���
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetFunctions(string appCodeName, RightMaskType rightMask)
		{
			return SecurityCheck.GetFunctions(appCodeName, rightMask);
		}
		#endregion GetFunctions
		#endregion 

		#region ��ɫ�����ܡ�����Ȩ������Ա��֮��Ķ�Ӧ��ϵ
		#region GetRolesUsers		
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
		/// <param name="sidelineMask">��Աְλ����</param>
		/// <param name="extAttr">Ҫ���ȡ����չ����</param>
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
		/// ��ѯ����ָ�����ţ��ض�Ӧ����ĳЩ��ɫ�µ����л�������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ���������Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
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
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
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
		/// ����ָ��Ӧ���У�����ָ�����ܵĽ�ɫ��
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetFunctionsRoles(string appCodeName, string funcCodeNames)
		{
			return SecurityCheck.GetFunctionsRoles(appCodeName, funcCodeNames);
		}
		#endregion GetFunctionsRoles		

		#region GetFunctionsUsers		
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ӵ��ָ�����ܵ�������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
		/// <param name="sidelineMask">��Աְλ����</param>
		/// <param name="extAttr">Ҫ���ȡ����չ����</param>
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

		#region Ȩ���ж�
		#region IsUserInRoles		
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		
		#region ���ָ����Ա��Ȩ����Ϣ
		#region GetUserRoles
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// ��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// ��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// ��ѯָ����Ա��ӵ�е�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
		/// <param name="scopeMask">����Χ����</param>
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
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
		/// <param name="scopeMask">����Χ����</param>
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

		#region ���ָ����Ա��ί����Ϣ
		#region GetOriginalUser
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ��ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
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
		/// ��ѯָ����Ա��ָ������Ӧ�������н�ɫ��ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
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
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ����ʱ�ö��ŷָ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
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
		/// ��ѯָ����Ա������Ӧ���С����н�ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���¼��</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
		/// <returns></returns>
		[WebMethod]
		public DataSet GetAllDelegatedUser(string userValues, UserValueType userValueType, bool includeDisabled)
		{
			return SecurityCheck.GetAllDelegatedUser(userValues, userValueType, includeDisabled);
		}
		#endregion GetAllDelegatedUser

		#region GetUserAllowDelegteRoles
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еģ��ɽ���ί�ɵĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ����Ȩ����</param>
		/// <param name="delegationMask">Ȩ��ί������</param>
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
		/// �������ݻ���
		/// </summary>
		[WebMethod]
		public void RemoveAllCache()
		{
			SecurityCheck.RemoveAllCache();
		}
		#endregion
	}
}
