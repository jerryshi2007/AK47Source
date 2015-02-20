using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 机构人员控件相关的查询接口定义，以后可以定义
	/// </summary>
	public interface IUserOUControlQuery
	{
		/// <summary>
		/// 根据ID查询对象的父对象信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Dictionary<string, IEnumerable<IOrganization>> QueryObjectsParents(params string[] ids);

		/// <summary>
		/// 得到用户的扩展信息
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		UserInfoExtendCollection QueryUsersExtendedInfo(params string[] ids);

		/// <summary>
		/// 得到用户的即时消息地址
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		UserIMAddressCollection QueryUsersIMAddress(params string[] ids);

		/// <summary>
		/// 获取指定组织的下级对象
		/// </summary>
		/// <param name="parent">上级组织</param>
		/// <returns></returns>
		IEnumerable<IOguObject> GetChildren(IOrganization parent);

		/// <summary>
		/// 根据指定的前缀和路径，检索所有子代对象
		/// </summary>
		/// <typeparam name="T">返回对象的类型</typeparam>
		/// <param name="type"><see cref="SchemaQueryType"/>的按位组合值，表示包含进查询的对象类型</param>
		/// <param name="parent">父级组织</param>
		/// <param name="prefix">查询前缀</param>
		/// <param name="maxCount">最大返回结果条数</param>
		/// <returns></returns>
		OguObjectCollection<T> QueryDescendants<T>(SchemaQueryType type, IOrganization parent, string prefix, int maxCount) where T : IOguObject;

		/// <summary>
		/// 根据指定的根节点路径，获取根组织
		/// </summary>
		/// <param name="rootPath">根组织的全路径（尽管提供了此参数，也可能会被忽略）</param>
		/// <returns></returns>
		IOrganization GetOrganizationByPath(string rootPath);

		/// <summary>
		/// 根据ID查找对象
		/// </summary>
		/// <param name="ids">一个或多个ID</param>
		/// <returns></returns>
		OguObjectCollection<IOguObject> GetObjects(params string[] ids);
	}

	/// <summary>
	/// 查询的模式类型
	/// </summary>
	[Flags]
	public enum SchemaQueryType
	{
		None = 0,
		/// <summary>
		/// 包含用户查询
		/// </summary>
		Users = 1,
		/// <summary>
		/// 包含群组的查询
		/// </summary>
		Groups = 2,
		/// <summary>
		/// 包含组织的查询
		/// </summary>
		Organizations = 4,
		/// <summary>
		/// 包含所有可查询的类型
		/// </summary>
		All = ~0,
	}
}
