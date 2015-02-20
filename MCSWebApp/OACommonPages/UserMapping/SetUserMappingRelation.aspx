<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetUserMappingRelation.aspx.cs" Inherits="MCS.OA.CommonPages.UserMapping.SetUserMappingRelation" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <base target="_self" />
    <title></title>
    <script type="text/javascript">
     function refreshParent() 
        {
            top.returnValue = "reload";
            top.close();
        }
        </script>
</head>
<body>
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;" border="0">
            <tr style="height: 45px;">
                <td valign="top">
                    <div id="dheader">
                        <h1>
                            设置关系
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
                                    内网用户：
                                </td>
                                <td>
                                    <HB:OuUserInputControl ID="DelegatedUserInput" runat="server" MultiSelect="false"
                                        Width="200px" SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
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
    </form>
</body>
</html>
