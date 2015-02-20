<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.SOLOptionDataSourceControlTest" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCXC" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DeluxeQueryConditionDataSourceControlTest</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <b>HBDropDownList</b><br/> 
        <SOA:HBDropDownList ID="HBDropDownList1" runat="server" DataSourceID="DeluxeObjectDataSource1" 
            DataTextField="CnName" DataValueField="Code">
        </SOA:HBDropDownList><br/> 
        <b>HBRadioButtonList</b>
        <SOA:HBRadioButtonList ID="HBRadioButtonList1" runat="server" DataSourceID="DeluxeObjectDataSource1" 
            DataTextField="CnName" DataValueField="Code">
        </SOA:HBRadioButtonList>
        <b>CheckBoxList</b> 
        <asp:CheckBoxList ID="CheckBoxList1" runat="server"  DataSourceID="DeluxeObjectDataSource1" 
            DataTextField="CnName" DataValueField="Code" >
        </asp:CheckBoxList>
        <b>DeluxeGrid</b> 
            <CCXC:DeluxeGrid  ID="relativeLinkGroupGrid" runat="server" AutoGenerateColumns="False"
			DataSourceID="DeluxeObjectDataSource1" DataSourceMaxRow="0" AllowPaging="True" PageSize="2"
			Width="800px" DataKeyNames="Code" GridTitle="Test"
			CssClass="dataList" ShowCheckBoxes="True" TitleColor="141, 143, 149" 
        TitleFontSize="Large" CascadeControlID=""  >             
        <EmptyDataTemplate>
            暂时没有您需要的数据
        </EmptyDataTemplate>
        <Columns>
            <asp:BoundField DataField="Code" SortExpression="Code" HeaderText="Code" Visible="True">                
            </asp:BoundField>
            <asp:BoundField DataField="CnName" SortExpression="CnName" HeaderText="CnName" Visible="True">                
            </asp:BoundField>
        </Columns>
    </CCXC:DeluxeGrid>
    
    <SOA:DeluxeQueryConditionDataSource ID="DeluxeObjectDataSource1" runat="server" CacheDuration="10" CacheExpirationPolicy="Absolute"
        Connection="SINOOCEAN_PRODUCTRESEARCH" TableName="[Prodefine].[CheckItem]" SelectFields="Code,CnName" OrderByClause="Code" 
        WhereClause="" EnableCaching="True">
    </SOA:DeluxeQueryConditionDataSource>
           </div>
       </form>
</body>
</html>
