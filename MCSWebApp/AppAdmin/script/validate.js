<!--
var STD_NUMBER_FORMAT = "#,##0.00";
var STD_RATIO_FORMAT = "0.00####";
var STD_COUNT_FORMAT = "#,##0.00####";
var STD_AMOUNT_FORMAT = "#,##0";
var STD_YEAR_FORMAT = "###0";
var STD_MONTH_FORMAT = "#00";

var YEAR_MAX = "2199";
var YEAR_MIN = "1900";

//数据类型常量
var DT_STRING = "string";
var DT_NUMBER = "number";
var DT_DATETIME = "datetime";

var msInDay = 86400000;	//每天毫秒数
var msInHour = 3600000;	//每小时的毫秒数
var msInMinute = 60000;	//每分钟的毫秒数
var msInSecond =  1000;	//每秒的毫秒数

var m_oldElement = null;
var m_oldElementClass = null;

function falseThrow(bCondition, strDescription)
{
	if (!bCondition)
		throw strDescription;
}

function trueThrow(bCondition, strDescription)
{
	if (bCondition)
		throw strDescription;
}

function isUndefined(objType)
{
	return (objType == "undefined" || objType == "unknown");
}

function originalValue(obj)
{
	var strValue = "";

	if (obj.originalValue)
		strValue = obj.originalValue;
	else
		strValue = obj.value;

	if (obj.tagName == "INPUT")
		if (obj.type == "checkbox")
		{
			if (obj.checked)
				strValue = "y";
			else
				strValue = "n";
		}
	return strValue;
}

function clearValue(obj)
{
	if (typeof(obj.originalValue) != "undefined")
		obj.originalValue = "";

	obj.value = "";
}

//过滤掉指定字符
function filterChar(nstr, chFilter)
{
	var strNew = "";

	for (var i = 0; i < nstr.length; i++)
	{
		var ch = nstr.substr(i, 1);
		if (ch != chFilter)
			strNew += ch;
	}

	return strNew;
}

//Numeric check: numericCheck(nr, [intDigit], [fracDigit], [minValue], [maxValue])
function numericCheck(nr)
{
	//trueThrow(nr.length == 0, "必需输入内容");

	var nArgs = arguments.length;
	var nCount = 0;
	var nPointIndex = -1;

	for (var i = 0; i < nr.length; i++)
	{
		var ch;

		ch = nr.substr(i, 1);

		if (ch < "0" || ch > "9" )
		{
			if (ch == ".")
			{
				if (nPointIndex != -1)
					throw "数字类型只能有一个小数点";
				else
					nPointIndex = i;
			}
			else
			if (ch != ",")	//过滤掉数字
				throw "必需输入合法的数字";
		}
	}

	if (nPointIndex == -1)
		nPointIndex = nr.length;

	if (nArgs > 1)	//参数个数大于1
	{
		var nNumber = nr * 1;
		var intDigit = arguments[1];
		var fracDigit;
		var minValue;
		var maxvalue;

		if (nArgs > 2)
		{
			fracDigit = arguments[2];
			if (nArgs > 3)
			{
				minValue = arguments[3];
				if (nArgs > 4)
					maxValue = arguments[4];
			}
		}
	}

	trueThrow(typeof(intDigit) != "undefined" && (nr.substring(0, nPointIndex) * 1).toString().length > intDigit,
		 "整数部分的位数不能超过" + intDigit + "位");

	var strFrac = nr.substring(nPointIndex + 1, nr.length);

	if (strFrac.length > 0)
	{
		strFrac = "0." + strFrac;
		trueThrow(typeof(fracDigit) != "undefined" && (strFrac * 1).toString().length - 2 > fracDigit,
			"小数部分的位数不能超过" + fracDigit + "位");
	}

	if (typeof(minValue) != "undefined" && typeof(maxValue) != "undefined")
	{
		trueThrow((nr * 1) < minValue || (nr * 1) > maxValue, "数字必需在" + minValue + "和" + maxValue + "之间");
	}
	else
	if (typeof(minValue) != "undefined")
	{
		trueThrow((nr * 1) < minValue, "数字必需大于等于" + minValue);
	}
	else
	if (typeof(maxValue) != "undefined")
	{
		trueThrow((nr * 1) > maxValue, "数字必需小于等于" + maxValue);
	}
}

