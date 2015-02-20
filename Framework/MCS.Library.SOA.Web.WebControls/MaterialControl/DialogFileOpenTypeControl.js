// -------------------------------------------------
// FileName	：	DialogFileOpenTypeControl.js
// Remark	：	修改文件打开方式设置
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070928		创建
// -------------------------------------------------

$HBRootNS.DialogFileOpenTypeControl = function(element)
{
	$HBRootNS.DialogFileOpenTypeControl.initializeBase(this, [element]);

	this._okButton = null;					//确定按钮
	this._cancelButton = null;				//取消按钮
	this._setDefaultButton = null;			//设置为默认的按钮
	this._tableMain = null;					//主TABLE
	this._currentFileExtensionNames = null;	//当前扩展名
	this._inputFileExtensionNames = null;	//填写扩展名的input对象
	this._computerImagePath = null;			//计算机图片
	this._inlineDemoImagePath = null;		//demo图片
	this._userID = "";						//用户ID
	this._currentFileExtensionNames = "";	//默认
	this._openTypeImagePath = "";			//图标路径
	
	this._okButtonEvents =
		{
			click : Function.createDelegate(this, this._onOkButtonClick)
		};
	this._cancelButtonEvents =
		{
			click : Function.createDelegate(this, this._onCancelButtonClick)
		};
	this._setDefaultButtonEvents =
		{
			click : Function.createDelegate(this, this._onSetDefaultButtonClick)
		};
}

