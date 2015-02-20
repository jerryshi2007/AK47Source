<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRoleProperty.aspx.cs"
	Inherits="WorkflowDesigner.MatrixModalDialog.EditRoleProperty" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Library.Passport" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>角色属性在线编辑</title>
	<script src="../js/jquery-1.4.3.js" type="text/javascript"></script>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" EnableScriptGlobalization="true" AsyncPostBackTimeout="1000"
		runat="server">
	</asp:ScriptManager>
	<mcs:AccessTicketChecker runat="server" ID="ticket1" CheckPhase="Load" />
	<div>
		<div id="editPan" runat="server">
			<p>
				<input id="btclientSave" type="button" value="提交" class="formButton" onclick="return btnSave_onclick()" />
			</p>
			<div style="display: none">
				<%-- <asp:UpdatePanel ID="upan_Save" UpdateMode="Conditional" runat="server">
                <ContentTemplate>--%>
				<asp:Button ID="btn_serverSave" runat="server" Text="保存" OnClick="btnServerSave_Click" />
				<%--     </ContentTemplate>
            </asp:UpdatePanel>--%>
			</div>
			<script language="javascript" type="text/javascript">
				function btnSave_onclick() {
				    $get("btn_serverSave").click();
				}

//				function RefreshPage() {
//					var control = $find("MCRolePropertyEditTemplate");
//					control._materials = new Array();
//					control._windowdataBind(control._materials);
//					control._createDraftMaterial();
//					control._buildControl();

//					control._draftBtnClick();
//				}
			</script>
		</div>
		<SOA:MaterialControl ID="MCRolePropertyEditTemplate" runat="server" RootPathName="GenericProcess"
			DefaultClass="NormalText" AllowEdit="true" AllowEditContent="true" MaterialTableShowMode="Inline"
			MaterialUseMode="SingleDraft" EditDocumentInCurrentPage="true" AutoOpenDocument="true"
			ShowFileTitle="false" TemplateUrl="~/MatrixModalDialog/Templates/EditRowTemplate.xlsx"
			OnPrepareDownloadStream="RolePropertyEdit_PrepareDownloadStream" OfficeViewerHeight="100%"
			OfficeViewerWidth="100%"  style="display:none;" />
	</div>
	</form>
</body>
</html>
