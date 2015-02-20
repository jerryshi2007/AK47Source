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
	[ORTableMapping("WF.[GROUP]")]
	public class WfGroup
	{
		[ORFieldMapping("GROUP_ID", PrimaryKey = true)]
		public string GroupID
		{
			get;
			set;
		}

		[ORFieldMapping("GROUP_NAME")]
		public string GroupName
		{
			get;
			set;
		}

		[ORFieldMapping("CATEGORY")]
		public string Category
		{
			get;
			set;
		}

		[ORFieldMapping("CREATE_TIME")]
		public DateTime CreateTime
		{
			get;
			set;
		}

		private IUser _Creator = null;

		[SubClassORFieldMapping("ID", "CREATOR_ID")]
		[SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		private IUser _Manager = null;

		[SubClassORFieldMapping("ID", "MANAGER_ID")]
		[SubClassORFieldMapping("DisplayName", "MANAGER_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser Manager
		{
			get
			{
				return this._Manager;
			}
			set
			{
				this._Manager = (IUser)OguUser.CreateWrapperObject(value);
			}
		}
	}

	public class WfGroupCollection : EditableDataObjectCollectionBase<WfGroup>
	{
	}
}
