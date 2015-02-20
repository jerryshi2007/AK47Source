<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobList.aspx.cs" Inherits="WorkflowDesigner.PlanScheduleDialog.JobList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>作业列表</title>
    <base target="_self" />
    <link href="../css/dlg.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function onClick() {
            var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var result;
            result = window.showModalDialog("JobEditor.aspx", null, sFeature);

            if (result) {
                document.getElementById("hiddenServerBtn").click();
            }
        }

        function onOKBtnClick() {
            var selectedKeys = $find("JobDeluxeGrid").get_clientSelectedKeys();

            if (selectedKeys.length > 0)
                window.returnValue = selectedKeys[0];

            top.close();
        }

        function openModalDialog(jobId) {
            var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var result;
            var address = String.format("JobEditor.aspx?jobId={0}", jobId);
            result = window.showModalDialog(address, null, sFeature);

            if (result) {
                document.getElementById("hiddenServerBtn").click();
            }
            return false;
        }


        function onDeleteClick() {
            var selectedKeys = $find("JobDeluxeGrid").get_clientSelectedKeys();
            if (selectedKeys.length <= 0) {
                alert("请选择要删除的任务定义！");
                return false;
            }
            var msg = "您确定要删除吗？";
            if (confirm(msg) == true) {
                document.getElementById("btnConfirm").click();
            }
        }

        function onSearchClick() {
            var result = verifyTime("dc_LastExecuteStartDate", "dc_LastExecuteEndDate");
            if (result == false)
                alert("开始日期比较不合法");
            else
                document.getElementById("SubmitbtnSearch").click();
        }

        //验证日期起始时间小于结束时间
        function verifyTime(beginTimeControlID, endTimeControlID) {
            var begin = document.getElementById(beginTimeControlID).value;
            var end = document.getElementById(endTimeControlID).value;
            var result = true;
            //有一个为空就不用验证
            if ((begin != "") && (end != "")) {
                var arrBegin = begin.split("-");
                var arrEnd = end.split("-");
                //构造Date对象
                var dateBegin = new Date(arrBegin[0], arrBegin[1] - 1, arrBegin[2]);
                var dateEnd = new Date(arrEnd[0], arrEnd[1] - 1, arrEnd[2]);

                if (dateBegin > dateEnd) {
                    result = false;
                }
            }

            return result;
        }
    </script>
