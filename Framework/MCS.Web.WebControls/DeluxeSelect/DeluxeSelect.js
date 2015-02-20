// -------------------------------------------------
// Assembly	：	
// FileName	：	DeluxeSelect.js
// Remark	：  数据选择控件脚本
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		创建
// -------------------------------------------------

var $HGRootNSName = 'MCS.Web.WebControls';
Type.registerNamespace($HGRootNSName);
var $HGRootNS = eval($HGRootNSName); 

$HGRootNS.selectActionType = function()
{
	throw Error.invalidOperation();
};
$HGRootNS.selectActionType.prototype =
{
	Select : 1,		//选择
	UnSelect : 0	//取消选择
}
$HGRootNS.selectActionType.registerEnum($HGRootNSName + ".selectActionType");

$HGRootNS.DeluxeSelect = function(element)
{
    $HGRootNS.DeluxeSelect.initializeBase(this, [element]);
    //待选择列表
    this._candidateList = null; 
    //已选择列表
    this._selectedList = null;    
    //待选择列表Css
    this._candidateListCssClass = "ajax_deluxeselect_select";
    //已选择列表Css
    this._selectedListCssClass = "ajax_deluxeselect_select";
    //设置按钮类别
    this._buttonItems = null;
    //待选择列表的选择模式（单选Or多选）
    this._candidateSelectionMode = null;
    //已选择列表的选择模式（单选Or多选）
    this._selectedSelectionMode = null;
    //待选择数据列表
    this._candidateItems = new Array();
    //已选择数据列表
    this._selectedItems = new Array();
    //初始选择集合的副本
    this._originalSelectedItems = new Array();
    //变化的集合
    this._deltaItems = new Object();
    //待选择排序字段
    this._candidateListSortColumn = "SelectListBoxSortColumn";
    //已选择排序字段
    this._selectedListSortColumn = "SelectListBoxSortColumn";
    //通用的排序字段
    this._commonListSortColumn = "CommonListBoxSortColumn";
    //待选择数据列表排序方式
    this._candidateListSortDirection = $HGRootNS.SortDirection.Ascending;
    //已选择数据列表排序方式
    this._selectedListSortDirection = $HGRootNS.SortDirection.Ascending;
    //上下移按钮的Css
    this._moveButtonCssClass = "ajax_deluxeselect_movebutton";
    //按钮的Css
    this._selectButtonCssClass = "ajax_deluxeselect_button";
    //是否显示选择按钮
    this._showSelectButton = true;
    //是否显示全部选择按钮
    this._showSelectAllButton = true;
    //选择按钮的文本值
    this._selectButtonText = "选择";
    //全部选择按钮的文本值
    this._selectAllButtonText = "全部选择";
    //取消按钮的文本值
    this._cancelButtonText = "取消";
    //全部取消按钮的文本值
    this._cancelAllButtonText = "全部取消";
    //是否允许上下移已选择列表的项
    this._moveOption = true;
    //类别按钮的宽度
    this._selectButtonWidth = "80";
    //上下移按钮的宽度
    this._sortButtonWidth = "30";
    //
    this._selectedFromatString = "{0}[{1}]";
    //Events-------------------------------------------------
    //选择按钮的单击事件
    this._btnOnClick$Delegate = 
    {
        click : Function.createDelegate(this, this._btnOnClick)
    },
    //取消按钮的单击事件
    this._btnCancelClick$Delegate = 
    {
        click : Function.createDelegate(this, this._btnCancelClick)
    },
    //全部取消按钮的单击事件
    this._btnCancelAllClick$Delegate = 
    {
        click : Function.createDelegate(this, this._btnCancelAllClick)
    },
    //上移按钮的单击事件
    this._btnUpClick$Delegate = 
    {
        click : Function.createDelegate(this, this._btnUpClick)
    },
    //下移按钮的单击事件
    this._btnDownClickEvents$Delegate = 
    {
        click : Function.createDelegate(this, this._btnDownClick)
    },
    //全部选择按钮的单击事件
    this._btnSelectAllClickEvents$Delegate = 
    {
        click : Function.createDelegate(this, this._btnSelectAllClick)
    },
    //待选择列表的双击事件
    this._candidateDblClick$Delegate = 
    {
        dblclick : Function.createDelegate(this, this._candidateDblClick)
    },
    //已选择列表的双击事件
    this._selectedDblClick$Delegate = 
    {
        dblclick : Function.createDelegate(this, this._selectedDblClick)
    },
    //排序方法
    this._sortDESCColumnDelegate = Function.createDelegate(this, this._sortDESCColumn);
    this._sortASCColumnDelegate = Function.createDelegate(this, this._sortASCColumn);
    
    this._sortDESCTypeAndColumnDelegate = Function.createDelegate(this, this._sortDESCTypeAndColumn);
    this._sortASCTypeAndColumnDelegate = Function.createDelegate(this, this._sortASCTypeAndColumn);

    //--------------------------------------------------------------
}

//排序类型（升序\降序）
$HGRootNS.SortDirection = function()
{
    throw Error.invalidOperation();
}
$HGRootNS.SortDirection.prototype = 
{
    Ascending : 0,
    Descending : 1
}
$HGRootNS.SortDirection.registerEnum($HGRootNSName + '.SortDirection');

//按钮类型
$HGRootNS.ButtonType = function()
{
    throw Error.invalidOperation();
}
$HGRootNS.ButtonType.prototype = 
{
    Button : 0,
    ImageButton : 1,
    LinkButton : 2
}
$HGRootNS.ButtonType.registerEnum($HGRootNSName + '.ButtonType');

