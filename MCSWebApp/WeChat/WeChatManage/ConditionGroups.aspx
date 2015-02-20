<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConditionGroups.aspx.cs"
    Inherits="WeChatManage.ConditionGroups" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>条件组</title>
    <link href="css/Style.css" rel="stylesheet" type="text/css" />
	<link href="css/form.css" rel="stylesheet" type="text/css" />
    <link href="css/weChatManage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function addGroup() {
            var url = "ModalDialogs/EditConditionGroup.aspx?tt=" + Date.parse(new Date());
            var result = window.showModalDialog(url, null, "dialogWidth=500px;dialogHeight=350px");
            if (result) {
                window.location.reload();
            }
        }

        function editGroup(groupID) {
            var url = "ModalDialogs/EditConditionGroup.aspx?tt=" + Date.parse(new Date()) + "&groupID=" + groupID;
            var result = window.showModalDialog(url, null, "dialogWidth=500px;dialogHeight=350px");
            if (result) {
                window.location.reload();
            }
        }

        function checkGroup(groupID) {
            var url = "ModalDialogs/CheckGroupMembers.aspx?tt=" + Date.parse(new Date()) + "&groupID=" + groupID;
            window.showModalDialog(url, null, "dialogWidth=800px;dialogHeight=500px");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align:left">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td style="vertical-align: top;">
					<table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
						<tr>
							<td class="leftsmalltitle">
								<asp:Image ID="Image1" ImageUrl="~/img/icon_01.gif" Width="14" Height="16" runat="server" />
								条件组
							</td>
						</tr>
						<tr>
							<td>
								<div>
                                    <input type="button" value="添加条件组" onclick="addGroup();" /></div>
							</td>
						</tr>
						<tr>
							<td style="height: 10px; display: block; vertical-align: top;">
                                <mcs:DeluxeGrid  ID="gridMain" runat="server" AutoGenerateColumns="False"
			                DataSourceID="supplierGrid" DataSourceMaxRow="0" AllowPaging="True" PageSize="5"
			                 DataKeyNames="GroupID" GridTitle=""
			                CssClass="dataList" TitleColor="141, 143, 149" TitleFontSize="Large" 
                                onrowdatabound="gridMain_RowDataBound" CascadeControlID="" 
                                ShowExportControl="False" >             
                        <EmptyDataTemplate>
                            暂时没有您需要的数据
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="组名称">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" 
                                        NavigateUrl='<%# "javascript:editGroup(\"" + Eval("GroupID") +"\");" %>' Text='<%# Eval("Name") %>'></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="150px" />
                            </asp:TemplateField>
                           <asp:BoundField DataField="Description" HeaderText="描述" SortExpression="Description">
                               <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                <ItemStyle  HorizontalAlign="Left"></ItemStyle>               
                            </asp:BoundField>
                            <asp:BoundField DataField="" HeaderText="会员人数" SortExpression="Description">
                                <ItemStyle Width="100" HorizontalAlign="Right"></ItemStyle>               
                            </asp:BoundField>
                          <asp:TemplateField HeaderText="会员">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" 
                                        NavigateUrl='<%# "javascript:checkGroup(\"" + Eval("GroupID") +"\");" %>' Text='查看'></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="150px" />
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pager" />
                        <RowStyle CssClass="item" />
                        <HeaderStyle CssClass="head" />
                        <AlternatingRowStyle CssClass="aitem" />
                    </mcs:DeluxeGrid>
                                <soa:DeluxeObjectDataSource ID="supplierGrid" runat="server" EnablePaging="True"
        TypeName="MCS.Web.Apps.WeChat.DataSources.ConditionalGroupQueryAdapter">
    </soa:DeluxeObjectDataSource>   
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
    </div>
    </form>
</body>
</html>
