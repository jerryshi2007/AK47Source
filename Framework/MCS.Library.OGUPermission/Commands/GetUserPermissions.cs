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
	public class GetUserPermissions : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetUserPermissions(string name)
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

			ExceptionHelper.FalseThrow(args.Length > 0, "查询人员的权限必须提供登录名");

			OguObjectCollection<IUser> users = QueryHelper.QueryUser(args[0]);

			foreach (IUser user in users)
			{
				if (args.Length > 1)
				{
					user.Permissions[args[1]].ToList().ForEach(obj => OutputHelper.OutputPermissionInfo(obj));
				}
				else
				{
					Dictionary<IApplication, PermissionCollection> appRoles = user.Permissions.GetAllAppPermissions();

					foreach (KeyValuePair<IApplication, PermissionCollection> kp in appRoles)
					{
						Console.WriteLine("Application");
						OutputHelper.OutputPermissionInfo(kp.Key);

						if (kp.Value.Count > 0)
						{
							Console.WriteLine("Permissions");
							kp.Value.ForEach(obj => OutputHelper.OutputPermissionInfo(obj));
						}
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
				return "getUserPermissions {userlogonName} {appCodeName}";
			}
		}
	}
}
