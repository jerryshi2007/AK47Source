using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	[ORTableMapping("SC.PermissionCenter_AD_IDMapping")]
	public class IDMapping
	{
		[ORFieldMapping("SCObjectID", PrimaryKey = true)]
		public string SCObjectID { get; set; }

		/// <summary>
		/// 获取或设置NativeGuid（二进制字节数组直接转成十六进制）
		/// </summary>
		public string ADObjectGuid { get; set; }
		public DateTime LastSynchronizedVersionTime { get; set; }

		public override string ToString()
		{
			return string.Format("AD:{0}→OGU:{1}", ADObjectGuid, SCObjectID);
		}
	}

	public class IDMappingCollection : EditableDataObjectCollectionBase<IDMapping>
	{
		public SCIDMappingCollection ToSCIDMappingCollection()
		{
			SCIDMappingCollection result = new SCIDMappingCollection();

			foreach (var item in this)
				result.Add(item);

			return result;
		}

		public ADIDMappingCollection ToADIDMappingCollection()
		{
			ADIDMappingCollection result = new ADIDMappingCollection();

			foreach (var item in this)
				result.Add(item);

			return result;
		}
	}

	public class SCIDMappingCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IDMapping>
	{
		public SCIDMappingCollection()
		{
		}

		protected SCIDMappingCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override string GetKeyForItem(IDMapping item)
		{
			return item.SCObjectID;
		}
	}

	public class ADIDMappingCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IDMapping>
	{
		public ADIDMappingCollection()
		{
		}

		protected ADIDMappingCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override string GetKeyForItem(IDMapping item)
		{
			return item.ADObjectGuid;
		}
	}
}