using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WfResourceEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WfConverterHelper.RegisterConverters();

            var user = new WfUserResourceDescriptor();
            //user.
            var typeName = typeof(WfUserResourceDescriptor).Name;

            List<object> resTypes = new List<object>() { 
                new  { shortType = typeof(WfUserResourceDescriptor).Name,displayName = "用户", imgUrl = ControlResources.UserLogoUrl},
                new  { shortType = typeof(WfDepartmentResourceDescriptor).Name,displayName = "组织", imgUrl = ControlResources.OULogoUrl},
                new  { shortType = typeof(WfGroupResourceDescriptor).Name,displayName = "用户组", imgUrl = ControlResources.GroupLogoUrl},
                new  { shortType = typeof(WfActivityOperatorResourceDescriptor).Name,displayName = "执行人", imgUrl = ControlResources.UserLogoUrl},
                new  { shortType = typeof(WfActivityAssigneesResourceDescriptor).Name,displayName = "指派人", imgUrl = ControlResources.UserLogoUrl},
                new  { shortType = typeof(WfRoleResourceDescriptor).Name,displayName = "角色", imgUrl = ControlResources.RoleLogoUrl},
                new  { shortType = typeof(WfDynamicResourceDescriptor).Name,displayName = "动态角色", imgUrl = ControlResources.RoleLogoUrl},
                new  { shortType = typeof(WfActivityMatrixResourceDescriptor).Name,displayName = "矩阵角色", imgUrl = ControlResources.RoleLogoUrl},
				new  { shortType = typeof(WfAURoleResourceDescriptor).Name,displayName = "(AU)架构角色", imgUrl = ControlResources.RoleLogoUrl}
            };

            hiddenResTypesData.Value = JSONSerializerExecute.Serialize(resTypes);
            WebUtility.RequiredScript(typeof(ClientGrid));
            this.CreateResourceTemplate();
        }

        private void CreateResourceTemplate()
        {
            hiddenUserResTemplate.Value = JSONSerializerExecute.Serialize(WfUserResourceDescriptor.EmptyInstance);
            hiddenDepartResTemplate.Value = JSONSerializerExecute.Serialize(WfDepartmentResourceDescriptor.EmptyInstance);
            hiddenRoleResTemplate.Value = JSONSerializerExecute.Serialize(WfRoleResourceDescriptor.EmptyInstance);
            hiddenGroupResTemplate.Value = JSONSerializerExecute.Serialize(WfGroupResourceDescriptor.EmptyInstance);
            hiddenActOperatorResTemplate.Value = JSONSerializerExecute.Serialize(WfActivityOperatorResourceDescriptor.EmptyInstance);
            hiddenActAssigneeResTemplate.Value = JSONSerializerExecute.Serialize(WfActivityAssigneesResourceDescriptor.EmptyInstance);
            hiddenDynamicResTypesDataTemplate.Value = JSONSerializerExecute.Serialize(WfDynamicResourceDescriptor.EmptyInstance);
            hiddenActiveMatrixResTemplate.Value = JSONSerializerExecute.Serialize(WfActivityMatrixResourceDescriptor.EmptyInstance);
            hiddenAURoleResTemplate.Value = JSONSerializerExecute.Serialize(WfAURoleResourceDescriptor.EmptyInstance);
        }

        protected void UploadActivityMatrixProgress(HttpPostedFile file, UploadProgressResult result)
        {
            var fileType = Path.GetExtension(file.FileName).ToLower();

            if (fileType != ".xlsx")
                throw new SystemSupportException("'{0}' 必须是xlsx文件。");

            WorkBook workBook = WorkBook.Load(file.InputStream);

            WfActivityMatrixResourceDescriptor matrix = workBook.ToActivityMatrixResourceDescriptor("Matrix", "A3");

            result.Data = JSONSerializerExecute.Serialize(matrix);
            result.DataChanged = true;
            result.CloseWindow = true;
        }
    }
}