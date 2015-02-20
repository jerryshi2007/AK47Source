#region 作者版本
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	UserTaskCollection.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070705		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.Security.Permissions;
using MCS.Library.Data;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	public class UserTaskCollection : EditableDataObjectCollectionBase<UserTask>
	{
		/// <summary>
		/// 将Task的收件人，转换成AclItemCollection
		/// </summary>
		/// <returns></returns>
		public WfAclItemCollection ToAcl()
		{
			WfAclItemCollection result = new WfAclItemCollection();

			foreach (UserTask task in this)
			{
				WfAclItem item = new WfAclItem();

				item.ObjectID = task.SendToUserID;
				item.ObjectName = task.SendToUserName;
				item.ObjectType = SchemaType.Users.ToString();
				//item.ResourceID = task.ResourceID;
				item.ResourceID = WfRuntime.GetProcessByProcessID(task.ProcessID).SearchID;
				item.Source = task.ActivityID;

				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// 删除所有收件人为空的待办\待阅
		/// </summary>
		public void RemoveEmptySendToUserTasks()
		{
			int i = 0;

			while (i < this.Count)
			{
				UserTask task = this[i];

				if (task.SendToUserID.IsNullOrEmpty())
					this.RemoveAt(i);
				else
					i++;
			}
		}

		/// <summary>
		/// 根据Activity、收件人和阅办标记来排除相同的待办\待阅
		/// </summary>
		public void DistinctByActivityUserAndStatus()
		{
			RemoveEmptySendToUserTasks();
			Distinct((task1, task2) => task1.ActivityID == task2.ActivityID && task1.SendToUserID == task2.SendToUserID && task1.Status == task2.Status);
		}

		/// <summary>
		/// 若该集合类刚从库中取出，各UserTask的Category子对象中只有CategoryID被赋值，通过该方法取出Category的其他属性
		/// </summary>
		public void FillUserTasksCategory()
		{
			string[] CategoryIDs = GetCategoryIDsFromUserTaskCollection(this);

			TaskCategoryCollection taskCategories = TaskCategoryAdapter.Instance.GetCategoriesByCategoryIDs(CategoryIDs);

			foreach (UserTask task in this)
			{
				foreach (TaskCategory category in taskCategories)
				{
					if (task.Category.CategoryID == category.CategoryID)
					{
						task.Category = category;
						break;
					}
				}
			}
		}

		/// <summary>
		/// 由UserTaskCollection返回各UserTask的Category子对象的CategoryID数组
		/// </summary>
		/// <param name="userTasks"></param>
		/// <returns></returns>
		private string[] GetCategoryIDsFromUserTaskCollection(UserTaskCollection userTasks)
		{
			string[] CategoryIDs = new string[userTasks.Count];

			for (int i = 0; i < userTasks.Count; i++)
			{
				CategoryIDs[i] = userTasks[i].Category.CategoryID;
			}

			return CategoryIDs;
		}
	}
}
