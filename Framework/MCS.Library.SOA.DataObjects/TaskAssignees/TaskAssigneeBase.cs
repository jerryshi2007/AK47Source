using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	public abstract class TaskAssigneeBase<T> where T : IOguObject
	{
		private T assignee = default(T);

		[ORFieldMapping("ID", PrimaryKey = true)]
		public virtual string ID
		{
			get;
			set;
		}

		[ORFieldMapping("RESOURCE_ID")]
		public virtual string ResourceID { get; set; }

		[ORFieldMapping("INNER_ID")]
		public virtual int InnerID { get; set; }

		[ORFieldMapping("TYPE")]
		public virtual string Type { get; set; }

		public virtual T Assignee
		{
			get
			{
				return this.assignee;
			}
			set
			{
				this.assignee = (T)OguUser.CreateWrapperObject(value);
			}
		}

		[ORFieldMapping("URL")]
		public virtual string Url { get; set; }
	}

	[Serializable]
	public class TaskAssigneeCollectionBase<TTaskAssignee, TOguObject> :
		EditableDataObjectCollectionBase<TTaskAssignee>
		where TOguObject : IOguObject
		where TTaskAssignee : TaskAssigneeBase<TOguObject>, new()
	{
		public void Add(string resourceID, string type, IEnumerable<TOguObject> users)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(resourceID, "resourceID");
			ExceptionHelper.CheckStringIsNullOrEmpty(type, "type");

			foreach (TOguObject user in users)
				this.Add(new TTaskAssignee() { ResourceID = resourceID, Type = type, Assignee = user });
		}

        //public void Remove(TTaskAssignee assignee)
        //{
        //    List.Remove(assignee);
        //}

		/// <summary>
		/// 根据类别来筛选出用户
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public OguDataCollection<TOguObject> GetAssigneesByType(string type)
		{
			OguDataCollection<TOguObject> result = new OguDataCollection<TOguObject>();

			this.ForEach(assingee =>
			{
				if (string.Compare(assingee.Type, type, true) == 0)
					result.Add(assingee.Assignee);
			});

			return result;
		}
	}
}
