<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfServiceOperationDefList.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfServiceOperationDefList" %>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>调用服务列表</title>
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" src="../js/common.js"></script>
    <script type="text/javascript" >
        var allSvcOperationDef = [];
        function onDocumentLoad() {
            allSvcOperationDef = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.jsonStr);
            createClientGrid();
            bindGrid(allSvcOperationDef);
        }

        function bindGrid(dataSource) {
            var gridDatasource = dataSource ? dataSource : [];
            var grid = $find("grid");
            grid.set_dataSource(gridDatasource);
        }

        function openModalDialog(key) {
            var _extUser;
            var url = "WfServiceOperationDefEditor.aspx";
            var sFeature = "dialogWidth:680px; dialogHeight:460px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            if ("" != key) {
                var opDef = allSvcOperationDef.get(key, function (o, v) {
                    if (o.Key == v) return true;
                    return false;
                });

                result = window.showModalDialog(url,
                { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(opDef),
                    existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(allSvcOperationDef)
                }, sFeature);
            } else {
                result = window.showModalDialog(url, 
                { jsonStr: null,
                    existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(allSvcOperationDef)
                }, sFeature);
            }

            if (result) {
                var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                if (resultObj) {
                    var fnCompare = function (o, v) {
                        if (o.Key == v) return true;
                        return false;
                    };
                    var isExist = allSvcOperationDef.has(resultObj.Key, fnCompare);
                    if (!isExist) {
                        allSvcOperationDef.push(resultObj);
                    }
                    else {
                        allSvcOperationDef.remove(resultObj.Key, fnCompare);
                        allSvcOperationDef.push(resultObj);
                    }
                }
                bindGrid(allSvcOperationDef);
            }
        }

        function createSvcOperation() {
            openModalDialog("");
        }

        function deleteExtUser() {
            var grid = $find("grid");
            var selectedData = grid.get_selectedData();
            if (selectedData.length <= 0)
                alert("请选择要删除的数据。");
            if (!(selectedData.length > 0 && confirm("确定删除？"))) {
                return;
            }
            for (var i = 0; i < selectedData.length; i++) {
                var element = selectedData[i];
                allSvcOperationDef.remove(element.Key, function (o, v) {
                    if (o.Key == v) return true;
                    return false;
                });
            }
            bindGrid(allSvcOperationDef);
        }

        function onbtnOKClick() {
            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(allSvcOperationDef) };
            top.close();
        }
    </script>
</head>
<body>
	<form id="serverForm" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">调用服务列表</span>
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
                                <td style="height:30px; background-color:#C0C0C0"><a href="#" onclick="createSvcOperation();"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
                                        <a href="#" onclick="deleteExtUser();"><img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除"" border="0" /></a>
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
				{ dataField: "Key", headerText: "Key", itemStyle: { textAlign: "center" },
				    cellDataBound: function (grid, e) {
				        var linkText = "<a href='#' style='color: Black;' onclick='openModalDialog(\"{1}\");'>{0}</a>";
				        e.cell.innerHTML = String.format(linkText, e.data["Key"].toString(), e.data["Key"].toString());
				    }
				},
                { dataField: "AddressDef", headerText: "服务地址", itemStyle: { width: "250px" },
                    cellDataBound: function (grid, e) {
                        e.cell.innerHTML = e.data["AddressDef"] == null ? "" : e.data["AddressDef"].Address.toString();
                    }
                },
                { dataField: "OperationName", headerText: "方法名", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } },
                { dataField: "Params", headerText: "方法参数个数", itemStyle: { textAlign: "left" },
                    cellDataBound: function (grid, e) {
                        e.cell.innerHTML = e.data["Params"].length;
                    }
                },
                { dataField: "RtnXmlStoreParamName", headerText: "返回值保存变量名", itemStyle: { textAlign: "left" },
                    cellDataBound: function (grid, e) { }
                }
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
    <input type="hidden" id="hiddenEnumGenderList" runat="server" />
	</form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            onDocumentLoad();
        });
    </script>
</body>
</html>
