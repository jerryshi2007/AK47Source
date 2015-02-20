using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// 
	/// </summary>
	public class GetUserParentCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetUserParentCommand(string name)
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

			foreach (IUser user in users)
            {
                if (user.Parent != null)
                    OutputHelper.OutputObjectInfo(user.Parent);
                else
                    Console.WriteLine("User doesn't have parent.");
            }
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getUserParent {userLogonName}";
			}
		}
	}
}
