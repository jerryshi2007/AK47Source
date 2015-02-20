<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfMatrixTest.aspx.cs"
    Inherits="WorkflowDesigner.MatrixModalDialog.WfMatrixTest" %>

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
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="权限查询" onclick="Button1_Click" />

    &nbsp;<br />
        <br />
        矩阵ID：&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="txtID" runat="server"></asp:TextBox>
        <br />
        <br />
        成本中心：<asp:TextBox ID="txtCostCenter" runat="server"></asp:TextBox>
        <br />
        <br />
        支付方式：<asp:TextBox ID="txtPaymentType" 
            runat="server"></asp:TextBox>
        <br />
        <br />
        部门：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:TextBox ID="txtDepartment" runat="server"></asp:TextBox>
        <br />
        <br />
        <br />
        
        候选人:&nbsp;
        <br />
        <asp:Label ID="Label1" runat="server"></asp:Label>
    </div>
    </form>
</body>
</html>
