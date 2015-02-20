using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户的即时消息地址
	/// </summary>
	[Serializable]
	public class UserIMAddress
	{
		public UserIMAddress()
		{
		}

		public UserIMAddress(string userID, string imAddress)
		{
			this.UserID = userID;
			this.IMAddress = imAddress;
		}

		/// <summary>
		/// 用户ID
		/// </summary>
		public string UserID
		{
			get;
			set;
		}

		/// <summary>
		/// 用户即时消息地址
		/// </summary>
		public string IMAddress
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 用户的即时消息地址集合
	/// </summary>
	public class UserIMAddressCollection : SerializableEditableKeyedDataObjectCollectionBase<string, UserIMAddress>
	{
		public UserIMAddressCollection()
		{
		}

		protected UserIMAddressCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected override string GetKeyForItem(UserIMAddress item)
		{
			return item.UserID;
		}
	}
}
