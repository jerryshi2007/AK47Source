using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class AppCommonInfoArchiveOperation : IArchiveOperation
	{
		public static AppCommonInfoArchiveOperation Instance = new AppCommonInfoArchiveOperation();

		private AppCommonInfoArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			info.Context["AppCommonInfo"] = AppCommonInfoAdapter.Instance.Load(info.ResourceID, false);
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<AppCommonInfo>().Clone();

			mappings["CREATE_TIME"].BindingFlags |= (ClauseBindingFlags.Update | ClauseBindingFlags.Insert);
			ORMappingContextCache.Instance[typeof(AppCommonInfo)] = mappings;

			try
			{
				info.Context.DoAction<AppCommonInfo>("AppCommonInfo", acic =>
				{
					if (acic != null)
					{
						acic.Status = ArchiveStatus.Archived;
						AppCommonInfoAdapter.Instance.Update(acic);
					}
				});
			}
			finally
			{
				ORMappingContextCache.Instance.Remove(typeof(AppCommonInfo));
			}
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<AppCommonInfo>("AppCommonInfo", acic =>
			{
				if (acic != null)
					AppCommonInfoAdapter.Instance.Delete(acic);
			});
		}

		#endregion
	}
}
