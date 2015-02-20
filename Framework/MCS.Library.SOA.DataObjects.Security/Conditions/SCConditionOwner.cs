using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	[Serializable]
	public class SCConditionOwner
	{
		private SCConditionCollection _Conditions = null;

		public string OwnerID
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public SCConditionCollection Conditions
		{
			get
			{
				if (this._Conditions == null)
					this._Conditions = new SCConditionCollection();

				return this._Conditions;
			}
		}
	}

	[Serializable]
	public class SCConditionOwnerCollection : SerializableEditableKeyedDataObjectCollectionBase<string, SCConditionOwner>
	{
		public SCConditionOwnerCollection()
		{
		}

		protected SCConditionOwnerCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override string GetKeyForItem(SCConditionOwner item)
		{
			return item.OwnerID;
		}
	}
}