//判断某年是否为闰年
function isleapYear(nYear)
{
	var thisYear = nYear * 1;

	return ((thisYear % 4 == 0) && !(thisYear % 100 == 0)) || (thisYear % 400 == 0);
}

function dateCheck(dr)
{
	var nLen = dr.length;

	//trueThrow(dr.length == 0, "必需输入内容");

	var datePart = new Array("", "", "");
	var nSegment = 0;

	//过滤掉时间部分
	var nFirstTimeSept = -1;
	if (arguments[1] == true)
	{
		nFirstTimeSept = nLen;
	}
	else
	{
		for (var i = 0; i < nLen; i++)
			if (dr.substr(i, 1) == ":")
			{
				nFirstTimeSept = i;
				break;
			}
	}
	if (nFirstTimeSept != -1)	//有时间分割符
		for (var i = nFirstTimeSept; i >= 0; i--)
			if (dr.substr(i, 1) == " ")
			{
				nLen = i;
				break;
			}

	for (var i = nLen - 1; i >= 0; i--)
	{
		var ch = dr.substr(i, 1);

		if (ch == "-" || ch == "/" || ch == "." || ch == "\\")
		{
			nSegment++;
		}
		else
		if (ch != " ")
		{
			trueThrow(ch < "0" || ch > "9", "日期中出现非法字符'" + ch + "'");
			trueThrow(nSegment > 2, "日期格式错误，无法正确分清年月日各部分");
				
			if (nSegment < 2)
			{
				if (datePart[nSegment].length == 2)
					nSegment++;
			}
			datePart[nSegment] = ch + datePart[nSegment];
		}
	}

	trueThrow(datePart[0].length == 0, "日期格式错误");
	trueThrow(datePart[1].length == 0, "日期格式错误，没有输入正确的月份");
	trueThrow(datePart[1] * 1 < 1 || datePart[1] * 1 > 12, "日期格式错误，月份必需在1-12月之间");

	var dToday = new Date();
	var strYear = dToday.getYear();
	var daysInMonth = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);

	if (datePart[2].length == 0)
		datePart[2] = strYear;
	else
	{
		if (datePart[2].length == 4)
		{
			trueThrow(datePart[2] < YEAR_MIN, "日期输入错误，年份必需大于" + YEAR_MIN + "年");
			trueThrow(datePart[2] > YEAR_MAX, "日期输入错误，年份必需小于" + YEAR_MAX + "年");
		}
		else
		if (datePart[2].length == 2)
		{
			if (datePart[2] * 1 > 50)
				datePart[2] = "19" + datePart[2];
			else
				datePart[2] = strYear.toString().substr(0, 2) + datePart[2];
		}
		else
			throw "日期格式错误，年的位数必需为两位或四位";
	}

	if (isleapYear(datePart[2]))
		daysInMonth[1] = daysInMonth[1] + 1;

	trueThrow(datePart[0] * 1 < 1 || datePart[0] * 1 > daysInMonth[datePart[1] * 1 - 1],
				"日期输入错误，" + datePart[1] * 1 + "月的日期必需在1-" + daysInMonth[datePart[1] * 1 - 1] + "号之间");

	var dateStr = datePart[2] + "-" + datePart[1].toString() + "-" + datePart[0];
	if (arguments[1] == true && nLen < dr.length-1)
	{
		var timeRead = dr.substring(nLen+1, dr.length);
		dateStr += " "+timeCheck(timeRead);
	}
	return dateStr;
}

function timeCheck(tr)
{ 
	var nLen = tr.length;
	var nSegment = 0;
	var timePart = new Array("", "", "");
	var timeMax = new Array(23, 59, 59);
	var timeAlert = new Array("小时", "分钟", "秒");
	//var timePartlength = (timePart.length > 3) ? 3:timePart.length;

	for (var i = 0; i < nLen ;i++ )
	{
		if (!isNaN(tr.substr(i, 1)*1))
		{
			if (nSegment > 2)
			{
				throw("时间输入分隔符过多！");
			}				
			timePart[nSegment] += tr.substr(i, 1);
		}
		else
		{
			nSegment++;
		}
	}

	if (nSegment == 0)
	{
		timePart[0] = "";
		for (var i = 0; i < nLen ;i++ )
		{
			if ((timePart[nSegment] + tr.substr(i, 1)) * 1 <= timeMax[nSegment])
			{
				timePart[nSegment] += tr.substr(i, 1);
			}
			else
			{
				nSegment++;
				if (nSegment > 2)
				{
					throw(timeAlert[2] + "数输入大于 “ " + timeMax[2] + " ”");
				}				
				timePart[nSegment] += tr.substr(i, 1);
			}
		}
	}

	for (var i = 0; i < 3; i++)
	{
		if (timePart[i] == "")
		{
			timePart[i] = "00";
		}
		else
		{
			if (timePart[i] * 1 > timeMax[i])
			{
				throw(timeAlert[i] + "数输入大于 “ " + timeMax[i] + " ”");
			}
			if (timePart[i].length == 1)
			{
				timePart[i] = "0" + timePart[i];
			}
		}
	}

	return timePart[0] + ":" + timePart[1] + ":" +timePart[2];

}

