using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Passport.Social.DataObjects
{
	/// <summary>
	/// 社交账号所使用的通用用户信息
	/// </summary>
	[Serializable]
	public class SocialUserInfo
	{
		private Dictionary<string, object> _Properties = null;

		/// <summary>
		/// 用户的OpenID
		/// </summary>
		public string OpenID
		{
			get;
			set;
		}

		/// <summary>
		/// 用户的名称
		/// </summary>
		public string UserName
		{
			get;
			set;
		}

		/// <summary>
		/// 用户的头像
		/// </summary>
		public string FigureUrl
		{
			get;
			set;
		}

		/// <summary>
		/// 扩展属性
		/// </summary>
		public Dictionary<string, object> Properties
		{
			get
			{
				if (this._Properties == null)
					this._Properties = new Dictionary<string, object>();

				return this._Properties;
			}
		}
	}
}
