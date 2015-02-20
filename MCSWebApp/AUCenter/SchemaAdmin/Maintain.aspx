<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Maintain.aspx.cs" Inherits="AUCenter.SchemaAdmin.Maintain" %>

<%@ Register Src="../inc/Banner.ascx" TagName="Banner" TagPrefix="au" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>管理</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true" />
    <div class="pc-frame-header">
        <au:Banner ID="Banner1" runat="server" />
    </div>
    <div class="pc-frame-container">
        <div class="pc-container5">
            <fieldset>
                <legend>快照数据</legend>
                <div>
                    <div style="display: none">
                        <asp:Button ID="btnGenSchemaTable" runat="server" OnClick="GenSchemaTable" /><asp:Button
                            ID="btnGenSnap" runat="server" OnClick="GenSnapshot" />
                    </div>
                    <button type="button" class="pc-button" id="btnGenSchemaTrigger" title="当更改了配置文件后，请使用此功能重新生成Schema表。">
                        生成Schema表
                    </button>
                    <button type="button" id="btnGen" class="pc-button" title="当查询数据与实际情况不一致时，使用此按钮重新生成快照表。生成失败时，可能影响用户登录，请联系DBA。">
                        重新生成快照表<span id="iconGen" class="" style="vertical-align: middle">&nbsp;</span>
                    </button>
                    <span id="prompt1" class="pc-prompt"></span>
                </div>
                <div>
                </div>
            </fieldset>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        $pc.bindEvent("btnGen", "click", function () {

            function restore() {
                $pc.removeClass("iconGen", "pc-icon-loader");
                $pc.removeClass("btnGen", "disabled");
                $pc.get("btnGen").disabled = false;
            }

            if (confirm('这可能会暂时影响其他用户使用，确认要继续吗？\r\n此操作可能需要较长时间，如果出现错误，请联系DBA。')) {
                $pc.addClass("iconGen", "pc-icon-loader");
                $pc.get("btnGen").disabled = true;
                $pc.addClass("btnGen", "disabled");
                $pc.setText("prompt1", "请稍候。生成快照期间，无法做其他操作。请耐心等待操作完成，期间请勿关闭此窗口，直到操作结束。");
                var img = new Image();
                img.onload = function () {
                    restore();
                    $pc.setText("prompt1", "生成快照结束");
                }

                img.onerror = function (e) {
                    restore();
                    $pc.setText("prompt1", "生成快照遇到错误。这种情况下，请联系DBA在数据库中执行生成快照，否则所有用户可能无法正常使用。");
                }

                img.src = "Maintain.aspx?action=genSnapshot&magic=" + new Date();
            }
        });

        $pc.bindEvent("btnGenSchemaTrigger", "click", function () {
            if (confirm("确定要重新生成吗？")) {
                document.getElementById('<%=btnGenSchemaTable.ClientID %>').click();
            }
        });
    </script>
</body>
</html>
