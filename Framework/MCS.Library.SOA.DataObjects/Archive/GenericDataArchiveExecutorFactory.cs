using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class GenericDataArchiveExecutorFactory : IArchiveExecutorFactory
	{
		#region IArchiveExecutorFactory Members

		public IArchiveExecutor GetArchiveExecutor(ArchiveBasicInfo info)
		{
			return BasicFormDataArchiveExecutor.Instance;
		}

		#endregion
	}
}
