#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	PermissionObjectInterfaces.cs
// Remark	：	授权对象接口的公共基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 授权对象接口的公共基类
	/// </summary>
	public interface IPermissionObject
	{
		/// <summary>
		/// 对象的ID
		/// </summary>
		string ID
		{
			get;
		}

		/// <summary>
		/// 对象的名称
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 对象的英文标识
		/// </summary>
		string CodeName
		{
			get;
		}

		/// <summary>
		/// 对象的描述
		/// </summary>
		string Description
		{
			get;
		}
	}

	/// <summary>
	/// 授权系统中，应用的接口定义
	/// </summary>
	public interface IApplication : IPermissionObject
	{
		/// <summary>
		/// 应用的级别深度
		/// </summary>
		string ResourceLevel
		{
			get;
		}

		/// <summary>
		/// 应用中的角色
		/// </summary>
		RoleCollection Roles
		{
			get;
		}

		/// <summary>
		/// 应用中的功能
		/// </summary>
		PermissionCollection Permissions
		{
			get;
		}
	}

	/// <summary>
	/// 授权系统中，应用中所包含的对象的接口定义
	/// </summary>
	public interface IApplicationObject : IPermissionObject
	{
		/// <summary>
		/// 对象所属的应用程序
		/// </summary>
		IApplication Application
		{
			get;
		}

		/// <summary>
		/// 包含AppCode的全CodeName，格式如AppCode:CodeName
		/// </summary>
		string FullCodeName
		{
			get;
		}
	}

	/// <summary>
	/// 授权系统中，角色的接口定义
	/// </summary>
	public interface IRole : IApplicationObject
	{
		/// <summary>
		/// 角色中的对象
		/// </summary>
		OguObjectCollection<IOguObject> ObjectsInRole
		{
			get;
		}
	}

	/// <summary>
	/// 授权系统中，功能的接口定义
	/// </summary>
	public interface IPermission : IApplicationObject
	{
		/// <summary>
		/// 得到相关的角色
		/// </summary>
		RoleCollection RelativeRoles
		{
			get;
		}
	}

	/// <summary>
	/// 通用授权对象的实例创建的工厂接口
	/// </summary>
	public interface IPermissionObjectFactory
	{
		/// <summary>
		/// 根据接口类型创建对象
		/// </summary>
		/// <param name="type">需要创建的对象类型</param>
		/// <returns>对象实例</returns>
		IPermissionObject CreateObject(System.Type type);
	}
}
