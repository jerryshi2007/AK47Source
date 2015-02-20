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
	public class GetGroupUsers : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetGroupUsers(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			OguObjectCollection<IOguObject> groups = QueryHelper.QueryObjectByFullPath(argument);

			foreach (IGroup group in groups)
				group.Members.ToList().ForEach(obj => OutputHelper.OutputObjectInfo(obj));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getGroupUsers {groupFullPath}";
			}
		}
	}
}
