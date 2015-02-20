using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	[Serializable]
	[ORTableMapping("WeChat.Groups")]
	public class WeChatGroup
	{
		[ORFieldMapping("AccountID", PrimaryKey = true)]
		public string AccountID
		{
			get;
			set;
		}

		[ORFieldMapping("GroupID", PrimaryKey = true)]
		public int GroupID
		{
			get;
			set;
		}

		[ORFieldMapping("Name")]
		public string Name
		{
			get;
			set;
		}

		[ORFieldMapping("Count")]
		public int Count
		{
			get;
			set;
		}

		[ORFieldMapping("CreateTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime CreateTime
		{
			get;
			set;
		}

		[ORFieldMapping("UpdateTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime UpdateTime
		{
			get;
			set;
		}
	}

	[Serializable]
	public class WeChatGroupCollection : EditableDataObjectCollectionBase<WeChatGroup>
	{
	}
}
