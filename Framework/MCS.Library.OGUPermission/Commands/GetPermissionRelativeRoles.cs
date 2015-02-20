using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// 得到权限的相关角色
	/// </summary>
	public class GetPermissionRelativeRoles : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetPermissionRelativeRoles(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			string[] arguments = argument.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

			ExceptionHelper.FalseThrow(arguments.Length > 1, "参数太少，必须提供两个参数，用逗号或空格分开");

			ApplicationCollection applications = PermissionMechanismFactory.GetMechanism().GetApplications(arguments[0]);

			ExceptionHelper.FalseThrow(applications.Count > 0, "不能查询到CodeName为\"{0}\"应用", arguments[0]);

			ExceptionHelper.FalseThrow(applications[0].Permissions.ContainsKey(arguments[1]),
				"在应用\"{0}\"中不能查询到CodeName为\"{1}\"的权限", arguments[0], arguments[1]);

			applications[0].Permissions[arguments[1]].RelativeRoles.ForEach(r => OutputHelper.OutputPermissionInfo(r));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getPermissionRelativeRoles {appCodeName} {permissionCodeName}";
			}
		}
	}
}
