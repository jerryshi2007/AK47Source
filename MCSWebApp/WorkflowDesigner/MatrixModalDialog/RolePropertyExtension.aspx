<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RolePropertyExtension.aspx.cs"
	Inherits="MCS.Applications.AppAdmin.Dialogs.RolePropertyExtension" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.Passport" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<base target="_self" />
	<title>角色属性扩展矩阵</title>
	<style type="text/css">
		.matrixInfoContainer
		{
			font-size: 12px;
			margin-left: 16px;
			line-height: 32px;
		}
		.link
		{
			color: blue;
		}
	</style>
	<script type="text/javascript">
		var m_objParam = window.dialogArguments;
		var openedWindowHandle = null;

		function onSelectMatrixDefinition(change) {
			if (checkHasDeltaData()) {
				alert("矩阵正在编辑，请先保存！");
				return false;
			}

			var roleID = m_objParam["id"];
			var editMode = m_objParam.editMode;

			if (typeof (m_objParam["definitionID"]) != "undefined") {
				if (roleID != m_objParam["definitionID"])
					editMode = "readOnly";

				roleID = m_objParam["definitionID"];
			}

			var url = "RolePropertyDefine.aspx?AppID=" + m_objParam["appID"] + "&RoleID=" + roleID + "&CMD=" + change + "&editMode=" + editMode;
			var feature = "dialogWidth:800px; dialogHeight:540px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			var result = window.showModalDialog(url, m_objParam, feature);

			if (result) {
				$get("bindMatrixBtn").click();
			}

			return false;
		}

		function onDeleteMatrix() {
			if (checkHasDeltaData()) {
				alert("矩阵正在编辑，请先保存！");
				return false;
			}

			if (window.confirm("确定要删除授权矩阵吗？这将删除所有数据。"))
				$get("deleteMatrixBtn").click();

			return false;
		}

		function onExportMatrix(cmd) {
			if (checkHasDeltaData()) {
				alert("矩阵正在编辑，请先保存！");
				return false;
			}

			var format = "xml";
			if (document.getElementsByName('RaidoOpMode')[1].checked) {
				format = "xlsx";
			}

			var definitionID = m_objParam["id"];

			if (typeof (m_objParam["definitionID"]) != "undefined")
				definitionID = m_objParam["definitionID"];

			var url = "ExportRoleProperty.aspx?AppID=" + m_objParam["appID"] + "&appCode=" + encodeURI(m_objParam["App_CodeName"]) + "&appName=" + encodeURI(m_objParam["App_Name"]) +
				"&RoleID=" + m_objParam["id"] + "&format=" + format + "&roleName=" + encodeURI(m_objParam["Role_Name"]) + "&roleCode=" + encodeURI(m_objParam["Role_CodeName"]) +
				"&definitionID=" + encodeURI(definitionID);

			window.open(url, "_blank");
		}

		function onImportMatrix() {
			if (checkHasDeltaData()) {
				alert("矩阵正在编辑，请先保存！");
				return false;
			}

			$find("uploadProgress").showDialog();
		}

		function downloadPropertiesFile() {
			if (checkHasDeltaData()) {
				alert("矩阵正在编辑，请先保存！");
				return false;
			}

			event.returnValue = false;
			var downElement = event.srcElement;

			var roleID = m_objParam["id"];

			if (typeof (m_objParam["definitionID"]) != "undefined") {
				roleID = m_objParam["definitionID"];
			}

			var requestBody = String.format("{0}?roleID={1}&roleCode={2}&roleName={3}", downElement.href, roleID, encodeURI(m_objParam["codeName"]), encodeURI(m_objParam["Role_Name"]));

			window.open(requestBody, "down");

			event.cancelBubble = true;

			return false;
		}

		function onImportSOARolePropertyDefinition() {
			if (checkHasDeltaData()) {
				alert("矩阵正在编辑，请先保存！");
				return false;
			}

			$find("uploadProgressImportSOARole").showDialog();
		}

		function onCompleted(e) {
			if (e.dataChanged == true) {
				document.getElementById("noMartrixInfoContainer").style.display = 'none';
				document.getElementById("martrixInfoContainer").style.display = 'block';
				document.getElementById("exportBtn").disabled = false;
				document.getElementById("btnDeleteMatrix").disabled = false;
				document.getElementById("importBtn").disabled = false;
				//document.getElementById("edtBlock").disabled = false;
				document.getElementById("materialCtrlContainer").style.display = "block";
			}
		}

		function matrixFileOpen() {
			$get("btnSaveMatrix").disabled = false;
		}

		function checkMatrixFile() {
			$find("materialCtrlForMatrix")._materials[0].click();
		}

		function checkHasDeltaData() {
			var result = false;

			result = $get("btnSaveMatrix").disabled != true;

			return result;
		}
		//        function checkTicket() {
		//            $get("prompt").innerHTML = '(正在获取票据...)';
		//            $get("ticketAnchor").click();
		//        }

		//        function onAccquiredAccessTicket(lnk) {
		//            $get("prompt").innerHTML = '(点击进入编辑)';
		//            $get("shield").className = 'wd-shield-container wd-shield-hide';
		//            var href = lnk.href;
		//            var edtB = $get("edtBlock");
		//            edtB.onclick = function () {
		//                if (!openedWindowHandle || openedWindowHandle.closed)
		//                    try {
		//                        openedWindowHandle = window.open(href, "newWindow");
		//                        edtB.onclick = checkTicket;
		//                    } catch (ex) {
		//                        alert('弹出窗口失败，请检查是否启用了弹出窗口阻止程序。请尝试再单击一次。');
		//                    }
		//                else {
		//                    openedWindowHandle.focus();
		//                }
		//            };

		//            edtB.onclick();
		//        }

	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
	</asp:ScriptManager>
	<mcs:AccessTicketChecker runat="server" ID="ticketChecker" CheckPhase="Load" />
	<div style="display: none">
		<%--       <mcs:AccessTicketHtmlAnchor runat="server" ID="ticketAnchor" OnClientAccquiredAccessTicket="onAccquiredAccessTicket">
            点击此处获取令牌
        </mcs:AccessTicketHtmlAnchor>--%>
	</div>
	<div>
		<mcs:UploadProgressControl runat="server" ID="uploadProgress" DialogTitle="上传角色属性扩展矩阵"
			OnDoUploadProgress="uploadProgress_DoUploadProgress" OnUploadProgressContentInited="uploadProgress_UploadProgressContentInited" />
		<mcs:UploadProgressControl runat="server" ID="uploadProgressImportSOARole" DialogTitle="上传角色属性扩展矩阵属性定义"
			OnDoUploadProgress="ImportSOARole_DoUploadProgress" OnClientCompleted="onCompleted" />
	</div>
	<div>
		<asp:Button runat="server" ID="bindMatrixBtn" Text="BindMatrix" Style="display: none"
			OnClick="bindMatrixBtn_Click" />
		<asp:Button runat="server" ID="deleteMatrixBtn" Text="DeleteMatrix" Style="display: none"
			OnClick="deleteMatrixBtn_Click" />
		<input id="selectedMatrix" runat="server" type="hidden" />
		<input id="matrixID" runat="server" type="hidden" />
	</div>
	<table style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">角色属性扩展矩阵</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: middle">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<!--Put your dialog content here... -->
					<table width="100%" style="height: 100%; width: 100%">
						<tr>
							<td>
								<div id="noMartrixInfoContainer" runat="server" class="matrixInfoContainer">
									<div id="Div1" runat="server" style="display: inline">
										当前没有角色属性扩展矩阵信息，请选择如下操作
									</div>
									<a href="javascript:void(0);" class="link" onclick="onSelectMatrixDefinition(false);"
										runat="server" id="lnkPropertyDefinition">属性定义...</a>|<a href="javascript:void(0);"
											class="link" id="lnkImportDefinition" runat="server" onclick="onImportSOARolePropertyDefinition();">导入...</a>
								</div>
								<div id="martrixInfoContainer" runat="server" style="display: none" class="matrixInfoContainer">
									<div id="matrixInfo" runat="server" style="display: inline">
									</div>
									<a href="javascript:void(0);" id="lnkDeleteDenifition" runat="server" class="link"
										onclick="return onDeleteMatrix();">删除...</a>| <a href="javascript:void(0);" class="link"
											runat="server" id="lnkChangeDefinition" onclick="return onSelectMatrixDefinition(true);">
											修改...</a>|<a href="ExportSOARolePropertyDefinitionHandler.ashx" class="link" onclick="return downloadPropertiesFile();">导出...</a>|<a
												href="javascript:void(0);" class="link" runat="server" id="lnkImportDefinition2"
												onclick="return onImportSOARolePropertyDefinition();">导入...</a>
								</div>
								<div>
									<div>
										&nbsp;&nbsp;&nbsp;&nbsp;导入/导出 格式：
										<input type="radio" id="RaidoOpMode0" runat="server" name="RaidoOpMode" value="0" /><label
											for="RaidoOpMode0">Xml</label>
										<input type="radio" id="RaidoOpMode1" runat="server" name="RaidoOpMode" value="1"
											checked /><label for="RaidoOpMode1">Xlsx</label>
									</div>
									<div>
										<div style="float: left">
											<div style="float: left">
												<input type="button" id="exportBtn" class="formButton" value="导出..." runat="server"
													disabled="disabled" onclick="onExportMatrix();" />
											</div>
											<div style="float: left">
												<input type="button" id="importBtn" class="formButton" value="导入..." runat="server"
													disabled="disabled" onclick="onImportMatrix();" />
											</div>
											<div style="float: left">
												<input disabled="disabled" type="button" id="btnDeleteMatrix" class="formButton"
													value="删除" runat="server" onserverclick="btnDeleteMatrix_Click" />
											</div>
										</div>
										<%--<div style="display: none">
											<input type="button" id="editBtn" class="formButton" value="在线编辑..." runat="server"
												disabled="disabled" onclick="onEditMatrix();" />
										</div>--%>
										<%--<div style="float: left; position: relative;">
                                            <button type="button" id="edtBlock" runat="server" onclick="checkTicket();" class="formButton"
                                                style="height: auto; background: none; position: relative;">
                                                <div class="wd-shield-container" id="shield">
                                                    <span class="wd-shield" title="需要获取票据后方可继续"></span>
                                                </div>
                                                在线编辑...
                                                <div id="prompt">
                                                    (点击获取票据)</div>
                                            </button>
                                        </div>--%>
										<div id="materialCtrlContainer" runat="server">
											<div style="float: left; padding-top: 5px">
												<mcs:MaterialControl ID="materialCtrlForMatrix" MaterialUseMode="SingleDraft" TemplateUrl="~/MatrixModalDialog/Templates/EditRowTemplate.xlsx"
													DraftText="编辑矩阵" MaterialTitle="矩阵" RootPathName="GenericProcess" OnDocumentOpen="matrixFileOpen"
													OnPrepareDownloadStream="materialCtrl_PrepareDownloadStream" runat="server">
												</mcs:MaterialControl></div>
											<div style="float: left">
												<input disabled="disabled" type="button" id="btnSaveMatrix" class="formButton" value="保存"
													runat="server" onserverclick="btnSaveMatrix_Click" />
											</div>
										</div>
										<div id="martrixInfoReadOnlyContainer" style="padding-top: 5px; display: none" runat="server">
											<a href="" target="_blank" id="lnkCheckMatrixFile" runat="server" class="link">查看矩阵</a>
										</div>
									</div>
								</div>
							</td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel"
								accesskey="C" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
