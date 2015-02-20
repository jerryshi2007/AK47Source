<!--//ʱ����غ�������

var DATEPART = new datePartEnum();
function datePartEnum()
{
	this.YEAR = 1;
	this.MONTH = 2;
	this.DAY = 4;
	this.HOUR = 8;
	this.MINUTE = 16;
	this.SECOND = 32;
	this.DATE = this.YEAR | this.MONTH | this.DAY;
	this.TIME = this.HOUR | this.MINUTE | this.SECOND;
	this.DATETIME = this.DATE | this.TIME;
}

var LANGUAGE = new languageEnum();
function languageEnum()
{
	this.NONE = 0;
	this.CN = 1;
}

function datePostfixEnum(language)
{
	if (!language)
		language = LANGUAGE.NONE;
	switch (language)
	{
		case LANGUAGE.NONE:
			this.YEAR = "-";
			this.MONTH = "-";
			this.DAY = "";
			this.HOUR = ":";
			this.MINUTE = ":";
			this.SECOND = "";
			break;
		case LANGUAGE.CN:
			this.YEAR = "��";
			this.MONTH = "��";
			this.DAY = "��";
			this.HOUR = "ʱ";
			this.MINUTE = "��";
			this.SECOND = "��";
			break;
		default:
			throw "Language Type error(function datePostfixEnum)";
			break;
	}
}

function dateToStr(dateObj, datePart, language)
{
	if (!datePart)	
		datePart = DATEPART.DATETIME;
	if (!language)
		language = LANGUAGE.NONE;
	var datePostfix = new datePostfixEnum(language);
	var dateStr = "";
	dateStr += datePart & DATEPART.YEAR ? dateObj.getFullYear() + datePostfix.YEAR: "";
	dateStr += datePart & DATEPART.MONTH ? ((dateObj.getMonth() + 1) > 9 ? (dateObj.getMonth() + 1) : "0" + (dateObj.getMonth() + 1)) + datePostfix.MONTH : "";
	dateStr += datePart	& DATEPART.DAY ? (dateObj. getDate() > 9 ? dateObj.getDate() : "0" + dateObj.getDate()) + datePostfix.DAY + " " : "";
	dateStr += datePart & DATEPART.HOUR ? (dateObj.getHours() > 9 ? dateObj.getHours() : "0" + dateObj.getHours()) + datePostfix.HOUR : "";
	dateStr += datePart & DATEPART.MINUTE ? (dateObj.getMinutes() > 9 ? dateObj.getMinutes() : "0" + dateObj.getMinutes()) + datePostfix.MINUTE  : "";
	dateStr += datePart & DATEPART.SECOND ? (dateObj.getSeconds() > 9 ? dateObj.getSeconds() : "0" + dateObj.getSeconds()) + datePostfix.SECOND : "";
	
	if (dateStr && dateStr.charAt(dateStr.length - 1) == " ")
		dateStr = dateStr.substring(0, dateStr.length - 1);
	return dateStr;
}

function dateToCnStr(dateObj, datePart)
{
	return dateToStr(dateObj, datePart, LANGUAGE.CN);
}

//������������ַ���
function getCnDateStr(dateObj, datePart)
{
	if (typeof(dateObj) == "string")
		return dateStrToCnStr(dateObj, datePart);
	else if(typeof(dateObj) == "object")
		return dateToCnStr(dateObj, datePart)
	else
		throw "parameter format is error(function getCnDateStr)";
}
		
//ת�������������ַ���
function dateStrToCnStr(dateStr, datePart)
{
	var cnStr = "";
	var dt = dateStr.split(" ");
	var dateStr = dt[0];
	var timeStr = dt[1];

	if (!datePart)	
		datePart = DATEPART.DATETIME;
	var datePostfix = new datePostfixEnum(LANGUAGE.CN);
	
	var arrayDate = dateStr.split("-");
	
	if (arrayDate[0] && datePart & DATEPART.YEAR)
		cnStr += arrayDate[0] + datePostfix.YEAR;
	if (arrayDate[1] && datePart & DATEPART.MONTH)
		cnStr += arrayDate[1] + datePostfix.MONTH;
	if (arrayDate[2] && datePart & DATEPART.DAY)
		cnStr += arrayDate[2] + datePostfix.DAY;
	
	cnStr += " ";
	
	if (timeStr)
	{
		var arrayTime = timeStr.split(":");
		
		if (arrayTime[0] && datePart & DATEPART.HOUR)
			cnStr += arrayTime[0] + datePostfix.HOUR;
		if (arrayTime[1] && datePart & DATEPART.MINUTE)
			cnStr += arrayTime[1] + datePostfix.MINUTE;
		if (arrayDate[2] && datePart & DATEPART.SECOND)
			cnStr += arrayTime[2] + datePostfix.SECOND;			
	}
	
	if (cnStr && cnStr.charAt(dateStr.length - 1) == " ")
		cnStr = cnStr.substring(0, cnStr.length - 1);
		
	return cnStr;
}

//��ø�URL
function getHttpRootDir(strURL)
{
	return strURL.substring(0, strURL.indexOf("/", strURL.indexOf("/", 7) + 1));
}

//�������
function AddDayDate(dateObj, nDay)
{
	return dateAdd(dateObj, nDay * msInDay);
}

//ĳ����ʼ��
function GetMonthStartDate(dateObj)
{
	var year = dateObj.getFullYear();
	var month = dateObj.getMonth();
	
	return new Date(year, month, 1);
}

//������ʼ��
function GetThisMonthStartDate()
{
	return GetMonthStartDate(new Date());
}

//������ʼ���ַ���
function GetThisMonthStartDateStr()
{
	return dateToStr(GetThisMonthStartDate());
}

//ĳ�½�����
function GetMonthEndDate(dateObj)
{
	var year = dateObj.getFullYear();
	var month = dateObj.getMonth();
	
	var endDate = new Date(year, month, 28);
	
	var nextDate = AddDayDate(endDate, 1);
	while (endDate.getMonth() == nextDate.getMonth())
	{
		endDate = nextDate;
		nextDate = AddDayDate(endDate, 1);
	}
	
	return endDate;
}

//���½�����
function GetThisMonthEndDate()
{
	return GetMonthEndDate(new Date());
}

//���½������ַ���
function GetThisMonthEndDateStr()
{
	return dateToStr(GetThisMonthEndDate());
}

//ĳ����ʼ��
function GetYearStartDate(dateObj)
{
	var year = dateObj.getFullYear();
	
	return new Date(year, 0, 1);
}

//������ʼ��
function GetThisYearStartDate()
{
	return GetYearStartDate(new Date());
}

//������ʼ���ַ���
function GetThisYearStartDateStr()
{
	return dateToStr(GetThisYearStartDate());
}

//ĳ�������
function GetYearEndDate(dateObj)
{
	var year = dateObj.getFullYear();
	
	return new Date(year, 11, 31);
}

//���������
function GetThisYearEndDate()
{
	return GetYearEndDate(new Date());
}

//����������ַ���
function GetThisYearEndDateStr()
{
	return dateToStr(GetThisYearEndDate());
}
//-->