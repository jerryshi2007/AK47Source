<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportWfMatrix.aspx.cs"
    Inherits="WorkflowDesigner.MatrixModalDialog.ImportWfMatrix" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>导入权限矩阵</title>
    <script type="text/javascript">
        function onOpenDialogClick() {
            var result = $find("uploadProgress").showDialog();

        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <MCS:UploadProgressControl runat="server" ID="uploadProgress" DialogTitle="上传权限矩阵"
            OnDoUploadProgress="uploadProgress_DoUploadProgress" />
    </div>
    <div>
        <input type="button" value="导入权限矩阵" onclick="onOpenDialogClick();" />
    </div>
    </form>
</body>
</html>
