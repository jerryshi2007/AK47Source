using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 意见
	/// </summary>
	public class GenericOpinionArchiveOperation : IArchiveOperation
	{
		public static readonly GenericOpinionArchiveOperation Instance = new GenericOpinionArchiveOperation();

		private GenericOpinionArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			info.Context["Opinions"] = GenericOpinionAdapter.Instance.LoadFromResourceID(info.ResourceID);
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<GenericOpinion>().Clone();

			mappings["ISSUE_DATETIME"].BindingFlags |= (ClauseBindingFlags.Update | ClauseBindingFlags.Insert);
			mappings["APPEND_DATETIME"].BindingFlags |= (ClauseBindingFlags.Update | ClauseBindingFlags.Insert);
			ORMappingContextCache.Instance[typeof(GenericOpinion)] = mappings;

			try
			{
				info.Context.DoAction<GenericOpinionCollection>("Opinions", opinions =>
				{
					opinions.ForEach(o => GenericOpinionAdapter.Instance.Update(o));
				});
			}
			finally
			{
				ORMappingContextCache.Instance.Remove(typeof(GenericOpinion));
			}
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<GenericOpinionCollection>("Opinions", opinions =>
			{
				opinions.ForEach(o => GenericOpinionAdapter.Instance.Delete(o));
			});
		}

		#endregion
	}
}
