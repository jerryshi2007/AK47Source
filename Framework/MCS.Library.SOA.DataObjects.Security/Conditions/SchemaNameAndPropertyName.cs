using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	/// <summary>
	/// 将类似于Users.Name解析为{Users, Name}结构的类
	/// </summary>
	public struct SchemaNameAndPropertyName
	{
		private static readonly char[] _SplitChars = new char[] { '.' };

		public static SchemaNameAndPropertyName FromFullName(string fullName)
		{
			fullName.CheckStringIsNullOrEmpty("fullName");

			string[] nameParts = fullName.Split(SchemaNameAndPropertyName._SplitChars, StringSplitOptions.RemoveEmptyEntries);

			SchemaNameAndPropertyName result = new SchemaNameAndPropertyName();

			result.SchemaName = nameParts[0];

			if (nameParts.Length > 1)
				result.PropertyName = nameParts[1];
			else
				result.PropertyName = null;

			return result;
		}

		public string SchemaName;

		public string PropertyName;

		public bool IsValid
		{
			get
			{
				return this.PropertyName.IsNotEmpty();
			}
		}

		public void CheckIsValid()
		{
			if (IsValid == false)
				throw new InvalidOperationException(string.Format("{0}不是完整Schema和属性结构", this.SchemaName));
		}
	}
}
