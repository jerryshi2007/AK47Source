(function ($) {

    $.fn.HSR.Controls.WFStartWorkflow = {};
    $.fn.HSR.Controls.WFMoveTo = {};
    $.fn.HSR.Controls.WFCancelWorkflow = {};
    $.fn.HSR.Controls.WFPauseWorkflow = {};
    $.fn.HSR.Controls.WFResumeWorkflow = {};
    $.fn.HSR.Controls.WFRestoreWorkflow = {};
    $.fn.HSR.Controls.WFWithdrawWorkflow = {};
    $.fn.HSR.Controls.WFSaveWorkflow = {};
    $.fn.HSR.Controls.WFTrackWorkflow = {};
    $.fn.HSR.Controls.WFOpinionGridList = {};
    $.fn.HSR.Controls.WFUpdateProcess = {};
    $.fn.HSR.Controls.WFOpenWindow = {};
    $.fn.HSR.Controls.WFComments = {};


    $.fn.HSR.Controls.WFStartWorkflow.Invoke = function (sender, progressbar, url, wfParams, beforeClickHandler, afterClickHandler, callback, hasProgress) {

        if (url == undefined || url == "")
            return;

        var requestUrl = url;

        if (wfParams) {
            var and = "";
            if (url.indexOf("?") == -1) {
                requestUrl = requestUrl + "?";
            }
            if (url.indexOf("&") > -1) {
                and = "&";
            }
            if (wfParams.ResourceId && wfParams.ResourceId != '') {
                requestUrl = requestUrl + and + "resourceID=" + wfParams.ResourceId;
                and = "&";
            }
            if (wfParams.ActivityId && wfParams.ActivityId != '') {
                requestUrl = requestUrl + and + "activityID=" + wfParams.ActivityId;
            }
        }

        var bizData = "";
        if (beforeClickHandler && "" != beforeClickHandler) {
            var handler = eval(beforeClickHandler);
            bizData = handler(sender, progressbar, wfParams);
            if (typeof (bizData) == "boolean" && bizData == false) {
                if (progressbar && progressbar.Close)
                    progressbar.Close();
                return;
            }
        }

        var serializedWfParams = JSON.stringify(wfParams);
        if (bizData && bizData != "") {
            bizData = bizData + "&WFParas=" + encodeURIComponent(encodeURIComponent(serializedWfParams));
        }
        else {
            bizData = "WFParas=" + encodeURIComponent(encodeURIComponent(serializedWfParams));
        }

        $.ajax({
            cache: false,
            type: "POST",
            url: requestUrl,
            data: bizData,
            async: true,
            beforeSend: function () {
                // sender.attr("disabled", "disabled");
            },
            success: function (data) {
                //sender.removeAttr("disabled");

                if (progressbar)
                    progressbar.Close();

                var callbackResult = true;

                if (callback)
                    callbackResult = callback(data, true);

                if (callbackResult == undefined || callbackResult) {
                    if (data.IsSuccess == undefined || data.IsSuccess) {
                        if (afterClickHandler && "" != afterClickHandler) {
                            var hanler = eval(afterClickHandler);
                            hanler(data, sender, progressbar);
                        }
                    }
                    else
                        alert(data.Message);
                }
            },
            complete: function () {
            },
            error: function (err) {
                if (progressbar)
                    progressbar.Close();

                // sender.removeAttr("disabled");

                if (callback)
                    callback(err, false);

                alert("调用失败:" + err.status);
            }
        });
    };

    $.fn.HSR.Controls.WFTrackWorkflow.Invoke = function (sender, progressbar, windowUrl, callback, hasProgress) {
        if (progressbar) {
            progressbar.Close();
        }

        window.open(windowUrl);
    };

    //启动工作流
    $.fn.HSR.Controls.WFStartWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));

        if (wfParams.IsSelectCandidates) {

            $.fn.HSR.Controls.WFOpenWindow.OpenWFCandidatesSelectWindow(progressbar, wfParams, sender.context.id, "False");

        } else {

            //属性恢复
            if (!$.fn.HSR.Controls.WFOpenWindow.RockBackAttribute(sender.context.id, "False", wfParams))
            { return }



            //获取意见Id和意见文本值
            var commentsControl = $("#" + wfParams.CommentsControlId);
            if (commentsControl.attr("WFParas") != undefined) {
                wfParams.ClientOpinionComments = commentsControl.val();
                wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
            }

            $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
                function (result, status) {
                    if (status)
                        sender.parent().children("button").removeAttr("disabled", "disabled");

                    return true;
                }, true);

        }
    }

    //流转工作流
    $.fn.HSR.Controls.WFStartWorkflow.ItemClick = function (sender, command, container, progressbar) {

        var wfParams = jQuery.parseJSON(container.attr("WFParas"));
        wfParams.AutoNext = false;

        //获取意见Id和意见文本值
        var commentsControl = $("#" + wfParams.CommentsControlId);
        if (commentsControl.attr("WFParas") != undefined) {
            wfParams.ClientOpinionComments = commentsControl.val();
            wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
        }

        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.parent().parent().children("button").removeAttr("disabled", "disabled");

                return true;
            }, true);

    }

    //流转工作流
    $.fn.HSR.Controls.WFMoveTo.Click = function (sender, progressbar) {

        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        if (wfParams.IsSelectCandidates) {

            $.fn.HSR.Controls.WFOpenWindow.OpenWFCandidatesSelectWindow(progressbar, wfParams, sender.context.id, "False");

        } else {

            //属性恢复
            if (!$.fn.HSR.Controls.WFOpenWindow.RockBackAttribute(sender.context.id, "False", wfParams))
            { return }

            wfParams.ActionResult = wfParams.Target.ActionResult;
            //获取意见Id和意见文本值
            var commentsControl = $("#" + wfParams.CommentsControlId);
            if (commentsControl.attr("WFParas") != undefined) {
                wfParams.ClientOpinionComments = commentsControl.val();
                wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
            }

            $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
                function (result, status) {
                    if (status)
                        sender.parent().children("button").removeAttr("disabled", "disabled");

                    return true;
                }, false);
        }
    }


    //流转工作流
    $.fn.HSR.Controls.WFMoveTo.ItemClick = function (sender, command, container, progressbar) {

        var wfParams = jQuery.parseJSON(container.attr("WFParas"));
        wfParams.IsDefault = false;
        wfParams.Target = jQuery.parseJSON(command);
        wfParams.ActionResult = wfParams.Target.ActionResult;
        wfParams.IsSelectCandidates = wfParams.Target.IsSelectCandidates;
        wfParams.IsAssignToMultiUsers = wfParams.Target.IsAssignToMultiUsers;

        if (wfParams.IsSelectCandidates) {

            $.fn.HSR.Controls.WFOpenWindow.OpenWFCandidatesSelectWindow(progressbar, wfParams, sender.context.id, "True");

        } else {

            //属性恢复
            if (!$.fn.HSR.Controls.WFOpenWindow.RockBackAttribute(sender.context.id, "True", wfParams))
            { return }

            //获取意见Id和意见文本值
            var commentsControl = $("#" + wfParams.CommentsControlId);
            if (commentsControl.attr("WFParas") != undefined) {
                wfParams.ClientOpinionComments = commentsControl.val();
                wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
            }

            $.fn.HSR.Controls.WFStartWorkflow.Invoke(container, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
                function (result, status) {
                    if (status)
                        sender.parent().parent().children("button").removeAttr("disabled", "disabled");

                    return true;
                }, false);
        }

    }

    //废弃工作流
    $.fn.HSR.Controls.WFCancelWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        //获取意见Id和意见文本值
        var commentsControl = $("#" + wfParams.CommentsControlId);
        if (commentsControl.attr("WFParas") != undefined) {
            wfParams.ClientOpinionComments = commentsControl.val();
            wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
        }

        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                return true;
            }, true);
    }

    //暂停工作流
    $.fn.HSR.Controls.WFPauseWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                return true;
            }, true);
    }

    //重启工作流
    $.fn.HSR.Controls.WFResumeWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                return true;
            }, true);
    }

    //恢复工作流
    $.fn.HSR.Controls.WFRestoreWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                return true;
            }, true);
    }

    //撤回工作流
    $.fn.HSR.Controls.WFWithdrawWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                return true;
            }, true);
    }

    //保存工作流
    $.fn.HSR.Controls.WFSaveWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        //获取意见Id和意见文本值
        var commentsControl = $("#" + wfParams.CommentsControlId);

        if (commentsControl.attr("WFParas") != undefined) {
            wfParams.ClientOpinionComments = commentsControl.val();
            wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
        }
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                return true;
            }, true);
    }

    //跟踪工作流
    $.fn.HSR.Controls.WFTrackWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFTrackWorkflow.Invoke(sender, progressbar, wfParams.WindowUrl,
            function (result, status) {
                return true;
            }, false);
    }

    $.fn.HSR.Controls.WFUpdateProcess.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));

        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status)
                    sender.removeAttr("disabled");

                if (result.IsSuccess == undefined || result.IsSuccess) {
                    //设置HTML
                    for (var info in result.OtherData) {
                        $("#" + info).html(result.OtherData[info]);
                    }
                }
                else {
                    alert(result.Message);
                }
                return false;
            }, true);
    }

    ///弹出选择审批人对话框
    $.fn.HSR.Controls.WFOpenWindow.OpenWFCandidatesSelectWindow = function (progressbar, wfParams, contextId, isDropItem) {

        if (progressbar || progressbar != null)
            progressbar.Close();

        var parentId = $.fn.HSR.Controls.WFOpenWindow.GetParentId(contextId);
        var openWindowId = '#' + parentId + 'MoveToWindow';
        var innerOpenWindowId = '#' + parentId + 'InnerMoveToWindow';
        var candidatesList = wfParams.Target.Candidates;

        $(innerOpenWindowId).empty();
        $(innerOpenWindowId).attr("btnControlId", contextId);
        $(innerOpenWindowId).attr("isDropItem", isDropItem);
        $(innerOpenWindowId).attr("candidatesSave", JSON.stringify(candidatesList));


        candidatesList.forEach(function (e) {
            if (wfParams.IsAssignToMultiUsers) {
                $(innerOpenWindowId).append('<input name="candidates" value="' + e.User.id + '" type="checkbox">' + e.User.name + '</input></br>');
            } else {
                $(innerOpenWindowId).append('<input type="radio" name="candidates" value="' + e.User.id + '"  checked />' + e.User.name + '</br>');
            }
        })

        var window = $(openWindowId);
        window.data('kendoWindow').center().open().title("流程审批人选择");

    }


    ///流程审批人选择
    $.fn.HSR.Controls.WFOpenWindow.WFCandidatesSelect = function (isDropdownBtn, innerMoveToWindowId, moveToWindowId) {

        //取得单击按钮的ID
        var btnClickId = $('#' + innerMoveToWindowId).attr("btnControlId");
        var isDropItem = $('#' + innerMoveToWindowId).attr("isDropItem");

        //保存属性
        $('#' + innerMoveToWindowId).attr("isDropdownBtn", isDropdownBtn);

        //验证是否有选择项
        var arrayUserId = new Array();
        $("[name='candidates']:checked").each(function () {
            arrayUserId.push($(this).val());
        })

        if (arrayUserId.length == 0) {
            alert("请选择审批人");
            return;
        }

        //重构审批人
        var wfParams;
        var candidatesList;
        if (isDropItem == "True") {
            wfParams = jQuery.parseJSON($('#' + btnClickId).attr("command"));
            candidatesList = wfParams.Candidates;
        } else {
            wfParams = jQuery.parseJSON($('#' + btnClickId).attr("WFParas"));
            candidatesList = wfParams.Target.Candidates;
        }

        for (var i = candidatesList.length - 1; i >= 0; i--) {
            var isRemove = true;
            for (var j = 0; j < arrayUserId.length; j++) {
                if (arrayUserId[j] == candidatesList[i].User.id)
                    isRemove = false;
            }
            if (isRemove)
                candidatesList.splice(i, 1);
        }

        if (isDropItem == "True") {
            wfParams.Candidates = candidatesList;
        } else {
            wfParams.Target.Candidates = candidatesList;
        }

        //重新赋值
        wfParams.IsSelectCandidates = false;
        var serializedWfParams = JSON.stringify(wfParams);

        if (isDropItem == "True") {
            $('#' + btnClickId).attr("command", "");
            $('#' + btnClickId).attr("command", serializedWfParams);
        } else {
            $('#' + btnClickId).attr("WFParas", "");
            $('#' + btnClickId).attr("WFParas", serializedWfParams);
        }

        if (isDropdownBtn == "True") {
            if (isDropItem == "False") {
                //设置对话框
                var parenId = $('#' + btnClickId).parent().attr("id");
                var dropdownButton = $.fn.HSR.Controls.DropDowButton('#' + parenId);
                dropdownButton.SetEnableDialog(false);
            } else {
                //下拉按钮时动态改变属性
                var dropitemdata = jQuery.parseJSON($("#" + btnClickId).attr("dropitemdata"));
                dropitemdata.EnableDialog = false;
                dropitemdata.Command = JSON.stringify(wfParams);
                var serializeddropitemdata = JSON.stringify(dropitemdata);
                $("#" + btnClickId).attr("dropitemdata", serializeddropitemdata);
            }

        } else {
            //设置对话框
            var button = $.fn.HSR.Controls.Button('#' + btnClickId);
            button.SetEnableDialog(false);
        }

        //触发单击事件
        parent.$('#' + btnClickId).click();
        //关闭窗口
        parent.$('#' + moveToWindowId).data('kendoWindow').close();
    }
    //返回父类ID
    $.fn.HSR.Controls.WFOpenWindow.GetParentId = function (id) {
        var returnId = $("#" + id).parent("div").attr("id");

        if (returnId == undefined || returnId == "" || returnId == null)
            returnId = $("#" + id).parent().parent("div").attr("id");

        return returnId;
    }

    //属性恢复 
    $.fn.HSR.Controls.WFOpenWindow.RockBackAttribute = function (contextId, isDropItem, wfParamsRet) {

        var parentId = '#' + $.fn.HSR.Controls.WFOpenWindow.GetParentId(contextId);
        var innerOpenWindowId = parentId + 'InnerMoveToWindow';
        var btnClickId = $(innerOpenWindowId).attr("btnControlId");
        var candidateslist = $(innerOpenWindowId).attr("candidatesSave");
        var isDropdownBtn = $(innerOpenWindowId).attr("isDropdownBtn");

        if (btnClickId == undefined || btnClickId == null || btnClickId == "") {

            return true;
        }

        btnClickId = '#' + btnClickId;

        if (candidateslist == undefined || candidateslist == null || candidateslist == "") {
            return false;
        }
        wfParamsRet.IsSelectCandidates = true;
        candidateslist = jQuery.parseJSON(candidateslist);

        //恢复审批人
        var wfParams;
        if (isDropItem == "True") {
            wfParams = jQuery.parseJSON($(btnClickId).attr("command"));
            wfParams.Candidates = candidateslist;
        } else {
            wfParams = jQuery.parseJSON($(btnClickId).attr("WFParas"));
            wfParams.Target.Candidates = candidateslist;
        }

        //重新赋值
        wfParams.IsSelectCandidates = true;
        var serializedWfParams = JSON.stringify(wfParams);

        if (isDropItem == "True") {
            $(btnClickId).attr("command", "");
            $(btnClickId).attr("command", serializedWfParams);
        } else {
            $(btnClickId).attr("WFParas", "");
            $(btnClickId).attr("WFParas", serializedWfParams);
        }

        //恢复对话框
        if (isDropdownBtn == "True") {
            if (isDropItem == "False") {
                //设置对话框
                var parenIdTemp = "#" + $(btnClickId).parent().attr("id");
                var dropdownButton = $.fn.HSR.Controls.DropDowButton(parenIdTemp);
                dropdownButton.SetEnableDialog(true);
            } else {
                //下拉按钮时动态改变属性
                var dropitemdata = jQuery.parseJSON($(btnClickId).attr("dropitemdata"));
                dropitemdata.EnableDialog = true;
                dropitemdata.Command = JSON.stringify(wfParams);
                var serializeddropitemdata = JSON.stringify(dropitemdata);
                $(btnClickId).attr("dropitemdata", serializeddropitemdata);
            }

        } else {
            //设置对话框
            var button = $.fn.HSR.Controls.Button(btnClickId);
            button.SetEnableDialog(true);
        }

        return true;
    }

    //意见输入框输入长度限制
    $.fn.HSR.Controls.WFComments.InputLimit = function (id) {
        var inputVal = $("#" + id).val();
        var inputLength = inputVal.length;
        if (inputLength > 1000) {
            $("#" + id).val(inputVal.substr(0, 1000));
        }

    }
})(jQuery);

