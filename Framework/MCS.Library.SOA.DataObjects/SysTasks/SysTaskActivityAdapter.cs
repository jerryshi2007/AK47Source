using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects
{
	public class SysTaskActivityAdapter : UpdatableAndLoadableAdapterBase<SysTaskActivity, SysTaskActivityCollection>
	{
		public static readonly SysTaskActivityAdapter Instance = new SysTaskActivityAdapter();

		private SysTaskActivityAdapter()
		{
		}

		public SysTaskActivityCollection LoadByProcessID(string processID)
		{
			return this.LoadByInBuilder(inBuilder =>
						{
							inBuilder.DataField = "PROCESS_ID";
							inBuilder.AppendItem(processID);
						},
						orderBuilder =>
						{
							orderBuilder.AppendItem("SEQUENCE", FieldSortDirection.Ascending);
						});
		}

		protected override void AfterLoad(SysTaskActivityCollection data)
		{
			base.AfterLoad(data);

			data.ForEach(a => a.Loaded = true);
		}

		protected override void BeforeInnerUpdate(SysTaskActivity data, Dictionary<string, object> context)
		{
			if (data.Loaded == false && data.Task != null)
			{
				data.Task.FillData(null);
				data.TaskData = JSONSerializerExecute.Serialize(data.Task);
			}

			base.BeforeInnerUpdate(data, context);
		}
	}
}
