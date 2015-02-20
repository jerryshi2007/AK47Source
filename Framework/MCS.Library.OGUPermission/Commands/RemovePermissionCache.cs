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
	public class RemovePermissionCache : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public RemovePermissionCache(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			PermissionMechanismFactory.GetMechanism().RemoveAllCache();

			Console.WriteLine("Permission cache cleared");
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "removePermissionCache";
			}
		}
	}
}
