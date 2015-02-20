using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 用于客户端更新角色的包
	/// </summary>
	[Serializable]
	public class AURoleMemberPack
	{
		Dictionary<string, List<string>> members = new Dictionary<string, List<string>>();

		public Dictionary<string, List<string>> Members
		{
			get { return members; }
		}
	}
}
