#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	OguObjInterfaces.cs
// Remark	��	��Ա�Ͳ��ŽӿڵĻ���
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Passport;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// ��򵥵Ļ�����Ա����
    /// </summary>
    public interface IOguSimpleObject
    {
        /// <summary>
        /// �����ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// �������ʾ����
        /// </summary>
        string DisplayName
        {
            get;
        }

        /// <summary>
        /// ������ϵͳ�е�ȫ��·��
        /// </summary>
        string FullPath
        {
            get;
        }
    }

    /// <summary>
    /// ��Ա�Ͳ��ŽӿڵĻ���
    /// </summary>
    public interface IOguObject: ITicketToken
    {
        ///// <summary>
        ///// �����ID
        ///// </summary>
        //string ID
        //{
        //    get;
        //}

        ///// <summary>
        ///// ���������
        ///// </summary>
        //string Name
        //{
        //    get;
        //}

        ///// <summary>
        ///// �������ʾ����
        ///// </summary>
        //string DisplayName
        //{
        //    get;
        //}

        /// <summary>
        /// ������Ϣ
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// ������ϵͳ�е�ȫ��·��
        /// </summary>
        string FullPath
        {
            get;
        }

        /// <summary>
        /// ���������
        /// </summary>
        SchemaType ObjectType
        {
            get;
        }

        /// <summary>
        /// ������
        /// </summary>
        IOrganization Parent
        {
            get;
        }

        /// <summary>
        /// �ڲ����ڵ������
        /// </summary>
        string SortID
        {
            get;
        }

        /// <summary>
        /// �����������е������
        /// </summary>
        string GlobalSortID
        {
            get;
        }

        /// <summary>
        /// �û���ȱʡ��������
        /// </summary>
        IOrganization TopOU
        {
            get;
        }

        /// <summary>
        /// ��������֯�������ϵĲ��
        /// </summary>
        int Levels
        {
            get;
        }

        /// <summary>
        /// �жϵ�ǰ�����Ƿ���parent������������
        /// </summary>
        /// <param name="parent">ĳ����������</param>
        /// <returns>�Ƿ�����ĳ����</returns>
        bool IsChildrenOf(IOrganization parent);

        /// <summary>
        /// ���Լ���
        /// </summary>
        IDictionary Properties
        {
            get;
        }
    }

    /// <summary>
    /// �û��Ľӿ�
    /// </summary>
    public interface IUser : IOguObject
    {
        /// <summary>
        /// ��¼����
        /// </summary>
        string LogOnName
        {
            get;
        }

        /// <summary>
        /// �û����ʼ���ַ
        /// </summary>
        string Email
        {
            get;
        }

        /// <summary>
        /// �û���ְλ
        /// </summary>
        string Occupation
        {
            get;
        }

        /// <summary>
        /// ��Ա�ļ���
        /// </summary>
        UserRankType Rank
        {
            get;
        }

        /// <summary>
        /// �û�������
        /// </summary>
        UserAttributesType Attributes
        {
            get;
        }

        /// <summary>
        /// �û���������
        /// </summary>
        OguObjectCollection<IGroup> MemberOf
        {
            get;
        }

        /// <summary>
        /// �û��Ƿ�����ĳһ���򼸸���
        /// </summary>
        /// <param name="groups">һ���򼸸���</param>
        /// <returns>�Ƿ�����ĳһ���򼸸���</returns>
        bool IsInGroups(params IGroup[] groups);

        /// <summary>
        /// �Ƿ��Ǽ�ְ���û���Ϣ
        /// </summary>
        bool IsSideline
        {
            get;
        }

        /// <summary>
        /// ������صģ�������ְ����ְ���û���Ϣ
        /// </summary>
        OguObjectCollection<IUser> AllRelativeUserInfo
        {
            get;
        }

        /// <summary>
        /// �ж��û��Ƿ���ĳ�������������������û���Ϣ���԰������м�ְ��Ϣ
        /// </summary>
        /// <param name="parent">��������</param>
        /// <param name="includeSideline">�Ƿ��жϼ�ְ����</param>
        /// <returns>�Ƿ�����ĳ����</returns>
        bool IsChildrenOf(IOrganization parent, bool includeSideline);

        /// <summary>
        /// ��ǰ�û�������
        /// </summary>
        OguObjectCollection<IUser> Secretaries
        {
            get;
        }

        /// <summary>
        /// ��ǰ�û���˭������
        /// </summary>
        OguObjectCollection<IUser> SecretaryOf
        {
            get;
        }
        #region ��������Ȩ�Ľӿڷ���
        /// <summary>
        /// �û��Ľ�ɫ
        /// </summary>
        UserRoleCollection Roles
        {
            get;
        }

        /// <summary>
        /// �û���Ȩ��
        /// </summary>
        UserPermissionCollection Permissions
        {
            get;
        }
        #endregion
    }

    /// <summary>
    /// ��֯�����Ľӿ�
    /// </summary>
    public interface IOrganization : IOguObject
    {
        /// <summary>
        /// ��������
        /// </summary>
        string CustomsCode
        {
            get;
        }
        /// <summary>
        /// ���ŵ�����
        /// </summary>
        DepartmentTypeDefine DepartmentType
        {
            get;
        }

        /// <summary>
        /// ���ŵ����
        /// </summary>
        DepartmentClassType DepartmentClass
        {
            get;
        }

        /// <summary>
        /// ���ŵļ���
        /// </summary>
        DepartmentRankType Rank
        {
            get;
        }

        /// <summary>
        /// �ò����Ƿ��Ƕ�������
        /// </summary>
        bool IsTopOU
        {
            get;
        }

        /// <summary>
        /// �ò��ŵ���һ���Ӷ���
        /// </summary>
        OguObjectCollection<IOguObject> Children
        {
            get;
        }

        /// <summary>
        /// �õ����е��Ӷ������м�����ȣ�
        /// </summary>
        /// <typeparam name="T">����������</typeparam>
        /// <param name="includeSideLine">�Ƿ������ְ����Ա</param>
        /// <returns>�õ����е��Ӷ������м�����ȣ�</returns>
        OguObjectCollection<T> GetAllChildren<T>(bool includeSideLine) where T : IOguObject;

        /// <summary>
        /// ���Ӷ�����в�ѯ�����м�����ȣ�
        /// </summary>
        /// <typeparam name="T">����������</typeparam>
        /// <param name="matchString">ģ����ѯ���ַ���</param>
        /// <param name="includeSideLine">�Ƿ������ְ����Ա</param>
        /// <param name="level">��ѯ�����</param>
        /// <param name="returnCount">���ص�����</param>
        /// <returns>�õ���ѯ���Ӷ���</returns>
        OguObjectCollection<T> QueryChildren<T>(string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject;
    }

    /// <summary>
    /// ������֯Ӧ���ṩ�Ĺ���
    /// </summary>
    public interface IVirtualOrganization
    {
        /// <summary>
        /// ��ȡ������һ��ֵ����ʾ�Ӳ����Ƿ��ų����ⲿ��
        /// </summary>
        bool ExcludeVirtualDepartment { get; set; }
    }

    /// <summary>
    /// �ڽ�ɫ����֯�����Ľӿ�
    /// </summary>
    public interface IOrganizationInRole : IOrganization
    {
        /// <summary>
        /// �����е���Ա��������
        /// </summary>
        UserRankType AccessLevel
        {
            get;
        }
    }

    /// <summary>
    /// �û���Ľӿ�
    /// </summary>
    public interface IGroup : IOguObject
    {
        /// <summary>
        /// ���ڵ���Ա
        /// </summary>
        OguObjectCollection<IUser> Members
        {
            get;
        }
    }

    /// <summary>
    /// ������Ա��������ʵ�������Ĺ����ӿ�
    /// </summary>
    public interface IOguObjectFactory
    {
        /// <summary>
        /// ���ݽӿ����ʹ�������
        /// </summary>
        /// <param name="type">��Ҫ�����Ķ�������</param>
        /// <returns>����ʵ��</returns>
        IOguObject CreateObject(SchemaType type);
    }
}
