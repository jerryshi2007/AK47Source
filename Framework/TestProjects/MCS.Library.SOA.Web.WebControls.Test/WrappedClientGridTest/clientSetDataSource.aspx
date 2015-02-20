<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientSetDataSource.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.clientSetDataSource" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCXC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function showDialog(ctrlid) {
            var result = $find(ctrlid).showDialog();

            detailGridSetDataSource(result);
        }

        //获取选中项数据
        function showselectedData() {
            var grid = $find("detailGrid");

            var selecteddata = grid.get_selectedData();
            var dataSource = grid.get_dataSource();

            alert("selecteddata" + $Serializer.serialize(selecteddata) + "\n\ndatasource:" + $Serializer.serialize(dataSource));

            //debugger;
        }
    </script>
    <script type="text/javascript">
        function detailGridSetDataSource(data) {
            var datasource = [];
            if (data) {
                for (var i = 0; i < data.users.length; i++) {
                    var user = {
                        SelectedUserId: data.users[i].id,
                        SelectedUserDisplayName: data.users[i].displayName,
                        SelectedUserUserFullPath: data.users[i].fullPath
                    };

                    Array.add(datasource, user);
                }

                $find("detailGrid").set_dataSource(datasource);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <SOA:UserSelector runat="server" ID="UserSelector_MultiSelect" MultiSelect="true"
            DialogHeaderText="人员选择" DialogTitle="人员选择" ShowingMode="Dialog" ShowOpinionInput="false"
            ListMask="All" />
        <div>
            <input type="button" id="btnAddUser" value="新增" onclick="showDialog('UserSelector_MultiSelect');" />
        </div>
    </div>
    <br />
    <div>
        <SOA:ClientGrid runat="server" ID="detailGrid" Caption="" Width="100%">
            <Columns>
                <SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="true" />
                <SOA:ClientGridColumn DataField="SelectedUserId" HeaderText="员工编码" DataType="string"
                    ItemStyle="{width:'20%'}">
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="SelectedUserDisplayName" HeaderText="员工姓名" DataType="string">
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="SelectedUserUserFullPath" HeaderText="人员标示" DataType="string">
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
    </div>
    <br />
    <input type="button" value="[getvalue]" onclick="showselectedData();" />
    <asp:Button ID="Button1" runat="server" Text="[postback]" OnClick="Button1_Click" />
    </form>
</body>
</html>
