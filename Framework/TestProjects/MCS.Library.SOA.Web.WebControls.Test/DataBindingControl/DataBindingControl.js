/****************************************************************************************/
/* BaseCreator                                                                          */
/****************************************************************************************/
function baseControlCreator() {

    var my = {};

    my.createEditor = function (container, executed, dataBindingControl) {
    };

    my.optionsToEditor = function (editor, options, executed, dataBindingControl) {
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
    };

    my.readOnlyToEditor = function (editor, readOnly, executed, dataBindingControl) {
        $(editor).attr("disabled", readOnly);
    };

    my.visibleToEditor = function (editor, visible, dataBindingItem, dataBindingControl) {
        if (visible) {
            $(editor).show();
        } else {
            $(editor).hide();
        }
    };

    my.clear = function (container, editor, dataBindingItem, dataBindingControl) {
        editor.removeNode(true);
    };

    my.supportedEditMode = {};

    return my;
};

/****************************************************************************************/
/* TextboxCreator                                                                       */
/****************************************************************************************/
function textboxCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var editor,
            dataBindingItem = executed.dataBindingItem;
        if (dataBindingItem.template && dataBindingItem.template != '') {
            editor = $('#' + dataBindingItem.template).clone()[0];
        } else {
            editor = $('<input type="' + dataBindingItem.EditMode + '" />')[0];
        }
        $(editor).change(Function.createDelegate(executed, _textboxChange));
        $(editor).appendTo(container);

        dataBindingControl._raiseCellCreatedEditor(editor, container, dataBindingItem);

        return editor;
    };

    _textboxChange = function () {
        this.data(this.editor.value);
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {

        editor.value = data;
    };

    my.supportedEditMode = { 'text': true, 'password': true };

    return my;
};
/****************************************************************************************/
/* CheckboxCreator                                                                      */
/****************************************************************************************/
function checkboxCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var editor = $('<span></span>')[0];
        $(editor).appendTo(container);

        var multiSelect = true;
        if (executed.dataBindingItem.Attribute && executed.dataBindingItem.Attribute != 'undefined') {
            multiSelect = executed.dataBindingItem.Attribute.MultiSelect;
        }

        editor.multiSelect = multiSelect;

        return editor;
    };

    my.optionsToEditor = function (editor, options, executed, dataBindingControl) {

        if (dataBindingControl.isArray(options)) {
            $(editor).html('');

            editor.branchEditors = editor.branchEditors || [];

            for (var i in options) {
                var option = options[i];
                var text = executed.dataBindingItem.OptionText ? option[executed.dataBindingItem.OptionText] : option.toString();
                var value = executed.dataBindingItem.OptionValue ? option[executed.dataBindingItem.OptionValue] : option;

                var spanValue = $('<span></span>').appendTo(editor);
                var branch = $('<input type="checkbox">')
                                .attr('name', executed.dataBindingItem.ContainerID)
                                .val(value)
                                .appendTo(spanValue);

                branch[0].executed = executed;
                $(branch).change(_checkboxValueChange);

                editor.branchEditors.push(branch[0]);
                var spanText = $('<span></span>').text(text).appendTo(editor);
            }
        }
    };

    _checkboxValueChange = function () {

        if (this.executed.editor.multiSelect) {
            if (this.checked) {
                this.executed.data.push(this.value);
            } else {
                this.executed.data.remove(this.value);
            }
        } else {
            this.checked ? this.executed.data(this.value) : this.executed.data(null);
        }
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
        editor.branchEditors = editor.branchEditors || [];
        if (editor.multiSelect) {
            for (var i = 0; i < editor.branchEditors.length; i++) {
                editor.branchEditors[i].checked = false;
            }

            for (var j in data) {
                var isExit = false;
                var val = typeof data[j] == 'function' ? data[j]() : data[j];

                for (var i = 0; i < editor.branchEditors.length; i++) {
                    if (editor.branchEditors[i].value === val) {
                        editor.branchEditors[i].checked = true;
                        isExit = true;
                        break;
                    }
                }
                if (!isExit) {
                    executed.data.remove(val);
                }
            }
        }
        else {
            for (var i = 0; i < editor.branchEditors.length; i++) {
                editor.branchEditors[i].checked = false;

                if (editor.branchEditors[i].value === data) {
                    editor.branchEditors[i].checked = true;
                }
            }
        }
    };

    my.readOnlyToEditor = function (editor, readdOnly, executed, dataBindingControl) {
        for (var i = 0; i < editor.branchEditors.length; i++) {
            $(editor.branchEditors[i]).attr("disabled", readdOnly)
        }
    };

    my.supportedEditMode = { 'checkbox': true };

    return my;
};
/****************************************************************************************/
/* RadioCreator                                                                         */
/****************************************************************************************/
function radioCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var editor = $('<span></span>')[0];
        $(editor).appendTo(container);
        return editor;
    };

    my.optionsToEditor = function (editor, options, executed, dataBindingControl) {

        if (dataBindingControl.isArray(options)) {
            $(editor).html('');

            editor.branchEditors = editor.branchEditors || [];

            for (var i in options) {
                var option = options[i];
                var text = executed.dataBindingItem.OptionText ? option[executed.dataBindingItem.OptionText] : option.toString();
                var value = executed.dataBindingItem.OptionValue ? option[executed.dataBindingItem.OptionValue] : option;

                var spanValue = $('<span></span>').appendTo(editor);
                var branch = $('<input type="radio">')
                                .attr('name', executed.dataBindingItem.ContainerID)
                                .attr('text', text)
                                .val(value)
                                .appendTo(spanValue);
                editor.branchEditors.push(branch[0]);

                $(branch).change(Function.createDelegate(executed, _radioValueChange));

                var spanText = $('<span></span>').text(text).appendTo(editor);
            }
        }
    };

    _radioValueChange = function () {

        var value,
            text;
        for (var i in this.editor.branchEditors) {
            if (this.editor.branchEditors[i].checked) {
                value = this.editor.branchEditors[i].value;
                text = this.editor.branchEditors[i].text;
                break;
            }
        }
        if (value) {
            this.data(value);

            if (this.dataBindingItem.Attribute && this.dataBindingItem.Attribute.TextDataPropertyName) {

                this.viewModel[this.dataBindingItem.Attribute.TextDataPropertyName](text);
            }
        }
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
        var isExit = false;
        editor.branchEditors = editor.branchEditors || [];

        for (var i = 0; i < editor.branchEditors.length; i++) {
            if (editor.branchEditors[i].value === data) {
                isExit = true;
                editor.branchEditors[i].checked = true;
                break;
            }
        }

        if (!isExit) {
            for (var i in editor.branchEditors) {
                var value = null;
                if (editor.branchEditors[i].checked) {
                    value = editor.branchEditors[i].value;
                    break;
                }
                executed.data(value);
            }
        }
    };

    my.supportedEditMode = { 'radio': true };

    return my;
};

