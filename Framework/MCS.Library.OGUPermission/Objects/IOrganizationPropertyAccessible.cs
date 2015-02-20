using System;
namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 表示Organization对象的属性访问功能
	/// </summary>
	public interface IOrganizationPropertyAccessible : IOguPropertyAccessible
	{
		/// <summary>
		/// 获取一个集合，表示机构的子级对象
		/// </summary>
		MCS.Library.OGUPermission.OguObjectCollection<MCS.Library.OGUPermission.IOguObject> Children { get; }

		/// <summary>
		/// 或取或设置关区号
		/// </summary>
		string CustomsCode { get; set; }

		/// <summary>
		/// 获取或设置部门分类
		/// </summary>
		MCS.Library.OGUPermission.DepartmentClassType DepartmentClass { get; set; }

		/// <summary>
		/// 获取或设置部门的特殊属性
		/// </summary>
		MCS.Library.OGUPermission.DepartmentTypeDefine DepartmentType { get; set; }

		/// <summary>
		/// 获取一个值，表示此组织是否顶级组织
		/// </summary>
		bool IsTopOU { get; }

		/// <summary>
		/// 获取或设置
		/// </summary>
		MCS.Library.OGUPermission.DepartmentRankType Rank { get; set; }
	}
}
