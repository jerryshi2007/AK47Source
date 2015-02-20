using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WfFormTemplate.DataObjects
{
    public class PropertyFormDemoSource
    {
        public DataTable GetPropertiesTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ColName") { DataType = typeof(string) });
            dt.Columns.Add(new DataColumn("ColVale") { DataType = typeof(Int32) });

            for (int i = 0; i < 50; i++)
            {
                DataRow dr = dt.NewRow();
                dr["ColVale"] = i;
                dr["ColName"] = string.Format("ColName{0}", i);
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public IList<DropdownLitsItem> GetPropertiesList()
        {
            List<DropdownLitsItem> result = new List<DropdownLitsItem>();
            
            result.Add(new DropdownLitsItem() { ID = "1", Name = "男", Caption = string.Format("Caption1") });
            result.Add(new DropdownLitsItem() { ID = "0", Name = "女", Caption = string.Format("Caption2") });

            return result;
        }
    }

    public class DropdownLitsItem
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Caption { get; set; }
    }
}