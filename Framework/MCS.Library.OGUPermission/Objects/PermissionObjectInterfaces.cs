#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	PermissionObjectInterfaces.cs
// Remark	��	��Ȩ����ӿڵĹ�������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// ��Ȩ����ӿڵĹ�������
	/// </summary>
	public interface IPermissionObject
	{
		/// <summary>
		/// �����ID
		/// </summary>
		string ID
		{
			get;
		}

		/// <summary>
		/// ���������
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// �����Ӣ�ı�ʶ
		/// </summary>
		string CodeName
		{
			get;
		}

		/// <summary>
		/// ���������
		/// </summary>
		string Description
		{
			get;
		}
	}

	/// <summary>
	/// ��Ȩϵͳ�У�Ӧ�õĽӿڶ���
	/// </summary>
	public interface IApplication : IPermissionObject
	{
		/// <summary>
		/// Ӧ�õļ������
		/// </summary>
		string ResourceLevel
		{
			get;
		}

		/// <summary>
		/// Ӧ���еĽ�ɫ
		/// </summary>
		RoleCollection Roles
		{
			get;
		}

		/// <summary>
		/// Ӧ���еĹ���
		/// </summary>
		PermissionCollection Permissions
		{
			get;
		}
	}

	/// <summary>
	/// ��Ȩϵͳ�У�Ӧ�����������Ķ���Ľӿڶ���
	/// </summary>
	public interface IApplicationObject : IPermissionObject
	{
		/// <summary>
		/// ����������Ӧ�ó���
		/// </summary>
		IApplication Application
		{
			get;
		}

		/// <summary>
		/// ����AppCode��ȫCodeName����ʽ��AppCode:CodeName
		/// </summary>
		string FullCodeName
		{
			get;
		}
	}

	/// <summary>
	/// ��Ȩϵͳ�У���ɫ�Ľӿڶ���
	/// </summary>
	public interface IRole : IApplicationObject
	{
		/// <summary>
		/// ��ɫ�еĶ���
		/// </summary>
		OguObjectCollection<IOguObject> ObjectsInRole
		{
			get;
		}
	}

	/// <summary>
	/// ��Ȩϵͳ�У����ܵĽӿڶ���
	/// </summary>
	public interface IPermission : IApplicationObject
	{
		/// <summary>
		/// �õ���صĽ�ɫ
		/// </summary>
		RoleCollection RelativeRoles
		{
			get;
		}
	}

	/// <summary>
	/// ͨ����Ȩ�����ʵ�������Ĺ����ӿ�
	/// </summary>
	public interface IPermissionObjectFactory
	{
		/// <summary>
		/// ���ݽӿ����ʹ�������
		/// </summary>
		/// <param name="type">��Ҫ�����Ķ�������</param>
		/// <returns>����ʵ��</returns>
		IPermissionObject CreateObject(System.Type type);
	}
}
