// -------------------------------------------------
// FileName	：	DialogUploadFileProcessControl.js
// Remark	：	上传文件等待页面
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070929		创建
// -------------------------------------------------

$HBRootNS.DialogUploadFileProcessControl = function (element) {
    $HBRootNS.DialogUploadFileProcessControl.initializeBase(this, [element]);

    this._cancelButton = null; 			//取消按钮
    this._tableMain = null; 				//主TABLE
    this._circleImagePath = null; 		//等待图片
    this._filesToUpload = new Array(); 	//要上传的文件
    this._container = null; 				//主控件
    this._fileMaxSize = 0;
    this._requestContext = "";
    this._isNew = false;
}

$HBRootNS.DialogUploadFileProcessControl.prototype =
{

	initialize: function () {

		$HBRootNS.DialogUploadFileProcessControl.callBaseMethod(this, 'initialize');

		var information = window.dialogArguments;
		if (information) {
			this._filesToUpload = information.filesToUpload;
			this._fileMaxSize = information.fileMaxSize;
			this._requestContext = information.requestContext;
			this._isNew = information.isNew;

			if (information.container)
				this._container = information.container;
		}
		this._buildControl();
		this._uploadFiles();
	},

	dispose: function () {
		//		$HGDomEvent.removeHandlers(this._cancelButton, this._cancelButtonEvents);

		this._cancelButton = null;
		this._tableMain = null;

		$HBRootNS.DialogUploadFileProcessControl.callBaseMethod(this, 'dispose');
	},

	_buildControl: function () {
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

		this._showInformation();
		//		this._showMainButtons();
	},

	_uploadFiles: function () {
		if (this._container != null) {
			$HBRootNS.fileIO.uploadFiles(this._filesToUpload, this._fileMaxSize, this._container.get_uniqueID());
		}
		else
			$HBRootNS.fileIO.uploadFiles(this._filesToUpload, this._fileMaxSize, null);
	},

	_showInformation: function () {
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow);

		$HGDomElement.createElementFromTemplate(
			{
				nodeName: "img",
				properties:
				{
					align: "absmiddle",
					src: this._circleImagePath
				}
			},
			tableCell
			);

		tableCell = this._buildTableCell(tableRow);
		tableCell.innerHTML = "<b>" + $NT.getText("SOAWebControls", "正在上传文件......") + "<b/>";
	},

	_showMainButtons: function () {
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 2);
		tableCell.align = "center";
		tableCell.style.verticalAlign = "top";
		tableCell.style.height = "30px";

		this._showCancelButton(tableCell);
	},

	_showCancelButton: function (tableCell) {
		this._cancelButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "input",
				properties:
				{
					type: "button",
					value: $NT.getText("SOAWebControls", "取消(C)"),
					accessKey: "C"
				},
				cssClasses: ["formButton"],
				events: this._cancelButtonEvents
			},
			tableCell
			);
	},

	_onCancelButtonClick: function () {
		window.close();
	},

	_buildTableRow: function (table) {
		if (table.firstChild == null)
			$HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, table);

		return $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, table.firstChild);
	},

	_buildTableCell: function (tableRow, span, className) {
		if (className == null)
			className = "";

		if (span == null)
			span = 1

		var tableCell = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "TD",
				properties:
				{
					colSpan: span
				},
				cssClasses: [className]
			},
			tableRow);

		return tableCell;
	},

	get_circleImagePath: function () {
		return this._circleImagePath;
	},
	set_circleImagePath: function (value) {
		if (this._circleImagePath != value) {
			this._circleImagePath = value;
			this.raisePropertyChanged("circleImagePath");
		}
	}
}

$HBRootNS.DialogUploadFileProcessControl.registerClass($HBRootNSName + ".DialogUploadFileProcessControl", $HGRootNS.ControlBase);