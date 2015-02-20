using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data;

namespace MCS.Library.Accredit
{
	internal abstract class OriginalDataTableOperationBase
	{
		private AD2DBTransferContext context = null;

		private int addCount = 0;

		public int AddCount
		{
			get { return addCount; }
			set { addCount = value; }
		}

		private int updateCount = 0;

		public int UpdateCount
		{
			get { return updateCount; }
			set { updateCount = value; }
		}

		private int deleteCount = 0;

		public int DeleteCount
		{
			get { return deleteCount; }
			set { deleteCount = value; }
		}

		public OriginalDataTableOperationBase(AD2DBTransferContext ctx)
		{
			this.context = ctx;
		}

		public AD2DBTransferContext Context
		{
			get { return this.context; }
		}

		public abstract void CompareAndModifyData();

		public abstract void DeleteOperation();

		public abstract void UpdateOperation();

		public abstract void AddOperation();

		public static void UpdateSort(DataRow dr, AD2DBTransferContext context)
		{
			context.OriginalData.Tables["ORGANIZATIONS"].PrimaryKey = new DataColumn[] { context.OriginalData.Tables["ORGANIZATIONS"].Columns["GUID"] };
			DataRow tempdr = context.OriginalData.Tables["ORGANIZATIONS"].Rows.Find(dr["PARENT_GUID"]);
			int counter = Convert.ToInt32(tempdr["CHILDREN_COUNTER"]) + 1;
			tempdr["CHILDREN_COUNTER"] = counter;

			string innerSort = string.Format("{0:000000}", counter);
			string originalSort = tempdr["ORIGINAL_SORT"].ToString() + innerSort;
			string globalSort = tempdr["GLOBAL_SORT"].ToString() + innerSort;

			dr["ORIGINAL_SORT"] = originalSort;
			dr["GLOBAL_SORT"] = globalSort;
			dr["INNER_SORT"] = innerSort;
		}
	}
}
