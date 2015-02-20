using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ExcelIndexCollection<T> : ExcelCollectionBase<int, T>
		where T : IIndex, new()
	{
		protected WorkSheet _WorkSheet = null;

		public ExcelIndexCollection(WorkSheet sheet)
			: base()
		{
			_WorkSheet = sheet;
		}

		public ExcelIndexCollection(WorkSheet sheet, int capacity)
			: base(capacity)
		{
			_WorkSheet = sheet;
		}

		public ExcelIndexCollection(WorkSheet sheet, IEqualityComparer<int> comparer)
			: base(comparer)
		{
			_WorkSheet = sheet;
		}

		public ExcelIndexCollection(WorkSheet sheet, IEnumerable<T> collection)
			: base(collection)
		{
			_WorkSheet = sheet;
		}

		protected override int GetKeyForItem(T item)
		{
			return item.Index;
		}

		public void Insert(int index, int count)
		{
			if (count <= 0) return;

			if (!this.InnerDict.ContainsKey(index))
			{
				throw new ArgumentOutOfRangeException("未能找到要插入的index位置");
			}

			Dictionary<int, T> dict = new Dictionary<int, T>();
			List<T> indexChangedRows = new List<T>();

			foreach (var item in this.InnerDict)
			{
				if (item.Key > index)
				{
					item.Value.Index += count;
					indexChangedRows.Add(item.Value);
				}
				else
				{
					dict.Add(item.Key, item.Value);
				}
			}

			foreach (var item in indexChangedRows)
			{
				dict.Add(item.Index, item);
			}
			
			for (int i = 1; i <= count; i++)
			{
				int newIndex = index + i;
				dict.Add(newIndex, new T() { Index = newIndex });
			}

			this.InnerDict = dict;
			this._WorkSheet.Cells.SyncIndex<T>(index);
		}

		public void Sort()
		{
			var orderResult = this.InnerDict.OrderBy(p => p.Key);
			Dictionary<int, T> orderedDict = new Dictionary<int, T>(this.InnerDict.Count);

			foreach (var item in orderResult)
			{
				orderedDict.Add(item.Key, item.Value);
			}

			this.InnerDict = orderedDict;
		}
	}
}
