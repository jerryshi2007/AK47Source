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
	[ORTableMapping("WF.POST")]
	public class WfPost
	{
		[ORFieldMapping("POST_ID", PrimaryKey = true)]
		public string PostID
		{
			get;
			set;
		}

		[ORFieldMapping("POST_NAME")]
		public string PostName
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
	}

	public class WfPostCollection : EditableDataObjectCollectionBase<WfPost>
	{
	}
}
