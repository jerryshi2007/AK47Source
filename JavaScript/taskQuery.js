function setControlValuesToObject(arg)
{
	if (arguments.length > 1)
		for(var i = 1; i < arguments.length; i++)
			setControlValueToObject(arg, arguments[i]);
}

/*   周杨于20090626日注释
function setControlValueToObject(arg, controlID, valuePropertyName)
{
	if (!valuePropertyName)
		valuePropertyName = "value";

    arg[controlID] = $get(controlID)[valuePropertyName];
	//eval("arg." + controlID + "=$get(\"" + controlID + "\")." + valuePropertyName);
}
*/

/* 周杨于20090624日新增  */
function setControlValueToObject(arg, controlID, valuePropertyName) {
	if (!valuePropertyName)
		valuePropertyName = "value";
	eval("arg." + controlID + "=$get(\"" + controlID + "\")." + valuePropertyName);
}

function setObjectToControlValues(result, controlID)
{
	if (arguments.length > 1)
		for(var i = 1; i < arguments.length; i++)
			setObjectToControlValue(result, arguments[i]);
}

function setObjectToControlValue(result, controlID, valuePropertyName) {
	if (!valuePropertyName)
		valuePropertyName = "value";

	eval("$get(\"" + controlID + "\")." + valuePropertyName + "=result." + controlID);
}

function initialFormCategoris(controlId) {
	var formCategoris = document.getElementById(controlId).value;
	if (formCategoris == "") {
		document.getElementById(controlId).value = "全部";
		
	}
}

//验证日期起始时间小于结束时间
function verifyTime(beginTimeID,endTimeID)
{
    var begin = document.getElementById(beginTimeID).value;
    var end = document.getElementById(endTimeID).value;
    var result = true;
    //有一个为空就不用验证
    if ((begin != "") && (end != ""))
    {
        var arrBegin = begin.split("-");
        var arrEnd = end.split("-");
        //构造Date对象
        var dateBegin = new Date(arrBegin[0],arrBegin[1] - 1,arrBegin[2]);
        var dateEnd = new Date(arrEnd[0],arrEnd[1] - 1,arrEnd[2]);


        if (dateBegin > dateEnd)
        {
            alert("起始时间不能大于结束时间");
            result = false;
        }
    }
    
    return result;
}

function verifyUserInput(verifyUserInput)
{
	var oucontrol = $find(verifyUserInput);
	var result = oucontrol.checkData();
	
	if(result == false)
	{
		alert("请输入正确的人员或机构");
	}	
	return result;
}

