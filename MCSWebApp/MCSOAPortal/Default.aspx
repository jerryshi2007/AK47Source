<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MCS.OA.Portal.Default" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>远洋地产流程中心</title>
	<meta http-equiv="X-UA-Compatible" content="IE=7" />
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312">
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<script language="c#" runat="server">
    protected string redirectUrl()
    {
        string url="";
        switch (Request.QueryString["todo"])
        { 
            case null:
                url = "TaskList/UnCompletedTaskList.aspx";
                break;
            case "1":
                url = "TaskList/UnCompletedTaskList.aspx";
                break;
            case "2":
                url = "../MCS.OA.stat/Query/FormQueryList.aspx";
                break;
            default:
                url = "TaskList/UnCompletedTaskList.aspx";
                break;
        }
        return url;
    }
</script>
<frameset id="frmTool" border="0" framespacing="0" rows="167,*,0" frameborder="NO">                         
	    <frame name="header" id="header" src="./frames/header.aspx" noresize="noresize" scrolling="no" />
	    <frameset id="frmMenu" border="0" framespacing="0" frameborder="NO" cols="178,*">
		    <frame id="left" name="left" src="./frames/left.aspx" noresize="noresize" />
		    <frame name="content" src='<% =redirectUrl()%>' noresize="noresize" />
	    </frameset>
	    <noframes>
		    <body>
			    <p>
				    您的浏览器不支持Frame格式，请下载新版本。
			    </p>
		    </body>
	    </noframes>
</frameset>
</html>
