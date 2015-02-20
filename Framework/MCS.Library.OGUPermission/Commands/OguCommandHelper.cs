using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// Ogu相关命令的帮助类，帮助注册
	/// </summary>
	public static class OguCommandHelper
	{
		/// <summary>
		/// 注册所有的命令
		/// </summary>
		public static void RegisterAllCommands()
		{
			CommandHelper.RegisterCommand(new HelpCommand("?"));
			CommandHelper.RegisterCommand(new GetUserCommand("getUser"));
			CommandHelper.RegisterCommand(new GetUserDetailCommand("getUserDetail"));
			CommandHelper.RegisterCommand(new GetObjectByIDCommand("getObjectByID"));
			CommandHelper.RegisterCommand(new GetObjectByFullPath("getObjectByFullPath"));
			CommandHelper.RegisterCommand(new GetUserParentCommand("getUserParent"));
			CommandHelper.RegisterCommand(new GetUserTopOUCommand("getUserTopOU"));
			CommandHelper.RegisterCommand(new GetUserSiblings("getUserSiblings"));
			CommandHelper.RegisterCommand(new GetUserGroups("getUserGroups"));
			CommandHelper.RegisterCommand(new GetGroupUsers("getGroupUsers"));
			CommandHelper.RegisterCommand(new GetUserSecretaries("getUserSecretaries"));
			CommandHelper.RegisterCommand(new GetUserLeaders("getUserLeaders"));
			CommandHelper.RegisterCommand(new GetApplications("getApplications"));
			CommandHelper.RegisterCommand(new GetRoles("getRoles"));
			CommandHelper.RegisterCommand(new GetPermissions("getPermissions"));
			CommandHelper.RegisterCommand(new GetPermissionRelativeRoles("getPermissionRelativeRoles"));
			CommandHelper.RegisterCommand(new GetUserRoles("getUserRoles"));
			CommandHelper.RegisterCommand(new GetUserPermissions("getUserPermissions"));
			CommandHelper.RegisterCommand(new GetRoot("getRoot"));
			CommandHelper.RegisterCommand(new GetAllChildren("getAllChildren"));
			CommandHelper.RegisterCommand(new RemoveOguCache("removeOguCache"));
			CommandHelper.RegisterCommand(new RemovePermissionCache("removePermissionCache"));
		}
	}
}