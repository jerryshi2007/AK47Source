using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class SimpleUser
	{
		System.Collections.Specialized.HybridDictionary dic = new System.Collections.Specialized.HybridDictionary();

		public SimpleUser(System.Data.IDataReader reader)
		{
			int columnCount = reader.FieldCount;
			for (int i = 0; i < columnCount; i++)
			{
				this.dic[reader.GetName(i)] = reader[i];
			}
		}

		public string SCObjectID
		{
			get
			{
				return (string)this.dic["ID"];
			}

			set
			{
				this.dic["ID"] = value;
			}
		}

		public string Mail
		{
			get { return (string)this.dic["Mail"]; }

			set { this.dic["Mail"] = value; }
		}

		public string Sip
		{
			get { return (string)this.dic["Sip"]; }

			set { this.dic["Sip"] = value; }
		}

		public string CodeName
		{
			get { return (string)this.dic["CodeName"]; }

			set { this.dic["CodeName"] = value; }
		}

		public object this[string key]
		{
			get { return this.dic[key]; }
			set { this.dic[value] = value; }
		}

		public System.DirectoryServices.SearchResult Tag { get; set; }
	}

	public class EntityMappingCollection : EditableKeyedDataObjectCollectionBase<string, SimpleUser>
	{
		public string[] ToSCObjectIDArray()
		{
			return this.ToSCObjectIDArray(0, this.Count);
		}

		public string[] ToSCObjectIDArray(int startIndex, int size)
		{
			int actualSize = AdjustSize(startIndex, size);

			string[] result = new string[actualSize];

			for (int i = 0; i < actualSize; i++)
				result[i] = this[i + startIndex].SCObjectID;

			return result;
		}

		private int AdjustSize(int startIndex, int size)
		{
			if (size != 0)
			{
				(size > 0).FalseThrow<ArgumentOutOfRangeException>("size");
				(startIndex >= 0 && startIndex < this.Count).FalseThrow<ArgumentOutOfRangeException>("startIndex");

				size = startIndex + size <= this.Count ? size : this.Count - startIndex;
			}

			return size;
		}

		public string[] ToKeyArray(string keyName, int startIndex, int size)
		{
			int actualSize = AdjustSize(startIndex, size);

			string[] result = new string[actualSize];

			for (int i = 0; i < actualSize; i++)
				result[i] = (string)this[i + startIndex][keyName];

			return result;
		}

		public string[] ToKeyArray(string keyName)
		{
			return this.ToKeyArray(keyName, 0, this.Count);
		}

		public Dictionary<Key, SimpleUser> ToPartDictionary<Key>(int startIndex, int size, Func<SimpleUser, Key> func)
		{
			Dictionary<Key, SimpleUser> dic = new Dictionary<Key, SimpleUser>(size);

			size = AdjustSize(startIndex, size);

			for (int i = 0; i < size; i++)
			{
				SimpleUser item = this[startIndex + i];

				//要么写死CodeName，要么这里作判断
				try
				{
					dic.Add(func(item), item);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(ex.Message + "\r\n参数值:" + func(item), ex.ParamName, ex.InnerException);
				}
			}

			return dic;
		}

		public Dictionary<string, SimpleUser> ToCodeNameDictionary(int startIndex, int size)
		{
			Dictionary<string, SimpleUser> dic = new Dictionary<string, SimpleUser>(size);

			size = AdjustSize(startIndex, size);

			for (int i = 0; i < size; i++)
			{
				SimpleUser item = this[startIndex + i];
				try
				{
					dic.Add(item.CodeName, item);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(ex.Message + "\r\n参数值:" + item.CodeName, ex.ParamName, ex.InnerException);
				}
			}

			return dic;
		}

		protected override string GetKeyForItem(SimpleUser item)
		{
			return item.CodeName;
		}
	}
}
