$HBRootNS.OfficeViewerWrapper = function (element) {
	$HBRootNS.OfficeViewerWrapper.initializeBase(this, [element]);

	this._viewerControlID = null;
	this._autoOpenDefaultUrl = true;
	this._defaultOpenUrl = null;
	this._absoluteDefaultOpenUrl = null;
	this._showToolbars = true;
}

$HBRootNS.OfficeViewerWrapper.prototype =
{
	initialize: function () {
		$HBRootNS.OfficeViewerWrapper.callBaseMethod(this, 'initialize');
	},

	dispose: function () {
		$HBRootNS.OfficeViewerWrapper.callBaseMethod(this, 'dispose');
	},

	get_showToolbars: function () {
		return this._showToolbars;
	},

	set_showToolbars: function (value) {
		this._showToolbars = value;
	},

	get_viewer: function () {
		return $get(this.get_viewerControlID());
	},

	get_autoOpenDefaultUrl: function () {
		return this._autoOpenDefaultUrl;
	},

	set_autoOpenDefaultUrl: function (value) {
		this._autoOpenDefaultUrl = value;
	},

	get_defaultOpenUrl: function () {
		return this._defaultOpenUrl;
	},

	set_defaultOpenUrl: function (value) {
		this._defaultOpenUrl = value;
	},

	get_absoluteDefaultOpenUrl: function () {
		return this._absoluteDefaultOpenUrl;
	},

	set_absoluteDefaultOpenUrl: function (value) {
		this._absoluteDefaultOpenUrl = value;
	},

	get_viewerControlID: function () {
		return this._viewerControlID;
	},

	set_viewerControlID: function (value) {
		this._viewerControlID = value;
	},

	add_documentOpened: function (handler) {
		this.get_events().addHandler("documentOpened", handler);
	},

	remove_documentOpened: function (handler) {
		this.get_events().removeHandler("documentOpened", handler);
	},

	raiseDocumentOpened: function () {
		var handlers = this.get_events().getHandler("documentOpened");
		var e = new Sys.EventArgs();
		this.get_viewer().SetAppFocus();
		try {
			e.document = this.get_viewer().activeDocument;
		}
		catch (ex) {
			alert(ex);
		}
		if (handlers)
			handlers(this, e);

		return e;
	},

	get_cloneableProperties: function () {
		var baseProperties = $HBRootNS.OfficeViewerWrapper.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["showToolbars", "autoOpenDefaultUrl", "defaultOpenUrl", "absoluteDefaultOpenUrl", "viewerControlID"];

		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	onAfterCloneElement: function (sourceElement, newElement) {
		var officeViewerObj = sourceElement.childNodes[0].cloneNode(false);
		officeViewerObj.id = newElement.id + "_Viewer";
		newElement.appendChild(officeViewerObj);
	},

	onAfterCloneComponent: function (element, result) {
		result.set_viewerControlID(element.childNodes[0].id);
	},

	RI: function () {
	}
}

$HBRootNS.OfficeViewerWrapper.registerClass($HBRootNSName + ".OfficeViewerWrapper", $HGRootNS.ControlBase);
