using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 缺省的归档执行器
	/// </summary>
	public class BasicFormDataArchiveExecutor : GenericDataArchiveExecutorBase
	{
		public static readonly IArchiveExecutor Instance = new BasicFormDataArchiveExecutor();

		protected BasicFormDataArchiveExecutor()
		{
		}

		protected override void LoadOriginalData(ArchiveBasicInfo info)
		{
			FormDataArchiveDataOperation.Instance.LoadOriginalData(info);

			base.LoadOriginalData(info);
		}

		protected override void SaveArchiveData(ArchiveBasicInfo info)
		{
			FormDataArchiveDataOperation.Instance.SaveArchiveData(info);

			base.SaveArchiveData(info);
		}

		protected override void DeleteOriginalData(ArchiveBasicInfo info)
		{
			FormDataArchiveDataOperation.Instance.DeleteOriginalData(info);

			base.DeleteOriginalData(info);
		}
	}
}
