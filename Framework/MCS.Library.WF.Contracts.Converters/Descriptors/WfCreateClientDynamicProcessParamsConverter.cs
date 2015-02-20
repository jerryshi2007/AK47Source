using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfCreateClientDynamicProcessParamsConverter
    {
        public static readonly WfCreateClientDynamicProcessParamsConverter Instance = new WfCreateClientDynamicProcessParamsConverter();

        private WfCreateClientDynamicProcessParamsConverter()
        {
        }

        /// <summary>
        /// EXCEL转换为动态活动流程的参数
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public WfCreateClientDynamicProcessParams ExcelToClient(string processKey, WorkBook wb, ref WfCreateClientDynamicProcessParams client)
        {
            processKey.NullCheck("processKey");
            wb.NullCheck("wb");

            DataTable processTable = DocumentHelper.GetRangeValuesAsTable(wb, "Process", "A3");
            DataTable matrixTable = DocumentHelper.GetRangeValuesAsTable(wb, "Matrix", "A3");

            if (client == null)
                client = new WfCreateClientDynamicProcessParams();

            client.Key = processKey;

            foreach (DataRow row in processTable.Rows)
            {
                string propertyValue = row[0].ToString();

                if (propertyValue.IsNullOrEmpty() || string.Equals(propertyValue, "Key", StringComparison.OrdinalIgnoreCase))
                    continue;

                string dataValue = row[1].ToString();   //dataValue 可以为空

                client.Properties.AddOrSetValue(propertyValue, dataValue);
            }

            client.ActivityMatrix = new WfClientActivityMatrixResourceDescriptor(matrixTable);

            return client;
        }

        public WfCreateClientDynamicProcessParams ExcelStreamToClient(string processKey, Stream stream, ref WfCreateClientDynamicProcessParams client)
        {
            stream.NullCheck("stream");

            WorkBook workBook = WorkBook.Load(stream);

            return ExcelToClient(processKey, workBook, ref client);
        }
    }
}
