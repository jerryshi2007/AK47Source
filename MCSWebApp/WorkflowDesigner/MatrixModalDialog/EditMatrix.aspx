<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditMatrix.aspx.cs" Inherits="WorkflowDesigner.MatrixModalDialog.EditMatrix" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程的权限矩陈在线编辑</title>
    <script src="../js/jquery-1.4.3.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnableScriptGlobalization="true" AsyncPostBackTimeout="1000"
        runat="server">
    </asp:ScriptManager>
    <div style="width: 100%">
        <p>
            <input id="btclientSave" type="button" value="保存" class="formButton" onclick="return btnSave_onclick()" />
        </p>
        <div style="display: none">
            <asp:UpdatePanel ID="upan_Save" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:Button ID="btn_serverSave" runat="server" Text="保存" OnClick="btnServerSave_Click" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <SOA:MaterialControl ID="MCMatrixEditTemplate" runat="server" RootPathName="GenericProcess"
            SaveOriginalDraft="false" DefaultClass="NormalText" AllowEdit="true" AllowEditContent="true"
            MaterialTableShowMode="Inline" MaterialUseMode="SingleDraft" EditDocumentInCurrentPage="true"
            AutoOpenDocument="true" TemplateUrl="~/MatrixModalDialog/Templates/EditRowTemplate.xlsx"
            OnPrepareDownloadStream="RolePropertyEdit_PrepareDownloadStream" OfficeViewerHeight="100%"
            OfficeViewerWidth="100%" ShowFileTitle="false" Style="display: none" />
    </div>
    </form>
    <script language="javascript" type="text/javascript">
        function btnSave_onclick() {
            var viewer = $find("MCMatrixEditTemplate")._get_officeViewerWrapperViewer();
            viewer.save();

            $get("btn_serverSave").click();
        }

        function RefreshPage() {
            var control = $find("MCMatrixEditTemplate");
            control._materials = new Array();
            control._windowdataBind(control._materials);
            control._createDraftMaterial();
            control._buildControl();

            control._draftBtnClick();
        }
    </script>
</body>
</html>
