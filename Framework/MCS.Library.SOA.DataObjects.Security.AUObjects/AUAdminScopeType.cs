using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示一种管理范围类型
	/// </summary>
	[Serializable]
	public class AUAdminScopeType
	{
		public string SchemaType { get; set; }
		public string SchemaName { get; set; }
	}

	[Serializable]
	public class AUAdminScopeTypeCollection : EditableDataObjectCollectionBase<AUAdminScopeType>
	{

	}
}
