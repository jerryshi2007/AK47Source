using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Common;
using MCS.Library.WF.Contracts.PropertyDefine;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientProcessDescriptorConverter : WfClientKeyedDescriptorConverterBase<WfClientProcessDescriptor, WfProcessDescriptor>
    {
        public static readonly WfClientProcessDescriptorConverter Instance = new WfClientProcessDescriptorConverter();

        private WfClientProcessDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientProcessDescriptor client, ref WfProcessDescriptor server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfProcessDescriptor(client.Key);

            base.ClientToServer(client, ref server);

            WfClientRelativeLinkDescriptorCollectionConverter.Instance.ClientToServer(client.RelativeLinks, server.RelativeLinks);
            WfClientVariableDescriptorCollectionConverter.Instance.ClientToServer(client.Variables, server.Variables);
            WfClientResourceDescriptorCollectionConverter.Instance.ClientToServer(client.CancelEventReceivers, server.CancelEventReceivers);

            foreach (WfClientActivityDescriptor cad in client.Activities)
            {
                WfActivityDescriptor actDesp = null;

                WfClientActivityDescriptorConverter.Instance.ClientToServer(cad, ref actDesp);

                server.Activities.Add(actDesp);
            }

            TransitionsClientToServer(client, server);
        }

        public override void ServerToClient(WfProcessDescriptor server, ref WfClientProcessDescriptor client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientProcessDescriptor();

            base.ServerToClient(server, ref client);

            WfClientRelativeLinkDescriptorCollectionConverter.Instance.ServerToClient(server.RelativeLinks, client.RelativeLinks);
            WfClientVariableDescriptorCollectionConverter.Instance.ServerToClient(server.Variables, client.Variables);
            WfClientResourceDescriptorCollectionConverter.Instance.ServerToClient(server.CancelEventReceivers, client.CancelEventReceivers);

            foreach (WfActivityDescriptor actDesp in server.Activities)
            {
                WfClientActivityDescriptor cad = null;

                WfClientActivityDescriptorConverter.Instance.ServerToClient(actDesp, ref cad);

                client.Activities.Add(cad);
            }

            TransitionsServerToClient(server, client);
        }

        /// <summary>
        /// 将线转换到服务端对象
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        private static void TransitionsClientToServer(WfClientProcessDescriptor client, WfProcessDescriptor server)
        {
            foreach (WfClientActivityDescriptor cad in client.Activities)
            {
                foreach (WfClientTransitionDescriptor ct in cad.ToTransitions)
                {
                    IWfActivityDescriptor fromActDesp = server.Activities[ct.FromActivityKey];
                    IWfActivityDescriptor toActDesp = server.Activities[ct.ToActivityKey];

                    WfTransitionDescriptor st = null;

                    WfClientTransitionDescriptorConverter.Instance.ClientToServer(ct, ref st);

                    if (fromActDesp != null && toActDesp != null)
                        st.ConnectActivities(fromActDesp, toActDesp);
                }
            }
        }

        private static void TransitionsServerToClient(WfProcessDescriptor server, WfClientProcessDescriptor client)
        {
            foreach (IWfActivityDescriptor sa in server.Activities)
            {
                foreach (WfTransitionDescriptor st in sa.ToTransitions)
                {
                    WfClientTransitionDescriptor ct = null;

                    WfClientTransitionDescriptorConverter.Instance.ServerToClient(st, ref ct);

                    WfClientActivityDescriptor fromActDesp = client.Activities[sa.Key];

                    if (fromActDesp != null)
                        fromActDesp.ToTransitions.Add(ct);
                }
            }
        }


        public WorkBook ClientDynamicProcessToExcel(WfClientProcessDescriptor client)
        {
            bool isDynamic = client.Variables.GetValue("ClientDynamicProcess", false);

            ExceptionHelper.TrueThrow<ArgumentException>(isDynamic == false,
                Translator.Translate(Define.DefaultCulture, "非动态活动流程不能导出{0}", client.Key));

            WorkBook wb = WorkBook.CreateNew();
            wb.Sheets.Clear();

            WorkSheet workSheet = null;
            Row titleRow = null;

            #region 流程定义信息
            workSheet = new WorkSheet(wb, "Process");
            titleRow = new Row(1) { Height = 30d };
            titleRow.Style.Fill.SetBackgroundColor(Color.LightGray, ExcelFillStyle.Solid);
            titleRow.Style.Font.Size = 20;
            workSheet.Rows.Add(titleRow);
            workSheet.Cells[titleRow.Index, 1].Value = "流程属性";

            CreateCommonHeaderRow(client, workSheet);

            FillCommonSheetData(client, workSheet);
            wb.Sheets.Add(workSheet);
            #endregion

            #region 矩阵信息
            WfClientActivityDescriptor activity = client.Activities.Find(w => w.Resources.Count > 0 && w.Resources[0] is WfClientActivityMatrixResourceDescriptor);
            if (activity == null)
            {
                return wb;
            }

            WfClientActivityMatrixResourceDescriptor matrix = activity.Resources[0] as WfClientActivityMatrixResourceDescriptor;

            workSheet = new WorkSheet(wb, "Matrix");

            titleRow = new Row(1) { Height = 30d };
            titleRow.Style.Fill.SetBackgroundColor(Color.LightGray, ExcelFillStyle.Solid);
            titleRow.Style.Font.Size = 20;
            workSheet.Rows.Add(titleRow);
            workSheet.Cells[titleRow.Index, 1].Value = "角色属性";

            CreateMatrixHeaderRow(matrix, workSheet);

            FillMatrixSheetData(matrix, workSheet);
            wb.Sheets.Add(workSheet);
            #endregion

            return wb;
        }

        public Stream ClientDynamicProcessToExcelStream(WfClientProcessDescriptor client)
        {
            WorkBook workBook = ClientDynamicProcessToExcel(client);

            MemoryStream stream = new MemoryStream();
            workBook.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        #region  EXCEL FillData
        private static void CreateCommonHeaderRow(WfClientProcessDescriptor client, WorkSheet ws)
        {
            Row headRow = new Row(3);

            headRow.Style.Fill.SetBackgroundColor(Color.Gold, ExcelFillStyle.Solid);
            headRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Top.Color.SetColor(Color.Black);
            headRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Bottom.Color.SetColor(Color.Black);
            headRow.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Left.Color.SetColor(Color.Black);
            headRow.Style.Font.Bold = true;
            ws.Rows.Add(headRow);

            int columnIndex = 1;
            ws.Cells[headRow.Index, columnIndex].Value = "属性名";
            ws.Names.Add(CellAddress.Parse(columnIndex, headRow.Index).ToString(), "PropertyKey");

            columnIndex++;
            ws.Cells[headRow.Index, columnIndex].Value = "属性值";
            ws.Names.Add(CellAddress.Parse(columnIndex, headRow.Index).ToString(), "PropertyValue");

        }

        private static void FillCommonSheetData(WfClientProcessDescriptor client, WorkSheet ws)
        {

            int rowIndex = 4;
            foreach (ClientPropertyValue row in client.Properties)
            {
                string propertyValue = row.Key;
                string dataValue = row.StringValue;
                if (dataValue.IsNotEmpty())
                {
                    ws.Cells[rowIndex, 1].Value = propertyValue;
                    ws.Cells[rowIndex, 2].Value = dataValue;
                    rowIndex++;
                }
            }
        }

        private static void CreateMatrixHeaderRow(WfClientActivityMatrixResourceDescriptor matrix, WorkSheet ws)
        {
            Row headRow = new Row(3);

            headRow.Style.Fill.SetBackgroundColor(Color.Gold, ExcelFillStyle.Solid);
            headRow.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Top.Color.SetColor(Color.Black);
            headRow.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Bottom.Color.SetColor(Color.Black);
            headRow.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headRow.Style.Border.Left.Color.SetColor(Color.Black);
            headRow.Style.Font.Bold = true;
            ws.Rows.Add(headRow);

            int columnIndex = 1;

            foreach (WfClientRolePropertyDefinition dimension in matrix.PropertyDefinitions)
            {
                ws.Cells[headRow.Index, columnIndex].Value = dimension.Description.IsNotEmpty() ? dimension.Description : dimension.Name;
                ws.Names.Add(CellAddress.Parse(columnIndex, headRow.Index).ToString(), dimension.Name);

                columnIndex++;
            }
        }

        private static void FillMatrixSheetData(WfClientActivityMatrixResourceDescriptor matrix, WorkSheet ws)
        {
            int rowIndex = 4;
            WfClientRolePropertyRowCollection rows = matrix.Rows;

            foreach (WfClientRolePropertyRow row in rows)
            {

                foreach (DefinedName name in ws.Names)
                {
                    var propertyValue = row.Values.FindByColumnName(name.Name);

                    object dataValue = null;

                    if (propertyValue != null)
                    {
                        if (propertyValue.Column.DataType != Data.DataObjects.ColumnDataType.String)
                        {
                            dataValue = DataConverter.ChangeType(typeof(string)
                                , propertyValue.Value
                                , propertyValue.Column.RealDataType);
                        }
                        else
                        {
                            dataValue = propertyValue.Value;
                        }
                    }
                    else
                    {
                        switch (name.Name.ToLower())
                        {
                            case "operatortype":
                                dataValue = row.OperatorType.ToString();
                                break;
                            case "operator":
                                dataValue = row.Operator;
                                break;
                        }
                    }

                    if (dataValue != null)
                        ws.Cells[rowIndex, name.Address.StartColumn].Value = dataValue;
                }

                rowIndex++;
            }
        }
        #endregion
    }
}
