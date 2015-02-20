function getDefaultTaskFeature() {
    var width = 820;
    var height = 700;

    var left = (window.screen.width - width) / 2;
    var top = (window.screen.height - height) / 2;

    return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
}

function getSmallTaskFeature() {
    var width = 540;
    var height = 360;

    var left = (window.screen.width - width) / 2;
    var top = (window.screen.height - height) / 2;

    return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=no";
}

function onTaskLinkClick(url, feature) {


    var a = event.srcElement;

    if (!feature)
        feature = getDefaultTaskFeature();

    var returnValue = false;

    if (a.target.toLowerCase() == "_self")
        returnValue = true;
    else
        window.open(url, a.target, feature);

    event.returnValue = returnValue;

    if (a.unreadflag == 'True') {
        if (document.title == '已办列表') {
            callUpdateCompletedTaskStatus(a.taskid);
        }
        else {
            callUpdateTaskStatus(a.taskid);
        }
        setTaskReaded(a);
    }

    event.cancelBubble = true;

    return returnValue;
}

function setTaskReaded(a) {
    var tr = a.parentElement.parentElement;
    if (tr.style.fontWeight) {
        tr.style.fontWeight = 'normal';
    }
    a.unreadflag = 'False';
}

function callUpdateTaskStatus(taskid) {
    MCS.OA.Portal.Services.PortalServices.UpdateTaskReadTime(taskid);
}

function callUpdateCompletedTaskStatus(taskid) {
    MCS.OA.Portal.Services.PortalServices.UpdateCompletedTaskReadTime(taskid);
}

function onPopUpClick(url, nofityFeature, taskid) {
    if (!nofityFeature)
        nofityFeature = "height=600,width=800,status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";

    window.open(url, "_blank", nofityFeature);

    callUpdateTaskStatus(taskid);
}

function openStandardForm(url, name) {
    feature = getDefaultTaskFeature();
    window.open(url, name, feature);

    return false;
}

function openSmallForm(url, name) {
    feature = getSmallTaskFeature();
    window.open(url, name, feature);

    return false;
}