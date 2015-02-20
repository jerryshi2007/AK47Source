using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.OguObjects;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
	[Serializable]
	public class WfAssigneeCollection : WfCollectionBase<IUser>
	{
		public void Add(IUser user)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(user != null, "user");

			InnerAdd(user);
		}

		public void CopyFrom(WfAssigneeCollection source)
		{
			if (source != null)
			{
				this.RWLock.AcquireWriterLock(lockTimeout);
				try
				{
					this.Clear();
					CopyCollection(this, source);
				}
				finally
				{
					this.RWLock.ReleaseWriterLock();
				}
			}
		}

		public void CopyFrom(IEnumerable<IUser> users)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(users != null, "users");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				this.Clear();
				foreach (IUser user in users)
					this.Add(user);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		public void CopyTo(Array array, int index)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(array != null, "array");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				List.CopyTo(array, index);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		public IUser this[int index]
		{
			get
			{
				return InnerGet(index);
			}
			set
			{
				ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

				this.RWLock.AcquireWriterLock(lockTimeout);
				try
				{
					if (Contains(value) == false)
					{
						if ((value is WfOguUser) == false)
							value = new WfOguUser((IUser)value);

						List[index] = value;
					}
				}
				finally
				{
					this.RWLock.ReleaseWriterLock();
				}
			}
		}

        public IUser this[string userID]
        {
            get
            {
                IUser user = null;

                ExceptionHelper.CheckStringIsNullOrEmpty(userID, "AssigneeUserID");

				this.RWLock.AcquireReaderLock(lockTimeout);
				try
				{
					foreach (IUser tempUser in this)
					{
						if (tempUser.ID == userID)
						{
							user = tempUser;
							break;
						}
					}

					return user;
				}
				finally
				{
					this.RWLock.ReleaseReaderLock();
				}
            }
        }

		public override bool Contains(IUser target)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");

			bool found = false;

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (IUser user in this)
				{
					if (string.Compare(user.ID, target.ID, true) == 0 &&
						string.Compare(user.FullPath, target.FullPath, true) == 0)
					{
						found = true;
						break;
					}
				}

				return found;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		public bool Contains(string userID)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(userID, "userID");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				return Find(delegate(IUser user)
						{ return string.Compare(user.ID, userID, true) == 0; }) != null;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		public void Remove(IUser user)
		{
			InnerRemove(user);
		}

		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null , "value");

			base.OnValidate(value);
		}

		protected override void OnRemove(int index, object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

			base.OnRemove(index, value);
		}

		protected override void InnerAdd(IUser user)
		{
			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				if ((user is WfOguUser) == false)
					user = new WfOguUser(user);

				if (Contains(user) == false)
					List.Add(user);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		private void CopyCollection(WfAssigneeCollection dest, WfAssigneeCollection source)
		{
			foreach (IUser user in source)
				dest.Add(user);
		}
	}
}
