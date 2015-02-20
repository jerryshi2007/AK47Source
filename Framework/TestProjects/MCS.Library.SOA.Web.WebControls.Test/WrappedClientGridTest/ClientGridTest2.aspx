<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientGridTest2.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.ClientGridTest2" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function onBeforeDataBind(gridControl, e) {
     
            for (var i = 0; i < gridControl.get_columns().length; i++) {

                if (gridControl.get_columns()[i].dataField == "GoodsMarque") {
                    var column = gridControl.get_columns()[i];
                    //column.editorReadOnly = true;
                     column.editTemplate=null;

                }
            }

        }

      
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <SOA:ClientGrid ID="clientGrid" runat="server" Width="50%" Caption="" PageSize="20"
            AllowPaging="true" AutoPaging="true" ShowEditBar="true" OnBeforeDataBind="onBeforeDataBind "  
            >
            <Columns>
                <SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="false" HeaderStyle="{width:'20px'}"
                    ItemStyle="{width:'20px'}" />
                <SOA:ClientGridColumn DataField="NCGoodsName" HeaderText="账套资产名称" HeaderStyle="{textAlign:'center'}" />
                <SOA:ClientGridColumn DataField="GoodsMarque" HeaderText="物品型号" HeaderStyle="{textAlign:'center'}"
                    DataType="String">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="GoodsName" HeaderText="资产名称" HeaderStyle="{width:'200px',textAlign:'center'}"
                    ItemStyle="{width:'200px',textAlign:'left'}" EditorStyle="{width:'200px',textAlign:'left'}">
                    <EditTemplate EditMode="A" TargetOfA="_self" TextFieldOfA="GoodsName" DefaultTextOfA="资产名称" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
        <%--  <SOA:ClientGrid ID="AssetInfoGrid" runat="server" Width="100%" Caption="" PageSize="20"
            AllowPaging="true" AutoPaging="true">
            <Columns>
                <SOA:ClientGridColumn DataField="NCGoodsName" HeaderText="账套资产名称" HeaderStyle="{textAlign:'center'}" />
                <SOA:ClientGridColumn DataField="GoodsMarque" HeaderText="物品型号" HeaderStyle="{textAlign:'center'}" />
                <SOA:ClientGridColumn DataField="GoodsName" HeaderText="资产名称" HeaderStyle="{width:'200px',textAlign:'center'}"
                    ItemStyle="{width:'200px',textAlign:'left'}" EditorStyle="{width:'200px',textAlign:'left'}">
                    <EditTemplate EditMode="A" TargetOfA="_self" TextFieldOfA="GoodsName" DefaultTextOfA="资产名称" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>--%>
    </div>
    </form>
</body>
</html>