/****************************************************************************************/
/* SelectCreator                                                                        */
/****************************************************************************************/
function selectCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var editor,
            dataBindingItem = executed.dataBindingItem;

        if (dataBindingItem.TemplateControlID && dataBindingItem.TemplateControlID != '') {
            editor = $('#' + dataBindingItem.TemplateControlID).clone()[0];
        } else {
            editor = $('<select></select>')[0];
        }

        $(editor).change(Function.createDelegate(executed, _selectDataValueChange));
        $(editor).appendTo(container);
        //抛出事件
        dataBindingControl._raiseCellCreatedEditor(container, editor, dataBindingItem);

        return editor;
    };

    my.optionsToEditor = function (editor, options, executed, dataBindingControl) {

        if (dataBindingControl.isArray(options)) {
            $(editor).html('');

            for (var i in options) {
                var option = options[i];
                var text = executed.dataBindingItem.OptionText ? option[executed.dataBindingItem.OptionText] : option;
                var value = executed.dataBindingItem.OptionValue ? option[executed.dataBindingItem.OptionValue] : option;

                var branch = $('<option></option>')
                    .text(text)
                    .val(value)
                    .appendTo(editor);
            }
        }

    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
        var isExit = false;
        for (var i = 0; i < editor.options.length; i++) {
            if (editor.options[i].value === data) {
                isExit = true;
                break;
            }
        }

        if (isExit) {
            $(editor).val(data);
        } else {
            if (editor.selectedIndex > -1) {
                executed.data(editor.options[editor.selectedIndex].value);

                if (executed.dataBindingItem.Attribute && executed.dataBindingItem.Attribute.TextDataPropertyName) {
                    executed.viewModel[executed.dataBindingItem.Attribute.TextDataPropertyName](_findDropDownListOptionText(executed.editor))
                }
            } else {
                executed.data(null);
            }
        }
    };

    _selectDataValueChange = function () {
        this.data(this.editor.value);

        if (this.dataBindingItem.Attribute && this.dataBindingItem.Attribute.TextDataPropertyName) {

            this.viewModel[this.dataBindingItem.Attribute.TextDataPropertyName](_findDropDownListOptionText(this.editor))
        }
    };

    _findDropDownListOptionText = function (select) {
        var result = null;

        if (select.tagName == "SELECT") {
            for (var i = 0; i < select.options.length; i++) {
                var opt = select.options.item(i);

                if (opt.value == select.value) {
                    result = opt;
                    break;
                }
            }
        }

        return result.innerText;
    };

    my.supportedEditMode = { 'select': true };

    return my;
};

