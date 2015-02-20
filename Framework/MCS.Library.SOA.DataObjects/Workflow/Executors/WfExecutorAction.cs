using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Executor的执行步骤
	/// </summary>
	[Serializable]
	internal class WfExecutorAction
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="actionName"></param>
		/// <param name="action"></param>
		public WfExecutorAction(string actionName, Action<WfExecutorDataContext> action)
		{
			actionName.CheckStringIsNullOrEmpty("actionName");
			action.NullCheck("action");

			this.ActionName = actionName;
			this.Action = action;
		}

		/// <summary>
		/// 执行步骤
		/// </summary>
		public string ActionName
		{
			get;
			private set;
		}

		/// <summary>
		/// 动作
		/// </summary>
		public Action<WfExecutorDataContext> Action
		{
			get;
			private set;
		}
	}

	internal class WfExecutorActionCollection : EditableKeyedDataObjectCollectionBase<string, WfExecutorAction>
	{
		protected override string GetKeyForItem(WfExecutorAction item)
		{
			return item.ActionName;
		}

		public void ExecuteActionByName(string actionName, WfExecutorDataContext dataContext)
		{
			actionName.CheckStringIsNullOrEmpty("actionName");

			WfExecutorAction action = this[actionName];

			(action != null).FalseThrow("不能找到名称为{0}的操作", actionName);

			action.Action(dataContext);
		}
	}
}
