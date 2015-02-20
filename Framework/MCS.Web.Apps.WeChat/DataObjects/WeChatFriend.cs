using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 好友信息
	/// </summary>
	[Serializable]
	[ORTableMapping("WeChat.Friends")]
	public class WeChatFriend
	{
		/// <summary>
		/// FakeID
		/// </summary>
		[ORFieldMapping("FakeID", PrimaryKey = true)]
		public string FakeID
		{
			get;
			set;
		}

		/// <summary>
		/// AccountID
		/// </summary>
		[ORFieldMapping("AccountID", PrimaryKey = true)]
		public string AccountID
		{
			get;
			set;
		}

		/// <summary>
		/// nick_name
		/// </summary>
		[ORFieldMapping("NickName")]
		public string NickName
		{
			get;
			set;
		}

		/// <summary>
		/// remark_name
		/// </summary>
		[ORFieldMapping("RemarkName")]
		public string RemarkName
		{
			get;
			set;
		}

		/// <summary>
		/// OpenID
		/// </summary>
		[ORFieldMapping("OpenID")]
		public string OpenID
		{
			get;
			set;
		}

		/// <summary>
		/// group_id
		/// </summary>
		[ORFieldMapping("GroupID")]
		public int GroupID
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
	public class WeChatFriendCollection : EditableDataObjectCollectionBase<WeChatFriend>
	{
	}
}
