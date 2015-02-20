using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.ComponentModel;
using MCS.Library.Data.DataObjects;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户扩展信息
	/// </summary>
	[Serializable]
	[ORTableMapping("USERS_INFO_EXTEND")]
	public class UserInfoExtendDataObject
	{
		#region 全局私有变量

		private string _ID = string.Empty;
		private string _LogonName = string.Empty;
		private string _DisplayName = string.Empty;
		private string _FullPath = string.Empty;
		private Gender _Gender = Gender.Male;
		private string _Nation = string.Empty;
		private DateTime _Birthday = DateTime.MinValue;
		private DateTime _StartWorkTime = DateTime.MinValue;
		private string _Mobile = string.Empty;
		private string _Mobile2 = string.Empty;
		private string _OfficeTel = string.Empty;
		private string _IntranetEmail = string.Empty;
		private string _InternetEmail = string.Empty;
		private string _IMAddress = string.Empty;
		private string _Memo = string.Empty;

		#endregion

		#region 公共属性

		/// <summary>
		/// 用户ID
		/// </summary>
		[Description("用户ID")]
		[ORFieldMapping("ID", PrimaryKey = true)]
		public string ID
		{
			get { return _ID; }
			set { _ID = value; }
		}

		/// <summary>
		/// 登录名
		/// </summary>
		[Description("登录名")]
		[NoMapping]
		public string LogonName
		{
			get { return _LogonName; }
			set { _LogonName = value; }
		}

		/// <summary>
		/// 显示名称
		/// </summary>
		[Description("显示名称")]
		[NoMapping]
		public string DisplayName
		{
			get { return _DisplayName; }
			set { _DisplayName = value; }
		}

		/// <summary>
		/// 全路径
		/// </summary>
		[Description("全路径")]
		[NoMapping]
		public string FullPath
		{
			get { return _FullPath; }
			set { _FullPath = value; }
		}

		/// <summary>
		/// 性别
		/// </summary>
		[Description("性别")]
		[ORFieldMapping("GENDER", IsNullable = true)]
		public Gender Gender
		{
			get { return _Gender; }
			set { _Gender = value; }
		}

		/// <summary>
		/// 民族
		/// </summary>
		[Description("民族")]
		[ORFieldMapping("Nation", IsNullable = false)]
		public string Nation
		{
			get { return _Nation; }
			set { _Nation = value; }
		}

		/// <summary>
		/// 出生日期
		/// </summary>
		[Description("生日")]
		[ORFieldMapping("BIRTHDAY", IsNullable = true)]
		// [DateTimeRangeValidator("1900-01-01", "9999-12-31", MessageTemplate = "请填写出生日期，而且范围必须合理")]
		public DateTime Birthday
		{
			get { return _Birthday; }
			set { _Birthday = value; }
		}

		/// <summary>
		/// 入职时间
		/// </summary>
		[Description("入职时间")]
		[ORFieldMapping("START_WORK_TIME", IsNullable = false)]
		//    [DateTimeRangeValidator("1900-01-01", "9999-12-31", MessageTemplate = "请填写入职日期，而且范围必须合理")]
		public DateTime StartWorkTime
		{
			get { return _StartWorkTime; }
			set { _StartWorkTime = value; }
		}

		/// <summary>
		/// 手机1
		/// </summary>
		[Description("手机1")]
		[ORFieldMapping("MOBILE", IsNullable = true)]
		public string Mobile
		{
			get { return _Mobile; }
			set { _Mobile = value; }
		}

		/// <summary>
		/// 手机1
		/// </summary>
		[Description("手机2")]
		[ORFieldMapping("MOBILE2", IsNullable = true)]
		public string Mobile2
		{
			get { return _Mobile2; }
			set { _Mobile2 = value; }
		}

		/// <summary>
		/// 办公室电话
		/// </summary>
		[Description("办公室电话")]
		[ORFieldMapping("OFFICE_TEL", IsNullable = true)]
		public string OfficeTel
		{
			get { return _OfficeTel; }
			set { _OfficeTel = value; }
		}

		/// <summary>
		/// 内网Email
		/// </summary>
		[Description("内网Email")]
		[ORFieldMapping("INTRANET_EMAIL", IsNullable = true)]
		public string IntranetEmail
		{
			get { return _IntranetEmail; }
			set { _IntranetEmail = value; }
		}

		/// <summary>
		/// 外网Email
		/// </summary>
		[Description("外网Email")]
		[ORFieldMapping("INTERNET_EMAIL", IsNullable = true)]
		public string InternetEmail
		{
			get { return _InternetEmail; }
			set { _InternetEmail = value; }
		}

		/// <summary>
		/// 即时消息
		/// </summary>
		[Description("即时消息")]
		[ORFieldMapping("IM_ADDRESS", IsNullable = true)]
		public string IMAddress
		{
			get { return _IMAddress; }
			set { _IMAddress = value; }
		}

		/// <summary>
		/// MEMO
		/// </summary>
		[Description("MEMO")]
		[ORFieldMapping("MEMO", IsNullable = true)]
		public string MEMO
		{
			get { return _Memo; }
			set { _Memo = value; }
		}

		/// <summary>
		/// MEMO
		/// </summary>
		[Description("个人签名图片路径")]
		[ORFieldMapping("SIGN_IMAGE_PATH", IsNullable = true)]
		public string SignImagePath { get; set; }

		#endregion

		public string GetSignImagePath()
		{
			string result = "";

			if (string.IsNullOrEmpty(this.SignImagePath))
			{
				return result;
			}

			if (ResourceUriSettings.GetConfig().Paths.ContainsKey("signImageFilePath")
				&& !string.IsNullOrEmpty(ResourceUriSettings.GetConfig().Paths["signImageFilePath"].Uri.ToString()))
			{
				result = ResourceUriSettings.GetConfig().Paths["signImageFilePath"].Uri.ToString() + this.SignImagePath;
			}
			else
			{
				result = "/MCSWebApp/OACommonPages/UserInfoExtend/GetSignImage.aspx?SignImagePath="
					  + this.SignImagePath;
			}

			return result;
		}
	}

	[Serializable]
	public class UserInfoExtendCollection : EditableDataObjectCollectionBase<UserInfoExtendDataObject>
	{
		internal void LoadFromDataView(DataView dv)
		{
			this.Clear();

			ORMapping.DataViewToCollection(this, dv);
		}
	}
}
