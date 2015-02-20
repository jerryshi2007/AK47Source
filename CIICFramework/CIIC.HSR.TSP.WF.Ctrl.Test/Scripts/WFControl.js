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

    $.fn.HSR.Controls.WFStartWorkflow.Invoke = function (sender, progressbar, url, wfParams, beforeClickHandler, afterClickHandler, callback, hasProgress) {
        var bizData = "";
        if (beforeClickHandler && "" != beforeClickHandler) {
            var handler = eval(beforeClickHandler);
            bizData = handler(sender, progressbar);
        }

        var serializedWfParams = JSON.stringify(wfParams);

        if (bizData && bizData != "") {
            bizData = bizData + "&WFParas=" + encodeURIComponent(serializedWfParams);
        }
        else {
            bizData = "WFParas=" + encodeURIComponent(serializedWfParams);
        }
        $.ajax({
            cache: true,
            type: "POST",
            url: url,
            data: bizData,
            async: true,
            beforeSend: function () {
                sender.attr("disabled", "disabled");
            },
            success: function (data) {
                sender.removeAttr("disabled");
                if (hasProgress)
                    progressbar.Close();

                if (callback)
                    callback(data, true);

                if (data.IsSuccess == undefined || data.IsSuccess) {
                    if (afterClickHandler && "" != afterClickHandler) {
                        var hanler = eval(afterClickHandler);
                        hanler(data, sender, progressbar);
                    }
                }
                else
                    alert(data.Message);

            },
            complete: function () {

            },
            error: function (err) {
                if (hasProgress)
                    progressbar.Close();
                sender.removeAttr("disabled");
                if (callback)
                    callback(err, false);
                alert("调用失败:" + err.status);
            }
        });
    };

    $.fn.HSR.Controls.WFTrackWorkflow.Invoke = function (sender, progressbar, windowUrl, callback, hasProgress) {
        progressbar.Close();
        window.open(windowUrl);
    };


    //启动工作流
    $.fn.HSR.Controls.WFStartWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) { }, true);
    }
    //流转工作流
    $.fn.HSR.Controls.WFMoveTo.Click = function (sender) {
        var wfParams = jQuery.parseJSON(container.attr("WFParas"));
        wfParams.IsDefault = false;
        wfParams.Target = jQuery.parseJSON(command);

        //获取意见Id和意见文本值
        alert(wfParams.CommentsControlId);

        alert($("#" + wfParams.CommentsControlId));
        var commentsControl = $("#" + wfParams.CommentsControlId);
        if (typeof commentsControl != "undefined" && commentsControl != null) {
            wfParams.ClientOpinionComments = commentsControl.val();
            wfParams.ClientOpinionId = jQuery.parseJSON(commentsControl.attr("WFParas")).ClientOpinionId;
        }

        $.fn.HSR.Controls.WFStartWorkflow.Invoke(container, command, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.parent().parent().children("button").attr("disabled", "disabled");
            }, false);
    }
    //流转工作流
    $.fn.HSR.Controls.WFMoveTo.ItemClick = function (sender, command, container) {
        var wfParams = jQuery.parseJSON(container.attr("WFParas"));
        wfParams.IsDefault = false;
        wfParams.Target = jQuery.parseJSON(command);

        $.fn.HSR.Controls.WFStartWorkflow.Invoke(container, command, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.parent().parent().children("button").attr("disabled", "disabled");
            }, false);
    }
    //废弃工作流
    $.fn.HSR.Controls.WFCancelWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }

    //暂停工作流
    $.fn.HSR.Controls.WFPauseWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }
    //重启工作流
    $.fn.HSR.Controls.WFResumeWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }
    //恢复工作流
    $.fn.HSR.Controls.WFRestoreWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }
    //撤回工作流
    $.fn.HSR.Controls.WFWithdrawWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }
    //保存工作流
    $.fn.HSR.Controls.WFSaveWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }

    //跟踪工作流
    $.fn.HSR.Controls.WFTrackWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFTrackWorkflow.Invoke(sender, progressbar, wfParams.WindowUrl,
            function (result, status) { }, false);
    }

})(jQuery);