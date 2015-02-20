using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// 
	/// </summary>
	public class GetUserGroups : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetUserGroups(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			OguObjectCollection<IUser> users = QueryHelper.QueryUser(argument);
			List<IUser> userList = new List<IUser>();

			foreach (IUser user in users)
			{
				if (userList.Exists(u => u.ID == user.ID) == false)
					userList.Add(user);
			}

			foreach (IUser user in userList)
				user.MemberOf.ToList().ForEach(obj => OutputHelper.OutputObjectInfo(obj));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getUserGroups {userLogonName}";
			}
		}
	}
}
