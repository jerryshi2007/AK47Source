using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Builders;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.Office.OpenXml.Excel;
using System.IO;
using MCS.Library.Data.DataObjects;
namespace MCS.Library.WF.Contracts.Converters.Descriptors.Tests
{
    [TestClass()]
    public class WfCreateClientDynamicProcessParamsConverterTests
    {
        [TestMethod()]
        public void ExcelStreamToClientTest()
        {
            WfCreateClientDynamicProcessParams createParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();  
            WfCreateClientDynamicProcessParams outProcessParams = null;

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(createParams);
            WfClientProcessDescriptor client = builder.Build(createParams.Key, createParams.Name);
 
            string processKey = createParams.Key;

            using (Stream stream = WfClientProcessDescriptorConverter.Instance.ClientDynamicProcessToExcelStream(client))
            {
                WfCreateClientDynamicProcessParamsConverter.Instance.ExcelStreamToClient(processKey, stream, ref outProcessParams);
            }
            createParams.ActivityMatrix.PropertyDefinitions.ForEach(action => 
              {
                  action.DataType = ColumnDataType.String;              
              }); //EXCEL 无法存储类型信息，所有默认都为string类型
            createParams.AreSame(outProcessParams);
        }
    }
}
