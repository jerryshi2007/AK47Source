using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class WfExternalUser
	{
		public WfExternalUser()
		{ }

		private string _Key;
		/// <summary>
		/// 人员标识
		/// </summary>
		public string Key 
		{
			get { return _Key; }
			set { _Key = value; }
		}

		private string _Name;
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private Gender _Gender;
		/// <summary>
		/// 性别
		/// </summary>
		public Gender Gender
		{
			get { return _Gender; }
			set { _Gender = value; }
		}

		private string _Phone;
		/// <summary>
		/// 电话
		/// </summary>
		public string Phone
		{
			get { return _Phone; }
			set { _Phone = value; }
		}

		private string _MobilePhone;
		/// <summary>
		/// 手机
		/// </summary>
		public string MobilePhone
		{
			get { return _MobilePhone; }
			set { _MobilePhone = value; }
		}

		private string _Title;
		/// <summary>
		/// 职称
		/// </summary>
		public string Title
		{
			get { return _Title; }
			set { _Title = value; }
		}

		private string _Email;
		/// <summary>
		/// 邮箱
		/// </summary>
		public string Email
		{
			get { return _Email; }
			set { _Email = value; }
		}

	}

	/// <summary>
	/// 外部的相关人员列表.
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfExternalUserCollection : EditableDataObjectCollectionBase<WfExternalUser>
	{
		public void SyncPropertiesToFields(PropertyValue property)
		{
			if (property != null)
			{
				this.Clear();

				if (property.StringValue.IsNotEmpty())
				{
					IEnumerable<WfExternalUser> deserializedData = (IEnumerable<WfExternalUser>)JSONSerializerExecute.DeserializeObject(property.StringValue, this.GetType());

					this.CopyFrom(deserializedData);
				}
			}
		}
	}
}
