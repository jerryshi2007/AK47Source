<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test1.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.PopupTip.Test1" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">    
            <table width="400" align="left" style="margin-top: 120px" border="1px">
        <tr>
            <td colspan="2">
                用户名:
            </td>
            <td colspan="2">
                <asp:TextBox ID="TextBox1" runat="server" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                密码:
            </td>
            <td colspan="2">
                <asp:TextBox ID="TextBox2" runat="server" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                邮件地址
            </td>
            <td colspan="2">
                <asp:TextBox ID="TextBox3" runat="server" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                手机号码
            </td>
            <td colspan="2">
                <asp:TextBox ID="TextBox4" runat="server" Width="300"></asp:TextBox>
                
            </td>
        </tr>
                <tr>
                    <td colspan="2">
                        Message号码
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="TextBox5" runat="server" Width="300"></asp:TextBox>
                    </td>
                </tr>                  
    </table>
    <table width="600" style="margin-top: 100px" border="1px">
    <tr>
        <td colspan="4" id="container1"></td>
    </tr>
    <tr>
        <td colspan="4" id="container2"></td>
    </tr>
    <tr>
        <td colspan="4" id="container3"></td>
    </tr>
    </table> 
    <cc1:PopupTip ID="PopupTip2" runat="server" Reference="TextBox2" IsPopup="False" FixedContainer="container1" TipName="yy"  />
    <cc1:PopupTip ID="PopupTip3" runat="server" Reference="TextBox3" IsPopup="False" FixedContainer="container2"  TipName="xx"/>
    <cc1:PopupTip ID="PopupTip4" runat="server" Reference="TextBox4" IsPopup="False" FixedContainer="container3" PreLoadContent="True" TipName="zz"/>
       <cc1:PopupTip ID="PopupTip5" runat="server" Reference="TextBox5"  PopupPostion="Down"  PopupStyle="BottomDialog" TimerPopShow="108000"
        HtmlContent="<b>鸟</b>儿飞过旷野。一批又一批成,群的鸟儿接连不断地飞了过去。"  />
    <cc1:PopupTip ID="PopupTip1" runat="server" Reference="TextBox1" PopupPostion="Up"
        PopupStyle="TopDialog" TimerPopShow="108000" HtmlContent="鸟儿飞过旷野。一批又一批成,群的鸟儿接连不断地飞了过去。" />
    </form>
</body>
</html>
