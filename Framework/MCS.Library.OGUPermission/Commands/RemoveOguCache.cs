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
	public class RemoveOguCache : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public RemoveOguCache(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			OguMechanismFactory.GetMechanism().RemoveAllCache();

			Console.WriteLine("Ogu cache cleared");
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "removeOguCache";
			}
		}
	}
}
