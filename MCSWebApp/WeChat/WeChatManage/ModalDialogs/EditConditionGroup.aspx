<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditConditionGroup.aspx.cs" Inherits="WeChatManage.ModalDialogs.EditConditionGroup" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self"/>
    <title>编辑条件组</title>
     <link href="/MCSWebApp/Css/basePage.css" type="text/css" rel="stylesheet" />
    <link href="/MCSWebApp/Css/baseForm.css" type="text/css" rel="stylesheet" />
    <link href="/MCSWebApp/Css/toolbar.css" type="text/css" rel="stylesheet" />
    <script src="../jquery/jquery-1.10.2.js" type="text/javascript"></script>
    <script type="text/javascript">
        function onCheck() {
            var url = '../CommonHandler.ashx';
            if ($("#txtCondition").val() == "") {
                alert("请输入表达式！");
                return;
            }
            jQuery.post(url,
				{ express: $("#txtCondition").val() },
				function (rtn) {
				    var rtnObj = jQuery.parseJSON(rtn);
				    if (eval(rtnObj.Success)) {
				        alert('表达式设置正确！');
				    }
				    else {
				        alert('表达式设置错误！原因：' + rtnObj.Message);
				    }
				});
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height: 100%; width: 100%;">
        <table style="width: 100%; height: 100%">
            <tr>
                <td class="gridHead" style="height: 32px;">
                    <div class="dialogTitle">
                        <img src="../img/dialoglogo.png" alt="logo" style="vertical-align: middle" />
                        <span id="dialogHeaderText" class="title">编辑条件组</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; height: 130px">
                    <div class="dialogContent" style="width: 100%;
                        overflow: auto">
                        <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
                            <tr>
                                <td style="vertical-align: middle; text-align: center;">
                                    <div id="WorkItemNodeEditControlDialog_workitemNodeEditControlContainer" style="width: 95%;
                                        text-align: center;">
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="width: 50px; text-align: right">
                                                    组名称
                                                </td>
                                                <td style="text-align: left">
                                                    <soa:HBTextBox ID="txtGroupName" runat="server" MaxLength="6"></soa:HBTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 50px; text-align: right">
                                                    描述
                                                </td>
                                                <td style="text-align: left">
                                                   <soa:HBTextBox ID="txtDescript" runat="server"></soa:HBTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 50px; text-align: right;vertical-align:top">
                                                    条件
                                                </td>
                                                <td style="text-align: left">
                                                   <textarea id="txtCondition" cols="40" rows="10" runat="server"></textarea>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 50px; text-align: right">
                                                    &nbsp;
                                                </td>
                                                <td style="text-align:left">
                                                    <input type="button" style="margin-left: 0" value="校验" class="formButton" onclick="onCheck();"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="gridfileBottom">
                </td>
            </tr>
            <tr>
                <td style="height: 40px;">
                    <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
                        <tr>
                            <td style="text-align: center">
                                <asp:Button ID="btnSave" runat="server" Text="保存" onclick="btnSave_Click" Style="width: 96px" AccessKey="O" CssClass="formButton"/>
      
                            </td>
                            <td style="text-align: center">
                                <input name="cancelButton" type="button" id="cancelButton" accesskey="C" category="SOAWebControls"
                                    class="formButton" value="取消" style="width: 96px" onclick="top.close();" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