/****************************************************************************************/
/* SpanCreator                                                                          */
/****************************************************************************************/
function spanCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {
        var editor = $('<span></span>')[0];
        $(editor).appendTo(container);
        return editor;
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
        $(editor).text(data);
    };

    my.supportedEditMode = { 'span': true };

    return my;
};


/* DeluxeControl                                                                        */
/* MCS.Library.SOA.Web.WebControls                                                      */
/****************************************************************************************/
/* DeluxeCalendarCreator                                                                */
/****************************************************************************************/
function deluxeCalendarCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var template = $find(executed.dataBindingItem.TemplateControlID);
        var editor = template.cloneAndAppendToContainer(container);

        editor.add_clientValueChanged(Function.createDelegate(executed, _calendarDataValueChange));
        return editor;
    };

    _calendarDataValueChange = function () {
        this.data(this.editor.get_value());
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
        editor.set_value(data);
    };

    my.supportedEditMode = { 'mcs:calendar': true };

    return my;
};

/****************************************************************************************/
/* CommonAutoCompleteWithSelectorCreator                                                */
/****************************************************************************************/
function commonAutoCompleteWithSelectorCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var template = $find(executed.dataBindingItem.TemplateControlID);
        var editor = template.cloneAndAppendToContainer(container);

        editor.add_selectedDataChanged(Function.createDelegate(executed, _commonAutoCompleteWithSelectorValueChange));

        return editor;
    };

    _commonAutoCompleteWithSelectorValueChange = function (value) {

        if (value[0]) {
            var code = value[0][this.dataBindingItem.OptionValue];
            var text = value[0][this.dataBindingItem.OptionText];

            this.viewModel[this.dataBindingItem.Attribute.TextDataPropertyName](text);
            this.data(code);
        }
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {

        var selectedData = [];


        var obj = {};
        obj.__type = executed.dataBindingItem.Attribute.ObjectType;
        obj[executed.dataBindingItem.OptionValue] = data;
        obj[executed.dataBindingItem.OptionText] = executed.viewModel[executed.dataBindingItem.Attribute.TextDataPropertyName]();

        selectedData.push(obj);

        editor.set_selectedData(selectedData);
        editor.dataBind();
    };

    my.supportedEditMode = { 'mcs:commonautocompletewithselectorcreator': true };

    return my;
};

