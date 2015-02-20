using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;
using System.Security.Cryptography;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	public abstract class UserPasswordPersisterBase<T> : PropertyPersisterBase<T> where T : IPropertyValueAccessor
	{
		public override void Read(T currentProperty, PersisterContext<T> context)
		{

		}

		public override void Write(T currentProperty, PersisterContext<T> context)
		{
			string value = currentProperty.StringValue;
			EditorParamsDefine paraDefine = null;

			if (currentProperty.Definition.EditorParams.IsNotEmpty())
			{
				base.Register();
				paraDefine = JSONSerializerExecute.Deserialize<EditorParamsDefine>(currentProperty.Definition.EditorParams);
			}

			if (value.IsNotEmpty())
			{
				if (string.Compare(currentProperty.StringValue, currentProperty.Definition.DefaultValue, true) != 0)
				{
					if (paraDefine.ContainsKey("TagPropertyName") == true)
						currentProperty.StringValue = PwdCalculate("", string.Format("{0},{1}", context.Properties[paraDefine["TagPropertyName"]].StringValue, value));
					else
						currentProperty.StringValue = PwdCalculate("", value);
				}
			}
		}

		//MCS.Library.Accredit.dll
		//MCS.Library.Accredit.OguAdmin 
		//SecurityCalculate

		/// <summary>
		/// 按照一定的加密算法生成转换后的加密数据（用于密码值计算）
		/// </summary>
		/// <param name="strPwdType">指定的加密算法类型</param>
		/// <param name="strPwd">指定要求被加密的数据</param>
		/// <returns>按照一定的加密算法生成转换后的加密数据（用于密码值计算）</returns>
		public static string PwdCalculate(string strPwdType, string strPwd)
		{
			string strResult = strPwd;

			MD5 md = new MD5CryptoServiceProvider();
			strResult = BitConverter.ToString(md.ComputeHash((new UnicodeEncoding()).GetBytes(strPwd)));

			return strResult;
		}

		// MCS.Library.Accredit.dll CommonResource.OriginalSortDefault
		public const string originalSortDefault = "000000";
	}

	public sealed class UserPasswordPersister : UserPasswordPersisterBase<PropertyValue>
	{
	}
}
