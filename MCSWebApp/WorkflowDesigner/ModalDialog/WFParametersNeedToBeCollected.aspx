<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WFParametersNeedToBeCollected.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.WFParametersNeedToBeCollected" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>自动收集流程参数配置</title>
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var parameters = new Array();
        var enumList = new Array();
        function onPageLoad() {
            var arrParaObj = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.jsonStr);
            enumList =Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenEnumParameterTypeList").value);
            for (var i = 0; i < arrParaObj.length; i++) {
                parameters.push(arrParaObj[i]);
            }
            createClientGrid();
            bindClientGrid(parameters);
        }

        function bindClientGrid(dataSource) {
            var gridDatasource = dataSource ? dataSource : branchProcessTemplates;
            var grid = $find("grid");
            grid.set_dataSource(gridDatasource);
        }

        function getParameterByName(key) {
            var wfParameter = {};
            for (var i = 0; i < parameters.length; i++) {
                if (parameters[i].parameterName == key) {
                    wfParameter["wfParameter"] = parameters[i];
                    wfParameter["index"] = i;
                    break;
                }
            }
            return wfParameter;
        }

        function openParametersModalDialog(key) {
            var _extUser;
            var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            if ("" != key) {
                _extUser = getParameterByName(key)
                result = window.showModalDialog("WFParametersNeedToBeCollectedEditor.aspx",
                { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(_extUser),
                    enumParameterTypeListStr: document.getElementById("hiddenEnumParameterTypeList").value,
                    parametersStr: Sys.Serialization.JavaScriptSerializer.serialize(parameters)
                }, sFeature);
            } else {
                result = window.showModalDialog("WFParametersNeedToBeCollectedEditor.aspx", { jsonStr: null,
                    enumParameterTypeListStr: document.getElementById("hiddenEnumParameterTypeList").value,
                    parametersStr: Sys.Serialization.JavaScriptSerializer.serialize(parameters)
                }, sFeature);
            }
            if (result) {
                var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                if (resultObj.wfParameter) {
                    parameters[resultObj.index] = resultObj.wfParameter;
                } else {
                    parameters.push(resultObj);
                }
                bindClientGrid(parameters);
            }
        }

        function addParameter() {
            openParametersModalDialog("");
        }

        function deleteParameter() {
            var grid = $find("grid");
            var selectedData = grid.get_selectedData();
            if (selectedData.length <= 0)
                alert("没有选择外部人员。");
            if (!(selectedData.length > 0 && confirm("确定删除？"))) {
                return;
            }
            for (var i = 0; i < selectedData.length; i++) {
                var element = selectedData[i];
                for (var j = 0; j < parameters.length; j++) {
                    if (parameters[j].parameterName == element.parameterName) {
                        parameters.splice(j, 1);
                    }
                }
            }
            bindClientGrid(parameters);
        }

        function onbtnOKClick() {
            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(parameters) };

            top.close();
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">自动收集流程参数配置</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: center">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<!--Put your dialog content here... -->
                    <table style="width: 100%; height: 100%; vertical-align:top;">
                            <tr >
                                <td style="height:30px; background-color:#C0C0C0"><a href="#" onclick="addParameter();"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
                                        <a href="#" onclick="deleteParameter();"><img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除"" border="0" /></a>
                                </td>
                            </tr>
                            <tr> 
                                <td colspan="3"  style="width: 100%;height: 100%; vertical-align:top;" >
                                        <table id="grid" align="center" ></table>
                                </td>
                            </tr>
                        </table>
				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
                        <td style="text-align: center;"><input type="button" class="formButton" onclick="onbtnOKClick();" value="确定(O)"  id="btnOK" accesskey="O"/></td>
                        <td style="text-align: center;"><input type="button" class="formButton" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"/></td>
					</tr>
				</table>
			</td>
		</tr>
        <tr>
          <td style=" height:2px">
            <input type="hidden" runat="server" id="hiddenEnumParameterTypeList" />
          </td>
        </tr>
	</table>
    <script type="text/javascript">
        function createClientGrid() {
            //创建列表对象
            $create($HBRootNS.ClientGrid,
		    {
		        pageSize: 5,
		        cellPadding: "3px",
		        emptyDataText: "没有记录！",
		        columns: [
                { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px"} },
				{ dataField: "parameterName", headerText: "参数名称", itemStyle: { width: "50px" }, headerStyle: { width: "100px" },
				    cellDataBound: function (grid, e) {
				        var linkText = "<a href='#' style='color: Black;' onclick='openParametersModalDialog(\"{1}\");'>{0}</a>";
				        e.cell.innerHTML = String.format(linkText, e.data["parameterName"].toString(), e.data["parameterName"].toString());
				    }
				},
                { dataField: "parameterType", headerText: "数据类型", itemStyle: { textAlign: "left" },
                    cellDataBound: function (grid, e) {
                        var text = "";
                        for (var i = 0; i < enumList.length; i++) {
                            if (enumList[i].EnumValue == e.data["parameterType"]) {
                                text = enumList[i].Description;
                                break;
                            }
                        }
                        e.cell.innerHTML = text;
                    } 
                },
                { dataField: "controlID", headerText: "控件ID", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } },
                { dataField: "controlPropertyName", headerText: "收集控件属性", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } },
                { dataField: "autoCollect", headerText: "是否自动收", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
                ],

		        allowPaging: true,
		        autoPaging: true,
		        pagerSetting: { mode: $HBRootNS.PageSettingMode.text, position: $HBRootNS.PagerPosition.bottom, firstPageText: "<<", prevPageText: "上页", nextPageText: "下页", lastPageText: ">>" }

			, style: { width: "100%" }
		    },
		{ pageIndexChanged: onPageIndexChanged, sorted: onSorted },
		null, $get("grid"));
        }

        //相应翻页事件
        function onPageIndexChanged(gridControl, e) {
            if (!gridControl.get_autoPaging())
                setGridControlDataSource(gridControl);
        }
        function onSorted(gridControl, e) {
            //实现本页排序
            //           gridControl.get_dataSource().sort(Function.createDelegate(gridControl, dataSourceSort));
            //           gridControl.dataBind();
        }

    </script>
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            onPageLoad();
        });
    </script>
</body>
</html>
