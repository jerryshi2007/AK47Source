<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scene4Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Scene4Test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .blue
        {
            color: blue;
        }
        .red
        {
            color: red;
        }
    </style>
    <script type="text/javascript">
        //用户实现
        function onCreateDefaultViewData(bindingCtrl, e) {
            e.viewData = { color: bindingCtrl.makeObservable("red") };
            e.viewData.className = bindingCtrl.makeComputable(function () {
                return this.color.val();
            }, e.viewData);
            e.viewData.className.subscribeFrom(["color"], e.viewData);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:GangedDataBindingControl runat="server" ID="bindingControl" OnCreateDefaultViewData="onCreateDefaultViewData">
            <ItemBindings>
                <HB:GangedDataBindingItem ControlClientID="radio1" BindingSettings="{value:'color'}"/>
                <HB:GangedDataBindingItem ControlClientID="radio2" BindingSettings="{value:'color'}"/>
                <HB:GangedDataBindingItem ControlClientID="result" BindingSettings="{cssClass:'className'}"/>
            </ItemBindings>
        </HB:GangedDataBindingControl>
        <input id="radio1" type="radio" name="cssTest" value="blue" />
        <input id="radio2" type="radio" name="cssTest" value="red" />
        <span id="result">颜色</span>
    </div>
    </form>
</body>
</html>
