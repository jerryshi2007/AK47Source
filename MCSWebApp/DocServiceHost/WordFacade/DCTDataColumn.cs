using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordProcessing
{
    public class DCTDataColumn
    {
        public DCTDataColumn(string name)
        {
            this.name = name;
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public class DCTDataColumnCollection : Dictionary<int,DCTDataColumn>
    {
        public DCTDataColumnCollection(TableRow titleRow)
        {
            coveredControlIds = new List<int>();

            var cells = titleRow.Descendants<TableCell>().ToList();

            //针对每一个区域，先读取表头的控件,同时将控件加入黑名单
            for (int i = 0; i < cells.Count; i++)
            {
                var sdtElement = cells[i].Descendants<SdtElement>().Where(o => o.SdtProperties.Descendants<SdtAlias>().Any(a => a.Val != null)).FirstOrDefault();
                if (null == sdtElement)
                    continue;
                SdtAlias titleAlias = sdtElement.Descendants<SdtAlias>().FirstOrDefault();
                SdtId titleId = sdtElement.Descendants<SdtId>().FirstOrDefault();
                if (null == titleAlias)
                    continue;
                this[i] = new DCTDataColumn(titleAlias.Val);
                //properties.Add(alias);
                CoveredControlIds.Add(titleId.Val.Value);

            }
        }

        private List<int> coveredControlIds;

        public List<int> CoveredControlIds
        {
            get { return coveredControlIds; }
            set { coveredControlIds = value; }
        }

    }
}
