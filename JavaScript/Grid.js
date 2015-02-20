//function search()
//{
//    var hiddenShow = window.document.getElementById("hiddenShow");
//    if (hiddenShow.value == "hidden")
//    {
//        hiddenShow.value = "show";
//    }
//    else
//    {
//        hiddenShow.value = "hidden";
//    }        
//    show();    
//}
//function show()
//{
//    var hiddenShow = window.document.getElementById("hiddenShow");
//    var layer = document.getElementById("searchLayer"); 
//    if (hiddenShow.value == "show")
//    {
//        layer.style.display="";
//        layer.style.visibility="visible";
//    }
//    else
//    {
//        layer.style.visibility="hidden";
//        layer.style.display="none";
//    }
//}

function popupForm(url, sFeatures)
{
    if (!sFeatures)
        sFeatures = "height=700,width=820,center=yes,status=yes,toolbar=no,menubar=no,location=no,resizable=yes";
    window.open(url, "_blank", sFeatures);
}

function viewProcess(id)
{
    var url =  '/MCSWebApp/ViewProcess/AppTraceProcessTree.aspx?resourceID={0}';
    popupForm(url);
}

//本文件需要和源文件代码页的编码保持一致，否则在处理中文字符时会发生乱码错误

//弹出类别增改页面
function popupCategoryModalDialog(url,sFeatures)
{
    if ("" != trim(url) && null != trim(url))
    {
        if(!sFeatures)
        {
            sFeatures = "dialogHeight: 200px; dialogWidth: 300px; edge: Raised; center: Yes; help: No; status: No;scroll: No;";
        }
        var result = window.showModalDialog(url,"",sFeatures);
        
        //返回值刷新其父窗口
        if (result == 'True')
        {
//            window.location.reload();
            $get("RefreshButton").click();
            refreshCategoryButton();
        }
    }
}

function popupModalDialog(url,sFeatures)
{
    if ("" != trim(url) && null != trim(url))
    {
        if(!sFeatures)
        {
            sFeatures = "dialogHeight: 200px; dialogWidth: 300px; edge: Raised; center: Yes; help: No; status: No;scroll: No;";
        }
        var result = window.showModalDialog(url,"",sFeatures);
        //返回值刷新其父窗口
        //debugger;
        if (result == 'True')
        {
			try
			{
				if(window.parent.frames['navBar'])
				{
					window.parent.frames['navBar'].location.reload();
				}
				else if(window.opener)
				{
					window.opener.parent.frames['navBar'].location.reload();
				}
				$get("RefreshLink").click();
				if(window.opener && window.opener.parent.frames['content'].document.getElementById("RefreshLink"))
					window.opener.parent.frames['content'].document.getElementById("RefreshLink").click();
			}
			catch(e)
			{
				alert(e);
				//window.opener.parent.frames['navBar'].location.reload();
			}
        }
    }
}

function refreshCategoryButton()
{
	if(window.opener && window.opener.document.getElementById("RefreshCategoryButton"))
		window.opener.document.getElementById("RefreshCategoryButton").click();
}

function popupPage(url,sFeatures)
{
    if ("" != trim(url) && null != trim(url))
    {
        if(!sFeatures)
        {
            sFeatures = "height=470px,width=730px,status=no,resizable=no,toolbar=no,menubar=no,location=no,scrollbars=no";
        }
        var result = window.open(url,"",sFeatures);
    }
}

//显示或者隐藏节点的详细信息
function showContent(ID)
{
	whichEl = eval("Content" + ID);
	
	if (whichEl.style.display == "none")
	{
		eval("Content" + ID + ".style.display=\"\";");
		document.getElementById("labtn"+ID).innerHTML="隐藏详细信息";
	}
	else
	{
		eval("Content" + ID + ".style.display=\"none\";");
		document.getElementById("labtn"+ID).innerHTML="显示详细信息";
	}
}

//选择下拉框分隔符不Postback，同时验证是否选择了条目
function dropDownListSelected(gridName)
{
    var result = false;
    
    switch (event.srcElement.value)
    {
		case '$$$ListSeparator$$$':
			break;
		case 'CategoryManage':
			popupPage("UserTaskCategoryManager.aspx");
			break;
		case 'RetryAll':
			result = confirm('您确定要重试转换所有失败项吗?');
			break;
		case 'DeleteSelected':
			result = deleteItem(gridName)
			break;
		default:
			result = checkSelected(gridName);
			break;
	}
	
	if(result)
		$get("RefreshButton").click();
	else
		event.srcElement.selectedIndex = 0;
    
    return result;
}

//判断是否选择了条目
function checkSelected(gridName)
{
    var result = false;
    var grid = $find(gridName);
    if (grid && grid._clientSelectedKeys.length > 0)
    {
        result = true;
    }
    else
    {
        alert('您尚未选择条目');
    }
    return result;
}

//删除判断
function deleteItem(gridName)
{
    var submitFlag = false;
    var grid = $find(gridName);
    if (grid && grid._clientSelectedKeys.length > 0)
    {
        submitFlag = confirm('您确定要删除用户的数据吗?(删除的数据无法恢复!!)');
    }
    else
    {
        alert('您尚未选择删除的条目');
    }
    return submitFlag;
}

function deleteSingle(ret)
{
	if(ret != "0")
		return confirm('是否将属于该分类的' + ret + '条消息去除分类并将此分类删除?');
		
	return confirm('您确定要删除吗?');
}

//限制某些字符的输入
function textChange(obj)
{
    obj.value = obj.value.replace(/[^\d]/g,'');
}

//判断是否非空
function trim(s){
    re = /\s*/g;
    r = s.replace(re,'');
    return r;
}

var _isChecked=false;//必须放在函数外

