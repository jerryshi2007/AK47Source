<!--/**** Forever 个性化设置函数***/
/****************************状态栏上展现时间*********************************/
var timerID = null;
var timerRunning = false;

function stopclock ()
{
	if(timerRunning)
		clearTimeout(timerID);
	timerRunning = false;
}

function showtime ()
{
	var now = new Date();
	
	var hours = now.getHours();
	var minutes = now.getMinutes();
	var seconds = now.getSeconds()
	var timeValue = " " + ((hours >12) ? hours - 12 :hours)
	timeValue += ((minutes < 10) ? ":0" : ":") + minutes
	timeValue += ((seconds < 10) ? ":0" : ":") + seconds
	window.status = now.getYear() + "年" + (now.getMonth() + 1) + "月" + now.getDate() + "日" + ((hours >= 12) ? " 下午" : " 上午") + " " + timeValue;
	timerID = setTimeout("showtime()",1000);
	timerRunning = true;
}

function startclock () 
{
	stopclock();
	showtime();
}
//start with startclock()
/*****************************************************************************/


/***********************************动画显示打开新窗口*****************************************/
var winheight = 100;
var winsize = 100;
var x = 5;
var temploc = null;
/*
Animated Window Opener Script (updated 00/04/24)- 
?Dynamic Drive (www.dynamicdrive.com)
For full source code, installation instructions, 100's more DHTML scripts, and Terms Of
Use, visit dynamicdrive.com
*/



function openwindow(thelocation)
{
	var temploc = thelocation;
	
	if (!(window.resizeTo && document.all) && !(window.resizeTo && document.getElementById))
	{
		window.open(thelocation);
		return;
	}
	
	win2 = window.open("","","scrollbars");
	win2.moveTo(0,0);
	win2.resizeTo(100,100);
	go2();
}

function go2()
{
	if (winheight >= screen.availHeight - 3)
		x = 0;
		
	win2.resizeBy(5, x);
	winheight += 5;
	winsize += 5;
	
	if (winsize >= screen.width - 5)
	{
		win2.location = temploc;
		winheight = 100;
		winsize = 100;
		x = 5;
		return;
	}
	setTimeout("go2()",50)
}
//-->