</head>
<body class="pcdlg">
    <form id="form1" runat="server">
    <div style="display: none">
        <asp:ScriptManager runat="server" ID="ScriptManager" EnableScriptGlobalization="true">
        </asp:ScriptManager>
        <SOA:DataBindingControl runat="server" ID="bindingControl">
            <ItemBindings>
                <SOA:DataBindingItem ControlID="text_Name" ControlPropertyName="Text" Direction="Both"
                    DataPropertyName="Name" />
                <SOA:DataBindingItem ControlID="dr_JobType" ControlPropertyName="SelectedValue" Direction="Both"
                    DataPropertyName="JobType" />
                <SOA:DataBindingItem ControlID="dc_LastExecuteStartDate" ControlPropertyName="Value"
                    Direction="Both" DataPropertyName="LastExecuteStartTime" />
                <SOA:DataBindingItem ControlID="dc_LastExecuteEndDate" ControlPropertyName="Value"
                    Direction="Both" DataPropertyName="LastExecuteEndTime" />
            </ItemBindings>
        </SOA:DataBindingControl>
    </div>
    <div class="pcdlg-sky">
        <div class="gridHead" style="line-height: 28px">
            <div class="dialogTitle">
                <span class="dialogLogo">作业列表</span>
            </div>
        </div>
    </div>
    <div class="pcdlg-content">
        <div style="padding: 5px;">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <th>
                        作业名称：
                    </th>
                    <td>
                        <SOA:HBTextBox ID="text_Name" runat="server" Height="20px" Width="200px" />
                    </td>
                    <th>
                        作业类型：
                    </th>
                    <td>
                        <SOA:HBDropDownList ID="dr_JobType" runat="server" AppendDataBoundItems="True" DataSourceID="dsJobType"
                            DataTextField="Description" DataValueField="EnumValue" SelectedText="全部">
                            <asp:ListItem Text="全部" Selected="True" Value="" />
                        </SOA:HBDropDownList>
                        <asp:ObjectDataSource runat="server" ID="dsJobType" SelectMethod="GetJobTypeSource"
                            TypeName="WorkflowDesigner.PlanScheduleDialog.JobList" />
                    </td>
                    <th>
                        上次执行时间：
                    </th>
                    <td>
                        <MCS:DeluxeCalendar ID="dc_LastExecuteStartDate" runat="server" Width="70px">
                        </MCS:DeluxeCalendar>至
                        <MCS:DeluxeCalendar ID="dc_LastExecuteEndDate" runat="server" Width="70px">
                        </MCS:DeluxeCalendar>
                    </td>
                    <td>
                        <input type="button" class="formButton" onclick="onSearchClick();" runat="server"
                            value="查询(S)" id="btnSearch" accesskey="S" />
                        <SOA:SubmitButton runat="server" ID="SubmitbtnSearch" Style="display: none" OnClick="btnSearch_Click"
                            RelativeControlID="btnSearch" PopupCaption="正在查询..." />
                    </td>
                </tr>
            </table>
        </div>
        <div style="height: 30px; background-color: #C0C0C0">
            <a href="javascript:void(0);" onclick="onClick();">
                <img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" />
            </a><a href="#" onclick="onDeleteClick();">
                <img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除" border="0" /></a>
            <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
                RelativeControlID="btnDelete" PopupCaption="正在删除..." />
        </div>
        <div style="position: static; zoom: 1">
            <MCS:DeluxeGrid ID="JobDeluxeGrid" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource"
                DataSourceMaxRow="0" AllowPaging="True" PageSize="10" Width="100%" DataKeyNames="JobID"
                ExportingDeluxeGrid="False" GridTitle="Test" CssClass="dataList" ShowExportControl="False"
                ShowCheckBoxes="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField HeaderText="作业名称" SortExpression="JOB_NAME">
                        <ItemTemplate>
                            <a onclick="openModalDialog('<%#Server.UrlEncode((string)Eval("JobID"))%>')" href="#">
                                <%#(string)Eval("Name")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Description" HeaderText="作业描述" ItemStyle-HorizontalAlign="Center"
                        SortExpression="DESCRIPTION">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="作业类型" HeaderStyle-Width="140" SortExpression="JOB_TYPE">
                        <ItemTemplate>
                            <%#MCS.Library.Core.EnumItemDescriptionAttribute.GetDescription((MCS.Library.SOA.DataObjects.JobType)Eval("JobType"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="是否启用" HeaderStyle-Width="100" SortExpression="ENABLED">
                        <ItemTemplate>
                            <%# ConvertBoolToCN((bool)Eval("Enabled"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LastStartExecuteTime" HeaderText="上次执行时间" HeaderStyle-Width="170"
                        DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" SortExpression="LAST_START_EXE_TIME" />
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
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" class="formButton" onclick="onOKBtnClick();" runat="server"
                value="确定(O)" id="btnOK" accesskey="O" visible="false" /><input type="button" runat="server"
                    class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel" accesskey="C"
                    visible="false" /></div>
    </div>
    <asp:ObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True" SelectCountMethod="GetQueryCount"
        SelectMethod="Query" SortParameterName="orderBy" OnSelecting="objectDataSource_Selecting"
        OnSelected="objectDataSource_Selected" TypeName="WorkflowDesigner.PlanScheduleDialog.JobQuery"
        EnableViewState="False">
        <SelectParameters>
            <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                Type="String" />
            <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <input runat="server" type="hidden" id="whereCondition" />
    <div style="display: none">
        <asp:Button ID="hiddenServerBtn" runat="server" Text="Button" OnClick="hiddenServerBtn_Click" /></div>
    </form>
</body>
</html>
