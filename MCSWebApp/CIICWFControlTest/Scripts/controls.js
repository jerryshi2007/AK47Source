(function ($) {
    $.fn.HSR = {
        Controls: {
        },
        Utils:{
        
        }
    };


})(jQuery);


(function ($) {
    $.fn.HSR.Controls.Button = function (selector) {
        var setting = $(selector).attr("ButtonSetting");
        eval(setting);
        var btnInstance = new Object();
        btnInstance.CurrentInstance = $(selector);
        btnInstance.Settings = ButtonSetting;
        btnInstance.Init = function () {
            this.CurrentInstance.click(function () {
                var curSetting = $(this).attr("ButtonSetting");
                eval(curSetting);
                if (ButtonSetting.EnableDialog) {
                    if (!confirm(ButtonSetting.DialogText)) {
                        return false;
                    }
                }
                if (ButtonSetting.ClientClick && ButtonSetting.ClientClick != '') {
                    var progressbar=$.fn.HSR.Controls.ShowProgressBar(100, 20);
                    var handle = eval(ButtonSetting.ClientClick);
                    return handle($(this), progressbar);
                }
            });
        };
        btnInstance.SetEnabled = function (enabled) {
            if (enabled) {
                this.CurrentInstance.removeAttr("disabled");
            }
            else {
                this.CurrentInstance.attr("disabled", "disabled");
            }
        };
        btnInstance.SetStyle = function (style) {
            this.CurrentInstance.removeClass("btn-danger");
            this.CurrentInstance.removeClass("btn-info");
            this.CurrentInstance.removeClass("btn-link");
            this.CurrentInstance.removeClass("btn-primary");
            this.CurrentInstance.removeClass("btn-success");
            this.CurrentInstance.removeClass("btn-warning");
            this.CurrentInstance.removeClass("btn-default");

            switch (style) {
                case $.fn.HSR.Controls.Button.ButtonStyles.Danger:
                    this.CurrentInstance.addClass("btn-danger");
                    break;
                case $.fn.HSR.Controls.Button.ButtonStyles.Info:
                    this.CurrentInstance.addClass("btn-info");
                    break;
                case $.fn.HSR.Controls.Button.ButtonStyles.Link:
                    this.CurrentInstance.addClass("btn-link");
                    break;
                case $.fn.HSR.Controls.Button.ButtonStyles.Primary:
                    this.CurrentInstance.addClass("btn-primary");
                    break;
                case $.fn.HSR.Controls.Button.ButtonStyles.Success:
                    this.CurrentInstance.addClass("btn-success");
                    break;
                case $.fn.HSR.Controls.Button.ButtonStyles.Warning:
                    this.CurrentInstance.addClass("btn-warning");
                    break;
                case $.fn.HSR.Controls.Button.ButtonStyles.Default:
                    this.CurrentInstance.addClass("btn-default");
                    break;
            }
        };
        btnInstance.SetSize=function (size) {
            this.CurrentInstance.removeClass("btn-lg");
            this.CurrentInstance.removeClass("btn-sm");
            this.CurrentInstance.removeClass("btn-xs");
            switch (size)
            {
                case $.fn.HSR.Controls.Button.SizeModes.Large:
                    this.CurrentInstance.addClass("btn-lg");
                    break;
                case $.fn.HSR.Controls.Button.SizeModes.Small:
                    this.CurrentInstance.addClass("btn-sm");
                    break;
                case $.fn.HSR.Controls.Button.SizeModes.XSmall:
                    this.CurrentInstance.addClass("btn-xs");
                    break;
            }
        };
     
        return btnInstance;
    }
    $.fn.HSR.Controls.Button.ButtonStyles = {
        Default : 1,
        Primary : 2,
        Success : 3,
        Info : 4,
        Warning : 5,
        Danger:6,
        Link:7
    };
    $.fn.HSR.Controls.Button.ButtonTypes = {
        Button: 1,
        Submit: 2
    };
    $.fn.HSR.Controls.Button.SizeModes = {
        Default: 1,
        Large: 2,
        Small: 3,
        XSmall: 4
    };
})(jQuery);
(function ($) {
    $.fn.HSR.Controls.DropDowButton = function (selector) {
        var btnInstance = new Object();
        btnInstance.CurrentInstance = $(selector);
        btnInstance.Init = function () {
            this.CurrentInstance.children("ui").children("li").each(function () {
                $(this).click(function () {
                    var itemSetting = eval("itemSetting=" + $(this).attr("DropItemData")+";");
                    if (itemSetting.ClientHandler && itemSetting.ClientHandler != '')
                    {
                        if (!itemSetting.Enabled)
                        {
                            return false;
                        }
                        var handler = eval(itemSetting.ClientHandler);
                        handler($(this), itemSetting.Command, $(this).parent().parent().children("button:first"));
                    }
                });
            });
            this.CurrentInstance.children("button:first").click(function () {
                var setting = eval("setting="+$(this).attr("DropDownButtonData")+";");
                if (setting.EnableDialog) {
                    if (!confirm(setting.DialogText)) {
                        return false;
                    }
                }
                if (setting.ClientClick && setting.ClientClick != '') {
                    var handle = eval(setting.ClientClick);
                    return handle($(this));
                }
            });
        };
        btnInstance.SetEnabled = function (enabled) {
            if (enabled) {
                this.CurrentInstance.children("button:first").removeAttr("disabled");
            }
            else {
                this.CurrentInstance.children("button:first").attr("disabled", "disabled");
            }
        };

        return btnInstance;
    }
    
})(jQuery);
(function ($) {
    $.fn.HSR.Controls.TextBox = function (selector) {
        var txtInstance = new Object();
        txtInstance.CurrentInstance = $(selector);
        txtInstance.Init = function () {
            $("span[tbIdentifier='" + this.CurrentInstance.attr("tbIdentifier") + "']").click(function () {
                if ($(this).attr("scrollType") == 'up') {
                    $.fn.HSR.Controls.TextBox.ChangeNumber($(this), 1);
                }
                else {
                    $.fn.HSR.Controls.TextBox.ChangeNumber($(this), -1);
                }
            });
            this.CurrentInstance.change(function () {
                eval("var currentNumber=" + $(this).attr("TextBoxData") + ";");
                var currentValue = $(this).val();
                if (currentNumber.ReadOnly || !currentNumber.Enabled || !currentNumber.IsNumeric) {
                    return;
                }
                if (currentValue && currentValue != '') {
                    oldValue = parseFloat(currentValue);
                    if (isNaN(oldValue)) {
                        $(this).val('');
                    }
                }
            });
            eval("var setting = " + txtInstance.CurrentInstance.attr("TextBoxData") + ";");
            if (setting && setting.Events) {
                for (var key in setting.Events) {
                    txtInstance.CurrentInstance.bind(key, eval(setting.Events[key]));
                }
            }
        };

        return txtInstance;
    }
    $.fn.HSR.Controls.TextBox.ChangeNumber = function (sender, plus) {
        eval("var currentNumber=" + $("input[tbIdentifier='" + sender.attr("tbIdentifier") + "']").attr("TextBoxData") + ";");
        var currentValue = $("input[tbIdentifier='" + sender.attr("tbIdentifier") + "']").val();
        if (currentNumber.ReadOnly || !currentNumber.Enabled || !currentNumber.IsNumeric || !currentNumber.Enabled) {
            return;
        }
        var stepLength = parseFloat(currentNumber.StepLength);
        var oldValue = 0;
        if (currentValue && currentValue != '') {
            oldValue = parseFloat(currentValue);
            if (isNaN(oldValue)) {
                oldValue = 0;
            }
        }
        oldValue = oldValue + stepLength * plus;
        $("input[tbIdentifier='" + sender.attr("tbIdentifier") + "']").val(oldValue);
    }
})(jQuery);
(function ($) {

    $.fn.HSR.Controls.TabStrip = function (selector) {
        var loadContent = function (forme) {

            //加载自ID
            if (forme.attr('loaded') == '0' && forme.attr('content-id') != null) {
                var content = $(forme.attr('content-id'));
                content.appendTo(forme.attr('href'));
                return;
            }
            //加载自Url
            if (forme.attr('loaded') == '0' && forme.attr('content-url') != null && forme.parent().hasClass("active")) {
                var target = $(forme.attr('href'));
                var url = forme.attr('content-url');
                target.html('加载中...');
                target.load(url, function (rs, status, xhr) {
                    if (status == 'success') {
                        target.empty().html(rs);
                        if (forme.attr('refresh-always') == '0') {
                            forme.attr('loaded', '1');
                        }
                        var loadEvent = window.ciic_tabstrip_options.Load; //加载后处理
                        if (loadEvent != undefined) {
                            var ed = { loadingTo: forme.attr('href'), data: rs };
                            loadEvent(ed);
                        }
                    } else {
                        var errorEvent = window.ciic_tabstrip_options.Error; //错误处理
                        if (errorEvent != undefined) {
                            var event = { loadingTo: forme.attr('href'), message: xhr.status + ':' + xhr.statusText };
                            errorEvent(event);
                        }
                    }
                });
            }
        };
        var instance = {

            Init: function (options) {
                //保存设置选项
                window.ciic_tabstrip_options = options;

                var activetab = $(selector).find('li > a');
                loadContent(activetab);
                //设置事件
                var tabheaders = $(selector).find('a');
                tabheaders.click(function () {

                    //加载内容
                    var $this = $(this);
                    loadContent($this);

                    var selectEvent = window.ciic_tabstrip_options.Select; //选中事件
                    if (selectEvent != undefined) {
                        var e = {
                            //selectedHeadId: $this.attr("id"),
                            selectedContentId: ($this.attr('href'))
                        };

                        selectEvent(e);
                    }
                    ;
                });
            },
            SelectIndex: function (index) {
                var id = selector+ "_tab_" + index;
                var tabA = $(selector).find('li > a[href="' + id + '"]').first();
                tabA.tab('show');
                loadContent(tabA);
            }
        };
        return instance;
    };
})(jQuery);
(function ($) {

    $.fn.HSR.Controls.Panel = function (selector) {
        var loadPanelContent = function (forme) {
            
            //加载自标签id
            if (forme.attr('loaded') == '0' && forme.attr('content-id') != null) {
                var content = $(forme.attr('content-id'));
                content.appendTo(selector + ' .panel-body:first');
                return;
            }
            
            if (forme.attr('loaded') == '0' && forme.attr('content-url') != null) {
                var target = forme.children('.panel-body').first();
                var url = forme.attr('content-url');
                target.html('加载中...');
                target.load(url, function (rs, status, xhr) {
                    if (status == 'success') {
                        target.empty().html(rs);
                        forme.attr('loaded', '1');
                        var loadEvent = window.ciic_panel_options.Load; //加载后处理
                        if (loadEvent != undefined) {
                            var ev = { loadingTo: selector + ' .panel-body', data: rs };
                            loadEvent(ev);
                        }
                    } else {
                        var errorEvent = window.ciic_panel_options.Error; //错误处理
                        if (errorEvent != undefined) {
                            var event = { loadingTo: selector + ' .panel-body', message: xhr.status + ':' + xhr.statusText };
                            errorEvent(event);
                        }
                    }
                });
            }
        };
        var instance = {

            Init: function (options) {
                //保存设置选项
                window.ciic_panel_options = options;
                var activetab = $(selector);
                loadPanelContent(activetab);
            },
            HideBody: function() {
                $(selector).children(".panel-body").toggle();
                $(selector).children(".panel-footer").toggle();
            }
            
        };
        return instance;
    };
})(jQuery);
(function ($) {
    $.fn.HSR.Controls.SwitchThemes = function () {

        var styleName = "hsr" + "." + "theme";
        
        var instance = {
            LoadStyle : function() {
                var themesLink = $('head').children('link[href*="NewStyle"]');

                themesLink.each(function () {
                    var styleCookie = $.cookie(styleName);
                    if (styleCookie != undefined) {
                        var hrefo = $(this).attr("href");
                        var reg = new RegExp('NewStyle[0-9]');
                        var hreft = hrefo.replace(reg, styleCookie);
                        $(this).attr("href", hreft);
                    }
                    
                    
                });
            },
            Preview: function (windowId) {
                //保存设置选项
                $.fn.HSR.Controls.Window(windowId).Show();
            },
            SetTheme:function(style) {
                $.cookie(styleName, style);
                $.fn.HSR.Controls.SwitchThemes().LoadStyle();
            }
        };
        return instance;
    };
})(jQuery);
$(document).ready(function() {
    $.fn.HSR.Controls.SwitchThemes().LoadStyle();
});

