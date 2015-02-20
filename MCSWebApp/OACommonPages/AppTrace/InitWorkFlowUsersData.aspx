<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InitWorkFlowUsersData.aspx.cs" Inherits="MCS.OA.CommonPages.AppTrace.InitWorkFlowUsersData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>同步流程数据人员</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <div>
	    <asp:Button ID="initWorkFlowUserData" runat="server" Text="同步流程数据人员" OnClick="InitWorkFlowUserData_Click" />
	  </div>
	  <div>
	  </div>
    </div>
    </form>
</body>
</html>
