<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpressionHint.aspx.cs"
    Inherits="PermissionCenter.dialogs.ExpressionHint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hint</title>
    <link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .datatype-label
        {
            padding: 0 10px;
            color: #39f;
        }
    </style>
</head>
<body class="pcdlg">
    <form id="form1" runat="server">
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            表达式编写提示
        </h1>
    </div>
    <div class="pcdlg-content">
        <div class="pc-container5">
            <p>
                &nbsp;</p>
            <p>
                <b>取对象属性的值：</b>
            </p>
            <samp>
                <em>
                    <asp:Literal runat="server" ID="lblSchemaName" Mode="Encode"></asp:Literal></em>.<em>属性名</em></samp>
            <dl>
                <dd>
                    <b>可用的属性:</b></dd>
                <asp:Repeater runat="server" ID="propRepeater">
                    <ItemTemplate>
                        <dt><em>
                            <%#HttpUtility.HtmlEncode ((string) Eval("Name")) %></em><span class="datatype-label">
                                <%#Eval("DataType").ToString() %>
                            </span>
                            <%#HttpUtility.HtmlEncode ((string) Eval("Description")) %></dt>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <button class="pcdlg-button" type="button" onclick="window.close();">
                关闭
            </button>
        </div>
    </div>
    </form>
</body>
</html>
