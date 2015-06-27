using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	public class SOARolePropertyRowUsers
	{
		public SOARolePropertyRowUsers(SOARolePropertyRow rolePropertyRow)
		{
			Row = rolePropertyRow;
		}

		private SOARolePropertyRow _Row;

		public SOARolePropertyRow Row
		{
			get { return this._Row; }
			internal set { _Row = value; }
		}

		private OguDataCollection<IUser> _Users = new OguDataCollection<IUser>();

		public OguDataCollection<IUser> Users
		{
			get
			{
				return this._Users;
			}
		}

		internal List<string> ObjectIDs = null;

        internal List<string> EnterNotifyIDs = null;

        internal List<string> LeaveNotifyIDs = null;

        private OguDataCollection<IUser> _EnterNotifyUsers = new OguDataCollection<IUser>();

        public OguDataCollection<IUser> EnterNotifyUsers
        {
            get
            {
                return this._EnterNotifyUsers;
            }
        }

        private OguDataCollection<IUser> _LeaveNotifyUsers = new OguDataCollection<IUser>();

        public OguDataCollection<IUser> LeaveNotifyUsers
        {
            get
            {
                return this._LeaveNotifyUsers;
            }
        }
	}

	[Serializable]
	public class SOARolePropertyRowUsersCollection : SerializableEditableKeyedDataObjectCollectionBase<SOARolePropertyRow, SOARolePropertyRowUsers>
	{
		protected override SOARolePropertyRow GetKeyForItem(SOARolePropertyRowUsers item)
		{
			return item.Row;
		}

		public void SortByActivitySN()
		{
			this.Sort((x, y) =>
			{
				float xSN = 0;
                float.TryParse(x.Row.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, "0"), out xSN);

                float ySN = 0;
                float.TryParse(y.Row.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, "0"), out ySN);

				return (int)(xSN - ySN);
			});
		}

		/// <summary>
		/// 去掉可以合并的行（人员在后续环节出现过，前面的环节就可以合并）。
		/// </summary>
		public void RemoveMergeableRows()
		{
			WfMergeMatrixRowParams eventArgs = new WfMergeMatrixRowParams();

			WfRuntime.ProcessContext.FireRemoveMatrixMergeableRows(this, eventArgs);

			switch (eventArgs.Method)
			{
				case WfMergeMatrixRowMethod.KeepTheLastestRow:
					MergeLastestRows();
					break;
				case WfMergeMatrixRowMethod.KeepTheEarliestRow:
					MergeEarliestRows();
					break;
			}
		}

		private void MergeLastestRows()
		{
			List<SOARolePropertyRowUsers> tempList = new List<SOARolePropertyRowUsers>();

			for (int i = 0; i < this.Count; i++)
			{
				SOARolePropertyRowUsers rowUsers = this[i];

				//得到当前行中，在后续环节中不存在的用户
				OguDataCollection<IUser> mergedUsers = GetLastestMergedUsers(rowUsers, i + 1);

				if (rowUsers.Row.IsMergeable())
				{
					//本行可合并，并且本行经过合并后，还有人存在...
					if (mergedUsers.Count > 0)
					{
						tempList.Add(rowUsers);

						//以合并后的人为准
						if (mergedUsers.Count != rowUsers.Users.Count)
						{
							rowUsers.Users.Clear();
							rowUsers.Users.CopyFrom(mergedUsers);
						}
					}
					//合并后没有人存在时，则舍弃该行
				}
				else
					tempList.Add(rowUsers);
			}

			this.Clear();
			this.CopyFrom(tempList);
		}

		private void MergeEarliestRows()
		{
			List<SOARolePropertyRowUsers> tempList = new List<SOARolePropertyRowUsers>();

			for (int i = this.Count - 1; i >= 0; i--)
			{
				SOARolePropertyRowUsers rowUsers = this[i];

				//得到当前行中，在后续环节中不存在的用户
				OguDataCollection<IUser> mergedUsers = GetEarliestMergedUsers(rowUsers, i - 1);

				if (rowUsers.Row.IsMergeable())
				{
					//本行可合并，并且本行经过合并后，还有人存在...
					if (mergedUsers.Count > 0)
					{
						tempList.Add(rowUsers);

						//以合并后的人为准
						if (mergedUsers.Count != rowUsers.Users.Count)
						{
							rowUsers.Users.Clear();
							rowUsers.Users.CopyFrom(mergedUsers);
						}
					}
					//合并后没有人存在时，则舍弃该行
				}
				else
					tempList.Add(rowUsers);
			}

			this.Clear();
			this.CopyFrom(tempList);
		}

		/// <summary>
		/// 得到当前行中，在后续环节中不存在的用户
		/// </summary>
		/// <param name="rowUsers"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		private OguDataCollection<IUser> GetLastestMergedUsers(SOARolePropertyRowUsers rowUsers, int startIndex)
		{
			OguDataCollection<IUser> result = new OguDataCollection<IUser>();

			foreach (IUser user in rowUsers.Users)
			{
				if (ExistsLastestUser(user, startIndex) == false)
					result.Add(user);
			}

			return result;
		}

		/// <summary>
		/// user是否在startIndex及其之后的行中存在？
		/// </summary>
		/// <param name="user"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		private bool ExistsLastestUser(IUser user, int startIndex)
		{
			bool result = false;

			for (int i = startIndex; i < this.Count; i++)
			{
				SOARolePropertyRowUsers rowUsers = this[i];

				if (rowUsers.Users.Exists(u => string.Compare(u.ID, user.ID, true) == 0))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		private OguDataCollection<IUser> GetEarliestMergedUsers(SOARolePropertyRowUsers rowUsers, int startIndex)
		{
			OguDataCollection<IUser> result = new OguDataCollection<IUser>();

			foreach (IUser user in rowUsers.Users)
			{
				if (ExistsEarliestUser(user, startIndex) == false)
					result.Add(user);
			}

			return result;
		}

		private bool ExistsEarliestUser(IUser user, int startIndex)
		{
			bool result = false;

			for (int i = startIndex; i >= 0; i--)
			{
				SOARolePropertyRowUsers rowUsers = this[i];

				if (rowUsers.Users.Exists(u => string.Compare(u.ID, user.ID, true) == 0))
				{
					result = true;
					break;
				}
			}

			return result;
		}
	}
}
