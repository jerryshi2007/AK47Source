using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Web.Apps.WeChat.DataObjects;

namespace MCS.Web.Apps.WeChat.Commands
{
	public class GetAllGroupsCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public GetAllGroupsCommand(string name)
			: base(name)
		{
		}

		public override void Execute(string argument)
		{
			WeChatGroupCollection groups = WeChatHelper.GetAllGroups(WeChatRequestContext.Current.LoginInfo);

			groups.Output();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "getAllGroups";
			}
		}
	}
}
