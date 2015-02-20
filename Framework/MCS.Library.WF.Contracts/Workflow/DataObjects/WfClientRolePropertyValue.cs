using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
	[DataContract]
	[Serializable]
	public class WfClientRolePropertyValue : RowValueBase<WfClientRolePropertyDefinition, string>
	{
		public WfClientRolePropertyValue()
		{
		}

		public WfClientRolePropertyValue(WfClientRolePropertyDefinition rpd)
		{
			rpd.NullCheck("rpd");

			this.Column = rpd;
		}

		public void SetColumnInfo(WfClientRolePropertyDefinition rpd)
		{
			rpd.NullCheck("rpd");

			this.Column = rpd;
		}
	}

	[DataContract]
	[Serializable]
	public class WfClientRolePropertyValueCollection : RowValueCollectionBase<WfClientRolePropertyDefinition, WfClientRolePropertyValue, string>
	{
	}
}
