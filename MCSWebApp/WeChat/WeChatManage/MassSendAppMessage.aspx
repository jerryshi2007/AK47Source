<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MassSendAppMessage.aspx.cs"
    Inherits="WeChatManage.MassSendAppMessage" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="overflow: auto">
<head runat="server">
    <title>群发图文消息</title>
    <%--<link rel="stylesheet" type="text/css" href="css/form.css" />--%>
    <script src="jquery/jquery-1.10.2.js" type="text/javascript"></script>
    <script type="text/javascript">
        function checkMember() {
            var groupID = $("#ddlGroups").val();
            if (groupID == "-1") {
                alert("请选择条件组");
            } else {
                var url = "ModalDialogs/CheckGroupMembers.aspx?tt=" + Date.parse(new Date()) + "&groupID=" + groupID;
                window.showModalDialog(url, null, "dialogWidth=800px;dialogHeight=500px");
            }
        }
    </script>
</head>
<body style="overflow: auto">
    <form id="form1" runat="server" target="innerFrame">
    <div id="" style="text-align: left;">
        <div>
            <table style="border: 1px solid #ccc; width: 100%;" cellspacing="1" cellpadding="0">
                <tr style="display: none">
                    <td style="width: 100px;">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="lefttitle" align="center" colspan="2">
                        <asp:Label ID="lbTaskName" Text="群发图文消息" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="lefttitle">
                        <h3>
                            <img id="icon" runat="server" src="~/img/icon_01.gif" alt="" style="width: 14px;
                                height: 16px; vertical-align: middle" />
                            发送对象
                        </h3>
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle" width="100px">
                        选择并发送
                    </td>
                    <td class="">
                        <asp:DropDownList ID="ddlGroups" runat="server">
                        </asp:DropDownList>
                        <a href="javascript:void(0);" onclick="checkMember();">查看</a>
                        <asp:DropDownList ID="ddlAccount" runat="server">
                        </asp:DropDownList>
                        <soa:SubmitButton ID="btnSend" Text="发送" PopupCaption="正在发送..." runat="server" ProgressMode="BySteps"
                            OnClick="btnSend_Click" />
                    </td>
                </tr>
                <tr>
                    <td class="lefttitle" style="font-size: 12px; font-weight: normal;" colspan="2">
                        <h3>
                            <asp:Image ID="Image1" ImageUrl="~/img/icon_01.gif" runat="server" />
                            消息内容
                        </h3>
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        标题
                    </td>
                    <td class="normaltext">
                        <soa:HBTextBox ID="txtTitle" runat="server" Width="320px"></soa:HBTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        作者
                    </td>
                    <td class="normaltext">
                        <soa:HBTextBox ID="txtAuthor" runat="server" Width="320px"></soa:HBTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        封面
                    </td>
                    <td>
                        <soa:ImageUploader Style="overflow: hidden" runat="server" ID="imgUploader" Width="320"
                            Height="240" ImageHeight="200" ImageWidth="320" ResourceID="xyz" ReadOnly="false"
                            AutoUpload="True" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        封面选项
                    </td>
                    <td class="">
                        <asp:CheckBox ID="chkShowInContent" runat="server" Text="封面图片显示在正文中" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        摘要
                    </td>
                    <td class="normaltext">
                        <textarea id="txtDigest" cols="40" rows="5" runat="server"></textarea>
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        内容
                    </td>
                    <td>
                        <soa:UEditorWrapper runat="server" ID="editor" Width="600px" InitialData="" ReadOnly="false"
                            AutoDownloadUploadImages="False" AutoUploadImages="False" RootPathName="ImageUploadRootPath"
                            Toolbars="|,Undo,Redo,|,
Bold,Italic,Underline,StrikeThrough,Superscript,Subscript,RemoveFormat,FormatMatch,|,
BlockQuote,|,PastePlain,|,ForeColor,BackColor,InsertOrderedList,InsertUnorderedList,|,CustomStyle,
Paragraph,RowSpacing,LineHeight,FontFamily,FontSize,|,
DirectionalityLtr,DirectionalityRtl,|,Indent,|,
JustifyLeft,JustifyCenter,JustifyRight,JustifyJustify,|,
Link,Unlink,Anchor,|,InsertFrame,PageBreak,HighlightCode,|,
Horizontal,Date,Time,Spechars,|,
InsertTable,DeleteTable,InsertParagraphBeforeTable,InsertRow,DeleteRow,InsertCol,DeleteCol,MergeCells,MergeRight,MergeDown,SplittoCells,SplittoRows,SplittoCols,|,
SelectAll,ClearDoc,SearchReplace,Print,Preview,CheckImage,WordImage,InsertImage,|" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldtitle">
                        原文链接
                    </td>
                    <td class="normaltext">
                        <soa:HBTextBox ID="txtSourceUrl" runat="server" Width="600px"></soa:HBTextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div style="display: none">
        <iframe id="innerFrame" name="innerFrame"></iframe>
    </div>
    </form>
</body>
</html>
