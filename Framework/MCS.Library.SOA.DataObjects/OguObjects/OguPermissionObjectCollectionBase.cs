using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	public abstract class OguPermissionObjectCollectionBase<T> : EditableDataObjectCollectionBase<T> where T : IPermissionObject
	{
		public OguPermissionObjectCollectionBase()
		{
		}

		public OguPermissionObjectCollectionBase(int capacity)
			: base(capacity)
		{
		}

		public OguPermissionObjectCollectionBase(IEnumerable<T> objs)
		{
			objs.NullCheck("objs");

			objs.ForEach(obj => this.Add(obj));
		}

		protected override void OnInsertComplete(int index, object value)
		{
			T wrappedApp = CreateWrapperObject((T)value);

			this.InnerList[index] = wrappedApp;

			base.OnInsertComplete(index, wrappedApp);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			T wrappedApp = CreateWrapperObject((T)newValue);

			this.InnerList[index] = wrappedApp;

			base.OnSetComplete(index, oldValue, newValue);
		}

		protected abstract T CreateWrapperObject(T obj);
	}
}
