using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Web.Library.Script;
using System.Web.Services;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;
using System.IO;
using MCS.Web.WebControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter.SchemaAdmin
{
    [SceneUsage("~/App_Data/ListScene.xml")]
    public partial class SchemaRoleList : System.Web.UI.Page, ITimeSceneDescriptor
    {
        private AU.AUSchema schema;

        public bool EditEnabled
        {
            get
            {
                return Util.SuperVisiorMode && TimePointContext.Current.UseCurrentTime;
            }
        }

        public string NormalSceneName
        {
            get { return this.EditEnabled ? "Normal" : "ReadOnly"; }
        }

        public string ReadOnlySceneName
        {
            get { return "ReadOnly"; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            this.schema = DbUtil.GetEffectiveObject<AU.AUSchema>(this.Request.QueryString["id"]);

            this.schemaInfoLabel.Text = this.schema.Name;
            this.schemaIDField.Value = this.schema.ID;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);


            if (this.Request.QueryString["showAll"] == "yes")
            {
                this.lnkToggle.Text = ">>不显示已删除的角色";
                this.lnkToggle.NavigateUrl = "SchemaRoleList.aspx?id=" + Server.UrlEncode(this.schema.ID);
                this.lnkRefresh.HRef = "SchemaRoleList.aspx?showAll=yes&id=" + Server.UrlEncode(this.schema.ID);
            }
            else
            {
                this.lnkToggle.Text = ">>显示已删除的角色";
                this.lnkToggle.NavigateUrl = "SchemaRoleList.aspx?showAll=yes&id=" + Server.UrlEncode(this.schema.ID);
                this.lnkRefresh.HRef = "SchemaRoleList.aspx?id=" + Server.UrlEncode(this.schema.ID);
            }



            this.lnkAdd.Attributes["data-id"] = schema.ID;
        }

        protected void RefreshList(object sender, EventArgs e)
        {
            this.InnerRefreshList();
        }

        private void InnerRefreshList()
        {
            // 重新刷新列表
            this.dataSourceMain.LastQueryRowCount = -1;
            this.gridMain.SelectedKeys.Clear();
            this.Page.PreRender += new EventHandler(this.DelayRefreshList);
        }

        private void DelayRefreshList(object sender, EventArgs e)
        {
            this.gridMain.DataBind();
        }


        protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (e.ExecutingSelectCount == false)
            {
                e.InputParameters["normalOnly"] = this.Request.QueryString["showAll"] != "yes";
            }
        }

        protected void gridMain_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                var key = (string)e.CommandArgument;

                this.InnerRefreshList();
            }
        }

        protected void ctlProgress_ExecuteStep(object data)
        {
            var role = DbUtil.GetEffectiveObject<AU.AUSchemaRole>((string)data, false);

            if (role.Status == SchemaObjectStatus.Normal)
            {
                AU.Operations.Facade.InstanceWithoutPermissionsAndLockCheck.DeleteAdminSchemaRole(role);
            }
        }

        protected void ctlProgress_ExecuteSingleStep(object data)
        {
            var role = DbUtil.GetEffectiveObject<AU.AUSchemaRole>((string)data, false);

            if (role.Status == SchemaObjectStatus.Normal)
            {
                AU.Operations.Facade.InstanceWithoutPermissionsAndLockCheck.DeleteAdminSchemaRole(role);
            }
            else
            {
                var owner = role.GetOwnerAUSchema();

                if (owner == null || owner.Status != SchemaObjectStatus.Normal)
                {
                    throw new AU.AUObjectValidationException("对象状态无效");
                }
                else
                {
                    AU.Operations.Facade.InstanceWithoutPermissionsAndLockCheck.AddAdminSchemaRole(role, owner);
                }
            }
        }

        protected void gridMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (int)DataBinder.Eval(e.Row.DataItem, "Status") != 1)
                e.Row.CssClass = e.Row.RowIndex % 2 == 0 ? "item au-deleted-row" : "aitem au-deleted-row";
        }

        protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
        {
            var fileType = Path.GetExtension(file.FileName).ToLower();

            if (fileType != ".xml")
                throw new InvalidDataException("上传的文件类型错误");

            ImportExecutor executor = new ImportExecutor(file, result);

            executor.AddAction(new AUSchemaRoleImportAction()
            {
                AUSchemaID = Request.QueryString["id"]
            });

            executor.Execute();
        }

        protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
        {
            e.Content = WebXmlDocumentCache.GetDocument("~/inc/CommonTemplate.htm");
        }
    }
}