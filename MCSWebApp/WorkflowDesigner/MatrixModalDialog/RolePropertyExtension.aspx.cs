using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;


namespace MCS.Applications.AppAdmin.Dialogs
{
    public partial class RolePropertyExtension : System.Web.UI.Page
    {
        protected string AppID
        {
            get
            {
                return WebUtility.GetRequestParamString("AppID", string.Empty);
            }
        }

        protected string RoleID
        {
            get
            {
                return WebUtility.GetRequestParamString("RoleID", string.Empty);
            }
        }

        protected string DefinitionID
        {
            get
            {
                return WebUtility.GetRequestParamString("DefinitionID", this.RoleID);
            }
        }

        /// <summary>
        /// 是否可以编辑属性定义
        /// </summary>
        protected bool CanEditDefinition
        {
            get
            {
                return string.Compare(this.RoleID, this.DefinitionID, true) == 0;
            }
        }

        protected string AppCodeName
        {
            get
            {
                return WebUtility.GetRequestParamString("AppCodeName", string.Empty);
            }
        }

        protected string RoleCodeName
        {
            get
            {
                return WebUtility.GetRequestParamString("RoleCodeName", string.Empty);
            }
        }

        protected string EditMode
        {
            get
            {
                return WebUtility.GetRequestParamString("editMode", string.Empty);
            }
        }

        protected bool NeedValidateSource
        {
            get
            {
                return WebUtility.GetRequestFormString("validateSource", string.Empty) == "validateSource";
            }
        }

        private SOARolePropertyDefinitionCollection definition = null;

        public SOARolePropertyDefinitionCollection Definition
        {
            get
            {
                if (this.definition == null)
                    this.definition = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(this.DefinitionID);

                return this.definition;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //this.ticketAnchor.HRef = "EditRoleProperty.aspx?AppID=" + this.AppID + "&RoleID=" + this.RoleID + "&definitionID=" + this.DefinitionID;

            this.lnkCheckMatrixFile.HRef = "RolePropertyExtension.aspx?editMode=download&AppID=" + this.AppID + "&RoleID=" + this.RoleID + "&definitionID=" + this.DefinitionID; ;

            if (!IsPostBack)
            {
                this.materialCtrlForMatrix.RequestContext = string.Join(";", this.AppID, this.RoleID, this.DefinitionID);
            }
        }


        protected void btnSaveMatrix_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["editMode"] == "readOnly")
                throw new InvalidOperationException("只读模式不可以操作");

            int count = this.materialCtrlForMatrix.Materials.Count;

            if (count > 0 && !this.materialCtrlForMatrix.DeltaMaterials.IsEmpty())
            {
                Material roleTerial = null;

                if (this.materialCtrlForMatrix.DeltaMaterials.Inserted.Count > 0)
                    roleTerial = this.materialCtrlForMatrix.DeltaMaterials.Inserted[this.materialCtrlForMatrix.DeltaMaterials.Inserted.Count - 1];
                else if (this.materialCtrlForMatrix.DeltaMaterials.Updated.Count > 0)
                    roleTerial = this.materialCtrlForMatrix.DeltaMaterials.Updated[this.materialCtrlForMatrix.DeltaMaterials.Updated.Count - 1];

                if (roleTerial != null)
                {
                    using (Stream LoadFile = roleTerial.GetTemporaryContent(this.materialCtrlForMatrix.RootPathName))
                    {
                        Save(LoadFile);
                    }
                }

                this.materialCtrlForMatrix.Materials.Clear();
            }
        }

