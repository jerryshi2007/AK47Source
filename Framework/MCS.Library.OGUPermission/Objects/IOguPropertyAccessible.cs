using System;
namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 表示OGU对象的属性访问器
	/// </summary>
	public interface IOguPropertyAccessible
	{
		/// <summary>
		/// 获取或设置描述
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// 获取或设置显示名称
		/// </summary>
		string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置全路径
		/// </summary>
		string FullPath { get; set; }

		/// <summary>
		/// 获取或设置全局排序ID
		/// </summary>
		string GlobalSortID { get; set; }

		/// <summary>
		/// 获取或设置ID
		/// </summary>
		string ID { get; set; }

		/// <summary>
		/// 获取级别
		/// </summary>
		int Levels { get; }

		/// <summary>
		/// 获取或设置名称
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 获取或设置对象类型
		/// </summary>
		MCS.Library.OGUPermission.SchemaType ObjectType { get; set; }

		/// <summary>
		/// 获取父级
		/// </summary>
		MCS.Library.OGUPermission.IOrganization Parent { get; set; }

		/// <summary>
		/// 获取属性的集合
		/// </summary>
		System.Collections.IDictionary Properties { get; }

		/// <summary>
		/// 获取或设置排序ID
		/// </summary>
		string SortID { get; set; }

		/// <summary>
		/// 获取或设置顶级OU
		/// </summary>
		MCS.Library.OGUPermission.IOrganization TopOU { get; set; }
	}
}
