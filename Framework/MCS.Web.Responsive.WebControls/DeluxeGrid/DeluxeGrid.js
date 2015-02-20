$HGRootNS.DeluxeGrid = function (element) {
	$HGRootNS.DeluxeGrid.initializeBase(this, [element]);

	//客户端对象集合
	this._clientSelectedKeys = [];
	this._checkAll = null;
	this._checkAllBox = null;

	this._exportExcelTopButton = null;     //导出excel格式的top按钮
	this._exportExcelButton = null;        //导出excel格式的按钮

	this._exportWordTopButton = null;      //导出word格式的top按钮
	this._exportWordButton = null;         //导出word格式的按钮 

	this._excelTopButtonClientID = null;     //导出excel格式的top按钮
	this._excelButtonClientID = null;        //导出excel格式的按钮

	this._wordTopButtonClientID = null;      //导出word格式的top按钮
	this._wordButtonClientID = null;         //导出word格式的按钮

	this._isChecked = null;
	this._isSelectedAll = null;

	this._showCheckBoxes = false; //是否启用checkbox选择框
	this._openExportWordDocumentUrl = null;
	this._openExportExcelDocumentUrl = null;

	this._btnWordClientID = null;
	this._btnExcelClientID = null;
	this._btnWordServer = null;
	this._btnExcelServer = null;
	this._showExportControl = false;
	this._showPager = false;

	this._checkItemName = "";
	this._allowClientDataBind = false;

	this._checkEvents =
	{
		click: Function.createDelegate(this, this.deluxeGrid_ClickCheckAll)
	};

	this._checkEvent =
	{
		click: Function.createDelegate(this, this.deluxeGrid_ClickCheckItem)
	};

	this._onExportEvents =
	{
		click: Function.createDelegate(this, this.deluxeGrid_ClickExport)
	};

	this._onExportWordEvents =
	{
		click: Function.createDelegate(this, this.deluxeGrid_ClickExportWord)
	};

}

