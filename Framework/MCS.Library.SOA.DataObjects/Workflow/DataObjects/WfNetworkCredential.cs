using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System.Net;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfNetworkCredential : LogOnIdentity
	{
		private string _Key = string.Empty;

		public string Key
		{
			get
			{
				return this._Key;
			}
			set
			{
				this._Key = value;
			}
		}

		public static explicit operator NetworkCredential(WfNetworkCredential credential)
		{
			return new NetworkCredential(credential.LogOnName, credential.Password, credential.Domain);
		}
	}

	[Serializable]
	[XElementSerializable]
	public class WfNetworkCredentialCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfNetworkCredential>
	{
		protected override string GetKeyForItem(WfNetworkCredential item)
		{
			return item.Key;
		}
	}

}
