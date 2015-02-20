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
	public class GetRoles : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetRoles(string name)
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
			
			applications[0].Roles.ForEach(role => OutputHelper.OutputPermissionInfo(role));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getRoles {appCodeName}";
			}
		}
	}
}
