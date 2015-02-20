<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropertyFormTestForMaterialEditor.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.PropertyFormTestForMaterialEditor" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
      <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" />
          <asp:Button ID="btnPostBack" runat="server" Text="PostBack" 
              onclick="btnPostBack_Click" />
    </div>
    </form>
</body>
</html>
