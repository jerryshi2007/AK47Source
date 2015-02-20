(function () {
    Sys.Application.add_unload(function () {

    });

    Sys.Application.add_load(function () {
        $pc.show("progressbar");
        $pc.show("prompt");

        var appData = [];
        var currentApp;
        var context = "app";
        var roleData = {};
        var selectedElem = null;

        var currentData = appData;

        function val(properties, key) {
            for (var i = properties.length - 1; i >= 0; i--) {
                if (properties[i].Key == key) {
                    return properties[i];
                }
            }

            return null;
        }

        function nextLi(self) {
            do {
                self = self.nextSibling;
            } while (self != null && (self.nodeType !== 1 && self.nodeName.toUpperCase() !== "LI"));

            return self;
        }

        function appyFilter(key) {
            selectedElem = null;
            var regex = new RegExp(key, "i");
            var len = currentData.length;
            var item;
            var row;
            var li = $get("list").firstChild;
            if (li) {
                for (var i = 0; i < len && li != null; i++) {
                    item = currentData[i];
                    var name = val(item.Properties, "Name").StringValue;
                    var code = val(item.Properties, "CodeName").StringValue;
                    if (regex.test(name) || regex.test(code)) {
                        $pc.removeClass(li, "pc-hide");
                    } else {
                        $pc.addClass(li, "pc-hide");
                    }

                    li = nextLi(li);
                }
            }
        }

        $pc.bindEvent($get("pickApp"), "click", function () {
            initApplications();
        });

        $pc.bindEvent($get("btnOk"), "click", function () {
            if (context == "role" && selectedElem != null) {
                var id = $pc.getAttr(selectedElem, "data-id");
                if (id) {
                    service.GetRoleDisplayItems([id], function (data) {
                        if (typeof (window.dialogArguments) == "object" && data.length) {
                            if ("window" in window.dialogArguments && "inputElem" in window.dialogArguments) {
                                var hiddenField = window.dialogArguments.window.document.getElementById(window.dialogArguments.inputElem);
                                if (hiddenField != null && hiddenField.nodeType == 1 && hiddenField.nodeName.toUpperCase() == "INPUT") {
                                    hiddenField.value = Sys.Serialization.JavaScriptSerializer.serialize(data);
                                    window.returnValue = true;
                                    hiddenField = null;
                                    window.close();
                                }
                            }
                        }
                    }, function (err) {
                        alert("出错" + err.get_message());
                    });
                }
            }
        });

        var clickHandler = function (e) {

            ea = e || window.event;
            var src = ea.srcElement || ea.currentTarget;

            while (src && src.nodeName.toUpperCase() !== "LI") {
                src = src.parentNode;
            }

            if ("preventDefault" in e)
                e.preventDefault();
            if ("returnValue" in e)
                e.returnValue = false;
            var id = $pc.getAttr(src, "data-id");

            if (context == "app") {
                requestRoles(id);
            } else if (context == "role") {
                if (selectedElem)
                    $pc.removeClass(selectedElem, "au-selected");
                selectedElem = src;
                $pc.addClass(selectedElem, "au-selected");
            }
        };

        var filterElem = $get('filter');
        if (filterElem) {
            var lastKeyDownTime = new Date().getTime();
            var timeOutWasSet = false;

            function addTimeOut() {
                if (!timeOutWasSet) {
                    timeOutWasSet = true;

                    if (selectedElem)
                        $pc.removeClass(selectedElem, "au-selected");
                    selectedElem = null;

                    window.setTimeout(function () {
                        timeOutWasSet = false;
                        appyFilter(filterElem.value);
                    }, 500);
                }
            }

            $pc.bindEvent(filterElem, "keydown", function (e) {
                addTimeOut();
            });
        }

        function renderList(currentData) {
            var list = $get("list");
            selectedElem = null;
            list.innerHTML = '';
            $get("filter").value = "";
            var li, span, button;
            var len = currentData.length;
            var item;
            for (var i = 0; i < len; i++) {
                item = currentData[i];
                if (item.Status == 1) {
                    li = document.createElement("li");
                    list.appendChild(li);
                    $pc.addClass(li, "au-row");
                    $pc.bindEvent(li, "click", clickHandler);

                    span = document.createElement("span");
                    li.appendChild(span);
                    $pc.addClass(span, "au-title");
                    span.unselectable = true;
                    span.appendChild(document.createTextNode(val(item.Properties, "Name").StringValue));

                    span = document.createElement("span");
                    li.appendChild(span);
                    $pc.addClass(span, "au-description");
                    span.unselectable = true;
                    span.appendChild(document.createTextNode("(" + val(item.Properties, "CodeName").StringValue + ")"));

                    $pc.setAttr(li, "data-id", item.ID);
                }
            }
        }

        function initApplications() {

            context = "app";
            currentApp = null;
            currentData = appData;
            $pc.setText("appLabel", "");
            renderList(currentData);
        }

        function initRoles() {
            currentApp = null;
            currentData = roleData.roles;

            for (var i = appData.length - 1; i >= 0; i--) {
                if (appData[i].ID == roleData.appId) {
                    currentApp = appData[i];
                    break;
                }
            }

            if (currentApp != null) {
                $pc.setText("appLabel", val(currentApp.Properties, "Name").StringValue);
                context = "role";
            }

            renderList(currentData);
        }

        var service = new PermissionCenter.Services.PermissionCenterQueryService();
        service.GetObjectsByIDs(null, ["Applications"],true, function (data) {

            appData = data;

            initApplications();

            $pc.hide("prompt");
            $pc.hide("progressbar");

        }, function (err) {
            $pc.setText("prompt", "载入失败，请刷新后重试。" + err.get_message());
            $pc.hide("prompt");
            $pc.hide("progressbar");
        });

        function requestRoles(id) {
            if (id) {
                $pc.show("progressbar");
                $pc.show("prompt");
                service.GetMembers(id, ["Roles"],true, function (data) {

                    roleData = {
                        appId: id,
                        roles: data
                    };
                    initRoles();
                    $pc.hide("prompt");
                    $pc.hide("progressbar");

                }, function (err) {
                    $pc.setText("prompt", "载入失败，请刷新后重试。" + err.get_message());
                    $pc.hide("prompt");
                    $pc.hide("progressbar");
                });
            }
        }
    });

})();