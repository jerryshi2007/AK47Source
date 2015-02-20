using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Office.OpenXml.Word
{
    public class DCTDataRow
    {
        public string this[DCTDataColumn column]
        {
            get
            {
                string result="";
                if (dataCells.TryGetValue(column.Name, out result))
                    return result;
                return "";
            }
            set
            {
                dataCells[column.Name] = value;
            }
        }

        public DCTDataRow()
        {
            dataCells = new Dictionary<string, string>();
            isEmpty = true;
        }

        bool isEmpty;

        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; }
        }

        public List<DCTSimpleProperty> ToSimpleProperties()
        {
            List<DCTSimpleProperty> result = new List<DCTSimpleProperty>();
            foreach (string key in dataCells.Keys)
            {
                result.Add(new DCTSimpleProperty() { TagID = key, Value = dataCells[key] });
            }
            return result;
        }

        Dictionary<string, string> dataCells;

        public static DCTDataRow FromTableRow(TableRow row, DCTDataColumnCollection colCollection)
        {
            List<TableCell> cells = row.Descendants<TableCell>().ToList();
            DCTDataRow result = new DCTDataRow();
            result.isEmpty = true;
            for(int i=0;i<cells.Count;i++)
            {
                DCTDataColumn column = null;
                if (colCollection.TryGetValue(i, out column))
                {
                    if (!string.IsNullOrEmpty(cells[i].InnerText))
                        result.isEmpty = false;
                    result[column] = cells[i].InnerText;
                }
            }
            return result;

        }
    }
}
