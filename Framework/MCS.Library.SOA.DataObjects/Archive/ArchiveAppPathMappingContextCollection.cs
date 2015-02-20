using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class ArchiveAppPathMappingContextCollection : ReadOnlyDataObjectCollectionBase<AppPathMappingContext>, IDisposable
	{
		internal void Add(AppPathMappingContext context)
		{
			this.List.Add(context);
		}

		public AppPathMappingContext this[int index]
		{
			get
			{
				return (AppPathMappingContext)List[index];
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			foreach (AppPathMappingContext context in List)
			{
				context.Dispose();
			}
		}

		#endregion
	}
}
