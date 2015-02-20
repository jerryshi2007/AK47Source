using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WF.GROUP_USERS")]
	public class WfGroupUser
	{
		[ORFieldMapping("GROUP_ID", PrimaryKey = true)]
		public string GroupID
		{
			get;
			set;
		}

        public string UserID
        {
            get
            {
                return _User.ID;
            }
        }
		private IUser _User = null;

		[SubClassORFieldMapping("ID", "USER_ID", PrimaryKey = true)]
		[SubClassORFieldMapping("DisplayName", "USER_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser User
		{
			get
			{
				return this._User;
			}
			set
			{
				this._User = (IUser)OguUser.CreateWrapperObject(value);
			}
		}
	}

	[Serializable]
	public class WfGroupUserCollection : EditableDataObjectCollectionBase<WfGroupUser>
	{
	}
}
