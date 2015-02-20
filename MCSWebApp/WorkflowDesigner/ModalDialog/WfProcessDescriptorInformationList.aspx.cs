using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Workflow.Importers;

namespace WorkflowDesigner.ModalDialog
{
    class AppListItem
    {
        public AppListItem(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }

    public partial class WfProcessDescriptorInformationList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WfConverterHelper.RegisterConverters();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (!IsPostBack)
            {
                dropdownExtender.DataSource = GetApplicationsByCurrentUser();
                dropdownExtender.DataValueField = "Name";
                dropdownExtender.DataTextField = "Name";
                dropdownExtender.DataBind();
                ExecuteQuery();
            }

            //bool isAdmin = (bool)ViewState["IsAdminUser"];
            //if(isAdmin){
            //    txtApplicationName.ReadOnly=false;
            //}else{
            //    txtApplicationName.ReadOnly = true;
            //}           

        }

        protected override void OnPreInit(EventArgs e)
        {
            if (Request["multiselect"] == "false")
            {
                this.ProcessDescInfoDeluxeGrid.MultiSelect = false;

            }
            base.OnPreInit(e);
        }

        private void ExecuteQuery()
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            string appName = string.IsNullOrEmpty(txtApplicationName.Text) == true ? "全部" : txtApplicationName.Text;
            if (appName.IsNotEmpty())
            {
                if ("全部" == appName)
                {
                    if (!RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
                    {
                        InSqlClauseBuilder inBuilder = new InSqlClauseBuilder();
                        inBuilder.AppendItem(AllApplicationNames());
                        builder.AppendItem("APPLICATION_NAME", inBuilder.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
                    }
                }
                else
                {
                    if ("其它" == appName)
                    {
                        builder.AppendItem("APPLICATION_NAME", " ");
                    }
                    else
                    {
                        builder.AppendItem("APPLICATION_NAME", appName);
                    }
                }
            }

            if (!string.IsNullOrEmpty(txtProgramName.Text))
                builder.AppendItem("PROGRAM_NAME", "%" + TSqlBuilder.Instance.CheckQuotationMark(txtProgramName.Text, false) + "%", "like");

            if (!string.IsNullOrEmpty(txtProcessKey.Text))
                builder.AppendItem("PROCESS_KEY", "%" + TSqlBuilder.Instance.CheckQuotationMark(txtProcessKey.Text, false) + "%", "like");

            if (!string.IsNullOrEmpty(txtProcessName.Text))
                builder.AppendItem("PROCESS_NAME", "%" + TSqlBuilder.Instance.CheckQuotationMark(txtProcessName.Text, false) + "%", "like");

            if (!string.IsNullOrEmpty(ddlEnabled.SelectedValue))
                builder.AppendItem("ENABLED", TSqlBuilder.Instance.CheckQuotationMark(ddlEnabled.SelectedValue, false));

            builder.AppendTenantCode();

            this.BindGrid(builder);
        }

        private void BindGrid(IConnectiveSqlClause whereSqlClause)
        {
            LastQueryRowCount = -1;
            var whereCondition = (HtmlInputHidden)this.FindControlByID("whereCondition", true);
            whereCondition.Value = whereSqlClause.ToSqlString(TSqlBuilder.Instance);

            this.ProcessDescInfoDeluxeGrid.SelectedKeys.Clear();
            this.ProcessDescInfoDeluxeGrid.PageIndex = 0;
        }

        protected void objectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        protected void objectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        private int LastQueryRowCount
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteWfProcesses();

                ProcessDescInfoDeluxeGrid.SelectedKeys.Clear();
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }

        private void DeleteWfProcesses()
        {
            if (ProcessDescInfoDeluxeGrid.SelectedKeys.Count == 0) return;

            foreach (var key in ProcessDescInfoDeluxeGrid.SelectedKeys)
            {
                WfDeleteTemplateExecutor executor = new WfDeleteTemplateExecutor(key);

                executor.Execute();
            }

            LastQueryRowCount = -1;
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                List<IWfProcessDescriptor> processDesps = new List<IWfProcessDescriptor>();

                foreach (string key in this.ProcessDescInfoDeluxeGrid.SelectedKeys)
                    processDesps.Add(WfProcessDescriptorManager.LoadDescriptor(key));

                resultData.Value = JSONSerializerExecute.Serialize(processDesps);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "returnProcesses",
                    string.Format("window.returnValue = $get('resultData').value; top.close();"),
                    true);
            }
            catch (System.Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.Compare(this.tabStrip.SelectedKey, "wherePanel", true) == 0)
                SelectExcutQuery();
            else
                ExecuteQuery();
        }

        private void SelectExcutQuery()
        {
            if (string.IsNullOrEmpty(this.tb_AllKeys.Text) == false)
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder();
                inBuilder.DataField = "PROCESS_KEY";
                inBuilder.AppendItem(this.tb_AllKeys.Text.Split(','));

                WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

                whereBuilder.AppendTenantCode();

                this.BindGrid(new ConnectiveSqlClauseCollection(LogicOperatorDefine.And, whereBuilder, inBuilder));
            }
        }

        private List<AppListItem> GetApplicationsByCurrentUser()
        {
            List<AppListItem> applications = new List<AppListItem>();
            if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
            {
                var defaultApp = WfProcessDescriptionCategoryAdapter.Instance.Load(
                    w => w.AppendItem("ID", "", "<>"),
                    o => o.AppendItem("NAME", FieldSortDirection.Ascending));

                foreach (var item in defaultApp)
                {
                    AppListItem obj = new AppListItem(item.Name);
                    if (!applications.Exists(a => a.Name == obj.Name))
                        applications.Add(obj);
                }
            }

            IRole[] roles = RolesDefineConfig.GetConfig().GetRolesInstances("DesignerRoleMatrix");
            foreach (var role in roles)
            {
                AppendApplicationName(role, applications);
            }

            AppListItem other = new AppListItem("其它");
            applications.Add(other);

            AppListItem all = new AppListItem("全部");
            applications.Insert(0, all);

            return applications;
        }

        private static void AppendApplicationName(IRole role, List<AppListItem> appList)
        {
            SOARolePropertyRowCollection allRows = SOARolePropertiesAdapter.Instance.GetByRole(role);
            SOARolePropertyRowCollection rows = allRows;

            if (!RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin"))
                rows = allRows.Query(r => r.Operator == DeluxeIdentity.Current.User.LogOnName);

            foreach (var row in rows)
            {
                string appName = row.Values.GetValue("ApplicationName", string.Empty);
                AppListItem obj = new AppListItem(appName);
                if (!appList.Exists(a => a.Name == obj.Name))
                    appList.Add(obj);
            }
        }

        private static string[] AllApplicationNames()
        {
            List<string> appList = new List<string>();
            IRole[] roles = RolesDefineConfig.GetConfig().GetRolesInstances("DesignerRoleMatrix");
            foreach (var role in roles)
            {
                SOARolePropertyRowCollection allRows = SOARolePropertiesAdapter.Instance.GetByRole(role);
                SOARolePropertyRowCollection rows = allRows.Query(r => r.Operator == DeluxeIdentity.Current.User.LogOnName);
                foreach (var row in rows)
                {
                    string appName = row.Values.GetValue("ApplicationName", string.Empty);
                    if (!appList.Contains(appName))
                        appList.Add(appName);
                }
            }
            appList.Add(" ");
            return appList.ToArray();
        }

        protected void ProcessDescInfoDeluxeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (DateTime.Parse(e.Row.Cells[6].Text) == DateTime.MinValue)
                        e.Row.Cells[6].Text = string.Empty;

                    if (DateTime.Parse(e.Row.Cells[7].Text) == DateTime.MinValue)
                        e.Row.Cells[7].Text = string.Empty;
                }

                e.Row.Cells[8].Visible = WfSimulationSettings.GetConfig().Enabled;
            }
        }

        protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
        {
            var fileType = Path.GetExtension(file.FileName).ToLower();

            if (fileType != ".xml" && fileType != ".zip")
            {
                throw new InvalidDataException("请上传一个xml或zip文件！");
            }

            if (fileType == ".xml")
            {
                var logger = ParseXmlFile(file);
                result.DataChanged = true;
                result.CloseWindow = false;
                result.ProcessLog = logger.ToString();

                return;
            }

            if (fileType == ".zip")
            {
                var logger = ParseZipFile(file);
                result.DataChanged = true;
                result.CloseWindow = false;
                result.ProcessLog = logger.ToString();
                return;
            }
        }

        /// <summary>
        /// 解析上传的xml文件，返回解析日志
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private StringBuilder ParseXmlFile(HttpPostedFile file)
        {
            StringBuilder logger = new StringBuilder();
            try
            {
                var xmlDoc = XDocument.Load(file.InputStream);
                var wfProcesses = xmlDoc.Descendants("Root");
                XElementFormatter formatter = new XElementFormatter();
                UploadProgressStatus status = new UploadProgressStatus();

                status.CurrentStep = 1;
                status.MinStep = 1;
                status.MaxStep = wfProcesses.Count() + 1;
                logger.AppendFormat("开始导入，共发现{1}个流程模板...\n", file.FileName, status.MaxStep);

                foreach (var wfProcess in wfProcesses)
                {
                    IWfProcessDescriptor wfProcessDesc = (IWfProcessDescriptor)formatter.Deserialize(wfProcess);

                    using (TransactionScope tran = TransactionScopeFactory.Create())
                    {
                        WfProcessDescHelper.SaveWfProcess(wfProcessDesc);
                        tran.Complete();
                    }

                    logger.AppendFormat("	{0}保存成功...\n", wfProcessDesc.Key);

                    status.CurrentStep++;
                    status.Response();
                }
                logger.AppendFormat("导入完成!", file.FileName);
            }
            catch (Exception ex)
            {
                logger.AppendFormat("导入错误，{0}，错误堆栈：{1}", ex.Message, ex.StackTrace);
            }

            return logger;
        }

        /// <summary>
        /// 解析上传的zip文件，返回解析日志
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private StringBuilder ParseZipFile(HttpPostedFile file)
        {
            StringBuilder logger = new StringBuilder();
            try
            {
                UploadProgressStatus status = new UploadProgressStatus();
                status.CurrentStep = 1;
                status.MinStep = 0;
                status.MaxStep = WfImportTemplateExecutor.MaxStep;

                WfImportTemplateExecutor executor = new WfImportTemplateExecutor(file.InputStream, info =>
                {
                    logger.Append(info);
                    status.CurrentStep++;
                    status.Response();
                });

                executor.Execute();
            }
            catch (Exception ex)
            {
                logger.AppendFormat("导入错误，{0}，错误堆栈：{1}", ex.Message, ex.StackTrace);
            }

            return logger;
        }

        ///// <summary>
        ///// 解析上传的zip文件，返回解析日志
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //private StringBuilder ParseZipFile(HttpPostedFile file)
        //{
        //    StringBuilder logger = new StringBuilder();
        //    try
        //    {
        //        UploadProgressStatus status = new UploadProgressStatus();
        //        status.CurrentStep = 1;
        //        status.MinStep = 0;
        //        status.MaxStep = ZipPackageImporter.StepCount;

        //        var importer = new ZipPackageImporter(file.InputStream);
        //        importer.NotifyEveryStep += info =>
        //        {
        //            logger.Append(info);
        //            status.CurrentStep++;
        //            status.Response();
        //        };
        //        importer.Import();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.AppendFormat("导入错误，{0}，错误堆栈：{1}", ex.Message, ex.StackTrace);
        //    }

        //    return logger;
        //}
    }
}