//将一个以字符串格式表示的日期转化为日期类型
function strToDate(strDate)
{
	trueThrow(strDate.length == 0, "需要转化为日期的字符串不能为空");

	var vDateTime = strDate.split(" ");	//区分日期和时间
	var vDate = vDateTime[0].split("-");

	var dt = new Date(vDate[0], vDate[1] - 1, vDate[2]);
	
	return dt;
}

function strToDateTime(strDate)
{
	trueThrow(strDate.length == 0, "需要转化为日期的字符串不能为空");

	var vDateTime = strDate.split(" ") ;	//区分日期和时间
	var vDate = vDateTime[0].split("-");
	var vTime = vDateTime[1].split(":");
	var dt = new Date(vDate[0], vDate[1] - 1, vDate[2], vTime[0], vTime[1], vTime[2]);
	
	return dt;
}

//为当前日期加上一个毫秒数，返回一个新的日期
function dateAdd(dt, ms)
{
	var dt1 = dt * 1;
	
	return new Date(dt1 + ms);
}

//得出两个日期的差(dt1-dt2)，ms请参照文件头的定义(毫秒)
function dateDiff(dt1, dt2, ms)
{
	var diff = dt1 * 1 - dt2 * 1;

	if (!ms)
		ms = msInDay;

	var nDiff = 0;

	if (diff > 0)
	{
		nDiff = Math.floor(diff / ms);
	}
	else
	{
		if (diff < 0)
		{
			nDiff = Math.ceil(diff / ms);
		}
	}

	return nDiff;
}

//比较两个字符串的日期格式
function compareDate(strDate1, strDate2)
{
	var vDate1 = strDate1.split("-");
	var vDate2 = strDate2.split("-");

	var dt1 = new Date(vDate1[0], vDate1[1], vDate1[2]);
	var dt2 = new Date(vDate2[0], vDate2[1], vDate2[2]);

	var nResult = 0;
	if (dt1 < dt2)
	{
		nResult = -1;
	}
	else
	{
		if (dt1 > dt2)
		{
			nResult = 1;
		}
	}

	return nResult;
}

//去掉日期字符串的时间部分
function removeTime(strDate)
{
	return strDate.split(" ")[0];
}

//将日期格式化成yyyy-m-d
function formatDate(dt)
{
	return dt.getYear() + "-" + (dt.getMonth() + 1) + "-"  + dt.getDate();
}

//将当前日期格式化成yyyy-m-d
function formatToday()
{
	return formatDate(new Date());
}

//将日期格式化成yyyy-m-d hh:mm:ss
function formatDatetime(dt)
{
	return dt.getYear() + "-" + (dt.getMonth() + 1) + "-"  + dt.getDate() + 
			" " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
}

/*************************
//键盘及界面事件处理
/*************************/

//将回车键处理为Tab
function onKeyDownDefault()
{
	if (window.event.keyCode == 13 && window.event.ctrlKey == false && window.event.altKey == false)
	{
		if (window.event.srcElement.type != "button" && window.event.srcElement.tagName != "TEXTAREA")
		{
			window.event.keyCode = 9;
		}
	}
}

//如果某个输入域为整数，那么过滤掉所有非数字字符
function onKeyPressInputInteger()
{
	var nKey = window.event.keyCode;

	if (nKey < 48 || nKey > 57)
	{
		window.event.keyCode = 0;
	}
}

//如果某个输入域为实数，那么过滤掉所有非数字字符（包含小数点）
function onKeyPressInputFloat()
{
	var nKey = window.event.keyCode;

	if (nKey > 57 || (nKey != 46 && nKey < 48))
	{
		window.event.keyCode = 0;
	}
}

