<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessAdjustment.aspx.cs"
    Inherits="MCS.OA.CommonPages.AppTrace.ProcessAdjustment" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Import Namespace="MCS.Library.OGUPermission" %>
<%@ Import Namespace="MCS.Library.SOA.DataObjects" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程调整</title>
    <style type="text/css">
        .labelCell
        {
            text-align: right;
            background-color: #f2f8f8;
            height: 30px;
            width: 120px;
        }
        .inputCell
        {
            background-color: #f7fbfa;
            text-align: left;
            padding-left: 12px;
        }
    </style>
    <script type="text/javascript">
        function onCommandInput(commandInputControl, e) {
            switch (e.commandValue) {
                case "refreshUserTasks":
                case "refresh":
                    e.stopCommand = true; //设置后，不再执行默认的处理
                    $get("RefreshButton").click();
                    break;
            }
        }

        function buildProcessStartupParam(e, confirmationPrompt) {
            var processIDs = $find("dataGrid").get_clientSelectedKeys();

            if (processIDs.length == 0) {
                e.cancel = true;
                return e;
            }

            if (window.confirm(confirmationPrompt) == false) {
                e.cancel = true;
                return e;
            }

            e.steps = processIDs;

            return e;
        }

        function onBeforeCancelProcessStart(e) {
            return buildProcessStartupParam(e, "确认要作废流程吗？");
        }

        function onBeforeChangeAssigneesStart(e) {
            var processIDs = $find("dataGrid").get_clientSelectedKeys();

            if (processIDs.length == 0) {
                e.cancel = true;
                return e;
            }

            var userSelector = $find("userSelector");

            var result = userSelector.showDialog();

            if (result && result.users.length > 0) {
                var steps = new Array(processIDs.length);

                for (var i = 0; i < processIDs.length; i++) {
                    var step = { ProcessID: processIDs[i], Users: result.users };

                    steps[i] = Sys.Serialization.JavaScriptSerializer.serialize(step);
                }

                e.steps = steps;
            }
            else
                e.cancel = true;
        }

        function onBeforeRegen(e) {
            var processIDs = $find("dataGrid").get_clientSelectedKeys();

            if (processIDs.length == 0) {
                e.cancel = true;
            } else {
                e.steps = [];
                for (var i = processIDs.length - 1; i >= 0; i--) {
                    e.steps.push(processIDs[i]);
                }

                e.cancel = false;
            }
        }

        function onBeforeChangeCandidatesStart(e) {
            var processIDs = $find("dataGrid").get_clientSelectedKeys();

            if (processIDs.length == 0) {
                e.cancel = true;
                return e;
            }

            var feature = "dialogWidth:420px; dialogHeight:320px; center:yes; help:no; resizable:no;status:no;scroll:no";

            var result = window.showModalDialog("selectReplaceAssignees.aspx", null, feature);

            if (result) {
                var deserialized = Sys.Serialization.JavaScriptSerializer.deserialize(result);
                var steps = new Array(processIDs.length);

                for (var i = 0; i < processIDs.length; i++) {
                    var step = { ProcessID: processIDs[i], Users: [deserialized.targetUser], OriginalUser: deserialized.originalUser };

                    steps[i] = Sys.Serialization.JavaScriptSerializer.serialize(step);
                }

                e.steps = steps;
            }
            else
                e.cancel = true;
        }

        //完成处理
        function onFinished(e) {
            //检查处理结果
            if (!e.error || !e.error.message)
                alert("处理完成");
            else {
                $showError(e.error.message);
            }

            //处理完成后，不论成功与否，均主动执行一遍查询
            $get("RefreshButton").click();
        }

        function rangeAction() {
            window.showModalDialog("TimeRange.aspx", null, "dialogWidth:820px; dialogHeight:420px; center:yes; help:no; resizable:no;status:no;scroll:no");
        }

        function getDefaultTaskFeature() {
            var width = 820;
            var height = 700;

            var left = (window.screen.width - width) / 2;
            var top = (window.screen.height - height) / 2;

            return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
        }

        function onLinkClick() {
            event.returnValue = false;
            var a = event.srcElement;

            var feature = getDefaultTaskFeature();

            window.open(a.href, "_blank", feature);
            event.cancelBubble = true;

            return false;
        }
    </script>
