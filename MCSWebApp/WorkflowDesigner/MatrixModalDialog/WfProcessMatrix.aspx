<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfProcessMatrix.aspx.cs"
    Inherits="WorkflowDesigner.MatrixModalDialog.WfProcessMatrix" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self">
    <title>流程的权限矩阵</title>
    <link href="../css/StyleSheet1.css" />
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
        function onSelectMatrixDefinition() {
            if (checkHasDeltaData()) {
                alert("矩阵正在编辑，请先保存！");
                return false;
            }
            
            var url = "WfMatrixDefinitionList.aspx?canSelect=true";
            var feature = "dialogWidth:800px; dialogHeight:540px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var result = window.showModalDialog(url, "", feature);

            if (result) {
                $get("selectedMatrix").value = result;
                $get("bindMatrixBtn").click();
            }

            return false;
        }

        function onDeleteMatrix() {
            if (window.confirm("确定要删除授权矩阵吗？这将删除所有数据。"))
                $get("deleteMatrixBtn").click();

            return false;
        }

        function onExportMatrix() {
            if (checkHasDeltaData()) {
                alert("矩阵正在编辑，请先保存！");
                return false;
            }
            
            var roleAsPerson = $get("chkRoleAsPerson").checked;
            var format = "xml";
            if (document.getElementsByName('RaidoOpMode')[1].checked) {
                format = "xlsx";
            }
            var url = "ExportMatrix.aspx?cmd=ExportMatrix&key=" + $get("matrixID").value + "&roleAsPerson=" + roleAsPerson + "&format=" + format;

            window.open(url, "_blank");
        }

        function onEditMatrix() {
            var roleAsPerson = $get("chkRoleAsPerson").checked;
            var url = "EditMatrix.aspx?processkey=" + $get("processKey").value + "&key=" + $get("matrixID").value + "&roleAsPerson=" + roleAsPerson;

            var feature = "dialogWidth:600px; dialogHeight:500px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            window.open(url, "newWindow", feature);
            //window.showModelessDialog(url, "", feature);
            //var result = window.showModalDialog(url, "", feature);

           // window.showDialog(url, "", feature);
            //window.open(url, "_blank");
        }

        function onImportMatrix() {
            if (checkHasDeltaData()) {
                alert("矩阵正在编辑，请先保存！");
                return false;
            }
            $find("uploadProgress").showDialog();
        }
        
        function matrixFileOpen() {
            $get("btnSaveMatrix").disabled = false;
        }
        
        function checkMatrixFile() {
            $find("materialCtrlForMatrix")._materials[0].click();
        }

        function checkHasDeltaData() {
            var result = $get("btnSaveMatrix").disabled != true;
            return result;
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
        <asp:Button runat="server" ID="bindMatrixBtn" Text="BindMatrix" Style="display: none"
            OnClick="bindMatrixBtn_Click" />
        <asp:Button runat="server" ID="deleteMatrixBtn" Text="DeleteMatrix" Style="display: none"
            OnClick="deleteMatrixBtn_Click" />
        <input id="selectedMatrix" runat="server" type="hidden" />
        <input id="matrixID" runat="server" type="hidden" />
        <input id="processKey" runat="server" type="hidden" />
    </div>
    <table width="100%" style="height: 100%; width: 100%">
        <tr>
            <td class="gridHead">
                <div class="dialogTitle">
                    <span class="dialogLogo">权限矩阵</span>
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
                                    <div runat="server" style="display: inline">
                                        当前没有授权矩阵信息，如需要添加，请选择</div>
                                    <a href="#" class="link" onclick="onSelectMatrixDefinition();">矩阵定义...</a>
                                </div>
                                <div id="martrixInfoContainer" runat="server" visible="false" class="matrixInfoContainer">
                                    <div id="matrixInfo" runat="server" style="display: inline">
                                    </div>
                                    <a href="#" class="link" onclick="onDeleteMatrix();">删除...</a>
                                </div>
                                <div>
                                    <p>
                                        &nbsp;&nbsp;&nbsp;&nbsp;<input id="chkRoleAsPerson" runat="server" type="checkbox" /><label style="cursor: pointer"
                                            for="chkRoleAsPerson">是否将角色显示为人员</label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;格式：
                                        <input type="radio" id="RaidoOpMode0" runat="server" name="RaidoOpMode" value="0" /><label
                                            for="RaidoOpMode0">Xml</label>
                                        <input type="radio" id="RaidoOpMode1" runat="server" name="RaidoOpMode" value="1" checked/><label
                                            for="RaidoOpMode1">Xlsx</label>
                                    </p>
                                    <div>
                                        	<div style="float: left">
                                        <input type="button" id="exportBtn" class="formButton" value="导出..." runat="server"
                                            disabled="disabled" onclick="onExportMatrix();" />
                                        <input type="button" id="importBtn" class="formButton" value="导入..." runat="server"
                                            disabled="disabled" onclick="onImportMatrix();" />
                                        <input disabled="disabled" type="button" id="btnDeleteMatrix" class="formButton" value="删除"
													runat="server" onserverclick="btnDeleteMatrix_Click" /></div>
                                       <div id="materialCtrlContainer" runat="server"  style="float:left">
                                        <div style="float: left;padding-top: 5px">
												<mcs:MaterialControl ID="materialCtrlForMatrix" MaterialUseMode="SingleDraft" TemplateUrl="~/MatrixModalDialog/Templates/EditRowTemplate.xlsx"
													DraftText="编辑矩阵" MaterialTitle="矩阵" RootPathName="GenericProcess" OnDocumentOpen="matrixFileOpen"
													OnPrepareDownloadStream="materialCtrl_PrepareDownloadStream" runat="server">
												</mcs:MaterialControl></div>
											<div style="float: left">
												<input disabled="disabled" type="button" id="btnSaveMatrix" class="formButton" value="保存"
													runat="server" onserverclick="btnSaveMatrix_Click" />
											</div></div>
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
