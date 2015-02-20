using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Web.WebControls;
using MCS.Library.Office.SpreadSheet;


namespace WorkflowDesigner.MatrixModalDialog
{
    public partial class WfProcessMatrix : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        private static string ProcessKey
        {
            get
            {
                return HttpContext.Current.Request.QueryString.GetValue("processKey", string.Empty);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string processKey = ProcessKey;

            if (processKey.IsNotEmpty())
            {
                WfMatrix matrix = WfMatrixAdapter.Instance.LoadByProcessKey(processKey, false);

                RenderByMatrix(matrix);
            }
        }

        private void RenderByMatrix(WfMatrix matrix)
        {
            if (matrix != null)
            {
                noMartrixInfoContainer.Visible = false;
                martrixInfoContainer.Visible = true;
                exportBtn.Disabled = false;
                importBtn.Disabled = false;
                btnDeleteMatrix.Disabled = false;
                //editBtn.Disabled = false;
                matrixID.Value = matrix.MatrixID;
                processKey.Value = matrix.ProcessKey;
                matrixInfo.InnerText = string.Format("当前权限矩阵定义为\"{0}({1})\"。点击这里",
                matrix.Definition.Name, matrix.Definition.Key);

                materialCtrlContainer.Style["display"] = "block";

                this.materialCtrlForMatrix.RequestContext = matrixID.Value + ";" + chkRoleAsPerson.Checked + ";" + processKey.Value + ";";
            }
            else
            {
                noMartrixInfoContainer.Visible = true;
                martrixInfoContainer.Visible = false;
                exportBtn.Disabled = true;
                importBtn.Disabled = true;
                btnDeleteMatrix.Disabled = true;
                //editBtn.Disabled = true;
                matrixID.Value = string.Empty;
                processKey.Value = string.Empty;

                materialCtrlContainer.Style["display"] = "none";
            }
        }

        protected void bindMatrixBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string mdKey = selectedMatrix.Value;

                WfMatrixDefinition md = WfMatrixDefinitionAdapter.Instance.Load(mdKey);

                WfMatrix matrix = new WfMatrix(md);

                matrix.MatrixID = UuidHelper.NewUuidString();
                matrix.ProcessKey = ProcessKey;

                if (DeluxePrincipal.IsAuthenticated)
                {
                    matrix.CreatorID = DeluxeIdentity.CurrentUser.ID;
                    matrix.CreatorName = DeluxeIdentity.CurrentUser.DisplayName;
                }

                WfMatrixAdapter.Instance.Update(matrix);
            }
            catch (System.Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }

        protected void deleteMatrixBtn_Click(object sender, EventArgs e)
        {
            try
            {
                WfMatrixAdapter.Instance.Delete(matrixID.Value);
                this.materialCtrlForMatrix.Materials.Clear();
            }
            catch (System.Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }

        protected void uploadProgress_DoUploadProgress(HttpPostedFile file, UploadProgressResult result)
        {
            var fileType = Path.GetExtension(file.FileName).ToLower();
            if (fileType != ".xml" && fileType != ".xlsx")
            {
                throw new Exception("'{0}' must be a xml or xlsx file.");
            }

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
                WfMatrix.ImportExistMatrixFromExcelXml(file.InputStream, notifier, ProcessKey);
            }
            else if (fileType == ".xlsx")
            {
                WfMatrix.ImportExistMatrixFromExcel2007(file.InputStream, notifier, ProcessKey);
            }

            result.DataChanged = true;
            result.CloseWindow = true;
        }

        protected void btnDeleteMatrix_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["editMode"] == "readOnly")
                throw new InvalidOperationException("只读模式不可以操作");

            this.materialCtrlForMatrix.Materials.Clear();

            DeleteMatrix();
        }

        private void DeleteMatrix()
        {
            WfMatrix matrix = WfMatrixAdapter.Instance.LoadByProcessKey(ProcessKey, false);
            matrix.Rows.Clear();
            WfMatrixAdapter.Instance.Update(matrix);
        }

        protected void btnSaveMatrix_Click(object sender, EventArgs e)
        {
            int count = this.materialCtrlForMatrix.Materials.Count;
            if (count > 0 && !this.materialCtrlForMatrix.DeltaMaterials.IsEmpty())
            {
                Material keyTerial = null;
                if (this.materialCtrlForMatrix.DeltaMaterials.Inserted.Count > 0)
                {
                    keyTerial = this.materialCtrlForMatrix.DeltaMaterials.Inserted[this.materialCtrlForMatrix.DeltaMaterials.Inserted.Count - 1];
                }
                else if (this.materialCtrlForMatrix.DeltaMaterials.Updated.Count > 0)
                {
                    keyTerial = this.materialCtrlForMatrix.DeltaMaterials.Updated[this.materialCtrlForMatrix.DeltaMaterials.Updated.Count - 1];
                }

                if (keyTerial != null)
                {
                    string[] itemsInfio = this.materialCtrlForMatrix.RequestContext.Split(';');
                    string processKey = itemsInfio[2];
                    using (Stream LoadFile = keyTerial.GetTemporaryContent(this.materialCtrlForMatrix.RootPathName))
                    {
                        WfMatrix.ImportExistMatrixFromExcel2007(LoadFile, null, processKey);
                    }
                }
            }
        }

        protected void materialCtrl_PrepareDownloadStream(object sender, PrepareDownloadStreamEventArgs args)
        {
            bool roleAsPerson = false;
            string[] itemsInfio = args.DownloadInfo.RequestContext.Split(';');
            Boolean.TryParse(itemsInfio[1], out roleAsPerson);

            string downloadkey = itemsInfio[0];
            WfMatrix matrix = WfMatrixAdapter.Instance.Load(downloadkey);
            matrix.Loaded = true;

            using (MemoryStream excelStream = matrix.ExportToExcel2007(roleAsPerson))
            {
                excelStream.CopyTo(args.OutputStream);
            }
        }
    }
}