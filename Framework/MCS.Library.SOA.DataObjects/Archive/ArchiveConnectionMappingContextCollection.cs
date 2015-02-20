using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 归档时，数据库连接的映射
	/// </summary>
	public class ArchiveConnectionMappingContextCollection : ReadOnlyDataObjectCollectionBase<DbConnectionMappingContext>, IDisposable
	{
		internal void Add(DbConnectionMappingContext context)
		{
			this.List.Add(context);
		}

		public DbConnectionMappingContext this[int index]
		{
			get
			{
				return (DbConnectionMappingContext)List[index];
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			foreach (DbConnectionMappingContext context in List)
			{
				context.Dispose();
			}
		}

		#endregion
	}
}
