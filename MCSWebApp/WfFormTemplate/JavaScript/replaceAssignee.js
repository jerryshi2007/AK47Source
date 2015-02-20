//获取GridView选中的任务ID
function getTasksID() {
    //从GridView中获取被选中任务的TaskID
    var tasksID = $find("gridViewTask").get_clientSelectedKeys();
    if (tasksID.length == 0) {
        $showError("请选择至少一个任务后再提交！");
    }
    return tasksID;
}

//获取原始待办人ID
function getOriginalUserID() {
    return $get("hiddenOriginalUserID").value;
}

//获取目的待办人ID数组
function getTargetUsersID() {

    var targetUsersResult = { Tag: true, Users : [] };
    //目的待办人数组
   // var targetUsersID = new Array();

    //显示目的待办人选择对话框
    var result = $find("targetUserSelector").showDialog();

    if (result.result == false) {
        targetUsersResult.Tag = false;

    } else {
        //从选择中取出目的待办人
        if (result && result.users.length > 0) {
            //取原始待办人
            var originalUserID = getOriginalUserID();
            for (var i = 0; i < result.users.length; i++) {
                //检查原始待办人与目的待办人是否相同
                if (originalUserID != result.users[i].id) {
                    targetUsersResult.Users.push(result.users[i].id);
                   // targetUsersID.push(result.users[i].id);
                }
            }

            //如果目的待办人数组长度为0，说明只选择了一个目的待办人，且目的待办人与原始待办人相同
            if (targetUsersResult.Users.length == 0) {
                $showError("原始待办人与目的待办人相同，请重新选择目的待办人！");
            }
        }
        else {
            $showError("请选择待办人后再提交！");
        }
    }

    return targetUsersResult;
}

//提交修改数据
function onBeforeStart(e) {
    //从GridView中获取被选中任务的TaskID
    var tasksID = getTasksID();
    if (tasksID.length == 0) {
        e.cancel = true;
        return e;
    }

    var result = getTargetUsersID();
    if (result.Tag == false) {
        e.cancel = true;
        return e;
    }    

    //取目的待办人
    var targetUsersID =result.Users;
    if (targetUsersID.length == 0) {
        e.cancel = true;
        return e;
    }

    //组织分步数据并将其序列化
    e.steps = new Array(tasksID.length);
    for (var j = 0; j < tasksID.length; j++) {
        var stepData = {
            TaskID: tasksID[j],
            OriginalUserID: getOriginalUserID(),
            TargetUsersID: targetUsersID
        };
        e.steps[j] = Sys.Serialization.JavaScriptSerializer.serialize(stepData);
    }

    return e;
}

//完成处理
function onFinished(e) {
    //检查处理结果
    if (e.value)
        alert("处理完成");
    else {
        $showError(e.error.message);
    }

    //处理完成后，不论成功与否，均主动执行一遍查询
    $get("btnQuery").click();
}