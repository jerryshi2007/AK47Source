using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	public class WfMatrixRowUsers
	{
		public WfMatrixRowUsers(WfMatrixRow wfMatrixRow)
		{
			Row = wfMatrixRow;
		}

		private WfMatrixRow _Row;

		public WfMatrixRow Row
		{
			get { return this._Row; }
			internal set { _Row = value; }
		}

		private OguDataCollection<IUser> _Users = new OguDataCollection<IUser>();

		public OguDataCollection<IUser> Users
		{
			get
			{
				return this._Users;
			}
		}

		internal List<string> ObjectIDs = null;
	}

	public class WfMatrixRowUsersCollection : SerializableEditableKeyedDataObjectCollectionBase<WfMatrixRow, WfMatrixRowUsers>
	{
		protected override WfMatrixRow GetKeyForItem(WfMatrixRowUsers item)
		{
			return item.Row;
		}
	}

}
