<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfProcessDescriptorInformationList.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.WfProcessDescriptorInformationList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self">
    <title>流程模板定义列表</title>
    <script type="text/javascript">
        function importProcess() {
            var result = $find("uploadProgress").showDialog()
            if (result) {
                reload.click();
            }
        }

        function exportProcess() {
            var processDescInfoDeluxeGrid = $find("ProcessDescInfoDeluxeGrid");
            var selectedWfProcessKeys = processDescInfoDeluxeGrid ? processDescInfoDeluxeGrid.get_clientSelectedKeys() : [];
            if (selectedWfProcessKeys.length <= 0) {
                alert("请选择至少一个流程模板。");
                return;
            }

            $get("wfProcessKeys").value = selectedWfProcessKeys.join(",");
            var url = 'ExportWfProcesses.aspx';

            //zip mode
            if (document.getElementsByName('RaidoOpMode')[1].checked) {
                url = 'ExportWfProcessAsZip.ashx';
            }

            $get("exportProcessForm").action = url;
            $get("exportProcessForm").submit();
        }

        function onLogClick(resourceID) {
            var strLink = "/MCSWebApp/OACommonPages/UserOperationLog/UserOperationLogView.aspx?resourceID=" + resourceID;
            window.showModalDialog(strLink, "", "dialogHeight: 500px; dialogWidth: 670px; resizable:yes; edge: Raised; center: Yes; help: No; status: No;scroll: No;");
        }

        function getSimulationWindowFeature() {
            var width = 900;
            var height = 680;

            var left = (window.screen.width - width) / 2;
            var top = (window.screen.height - height) / 2;

            return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
        }

        function onSimulationClick() {
            event.returnValue = false;
            window.open(event.srcElement.href, "wfSimulation", getSimulationWindowFeature());

            return false;
        }
    </script>
    <style type="text/css">
        .searchTbl {
            border: 0px solid lightGrey;
            border-collapse: collapse;
        }

            .searchTbl th {
                border: lightgrey 1px solid;
            }

            .searchTbl .head {
                border: lightgrey 1px solid;
                font-size: 13px;
                color: Black;
                font-weight: normal;
                text-align: center;
                height: 24px;
                background-color: #F2F2F2;
            }

            .searchTbl td {
                border: 1px solid lightGrey;
            }
    </style>
