using System;
using System.Collections;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	public enum ClientPropertyDataType
	{
		/// <summary>
		/// JSON格式，其实还是字符串
		/// </summary>
		[EnumItemDescription("JSON对象", 1)]
		DataObject = 1,
		[EnumItemDescription("布尔", 3)]
		Boolean = 3,
		[EnumItemDescription("整型", 9)]
		Integer = 9,
		[EnumItemDescription("浮点", 15)]
		Decimal = 15,
		[EnumItemDescription("时间", 16)]
		DateTime = 16,
		[EnumItemDescription("文本", 18)]
		String = 18,
		[EnumItemDescription("枚举", 20)]
		Enum = 20
	}

	[Serializable]
	public class ClientPropertyDefine
	{
		public ClientPropertyDefine()
		{
		}

		public bool AllowOverride
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public ClientPropertyDataType DataType
		{
			get;
			set;
		}

		public string DefaultValue
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}

		public string EditorKey
		{
			get;
			set;
		}

		public string EditorParams
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get;
			set;
		}

		public int MaxLength
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string PersisterKey
		{
			get;
			set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public int SortOrder
		{
			get;
			set;
		}

		public bool Visible
		{
			get;
			set;
		}
	}

	[Serializable]
	public class ClientPropertyDefineCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ClientPropertyDefine>
	{
		public ClientPropertyDefineCollection()
			: base()
		{
		}

		protected ClientPropertyDefineCollection(int capacity)
			: base(capacity)
		{
		}

		protected ClientPropertyDefineCollection(int capacity, IEqualityComparer comparer)
			: base(capacity, comparer)
		{
		}

		protected ClientPropertyDefineCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ClientPropertyDefineCollection(ClientPropertyDefine[] items)
			: this()
		{
			items.NullCheck("items");

			foreach (var item in items)
			{
				this.Add(item);
			}
		}

		protected override string GetKeyForItem(ClientPropertyDefine item)
		{
			return item.Name;
		}
	}
}