//挂在日期输入控件的onChange事件上
function onDateCheck()
{
	var obj;

	if (arguments.length > 0 && typeof(arguments[0]) == "object")
	{
		obj = arguments[0];
	}
	else
	{
		obj = window.event.srcElement;
	}

	var strLenMsg = "";

	try
	{
		var bCheckTime = false;

		if (obj.nodeColumn)
			bCheckTime = obj.nodeColumn.getAttribute("format") == "datetime";

		if (obj.value.length > 0)
		{
			obj.value = dateCheck(obj.value, bCheckTime);
		}

		obj.formatValue = obj.value;
		obj.datatype = DT_DATETIME;

		window.event.returnValue = true;

		return true;
	}
	catch(e)
	{
		window.alert(e);
		window.event.returnValue = false;
		return false;
	}
}

//挂在输入控件的onBlur事件上
function onCheckBlur()
{
	var obj = window.event.srcElement;

	if (obj.value && obj.value.length > 0)
	{
		if (isUndefined(typeof(obj.formatValue)) == false)
		{
			if (obj.formatValue != obj.value)
			{
				obj.value = obj.formatValue;
			}
		}
	}
}

//挂在数字输入控件的onChange事件上
//Invoke Format:onNumericCheck([obj], [format], [minValue], [maxValue])
function onNumericCheck()
{
	var obj;
	var nParamStart = 0;

	if (arguments.length > 0 && typeof(arguments[0]) == "object")
	{
		obj = arguments[0];
		nParamStart = 1;
	}
	else
		obj = window.event.srcElement;

	var strFormat = STD_NUMBER_FORMAT;
	var minValue = "0";
	var maxValue = "1000000000";

	if (nParamStart < arguments.length)
	{
		strFormat = arguments[nParamStart];
	}

	if (nParamStart + 1 < arguments.length)
	{
		minValue = arguments[nParamStart + 1];
	}

	if (nParamStart + 2 < arguments.length)
	{
		maxValue = arguments[nParamStart + 2];
	}

	try
	{
		var dblValue = filterChar(obj.value, ",");

		numericCheck(dblValue, 1000, 1000, minValue, maxValue);

		formatNumberObj(obj, strFormat, dblValue);

		return true;
	}
	catch(e)
	{
		window.alert(e);
		return false;
	}
}

function isXSDIntType(strType)
{
	var bResult = false;

	switch(strType)
	{
		case "int":
		case "integer": 
		case "long":
		case "short":
		case "unsignedInt":
		case "unsignedLong":
		case "unsignedShort":	bResult = true;
								break;
	}

	return bResult;
}

function isXSDNumericType(strType)
{
	var bResult = false;

	switch(strType)
	{
		case "double":
		case "decimal":
		case "float":	bResult = true;
						break;
	}

	return bResult;
}

function isXSDStringType(strType)
{
	var bResult = false;

	switch(strType)
	{
		case "string":	bResult = true;
						break;
	}

	return bResult;
}

function isXSDDatetimeType(strType)
{
	var bResult = false;

	switch(strType)
	{
		case "dateTime":	bResult = true;
							break;
	}

	return bResult;
}

//当界面上述据字典中的编辑项被改变时，激活此事件
function onDictFieldChange()
{
	var obj = window.event.srcElement;
	var bResult = true;

	if (isUndefined(typeof(obj.nodeColumn)) == false)
	{
		var nodeColumn = obj.nodeColumn;
		var strType = getXSDColumnAttr(nodeColumn, "type");

		strType = strType.split(":")[1];

		if (isXSDIntType(strType))
		{	
			bResult = onNumericCheck(obj, STD_AMOUNT_FORMAT);
		}
		else
		{
			if (isXSDNumericType(strType))
			{
				bResult = onNumericCheck(obj, STD_NUMBER_FORMAT);
			}
			else
			{
				if (isXSDDatetimeType(strType))
				{
					bResult = onDateCheck(obj);
				}
			}
		}
	}

	return bResult;
}

function checkNullObj(obj)
{
	var strCaption = "";

	if (isUndefined(typeof(obj.nodeColumn)) == false)
	{
		if (obj.value.length == 0)
		{
			var strAllowNull = getXSDColumnAttr(obj.nodeColumn, "allowNull");

			if (strAllowNull == "false")
			{
				strCaption = getXSDColumnAttr(obj.nodeColumn, "caption");
			}
		}
	}

	return strCaption;
}

