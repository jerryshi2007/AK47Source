using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Logging;
using MCS.Library.SOA.Security.ADSyncUtilities.Adapters;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class IDMapper
	{
		/// <summary>
		/// 以SCID为Key的ID映射表
		/// </summary>
		public SCIDMappingCollection SCIDMappingDictionary = new SCIDMappingCollection();

		/// <summary>
		/// 以ADID为Key的ID映射表
		/// </summary>
		public ADIDMappingCollection ADIDMappingDictionary = new ADIDMappingCollection();

		/// <summary>
		/// 以SCID为Key的ID删除信息表
		/// </summary>
		public SCIDMappingCollection DeleteIDMappingDictionary = new SCIDMappingCollection();

		public Dictionary<string, IDMapping> NewIDMappingDictionary = new Dictionary<string, IDMapping>();	//这个没用KeyedCollection，因为特殊用法不适用。

		public void Initialize()
		{
			SCIDMappingDictionary.Clear();
			ADIDMappingDictionary.Clear();
			NewIDMappingDictionary.Clear();
			DeleteIDMappingDictionary.Clear();

			IDMappingCollection allMappings = IDMappingAdapter.Instance.Load(b => b.AppendItem("1", "1", "=", true));

			SCIDMappingDictionary = allMappings.ToSCIDMappingCollection();
			ADIDMappingDictionary = allMappings.ToADIDMappingCollection();
		}

		public void UpdateIDMapping()
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				IDMappingAdapter.Instance.BatchDelete(PrepareDataTable(DeleteIDMappingDictionary));
				IDMappingAdapter.Instance.BatchInsert(PrepareDataTable(NewIDMappingDictionary));

				scope.Complete();
			}
		}

		private DataTable PrepareDataTable(SCIDMappingCollection collection)
		{
			DataTable table = new DataTable();
			table.Columns.Add("SCObjectID");
			table.Columns.Add("ADObjectGuid");
			table.Columns.Add("LastSynchronizedVersionTime");

			foreach (var item in collection)
				table.Rows.Add(item.SCObjectID, item.ADObjectGuid, item.LastSynchronizedVersionTime);

			return table;
		}

		private DataTable PrepareDataTable(Dictionary<string, IDMapping> mappingsDic)
		{
			DataTable table = new DataTable();
			table.Columns.Add("SCObjectID");
			table.Columns.Add("ADObjectGuid");
			table.Columns.Add("LastSynchronizedVersionTime");

			foreach (var key in mappingsDic.Keys)
			{
				var obj = mappingsDic[key];
				if (obj != null)
					table.Rows.Add(obj.SCObjectID, obj.ADObjectGuid, obj.LastSynchronizedVersionTime);
			}

			return table;
		}

		internal IDMapping GetAdObjectMapping(string oguObjectID)
		{
			return GetAdObjectMapping(oguObjectID, true);
		}

		internal IDMapping GetAdObjectMapping(string oguObjectID, bool throwIfNotFound)
		{
			if (this.NewIDMappingDictionary.ContainsKey(oguObjectID))
				return this.NewIDMappingDictionary[oguObjectID];
			else if (this.SCIDMappingDictionary.ContainsKey(oguObjectID))
				return this.SCIDMappingDictionary[oguObjectID];
			else if (throwIfNotFound)
				throw new KeyNotFoundException("ID映射不存在");
			else
				return null;
		}

		//可以考虑提取到IDMapper中
		public IList<string> GetMappedADObjectIDs(IEnumerable<string> oguObjectIDs)
		{
			List<string> result = new List<string>();

			foreach (string oguID in oguObjectIDs)
			{
				IDMapping mapping = this.GetAdObjectMapping(oguID, false); // 可能在同步范围中没有

				if (mapping != null)
					result.Add(mapping.ADObjectGuid);
			}

			return result;
		}

		internal IEnumerable<IDMapping> GetAdObjectMappings(IEnumerable<OGUPermission.IOguObject> groups)
		{
			return GetAdObjectMappings(groups, true);
		}

		internal IEnumerable<IDMapping> GetAdObjectMappings(IEnumerable<OGUPermission.IOguObject> groups, bool throwIfNotFound)
		{
			foreach (var item in groups)
			{
				string oguObjectID = item.ID;

				if (this.NewIDMappingDictionary.ContainsKey(oguObjectID))
					yield return this.NewIDMappingDictionary[oguObjectID];
				else if (this.SCIDMappingDictionary.ContainsKey(oguObjectID))
					yield return this.SCIDMappingDictionary[oguObjectID];
				else if (throwIfNotFound)
					throw new KeyNotFoundException("ID映射不存在");
			}
		}
	}
}