<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NetworkCredentialList.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.NetworkCredentialList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>网络凭据列表</title>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <META HTTP-EQUIV="Cache-Control" CONTENT="no-cache">
    <META HTTP-EQUIV="Expires" CONTENT="0">
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>
    <script type="text/javascript" src="../js/common.js"></script>
    <script type="text/javascript" src="../js/wfweb.js"></script>
    <base target="_self"/>
    
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function addCredential() {
            var sFeature = "dialogWidth:400px; dialogHeight:260px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var result;
            result = window.showModalDialog("NetworkCredentialEditor.aspx", null, sFeature);
            if (result) {
                document.getElementById("btnRefresh").click();
            }
        }

        function modifyCredential(key) {
            var sFeature = "dialogWidth:400px; dialogHeight:260px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            result = window.showModalDialog(String.format("NetworkCredentialEditor.aspx?key={0}", escape(key)), null, sFeature);
            if (result) {
                document.getElementById("btnRefresh").click();
            }
        }

        function onDeleteClick() {
            var selectedKeys = $find("CredentialDeluxeGrid").get_clientSelectedKeys();
            if (selectedKeys.length <= 0) {
                alert("请选择要删除的凭据！");
                return false;
            }
            var msg = "您确定要删除吗？";
            if (confirm(msg) == true) {
                document.getElementById("btnConfirm").click();
            }

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="display:none">
    <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" />
    </div>
    <a id="reload" href="NetworkCredentialList.aspx" style="display: none"></a>
    <table style="width: 100%; height: 100%; vertical-align: top;">
        <tr>
            <td class="gridHead">
                <div class="dialogTitle">
                    <span class="dialogLogo">网络凭据列表</span>
                </div>
            </td>
        </tr>
        <tr>
            <td style="height: 30px; background-color: #C0C0C0">
                <a href="#" onclick="addCredential();">
                    <img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" />
                </a><a href="#" onclick="onDeleteClick();">
                    <img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除" border="0" /></a>
                    <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
								RelativeControlID="btnDelete" PopupCaption="正在删除..." />
                
            </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%; height: 100%; vertical-align: top;">
                <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
                    height: 100%; overflow: auto">
                    <MCS:DeluxeGrid ID="CredentialDeluxeGrid" runat="server" AutoGenerateColumns="False"  OnPageIndexChanging="CredentialDeluxeGrid_PageIndexChanging"
                        DataSourceMaxRow="0" AllowPaging="True" PageSize="10" Width="100%"
                        DataKeyNames="Key" ExportingDeluxeGrid="False" GridTitle="Test" CssClass="dataList"
                        ShowExportControl="False" ShowCheckBoxes="True" OnRowDataBound="CredentialDeluxeGrid_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="Key" HeaderText="凭据Key" SortExpression="" />
                            <asp:BoundField DataField="LogOnName" HeaderText="登录名" SortExpression="" />
                        </Columns>
                        <PagerStyle CssClass="pager" />
                        <RowStyle CssClass="item" />
                        <CheckBoxTemplateItemStyle CssClass="checkbox" />
                        <CheckBoxTemplateHeaderStyle CssClass="checkbox" />
                        <HeaderStyle CssClass="head" />
                        <AlternatingRowStyle CssClass="aitem" />
                        <EmptyDataTemplate>
                            暂时没有您需要的数据
                        </EmptyDataTemplate>
                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                            NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
                    </MCS:DeluxeGrid>
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
                            <input type="button" class="formButton" onclick="top.close();" value="确定(O)" id="btnOK"
                                accesskey="O" />
                        </td>
                        <td style="text-align: center;">
                            <input type="button" class="formButton" onclick="top.close();" value="取消(C)" id="btnCancel"
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
