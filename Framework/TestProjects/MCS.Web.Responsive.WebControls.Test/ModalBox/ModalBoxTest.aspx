<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModalBoxTest.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test.ModalBox.ModalBoxTest" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>模态窗口</title>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#simpleDialog").click(function () {
                var options = {
                    title: "委托授权定义",
                    width: 600,
                    height: 350,
                    onOk: function () {
                        alert($("#delegationBeginTime").val());
                    },
                    control: {
                        id: 'delagateUser',
                        clone: false
                    }
                };
                $HGModalBox.show(options);
            });
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="row" style="width: 90%">
        <div class="col-md-3 col-sm-3 col-xs-3">
            <a href="javascript:void(0)" class="btn btn-primary bt-lg" data-toggle="modal" id="simpleDialog">
                简单对话框 </a>
        </div>
        <%--<div class="col-md-3 col-sm-3 col-xs-3">
            <a href="javascript:void(0)" class="btn btn-primary bt-lg" data-toggle="modal" id="scrollGrid">滚动列表对话框
            </a>
        </div>
        <div class="col-md-3 col-sm-3 col-xs-3">
            <a href="javascript:void(0)" class="btn btn-primary bt-lg" data-toggle="modal" id="btnlayout">左右布局对话框
            </a>
        </div>
        <div class="col-md-3 col-sm-3 col-xs-3">
            <a href="javascript:void(0)" class="btn btn-primary bt-lg" data-toggle="modal" id="btntabSearch">标签页与高级搜索
            </a>
        </div>--%>
    </div>
    <div class="container">
    </div>
    <!-- 简单对话框 -->
    <div id="delagateUser" class="modal-body" style="display: none">
        <div class="form-group">
            <label class="col-md-3 control-lable" for="delegationBeginTime">
                委托开始时间：</label>
            <div class="col-md-9">
                <div class="input-group">
                    <input type="datetime" class="form-control" id="delegationBeginTime" />
                </div>
            </div>
        </div>
        <br />
        <br />
        <div class="form-group">
            <label class="col-md-3 control-lable" for="delegationEndTime">
                委托结束时间：</label>
            <div class="col-md-9">
                <div class="input-group">
                    <input type="datetime" class="form-control" id="delegationEndTime" />
                </div>
            </div>
        </div>
        <br />
        <br />
    </div>
    <!-- /简单对话框 -->
    </form>
</body>
</html>
