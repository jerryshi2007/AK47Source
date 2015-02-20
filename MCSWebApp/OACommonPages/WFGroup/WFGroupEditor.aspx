<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WFGroupEditor.aspx.cs"
    Inherits="MCS.OA.CommonPages.WFGroup.WFGroupEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <title>群组定义</title>
    <link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/style.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function refreshParent() 
        {
            top.returnValue = "reload";
            top.close();
        }
        function CheckGroup() 
        {
            if (document.getElementById('txtGroupName').value == "")
            {
                alert("组名称不能为空");
                return false;
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server" >
    <div>
		<input runat="server" type="hidden" id="whereCondition" />
	</div>
      <div id="groupProperties">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: auto;" border="0">
            <tr style="height: 40px;">
             		<td style="height: 32px">
				<div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;">
					<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
						line-height: 30px; padding-bottom: 0px">
						群组定义
					</div>
				</div>
			</td>
            </tr>
            <tr>
                <td>
                    <div id="dcontent">
                        <table cellspacing="0" cellpadding="0" style="height:auto; width: 96%;" border="0">
                            <tr>
                                <td class="fim_l" style="width: 100px;height:30px;">
                                    群组名称：
                                </td>
                                <td style="width: 200px;;height:30px;">
                                    <asp:TextBox ID="txtGroupName" runat="server" Width="140px"></asp:TextBox>
                                </td>
                                <td class="fim_l" style=";height:30px;width: 100px;">
                                    群组类别：
                                </td>
                                <td style=";height:30px;width: 200px;">
                                    <asp:TextBox ID="txtGroupCategory" runat="server" Width="140px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style=";height:30px;width: 100px;">
                                    负责人：
                                </td>
                                <td style=";height:30px;width: 200px;">
                                       <HB:OuUserInputControl ID="OuUserInputControlManager" runat="server" MultiSelect="false"
                                        Width="200px" SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
                                </td>
                                <td class="fim_l" style=";height:30px;width: 100px;">
                                </td>
                                <td style=";height:30px;width: 200px;">
                                     <asp:Button ID="BtnSave" OnClientClick="return CheckGroup();" CssClass="formButton" runat="server" Width="85" Text="保存"
                                OnClick="BtnSave_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="userList">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: auto;" border="0">
            <tr>
                <td valign="middle">                   
                        组内人员:
                        </td>
            </tr>
           <tr>
                <td valign="middle">
                <table>
                <tr>
                <td>
                   <HB:OuUserInputControl ID="groupUserInput" runat="server" MultiSelect="true"
                                        Width="200px" SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
                </td>
                <td>
                  <asp:Button ID="btnAddUsers" CssClass="formButton" runat="server" Width="130" Text="添加人员到组"
                                OnClick="BtnAddUser_Click" />
                                 <asp:Button ID="btnDeleteUsers" CssClass="formButton" runat="server" Width="130" Text="删除选中人员"
                                OnClick="BtnDeleteUser_Click" />
                </td>
                </tr>
                </table>
                </td>
            </tr>
            <tr>
                <td>
                   <MCS:DeluxeGrid ID="DeluxeGridGroupUser" runat="server" ShowExportControl="false"
					TitleFontSize="Small" Width="100%" DataSourceID="ObjectDataSourceGroupUsers"
					AllowPaging="true" PageSize="10" AllowSorting="true" AutoGenerateColumns="false"
					PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
					PagerSettings-NextPageText="下一页" PagerSettings-Position="Bottom" CaptionAlign="Right"
					CssClass="dataList" TitleCssClass="title" GridTitle="组内人员" ShowCheckBoxes="true" MultiSelect="true" DataKeyNames="UserID" 
					>
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
					<PagerStyle CssClass="pager" />
                     <CheckBoxTemplateItemStyle Width="30px" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
				      <Columns>
                        <asp:TemplateField HeaderText="名称">
                            <ItemTemplate>
                                <%#Server.HtmlEncode((string)Eval("User.DisplayName")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                       <%-- <asp:TemplateField HeaderText="编辑">
                            <ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="15%" />
                            <ItemTemplate>
                                     <asp:LinkButton ID="LinkBtnDel" runat="server" Style="cursor: hand; text-decoration: underline;
                                    color: Blue;" Text="删除" CommandName="DeleteGroupUser" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                    </Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="Bottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<asp:ObjectDataSource ID="ObjectDataSourceGroupUsers" runat="server" EnablePaging="True"
					SelectCountMethod="GetQueryCount" SelectMethod="Query" SortParameterName="orderBy"
					TypeName="MCS.Library.SOA.DataObjects.WfGroupUserQuery"
					EnableViewState="False" OnSelected="ObjectDataSourceGroupUsers_Selected"
					OnSelecting="ObjectDataSourceGroupUsers_Selecting">
                    <SelectParameters>
                    <asp:ControlParameter ControlID="whereCondition" Name="where" PropertyName="Value"	Type="String" />
						<asp:Parameter Direction="InputOutput" Name="totalCount" Type="Int32" />
                        </SelectParameters>
				</asp:ObjectDataSource>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <HB:DataBindingControl runat="server" ID="bindingControl" AutoValidate="true" IsValidateOnSubmit="true">
            <ItemBindings>
                <HB:DataBindingItem ControlID="txtGroupName" DataPropertyName="GroupName" ControlPropertyName="Text" />
                <HB:DataBindingItem ControlID="txtGroupCategory" DataPropertyName="Category" ControlPropertyName="Text" />
            </ItemBindings>
        </HB:DataBindingControl>
    </div>
    </form>
</body>
</html>
