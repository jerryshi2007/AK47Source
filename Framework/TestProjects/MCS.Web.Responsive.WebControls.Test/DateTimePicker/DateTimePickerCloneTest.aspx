<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DateTimePickerCloneTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DateTimePicker.DateTimePickerCloneTest" %>

<!DOCTYPE html>
<html lang="zh">
<head runat="server">
    <title>日期控件克隆</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../css/form.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
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
                    <res:DateTimePicker runat="server" ID="picker" Mode="DatePicker" Value="2013-3-3" />
                </div>
                <ul id="list">
                </ul>
                <div>
                    <button id="btnClone" type="button">
                        克隆</button>
                </div>
            </div>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnClone").click(function () {
                var li = document.createElement("li");
                $get("list").appendChild(li);
                $find("picker").cloneAndAppendToContainer(li);
            });
        });

    </script>
</body>
</html>
