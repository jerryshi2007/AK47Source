// -------------------------------------------------
// FileName	：	DialogUploadFileTraditionalControl.js
// Remark	：	上传文件
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070807		创建
// -------------------------------------------------

$HBRootNS.DialogUploadFileTraditionalControl = function (element) {
	$HBRootNS.DialogUploadFileTraditionalControl.initializeBase(this, [element]);

	this._okButton = null; 				//确定按钮
	this._cancelButton = null; 			//取消按钮
	this._tableMain = null; 				//主TABLE
	this._fileList = null; 				//选择的文件列表
	this._materialTable = null; 			//显示material的table
	this._checkBoxSelectAll = null; 		//全选的checkbox
	this._selectFileButton = null; 		//选择文件按钮
	this._removeFileButton = null; 		//移除文件按钮
	this._uploadFileImageButton = null; 		//上传文件按钮
	this._deleteFileButton = null; 		//删除文件按钮
	this._divContainer = null; 			//包含_tableMaterials的div
	this._materials = new Array(); 		//material集合
	this._deltaMaterials = null; 		//deltaMaterials集合
	this._checkBoxCellWith = "22"; 	//显示checkbox的单元格宽度
	this._fileIconCellWith = "22"; 	//显示文件图标的单元格宽度
	this._titleCellWith = "130"; 		//显示title的单元格宽度
	this._originalNameCellWith = "130"; //显示原始名称的单元格宽度
	this._pageQuantityCellWith = "30"; //显示页数的单元格宽度
	this._sortIDCellWidth = "30"; 		//显示排序号的单元格宽度
	this._titleInputWidth = "118"; 	//标题输入框的宽度
	this._otherInputWidth = "25"; 		//其他输入框的宽度
	this._container = null; 				//所在控件的对象
	this._uploadFileImage = null; 		//文件夹图片路径
	this._circleImagePath = null;
	this._userID = "";
	this._deletedIDs = new Array(); 		//删除的ID组成的数组
	this._fileExts = "";                     //可以上传的文件的拓展名
	this._fileMaxSize = 0;                   //上传文件的大小限制 单位是字节
	this._fileCountLimited = 0;
	this._fileSelectMode = null;
	this._fileInput = null;
	this._circleImage = null;
	this._hidFrame = null;
	this._uploadingFile = null;
	this._controlID = null;

	this._okButtonEvents =
		{
			click: Function.createDelegate(this, this._onOkButtonClick)
		};
	this._cancelButtonEvents =
		{
			click: Function.createDelegate(this, this._onCancelButtonClick)
		};
	this._uploadFileImageButtonEvents =
		{
			click: Function.createDelegate(this, this._onUploadFileButtonClick)
		};
	this._deleteFileButtonEvents =
		{
			click: Function.createDelegate(this, this._onDeleteFileButtonClick)
		};
	this._selectAllEvents =
		{
			click: Function.createDelegate(this, this._onSelectAllClick)
		};
	this._selectOneEvents =
		{
			click: Function.createDelegate(this, this._onSelectOneClick)
		};
	this._scrollEvents =
		{
			scroll: Function.createDelegate(this, this._onScrollDivContainer)
		};
	this._clickTableCellEvents =
		{
			click: Function.createDelegate(this, this._onClickTableCell)
		};
	this._inputEvents =
		{
			blur: Function.createDelegate(this, this._onSaveInput),
			keydown: Function.createDelegate(this, this._onSaveInput)
		};
	this._onOpenFileEvents =
		{
			click: Function.createDelegate(this, this._checkDownloadFileInline)
		};
}

