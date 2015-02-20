using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	[Serializable]
	[ORTableMapping("SC.Conditions")]
	public class SCCondition : ISCStatusObject
	{
		private SchemaObjectStatus _Status = SchemaObjectStatus.Normal;

		public SCCondition()
		{
		}

		public SCCondition(string expression)
		{
			this.Condition = expression;
		}

		[ORFieldMapping("OwnerID", PrimaryKey = true)]
		public string OwnerID
		{
			get;
			set;
		}

		[ORFieldMapping("Type", PrimaryKey = true)]
		public string Type
		{
			get;
			set;
		}

		[ORFieldMapping("SortID", PrimaryKey = true)]
		public int SortID
		{
			get;
			set;
		}

		[ORFieldMapping("Condition")]
		public string Condition
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

		[ORFieldMapping("VersionStartTime")]
		public DateTime VersionStartTime
		{
			get;
			set;
		}

		[ORFieldMapping("VersionEndTime")]
		public DateTime VersionEndTime
		{
			get;
			set;
		}

		[ORFieldMapping("Status")]
		public SchemaObjectStatus Status
		{
			get
			{
				return this._Status;
			}

			set
			{
				this._Status = value;
			}
		}
	}

	[Serializable]
	public class SCConditionCollection : EditableDataObjectCollectionBase<SCCondition>
	{
		/// <summary>
		/// 按照类型筛选出条件集合
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public SCConditionCollection GetConditionsByType(string type)
		{
			SCConditionCollection result = new SCConditionCollection();

			foreach (SCCondition condition in this)
			{
				if (condition.Type == type)
					result.Add(condition);
			}

			return result;
		}

		public void ReplaceItemsWith(SCConditionCollection source, string ownerID, string type)
		{
			DateTime ve = DateTime.Now;
			DateTime vm = new DateTime(9999, 9, 9);

			//合并时，先将已经删除的过滤，将剩下的设置成删除，添加新的项目。
			int i = 0;

			while (i < this.Count)
			{
				if (this[i].Status != SchemaObjectStatus.Normal)
				{
					this.RemoveAt(i);
				}
				else
				{
					this[i].Status = SchemaObjectStatus.Deleted;
					this[i].VersionEndTime = ve;
					i++;
				}
			}

			for (i = 0; i < source.Count; i++)
			{
				var item = source[i];
				item.SortID = i + 1;
				item.OwnerID = ownerID;
				item.Type = type;
				item.VersionEndTime = vm;
				item.VersionStartTime = ve;
				this.Add(item);
			}
		}
	}
}