/* MyControl                                                                            */
/****************************************************************************************/
/* DeluxeMoneyInputCreator                                                              */
/****************************************************************************************/
function deluxeMoneyInputControlCreator() {

    var my = baseControlCreator();

    my.createEditor = function (container, executed, dataBindingControl) {

        var editor,
            dataBindingItem = executed.dataBindingItem;

        if (dataBindingItem.TemplateControlID && dataBindingItem.TemplateControlID != '') {
            editor = $('#' + dataBindingItem.TemplateControlID).clone()[0];
        } else {
            editor = $('<input type="text" />')[0];
        }

        $(editor).change(Function.createDelegate(executed, _deluxeMoneyInputValueChange));

        //增加金额输入的特殊设置
        $(editor).css('text-align', 'right');
        $(editor).css('padding-right', '2px');

        //焦点全选
        $(editor).focus(function () {
            $(this).select();
        });

        $(editor).appendTo(container);
        //抛出事件
        dataBindingControl._raiseCellCreatedEditor(container, editor, dataBindingItem);

        return editor;
    };

    my.dataToEditor = function (editor, data, executed, dataBindingControl) {
        $(editor).val(data);
    };

    _deluxeMoneyInputValueChange = function () {

        if (this.editor.value != '') {
            var num = this.editor.value + "";
            num = num.replace(new RegExp(",", "g"), "");

            if (!/^(-)?[0-9]+(.[0-9]{0,3})?$/.test(num)) {
                this.editor.value = '';
                this.data(0);
                alert('输入金额不正确');
            } else {
                this.data(Number(num));
            }
        }
        else {
            this.data(0);
        }
    };

    my.supportedEditMode = { 'my:moneyinput': true };

    return my;
};