</head>
<body>
    <form id="exportProcessForm" name="exportProcessForm" target="_parent" method="post">
        <input type="hidden" id="wfProcessKeys" name="wfProcessKeys" />
    </form>
    <form id="serverForm" runat="server" defaultbutton="btnSearch">
        <asp:ScriptManager ID="ScriptManager1" EnableScriptGlobalization="true" runat="server">
        </asp:ScriptManager>
        <div>
            <input runat="server" type="hidden" id="resultData" />
        </div>
        <table style="height: 100%; width: 100%">
            <tr>
                <td class="gridHead">
                    <div class="dialogTitle">
                        <span class="dialogLogo">流程模板定义列表</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 40px; text-align: center; vertical-align: middle">
                    <table style="width: 100%; height: 100%">
                        <tr>
                            <td colspan="3">导入/导出格式：
							<input type="radio" id="RaidoOpMode0" name="RaidoOpMode" value="0" title="导出时请将页面“另存为”xml格式文件，或者通过查看页面源代码方式复制导出的信息。" /><label
                                for="RaidoOpMode0">Xml</label>
                                <input type="radio" id="RaidoOpMode1" name="RaidoOpMode" value="1" checked /><label
                                    for="RaidoOpMode1">Zip</label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center;">
                                <input type="button" onclick="importProcess();" value="导入(I)" id="btnImport" accesskey="I"
                                    class="formButton" />
                                <SOA:UploadProgressControl runat="server" ID="uploadProgress" DialogTitle="导入流程模板"
                                    OnDoUploadProgress="uploadProgress_DoUploadProgress" />
                            </td>
                            <td style="text-align: center;">
                                <input type="button" onclick="exportProcess();" value="导出(E)" id="btnExport" accesskey="E"
                                    class="formButton" />
                            </td>
                            <td style="text-align: center;">
                                <input type="button" onclick="onDeleteClick();" value="删除(D)" id="btnDelete" accesskey="D"
                                    runat="server" class="formButton" />
                                <SOA:SubmitButton runat="server" ID="btnDeleteConfirm" Style="display: none" OnClick="btnDelete_Click"
                                    RelativeControlID="btnDelete" PopupCaption="正在删除..." />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="height: 5px; text-align: left; vertical-align: middle">
                <td>
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="height: 60px; text-align: left; vertical-align: middle">
                    <div>
                        <SOA:SimpleTabStrip ID="tabStrip" runat="server" SelectedKey="likePanel">
                            <TabStrips>
                                <SOA:TabStripItem Key="likePanel" Text="查询" ControlID="likePanel" />
                                <SOA:TabStripItem Key="wherePanel" Text="精确查询" ControlID="wherePanel" />
                            </TabStrips>
                        </SOA:SimpleTabStrip>
                    </div>
                    <div style="display: none">
                        <div id="likePanel" runat="server">
                            <table class="searchTbl" style="width: 90%" cellpadding="0" cellspacing="0">
                                <tr class="head">
                                    <th scope="col" style="width: 180px">应用名称
                                    </th>
                                    <th scope="col">模块名称
                                    </th>
                                    <th scope="col">流程模板Key
                                    </th>
                                    <th scope="col">流程模板名称
                                    </th>
                                    <th scope="col">启用
                                    </th>
                                    <th></th>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtApplicationName" runat="server" Width="160" />
                                        <MCS:TextBoxDropdownExtender ID="dropdownExtender" runat="server" TargetControlID="txtApplicationName"
                                            Height="140px" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtProgramName" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtProcessKey" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtProcessName" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlEnabled" runat="server">
                                            <asp:ListItem Value="">请选择</asp:ListItem>
                                            <asp:ListItem Value="1">是</asp:ListItem>
                                            <asp:ListItem Value="0">否</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td rowspan="2">
                                        <asp:Button ID="btnSearch" runat="server" Text="查询" class="formButton" OnClick="btnSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="wherePanel" runat="server">
                            <table class="searchTbl" style="width: 90%" cellpadding="0" cellspacing="0">
                                <tr class="head">
                                    <th scope="col" style="width: 100%" colspan="2">请输入流程key，并以逗号隔开
                                    </th>
                                </tr>
                                <tr>
                                    <td style="width: 90%">
                                        <asp:TextBox runat="server" ID="tb_AllKeys" Width="100%"></asp:TextBox>
                                    </td>
                                    <td style="width: 10%">
                                        <asp:Button ID="Button1" runat="server" Text="查询" class="formButton" OnClick="btnSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
            <tr style="height: 5px; text-align: left; vertical-align: middle">
                <td>
                    <hr />
                </td>
            </tr>
            <%--<tr>
			<td style="height: 60px; text-align: left; vertical-align: middle">
				<table class="searchTbl" style="width: 90%" cellpadding="0" cellspacing="0">
					<tr class="head">
						<td colspan="3">
							精确查找
						</td>
					</tr>
					<tr>
						<td style="width: 85%">
							<asp:TextBox ID="TB_processKeys" runat="server" Style="width: 100%"></asp:TextBox>
						</td>
						<td style="width: 15%">
							<asp:Button ID="Button1" runat="server" Text="精确查找" class="formButton" OnClick="btnSearch_Click" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr style="height: 5px; text-align: left; vertical-align: middle">
			<td>
				<hr />
			</td>
		</tr>--%>
            <tr>
                <td style="vertical-align: top">
                    <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%; height: 100%; overflow: auto">
                        <MCS:DeluxeGrid ID="ProcessDescInfoDeluxeGrid" runat="server" AutoGenerateColumns="False"
                            DataSourceID="ObjectDataSource" DataSourceMaxRow="0" AllowPaging="True" PageSize="10"
                            AllowSorting="true" Width="100%" DataKeyNames="ProcessKey" ExportingDeluxeGrid="False"
                            GridTitle="流程选择" CssClass="dataList" ShowExportControl="False" ShowCheckBoxes="True"
                            OnRowDataBound="ProcessDescInfoDeluxeGrid_RowDataBound">
                            <Columns>
                                <asp:TemplateField SortExpression="PROCESS_KEY" HeaderText="流程模板Key">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <div style="padding-left: 8px">
                                            <%#Server.HtmlEncode((string)Eval("ProcessKey"))%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ApplicationName" HeaderText="应用名称" SortExpression="APPLICATION_NAME"
                                    ItemStyle-HorizontalAlign="Center">
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="ProgramName" HeaderText="模块名称" SortExpression="PROGRAM_NAME" />
                                <asp:BoundField DataField="ProcessName" HeaderText="模板名称" SortExpression="PROCESS_NAME" />
                                <asp:BoundField DataField="CreateTime" HeaderText="创建时间" HtmlEncode="False" SortExpression="CREATE_TIME"
                                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="70" />
                                <asp:BoundField DataField="ModifyTime" HeaderText="修改时间" HtmlEncode="False" SortExpression="MODIFY_TIME"
                                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="80" />
                                <asp:BoundField DataField="ImportTime" HeaderText="上传时间" HtmlEncode="False" SortExpression="IMPORT_TIME"
                                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="80" />
                                <asp:TemplateField>
                                    <ItemStyle Width="48" />
                                    <HeaderTemplate>
                                        仿真
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <a onclick="onSimulationClick();" target="wfSimulation" href='../Simulation/WorkflowSimulation.aspx?processDescKey=<%#Server.UrlEncode((string)Eval("ProcessKey"))%>'>仿真</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Width="48" />
                                    <HeaderTemplate>
                                        日志
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <a href="#" onclick="onLogClick('<%#Eval("ProcessKey")%>')">查看</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
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
                <td class="gridfileBottom"></td>
            </tr>
            <tr>
                <td style="height: 40px; text-align: center; vertical-align: middle">
                    <table style="width: 100%; height: 100%">
                        <tr>
                            <td style="text-align: center;">
                                <input type="button" onclick="onClick();" value="确定(O)" id="btnOK" accesskey="O"
                                    runat="server" class="formButton" />
                                <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
                                    RelativeControlID="btnOK" PopupCaption="正在读取..." />
                            </td>
                            <td style="text-align: center;">
                                <input type="button" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"
                                    class="formButton" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
            SelectMethod="Query" SortParameterName="orderBy" OnSelecting="objectDataSource_Selecting"
            OnSelected="objectDataSource_Selected" TypeName="WorkflowDesigner.ModalDialog.ProcessDescriptorInfoQuery"
            EnableViewState="False">
            <SelectParameters>
                <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                    Type="String" />
                <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <input runat="server" type="hidden" id="whereCondition" />
        <div>
            &nbsp;
        </div>
        <a id="reload" href="WfProcessDescriptorInformationList.aspx" style="display: none"></a>
        <script type="text/javascript">
            function onClick() {
                var processDescInfoDeluxeGrid = $find("ProcessDescInfoDeluxeGrid");
                var selectedKeys = processDescInfoDeluxeGrid ? processDescInfoDeluxeGrid.get_clientSelectedKeys() : [];

                if (selectedKeys.length > 0) {
                    $get("btnConfirm").click();
                    //$get("btnOK").disabled = true;
                }
                else {
                    alert('请选择要打开的流程模板！');
                }
            }

            function onDeleteClick() {
                var processDescInfoDeluxeGrid = $find("ProcessDescInfoDeluxeGrid")
                var selectedKeys = processDescInfoDeluxeGrid ? processDescInfoDeluxeGrid.get_clientSelectedKeys() : [];
                if (selectedKeys.length == 0) {
                    alert('请先选择要删除的流程模板！');
                    return false;;
                }

                var msg = "您确定要删除吗？";
                if (confirm(msg) == true) {
                    $get("btnDeleteConfirm").click();
                    return true;
                } else {
                    return false;
                }
            }

            Sys.Application.add_load(function () {
                document.onkeydown = function () {
                    if (window.event && window.event.keyCode == 13) {
                        $get('btnSearch').click();
                        window.event.cancelBubble = true;
                        return false;
                    }
                }
            });
        </script>
    </form>
</body>
</html>