$HGRootNS.DeluxeSelect.prototype = 
{
    initialize : function()
    {
        $HGRootNS.DeluxeSelect.callBaseMethod(this, 'initialize');
        this._buildControl();
    },
    
    dispose : function()
    {
        this._candidateList = null; //待选择列表
        this._selectedList = null; //已选择列表
        this._candidateItems = null;
        this._buttonItems = null;
        this._selectedItems = null;
        this._originalSelectedItems = null;

        $HGRootNS.DeluxeSelect.callBaseMethod(this, 'dispose');
    }, 
    //Property
    //待选择列表
    get_candidateList : function() 
    {
        return this._candidateList;
    },
    set_candidateList : function(value) 
    {
        this._candidateList = value;
    },
    //待选择列表的选择模式
    get_candidateSelectionMode : function() 
    {
        return this._candidateSelectionMode;
    },
    set_candidateSelectionMode : function(value) 
    {
        this._candidateSelectionMode = value;
    },
    //已选择列表的选择模式
    
    get_selectedSelectionMode : function() 
    {
        return this._selectedSelectionMode;
    },
    set_selectedSelectionMode : function(value) 
    {
        this._selectedSelectionMode = value;
    },
    //待选择列表的数据集合
    get_candidateItems : function() 
    {
        return this._candidateItems;
    },
    set_candidateItems : function(value) 
    {
        if (this._candidateItems != value) 
        {
            this._candidateItems = value;
        }
    },
    //已选择列表的数据集合
    get_selectedItems : function() 
    {
        return this._selectedItems;
    },
    set_selectedItems : function(value) 
    {
        if (this._selectedItems != value) 
        {
            this._selectedItems = value;

            for (var i = 0; i < this._selectedItems.length; i ++)
                Array.add(this._originalSelectedItems, this._selectedItems[i]);
        }
    }, 
	get_deltaItems : function()
	{
	    this._deltaItems = this._getDeltaItems();
		return this._deltaItems;
	},
	set_deltaItems : function(value) 
    {
		if (value != this._deltaItems)
		{
			this._deltaItems = value;
			this.raisePropertyChanged("deltaItems");
		}
    },
    //待选择列表的排序列
    get_candidateListSortColumn : function() 
    {
        return this._candidateListSortColumn;
    },
    set_candidateListSortColumn : function(value) 
    {
        this._candidateListSortColumn = value;
    },
    //已选择列表的排序列
    get_selectedListSortColumn : function() 
    {
        return this._selectedListSortColumn;
    },
    set_selectedListSortColumn : function(value) 
    {
        this._selectedListSortColumn = value;
    },
    //待选择列表的排序模式
    get_candidateListSortDirection : function() 
    {
        return this._candidateListSortDirection;
    },
    set_candidateListSortDirection : function(value) 
    {
        this._candidateListSortDirection = value;
    },
     //通用的排序字段
    get_commonListSortColumn : function() 
    {
        return this._commonListSortColumn;
    },
    set_commonListSortColumn : function(value) 
    {
        this._commonListSortColumn = value;
    },
    //已选择列表的排序模式
    get_selectedListSortDirection : function() 
    {
    return this._selectedListSortDirection;
    },
    set_selectedListSortDirection : function(value) 
    {
        this._selectedListSortDirection = value;
    },
    //类别按钮的数据集合
    get_buttonItems : function() 
    {
        return this._buttonItems;
    },
    set_buttonItems : function(value) 
    {
        if (this._buttonItems != value) 
        {
            this._buttonItems = value;
        }
    },
    //按钮的样式
    get_buttonCssClass : function() 
    {
        return this._buttonCssClass;
    },
    set_buttonCssClass : function(value) 
    {
        this._buttonCssClass = value;
    },
    //待选择数据列表的样式
    get_candidateListCssClass : function() 
    {
        return this._candidateListCssClass;
    },
    set_candidateListCssClass : function(value) 
    {
        this._candidateListCssClass = value;
    },
    //已选择数据列表的样式
    get_selectedListCssClass : function() 
    {
        return this._selectedListCssClass;
    },
    set_selectedListCssClass : function(value) 
    {
        this._selectedListCssClass = value;
    },
     //clientstate
    //选择按钮的样式
    get_selectButtonCssClass : function() 
    {
        return this._selectButtonCssClass;
    },
    set_selectButtonCssClass : function(value) 
    {
        this._selectButtonCssClass = value;
    },
    //上下移按钮的样式
    get_moveButtonCssClass : function() 
    {
        return this._moveButtonCssClass;
    },
    set_moveButtonCssClass : function(value) 
    {
        this._moveButtonCssClass = value;
    },
    //是否显示选择按钮
    get_showSelectButton : function() 
    {
        return this._showSelectButton;
    },
    set_showSelectButton : function(value) 
    {
        this._showSelectButton = value;
    },
    //是否显示全部选择按钮
    get_showSelectAllButton : function() 
    {
        return this._showSelectAllButton;
    },
    set_showSelectAllButton : function(value) 
    {
        this._showSelectAllButton = value;
    },
    //选择按钮的文本值
    get_selectButtonText : function() 
    {
        return this._selectButtonText;
    },
    set_selectButtonText : function(value) 
    {
        this._selectButtonText = value;
    },
    //全部选择按钮的文本值
    get_selectAllButtonText : function() 
    {
        return this._selectAllButtonText;
    },
    set_selectAllButtonText : function(value) 
    {
        this._selectAllButtonText = value;
    },
    //取消按钮的文本值
    get_cancelButtonText : function() 
    {
        return this._cancelButtonText;
    },
    set_cancelButtonText : function(value) 
    {
        this._cancelButtonText = value;
    },
    //全部取消的文本值
    get_cancelAllButtonText : function() 
    {
        return this._cancelAllButtonText;
    },
    set_cancelAllButtonText : function(value) 
    {
        this._cancelAllButtonText = value;
    },
    //是否允许上下移动数据列表的数据项
    get_moveOption : function() 
    {
        return this._moveOption;
    },
    set_moveOption : function(value) 
    {
        this._moveOption = value;
    },
    
    //FromatString
    get_selectedFromatString : function() 
    {
        return this._selectedFromatString;
    },
    set_selectedFromatString : function(value) 
    {
        this._selectedFromatString = value;
    },
    //---------------------------------------------------创建控件开始-----------------------------------------//
    //创建控件
    _buildControl : function()
    {
        //
        var divMain = this.get_element();
        
        //Create Body
        this._createBody(divMain); 
   },
    //创建控件的Body
    _createBody : function(divMain)
    {
        var tableMain = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "table" , 
            properties : 
            {
                width : "100%",
                height : "100%",
                cellPadding : "0px",
                cellSpacing : "0px"
            }
        },divMain
        );
        
        var tbodyMain = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "tbody" , 
            properties : {}
        },tableMain
        );
        
        var trMain = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "tr" , 
            properties : {}
        },tbodyMain
        );
        //设置布局
        var totalWidth = divMain.style.pixelWidth;
        var totalHeight = divMain.style.pixelHeight;
        var selectWidth = Math.round((totalWidth - this._selectButtonWidth - this._sortButtonWidth) / 2);

        //创建待选择列表
        this._createCandidateList(trMain, selectWidth, totalHeight);
        //创建类别按钮
        this._createButton(trMain);
        //创建已选择列表
        this._createSelectedList(trMain, selectWidth, totalHeight);
        //创建上下移按钮
        this._createUpDownButton(trMain);      
    },
    //创建待选择列表
    _createCandidateList : function(trMain, width, height)
    {
        var tdSelecting = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "td" , 
            properties : 
            { 
                style : 
                {
                    width : width, 
                    height : height
                }
            }
        },trMain
        );
      
        var _defaultSize;
        if(this._candidateItems.length < 2)
        {
            _defaultSize = "10";
        }
        else
        {
            _defaultSize = this._candidateItems.length.toString();
        }
       
        this._candidateList = $HGDomElement.createElementFromTemplate(
        { 
            nodeName : "select" , 
            properties : 
            {
                multiple : this._candidateSelectionMode,
                size : _defaultSize,
                style : 
                {
                    //width : width + 6, 
                    width : "100%",
                    height : height + Math.round(height * 0.1)
                }
            },
            cssClasses : [this._candidateListCssClass],
            events : this._candidateDblClick$Delegate
        },tdSelecting
        );
        //排序后，添加Options
        this._buildOptionsAndSort(this._candidateList,null);
        
    },
    ////排序后，添加Options
    _buildOptionsAndSort : function(_candidateList,selectedArray)
    {
        if(this._candidateItems) 
        {
           this._candidateItems = this._sortSelectByColumn(this._candidateItems,this._candidateListSortDirection);
           
           //排序后，添加Options
           this._createOptions(this._candidateItems , _candidateList , "left");   
        }        
    },
    //创建列别按钮
    _createButton : function(trMain)
    {
        var tdButton = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "td",
            properties : {
                align:"center", 
                style : { width : this._selectButtonWidth } 
            }
        },trMain
        );
        //-----------------------------根据类别动态创建按钮--开始------------------------------------//
        if(this._buttonItems)
        {
            //按按钮的顺序号进行排序
           this._buttonItems.sort(this._sortButton);
            for(var i = 0 ; i < this._buttonItems.length ; i++)
            {
                switch( this._buttonItems[i].ButtonType )
                {
                    case $HGRootNS.ButtonType.Button ://button 
                       var _button = $HGDomElement.createElementFromTemplate(
                        {
                            nodeName : "input",
                            properties : { 
                                type : "button",
                                value : this._buttonItems[i].ButtonName,
                                maxcount : this._buttonItems[i].ButtonTypeMaxCount,
                                sortid : this._buttonItems[i].ButtonSortID,
                                datatype : this._buttonItems[i].ButtonName
                            },
                            cssClasses : [this._buttonItems[i].ButtonCssClass],
                            events : this._btnOnClick$Delegate
                        },
                        tdButton
                        );
                        break;
                    case $HGRootNS.ButtonType.ImageButton://ImageButton
                        var _button = $HGDomElement.createElementFromTemplate(
                        {
                            nodeName : "image",
                            properties : { 
                                src : this._buttonItems[i].ImageSrc,
                                maxcount : this._buttonItems[i].ButtonTypeMaxCount,
                                sortid : this._buttonItems[i].ButtonSortID,
                                datatype : this._buttonItems[i].ButtonName,
                                style : {cursor:"hand"}
                            },
                            cssClasses : [this._buttonItems[i].ButtonCssClass],
                            events : this._btnOnClick$Delegate
                        },
                        tdButton
                        );
                        break;
                    case $HGRootNS.ButtonType.LinkButton://LinkButton
                        var _button = $HGDomElement.createElementFromTemplate(
                        {
                            nodeName : "a",
                            properties : { 

                                href : "#",
                                innerText : this._buttonItems[i].ButtonName,
                                maxcount : this._buttonItems[i].ButtonTypeMaxCount,
                                sortid : this._buttonItems[i].ButtonSortID,
                                datatype : this._buttonItems[i].ButtonName
                            },
                           cssClasses : [this._buttonItems[i].ButtonCssClass],
                           events : this._btnOnClick$Delegate
                        },
                        tdButton
                        );
                        break;
                    default:
                        break;
                    
                }
                $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "br"
                },
                tdButton
                );
            }  
        }
        //-----------------------------根据类别动态创建按钮--结束------------------------------------//
        
       
        //-------------------------- 默认显示按钮---------------开始--------------------//
        //选择按钮
        if(this._showSelectButton)
        {
            var buttonSelect = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "input",
                properties : 
                { 
                    type : "button",
                    value : this._selectButtonText,
                    maxcount : "-1",
                    datatype : "null",
                    style :
                    {
						width : "50px"
                    }
                },
                cssClasses : [this._selectButtonCssClass],
                events : this._btnOnClick$Delegate
            },
            tdButton
            );  
            //换行
            $HGDomElement.createElementFromTemplate(
            {
                nodeName : "br"
            },
            tdButton
            );
        }
        //全部选择按钮
        if(this._showSelectAllButton)
        {
            var buttonSelectAll = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "input",
                properties : 
                { 
                    type : "button",
                    value : this._selectAllButtonText,
                    style :
                    {
						width : "50px"
                    }
                },
                cssClasses : [this._selectButtonCssClass],
                events : this._btnSelectAllClickEvents$Delegate
            },
            tdButton
            );        
            //换行
            $HGDomElement.createElementFromTemplate(
            {
                nodeName : "br"
            },
            tdButton
            );
        }
        //取消按钮
        var buttonCancel = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "input",
            properties : 
            { 
                type : "button",
                value : this._cancelButtonText,
                style :
                {
					width : "50px"
                }
            },
            cssClasses : [this._selectButtonCssClass],
            events : this._btnCancelClick$Delegate
        },
        tdButton
        );         
         //换行
        $HGDomElement.createElementFromTemplate(
        {
            nodeName : "br"
        },
        tdButton
        );
        //全部取消按钮
        var buttonAllCancel = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "input",
            properties : 
            { 
                type : "button", 
                value : this._cancelAllButtonText,
                style :
                {
					width : "50px"
                }
            },
            cssClasses : [this._selectButtonCssClass],
            events : this._btnCancelAllClick$Delegate
        },
        tdButton
        ); 
        
    //-------------------------- 默认显示按钮------结束--------------------------------//
    },
    //创建已选择列表
    _createSelectedList : function(trMain, width, height)
    {
        var tdSelected = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "td" , 
           properties : { style : {width : width, height : height}}
           
        },trMain
        );
     
        var _defaultSize;
        if(this._candidateItems.length <  2)
        {
            _defaultSize = "10";
        }
        else
        {
            _defaultSize = this._candidateItems.length.toString();
        }
        this._selectedList = $HGDomElement.createElementFromTemplate(
        { 
            nodeName : "select" , 
            properties : 
            {
                multiple : this._selectedSelectionMode,
                size : _defaultSize,
                style : 
                {
                    //width : width + 6, 
                    width : "100%",
                    height : height + Math.round(height * 0.1)
                }
            },
            cssClasses : [this._selectedListCssClass],
            events : this._selectedDblClick$Delegate

        },tdSelected
        );
        //已选择列表按指定字段排序
        if(this._selectedListSortDirection == $HGRootNS.SortDirection.Descending)
        {
            //首先按类别将已选择列表项排序,然后按指定字段排序
            this._selectedItems.sort(this._sortDESCTypeAndColumnDelegate); // 开始排序
        }
        else
        {
            //首先按类别将已选择列表项排序,然后按指定字段排序
            this._selectedItems.sort(this._sortASCTypeAndColumnDelegate); // 开始排序
        }
            
        if(this._selectedItems) 
        {
            this._createOptions(this._selectedItems , this._selectedList , "right"); 
        }
    },

    //创建上下移动按钮
    _createUpDownButton : function(trMain)
    {
        var tdUpDown = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "td" ,
            properties : 
            { 
                width : this._sortButtonWidth,
                align : "center"
            },
            cssClasses : ["ajax_deluxeselect_td"]
        },trMain
        );
        
        var buttonUp = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "button",
            properties : 
            { 
                value : "↑",
                disabled : !this._moveOption
            },
            cssClasses : [this._moveButtonCssClass],
            events : this._btnUpClick$Delegate
        },
        tdUpDown
        );
        //换行
        $HGDomElement.createElementFromTemplate(
        {
            nodeName : "br"
        },
        tdUpDown
        );
        var buttonDown = $HGDomElement.createElementFromTemplate(
        {
            nodeName : "button",
            properties : 
            { 
                value : "↓",
                disabled : !this._moveOption
            },
            cssClasses : [this._moveButtonCssClass],
            events : this._btnDownClickEvents$Delegate
        },
        tdUpDown
        );
     },
     //----------------------------------------------创建控件结束-------------------------------------------------//
     
    //-----------------------------------------------Private Methods结束------------------------------------------//
    //按指定条件排序
    _sortSelectByColumn : function(_selectItems,_sortDirection)
    {      
        if(this._candidateListSortColumn == "")
            return _selectItems;
           
        if(_sortDirection == $HGRootNS.SortDirection.Descending)
        {
            _selectItems.sort(this._sortDESCColumnDelegate); // 开始排序         
        }
        else
        {
            _selectItems.sort(this._sortASCColumnDelegate); // 开始排序
        }

        return _selectItems;
    },
    //按指定条件排序
    _sortDESCColumn : function(x,y)
    {  
        try
        {
           // var vResult = x[this._candidateListSortColumn].localeCompare(y[this._candidateListSortColumn]);
           var vResult = x[this._commonListSortColumn].localeCompare(y[this.CommonListBoxSortColumn]);
            if(vResult == 1)
                return -1;
            if(vResult == -1)
                return 1;
            if(vResult == 0)
                return 0;
        }
        catch(exception)
        {
            return 0;
        }
        
    },
    //按指定条件排序
    _sortASCColumn : function(x,y)
    {   
        try
        {
            //var vResult = x[this._candidateListSortColumn].localeCompare(y[this._candidateListSortColumn]);
            var vResult = x[this._commonListSortColumn].localeCompare(y[this._commonListSortColumn]);
                    
        }
        catch(exception)
        {
            return 0;
        }
        
        return vResult;
    },
    //按指定条件排序
    _sortDESCTypeAndColumn : function(x,y)
    {
        if(x["SelectItemType"] == null)
        {
            x["SelectItemType"] = "null";
        }
        if(y["SelectItemType"] == null)
        {
            y["SelectItemType"] = "null";
        }
        var vResult = this._sortByButtonItems(x, y);
        if(vResult == 1)
            return -1;
        if(vResult == -1)
            return 1;
        if(vResult == 0)
        {  
            try
            {
                if(this._selectedListSortColumn == "")
                    return 0;  
                var vResultCol = x[this._selectedListSortColumn].localeCompare(y[this._selectedListSortColumn]);
                if(vResultCol == 1)
                    return -1;
                if(vResultCol == -1)
                    return 1;
                if(vResultCol == 0)
                    return 0;
            }
            catch(exception)
            {
                return 0;
            }
        }
    },
    //按指定条件排序
    _sortASCTypeAndColumn : function(x,y)
    {
        if(x["SelectItemType"] == null)
        {
            x["SelectItemType"] = "null";
        }
        if(y["SelectItemType"] == null)
        {
            y["SelectItemType"] = "null";
        }
        var vResult = this._sortByButtonItems(x, y);
        if(vResult == 1)
            return 1;
        if(vResult == -1)
            return -1;
        if(vResult == 0)
        {   
            try
            {
                if(this._selectedListSortColumn == "")
                    return 0;  
                var vResultCol = x[this._selectedListSortColumn].localeCompare(y[this._selectedListSortColumn]);
                if(vResultCol == 1)
                    return 1;
                if(vResultCol == -1)
                    return -1;
                if(vResultCol == 0)
                    return 0;
            }
            catch(exception)
            {
                return 0;
            }
        }    
    },
    _sortByButtonItems : function(x, y)
    {
		if(this._buttonItems == null)
			return 0;
	
		if(this._buttonItems.length == null)
			return 0;

		var indexX = 0;
		var indexY = 0;
		
		for (var i = 0; i < this._buttonItems.length; i ++)
		{
			if (this._buttonItems[i].ButtonName == x["SelectItemType"])
				indexX = i;
			
			if (this._buttonItems[i].ButtonName == y["SelectItemType"])
				indexY = i;
		}
		
		var result = indexX - indexY;
		
		if (result > 0)
			return 1;
		
		if (result < 0)
			return -1;
		
		return 0;
    },
    //根据按钮的SortId进行排序
    _sortButton : function(x,y)
    {   
        try
        {
            if(x["ButtonSortID"] > y["ButtonSortID"])
            {
                return 1;
            }
            if(x["ButtonSortID"] < y["ButtonSortID"])
            {
                return -1;
            }
            if(x["ButtonSortID"] == y["ButtonSortID"])
            {
                return 0;
            }
        
        }
        catch(exception)
        {
            return 0;
        }
        return vResult;
    },
    //清空options集合
    _clearOptions : function(_selectList)
    {
        var length = _selectList.options.length;
        for(var i=length-1;i>=0;i--)
        {
            _selectList.options.remove(i);
        }
    },
    //将Items数据集合绑定到Option
    _createOptions : function(Items , List , ListFlag)
    {
        for(var i = 0; i < Items.length; i++) 
        {
            var  n1 = document.createElement("option");  
            n1.value = Items[i].SelectListBoxValue; 
            
            if (Items[i].Title && Items[i].Title != "")
				n1.title = Items[i].Title;

//            n1.text = Items[i].SelectListBoxText;
            if(ListFlag == "left")
            {
                n1.text = Items[i].SelectListBoxText;
            }
            else
            {
                if(this._selectedFromatString != "" && Items[i].SelectItemType != null && Items[i].SelectItemType != ""  && Items[i].SelectItemType != "null")
                {
                    n1.text = String.format(this._selectedFromatString , Items[i].SelectListBoxText , Items[i].SelectItemType);
                }
                else
                {
                    n1.text = Items[i].SelectListBoxText;
                }
            }
            n1.item = Items[i];
            List.add(n1);
            

            if(Items[i].Locked == true)
            {
                n1.style.color = "SkyBlue";
            }
            if(Items[i].Selected == true)
            {
                n1.selected = true;
            }
        }
    },
    _checkItemInList : function(list, item)
    {
		if(list)
		{
			for (var i = 0; i < list.length; i ++)
			{
				if (list[i].SelectListBoxValue == item.SelectListBoxValue
					&& list[i].SelectItemType == item.SelectItemType)
					return i;
			}
		}

		return -1;
    },
	_getDeltaItems : function()
	{
	    var item, index, deltaItems;

        deltaItems = new Object();
        deltaItems.DeletedItems = new Array();
        deltaItems.InsertedItems = new Array();

	    //删除的
	    for (var i = 0; i < this._originalSelectedItems.length; i ++)
	    {
	        item =  this._originalSelectedItems[i];
	        
	        if (this._checkItemInList(this._selectedItems, item) == -1
	            && this._checkItemInList(deltaItems.DeletedItems, item) == -1)
	            Array.add(deltaItems.DeletedItems, item);
	    }
	    //增加的
	     for (var i = 0; i < this._selectedItems.length; i ++)
	    {
	        item =  this._selectedItems[i];
	        
	        if (this._checkItemInList(this._originalSelectedItems, item) == -1
	            && this._checkItemInList(deltaItems.InsertedItems, item) == -1)
	            Array.add(deltaItems.InsertedItems, item);
	    }
	    
	    return this._mergerDeltaItems(this._deltaItems, deltaItems);
	},
	
	_mergerDeltaItems : function(oldDeltaItems, newDeltaItems)
	{
	    var item, index, i;
	    
	    //合并本次操作的insert
	    for (var i = 0; i < newDeltaItems.InsertedItems.length; i ++)
	    {
	       item = newDeltaItems.InsertedItems[i];
            
           if (this._checkItemInList(oldDeltaItems.DeletedItems, item) == -1
                && this._checkItemInList(oldDeltaItems.InsertedItems, item) == -1)
	            Array.add(oldDeltaItems.InsertedItems, item);
	    }
	    
	    //合并本次操作的delete
	    for (var i = 0; i < newDeltaItems.DeletedItems.length; i ++)
	    {
	        item = newDeltaItems.DeletedItems[i];
            
            if (this._checkItemInList(oldDeltaItems.DeletedItems, item) == -1
                && this._checkItemInList(oldDeltaItems.InsertedItems, item) == -1)
	            Array.add(oldDeltaItems.DeletedItems, item);
	    }
	    
        i = 0;

	    //如果本次操作删除了上次操作中insert的某个item 则从中删除
	    while(i < oldDeltaItems.InsertedItems.length)
	    {
	        item = oldDeltaItems.InsertedItems[i];
	         
	        if (this._checkItemInList(newDeltaItems.DeletedItems, item) != -1)
	            Array.remove(oldDeltaItems.InsertedItems, item);
	        else
	            i += 1;
	    }
	    
	    i = 0;

	    //如果本次操作增加了上次操作中delete的某个item 则从中删除
	    while(i < oldDeltaItems.DeletedItems.length)
	    {
	        item = oldDeltaItems.DeletedItems[i];
	         
	        if (this._checkItemInList(newDeltaItems.InsertedItems, item) != -1)
	            Array.remove(oldDeltaItems.DeletedItems, item);
	        else
	           i += 1;
	    }

	    return oldDeltaItems;
	},
    //移动数据项
    _setItems : function(sourceItems , sourceList , selectedArray , index , arrayIndex , dataType, actionType)
    {
        var selectedItem = {};
        selectedItem.SelectListBoxText = sourceList.options[index].item.SelectListBoxText;
        selectedItem.SelectListBoxValue = sourceList.options[index].item.SelectListBoxValue;
        selectedItem.SelectListBoxSortColumn  = sourceList.options[index].item.SelectListBoxSortColumn;
        selectedItem.CommonListBoxSortColumn  = sourceList.options[index].item.CommonListBoxSortColumn;
        selectedItem.SelectItemType = dataType;  
        selectedItem.Locked = false;
        selectedItem.Selected = true;
        selectedArray[arrayIndex] = selectedItem;
        
        //2、从已选择数据列表中删除选中的纪录
        sourceList.options.remove(index);

        var part1 = sourceItems.slice(0, index + 1);         
        var part2 = sourceItems.slice(index + 1);        
        part1.pop();         
        sourceItems =  part1.concat(part2);  

        return sourceItems;
    },
    //取得选择数量
    _getSelectCount : function(selectList)
    {
        var intSelectedCount = 0;
        for (var i = 0; i < selectList.options.length; i++)
        {
            if (selectList.options[i].selected == true)
            {
                intSelectedCount++;
            }
        }
        
        return intSelectedCount;
    },
    //
    _getSortColumn : function (Items , text , value)
    {
        for(var i=0;i<Items.length;i++)
        {
            if(Items[i].SelectListBoxText == text && Items[i].SelectListBoxValue == value )
            {
                return Items[i].SelectListBoxSortColumn;               
            }
        }
    },
    //取得列表的锁定状态
    _getLock : function(Items,index)
    {
        for(var i=0;i<Items.length;i++)
        {
            if( i == index )
            {
                return Items[i].Locked;               
            }
        } 
    },
    //按钮事件的数据操作方法
    //sourceList -- 源List
    //destList -- 目的List
    //sourceItems -- 源Items
    //destItems -- 目的Items
    //sortDirection -- 排序
    //listFlag -- 操作的List，左右两个列表的排序规则不同
    //isAll -- 是否是全部数据的操作
    //dataType -- 类别
    _fillList : function ( sourceList , destList , sourceItems , destItems , sortDirection , listFlag , isAll , dataType, actionType)
    {
        var j = 0;
        var selectedArray = new Array(); 
        var sourceListLength = sourceList.options.length ;
        //重置列表的Selected属性
        this._resetSelectedState(sourceItems);
        this._resetSelectedState(destItems);
        //将排序后的数据添加至已选择列表 Options   
        if( isAll == false )
        {
            for(var i = 0; i < sourceList.options.length; i++) 
            { 
                if(sourceList.options[i].selected 
                    && this._getLock(sourceItems , i) == false)
                { 
                    sourceItems = this._setItems(sourceItems , sourceList, selectedArray , i , j , dataType, actionType);     
                    j++;
                    i--;
                }
            }
        }
        else        //全部选择
        {
            for(var i = 0; i < sourceList.options.length; i++) 
            { 
                if( this._getLock(sourceItems , i) == false)
                { 
                    sourceItems = this._setItems(sourceItems , sourceList, selectedArray , i , j , dataType, actionType); 
                    j++;
                    i--;
                }
            }
        }
        Array.addRange(destItems,selectedArray);
      
        //排序
        if(sortDirection == $HGRootNS.SortDirection.Ascending)
        {
            //首先按类别将已选择列表项排序,然后按指定字段排序
            if( listFlag == "left" )
            {
                destItems.sort(this._sortASCColumnDelegate);
                this._candidateItems = destItems ;
                this._selectedItems = sourceItems ; 
            }
            else
            {
                destItems.sort(this._sortASCTypeAndColumnDelegate); // 开始排序
                this._candidateItems = sourceItems ;
                this._selectedItems = destItems; 
            }
        }
        else
        {
            //首先按类别将已选择列表项排序,然后按指定字段排序
            if( listFlag == "left" )
            {
                destItems.sort(this._sortDESCColumnDelegate);
                this._candidateItems = destItems ;
                this._selectedItems = sourceItems ;
            }
            else
            {
                destItems.sort(this._sortDESCTypeAndColumnDelegate); // 开始排序
                this._candidateItems = sourceItems ;
                this._selectedItems = destItems; 
            }
        }
        
        //按排序后的结果设置已选择列表的Options
        //先清除，后添加
        this._clearOptions(destList);
        
        //绑定Options
        this._createOptions(destItems , destList , listFlag); 

    },
    //重置Items的选中属性
    _resetSelectedState : function(Items)
    {
        for(var i = 0; i < Items.length ; i++ )
        {
            Items[i].Selected = false;
        }
    },
    //-----------------------------------------------Private Methods结束-----------------------------------------//
    
   //------------------------------------------------事件处理 开始-----------------------------------------------//
   //选择事件
    _btnOnClick : function(e)
    {     
       //校验此按钮对应的类别最大选择数量    
        var selectedCount = this._getSelectCount(this._candidateList);
        var selectedItemsCount = this._getSelectedItemsByType(e.target.datatype);
        //-1表示可选无限多个

        if((selectedCount + selectedItemsCount) <= e.target.maxcount || e.target.maxcount == -1)
        {   
           this._fillList(this._candidateList , this._selectedList , this._candidateItems , this._selectedItems , this._selectedListSortDirection , "right" , false ,e.target.datatype, $HGRootNS.selectActionType.Select);  
        }
        else
        {
            alert("超出此类别最大选择记录数："　+ e.target.maxcount);
        }   
    },
    
    _getSelectedItemsByType : function(type)
    {
		var sum = 0 ;

		for(var i = 0; i < this._selectedItems.length; i ++)
		{
			if(this._selectedItems[i].SelectItemType == type)
				sum += 1;
		}

		return sum;
    },

    //全部选择事件
    _btnSelectAllClick : function(e)
    {
        this._fillList(this._candidateList , this._selectedList , this._candidateItems , this._selectedItems , this._selectedListSortDirection , "right" , true , "", $HGRootNS.selectActionType.Select);  
    },
    //数据列表的双击选择事件
    _candidateDblClick : function(e)
    {
        if(this._buttonItems.length == 0 && this._candidateItems.length > 0)
        { 
            var selectedIndex = e.target.selectedIndex;
            var selectedArray = new Array();       
//            //将排序后的数据添加至已选择列表 Options   
            if(this._getLock(this._candidateItems , selectedIndex) == false)
            { 
                this._candidateItems = this._setItems(this._candidateItems , this._candidateList , selectedArray , selectedIndex , 0 , e.target.datatype, $HGRootNS.selectActionType.Select);
            
            }
            Array.addRange(this._selectedItems,selectedArray);
            
            //排序
            if(this._selectedListSortDirection == $HGRootNS.SortDirection.Ascending)
            {
                //首先按类别将已选择列表项排序,然后按指定字段排序
                this._selectedItems.sort(this._sortASCTypeAndColumnDelegate); // 开始排序
            }
            else
            {
                //首先按类别将已选择列表项排序,然后按指定字段排序
                this._selectedItems.sort(this._sortDESCTypeAndColumnDelegate); // 开始排序
            }
            //按排序后的结果设置已选择列表的Options
            //先清除，后添加
            this._clearOptions(this._selectedList);
            //绑定Options
            this._createOptions(this._selectedItems , this._selectedList , "right"); 
        }
    },
    //已选择列表的双击选择事件
    _selectedDblClick : function(e)
    {
        if(this._buttonItems.length == 0 && this._selectedItems.length > 0)
        { 
            var selectedIndex = e.target.selectedIndex;
            var selectedArray = new Array();  
            //1、把要移动的数据移至待选择列表的Items中
            if(this._getLock(this._selectedItems , selectedIndex) == false)
            {
                this._selectedItems = this._setItems(this._selectedItems , this._selectedList , selectedArray , selectedIndex , 0 , "", $HGRootNS.selectActionType.UnSelect);            
            }

            //将选择数据添加到待选择列表中
            Array.addRange(this._candidateItems,selectedArray);
            
            //3、对待选择数据列表的项按预设值进行排序，以保证数据移至待选择数据列表的原位置
            //排序后，先清空待选择列表，后添加Options
            this._clearOptions(this._candidateList);     
            
            
            this._buildOptionsAndSort(this._candidateList,selectedArray);
        }
    },
    //上移单击事件
    _btnUpClick : function()
    {
        //上移      
        var iSelectedCount = this._getSelectCount(this._selectedList);
        if (iSelectedCount > 1 || iSelectedCount == 0)
        {
            return;
        }        

        var intSelectIndex = this._selectedList.selectedIndex;
        if (intSelectIndex == 0) return;
        //上一位置信息
        var strLastText = this._selectedList.options[intSelectIndex - 1].text;
        var strLastValue = this._selectedList.options[intSelectIndex - 1].value;
        //选中移动信息
        var strThisText = this._selectedList.options[intSelectIndex].text;
        var strThisValue = this._selectedList.options[intSelectIndex].value;


        //和上一位置调换
        this._selectedList.options[intSelectIndex - 1].text = strThisText;
        this._selectedList.options[intSelectIndex - 1].value = strThisValue;

        //
        this._selectedList.options[intSelectIndex].text = strLastText;
        this._selectedList.options[intSelectIndex].value = strLastValue;

        this._selectedList.selectedIndex = intSelectIndex - 1;
        
        //调整Items的位置
        //上一Item信息
        var strLastItemText = this._selectedItems[intSelectIndex - 1].SelectListBoxText;
        var strLastItemValue = this._selectedItems[intSelectIndex - 1].SelectListBoxValue;
        var strLastItemSortColumn = this._selectedItems[intSelectIndex - 1].SelectListBoxSortColumn;
        var strLastItemCommonSortColumn = this._selectedItems[intSelectIndex - 1].CommonListBoxSortColumn;
        var strLastItemType = this._selectedItems[intSelectIndex - 1].SelectItemType;
        var strLastItemLocked = this._selectedItems[intSelectIndex - 1].Locked;
        //选中移动Item信息
        var strThisItemText = this._selectedItems[intSelectIndex].SelectListBoxText;
        var strThisItemValue = this._selectedItems[intSelectIndex].SelectListBoxValue;
        var strThisItemSortColumn = this._selectedItems[intSelectIndex].SelectListBoxSortColumn;
        var strThisItemCommonSortColumn = this._selectedItems[intSelectIndex].CommonListBoxSortColumn;
        var strThisItemType = this._selectedItems[intSelectIndex].SelectItemType;
        var strThisItemLocked = this._selectedItems[intSelectIndex].Locked;
        
        //和上一Item调换
        this._selectedItems[intSelectIndex - 1].SelectListBoxText = strThisItemText;
        this._selectedItems[intSelectIndex - 1].SelectListBoxValue = strThisItemValue;
        this._selectedItems[intSelectIndex - 1].SelectListBoxSortColumn = strThisItemSortColumn;
        this._selectedItems[intSelectIndex - 1].CommonListBoxSortColumn = strThisItemCommonSortColumn;
        this._selectedItems[intSelectIndex - 1].SelectItemType = strThisItemType;
        this._selectedItems[intSelectIndex - 1].Locked = strThisItemLocked;
        
        this._selectedItems[intSelectIndex].SelectListBoxText = strLastItemText;
        this._selectedItems[intSelectIndex].SelectListBoxValue = strLastItemValue;
        this._selectedItems[intSelectIndex].SelectListBoxSortColumn = strLastItemSortColumn;
        this._selectedItems[intSelectIndex].CommonListBoxSortColumn = strLastItemCommonSortColumn;
        this._selectedItems[intSelectIndex].SelectItemType = strLastItemType;
        this._selectedItems[intSelectIndex].Locked = strLastItemLocked;  

    },
    //下移单击事件
    _btnDownClick : function()
    {
        //下移 
        var iSelectedCount = this._getSelectCount(this._selectedList);
        if (iSelectedCount > 1 || iSelectedCount == 0)
        {
            return;
        }        

        var intSelectIndex = this._selectedList.selectedIndex;
        if (intSelectIndex == this._selectedList.options.length - 1) return;
        //下一位置信息
        var strNextText = this._selectedList.options[intSelectIndex + 1].text;
        var strNextValue = this._selectedList.options[intSelectIndex + 1].value;
        //选中移动信息
        var strThisText = this._selectedList.options[intSelectIndex].text;
        var strThisValue = this._selectedList.options[intSelectIndex].value;


        //和下一位置调换
        this._selectedList.options[intSelectIndex + 1].text = strThisText;
        this._selectedList.options[intSelectIndex + 1].value = strThisValue;

        //
        this._selectedList.options[intSelectIndex].text = strNextText;
        this._selectedList.options[intSelectIndex].value = strNextValue;

        this._selectedList.selectedIndex = intSelectIndex + 1;
        
        //调整Items的位置
        //下一Item信息
        var strNextItemText = this._selectedItems[intSelectIndex + 1].SelectListBoxText;
        var strNextItemValue = this._selectedItems[intSelectIndex + 1].SelectListBoxValue;
        var strNextItemSortColumn = this._selectedItems[intSelectIndex + 1].SelectListBoxSortColumn;
        var strNextItemCommonSortColumn = this._selectedItems[intSelectIndex + 1].CommonListBoxSortColumn;
        var strNextItemType = this._selectedItems[intSelectIndex + 1].SelectItemType;
        var strNextItemLocked = this._selectedItems[intSelectIndex + 1].Locked;
        //选中移动Item信息
        var strThisItemText = this._selectedItems[intSelectIndex].SelectListBoxText;
        var strThisItemValue = this._selectedItems[intSelectIndex].SelectListBoxValue;
        var strThisItemSortColumn = this._selectedItems[intSelectIndex].SelectListBoxSortColumn;
        var strThisItemCommonSortColumn = this._selectedItems[intSelectIndex].CommonListBoxSortColumn;
        var strThisItemType = this._selectedItems[intSelectIndex].SelectItemType;
        var strThisItemLocked = this._selectedItems[intSelectIndex].Locked;
        
        //和上一Item调换
        this._selectedItems[intSelectIndex + 1].SelectListBoxText = strThisItemText;
        this._selectedItems[intSelectIndex + 1].SelectListBoxValue = strThisItemValue;
        this._selectedItems[intSelectIndex + 1].SelectListBoxSortColumn = strThisItemSortColumn;
        this._selectedItems[intSelectIndex + 1].CommonListBoxSortColumn = strThisItemCommonSortColumn;
        this._selectedItems[intSelectIndex + 1].SelectItemType = strThisItemType;
        this._selectedItems[intSelectIndex + 1].Locked = strThisItemLocked;
        
        this._selectedItems[intSelectIndex].SelectListBoxText = strNextItemText;
        this._selectedItems[intSelectIndex].SelectListBoxValue = strNextItemValue;
        this._selectedItems[intSelectIndex].SelectListBoxSortColumn = strNextItemSortColumn;
        this._selectedItems[intSelectIndex].CommonListBoxSortColumn = strNextItemCommonSortColumn;
        this._selectedItems[intSelectIndex].SelectItemType = strNextItemType;
        this._selectedItems[intSelectIndex].Locked = strNextItemLocked;  
        
    },
   
    //取消事件按钮 
    _btnCancelClick : function()
    {   
        var selectedCount = this._getSelectCount(this._selectedList);
        if(selectedCount == 0)
            return;
        this._fillList(this._selectedList ,this._candidateList , this._selectedItems , this._candidateItems , this._selectedListSortDirection , "left" , false , "", $HGRootNS.selectActionType.UnSelect);        
    },
    //全部取消
    _btnCancelAllClick : function()
    {
        this._fillList(this._selectedList ,this._candidateList , this._selectedItems , this._candidateItems , this._selectedListSortDirection , "left" , true , "", $HGRootNS.selectActionType.UnSelect);        
    },
    
    //------------------------------------------------事件处理 结束-----------------------------------------------//
    
    // 加载ClientState
    loadClientState : function(value) 
    {
        if(value)
        {
            var fsArray = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            if(fsArray && fsArray.length > 0)
            {
                this.set_selectedItems(fsArray[0]);
                this.set_candidateItems(fsArray[1]);
                this.set_buttonItems(fsArray[2]);
                this._deltaItems = fsArray[3];
            }
        }
    },
    
    // 保存ClientState
    saveClientState : function() 
    {
        var arrayList = new Array(3);
        arrayList[0] = this.get_selectedItems();
        arrayList[1] = this.get_candidateItems();
        arrayList[2] = this.get_buttonItems();
        arrayList[3] = this.get_deltaItems();

        return Sys.Serialization.JavaScriptSerializer.serialize(arrayList);
    } 
}
    
$HGRootNS.DeluxeSelect.registerClass($HGRootNSName + ".DeluxeSelect", $HGRootNS.ControlBase);