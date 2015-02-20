using System;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class WorkBookViewCollection : ExcelCollectionBase<int, WorkBookView>
	{
		public int GetNextBookViewID()
		{
			return this.Count;
		}

		protected override int GetKeyForItem(WorkBookView item)
		{
			return item.BookViewID;
		}
	}
}
