using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 
	/// </summary>
	public class WfActivityLevelGroupCollection : GroupNodeCollection<WfActivityLevelGroup, string, WfSimpleActivityDescriptorCollection>
	{
		public WfActivityLevelGroupCollection(IEnumerable<IWfActivityDescriptor> activities)
		{
			FillData(activities, new GetGroupKeyForItemDelegate<string, IWfActivityDescriptor>(GetGroupKeyForItem));
		}

		/// <summary>
		/// 得到所有顺序的环节节点
		/// </summary>
		/// <returns></returns>
		public IList<IWfActivityDescriptor> GetAllSequenceActivities()
		{
			List<IWfActivityDescriptor> allDesps = new List<IWfActivityDescriptor>();

			foreach (WfActivityLevelGroup group in this)
				group.Data.ForEach(actDesp => allDesps.Add(actDesp));

			return allDesps;
		}

		/// <summary>
		/// 找到当前节点所属的环节
		/// </summary>
		/// <param name="srcActDesp"></param>
		/// <returns></returns>
		public IWfActivityDescriptor FindLevelActivity(IWfActivityDescriptor srcActDesp)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(srcActDesp != null, "srcActDesp");

			IWfActivityDescriptor result = null;

			foreach (WfActivityLevelGroup group in this)
			{
				if (group.Data.Exists(actDesp => string.Compare(actDesp.Key, srcActDesp.Key, true) == 0))
				{
					result = group.Data[0];
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="IWfActivityDescriptor"></typeparam>
		/// <param name="key"></param>
		/// <param name="actDesp"></param>
		/// <returns></returns>
		protected override WfActivityLevelGroup CreateGroupNode<T>(string key, T actDesp)
		{
			WfActivityLevelGroup result = base.CreateGroupNode(key, actDesp);

			result.Description = ((IWfActivityDescriptor)actDesp).Name;

			return result;
		}

		private string GetGroupKeyForItem(IWfActivityDescriptor activity)
		{
			return activity.LevelName;
		}
	}
}
