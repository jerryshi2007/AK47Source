<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfExternalUsers.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfExternalUsers" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>外部相关人员</title>
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css"/>

    <script type="text/javascript" >
        var arrParaObj = [];
        var arrExternalUsers = [];
        function onDocumentLoad() {
            arrParaObj = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.jsonStr);
            for (var i = 0; i < arrParaObj.length; i++) {
                arrExternalUsers.push(arrParaObj[i]);
            }
            createClientGrid();
            bindGrid(arrExternalUsers);
        }
        function bindGrid(dataSource) {
            var gridDatasource = dataSource ? dataSource : branchProcessTemplates;
            var grid = $find("grid");
            grid.set_dataSource(gridDatasource);
        }
        function openModalDialog(key) {
            var _extUser;
            var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            if ("" != key) {
                _extUser = getExtUserByKey(key)
                result = window.showModalDialog("WfExternalUserEditor.aspx",
                { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(_extUser),
                    existExternalUserJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrExternalUsers)
                }, sFeature);
            } else {
                result = window.showModalDialog("WfExternalUserEditor.aspx", { jsonStr: null,
                    existExternalUserJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrExternalUsers)
                }, sFeature);
            }
            if (result) {
                var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                if (resultObj.extUserobj) {
                    arrExternalUsers[resultObj.index] = resultObj.extUserobj;
                } else {
//                    for (var i = 0; i < arrExternalUsers.length; i++) {
//                        if (arrExternalUsers[i].Key == resultObj.Key) {
//                            alert("已经有相同Key的用户存在");
//                            return;
//                        }
//                    }
                    arrExternalUsers.push(resultObj);
                }
                bindGrid(arrExternalUsers);

            }
        }

        function getExtUserByKey(key) {
            var extUser = {};
            for (var i = 0; i < arrExternalUsers.length; i++) {
                if (arrExternalUsers[i].Key == key) {
                    extUser["extUserobj"] = arrExternalUsers[i];
                    extUser["index"] = i;
                    break;
                }
            }
            return extUser;
        }

        function addExtUsere() {
            openModalDialog("");
        }
        function deleteExtUser() {
            var grid = $find("grid");
            var selectedData = grid.get_selectedData();
            if (selectedData.length <= 0)
                alert("没有选择外部人员。");
            if (!(selectedData.length > 0 && confirm("确定删除？"))) {
                return;
            }
            for (var i = 0; i < selectedData.length; i++) {
                var element = selectedData[i];
                for (var j = 0; j < arrExternalUsers.length; j++) {
                    if (arrExternalUsers[j].Key == element.Key) {
                        arrExternalUsers.splice(j, 1);
                    }
                }
            }
            bindGrid(arrExternalUsers);
        }
        function onbtnOKClick() {
            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrExternalUsers) };

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
					<span class="dialogLogo">外部相关人员</span>
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
                                <td style="height:30px; background-color:#C0C0C0"><a href="#" onclick="addExtUsere();"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
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
				{ dataField: "Name", headerText: "人员名称", itemStyle: { width: "50px" }, headerStyle: { width: "100px" },
				    cellDataBound: function (grid, e) {
				        var linkText = "<a href='#' style='color: Black;' onclick='openModalDialog(\"{1}\");'>{0}</a>";
				        e.cell.innerHTML = String.format(linkText, e.data["Name"].toString(), e.data["Key"].toString());
                    } },
                { dataField: "Gender", headerText: "性别", itemStyle: { textAlign: "left" },
                    cellDataBound: function (grid, e) {
                        var enumList = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenEnumGenderList").value);
                        var text = "";
                        for (var i = 0; i < enumList.length; i++) {
                            if (enumList[i].EnumValue == e.data["Gender"]) {
                                text = enumList[i].Description;
                                break;
                            }
                        }
                        e.cell.innerHTML = text;
                    } },
                { dataField: "Phone", headerText: "固定电话", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } },
                { dataField: "MobilePhone", headerText: "移动电话", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } },
                { dataField: "Title", headerText: "头衔", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } },
                { dataField: "Email", headerText: "邮箱地址", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
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
