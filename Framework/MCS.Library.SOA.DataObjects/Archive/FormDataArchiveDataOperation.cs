using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class FormDataArchiveDataOperation : IArchiveOperation
	{
		public static readonly FormDataArchiveDataOperation Instance = new FormDataArchiveDataOperation();

		private FormDataArchiveDataOperation()
		{
		}

		#region IArchiveOperation Members
		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			info.Context["FormData"] = GenericFormDataAdapter.Instance.Load(info.ResourceID, false);
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<GenericFormData>("FormData", data =>
			{
				if (data != null)
					GenericFormDataAdapter.Instance.Update(data);
			});
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<GenericFormData>("FormData", data =>
				{
					if (data != null)
						GenericFormDataAdapter.Instance.Delete(data);
				});
		}
		#endregion
	}
}
