(function ($) {
    $.fn.HSR.Controls.WFStartWorkflow = {};
    $.fn.HSR.Controls.WFMoveTo = {};
    $.fn.HSR.Controls.WFCancelWorkflow = {};
    $.fn.HSR.Controls.WFPauseWorkflow = {};
    $.fn.HSR.Controls.WFResumeWorkflow = {};
    $.fn.HSR.Controls.WFRestoreWorkflow = {};
    $.fn.HSR.Controls.WFCallbackWorkflow = {};
    $.fn.HSR.Controls.WFWithdrawWorkflow = {};
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

    $.fn.HSR.Controls.WFStartWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) { }, true);
    }

    $.fn.HSR.Controls.WFMoveTo.Click = function (sender) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        //wfparas.IsDefault = true;
        //sender.attr("WFParas", JSON.stringify(wfparas));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, "default", wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.parent().children("button").attr("disabled", "disabled");
            }, false);
    }

    $.fn.HSR.Controls.WFMoveTo.ItemClick = function (sender, command, container) {
        var wfParams = jQuery.parseJSON(container.attr("WFParas"));
        wfParams.IsDefault = false;
        wfParams.Target = jQuery.parseJSON(command);

        $.fn.HSR.Controls.WFStartWorkflow.Invoke(container, command, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.parent().parent().children("button").attr("disabled", "disabled");
            }, false);
    }

    $.fn.HSR.Controls.WFCancelWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }

    $.fn.HSR.Controls.WFPauseWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
                if (status) sender.attr("disabled", "disabled");
            }, true);
    }

    $.fn.HSR.Controls.WFResumeWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
            }, true);
    }

    $.fn.HSR.Controls.WFRestoreWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
            }, true);
    }

    $.fn.HSR.Controls.WFCallbackWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
            }, true);
    }

    $.fn.HSR.Controls.WFWithdrawWorkflow.Click = function (sender, progressbar) {
        var wfParams = jQuery.parseJSON(sender.attr("WFParas"));
        $.fn.HSR.Controls.WFStartWorkflow.Invoke(sender, progressbar, wfParams.ActionUrl, wfParams, wfParams.BeforeClick, wfParams.AfterClick,
            function (result, status) {
            }, true);
    }
})(jQuery);