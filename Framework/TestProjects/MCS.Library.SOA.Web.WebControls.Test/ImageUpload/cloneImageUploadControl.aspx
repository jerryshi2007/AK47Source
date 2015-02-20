<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cloneImageUploadControl.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.ImageUpload.cloneImageUploadControl" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function onCloneComponent() {
            var parent = $get("container");

            var template = $find("imgUploader");

            template.cloneAndAppendToContainer(parent);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<input type="button" onclick="onCloneComponent();" value="Clone Component" />
		
	</div>

  	<div style="">
		<SOA:ImageUploader runat="server" ID="imgUploader" FileMaxSize="1024000"
			 Width="200" Height="350" ImageHeight="300" ImageWidth="200"  ResourceID="xyz" ReadOnly="false" AutoUpload="true" />
	</div>
    <div id="container">
	</div>
    </form>
</body>
</html>
