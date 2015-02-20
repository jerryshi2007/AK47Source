using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("USERS_REPORT_LINE")]
	public class UserReportInfo
	{
		private IUser _User = null;

		[SubClassORFieldMapping("ID", "USER_ID", IsNullable = false, PrimaryKey = true)]
		[SubClassORFieldMapping("DisplayName", "USER_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser User
		{
			get { return this._User; }
			set { this._User = value; }
		}

		private IUser _ReportTo = null;

		[SubClassORFieldMapping("ID", "REPORT_TO_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "REPORT_TO_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser ReportTo
		{
			get { return this._ReportTo; }
			set { this._ReportTo = value; }
		}
	}

	[Serializable]
	public class UserReportInfoCollection : EditableDataObjectCollectionBase<UserReportInfo>
	{
		internal void LoadFromDataView(DataView dv)
		{
			this.Clear();

			ORMapping.DataViewToCollection(this, dv);
		}
	}
}
