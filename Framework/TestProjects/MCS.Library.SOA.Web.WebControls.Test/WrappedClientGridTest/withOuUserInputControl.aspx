<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="withOuUserInputControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.withOuUserInputControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function OnCellCreatingEditor(grid, e) {
            if (e.column.dataField == "USER") {

                var div = $HGDomElement.createElementFromTemplate({ nodeName: "div" });
                div.id = "div_userSelectControl_1";

                var userSelectControl = new $HBRootNS.OuUserInputControl(div);
                userSelectControl.id = "div_userSelectControl_1_div_userSelectControl_1";

                userSelectControl.initialize();

                e.editor.set_editorElement(userSelectControl);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="带选人控件的clientGrid" OnCellCreatingEditor="OnCellCreatingEditor">
            <Columns>
                <HB:ClientGridColumn DataField="ID" DataType="Integer" HeaderText="序号">
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="USER" DataType="String" HeaderText="人员">
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    <br />
    <div>
        <HB:OuUserInputControl runat="server" ID="OuUserInputControl_template" Width="300px" />
    </div>
    </form>
</body>
</html>
