using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class DelayActionCollection : EditableDataObjectCollectionBase<IDelayAction>
	{
		protected override void OnInsert(int index, object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			base.OnInsert(index, value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (newValue == null)
				throw new ArgumentNullException("newValue");

			base.OnSet(index, oldValue, newValue);
		}

		/// <summary>
		/// 执行Action操作
		/// </summary>
		/// <param stringValue="context"></param>
		public void DoActions(SynchronizeContext context)
		{
			for (int i = 0; i < this.Count; i++)
			{
				SynchronizeContext.Current.ExtendLockTime();

				this[i].DoAction(context);
			}

			this.Clear();
		}
	}
}
