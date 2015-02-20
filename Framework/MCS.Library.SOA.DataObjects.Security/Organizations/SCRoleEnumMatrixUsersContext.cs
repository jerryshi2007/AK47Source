using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 枚举角色矩阵中时的上下文对象
	/// </summary>
	internal class SCRoleEnumMatrixUsersContext
	{
		private Dictionary<string, SCApplication> _CachedApplication = new Dictionary<string, SCApplication>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, string> _CalculatedRolesCodeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, string> _UsersCodeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		/// <summary>
		/// 缓存的应用
		/// </summary>
		public Dictionary<string, SCApplication> CachedApplication
		{
			get
			{
				return this._CachedApplication;
			}
		}

		/// <summary>
		/// 已经处理的角色的全代码名称，包括APP的CodeName
		/// </summary>
		public Dictionary<string, string> CalculatedRolesCodeNames
		{
			get
			{
				return this._CalculatedRolesCodeNames;
			}
		}

		/// <summary>
		/// 已经生成的用户的CodeName
		/// </summary>
		public Dictionary<string, string> UsersCodeNames
		{
			get
			{
				return this._UsersCodeNames;
			}
		}
	}
}
