using MCS.Library.Data.DataObjects;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkflowDesigner.ModalDialog
{
    public partial class DownloadActivityMatrixExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WfConverterHelper.RegisterConverters();

            WfActivityMatrixResourceDescriptor activityMatrix = null;
            string fileName = "ActivityMatrix";

            if (Request.Form["matrixData"] != null)
            {
                activityMatrix = JSONSerializerExecute.Deserialize<WfActivityMatrixResourceDescriptor>(Request.Form["matrixData"]);

                fileName = "SampleMatrix";
            }
            else
            {
                activityMatrix = PrepareSampleActivityMatrixResourceDescriptor();
            }

            ResponseExcelWorkBook(activityMatrix, fileName);
        }

        private void ResponseExcelWorkBook(WfActivityMatrixResourceDescriptor activityMatrix, string fileName)
        {
            WorkBook workBook = WorkBook.CreateNew();

            workBook.Sheets.Clear();

            workBook.FillActivityMatrixResourceDescriptor(activityMatrix);

            Response.AppendExcelOpenXmlHeader(fileName);

            workBook.Save(Response.OutputStream);

            Response.End();
        }

        private static WfActivityMatrixResourceDescriptor PrepareSampleActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PrepareSamplePropertiesDefinition());
            resource.Rows.CopyFrom(PrepareSampleRows(resource.PropertyDefinitions));

            return resource;
        }

        private static SOARolePropertyDefinitionCollection PrepareSamplePropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0, Description = "成本中心" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "ActivitySN", SortOrder = 1, Description = "活动序号" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "ActivityProperties", SortOrder = 2, Description = "活动属性" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "OperatorType", SortOrder = 3, DataType = ColumnDataType.String, Description = "操作人类型" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Operator", SortOrder = 4, DataType = ColumnDataType.String, Description = "操作人" });

            return propertiesDefinition;
        }

        private static SOARolePropertyRowCollection PrepareSampleRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "10" });
            row1.Values.Add(new SOARolePropertyValue(pds["ActivityProperties"]) { Value = "{Name:\"部门领导\"}" });

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "liming" };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row2.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "20" });
            row2.Values.Add(new SOARolePropertyValue(pds["ActivityProperties"]) { Value = "{Name:\"公司领导\"}" });

            SOARolePropertyRow row3 = new SOARolePropertyRow() { RowNumber = 3, OperatorType = SOARoleOperatorType.User, Operator = "quym" };

            row3.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row3.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "10" });
            row3.Values.Add(new SOARolePropertyValue(pds["ActivityProperties"]) { Value = "{Name:\"部门领导\"}" });

            SOARolePropertyRow row4 = new SOARolePropertyRow() { RowNumber = 4, OperatorType = SOARoleOperatorType.User, Operator = "liming" };

            row4.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row4.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "20" });
            row4.Values.Add(new SOARolePropertyValue(pds["ActivityProperties"]) { Value = "{Name:\"公司领导\"}" });

            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);

            return rows;
        }
    }
}