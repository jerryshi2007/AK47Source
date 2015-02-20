<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="serverAddClientGrid.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.serverAddClientGrid" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
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
            var grid = $find("ClientGrid1");

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

                $find("ClientGrid1").set_dataSource(datasource);
            }
        }
    </script>
    <script type="text/javascript">
        function OnCellCreatedEditor(grid, e) {
            //alert("OnCellCreatedEditor");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div runat="server" id="div_container" style="width: 100%">
    </div>
    <div>
        <div>
            <SOA:UserSelector runat="server" ID="UserSelector_MultiSelect" MultiSelect="true"
                DialogHeaderText="人员选择" DialogTitle="人员选择" ShowingMode="Dialog" ShowOpinionInput="false"
                ListMask="All" />
            <div>
                <input type="button" id="btnAddUser" value="新增" onclick="showDialog('UserSelector_MultiSelect');" />
            </div>
        </div>
        <br />
        <br />
        <input type="button" value="[getvalue]" onclick="showselectedData();" />
        <asp:Button ID="Button1" runat="server" Text="[postback]" OnClick="Button1_Click" /></div>
    </form>
</body>
</html>
