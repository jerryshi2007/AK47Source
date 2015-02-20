<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfInternalRelativeUsers.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfInternalRelativeUsers" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>内部相关人员</title>
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" >
        var arrParaObj = [];
        var arrRelateUsers = [];
        function onDocumentLoad() {
            arrParaObj = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.jsonStr);
            for (var i = 0; i < arrParaObj.length; i++) {
                for (var item in arrParaObj[i]) {
                    if (item != "__type" && item != "shortType") {
                        arrRelateUsers.push(arrParaObj[i][item]);
                        break;
                    }
                }
            }
            createClientGrid();
            bindGrid(arrRelateUsers);
        }
        function bindGrid(dataSource) {
            var gridDatasource = dataSource ? dataSource : branchProcessTemplates;
            var grid = $find("grid");
            grid.set_dataSource(gridDatasource);
        }
        function addRelateUsere() {
            var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var result;
            result = window.showModalDialog("SelectOUUser.aspx?resourceType=2", null, sFeature);
            if (!result) {
                return;
                }
            result = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
            if (result) {
                for (var i = 0; i < result.length; i++) {
                    if (checkUserExist(result[i])) {
                        alert("不能添加重复人员。");
                        return; 
                    }
                    arrRelateUsers.push(result[i]);
                    var wfUserResDesc = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenWfUserDescTemplate").value);
                    wfUserResDesc.User = result[i];
                    arrParaObj.push(wfUserResDesc);
                }
                bindGrid(arrRelateUsers);
            }
        }
        function checkUserExist(user) {
            for (var i = 0; i < arrRelateUsers.length; i++) {
                if (arrRelateUsers[i].id == user.id) {
                    return true;
                }
            }
            return false;
        }
        function deleteRelateUser() {
            var grid = $find("grid");
            var selectedData = grid.get_selectedData();
            if (selectedData.length <= 0)
                alert("没有选择内部人员。");
            for (var i = 0; i < selectedData.length; i++) {
                var element = selectedData[i];
                for (var j = 0; j < arrRelateUsers.length; j++) {
                    if (arrRelateUsers[j] == element) {
                        arrRelateUsers.splice(j, 1);
                    }
                    for (var item in arrParaObj[j]) {
                        if (item != "__type" && item != "shortType") {
                            if (arrParaObj[j][item] == element) {
                                arrParaObj.splice(j, 1);
                            }
                        }
                    }
                }
            }
            bindGrid(arrRelateUsers);
        }

        function onbtnOKClick() {
            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(arrParaObj) }
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
					<span class="dialogLogo">内部相关人员</span>
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
                                <td style="height:30px; background-color:#C0C0C0"><a href="#" onclick="addRelateUsere();"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
                                        <a href="#" onclick="deleteRelateUser();"><img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除"" border="0" /></a>
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
		        columns: [{ selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px"} },
                { dataField: "logOnName", headerText: "登录名", sortExpression: "logOnName", headerStyle: { width: "100px"} },
				{ dataField: "displayName", headerText: "人员名称", itemStyle: { width: "50px" }, headerStyle: { width: "100px" }, cellDataBound: function (grid, e) { } },
				{ dataField: "fullPath", headerText: "人员标示", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
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
    <input type="hidden" id="hiddenWfUserDescTemplate" runat="server" />
	</form>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
	        onDocumentLoad();
	    });
    </script>
</body>
</html>
