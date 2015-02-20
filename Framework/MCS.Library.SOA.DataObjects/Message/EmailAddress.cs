using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System.Net.Mail;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 邮件地址类
	/// </summary>
	[Serializable]
	[ORTableMapping("MSG.EMAIL_ADDRESSES")]
	public class EmailAddress
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public EmailAddress()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="address"></param>
		public EmailAddress(string address)
		{
			this.Address = address;
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="address"></param>
		/// <param name="displayName"></param>
		public EmailAddress(string address, string displayName)
		{
			this.Address = address;
			this.DisplayName = displayName;
		}

		/// <summary>
		/// 地址
		/// </summary>
		[ORFieldMapping("ADDRESS")]
		public string Address
		{
			get;
			set;
		}

		/// <summary>
		/// 显示名称
		/// </summary>
		[ORFieldMapping("DISPLAY_NAME")]
		public string DisplayName
		{
			get;
			set;
		}

		/// <summary>
		/// 转换为字符串格式DisplayName&lt;Address&gt;
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string result = Address;

			if (DisplayName.IsNotEmpty())
				result = string.Format("{0}<{1}>", DisplayName, Address);

			return result;
		}

		public static EmailAddress FromDescription(string description)
		{
			description.CheckStringIsNullOrEmpty("description");

			string address = string.Empty;
			string displayName = string.Empty;

			if (description.IsNotEmpty())
			{
				int flagIndex = description.IndexOf("<");

				if (flagIndex >= 0)
				{
					displayName = description.Substring(0, flagIndex);
					address = description.Substring(flagIndex).TrimStart('<').TrimEnd('>');
				}
				else
				{
					address = description;
				}
			}

			return new EmailAddress(address, displayName);
		}
	}

	[Serializable]
	public class EmailAddressCollection : EditableDataObjectCollectionBase<EmailAddress>
	{
		public void Add(string addressDescription)
		{
			addressDescription.CheckStringIsNullOrEmpty("addressDescription");

			this.Add(EmailAddress.FromDescription(addressDescription));
		}

		public void CopyTo(MailAddressCollection addresses)
		{
			addresses.NullCheck("addresses");

			foreach (EmailAddress ea in this)
			{
				addresses.Add(ea.ToMailAddress());
			}
		}
	}
}