//检查那些输入项不能为空
function checkInputNull(checkElement)
{
	var strResult = "";
	var nCounter = 0;

	var allCheck = document.all;
	if (checkElement)
		allCheck = checkElement;
	for (var i = 0; i < allCheck.length; i++)
	{
		var obj = allCheck[i];
		
		var strCaption = "";

		if (obj.tagName == "INPUT" && (obj.type == "text" || obj.type == "file"))
		{
			strCaption = checkNullObj(obj);
		}
		else
		{
			if (obj.tagName == "SELECT")
			{
				strCaption = checkNullObj(obj);
			}
		}

		if (strCaption.length > 0)
		{
			if (strResult.length > 0)
			{
				strResult += ", ";
			}

			strResult += strCaption;

			nCounter++;
		}
	}

	var strChar = "";

	if (nCounter > 0)
	{
		if (nCounter > 1)
			strChar = "这些";
		else
			strChar = "此";

		strResult += ":" + strChar + "域不能为空";

		throw strResult;
	}
}

//检查某些id的控件是否为空
function checkNullID(idArray)
{
	var i = 0;
	var strResult = "";
	var nCounter =0;
	var oFirstElem = null;

	while (i < idArray.length)
	{
		var obj = document.all(idArray[i]);

		var value = rTrim(obj.value);

		if (value.length == 0)
		{
			if (strResult.length > 0)
			{
				strResult += ",";
			}
			
			strResult += idArray[i + 1];

			if (oFirstElem == null)
			{
				oFirstElem = obj;
			}

			nCounter++;
		}
		i += 2;
	}

	var strChar = "";

	if (nCounter > 1)
	{
		strChar = "这些";
	}
	else
	{
		strChar = "此";
	}
	
	if (strResult.length > 0)
	{
		strResult += ":" + strChar + "域不能为空";
	}

	if (oFirstElem)
	{
		oFirstElem.focus();
	}

	return strResult;
}

function checkNullIDThrow(idArray)
{
	var strResult = checkNullID(idArray);

	if (strResult.length > 0)
	{
		throw strResult;
	}
}

//将字符串的右空格去掉
function rTrim(str)
{
	var nLen = str.length;
	
	for (var i = str.length - 1; i >= 0; i--)
	{
		if (str.substr(i, 1) != " ")
		{
			break;
		}
		else
		{
			nLen--;
		}
	}
	
	return str.substr(0, nLen);	
}

//根据数据字典初始化界面上的元素
function initElementsByDict(xmlDict, dictElement)
{
	var fieldRoot = getXSDColumnsRoot(xmlDict);

	var allInit = document.all;
	if (dictElement)
	{
		allInit = dictElement;
	}
		
	for (var i = 0; i < allInit.length; i++)
	{
		var obj = allInit(i);

		if ((obj.tagName == "INPUT" && obj.type == "text") || (obj.tagName == "TEXTAREA") || (obj.tagName == "SELECT") || (obj.tagName == "DNINPUT"))
		{
			if (isUndefined(typeof(obj.dataFld)) == false)
			{
				var nodeColumn = getXSDColumnFromRoot(fieldRoot, obj.dataFld);

				if (nodeColumn != null)
				{
					obj.nodeColumn = nodeColumn;

					if (obj.onchange == null)
					{
						obj.onchange = onDictFieldChange;
					}

					var strImeMode = getXSDColumnAttr(nodeColumn, "imeMode");

					if (strImeMode.length > 0)
					{
						obj.style.imeMode = strImeMode;
					}
					var strType = getXSDColumnAttr(nodeColumn, "type");

					var strType = strType.split(":")[1];

					if (obj.onblur == null && obj.onchange == null)
					{
						if (isXSDDatetimeType(strType) || isXSDNumericType(strType) || isXSDIntType(strType))
						{
							obj.onblur = onCheckBlur;
						}
					}

					if (obj.onkeypress == null)
					{
						if (isXSDIntType(strType))
						{
							obj.onkeypress = onKeyPressInputInteger;
							obj.style.textAlign = "right";
						}
						else
						{
							if (isXSDNumericType(strType))
							{
								obj.onkeypress = onKeyPressInputFloat;
								obj.style.textAlign = "right";
							}
						}
					}

					if (isXSDStringType(strType))
					{
						var strLength = getXSDColumnAttr(nodeColumn, "size");
						
						if (strLength.length > 0)
						{
							obj.maxLength = parseInt(strLength);
						}
					}
				}				
			}
		}
	}
}