(function ($) {
    $.fn.HSR.Controls.ProgressBar = function (selector) {
        var progressBarInstance = new Object();
        progressBarInstance.CurrentInstance = $(selector);

        progressBarInstance.Show = function () {
            this.CurrentInstance.attr("style", "visibility:visible");
        };

        progressBarInstance.Hide = function () {
            this.CurrentInstance.attr("style", "visibility:hidden");
        };

        progressBarInstance.Collapse = function () {
            this.CurrentInstance.attr("style", "width:0px;height:0px");
        };

        progressBarInstance.SetMaxValue = function (value) {
            var min = progressBarInstance.GetMinValue();
            if (value < min) { return; }

            var node = progressBarInstance.GetProgressBarNode();
            node.attr("aria-valuemax", value);
            var width = progressBarInstance.GetWidthOfProgressBar();
            node.css("width", width);
        };

        progressBarInstance.SetMinValue = function (value) {
            var max = progressBarInstance.GetMaxValue();
            if (value > max) { return; }

            var node = progressBarInstance.GetProgressBarNode();
            node.attr("aria-valuemin", value);
            var width = progressBarInstance.GetWidthOfProgressBar();
            node.css("width", width);
        };

        progressBarInstance.SetCurrentValue = function (value) {
            var min = progressBarInstance.GetMinValue();
            var max = progressBarInstance.GetMaxValue();
            if (value > max){
                value = max;
            }
            if (value < min) {
                value = min;
            }

            var node = progressBarInstance.GetProgressBarNode();
            node.attr("aria-valuenow", value);
            var width = progressBarInstance.GetWidthOfProgressBar();
            node.css("width", width);
        };

        progressBarInstance.GetProgressBarNode = function () {
            var node = this.CurrentInstance.find("div:first");
            return node;
        };

        progressBarInstance.GetSpanNode = function () {
            var node = this.CurrentInstance.find("span:first");
            return node;
        };

        progressBarInstance.GetWidthOfProgressBar = function () {
            var node = progressBarInstance.GetProgressBarNode();
            var max = progressBarInstance.GetMaxValue();
            var min = progressBarInstance.GetMinValue();
            var current = progressBarInstance.GetCurrentValue();
            var width = max - min == 0 ? 0 : Math.abs(((current - min) * 100 / (max - min)));
            return "" + width + "%";
        };

        progressBarInstance.GetMaxValue = function () {
            var node = progressBarInstance.GetProgressBarNode();
            return parseInt(node.attr("aria-valuemax"));
        };

        progressBarInstance.GetMinValue = function () {
            var node = progressBarInstance.GetProgressBarNode();
            return parseInt(node.attr("aria-valuemin"));
        };

        progressBarInstance.GetCurrentValue = function () {
            var node = progressBarInstance.GetProgressBarNode();
            return parseInt(node.attr("aria-valuenow"));
        };

        return progressBarInstance;
    };

    $.fn.HSR.Controls.ShowProgressBar = function (w, h) {
        function progressBarInstance(overlayDiv, containerDiv) {
            this._overlayDiv = overlayDiv;
            this._containerDiv = containerDiv;

            this.Close = function () {
                this._overlayDiv.remove();
                this._containerDiv.remove();
            };

            return this;
        }

        var overlayDiv = $("<div></div>");
        overlayDiv.css("opacity", "0.3");
        overlayDiv.css("position", "fixed");
        overlayDiv.css("top", "0px");
        overlayDiv.css("left", "0px");
        overlayDiv.css("width", "100%");
        overlayDiv.css("height", "100%");
        overlayDiv.css("z-index", "100000");
        overlayDiv.css("background-color", "black");

        var containerDiv = $("<div></div>");

        containerDiv.addClass("progress");
        containerDiv.addClass("progress-bar");
        containerDiv.addClass("progress-striped");
        containerDiv.addClass("active");
        containerDiv.css("width", "" + w + "px");
        containerDiv.css("height", "" + h + "px");
        containerDiv.css("position", "fixed");
        containerDiv.css("left", "0px");
        containerDiv.css("right", "0px");
        containerDiv.css("top", "0px");
        containerDiv.css("bottom", "0px");
        containerDiv.css("margin", "auto");
        containerDiv.css("z-index", "100001");

        var progressDiv = $("<div></div>");
        progressDiv.addClass("progress-bar");
        progressDiv.addClass("progress-striped");
        progressDiv.addClass("active");

        progressDiv.attr("role", "progressbar");
        progressDiv.attr("aria-valuenow", "100");
        progressDiv.attr("aria-valuemax", "100");
        progressDiv.attr("aria-valuemin", "0");
        progressDiv.css("width", "100%");

        var spanDiv = $("<span></span>");
        spanDiv.addClass("sr-only");
        spanDiv.html("100% Complete");

        progressDiv.append(spanDiv);
        containerDiv.append(progressDiv);
        $("body").append(overlayDiv);
        $("body").append(containerDiv);

        return new progressBarInstance(overlayDiv, containerDiv);
    };

})(jQuery);


