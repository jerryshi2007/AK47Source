using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.APPLICATIONS")]
	public class WfApplication
	{
		[ORFieldMapping("CODE_NAME", PrimaryKey = true)]
		public string CodeName
		{
			get;
			set;
		}

		[ORFieldMapping("NAME")]
		public string Name
		{
			get;
			set;
		}

		[ORFieldMapping("SORT")]
		public int Sort
		{
			get;
			set;
		}
	}

	[Serializable]
	public class WfApplicationCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfApplication>
	{
		public WfApplicationCollection()
		{
		}

		protected WfApplicationCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override string GetKeyForItem(WfApplication item)
		{
			return item.CodeName;
		}
	}
}
