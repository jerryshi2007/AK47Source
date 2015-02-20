using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class ModifiedItem
	{
		public IOguObject OguObjectData
		{
			get;
			set;
		}

		public ADObjectWrapper ADObjectData
		{
			get;
			set;
		}

		public ObjectModifyType ModifyType
		{
			get;
			set;
		}
	}

	public class ModifiedItemgCollection : EditableDataObjectCollectionBase<ModifiedItem>
	{
		public bool ExistedADObject(string adGuid)
		{
			return this.Exists(m => m.ADObjectData != null && m.ADObjectData.NativeGuid == adGuid);
		}
	}
}