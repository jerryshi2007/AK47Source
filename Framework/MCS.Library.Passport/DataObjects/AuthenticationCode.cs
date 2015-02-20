using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 短信验证码
	/// </summary>
	[Serializable]
	[ORTableMapping("AUTHENTICATION_CODE")]
	public class AuthenticationCode
	{
		/// <summary>
		/// 认证码对应的ID
		/// </summary>
		[ORFieldMapping("AUTHENTICATION_ID", PrimaryKey = true)]
		public string AuthenticationID
		{
			get;
			set;
		}

		/// <summary>
		/// 认证码的类型
		/// </summary>
		[ORFieldMapping("AUTHENTICATION_TYPE")]
		public string AuthenticationType
		{
			get;
			set;
		}

		/// <summary>
		/// 认证码
		/// </summary>
		[ORFieldMapping("AUTHENTICATION_CODE")]
		public string Code
		{
			get;
			set;
		}

		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(ClauseBindingFlags.Select | ClauseBindingFlags.Where, DefaultExpression = "GETDATE()")]
		public DateTime CreateTime
		{
			get;
			set;
		}

		/// <summary>
		/// 过期时间
		/// </summary>
		[ORFieldMapping("EXPIRE_TIME")]
		[SqlBehavior(ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime ExpireTime
		{
			get;
			set;
		}

		/// <summary>
		/// 该字段仅用于返回值
		/// </summary>
		[ORFieldMapping("IS_VALID")]
		[SqlBehavior(ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public bool IsValid
		{
			get;
			set;
		}

		/// <summary>
		/// 随机生成一个符合长度的整数
		/// </summary>
		/// <param name="codeLength"></param>
		/// <returns></returns>
		public static string GenerateCode(int codeLength)
		{
			(codeLength > 0).FalseThrow("需要生成的认证码长度");
			Random rnd = new Random((int)DateTime.Now.Ticks);

			string template = new string('0', codeLength);

			template = "{0:" + template + "}";

			return string.Format(template, rnd.Next((int)Math.Pow(10, codeLength)));
		}

		/// <summary>
		/// 创建一个新的验证码
		/// </summary>
		/// <param name="authenticationType"></param>
		/// <param name="codeLength"></param>
		/// <returns></returns>
		public static AuthenticationCode Create(string authenticationType, int codeLength)
		{
			authenticationType.CheckStringIsNullOrEmpty("authenticationType");

			AuthenticationCode result = new AuthenticationCode();

			result.AuthenticationID = UuidHelper.NewUuidString();
			result.AuthenticationType = authenticationType;

			result.Code = GenerateCode(codeLength);

			return result;
		}
	}
}