//格式化数字
function formatNumberObj(obj, strFmt)
{
	var dblValue;

	if (arguments.length > 2)
		dblValue = arguments[2];
	else
		dblValue = filterChar(obj.value, ",");

	if (!isNaN(dblValue))
	{
		if (!(typeof(dblValue == "string") && dblValue.length == 0))
		{
			obj.originalValue = dblValue;
			obj.datatype = DT_NUMBER;
			obj.value = formatNumber(dblValue, strFmt);
			obj.formatValue = obj.value;
		}
	}
}

//以下为格式化数字程序
function statCharCount(str,chr)
{
	var aMatch;
	var strPattern	= chr;
	var nI;

	for (nI = 2; nI < arguments.length; nI++)
		strPattern = strPattern.concat("|",arguments[nI]);
	
	aMatch = str.match(new RegExp(strPattern,"g"));
	
	if (aMatch == null)
		return 0;
	else
		return aMatch.length;
}

function paramFmt(strFmt)
{
	var objFmt = new Object();
	var nPointPos;
	var nZeroPos;
	
	nPointPos = strFmt.search(/\./g);
	if (nPointPos < 0)
		nPointPos = strFmt.length

	//确定格式串的整数部分、小数部分及舍入位数；
	objFmt.IntPart		= strFmt.substr(0,nPointPos);
	objFmt.DecPart		= strFmt.substr(nPointPos + 1);
	objFmt.DecRndCount	= statCharCount(objFmt.DecPart,'0','#');

	//确定整数部分的最小长度；
	with (objFmt.IntPart)
	{	nZeroPos = search(/0/g);
		if (nZeroPos < 0)
			nZeroPos = length;
			
		objFmt.IntFixLen = statCharCount(substr(nZeroPos),'0','#');
	}		
	
	//确定小数部分的最小长度；
	with (objFmt.DecPart)
	{	var aMatch = match(/0/g);
		if (aMatch == null)
			objFmt.DecFixLen = 0
		else
			objFmt.DecFixLen = statCharCount(substr(0,aMatch.lastIndex),'0','#');
	}

	return objFmt;
}

function getRidOfChar(str,chr)
{
	var aBuf;
	var strBuf	= "";
	var nI;
	
	aBuf = str.split(",");
	
	if (aBuf != null)
	{	for (nI = 0; nI < aBuf.length; nI++)
			strBuf = strBuf + aBuf[nI];
		return strBuf;
	}
	else
		return "";
}

function numCommaSplit(str)
{
	var nI;
	var strBuf = "";
	var bNegative = false;
	//if negative get rid of "-"
	if (str.substr(0, 1) == "-")
	{
		str = str.substr(1);
		bNegative = true;
	}
	
	str = getRidOfChar(str,',');
	
	for (nI = str.length - 3; nI > 0; nI -= 3)
		strBuf = "," + str.substr(nI,3) + strBuf;

	strBuf = str.substr(0,3 + nI) + strBuf;
	
	//if Negative add "-"
	if(bNegative)
		strBuf = "-" + strBuf;
		
	return strBuf;
}