(function ($) {
    $.fn.HSR.Controls.Window = function (selector) {
        var windowInstance = new Object();
        windowInstance.CurrentInstance = $(selector);


        function WinInstance(node) {
            this._isClosed = false;
            this._node = node;
            this._onCloseCallback = null;

            this.OnClose = function (callback) {
                this._onCloseCallback = callback;
            };

            this.Close = function () {
                if (this._isClosed) { return; }
                if (this._onCloseCallback != null) {
                    this._onCloseCallback();
                }
                this._isClosed = true;
            };

            return this;
        }

        windowInstance.Show = function () {
            var instanceMode = this.CurrentInstance.attr("hsr_instance_mode");
            if (instanceMode != "single" && instanceMode != "multiple") {
                throw new Exception("Unknown Instance Mode of Window.");
            }

            var isShowed = this.CurrentInstance.attr("hsr_showed");
            if (instanceMode == "single" && isShowed == "true") {
                return new WinInstance(null);
            }

            var width = this.CurrentInstance.css("hsr_width");
            var height = this.CurrentInstance.css("hsr_height");
            var title = this.CurrentInstance.attr("hsr_title");
            var contentMode = this.CurrentInstance.attr("hsr_content_mode");
            var loadURL = this.CurrentInstance.attr("hsr_ajax_load_url");
            var modal = this.CurrentInstance.attr("hsr_modal");
            var iframe = this.CurrentInstance.attr("hsr_iframe");

            var win = $("<div></div>");
            win.attr("hsr_win_id", selector);
            win.attr("hsr_control_type", "Window");
            var contentHtml = this.CurrentInstance.html();
            var content = $(contentHtml);
            //win.append(content);

            var thisPointer = this;
            var winInstance = new WinInstance(win);
            function onClose() {
                if (instanceMode == "single") {
                    thisPointer.CurrentInstance.attr("hsr_showed", "false");
                }

                winInstance.Close();
            }

            var isModal = true;
            if (modal == "true") {
                isModal = true;
            } else if (modal == "false") {
                isModal = false;
            } else {
                throw new Exception("Unknown Modal of Window.");
            }

            var ifram = true;
            if (iframe == "true") {
                ifram = true;
            } else if (iframe == "false") {
                ifram = false;
            } else {
                throw new Exception("Unknown iframe status of Window.");
            }

            if (contentMode == "inner_html") {
                win.kendoWindow({
                    width: width,
                    height: height,
                    title: title,
                    actions: ["Close"],
                    close: onClose,
                    modal: isModal,
                    content: content,
                    iframe: ifram
                });
                win.data('kendoWindow').center().open();
            } else if (contentMode == "ajax_load") {
                win.kendoWindow({
                    width: width,
                    height: height,
                    title: title,
                    actions: ["Close"],
                    content: {
                        url: loadURL,
                        iframe: ifram
                    },
                    close: onClose,
                    modal: isModal
                });
                win.data('kendoWindow').center().open();
            } else {
                throw new Exception("Unknown Content Mode of Window.");
            }

            this.CurrentInstance.attr("hsr_showed", "true");

            return winInstance;
        };


        return windowInstance;
    };

    $.fn.HSR.Controls.Window.Close = function () {
        try {
            window.parent.$('div[hsr_control_type="Window"]').data("kendoWindow").close();
            window.parent.$('div[hsr_control_type="Window"]').remove();
        } catch (e) {

        }

    };

})(jQuery);

