<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyView.aspx.cs" Inherits="PermissionCenter.Dialogs.PropertyView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>对象信息</title>
    <link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function onSaveClick() {
            var strips = tabStrip.get_control().strips;
            var allProperties = [];

            for (var i = 0; i < strips.length; i++) {
                var propertyForm = $find(strips[i].tag);

                Array.addRange(allProperties, propertyForm.get_properties());
            }

            $get("properties").value = Sys.Serialization.JavaScriptSerializer.serialize(allProperties);

            var saveBtn = $get("btSave");

            window.returnValue = true;

            if (saveBtn) {
                saveBtn.click();
            }
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server" target="innerFrame">
    <div>
        <asp:ScriptManager runat="server" ID="scriptManager" EnableScriptGlobalization="true">
            <Services>
                <asp:ServiceReference Path="~/Services/CommonService.asmx" />
            </Services>
        </asp:ScriptManager>
    </div>
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            对象属性<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <mcs:RelaxedTabStrip runat="server" ID="tabStrip">
        </mcs:RelaxedTabStrip>
        <div style="display: none">
            <input type="hidden" id="properties" runat="server" />
            <input type="hidden" id="currentSchemaType" runat="server" />
            <asp:Button runat="server" ID="btSave" OnClick="Save_Click" Text="保存(S)" CssClass="pcdlg-button btn-def" />
        </div>
        <div style="display: none">
            <iframe name="innerFrame"></iframe>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" id="okButton" runat="server" onclick="return onSaveClick()"
                accesskey="S" class="pcdlg-button btn-def" value="保存(S)" /><input type="button" accesskey="C"
                    class="pcdlg-button btn-cancel" onclick="window.returnValue = false;window.close()"
                    value="关闭(C)" />
        </div>
    </div>
    <script type="text/javascript" src="../scripts/PermissionPropertyEditors.js"></script>
    </form>
</body>
</html>