function formatNumber(strExp,strFmt)
{
	var	vResult;
	var objFmt;
	var nI,nJ;
	
	if(isNaN(strExp))
		throw "Parameter1 is not numeric.";
	
	vResult = new Number(strExp);
	objFmt	= paramFmt(strFmt);

	//根据格式串定义，对vResult进行舍入；结果整数部分->strIntPrt，小数部分->strDecPart；
	vResult *= Math.pow(10,objFmt.DecRndCount);
	vResult = Math.round(vResult);
	vResult = vResult.toString();

	var strIntPart,strDecPart;

	with (vResult)
	{	strIntPart = substr(0, length - objFmt.DecRndCount);
		strDecPart = substr(length - objFmt.DecRndCount, objFmt.DecRndCount);
		//去除小数部分尾部多余的零；
		strDecPart = (Math.pow(10,-objFmt.DecRndCount) * strDecPart.valueOf()).toString();
		strDecPart = strDecPart.substr(2,objFmt.DecRndCount);
	}
		
	//定长整数部分(前补零)及小数部分(后补零)；
	for (nI = strIntPart.length; nI < objFmt.IntFixLen; nI++)
		strIntPart = "0" + strIntPart;
	
	for (nI = strDecPart.length; nI < objFmt.DecFixLen; nI++)
		strDecPart = strDecPart + "0";

	//整数部分根据格式串就位；
	var strBuf,aBuf;

	strBuf	= objFmt.IntPart.replace(/#/g,"0");
	aBuf	= strBuf.split("0");
	if (aBuf.length)
	{	strBuf = aBuf[aBuf.length - 1];
		for (nI = aBuf.length - 2,nJ = strIntPart.length -1; nI >= 0; nI--, nJ--)
			strBuf = aBuf[nI] + strIntPart.charAt(nJ) + strBuf;

		if (nJ >= 0)
			strBuf = strIntPart.substr(0,nJ + 1) + strBuf;

		strIntPart = strBuf;
	}

	//小数部分根据格式串就位；
	strBuf	= objFmt.DecPart.replace(/#/g,"0");
	aBuf	= strBuf.split("0");
	if (aBuf.length)
	{	strBuf = aBuf[0];
		for (nI = 1,nJ = 0; nI < aBuf.length; nI++, nJ++)
			strBuf = strBuf + strDecPart.charAt(nJ) +  aBuf[nI];

		if (nJ < strDecPart.length)
			strBuf = strBuf + strIntPart.substr(nJ);
			
		strDecPart = strBuf;
	}

	//处理逗分数值表示法；
	if (strIntPart.search(/,/g) >= 0)
		strIntPart = numCommaSplit(strIntPart);

	//合并整数部分和小数部分，返回；
	if (strDecPart.length > 0)
		strDecPart = "." + strDecPart;
	
	return strIntPart + strDecPart;
}

//当某个输入项处于激活状态
function onElementActive()
{
	var newElement = window.document.activeElement;

	try
	{
		if (m_oldElement != null)
			m_oldElement.className = m_oldElementClass;

		if (newElement)
			if (newElement.tagName == "INPUT")
			{
				if ((newElement.type == "text" || newElement.type == "textarea" || 
						newElement.type == "file" || newElement.type == "password"))
					if (newElement.readOnly == false)
					{
						
						m_oldElementClass = newElement.className;
						m_oldElement = newElement;
						newElement.className = "activeElement";
					}
			}
			if (newElement.tagName == "SELECT")
			{
				m_oldElementClass = newElement.className;
				m_oldElement = newElement;
			}
	}
	catch(e)
	{
		showError(e);
	}
}

//缺省的事件处理函数
function initDocumentEvents(initElement)
{
	m_oldElement = null;
	m_oldElementClass = "";

	if (document.body.onkeydown == null)
		document.body.onkeydown = onKeyDownDefault;
	
	var allInit = document.all;
	if (initElement)
		allInit = initElement;
		
	for (var i = 0; i < allInit.length; i++)
	{
		var obj = allInit(i);

		if (obj.tagName == "INPUT" && 
			(obj.type == "text" || obj.type == "textarea" || obj.type == "file" || obj.type == "password"))
		{
			if (obj.onfocus == null)
				obj.onfocus = onElementActive;

		}
		else
		if (obj.tagName == "SELECT")
		{
			if (obj.onfocus == null)
				obj.onfocus = onElementActive;
		}
	}
}

function getOwnerTag(element, strTag)
{
	while (element.tagName && element.tagName.toUpperCase() != strTag.toUpperCase())
	{
		element = element.parentNode;
		if (element == null)
			break;
	}

	return(element);
}

function getChildTag(element, strTag)
{
	var nodeResult = null;

	if (element)
	{
		for (var i = 0; i < element.childNodes.length; i++)
		{
			var node = element.childNodes(i);
	
			if (node.tagName.toUpperCase() == strTag.toUpperCase())
			{
				nodeResult = node;
				break;
			}
		}
	
		if (!nodeResult)
		{		
			for (var i = 0; i < element.childNodes.length; i++)
			{
				nodeResult = getChildTag(element.childNodes(i));
		
				if (nodeResult)
					break;
			}
		}
	}

	return nodeResult;
}

//判断数据是否在数组中
function dataInArray(data, arrayObj)
{
	var bResult = false;

	for (var i = 0; i < arrayObj.length; i++)
	{
		if (data == arrayObj[i])
		{
			bResult = true;
			break;
		}
	}

	return bResult;
}
//-->