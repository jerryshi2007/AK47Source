#region
// -------------------------------------------------
// Assembly	：	HB.DataEntities
// FileName	：	MaterialList.cs
// Remark	：	Material集合类
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070724		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Properties;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// Material集合类
	/// </summary>
	[Serializable]
	public class MaterialList : EditableDataObjectCollectionBase<Material>
	{
		/// <summary>
		/// 生成副本
		/// </summary>
		/// <returns>生成的副本</returns>
		internal MaterialList GenerateCopyVersion()
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");

			MaterialList materialList = new MaterialList();

			foreach (Material material in this)
				materialList.Add(material.GenerateCopyVersion());

			return materialList;
		}

		/// <summary>
		/// 产生文件版本
		/// </summary>
		/// <param name="resourceID">新表单ID</param>
		/// <param name="wfProcessID">新工作流ID</param>
		/// <param name="wfActivityID">新工作流节点ID</param>
		/// <param name="wfActivityName">新工作流节点名称</param>
		/// <param name="department">产生的副本所在的部门</param>
		/// <returns>生成的副本</returns>
		public MaterialList GenerateOtherVersion(string resourceID, string wfProcessID, string wfActivityID, string wfActivityName, IOrganization department)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");
			ExceptionHelper.CheckStringIsNullOrEmpty(resourceID, "resourceID");
			ExceptionHelper.CheckStringIsNullOrEmpty(wfProcessID, "wfProcessID");
			ExceptionHelper.CheckStringIsNullOrEmpty(wfActivityID, "wfActivityID");

			MaterialList materialList = new MaterialList();

			foreach (Material material in this)
				materialList.Add(material.GenerateOtherVersion(resourceID, wfProcessID, wfActivityID, wfActivityName, department));

			return materialList;
		}

		/// <summary>
		/// 由类型分组
		/// </summary>
		/// <returns></returns>
		public MaterialList GetMaterialsByClass(string className)
		{
			MaterialList materials = new MaterialList();

			foreach (Material m in this)
			{
				if (m.MaterialClass == className)
					materials.Add(m);
			}

			return materials;
		}

		/// <summary>
		/// 附加新的list
		/// </summary>
		/// <param name="materials">要附加的MaterialList</param>
		public void Append(MaterialList materials)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(materials == null, "materials");

			if (materials.Count != 0)
			{
				foreach (Material material in materials)
					this.Add(material);
			}
		}

		/// <summary>
		/// 将变化的的附件合并到MaterialList中
		/// </summary>
		/// <param name="delta"></param>
		public void Merge(DeltaMaterialList delta)
		{
			delta.NullCheck("delta");

			MaterialList copiedMaterialed = new MaterialList();

			copiedMaterialed.CopyFrom(this);

			copiedMaterialed.RemoveExistsMaterials(delta.Inserted);
			copiedMaterialed.RemoveExistsMaterials(delta.Updated);
			copiedMaterialed.RemoveExistsMaterials(delta.Deleted);
			copiedMaterialed.CopyFrom(delta.Inserted);
			copiedMaterialed.CopyFrom(delta.Updated);

			copiedMaterialed.Sort((m1, m2) => m1.SortID - m2.SortID);

			this.Clear();
			this.CopyFrom(copiedMaterialed);
		}

		#region Merge相关私有方法
		/// <summary>
		/// 删除在compareList中已经存在的Material
		/// </summary>
		/// <param name="compareList"></param>
		private void RemoveExistsMaterials(MaterialList compareList)
		{
			foreach (Material cm in compareList)
			{
				this.Remove(m => string.Compare(m.ID, cm.ID, true) == 0);
			}
		}

		#endregion Merge相关私有方法

		/// <summary>
		/// 初始化，或者准备MaterialContent
		/// </summary>
		public void EnsureMaterialContent()
		{
			this.ForEach(m => m.EnsureMaterialContent());
		}

		/// <summary>
		/// 复制一个附件列表
		/// </summary>
		/// <returns></returns>
		public MaterialList Clone()
		{
			MaterialList result = new MaterialList();

			this.ForEach(m => result.Add(m.Clone()));

			return result;
		}

		/// <summary>
		/// 得到文件的物理路径
		/// </summary>
		/// <param name="rootPathName"></param>
		public void GenerateTempPhysicalFilePath(string rootPathName)
		{
			this.ForEach(m => m.GenerateTempPhysicalFilePath(rootPathName));
		}

		#region 构造函数

		public MaterialList()
		{

		}

		#endregion

		#region 集合操作

		public void Insert(int index, Material material)
		{
			List.Insert(index, material);
		}

        //public void Remove(Material material)
        //{
        //    List.Remove(material);
        //}

		#endregion

		#region 重载基类的方法

		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
			ExceptionHelper.FalseThrow(
				value.GetType() == typeof(Material),
				string.Format(Resource.CollectionValidateError, typeof(Material)));

			ExceptionHelper.CheckStringIsNullOrEmpty(((Material)value).ID, "ID");
			ExceptionHelper.CheckStringIsNullOrEmpty(((Material)value).Title, "Title");
			ExceptionHelper.CheckStringIsNullOrEmpty(((Material)value).RelativeFilePath, "RelativeFilePath");
		}

		#endregion

	}

}