<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyFormValidatorTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.PropertyFormValidatorTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>property form 验证测试</title>
    <script type="text/javascript">
        function onPostClick() {
            try {
                var reValue = $HGRootNS.PropertyEditorControlBase.ValidateProperties();

                if (!reValue.isValid)	//返回是否通过
                    throw reValue.errorMessages;
            }
            catch (e) {
                alert(e);
            }

            //$HGRootNS.PropertyEditorControlBase.ValidateProperties();
        }
    </script>
    <link href="PropertyForm.css" rel="styleproertyform" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
    <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" Height="100px" CssClass="styleproertyform" />
    </div>
    <p>
        <input type="button" value="Click Validator" style="width: 120px" onclick="onPostClick();" />
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" Text="Server Validator" OnClick="Button1_Click" />
    </p>
    </form>
</body>
</html>
