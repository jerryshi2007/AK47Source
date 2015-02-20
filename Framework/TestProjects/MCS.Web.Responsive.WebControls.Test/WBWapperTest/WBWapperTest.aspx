<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBWapperTest.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test.WBWapperTest.WBWapperTest" %>

<html>
<head runat="server">
    <title>显示IE的打印预览</title>
    <script type="text/javascript">
        var WebBrowserWrapperInstance;

        function onBtnClick() {
            //var wrapper = $find("testWapper");
            WebBrowserWrapperInstance = WebBrowserWrapperFactory.Create();
            var Web2 = WebBrowserWrapperFactory.Create();

            alert(WebBrowserWrapperInstance == Web2)

            WebBrowserWrapperInstance.preview();
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <res:WebBrowserWrapper ID="testWapper" runat="server" />
        <br />
        <input id="Button1" type="button" onclick="onBtnClick();" value="Preview" runat="server" /><br />
    </div>
    </form>
</body>
</html>
