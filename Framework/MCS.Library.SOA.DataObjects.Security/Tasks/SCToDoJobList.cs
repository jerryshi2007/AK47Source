using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.Tasks
{
	[Serializable]
	[ORTableMapping("SC.ToDoJobList")]
	public class SCToDoJob
	{
		/// <summary>
		/// 构造一个生成FullPath的作业
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static SCToDoJob CreateGenerateFullPathsJob(SchemaObjectBase obj)
		{
			obj.NullCheck("obj");

			SCToDoJob job = new SCToDoJob();

			job.Type = "GenerateFullPaths";

			if (obj != null)
			{
				job.SourceID = obj.ID;
				job.Description = string.Format("Generate full paths from '{0}'", obj.ID);
				job.Data = obj.ToString();
			}

			return job;
		}

		[ORFieldMapping("ID", PrimaryKey = true, IsIdentity = true)]
		public int ID
		{
			get;
			set;
		}

		[ORFieldMapping("SourceID")]
		public string SourceID
		{
			get;
			set;
		}

		[ORFieldMapping("Type")]
		public string Type
		{
			get;
			set;
		}

		[ORFieldMapping("Description")]
		public string Description
		{
			get;
			set;
		}

		[ORFieldMapping("Data")]
		public string Data
		{
			get;
			set;
		}

		[ORFieldMapping("CreateTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Where | ClauseBindingFlags.Select, DefaultExpression = "GETDATE()")]
		public DateTime CreateTime
		{
			get;
			set;
		}

		[ORFieldMapping("ExecuteTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Where | ClauseBindingFlags.Select)]
		public DateTime ExecuteTime
		{
			get;
			set;
		}
	}
}
