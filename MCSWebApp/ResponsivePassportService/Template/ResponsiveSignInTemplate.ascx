<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResponsiveSignInTemplate.ascx.cs"
    Inherits="ResponsivePassportService.Template.ResponsiveSignInTemplate" %>
<link href="../Resources/Bootstrap/css/bootstrap.css" rel="stylesheet" type="text/css" />
<link href="../Resources/Bootstrap/css/bootstrap-theme.css" rel="stylesheet" type="text/css" />
<link href="../Resources/Styles/layout.css" rel="stylesheet" type="text/css" />
<link href="../Resources/Styles/elements.css" rel="stylesheet" type="text/css" />
<link href="../Resources/Font-awesome/css/font-awesome.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
    function onLoginClick() {
        window.open("QQLogin.aspx", "TencentLogin",
				"width=480,height=420,menubar=0,scrollbars=1,resizable=1,status=1,titlebar=0,toolbar=0,location=1");

        return false;
    }

    function onBridgeButtonClick() {
        var template = document.getElementById("bindingUrlTemplate");
        window.location = template.href + "&" + document.getElementById("bridgeDataField").value;
    }
</script>
<style type="text/css">
    body {
        padding-top: 40px;
        padding-bottom: 40px;
        background-color: #f5f5f5;
    }

    .form-signin {
        max-width: 450px;
        padding: 19px 29px 29px;
        margin: 0 auto 20px;
        background-color: #fff;
        border: 1px solid #e5e5e5;
        -webkit-border-radius: 5px;
        -moz-border-radius: 5px;
        border-radius: 5px;
        -webkit-box-shadow: 0 1px 2px rgba(0,0,0,.05);
        -moz-box-shadow: 0 1px 2px rgba(0,0,0,.05);
        box-shadow: 0 1px 2px rgba(0,0,0,.05);
    }

        .form-signin .form-signin-heading, .form-signin .checkbox {
            margin-bottom: 10px;
        }
</style>
<div class="hidden">
    <asp:Literal runat="server" ID="bindingUrlContainer"></asp:Literal>
    <input id="bridgeDataField" type="hidden" />
    <input id="bridgeButton" type="button" onclick="onBridgeButtonClick();" />
</div>
<div class="container">
    <div class="form-signin">
        <div class="row">
            <h2>用户认证</h2>
            <div class="form-horizontal" role="form">
                <div class="form-group">
                    <label for="txtUserName" class="col-lg-2 col-md-2 col-sm-3 control-label">
                        用户名</label>
                    <div class="col-lg-10 col-md-10 col-sm-9 col-xs-12">
                        <asp:TextBox runat="server" CssClass="form-control" ID="signInName" placeholder="用户名" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="txtPassword" class="col-lg-2 col-md-2 col-sm-3 control-label">
                        密码</label>
                    <div class="col-lg-10 col-md-10 col-sm-9 col-xs-12">
                        <input type="password" runat="server" class="form-control" id="password" placeholder="口令" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="culture" class="col-lg-2 col-md-2 col-sm-3 control-label">
                        语言</label>
                    <div class="col-lg-10 col-md-10 col-sm-9 col-xs-12">
                        <select class="form-control" id="culture" runat="server">
                            <option value="zh-CN">中文</option>
                            <option value="en-US">English</option>
                        </select>
                    </div>
                </div>
                <div class="form-group">
                    <label for="tenantCode" class="col-lg-2 col-md-2 col-sm-3 control-label">
                        租户</label>
                    <div class="col-lg-10 col-md-10 col-sm-9 col-xs-12">
                        <asp:TextBox runat="server" CssClass="form-control" ID="tenantCode" placeholder="租户" />
                    </div>
                </div>
                <div class="form-group" runat="server" id="simulateDateRow">
                    <label for="simulateDate" class="col-lg-2 col-md-2 col-sm-3 control-label">
                        模拟</label>
                    <div class="col-lg-10 col-md-10 col-sm-9 col-xs-12">
                        <resp:DateTimePicker runat="server" ID="simulateDate" Mode="DateTimePicker" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-2 col-md-2 col-sm-3">
                </div>
                <div class="col-lg-10 col-md-10 col-sm-9">
                    <asp:CheckBox runat="server" ID="autoSignIn" Text="下次自动登录" />
                </div>
            </div>
            <div class="row">
                <div class="col-lg-2 col-md-2 col-sm-3 ">
                </div>
                <div class="col-lg-10 col-md-10 col-sm-9">
                    <asp:CheckBox runat="server" ID="dontSaveUserName" Text="请不要记录我的登录名" />
                </div>
            </div>
            <div class="row">
                <asp:Label CssClass="text-danger" ID="errorMessage" runat="server" Style="line-height: 100%; word-break: break-all"></asp:Label>
                <a href="#" id="detailErrorMessageLink" runat="server" style="display: none; line-height: 100%">点击此处展开详细的错误信息...</a>
                <asp:Label ID="detailErrorMessage" runat="server" Style="display: none; word-break: break-all"></asp:Label>
            </div>
            <div class="row">
                <div class="col-md-2 col-sm-1 col-xs-1">
                </div>
                <div class="col-md-8 col-sm-10 col-xs-10">
                    <asp:Button runat="server" ID="SignInButton" CssClass="form-control btn btn-large btn-primary btn-default"
                        Text="用户登录" />
                </div>
                <div class="col-md-2 col-sm-1 col-sm-1">
                </div>
            </div>
            <div class="row">
                <div class="col-md-2 col-sm-1 col-xs-1">
                </div>
                <div class="col-md-8 col-sm-10 col-xs-10">
                    <a class="btn" href="#" onclick="return onLoginClick();" target="TencentLogin">
                        <img src="../images/QQ/Connect_logo_4.png" />
                    </a>
                </div>
                <div class="col-md-2 col-sm-1 col-sm-1">
                </div>
            </div>
        </div>
    </div>
</div>
