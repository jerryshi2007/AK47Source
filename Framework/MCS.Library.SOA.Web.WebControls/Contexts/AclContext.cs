using System;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library.MVC;
using MCS.Library.Core;


namespace MCS.Web.WebControls
{
	[Serializable]
	public class AclContext : CommandStateBase
	{
		private AclContext()
		{
		}

		public static AclContext Current
		{
			get
			{
				AclContext context = (AclContext)CommandStateHelper.GetCommandState(typeof(AclContext));

				if (context == null)
				{
					context = new AclContext();
					CommandStateHelper.RegisterState(context);
				}

				return context;
			}
		}
	}
}
