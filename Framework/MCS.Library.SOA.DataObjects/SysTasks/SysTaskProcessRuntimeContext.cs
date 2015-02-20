using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	public class SysTaskProcessRuntimeContext
	{
		private SysTaskProcessCollection _AffectedProcesses = null;
		private SysTaskActivityCollection _AffectedActivities = null;
		private SysTaskActivityCollection _DeletedActivities = null;
		private SysTaskProcessCollection _LoadedProcesses = null;

		internal SysTaskProcessRuntimeContext()
		{
		}

		internal SysTaskProcessCollection LoadedProcesses
		{
			get
			{
				if (this._LoadedProcesses == null)
					this._LoadedProcesses = new SysTaskProcessCollection();

				return this._LoadedProcesses;
			}
		}

		/// <summary>
		/// 受影响的流程
		/// </summary>
		public SysTaskProcessCollection AffectedProcesses
		{
			get
			{
				if (this._AffectedProcesses == null)
					this._AffectedProcesses = new SysTaskProcessCollection();

				return this._AffectedProcesses;
			}
		}

		/// <summary>
		/// 受影响的活动
		/// </summary>
		public SysTaskActivityCollection AffectedActivities
		{
			get
			{
				if (this._AffectedActivities == null)
					this._AffectedActivities = new SysTaskActivityCollection();

				return this._AffectedActivities;
			}
		}

		/// <summary>
		/// 需要被删除的活动
		/// </summary>
		public SysTaskActivityCollection DeletedActivities
		{
			get
			{
				if (this._DeletedActivities == null)
					this._DeletedActivities = new SysTaskActivityCollection();

				return this._DeletedActivities;
			}
		}
	}
}
