<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommandInputTest.aspx.cs" Inherits="MCS.SOA.Web.WebControls.Test.CommandInput.CommandInputTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
        <script language="javascript" type="text/javascript">
            var commandInputID = '<asp:Literal ID="Literal1" runat="server"></asp:Literal>';
            function onClick()
            {
                document.getElementById(commandInputID).value = document.getElementById("textInput").value;
            }
            
            function onCommandInput(commandInputControl, e)
            {                
                switch (e.commandValue)
                {
                    case "close":                        
                        e.stopCommand = true;//设置后，不再执行默认的处理
                        alert("close命令终止");
                        break;
                        
                    case "refresh":
                        alert("刷新");
                        //执行默认的处理
                        break;
                        
                    case "command001":
                        //没有默认的处理
                        alert("command001");
                        break;                        
                }                
            }
        </script>
 </head>
<body>
    <form id="form1" runat="server">
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
    <div>
        <cc1:commandinput id="CommandInput1" runat="server" OnClientCommandInput="onCommandInput"></cc1:commandinput>

        输入命令：<input type="text" id="textInput" /><input type="button" onclick="onClick()" value="确认" />
     </div>
     <input type="button" value="打开窗口" onclick="window.open('CommandInputOpen.aspx')" />
    </form>
</body>
</html>
