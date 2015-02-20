using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class RichTextCollection : IEnumerable<RichText>
	{
		private List<RichText> _list = new List<RichText>();
		private Cell _Cell = null;

		public RichTextCollection(Cell cell)
		{
			this._Cell = cell;
		}

		public RichText this[int Index]
		{
			get
			{
				return this._list[Index];
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public RichText Add(string Text)
		{
			RichText rt = new RichText();
			if (this._list.Count > 0)
			{
				RichText prevItem = this._list[this._list.Count - 1];
				rt.FontName = prevItem.FontName;
				rt.Size = prevItem.Size;
				if (prevItem.DataBarColor == null)
				{
					rt.DataBarColor.SetColor(Color.Black);
				}
				else
				{
					rt._DataBarColor = prevItem._DataBarColor;
				}
				rt.PreserveSpace = rt.PreserveSpace;
				rt.Bold = prevItem.Bold;
				rt.Italic = prevItem.Italic;
				rt.UnderLine = prevItem.UnderLine;
			}
			//else if (this._Cell._Style == null)
			//{
			//    rt.FontName = "宋体";
			//    rt.Size = 11;
			//}
			else
			{
				rt.FontName = "宋体";
				rt.Size = 11;
			}
			rt.Text = Text;
			rt.PreserveSpace = true;
			this._Cell.IsRichText = true;

			this._list.Add(rt);

			return rt;
		}

		internal void Add(RichText rt)
		{
			this._list.Add(rt);
		}

		public void Clear()
		{
			this._list.Clear();
		}

		public void RemoveAt(int Index)
		{
			this._list.RemoveAt(Index);
		}

		public void Remove(RichText Item)
		{
			this._list.Remove(Item);
		}

		public string Text
		{
			get
			{
				StringBuilder sbText = new StringBuilder();
				foreach (RichText item in this._list)
				{
					sbText.Append(item.Text);
				}
				return sbText.ToString();
			}
			set
			{
				if (Count == 0)
				{
					Add(value);
				}
				else
				{
					this[0].Text = value;
					for (int ix = 1; ix < Count; ix++)
					{
						RemoveAt(ix);
					}
				}
			}
		}

		public IEnumerator<RichText> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._list.GetEnumerator();
		}
	}
}
