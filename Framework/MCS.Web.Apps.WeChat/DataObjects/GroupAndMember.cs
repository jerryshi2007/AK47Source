using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	[Serializable]
    [ORTableMapping("Biz.GroupsAndMembers")]
    public class GroupAndMember
	{
        [ORFieldMapping("GroupID", PrimaryKey = true)]
		public string GroupID
		{
			get;
			set;
		}

        [ORFieldMapping("MemberID",PrimaryKey = true)]
		public string MemberID
		{
			get;
			set;
		}
	}

	[Serializable]
    public class GroupAndMemberCollection : EditableDataObjectCollectionBase<GroupAndMember>
	{
	}
}
