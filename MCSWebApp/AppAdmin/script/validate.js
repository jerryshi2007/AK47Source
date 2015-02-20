<!--
var STD_NUMBER_FORMAT = "#,##0.00";
var STD_RATIO_FORMAT = "0.00####";
var STD_COUNT_FORMAT = "#,##0.00####";
var STD_AMOUNT_FORMAT = "#,##0";
var STD_YEAR_FORMAT = "###0";
var STD_MONTH_FORMAT = "#00";

var YEAR_MAX = "2199";
var YEAR_MIN = "1900";

//�������ͳ���
var DT_STRING = "string";
var DT_NUMBER = "number";
var DT_DATETIME = "datetime";

var msInDay = 86400000;	//ÿ�������
var msInHour = 3600000;	//ÿСʱ�ĺ�����
var msInMinute = 60000;	//ÿ���ӵĺ�����
var msInSecond =  1000;	//ÿ��ĺ�����

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

//���˵�ָ���ַ�
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
	//trueThrow(nr.length == 0, "������������");

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
					throw "��������ֻ����һ��С����";
				else
					nPointIndex = i;
			}
			else
			if (ch != ",")	//���˵�����
				throw "��������Ϸ�������";
		}
	}

	if (nPointIndex == -1)
		nPointIndex = nr.length;

	if (nArgs > 1)	//������������1
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
		 "�������ֵ�λ�����ܳ���" + intDigit + "λ");

	var strFrac = nr.substring(nPointIndex + 1, nr.length);

	if (strFrac.length > 0)
	{
		strFrac = "0." + strFrac;
		trueThrow(typeof(fracDigit) != "undefined" && (strFrac * 1).toString().length - 2 > fracDigit,
			"С�����ֵ�λ�����ܳ���" + fracDigit + "λ");
	}

	if (typeof(minValue) != "undefined" && typeof(maxValue) != "undefined")
	{
		trueThrow((nr * 1) < minValue || (nr * 1) > maxValue, "���ֱ�����" + minValue + "��" + maxValue + "֮��");
	}
	else
	if (typeof(minValue) != "undefined")
	{
		trueThrow((nr * 1) < minValue, "���ֱ�����ڵ���" + minValue);
	}
	else
	if (typeof(maxValue) != "undefined")
	{
		trueThrow((nr * 1) > maxValue, "���ֱ���С�ڵ���" + maxValue);
	}
}

//�ж�ĳ���Ƿ�Ϊ����
function isleapYear(nYear)
{
	var thisYear = nYear * 1;

	return ((thisYear % 4 == 0) && !(thisYear % 100 == 0)) || (thisYear % 400 == 0);
}

function dateCheck(dr)
{
	var nLen = dr.length;

	//trueThrow(dr.length == 0, "������������");

	var datePart = new Array("", "", "");
	var nSegment = 0;

	//���˵�ʱ�䲿��
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
	if (nFirstTimeSept != -1)	//��ʱ��ָ��
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
			trueThrow(ch < "0" || ch > "9", "�����г��ַǷ��ַ�'" + ch + "'");
			trueThrow(nSegment > 2, "���ڸ�ʽ�����޷���ȷ���������ո�����");
				
			if (nSegment < 2)
			{
				if (datePart[nSegment].length == 2)
					nSegment++;
			}
			datePart[nSegment] = ch + datePart[nSegment];
		}
	}

	trueThrow(datePart[0].length == 0, "���ڸ�ʽ����");
	trueThrow(datePart[1].length == 0, "���ڸ�ʽ����û��������ȷ���·�");
	trueThrow(datePart[1] * 1 < 1 || datePart[1] * 1 > 12, "���ڸ�ʽ�����·ݱ�����1-12��֮��");

	var dToday = new Date();
	var strYear = dToday.getYear();
	var daysInMonth = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);

	if (datePart[2].length == 0)
		datePart[2] = strYear;
	else
	{
		if (datePart[2].length == 4)
		{
			trueThrow(datePart[2] < YEAR_MIN, "�������������ݱ������" + YEAR_MIN + "��");
			trueThrow(datePart[2] > YEAR_MAX, "�������������ݱ���С��" + YEAR_MAX + "��");
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
			throw "���ڸ�ʽ�������λ������Ϊ��λ����λ";
	}

	if (isleapYear(datePart[2]))
		daysInMonth[1] = daysInMonth[1] + 1;

	trueThrow(datePart[0] * 1 < 1 || datePart[0] * 1 > daysInMonth[datePart[1] * 1 - 1],
				"�����������" + datePart[1] * 1 + "�µ����ڱ�����1-" + daysInMonth[datePart[1] * 1 - 1] + "��֮��");

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
	var timeAlert = new Array("Сʱ", "����", "��");
	//var timePartlength = (timePart.length > 3) ? 3:timePart.length;

	for (var i = 0; i < nLen ;i++ )
	{
		if (!isNaN(tr.substr(i, 1)*1))
		{
			if (nSegment > 2)
			{
				throw("ʱ������ָ������࣡");
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
					throw(timeAlert[2] + "��������� �� " + timeMax[2] + " ��");
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
				throw(timeAlert[i] + "��������� �� " + timeMax[i] + " ��");
			}
			if (timePart[i].length == 1)
			{
				timePart[i] = "0" + timePart[i];
			}
		}
	}

	return timePart[0] + ":" + timePart[1] + ":" +timePart[2];

}