//流程图形导航
(function ($) {

    $.fn.HSR.Controls.WFGraph = function (selector) {
        var that = $(selector);
        var instance = {
            Init: function (options) {
                var opt = new _options(options);
                var $process = that.find('.eve-navigation:first');
                opt.processID = $process.attr("processID");

                that.data("wfGraph", opt);
                if (opt.processID != undefined) {
                    this.InitEvent(that);
                }

            }
            , InitEvent: function ($that) {
                var options = that.data('wfGraph');
                that.find('.eve-navigation').EventLine_DragSlider().init();
                var ctl = this;
                $that.find('.childFlag').click(function () {
                    var $item = $(this);
                    var $fitem = $item.find('span');//改变展开模式
                    ctl.LoadChildProcess($fitem, options);

                });
            }
            , Reload: function (processID) {
                var options = that.data('wfGraph');
                if (options == undefined) {
                    return;
                };
                if (processID != null && processID != undefined) {
                    //第一次加载属性
                    options.processID = processID;
                    that.data('wfGraph').processID = processID;
                }
                if (options.processID == null && processID == undefined) {
                    alert(options.errorMsg + "processID");
                    return;//非法调用
                }

                this.GetHtml(options, function (data) {
                    if (data.IsSuccess) {
                        var $wrap = that.find('.wfGraphBody');
                        $wrap.empty();
                        $wrap.append(data.Message);

                        that.find('.eve-navigation').EventLine_DragSlider().init();
                    }
                });
            }
            , GetHtml: function (options, callback) {//主流程

                $.ajax({
                    cache: false,
                    type: "POST",
                    url: options.url,
                    data: { "processId": options.processID, "graphOps": options.graphOps },
                    async: true,
                    success: function (data) {
                        if (callback) {
                            callback(data);
                        };
                    },
                    complete: function () {
                    },
                    error: function (err) {
                        alert(options.errorMsg + err.status);
                    }
                });

            }
              , GetBranchHtml: function (options, bid, callback) {//分支流程 
                  options.graphOps.IsBranch = true;
                  $.ajax({
                      cache: false,
                      type: "POST",
                      url: options.url,
                      data: { "processId": bid, "graphOps": options.graphOps },
                      async: true,
                      success: function (data) {
                          if (callback) {
                              callback(data);
                          };
                      },
                      complete: function () {
                      },
                      error: function (err) {
                          alert(options.errorMsg + err.status);
                      }
                  });
              }
            , GetChildHtml: function (options, activityID, win) {//分支流程列表
                var ctl = this;
                $.ajax({
                    cache: false,
                    type: "POST",
                    url: options.branchUrl,
                    data: { "activityID": activityID, "graphOps": options.graphOps, "branchOps": options.branchOps },
                    async: true,
                    success: function (data) {
                        win.empty();
                        //加载数据  
                        win.html(data.Message);

                        //取分页数据
                        var total = parseInt(win.find(".k-pager-wrap").attr("rowtotal"));
                        if (total != NaN && total != -1) {
                            options.branchOps.TotalRows = total;
                        }
                        //加载分页事件
                        win.find(".k-pager-nav").click(function () {

                            var $pnav = $(this);
                            var v = $pnav.attr("ops");
                            var startIndex = parseInt(v);
                            if (startIndex == NaN || startIndex < 0) {
                                startIndex = 0
                            }

                            options.branchOps.StartRowIndex = startIndex;

                            ctl.GetChildHtml(options, activityID, win)
                        });

                        //注册加载事件 
                        win.find(".list-group-item").click(function () {
                            var $branchgraph = $(this);
                            var $graphwrap = $branchgraph.next();
                            var bid = $branchgraph.attr("processID");
                            var haveload = $graphwrap.attr("dataload");
                            if ($graphwrap.is(':hidden')) {
                                $graphwrap.show();
                            }
                            else {
                                $graphwrap.hide()
                            }
                            if (haveload != 1) {
                                //载入进度  
                                ctl.loadProgress($graphwrap);

                                ctl.GetBranchHtml(options, bid, function (data) {
                                    $graphwrap.attr("dataload", 1);
                                    $graphwrap.empty();
                                    $graphwrap.html(data.Message);
                                    $graphwrap.find('.eve-navigation').EventLine_DragSlider().init();
                                })
                            }

                        });
                    },
                    complete: function () {
                    },
                    error: function (err) {
                        alert(options.errorMsg + err.status);
                    }
                });
            }
            , LoadChildProcess: function (fitem, options) {
                //获取数据
                if (fitem.hasClass("glyphicon-plus")) {
                    var id = fitem.attr("activityID");
                    var ctl = this;
                    var win = $("#childProcessDiv");
                    ctl.loadProgress(win);


                    win.kendoWindow({
                        height: "350px",
                        width: "500px",
                        visible: false,
                        modal: true,
                        close: function () {
                            fitem.removeClass('glyphicon-minus');
                            fitem.addClass('glyphicon-plus');
                        }
                    });
                    win.data('kendoWindow').center().open().title(options.graphOps.BranchWinTitle);
                    fitem.removeClass('glyphicon-plus');
                    fitem.addClass('glyphicon-minus');

                    //异步加载数据
                    options.branchOps.TotalRows = -1;
                    ctl.GetChildHtml(options, id, win);

                }
            }
            , loadProgress: function (oitem) {
                oitem.empty();
                var progress = that.find(".wfGraphProgress").html();
                oitem.html(progress);
            }
        };
        return instance;
    }
    var _options = function (opt) {
        this.defaults = {
            'url': '#',
            'branchUrl': '#',
            'processID': '0',
            'errorMsg': '调用失败:',
            'graphOps': {},
            'branchOps': {}
        };
        return $.extend({}, this.defaults, opt);
    }

})(jQuery);


