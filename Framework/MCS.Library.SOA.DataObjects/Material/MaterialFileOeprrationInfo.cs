using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 文件操作类型
	/// </summary>
	public enum FileOperation
	{
		/// <summary>
		/// 新增
		/// </summary>
		Add,
		/// <summary>
		/// 修改
		/// </summary>
		Update,
		/// <summary>
		/// 删除
		/// </summary>
		Delete
	}

	/// <summary>
	/// 文件操作信息
	/// </summary>
	public class MaterialFileOeprationInfo
	{
		private Material material;
		private FileOperation operation;

		public MaterialFileOeprationInfo(Material m, FileOperation op)
		{
			material = m;
			operation = op;
		}

		/// <summary>
		/// 附件对象
		/// </summary>
		public Material Material
		{
			get
			{
				return this.material;
			}
		}
		/// <summary>
		/// 操作
		/// </summary>
		public FileOperation Operation
		{
			get
			{
				return this.operation;
			}
		}
	}
}
