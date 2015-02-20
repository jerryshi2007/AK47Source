/* File Created: 八月 6, 2014 */
$HBRootNS.RoleGraphControl = function (element) {
    $HBRootNS.RoleGraphControl.initializeBase(this, [element]);

    this._applicationSelectorClientID = "";
    this._roleSelectorClientID = "";
    this._loadingTagClientID = "";
    this._relativeLinkClientID = "";
    this._relativeLinkTemplate = "";

    this._applicationsData = [];
    this._rolesData = [];

    this._applicationSelectorEvents = null;

    this._rolesBuffer = [];

    this._selectedFullCodeName = "";
}

$HBRootNS.RoleGraphControl.prototype =
{
    initialize: function () {
        $HBRootNS.RoleGraphControl.callBaseMethod(this, 'initialize');

        var apps = this.get_applicationsData();

        this._bindApplications(apps, this._get_appCodeNameFromFullCodeName(this.get_selectedFullCodeName()));

        var roles = this.get_rolesData();
        this._bindRoles(roles, this._get_roleCodeNameFromFullCodeName(this.get_selectedFullCodeName()));

        var appSelector = this.get_applicationSelector();

        if (appSelector) {
            this._applicationSelectorEvents = { change: Function.createDelegate(this, this._onApplicationChanged) };
            $addHandlers(appSelector, this._applicationSelectorEvents);

            if (appSelector.value != "") {
                this._pushRolesToBuffer(appSelector.value, roles);
                this._setRelativeLink(appSelector.value);
            }
        }
    },

    dispose: function () {
        var appSelector = this.get_applicationSelector();

        if (appSelector)
            $HGDomEvent.removeHandlers(appSelector, this._applicationSelectorEvents);

        $HBRootNS.RoleGraphControl.callBaseMethod(this, 'dispose');
    },

    loadClientState: function (value) {
    },

    saveClientState: function () {
        return this._selectedFullCodeName;
    },

    _setRelativeLink: function (appCodeName) {
        var relativeLink = this.get_relativeLink();

        if (relativeLink != null) {
            var template = this.get_relativeLinkTemplate();

            relativeLink.href = String.format(template, appCodeName);
        }
    },

    _onApplicationChanged: function (e) {
        var appCodeName = e.target.value;
        var roleInfo = this._getRolesInBuffer(appCodeName);

        if (roleInfo == null) {
            var invoker = this;

            this._set_loadingState(true);

            this._invoke("GetAppRoles", [appCodeName],
				function (roles) {
				    invoker._set_loadingState(false);
				    invoker._pushRolesToBuffer(appCodeName, roles);
				    invoker._bindRoles(roles);
				    invoker._setRelativeLink(appCodeName);
				},
			Function.createDelegate(this, this._executeError));
        }
        else {
            this._bindRoles(roleInfo.roles);
            this._setRelativeLink(appCodeName);
        }
    },

    _executeError: function (e) {
        this._set_loadingState(false);
        $showError(e);

        this._bindRoles([]);
    },

    _set_loadingState: function (isLoding) {
        this._showLoadingTag(isLoding);
    },

    get_applicationSelectorClientID: function () {
        return this._applicationSelectorClientID;
    },

    set_applicationSelectorClientID: function (value) {
        this._applicationSelectorClientID = value;
    },

    get_roleSelectorClientID: function () {
        return this._roleSelectorClientID;
    },

    set_roleSelectorClientID: function (value) {
        this._roleSelectorClientID = value;
    },

    get_loadingTagClientID: function () {
        return this._loadingTagClientID;
    },

    set_loadingTagClientID: function (value) {
        this._loadingTagClientID = value;
    },

    get_relativeLinkClientID: function () {
        return this._relativeLinkClientID;
    },

    set_relativeLinkClientID: function (value) {
        this._relativeLinkClientID = value;
    },

    get_relativeLinkTemplate: function () {
        return this._relativeLinkTemplate;
    },

    set_relativeLinkTemplate: function (value) {
        this._relativeLinkTemplate = value;
    },

    get_applicationsData: function () {
        return this._applicationsData;
    },

    set_applicationsData: function (value) {
        this._applicationsData = value;
    },

    get_rolesData: function () {
        return this._rolesData;
    },

    set_rolesData: function (value) {
        this._rolesData = value;
    },

    get_relativeLink: function () {
        var result = null;

        var relativeLinkID = this.get_relativeLinkClientID();

        if (relativeLinkID != "")
            result = $get(relativeLinkID);

        return result;
    },

    get_loadingTag: function () {
        var result = null;

        var loadingTagID = this.get_loadingTagClientID();

        if (loadingTagID != "")
            result = $get(loadingTagID);

        return result;
    },

    get_applicationSelector: function () {
        var result = null;

        var selectorID = this.get_applicationSelectorClientID();

        if (selectorID != "")
            result = $get(selectorID);

        return result;
    },

    get_roleSelector: function () {
        var result = null;

        var selectorID = this.get_roleSelectorClientID();

        if (selectorID != "")
            result = $get(selectorID);

        return result;
    },

    get_selectedFullCodeName: function () {
        return this._selectedFullCodeName;
    },

    set_selectedFullCodeName: function (value) {
        this._selectedFullCodeName = value;
    },

    _collectResult: function () {
        var appSelector = this.get_applicationSelector();
        var roleSelector = this.get_roleSelector();

        var appCodeName = "";
        var roleCodeName = "";

        if (appSelector != null)
            appCodeName = appSelector.value;

        if (roleSelector != null)
            roleCodeName = roleSelector.value;

        var result = "";

        if (appCodeName != "" && roleCodeName != "")
            result = appCodeName + ":" + roleCodeName;

        return result;
    },

    adjustDialogParameters: function (parameters) {
        parameters["selectedFullCodeName"] = encodeURIComponent(this.get_selectedFullCodeName());
    },

    _onConfirm: function (args) {
        var rst = this._collectResult();
        if (rst != null && rst.length) {
            this._selectedFullCodeName = rst;
            args.result = rst;
        } else {
            args.canceled = true;
        }
    },

    showDialog: function (callback) {
        try {
            this._showDialog(null, null, function (s, e) {
                callback(e.result);
            });
        }
        catch (e) {
            $showError(e);
        }
    },

    _showLoadingTag: function (visible) {
        var loadingTag = this.get_loadingTag();

        if (loadingTag != null) {
            if (visible)
                loadingTag.style.visibility = "visible";
            else
                loadingTag.style.visibility = "hidden";
        }
    },

    _bindApplications: function (apps, selectedAppCodeName) {
        var selector = this.get_applicationSelector();

        if (selector != null) {
            selector.options.length = 0;

            for (var i = 0; i < apps.length; i++) {
                $HGDomElement.addSelectOption(selector, apps[i].name, apps[i].codeName);
            }

            if (selectedAppCodeName && selectedAppCodeName != "")
                selector.value = selectedAppCodeName;
        }
    },

    _bindRoles: function (roles, selectedRoleCodeName) {
        var selector = this.get_roleSelector();

        if (selector != null) {
            selector.options.length = 0;

            for (var i = 0; i < roles.length; i++) {
                $HGDomElement.addSelectOption(selector, roles[i].name, roles[i].codeName);
            }

            if (selectedRoleCodeName && selectedRoleCodeName != "")
                selector.value = selectedRoleCodeName;
        }
    },

    _pushRolesToBuffer: function (appCN, r) {
        this._rolesBuffer.push({ appCodeName: appCN, roles: r });
    },

    _getRolesInBuffer: function (appCN) {
        var result = null;

        for (var i = 0; i < this._rolesBuffer.length; i++) {
            if (this._rolesBuffer[i].appCodeName == appCN) {
                result = this._rolesBuffer[i];
                break;
            }
        }

        return result;
    },

    _get_appCodeNameFromFullCodeName: function (fullCodeName) {
        var appCodeName = "";

        if (fullCodeName != null && fullCodeName != "") {
            var parts = fullCodeName.split(":");

            appCodeName = parts[0];
        }

        return appCodeName;
    },

    _get_roleCodeNameFromFullCodeName: function (fullCodeName) {
        var roleCodeName = "";

        if (fullCodeName != null && fullCodeName != "") {
            var parts = fullCodeName.split(":");

            if (parts.length > 0)
                roleCodeName = parts[1];
        }

        return roleCodeName;
    },

    pseudo: function () {
    }
}

$HBRootNS.RoleGraphControl.registerClass($HBRootNSName + ".RoleGraphControl", $HBRootNS.DialogControlBase);