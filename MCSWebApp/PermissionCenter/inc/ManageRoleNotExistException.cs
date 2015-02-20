using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter
{
	/// <summary>
	/// 表示不存在管理角色
	/// </summary>
	[Serializable]
	public class ManageRoleNotExistException : Exception
	{
		public ManageRoleNotExistException()
			: base("管理角色不存在")
		{
		}

		public ManageRoleNotExistException(string message)
			: base(message)
		{
		}

		public ManageRoleNotExistException(string appCodeName, string roleCodeName)
			: base(string.Format("未找到指定CodeName的管理应用{0}或管理角色{1}", appCodeName, roleCodeName))
		{
			this.AppCodeName = appCodeName;
			this.RoleCodeName = roleCodeName;
		}

		public ManageRoleNotExistException(string appCodeName, string roleCodeName, string message)
			: base(message)
		{
			this.AppCodeName = appCodeName;
			this.RoleCodeName = roleCodeName;
		}

		public ManageRoleNotExistException(string message, Exception inner) :
			base(message, inner)
		{
		}

		public ManageRoleNotExistException(string appCodeName, string roleCodeName, Exception inner)
			: base(string.Format("未找到指定CodeName的管理应用{0}或管理角色{1}", appCodeName, roleCodeName), inner)
		{
			this.AppCodeName = appCodeName;
			this.RoleCodeName = roleCodeName;
		}

		protected ManageRoleNotExistException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}

		public string AppCodeName { get; set; }

		public string RoleCodeName { get; set; }
	}
}