        protected void btnDeleteMatrix_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["editMode"] == "readOnly")
                throw new InvalidOperationException("只读模式不可以操作");

            this.materialCtrlForMatrix.Materials.Clear();
            DeleteMatrix();
        }

        private void Save(Stream strem)
        {
            DataTable dt = DocumentHelper.GetRangeValuesAsTable(strem, "Matrix", "A3");
            string[] itemsInfo = this.materialCtrlForMatrix.RequestContext.Split(';');

            string roleID = itemsInfo[1];
            string defID = itemsInfo[2];

            SOARole role = PrepareRole(roleID, defID);

            role.Rows.Clear();
            int rowIndex = 0;

            foreach (DataRow row in dt.Rows)
            {
                SOARolePropertyRow mRow = new SOARolePropertyRow(role) { RowNumber = rowIndex };

                foreach (var dimension in role.PropertyDefinitions)
                {
                    SOARolePropertyValue mCell = new SOARolePropertyValue(dimension);
                    mCell.Value = row[dimension.Name].ToString();

                    switch (dimension.Name)
                    {
                        case "Operator":
                            mRow.Operator = row[dimension.Name].ToString();
                            break;
                        case "OperatorType":
                            SOARoleOperatorType opType = SOARoleOperatorType.User;
                            Enum.TryParse(row[dimension.Name].ToString(), out opType);
                            mRow.OperatorType = opType;
                            break;
                        default:
                            break;
                    }

                    mRow.Values.Add(mCell);
                }

                rowIndex++;
                role.Rows.Add(mRow);
            }

            //更新数据库
            SOARolePropertiesAdapter.Instance.Update(role);
        }

        private void DeleteMatrix()
        {
            string[] itemsInfo = this.materialCtrlForMatrix.RequestContext.Split(';');

            string roleID = itemsInfo[1];
            string defID = itemsInfo[2];

            SOARole role = PrepareRole(roleID, defID);

            role.Rows.Clear();

            //更新数据库
            SOARolePropertiesAdapter.Instance.Update(role);
        }

        protected void materialCtrl_PrepareDownloadStream(object sender, PrepareDownloadStreamEventArgs args)
        {
            string[] itemsInfo = args.DownloadInfo.RequestContext.Split(';');

            string roleID = itemsInfo[1];
            string defID = itemsInfo[2];

            SOARole role = PrepareRole(roleID, defID);
            WorkBook workBook = role.ToExcelWorkBook();

            using (MemoryStream stream = new MemoryStream())
            {
                workBook.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(args.OutputStream);
            }
        }


        private static SOARole PrepareRole(string roleID, string definitionID)
        {
            SOARolePropertyDefinitionCollection definition = SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(definitionID);

            return new SOARole(definition) { ID = roleID };
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            bool editable = this.EditMode.ToLower() != "readonly";

            this.deleteMatrixBtn.Visible = editable;
            this.importBtn.Visible = editable;
            //this.edtBlock.Visible = editable;
            this.lnkDeleteDenifition.Visible = this.lnkImportDefinition.Visible = this.lnkImportDefinition2.Visible = this.lnkPropertyDefinition.Visible = editable;

            this.lnkPropertyDefinition.Visible = this.lnkPropertyDefinition.Visible && this.CanEditDefinition;
            this.lnkDeleteDenifition.Visible = this.lnkDeleteDenifition.Visible && this.CanEditDefinition;
            this.lnkImportDefinition.Visible = this.lnkImportDefinition.Visible && this.CanEditDefinition;
            this.lnkImportDefinition2.Visible = this.lnkImportDefinition2.Visible && this.CanEditDefinition;

            this.lnkChangeDefinition.InnerText = editable && this.CanEditDefinition ? "修改..." : "查看...";

            RegisterDefaultDialogParams();

            RenderByMatrix();

            if (!editable)
            {
                materialCtrlContainer.Style["display"] = "none";
                martrixInfoReadOnlyContainer.Style["display"] = "block";
            }
        }

        public override void ProcessRequest(HttpContext context)
        {
            switch (context.Request.QueryString["editMode"])
            {
                case "download":
                    {
                        SOARole role = PrepareRole(this.RoleID, this.DefinitionID);

                        WorkBook workBook = role.ToExcelWorkBook();

                        context.Response.CacheControl = "no-cache";

                        context.Response.AppendExcelOpenXmlHeader("matrix");
                        workBook.Save(context.Response.OutputStream);

                        context.Response.Flush();
                    }
                    break;
                default:
                    base.ProcessRequest(context);
                    break;
            }
        }

        /// <summary>
        /// 注册默认的对话框参数。当没有参数传递进来时，使用此参数
        /// </summary>
        private void RegisterDefaultDialogParams()
        {
            var data = new
            {
                appID = this.AppID,
                App_CodeName = this.AppCodeName,
                App_Name = this.AppCodeName,
                id = this.RoleID,
                Role_Name = this.RoleCodeName,
                Role_CodeName = this.RoleCodeName,
                editMode = this.EditMode
            };

            this.Page.ClientScript.RegisterHiddenField("defaultDialogParams", JSONSerializerExecute.Serialize(data));

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "initParams",
                "if (!m_objParam) m_objParam = Sys.Serialization.JavaScriptSerializer.deserialize($get('defaultDialogParams').value);",
                true);
        }

        private void RenderByMatrix()
        {
            if (this.RoleID.IsNotEmpty() && this.Definition != null && this.Definition.Count > 0)
            {
                noMartrixInfoContainer.Style[HtmlTextWriterStyle.Display] = "none";
                martrixInfoContainer.Style[HtmlTextWriterStyle.Display] = "block";

                exportBtn.Disabled = false;
                btnDeleteMatrix.Disabled = false;
                importBtn.Disabled = false;
                lnkCheckMatrixFile.Visible = true;

                materialCtrlContainer.Style["display"] = "block";
                matrixID.Value = RoleID;

                matrixInfo.InnerText = string.Format("当前角色矩阵维度为 [{0}] 点击这里 ", definition.Count);
            }
            else
            {
                //这里设置没有定义维度的情况
                noMartrixInfoContainer.Style[HtmlTextWriterStyle.Display] = "block";
                martrixInfoContainer.Style[HtmlTextWriterStyle.Display] = "none";

                exportBtn.Disabled = true;
                btnDeleteMatrix.Disabled = true;
                importBtn.Disabled = true;
                lnkCheckMatrixFile.Visible = false;

                materialCtrlContainer.Style["display"] = "none";
                matrixID.Value = string.Empty;
            }
        }

        protected void bindMatrixBtn_Click(object sender, EventArgs e)
        {
            RenderByMatrix();
        }

        protected void deleteMatrixBtn_Click(object sender, EventArgs e)
        {
            CheckEditMode();
            SOARolePropertyDefinitionAdapter.Instance.Delete(new SOARole { ID = this.DefinitionID });
        }

        private void CheckEditMode()
        {
            if (this.EditMode.ToLower() == "readonly")
                throw new InvalidOperationException("只读状态时不可以修改数据");
        }

        /// <summary>
        /// 上传矩阵的内容（行）
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
        {
            CheckEditMode();

            var fileType = Path.GetExtension(file.FileName).ToLower();

            if (fileType != ".xml" && fileType != ".xlsx")
                throw new SystemSupportException("'{0}' 必须是 xml 或 xlsx 文件。");

            UploadProgressStatus status = new UploadProgressStatus();
            status.CurrentStep = 1;
            status.MinStep = 0;
            status.MaxStep = 20;

            Action notifier = () =>
            {
                if (status.CurrentStep + 1 < status.MaxStep)
                {
                    status.CurrentStep++;
                }
                status.Response();
            };

            if (fileType == ".xml")
            {
                ImportFromXml(file.InputStream);
            }
            else if (fileType == ".xlsx")
            {
                ImportFromExcel2007(file.InputStream, notifier);
            }

            result.DataChanged = true;
            result.CloseWindow = true;
        }

        /// <summary>
        /// 上传属性定义
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        protected void ImportSOARole_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
        {
            var fileType = Path.GetExtension(file.FileName).ToLower();

            if (string.Compare(fileType, ".xml", true) == 0)
            {
                StringBuilder logger = new StringBuilder();
                try
                {
                    CheckEditMode();

                    var xmlDoc = XDocument.Load(file.InputStream);
                    var wfProcesses = xmlDoc.Descendants("Root");
                    XElementFormatter formatter = new XElementFormatter();
                    UploadProgressStatus status = new UploadProgressStatus();

                    status.CurrentStep = 1;
                    status.MinStep = 0;
                    status.MaxStep = wfProcesses.Count() + 1;
                    logger.AppendFormat("开始导入角色属性定义...\n", file.FileName, status.MaxStep);

                    SOARole role = new SOARole { ID = this.DefinitionID };

                    SOARolePropertyDefinitionAdapter.Instance.Delete(role);

                    foreach (var wfProcess in wfProcesses)
                    {
                        SOARolePropertyDefinitionCollection rowsColl = (SOARolePropertyDefinitionCollection)formatter.Deserialize(wfProcess);

                        SOARolePropertyDefinitionAdapter.Instance.Update(role, rowsColl);

                        logger.Append("保存成功...\n");

                        status.CurrentStep++;
                        status.Response();
                    }
                    logger.AppendFormat("导入完成!", file.FileName);
                }
                catch (Exception ex)
                {
                    logger.AppendFormat("导入错误，{0}，错误堆栈：{1}", ex.Message, ex.StackTrace);
                }

                result.DataChanged = true;
                result.CloseWindow = false;
                result.ProcessLog = logger.ToString();
            }
        }

        /// <summary>
        /// 导入Excel Xml格式的文件
        /// </summary>
        /// <param name="stream"></param>
        private void ImportFromXml(Stream stream)
        {
            WorkbookNode workbook = new WorkbookNode();

            workbook.Load(stream);

            ExceptionHelper.FalseThrow(workbook.Worksheets.Contains("Matrix"),
                "The workbook must contains a 'Matrix' worksheet.");

            SOARole role = null;

            ServiceBrokerContext.Current.SaveContextStates();
            try
            {
                if (this.AppCodeName.IsNotEmpty() && this.RoleCodeName.IsNotEmpty())
                    role = new SOARole(this.AppCodeName + ":" + this.RoleCodeName);
                else
                    role = new SOARole(this.Definition) { ID = RoleID };

                role.Rows.Clear();

                NamedLocationCollection fieldLocations = workbook.Names.ToLocations();

                TableNode table = workbook.Worksheets["Matrix"].Table;

                int baseRowIndex = GetStartRow(fieldLocations);

                RowNode titleRow = table.GetRowByIndex(baseRowIndex);

                int currentRowIndex = table.Rows.IndexOf(titleRow) + 1;

                if (table.Rows.Count > currentRowIndex)
                {
                    UploadProgressStatus status = new UploadProgressStatus();

                    status.CurrentStep = 1;
                    status.MinStep = 0;
                    status.MaxStep = table.Rows.Count - currentRowIndex;

                    int currentVirtualRow = baseRowIndex;

                    for (int i = status.MinStep; i < status.MaxStep; i++)
                    {
                        RowNode row = table.Rows[currentRowIndex];

                        if (row.Index > 0)
                            currentVirtualRow = row.Index;
                        else
                            currentVirtualRow++;

                        GenerateMatrixRow(role, row, fieldLocations, i);

                        status.CurrentStep = i;
                        status.Response();

                        currentRowIndex++;
                    }

                    status.CurrentStep = status.MaxStep;
                    status.Response();
                }
                //插入记录
                SOARolePropertiesAdapter.Instance.Update(role);
            }
            finally
            {
                ServiceBrokerContext.Current.RestoreSavedStates();
            }
        }

        /// <summary>
        /// 导入Open Xml格式的文件
        /// </summary>
        /// <param name="importStream"></param>
        /// <param name="notifier"></param>
        private void ImportFromExcel2007(Stream importStream, Action notifier)
        {
            WorkBook workbook = WorkBook.Load(importStream);

            SOARole role = null;

            ServiceBrokerContext.Current.SaveContextStates();

            try
            {
                ServiceBrokerContext.Current.UseLocalCache = false;
                ServiceBrokerContext.Current.UseServerCache = false;

                if (this.AppCodeName.IsNotEmpty() && this.RoleCodeName.IsNotEmpty())
                    role = new SOARole(this.AppCodeName + ":" + this.RoleCodeName);
                else
                    role = new SOARole(this.Definition) { ID = RoleID };

                if (NeedValidateSource)
                    CheckImportSource(role, workbook);

                DataTable dt = DocumentHelper.GetRangeValuesAsTable(workbook, "Matrix", "A3");

                role.Rows.Clear();

                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    SOARolePropertyRow mRow = new SOARolePropertyRow(role) { RowNumber = rowIndex };

                    foreach (var dimension in this.Definition)
                    {
                        SOARolePropertyValue mCell = new SOARolePropertyValue(dimension);
                        mCell.Value = row[dimension.Name].ToString();

                        switch (dimension.Name)
                        {
                            case "Operator":
                                mRow.Operator = row[dimension.Name].ToString();
                                break;
                            case "OperatorType":
                                SOARoleOperatorType opType = SOARoleOperatorType.User;
                                Enum.TryParse(row[dimension.Name].ToString(), out opType);
                                mRow.OperatorType = opType;
                                break;
                            default:
                                break;
                        }
                        mRow.Values.Add(mCell);
                    }

                    if (notifier != null)
                    {
                        notifier();
                    }

                    rowIndex++;
                    role.Rows.Add(mRow);
                }

                //插入记录
                SOARolePropertiesAdapter.Instance.Update(role);
            }
            finally
            {
                ServiceBrokerContext.Current.RestoreSavedStates();
            }
        }

        private static int GetStartRow(NamedLocationCollection locations)
        {
            int result = 0;

            if (locations.Count > 0)
                result = locations[0].Row;

            return result;
        }

        private SOARolePropertyRow GenerateMatrixRow(SOARole role, RowNode rowNode, NamedLocationCollection locations, int index)
        {
            SOARolePropertyRow mRow = new SOARolePropertyRow(role);

            mRow.RowNumber = index;

            int emptyCellCount = 0;

            foreach (var row in this.Definition)
            {
                CellLocation location = locations[row.Name];

                CellNode cell = rowNode.GetCellByIndex(location.Column);

                SOARolePropertyValue mCell = new SOARolePropertyValue(row);

                mCell.Value = cell.Data.InnerText.Trim();

                mRow.Values.Add(mCell);

                switch (row.Name)
                {
                    case "Operator":
                        mRow.Operator = cell.Data.InnerText;
                        break;
                    case "OperatorType":
                        SOARoleOperatorType opType = SOARoleOperatorType.User;
                        Enum.TryParse(cell.Data.InnerText, out opType);
                        mRow.OperatorType = opType;
                        break;
                    default:
                        if (mCell.Value.IsNullOrEmpty())
                            emptyCellCount++;
                        break;
                }
            }

            role.Rows.Add(mRow);

            return mRow;
        }

        /// <summary>
        /// 初始化上传矩阵行的界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected void uploadProgress_UploadProgressContentInited(object sender, UploadProgressContentEventArgs eventArgs)
        {
            CheckEditMode();

            if (eventArgs.AfterFileSelectorContainer != null)
            {
                HtmlGenericControl checkBox = new HtmlGenericControl("input");

                checkBox.Attributes["checked"] = "true";
                checkBox.Attributes["name"] = "validateSource";
                checkBox.Attributes["id"] = "validateSource";
                checkBox.Attributes["value"] = "validateSource";
                checkBox.Attributes["type"] = "checkbox";

                eventArgs.AfterFileSelectorContainer.Controls.Add(checkBox);

                HtmlGenericControl label = new HtmlGenericControl("label");

                label.InnerText = "校验导入的矩阵是否是相同的来源";
                label.Attributes["for"] = "validateSource";

                eventArgs.AfterFileSelectorContainer.Controls.Add(label);
            }
        }

        private static void CheckImportSource(SOARole role, WorkBook workbook)
        {
            string roleFullCodeName = GetRoleFullCodeName(role);

            if (roleFullCodeName.IsNotEmpty())
            {
                string excelSource = workbook.FileDetails.Subject;

                if (excelSource.IsNotEmpty())
                {
                    if (string.Compare(roleFullCodeName, excelSource, true) != 0)
                        throw new InvalidOperationException(string.Format("导入的角色矩阵的来源是{0}，和目标{1}不匹配。如果需要强制导入，请去掉\"校验导入的矩阵是否是相同的来源\"选择框",
                            excelSource, roleFullCodeName));
                }
            }
        }

        private static string GetRoleFullCodeName(SOARole role)
        {
            string result = string.Empty;

            try
            {
                result = role.FullCodeName;
            }
            catch (System.Exception)
            {
            }

            return result;
        }

    }
}