//流程图形导航 滑动
(function ($) {
    jQuery.extend(jQuery.easing, {
        easeOutExpo: function (x, t, b, c, d) {
            return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;
        }
    });

    $.fn.EventLine_DragSlider = function () {
        var that = this;
        var drag = {
            element: "",
            element_move: "",
            constraint: "",
            sliding: false,
            pagex: {
                start: 0,
                end: 0
            },
            pagey: {
                start: 0,
                end: 0
            },
            left: {
                start: 0,
                end: 0
            },
            time: {
                start: 0,
                end: 0
            },
            touch: false,
            ease: "easeOutExpo",
            width: 0,
            height: 100
        },
		is_sticky = false;

        this.init = function (touch, sticky) {
            drag.element = that;
            drag.element_move = that.find('.eve-nav');
            if (sticky != null && sticky != "") {
                is_sticky = sticky;
            }

            this.updateConstraint();

            if (touch) {
                drag.touch = touch;
            } else {
                drag.touch = false;
            }
            if (drag.touch) {
                dragevent = touchdrag;
            } else {
                dragevent = mousedrag;
            }

            $(window).resize(function () {
                that.updateConstraint();
            });

            makeDraggable(drag);
        };

        this.updateConstraint = function (constraint) {
            if (constraint != null && constraint != "") {
                drag.constraint = constraint;
            } else {
                //初始化限制条件，计算单元格               
                var owt = 0;
                var ow = 0;
                var marker = 0;
                var navwt = that.outerWidth();
                drag.height = that.outerHeight();
                if (navwt == undefined) {
                    navwt = 300;
                }
                if (drag.height == undefined) {
                    drag.height = 100;
                }
                that.find(".eve-content .item").each(function () {
                    var $that = $(this);

                    if ($that.hasClass('Running')) {
                        marker = owt;
                    }

                    $that.css('left', owt);

                    ow = $that.outerWidth();
                    owt += ow;
                });
                drag.width = owt;
                var owr = navwt - drag.width;
                if (owr > 0) {
                    owr = 0;
                }

                var constraint = {};
                constraint.left = 0;
                constraint.right = owr;//页面宽-导航宽

                //定位到焦点
                var mr = -marker + navwt / 2;
                if (mr > 0) {
                    mr = 0;
                }
                if (mr < owr) {
                    mr = owr;
                }
                drag.element_move.css('left', mr);

                drag.constraint = constraint;
            }
        };

        return that;
    }

    var dragevent = {
        down: "mousedown",
        up: "mouseup",
        leave: "mouseleave",
        move: "mousemove"
    }, mousedrag = {
        down: "mousedown",
        up: "mouseup",
        leave: "mouseleave",
        move: "mousemove"
    }, touchdrag = {
        down: "touchstart",
        up: "touchend",
        leave: "mouseleave",
        move: "touchmove"
    };

    function makeDraggable(drag) {
        var drag_object = drag.element, move_object = drag.element_move;
        drag_object.bind(dragevent.down, { drag: drag, element: move_object, delement: drag_object }, onDragStart);
        drag_object.bind(dragevent.up, { drag: drag, element: move_object, delement: drag_object }, onDragEnd);
        drag_object.bind(dragevent.leave, { drag: drag, element: move_object, delement: drag_object }, onDragLeave);
    }

    function onDragLeave(e) {
        var drag = e.data.drag;
        e.data.delement.unbind(dragevent.move, onDragMove);
        if (!drag.touch) {
            e.preventDefault();
        }
        e.stopPropagation();
        if (drag.sliding) {
            drag.sliding = false;
            dragEnd(drag, e.data.element, e.data.delement, e);
            return false;
        } else {
            return true;
        }
    }

    function onDragStart(e) {

        var drag = e.data.drag;
        dragStart(drag, e.data.element, e.data.delement, e);
        if (!drag.touch) {
            e.preventDefault();
        }
        return true;
    }

    function onDragEnd(e) {

        var drag = e.data.drag;

        if (!drag.touch) {
            e.preventDefault();
        }

        if (drag.sliding) {
            drag.sliding = false;
            dragEnd(drag, e.data.element, e.data.delement, e);
            return false;
        } else {
            e.data.delement.unbind(dragevent.move, onDragMove);
            return true;
        }
    }

    function onDragMove(e) {
        dragMove(e.data.drag, e.data.element, e);
    }

    function dragStart(drag, elem, delem, e) {
        if (drag.touch) {
            elem.css('-webkit-transition-duration', '0');
            drag.pagex.start = e.originalEvent.touches[0].screenX;
            drag.pagey.start = e.originalEvent.touches[0].screenY;
        } else {
            drag.pagex.start = e.pageX;
            drag.pagey.start = e.pageY;
        }
        drag.left.start = getLeft(elem);
        drag.time.start = new Date().getTime();

        elem.stop();
        delem.bind(dragevent.move, { drag: drag, element: elem }, onDragMove);


    }

    function dragEnd(drag, elem, delem, e) {

        delem.unbind(dragevent.move, onDragMove);
        dragMomentum(drag, elem, e);
    }

    function dragMove(drag, elem, e) {
        var drag_to, drag_to_y;
        drag.sliding = true;
        if (drag.touch) {
            drag.pagex.end = e.originalEvent.touches[0].screenX;
            drag.pagey.end = e.originalEvent.touches[0].screenY;
        } else {
            drag.pagex.end = e.pageX;
            drag.pagey.end = e.pageY;
        }

        drag.left.end = getLeft(elem);
        drag_to = -(drag.pagex.start - drag.pagex.end - drag.left.start);


        if (Math.abs(drag_to - drag.left.start) > 10) {
            elem.css('left', drag_to);
            e.preventDefault();
            e.stopPropagation();
        }
    }

    function dragMomentum(drag, elem, e) {
        var drag_info = {
            left: drag.left.end,
            left_adjust: 0,
            change: {
                x: 0
            },
            time: (new Date().getTime() - drag.time.start) * 10,
            time_adjust: (new Date().getTime() - drag.time.start) * 10
        },
            multiplier = 3000;

        if (drag.touch) {
            multiplier = 6000;
        }

        drag_info.change.x = multiplier * (Math.abs(drag.pagex.end) - Math.abs(drag.pagex.start));


        drag_info.left_adjust = Math.round(drag_info.change.x / drag_info.time);

        drag_info.left = Math.min(drag_info.left + drag_info.left_adjust);

        if (drag.constraint) {
            if (drag_info.left > drag.constraint.left) {
                drag_info.left = drag.constraint.left;
                if (drag_info.time > 5000) {
                    drag_info.time = 5000;
                }
            } else if (drag_info.left < drag.constraint.right) {
                drag_info.left = drag.constraint.right;
                if (drag_info.time > 5000) {
                    drag_info.time = 5000;
                }
            }
        }
        if (!drag.is_sticky) {
            if (drag_info.time > 0) {
                if (drag.touch) {
                    elem.animate({ "left": drag_info.left }, drag_info.time, "easeOutCirc");
                } else {
                    elem.animate({ "left": drag_info.left }, drag_info.time, drag.ease);
                }
            }
        }
    }

    function getLeft(elem) {
        return parseInt(elem.css('left').substring(0, elem.css('left').length - 2), 10);
    }
})(jQuery);
