<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DateTimePickerTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DateTimePicker.DateTimePickerTest" %>

<!DOCTYPE html>
<html lang="zh">
<head runat="server">
    <title>日期选择器测试页</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../css/form.css" rel="stylesheet" type="text/css" />
    <style>
        .box
        {
            margin-bottom: 20px;
            position: relative;
        }
        
        .blue-border
        {
            border-color: #00acec !important;
        }
        
        .box .box-header
        {
            font-size: 21px;
            font-weight: 200;
            line-height: 30px;
            padding: 10px 15px;
        }
        
        .box .box-header.red-background
        {
            color: white;
            background-color: #f34541 !important;
        }
        .box .box-content
        {
            padding: 10px;
            border: 1px solid #dddddd;
            background: white;
            display: block;
            -webkit-box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
            -moz-box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
        }
        
        hr
        {
            position: relative;
            margin-left: -15px;
            margin-right: -15px;
            border-top: 1px solid #eeeeee;
            border-bottom: 1px solid white;
        }
        hr.hr-normal
        {
            margin-left: 0;
            margin-right: 0;
        }
        
        .input-group-addon
        {
            cursor: pointer;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="box bordered-box blue-border box-nomargin" >
        <div class="box-header red-background">
            <div class="title">
                Datetime pickers
            </div>
        </div>
        <div class="box-content">
            <hr class="hr-normal" />
            <p>
                <strong>Datepicker</strong>
            </p>
            <div>
                <res:DateTimePicker runat="server" ID="serverdatepicker" Mode="DatePicker" />
            </div>
            <hr class="hr-normal" />
            <p>
                <strong>Timepicker</strong>
            </p>
            <div>
                <div>
                    <res:DateTimePicker runat="server" ID="serverTimePickeer" ToolTip="abc" OnClientErrorDate="HandleClientError"
                        Mode="TimePicker" />
                </div>
            </div>
            <hr class="hr-normal" />
            <p>
                <strong>Datetimepicker</strong>
            </p>
            <div>
                <div>
                    <res:DateTimePicker runat="server" ID="serverdatetimepicker" Mode="DateTimePicker" />
                </div>
            </div>
            <hr class="hr-normal" />
            <p>
                <strong>Datetimepicker As component</strong>
            </p>
            <div>
                <div>
                    <res:DateTimePicker runat="server" ID="DateTimePicker1" Mode="DateTimePicker" AsComponent="true" />
                </div>
            </div>
            <hr class="hr-normal" />
            <p>
                <strong>Free Test </strong>
            </p>
            <div>
                <div>
                    <res:DateTimePicker runat="server" ID="freePicker" Mode="DateTimePicker" />
                </div>
                <div class="form-table clearfix">
                    <div class="form-group form-table-row">
                        <div class="form-table-value">
                            <select class="form-control" id="ppts" style="float: left">
                            </select>
                        </div>
                        <div class="form-table-value">
                            <input type="text" class="form-control" id="inputValue" style="float: left" />
                        </div>
                        <div class="form-table-value" style="float: left">
                            <input type="button" class="form-control" value="应用" id="apply" />
                        </div>
                    </div>
                </div>
                <div>
                    <asp:Button Text="Submit" runat="server" OnClick="DoSubmit" />
                    <input type="text" runat="server" id="inputDate" />
                    <asp:Button Text="服务器设置日期" runat="server" OnClick="serverSetDate" />
                    <div>
                        文种
                        <asp:DropDownList runat="server" ID="cultureList" AutoPostBack="True" OnSelectedIndexChanged="cultureList_SelectedIndexChanged">
                            <asp:ListItem Text="英语(美国)" Value="en-US" />
                            <asp:ListItem Text="中文(简体)" Value="zh-Hans" />
                        </asp:DropDownList>
                    </div>
                    <div>
                        <div>
                            <asp:Literal Text="" runat="server" ID="ltDate" EnableViewState="false" />
                        </div>
                        <div>
                            <asp:Literal Text="" runat="server" ID="ltTime" EnableViewState="false" />
                        </div>
                        <div>
                            <asp:Literal Text="" runat="server" ID="ltDateTime" EnableViewState="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            var opt = $get("ppts");
            var ctrl = $find("freePicker");
            if (ctrl) {
                var pattern = /^set_.*/;
                for (var attr in $HGRootNS.DateTimePicker.prototype) {
                    if (pattern.test(attr)) {
                        var option = document.createElement("option");
                        option.value = attr.substring(4);
                        opt.appendChild(option);
                        option.appendChild(document.createTextNode(option.value));
                    }
                }
            }

            $("#ppts").change(function () {

                var attr = $(this).val()
                var ctrl = $find("freePicker");
                var val = ctrl["get_" + attr]();

                var strVal = val;

                if (val == null) {
                    strVal = "null";
                } else if (typeof (val) === "string") {
                    strVal = "\"" + val.replace("\"", "\\\"") + "\"";
                } else if (val instanceof Date) {
                    if (Date.isMinDate(val))
                        strVal = "Date.minDate";
                    else
                        strVal = "new Date(\"" + val.format("yyyy-MM-dd HH:mm:ss") + "\")";
                }

                $("#inputValue").val(strVal);
            });

            $("#apply").click(function () {
                var attr = $("#ppts").val();
                if (attr) {
                    var val = eval($("#inputValue").val());
                    var ctrl = $find("freePicker");
                    try {
                        ctrl["set_" + attr](val);
                    } catch (e) {
                        alert(e.message);
                    }
                }
            });

            $("#inputDate").val(new Date().toDateString());
        });

        function HandleClientError(s, e) {
            alert("输入了错误的值");
        }
    </script>
</body>
</html>
