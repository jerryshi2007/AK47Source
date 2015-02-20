<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScheduleList.aspx.cs" Inherits="WorkflowDesigner.PlanScheduleDialog.ScheduleList" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>计划列表</title>
    <base target="_self" />
    <link href="../css/dlg.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .checkfield
        {
            width: 12px;
            text-align: left;
        }
        
        .searchTbl
        {
            font-weight: normal;
        }
        
        .searchTbl .DisplayName
        {
            background-color: #F2F2F2;
            text-align: center;
        }
        
        .searchTbl .Value
        {
            text-align: left;
        }
    </style>
    <script type="text/javascript">

        function onClick() {
            var sFeature = "dialogWidth:500px; dialogHeight:450px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            result = window.showModalDialog("ScheduleEditor.aspx", null, sFeature);

            if (result) {
                document.getElementById("hiddenServerBtn").click();
            }

        }

        function openModalDialog(scheduleId) {
            var sFeature = "dialogWidth:500px; dialogHeight:450px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            var address = String.format("ScheduleEditor.aspx?scheduleId={0}", scheduleId);
            result = window.showModalDialog(address, null, sFeature);

            if (result) {
                document.getElementById("hiddenServerBtn").click();
            }
            return false;
        }

        function onOKBtnClick() {
            var selectedData = $find("gridMain").get_clientSelectedKeys();
            if (selectedData != null && selectedData.length) {
                PageMethods.QuerySchedulesJson(selectedData, function (ok) {
                    window.returnValue = { jsonStr: ok };
                    top.close();
                }, function (err) {
                    $showError(err);
                })
            }
        }

        function onDeleteClick() {
            var rs = false;

            var selectedData = $find("gridMain").get_clientSelectedKeys();
            if (selectedData.length) {
                if (window.confirm("确实要删除吗？")) {
                    $get("hiddenSelectedSchedule").value = selectedData.join(",");
                    document.getElementById("btnConfirm").click();
                }
            } else {
                alert("没有选择要删除的计划！");
            }

            return rs;
        }

        function onSearchClick() {
            var result = verifyTime("dc_StartDate", "dc_StartDateEndDate");
            if (result == false)
                alert("开始日期比较不合法");

            result = verifyTime("dc_EndTimeStartDate", "dc_EndTime");

            if (result == false)
                alert("结束日期比较不合法");
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
        <asp:ScriptManager runat="server" ID="ScriptManager" EnableScriptGlobalization="true"
            EnablePageMethods="true">
        </asp:ScriptManager>
        <SOA:DataBindingControl runat="server" ID="bindingControl" AutoBinding="true" AutoCollectDataWhenPostBack="false">
            <ItemBindings>
                <SOA:DataBindingItem ControlID="text_Name" ControlPropertyName="Text" Direction="Both"
                    DataPropertyName="Name" />
                <%--	<SOA:DataBindingItem ControlID="cb_Enabled" ControlPropertyName="Checked" Direction="Both"
					DataPropertyName="Enabled" />--%>
                <SOA:DataBindingItem ControlID="dc_StartDate" ControlPropertyName="Value" Direction="Both"
                    DataPropertyName="StartDate" />
                <SOA:DataBindingItem ControlID="dc_StartDateEndDate" ControlPropertyName="Value"
                    Direction="Both" DataPropertyName="StartDateEndDate" />
                <SOA:DataBindingItem ControlID="dc_EndTimeStartDate" ControlPropertyName="Value"
                    Direction="Both" DataPropertyName="EndTimeStartDate" />
                <SOA:DataBindingItem ControlID="dc_EndTime" ControlPropertyName="Value" Direction="Both"
                    DataPropertyName="EndTime" />
            </ItemBindings>
        </SOA:DataBindingControl>
    </div>
    <div class="pcdlg-sky">
        <div class="gridHead" style="line-height: 28px">
            <div class="dialogTitle">
                <span class="dialogLogo">计划列表</span>
            </div>
        </div>
    </div>
    <div class="pcdlg-content">
        <div>
            <table class="searchTbl">
                <tr>
                    <th class="DisplayName" rowspan="2">
                        计划名称：
                    </th>
                    <td class="Value" rowspan="2">
                        <SOA:HBTextBox ID="text_Name" runat="server" />
                    </td>
                    <th class="DisplayName">
                        开始时间：
                    </th>
                    <td class="Value" style="text-align: left;">
                        <MCS:DeluxeCalendar ID="dc_StartDate" runat="server">
                        </MCS:DeluxeCalendar>至
                        <MCS:DeluxeCalendar ID="dc_StartDateEndDate" runat="server">
                        </MCS:DeluxeCalendar>
                    </td>
                    <td rowspan="2">
                        <input type="button" class="formButton" onclick="onSearchClick();" runat="server"
                            value="查询(S)" id="btnSearch" accesskey="S" />
                        <SOA:SubmitButton runat="server" ID="SubmitbtnSearch" Style="display: none" OnClick="btnSearch_Click"
                            RelativeControlID="btnSearch" PopupCaption="正在查询..." />
                    </td>
                </tr>
                <tr>
                    <th class="DisplayName">
                        结束时间：
                    </th>
                    <td class="Value">
                        <MCS:DeluxeCalendar ID="dc_EndTimeStartDate" runat="server">
                        </MCS:DeluxeCalendar>至
                        <MCS:DeluxeCalendar ID="dc_EndTime" runat="server">
                        </MCS:DeluxeCalendar>
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
        <div style="position: static; zoom: 1;">
            <div>
                <MCS:DeluxeGrid runat="server" ID="gridMain" Width="100%" AllowPaging="True" AllowSorting="True"
                    CascadeControlID="" DataSourceID="dataSourceMain" DataSourceMaxRow="0" GridTitle="计划列表"
                    TitleColor="141, 143, 149" TitleFontSize="Large" ShowCheckBoxes="True" DataKeyNames="ID"
                    AutoGenerateColumns="False" EnableViewState="False">
                    <AlternatingRowStyle CssClass="aitem" />
                    <RowStyle CssClass="item" />
                    <CheckBoxTemplateHeaderStyle CssClass="checkfield" />
                    <Columns>
                        <asp:TemplateField HeaderText="计划名称" SortExpression="SCHEDULE_NAME">
                            <ItemTemplate>
                                <a href='javascript:void(0)' onclick="openModalDialog('<%#Eval("ID") %>')" title="点击进行编辑">
                                    <%# Server.HtmlEncode((string)Eval("Name")) %></a>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="启用状态" SortExpression="ENABLED">
                            <ItemTemplate>
                                <%# (("1").Equals (Eval("Enabled")))?"√":"×" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="开始时间" DataField="StartTime" DataFormatString="{0:yyyy-MM-dd}"
                            SortExpression="START_TIME" ReadOnly="true" />
                        <asp:BoundField HeaderText="结束时间" DataField="EndTime" DataFormatString="{0:yyyy-MM-dd}"
                            SortExpression="END_TIME" ReadOnly="true" />
                    </Columns>
                    <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                        NextPageText="下一页" Position="TopAndBottom" PreviousPageText="上一页"></PagerSettings>
                    <SelectedRowStyle CssClass="selecteditem" />
                    <PagerStyle CssClass="pager" />
                </MCS:DeluxeGrid>
                <SOA:DeluxeObjectDataSource ID="dataSourceMain" runat="server" TypeName="WorkflowDesigner.PlanScheduleDialog.ScheduleDataSource"
                    OnSelecting="DeluxeObjectDataSource1_Selecting" EnablePaging="True">
                </SOA:DeluxeObjectDataSource>
            </div>
            <div style="display: none">
            </div>
        </div>
        <div id="trBottom" runat="server">
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" class="formButton" onclick="onOKBtnClick();" runat="server"
                value="确定(O)" id="btnOK" accesskey="O" visible="false" />
            <input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel"
                runat="server" accesskey="C" visible="false" />
        </div>
        <div>
            <input runat="server" type="hidden" id="whereCondition" />
            <div style="display: none">
                <asp:Button ID="hiddenServerBtn" runat="server" Text="Button" /></div>
        </div>
        <div style="display: none">
            <asp:Button ID="Button2" runat="server" Text="Button" /></div>
        <asp:HiddenField ID="hiddenSelectedSchedule" runat="server" />
    </div>
    </form>
</body>
</html>
