using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public enum WfMatrixOperatorType
	{
		Person = 0,
		Role = 1
	}

	[Serializable]
	public class WfMatrixRow
	{
		public WfMatrixRow()
		{
		}

		public WfMatrixRow(WfMatrix matrix)
		{
			matrix.NullCheck("matrix");

			this.Matrix = matrix;
		}

		public int RowNumber
		{
			get;
			set;
		}

		internal WfMatrix Matrix
		{
			get;
			set;
		}

		private WfMatrixCellCollection _Cells = null;

		public WfMatrixCellCollection Cells
		{
			get
			{
				if (this._Cells == null)
					this._Cells = new WfMatrixCellCollection();

				return this._Cells;
			}
		}

		public string Operator { get; set; }

		public WfMatrixOperatorType OperatorType { get; set; }

		/// <summary>
		/// 将某个操作人，换成某几个操作人
		/// </summary>
		/// <param name="originalOperator"></param>
		/// <param name="replaceOperators"></param>
		/// <returns>替换了几次。最多是一次替换。没有替换则返回0</returns>
		public int ReplaceOperator(string originalOperator, params string[] replaceOperators)
		{
			int result = 0;

			if (this.Operator.IsNotEmpty() && originalOperator.IsNotEmpty())
			{
				string[] originalUsers = this.Operator.Split(SOARolePropertyRow.OperatorSplitters, StringSplitOptions.RemoveEmptyEntries);

				//先整理一下需要被替换的人
				List<string> replaceUsers = new List<string>();

				if (replaceOperators != null)
				{
					foreach (string user in replaceOperators)
					{
						if (originalUsers.Exists(s => string.Compare(s, user, true) == 0) == false)
							replaceUsers.Add(user);
					}
				}

				List<string> targetUsers = new List<string>();

				bool added = false;

				foreach (string user in originalUsers)
				{
					if (string.Compare(user, originalOperator, true) == 0)
					{
						result = 1;

						if (added == false)
						{
							targetUsers.AddRange(replaceUsers);
							added = true;
						}
					}
					else
						targetUsers.Add(user);
				}

				this.Operator = string.Join(",", targetUsers.ToArray());

				WfMatrixCell cell = this.Cells.FindByDefinitionKey("Operator");

				if (cell != null)
					cell.StringValue = this.Operator;
			}

			return result;
		}
	}

	[Serializable]
	public class WfMatrixRowCollection : SerializableEditableKeyedDataObjectCollectionBase<int, WfMatrixRow>
	{
		private WfMatrix _Matrix = null;

		public WfMatrixRowCollection()
		{
		}

		public WfMatrixRowCollection(WfMatrix matrix)
		{
			matrix.NullCheck("matrix");
			this._Matrix = matrix;
		}

		public void CopyFrom(System.Data.DataView view)
		{
			foreach (System.Data.DataRowView drv in view)
			{
				int rowID = (int)drv["MATRIX_ROW_ID"];

				WfMatrixRow row = this[rowID];

				if (row == null)
				{
					row = new WfMatrixRow(this._Matrix)
					{
						RowNumber = rowID,
						OperatorType = (WfMatrixOperatorType)drv["OPERATOR_TYPE"],
						Operator = drv["OPERATOR"].ToString(),
					};
					this.Add(row);
				}

				row.Cells.Add(new WfMatrixCell()
				{
					Definition = this._Matrix.Definition.Dimensions[drv["DIMENSION_KEY"].ToString()],
					StringValue = drv["STRING_VALUE"].ToString()
				});
			}
		}

		public int ReplcaeOperators(WfMatrixOperatorType typeFilter, string originalOperator, params string[] replaceOperators)
		{
			int result = 0;

			foreach (WfMatrixRow row in this)
			{
				if (row.OperatorType == typeFilter)
					result += row.ReplaceOperator(originalOperator, replaceOperators);
			}

			return result;
		}

		/// <summary>
		/// 生成矩阵行中所有人员，人员不会重复
		/// </summary>
		/// <returns></returns>
		public WfMatrixRowUsersCollection GenerateRowsUsers()
		{
			WfMatrixRowUsersCollection result = new WfMatrixRowUsersCollection();

			foreach (WfMatrixRow row in this)
			{
				WfMatrixRowUsers rowUsers = new WfMatrixRowUsers(row);

				rowUsers.ObjectIDs = GenerateObjectIDs(row);

				result.Add(rowUsers);
			}

			FillPersonTypeUsers(result);

			FillRoleTypeUsers(result);

			return result;
		}

		/// <summary>
		/// 输出矩阵行中直接包含的人员，人员不会重复
		/// </summary>
		/// <returns></returns>
		public WfMatrixRowUsersCollection GenerateRowsDirectUsers()
		{
			WfMatrixRowUsersCollection result = new WfMatrixRowUsersCollection();

			foreach (WfMatrixRow row in this)
			{
				WfMatrixRowUsers rowUsers = new WfMatrixRowUsers(row);

				rowUsers.ObjectIDs = GenerateObjectIDs(row);

				result.Add(rowUsers);
			}

			FillPersonTypeUsers(result);

			return result;
		}

		private void FillRoleTypeUsers(WfMatrixRowUsersCollection result)
		{
			foreach (var rowUsers in result)
			{
				if (rowUsers.Row.OperatorType == WfMatrixOperatorType.Role)
				{
					var users = WfMatrix.GetUsersInRole(rowUsers.Row.Operator);

					foreach (IUser userInRole in users)
					{
						if (rowUsers.Users.Exists(u => string.Compare(u.ID, userInRole.ID, true) == 0) == false)
							rowUsers.Users.Add(userInRole);
					}
				}
			}
		}

		private static void FillPersonTypeUsers(WfMatrixRowUsersCollection rowsUsers)
		{
			Dictionary<string, string> userIDs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach (WfMatrixRowUsers rowUsers in rowsUsers)
			{
				if (rowUsers.Row.OperatorType == WfMatrixOperatorType.Person)
				{
					foreach (string id in rowUsers.ObjectIDs)
						userIDs[id] = id;
				}
			}

			List<string> logonNames = new List<string>();

			foreach (KeyValuePair<string, string> kp in userIDs)
				logonNames.Add(kp.Key);

			OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, logonNames.ToArray());

			if (WfRuntime.ProcessContext.CurrentProcess != null)
				users = users.FilterUniqueSidelineUsers(WfRuntime.ProcessContext.CurrentProcess.OwnerDepartment);

			Dictionary<string, IUser> userDicts = GenerateUserDictionary(users);

			foreach (WfMatrixRowUsers rowUsers in rowsUsers)
			{
				if (rowUsers.Row.OperatorType == WfMatrixOperatorType.Person)
				{
					foreach (string id in rowUsers.ObjectIDs)
					{
						IUser user = null;
						if (userDicts.TryGetValue(id, out user))
						{
							if (rowUsers.Users.Exists(u => string.Compare(u.ID, user.ID, true) == 0) == false)
								rowUsers.Users.Add((IUser)OguUser.CreateWrapperObject(user));
						}
					}
				}
			}
		}

		private static Dictionary<string, IUser> GenerateUserDictionary(IEnumerable<IUser> users)
		{
			Dictionary<string, IUser> result = new Dictionary<string, IUser>(StringComparer.OrdinalIgnoreCase);

			foreach (IUser user in users)
			{
				IUser originalUser = null;

				if (result.TryGetValue(user.LogOnName, out originalUser))
				{
					if (originalUser.IsSideline && user.IsSideline == false)
						result[user.LogOnName] = user;
				}
				else
				{
					result.Add(user.LogOnName, user);
				}
			}

			return result;
		}

		private static List<string> GenerateObjectIDs(WfMatrixRow row)
		{
			List<string> objIds = new List<string>();

			string[] ids = row.Operator.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string id in ids)
			{
				string trimmedID = id.Trim();

				objIds.Add(trimmedID);
			}

			return objIds;
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
			((WfMatrixRow)value).Matrix = this._Matrix;
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			base.OnSet(index, oldValue, newValue);

			((WfMatrixRow)newValue).Matrix = this._Matrix;
		}

		protected override int GetKeyForItem(WfMatrixRow item)
		{
			return item.RowNumber;
		}
	}
}
