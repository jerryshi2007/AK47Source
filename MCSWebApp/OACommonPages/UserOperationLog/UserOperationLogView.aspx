<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserOperationLogView.aspx.cs"
    Inherits="MCS.OA.CommonPages.UserOperationLog.UserOperationLogView" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <asp:Literal runat="server" ID="compatible"></asp:Literal>
    <%--<meta http-equiv="X-UA-Compatible" content="IE=7" />--%>
    <title runat="server" category="OACommons">日志查看</title>
    <base target="_self" />
    <script src="../JavaScript/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function onAdvancedSearchClick() {
            //            event.returnValue = false;

            //            var sFeature = "dialogHeight: 350px; dialogWidth: 500px; edge: Raised; center: Yes; help: No; status: No;scroll: No;";
            //            var arg = new Object();

            //            setControlValuesToObject(arg, "Operator", "HeadTitle", "ActivityName", "StartDate", "EndDate");

            //            var result = window.showModalDialog("AdvancedSearch.aspx", arg, sFeature);

            //            if (result) {
            //                setObjectToControlValues(result, "Operator", "HeadTitle", "ActivityName", "StartDate", "EndDate");

            //                //反序列化为人员信息数组
            //                var fromPersonData = Sys.Serialization.JavaScriptSerializer.deserialize(result["Operator"]);

            //                var personIDString = "";
            //                var orgIDString = "";

            //                //将人员信息数组中人员ID取出，用逗号分割，准备构造IN查询
            //                //将部门的ID取出，准备在服务端转换为部门中的人员ID
            //                if (fromPersonData) {
            //                    for (i = 0; i < fromPersonData.length; i++) {
            //                        if (fromPersonData[i].objectType == 1) {
            //                            if (orgIDString != "") {
            //                                orgIDString += ",";
            //                            }
            //                            orgIDString += fromPersonData[i].id;
            //                        }
            //                        else {
            //                            if (personIDString != "") {
            //                                personIDString += ",";
            //                            }
            //                            personIDString += "'";
            //                            personIDString += fromPersonData[i].id;
            //                            personIDString += "'";
            //                        }
            //                    }
            //                }

            //                //将ID串暂存在页面	    	
            //                $get("PersonID")["value"] = personIDString;
            //                $get("OrgID")["value"] = orgIDString;

            //                $get("ButtonAdvanced").click();

            //            }
        }

        function openwindow(id) {
            var strLink = "LogDetail.aspx?id=" + id + "&lang=" + $language;
            window.showModalDialog(strLink, "", "dialogHeight: 580px; dialogWidth: 680px;  edge: Raised; center: Yes; help: No; status: No;scroll: No;");
        }

        //function document.onkeydown() {
        //    //onEnterKeyDown("queryBtn");
        //}

        //验证日期起始时间小于结束时间
        function verifyTime(beginTimeID, endTimeID) {
            var begin = document.getElementById(beginTimeID).value;
            var end = document.getElementById(endTimeID).value;
            var result = true;
            //有一个为空就不用验证
            if ((begin != "") && (end != "")) {
                var arrBegin = begin.split("-");
                var arrEnd = end.split("-");
                //构造Date对象
                var dateBegin = new Date(arrBegin[0], arrBegin[1] - 1, arrBegin[2]);
                var dateEnd = new Date(arrEnd[0], arrEnd[1] - 1, arrEnd[2]);


                if (dateBegin > dateEnd) {
                    alert("起始时间不能大于结束时间");
                    result = false;
                }
            }

            return result;
        }
    </script>
