#region
// -------------------------------------------------
// Assembly	��	HB.DataEntities
// FileName	��	MaterialTree.cs
// Remark	��	�����汾��
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070821		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Properties;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	public class MaterialTreeNode : TreeNodeBase<MaterialTreeNode, MaterialTreeNodeCollection>
	{
		private Material material;

		public MaterialTreeNode(Material m)
		{
			this.material = m;
		}

		public Material Material
		{
			get
			{
				return this.material;
			}
			set
			{
				this.material = value;
			}
		}
	}

	public class MaterialTreeNodeCollection : TreeNodeBaseCollection<MaterialTreeNode, MaterialTreeNodeCollection>
	{
	}

}
