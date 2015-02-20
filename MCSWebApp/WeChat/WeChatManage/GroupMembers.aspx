<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupMembers.aspx.cs" Inherits="WeChatManage.ModalDialogs.GroupMembers" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>条件组成员</title>
    <link href="css/Style.css" rel="stylesheet" type="text/css" />
	<link href="css/form.css" rel="stylesheet" type="text/css" />
    <link href="css/weChatManage.css" rel="stylesheet" type="text/css" />
    <script src="jquery/jquery-1.10.2.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server" target="innerFrame">
    <div style="text-align:left">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td style="vertical-align: top;">
					<table width="100%" border="0" align="center" cellpadding="0" cellspacing="0">
						<tr>
							<td class="leftsmalltitle">
								<asp:Image ID="Image1" ImageUrl="~/img/icon_01.gif" Width="14" Height="16" runat="server" />
								条件组成员
							</td>
						</tr>
						<tr id="trOperation" runat="server">
							<td>
								<div>
                                <asp:DropDownList ID="ddlGroups" runat="server" AutoPostBack="True" onselectedindexchanged="ddlGroups_SelectedIndexChanged"> 
                                </asp:DropDownList>  
                                 <soa:SubmitButton ID="btnCalculate" Text="重新计算"  PopupCaption="正在计算..." runat="server"  ProgressMode="BySteps" onclick="btnCalculate_Click"/>
                                 <soa:SubmitButton ID="btnSynchronizeToWeChat" Text="同步到微信" PopupCaption="正在同步..." runat="server"  ProgressMode="BySteps" onclick="btnSynchronizeToWeChat_Click"/>
                               </div>
							</td>
						</tr>
						<tr>
							<td style="height: 10px; display: block; vertical-align: top; ">
                            <mcs:DeluxeGrid  ID="gridMain" runat="server" AutoGenerateColumns="False"
			                    DataSourceID="gridDataSource" DataSourceMaxRow="0" AllowPaging="True" PageSize="10"
			                     DataKeyNames="MemberID" ExportingDeluxeGrid="False" GridTitle=""
			                    CssClass="dataList" TitleColor="141, 143, 149"  MultiSelect="true" 
                            TitleFontSize="Large" ShowExportControl="True" >             
                            <EmptyDataTemplate>
                                暂时没有您需要的数据
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="MemberName" SortExpression="MemberName" HeaderText="会员名称">
                                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                     <ItemStyle Width="100" HorizontalAlign="Left"></ItemStyle>                    
                                </asp:BoundField>
                                <asp:BoundField DataField="GroupName" SortExpression="GroupName" HeaderText="组名">
                                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                     <ItemStyle  HorizontalAlign="Left"></ItemStyle>                    
                                </asp:BoundField>
                               <asp:BoundField DataField="Age" HeaderText="年龄" SortExpression="Age">
                                   <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <ItemStyle Width="60" HorizontalAlign="Left"></ItemStyle>               
                                </asp:BoundField>
                                <asp:BoundField DataField="Gender" HeaderText="性别" SortExpression="Gender">
                                    <ItemStyle Width="60" HorizontalAlign="Center"></ItemStyle>               
                                </asp:BoundField>
                                 <asp:BoundField DataField="AnnualHouseholdIncome" HeaderText="家庭年收入" SortExpression="AnnualHouseholdIncome">
                                     <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <ItemStyle Width="150" HorizontalAlign="Left"></ItemStyle>               
                                </asp:BoundField>
                                  <asp:BoundField DataField="FamilyComposition" HeaderText="家庭结构" SortExpression="FamilyComposition">
                                      <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <ItemStyle Width="150" HorizontalAlign="Left"></ItemStyle>               
                                </asp:BoundField>
                                 <asp:BoundField DataField="NativePlace" HeaderText="籍贯" SortExpression="NativePlace ">
                                      <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <ItemStyle Width="120" HorizontalAlign="Left"></ItemStyle>               
                                </asp:BoundField>
                                 <asp:BoundField DataField="RegisteredPermanentResidence" HeaderText="户口所在地" SortExpression="RegisteredPermanentResidence">
                                      <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <ItemStyle Width="150" HorizontalAlign="Left"></ItemStyle>               
                                </asp:BoundField>
                                <asp:BoundField DataField="HousePaymentPrice" HeaderText="购房价格" SortExpression="HousePaymentPrice">
                                      <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    <ItemStyle Width="150" HorizontalAlign="Left"></ItemStyle>               
                                </asp:BoundField>
                            </Columns>
                             <PagerStyle CssClass="pager" />
                        <RowStyle CssClass="item" />
                        <HeaderStyle CssClass="head" />
                        <AlternatingRowStyle CssClass="aitem" />
                        </mcs:DeluxeGrid>
                                <soa:DeluxeObjectDataSource ID="gridDataSource" runat="server" EnablePaging="True" TypeName="MCS.Web.Apps.WeChat.DataSources.GroupMembersQueryAdapter">
                                </soa:DeluxeObjectDataSource>   
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
    </div>
    </form>
    <div style="display:none">
		<iframe id="innerFrame" name="innerFrame"></iframe>
	</div>
    <script type="text/javascript">
        var theForm = document.forms['form1'];
        if (!theForm) {
            theForm = document.form1;
        }

        var oldFun = window.__doPostBack;

        __doPostBack = function(eventTarget, eventArgument) {
            theForm.target = "_self";
            oldFun(eventTarget, eventArgument);
        };

    </script>
</body>
</html>
