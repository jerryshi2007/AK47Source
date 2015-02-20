#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	IOrganizationMechanism.cs
// Remark	��	������Ա�Ĳ����ӿ�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// ������Ա�Ĳ����ӿ�
    /// </summary>
    public interface IOrganizationMechanism
    {
        /// <summary>
        /// �õ���������Ա����
        /// </summary>
        /// <typeparam name="T">�������صĽ����������ͣ�IOrganization��IUser��IGroup��IOguObject</typeparam>
        /// <param name="idType">��ѯʱʹ�õ�ID���ͣ�GUID��LogonName��FullPath</param>
        /// <param name="ids">ID����</param>
        /// <returns>���󼯺�</returns>
        // <param name="objType">���������</param>
        OguObjectCollection<T> GetObjects<T>(SearchOUIDType idType, params string[] ids) where T : IOguObject;

        /// <summary>
        /// ���ظ������Ķ���ȡ���������ļ�
        /// </summary>
        /// <returns>����������</returns>
        IOrganization GetRoot();

		/// <summary>
		/// �û���֤��ͨ�����ж��û����û����Ϳ����Ƿ���ȷ
		/// </summary>
		/// <param name="identity">�û��ĵ�¼�����������</param>
		/// <returns>�Ƿ���֤�ɹ�</returns>
		bool AuthenticateUser(LogOnIdentity identity);

		/// <summary>
		/// �������Cache
		/// </summary>
		void RemoveAllCache();
    }

    /// <summary>
    /// ��Ȩϵͳ�Ĳ����ӿ�
    /// </summary>
    public interface IPermissionMechanism
    {
        /// <summary>
        /// �õ�����Ӧ�ö���
        /// </summary>
        /// <returns></returns>
        ApplicationCollection GetAllApplications();

        /// <summary>
        /// �õ�ָ�����Ƶ�Ӧ�ö���
        /// </summary>
        /// <param name="codeNames">Ӧ�õ�����</param>
        /// <returns>Ӧ�ö��󼯺�</returns>
        ApplicationCollection GetApplications(params string[] codeNames);

        /// <summary>
        /// �õ�ָ����ɫ�£�ĳЩ�����ڵ�������Ȩ��Ա
        /// </summary>
        /// <param name="roles">��ɫ���ϡ�</param>
        /// <param name="depts">��֯�������ϡ�</param>
        /// <param name="recursively">�Ƿ�ݹ顣</param>
        /// <returns></returns>
        OguObjectCollection<IOguObject> GetRolesObjects(RoleCollection roles, OguObjectCollection<IOrganization> depts, bool recursively);

		/// <summary>
		/// �������Cache
		/// </summary>
		void RemoveAllCache();
    }
}
