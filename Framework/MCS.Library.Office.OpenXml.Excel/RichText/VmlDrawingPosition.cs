using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class VmlDrawingPosition
	{
		private Comment comment;
		private int p;

		public VmlDrawingPosition(Comment comment, int startPos)
		{
			this.comment = comment;
			this.p = startPos;
		}

		/// <summary>
		/// 行最小0;
		/// </summary>
		public int Row
		{
			get
			{
				return GetNumber(2);
			}
			set
			{
				SetNumber(2, value);
			} 
		}

		public int RowOffset
		{
			get
			{
				return GetNumber(3);
			}
			set
			{
				SetNumber(3, value);
			}
		}

		public int Column
		{
			get
			{
				return GetNumber(0);
			}
			set
			{
				SetNumber(0, value);
			}
		}

		public int ColumnOffset
		{
			get
			{
				return GetNumber(1);
			}
			set
			{
				SetNumber(1, value);
			}
		}

		private void SetNumber(int pos, int value)
		{
			string anchor = this.comment.Anchor;
			if (string.IsNullOrEmpty(anchor))
			{
				anchor = string.Format("{0}, 15, {1}, 2, {2}, 31, {3}, 1", this.comment._Cell.Column.Index, this.comment._Cell.Row.Index, this.comment._Cell.Column.Index + 2, this.comment._Cell.Row.Index + 3);
			}
			string[] numbers = anchor.Split(',');
			ExceptionHelper.FalseThrow(numbers.Length == 8, "不符合规格");

			numbers[this.p + pos] = value.ToString();

			this.comment.Anchor = string.Join(",", numbers);
		}

		private int GetNumber(int pos)
		{
			string anchor = this.comment.Anchor;
			if (string.IsNullOrEmpty(anchor))
			{
				anchor = string.Format("{0}, 15, {1}, 2, {2}, 31, {3}, 1", this.comment._Cell.Column.Index, this.comment._Cell.Row.Index, this.comment._Cell.Column.Index + 2, this.comment._Cell.Row.Index + 3);
			}
			string[] numbers = anchor.Split(',');
			ExceptionHelper.FalseThrow(numbers.Length == 8, "不符合规格");
			int ret;
			if (int.TryParse(numbers[this.p + pos], out ret))
			{
				return ret;
			}

			return ret;
		}

	}
}
