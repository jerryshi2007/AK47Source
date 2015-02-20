using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Data;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// 
	/// </summary>
	public class GetUserRoles : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetUserRoles(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			string[] args = argument.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			ExceptionHelper.FalseThrow(args.Length > 0, "查询人员的角色必须提供登录名");

			OguObjectCollection<IUser> users = QueryHelper.QueryUser(args[0]);

			foreach (IUser user in users)
			{
				if (args.Length > 1)
				{
					user.Roles[args[1]].ToList().ForEach(obj => OutputHelper.OutputPermissionInfo(obj));
				}
				else
				{
                    List<IRole> roles = user.Roles.GetAllRoles();

                    foreach (IRole role in roles)
                    {
                        if (role.Application != null)
                        {
                            Console.WriteLine("Application");
                            OutputHelper.OutputPermissionInfo(role.Application);
                        }

                        Console.WriteLine("Role");
                        OutputHelper.OutputPermissionInfo(role);
                    }
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getUserRoles {userlogonName} [appCodeName]";
			}
		}
	}
}
