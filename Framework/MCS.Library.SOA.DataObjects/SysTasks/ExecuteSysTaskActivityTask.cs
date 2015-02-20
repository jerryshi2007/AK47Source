using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	public class ExecuteSysTaskActivityTask : SysTask
	{
		[NonSerialized]
		private string _ActivityID = string.Empty;

		public ExecuteSysTaskActivityTask()
		{
			this.TaskType = "ExecuteSysTaskActivity";
			this.Category = "SysTaskProcess";
		}

		public ExecuteSysTaskActivityTask(string activityID)
			: this()
		{
			activityID.CheckStringIsNullOrEmpty("activityID");

			this._ActivityID = activityID;
		}

		public ExecuteSysTaskActivityTask(SysTask other)
			: base(other)
		{
			this.AfterLoad();
		}

		/// <summary>
		/// 发送执行任务流程活动的任务
		/// </summary>
		/// <param name="sysTaskActivity"></param>
		/// <returns></returns>
		public static ExecuteSysTaskActivityTask SendTask(SysTaskActivity sysTaskActivity)
		{
			ExecuteSysTaskActivityTask task = ExecuteSysTaskActivityTask.CreateTask(sysTaskActivity);

			SysTaskAdapter.Instance.Update(task);

			return task;
		}

		public static ExecuteSysTaskActivityTask CreateTask(SysTaskActivity sysTaskActivity)
		{
			sysTaskActivity.NullCheck("sysTaskActivity");

			ExecuteSysTaskActivityTask task = new ExecuteSysTaskActivityTask(sysTaskActivity.ID);

			task.TaskID = UuidHelper.NewUuidString();
			task.ResourceID = sysTaskActivity.ID;

			if (sysTaskActivity.Task != null && sysTaskActivity.Task.TaskTitle.IsNotEmpty())
			{
				task.TaskTitle = string.Format("执行任务流程活动，任务ID为{0}，名称为{1}",
					sysTaskActivity.Task.TaskID,
					sysTaskActivity.Task.TaskTitle);
			}
			else
			{
				task.TaskTitle = string.Format("执行任务流程活动{0}", sysTaskActivity.ID);
			}

			return task;
		}

		public string ActivityID
		{
			get
			{
				return this._ActivityID;
			}
			set
			{
				this._ActivityID = value;
			}
		}

		public override void FillData(Dictionary<string, string> extraData)
		{
			if (extraData == null)
				extraData = new Dictionary<string, string>();

			extraData["ActivityID"] = this.ActivityID;

			base.FillData(extraData);
		}

		public override void AfterLoad()
		{
			if (this.Data.IsNotEmpty())
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument(this.Data);

				string serilizedSvcDefData = XmlHelper.GetSingleNodeText(xmlDoc.DocumentElement, "SvcOperationDefs");

				this.ActivityID = XmlHelper.GetSingleNodeText(xmlDoc.DocumentElement, "ActivityID");
			}
		}
	}
}
