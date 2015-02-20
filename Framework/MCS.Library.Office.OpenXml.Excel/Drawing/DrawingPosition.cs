using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class DrawingPosition
	{
		private int _Colum = 0;
		public int Column
		{
			get { return this._Colum; }
			set { this._Colum = value; }
		}

		private int _Row = 0;
		public int Row
		{
			get { return this._Row; }
			set { this._Row = value; }
		}

		private int _ColumnOff = 0;
		public int ColumnOff
		{
			get { return this._ColumnOff; }
			set { this._ColumnOff = value; }
		}

		private int _RowOff = 0;
		public int RowOff
		{
			get { return this._RowOff; }
			set { this._RowOff = value; }
		}
	}
}
