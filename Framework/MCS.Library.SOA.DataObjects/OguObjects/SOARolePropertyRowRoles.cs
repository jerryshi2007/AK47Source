using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 行中的角色信息
	/// </summary>
	[Serializable]
	public class SOARolePropertyRowRoles
	{
		public SOARolePropertyRowRoles(SOARolePropertyRow rolePropertyRow)
		{
			Row = rolePropertyRow;
		}

		private SOARolePropertyRow _Row;

		public SOARolePropertyRow Row
		{
			get { return this._Row; }
			internal set { _Row = value; }
		}

		private List<IRole> _Roles = new List<IRole>();

		public List<IRole> Roles
		{
			get
			{
				return this._Roles;
			}
		}
	}

	/// <summary>
	/// 行中的角色信息列表
	/// </summary>
	[Serializable]
	public class SOARolePropertyRowRolesCollection : SerializableEditableKeyedDataObjectCollectionBase<SOARolePropertyRow, SOARolePropertyRowRoles>
	{
		protected override SOARolePropertyRow GetKeyForItem(SOARolePropertyRowRoles item)
		{
			return item.Row;
		}
	}
}
