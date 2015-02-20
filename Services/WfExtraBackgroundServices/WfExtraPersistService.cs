using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Services;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Core;

namespace WfExtraBackgroundServices
{
	public sealed class WfExtraPersistService : ThreadTaskBase
	{
		public WfExtraPersistService()
		{
		}

		public override void OnThreadTaskStart()
		{
			try
			{
				WfPersistQueueAdapter.Instance.FetchQueueItemsAndDoOperation(this.Params.BatchCount);
			}
			catch (Exception ex)
			{
				this.Params.Log.Write(ex);
			}
		}
	}
}
