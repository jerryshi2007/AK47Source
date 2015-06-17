<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Default.aspx.cs"
    Inherits="WorkflowDesigner.Default" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=5" />
    <title>流程模板设计器</title>
    <link href="css/StyleSheet1.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="js/Silverlight.js"></script>
    <script type="text/javascript" src="js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="js/jquery.json-2.2.js"></script>
    <script type="text/javascript" src="js/common.js"></script>
    <script type="text/javascript" src="js/wfweb.js"></script>
    <script type="text/javascript" src="js/wfdesigner.js"></script>
    <script type="text/javascript">
        var m_inPostStatus = false;

        $.noConflict();
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

        /**********供silverlight调用的函数*/
        function OpenEditor(opType) {
            if (WFWeb.OpenEditor[opType]) {
                WFWeb.OpenEditor[opType]();
            }

            var disabled = true;
            if (WFWeb.GlobalProcList.GetCount() > 0) {
                disabled = false;
            }
            SetOperationLinks(disabled);
        }

        function SetPropertyValue(propertyName, strPropertyValue) {

            WFWeb.GetCurrentPropertiesItem.SetValueByName(propertyName, strPropertyValue);
            //var propertyGridControl = $find("propertyGrid");

            var propertyEditor = $find("propertyGrid")._propertyEditors[propertyName];

            if (propertyEditor) {
                if (propertyEditor.editor) {
                    propertyEditor = propertyEditor.editor;

                    if (propertyEditor.setDropDownListValue) {
                        propertyEditor.setDropDownListValue(strPropertyValue);
                    } else {
                        propertyEditor.get_property().value = strPropertyValue;
                        propertyEditor.commitValue();
                    }
                }
            }

            //			propertyGridControl.set_properties(WFWeb.Property.CurrentObj.Properties);
            //			propertyGridControl.dataBind();
        }

        /* 加载流程元素属性
		*  loadType:加载元素类型； 
		*  procKey：流程KEY； 
		*  jsonInfo：SL元素info信息json字符串
		*/
        function LoadProperty(loadType, procKey, jsonInfo) {
            if (WFWeb.LoadProperty[loadType]) {
                WFWeb.LoadProperty[loadType](procKey, jsonInfo);
            }
        }

        /*更新流程元素*/
        function UpdateProcess(exType, proKey, childKey, proName, proValue) {
            if (WFWeb.UpdateProcess[exType]) {
                WFWeb.UpdateProcess[exType](proKey, childKey, proName, proValue);
            }
        }

        /*删除流程元素*/
        function DeleteProcess(delType, procKey, childKey) {
            if (WFWeb.DeleteProcess[delType]) {
                WFWeb.DeleteProcess[delType](procKey, childKey);
            }

            var disabled = true;
            if (WFWeb.GlobalProcList.GetCount() > 0) {
                disabled = false;
            }
            SetOperationLinks(disabled);
        };

        function SaveActivityTemplate(templateID) {
            var url = 'SaveActivityTemplate.ashx';
            var postData = {
                action: 'insert',
                id: templateID,
                category: 'normal',
                content: jQuery.toJSON(WFWeb.Property.CurrentObj)
            };

            if (m_inPostStatus)
                return;

            m_inPostStatus = true;

            jQuery.post(url,
				postData,
				function (rtn) {
				    SubmitButton.resetAllStates();

				    var rtnObj = jQuery.parseJSON(rtn);
				    if (rtnObj.Success) {
				        var postObj = jQuery.parseJSON(postData.content);

				        postObj.Key = templateID;
				        jQuery.each(postObj.Properties, function (i, e) {
				            if (e.name == 'Key') postObj.Properties[i].value = templateID;
				        });

				        var userTempJSON = jQuery("#actUserTemplate").val();
				        var userTempArr = jQuery.parseJSON(userTempJSON);
				        userTempArr.push(postObj);
				        userTempJSON = jQuery.toJSON(userTempArr);
				        jQuery("#actUserTemplate").val(userTempJSON);

				        m_inPostStatus = false;

				        alert('设置模板成功！');
				    }
				    else {
				        m_inPostStatus = false;

				        alert('设置模板失败！原因：' + rtnObj.Message);
				    }
				});
        }

        function LoadActivityTemplate() {
            return jQuery("#actUserTemplate").val();
        }

        function OldToCurrentActity(itemActity, objEmptyActivity) {
            var result = new Object();
            result.Properties = new Array();
            result.ActivityType = itemActity.ActivityType;
            result.BranchProcessTemplates = itemActity.BranchProcessTemplates;

            result.Description = itemActity.Description;
            result.Enabled = itemActity.Enabled;
            result.Key = itemActity.Key;
            result.Name = itemActity.Name;
            result.Url = itemActity.Url;
            result.Properties = new Array();
            var tem = new Object();
            for (var i = 0; i < itemActity.Properties.length; i++) {
                var item = itemActity.Properties[i];
                tem[item.name] = item.value;
            }

            if (result.BranchProcessTemplates != undefined || result.BranchProcessTemplates != null) {
                if (result.BranchProcessTemplates.length > 0) {
                    tem.BranchProcessTemplates = itemActity.BranchProcessTemplates;
                }
            }

            for (var j = 0; j < objEmptyActivity.Properties.length; j++) {
                var currentitem = objEmptyActivity.Properties[j];
                if (tem.hasOwnProperty(currentitem.name)) {
                    currentitem.value = tem[currentitem.name];
                }
                result.Properties.push(currentitem);
            }
            return result;
        }

        function LoadProcessInstanceDescription() {
            var descJson = jQuery("#instanceDescription").val().trim();

            if (descJson != '') {
                WFWeb.OpenEditor.ProcessSuccess(descJson, null);
                SetOperationLinks(false);
            }
        }

        /**********供silverlight调用的函数 end*/
        function OnPropertyGridChanged(sender, data) {
            var obj = WFWeb.Property.CurrentObj;
            if (obj[data.property.name]) {
                obj[data.property.name] = data.property.value;
            }

            WFDesigner.DesignerInterAction.UpdateDiagramData(sender, data);
        }

        function SaveCurrent() {
            if (WFWeb.Property.CurrentProcessKey == undefined) {
                return false;
            }

            var diagramInfo = WFDesigner.DesignerInterAction.GetWorkflowGraph(WFWeb.Property.CurrentProcessKey);

            var arr = [];
            var proc = WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey);

            proc.GraphDescription = diagramInfo;
            arr.push(proc);
            PostWorkflowData(jQuery.toJSON(arr));

            return true; //submitbutton required
        }

        function Save() {
            var info = jQuery.parseJSON(WFDesigner.DesignerInterAction.GetWorkflowGraph(''));

            if (info.length == 0) {
                return false;
            }

            jQuery.each(info, function (i, e) {
                WFWeb.GlobalProcList.Get(e.Key).GraphDescription = e.Value;
            });

            var jsonStr = WFWeb.GlobalProcList.ToJson();
            PostWorkflowData(jsonStr);

            return true; //submitbutton required
        }

        function SaveAs() {
            if (WFWeb.Property.CurrentProcessKey == undefined) {
                return false;
            }

            WFWeb.OpenEditor['SaveAs']();

            return false; //submitbutton required
        }

        function PostWorkflowData(json) {
            var processID = jQuery("#processID").val();
            jQuery.post("SaveWorkflow.ashx", { info: json, processID: processID }, function (data) {
                SubmitButton.resetAllStates();
                alert(data);
            });
        }

        function RemoveActivityTemplate() {
            var idStr = WFDesigner.DesignerInterAction.RemoveActivityTemplate();

            if (idStr == '') {
                alert('请在左侧选择要删除的模板');
                return false;
            }

            if (idStr == 'canceled') {
                return false;
            }

            var url = 'SaveActivityTemplate.ashx';

            var postData = {
                action: 'delete',
                idstr: idStr
            };

            jQuery.post(url,
				postData,
				function (rtn) {
				    SubmitButton.resetAllStates();

				    var rtnObj = jQuery.parseJSON(rtn);
				    if (rtnObj.Success) {
				        var userTempJSON = jQuery("#actUserTemplate").val();
				        var userTempArr = jQuery.parseJSON(userTempJSON);
				        var idArr = idStr.split(',');

				        for (var i = 0; i < idArr.length; i++) {
				            userTempArr.remove(idArr[i], function (obj, val) {
				                if (obj.Key == val)
				                    return true;
				                return false;
				            });
				        }

				        userTempJSON = jQuery.toJSON(userTempArr);
				        jQuery("#actUserTemplate").val(userTempJSON);
				        alert('删除模板成功！');
				    }
				    else {
				        alert('删除模板失败！原因：' + rtnObj.Message);
				    }
				});

            return true; //submitbutton required
        }

        function onClickEditor(sender, e) {
            var activeEditor = sender.get_activeEditor();
            var opType;

            if (activeEditor.get_property().editorParams) {
                var editorParam = activeEditor.get_currentEditorParams();
                opType = editorParam;
                if (typeof (editorParam) == "object") {
                    if (editorParam.hasOwnProperty("tagName")) {
                        opType = editorParam.tagName;
                    }
                }
            } else {
                opType = activeEditor.get_property().name;
            }

            if (WFWeb.PropertGridOpenEditor[opType](activeEditor)) {
                WFWeb.PropertGridOpenEditor[opType]();
            }

            var disabled = true;

            if (WFWeb.GlobalProcList.GetCount() > 0) {
                disabled = false;
            }

            SetOperationLinks(disabled);
        }

        function TogglePropertyGrid() {
            var imgPath = jQuery("#imgSplitter").attr("src");
            if (imgPath == "images/arrow_toright.gif") {
                jQuery("#rightContent").hide();
                jQuery("#leftContent").animate({ width: '100%' }, 300, function () {
                    jQuery("#imgSplitter").attr("src", "images/arrow_toleft.gif");
                });
            }
            else {
                jQuery("#rightContent").show();
                jQuery("#leftContent").animate({ width: '80%' }, 300, function () {
                    jQuery("#imgSplitter").attr("src", "images/arrow_toright.gif");

                });
            };
        }

        function SetOperationLinks(disabled) {
            jQuery('#linkSaveCurrent').attr('disabled', disabled);
            jQuery('#linkSaveAs').attr('disabled', disabled);
            jQuery('#linkSave').attr('disabled', disabled);
            jQuery('#linkTemplate').attr('disabled', disabled);

            jQuery('#btnSaveCurrent').attr('disabled', disabled);
            jQuery('#btnSave').attr('disabled', disabled);
            jQuery('#btnSaveAs').attr('disabled', disabled);
            jQuery('#btnTemplate').attr('disabled', disabled);
        }

        jQuery(function () {
            SetOperationLinks(true);
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
            <tr style="height: 53px;">
                <td colspan="3">
                    <div class="workflow-header">
                        <div style="display: none">
                            <soa:SubmitButton ID="btnSaveCurrent" runat="server" AsyncInvoke="SaveCurrent" RelativeControlID="linkSaveCurrent"
                                PopupCaption="正在保存……" />
                            <soa:SubmitButton ID="btnSave" runat="server" AsyncInvoke="Save" RelativeControlID="linkSave"
                                PopupCaption="正在保存……" />
                            <soa:SubmitButton ID="btnSaveAs" runat="server" AsyncInvoke="SaveAs" RelativeControlID="linkSaveAs"
                                PopupCaption="正在保存……" />
                            <soa:SubmitButton ID="btnTemplate" runat="server" AsyncInvoke="RemoveActivityTemplate"
                                RelativeControlID="linkTemplate" PopupCaption="正在删除……" />
                        </div>
                        <span class="text">流 程 设 计 器</span> <span class="right"><a href="#" class="operationlink"
                            onclick="OpenEditor('NewProc');return false;">新建</a> <a href="#" class="operationlink"
                                onclick="OpenEditor('Process');return false;">打开</a> <a href="#" id="linkSaveCurrent"
                                    runat="server" class="operationlink" onclick="document.getElementById('btnSaveCurrent').click();">保存当前</a> <a href="#" id="linkSaveAs" runat="server" class="operationlink" onclick="document.getElementById('btnSaveAs').click();">另存为</a> <a href="#" id="linkSave" runat="server" class="operationlink" onclick="document.getElementById('btnSave').click();">保存全部</a> <a href="#" class="operationlink" onclick="OpenEditor('GlobalParam');return false;">全局设置</a> <a href="#" id="linkTemplate" runat="server" class="operationlink" onclick="document.getElementById('btnTemplate').click();">删除模板</a> <a href="#" class="operationlink" onclick="OpenEditor('WfMatrixDef');return false;">权限矩阵</a> <a href="Introduction/IntroductionIndex.aspx" class="operationlink" target="_blank"
                                        title="帮助中心" style="font-weight: bold;">?</a>
                            <%--
                        <a href="#" class="operationlink" onclick="WFDesigner.DesignerInterAction.AddActivitySelfLink();">
														添加自连线</a>
                        <a href="#" style="color: White; margin: 0 10 0 10;"
												onclick="WFDesigner.DesignerInterAction.LayoutCurrentDiagram();">排列</a>--%>
                        </span>
                    </div>
                </td>
            </tr>
            <tr id="trSLP">
                <td id="leftContent" style="width: 80%;">
                    <object style="margin: 3 0 0 3;" id="SLP" data="data:application/x-silverlight-2,"
                        type="application/x-silverlight-2" width="100%" height="100%" style="width: 100%; height: 100%">
                        <%--<param name="source" value="ClientBin/Designer.xap" />--%>
                        <param name="source" runat="server" id="xapPath" value="" />
                        <param name="onError" value="onSilverlightError" />
                        <param name="background" value="white" />
                        <param name="minRuntimeVersion" value="4.0.50826.0" />
                        <param name="autoUpgrade" value="true" />
                        <asp:Literal ID="enableSimulation" runat="server"></asp:Literal>
                        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0" style="text-decoration: none">
                            <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight"
                                style="border-style: none" />
                        </a>
                    </object>
                    <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>
                </td>
                <td style="width: 1px; cursor: hand;" onclick="TogglePropertyGrid();">
                    <img id="imgSplitter" src="images/arrow_toright.gif" />
                </td>
                <td id="rightContent" style="vertical-align: top; width: 20%;">
                    <table id="tbPropertyGrid" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%; padding-top: 3px; font-weight: bold; text-align: left; color: White; font-size: 14;">
                        <tr>
                            <td>
                                <soa:PropertyGrid runat="server" ID="propertyGrid" Width="100%" Height="100%" DisplayOrder="ByCategory"
                                    OnClientClickEditor="onClickEditor" OnClientEditorValidated="OnPropertyGridChanged" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="height: 5px;">
                <td colspan="3">
                    <asp:HiddenField ID="processTemplate" runat="server" />
                    <asp:HiddenField ID="initActTemplate" runat="server" />
                    <asp:HiddenField ID="completedActTemplate" runat="server" />
                    <asp:HiddenField ID="normalActTemplate" runat="server" />
                    <asp:HiddenField ID="tranTemplate" runat="server" />
                    <asp:HiddenField ID="actUserTemplate" runat="server" />
                    <%-- <asp:HiddenField ID="enumTypeStore" runat="server" />--%>
                    <asp:HiddenField ID="instanceDescription" runat="server" />
                    <asp:HiddenField ID="processID" runat="server" />
                </td>
            </tr>
        </table>
        <script type="text/javascript" src="js/WFObjectListPropertyEditor.js"></script>
        <script type="text/javascript" src="js/BranchProcessPropertyEditor.js"></script>
    </form>
</body>
</html>
