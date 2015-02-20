<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OriginalDelegationEdit.aspx.cs"
    Inherits="MCS.OA.CommonPages.DelegationAuthorized.OriginalDelegationEdit" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>委托授权</title>
    <link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/style.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function refreshParent() {
            top.returnValue = "reload";
            top.close();
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server" target="innerFrame">
    <div>
        <HB:DataBindingControl runat="server" ID="bindingControl" AutoValidate="true" IsValidateOnSubmit="true">
            <ItemBindings>
                <HB:DataBindingItem ControlID="StartTime" DataPropertyName="StartTime" ControlPropertyName="Value" />
                <HB:DataBindingItem ControlID="EndTime" DataPropertyName="EndTime" ControlPropertyName="Value" />
            </ItemBindings>
        </HB:DataBindingControl>
    </div>
    <div id="dcontainer">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;" border="0">
            <tr style="height: 45px;">
                <td valign="top">
                    <div id="dheader">
                        <h1>
                            委托授权定义
                        </h1>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="dcontent">
                        <table cellspacing="0" cellpadding="0" style="height: 100%; width: 96%;" border="0">
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    被委托用户：
                                </td>
                                <td>
                                    <HB:OuUserInputControl ID="DelegatedUserInput" runat="server" MultiSelect="false"
                                        Width="200px" SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    委托开始时间：
                                </td>
                                <td>
                                    <MCS:DeluxeCalendar ID="StartTime" runat="server" Width="100px">
                                    </MCS:DeluxeCalendar>
                                    00:00:00
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    委托结束时间：
                                </td>
                                <td>
                                    <MCS:DeluxeCalendar ID="EndTime" runat="server" Width="100px">
                                    </MCS:DeluxeCalendar>
                                    00:00:00
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 80px;" valign="middle">
                    <div id="dfooter">
                        <p style="vertical-align: middle; height: 40px;">
                            <asp:Button AccessKey="S" ID="BtnSave" CssClass="formButton" runat="server" Width="85" Text="保存(S)" OnClick="BtnSave_Click" />
                            <input accesskey="C" class="formButton" id="btnClose" onclick="top.close();" type="button"
                                value="关闭(C)" name="btnClose"/>
                        </p>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
    <iframe style="display: none" id="innerFrame" name="innerFrame"></iframe>
</body>
</html>
