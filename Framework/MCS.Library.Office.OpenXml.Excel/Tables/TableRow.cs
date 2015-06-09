using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
    public sealed class TableRow : IEnumerable<TableCell>
    {
        private Dictionary<TableColumn, TableCell> Cells = new Dictionary<TableColumn, TableCell>();

        internal TableRow(TableColumnCollection Columns)
        {
            Init(Columns);
        }

        private TableRow(int index)
        {
            this.RowIndex = index;
        }

        internal TableRow(TableColumnCollection Columns, int index)
            : this(Columns)
        {
            this.RowIndex = index;
        }


        private void Init(TableColumnCollection Columns)
        {
            foreach (TableColumn tc in Columns)
            {
                Cells.Add(tc, new TableCell());
            }
        }

        public int RowIndex
        {
            get;
            internal set;
        }

        public int Count
        {
            get
            {
                return this.Cells.Count;
            }
        }

        internal void Remove(TableColumn tc)
        {
            this.Cells.Remove(tc);
        }

        public TableCell this[TableColumn col]
        {
            get
            {
                return this.Cells[col];
            }
        }

        public IEnumerator<TableCell> GetEnumerator()
        {
            return this.Cells.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Cells.Values.GetEnumerator();
        }

        internal TableRow Clone(Table table)
        {
            TableRow trClone = new TableRow(this.RowIndex);

            foreach (KeyValuePair<TableColumn, TableCell> item in this.Cells)
            {
                trClone.Cells.Add(table.Columns[item.Key.Name], item.Value.Clone());
            }

            return trClone;
        }

    }
}