(function ($) {
    $.fn.HSR.Controls.BizDicControl = function (name) {


        var bizDicInstance =
        {

            InitOpenWin: function (eventName, actionUrl, bizDicId, bizDicCode) {

                var window = $("#divBizDicPopwindow");
                var url;
                if (eventName)
                    url = actionUrl + '?bizDicID=' + bizDicId + "&eventName=" + eventName + "&bizDicCode=" + bizDicCode;
                else
                    url = actionUrl + '?bizDicID=' + bizDicId + "&bizDicCode=" + bizDicCode;

                window.kendoWindow({
                    height: "480px",
                    width: "680px",
                    title: "高铁一号线",
                    content: {
                        url: url,
                        iframe: true
                    },
                    visible: false,
                    modal: true,
                });
                window.data('kendoWindow').center().open().title("高铁一号线");
            },

            //获得当前的字典项的值
            GetbizDicCode: function () {
                if ($("#" + name).attr("viewModel") == 'DropdownList')
                    return $("#" + name).data("kendoDropDownList").value();
                return $("#" + name).find("input[type='text']").val();
            },

            //获得当前字典项的名称
            GetBizDicName: function () {
                if ($("#" + name).attr("viewModel") == 'DropdownList')
                    return $("#" + name).data("kendoDropDownList").text();
                return $("#" + name).find("Label").html();
            },

        };

        return bizDicInstance;
    }
})(jQuery);


