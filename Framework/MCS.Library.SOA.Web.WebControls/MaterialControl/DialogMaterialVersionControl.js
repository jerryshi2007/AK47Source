// -------------------------------------------------
// FileName	：	DialogMaterialVersionControl.js
// Remark	：	文件版本
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070820		创建
// -------------------------------------------------
$HBRootNS.DialogMaterialVersionControl = function(element)
{
	$HBRootNS.DialogMaterialVersionControl.initializeBase(this, [element]);

	this._cancelButton = null;			//取消按钮
	this._tableMain = null;				//主TABLE
	this._treeControlID = null;			//树控件ID
//	this._processPageUrl = null;		//显示文件的页面
	this._rootPathName = null;			//文件根目录配置名称
	this._controlID = null;				//控件ID
	this._fileVersionPath = null;		//文件版本图标路径

	this._cancelButtonEvents =
		{
			click : Function.createDelegate(this, this._onCancelButtonClick)
		};
}

$HBRootNS.DialogMaterialVersionControl.prototype =
{
	initialize : function()
	{
		$HBRootNS.DialogMaterialVersionControl.callBaseMethod(this, 'initialize');

		var dialogArg = window.dialogArguments;

//		this._processPageUrl = dialogArg.processPageUrl;
		this._rootPathName = dialogArg.rootPathName;
		this._controlID = dialogArg.controlID;

		this._buildControl();
	},

	dispose : function()
	{
		$HGDomEvent.removeHandlers(this._cancelButton, this._cancelButtonEvents);

		this._cancelButton = null;
		this._tableMain = null;

		$HBRootNS.DialogMaterialVersionControl.callBaseMethod(this, 'dispose');
	},

	_buildControl : function()
	{
		var element = this.get_element();
		
		this._tableMain = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "table",
				properties :
				{
					style :
					{
						width : "100%",
						height : "100%"
					}
				}
			},
			element);

		this._showMainTitle();
		this._showTree();
		this._showLine();
		this._showCancelButton();
	},

	_showMainTitle : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		
		var tableCell = this._buildTableCell(tableRow, 2, "", "gridHead");
		
		$HGDomElement.createElementFromTemplate(
			{
				nodeName : "img",
				properties : 
				{
					src : this._fileVersionPath,
					align : "absmiddle"
				}
			},
			tableCell);
			
		$HGDomElement.createTextNode(" 文件版本", tableCell, $HGDomElement.get_currentDocument());
	},
	
	_showLine : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 2, "", "gridfileBottom");
	},
	 
	_showTree : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1);
		tableCell.style.verticalAlign = "top";

		var divContainer = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "div",
				properties :
				{
					style :
					{
						width : "438px",
						height : "100%",
						overflowX : "auto",
						overflowY : "auto",
						padding : "0px"
					}
				}
			},
			tableCell);
		
		var tree = $get(this._treeControlID);
		tree.parentNode.removeChild(tree);

		divContainer.appendChild(tree);
	},

	_buildTableRow : function(table)
	{
		if (table.firstChild == null)
			$HGDomElement.createElementFromTemplate({nodeName : "tbody"}, table);

		return $HGDomElement.createElementFromTemplate({nodeName : "tr"}, table.firstChild);
	},

	_buildTableCell : function(tableRow, span, text, className)
	{
		if (span == null)
			span = 1;
		if (text == null)
			text = "";
		if (className == null)
			className ="";

		var tableCell = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "TD",
				properties :
				{
					colSpan : span,
					innerText : text
				},
				cssClasses : [className]
			},
			tableRow);

		return tableCell;
	},

	_showCancelButton : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1);
		tableCell.align = "center";
		tableCell.style.height = "45px";

		this._cancelButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "input",
				properties :
				{
					type : "button",
					value : "关闭(C)",
					accessKey : "C",
					style :
					{
						width :"80px"
					}
				},
				cssClasses : ["formButton"],
				events : this._cancelButtonEvents
			},
			tableCell
			);
	},

	_showLine : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 2, "", "gridfileBottom");
	},

	_onCancelButtonClick : function()
	{
		window.close();
	},

	get_treeControlID : function()
	{
		return this._treeControlID;
	},
	set_treeControlID : function(value)
	{
		if (this._treeControlID != value)
		{
			this._treeControlID = value;
			this.raisePropertyChanged("treeControlID");
		}
	},
	
	get_fileVersionPath : function()
	{
		return this._fileVersionPath;
	},
	set_fileVersionPath : function(value)
	{
		if (this._fileVersionPath != value)
		{
			this._fileVersionPath = value;
			this.raisePropertyChanged("fileVersionPath");
		}
	}
}

$HBRootNS.DialogMaterialVersionControl.registerClass($HBRootNSName + ".DialogMaterialVersionControl", $HGRootNS.ControlBase);