<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Default" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCXC" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Deluxe Search Test</title>
<%--    <link href="css/style.css" rel="stylesheet" type="text/css" />--%>
<%--    <link href="deluxesearch.css" rel="stylesheet" type="text/css" />--%>
    <script type="text/javascript">
        function onAdvancedSearch(sender, e) {
            //e.resultValue = window.showModalDialog("DialogTest1.aspx", e.args, 'dialogWidth=800px;dialogHeight=250px;status=no;help=no;scroll=no');
            var result = $find("bindingControl").collectData(false);
            e.resultValue = result;
        }

        function onconditionClick(sender, e) {
        	var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
        	var bindingControl = $find("bindingControl");
        	bindingControl.dataBind(content);
        }

        function onChecked(sender, e) {
        	//alert(e.checkbox.checked);
        	alert(e.checkbox.value);
		}
        function allChecked(sender, e) {
        	//alert(e.checkbox.checked);        	
		}
    </script>
</head>
<body>
    <form id="form1" runat="server">  
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <cc1:DataBindingControl runat="server" ID="bindingControl" AllowClientCollectData="True">
        <ItemBindings>
           <%-- <cc1:DataBindingItem ControlID="ddlScope" DataPropertyName="SupplierRegionalCode">
            </cc1:DataBindingItem>--%>
            <cc1:DataBindingItem ControlID="ddlType" DataPropertyName="Status">
            </cc1:DataBindingItem>
    </ItemBindings>
    </cc1:DataBindingControl> 
    <div  style="width: 1000px;  padding: 15px; margin: auto;">
    <cc1:DeluxeSearch ID="DeluxeSearch1" runat="server"  OnSearching="BtnSearchClick" CustomSearchContainerControlID="searchContainer"
 
     CssClass="deluxe-search"   HasCategory="True" HasAdvanced="True" SearchField="SupplierName"  SearchMethod="PrefixLike" OnConditionClick="onconditionClick">
        <categories>
        <%-- <cc1:Category CategoryField="SupplierRegionalCode" DataSourceID="testDataSource"  DataTextField="区域" DataValueField="area" ConditionText="AreaName" ConditionValue="AreaValue">                                                       
               </cc1:Category>--%>
             <%--  <cc1:Category CategoryField="Status" DataSourceID="testDataSource1" DataTextField="供应商类型" 
                 DataValueField="type" ConditionText="TypeName" ConditionValue="TypeValue" >                   
               </cc1:Category>--%>
               </categories>        
    </cc1:DeluxeSearch>     
    <asp:ObjectDataSource runat="server" ID="testDataSource" TypeName="MCS.Library.SOA.Web.WebControls.Test.TestResult"
        SelectMethod="GetAreaItem"></asp:ObjectDataSource>
    <asp:ObjectDataSource runat="server" ID="testDataSource1" TypeName="MCS.Library.SOA.Web.WebControls.Test.TestResult"
        SelectMethod="GetTypeItem"></asp:ObjectDataSource>     
        <div style="margin-top:10px">
        </div>
        
    <cc1:DeluxeObjectDataSource ID="supplierGrid" runat="server" EnablePaging="True"
        TypeName="MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch.SupplierQueryAdapter">
    </cc1:DeluxeObjectDataSource>    
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
	<CCXC:DeluxeGrid  ID="relativeLinkGroupGrid" runat="server" AutoGenerateColumns="False"
			DataSourceID="supplierGrid" DataSourceMaxRow="0" AllowPaging="True" PageSize="2"
			Width="800px" DataKeyNames="Code" ExportingDeluxeGrid="False" GridTitle="Test"
			CssClass="dataList" ShowCheckBoxes="True" TitleColor="141, 143, 149"  MultiSelect="true" 
        TitleFontSize="Large" ShowExportControl="True"  OnSelectAllCheckBoxClick="allChecked"  OnSelectCheckBoxClick="onChecked">             
        <EmptyDataTemplate>
            暂时没有您需要的数据
        </EmptyDataTemplate>
        <Columns>
            <asp:BoundField DataField="Code" SortExpression="Code" HeaderText="Code" Visible="false">                
            </asp:BoundField>
            <asp:TemplateField HeaderText="供应商公司名称" SortExpression="SupplierName" ItemStyle-CssClass="123">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("SupplierName") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("SupplierName") %>'></asp:Label>
                </ItemTemplate>         
                <ItemStyle HorizontalAlign="Center" Width="180px" CssClass="1111"/>
            </asp:TemplateField>
  <%--          <asp:BoundField DataField="SupplierRegionalCode" HeaderText="区域" SortExpression="SupplierRegionalCode">
                <ItemStyle Width="135" HorizontalAlign="Center"></ItemStyle>                
            </asp:BoundField>--%>
            <asp:BoundField DataField="Status" HeaderText="供应商类型" SortExpression="Status">
                <ItemStyle Width="135" HorizontalAlign="Center"></ItemStyle>               
            </asp:BoundField>
            <asp:BoundField DataField="CreateTime" HeaderText="创建日期" SortExpression="Create_Time" 
            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" >
                <ItemStyle Width="150" HorizontalAlign="Center"></ItemStyle>              
            </asp:BoundField>
            <asp:TemplateField HeaderText="">
                <ItemStyle Width="80" HorizontalAlign="Center"></ItemStyle>              
                <ItemTemplate>
                    <a id="editItem" href='javascript:editLinkClick("<%#Eval("Code") %>");'>编辑</a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" >
                <ItemStyle Width="80" HorizontalAlign="Center"></ItemStyle>               
                <ItemTemplate>
                    <a id="removeItem" href='javascript:removeItem("<%#Eval("Code") %>");'>删除</a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
            <ItemTemplate>
                <cc1:UserPresence ID="UserPresence1" runat="server" UserID="22c3b351-a713-49f2-8f06-6b888a280fff">
                </cc1:UserPresence>
            </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </CCXC:DeluxeGrid>
        </ContentTemplate>
        </asp:UpdatePanel>
    <div id="searchContainer" runat="server" style="display:none;">
    <table style=" width: 100%; padding: 0px;border-collapse:collapse;border:0;margin:0;">
        <tr>
            <td style="">
                行业领域
            </td>
            <td style="">
                <asp:TextBox runat="server" ID="txtArea" ></asp:TextBox>
            </td>
            <td style="">
                供应商名称
            </td>
            <td style="">
                <asp:TextBox runat="server" ID="txtName" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="">
                区域
            </td>
            <td style="">
                <cc1:HBDropDownList runat="server" ID="ddlScope">
                </cc1:HBDropDownList>
            </td>
            <td style="">
                供应商类别
            </td>
            <td style="">
                <cc1:HBDropDownList runat="server" ID="ddlType" >
                </cc1:HBDropDownList>
            </td>
        </tr>
        <tr>
            <td style="">
                总部地址
            </td>
            <td style="">
                <asp:TextBox runat="server" ID="txtAddress" ></asp:TextBox>
            </td>
            <td style="">
                注册资本(万元)
            </td>
            <td style="">
                <asp:TextBox runat="server" ID="txtRegisteredCapital" ></asp:TextBox>
            </td>
        </tr>
       
   
    </table>

    </div>
    <a href="http://www.baidu.com">百度</a>
    </div>
    </form>
</body>
</html>