</head>
<body style="background-color: #f8f8f8;">
    <form id="serverForm" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManager1" EnableScriptGlobalization="true">
        </asp:ScriptManager>
        <div id="container">
            <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
                <tr>
                    <td style="height: 48px">
                        <div>
                            <input type="hidden" id="PersonID" runat="server" />
                            <input type="hidden" id="OrgID" runat="server" />
                            <input type="hidden" id="Operator" runat="server" value="" />
                            <input type="hidden" id="HeadTitle" runat="server" value="" />
                            <input type="hidden" id="ActivityName" runat="server" value="" />
                            <input runat="server" type="hidden" id="whereCondition" />
                            <soa:DataBindingControl runat="server" ID="bindingControl">
                                <ItemBindings>
                                    <soa:DataBindingItem ControlID="HeadTitle" ControlPropertyName="Value" Direction="ControlToData"
                                        DataPropertyName="Title" />
                                    <soa:DataBindingItem ControlID="PersonID" ControlPropertyName="Value" Direction="ControlToData"
                                        DataPropertyName="Operator" />
                                    <soa:DataBindingItem ControlID="ActivityName" ControlPropertyName="Value" Direction="ControlToData"
                                        DataPropertyName="ActivityName" />
                                    <soa:DataBindingItem ControlID="StartDate" ControlPropertyName="Value" Direction="ControlToData"
                                        DataPropertyName="StartDate" />
                                    <soa:DataBindingItem ControlID="EndDate" ControlPropertyName="Value" Direction="ControlToData"
                                        DataPropertyName="EndDate" />
                                </ItemBindings>
                            </soa:DataBindingControl>
                        </div>
                        <div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left; margin-right: -12px;">
                            <div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px; line-height: 30px; padding-bottom: 0px">
                                <mcs:TranslatorLabel runat="server" Category="OACommons" Text="日志查看" />
                            </div>
                        </div>
                        <div style="margin-top: 7px; margin-bottom: 7px" runat="server" id="divSearch">
                            <table style="background-color: Silver; width: 100%; text-align: left" cellspacing="1px"
                                border="0" cellpadding="0">
                                <tr style="background-color: White">
                                    <td id="tdFormCateogryLbl" runat="server" style="text-align: center; background-color: #f2f8f8; width: 80px;">表单类别：
                                    </td>
                                    <td id="tdFormCategory" runat="server" style="text-align: left; background-color: #f7fbfa; width: 320px;">
                                        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                &nbsp;&nbsp;
											<asp:DropDownList ID="FormCategory" runat="server" Width="100px" AutoPostBack="True"
                                                OnSelectedIndexChanged="FormCategory_SelectedIndexChanged">
                                            </asp:DropDownList>
                                                <asp:DropDownList ID="ProgramName" runat="server" Width="200px">
                                                </asp:DropDownList>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="FormCategory" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td style="text-align: center; background-color: #f2f8f8; height: 30px; width: 80px;">操作时间：
                                    </td>
                                    <td style="text-align: center; background-color: #f7fbfa; width: 210px;">
                                        <mcs:DeluxeCalendar ID="StartDate" runat="server" Width="70px">
                                        </mcs:DeluxeCalendar>至
									<mcs:DeluxeCalendar ID="EndDate" runat="server" Width="70px">
                                    </mcs:DeluxeCalendar>
                                    </td>
                                    <td style="text-align: left; background-color: #f2f8f8; height: 30px;">&nbsp;&nbsp;
									<asp:LinkButton runat="server" ID="queryBtn" OnClick="QueryBtnClick" OnClientClick="return verifyTime('StartDate','EndDate');">
						                <img src="../../images/16/search.gif" alt="查询" style="border:0;"/>查询
                                    </asp:LinkButton>
                                    </td>
                                </tr>
                                <%--<tr style="background-color: White; height: 30px;">
								 <td style="background-color: #f7fbfa; text-align: right;" align="right" colspan="5">
                        <asp:Button runat="server" ID="ButtonAdvanced" Text="高级查询" OnClick="ButtonAdvanced_Click"
                            CssClass="hidden" />
                        <a id="advancedSearch" href="#" onclick="onAdvancedSearchClick();" runat="server">
                            <img src="../../Images/16/find.gif" style="border: 0; margin-bottom: -3px;" alt="高级查询" />高级查询</a>
                    </td>
							</tr>--%>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; text-align: center;">
                        <div style="margin-top: 7px; margin-bottom: 7px;">
                            <mcs:DeluxeGrid ID="DeluxeGridLog" runat="server" ShowExportControl="true" PageSize="10" Category="OACommons"
                                TitleFontSize="Small" Width="98%" DataSourceID="ObjectDataSourceLogs" AllowPaging="true"
                                AllowSorting="true" AutoGenerateColumns="false" DataKeyNames="ID" CheckBoxPosition="Right"
                                PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
                                PagerSettings-NextPageText="下一页" PagerSettings-Position="TopAndBottom" CaptionAlign="Right" EmptyDataText="没有查询到您所需要的数据"
                                CssClass="dataList" TitleCssClass="title" GridTitle="应用日志浏览" OnExportData="DeluxeGridLog_ExportData">
                                <HeaderStyle CssClass="head" />
                                <RowStyle CssClass="item" />
                                <AlternatingRowStyle CssClass="aitem" />
                                <SelectedRowStyle CssClass="selecteditem" />
                                <PagerStyle CssClass="pager" />
                                <Columns>
                                    <asp:TemplateField HeaderText="标题">
                                        <ItemTemplate>
                                            <%#Server.HtmlEncode((string)Eval("Subject")) %>
                                        </ItemTemplate>
                                        <HeaderStyle Width="20%" />
                                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="表单类别">
                                        <ItemTemplate>
                                            <%# Server.HtmlEncode((string)Eval("ApplicationName")) %>
                                        </ItemTemplate>
                                        <HeaderStyle Width="12%" />
                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="操作人部门" Visible="false">
                                        <ItemTemplate>
                                            <%# Server.HtmlEncode(GetOguObjectName(Eval("TopDepartment")))%>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        <HeaderStyle Width="10%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="操作人员">
                                        <ItemTemplate>
                                            <%# Server.HtmlEncode(GetOguObjectName(Eval("Operator")))%>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        <HeaderStyle Width="10%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="环节名称">
                                        <ItemTemplate>
                                            <%#Server.HtmlEncode((string)Eval("ActivityName"))%>
                                        </ItemTemplate>
                                        <HeaderStyle Width="12%" />
                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="具体操作">
                                        <ItemTemplate>
                                            <%#Server.HtmlEncode((string)Eval("OperationName"))%>
                                        </ItemTemplate>
                                        <HeaderStyle Width="12%" />
                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="待办人列表">
                                        <ItemTemplate>
                                            <%#Server.HtmlEncode((string)Eval("TargetDescription"))%>
                                        </ItemTemplate>
                                        <HeaderStyle Width="12%" />
                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="操作时间">
                                        <ItemTemplate>
                                            <%# ((DateTime)Eval("OperationDateTime")).ToString("yyyy-MM-dd HH:mm:ss")%>
                                        </ItemTemplate>
                                        <HeaderStyle Width="17%" HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" Width="17%" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="详细">
                                        <ItemTemplate>
                                            <img src="../images/chakan.gif" alt="" onclick="openwindow('<%#Eval("ID") %>')" style="cursor: hand" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
                                    PreviousPageText="上一页"></PagerSettings>
                            </mcs:DeluxeGrid>
                            <asp:ObjectDataSource ID="ObjectDataSourceLogs" runat="server" EnablePaging="True"
                                SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
                                TypeName="MCS.OA.CommonPages.UserOperationLog.LogQuery" EnableViewState="False"
                                OnSelected="ObjectDataSourceLogs_Selected" OnSelecting="ObjectDataSourceLogs_Selecting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"
                                        Type="String" />
                                    <asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="height: 32px">
                        <div id="divClose" runat="server" visible="false" style="height: 32px; text-align: center; vertical-align: middle;">
                            <input accesskey="C" id="btnClose" class="formButton" onclick="window.close();" type="button" category="OACommons" runat="server"
                                value="关闭(C)" name="btnClose" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
