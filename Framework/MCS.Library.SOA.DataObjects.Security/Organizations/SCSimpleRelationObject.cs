using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示简单的对象关系
	/// </summary>
	[Serializable]
	public class SCSimpleRelationObject : SCSimpleObject
	{
		/// <summary>
		/// 获取或设置父级ID
		/// </summary>
		[ORFieldMapping("ParentID", PrimaryKey = true)]
		public virtual string ParentID
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置ID
		/// </summary>
		[ORFieldMapping("ObjectID", PrimaryKey = true)]
		public override string ID
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 表示简单对象关系<see cref="SCSimpleRelationObject"/>的集合
	/// </summary>
	[Serializable]
	public class SCSimpleRelationObjectCollection : EditableDataObjectCollectionBase<SCSimpleRelationObject>
	{
	}
}
