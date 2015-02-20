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
	public class GetAllChildren : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetAllChildren(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			OguObjectCollection<IOguObject> ogus = QueryHelper.QueryObjectByFullPath(argument);

			(ogus[0] is IOrganization).FalseThrow("对象\"{0}\"不是一个组织", argument);

			IOrganization ogu = (IOrganization)ogus[0];

			ogu.GetAllChildren<IOguObject>(true).ForEach(o => OutputHelper.OutputObjectInfo(o));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getAllChildren {fullPath}";
			}
		}
	}
}
