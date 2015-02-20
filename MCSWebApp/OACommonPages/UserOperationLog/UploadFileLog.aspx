<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadFileLog.aspx.cs"
    Inherits="MCS.OA.CommonPages.UserOperationLog.UploadFileLog" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Ajax.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/ItemDetail.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Login.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="dcontainer">
        <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;" border="0">
            <tr style="height: 45px;">
                <td valign="top">
                    <div id="dheader">
                        <h1>
                            详细上传文件日志
                        </h1>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="dcontent">
                        <table cellspacing="0" cellpadding="0" style="height: 100%; width: 96%;">
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    标题：
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lb_ProgramName"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    应用名称：
                                </td>
                                <td>
                                    <asp:Literal ID="lb_ApplicationName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    原文件名称：
                                </td>
                                <td>
                                    <asp:Literal ID="lb_OriginalFileName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    当前文件名：
                                </td>
                                <td>
                                    <asp:Literal ID="lb_CurrentFileName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    操作人：
                                </td>
                                <td>
                                    <asp:Literal ID="lb_Operator" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    状态说明：
                                </td>
                                <td>
                                    <asp:Literal ID="lb_StatusText" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="fim_l" style="width: 100px;">
                                    操作时间：
                                </td>
                                <td>
                                    <asp:Literal ID="lb_CreateTime" runat="server"></asp:Literal>
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
                            <input accesskey="C" type="button" class="portalButton" value="关闭(C)" onclick="top.close();" />
                        </p>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
