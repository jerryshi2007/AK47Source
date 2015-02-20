<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AUScopesCondition.aspx.cs"
    Inherits="AUCenter.AUScopesCondition" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>管理范围</title>
    <link href="Styles/dlg.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
</head>
<body class="pcdlg">
    <form id="form1" runat="server" class="au-full">
    <au:SceneControl ID="SceneControl1" runat="server" />
    <div>
        <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
            <Services>
                <asp:ServiceReference Path="~/Services/ConditionSvc.asmx" />
            </Services>
        </asp:ScriptManager>
        <div class="pcdlg-sky">
            <h1 class="pc-caption">
                <img src="Images/icon_01.gif" alt="图标" />
                <span id="schemaLabel" runat="server"></span>\<span id="schemUnitLabel" runat="server"></span>-<span>管理范围</span>
                <span class="pc-timepointmark">
                    <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
                </span>
            </h1>
        </div>
        <div class="pcdlg-content">
            <div class="pc-container5">
                <asp:Repeater runat="server" ID="scopeRepeater">
                    <HeaderTemplate>
                        <ul id="scopelist" class="au-scope-menu clearfix">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class='<%# this.GetMenuCssClass((string)Container.DataItem) %>'><a href='<%# "AUScopes.aspx?schemaType=" + Container.DataItem +"&unitId=" + this.AdminUnitObject.ID %>'
                            target="_self">
                            <%# this.GetSchemaName ((string)Container.DataItem)  %></a></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:ListView ID="ListView1" runat="server">
                    <ItemTemplate>
                    </ItemTemplate>
                </asp:ListView>
            </div>
            <ul class="pc-tabs-header">
                <li>
                    <asp:HyperLink ID="lnkToConst" NavigateUrl="AUScopesConst.aspx" runat="server" Text="固定成员" />
                </li>
                <li class="pc-active">
                    <asp:HyperLink ID="lnkToCondition" NavigateUrl="AUScopesCondition.aspx" runat="server"
                        Text="条件成员" />
                </li>
                <li>
                    <asp:HyperLink ID="lnkToPreview" NavigateUrl="AUScopes.aspx" runat="server" Text="预览成员" />
                </li>
            </ul>
            <div class="pc-container5" style="overflow: auto">
                <div>
                    描述</div>
                <asp:TextBox runat="server" ID="txtDesc" />
                <div>
                    编辑条件</div>
                <asp:TextBox runat="server" ID="expression" MaxLength="1024" Columns="60" Rows="5"
                    Wrap="true" onfocus="handleChange()" TextMode="MultiLine" ToolTip="在此编辑条件" />
            </div>
            <div>
                <asp:Button ID="btnSave" Text="保存条件" runat="server" CssClass="pc-button" OnClick="SaveClick" />
                <input type="button" value="检查表达式" class="pc-button" onclick="checkExpression()" />
                <asp:CheckBox Text="保存后自动计算动态人员" runat="server" ID="chkAutoCalc" ViewStateMode="Disabled" />
                <span id="prompt" runat="server" class="pc-prompt"></span>
            </div>
            <div class="pc-container5">
                <p>
                    Hint</p>
                <p>
                    取管理范围对象属性的值：
                </p>
                <samp>
                    <em>
                        <asp:Literal runat="server" ID="lblSchemaName" Mode="Encode"></asp:Literal></em>.<em>属性值</em></samp>
                <dl>
                    <dd>
                        可用的属性:</dd>
                    <asp:Repeater runat="server" ID="propRepeater">
                        <ItemTemplate>
                            <dt><em>
                                <%#HttpUtility.HtmlEncode ((string) Eval("Name")) %></em>:<%#HttpUtility.HtmlEncode ((string) Eval("Description")) %></dt>
                        </ItemTemplate>
                    </asp:Repeater>
                </dl>
            </div>
        </div>
        <div class="pcdlg-floor">
            <div class="pcdlg-button-bar">
                <input type="button" value="关闭" class="pcdlg-button" onclick="window.close();" />
            </div>
        </div>
        <div style="display: none">
            <asp:Button runat="server" ID="btnRecalc" OnClick="Nop" />
            <soa:PostProgressControl runat="server" ID="calcProgress" OnClientBeforeStart="onPrepareData"
                OnClientCompleted="postProcess" ControlIDToShowDialog="btnRecalc" OnDoPostedData="ProcessCaculating"
                DialogHeaderText="正在计算..." DialogTitle="计算进度" />
        </div>
    </div>
    </form>
    <script type="text/javascript">

        function handleChange() {
            $pc.setText("prompt", "");
        }
        function checkExpression() {
            $pc.setText("prompt", "");
            AUCenter.Services.ConditionSvc.ValidateExpression($get("expression").value, function (result) {
                if (result) {
                    $pc.setText("prompt", "表达式正确");
                } else {
                    $pc.setText("prompt", "表达式错误");
                }
            }, function (err) {
                alert("出错" + err.get_message());
            });
        }

        function onPrepareData(e) {

            e.steps = [1];
        }

        function postProcess(e) {
            document.getElementById("lnkToPreview").click();
        }
    
    </script>
    <asp:Literal runat="server" ID="postScript" Mode="Transform"></asp:Literal>
</body>
</html>