</head>
<body style="background-color: #f8f8f8;">
    <form id="serverForm" runat="server">
    <div>
        <soa:databindingcontrol runat="server" id="bindingControl">
            <ItemBindings>
                <SOA:DataBindingItem ControlID="ApplicationName" DataPropertyName="ApplicationName" />
                <SOA:DataBindingItem ControlID="ProcessName" DataPropertyName="ProcessName" />
                <SOA:DataBindingItem ControlID="DepartmentName" DataPropertyName="DepartmentName" />
                <SOA:DataBindingItem ControlID="StartDate" ControlPropertyName="Value" DataPropertyName="BeginStartTime" />
                <SOA:DataBindingItem ControlID="EndDate" ControlPropertyName="Value" DataPropertyName="EndStartTime" />
                <SOA:DataBindingItem ControlID="CurrentAssignees" DataPropertyName="CurrentAssignees"
                    ControlPropertyName="SelectedOuUserData" />
                <SOA:DataBindingItem ControlID="tb_AssigneesUserName" DataPropertyName="AssigneesUserName" />
                <SOA:DataBindingItem ControlID="RBAssigneesType" DataPropertyName="AssigneesSelectType"
                    ControlPropertyName="SelectedValue" />
                <SOA:DataBindingItem ControlID="RBProcessType" DataPropertyName="AssigneeExceptionFilterType"
                    ControlPropertyName="SelectedValue" />
                <SOA:DataBindingItem ControlID="processStatusSelector" DataPropertyName="ProcessStatus"
                    ControlPropertyName="SelectedValue" />
            </ItemBindings>
        </soa:databindingcontrol>
    </div>
    <div id="container">
        <div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;">
            <div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
                line-height: 30px; padding-bottom: 0px">
                流程调整
            </div>
        </div>
        <div style="margin-top: 7px; margin-bottom: 7px;" runat="server" id="divSearch">
            <table style="background-color: Silver; width: 100%; text-align: left" cellspacing="1px"
                border="0" cellpadding="0">
                <tr style="background-color: White">
                    <td id="tdFormCateogryLbl" runat="server" class="labelCell">
                        表单类别：
                    </td>
                    <td class="inputCell" style="width: 160px;">
                        <asp:TextBox runat="server" ID="ApplicationName" />
                    </td>
                    <td runat="server" class="labelCell">
                        流程名称：
                    </td>
                    <td class="inputCell" style="width: 160px;">
                        <asp:TextBox runat="server" ID="ProcessName" />
                    </td>
                    <td class="labelCell">
                        启动时间：
                    </td>
                    <td class="inputCell">
                        <mcs:deluxecalendar id="StartDate" runat="server" width="70px">
                        </mcs:deluxecalendar>
                        至
                        <mcs:deluxecalendar id="EndDate" runat="server" width="70px">
                        </mcs:deluxecalendar>
                    </td>
                    <td class="labelCell">
                        创建部门：
                    </td>
                    <td class="inputCell" style="width: 160px;">
                        <asp:TextBox runat="server" ID="DepartmentName" />
                    </td>
                </tr>
                <tr style="background-color: White">
                    <td id="td3" runat="server" class="labelCell">
                        环节操作人类型：
                    </td>
                    <td class="inputCell">
                        <soa:hbradiobuttonlist runat="server" id="RBAssigneesType" repeatdirection="Horizontal">
                            <asp:ListItem Text="仅当前环节" Value="0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="所有环节" Value="1"></asp:ListItem>
                        </soa:hbradiobuttonlist>
                    </td>
                    <td id="td1" runat="server" class="labelCell">
                        环节操作人：
                    </td>
                    <td class="inputCell">
                        <soa:ouuserinputcontrol id="CurrentAssignees" runat="server" multiselect="True" showdeletedobjects="False"
                            width="156px" selectmask="User" listmask="Organization,User" classname="inputStyle"
                            invokewithoutviewstate="true" />
                    </td>
                    <td id="Td2" runat="server" class="labelCell">
                        环节操作人名：
                    </td>
                    <td class="inputCell" style="width: 160px;">
                        <asp:TextBox runat="server" ID="tb_AssigneesUserName" />
                    </td>
                    <td runat="server" class="labelCell">
                        流程状态：
                    </td>
                    <td class="inputCell" style="width: 160px;">
                        <soa:hbdropdownlist runat="server" id="processStatusSelector">
                            <asp:ListItem Text="全部" Value="" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="运行中" Value="Running"></asp:ListItem>
                            <asp:ListItem Text="已完成" Value="Completed"></asp:ListItem>
                            <asp:ListItem Text="被终止" Value="Aborted"></asp:ListItem>
                            <asp:ListItem Text="未运行" Value="NotRunning"></asp:ListItem>
                        </soa:hbdropdownlist>
                    </td>
                </tr>
                <tr>
                    <td class="labelCell" style="">
                        筛选流程类型：
                    </td>
                    <td class="inputCell" colspan="3">
                        <soa:hbradiobuttonlist runat="server" id="RBProcessType" repeatdirection="Horizontal">
                            <asp:ListItem Text="全部" Value="0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="当前环节人员异常" Value="1"></asp:ListItem>
                            <asp:ListItem Text="当前环节和后续环节人员异常" Value="2"></asp:ListItem>
                        </soa:hbradiobuttonlist>
                    </td>
                    <td class="inputCell" style="text-align: center">
                        <asp:LinkButton runat="server" ID="queryBtn" OnClick="QueryBtnClick" CausesValidation="false">
						<img src="../../images/16/search.gif" alt="查询" style="border:0;"/>查询
                        </asp:LinkButton>
                    </td>
                    <td class="inputCell" style="text-align: center; padding-right: 16px;" colspan="3">
                        <input type="button" id="changeAssigneesBtn" runat="server" class="portalButton"
                            value="修改待办人" title="修改当前环节的待办人" />
                        <input type="button" id="changeCandidatesBtn" runat="server" class="portalButton"
                            value="修改候选人" title="修改当前环节的待办人和后续环节的候选人" />
                        <input type="button" id="cancelProcessBtn" runat="server" class="portalButton" value="作废流程" />
                        <input type="button" id="regenBtn" runat="server" class="portalButton" value="重算数据" />
                        <a href="javascript:void(0);" onclick="rangeAction()">时间范围重算</a>
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-top: 7px; margin-bottom: 7px;">
            <mcs:deluxegrid id="dataGrid" runat="server" showexportcontrol="true" pagesize="10"
                titlefontsize="Small" width="100%" datasourceid="objectDataSource" allowpaging="true"
                allowsorting="true" autogeneratecolumns="false" datakeynames="InstanceID" showcheckboxes="true"
                checkboxposition="Left" pagersettings-mode="NextPreviousFirstLast" pagersettings-previouspagetext="上一页"
                pagersettings-nextpagetext="下一页" pagersettings-position="TopAndBottom" captionalign="Right"
                cssclass="dataList" titlecssclass="title" gridtitle="流程调整">
                <Columns>
                    <asp:TemplateField HeaderText="流程类别" SortExpression="APPLICATION_NAME">
                        <ItemTemplate>
                            <%#HttpUtility.HtmlEncode((string)Eval("ApplicationName")) %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" Width="160px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="DescriptorKey" HeaderText="流程Key" SortExpression="DESCRIPTOR_KEY">
                        <ItemStyle HorizontalAlign="Left" Width="160px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="流程名称" SortExpression="PROCESS_NAME">
                        <ItemTemplate>
                            <div>
                                <a href='ApplicationBridgeForm.aspx?processID=<%#HttpUtility.UrlEncode((string)Eval("InstanceID"))%>'
                                    onclick="onLinkClick();">
                                    <%#HttpUtility.HtmlEncode((string)Eval("ProcessName")) %></a>
                            </div>
                            <div> <%# HttpUtility.HtmlEncode((string)Eval("ApplicationName"))  %> 
                            
                             <%# HttpUtility.HtmlEncode((string)Eval("ProgramName"))  %></div>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="创建者" SortExpression="CREATOR_NAME">
                        <ItemTemplate>
                            <SOA:UserPresence ID="userPresence" runat="server" UserID='<%# ((IOguObject)Eval("Creator")).ID %>'
                                UserDisplayName='<%# ((IOguObject)Eval("Creator")).DisplayName %>' />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" Width="120px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="创建部门" SortExpression="DEPARTMENT_NAME">
                        <ItemTemplate>
                            <%#HttpUtility.HtmlEncode(((IOguObject)Eval("Department")).ToDescription())%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" Width="120px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="StartTime" HeaderText="开始时间" HtmlEncode="False" SortExpression="START_TIME"
                        DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderStyle-Width="160px">
                        <ItemStyle HorizontalAlign="Center" Width="160px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="状态">
                        <HeaderStyle ForeColor="Black" />
                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                        <ItemTemplate>
                            <SOA:WfStatusControl ID="statusControl" runat="server" ProcessID='<%# Eval("InstanceID")%>'
                                DisplayMode="CurrentUsers" EnableUserPresence="true">
                            </SOA:WfStatusControl>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="head" />
                <RowStyle CssClass="item" />
                <AlternatingRowStyle CssClass="aitem" />
                <SelectedRowStyle CssClass="selecteditem" />
                <PagerStyle CssClass="pager" />
                <EmptyDataTemplate>
                    暂时没有您需要的数据
                </EmptyDataTemplate>
                <PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
                    PreviousPageText="上一页"></PagerSettings>
            </mcs:deluxegrid>
            <asp:ObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
                SelectMethod="Query" SortParameterName="orderBy" OnSelecting="objectDataSource_Selecting"
                OnSelected="objectDataSource_Selected" TypeName="MCS.OA.CommonPages.AppTrace.WfProcessCurrentInfoDataSource"
                EnableViewState="False">
                <SelectParameters>
                    <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                        Type="String" />
                    <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <input runat="server" type="hidden" id="whereCondition" />
            <asp:LinkButton ID="RefreshButton" runat="server" OnClick="RefreshButton_Click"></asp:LinkButton>
        </div>
    </div>
    <div>
        <soa:userselector runat="server" id="userSelector" listmask="Organization,User,Sideline"
            selectmask="User" dialogtitle="请选择待办人" />
        <soa:multiprocesscontrol runat="server" id="cancelProcesses" dialogtitle="正在作废流程..."
            controlidtoshowdialog="cancelProcessBtn" onclientbeforestart="onBeforeCancelProcessStart"
            onclientfinishedprocess="onFinished" onexecutestep="cancelProcess_ExecuteStep" />
        <soa:multiprocesscontrol runat="server" id="changeAssignees" dialogtitle="正在修改待办人..."
            controlidtoshowdialog="changeAssigneesBtn" onclientbeforestart="onBeforeChangeAssigneesStart"
            onclientfinishedprocess="onFinished" onexecutestep="changeAssignees_ExecuteStep" />
        <soa:multiprocesscontrol runat="server" id="regenProcesses" dialogtitle="正在生成数据..."
            showsteperrors="true" controlidtoshowdialog="regenBtn" onclientbeforestart="onBeforeRegen"
            onclientfinishedprocess="onFinished" onexecutestep="Regen_ExecuteStep" onerror="RegenProcesses_Error" />
        <soa:multiprocesscontrol runat="server" id="changeCandidates" dialogtitle="正在修改候选人..."
            controlidtoshowdialog="changeCandidatesBtn" onclientbeforestart="onBeforeChangeCandidatesStart"
            onclientfinishedprocess="onFinished" onexecutestep="changeCandidates_ExecuteStep" />
        <soa:commandinput id="CommandInputReceiver" runat="server" onclientcommandinput="onCommandInput">
        </soa:commandinput>
    </div>
    </form>
</body>
</html>
