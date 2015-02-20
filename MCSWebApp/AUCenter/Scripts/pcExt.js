/// <reference path="pc.js" />

if (typeof ($pc) === 'undefined')
    throw new "必须先加载pc.js";

(function (pcHelper) {

    pcHelper.appRoot = '/MCSWebApp/AUCenter/';

    pcHelper.popups = {
        editProperty: function (obj) {
            //编辑对象基本属性对话框，obj为命令元素，或者对象的ID
            if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
                return false;
            function cbAfterCommit(rst) {
                pcHelper.console.log("对象编辑对话框返回了值：" + rst);
            }
            var key = null;
            if (obj.nodeType && obj.nodeType === 1) //DOM
            {
                key = pcHelper.getAttr(obj, "data-id");
            }
            else if (typeof obj === "string" && rguid.test(obj)) {
                key = obj;
            }
            if (key) {
                if (pcHelper.showDialog(pcHelper.appRoot + "Dialogs/Editor.aspx?id=" + key, '', cbAfterCommit, false, 800, 600, true) === true)
                    return true;
            }
            return false;
        },
        newAdminSchema: function (obj) {
            //新建架构
            if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
                return false;
            function doPop(category) {
                if (typeof category != 'undefined' && category != '') {
                    if (pcHelper.showDialog(pcHelper.appRoot + "Dialogs/Editor.aspx?category=" + category, '', null, false, 800, 600, true) === true)
                        return true;
                }
                return false;
            }

            if (obj.nodeType && obj.nodeType === 1) //DOM，从元素上查找参数
            {
                var category = pcHelper.getAttr(obj, "data-category");
                if (pcHelper.getDisabled(obj)) {
                    return; //该按钮被禁用
                }
                return doPop(category);
            }
            else if (typeof obj === "string")
                return doPop(obj);
        },
        newAdminSchemaRole: function (obj) {
            //新建架构
            if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
                return false;
            function doPop(category) {
                if (typeof category != 'undefined' && category != '') {
                    if (pcHelper.showDialog(pcHelper.appRoot + "Dialogs/Editor.aspx?schemaType=AUSchemaRoles&parentID=" + category, '', null, false, 800, 600, true) === true)
                        return true;
                }
                return false;
            }

            if (obj.nodeType && obj.nodeType === 1) //DOM，从元素上查找参数
            {
                var id = pcHelper.getAttr(obj, "data-id");
                if (pcHelper.getDisabled(obj)) {
                    return; //该按钮被禁用
                }
                return doPop(id);
            }
            else if (typeof obj === "string")
                return doPop(obj);
        }, newAdminUnit : function(elem){
         //新建架构
            if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
                return false;
            function doPop(schemaId,parentId) {
                if (typeof schemaId != 'undefined' && schemaId != '') {
                    if (pcHelper.showDialog(pcHelper.appRoot + "Dialogs/Editor.aspx?auSchemaID=" + schemaId + "&parentID=" + parentId, '', null, false, 800, 600, true) === true)
                        return true;
                }
                return false;
            }

            if (elem.nodeType && elem.nodeType === 1) //DOM，从元素上查找参数
            {
                if (pcHelper.getDisabled(elem)) {
                    return false; //该按钮被禁用
                }
                var schemaId = pcHelper.getAttr(elem, "data-schemaId");
                var parentId = pcHelper.getAttr(elem, "data-parentId");

                return doPop(schemaId,parentId);
            }
            else if (typeof obj === "string")
                return doPop(obj);
        },browseUnits:function(schemaId,parentId,keys){
            var extQ = '';
            var extKeys = [];
            if(keys){
                for (var i = keys.length - 1; i >= 0 ; i--) {
                    extKeys.push('exclude='+keys[i]);
                }
                if(extKeys.length)
                {
                    extQ = '&'+ extKeys.join('&');
                }       
            }

           var result = pcHelper.showDialog(pcHelper.appRoot+"TargetSelector.aspx?schemaId="+ schemaId+ "&parentID=" + parentId + extQ, '', null, false, 800, 600, true);
           return result;
        },browseScopes : function (fillIn, scopeType, single, urlParamsArr, excludes) {
            var result = false;
            var fillElem = fillIn;

            if (typeof fillElem === 'string') {
                fillElem = pcHelper.get(fillIn);
            }
            if (fillElem && (fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT")) {
                var url = pcHelper.appRoot + "Dialogs/ScopeItemBrowser.aspx";
                urlParamsArr = urlParamsArr || [];
                if (single) {
                    urlParamsArr.push({ "mode": "single" });
                    //url += "?mode=single";
                }

                var pp = [];
                pp.push("scopeType=" + encodeURIComponent(scopeType));
                for (var i = urlParamsArr.length - 1; i >= 0; i--) {
                    for (var key in urlParamsArr[i]) {
                        pp.push(encodeURIComponent(key) + "=" + encodeURIComponent(urlParamsArr[i][key]));
                    }
                }

                if (pp.length)
                    url += "?" + pp.join("&");

                delete pp;
                pp = null;

                var returnObj = {
                    fillElem: fillElem
                };

                if (excludes) {
                    returnObj.excludes = excludes;
                }

                if (pcHelper.showDialog(url, returnObj, null, false, 800, 600, true) === true) {
                    result = true;
                }
            }

            return result;
        }, pickTime: function (trigger) {
            var url = pcHelper.appRoot + "Dialogs/TimePicker.aspx";
            var success = false;
            var rst = pcHelper.showDialog(url, "", null, false, 300, 300, true);
            if (typeof (rst) != 'undefined' && rst != '') {
                if (typeof (trigger) === 'string')
                    trigger = $get(trigger);
                if (typeof (trigger) !== 'undefined' && trigger.nodeType === 1) {
                    var fillElem;
                    if (trigger.getAttribute) {
                        fillElem = $get(trigger.getAttribute('data-rlctl'));
                    } else {
                        fillElem = $get(trigger['data-rlctl']);
                    }
                    if (fillElem) {
                        if (fillElem.nodeType === 1 && fillElem.nodeName.toUpperCase() === "INPUT") {
                            fillElem.value = rst;
                            success = true;
                        }
                    }

                    fillElem = null;
                }
                trigger = null;
            }
            return success;
        },historyProperty: function (obj, time) {
            //编辑对象基本属性对话框，obj为命令元素，或者对象的ID
            if (typeof (event) !== 'undefined' && event.srcElement && pcHelper.getDisabled(event.srcElement))
                return false;
            function cbAfterCommit(rst) {
                pcHelper.console.log("对象编辑对话框返回了值：" + rst);
            }

            var key = null;

            key = null;
            vtime = time;
            if (obj) {
                if (typeof (obj) === "string") {
                    key = obj;
                } else {
                    if (obj.nodeType && obj.nodeType === 1) //DOM
                    {
                        key = pcHelper.getAttr(obj, "data-id");
                        vtime = pcHelper.getAttr(obj, "data-time");
                    }
                }

                if (key != null && vtime != null) {
                    if (pcHelper.showDialog(pcHelper.appRoot + "Dialogs/Editor.aspx?reserved=1&id=" + key + "&time=" + vtime, '', cbAfterCommit, false, 800, 600, true) === true)
                        return true;
                }
            }

            return false;
        }
    }

    pcHelper.createDelegate = function(inst,fun){
        return function(){ return fun.apply(inst) };
    };

    pcHelper.poast = function(elem){
        this._elem = elem;
        this._clickHandler = null;
        this.init();
        pcHelper.hide(this._elem);
    }

    pcHelper.poast.Info = "au-toast-info";
    pcHelper.poast.Warning = "au-toast-warning";
    pcHelper.poast.ErrorState = "au-toast-error";

    pcHelper.poast.prototype = {
        init:function(){
            this._clickHandler = pcHelper.createDelegate(this,this.onClick);

            pcHelper.bindEvent(this._elem,"click",this._clickHandler);
        },onClick:function(){
            alert("Hello");
        },setPoast:function(txt,typeCss,time,afterHide){
            pcHelper.setText(this._elem,txt);
            pcHelper.show(this._elem);
        }
    }

})($pc);