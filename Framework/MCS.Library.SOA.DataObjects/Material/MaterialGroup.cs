#region
// -------------------------------------------------
// Assembly	：	HB.DataEntities
// FileName	：	MaterialClassGroup.cs
// Remark	：	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20071113		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Data;
using System.IO;
using System.Collections.Generic;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Properties;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	public class MaterialGroup : GroupNode<string, MaterialList>
	{

	}

	public class MaterialGroupCollection : GroupNodeCollection<MaterialGroup, string, MaterialList>
	{
		private class MaterialComparer : IComparer<MaterialGroup>
		{

			#region IComparer<Material> 成员

			public int Compare(MaterialGroup x, MaterialGroup y)
			{
				return string.Compare(x.GroupKey, y.GroupKey);
			}

			#endregion
		}

		public bool TryGetValue(string materialClass, out MaterialList materials)
		{
			MaterialGroup materialClassGroup = new MaterialGroup();

			if (base.TryGetValue(materialClass, out materialClassGroup))
			{
				materials = materialClassGroup.Data;
				return true;
			}
			else
			{
				materials = new MaterialList();
				return false;
			}
		}

		/// <summary>
		/// 按照附件类别分组
		/// </summary>
		/// <param name="materials">附件集合</param>
		public void FillGroupByMaterialClass(MaterialList materials)
		{
			base.FillData<Material>(materials,
				delegate(Material material)
				{
					return material.MaterialClass;
				}
			);
		}

		/// <summary>
		/// 按照表单ID分组
		/// </summary>
		/// <param name="materials">附件集合</param>
        public void FillGroupByResourceID(MaterialList materials)
        {
            base.FillData<Material>(materials,
                delegate(Material material)
                {
                    return material.ResourceID;
                }
            );
        }

		/// <summary>
		/// 按照附件类别排序
		/// </summary>
		public void SortByClassName()
		{
			this.SortGroups(new MaterialComparer());
		}
	}

}