/****************************************************************************************/
/* DataBindingControl                                                                   */
/****************************************************************************************/
var dataBindingControl = function () {
    var my = {},
        _controlCreators = [],
        _data,
        _dataBindingItems = [],
        _executedDataBindingItems = [],
        _count = 0;
    /*----------------------------------------------------------------------------------*/
    _dependencyDetection = (function () {
        var _frames = [];
        return {
            begin: function (ret) {
                _frames.push(ret);
            },
            end: function () {
                _frames.pop();
            },
            collect: function (self) {
                if (_frames.length > 0) {
                    self.list = self.list || [];
                    var fn = _frames[_frames.length - 1];
                    if (fn === self || _indexOf(self.list, fn) >= 0)
                        return;
                    self.list.push(fn);
                }
            }
        };
    })();

    _notifyUpdate = function (observable) {
        var list = observable.list;
        if (list && my.isArray(list)) {
            for (var i = 0, el; el = list[i++]; ) {
                delete el.cache; //清除缓存
                el();
            }
        }
    };

    _indexOf = function (data, o, from) {
        var len = data.length;
        from = from || 0;
        from += (from < 0) ? len : 0;
        for (; from < len; ++from) {
            if (data[from] === o) {
                return from;
            }
        }
        return -1;
    };

    my.isArray = function (v) {

        if (!v) return false;
        return Object.prototype.toString.call(v) == '[object Array]';
    };

    function internalObservable(old, obj, scope, isComputed) {
        var cur,
            getter = obj ? obj.getter : null,
            setter = obj ? obj.setter : null;

        function ret(neo) {
            var set; //判定是读方法还是写方法
            if (arguments.length) { //setter
                neo = typeof setter === "function" ? setter.apply(scope, arguments) : neo;
                ret.cache = neo;
                set = true;
            } else {  //getter
                if (typeof getter === "function") {
                    _dependencyDetection.begin(ret); //只有computed才在依赖链中暴露自身
                    if ("cache" in ret) {
                        neo = ret.cache; //从缓存中读取,防止递归
                    } else {
                        neo = getter.call(scope);
                        ret.cache = neo; //保存到缓存
                    }
                    _dependencyDetection.end()
                } else {
                    neo = cur
                }
                _dependencyDetection.collect(ret)//将暴露到依赖链的computed放到自己的通知列表中
            }

            if (!neo && typeof neo == 'number' && neo != 0) {
                neo = null;
            }

            if (cur !== neo) {
                cur = neo;
                _notifyUpdate(ret);
            }
            return set ? ret : cur
        }
        if (isComputed == true) {
            ret(); //必须先执行一次
        } else {
            cur = old; //将上一次的传参保存到cur中,ret与它构成闭包
            ret(old); //必须先执行一次
        }

        ret.subscribe = function (fn, scope) {
            ret.list = ret.list || [];

            if (fn === ret || _indexOf(ret.list, fn) >= 0)
                return;
            ret.list.push(Function.createDelegate(scope, fn));
        };

        ret.isObservable = true;

        return ret
    }

    my.observable = function (value) {
        return internalObservable(value);
    };

    my.computed = function (obj, scope) {
        var args,
            result;

        if (typeof obj == "function") {
            args = {
                getter: obj,
                scope: scope
            };
        } else if (typeof obj == "object" && obj && obj.getter) {
            args = obj;
            scope = obj.scope;
        }

        result = internalObservable(null, args, scope, true);

        return result;
    };

    my.observableArray = function (array) {
        var internalArray = [],
            result;

        if (this.isArray(array)) {
            for (var i in array) {
                internalArray.push(this.observable(array[i]));
            }
        }

        result = this.observable(internalArray);

        result.push = function (value) {
            var result;

            if (my.isArray(value)) {
                result = [];
                for (var i in value) {
                    var temp = my.observable(value[i]);
                    result.push(temp);
                    internalArray.push(temp);
                }

            } else {
                result = my.observable(value);
                internalArray.push(result);
            }
            _notifyUpdate(this);
            return result
        };

        result.remove = function (value) {

            var index = -1;
            for (var i = 0; i < internalArray.length; i++) {
                var val = internalArray[i];

                if (value === val || value === val()) {
                    index = i;
                    break;
                }
            }

            if (index > -1) {
                internalArray.splice(index, 1);
                _notifyUpdate(this);
            }
        };

        result.removeAll = function () {
            internalArray.length = 0;
            _notifyUpdate(this);
        };

        return result;
    };

    my.changeToModel = function (value) {

        if (typeof value != 'object' || value instanceof Date || !value) {
            return (typeof value == 'function' && value.isObservable) ? this.changeToModel(value()) : value;
        }

        var result = this.isArray(value) ? [] : {};

        for (var i in value) {
            result[i] = this.changeToModel(value[i]);
        }

        return result;
    };

    my.changeToViewModel = function (value) {
        var result = {};

        for (var i in value) {
            var item = value[i];

            switch (typeof item) {
                case 'boolean':
                case 'undefined':
                case 'number':
                case 'string':
                    result[i] = this.observable(item);
                    break;
                case 'object':
                    if (this.isArray(item)) {
                        result[i] = this.observableArray(item);
                    }
                    else {
                        result[i] = this.observable(item);
                    }
                    break;
                case 'function':
                    result[i] = item;
                    break;
                default:
                    break;
            }
        }

        return result;
    };
    /*----------------------------------------------------------------------------------*/
    my.onCreatedEditor = null;
    /*----------------------------------------------------------------------------------*/
    my.set_data = function (value) {
        _data = value;
    };

    my.set_dataBindingItems = function (value) {
        _dataBindingItems = value || [];
    };

    my.get_dataBindingItems = function () {
        return _dataBindingItems;
    };

    my.clearAll = function () {
        for (var i in _executedDataBindingItems) {           //清除旧的控件

            var executed = _executedDataBindingItems[i];

            if (executed) {
                executed.controlCreator.clear(executed.container, executed.editor, executed, this);
            }

            delete _executedDataBindingItems[i];
        }
    };

    my.dataBind = function () {

        this.clearAll();
        this.dataBindByItems(_data, _dataBindingItems);
    };

    my.dataBindByItems = function (viewModel, dataBindingItems) {

        if (!this.isArray(dataBindingItems)) {
            dataBindingItems = [dataBindingItems];
        }

        for (var i in dataBindingItems) {
            var item = dataBindingItems[i];
            item.Container = item.Container ? item.Container : document.getElementById(item.ContainerID);
            item.Container.id = (item.ContainerID && item.ContainerID != '') ? item.ContainerID : 'dataBindingControl_' + _count++;

            var executed = _executedDataBindingItems[item.Container.id];
            if (executed) {
                executed.controlCreator.clear(executed.container, executed.editor, executed, this);
                delete _executedDataBindingItems[item.Container.id];
            }

            //开始新的执行
            executed = {};
            executed.dataBindingControl = this;
            executed.dataBindingItem = item;
            executed.data = viewModel[item.DataPropertyName];
            executed.viewModel = viewModel;
            executed.controlCreator = loadControlCreator(item.EditMode);
            executed.container = item.Container;
            _executedDataBindingItems[item.Container.id] = executed;

            //执行
            executed.editor = executed.controlCreator.createEditor(executed.container, executed, this);

            if (this.isArray(item.Options)) {
                executed.options = this.observable(item.Options);
            } else if (typeof item.Options == 'function') {
                executed.options = this.computed(item.Options, viewModel);
                executed.options.subscribe(raiseOptionsToEditor, executed);
            } else {
                executed.options = void 0;
            }

            raiseOptionsToEditor.call(executed);

            if (typeof item.ReadOnly == 'boolean') {
                executed.readOnly = this.observable(item.ReadOnly);
            } else if (typeof item.ReadOnly == 'function') {
                executed.readOnly = this.computed(item.ReadOnly, viewModel);
                executed.readOnly.subscribe(raiseReadOnlyToEditor, executed);
            } else {
                executed.readOnly = this.observable(false);
            }

            raiseReadOnlyToEditor.call(executed);

            if (typeof item.Visible == 'boolean') {
                executed.visible = this.observable(item.Visible);
            } else if (typeof item.Visible == 'function') {
                executed.visible = this.computed(item.Visible, viewModel);
                executed.visible.subscribe(raiseVisibleToEditor, executed);
            } else {
                executed.visible = this.observable(true);
            }

            raiseVisibleToEditor.call(executed);
        }
    };

    function raiseReadOnlyToEditor() {
        this.controlCreator.readOnlyToEditor(this.editor, this.readOnly(), this, this.dataBindingControl)
    }

    function raiseVisibleToEditor() {
        this.controlCreator.visibleToEditor(this.editor, this.visible(), this, this.dataBindingControl)
    }

    function raiseOptionsToEditor() {

        if (this.options) {
            this.controlCreator.optionsToEditor(this.editor, this.options(), this, this.dataBindingControl)
        }
        raiseDataToEditor.call(this);

        this.data.subscribe(raiseDataToEditor, this);
    }

    function raiseDataToEditor() {
        var showValueTobeChange = this.data();

        if (this.dataBindingItem.Format && this.dataBindingItem.Format != '') {
            showValueTobeChange = String.format(this.dataBindingItem.Format, this.data());
        }
        this.controlCreator.dataToEditor(this.editor, showValueTobeChange, this, this.dataBindingControl)
    };

    function loadControlCreator(editMode) {
        for (var i in _controlCreators) {
            if (_controlCreators[i].supportedEditMode[editMode.toLowerCase()]) {
                return _controlCreators[i];
            }
        }
    };

    my._raiseCellCreatedEditor = function () {
    };
    /*----------------------------------------------------------------------------------*/
    my.registerCreator = function (creator) {
        _controlCreators.push(creator);
    };

    return my;
} ();

dataBindingControl.registerCreator(textboxCreator());
dataBindingControl.registerCreator(checkboxCreator());
dataBindingControl.registerCreator(radioCreator());
dataBindingControl.registerCreator(selectCreator());
dataBindingControl.registerCreator(spanCreator());
dataBindingControl.registerCreator(deluxeCalendarCreator());
dataBindingControl.registerCreator(commonAutoCompleteWithSelectorCreator());
dataBindingControl.registerCreator(deluxeMoneyInputControlCreator());