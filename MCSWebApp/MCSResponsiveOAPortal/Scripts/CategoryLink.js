/* File Created: 五月 25, 2014 */
jQuery(function ($) {

    function findNode(subNodes, name) {

        if (!subNodes)
            return null;
        for (var i = subNodes.length - 1; i >= 0; i--) {
            if (subNodes[i].Name == name)
                return subNodes[i];
        }

        return null;
    }

    function config(element) {

        var initData = Sys.Serialization.JavaScriptSerializer.deserialize($.attr(element, 'data-initial-data')), data = { name: "Root", subNodes: initData, currentPath: "Root" }, navBar = $("#navPath"), upButton = $('button[data-toggle="upward"]', element), caption = $('[data-role="caption"]', element), mainPanel = $("[data-category-link-role=buttons]", element).first(), urlPattern = $.attr(element, "data-post-pattern"), defaultFeature = $.attr(element, "data-default-feature");

        function renderItems(list, fullPath, panel, groupName) {
            var item, li, a, i, div, h1, span, ul = null, len = list.length, name;

            if (panel.nodeName.toUpperCase() === "UL")
                ul = panel;

            for (var ind = 0; ind < len; ind++) {
                item = list[ind];
                name = item.Name;

                if (item.Type === 'group') {
                    if (ul)
                        throw new Error("容器不应该是UL");

                    div = document.createElement("div");
                    div.className = "clearfix";
                    panel.appendChild(div);
                    h1 = document.createElement("h3");
                    h1.className = "text-muted";
                    div.appendChild(h1);
                    h1.appendChild(document.createTextNode(item.Title));

                    if (item.Categories) {
                        renderItems(item.Categories, fullPath, div, name);
                    }

                } else {

                    if (!ul) {
                        ul = document.createElement("ul");
                        ul.className = "navbar nav-panel-links clearfix";
                        panel.appendChild(ul);
                    }

                    li = document.createElement("li");
                    a = document.createElement("a");
                    i = document.createElement("span");
                    span = document.createElement("span");
                    ul.appendChild(li);
                    li.appendChild(a);
                    a.appendChild(i);
                    a.appendChild(document.createTextNode(" "));
                    a.appendChild(span);
                    span.appendChild(document.createTextNode(item.Title));

                    if (groupName)
                        $.attr(li, "data-group", groupName);

                    $.attr(li, "data-key", item.Name);
                    if (item.Type == "category") {
                        $.attr(li, "data-toggle", "navbutton");
                        li.className = "btn btn-default btn-sm";
                        i.className = "glyphicon glyphicon-folder-open";
                        a.title = item.Title;
                        a.href = "javascript:void(0);";
                        //                        buttons.append('<li data-key="' + item.Name + '" data-toggle="navbutton"><a href="javascript:void(0);">' + item.Title + '</a></li>');
                    } else {
                        $.attr(li, "data-toggle", "link");
                        i.className = "glyphicon glyphicon-new-window";
                        li.className = "btn btn-default btn-sm";
                        a.href = item.Url;
                        a.title = item.Title;
                        a.target = "_blank";
                        $.attr(li, "data-feature", item.Feature);
                        $.attr(li, "data-url", item.Url);
                        //                        buttons.append('<li data-key="' + item.Name + '" data-toggle="link"><a href="' + item.Href + '">' + item.Title + '</a></li>');
                    }
                }
            }

            panel = null;
            li = a = i = div = h1 = span = ul = null;

            mainPanel.data("fullpath", fullPath);
        }

        function removeNodesAfter(node) {
            while (node.nextSibling) {
                node.parentNode.removeChild(node.nextSibling);
            }
        }

        function arrangeNavBar(arr) {

            var node = $("li[data-toggle=nav][data-key=Root]", navBar)[0], n, ind, key, part0, part1, dNode, meet = true, pNode = data, group;
            if ($.attr(node, "data-key") == "Root") {

                for (var i = 1; i < arr.length; i++) {
                    n = arr[i];
                    if ((ind = n.indexOf(">")) > 0) { // group 
                        part0 = n.slice(0, ind);
                        part1 = n.slice(ind + 1);
                        pNode = findNode(pNode.subNodes, part0);
                        if (pNode) {
                            pNode = findNode(pNode.Categories, part1);
                        }
                    } else {
                        pNode = findNode(pNode.subNodes, n);
                    }
                    dNode = node;
                    meet = false;
                    while (dNode = dNode.nextSibling) {
                        if (dNode.nodeType === 1 && dNode.nodeName.toUpperCase() === "LI" && $.attr(dNode, "data-toggle") == "nav") {
                            key = $.attr(dNode, "data-key");

                            if (key == n) {
                                node = dNode; //继续匹配
                                meet = true;
                                break;
                            } else {
                                //右边的节点均无效
                                removeNodesAfter(node);
                            }
                        }
                    }

                    if (meet == false) {
                        var li = document.createElement("li");
                        node.parentNode.appendChild(li);
                        $.attr(li, "data-toggle", "nav");
                        $.attr(li, "data-key", n);
                        var a = document.createElement("a");
                        li.appendChild(a);
                        a.href = "javascript:void(0);";
                        a.appendChild(document.createTextNode(pNode.Title));
                        removeRequired = false;
                    }
                }

                if (meet)
                    removeNodesAfter(node);

            }
        }

        function navTo(arr) {
            var path = arr.join("."), i, part0, ind, part1, pNode, dNode, n, loaded = true, len;
            if (path == data.currentPath && path == mainPanel.data("fullpath"))
                return; //已经是当前节点
            mainPanel.empty();

            if ((len = arr.length) > 0 && arr[0] === data.name) {
                pNode = data;

                for (i = 1; i < len; i++) {
                    n = arr[i];
                    if ((ind = n.indexOf(">")) > 0) {
                        //表示组
                        part0 = n.slice(0, ind);
                        part1 = n.slice(ind + 1);

                        if (pNode.subNodes && (dNode = findNode(pNode.subNodes, part0))) {
                            if (dNode.Type !== "group")
                                throw new Error("A node of group is expected.");

                            if (dNode.Categories && (dNode = findNode(dNode.Categories, part1))) {
                                pNode = dNode;
                            } else {
                                throw new Error("无法在现有集合中找到" + part1);
                            }
                        } else {
                            loaded = false;
                            break;
                        }

                    } else {

                        if (pNode.subNodes && (dNode = findNode(pNode.subNodes, n))) {
                            pNode = dNode;
                        } else {
                            loaded = false;
                            break;
                        }
                    }
                }

                if (loaded)
                    loaded = typeof (pNode.subNodes) !== 'undefined';

                if (!loaded) {

                    $.post(urlPattern + encodeURIComponent(path), null, function (subData) {
                        pNode.subNodes = subData[0];
                        arrangeNavBar(arr);
                        renderItems(pNode.subNodes, path, mainPanel[0], path);
                        data.currentPath = path;
                    });
                } else {
                    arrangeNavBar(arr);
                    renderItems(pNode.subNodes, path, mainPanel[0], path);
                    data.currentPath = path;
                }

                if (arr.length == 1) {
                    upButton.addClass("hidden");
                    caption.text("");
                } else {
                    upButton.removeClass("hidden");
                    caption.text(pNode.Title);
                }
            }
        }

        navBar.on("click", "li[data-toggle=nav]", function () {
            var thisKey = $.attr(this, "data-key");

            var nameArray = [];
            var node = this;

            nameArray.push(thisKey);

            while (node = node.previousSibling) {
                if (node.nodeType === 1 && node.nodeName.toUpperCase() == "LI" && $.attr(node, "data-toggle") == "nav") {
                    nameArray.push($.attr(node, "data-key"));
                }
            }

            nameArray.reverse();
            navTo(nameArray);
        });

        mainPanel.on("click", "li[data-toggle=navbutton]", function () {
            var key = $.attr(this, "data-key");
            var group = $.attr(this, "data-group");
            if (key) {
                var arr = data.currentPath.split('.');
                if (group)
                    arr.push(group + ">" + key);
                else
                    arr.push(key);

                navTo(arr);
            }

            return false;
        }).on("click", "li[data-toggle=link]", function (e) {
            var url = $.attr(this, 'data-url'), fe = $.attr(this, 'data-feature'), parts, i = 0, sized = false, feature;
            if (fe) {
                parts = fe.split(",");  //高度，宽度，左边位置，顶端位置，大小可变,带滚动条，显示地址栏，显示状态栏，显示菜单栏，标题栏，剧院模式，全屏
                feature = [];
                if (parts.length > 2) {
                    w = parseInt(parts[i++]) || 0, h = parseInt(parts[i++]) || 0, left = (window.screen.width - w) / 2, top = (window.screen.height - h) / 2;

                    if (w > 0) { feature.push("width=" + w); sized = true; };
                    if (h > 0) { feature.push("height=" + h); sized = true; }

                    if (!isNaN(l = parseInt(parts[i++])))
                        feature.push("left=" + l);
                    else if (sized && isNaN(left) == false)
                        feature.push("left=" + left);

                    if (!isNaN(t = parseInt(parts[i++])))
                        feature.push("top=" + t);
                    else if (sized && isNaN(top) == false)
                        feature.push("top=" + top);

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("resizable=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("scrollbars=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("location=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("status=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("menubar=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("titlebar=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("channelmode=" + temp);
                    }

                    if (i + 1 < parts.length && (temp = parts[i++]).length > 0) {
                        feature.push("fullscreen=" + temp);
                    }
                }

                window.open(url, '', feature.join(", "));
            } else {
                window.open(url, '', defaultFeature);
                //                window.open(this.href, ' ', 'height=600, width=800, top=0,left=0, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');
            }


            e.preventDefault();
            return false;
        }); ;

        upButton.on("click", function () {
            var arr = data.currentPath.split('.');
            if (arr.length > 1) {
                arr.splice(arr.length - 1, 1);
            }

            navTo(arr);
        });

        $('<li data-key="Root" data-toggle="nav"><a href="javascript:void(0);">' + $("li:last", navBar).remove().text() + '</a> </li>').appendTo(navBar);

        renderItems(data.subNodes, "Root", mainPanel[0]);
    }

    $('[data-buildin-function=categorylink]').each(function () {
        config(this);
    });

});
