<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="MCS.OA.CommonPages.AppTrace.Category"
    Theme="Platform" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>分类权限查询</title>
    <link href="../../CSS/overrides.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/templatecss.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <style type="text/css">
		.c-modify
		{
			display: inline-block;
			width: 20px;
			height: 16px;
			line-height: 16px;
			border: 1px solid silver;
			text-align: center;
			padding:0;
		}
		
		img { border:0;}
		
		.c-viewrole
		{
			background: url("../images/shortcut.png") no-repeat scroll 0 bottom;
			display: inline-block;
			width: 16px;
			height: 16px;
		}
		
		.c-rdolist
		{
			display:block; height:100%;
		}
		
		.c-rdolist input{ width:16px; height:16px; margin-right:2px; }
		
		.thereSearch
		{
			margin: 5px;
			border: 1px solid silver;
			padding: 5px;
		}
	</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="t-container">
        <div class="t-dialog-caption">
            <span class="t-dialog-caption">分类权限查询 </span>
        </div>
        <div class="thereSearch">
            <asp:MultiView ActiveViewIndex="0" runat="server" ID="views">
                <asp:View runat="server" ID="searchView">
                    <table style="table-layout: auto">
                        <tr>
                            <th>
                                应用
                            </th>
                            <td>
                                <asp:DropDownList runat="server" AutoPostBack="True" ID="ddApps" DataSourceID="ObjectDataSource1"
                                    AppendDataBoundItems="True" OnSelectedIndexChanged="ReLoadPrograms" DataTextField="Name"
                                    DataValueField="CodeName">
                                </asp:DropDownList>
                                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="QueryAvaliableApplicationNames"
                                    TypeName="MCS.OA.CommonPages.AppTrace.CategorySearchSource"></asp:ObjectDataSource>
                            </td>
                            <th>
                                模块
                            </th>
                            <td>
                                <asp:DropDownList runat="server" ID="ddProgam" AutoPostBack="True" DataSourceID="ObjectDataSource2"
                                    DataTextField="Name" DataValueField="CodeName">
                                </asp:DropDownList>
                                <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="QueryAvaliableProgramNames"
                                    TypeName="MCS.OA.CommonPages.AppTrace.CategorySearchSource">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddApps" Name="appName" PropertyName="SelectedValue"
                                            Type="String" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                            <th>
                                权限
                            </th>
                            <td>
                                <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                    CssClass="c-rdolist" AutoPostBack="True" ID="rdoList">
                                    <asp:ListItem Text="全部" Value="0" Selected="True" />
                                    <asp:ListItem Text="表单流程调整者" Value="1" />
                                    <asp:ListItem Text="表单查看者" Value="2" />
                                </asp:RadioButtonList>
                            </td>
                            <td>
                                <asp:LinkButton Text=">>按人员筛选可以处理的类别" runat="server" CommandName="toggle" CommandArgument="ByPerson"
                                    OnCommand="DoCommand" />
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <asp:View runat="server">
                    <table style="table-layout: fixed; width: 100%;">
                        <tr>
                            <th style="width: 150px">
                                按用户筛选
                            </th>
                            <td>
                                <soa:OuUserInputControl runat="server" ID="user1" MultiSelect="false" />
                            </td>
                            <td>
                                <asp:Button Text="筛选" runat="server" OnClick="FilterByPerson" OnClientClick="return getSelectedUser()" />
                                <asp:HiddenField ID="lastUser" runat="server" />
                            </td>
                            <td>
                                <asp:LinkButton ID="lnkCmd" Text=">>按应用分类查询" runat="server" CommandName="toggle"
                                    CommandArgument="ByCategory" OnCommand="DoCommand" />
                            </td>
                        </tr>
                    </table>
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
    <div class="t-grid-container">
        <div style="display: none">
            <input type="hidden" id="postAppName" runat="server" />
            <input type="hidden" id="postProgram" runat="server" />
            <input type="hidden" id="postType" runat="server" />
            <input type="hidden" id="postRole" runat="server" />
            <input type="hidden" id="postRoleName" runat="server" />
            <asp:Button Text="OK" runat="server" ID="btnChangeRole" OnClick="ServerChangeRole" />
            <asp:Button Text="Refresh" ID="btnRefresh" runat="server" OnClick="RefreshList" />
        </div>
        <div style="height: 30px; background-color: #c0c0c0" id="panEdit" runat="server">
            <a onclick="onAddClick();" href="javascript:void(0);" runat="server" id="btnNew">
                <img alt="新建" src="../../../MCSWebApp/Images/appIcon/15.gif" />
            </a>
            <script type="text/javascript">
                function onAddClick() {
                    var feature = "dialogWidth:300px; dialogHeight:360px; center:yes; help:no; resizable:no;status:no;";

                    window.showModalDialog("CategoryEdit.aspx", null, feature);
                    $get("btnRefresh").click();
                }

                function onDeleteClick() {
                    var result = false;
                    var grid = $find("gridViewTask");
                    if (grid) {
                        if (grid.get_clientSelectedKeys().length) {
                            result = true;
                        }
                    }

                    grid = null;

                    return result;
                }
			
            </script>
        </div>
        <mcs:DeluxeGrid ID="gridViewTask" runat="server" AutoGenerateColumns="False" DataSourceID="src1"
            AllowPaging="True" AllowSorting="True" ShowExportControl="False" GridTitle="待办列表"
            DataKeyNames="APPLICATION_NAME,PROGRAM_NAME,AUTH_TYPE" CssClass="dataList gtasks"
            TitleCssClass="title" Width="100%" DataSourceMaxRow="0" TitleColor="141, 143, 149"
            TitleFontSize="Large" CascadeControlID="" SkinID="gridSkin">
            <CheckBoxTemplateHeaderStyle Width="16px" />
            <EmptyDataTemplate>
                暂时没有您需要的数据
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="APP_DISP" SortExpression="B.Name" HeaderText="应用名">
                    <ItemStyle HorizontalAlign="Center" CssClass="bg_td1" />
                </asp:BoundField>
                <asp:BoundField DataField="APPLICATION_NAME" SortExpression="APPLICATION_NAME" HeaderText="应用代码名">
                    <ItemStyle HorizontalAlign="Center" CssClass="bg_td1" />
                </asp:BoundField>
                <asp:BoundField DataField="PROGRAM_DISP" HeaderText="模块" SortExpression="C.Name"
                    ItemStyle-CssClass="bg_td1">
                    <ItemStyle CssClass="bg_td1"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="PROGRAM_NAME" HeaderText="模块代码名" SortExpression="PROGRAM_NAME"
                    ItemStyle-CssClass="bg_td1">
                    <ItemStyle CssClass="bg_td1"></ItemStyle>
                </asp:BoundField>
                <mcs:EnumDropDownField ReadOnly="true" HeaderText="类型" SortExpression="AUTH_TYPE"
                    DataField="AUTH_TYPE" EnumTypeName="MCS.Library.SOA.DataObjects.Workflow.WfApplicationAuthType, MCS.Library.SOA.DataObjects" />
                <asp:TemplateField HeaderText="角色" ItemStyle-CssClass="bg_td1">
                    <ItemTemplate>
                        <a target="_blank" title="查看人员" href='<%# Eval("ROLE_ID","/MCSWebApp/PermissionCenter/lists/AppRoleMembers.aspx?role={0}") %>'>
                            <span class="c-viewrole"></span>
                            <%# Server.HtmlEncode( Eval("ROLE_DESCRIPTION").ToString())%>
                        </a><a href="javascript:void(0);" class="c-modify" onclick='return applyChange(this);'
                            runat="server" visible='<%# this.SupervisiorMode %>' data-appname='<%# HttpUtility.HtmlAttributeEncode((string)Eval("APPLICATION_NAME")) %>'
                            data-pgname='<%# HttpUtility.HtmlAttributeEncode((string)Eval("PROGRAM_NAME")) %>'
                            data-type='<%# HttpUtility.HtmlAttributeEncode((string)Eval("AUTH_TYPE")) %>'>...</a>
                    </ItemTemplate>
                    <ItemStyle CssClass="bg_td1"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CREATE_TIME" SortExpression="CREATE_TIME" HeaderText="流程创建时间"
                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-CssClass="bg_td1">
                    <ItemStyle CssClass="bg_td1"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="操作">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" OnClick="HandleDelete" OnClientClick="return onDeleteClick(this);"
                            ID="btnDelete" data-appname='<%# HttpUtility.HtmlAttributeEncode((string)Eval("APPLICATION_NAME")) %>'
                            data-pgname='<%# HttpUtility.HtmlAttributeEncode((string)Eval("PROGRAM_NAME")) %>'
                            data-type='<%# HttpUtility.HtmlAttributeEncode((string)Eval("AUTH_TYPE")) %>'>
						<img alt="删除" src="../../../MCSWebApp/Images/16/delete.gif">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" Mode="NextPreviousFirstLast"
                NextPageText="下一页" Position="Bottom" PreviousPageText="上一页"></PagerSettings>
            <SelectedRowStyle CssClass="selecteditem" />
        </mcs:DeluxeGrid>
        <asp:ObjectDataSource runat="server" ID="src1" EnablePaging="True" TypeName="MCS.OA.CommonPages.AppTrace.CagetoryDataSource"
            OnSelecting="ObjectDataSourceSelecting" SelectCountMethod="GetQueryCount" SelectMethod="Query"
            SortParameterName="orderBy">
            <SelectParameters>
                <asp:Parameter DbType="Int32" Name="totalCount" Direction="Output" />
                <asp:ControlParameter ControlID="ddApps" Name="appName" PropertyName="SelectedValue"
                    Type="String" />
                <asp:ControlParameter ControlID="ddProgam" Name="programName" PropertyName="SelectedValue"
                    Type="String" />
                <asp:ControlParameter ControlID="rdoList" Name="authType" PropertyName="SelectedValue"
                    Type="Object" />
                <asp:Parameter Name="where" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    </form>
    <script type="text/javascript">
        function onConditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function attr(node, name) {
            if (node.getAttribute) {
                return node.getAttribute(name);
            } else {
                return node[name];
            }
        }

        function onDeleteClick(lnk) {
            if (lnk.nodeName.toUpperCase() === "A") {
                if (!lnk.disabled) {
                    var appname = attr(lnk, "data-appname");
                    var pgname = attr(lnk, "data-pgname");
                    var type = attr(lnk, "data-type");

                    $get("postAppName").value = appname;
                    $get("postProgram").value = pgname;
                    $get("postType").value = type;

                    return confirm('确定要删除此记录吗？');
                }
            }

            return false;
        }

        function applyChange(lnk) {
            if (lnk.nodeName.toUpperCase() === "A") {
                if (!lnk.disabled) {
                    var appname = attr(lnk, "data-appname");
                    var pgname = attr(lnk, "data-pgname");
                    var type = attr(lnk, "data-type");

                    $get("postAppName").value = appname;
                    $get("postProgram").value = pgname;
                    $get("postType").value = type;

                    var param = { window: window, inputElem: "postRole" };

                    var feature = "dialogWidth:800px; dialogHeight:640px; center:yes; help:no; resizable:no;status:no;";

                    var result = window.showModalDialog("/MCSWebApp/PermissionCenter/dialogs/RoleSearchDialog.aspx", param, feature);

                    param = null;

                    if (result) {
                        var roles = Sys.Serialization.JavaScriptSerializer.deserialize($get("postRole").value);
                        if (roles.length > 0) {
                            $get("postRole").value = roles[0].RoleID;
                            $get("postRoleName").value = roles[0].RoleName;
                            $get("btnChangeRole").click();
                        }
                    }
                }
            }
        }

        function getSelectedUser() {
            var data = $find("user1").get_selectedSingleData();
            if (data) {
                return true;
            } else {
                alert("请先选择用户");
                return false;
            }
        }
    </script>
</body>
</html>
