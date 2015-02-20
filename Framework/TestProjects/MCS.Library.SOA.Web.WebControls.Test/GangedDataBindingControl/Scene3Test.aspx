<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scene3Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Scene3Test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">
        //用户实现
        function onCreateDefaultViewData(bindingCtrl, e) {
            e.viewData = { show: bindingCtrl.makeObservable("1") };
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:GangedDataBindingControl runat="server" ID="bindingControl" OnCreateDefaultViewData="onCreateDefaultViewData">
            <ItemBindings>
                <HB:GangedDataBindingItem ControlClientID="chk1" BindingSettings="{value:'show'}"/>
                <HB:GangedDataBindingItem ControlClientID="result" BindingSettings="{visible:'show'}"/>
            </ItemBindings>
        </HB:GangedDataBindingControl>
        <input id="chk1" type="checkbox" value="1" />
        <span id="result">可以看到我么？</span>
    </div>
    </form>
</body>
</html>
