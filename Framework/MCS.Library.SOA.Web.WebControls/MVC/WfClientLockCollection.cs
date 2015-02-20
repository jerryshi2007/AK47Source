using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.Library.MVC
{
	[Serializable]
	public class WfClientLockCollection : SerializableEditableKeyedDataObjectCollectionBase<LockType, Lock>
	{
		protected override LockType GetKeyForItem(Lock item)
		{
			return item.LockType;
		}
	}
}
