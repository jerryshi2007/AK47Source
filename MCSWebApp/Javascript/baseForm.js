// -------------------------------------------------
// FileName	:	baseForm.js
// Remark	:	baseForm
// -------------------------------------------------
// VERSION		AUTHOR		     DATE			CONTENT
// 1.0			xuwenzhuo		 20080408		ceate
// 1.0.1        v-weirf          20120702       modify
// -------------------------------------------------

//create $HBPageNS namespace
if (typeof ($HBPageNS) === 'undefined') {
    var ChinaCustoms_Framework_HB_Web_Page = {};
    ChinaCustoms_Framework_HB_Web_Page.classList = new Array();

    //registerClass in a list ,you can get you classinstance from this list
    ChinaCustoms_Framework_HB_Web_Page.registerClass = function (name, classItem) {
        this.classList.push(new Array(name, classItem));
    }

    //get getInstance by name
    ChinaCustoms_Framework_HB_Web_Page.getInstance = function (name) {
        for (var i = 0; i < this.classList.length; i++) {
            if (this.classList[i][0] == name) {
                return this.classList[i][1];
            }
        }
        return null;
    }

    var $HBPageNS = ChinaCustoms_Framework_HB_Web_Page;

    //inittialize all the instances
    (function (win) {
    //20120702 fix Non-IE bug
        var initFun = function () {
            if (typeof (Sys) != 'undefined' && typeof (Sys.Application) != 'undefined') {
                Sys.Application.add_init(function () {
                    for (var i = 0; i < ChinaCustoms_Framework_HB_Web_Page.classList.length; i++) {
                        if ($HBPageNS.classList[i][1].initialize) {
                            $HBPageNS.classList[i][1].initialize();
                        }
                    }
                });
            }
        };

        if (win.addEventListener) {
            win.addEventListener("load", initFun, false); //W3C
        } else if (win.attachEvent) {
            win.attachEvent("onload", initFun); //IE
        } else {
            alert('由于使用不兼容的浏览器，无法为baseform.js中附加初始化代码。');
        }

    })(window);

}


$HBPageNS.SFPage = function () {
    this.navapp = null;
    this.conter = null;
    this.footblank = null;
    this.contItems = null;
}
$HBPageNS.SFPage.prototype =
{
    initialize: function () {
        this.navapp = this.getElementByClass('conNavapp');
        this.conter = this.getElementByClass('conContent');
        this.footblank = this.getElementByClass('conFootblank');
        this.contItems = new Array();
        this.setCon();
        this.setBlank();
        this.setNavApp();
        window.onresize = this.setNavApp;
    },

    getElementByClass: function (className) {
        var el = new Array();
        var _el = document.getElementsByTagName('*');
        for (var i = 0; i < _el.length; i++) {
            if (_el[i].className.toLowerCase() == className.toLowerCase()) {
                el.push(_el[i]);
            }

        }
        return el;
    },
    currBrower: function () {
        var OsObject = '';
        if (navigator.userAgent.indexOf('MSIE') > 0) {
            if (navigator.userAgent.indexOf('MSIE 6.0') > 0) {
                return 'IE6';
            }
            else {
                return 'IE7';
            }
        }
        if (isFirefox = navigator.userAgent.indexOf('Firefox') > 0) {
            return 'FF';
        }
        if (isSafari = navigator.userAgent.indexOf('Safari') > 0) {
            return 'SF';
        }
        if (isCamino = navigator.userAgent.indexOf('Camino') > 0) {
            return 'CI';
        }
        if (isMozilla = navigator.userAgent.indexOf('Gecko') > 0) {
            return 'GK';
        }

    },

    setNavApp: function () {
        var sender = $HBPageNS.getInstance('$HBPageNS.SFPage');
        var _footblank;
        var _navapp;
        var _currBrower;
        if (sender != null) {
            _footblank = sender.footblank;
            _navapp = sender.navapp;
            _currBrower = sender.currBrower();
        }
        else {
            _footblank = this.footblank;
            _navapp = this.navapp;
            _currBrower = this.currBrower();
        }
        if (!_navapp || _navapp.length <= 0) return;
        if (_currBrower != 'IE6') {
            var navBounds = Sys.UI.DomElement.getBounds(_navapp[0]);
            var navHeight = navBounds.height;
            var bodyHeight = document.documentElement.clientHeight;
            _navapp[0].style.top = (bodyHeight - navHeight) + 'px';
        }
    },
    setBlank: function () {
        var sender = $HBPageNS.getInstance('$HBPageNS.SFPage');
        var _footblank;
        var _navapp;
        var _currBrower;
        if (sender != null) {
            _footblank = sender.footblank;
            _navapp = sender.navapp;
            _currBrower = sender.currBrower();
        }
        else {
            _footblank = this.footblank;
            _navapp = this.navapp;
            _currBrower = this.currBrower();
        }
        if (!_navapp || _navapp.length <= 0) return;
        if (_currBrower != 'IE6') {
            if (!_footblank || _footblank.length <= 0) return;
            var navBounds = Sys.UI.DomElement.getBounds(_navapp[0]);
            _footblank[0].style.height = navBounds.height + 'px';
        }
    },
    setCon: function () {
        if (!this.conter || this.conter.length <= 0) return;
        for (var k = 0; k < this.conter.length; k++) {
            for (var l = 0; l < this.conter[k].childNodes.length; l++) {
                if (this.conter[k].childNodes[l].innerHTML)
                    this.contItems.push(this.conter[k].childNodes[l]);
            }
        }
        for (var i = 0; i < this.contItems.length; i = i + 2) {
            this.contItems[i].onclick = function () {
                var _contItems = new Array();
                var sender = $HBPageNS.getInstance('$HBPageNS.SFPage');
                var _conter = sender.conter;
                if (!_conter || _conter.length <= 0) return;

                for (var k = 0; k < _conter.length; k++) {
                    for (l = 0; l < _conter[k].childNodes.length; l++) {
                        if (_conter[k].childNodes[l].innerHTML)
                            _contItems.push(_conter[k].childNodes[l]);
                    }
                }
                for (var j = 0; j < _contItems.length; j = j + 2) {
                    if (_contItems[j] == this) {
                        if (_contItems[j + 1].style.display == '') {
                            _contItems[j + 1].style.display = 'none';
                        }
                        else {
                            _contItems[j + 1].style.display = '';
                        }
                        break;
                    }
                }
            }
            this.contItems[i + 1].style.display = '';
        }
    }
}
$HBPageNS.registerClass('$HBPageNS.SFPage', new $HBPageNS.SFPage());


function refreshDialogArguments() {
    window.dialogArguments.document.forms[0].submit();
    window.close();
}

function onEnterKeyDown(btnID) {
    if (event.keyCode == 13) {
        document.getElementById(btnID).focus();
        document.getElementById(btnID).click();
    }
}