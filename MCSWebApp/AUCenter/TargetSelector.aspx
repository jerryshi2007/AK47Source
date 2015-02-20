<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TargetSelector.aspx.cs"
    Inherits="AUCenter.TargetSelector" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>选择移动目标</title>
    <link href="Styles/dlg.css" rel="stylesheet" type="text/css" />
</head>
<body class="pcdlg">
    <form id="form1" runat="server">
    <div class="pcdlg-sky">
        <div class="pc-banner">
            <h1 class="pc-caption">
                <img src="Images/icon_01.gif" alt="图标" />
                <span id="schemaLabel" runat="server"></span>-管理单元浏览
            </h1>
        </div>
    </div>
    <div class="pcdlg-content">
        <mcs:DeluxeTree runat="server" ID="tree" OnGetChildrenData="tree_GetChildrenData">
        </mcs:DeluxeTree>
        <div>
            <asp:HiddenField runat="server" ID="hfSchemaID" />
            <asp:HiddenField runat="server" ID="hfParentID" />
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" value="确定" class="pcdlg-button" id="btnOk" onclick="saveClick();" />
            <input type="button" value="取消" class="pcdlg-button" onclick="window.close();" />
        </div>
    </div>
    </form>
    <script type="text/javascript">
        function saveClick() {
            var node = $find("tree").get_selectedNode();
            if (node) {
                if (node.get_value() == document.getElementById("hfParentID").value) {
                    alert('选择的单元与源单元相同');
                } else {
                    ;
                    window.returnValue = node.get_value();
                    window.close();
                }
            }
        }
    </script>
</body>
</html>