//��һ�����ַ�����ʽ��ʾ������ת��Ϊ��������
function strToDate(strDate)
{
	trueThrow(strDate.length == 0, "��Ҫת��Ϊ���ڵ��ַ�������Ϊ��");

	var vDateTime = strDate.split(" ");	//�������ں�ʱ��
	var vDate = vDateTime[0].split("-");

	var dt = new Date(vDate[0], vDate[1] - 1, vDate[2]);
	
	return dt;
}

function strToDateTime(strDate)
{
	trueThrow(strDate.length == 0, "��Ҫת��Ϊ���ڵ��ַ�������Ϊ��");

	var vDateTime = strDate.split(" ") ;	//�������ں�ʱ��
	var vDate = vDateTime[0].split("-");
	var vTime = vDateTime[1].split(":");
	var dt = new Date(vDate[0], vDate[1] - 1, vDate[2], vTime[0], vTime[1], vTime[2]);
	
	return dt;
}

//Ϊ��ǰ���ڼ���һ��������������һ���µ�����
function dateAdd(dt, ms)
{
	var dt1 = dt * 1;
	
	return new Date(dt1 + ms);
}

//�ó��������ڵĲ�(dt1-dt2)��ms������ļ�ͷ�Ķ���(����)
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

//�Ƚ������ַ��������ڸ�ʽ
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

//ȥ�������ַ�����ʱ�䲿��
function removeTime(strDate)
{
	return strDate.split(" ")[0];
}

//�����ڸ�ʽ����yyyy-m-d
function formatDate(dt)
{
	return dt.getYear() + "-" + (dt.getMonth() + 1) + "-"  + dt.getDate();
}

//����ǰ���ڸ�ʽ����yyyy-m-d
function formatToday()
{
	return formatDate(new Date());
}

//�����ڸ�ʽ����yyyy-m-d hh:mm:ss
function formatDatetime(dt)
{
	return dt.getYear() + "-" + (dt.getMonth() + 1) + "-"  + dt.getDate() + 
			" " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
}

/*************************
//���̼������¼�����
/*************************/

//���س�������ΪTab
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

//���ĳ��������Ϊ��������ô���˵����з������ַ�
function onKeyPressInputInteger()
{
	var nKey = window.event.keyCode;

	if (nKey < 48 || nKey > 57)
	{
		window.event.keyCode = 0;
	}
}

//���ĳ��������Ϊʵ������ô���˵����з������ַ�������С���㣩
function onKeyPressInputFloat()
{
	var nKey = window.event.keyCode;

	if (nKey > 57 || (nKey != 46 && nKey < 48))
	{
		window.event.keyCode = 0;
	}
}

//������������ؼ���onChange�¼���
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

//��������ؼ���onBlur�¼���
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

//������������ؼ���onChange�¼���
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

//�������������ֵ��еı༭��ı�ʱ��������¼�
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

//�����Щ�������Ϊ��
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
			strChar = "��Щ";
		else
			strChar = "��";

		strResult += ":" + strChar + "����Ϊ��";

		throw strResult;
	}
}

//���ĳЩid�Ŀؼ��Ƿ�Ϊ��
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
		strChar = "��Щ";
	}
	else
	{
		strChar = "��";
	}
	
	if (strResult.length > 0)
	{
		strResult += ":" + strChar + "����Ϊ��";
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

//���ַ������ҿո�ȥ��
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

//���������ֵ��ʼ�������ϵ�Ԫ��
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

//��ʽ������
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

//����Ϊ��ʽ�����ֳ���
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

	//ȷ����ʽ�����������֡�С�����ּ�����λ����
	objFmt.IntPart		= strFmt.substr(0,nPointPos);
	objFmt.DecPart		= strFmt.substr(nPointPos + 1);
	objFmt.DecRndCount	= statCharCount(objFmt.DecPart,'0','#');

	//ȷ���������ֵ���С���ȣ�
	with (objFmt.IntPart)
	{	nZeroPos = search(/0/g);
		if (nZeroPos < 0)
			nZeroPos = length;
			
		objFmt.IntFixLen = statCharCount(substr(nZeroPos),'0','#');
	}		
	
	//ȷ��С�����ֵ���С���ȣ�
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

	//���ݸ�ʽ�����壬��vResult�������룻�����������->strIntPrt��С������->strDecPart��
	vResult *= Math.pow(10,objFmt.DecRndCount);
	vResult = Math.round(vResult);
	vResult = vResult.toString();

	var strIntPart,strDecPart;

	with (vResult)
	{	strIntPart = substr(0, length - objFmt.DecRndCount);
		strDecPart = substr(length - objFmt.DecRndCount, objFmt.DecRndCount);
		//ȥ��С������β��������㣻
		strDecPart = (Math.pow(10,-objFmt.DecRndCount) * strDecPart.valueOf()).toString();
		strDecPart = strDecPart.substr(2,objFmt.DecRndCount);
	}
		
	//������������(ǰ����)��С������(����)��
	for (nI = strIntPart.length; nI < objFmt.IntFixLen; nI++)
		strIntPart = "0" + strIntPart;
	
	for (nI = strDecPart.length; nI < objFmt.DecFixLen; nI++)
		strDecPart = strDecPart + "0";

	//�������ָ��ݸ�ʽ����λ��
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

	//С�����ָ��ݸ�ʽ����λ��
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

	//��������ֵ��ʾ����
	if (strIntPart.search(/,/g) >= 0)
		strIntPart = numCommaSplit(strIntPart);

	//�ϲ��������ֺ�С�����֣����أ�
	if (strDecPart.length > 0)
		strDecPart = "." + strDecPart;
	
	return strIntPart + strDecPart;
}

//��ĳ��������ڼ���״̬
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

//ȱʡ���¼�������
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

//�ж������Ƿ���������
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