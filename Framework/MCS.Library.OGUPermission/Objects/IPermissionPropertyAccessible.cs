using System;
namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 表示Permission对象的属性访问功能
	/// </summary>
	public interface IPermissionPropertyAccessible
	{
		/// <summary>
		/// 获取或设置代码名称
		/// </summary>
		string CodeName { get; set; }

		/// <summary>
		/// 获取或设置描述
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// 获取或设置ID
		/// </summary>
		string ID { get; set; }

		/// <summary>
		/// 获取或设置名称
		/// </summary>
		string Name { get; set; }
	}
}