//NavTab导航条控件附带JS
(function ($) {
    $.fn.HSR.Controls.NavTab = function (selector) {
       
        var NavTabInstance = {
            RegisterClick: function () {
                $("#"+selector+ " div .item").click(function () {
                    
                    var itemId = $(this).attr("id");
                    var itemIdCount = itemId.replace("nav", "") * 1;
                    //移除所有done/current
                    $("#" + selector + " div div[id^='nav']").removeClass("current").removeClass("done")
                    //为之前导航添加done
                    for (var i = 1; i < itemIdCount; i++) {
                        $("#" + selector + " div #nav" + i).removeClass("current").addClass("done");
                    }
                    //为当前添加current
                    $(this).addClass("current");

                    var url = $(this).find("a").attr("value");
                    var target = $(this).find("a").attr("target");
                    window.open(url, target);

                })
            }
        }
        return NavTabInstance;
    };
})(jQuery);


(function ($) {

    $.fn.HSR.Controls.BizSelector = function (name) {

        var placeHolder = $("#" + name).attr("placeHolder");
        var headerTemplateStr = $("#" + name).find("div[type='headerTemplate']")[0].innerHTML;
        var templateStr = $("#" + name).find("div[type='template']")[0].innerHTML;
        var dataTextFieldStr = $("#" + name).attr("dataTextField");
        var selectorModel = $("#" + name).attr("selectorModel");
        var custID = $("#" + name).attr("custID");
        var actionUrl = $("#" + name).attr("actionUrl");
        var changeEvent = $("#" + name).attr("changeEvent");
        var selectEvent = $("#" + name).attr("selectEvent");

        //pop window settings
        var window = $("#" + name).find("div[type='openwin']");
        var width = $(window).attr("width") + "px";
        var height = $(window).attr("height") + "px";
        var title = $(window).attr("title");
        var popUpUrl = $(window).attr("popUpUrl") + "?ControlID=" + name;

        if (selectorModel == 'ContactSelector') {
            actionUrl = actionUrl + "?CustID=" + custID;
            popUpUrl = popUpUrl + "&CustID=" + custID;
        }

        var bizSelectorInstance = {

            InitAutoComplete: function () {
                $("#" + name).find("input[type='text']").kendoAutoComplete({
                    dataTextField: dataTextFieldStr,
                    filter: "startswith",
                    placeholder: placeHolder,
                    //minLength: 4,

                    dataSource: {
                        type: "json",
                        serverFiltering: true,
                        transport: {
                            read: actionUrl,
                            parameterMap: function (data, action) {
                                if (action === "read") {
                                    return {
                                        inputStr: data.filter.filters[0].value
                                    };
                                } else {
                                    return data;
                                }
                            }
                        }
                    },
                    headerTemplate: headerTemplateStr,
                    template: templateStr,

                    change: function (e) {
                        if (changeEvent && changeEvent != '') {
                            eval(changeEvent + "('" + e + "')")
                        }
                    },

                    select: function (e) {
                        var dataItem = this.dataItem(e.item.index());
                        if (selectEvent && selectEvent != '') {
                            eval(selectEvent + "('" + dataItem + "')")
                        }
                    }

                });
            },

            InitOpenWin: function () {

                window.kendoWindow({
                    height: width,
                    width: height,
                    title: title,
                    content: {
                        url: popUpUrl,
                        iframe: true
                    },
                    visible: false,
                    modal: true,
                });

            },

            RegisterClick: function () {

                $("#" + name).find("button").click(function () {

                    window.data('kendoWindow').center().open().title(title);

                })

            },

            CloseWindow: function (name) {
                var window = $("#" + name).find("div[type='openwin']");
                window.data('kendoWindow').close();
            }
        }

        return bizSelectorInstance;
    }
})(jQuery)