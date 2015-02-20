<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WFPostEditor.aspx.cs"
    Inherits="MCS.OA.CommonPages.WFPost.WFPostEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <title>岗位定义</title>
    <link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/style.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function refreshParent() {
            top.returnValue = "reload";
            top.close();
        }
        function CheckPost() {
            if (document.getElementById('txtPostName').value == "") {
                alert("岗位名称不能为空");
                return false;
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
		<input runat="server" type="hidden" id="whereCondition" />
	</div>
      <div id="postProperties">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: auto;" border="0">
            <tr style="height: 40px;">
             		<td style="height: 32px">
				<div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;">
					<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
						line-height: 30px; padding-bottom: 0px">
						岗位定义
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
                                    岗位名称：
                                </td>
                                <td style="width: 200px;;height:30px;">
                                    <asp:TextBox ID="txtPostName" runat="server" Width="140px"></asp:TextBox>
                                </td>
                                <td class="fim_l" style=";height:30px;width: 100px;">
                                    岗位类别：
                                </td>
                                <td style=";height:30px;width: 200px;">
                                    <asp:TextBox ID="txtPostCategory" runat="server" Width="140px"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td valign="right">
                    <div id="dfooter">
                        <p style="vertical-align: middle; text-align：right; height: 40px; text-align: right;">
                            <asp:Button ID="BtnSave" CssClass="formButton" runat="server" Width="85" Text="保存"
                                OnClick="BtnSave_Click" />
                        </p>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="userList">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: auto;" border="0">
            <tr>
                <td valign="middle">                   
                        岗位人员:
                </td>
            </tr>
           <tr>
                <td valign="middle">
                <table>
                <tr>
                <td>
                   <HB:OuUserInputControl ID="postUserInput" runat="server" MultiSelect="true"
                                        Width="200px" SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
                </td>
                <td>
                  <asp:Button ID="btnAddPostUser" CssClass="formButton" runat="server" Width="130" Text="添加人员到岗位"
                                OnClick="BtnAddUsers_Click" />
                                <asp:Button ID="btnDeletePostUser" CssClass="formButton" runat="server" Width="130" Text="删除选中人员"
                                OnClick="BtnDeleteUsers_Click" />
                </td>
                </tr>
                </table>
                </td>
            </tr>
            <tr>
                <td>
                   <MCS:DeluxeGrid ID="DeluxeGridPostUser" runat="server" ShowExportControl="false"
					TitleFontSize="Small" Width="100%" DataSourceID="ObjectDataSourcePostUsers"
					AllowPaging="true" PageSize="10" AllowSorting="true" AutoGenerateColumns="false"
					PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
					PagerSettings-NextPageText="下一页" PagerSettings-Position="Bottom" CaptionAlign="Right"
					CssClass="dataList" TitleCssClass="title" GridTitle="岗位人员"  MultiSelect="true" DataKeyNames="UserID" ShowCheckBoxes="true" >
					<HeaderStyle CssClass="head" />
					<RowStyle CssClass="item" />
					<AlternatingRowStyle CssClass="aitem" />
					<SelectedRowStyle CssClass="selecteditem" />
                    <CheckBoxTemplateItemStyle Width="30px" />
                    <PagerStyle CssClass="pager" />
					<EmptyDataTemplate>
						没有查询到您所需要的数据，请重新输入查询条件进行查询
					</EmptyDataTemplate>
				      <Columns>
                        <asp:TemplateField HeaderText="名称" ControlStyle-Width="80%">
                            <ItemTemplate>
                                <%#Server.HtmlEncode((string)Eval("User.DisplayName")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                      <%--  <asp:TemplateField HeaderText="编辑">
                            <ItemStyle ForeColor="blue" HorizontalAlign="Center" Width="15%" />
                            <ItemTemplate>
                                     <asp:LinkButton ID="LinkBtnDel" runat="server" Style="cursor: hand; text-decoration: underline;
                                    color: Blue;" Text="删除" CommandName="DeletePostUser" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                    </Columns>
					<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
						PreviousPageText="上一页"></PagerSettings>
				</MCS:DeluxeGrid>
				<asp:ObjectDataSource ID="ObjectDataSourcePostUsers" runat="server" EnablePaging="True"
					SelectCountMethod="GetQueryCount" SelectMethod="Query"  SortParameterName="orderBy"
					TypeName="MCS.Library.SOA.DataObjects.WfPostUserQuery"
					EnableViewState="False" OnSelected="ObjectDataSourcePostUsers_Selected"
					OnSelecting="ObjectDataSourcePostUsers_Selecting">
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
                <HB:DataBindingItem ControlID="txtPostName" DataPropertyName="PostName" ControlPropertyName="Text" />
                <HB:DataBindingItem ControlID="txtPostCategory" DataPropertyName="Category" ControlPropertyName="Text" />
            </ItemBindings>
        </HB:DataBindingControl>
    </div>
    </form>
</body>
</html>
