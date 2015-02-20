using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.Services.Contracts;
using MCS.Library.Data.DataObjects;


namespace MCS.Library.SOA.DocServiceContract
{
	[DataContract]
	[KnownType(typeof(DCTSimpleProperty))]
	[KnownType(typeof(DCTComplexProperty))]
	public abstract class DCTDataProperty
	{
		[DataMember]
		public string TagID { get; set; }

		[DataMember]
		public string LocalID { get; set; }
	}

	public class DCTDataPropertyCollection : SerializableEditableKeyedDataObjectCollectionBase<string, DCTDataProperty>
	{
		public DCTDataPropertyCollection()
		{
		}

		public DCTDataPropertyCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public new DCTDataProperty this[string key]
		{
			get
			{
				if (ContainsKey(key) == false)
					return null;

				return base[key];
			}
		}

		public void AddRange<T>(List<T> items)
			where T : DCTDataProperty
		{
			foreach (var item in items)
			{
				this.Add(item);
			}
		}

		protected override string GetKeyForItem(DCTDataProperty item)
		{
			return item.TagID;
		}
	}
}
