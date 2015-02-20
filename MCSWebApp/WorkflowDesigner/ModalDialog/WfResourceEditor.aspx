<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfResourceEditor.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.WfResourceEditor" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>流程模板资源编辑</title>
    <base target="_self" />
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css" />
    <link href="/MCSWebApp/Css/WebControls/DeluxeTree.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <script type="text/javascript">
        var RESOURCE_TYPE_ARRAY;
        var _ResourceTypeDictionary = {
            WfRoleResourceDescriptor: {
                columns:
                [
                    { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                    {
                        dataField: "id", headerText: "成员", headerStyle: { width: "32px", textAlign: "center" }, itemStyle: { textAlign: "center" }, cellDataBound: function (grid, e) {
                            e.cell.innerHTML = "<a target='_blank' href='/MCSWebApp/PermissionCenter/lists/AppRoleMembers.aspx?role=" + e.data.id + "'><img src='../images/role16.gif'/></a>";
                        }
                    },
                    { dataField: "codeName", headerText: "英文标识", headerStyle: { width: "50px" } },
                    { dataField: "name", headerText: "中文名称", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
                ],
                resourceUrl: "WfRoleResourceEditor.aspx",
                feature: "dialogWidth:400px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.id.toLowerCase() === selected.Role.id.toLowerCase(); },
                getSelectedName: function (selected) { return selected.Role.name },
                decroateSelected: function (selected) { return selected.Role; },
                templateID: "hiddenRoleResTemplate",
                overridable: false
            },
            WfGroupResourceDescriptor: {
                columns:
                [
                    { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                    { dataField: "displayName", headerText: "组名称" },
                    { dataField: "fullPath", headerText: "组路径", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
                ],
                resourceUrl: "SelectOUUser.aspx?resourceType=4",
                feature: "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.id.toLowerCase() === selected.id.toLowerCase(); },
                getSelectedName: function (selected) { return selected.displayName },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenGroupResTemplate",
                overridable: false
            },
            WfDepartmentResourceDescriptor: {
                columns:
                [
                    { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                    { dataField: "displayName", headerText: "组织名称", cellDataBound: function (grid, e) { } },
                    { dataField: "fullPath", headerText: "组织路径", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
                ],
                resourceUrl: "SelectOUUser.aspx?resourceType=1",
                feature: "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.id.toLowerCase() === selected.id.toLowerCase(); },
                getSelectedName: function (selected) { return selected.displayName },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenDepartResTemplate",
                overridable: false
            },
            WfUserResourceDescriptor: {
                columns:
                [
                    { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                    { dataField: "logOnName", headerText: "登录名", sortExpression: "logOnName", headerStyle: { width: "100px" } },
                    { dataField: "displayName", headerText: "人员名称", itemStyle: { width: "50px" }, headerStyle: { width: "100px" }, cellDataBound: function (grid, e) { } },
                    { dataField: "fullPath", headerText: "人员路径", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
                ],
                resourceUrl: "SelectOUUser.aspx?resourceType=2",
                feature: "dialogWidth:400px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.id.toLowerCase() === selected.id.toLowerCase(); },
                getSelectedName: function (selected) { return selected.displayName },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenUserResTemplate",
                overridable: false
            },
            WfActivityAssigneesResourceDescriptor: {
                columns:
                [
                    { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                    {
                        dataField: "ActivityKey", headerText: "环节Key", headerStyle: { width: "100px" }, itemStyle: { width: "50px" },
                        cellDataBound: function (grid, e) { e.cell.innerText = e.data; }
                    },
                    {
                        dataField: "ActivityKey", headerText: "环节名称", itemStyle: {}, headerStyle: {}, cellDataBound: function (grid, e) {
                            for (var i = 0; i < _activities.length; i++) {
                                if (_activities[i].Key == e.data) {
                                    e.cell.innerText = _activities[i].Name;
                                }
                            }
                        }
                    }
                ],
                resourceUrl: "ActivityKeyEditor.aspx",
                feature: "dialogWidth:400px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.toLowerCase() === selected.toLowerCase(); },
                getSelectedName: function (selected) { return selected; },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenActAssigneeResTemplate",
                overridable: false
            },
            WfActivityOperatorResourceDescriptor: {
                columns:
                [
                    { selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                    {
                        dataField: "ActivityKey", headerText: "环节Key", headerStyle: { width: "100px" }, itemStyle: { width: "50px" },
                        cellDataBound: function (grid, e) { e.cell.innerText = e.data; }
                    },
                    {
                        dataField: "ActivityKey", headerText: "环节名称", itemStyle: {}, headerStyle: {}, cellDataBound: function (grid, e) {
                            for (var i = 0; i < _activities.length; i++) {
                                if (_activities[i].Key == e.data) {
                                    e.cell.innerText = _activities[i].Name;
                                }
                            }
                        }
                    }
                ],
                resourceUrl: "ActivityKeyEditor.aspx",
                feature: "dialogWidth:400px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.toLowerCase() === selected.toLowerCase(); },
                getSelectedName: function (selected) { return selected; },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenActOperatorResTemplate",
                overridable: false
            },
            WfDynamicResourceDescriptor: {
                columns:
                [
                    {
                        selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" }
                    },
                    {
                        dataField: "Name", headerText: "名称", sortExpression: "Name", headerStyle: { width: "200px" }, itemStyle: { width: "200px" },
                        cellDataBound: function (grid, e) {
                            var linkText = "<a href='#' style='color: Red;' onclick='openConditionlDialog(\"{1}\");'>{0}</a>";
                            e.cell.innerHTML = String.format(linkText, e.data["Name"].toString(), e.data["Name"].toString());
                        }
                    },
                    {
                        dataField: "Expression", headerText: "条件", itemStyle: { width: "380px" }, headerStyle: { width: "380px" },
                        cellDataBound: function (grid, e) {
                            e.cell.innerHTML = "<div style='width:375px;overflow:auto'>" + e.data.Condition.Expression + "</div>";
                        }
                    }
                ],
                resourceUrl: "WfConditionEditor.aspx?showNameInput=true",
                feature: "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return true; },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenDynamicResTypesDataTemplate",
                overridable: true
            },
            WfAURoleResourceDescriptor: {
                columns:
                [
                    {
                        selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" }
                    },
                    {
                        dataField: "AUSchemaRole", headerText: "角色", sortExpression: "AUSchemaRole", headerStyle: { width: "200px" }, itemStyle: { width: "200px" },
                        cellDataBound: function (grid, e) {
                            e.cell.innerHTML = '';
                            e.cell.appendChild(document.createTextNode(e.data.Description || ''));
                        }
                    }
                ],
                resourceUrl: "WfAUSchemaRoleEditor.aspx",
                feature: "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no",
                comparer: function (resObj, selected) { return resObj.SchemaRoleID.toLowerCase() === selected.AUSchemaRole.SchemaRoleID.toLowerCase(); },
                getSelectedName: function (selected) { return selected.AUSchemaRole.Description; },
                decroateSelected: function (selected) { return selected.AUSchemaRole; },
                templateID: "hiddenAURoleResTemplate",
                overridable: false
            },
            WfActivityMatrixResourceDescriptor: {
                columns: [],
                comparer: function (resObj, selected) { return true; },
                decroateSelected: function (selected) { return selected; },
                templateID: "hiddenActiveMatrixResTemplate",
                overridable: true
            }
        };

        var _resourceCategory = null;
        var _resources;
        var _activities;
        var _currentResourceType = "";
        var _treeRootNode;

        function onDocumentLoad() {
            prepareData();
            createClientGrid();
            selectFirstTreeNode();
            bindGrid();
        }

        function selectFirstTreeNode() {
            if (_currentResourceType == '') {
                var node = _treeRootNode.get_children()[0];
                var nodeValue = node.get_value();
                node.set_selected(true);
                _currentResourceType = nodeValue;
            }
            else {
                var nodes = $find('tree').get_nodes()[0].get_children();
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].get_value() == _currentResourceType) {
                        nodes[i].set_selected(true);
                        break;
                    }
                }
            }
        }

        function bindGrid(dataSource) {
            var treeSelectedNode = $find("tree").get_selectedNode();
            var gridDatasource = dataSource ? dataSource : _resourceCategory[_currentResourceType].gridResources;

            var grid = $find("grid");

            setGridControlDataSource(grid, gridDatasource);
        }

        function bindTree(dataObject) {
            var tree = $find("tree");
            tree._rootNode.clearChildren();
            var root = $HGRootNS.DeluxeTreeNode.createNode({
                text: "资源分类",
                expanded: true
            });

            tree._rootNode.appendChild(root);

            if (!_treeRootNode) {
                _treeRootNode = root;
            }

            for (var item in dataObject) {
                root.appendChild(createFieldTreeNode(dataObject[item]));
            }

            selectFirstTreeNode();
        }

        function createFieldTreeNode(field) {
            return $HGRootNS.DeluxeTreeNode.createNode({
                text: String.format("{0}{1}", field.displayName, getResourceCount(field.shortType)),
                nodeCloseImg: field.imgUrl,
                value: field.shortType
            });
        }

        function getResourceCount(category) {
            var reses = _resourceCategory[category].gridResources;

            return reses.length == 0 ? "" : String.format("({0})", reses.length);
        }

        function switchListEditor(useListEditor) {
            if (useListEditor) {
                $get("listEditor").style.display = "block";
                $get("activityMatrixEditor").style.display = "none";
            }
            else {
                $get("listEditor").style.display = "none";
                $get("activityMatrixEditor").style.display = "block";
            }
        }

        function onTreeNodeSelecting(sender, e) {
            var nodeValue = e.node.get_value();
            if (nodeValue) {
                _currentResourceType = nodeValue;

                if (_currentResourceType == "WfActivityMatrixResourceDescriptor") {
                    switchListEditor(false);
                }
                else {
                    switchListEditor(true);
                    bindGridByResourceType(_currentResourceType);
                }
            }
            else
                e.cancel = true;
        }

        function bindGridByResourceType(resourceType) {
            var grid = $find("grid");

            var dataSource = _resourceCategory[resourceType].gridResources.length > 0 ? _resourceCategory[resourceType].gridResources : _resourceCategory[resourceType].Resources;
            var grid = $find("grid");
            grid._clearAllRows();

            grid.set_columns(_ResourceTypeDictionary[resourceType].columns);

            setGridControlDataSource(grid, dataSource);
            grid.set_pageIndex(0);
        }

        function openConditionlDialog(name) {
            var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:no;scroll:no;status:no";
            var sUrl = "WfConditionEditor.aspx?showNameInput=true";
            var result;
            var editItem;
            var index;

            for (var i = 0; i < _resources.length; i++) {
                if (_resources[i].Name && _resources[i].Name == name) {
                    editItem = _resources[i];
                    index = i;
                }
            }

            result = window.showModalDialog(sUrl,
                { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(editItem), index: index }, sFeature);

            if (result) {
                var editDynamicRole = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);

                _resources[result.index] = editDynamicRole;
                _resourceCategory = convertResourcesToCategries(_resources);

                var grid = $find("grid");

                grid.set_pageIndex(0);

                var dataSource = _resourceCategory[_currentResourceType].gridResources;

                setGridControlDataSource(grid, dataSource);
            }
        }

        function convertResourcesToCategries(_resources) {
            var categories = createCategories();

            for (var i = 0; i < _resources.length; i++) {
                var resource = _resources[i];

                if (typeof (resource) != "undefined") {
                    var categoryName = resource.shortType;
                    var category = categories[categoryName];

                    if (typeof (category) != "undefined") {
                        category.Resources.push(resource);

                        if (categoryName != "WfDynamicResourceDescriptor") {
                            for (var item in resource) {
                                if (item != "__type" && item != "shortType") {
                                    category.gridResources.push(resource[item]);
                                    break;
                                }

                            }
                        } else {
                            category.gridResources.push(resource);
                        }
                    }
                }
            }

            return categories;
        }

        function createCategories() {
            var categories = {};

            for (var i = 0; i < RESOURCE_TYPE_ARRAY.length; i++) {
                var resource = RESOURCE_TYPE_ARRAY[i];
                var categoryName = resource.shortType;
                var category = categories[categoryName];

                if (!category) {
                    category = { Name: categoryName, Resources: [], gridResources: [] };
                    categories[categoryName] = category;
                }
            }

            return categories;
        }

        function prepareData() {
            RESOURCE_TYPE_ARRAY = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenResTypesData").value)

            _resources = [];
            _activities = [];

            if (window.dialogArguments) {
                if (window.dialogArguments.jsonStr) //processDesc._activities[0].Resources;
                    _resources = Sys.Serialization.JavaScriptSerializer.deserialize(window.dialogArguments.jsonStr);

                if (window.dialogArguments.Activities)  //? paraData.Activities : []; 
                    _activities = window.dialogArguments.Activities;
            }

            _resourceCategory = convertResourcesToCategries(_resources);

            setActivityMatrixStatus(findFirstItemInResourcesByType("WfActivityMatrixResourceDescriptor"));

            bindTree(RESOURCE_TYPE_ARRAY);
        }

        function compareSelectedInGridResources(currentResourceType, selected) {
            var existed = false;

            for (var i = 0; i < _resourceCategory[currentResourceType].gridResources.length; i++) {
                var item = _resourceCategory[currentResourceType].gridResources[i];

                if (_ResourceTypeDictionary[currentResourceType].comparer(item, selected)) {
                    existed = true;
                    break;
                }
            }

            return existed;
        }

        function checkSelectedInGridResources(currentResourceType, selected) {
            var existed = compareSelectedInGridResources(currentResourceType, selected);

            if (existed && _ResourceTypeDictionary[currentResourceType].overridable == false) {
                var e = Error.create();

                e.message = _ResourceTypeDictionary[currentResourceType].getSelectedName(selected) + "已存在";

                throw e;
            }
        }

        function getResourceTemplate(currentResourceType) {
            var template = $get(_ResourceTypeDictionary[currentResourceType].templateID).value;

            return Sys.Serialization.JavaScriptSerializer.deserialize(template);
        }

        function addSelectedResource() {
            try {
                addResource();
            }
            catch (e) {
                $showError(e);
            }
        }

        function addResource() {
            var sResourceURL = _ResourceTypeDictionary[_currentResourceType].resourceUrl;
            var sFeature = _ResourceTypeDictionary[_currentResourceType].feature;

            var result;

            if (_currentResourceType === "WfDynamicResourceDescriptor") {
                result = window.showModalDialog(sResourceURL, { jsonStr: document.getElementById("hiddenDynamicResTypesDataTemplate").value }, sFeature);
            }
            else {
                result = window.showModalDialog(sResourceURL, _activities, sFeature);
            }

            if (!result)
                return;

            switch (_currentResourceType) {
                case "WfDynamicResourceDescriptor":
                    result = [Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr)];
                    break;
                case "WfRoleResourceDescriptor":
                case "WfAURoleResourceDescriptor":
                    result = [Sys.Serialization.JavaScriptSerializer.deserialize(result)];
                    break;
                case "WfActivityAssigneesResourceDescriptor":
                case "WfActivityOperatorResourceDescriptor":
                    break;
                default:
                    result = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
                    break
            }

            if (!result) {
                return;
            }

            for (var i = 0; i < result.length; i++) {
                checkSelectedInGridResources(_currentResourceType, result[i]);
            }

            for (var i = 0; i < result.length; i++) {
                var decroateResult = _ResourceTypeDictionary[_currentResourceType].decroateSelected(result[i]);
                var resourceObj = getResourceTemplate(_currentResourceType);

                _resourceCategory[_currentResourceType].gridResources.push(decroateResult);

                if (_currentResourceType != "WfDynamicResourceDescriptor") {
                    for (var item in resourceObj) {
                        if (item != "__type" && item != "shortType") {
                            resourceObj[item] = decroateResult;
                            break;
                        }
                    }
                } else {
                    resourceObj = decroateResult;
                }

                _resources.push(resourceObj);
            }

            var grid = $find("grid");
            grid.set_pageIndex(0);

            var dataSource = _resourceCategory[_currentResourceType].gridResources.length > 0 ? _resourceCategory[_currentResourceType].gridResources : _resourceCategory[_currentResourceType].Resources;

            setGridControlDataSource(grid, dataSource);

            bindTree(RESOURCE_TYPE_ARRAY);
        }

        function findFirstItemIndexInResourcesByType(resourceType) {
            var result = -1;

            for (var i = 0; i < _resources.length; i++) {
                if (_resources[i].shortType == resourceType) {
                    result = i;
                    break;
                }
            }
            return result;
        }

        function findFirstItemInResourcesByType(resourceType) {
            var index = findFirstItemIndexInResourcesByType(resourceType);
            var result = null;

            if (index != -1)
                result = _resources[index];

            return result;
        }

        function deleteResource() {
            var grid = $find("grid");
            var selectedResources = grid.get_selectedData();
            if (selectedResources.length <= 0) {
                alert("没有选择资源。");
            }
            if (selectedResources.length > 0 && $HGClientMsg.confirm("确定删除所选数据？")) {
                if (_currentResourceType != "WfDynamicResourceDescriptor") {
                    Array.forEach(selectedResources,
                        function (element, index, array) {
                            var itemInResources;
                            Array.forEach(_resourceCategory[_currentResourceType].Resources, function (innerElement, index, array) {
                                for (var i in innerElement) {
                                    if (i != "__type" && i != "shortType" && innerElement[i] === element) {
                                        itemInResources = innerElement;
                                        break;
                                    }
                                }
                            },
                    null
                    );
                            Array.remove(_resourceCategory[_currentResourceType].gridResources, element);
                            Array.remove(_resourceCategory[_currentResourceType].Resources, itemInResources ? itemInResources : element);

                            for (var j = 0; j < _resources.length; j++) {
                                if (_resources[j] == itemInResources) {
                                    _resources.splice(j, 1);
                                }
                            }
                        },
                    null);
                }
                else {
                    for (var l = 0; l < selectedResources.length; l++) {
                        for (var m = 0; m < _resources.length; m++) {
                            if (_resources[m].Name && selectedResources[l].Name && _resources[m].Name == selectedResources[l].Name) {
                                _resources.splice(m, 1);
                            }
                        }
                    }
                    _resourceCategory = convertResourcesToCategries(_resources);
                }

                var grid = $find("grid");

                var dataSource = _resourceCategory[_currentResourceType].gridResources.length > 0 ? _resourceCategory[_currentResourceType].gridResources : _resourceCategory[_currentResourceType].Resources;
                setGridControlDataSource(grid, dataSource);

                bindTree(RESOURCE_TYPE_ARRAY);
            }
        }

        function onBtnOKClick() {
            window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(_resources) };
            top.close();
        }

        function onUploadActivityMatrix() {
            $find("uploadActivityMatrix").showDialog();
        }

        function onUploadActivityMatrixCompleted(e) {
            if (e.dataChanged == true) {
                var activityMatrix = Sys.Serialization.JavaScriptSerializer.deserialize(e.data);

                _resourceCategory[_currentResourceType].gridResources = [];
                _resourceCategory[_currentResourceType].gridResources.push(activityMatrix);

                var index = findFirstItemIndexInResourcesByType(_currentResourceType);

                if (index == -1)
                    _resources.push(activityMatrix);
                else
                    _resources[index] = activityMatrix;

                setActivityMatrixStatus(activityMatrix);

                bindTree(RESOURCE_TYPE_ARRAY);
            }
        }

        function setActivityMatrixStatus(activityMatrix) {

            $get("deleteActivityMatrix").disabled = (activityMatrix == null);
            $get("downloadActivityMatrix").disabled = (activityMatrix == null);

            setActivityMatrixInfo(activityMatrix);
        }

        function setActivityMatrixInfo(activityMatrix) {
            $get("activityMatrixInfo").innerText = getActivityMatrixDescription(activityMatrix);
            $get("warning").innerText = getMatrixActivityMatrixWarnings(activityMatrix);
        }

        function getActivityMatrixDescription(activityMatrix) {
            var strB = new Sys.StringBuilder();

            if (activityMatrix) {
                strB.append(String.format("活动矩阵总共{0}行{1}列。", activityMatrix.rows.length, activityMatrix.definitions.length));

                if (activityMatrix.definitions.length > 0) {
                    strB.append("列包括: ");

                    for (var i = 0; i < activityMatrix.definitions.length; i++) {
                        if (i > 0)
                            strB.append(", ");

                        strB.append(activityMatrix.definitions[i].name);
                    }
                }
            }

            return strB.toString();
        }

        function getMatrixActivityMatrixWarnings(activityMatrix) {
            var strB = new Sys.StringBuilder();

            if (activityMatrix) {
                checkDefinitions(activityMatrix.definitions, strB);
            }

            return strB.toString();
        }

        function checkDefinitions(definitions, strB) {
            if (definitions.length == 0)
                strB.append("没有列定义");
            else {
                if (findColumnByName(definitions, "OperatorType") == null)
                    addWarning(strB, "没有定义列OperatorType");

                if (findColumnByName(definitions, "Operator") == null)
                    addWarning(strB, "没有定义列Operator");
            }
        }

        function addWarning(strB, text) {
            if (strB.isEmpty() == false)
                strB.append("； ");

            strB.append(text);
        }

        function findColumnByName(definitions, name) {
            var column = null;

            for (var i = 0; i < definitions.length; i++) {
                if (definitions[i].name.toLowerCase() == name.toLowerCase()) {
                    column = definitions[i];
                    break;
                }
            }

            return column;
        }

        function onDeleteActivityMatrix() {
            if ($HGClientMsg.confirm("确定删除活动矩阵？")) {
                _resourceCategory[_currentResourceType].gridResources = [];
                var index = findFirstItemIndexInResourcesByType(_currentResourceType);

                if (index != -1)
                    Array.removeAt(_resources, index);

                setActivityMatrixStatus(null);

                bindTree(RESOURCE_TYPE_ARRAY);
            }
        }

        function onDownloadActivityMatrix() {
            var activityMatrix = findFirstItemInResourcesByType("WfActivityMatrixResourceDescriptor");

            if (activityMatrix != null) {
                $get("matrixData").value = Sys.Serialization.JavaScriptSerializer.serialize(activityMatrix);
                $get("downloadActivityMatrixForm").submit();
            }
        }
    </script>
    <form id="serverForm" runat="server">
        <table style="height: 100%; width: 100%">
            <tr>
                <td class="gridHead">
                    <div class="dialogTitle">
                        <span class="dialogLogo">流程模板资源编辑</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: middle">
                    <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%; height: 100%; overflow: auto">

                        <table style="height: 100%; width: 100%">
                            <tr>
                                <td style="width: 20%">
                                    <MCS:DeluxeTree ID="tree" runat="server" Width="100%" Height="100%" BorderStyle="Solid"
                                        BorderWidth="0px" OnNodeSelecting="onTreeNodeSelecting">
                                    </MCS:DeluxeTree>
                                </td>
                                <td style="vertical-align: top">
                                    <div id="listEditor">
                                        <table style="width: 100%; height: 100%; vertical-align: top;">
                                            <tr>
                                                <td style="height: 30px; background-color: #C0C0C0">
                                                    <a href="#" onclick="addSelectedResource();">
                                                        <img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" />
                                                    </a><a href="#" onclick="deleteResource();">
                                                        <img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除" border="0" />
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="width: 100%; height: 100%; vertical-align: top;">
                                                    <table id="grid" style="text-align: center">
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="activityMatrixEditor" style="display: none">
                                        <div style="text-align: left">
                                            <p>
                                                选择上传可以改变活动矩阵。矩阵的格式可以用Open Xml格式的Excel来表示，除了操作人类型（OperatorType）和操作人（Operator）列之外，都是对应着流程上下文中或全局参数。
                                            </p>
                                            <p>
                                                在Excel中，矩阵的行标题必须从A3开始，第一行为列定义。列的文字描述并不重要，关键是列的名称（在Excel中名称管理器中定义的）。
                                            点击<a href="DownloadActivityMatrixExcel.aspx" style="color: blue" target="_blank">这里</a>可以下载一个示例文件。
                                            </p>
                                        </div>
                                        <MCS:UploadProgressControl runat="server" ID="uploadActivityMatrix" DialogTitle="上传活动矩阵"
                                            OnDoUploadProgress="UploadActivityMatrixProgress" OnClientCompleted="onUploadActivityMatrixCompleted" />
                                        <input type="button" id="importBtn" class="formButton" value="上传..." runat="server"
                                            onclick="onUploadActivityMatrix();" />
                                        <input type="button" id="downloadActivityMatrix" class="formButton" value="下载..." runat="server"
                                            onclick="onDownloadActivityMatrix();" />
                                        <input type="button" id="deleteActivityMatrix" class="formButton" value="删除..." runat="server"
                                            onclick="onDeleteActivityMatrix();" />
                                        <div style="text-align: left">
                                            <p id="activityMatrixInfo">
                                            </p>
                                            <p id="warning" style="color: brown"></p>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="gridfileBottom"></td>
            </tr>
            <tr>
                <td style="height: 40px; text-align: center; vertical-align: middle">
                    <table style="width: 100%; height: 100%">
                        <tr>
                            <td style="text-align: center;">
                                <input type="button" class="formButton" onclick="onBtnOKClick();" value="确定(O)" id="btnOK"
                                    accesskey="O" />
                            </td>
                            <td style="text-align: center;">
                                <input type="button" class="formButton" onclick="top.close();" value="取消(C)" id="btnCancel"
                                    accesskey="C" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <input runat="server" id="hiddenData" type="hidden" />
        <input runat="server" id="hiddenResourceResult" type="hidden" />
        <input runat="server" id="hiddenUserResTemplate" type="hidden" />
        <input runat="server" id="hiddenDepartResTemplate" type="hidden" />
        <input runat="server" id="hiddenRoleResTemplate" type="hidden" />
        <input runat="server" id="hiddenGroupResTemplate" type="hidden" />
        <input runat="server" id="hiddenActOperatorResTemplate" type="hidden" />
        <input runat="server" id="hiddenActAssigneeResTemplate" type="hidden" />
        <input runat="server" id="hiddenResTypesData" type="hidden" />
        <input runat="server" id="hiddenDynamicResTypesDataTemplate" type="hidden" />
        <input runat="server" id="hiddenActiveMatrixResTemplate" type="hidden" />
        <input runat="server" id="hiddenAURoleResTemplate" type="hidden" />
        <script type="text/javascript">
            function createClientGrid() {
                //创建列表对象          

                $create($HBRootNS.ClientGrid,
                {
                    pageSize: 10,
                    cellPadding: "3px",

                    emptyDataText: "没有记录！",
                    columns: [{ selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px" } },
                        { dataField: "logOnName", headerText: "登录名", sortExpression: "logOnName", headerStyle: { width: "100px" } },
                        { dataField: "displayName", headerText: "人员名称", itemStyle: { width: "50px" }, headerStyle: { width: "100px" }, cellDataBound: function (grid, e) { } },
                        { dataField: "fullPath", headerText: "人员路径", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }],

                    allowPaging: true,
                    autoPaging: true,
                    pagerSetting:
                    {
                        mode: $HBRootNS.PageSettingMode.text,
                        position: $HBRootNS.PagerPosition.bottom,
                        firstPageText: "<<",
                        prevPageText: "上页",
                        nextPageText: "下页",
                        lastPageText: ">>"
                    },
                    style: { width: "100%" }
                },
                {
                    pageIndexChanged: onPageIndexChanged,
                    sorted: onSorted,
                    cellDataBound: onCellDataBound
                },
                null,
                $get("grid"));
            }

            //设置列表属性，绑定数据
            function dataBind() {
                var grid = $find("grid");
                grid.set_caption("列表1");

                grid.set_rowCount(201);
                grid.set_showFooter(true);
                setGridControlDataSource(grid);
            }

            //相应翻页事件
            function onPageIndexChanged(gridControl, e) {
                if (!gridControl.get_autoPaging())
                    setGridControlDataSource(gridControl);
            }

            //相应表格数据绑定事件
            function onCellDataBound(gridControl, e) {
                var cell = e.cell;
                var dataField = e.column.dataField;
                var data = e.data;
                switch (dataField) {
                    case "price":
                        var price = data["price"];
                        if (price < 100)
                            cell.style.color = "red";
                        else
                            cell.style.color = "blue";
                        break;

                    case "deleteCommand":
                        e.gridControl = gridControl;
                        var delBtn = $HGDomElement.createElementFromTemplate(
                    {
                        nodeName: "input",
                        properties:
                        {
                            type: "button",
                            value: "删除"
                        },
                        events: { click: Function.createDelegate(e, onGridDeleteBtnClick) }
                    },
                    cell
                );
                        break;
                }
            }

            function onSorted(gridControl, e) {
                //实现本页排序
                //           gridControl.get_dataSource().sort(Function.createDelegate(gridControl, dataSourceSort));
                //           gridControl.dataBind();
            }

            function dataSourceSort(data1, data2) {
                var pName = this.get_sortExpression();
                var sortDirection = this.get_sortDirection();

                v1 = data1[pName];
                v2 = data2[pName];

                if (v1 == v2) return 0;
                if (sortDirection == $HBRootNS.SortDirection.asc)
                    result = v1 > v2 ? 1 : -1;
                else
                    result = v1 > v2 ? -1 : 1;

                return result;
            }

            function onGridDeleteBtnClick() {
                var gridControl = this.gridControl;

                var data = this.data;

                if ($HGClientMsg.confirm("是否删除本行!")) {
                    Array.remove(gridControl.get_dataSource(), data);
                    gridControl.dataBind();
                }
            }

            function setGridControlDataSource(gridControl, dataSource) {
                if (gridControl.get_autoPaging()) {
                    var pageIndex = 0;
                    var pageSize = 10;
                } else {
                    var pageIndex = gridControl.get_pageIndex();
                    var pageSize = gridControl.get_pageSize();
                }

                gridControl.set_dataSource(dataSource);
            }
        </script>
    </form>
    <div style="display: none">
        <form id="downloadActivityMatrixForm" action="DownloadActivityMatrixExcel.aspx" target="_blank" method="post">
            <input type="hidden" id="matrixData" name="matrixData" />
        </form>
    </div>
    <script type="text/javascript">
        Sys.Application.add_load(function () {
            onDocumentLoad();
        });
        //]]>
    </script>
</body>
</html>
