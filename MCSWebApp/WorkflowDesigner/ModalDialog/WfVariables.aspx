<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfVariables.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfVariables" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self">
    <title>变量列表</title>
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css"/>
    <link href="/MCSWebApp/Css/WebControls/DeluxeTree.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>
        <script type="text/javascript">
            var arrVariables;
            function onDocumentLoad() {
                arrVariables = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.jsonStr);
                createClientGrid();
                if (arrVariables) {
                    bindGrid(arrVariables);
                }
            }

            function getVariableByKey(key) {
                var variable = {};
                for (var i = 0; i < arrVariables.length; i++) {
                    if (arrVariables[i].Key == key) {
                        variable["variableObj"] = arrVariables[i];
                        variable["index"] = i;
                        break;
                    }
                }
                return variable;
            }

            function bindGrid(dataSource) {
                var grid = $find("grid");
                grid.set_dataSource(dataSource);
            }

            function openModalDialog(variableKey) {
                var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:no;scroll:no;status:no";
                var result;
                if ("" != variableKey) {
                    result = window.showModalDialog("WfVariableEditor.aspx",
                    { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(getVariableByKey(variableKey)),
                      existVarJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrVariables) }, sFeature);
                } else {
                  result = window.showModalDialog("WfVariableEditor.aspx", { jsonStr: null, existVarJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrVariables) }, sFeature);
                }
                if (result) {
                    var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                    if (resultObj.variableObj) {
                        arrVariables[resultObj.index] = resultObj.variableObj;
                    } else {
//                        for (var i = 0; i < arrVariables.length; i++) {
//                            if (arrVariables[i].Key == resultObj.Key) {
//                                alert("已经有相同Key的变量存在");
//                                return;
//                            }
//                        }
                        arrVariables.push(resultObj);
                    }
                    bindGrid(arrVariables);
                }
            }

            function addVariable() {
                openModalDialog("");
            }
            function deleteVariable() {
                var grid = $find("grid");
                var selectedData = grid.get_selectedData();
                if (selectedData.length <= 0) {
                    alert("没有选择变量。");
                }
                if (!(selectedData.length > 0 && confirm("确定删除？"))) {
                    return;
                }
                for (var i = 0; i < selectedData.length; i++) {
                    var element = selectedData[i];
                    for (var j = 0; j < arrVariables.length; j++) {
                        if (arrVariables[j].Key == element.Key) {
                            arrVariables.splice(j, 1);
                        }
                    }
                }
                bindGrid(arrVariables);
            }
            function onbtnOKClick() {
                window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrVariables) };

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
					<span class="dialogLogo">变量列表</span>
				</div>
			</td>
		</tr>
        <tr>    
            <td style="height:30px; background-color:#C0C0C0">
                <a href="#" onclick="addVariable();"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
                <a href="#" onclick="deleteVariable();"><img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除"" border="0" /></a>
            </td>
        </tr>
		<tr>
			<td style="vertical-align: middle">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
                    <table id="grid" align="center" ></table>
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
	</table>
    <script type="text/javascript">
            function createClientGrid() {
                //创建列表对象
                $create($HBRootNS.ClientGrid,
		    {
		        pageSize: 10,
		        cellPadding: "3px",

		        emptyDataText: "没有记录！",
		        columns: [
		         { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px"} },
                 { dataField: "Key", headerText: "变量Key", itemStyle: { width: "150px" }, headerStyle: { width: "50px" },
                     cellDataBound: function (grid, e) {
                         //var data = Sys.Serialization.JavaScriptSerializer.serialize(e.data);
                         var linkText = "<a href='#' style='color: Black;' onclick='openModalDialog(\"{1}\");'>{0}</a>";
                         e.cell.innerHTML = String.format(linkText, e.data["Key"].toString(), e.data["Key"].toString());
                     }
                 }
                 , { dataField: "OriginalType", headerText: "类型", itemStyle: { width: "100px" }, headerStyle: { width: "100px" },
                     cellDataBound: function (grid, e) {
                         var enumList = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenDatatypeJson").value);
                         var text = "";
                         for (var i = 0; i < enumList.length; i++) {
                             if (enumList[i].EnumValue == e.data["OriginalType"]) {
                                 text = enumList[i].Name;
                                 break;
                             }
                         }
                         e.cell.innerHTML = text;
                     }
                 }
                , { dataField: "OriginalValue", headerText: "值", itemStyle: { width: "190px" }, headerStyle: { width: "100px"} }
                , { dataField: "Name", headerText: "变量名", itemStyle: { width: "200px" },headerStyle: { width: "50px"} }
                , { dataField: "Description", headerText: "描述", itemStyle: { width: "100px" }, headerStyle: { width: "50px"} }
            ],

		        allowPaging: true,
		        autoPaging: true,
		        pagerSetting: { mode: $HBRootNS.PageSettingMode.text, position: $HBRootNS.PagerPosition.bottom, firstPageText: "<<", prevPageText: "上页", nextPageText: "下页", lastPageText: ">>" }

			, style: { width: "800px" }
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
        <input type="hidden" runat="server" id="hiddenBranchProcessTemplate" />
        <input type="hidden" runat="server" id="hiddenDatatypeJson" />
    </form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
        	onDocumentLoad();
        });
	</script>

</body>
</html>
