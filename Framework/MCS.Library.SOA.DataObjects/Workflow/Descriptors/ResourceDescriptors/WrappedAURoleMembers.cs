using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 包装过的角色成员
	/// </summary>
	internal class WrappedAURoleMembers
	{
		public WrappedAURoleMembers()
		{
		}

		public WrappedAURoleMembers(string auCodeName, IEnumerable<IUser> members)
		{
			this.AUCodeName = auCodeName;
			this.Members = members;
		}

		public string AUCodeName
		{
			get;
			set;
		}

		public IEnumerable<IUser> Members
		{
			get;
			internal set;
		}
	}
}
