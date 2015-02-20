<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchEditor.aspx.cs" Inherits="PermissionCenter.BatchEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>批量属性</title>
    <link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .done .consoleProgress
        {
            display: none;
        }
        
        .consoleClose
        {
            display: none;
        }
        
        .done .consoleClose
        {
            display: block;
        }
    </style>
    <script type="text/javascript">

        function onSaveClick() {

            var reValue = $HGRootNS.PropertyEditorControlBase.ValidateProperties();

            if (!reValue.isValid)	//返回是否通过
                return false; //不通过则阻止提交

            var strips = $find("tabs"); ;
            var allProperties = [];

            for (var i = 0; i < strips.get_tabPageCount(); i++) {
                var propertyForm = $find(strips.get_tabPage(i).get_tag() + "_Form");
                var ppts = propertyForm.get_properties();
                for (var k = 0; k < ppts.length; k++) {
                    if (propertyForm.get_checkBoxStates().get_checked(ppts[k].name)) {
                        allProperties.push(ppts[k]);
                    }
                }
            }

            if (!allProperties.length) {
                alert('请勾选需要编辑的属性再点确定');
                return false;
            }

            var ppts = Sys.Serialization.JavaScriptSerializer.serialize(allProperties);
            $get("properties").value = ppts;

            obj = Sys.Serialization.JavaScriptSerializer.deserialize($get("objectDetails").value);
            if (obj) {
                treat(obj, ppts);
            }

            window.returnValue = true;
        }

        function closePanel(closeWindow) {
            $pc.hide('mask');
            $pc.hide('progressPanel');
            if (closeWindow)
                window.close();
        }

        function showError() {
            if (this.getAttribute) {
                $HGRootNS.ClientMsg.stop(this.getAttribute("title"), this.getAttribute("stackTrace"), "错误信息");
            } else {
                $HGRootNS.ClientMsg.stop(this.title, this.stackTrace, "错误信息");
            }
        }

        function callback(obj, fun) {
            if (obj)
                fun(obj);
        }


        function setPreparing(i) {
            callback($get("dataRows").rows[i], function (row) {
                row.className = "pc-status-running";
            });
        }

        function setDone(i) {
            callback($get("dataRows").rows[i], function (row) {
                row.className = "pc-status-ok";
                callback(row.cells[2], function (cell) {
                    cell.replaceChild(document.createTextNode('已修改'), cell.firstChild);
                });
            });
        }

        function setNotFound(i) {
            callback($get("dataRows").rows[i], function (row) {
                row.className = "pc-status-error";
                callback(row.cells[2], function (cell) {
                    cell.replaceChild(document.createTextNode('对象不存在或已删除'), cell.firstChild);
                });
            });
        }

        function setError(i, title, stack) {
            callback($get("dataRows").rows[i], function (row) {
                row.className = "pc-status-error";
                callback(row.cells[2], function (cell) {
                    var lnk = document.createElement("a");
                    lnk.href = "javascript:void(0)";
                    lnk.onclick = showError;
                    lnk.title = title;
                    lnk.className = "pc-error";
                    lnk.stackTrace = stack;
                    cell.replaceChild(lnk, cell.firstChild);
                    lnk.appendChild(document.createTextNode('未成功，点击此处查看。'));
                    lnk = null;
                });
            });
        }

        function setFinished() {
            $pc.addClass($get("bottomConsole"), "done");
        }

        function treat(objects, ppts) {
            var tb = $get("dataRows");
            var len = tb.rows.length;
            while (len > 0)
                tb.deleteRow(--len);

            var len = objects.length;
            var row;
            var cell;
            var elem;
            var ids = [];
            for (var i = len - 1; i >= 0; i--) {
                row = tb.insertRow(0);
                cell = row.insertCell(0);

                elem = document.createElement("span");
                elem.className = "pc-batch-status";
                cell.appendChild(elem);

                cell = row.insertCell(1);
                elem = document.createTextNode(objects[i].Name);
                cell.appendChild(elem);

                cell = row.insertCell(2);
                elem = document.createTextNode("-");
                cell.appendChild(elem);

                ids.push(objects[i].ID);
            }

            $pc.show('mask');
            $pc.show('progressPanel');
            $pc.removeClass("bottomConsole", "done");

            $pc.postViaIFrame("BatchEditor.aspx?action=submit", { ids: ids, properties: ppts });

            //			PageMethods.ChangeProperties(ids, ppts, function (data) {
            //				var rows = tb.rows;
            //				var row;
            //				len = ids.length;
            //				for (var k = 0; k < len; k++) {
            //					row = rows[k];
            //					$pc.addClass(row, data[k].Success ? "pc-status-ok" : "pc-status-error");
            //					if (data[k].Success) {
            //						row.cells[2].replaceChild(document.createTextNode(data[k].Message), row.cells[2].firstChild);
            //					} else {
            //						var lnk = document.createElement("a");
            //						lnk.href = "javascript:void(0)";
            //						lnk.onclick = showError;
            //						lnk.title = data[k].Message;
            //						lnk.className = "pc-error";
            //						lnk.stackTrace = data[k].StackTrace;
            //						row.cells[2].replaceChild(lnk, row.cells[2].firstChild);
            //						lnk.appendChild(document.createTextNode('未成功，点击此处查看。'));
            //					}
            //				}

            //				$pc.addClass("bottomConsole", "done");

            //			}, function (err) {
            //				if (err.get_timedOut()) {
            //					alert("操作超时");
            //				} else {
            //					alert('执行批处理时遇到了异常。' + err.get_message());
            //				}

            //				$pc.addClass("bottomConsole", "done");
            //			});
        }

    </script>
