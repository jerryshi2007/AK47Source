<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ZyTestMaterial.aspx.cs" Inherits="MCS.OA.CommonPages.ZyTestMaterial" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
		<SOA:MaterialControl ID="MaterialControl2" runat="server" RootPathName="GenericProcess"
			DefaultClass="NormalText" AllowEdit="true" AllowEditContent="true" MaterialTableShowMode="Inline"
			MaterialUseMode="UploadFile" />
		<asp:Button ID="btnSaveMaterialControl2" runat="server" Text="save" OnClick="btnSaveMaterialControl2_Click" />
    </form>
</body>
</html>
