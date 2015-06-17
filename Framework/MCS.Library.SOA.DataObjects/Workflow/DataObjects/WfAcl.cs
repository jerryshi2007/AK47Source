using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 资源的Acl项
	/// </summary>
    [Serializable]
	[ORTableMapping("WF.ACL")]
    [TenantRelativeObject]
	public class WfAclItem
	{
		[ORFieldMapping("RESOURCE_ID", PrimaryKey = true)]
		public string ResourceID
		{
			get;
			set;
		}

		[ORFieldMapping("OBJECT_ID", PrimaryKey = true)]
		public string ObjectID
		{
			get;
			set;
		}

		[ORFieldMapping("OBJECT_TYPE")]
		public string ObjectType
		{
			get;
			set;
		}

		[ORFieldMapping("OBJECT_NAME")]
		public string ObjectName
		{
			get;
			set;
		}

		[ORFieldMapping("SOURCE")]
		public string Source
		{
			get;
			set;
		}
	}

	/// <summary>
	/// Acl条目的集合
	/// </summary>
    [Serializable]
	public class WfAclItemCollection : EditableDataObjectCollectionBase<WfAclItem>
	{
	}
}
