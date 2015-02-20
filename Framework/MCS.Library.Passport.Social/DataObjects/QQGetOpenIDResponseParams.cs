using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Passport.Social.DataObjects
{
	/// <summary>
	/// 得到OpenID的返回信息
	/// </summary>
	[Serializable]
	public class QQGetOpenIDResponseParams
	{
		public string ClientID
		{
			get;
			set;
		}

		public string OpenID
		{
			get;
			set;
		}
	}
}
