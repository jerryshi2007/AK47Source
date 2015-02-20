using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 组的实现类
	/// </summary>
	[Serializable]
	public class OguGroupImpl : OguBaseImpl, IGroup
	{
		private OguObjectCollection<IUser> members = null;

		#region Sync Objects
		private object membersSyncObj = new object();
		#endregion

		/// <summary>
		/// 构造方法
		/// </summary>
		public OguGroupImpl()
		{
		}

		#region IGroup 成员

		/// <summary>
		/// 组成员
		/// </summary>
		public OguObjectCollection<IUser> Members
		{
			get
			{
				if (this.members == null)
				{
					lock (this.membersSyncObj)
					{
						if (this.members == null)
							this.members = OguPermissionSettings.GetConfig().OguObjectImpls.GetGroupMembers(this);
					}
				}

				return this.members;
			}
		}

		#endregion
	}
}