$HGRootNS.DeluxeGrid.prototype =
{
	initialize: function () {
		$HGRootNS.DeluxeGrid.callBaseMethod(this, 'initialize');

		//初始化控件
		this._initializeControls();

		//当前页的总记录数
		var currentCount = 0;

		if (this._showCheckBoxes && this._isChecked) {
			if (this._checkAll != null && this._checkAllBox != null) {
				if (this._isSelectedAll)
					this._checkAllBox.checked = true;
				else
					this._checkAllBox.checked = false;

				$addHandlers(this._checkAllBox, this._checkEvents);

				var allCheckBoxes = this.get_allCheckBoxes();

				for (var i = 0; i < allCheckBoxes.length; i++) {
					$addHandlers(allCheckBoxes[i], this._checkEvent);
				}

				this.deluxeGrid_SetCheckAllButtonStatus();
			}
		}

	},

	//add by longmark 2008-04-18
	_initializeControls: function () {
		if (this._checkAll)
			this._checkAllBox = $get(this._checkAll);

		//显示导出按钮
		if (this._showPager && this._showExportControl) {
			this._exportExcelTopButton = this._excelTopButtonClientID ? $get(this._excelTopButtonClientID) : null;
			this._exportExcelButton = this._excelButtonClientID ? $get(this._excelButtonClientID) : null;
			this._exportWordTopButton = this._wordTopButtonClientID ? $get(this._wordTopButtonClientID) : null;
			this._exportWordButton = this._wordButtonClientID ? $get(this._wordButtonClientID) : null;

			this._btnWordServer = this._btnWordClientID ? $get(this._btnWordClientID) : null;
			this._btnExcelServer = this._btnExcelClientID ? $get(this._btnExcelClientID) : null;

			if (this._exportExcelTopButton)
				$addHandlers(this._exportExcelTopButton, this._onExportEvents);

			if (this._exportExcelButton)
				$addHandlers(this._exportExcelButton, this._onExportEvents);

			if (this._exportWordTopButton)
				$addHandlers(this._exportWordTopButton, this._onExportWordEvents);

			if (this._exportWordButton)
				$addHandlers(this._exportWordButton, this._onExportWordEvents);
		}
	},

	set_showCheckBoxes: function (value) {
		this._showCheckBoxes = value;
	},

	get_showCheckBoxes: function () {
		return this._showCheckBoxes;
	},

	set_showExportControl: function (value) {
		this._showExportControl = value;
	},

	get_showExportControl: function () {
		return this._showExportControl;
	},

	set_showPager: function (value) {
		this._showPager = value;
	},

	get_showPager: function () {
		return this._showPager;
	},

	set_isSelectedAll: function (value) {
		this._isSelectedAll = value;
	},

	get_isSelectedAll: function () {
		return this._isSelectedAll;
	},

	set_serverExportButton: function (value) {
		this._serverExportButton = value;
	},

	get_serverExportButton: function () {
		return this._serverExportButton;
	},

	set_exportExcelButton: function (value) {
		this._exportExcelButton = value;
	},

	get_exportExcelButton: function () {
		return this._exportExcelButton;
	},

	set_exportExcelTopButton: function (value) {
		this._exportExcelTopButton = value;
	},

	get_exportExcelTopButton: function () {
		return this._exportExcelTopButton;
	},

	set_clientSelectedKeys: function (value) {
		this._clientSelectedKeys = value;
	},

	get_clientSelectedKeys: function () {
		return this._clientSelectedKeys;
	},

	set_checkAll: function (value) {
		this._checkAll = value;
	},

	get_checkAll: function () {
		return this._checkAll;
	},

	set_isChecked: function (value) {
		this._isChecked = value;
	},

	get_isChecked: function () {
		return this._isChecked;
	},

	set_exportWordButton: function (value) {
		this._exportWordButton = value;
	},

	get_exportWordButton: function () {
		return this._exportWordButton;
	},

	set_exportWordTopButton: function (value) {
		this._exportWordTopButton = value;
	},

	get_exportWordTopButton: function () {
		return this._exportWordTopButton;
	},

	set_excelTopButtonClientID: function (value) {
		this._excelTopButtonClientID = value;
	},

	get_excelTopButtonClientID: function () {
		return this._excelTopButtonClientID;
	},

	set_excelButtonClientID: function (value) {
		this._excelButtonClientID = value;
	},

	get_excelButtonClientID: function () {
		return this._excelButtonClientID;
	},

	set_wordTopButtonClientID: function (value) {
		this._wordTopButtonClientID = value;
	},

	get_wordTopButtonClientID: function () {
		return this._wordTopButtonClientID;
	},

	set_wordButtonClientID: function (value) {
		this._wordButtonClientID = value;
	},

	get_wordButtonClientID: function () {
		return this._wordButtonClientID;
	},

	get_openExportWordDocumentUrl: function () {
		return this._openExportWordDocumentUrl;
	},

	set_openExportWordDocumentUrl: function (value) {
		this._openExportWordDocumentUrl = value;
	},

	get_openExportExcelDocumentUrl: function () {
		return this._openExportExcelDocumentUrl;
	},

	set_openExportExcelDocumentUrl: function (value) {
		this._openExportExcelDocumentUrl = value;
	},

	//********
	get_btnWordClientID: function () {
		return this._btnWordClientID;
	},

	set_btnWordClientID: function (value) {
		this._btnWordClientID = value;
	},

	get_btnExcelClientID: function () {
		return this._btnExcelClientID;
	},

	set_btnExcelClientID: function (value) {
		this._btnExcelClientID = value;
	},

	get_btnWordServer: function () {
		return this._btnWordServer;
	},

	set_btnWordServer: function (value) {
		this._btnWordServer = value;
	},

	get_btnExcelServer: function () {
		return this._btnExcelServer;
	},

	set_btnExcelServer: function (value) {
		this._btnExcelServer = value;
	},

	get_checkItemName: function () {
		return this._checkItemName;
	},

	set_checkItemName: function (value) {
		this._checkItemName = value;
	},

	get_allowClientDataBind: function () {
		return this._allowClientDataBind;
	},

	set_allowClientDataBind: function (value) {
		this._allowClientDataBind = value;
	},

	//********
	dispose: function () {
		this._input = null;
		$HGRootNS.DeluxeGrid.callBaseMethod(this, 'dispose');
	},

	get_allCheckBoxes: function () {
		var chkName = this.get_checkItemName();

		return document.getElementsByName(chkName);
	},

	//全选||取消全选
	deluxeGrid_ClickCheckAll: function (objAll) {
		var allCheckBoxes = this.get_allCheckBoxes();

		for (var i = 0; i < allCheckBoxes.length; i++) {
			var e = allCheckBoxes[i];

			if (e.disabled == false)
				e.checked = this._checkAllBox.checked;

			if (e.checked == true)
				this.deluxeGrid_AddHiddenValue(e.value);

			if (e.checked == false)
				this.deluxeGrid_DeleteHiddenValue(e.value);

			this._raise_selectCheckboxClickEvent(e);
		}

		this._raise_allSelectCheckboxClickedEvent(this._checkAllBox);
	},

	deluxeGrid_ClickCheckItem: function (e) {
		var cb = e.target;

		if (cb.type == "checkbox") {
			if (cb.checked == true)
				this.deluxeGrid_AddHiddenValue(cb.value);
			else
				this.deluxeGrid_DeleteHiddenValue(cb.value);
			
			this.deluxeGrid_SetCheckAllButtonStatus();
		}
		else {
			this._clientSelectedKeys = [cb.value];
		}

		this._raise_selectCheckboxClickEvent(cb);
	},

	/*
	deluxeGrid_GetObject: function(param) {
	if (document.getElementById(param))
	return document.getElementById(param);
	},
	*/

	//隐藏控件写选中的checkbox值
	deluxeGrid_AddHiddenValue: function (param) {
		//填充的对象中是否有重复值    
		if (this._keySelected(param) == false)
			Array.add(this._clientSelectedKeys, param);
	},

	_keySelected: function (key) {
		var result = false;

		for (var i = 0; i < this._clientSelectedKeys.length; i++) {
			if (key == this._clientSelectedKeys[i]) {
				result = true;
				break;
			}
		}

		return result;
	},

	////隐藏控件删除选中的checkbox值
	deluxeGrid_DeleteHiddenValue: function (param) {
		Array.remove(this._clientSelectedKeys, param);
	},

	//判断全选按钮的状态
	deluxeGrid_SetCheckAllButtonStatus: function () {

		var currentCount = 0;

		if (this._checkAll != null) {
			var allCheckBoxes = this.get_allCheckBoxes();

			for (var i = 0; i < allCheckBoxes.length; i++) {

				var cb = allCheckBoxes[i];

				if (this._keySelected(cb.value)) {
					currentCount++;
				}
			}

			this._checkAllBox.checked = (currentCount == allCheckBoxes.length);
		}
	},

	loadClientState: function (value) {
	},

	saveClientState: function () {
		return Sys.Serialization.JavaScriptSerializer.serialize(this._clientSelectedKeys);
	},

	deluxeGrid_ClickExport: function () {
		this.doExportEvents("excel", this.get_openExportExcelDocumentUrl());
	},

	deluxeGrid_ClickExportWord: function () {
		this.doExportEvents("word", this.get_openExportWordDocumentUrl());
	},

	doExportEvents: function (cmd, url) {
		this._initPostFrame();
		var form = this._findServerForm(this._btnWordServer);

		if (form) {
			var oldTarget = form.target;
			var oldAction = form.action;
			var oldEventTarget = form.__EVENTTARGET.value;

			try {
				form.target = "_blank";
				form.action = url;

				if (cmd == "word") {
					if (this._btnWordServer)
						this._innerPost(form, this._btnWordServer.id);
				}
				else {
					if (this._btnExcelServer)
						this._innerPost(form, this._btnExcelServer.id);
				}
			}
			finally {
				form.target = oldTarget;
				form.action = oldAction;
				form.__EVENTTARGET.value = oldEventTarget;
			}
		}
	},

	_selectAllCheckboxClickedEventKey: "selectAllCheckBoxClick",
	add_selectAllCheckBoxClick: function (value) {
		    this.get_events().addHandler(this._selectAllCheckboxClickedEventKey, value);
	},
	remove_selectAllCheckBoxClick: function (value) {
		this.get_events().removeHandler(this._selectAllCheckboxClickedEventKey, value);
	},

	//复选框单击事件
	_selectCheckboxClickEventKey: "selectCheckBoxClick",
	add_selectCheckBoxClick: function (value) {
		this.get_events().addHandler(this._selectCheckboxClickEventKey, value);
	},
	remove_selectCheckBoxClick: function (value) {
		this.get_events().removeHandler(this._selectCheckboxClickEventKey, value);
	},

	//(全选)复选框单击事件 
	_raise_allSelectCheckboxClickedEvent: function (checkbox) {
		var handler = this.get_events().getHandler(this._selectAllCheckboxClickedEventKey);

		var e = new Sys.EventArgs;
		e.checkbox = checkbox;

		if (handler) {
			handler(this, e);
		}
	},

	//复选框单击事件 
	_raise_selectCheckboxClickEvent: function (checkbox) {
		var handler = this.get_events().getHandler(this._selectCheckboxClickEventKey);

		var e = new Sys.EventArgs;
		e.checkbox = checkbox;

		if (handler) {
			handler(this, e);
		}
	},

	_innerPost: function (form, eventTarget) {
		form.__EVENTTARGET.value = eventTarget;
		form.__EVENTARGUMENT.value = "";
		form.submit();
	},

	_findServerForm: function (elem) {
		var form = null;

		while (elem) {
			if (elem.tagName == "FORM") {
				form = elem;
				break;
			}

			elem = elem.parentElement;
		}

		return form;
	},

	_initPostFrame: function () {
		var fc = $get("frameContainer");

		if (!fc) {
			fc = $HGDomElement.get_currentDocument().createElement("div");
			fc.style.display = "none";
			$HGDomElement.get_currentDocument().body.appendChild(fc);
		}

		fc.innerHTML = "<iframe name=\"gridOpenWindowFrame\"></iframe>";
	}
}

$HGRootNS.DeluxeGrid.registerClass($HGRootNSName + ".DeluxeGrid", $HGRootNS.ControlBase);