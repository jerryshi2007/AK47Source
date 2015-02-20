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
	public class GetPermissions : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetPermissions(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			ApplicationCollection applications = PermissionMechanismFactory.GetMechanism().GetApplications(argument);

			ExceptionHelper.FalseThrow(applications.Count > 0, "不能查询到CodeName为\"{0}\"应用", argument);

			applications[0].Permissions.ForEach(p => OutputHelper.OutputPermissionInfo(p));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getPermissions {appCodeName}";
			}
		}
	}
}
