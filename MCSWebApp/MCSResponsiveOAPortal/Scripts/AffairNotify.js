/// <reference path="jquery/jquery-2.0.3.js" />

/* File Created: 五月 16, 2014 */
/* 任务通知 */

jQuery(document).ready(function () {

    window.affairService = new Object(); //全局对象
    var AUTO_CLOSE_DELAY_SECONDS = 5; //自动隐藏时间 秒
    var notifier = null;
    var isIE9 = false;
    var baseName = "MCSResponsiveOAPortal";
    var basePathParts = window.location.href.split("/");
    var baseIndex = -1;
    var iconBase = null;

    for (var i = basePathParts.length - 1; i >= 0; i--) {
        if (basePathParts[i] === baseName) {
            baseIndex = i;
            break;
        }
    }

    function makeAbsolute(path) {
        var index = baseIndex, i, s;
        if (typeof (path) === 'string') {
            if (path.length > 0 && path.charAt(0) == '~') {
                if (baseIndex >= 0) {
                    var input = path.split("/");

                    if (input.length > 0 && input[0] === '~') {
                        var stack = [];
                        for (i = 0; i <= baseIndex; i++) {
                            stack.push(basePathParts[i]);
                        }

                        for (i = 1; i < input.length; i++) {
                            s = input[i];
                            if (s == '..') {
                                if (stack.length > 0) {
                                    stack.pop();
                                } else {
                                    throw new Error("路径错误，无法从顶级目录退出。");
                                }
                            } else if (s !== '.') {
                                stack.push(s);
                            }
                        }

                        return stack.join("/");
                    } else {
                        throw new Error("路径格式错误");
                    }

                }
            } else {
                return path;
            }
        }
    }

    function CookieMan() {
    }

    CookieMan.prototype = {
        openStore: function () {
            return this._innerGetStore();
        },

        saveStore: function (cookie) {
            var arr = [];
            for (var n in cookie) {
                arr.push(encodeURIComponent(n) + "=" + encodeURIComponent(cookie[n]));
            }

            var s = "MCSROAP=" + arr.join("&");
            document.cookie = s;
        },

        _innerGetStore: function () {
            var cook = document.cookie, subCook, ind, indL, cookParts, result = new Object(), indB, name, val;
            ind = cook.indexOf("MCSROAP=");
            if (ind >= 0) {
                indL = cook.indexOf(";", ind + 1);
                subCook = indL > 0 ? cook.slice(ind + 8, indL) : cook.substring(ind + 8);
                cookParts = subCook.split("&");
                for (var i = cookParts.length - 1; i >= 0; i--) {
                    indB = cookParts[i].indexOf("=");
                    if (indB <= 0)
                        throw new Error("Cookie格式错误");
                    name = decodeURIComponent(cookParts[i].slice(0, indB));
                    val = decodeURIComponent(cookParts[i].substring(indB + 1));
                    result[name] = val;
                }
            }

            return result;
        }
    };

    affairService.cookieMan = new CookieMan();

    if (window.external) {
        try {
            window.external.msIsSiteMode();
            iconBase = makeAbsolute("~/Images");
            isIE9 = true;
        } catch (ex) {
            isIE9 = false;
        }
    }

    var isIePinedSites = function () {
        var isPined = false;
        if (isIE9) {
            try {
                if (window.external.msIsSiteMode()) {
                    // TRUE
                    isPined = true;
                }
            } catch (e) { }
        }

        return isPined;
    }

    if (window.webkitNotifications) {
        notifier = window.webkitNotifications;
    } else if (window.Notification) {
        notifier = window.Notification;
        notifier.checkPermission = function () {
            switch (window.Notification.permission) {
                case "granted":
                    return 0;
                case "denied":
                    return 2;
                case "default":
                    return 1;
            }
            return 2;
        }

        notifier.createNotification = function (icon, url, body) {
            return new Notification(url, { icon: icon, body: body });
        }
    }

    var initNotifier = function () {
        var btn, div, btn2;
        var cook = affairService.cookieMan.openStore();
        if (cook.nevershowprompt == '1') {
            // 不再显示
        } else if (notifier) {
            switch (notifier.checkPermission()) {
                case 0:
                    break;
                case 1:
                    //用户未处理
                    btn = document.createElement("button");
                    div = document.createElement("div");
                    btn2 = document.createElement("button");
                    div.className = "alert alert-warning alert-dismissable";

                    btn2.type = btn.type = "button";
                    btn.className = "btn btn-primary ";
                    btn2.className = "close";
                    $.attr(btn2, "data-dismiss", "alert");
                    $.attr(btn2, "aria-hidden", "true");
                    btn2.title = "关闭";
                    btn.appendChild(document.createTextNode("设置桌面通知"));
                    div.style.display = "block";
                    div.style.width = "600px";
                    div.style.margin = "auto";
                    document.body.appendChild(div);
                    div.appendChild(document.createTextNode("要在有新的待办到来时为您显示桌面通知，请点击 "));
                    div.appendChild(btn);
                    div.appendChild(btn2);
                    btn2.innerHTML = "&times;";
                    btn.onclick = function () {
                        notifier.requestPermission();
                        div.style.display = "none";
                    }

                    div.style.position = "fixed";
                    div.style.bottom = "0";
                    div.style.right = "0";
                    btn2.onclick = function () {
                        var cook = affairService.cookieMan.openStore();
                        cook["nevershowprompt"] = "1";
                        affairService.cookieMan.saveStore(cook);
                    }
                    break;
                case 2:
                    break;
            }

            window.affairService.notify = notify;
        } else if (isIE9) {
            if (!isIePinedSites()) {
                btn = document.createElement("button");
                div = document.createElement("div");
                btn2 = document.createElement("button");
                div.className = "alert alert-warning alert-dismissable";

                btn2.type = btn.type = "button";
                btn.className = "btn btn-primary ";
                btn2.className = "close";
                $.attr(btn2, "data-dismiss", "alert");
                $.attr(btn2, "aria-hidden", "true");
                btn2.title = "关闭";
                btn.appendChild(document.createTextNode("添加到开始菜单"));
                div.style.display = "block";
                div.style.width = "600px";
                div.style.margin = "auto";
                document.body.appendChild(div);
                div.appendChild(document.createTextNode("建议将此站点"));
                div.appendChild(document.createTextNode("固定到任务栏（拖动页面顶部标签页到任务栏） 或"));
                div.appendChild(btn);

                div.appendChild(btn2);
                btn2.innerHTML = "&times;";
                btn.onclick = function () {
                    window.external.msAddSiteMode();
                    div.style.display = "none";
                }

                div.style.position = "fixed";
                div.style.bottom = "0";
                div.style.right = "0";

                btn2.onclick = function () {
                    var cook = affairService.cookieMan.openStore();
                    cook["nevershowprompt"] = "1";
                    affairService.cookieMan.saveStore(cook);
                }
            } else {

            }
        }
    }

    initNotifier();

    /*(╯▔皿▔)╯ 有一大波丧尸正在靠近 ㄟ(▔皿▔ㄟ)*/
    function notify(e) {
        if (notifier) {
            if (notifier.checkPermission() == 0) {
                var popup = notifier.createNotification(e.icon, e.title, e.body);
                popup.onclick = function () {
                    if (e.url) {
                        var w = window.open(e.url, "", "width=800,height=600");
                        w.fullScreen = true;
                    }
                    this.close();
                }
                popup.ondisplay = function (event) {
                    setTimeout(function () {
                        event.currentTarget.cancel();
                    }, AUTO_CLOSE_DELAY_SECONDS * 1000);
                }
                if (("show" in popup)) {
                    popup.show();
                }
            } else {
                //checkPermission();
            }
        } else if (isIE9) {
            if (isIePinedSites()) {
                window.external.msSiteModeActivate();
            }
        }
    }

    (function ($) {

        var iconPath = makeAbsolute("~/Images/xinxi01.gif");
        var servicePath = makeAbsolute("~/Services/JsonServices.ashx?method=GetTaskStat");
        var cbFetchTaskStat = function (data) {
            var taskCounts, newTask, banCount;
            if (data && data.length > 2) {
                taskCounts = data[0];
                newTask = data[2];
                banCount = taskCounts.BanCount || 0;
                $("span.badge[data-badgekey=todoCount]").text(banCount > 99 ? "99+" : banCount).attr("title", banCount);

                try {
                    if (banCount > 0) {
                        window.external.msSiteModeSetIconOverlay(iconBase + '/bell_iphone.ico', '有未处理待办');
                    } else {
                        window.external.msSiteModeClearIconOverlay();
                    }
                } catch (ex) { }

                if (newTask) {
                    var cook = affairService.cookieMan.openStore(), hasReadBefore = false;

                    if (typeof (cook.lcTaskID) === 'string') {
                        if (cook.lcTaskID == newTask.TaskID) {
                            hasReadBefore = true;
                        }
                    }

                    if (hasReadBefore == false) {
                        cook.lcTaskID = newTask.TaskID;
                        affairService.cookieMan.saveStore(cook);
                        notify({
                            "url": newTask.Url,
                            "icon": iconPath,
                            "title": newTask.PopupTitle,
                            "body": newTask.ApplicationName + " - " + newTask.TaskTitle
                        });

                        jQuery(document).trigger("taskarrived.mcsroap", newTask.TaskID); //触发修改事件
                        window.setTimeout(querys, 1000); // 再查一次
                    }
                }
            }
        }

        function isWebServiceReady() {
            return typeof (MCSResponsiveOAPortal) === 'object' && typeof (MCSResponsiveOAPortal.PortalServices) === 'function';
        }

        var fetchTaskStat = function () {

            if (isWebServiceReady()) {
                MCSResponsiveOAPortal.PortalServices.QueryUserTaskStatus(cbFetchTaskStat);

            } else {
                $.post(servicePath, null, cbFetchTaskStat, "json").fail(function (err, desc) {
                });
            }
        }

        window.affairService.fetchTaskStat = fetchTaskStat;

        function querys() {
            // 以下被注释掉，每个窗口独立查询，互不干扰，否则可能显示的待办数不刷新
            //            var cook = affairService.cookieMan.openStore();
            //            var lastChkTime = null;
            //            if (typeof (cook.lcChkTime) !== 'undefined') {
            //                lastChkTime = new Date(parseInt(cook.lcChkTime));
            //            }

            //            if (!lastChkTime || (new Date().getTime() - lastChkTime.getTime()) > 2000) {
            //                cook.lcChkTime = new Date().getTime();
            //                affairService.cookieMan.saveStore(cook);
            fetchTaskStat();
            //            }
        }

        window.setInterval(function () {
            querys();
        }, 30 * 1000);

        querys();

    })(jQuery);

    (function ($) {
        // 杂项
        $("span[data-role=userNameHolder]").text($("a[data-role=logoutlink]").attr("data-login-user"));
        $("a[data-action=logout]").click(function () {
            window.location.replace($("a[data-role=logoutlink]").attr("href"));
        })
    })(jQuery);

    //    jQuery(document).ajaxSuccess(function (e, q, s, er) {
    //        debugger;
    //    }).ajaxError(function (e, q, setting, error) {
    //        debugger;
    //    });
});