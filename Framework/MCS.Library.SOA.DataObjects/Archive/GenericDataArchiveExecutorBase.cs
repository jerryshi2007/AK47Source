using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public abstract class GenericDataArchiveExecutorBase : ArchiveExecutorBase
	{
		protected override void LoadOriginalData(ArchiveBasicInfo info)
		{
			WorkflowArchiveOperation.Instance.LoadOriginalData(info);

			GenericOpinionArchiveOperation.Instance.LoadOriginalData(info);
			MaterialArchiveOperation.Instance.LoadOriginalData(info);

			AppCommonInfoArchiveOperation.Instance.LoadOriginalData(info);

			AclArchiveOperation.Instance.LoadOriginalData(info);

			UserAccomplishedTaskArchiveOperation.Instance.LoadOriginalData(info);
			UserOpLogArchiveOperation.Instance.LoadOriginalData(info);

			UserTaskArchiveOperation.Instance.LoadOriginalData(info);
		}

		protected override void SaveArchiveData(ArchiveBasicInfo info)
		{
			WorkflowArchiveOperation.Instance.SaveArchiveData(info);

			GenericOpinionArchiveOperation.Instance.SaveArchiveData(info);
			MaterialArchiveOperation.Instance.SaveArchiveData(info);

			AppCommonInfoArchiveOperation.Instance.SaveArchiveData(info);

			AclArchiveOperation.Instance.SaveArchiveData(info);

			UserAccomplishedTaskArchiveOperation.Instance.SaveArchiveData(info);
			UserOpLogArchiveOperation.Instance.SaveArchiveData(info);

			UserTaskArchiveOperation.Instance.SaveArchiveData(info);
		}

		protected override void DeleteOriginalData(ArchiveBasicInfo info)
		{
			WorkflowArchiveOperation.Instance.DeleteOriginalData(info);

			GenericOpinionArchiveOperation.Instance.DeleteOriginalData(info);
			MaterialArchiveOperation.Instance.DeleteOriginalData(info);

			AppCommonInfoArchiveOperation.Instance.DeleteOriginalData(info);

			AclArchiveOperation.Instance.DeleteOriginalData(info);

			UserAccomplishedTaskArchiveOperation.Instance.DeleteOriginalData(info);
			UserOpLogArchiveOperation.Instance.DeleteOriginalData(info);

			UserTaskArchiveOperation.Instance.DeleteOriginalData(info);
		}
	}
}
