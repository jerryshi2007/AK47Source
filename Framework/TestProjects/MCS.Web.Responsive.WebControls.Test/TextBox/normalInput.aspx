<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="normalInput.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test.TextBox.normalInput" %>

<html>
<head runat="server">
    <title>一般输入框测试</title>
</head>
<body>
    <form id="serverForm" runat="server" role="form" class="form-horizontal">
    <div class="form-group">
        <label for="inputEmail3" class="col-sm-2 control-label">
            Email</label>
        <div class="col-sm-5">
            <input type="email" class="form-control" id="inputEmail3" placeholder="Email">
        </div>
    </div>
    <div class="form-group">
        <label for="inputPassword3" class="col-sm-2 control-label">
            Password</label>
        <div class="col-sm-10">
            <input type="password" class="form-control" id="inputPassword3" placeholder="Password">
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <div class="checkbox">
                <label>
                    <input type="checkbox">
                    Remember me
                </label>
            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">
                Sign in</button>
        </div>
    </div>
    </form>
</body>
</html>