$HBRootNS.DialogFileOpenTypeControl.prototype =
{
	initialize : function()
	{
		$HBRootNS.DialogFileOpenTypeControl.callBaseMethod(this, 'initialize');

		this._userID = window.dialogArguments;
		
		this._buildControl();		
	},

	dispose : function()
	{
		$HGDomEvent.removeHandlers(this._okButton, this._okButtonEvents);
		$HGDomEvent.removeHandlers(this._cancelButton, this._cancelButtonEvents);
		$HGDomEvent.removeHandlers(this._setDefaultButton, this._setDefaultButtonEvents);

		this._okButton = null;
		this._cancelButton = null;
		this._setDefaultButton = null;
		this._tableMain = null;

		$HBRootNS.DialogFileOpenTypeControl.callBaseMethod(this, 'dispose');
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
		this._showInputFileExtensionNames();
		this._showMainContent();
		this._showLine();
		this._showMainButtons();
	},

	_showMainTitle : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1, "gridHead");

		$HGDomElement.createElementFromTemplate(
			{
				nodeName : "img",
				properties : 
				{
					src : this._openTypeImagePath,
					align : "absmiddle"
				}
			},
			tableCell);
			
		$HGDomElement.createTextNode(" 设置以内嵌方式打开的文档类型", tableCell, $HGDomElement.get_currentDocument());
	},

	_showLine : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1, "gridfileBottom");
	},

	_showInputFileExtensionNames : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1, "oa_ajax_control_popup_tableCell_noBorder");
		tableCell.innerHTML = "<b>文件后缀名：</b>";

		this._inputFileExtensionNames = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "input",
				properties :
				{
					type : "text",
					value : this._currentFileExtensionNames,
					maxLength : "100",
					style :
					{
						width : "350px"
					}
				}
			},
			tableCell
			);		
	},

	_showMainContent : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1, "oa_ajax_control_popup_tableCell_noBorder");

		tableCell.innerHTML = "<br><img src='" + this._inlineDemoImagePath
			+ "' align='right'/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;如右图所示，某些类型的文件我们习惯于内嵌在浏览器中打开，例如Word文档、Excel表格、PowerPoint幻灯片、Html、以及一些图片文件。"
			+ "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;不是所有的文档都是能够内嵌在浏览器打开的，例如rar文件。主要取决于客户端有没有安装合适的软件，例如tif图片，很多看图软件就不能内嵌在浏览器中打开tif文件。"
			+ "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;文件如果不能内嵌在浏览器中显示，通常在打开前会弹出提示框提示您确认，给用户带来一些不便。"
			+ "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;如果您需要设置哪些文件是内嵌打开的，请设置相应的文件后缀名，格式如下："
			+ "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=\"TEXT-INDENT:1cm;FONT-STYLE:italic\">doc;dot;html;htm;tif;mht</span>"
			+ "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;后缀名之间是用<b>逗号或者分号</b>分隔的。</p>"
	},

	_buildTableRow : function(table)
	{
		if (table.firstChild == null)
			$HGDomElement.createElementFromTemplate({nodeName : "tbody"}, table);

		return $HGDomElement.createElementFromTemplate({nodeName : "tr"}, table.firstChild);
	},

	_buildTableCell : function(tableRow, span, className)
	{
		if (span == null)
			span = 1;

		if (className == null)
			className ="";

		var tableCell = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "TD",
				properties :
				{
					colSpan : span
				},
				cssClasses : [className]
			},
			tableRow);

		return tableCell;
	},

	_showMainButtons : function()
	{
		var tableRow = this._buildTableRow(this._tableMain);
		var tableCell = this._buildTableCell(tableRow, 1);
		tableCell.align = "center";
		tableCell.style.height = "40px";

		this._showOKButton(tableCell);
		this._showSetDefaultButton(tableCell);
		this._showCancelButton(tableCell);
	},

	_showOKButton : function(tableCell)
	{
		this._okButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "input",
				properties :
				{
					type : "button",
					value : "保存(S)",
					accessKey : "S",
					style :
					{
						width :"80px"
					}
				},
				cssClasses : ["formButton"],
				events : this._okButtonEvents
			},
			tableCell
			);
	},

	_showSetDefaultButton : function(tableCell)
	{
		this._setDefaultButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "input",
				properties :
				{
					type : "button",
					value : "恢复缺省值(D)",
					accessKey : "D"
				},
				cssClasses : ["formButton"],
				events : this._setDefaultButtonEvents
			},
			tableCell
			);
	},

	_showCancelButton : function(tableCell)
	{
		this._cancelButton = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "input",
				properties :
				{
					type : "button",
					value : "取消(C)",
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

	_onOkButtonClick : function()
	{
		this._invoke(
			"SetOpenInLineFileExtensionNames",
			[this._userID, this._inputFileExtensionNames.value],
			Function.createDelegate(this, this._setFileExtensionNamesCallback),
			Function.createDelegate(this, this._setFileExtensionNamesCallbackError),
			false,
			false);
	},

	_setFileExtensionNamesCallbackError : function(err)
	{
        switch (err.name)
        {
            case "Error":
                alert(String.format("调用函数{0}出现错误:{1}", "SetOpenInLineFileExtensionNames", err.message));
                break;
            case "System.Exception":
                alert(String.format("调用函数{0}出现异常:{1}", "SetOpenInLineFileExtensionNames", err.message));
                break;            
            default:
                alert(String.format("调用函数{0}出现其他异常:{1}", "SetOpenInLineFileExtensionNames", err.message));
                break;
        }
	 },
	 
	_setFileExtensionNamesCallback : function(fileExts)
	{
		window.returnValue = fileExts;

		window.close();
	},
	 
	_onSetDefaultButtonClick : function()
	{
		if (window.confirm("确定要恢复成缺省值吗？"))
		{
			this._invoke(
				"GetDefaultValue",
				[],
				Function.createDelegate(this, this._setDefaultCallback),
				Function.createDelegate(this, this._setDefaultCallbackError),
				false,
				false);
		}
	},

	_setDefaultCallback : function(value)
	{
		this._inputFileExtensionNames.value = value;
	},
	
	_setDefaultCallbackError : function(err)
	{
		switch (err.name)
        {
            case "Error":
                alert(String.format("调用函数{0}出现错误:{1}", "GetDefaultValue", err.message));
                break;
            case "System.Exception":
                alert(String.format("调用函数{0}出现异常:{1}", "GetDefaultValue", err.message));
                break;            
            default:
                alert(String.format("调用函数{0}出现其他异常:{1}", "GetDefaultValue", err.message));
                break;
        }
	},

	_onCancelButtonClick : function()
	{
		window.close();
	},

	get_currentFileExtensionNames : function()
	{
		return this._currentFileExtensionNames;
	},
	set_currentFileExtensionNames : function(value)
	{
		if (this._currentFileExtensionNames != value)
		{
			this._currentFileExtensionNames = value;
			this.raisePropertyChanged("currentFileExtensionNames");
		}
	},
	
	get_computerImagePath : function()
	{
		return this._computerImagePath;
	},
	set_computerImagePath : function(value)
	{
		if (this._computerImagePath != value)
		{
			this._computerImagePath = value;
			this.raisePropertyChanged("computerImagePath");
		}
	},

	get_inlineDemoImagePath : function()
	{
		return this._inlineDemoImagePath;
	},
	set_inlineDemoImagePath : function(value)
	{
		if (this._inlineDemoImagePath != value)
		{
			this._inlineDemoImagePath = value;
			this.raisePropertyChanged("inlineDemoImagePath");
		}
	},
	
	get_openTypeImagePath : function()
	{
		return this._openTypeImagePath;
	},
	set_openTypeImagePath : function(value)
	{
		if (this._openTypeImagePath != value)
		{
			this._openTypeImagePath = value;
			this.raisePropertyChanged("openTypeImagePath");
		}
	}
}

$HBRootNS.DialogFileOpenTypeControl.registerClass($HBRootNSName + ".DialogFileOpenTypeControl", $HGRootNS.ControlBase);
