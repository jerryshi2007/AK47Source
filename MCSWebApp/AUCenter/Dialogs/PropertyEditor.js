/// <reference path="../Scripts/MicrosoftAjax.debug.js" />
/// <reference path="../Scripts/jquery-1.4.1-vsdoc.js" />
/// <reference path="../Scripts/pc.js" />



$(function () {
    var getPropertyIndex = function (key) {
        var result = -1;
        for (var i = initProperties.length - 1; i >= 0; i--) {
            if (initProperties[i].Name == key) {
                result = i;
                break;
            }
        }

        return result;
    }

    function PropertyItem(name) {
        this.Name = name;
        this.DisplayName = name;
        this.DataType = 18;

        this.Tab = "";
        this.SnapshotMode = 0;
        this.Category = "";
        this.Description = "";
        this.DefaultValue = "";
        this.ReadOnly = false;
        this.EditorKey = "";
        this.EditorParams = null;
        this.MaxLength = 0;
        this.IsRequired = false;
        this.ShowTitle = false;
        this.Visible = true;
        this.PersisterKey = null;
        this.AllowOverride = false;
    }

    PropertyItem.PropertyDataType = {
        DataObject: 1,
        Boolean: 3,
        Integer: 9,
        Decimal: 15,
        DateTime: 16,
        String: 18,
        Enum: 20
    };

    PropertyItem.metaData = {
        DisplayName: { type: 'String', length: 255, required: true, description: "属性的显示名" },
        DataType: { type: 'Enum', required: true, description: "数据类型", enums: PropertyItem.PropertyDataType },
        Tab: { type: 'StringList', length: 50, required: true, description: "此属性的标签页的名称", dataSource: tabNames },
        Category: { type: 'String', length: 64, required: true, description: "分类" },
        Description: { type: 'String', length: 255, description: "属性的描述" },
        DefaultValue: { type: 'String', length: 50, description: "属性的初始值（根据字符串值）" },
        ReadOnly: { type: 'Bool', length: 50, required: true, description: "属性是否应该只读" },
        EditorKey: { type: 'String', length: 255, description: "与此属性关联的编辑器的键" },
        EditorParams: { type: 'String', length: 1024, required: false, description: "与此属性关联的编辑器的参数" },
        MaxLength: { type: 'Integer', length: 4, required: true, description: "最大的长度" },
        IsRequired: { type: 'Bool', length: 10, required: true, description: "表示此属性是否必填" },
        ShowTitle: { type: 'Bool', length: 10, required: true, description: "表示是否显示标题" },
        Visible: { type: 'Bool', length: 10, required: true, description: "此属性的可见性" },
        PersisterKey: { type: 'String', length: 255, required: false, description: "当持久化关联数据时，属性持久化器的键" }
    };

    function Adapter() {
        this.createEditor = function (container, feature, name, item) {
        };

        this.createText = function (feature, name, item) {
        };

        this.collectProperty = function (editor, name, feature, obj) {
            return true;
        }
    }

    StringAdapter = function () {
    }

    StringAdapter.prototype = new Adapter();

    StringAdapter.prototype.createEditor = function (container, feature, name, item) {
        var input = document.createElement("input");
        input.type = "text";
        container.appendChild(input);

        input.className = "editor";
        $.attr(input, "data-name", name);
        $.attr(input, "data-type", "String");
        if (feature.length)
            input.maxLength = feature.length;

        if (item[name])
            input.value = item[name];
    }

    StringAdapter.prototype.createText = function (feature, name, item) {
        return item[name];
    }

    StringAdapter.prototype.collectProperty = function (editor, name, feature, obj) {
        var val = editor.value, valid = true;
        if ((val.length == 0 && feature.required) || (feature.length && val.length > feature.length)) {
            valid = false;
            $pc.addClass(editor, "input-error");
        } else {
            obj[name] = val;
            $pc.removeClass(editor, "input-error");
        }

        return valid;
    }

    BoolAdapter = function () {

    }

    BoolAdapter.prototype = new Adapter();
    BoolAdapter.prototype.createEditor = function (container, feature, name, item) {
        var input = document.createElement("input");
        input.type = "checkbox";
        container.appendChild(input);
        input.className = "editor";
        $.attr(input, "data-name", name);
        $.attr(input, "data-type", "Bool");
        if (feature.length)
            input.maxLength = feature.length;

        input.checked = !!item[name];
    }

    BoolAdapter.prototype.createText = function (feature, name, item) {
        return item[name] ? "是" : "否";
    }

    BoolAdapter.prototype.collectProperty = function (editor, name, feature, obj) {
        obj[name] = editor.checked;
        return true;
    }

    EnumAdapter = function () {
    }

    EnumAdapter.prototype = new Adapter();

    EnumAdapter.prototype.createEditor = function (container, feature, name, item) {
        var input = document.createElement("select"), en, option;
        container.appendChild(input);
        input.className = "editor";
        $.attr(input, "data-name", name);
        $.attr(input, "data-type", "Enum");

        for (en in feature.enums) {
            option = document.createElement("option");
            option.text = en;
            option.value = feature.enums[en];
            input.add(option);
        }

        if (item[name])
            input.value = item[name];
    }

    EnumAdapter.prototype.createText = function (feature, name, item) {
        for (var en in feature.enums) {
            if (feature[en] = item[name])
                return en;
        }

        return "未知";
    }

    EnumAdapter.prototype.collectProperty = function (editor, name, feature, obj) {
        var val = editor.value, valid = true;
        obj[name] = parseInt(val);
        $pc.removeClass(editor, "input-error");
        return true;
    }

    StringListAdapter = function () {
    }

    StringListAdapter.prototype = new Adapter();

    StringListAdapter.prototype.createEditor = function (container, feature, name, item) {
        var input = document.createElement("select"), en, option;
        container.appendChild(input);
        input.className = "editor";
        $.attr(input, "data-name", name);
        $.attr(input, "data-type", "Enum");
        var ds = feature.dataSource;
        var len = ds.length;
        for (var i = 0; i < len; i++) {
            option = document.createElement("option");
            option.text = ds[i];
            option.value = ds[i];
            input.add(option);

        }

        if (item[name])
            input.value = item[name];
    }

    StringListAdapter.prototype.createText = function (feature, name, item) {

        return item[name];
    }

    StringListAdapter.prototype.collectProperty = function (editor, name, feature, obj) {
        var val = editor.value, valid = true;
        obj[name] = val;
        $pc.removeClass(editor, "input-error");
        return true;
    }

    IntegerAdapter = function () {

    }

    IntegerAdapter.prototype = new Adapter();

    IntegerAdapter.prototype.createEditor = function (container, feature, name, item) {
        var input = document.createElement("input");
        input.type = "text";
        container.appendChild(input);

        input.className = "editor";
        $.attr(input, "data-name", name);
        $.attr(input, "data-type", "Integer");
        if (feature.length)
            input.maxLength = feature.length;
        input.value = item[name];
    }

    IntegerAdapter.prototype.createText = function (feature, name, item) {
        return item[name];
    }

    IntegerAdapter.prototype.collectProperty = function (editor, name, feature, obj) {
        var val = editor.value, valid = val.match(/^\d+$/);
        if (!valid) {
            $pc.addClass(editor, "input-error");
        } else if ((val.length == 0 && feature.required) || (feature.length && val.length > feature.length)) {
            valid = false;
            $pc.addClass(editor, "input-error");
        } else {
            obj[name] = parseInt(val);
            $pc.removeClass(editor, "input-error");
        }

        return valid;
    }

    PropertyItem.TypeAdapters = {};
    PropertyItem.TypeAdapters.String = new StringAdapter();
    PropertyItem.TypeAdapters.Bool = new BoolAdapter();
    PropertyItem.TypeAdapters.Enum = new EnumAdapter();
    PropertyItem.TypeAdapters.Integer = new IntegerAdapter();
    PropertyItem.TypeAdapters.StringList = new StringListAdapter();


    function renderItem(item, container, readOnly, opened) {
        var head, body, headIcon, headTitle, pptList;
        if (opened)
            container.className = "property-list-item open";
        else
            container.className = "property-list-item";
        $.attr(container, "data-key", item.Name);
        head = document.createElement("div");
        head.className = "property-list-item-head";
        container.appendChild(head);
        headIcon = document.createElement("i");
        headIcon.className = "icon-toggle";
        head.appendChild(headIcon);
        headTitle = document.createElement("span");
        headTitle.className = "caption";
        head.appendChild(headTitle);
        headTitle.appendChild(document.createTextNode(item.Name));
        body = document.createElement("div");
        body.className = "property-list-item-body";
        container.appendChild(body);
        pptList = document.createElement("ul");
        pptList.className = "property-item-list";
        body.appendChild(pptList);
        renderProperties(item, pptList, readOnly);
    }

    function renderProperties(item, container, readOnly) {
        var li, label, spanName, spanHint, p, i, editor, spanVal;

        if (!readOnly) {
            li = document.createElement("li");
            li.className = "property-item-list-item";
            container.appendChild(li);
            editor = document.createElement("input");
            editor.type = "button";
            editor.value = "删除此属性";
            editor.className = "pc-button button-delete";
            li.appendChild(editor);
        }

        for (var m in PropertyItem.metaData) {
            p = PropertyItem.metaData[m];
            li = document.createElement("li"); label = document.createElement("label"); spanName = document.createElement("span"); spanHint = document.createElement("span");
            li.className = "property-item-list-item";
            $.attr(li, "data-name", m);
            label.className = "property-label";
            spanName.className = "property-label-name";
            spanHint.className = "property-hint";
            container.appendChild(li);
            li.appendChild(label);
            label.appendChild(spanName);
            li.appendChild(spanHint);

            if (!readOnly) {
                editor = PropertyItem.TypeAdapters[p.type].createEditor(label, p, m, item);
            } else {
                spanVal = document.createElement("span");
                label.appendChild(spanVal);
                spanVal.className = "property-value";
                spanVal.appendChild(document.createTextNode(PropertyItem.TypeAdapters[p.type].createText(p, m, item)));
            }

            spanName.appendChild(document.createTextNode(m));
            spanHint.appendChild(document.createTextNode(p.description));
            if (p.required) {
                i = document.createElement("i");
                i.className = "required";
                spanName.appendChild(i);
                i.appendChild(document.createTextNode("*"));
            }
        }
    }

    $(".property-list .property-list-item .property-list-item-head ").live("click", function () {
        $(this).parent().toggleClass("open");
    });

    $(".property-list .property-list-item .property-list-item-body .button-delete").live("click", function () {
        var parent = this.parentNode.parentNode.parentNode.parentNode, key = $.attr(parent, "data-key"), i;
        if (key) {
            i = getPropertyIndex(key);
            if (i >= 0) {
                var ppt = initProperties[i];
                if (confirm("确实要删除属性" + ppt.Name + "?")) {
                    initProperties.splice(i, 1);
                    $(parent).remove();
                }
            }
        }
    });

    $("#btnAddNew").click(function () {
        var txt = $("#txtAddNew").val();
        if (!txt.length) {
            $("#lblAddPrompt").text("名称不能为空");
        } else if (!txt.match((/^[a-z_]\w+$/i))) {
            $("#lblAddPrompt").text("名称中仅可使用英文数字和下划线，且不以数字开头");
        } else if (getPropertyIndex(txt) >= 0) {
            $("#lblAddPrompt").text("已经存在一个同名的属性");
        } else if (Array.indexOf(reservedPropertiesNames, txt) >= 0) {
            $("#lblAddPrompt").text("此属性名为保留名称，请换另一个名称。");
        } else {
            $("#lblAddPrompt").text("");
            var newItem = new PropertyItem(txt);
            initProperties.push(newItem);

            var cont = document.createElement("li");
            document.getElementById("pptList").appendChild(cont);
            renderItem(newItem, cont, false, true);
        }
    });

    $("#btnSave").click(function () {
        var p, li, i = 0, j, ppt, key, obj, name, t, editor, totalHasError = false, hasError = false;
        for (li = document.getElementById("pptList").firstChild; li; li = li.nextSibling) {
            if (li.nodeType === 1 && li.nodeName.toUpperCase() == 'LI') {
                key = $.attr(li, "data-key");
                j = getPropertyIndex(key);
                ppt = j >= 0 ? initProperties[j] : null;
                if (ppt) {
                    hasError = false;
                    $(".editor", li).each(function (num) {
                        name = $.attr(this, "data-name");
                        t = PropertyItem.metaData[name];
                        hasError |= !PropertyItem.TypeAdapters[t.type].collectProperty(this, name, t, ppt);
                    });
                    totalHasError |= hasError;

                    if (hasError) {
                        $pc.addClass(li, "open");
                    }
                }
            }
        }

        if (totalHasError) {
            alert("部分属性未填写正确(检查黄色背景的项)，请修改。");
        } else {
            var json = Sys.Serialization.JavaScriptSerializer.serialize(initProperties);
            document.getElementById("hfPostData").value = json;
            $get("btnSubmit").click();
        }
    });

    (function () {
        var list = document.getElementById("pptList"), li;
        var readOnly = false;
        if (!document.getElementById("btnSave")) {
            readOnly = true;
        }

        for (var k = 0; k < initProperties.length; k++) {
            li = document.createElement("li");
            list.appendChild(li);
            renderItem(initProperties[k], li, readOnly);
        }
    })();
});