$HBRootNS.DialogUploadFileTraditionalControl.prototype =
{
	initialize: function () {
		$HBRootNS.DialogUploadFileTraditionalControl.callBaseMethod(this, 'initialize');

		var dialogArg = window.dialogArguments;

		this._container = dialogArg.container;
		this._materials = Sys.Serialization.JavaScriptSerializer.deserialize(dialogArg.materials);
		this._deltaMaterials = Sys.Serialization.JavaScriptSerializer.deserialize(dialogArg.deltaMaterials);
		this._userID = dialogArg.userID;
		this._fileExts = dialogArg.fileExts;
		this._fileMaxSize = dialogArg.fileMaxSize;
		this._fileCountLimited = dialogArg.fileCountLimited;
		this._fileSelectMode = this._container.get_fileSelectMode();

		$HBRootNS.material.openInlineFileExt = dialogArg.openInlineFileExts;
		this._buildControl();

		$HBRootNS.material.initDownloadFileFrame();
	},

	dispose: function () {
		$HGDomEvent.removeHandlers(this._okButton, this._okButtonEvents);
		$HGDomEvent.removeHandlers(this._cancelButton, this._cancelButtonEvents);
		$HGDomEvent.removeHandlers(this._uploadFileImageButton, this._uploadFileImageButtonEvents);
		$HGDomEvent.removeHandlers(this._deleteFileButton, this._deleteFileButtonEvents);
		$HGDomEvent.removeHandlers(this._checkboxSelectAll, this._selectAllEvents);
		$HGDomEvent.removeHandlers(this._divContainer, this._scrollEvents);

		this._okButton = null;
		this._cancelButton = null;
		this._tableMain = null;
		this._selectFileButton = null;
		this._removeFileButton = null;
		this._uploadFileImageButton = null;
		this._deleteFileButton = null;
		this._checkBoxSelectAll = null;
		this._divContainer = null;
		this._fileList = null;
		this._materials = null;
		this._deltaMaterials = null;
		this._materialTable = null;
		this._checkBoxSelectAll = null;
		this._actXSelectFile = null;
		this._container = null;
		this._uploadingFile = null;
		$HBRootNS.DialogUploadFileTraditionalControl.callBaseMethod(this, 'dispose');
	},

	_buildControl: function () {
		window.document.body.className = "Dialog";

		var element = this.get_element();

		this._tableMain = $HGDomElement.createElementFromTemplate(
		{
			nodeName: "table",
			properties:
			{
				style:
				{
					width: "100%",
					height: "100%"
				}
			}
		},
		element);

		this._showMainTitle();
		this._showSelectFiles();
		this._showUploadedFiles();
		this._showLine();
		this._showMainButtons();

		this._setControlDisabled();
	},

	_setControlDisabled: function () {
		if (this._fileCountLimited != 0 && this._fileCountLimited <= this._materials.length) {
			this._fileInput.disabled = true;
			this._uploadFileImageButton.disabled = true;
		}
		else {
			this._fileInput.disabled = false;
			this._uploadFileImageButton.disabled = false;
		}
	},

	_checkDownloadFileInline: function (obj) {
		var fileName = obj.handlingElement.innerText;

		$HBRootNS.material.checkDownloadFileInline(fileName);
	},

	_showMainTitle: function () {
		var tableRow = this._buildTableRow(this._tableMain);

		var tableCell = this._buildTableCell(tableRow, 2, "", "gridHead");

		$HGDomElement.createElementFromTemplate(
			{
				nodeName: "img",
				properties:
				{
					src: this._uploadFileImage,
					align: "absmiddle"
				}
			},
			tableCell);

		$HGDomElement.createTextNode(" " + $NT.getText("SOAWebControls", "上传文件"), tableCell, $HGDomElement.get_currentDocument());
	},

	_showLine: function () {
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 2, "", "gridfileBottom");
	},

	_showMainButtons: function () {
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 2);
		tableCell.align = "center";
		tableCell.style.height = "45px";

		this._showOKButton(tableCell);

		$HGDomElement.createTextNode("  ", tableCell, $HGDomElement.get_currentDocument());

		this._showCancelButton(tableCell);
	},

	_showOKButton: function (tableCell) {
		this._okButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "button",
					value: $NT.getText("SOAWebControls", "确定(O)"),
					accessKey: "O",
					style:
					{
						width: "80px"
					}
				},
				cssClasses: ["formButton"],
				events: this._okButtonEvents
			},
			tableCell
			);
	},

	_showCancelButton: function (tableCell) {
		this._cancelButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "button",
					value: $NT.getText("SOAWebControls", "取消(C)"),
					accessKey: "C",
					style:
					{
						width: "80px"
					}
				},
				cssClasses: ["formButton"],
				events: this._cancelButtonEvents
			},
			tableCell
			);
	},

	_showSelectFiles: function () {
		var tableRow, tableCell;

		tableRow = this._buildTableRow(this._tableMain);
		tableCell = this._buildTableCell(tableRow, 2);
		tableCell.innerHTML = "<b>" + $NT.getText("SOAWebControls", "选择文件并上传") + "</b>";

		tableRow = this._buildTableRow(this._tableMain);
		tableCell = this._buildTableCell(tableRow, 1);

		this._hidFrame = window.document.createElement("<iframe name='hidden_frame' id='hidden_frame'/>");
		this._hidFrame.style.display = "none";
		document.body.appendChild(this._hidFrame);

		this._fileInput = $HGDomElement.createElementFromTemplate(
        {
        	nodeName: "input",
        	properties:
        				{
        					name: "fileInput",
        					type: "file",
        					style: { width: "300px", textAlign: "left", letterSpacing: "0px" }

        				},
        	cssClasses: ["formButton"]
        },
        tableCell
        );

		this._circleImage = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "img",
				properties:
				{
					src: this._circleImagePath,
					style: { display: "none" }
				}
			},
			tableCell
			);

		this._circleImage.style.height = "25px";
		this._circleImage.style.width = "25px";

		this._showSelectFilesButtons(tableRow);
	},

	_showSelectFilesButtons: function (tableRow) {
		var tableRow, tableCell, tableButton;

		tableCell = this._buildTableCell(tableRow);
		tableButton = $HGDomElement.createElementFromTemplate({ nodeName: "table" }, tableCell);

		//        this._showSelectFileButton(tableButton);
		//        this._showRemoveFileButton(tableButton);
		this._showUploadFileButton(tableButton);
		this._showFileSizeHintContainer(tableButton);
	},

	_showUploadFileButton: function (tableButton) {
		var tableRow = this._buildTableRow(tableButton);
		var tableCell = this._buildTableCell(tableRow);

		this._uploadFileImageButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "button",
					value: $NT.getText("SOAWebControls", "上传文件(U)"),
					accessKey: "U",
					style:
					{
						fontSize: "10px",
						width: "120px"
					}
				},
				cssClasses: ["formButton"],
				events: this._uploadFileImageButtonEvents
			},
			tableCell
			);
	},

	_showUploadedFiles: function () {
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 2);
		tableCell.innerHTML = "<b>" + $NT.getText("SOAWebControls", "已上传文件列表") + "</b>";

		var tableRow = this._buildTableRow(this._tableMain);

		this._showMaterialTableContainer(tableRow);
		this._showMaterials();
		this._showDeleteFileButton(tableRow);
	},

	_showMaterialTableContainer: function (tableRow) {
		var tableCell = this._buildTableCell(tableRow);

		this._divContainer = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "div",
				ID: "divContainer",
				properties:
				{
					style:
					{
						overflowY: "auto",
						height: "230px",
						padding: "0px"
					}
				},
				events: this._scrollEvents
			},
			tableCell
			);
	},

	_showFileSizeHintContainer: function (tableButton) {
		if (this._fileMaxSize > 0) {
			var tableRow = this._buildTableRow(tableButton);
			var tableCell = this._buildTableCell(tableRow);

			var div = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "div",
				ID: "divContainer",
				properties:
				{
					style:
					{
						overflowY: "auto",
						width: "120px",
						padding: "4px"
					}
				}
			},
			tableCell
			);

			div.innerText = String.format($NT.getText("SOAWebControls", "每个上传文件的尺寸必须小于{0}字节"),
				$HGRootNS.Formatter.pictureFormat(this._fileMaxSize, "#,##0"));
		}
	},

	_onScrollDivContainer: function () {
		var tableCell;
		try {
			for (var i = 0; i < this._materialTable.firstChild.firstChild.childNodes.length; i++) {
				tableCell = this._materialTable.firstChild.firstChild.childNodes[i];
				tableCell.style.position = "relative";
				tableCell.style.left = this._divContainer.scrollLeft;
				tableCell.style.top = this._divContainer.scrollTop - 1;
			}
		}
		catch (e)
		{ }
	},

	_showMaterials: function () {
		if (this._materialTable == null) {
			this._showMaterialTable();
			this._showMaterialsTableHead();
		}
		else {
			this._clearMaterialsTableContent();
		}

		if (this._materials)
			this._showMaterialsTableContent();
	},

	_showMaterialTable: function () {
		this._materialTable = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "table",
				cssClasses: ["dataList"]
			},
			this._divContainer
			);

		$HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, this._materialTable);
	},

	_clearMaterialsTableContent: function () {
		while (this._materialTable.rows.length > 1)
			this._materialTable.deleteRow(1);
	},

	_showMaterialsTableHead: function () {
		var tableRow = this._buildTableRow(this._materialTable);
		var tableCell = this._buildTableMaterialsCell(tableRow, "head", "", this._checkBoxCellWith);

		this._checkboxSelectAll = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "checkbox"
				},
				events: this._selectAllEvents
			},
			tableCell
			);

		tableCell.style.borderLeftStyle = "none";
		tableCell.style.borderRightStyle = "none";
		tableCell = this._buildTableMaterialsCell(tableRow, "head", " ", this._fileIconCellWith);
		tableCell.style.borderRightStyle = "none";

		tableCell = this._buildTableMaterialsCell(tableRow, "head", $NT.getText("SOAWebControls", "文件名"), this._originalNameCellWith);
		tableCell.style.borderLeftStyle = "none";

		tableCell = this._buildTableMaterialsCell(tableRow, "head", $NT.getText("SOAWebControls", "备注"), this._titleCellWith);
		tableCell.style.borderLeftStyle = "none";

		/*不要页数
		tableCell = this._buildTableMaterialsCell(tableRow, "head", "页数", this._pageQuantityCellWith);
		tableCell.style.borderLeftStyle = "none";
		*/
		tableCell = this._buildTableMaterialsCell(tableRow, "head", $NT.getText("SOAWebControls", "序号"), this._sortIDCellWidth);
		tableCell.style.borderLeftStyle = "none";
		tableCell.style.borderRightStyle = "none";
	},

	_showMaterialsTableContent: function () {
		var tableRow, tableCell, checkbox;

		this._materials = this._materials.sort(this._sortMaterial);

		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i] == null || this._materials[i].id == $HBRootNS.material.newMaterialID)
				continue;

			tableRow = this._buildTableRow(this._materialTable);

			this._showMaterialSelectCell(tableRow, i);
			this._showMaterialFileIconCell(tableRow, i);
			this._showMaterialOriginalNameCell(tableRow, i);
			this._showMaterialTitleCell(tableRow, i);
			//this._showMaterialPageQuantityCell(tableRow, i);	//不要页数
			this._showMaterialSortIDCell(tableRow, i);
		}
	},

	_showMaterialSelectCell: function (tableRow, index) {
		var tableCell = this._buildTableMaterialsCell(tableRow, "item", "", this._checkBoxCellWith);
		var material = this._materials[index];

		var currentActivityID = this._container.get_wfActivityID();

		var canSelect = this._canDoAction(material, $HBRootNS.materialActivityControlMode.AllowDelete);

		if (canSelect) {
			$HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "checkbox",
					index: index,
					checked: material.selected
				},
				events: this._selectOneEvents
			},
			tableCell
			);
		}
	},

	_showMaterialFileIconCell: function (tableRow, index) {
		var tableCell = this._buildTableMaterialsCell(tableRow, "item", "", this._fileIconCellWith);
		tableCell.style.borderRightStyle = "none";
		$HGDomElement.createElementFromTemplate(
			{
				nodeName: "img",
				properties:
				{
					src: this._materials[index].fileIconPath,
					align: "absmiddle"
				}
			},
			tableCell
			);
	},

	_showMaterialTitleCell: function (tableRow, index) {
		var tableCell = this._buildTableMaterialsCell(tableRow, "item",
			this._materials[index].title, this._titleCellWith, this._clickTableCellEvents, index, "title");

		tableCell.style.textAlign = "left";

		tableCell.style.borderLeftStyle = "none";
	},

	_getShowFileUrl: function (index) {
		var material = this._materials[index];
		var filePath = "";
		var processPageUrl = "";

		if (material.showFileUrl != "") {
			if (material.showFileUrl.indexOf("Temp\\") == 0)
				filePath = material.showFileUrl;
			else
				processPageUrl = material.showFileUrl;
		}
		else {
			filePath = material.relativeFilePath;
		}

		if (processPageUrl == "") {
			var fileName = $HBRootNS.fileIO.getFileNameWithExt(material.originalName.replace("/", "\\"));
			processPageUrl = String.format("{0}?requestType=download&rootPathName={1}&controlID={2}&fileName={3}&pathType={4}&filePath={5}&userID={6}&materialID={7}&fileReadonly=true",
				this._container.get_currentPageUrl(),
				this._container.get_rootPathName(),
				this._container.get_uniqueID(),
				escape(fileName),
				$HBRootNS.pathType.relative,
				filePath,
				this._userID,
				material.id);
		}

		return processPageUrl;
	},

	_showMaterialOriginalNameCell: function (tableRow, index) {
		var tableCell = this._buildTableMaterialsCell(tableRow, "item", "", this._originalNameCellWith);

		$HGDomElement.createElementFromTemplate(
			{
				nodeName: "a",
				properties:
				{
					innerText: this._materials[index].originalName,
					href: this._getShowFileUrl(index),
					target: "_blank",
					style:
					{
						cursor: "pointer"
					}
				},
				events: this._onOpenFileEvents
			},
			tableCell);
	},

	_showMaterialPageQuantityCell: function (tableRow, index) {
		this._buildTableMaterialsCell(tableRow, "item",
			this._materials[index].pageQuantity, this._pageQuantityCellWith, this._clickTableCellEvents, index, "pageQuantity");
	},

	_showMaterialSortIDCell: function (tableRow, index) {
		this._buildTableMaterialsCell(tableRow, "item",
			this._materials[index].sortID, this._sortIDCellWidth, this._clickTableCellEvents, index, "sortID");
	},

	_showDeleteFileButton: function (tableRow) {
		var tableCell = this._buildTableCell(tableRow);
		this._deleteFileButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "button",
					value: $NT.getText("SOAWebControls", "删除文件(D)"),
					accessKey: "D",
					style:
					{
						fontSize: "10px",
						width: "120px"
					}
				},
				cssClasses: ["formButton"],
				events: this._deleteFileButtonEvents
			},
			tableCell
			);
	},

	_sortMaterial: function (material1, material2) {
		return (material1.sortID - material2.sortID);
	},

	_canDoAction: function (material, actionMode) {
		var result = true;
		var currentActivityID = this._container.get_wfActivityID();

		if (currentActivityID != null && currentActivityID != "") {
			var acm = this._container.get_activityControlMode();

			result = (acm & actionMode) != $HBRootNS.materialActivityControlMode.None
							|| currentActivityID == material.wfActivityID;
		}

		return result;
	},

	_buildTableMaterialsCell: function (row, className, text, cellWidth, clickEvent, materialIndex, propertyName) {
		var tableCell;
		var material = this._materials[materialIndex];

		if (clickEvent == null) {
			tableCell = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "td",
					properties:
					{
						width: cellWidth,
						innerText: text,
						style:
						{
							wordWrap: "break-word",
							padding: "0px",
							textAlign: "left"
						}
					},
					cssClasses: [className]
				},
				row);
		}
		else {
			tableCell = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "td",
					properties:
					{
						width: cellWidth,
						innerText: text,
						title: this._canDoAction(material, $HBRootNS.materialActivityControlMode.AllowEdit) ? $NT.getText("SOAWebControls", "单击此处编辑") : "",
						index: materialIndex,
						materialProperty: propertyName,
						style:
						{
							wordWrap: "break-word",
							padding: "0px"
						}
					},
					cssClasses: [className],
					events: clickEvent
				},
				row);
		}

		return tableCell;
	},

	_onClickTableCell: function (obj) {
		var tableCell = obj.target;

		if (tableCell.tagName == "INPUT")
			return;

		var material = this._materials[tableCell.index];

		if (this._canDoAction(material, $HBRootNS.materialActivityControlMode.AllowEdit) == false)
			return;

		var textMaxLength = "128";
		var inputTextWidth = this._titleInputWidth;

		if (tableCell.width != this._titleCellWith) {
			textMaxLength = "4";
			inputTextWidth = this._otherInputWidth;
		}

		var text = (tableCell.innerText == 0 ? "" : tableCell.innerText);
		tableCell.innerText = "";

		var input = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "text",
					title: $NT.getText("SOAWebControls", "回车保存，ESC取消"),
					maxLength: textMaxLength,
					value: text,
					style:
					{
						width: inputTextWidth
					}
				},
				events: this._inputEvents
			},
			tableCell
			);

		this._onScrollDivContainer();

		input.focus();
		input.select();
	},

	_onSaveInput: function (obj) {
		var keyCode = event.keyCode;

		//正常输入时，不做处理
		if (keyCode != 13 && keyCode != 27 && obj.type != "blur")
			return;

		//回车
		if (keyCode == 13)
			event.returnValue = false;

		var input = obj.target;
		var tableCell = input.parentNode;

		if (tableCell == null)
			return;

		var text = input.value;
		var index = tableCell.getAttribute("index");
		var materialProperty = tableCell.getAttribute("materialProperty");

		if (index == null)
			return;

		var attributeEdited = false;
		var sortEdited = false;

		if (text.trim() == "" || keyCode == 27) {
			this._resetInputContent(materialProperty, index, tableCell);
		}
		else {
			var result = this._saveInputContent(materialProperty, index, text, tableCell);
			attributeEdited = result.attributeEdited;
			sortEdited = result.sortEdited;

			if (attributeEdited) {
				var indexInInserted = this._findInInsertedMaterials(this._materials[index].id);
				var indexInUpdated = this._findInUpdatedMaterials(this._materials[index].id);

				if (indexInInserted != -1)
					this._deltaMaterials.insertedMaterials[indexInInserted][materialProperty] = this._materials[index][materialProperty];
				else if (indexInUpdated != -1)
					this._deltaMaterials.updatedMaterials[indexInUpdated][materialProperty] = this._materials[index][materialProperty];
				else
					Array.add(this._deltaMaterials.updatedMaterials, this._materials[index]);
			}

			if (sortEdited)
				this._showMaterials();
		}

		$HGDomEvent.removeHandlers(input, this._inputEvents);
		input = null;

		this._onScrollDivContainer();
	},

	_resetInputContent: function (materialProperty, index, tableCell) {
		switch (materialProperty) {
			case "title":
				tableCell.innerText = this._materials[index].title;
				break;
			case "pageQuantity":
				tableCell.innerText = this._materials[index].pageQuantity;
				break;
			case "sortID":
				tableCell.innerText = this._materials[index].sortID;
				break;
		}
	},

	_saveInputContent: function (materialProperty, index, text, tableCell) {
		var result = new Object();
		result.attributeEdited = false;
		result.sortEdited = false;

		switch (materialProperty) {
			case "title":
				if (this._materials[index].title != text) {
					this._materials[index].title = text;
					result.attributeEdited = true;
				}
				tableCell.innerText = this._materials[index].title;
				break;
			case "pageQuantity":
				if (this._materials[index].pageQuantity != text && this._checkIsInt(text)) {
					this._materials[index].pageQuantity = text;
					result.attributeEdited = true;
				}
				tableCell.innerText = this._materials[index].pageQuantity;
				break;
			case "sortID":
				if (this._materials[index].sortID != text && this._checkIsInt(text)) {
					this._materials[index].sortID = text;
					result.attributeEdited = true;
					result.sortEdited = true;
				}

				tableCell.innerText = this._materials[index].sortID;
				break;
		}

		return result;
	},

	_checkIsInt: function (text) {
		return (!isNaN(text) && text.indexOf('.') == -1 && text.indexOf('-') == -1)
	},

	_onSelectOneClick: function (obj) {
		var isSelectAll = true;

		var checkbox = obj.target;
		var index = checkbox.getAttribute("index");
		this._materials[index].selected = checkbox.checked;

		var tableMaterialsContent = this._materialTable.firstChild;
		for (var i = 1; i < tableMaterialsContent.childNodes.length; i++) {
			if (tableMaterialsContent.childNodes[i].firstChild.firstChild) {
				if (tableMaterialsContent.childNodes[i].firstChild.firstChild.checked == false)
					isSelectAll = false;
			}
		}

		if (isSelectAll)
			this._checkboxSelectAll.checked = true;
		else
			this._checkboxSelectAll.checked = false;
	},

	_onSelectAllClick: function () {
		for (var i = 1; i < this._materialTable.firstChild.childNodes.length; i++) {
			if (this._materialTable.firstChild.childNodes[i].firstChild.firstChild) {
				this._materialTable.firstChild.childNodes[i].firstChild.firstChild.checked = this._checkboxSelectAll.checked;
				this._materials[i - 1].selected = this._checkboxSelectAll.checked;
			}
		}
	},

	_onRemoveFileButtonClick: function () {
		for (var i = this._fileList.length - 1; i >= 0; i--) {
			if (this._fileList.options[i].selected)
				this._fileList.remove(i);
		}
	},

	_onUploadFileButtonClick: function () {
		if (this._fileInput.value == "") {
			alert($NT.getText("SOAWebControls", "请选择要上传的文件！"));
			return;
		}

		var sampleMaterial = new Object();
		sampleMaterial.filePath = this._fileInput.value;
		sampleMaterial.container = this._container;

		this._uploadingFile = sampleMaterial;

		this._uploadOneFile();

		this._setControlDisabled();
	},

	_uploadOneFile: function () {
		this._uploadFileImageButton.disabled = true;
		this._circleImage.style.display = "inline";
		var currentFile = this._uploadingFile;
		this._container._setNewMaterialInfo(currentFile, 1);
		var localPath = currentFile.filePath;

		var processPageUrl = this._container.get_currentPageUrl()
		                    + "?requestType=upload"
                            + "&upmethod=new"
		                    + "&lockID=" + this._container.get_lockID()
		                    + "&userID=" + this._container.get_user().id
		                    + "&rootPathName=" + this._container.get_rootPathName()
		                    + "&fileMaxSize=" + this._fileMaxSize
		                    + "&controlID=" + this._container.get_uniqueID()
                            + "&dialogControlID=" + this.get_controlID();

		var extName = "";
		var nFileNameStart = localPath.lastIndexOf(".");
		if (nFileNameStart > -1) {
			extName = localPath.substring(nFileNameStart + 1);
		}
		processPageUrl += "&fileName=" + escape(currentFile.newMaterialID + "." + extName);

		var file = this._fileInput;
		var form = document.createElement('form');
		document.body.appendChild(form);
		form.encoding = "multipart/form-data";
		form.method = "post";
		form.action = processPageUrl;
		form.target = "hidden_frame";
		var pos = file.nextSibling; //记住file在旧表单中的的位置
		form.appendChild(file);
		form.submit();
		form.reset();
		pos.parentNode.insertBefore(file, pos);
		document.body.removeChild(form);
		file.value = "";
		var uploadedFiles = new Array();
		uploadedFiles.push(currentFile);
		return uploadedFiles;
	},

	onUploadFinish: function (type, msg) {
		this._circleImage.style.display = "none";
		this._uploadFileImageButton.disabled = false;
		if (type == 1) {
			this._circleImage.style.display = "none";
			this._addNewMateial(this._uploadingFile);
			this._showMaterials();
			this._setControlDisabled();
			this._uploadingFile = null;
		}
		else {
			alert("上传失败，" + msg);
		}
	},

	_removeUploadedFiles: function (filesUploaded) {
		//        for (var i = this._fileList.length - 1; i >= 0; i--) {
		//            if (this._checkUploaded(filesUploaded, this._fileList[i].value))
		//                this._fileList.remove(i);
		//        }
	},

	_checkUploaded: function (filesUploaded, filePath) {
		for (var i = 0; i < filesUploaded.length; i++) {
			if (filesUploaded[i].filePath == filePath)
				return true;
		}

		return false;
	},

	_addNewMateial: function (newFile) {
		var filePath = newFile.filePath;
		var tempFilePath = newFile.tempFilePath;
		var id = newFile.newMaterialID;

		var newMaterial = new Object();
		newMaterial.id = id;
		newMaterial.department = this._container.get_department();
		newMaterial.resourceID = this._container.get_defaultResourceID();
		newMaterial.sortID = this._getNextMaxSortID();
		newMaterial.materialClass = this._container.get_defaultClass();
		newMaterial.title = $HBRootNS.fileIO.getFileNameWithoutExt(filePath);
		newMaterial.pageQuantity = 0;
		newMaterial.originalName = $HBRootNS.fileIO.getFileNameWithExt(filePath);

		materialFileName = id + "." + $HBRootNS.fileIO.getFileExt(filePath);
		newMaterial.relativeFilePath = this._container.get_relativePath() + materialFileName;
		newMaterial.creator = this._container.get_user();

		if (this._container._allowEditContent == true) {
			newMaterial.localPath = $HBRootNS.fileIO.getTempDirName() + newMaterial.relativeFilePath;
			newMaterial._realLocalPath = filePath;
		}

		newMaterial.lastUploadTag = null;
		newMaterial.createDateTime = null;
		//newMaterial.modifyTime = $HBRootNS.fileIO.getFileLastModifiedTime(filePath);
		newMaterial.modifyTime = new Date();
		newMaterial.wfProcessID = this._container.get_wfProcessID();
		newMaterial.wfActivityID = this._container.get_wfActivityID();
		newMaterial.wfActivityName = this._container.get_wfActivityName();
		newMaterial.parentID = null;
		newMaterial.sourceMaterial = null;
		newMaterial.versionType = $HBRootNS.materialVersionType.Normal;
		newMaterial.extraData = {};
		newMaterial.lastUploadTag = newFile.lastUploadTag;
		newMaterial.fileIconPath = (newFile.fileIconPath == "" ? this._container.get_defaultFileIconPath() : newFile.fileIconPath);
		newMaterial.showFileUrl = "Temp\\" + materialFileName;

		//newMaterial.

		//        if ($HBRootNS.officeDocument.checkIsOfficeDocument(newMaterial.originalName)) {
		//            var newFilePath = tempFilePath.replace($HBRootNS.material.newMaterialID, id);
		//            this._setMaterialInfomation(newFilePath, newMaterial);
		//        }


		Array.add(this._materials, newMaterial);
		Array.add(this._deltaMaterials.insertedMaterials, newMaterial);
	},

	_getSelectedMarerials: function () {
		var selected = [];

		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i].selected == true)
				selected.push(this._materials[i]);
		}

		return selected;
	},

	_onDeleteFileButtonClick: function () {
		var confirmDelete = false;

		if (this._getSelectedMarerials().length > 0) {
			confirmDelete = window.confirm($NT.getText("SOAWebControls", "确认删除所选的文件吗？"));
		}
		else {
			alert($NT.getText("SOAWebControls", "请选择要删除的文件!"));
		}

		if (confirmDelete) {
			var findIndex = -1;

			for (var i = this._materials.length - 1; i >= 0; i--) {
				if (this._materials[i].selected == true) {
					findIndex = this._findInUpdatedMaterials(this._materials[i].id);
					if (findIndex != -1)
						Array.removeAt(this._deltaMaterials.updatedMaterials, findIndex);

					findIndex = this._findInInsertedMaterials(this._materials[i].id);
					if (findIndex != -1)
						Array.removeAt(this._deltaMaterials.insertedMaterials, findIndex);
					else
						Array.add(this._deltaMaterials.deletedMaterials, this._materials[i]);

					Array.add(this._deletedIDs, this._materials[i].id);

					Array.removeAt(this._materials, i);
				}
			}
			this._checkboxSelectAll.checked = false;
			this._showMaterials();
			this._onScrollDivContainer();

			this._setControlDisabled();
		}
	},

	_onOkButtonClick: function () {
		this._returnDataAndClose();
	},

	_returnDataAndClose: function () {
		var information = new Object();
		information.materials = this._materials;
		information.deltaMaterials = this._deltaMaterials;
		information.deletedIDs = this._deletedIDs;

		window.returnValue = Sys.Serialization.JavaScriptSerializer.serialize(information);
		window.close();
	},

	_onCancelButtonClick: function () {
		window.close();
	},

	_setMaterialInfomation: function (filePath, material) {
		var fileExt = $HBRootNS.fileIO.getFileExt(filePath);
		var recordFilePath = filePath.replace(fileExt, "txt");

		$HBRootNS.fileIO.saveFileContent(recordFilePath, escape(Sys.Serialization.JavaScriptSerializer.serialize(material)));
	},

	_findInUpdatedMaterials: function (id) {
		for (var i = 0; i < this._deltaMaterials.updatedMaterials.length; i++) {
			if (this._deltaMaterials.updatedMaterials[i].id == id)
				return i;
		}
		return -1;
	},

	_findInInsertedMaterials: function (id) {
		for (var i = 0; i < this._deltaMaterials.insertedMaterials.length; i++) {
			if (this._deltaMaterials.insertedMaterials[i].id == id)
				return i;
		}
		return -1;
	},

	_buildTableRow: function (table) {
		if (table.firstChild == null)
			$HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, table);

		return $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, table.firstChild);
	},

	_buildTableCell: function (tableRow, span, text, className) {
		if (span == null)
			span = 1;
		if (text == null)
			text = "";
		if (className == null)
			className = "";

		var tableCell = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "TD",
				properties:
				{
					colSpan: span,
					innerText: text
				},
				cssClasses: [className]
			},
			tableRow);

		return tableCell;
	},

	_getNextMaxSortID: function () {
		var sortID = 0;

		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i] != null && sortID < parseInt(this._materials[i].sortID))
				sortID = parseInt(this._materials[i].sortID);
		}

		if (sortID == 9999)
			return sortID;
		else
			return sortID + 1;
	},

	_buildMaterialSpan: function (element) {
		var span = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "span"
			},
			this.get_element()
			);

		return span;
	},

	_initDownloadFileFrame: function () {
		var frame = $get($HBRootNS.material.downloadFrameName);

		if (frame == null) {
			$HGDomElement.createElementFromTemplate(
				{
					nodeName: "iframe",
					properties:
					{
						name: $HBRootNS.material.downloadFrameName,
						id: $HBRootNS.material.downloadFrameName,
						style:
						{
							display: "none"
						}
					}
				},
				$HGDomElement.get_currentDocument());
		}
	},

	get_uploadFileImage: function () {
		return this._uploadFileImage;
	},
	set_uploadFileImage: function (value) {
		if (this._uploadFileImage != value) {
			this._uploadFileImage = value;
			this.raisePropertyChanged("uploadFileImage");
		}
	},

	get_circleImagePath: function () {
		return this._circleImagePath;
	},
	set_circleImagePath: function (value) {
		if (this._circleImagePath != value) {
			this._circleImagePath = value;
			this.raisePropertyChanged("circleImagePath");
		}
	},

	get_controlID: function () {
		return this._controlID;
	},

	set_controlID: function (value) {
		if (this._controlID != value) {
			this._controlID = value;
		}
	}
}

$HBRootNS.DialogUploadFileTraditionalControl.registerClass($HBRootNSName + ".DialogUploadFileTraditionalControl", $HGRootNS.ControlBase);