</head>
<body>
    <form id="form1" runat="server" action="BatchEditor.aspx" target="innerFrame">
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true">
        <Services>
            <asp:ServiceReference Path="~/Services/CommonService.asmx" />
        </Services>
    </asp:ScriptManager>
    <pc:SceneControl ID="SceneControl1" runat="server" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            对象属性<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <div class="pc-container5">
            <p>
                选择了<span runat="server" id="objNum">0 </span>个对象。 要为多个对象更改属性，首先勾选复选框启用更改，然后键入更改。是否更改成功取决于您的权限。
            </p>
        </div>
        <pc:BannerNotice runat="server" ID="notice" />
        <mcs:RelaxedTabStrip runat="server" ID="tabs">
        </mcs:RelaxedTabStrip>
        <asp:HiddenField runat="server" ID="properties" />
        <asp:HiddenField runat="server" ID="checkBoxes" />
        <input type="hidden" runat="server" id="objectDetails" />
        <div style="display: none">
            <iframe name="innerFrame"></iframe>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" id="okButton" runat="server" onclick="return ($pc.getEnabled(this) && onSaveClick())"
                accesskey="S" class="pcdlg-button btn-def" value="确定(S)" /><input type="button" accesskey="C"
                    class="pcdlg-button btn-cancel" onclick="window.returnValue = false;window.close()"
                    value="关闭(C)" />
        </div>
    </div>
    </form>
    <div class="pc-overlay-mask" id="mask" style="display: none">
    </div>
    <div class="pc-overlay-panel" id="progressPanel" style="vertical-align: middle; display: none">
        <div style="width: 500px; height: 80%; margin: auto; vertical-align: baseline;">
            <div style="background: #ffffff; margin-top: 30px; height: 100%; position: relative">
                <div class="pc-banner-sub" style="position: absolute; top: 0; left: 0;">
                    <h1>
                        正在保存
                    </h1>
                </div>
                <div class="pc-banner-sub" id="bottomConsole" style="position: absolute; bottom: 0;
                    left: 0; text-align: center;">
                    <div class="consoleProgress" style="padding-left: 0; width: 100%; zoom: 1">
                        <div class="pc-progress-border" style="width: auto">
                            <div id="statusText" class="pc-progress-prompt">
                                正在处理，请稍候…
                            </div>
                            <div class="pc-progress-bar">
                            </div>
                        </div>
                    </div>
                    <h1 class="consoleClose">
                        <input type="button" class="pcdlg-button" value="关闭" onclick="closePanel(true);" />
                    </h1>
                </div>
                <div style="position: absolute; top: 32px; bottom: 32px; left: 0; height: auto; width: 100%;
                    overflow: auto">
                    <div style="height: auto; text-align: left;">
                        <div style="height: 100%; overflow: auto">
                            <table style="width: 100%" id="table1">
                                <thead style="line-height: 1px">
                                    <tr>
                                        <th style="width: 32px">
                                            &nbsp;
                                        </th>
                                        <th style="width: 150px">
                                            &nbsp
                                        </th>
                                        <th>
                                            &nbsp;
                                        </th>
                                    </tr>
                                </thead>
                                <tbody id="dataRows">
                                    <tr class="pc-status-ok">
                                        <td>
                                            <span class="pc-batch-status"></span>
                                        </td>
                                        <td>
                                            某
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
