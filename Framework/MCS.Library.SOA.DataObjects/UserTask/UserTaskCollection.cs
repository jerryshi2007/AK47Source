#region ���߰汾
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	UserTaskCollection.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070705		����
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
		/// ��Task���ռ��ˣ�ת����AclItemCollection
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
		/// ɾ�������ռ���Ϊ�յĴ���\����
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
		/// ����Activity���ռ��˺��İ������ų���ͬ�Ĵ���\����
		/// </summary>
		public void DistinctByActivityUserAndStatus()
		{
			RemoveEmptySendToUserTasks();
			Distinct((task1, task2) => task1.ActivityID == task2.ActivityID && task1.SendToUserID == task2.SendToUserID && task1.Status == task2.Status);
		}

		/// <summary>
		/// ���ü�����մӿ���ȡ������UserTask��Category�Ӷ�����ֻ��CategoryID����ֵ��ͨ���÷���ȡ��Category����������
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
		/// ��UserTaskCollection���ظ�UserTask��Category�Ӷ����CategoryID����
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
