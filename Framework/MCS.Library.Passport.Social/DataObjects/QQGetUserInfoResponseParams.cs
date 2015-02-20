using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Passport.Social.DataObjects
{
	[Serializable]
	public class QQGetUserInfoResponseParams
	{
		public QQGetUserInfoResponseParams()
		{
		}

		public QQGetUserInfoResponseParams(string openID)
		{
			this.OpenID = openID;
		}

		/// <summary>
		/// 
		/// </summary>
		public string OpenID
		{
			get;
			set;
		}

		/// <summary>
		/// 用户在QQ空间的昵称
		/// </summary>
		public string Nickname
		{
			get;
			set;
		}

		/// <summary>
		/// 大小为30×30像素的QQ空间头像URL
		/// </summary>
		public string FigureUrl
		{
			get;
			set;
		}

		/// <summary>
		/// 50*50
		/// </summary>
		public string FigureUrl50
		{
			get;
			set;
		}

		/// <summary>
		/// 100*100
		/// </summary>
		public string FigureUrl100
		{
			get;
			set;
		}

		/// <summary>
		/// 40*40
		/// </summary>
		public string FigureUrlQQ
		{
			get;
			set;
		}

		/// <summary>
		/// 100*100
		/// </summary>
		public string FigureUrlQQ100
		{
			get;
			set;
		}

		public string Gender
		{
			get;
			set;
		}

		public bool IsYellowVIP
		{
			get;
			set;
		}

		public bool IsVIP
		{
			get;
			set;
		}

		public int YellowVIPLevel
		{
			get;
			set;
		}

		public int Level
		{
			get;
			set;
		}

		public bool IsYellowYearVIP
		{
			get;
			set;
		}

		public void FromDictionary(Dictionary<string, object> data)
		{
			data.NullCheck("data");

			this.Nickname = data.GetValue("nickname", string.Empty);
			this.FigureUrl = data.GetValue("figureurl", string.Empty);
			this.FigureUrl50 = data.GetValue("figureurl_1", string.Empty);
			this.FigureUrl100 = data.GetValue("figureurl_2", string.Empty);
			this.FigureUrlQQ = data.GetValue("figureurl_qq_1", string.Empty);
			this.FigureUrlQQ100 = data.GetValue("figureurl_qq_2", string.Empty);

			this.Gender = data.GetValue("gender", string.Empty);
			this.IsYellowVIP = data.GetValue("is_yellow_vip", false);
			this.IsVIP = data.GetValue("vip", false);
			this.YellowVIPLevel = data.GetValue("yellow_vip_level", 0);
			this.Level = data.GetValue("level", 0);
			this.IsYellowYearVIP = data.GetValue("is_yellow_year_vip", false);
		}

		public SocialUserInfo ToSocialUserInfo()
		{
			SocialUserInfo userInfo = new SocialUserInfo();

			userInfo.OpenID = this.OpenID;
			userInfo.UserName = this.Nickname;
			userInfo.FigureUrl = this.FigureUrlQQ;

			userInfo.Properties["Nickname"] = this.Nickname;
			userInfo.Properties["FigureUrl"] = this.FigureUrl;
			userInfo.Properties["FigureUrl50"] = this.FigureUrl50;
			userInfo.Properties["FigureUrl100"] = this.FigureUrl100;
			userInfo.Properties["FigureUrlQQ"] = this.FigureUrlQQ;
			userInfo.Properties["FigureUrlQQ100"] = this.FigureUrlQQ100;

			userInfo.Properties["Gender"] = this.Gender;
			userInfo.Properties["IsYellowVIP"] = this.IsYellowVIP;
			userInfo.Properties["IsVIP"] = this.IsVIP;
			userInfo.Properties["YellowVIPLevel"] = this.YellowVIPLevel;
			userInfo.Properties["Level"] = this.Level;
			userInfo.Properties["IsYellowYearVIP"] = this.IsYellowYearVIP;

			return userInfo;
		}
	}
}
