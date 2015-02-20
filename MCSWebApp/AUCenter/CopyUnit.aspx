<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopyUnit.aspx.cs" Inherits="AUCenter.CopyUnit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>复制管理单元</title>
    <link href="Styles/dlg.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
</head>
<body class="pcdlg">
    <form id="form1" runat="server">
    <au:SceneControl ID="SceneControl1" runat="server" />
    <asp:ScriptManager runat="server" EnablePageMethods="true" 
        EnableScriptGlobalization="true" ID="sm" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            复制管理单元</h1>
    </div>
    <div class="pcdlg-content">
        <div class="pc-container5">
            <div>
            </div>
            <div>
                <table>
                    <tr>
                        <th>
                            新的名称
                        </th>
                        <td>
                            <asp:TextBox runat="server" ID="txtNewName" />
                        </td>
                        <th>
                            新的代码名称
                        </th>
                        <td>
                            <asp:TextBox runat="server" ID="txtNewCodeName" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:CheckBox ID="chkCopyRoleMembers" Text="复制角色成员" runat="server" />
                            <asp:CheckBox ID="chkCopyScopeMembers" Text="复制所有管理范围固定成员" runat="server" />
                            <asp:CheckBox ID="chkCopyScopeConditions" Text="复制所有管理范围表达式" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="">
                <div>
                    复制目标</div>
                <mcs:DeluxeTree runat="server" ID="tree" OnGetChildrenData="tree_GetChildrenData">
                </mcs:DeluxeTree>
            </div>
        </div>
        <div style='display: none'>
            <asp:HiddenField runat="server" ID="hfFromUnit" />
            <asp:HiddenField runat="server" ID="hfSchemaID" />
            <soa:PostProgressControl runat="server" ID="calcProgress" OnClientBeforeStart="onPrepareData"
                OnClientCompleted="postProcess" ControlIDToShowDialog="btnSure" OnDoPostedData="Processing"
                DialogHeaderText="正在处理" DialogTitle="处理进度" 
                onloadingdialogcontent="calcProgress_LoadingDialogContent" />
            <asp:Button Text="Sure" runat="server" ID="btnSure" OnClick="btnOk_Click" />
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" id="btnOk" class="pcdlg-button" value="继续(O)" accesskey="O"
                onclick="check();" />
            <input type="button" class="pcdlg-button" value="关闭" onclick="window.close();" />
        </div>
    </div>
    <div class="pc-overlay-mask" id="checkmask" style="display: none">
    </div>
    <div class="pc-overlay-panel" id="checkpanel" style="display: none">
        <div style="width: 600px; height: 200px; margin: auto; vertical-align: baseline;">
            <div style="background: #ffffff; margin-top: 30px; height: 100%; padding: 20px; position: relative;
                text-align: center;">
                <h1 class="pc-caption">
                    验证数据
                </h1>
                <div>
                    <div class="au-progress" id="progressbar">
                    </div>
                </div>
                <div>
                    验证目标：<span id="lblValidationTarget">未验证</span>
                </div>
                <div>
                    验证代码名称：<span id="lblValidationCodeName">未验证</span>
                </div>
                <div>
                    验证名称：<span id="lblValidationName">未验证</span>
                </div>
                <div>
                    验证对象：<span id="lblValidationObj">未验证</span>
                </div>
                <div class="pc-prompt">
                    <span id="checkResult"></span>
                </div>
                <div class="pcdlg-button-bar" id="continuePanel">
                    <input type="button" value="继续" class="pcdlg-button" id="btnContinue" style="display: none"
                        onclick="startCopy();" />
                    <input type="button" value="返回修改" class="pcdlg-button" id="btnCancel" onclick="cancelCopy()" />
                </div>
            </div>
        </div>
    </div>
    </form>
    <script type="text/javascript">

        function check() {
            $pc.setText($pc.get("lblValidationName"), '未验证');
            $pc.setText($pc.get("lblValidationCodeName"), '未验证');
            $pc.setText($pc.get("lblValidationTarget"), '未验证');
            $pc.setText($pc.get("lblValidationObj"), '未验证');
            $pc.setText($pc.get("checkResult"), '');

            var node = $find("tree").get_selectedNode();
            if (node) {
                $pc.show('checkmask');
                $pc.show('checkpanel');
                $pc.hide("btnContinue");
                $pc.show("progressbar");
                var schemaID = $pc.get("hfSchemaID").value;
                var parentID = node.get_value();
                var name = $pc.get("txtNewName").value;
                var codeName = $pc.get("txtNewCodeName").value;

                PageMethods.DoClientValidation(schemaID, parentID, name, codeName, function (ok) {
                    $pc.setText($pc.get("lblValidationName"), ok.NameValidationResult);
                    $pc.setText($pc.get("lblValidationCodeName"), ok.CodeNameValidationResult);
                    $pc.setText($pc.get("lblValidationTarget"), ok.TargetValidationResult);
                    $pc.setText($pc.get("lblValidationObj"), ok.ObjectValidationResult);
                    $pc.hide("progressbar");
                    if (ok.Passed) {
                        $pc.setText($pc.get("checkResult"), "");
                        $pc.get("btnContinue").style.display = "inline";
                    } else {
                        $pc.setText($pc.get("checkResult"), "验证失败，请返回修改");
                        $pc.hide("btnContinue");
                    }

                }, function (err) {
                    $pc.setText($pc.get("checkResult"), err.get_message());
                    $pc.hide("btnContinue");
                    $pc.hide("progressbar");
                });
            } else {
                alert("请选择复制的目标位置");
            }
        }

        function cancelCopy() {
            $pc.hide('checkmask');
            $pc.hide('checkpanel');
            $pc.hide("btnContinue");
        }

        function startCopy() {
            $pc.hide('checkpanel');
            $pc.get('btnSure').click();
        }

        function onPrepareData(e) {
            var node = $find("tree").get_selectedNode();
            if (node) {
                var paras = [];
                paras.push($pc.get("hfFromUnit").value);
                paras.push($pc.get("txtNewName").value);
                paras.push($pc.get("txtNewCodeName").value);
                paras.push(node.get_value());
                paras.push($pc.get("chkCopyRoleMembers").checked);
                paras.push($pc.get("chkCopyScopeMembers").checked);
                paras.push($pc.get("chkCopyScopeConditions").checked);
                e.steps = paras;
            } else {
                e.cancel = true;
            }
        }

        function postProcess(e) {
            if (e.dataChanged) {
                window.returnValue = true;
                window.close();
            }
        }

    </script>
</body>
</html>
