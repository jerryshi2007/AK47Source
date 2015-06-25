(function () {
    if (window.WFWeb == undefined) window.WFWeb = {};
    /*文件所在页面须有scriptmanager控件，并且放到控件之后*/
    //    WFWeb.ToJSON = Sys.Serialization.JavaScriptSerializer.serialize;
    //    WFWeb.ParseJSON = Sys.Serialization.JavaScriptSerializer.deserialize;

    /*callback签名:function(result,arg) result:窗口返回内容; arg:修改后的参数值*/
    WFWeb.Dialog = function (url, paraStr, featureStr, arg, fnCallback) {
        if (paraStr.lastIndexOf("?") == 0) {
            paraStr = paraStr.substr(1);
        }
        var fullURL = url + "?" + paraStr;

        var result = window.showModalDialog(fullURL, arg, featureStr);
        if (result) {
            if (fnCallback == undefined || fnCallback == null) return;
            fnCallback(result, arg);
        }
    }

    WFWeb.ModifyWfObject = function (obj, prop, val) {
        obj[prop] = val;
        if (obj.Properties == undefined) return;

        jQuery.each(obj.Properties, function (i, e) {
            if (e.name == prop) obj.Properties[i].value = val;
        });
    }

    /*全局工作流属性信息存储*/
    WFWeb.GlobalProcList = {
        _list: [],
        Add: function (process) {
            var isExist = WFWeb.GlobalProcList._list.has(process.Key, function (proc, value) {
                if (proc.Key == value) return true;
                return false;
            });

            if (isExist == true) throw ("流程模板" + process.Key + "已在列表中");
            WFWeb.GlobalProcList._list.push(process);
        },
        Get: function (key) {
            var result = WFWeb.GlobalProcList._list.get(key, function (proc, value) {
                if (proc.Key == value) return true;
                return false;
            });

            return result;
        },
        Del: function (key) {
            WFWeb.GlobalProcList._list.remove(key, function (proc, value) {
                if (proc.Key == value) return true;
                return false;
            });
        },
        GetActivity: function (procKey, actKey) {
            var process = WFWeb.GlobalProcList.Get(procKey);
            if (process == undefined) return;

            var result = process.Activities.get(actKey, function (act, value) {
                if (act != undefined && act.Key == value) return true;
                return false;
            });

            return result;
        },
        DelActivity: function (procKey, actKey) {
            var process = WFWeb.GlobalProcList.Get(procKey);
            if (process == undefined) return;

            process.Activities.remove(actKey, function (act, value) {
                if (act != undefined && act.Key == value) return true;
                return false;
            });
        },

        GetTransition: function (procKey, tranKey) {
            var process = WFWeb.GlobalProcList.Get(procKey);
            if (process == undefined) return;

            var result = process.Transitions.get(tranKey, function (act, value) {
                if (act != undefined && act.Key == value) return true;
                return false;
            });

            return result;
        },

        DelTransition: function (procKey, tranKey) {
            var process = WFWeb.GlobalProcList.Get(procKey);
            if (process == undefined) return;

            process.Transitions.remove(tranKey, function (act, value) {
                if (act != undefined && act.Key == value) return true;
                return false;
            });
        },

        ToJson: function () {
            return jQuery.toJSON(WFWeb.GlobalProcList._list);
        },

        GetProcessKey: function () {
            var result = [];
            for (i = 0; i < WFWeb.GlobalProcList._list.length; i++) {
                result.push(WFWeb.GlobalProcList._list[i].Key);
            };
            return result;
        },

        GetCount: function () {
            return this._list.length;
        }
    };

    WFWeb.SetPropertyValue = {
        IsDynamic: function (strPropertyValue) {
            if (strPropertyValue instanceof Boolean) {
                WFWeb.Property.CurrentObj.IsDynamic = strPropertyValue;
            } else {
                WFWeb.Property.CurrentObj.IsDynamic = new Boolean(strPropertyValue);
            }
        },

        ObjectValueSet: function (strPropertyName, strPropertyValue) {

            if (WFWeb.Property.CurrentObj[strPropertyName]) {
                var setProperty = WFWeb.Property.CurrentObj[strPropertyName];
                switch (prop.dataType) {
                    case ($HGRootNS.PropertyDataType.Boolean):
                        if (WFWeb.Property.CurrentObj[strPropertyName]) {
                            if (strPropertyValue instanceof Boolean) {
                                WFWeb.Property.CurrentObj[strPropertyName] = strPropertyValue;
                            } else {
                                WFWeb.Property.CurrentObj[strPropertyName] = new Boolean(strPropertyValue);
                            }
                        }
                        break;
                    case ($HGRootNS.PropertyDataType.Object):
                        break;
                    case ($HGRootNS.PropertyDataType.DateTime):
                        if (WFWeb.Property.CurrentObj[strPropertyName]) {
                            if (strPropertyValue instanceof Date) {
                                WFWeb.Property.CurrentObj[strPropertyName] = strPropertyValue;
                            } else {
                                WFWeb.Property.CurrentObj[strPropertyName] = new Date(Date.parse(this.get_property().value.replace(/-/g, "/")));
                            }
                        }
                        break;
                }
            }
        }
    },

	WFWeb.GetCurrentPropertiesItem = {
	    GetValueByName: function (itemName) {
	        var result;
	        if (WFWeb.Property.CurrentObj[itemName]) {
	            result = WFWeb.Property.CurrentObj[itemName];
	        } else {
	            Array.forEach(WFWeb.Property.CurrentObj.Properties, function (itemObject, index, ar) {
	                if (itemObject.name == itemName) {
	                    result = itemObject.value;
	                    return;
	                }
	            }, itemName);
	        }

	        return result;
	    },

	    SetValueByName: function (itemName, itemValue) {
	        if (WFWeb.Property.CurrentObj[itemName]) {
	            WFWeb.Property.CurrentObj[itemName] = WFWeb.GetCurrentPropertiesItem.ItemToObject(itemValue);
	        }

	        Array.forEach(WFWeb.Property.CurrentObj.Properties, function (itemObject, index, ar) {
	            if (itemObject.name == itemName) {
	                itemObject.value = itemValue;
	                return;
	            }
	        }, itemName);
	    },

	    ItemToObject: function (str) {
	        if (typeof (str) == "string") {
	            return Sys.Serialization.JavaScriptSerializer.deserialize(str);
	        } else if (str instanceof Object || str instanceof Array) {
	            return str;
	        }
	    },

	    ItemToJson: function (obj) {
	        if (typeof (obj) != "undefined") {
	            if ((typeof (obj) == "object") || obj instanceof Array) {
	                return Sys.Serialization.JavaScriptSerializer.serialize(obj);
	            } else {
	                return obj.toString();
	            }
	        }
	    }
	}

    /*打开编辑器对话框*/
    WFWeb.OpenEditor = {
        NewProc: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/ProcessKeyEditor.aspx";
            var commonFeature = "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var procKey = WFWeb.GlobalProcList.GetProcessKey();
            WFWeb.Dialog(url, "", commonFeature, procKey, function (result, arg) {
                //url参数是要弹出的窗口链接
                //result是弹出窗口返回值,此处是json格式的数据
                var resultJson = eval("(" + result + ")");

                WFDesigner.DesignerInterAction.CreateNewWorkflow(resultJson.Key);
                var process = WFWeb.GlobalProcList.Get(resultJson.Key);
                //遍历流程中所有的属性
                jQuery.each(process.Properties, function (i, e) {
                    if (e.name == 'ApplicationName') process.Properties[i].value = resultJson.AppName;
                });

                //绑定流程的属性
                WFWeb.BindPropertyGrid(resultJson.Key, process);
            });
        },
        GlobalParam: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/ExtGlobalParametersEditor.aspx";
            var commonFeature = "dialogWidth:720px; dialogHeight:540px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var procKey = WFWeb.GlobalProcList.GetProcessKey();
            WFWeb.Dialog(url, "", commonFeature, null, null);
        },
        Process: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfProcessDescriptorInformationList.aspx";
            var params = "condition=APPLICATION_NAME&value=";
            var commonFeature = "dialogWidth:800px; dialogHeight:680px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            WFWeb.Dialog(url, params, commonFeature, null, WFWeb.OpenEditor.ProcessSuccess);
        },
        ProcessSuccess: function (result, arg) {
            var procJsonArr = Sys.Serialization.JavaScriptSerializer.deserialize(result);
            var procArr = [];
            var procKey;
            for (var i = 0; i < procJsonArr.length; i++) {
                procKey = procJsonArr[i].Key;

                if (WFWeb.GlobalProcList.Get(procKey) == undefined) {
                    WFWeb.GlobalProcList.Add(procJsonArr[i]);
                    procArr.push(procJsonArr[i]);
                }
                else {
                    alert("流程模板 " + procKey + " 已经在编辑中!");
                }
            }
            // var json = jQuery.toJSON(procArr);
            var json = Sys.Serialization.JavaScriptSerializer.serialize(procArr);
            WFDesigner.DesignerInterAction.OpenWorkflow(json);
            //return jQuery.toJSON(procArr);
        },
        Condition: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfConditionEditor.aspx";
            var commonFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no";

            var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("Condition");
            var args = { jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result) };
            //            var args = { jsonStr: jQuery.toJSON(WFWeb.Property.CurrentObj.Condition) };

            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                var conditionValue = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                if (WFWeb.Property.CurrentObj.Priority != undefined) {
                    var nameIsEmpty = WFWeb.Property.CurrentObj.Properties.has("", function (o, v) {
                        if (o.name == "Name" && o.value == v) return true;
                        return false;
                    });
                    //在名称为空的时候线上显示条件表达式
                    if (nameIsEmpty) {
                        var val = '';
                        if (conditionValue.Expression != '') {
                            var val = '[' + conditionValue.Expression + ']';
                        }
                        WFDesigner.DesignerInterAction.UpdateDiagramData(null, { property: { name: "Condition", value: val } });
                    }
                }
                WFWeb.GetCurrentPropertiesItem.SetValueByName("Condition", result.jsonStr);
            });
        },
        ParametersNeedToBeCollected: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WFParametersNeedToBeCollected.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var args = { jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("ParametersNeedToBeCollected") };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("ParametersNeedToBeCollected", result.jsonStr);
                //activeEditor.commitValue(result.jsonStr);
            });
        },
        Resource: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:500px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("Resources");
            var obj = {
                jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result),
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("Resources", result.jsonStr);
            });
        },
        CancelReceivers: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var strcancelEventReceivers = WFWeb.GetCurrentPropertiesItem.ItemToJson(WFWeb.GetCurrentPropertiesItem.GetValueByName("CancelEventReceivers"));

            var obj = {
                jsonStr: strcancelEventReceivers,
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("CancelEventReceivers", result.jsonStr);
            });
        },
        ProcessStarters: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var strProcessStarters = WFWeb.GetCurrentPropertiesItem.ItemToJson(WFWeb.GetCurrentPropertiesItem.GetValueByName("ProcessStarters"));

            var obj = {
                jsonStr: strProcessStarters,
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("ProcessStarters", result.jsonStr);
            });
        },
        _getSimulationWindowFeature: function () {
            var width = 900;
            var height = 680;

            var left = (window.screen.width - width) / 2;
            var top = (window.screen.height - height) / 2;

            return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
        },
        Simulation: function () {
            var url = "./Simulation/WorkflowSimulation.aspx?processDescKey=" + WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Key;
            var commonFeature = this._getSimulationWindowFeature();

            window.open(url, "wfSimulation", commonFeature);
        },
        EnterReceivers: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var obj = {
                jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("EnterEventReceivers"),
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("EnterEventReceivers", result.jsonStr);
            });
        },
        LeaveReceivers: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var obj = {
                jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("LeaveEventReceivers"),
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("LeaveEventReceivers", result.jsonStr);
            });
        },
        RelativeLink: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfRelativeLinkEditor.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("RelativeLinks");
            var linkArg = { jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result) };
            WFWeb.Dialog(url, "", commonFeature, linkArg, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("RelativeLinks", result.jsonStr);
            });
        },
        BranchProcess: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfBranchProcessTemplates.aspx";
            var commonFeature = "dialogWidth:810px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("BranchProcessTemplates");
            var args = {
                jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result),
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            };

            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("BranchProcessTemplates", result.jsonStr);
                var branchProcessTemplatesValue = WFWeb.GetCurrentPropertiesItem.ItemToObject(result.jsonStr);
                var hasBranchProcess = false;
                if (branchProcessTemplatesValue.length > 0) {
                    hasBranchProcess = true;
                }
                WFDesigner.DesignerInterAction.UpdateDiagramData(null, { property: { name: "Branch", value: hasBranchProcess } });
            });
        },
        Variables: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfVariables.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("Variables");
            var args = { jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result) };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("Variables", result.jsonStr)
            });
        },
        InternalUsers: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfInternalRelativeUsers.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var args = { jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("InternalRelativeUsers") };

            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("InternalRelativeUsers", result.jsonStr)
            });
        },
        ExternalUsers: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfExternalUsers.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var args = { jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("ExternalUsers") };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("ExternalUsers", result.jsonStr)
            });
        },
        SaveAs: function (prockey) {
            var url = _DeluxeApplicationPath + "/modaldialog/ProcessKeyEditor.aspx";
            var commonFeature = "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var procKey = WFWeb.GlobalProcList.GetProcessKey();
            WFWeb.Dialog(url, "prockey" + prockey, commonFeature, procKey, function (result, arg) {
                //url参数是要弹出的窗口链接
                //result是弹出窗口返回值,此处是json格式的数据
                var resultJson = eval("(" + result + ")");

                var oldProcessKey = WFWeb.Property.CurrentProcessKey;
                var process = WFWeb.GlobalProcList.Get(oldProcessKey);
                process.Key = resultJson.Key;

                //遍历流程中所有的属性
                jQuery.each(process.Properties, function (i, e) {
                    if (e.name == 'Key') process.Properties[i].value = resultJson.Key;
                    if (e.name == 'ApplicationName') process.Properties[i].value = resultJson.AppName;
                });

                var diagramInfo = WFDesigner.DesignerInterAction.GetWorkflowGraph(oldProcessKey);
                process.GraphDescription = diagramInfo;

                WFDesigner.DesignerInterAction.SLManager().UpdateDiagramData("Workflow", oldProcessKey, 'Key', process.Key);
                var arr = [process];
                PostWorkflowData(jQuery.toJSON(arr));
                WFWeb.BindPropertyGrid(resultJson.Key, process);
            });
        },
        WfMatrixDef: function () {
            var url = _DeluxeApplicationPath + "/MatrixModalDialog/WfMatrixDefinitionList.aspx";
            var commonFeature = "dialogWidth:700px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            WFWeb.Dialog(url, "", commonFeature, null, null);
        },
        ImportWfMatrix: function () {
            var url = _DeluxeApplicationPath + "/MatrixModalDialog/WfProcessMatrix.aspx";
            var para = "processkey=" + WFWeb.Property.CurrentProcessKey;

            //如果是节点
            if (WFWeb.Property.CurrentObj.ActivityType) {
                para += "&activitykey=" + WFWeb.Property.CurrentObj.Key;
            }

            var commonFeature = "dialogWidth:480px; dialogHeight:320px;center:yes;help:no;resizable:no;scroll:no;status:no";

            WFWeb.Dialog(url, para, commonFeature, null, null);
        },
        EnterService: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfServiceOperationDefList.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var obj = { jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("EnterEventExecuteServices") }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("EnterEventExecuteServices", result.jsonStr);
            });
        },
        LeaveService: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfServiceOperationDefList.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var obj = { jsonStr: WFWeb.GetCurrentPropertiesItem.GetValueByName("LeaveEventExecuteServices") }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                WFWeb.GetCurrentPropertiesItem.SetValueByName("LeaveEventExecuteServices", result.jsonStr);
            });
        }
    };

    WFWeb.PropertGridOpenEditor = {
        NewProc: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/ProcessKeyEditor.aspx";
            var commonFeature = "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var procKey = WFWeb.GlobalProcList.GetProcessKey();
            WFWeb.Dialog(url, "", commonFeature, procKey, function (result, arg) {
                //url参数是要弹出的窗口链接
                //result是弹出窗口返回值,此处是json格式的数据
                var resultJson = eval("(" + result + ")");

                WFDesigner.DesignerInterAction.CreateNewWorkflow(resultJson.Key);
                var process = WFWeb.GlobalProcList.Get(resultJson.Key);
                //遍历流程中所有的属性
                jQuery.each(process.Properties, function (i, e) {
                    if (e.name == 'ApplicationName') process.Properties[i].value = resultJson.AppName;
                });

                //绑定流程的属性
                WFWeb.BindPropertyGrid(resultJson.Key, process);
            });
        },
        GlobalParam: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/ExtGlobalParametersEditor.aspx";
            var commonFeature = "dialogWidth:720px; dialogHeight:540px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var procKey = WFWeb.GlobalProcList.GetProcessKey();
            WFWeb.Dialog(url, "", commonFeature, null, null);
        },
        Process: function () {
            var url = _DeluxeApplicationPath + "/modaldialog/WfProcessDescriptorInformationList.aspx";
            var params = "condition=APPLICATION_NAME&value=";
            var commonFeature = "dialogWidth:800px; dialogHeight:680px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            WFWeb.Dialog(url, params, commonFeature, null, WFWeb.OpenEditor.ProcessSuccess);
        },
        ProcessSuccess: function (result, arg) {
            var procJsonArr = Sys.Serialization.JavaScriptSerializer.deserialize(result);
            var procArr = [];
            var procKey;
            for (var i = 0; i < procJsonArr.length; i++) {
                procKey = procJsonArr[i].Key;

                if (WFWeb.GlobalProcList.Get(procKey) == undefined) {
                    WFWeb.GlobalProcList.Add(procJsonArr[i]);
                    procArr.push(procJsonArr[i]);
                }
                else {
                    alert("流程模板 " + procKey + " 已经在编辑中!");
                }
            }
            var json = jQuery.toJSON(procArr);
            WFDesigner.DesignerInterAction.OpenWorkflow(json);
            //return jQuery.toJSON(procArr);
        },
        ParametersNeedToBeCollected: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WFParametersNeedToBeCollected.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var args = { jsonStr: activeEditor.get_property().value };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
            });
        },
        AutoStartBranch: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfProcessDescriptorInformationList.aspx?multiselect=false";
            var sFeature = "dialogWidth:800px; dialogHeight:680px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var result = window.showModalDialog(url, null, sFeature);
            if (result) {
                var processDescList = Sys.Serialization.JavaScriptSerializer.deserialize(result);
                if (processDescList.length != 1) {
                    alert('请选择子流程！');
                    return;
                }
                activeEditor.commitValue(processDescList);
            }
        },
        Condition: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfConditionEditor.aspx";
            var commonFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no";
            //var coValue = activeEditor.get_property().value;
            var coValue = WFWeb.GetCurrentPropertiesItem.GetValueByName("Condition");
            var args = { jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(coValue) };

            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                var conditionObject = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);

                if (WFWeb.Property.CurrentObj.Priority != undefined) {
                    var nameIsEmpty = WFWeb.Property.CurrentObj.Properties.has("", function (o, v) {
                        if (o.name == "Name" && o.value == v) return true;
                        return false;
                    });
                    //在名称为空的时候线上显示条件表达式
                    if (nameIsEmpty) {
                        var val = '';
                        if (conditionObject.Expression != '') {
                            var val = '[' + conditionObject.Expression + ']';
                        }
                        WFDesigner.DesignerInterAction.UpdateDiagramData(null, { property: { name: "Condition", value: val } });
                    }
                }
                activeEditor.commitValue(result.jsonStr);
                WFWeb.GetCurrentPropertiesItem.SetValueByName("Condition", result.jsonStr);
            });
        },
        ProcessStarters: function (activeEditor) {
            this.callResourceEditor(activeEditor);
            //var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            //var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            //var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("ProcessStarters");
            //var obj = {
            //    jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result),
            //    Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            //};

            //WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
            //    activeEditor.commitValue(result.jsonStr);
            //});
        },
        Resources: function (activeEditor) {
            this.callResourceEditor(activeEditor);
            //var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            //var commonFeature = "dialogWidth:800px; dialogHeight:500px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            //var obj = {
            //    jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(activeEditor.get_property().value),
            //    Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            //}
            //WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
            //    activeEditor.commitValue(result.jsonStr);;
            //});
        },
        CancelReceivers: function (activeEditor) {
            this.callResourceEditor(activeEditor);
            //var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            //var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            //var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("CancelEventReceivers");
            //var obj = {
            //    jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result),
            //    Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            //};

            //WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
            //    activeEditor.commitValue(result.jsonStr);
            //});
        },
        EnterReceivers: function (activeEditor) {
            this.callResourceEditor(activeEditor);
            //var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            //var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            //var obj = {
            //    jsonStr: activeEditor.get_property().value,
            //    Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            //}
            //WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
            //    activeEditor.commitValue(result.jsonStr);
            //});
        },
        LeaveReceivers: function (activeEditor) {
            this.callResourceEditor(activeEditor);
            //var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            //var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            //var obj = {
            //    jsonStr: activeEditor.get_property().value,
            //    Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            //}
            //WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
            //    activeEditor.commitValue(result.jsonStr);
            //});
        },
        RelativeLink: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfRelativeLinkEditor.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var linkArg = { jsonStr: activeEditor.get_property().value };
            WFWeb.Dialog(url, "", commonFeature, linkArg, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
            });
        },
        BranchProcess: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfBranchProcessTemplates.aspx";
            var commonFeature = "dialogWidth:810px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var args = {
                jsonStr: activeEditor.get_property().value,
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
                var branchProcessTemplatesValue = WFWeb.GetCurrentPropertiesItem.ItemToObject(result.jsonStr);
                var hasBranchProcess = false;
                if (branchProcessTemplatesValue.length > 0) {
                    hasBranchProcess = true;
                }
                WFDesigner.DesignerInterAction.UpdateDiagramData(null, { property: { name: "Branch", value: hasBranchProcess } });
            });
        },
        Variables: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfVariables.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var result = WFWeb.GetCurrentPropertiesItem.GetValueByName("Variables");
            var ajaxargs = { jsonStr: WFWeb.GetCurrentPropertiesItem.ItemToJson(result) };

            WFWeb.Dialog(url, "", commonFeature, ajaxargs, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
                WFWeb.GetCurrentPropertiesItem.SetValueByName("Variables", result.jsonStr)
            });
        },
        InternalUsers: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfInternalRelativeUsers.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var args = { jsonStr: activeEditor.get_property().value };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
            });
        },
        CanActivitysKeys: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/CanActivityKeysEditor.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var args = {
                jsonStr: activeEditor.get_property().value,
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
                // WFWeb.Property.CurrentObj.InternalRelativeUsers = jQuery.parseJSON(result.jsonStr);
            });
        },
        CancelExecuteServices: function (activeEditor) {
            this.callWebServiceEditor(activeEditor);
        },
        ExternalUsers: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfExternalUsers.aspx";
            var commonFeature = "dialogWidth:600px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var args = { jsonStr: activeEditor.get_property().value };
            WFWeb.Dialog(url, "", commonFeature, args, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
            });
        },
        SaveAs: function (prockey) {
            var url = _DeluxeApplicationPath + "/modaldialog/ProcessKeyEditor.aspx";
            var commonFeature = "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var procKey = WFWeb.GlobalProcList.GetProcessKey();
            WFWeb.Dialog(url, "prockey" + prockey, commonFeature, procKey, function (result, arg) {
                //url参数是要弹出的窗口链接
                //result是弹出窗口返回值,此处是json格式的数据
                var resultJson = eval("(" + result + ")");

                var oldProcessKey = WFWeb.Property.CurrentProcessKey;
                var process = WFWeb.GlobalProcList.Get(oldProcessKey);
                process.Key = resultJson.Key;

                //遍历流程中所有的属性
                jQuery.each(process.Properties, function (i, e) {
                    if (e.name == 'Key') process.Properties[i].value = resultJson.Key;
                    if (e.name == 'ApplicationName') process.Properties[i].value = resultJson.AppName;
                });

                var diagramInfo = WFDesigner.DesignerInterAction.GetWorkflowGraph(oldProcessKey);
                process.GraphDescription = diagramInfo;

                WFDesigner.DesignerInterAction.SLManager().UpdateDiagramData("Workflow", oldProcessKey, 'Key', process.Key);
                var arr = [process];
                PostWorkflowData(jQuery.toJSON(arr));
                WFWeb.BindPropertyGrid(resultJson.Key, process);
            });
        },
        WfMatrixDef: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/MatrixModalDialog/WfMatrixDefinitionList.aspx";
            var commonFeature = "dialogWidth:700px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            WFWeb.Dialog(url, "", commonFeature, null, null);
        },
        ImportWfMatrix: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/MatrixModalDialog/WfProcessMatrix.aspx";
            var para = "processkey=" + WFWeb.Property.CurrentProcessKey;

            //如果是节点
            if (WFWeb.Property.CurrentObj.ActivityType) {
                para += "&activitykey=" + WFWeb.Property.CurrentObj.Key;
            }

            var commonFeature = "dialogWidth:480px; dialogHeight:320px;center:yes;help:no;resizable:no;scroll:no;status:no";

            WFWeb.Dialog(url, para, commonFeature, null, null);
        },
        EnterService: function (activeEditor) {
            this.callWebServiceEditor(activeEditor);
        },
        LeaveService: function (activeEditor) {
            this.callWebServiceEditor(activeEditor);
        },
        BeWithdrawnExecuteServices: function (activeEditor) {
            this.callWebServiceEditor(activeEditor);
        },
        WithdrawExecuteServices: function (activeEditor) {
            this.callWebServiceEditor(activeEditor);
        },
        //调用Web服务的编辑器
        callWebServiceEditor: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfServiceOperationDefList.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var obj = { jsonStr: activeEditor.get_property().value }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
            });
        },
        callResourceEditor: function (activeEditor) {
            var url = _DeluxeApplicationPath + "/modaldialog/WfResourceEditor.aspx";
            var commonFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            var obj = {
                jsonStr: activeEditor.get_property().value,
                Activities: WFWeb.GlobalProcList.Get(WFWeb.Property.CurrentProcessKey).Activities
            }
            WFWeb.Dialog(url, "", commonFeature, obj, function (result, arg) {
                activeEditor.commitValue(result.jsonStr);
            });
        }
    };

    /*加载属性设计器数据*/
    WFWeb.LoadProperty = {
        Workflow: function (procKey, jsonInfo) {
            var oInfo = jQuery.evalJSON(jsonInfo);
            var process = WFWeb.GlobalProcList.Get(procKey);
            if (process == undefined) {
                process = WFWeb.Utils.CreateNewWorkflow(jQuery("#processTemplate").val(), oInfo);
                WFWeb.GlobalProcList.Add(process);
            }
            WFWeb.BindPropertyGrid(procKey, process);
        },
        Activity: function (procKey, jsonInfo) {
            var oInfo = jQuery.evalJSON(jsonInfo);
            var activity = WFWeb.GlobalProcList.GetActivity(procKey, oInfo.Key);
            if (activity == undefined) {
                var templateElementID = 'normalActTemplate';
                if (oInfo.ActivityType == 1) templateElementID = 'initActTemplate'; //起始活动点
                else if (oInfo.ActivityType == 4) templateElementID = 'completedActTemplate';    //结束活动点

                if (oInfo.TemplateID) {
                    var tempjson = jQuery("#actUserTemplate").val();
                    var userTempArr = jQuery.evalJSON(tempjson);
                    activity = userTempArr.get(oInfo.TemplateID, function (template, val) {
                        if (template.Key == val) return true;
                        return false;
                    });

                    if (activity == undefined) {
                        activity = WFWeb.Utils.CreateNewActivity(jQuery("#" + templateElementID).val(), oInfo);
                    }
                    else {
                        activity.Key = oInfo.Key;

                        jQuery.each(activity.Properties, function (i, e) {
                            if (e.name == 'Key') {
                                activity.Properties[i].value = oInfo.Key;
                            }
                        });
                    }
                }
                else {
                    activity = WFWeb.Utils.CreateNewActivity(jQuery("#" + templateElementID).val(), oInfo);
                }
                WFWeb.GlobalProcList.Get(procKey).Activities.push(activity);
            }

            WFWeb.BindPropertyGrid(procKey, activity);
        },
        Transition: function (procKey, jsonInfo) {
            var oInfo = jQuery.evalJSON(jsonInfo);
            var transition = WFWeb.GlobalProcList.GetTransition(procKey, oInfo.Key);
            if (transition == undefined) {
                transition = WFWeb.Utils.CreateNewTransition(jQuery("#tranTemplate").val(), oInfo);
                WFWeb.GlobalProcList.Get(procKey).Transitions.push(transition);
            } else if (oInfo.FromActivityKey != transition.FromActivityKey || oInfo.ToActivityKey != transition.ToActivityKey) {
                transition.FromActivityKey = oInfo.FromActivityKey;
                transition.ToActivityKey = oInfo.ToActivityKey;
            }

            WFWeb.BindPropertyGrid(procKey, transition);
        }
    };

    /*修改流程元素*/
    WFWeb.UpdateProcess = {
        Workflow: function (proKey, childKey, proName, proValue) {
            var process = WFWeb.GlobalProcList.Get(proKey);
            jQuery.each(process.Properties, function (i, e) {
                if (e.name == proName) process.Properties[i].value = proValue;
            });

            WFWeb.BindPropertyGrid(proKey, process);
        },
        Activity: function (proKey, childKey, jsonInfo) {
            var activity = WFWeb.GlobalProcList.GetActivity(proKey, childKey);
            if (activity == undefined || activity == null) return;

            var info = jQuery.parseJSON(jsonInfo);
            jQuery.each(activity.Properties, function (i, e) {
                //将info中的字段同步到全局变量中
                if (info[e.name] != undefined && activity.Properties[i].value != info[e.name]) {
                    activity.Properties[i].value = info[e.name];
                    activity[e.name] = info[e.name];
                }
            });
            WFWeb.BindPropertyGrid(proKey, activity);
        },
        Transition: function (proKey, childKey, jsonInfo) {
            var transition = WFWeb.GlobalProcList.GetTransition(proKey, childKey);
            if (transition == undefined || transition == null) return;

            var info = jQuery.parseJSON(jsonInfo);
            jQuery.each(transition.Properties, function (i, e) {
                //将info中的字段同步到全局变量中
                if (info[e.name] != undefined && transition.Properties[i].value != info[e.name]) {
                    transition.Properties[i].value = info[e.name];
                    transition[e.name] = info[e.name];
                }
            });
            WFWeb.BindPropertyGrid(proKey, transition);
        }
    }

    /*删除流程元素*/
    WFWeb.DeleteProcess = {
        Workflow: function (procKey, childKey) {
            WFWeb.GlobalProcList.Del(procKey);
            //必要时从sl端取当前key
            if (WFWeb.Property.CurrentProcessKey == procKey) {
                WFWeb.Property.CurrentProcessKey = undefined;
            }
            var propertyGridControl = $find("propertyGrid");
            propertyGridControl.set_properties({});
            propertyGridControl.dataBind();
        },
        Activity: function (procKey, childKey) {
            WFWeb.GlobalProcList.DelActivity(procKey, childKey);
            var propertyGridControl = $find("propertyGrid");
            propertyGridControl.set_properties({});
            propertyGridControl.dataBind();
        },
        Transition: function (procKey, childKey) {
            WFWeb.GlobalProcList.DelTransition(procKey, childKey);
            var propertyGridControl = $find("propertyGrid");
            propertyGridControl.set_properties({});
            propertyGridControl.dataBind();
        }
    }

    WFWeb.BindPropertyGrid = function (proKey, obj) {
        WFWeb.Property.CurrentProcessKey = proKey;
        WFWeb.Property.CurrentObj = obj;
        var propertyGridControl = $find("propertyGrid");
        propertyGridControl.set_properties(obj.Properties);
        propertyGridControl.dataBind();
    }

    /*silverlight 工作流信息*/
    WFWeb.WorkflowInfo = function (wfKey, wfName) {
        this.Key = wfKey;
        this.Name = wfName;
        this.Activities = [];
        this.Transitions = [];
    };

    /*初始化WorkflowInfo*/
    WFWeb.WorkflowInfo.Initialize = function (process) {
        var wfInfo = new WFWeb.WorkflowInfo();

        wfInfo.Key = process.Key;

        jQuery.each(process.Properties, function (index, element) {
            switch (element.name) {
                case "Name": wfInfo.Name = element.value; break;
            }
        });

        wfInfo.Activities = WFWeb.ActivityInfo.Initialize(process.Activities);
        wfInfo.Transitions = WFWeb.TransitionInfo.Initialize(process.Transitions);

        return wfInfo;
    };

    WFWeb.WorkflowInfo.prototype.ToJson = function () {
        return jQuery.toJSON(this);
    };

    /*silverlight 活动点信息*/
    WFWeb.ActivityInfo = function (actKey, actName, actType) {
        this.Key = actKey;
        this.Name = actName;
        this.ActivityType = actType;
    };

    /*初始化ActivityInfo*/
    WFWeb.ActivityInfo.Initialize = function (activities) {
        var actInfos = new Array();
        jQuery.each(activities, function (index, element) {
            var actInfo = new WFWeb.ActivityInfo();

            actInfo.Key = element.Key;
            actInfo.ActivityType = element.ActivityType;

            jQuery.each(element.Properties, function (i, e) {
                switch (e.name) {
                    case "Name": actInfo.Name = e.value; break;
                };
            });

            actInfos[actInfos.length] = actInfo;
        });

        return actInfos;
    };


    /*silverlight 线信息*/
    WFWeb.TransitionInfo = function (tranKey, tranName, tranFrom, tranTo) {
        this.Key = tranKey;
        this.Name = tranName;
        this.FromActivity = tranFrom;
        this.ToActivity = tranTo;
    };

    WFWeb.TransitionInfo.Initialize = function (transitions) {
        var tranInfos = new Array();
        jQuery.each(transitions, function (index, element) {
            var tranInfo = new WFWeb.TransitionInfo();

            tranInfo.Key = element.Key;
            tranInfo.FromActivity = element.FromActivityKey;
            tranInfo.ToActivity = element.ToActivityKey;

            jQuery.each(element.Properties, function (i, e) {
                switch (e.name) {
                    case "Name": tranInfo.Name = e.value; break;
                };
            });

            tranInfos[tranInfos.length] = tranInfo;
        });

        return tranInfos;
    };


    WFWeb.Property = {};
    /*属性编辑器当前绑定对象*/
    WFWeb.Property.CurrentObj = null;
    WFWeb.Property.CurrentProcessKey = null;

    WFWeb.Utils = {};

    /*根据流程模板创建流程 templateStr：模板json格式 procInfo：流程属性信息*/
    WFWeb.Utils.CreateNewWorkflow = function (templateStr, procInfo) {
        var result = jQuery.evalJSON(templateStr);
        result.Key = procInfo.Key;
        result.Name = procInfo.Name;

        jQuery.each(result.Properties, function (i, e) {
            //布尔类型转为小写
            if (e.dataType == 3) e.value = e.value.toLowerCase();
            switch (e.name) {
                case "Key": e.value = result.Key; break;
                case "Name": e.value = result.Name; break;
            };
        });
        return result;
    };

    /*根据活动模板创建活动 templateStr：模板json格式 actInfo：活动属性信息*/
    WFWeb.Utils.CreateNewActivity = function (templateStr, actInfo) {
        var result = jQuery.evalJSON(templateStr);
        result.Key = actInfo.Key;
        result.Name = actInfo.Name;
        result.ActivityType = actInfo.ActivityType;

        jQuery.each(result.Properties, function (i, e) {
            //布尔类型转为小写
            if (e.dataType == 3) e.value = e.value.toLowerCase();
            switch (e.name) {
                case "Key": e.value = result.Key; break;
                case "Name": e.value = result.Name; break;
                case "Description": e.value = actInfo.Description; break;
            };
        });

        return result;
    };


    /*根据线模板创建线 templateStr：模板json格式 tranInfo：线属性信息*/
    WFWeb.Utils.CreateNewTransition = function (templateStr, tranInfo) {
        var result = jQuery.evalJSON(templateStr);
        result.Key = tranInfo.Key;
        result.Name = tranInfo.Name;
        result.FromActivityKey = tranInfo.FromActivityKey;
        result.ToActivityKey = tranInfo.ToActivityKey;

        jQuery.each(result.Properties, function (i, e) {
            //布尔类型转为小写
            if (e.dataType == 3) e.value = e.value.toLowerCase();
            switch (e.name) {
                case "Key": e.value = result.Key; break;
                case "Name": e.value = result.Name; break;
                case "IsReturn": e.value = tranInfo.IsReturn; break;
            };
        });
        return result;
    };

    WFWeb.Utils.checkInputKey = function (key) {
        var reg = /^(\w)+$/

        return reg.test(key);
    };
})();


