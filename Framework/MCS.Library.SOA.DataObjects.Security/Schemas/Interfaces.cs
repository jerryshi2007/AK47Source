using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security
{
	public interface ISCMemberObject
	{
		SCObjectContainerRelationCollection GetCurrentMemberOfRelations();
	}

	/// <summary>
	/// 应用程序所包含的对象，包括角色和权限
	/// </summary>
	public interface ISCApplicationMember
	{
		/// <summary>
		/// 所属的应用
		/// </summary>
		SCApplication CurrentApplication
		{
			get;
		}
	}

	public interface ISCContainerObject
	{
		SCObjectMemberRelationCollection GetCurrentMembersRelations();
		SchemaObjectCollection GetCurrentMembers();
	}

	/// <summary>
	/// 用户容器需要实现的接口
	/// </summary>
	public interface ISCUserContainerObject
	{
		/// <summary>
		/// 得到所有的用户，这个实现开销有可能很大
		/// </summary>
		/// <returns></returns>
		SchemaObjectCollection GetCurrentUsers();
	}

	/// <summary>
	/// 带条件的用户容器
	/// </summary>
	public interface ISCUserContainerWithConditionObject : ISCUserContainerObject
	{
		/// <summary>
		/// 得到计算好的用户信息（放在SC.ConditionCalculateResult中，直接从这里读取）。
		/// </summary>
		/// <returns></returns>
		SchemaObjectCollection GetCalculatedUsers();

		/// <summary>
		/// 得到包括计算用户的用户，这个实现开销有可能很大。
		/// </summary>
		/// <returns></returns>
		SchemaObjectCollection GetAllCurrentAndCalculatedUsers();
	}

	/// <summary>
	/// 关系类容器需要实现的接口，例如组织
	/// </summary>
	public interface ISCRelationContainer
	{
		[NoMapping]
		[ScriptIgnore]
		SCChildrenRelationObjectCollection AllChildrenRelations
		{
			get;
		}

		[NoMapping]
		[ScriptIgnore]
		SCChildrenRelationObjectCollection CurrentChildrenRelations
		{
			get;
		}

		[NoMapping]
		[ScriptIgnore]
		SchemaObjectCollection AllChildren
		{
			get;
		}

		[NoMapping]
		[ScriptIgnore]
		SchemaObjectCollection CurrentChildren
		{
			get;
		}

		/// <summary>
		/// 获取当前子对象个数
		/// </summary>
		/// <returns></returns>
		int GetCurrentChildrenCount();

		/// <summary>
		/// 获取当前最大的内部序号
		/// </summary>
		/// <returns></returns>
		int GetCurrentMaxInnerSort();
	}

	/// <summary>
	/// Acl容器所必须实现的接口
	/// </summary>
	public interface ISCAclContainer
	{
		/// <summary>
		/// 得到Acl中的成员
		/// </summary>
		/// <returns></returns>
		SCAclMemberCollection GetAclMembers();
	}

	/// <summary>
	/// 表示可继承ACL的容器，此接口仅用于标记
	/// </summary>
	public interface IAllowAclInheritance
	{
	}

	/// <summary>
	/// Acl成员所必须实现的接口
	/// </summary>
	public interface ISCAclMember
	{
		SCAclContainerCollection GetAclContainers();
	}

	/// <summary>
	/// 可提供限定名称的对象
	/// </summary>
	public interface ISCQualifiedNameObject
	{
		string GetQualifiedName();
	}
}