//* obj:要检查的文本框对象,一般调用的时候请填this检查本身,
//* type:要输入的数据类型,整数请输入integer,小暑请输入double
//* unsigned:限制数据是非负数请填true,允许输入负数请填false

function checkNumber(obj,type,unsigned)
{
	obj.style.imeMode='disabled';
	if(type.toUpperCase()!=new String("integer").toUpperCase())
		type="double";
	else
		type="integer";
		
	if(unsigned)
		unsigned=true;
	else
		unsigned=false;
		
	if(!_isChecked) 
	{
		_isChecked=true;
		var str=new String(obj.value);
		var num=new Number(obj.value);
		var ok=true;
		if(unsigned)
			ok=str.match("-")==null;
		if(type=="integer"&&ok)
			ok=str.match("\\.")==null;
			/* \.是正则表达式中代替小数点的方法，但是因为match函数中要求输入一个字符串参数，所以要用\\来代替\，所以最终用\\.来代替小数点 */
		if(num.toString()=="NaN"&&str!="-")
			ok=false;
		if(obj.value.match("\\-0\\d|\\b^\\.?0\\d"))
			ok=false;//避免输入01,-0000.01之类的数字
		if(!ok)
			obj.value=obj.backupValue==undefined?"":obj.backupValue;
		else
		{
			matchStr=obj.value.match("\\-?\\b[0-9]*\\.?[0-9]*\\b\\.?|\\-");//用正则表达式清空前后空格
			if(matchStr!=obj.value)
			{
				obj.value=matchStr==null?"":matchStr;
				_isChecked=true;
			}
			obj.backupValue=obj.value;
		}
	}
	else
	{
		_isChecked=false;//避免无限递归
		return;
	}
}

//检查邮政编码等纯数字的输入，特点，只能由数字组合而成，该情况下类似00000的组合有意义
function checkSimpleNumber(obj)
{
	obj.style.imeMode='disabled';
		
	if(!_isChecked) 
	{
		_isChecked=true;
		var str=new String(obj.value);
		var num=new Number(obj.value);
		var ok=true;
			ok=str.match("-")==null;
		if(ok)
			ok=str.match("\\.")==null;
			/* \.是正则表达式中代替小数点的方法，但是因为match函数中要求输入一个字符串参数，所以要用\\来代替\，所以最终用\\.来代替小数点 */
		if(num.toString()=="NaN"&&str!="-")
			ok=false;
		if(!ok)
			obj.value=obj.backupValue==undefined?"":obj.backupValue;
		else
		{
			matchStr=obj.value.match("\\-?\\b[0-9]*\\.?[0-9]*\\b\\.?|\\-");//用正则表达式清空前后空格
			if(matchStr!=obj.value)
			{
				obj.value=matchStr==null?"":matchStr;
				_isChecked=true;
			}
			obj.backupValue=obj.value;
		}
	}
	else
	{
		_isChecked=false;//避免无限递归
		return;
	}
}
//function onCommandInputPropertyChange()
//{
//	if (event.propertyName == "value")
//	{
//		var cmdInput = event.srcElement;
//					
//		if (cmdInput.value.length > 0)
//		{
//			switch(cmdInput.value.toLowerCase())
//			{
//				case "refresh":	document.getElementById("RefreshButton").click();
//								break;
//			}
//		}
//	}
//}

function onCommandInput(commandInputControl, e)
{
	switch (e.commandValue)
	{
		case "refreshUserTasks":
		case "refresh":
			e.stopCommand = true;//设置后，不再执行默认的处理
			document.getElementById("RefreshButton").click();
			break;
	}
}

function onLinkUrlClick(url,target,linktype)
{
	var sFeature = "status=no,toolbar=no,menubar=no,center=yes,location=no,resizable=yes,scrollbars=yes,menubar=no;";
	try
	{
		target = target.toLowerCase();
		if (target == "content")
		{
			if(window.opener && window.opener.parent.frames['content'])
			{
				window.open(url, "content");
			}
			else
			{
				window.open(url, "_blank",sFeature);
			}
		}
		else
		{
			LinkUrlClick(url,linktype);
		}
	}
	catch(e)
	{
		window.open(url, "_blank",sFeature);
	}
}

function onLinkRunUrlClick(url,target,linktype)
{
	var sFeature = "status=no,toolbar=no,menubar=no,center=yes,location=no,resizable=yes,scrollbars=yes,menubar=no;";
	try
	{
		target = target.toLowerCase();
		if (target == "content")
		{
			if(window.parent.frames['content'])
			{
				window.open(url, "content");
			}
			else
			{
				window.open(url, "_blank",sFeature);
			}
		}
		else
		{
			LinkUrlClick(url,linktype);
		}
	}
	catch(e)
	{
		window.open(url, "_blank",sFeature);
	}
}

function onLinkContentUrlClick(url,target,linktype)
{
	var sFeature = "status=no,toolbar=no,menubar=no,center=yes,location=no,resizable=yes,scrollbars=yes,menubar=no;";
	try
	{
		target = target.toLowerCase();
		if (target == "content")
		{
			window.open(url, "_self");
		}
		else
		{
			LinkUrlClick(url,linktype);
		}
	}
	catch(e)
	{
		window.open(url, "_blank",sFeature);
	}
}

function LinkUrlClick(url,linktype)
{
	var sFeature = "status=no,toolbar=no,menubar=no,center=yes,location=no,resizable=yes,scrollbars=yes,menubar=no;";
	try
	{
		if(linktype=="0")
		{
			window.open(url, "_blank");
		}
		else
		{
			window.open(url, "_blank",sFeature);
		}
	}
	catch(e)
	{
		window.open(url, "_blank",sFeature);
	}
}