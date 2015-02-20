<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scene1Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Scene1Test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">
        //用户实现
        function onCreateDefaultViewData(bindingCtrl, e) {
            e.viewData = { data1: bindingCtrl.makeObservable("1"), data2: bindingCtrl.makeObservable("2") };

            e.viewData.result = bindingCtrl.makeComputable(function () {
                return this.data1.val() * this.data2.val();
            }, e.viewData);

            e.viewData.result.subscribeFrom(["data1", "data2"], e.viewData);//指定e.viewData对象的data1,data2属性的变化会引起result重新计算。
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:GangedDataBindingControl runat="server" ID="bindingControl" OnCreateDefaultViewData="onCreateDefaultViewData">
            <ItemBindings>
                <HB:GangedDataBindingItem ControlClientID="txtInput1" BindingSettings="{value:'data1'}">
                </HB:GangedDataBindingItem>
                <HB:GangedDataBindingItem ControlClientID="txtInput2" BindingSettings="{value:'data2'}">
                </HB:GangedDataBindingItem>
                <HB:GangedDataBindingItem ControlClientID="resultSpan" BindingSettings="{value:'result'}">
                </HB:GangedDataBindingItem>
                <HB:GangedDataBindingItem ControlClientID="txtResult" BindingSettings="{value:'result'}">
                </HB:GangedDataBindingItem>
            </ItemBindings>
        </HB:GangedDataBindingControl>
        <input id="txtInput1" type="text" /> <input id="txtInput2" type="text" />
        Result:<span id="resultSpan"></span> <input id="txtResult" type="text" />
    </div>
    </form>
</body>
</html>
