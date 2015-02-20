<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfBranchProcessTemplates.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfBranchProcessTemplates" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>分支流程编辑</title>
    <base target="_self">
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css"/>
    <link href="/MCSWebApp/Css/WebControls/DeluxeTree.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>

    <script type="text/javascript">
        var branchProcessTemplates;
        var activities;
        var executeSeq;
        var blockingType;
        function onDocumentLoad() {
            executeSeq = Sys.Serialization.JavaScriptSerializer.deserialize($get("hiddenWfBranchProcessExecuteSequence").value);
            blockingType = Sys.Serialization.JavaScriptSerializer.deserialize($get("hiddenWfBranchProcessBlockingType").value);

            var jsonStr = window.dialogArguments.jsonStr;

            branchProcessTemplates = Sys.Serialization.JavaScriptSerializer.deserialize(jsonStr);
            branchProcessTemplates = getWfProcessTemplates(branchProcessTemplates);
            activities = window.dialogArguments.Activities;

            createClientGrid();

            var temps = getWfProcessTemplates(branchProcessTemplates);
            if (temps) {
                bindGrid(temps);
            }
        }
        function getWfProcessTemplates(branchProcessTemps) {
            var temps = [];
            for (var i = 0; i < branchProcessTemps.length; i++) {
                temps.push(branchProcessTemps[i]);
            }
            return temps;
        }
        function bindGrid(dataSource) {
            var gridDatasource = dataSource ? dataSource : branchProcessTemplates;
            var grid = $find("grid");
            grid.set_dataSource(gridDatasource);
        }

        function setGridControlDataSource(gridControl, dataSource) {
            if (gridControl.get_autoPaging()) {
                var pageIndex = 0;
                var pageSize = 5;
            }
            else {
                var pageIndex = gridControl.get_pageIndex();
                var pageSize = gridControl.get_pageSize();
            }
            gridControl.set_dataSource(dataSource);

        }

        function openModalDialog(branchProcessKey) {
            var sFeature = "dialogWidth:480px; dialogHeight:650px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            var result;
            if ("" != branchProcessKey) {
                result = window.showModalDialog("BranchProcessTemplateEditorNew.aspx",
                //result = window.showModalDialog("BranchProcessTemplateEditor.aspx",
                { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(getBranchProcessWithKey(branchProcessKey)),
                activities:activities,
                existBranProcessTempJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(branchProcessTemplates)}, sFeature);
        } else {
            //result = window.showModalDialog("BranchProcessTemplateEditor.aspx",
            result = window.showModalDialog("BranchProcessTemplateEditorNew.aspx",
                { jsonStr: null, activities: activities, existBranProcessTempJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(branchProcessTemplates) }, sFeature);
            }
            if (result) {
                var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                if (resultObj.branchProcessTempObj) {
                    branchProcessTemplates[resultObj.index] = resultObj.branchProcessTempObj;
                } else {
//                    for (var i = 0; i < branchProcessTemplates.length; i++) {
//                        if (branchProcessTemplates[i].Key == resultObj.Key) {
//                            alert("已经有相同Key的子流程存在");
//                            return;
//                        }
//                    }
                    branchProcessTemplates.push(resultObj);
                }
                var grid = $find("grid");
                grid.set_dataSource(branchProcessTemplates);
            }
        }
        function getBranchProcessWithKey(branchProcessKey) {
            var branchProcessTemp = {};
            for (var i = 0; i < branchProcessTemplates.length; i++) {
                if (branchProcessTemplates[i].Key == branchProcessKey) {
                    branchProcessTemp["branchProcessTempObj"] = branchProcessTemplates[i];
                    branchProcessTemp["index"] = i;
                    break;
                }
            }
            return branchProcessTemp;
        }

        function addTemplate() {
            openModalDialog("");
        }
        function deleteTemplate() {
            var grid = $find("grid");
            var data = grid.get_selectedData();
            if (data.length <= 0)
                alert("没有选择分支流程。");

            if (!(data.length > 0 && confirm("确定删除？"))) {
                return;
            }
            for (var i = 0; i < data.length; i++) {
                var element = data[i];
                for (var j = 0; j < branchProcessTemplates.length; j++) {
                    if (branchProcessTemplates[j].Key == element.Key) {
                        branchProcessTemplates.splice(j, 1);
                    }
                }
            }
            grid.set_dataSource(branchProcessTemplates);
        }

        function onbtnOKClick() {
            window.returnValue = { jsonStr: jQuery.toJSON(branchProcessTemplates) };
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
					<span class="dialogLogo">分支流程编辑</span>
				</div>
			</td>
		</tr>
        <tr>
            <td style="height:30px; background-color:#C0C0C0">
                <a href="#" onclick="addTemplate();"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
                <a href="#" onclick="deleteTemplate();"><img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除"" border="0" /></a>
            </td>
        </tr>
		<tr>
			<td style="vertical-align: center">
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
		        pageSize: 5,
		        cellPadding: "3px",

		        emptyDataText: "没有记录！",
		        columns: [
		         { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px"} },
                 { dataField: "Key", headerText: "流程模板Key", itemStyle: { width: "200px" }, headerStyle: { width: "200px" }, cellDataBound: function (grid, e) {
                     //var data = Sys.Serialization.JavaScriptSerializer.serialize(e.data);
                     var linkText = "<a href='#' style='color: Black;' onclick='openModalDialog(\"{1}\");'>{0}</a>";
                     e.cell.innerHTML = String.format(linkText, e.data["Key"].toString(), e.data["Key"].toString());
                 }
                 }
                , { dataField: "Name", headerText: "分支流程模板名称", headerStyle: { width: "200px" }, itemStyle: { width: "200px"} }
                , { dataField: "BlockingType", headerText: "阻塞类型", headerStyle: { width: "200px" }, cellDataBound: function (grid, e) {
                    for (var i = 0; i < blockingType.length; i++) {
                        if (e.data["BlockingType"] == blockingType[i].EnumValue)
                            e.cell.innerHTML = blockingType[i].Description;
                    }
                } 
                }
				, { dataField: "ExecuteSequence", headerText: "串行/并行", itemStyle: { width: "180px" }, headerStyle: { width: "100px" }, cellDataBound: function (grid, e) {
				    for (var i = 0; i < executeSeq.length; i++) {
				        if (e.data["ExecuteSequence"] == executeSeq[i].EnumValue)
				            e.cell.innerHTML = executeSeq[i].Description;
				    }
				} 
				}
		        //,{ dataField: "fullPath", headerText: "人员标示", itemStyle: {  textAlign: "left" }, cellDataBound: function (grid, e) { } }
            ],

		        allowPaging: true,
		        autoPaging: true,
		        pagerSetting: { mode: $HBRootNS.PageSettingMode.text, position: $HBRootNS.PagerPosition.bottom, firstPageText: "<<", prevPageText: "上页", nextPageText: "下页", lastPageText: ">>" }

			, style: { width: "790px" }
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
        <input type="hidden" runat="server" id="hiddenWfBranchProcessBlockingType" />
        <input type="hidden" runat="server" id="hiddenWfBranchProcessExecuteSequence" />
    </form>
	<script type="text/javascript">
	    Sys.Application.add_load(function () {
	        onDocumentLoad();
	    });
	</script>
</body>
</html>
