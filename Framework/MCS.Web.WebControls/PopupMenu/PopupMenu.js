// -------------------------------------------------
// FileName	：	PopupMenu.js
// Remark	：	菜单控件
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    周维海	    20070412		创建
// -------------------------------------------------

$HGRootNS.PopupMenuItemSeparatorMode = function(){}
$HGRootNS.PopupMenuItemSeparatorMode.prototype = 
{
    None : 0,
    Bottom : 1,
    Top : 2
}
$HGRootNS.PopupMenuItemSeparatorMode.registerEnum($HGRootNSName + ".PopupMenuItemSeparatorMode");

$HGRootNS.PopupMenuNode = function(element)
{
    $HGRootNS.PopupMenuNode.initializeBase(this, [element]);
    
    this._childItems = null;
    this._popupMenu = null;
    this._isSeparator = false;
    this._enable = true;
    this._visible = true;
    this._imageUrl = "";
    this._text = "";
    this._toolTip = "";
    this._value = "";
    this._target = "_blank";
    this._navigateUrl = "";    
//    this._popOutImageUrl = "";
    //静态菜单项标示有子菜单的图片
    this._staticPopOutImageUrl = "";
    //菜单项目标示有子菜单的图片
    this._dynamicPopOutImageUrl = "";
    //菜单项左边的图片
    this._popOutImage = null;
    this._trItem = null;
    this._tdContent = null;
    this._imageContainer = null;
    this._trChildContainer = null;
    this._elementContainer = null;
    this._popupChildControl = null;
    this._currentActiveChildNode = null;
    this._separatorMode = $HGRootNS.PopupMenuItemSeparatorMode.None;
    this._selected = false;
    this._moveOut = false;
    this._nodeID = "";
    this._childControlsCreated = false;
    this._itemEvents = 
        {
            mouseover : Function.createDelegate(this, this._onItemMouseOver),
            mouseout : Function.createDelegate(this, this._onItemMouseOut),
            click : Function.createDelegate(this, this._onItemClick)
        };
    this._showEvents = Function.createDelegate(this, this._beforeShowCall);
    
}

$HGRootNS.PopupMenuNode.prototype = 
{   
    get_childItems : function()
    {
       //return this._childItems;
    },
    
    set_childItems : function(value)
    {
        //this._childItems = value;
    },
    
    get_popupMenu : function() {
        return this._popupMenu; 
    },
    set_popupMenu : function(value) { 
        if (this._popupMenu != value) {
            this._popupMenu = value; 
            this.raisePropertyChanged("popupMenu");
        }
    },
    
     get_isSeparator : function() {
        return this._isSeparator;
    },
   set_isSeparator : function(value) {
        if (this._isSeparator != value) {
            this._isSeparator = value;
            this.raisePropertyChanged('isSeparator');
        }
    },
    
    get_enable : function() {
        return this._enable;
    },
    set_enable : function(value) {
        if (this._enable != value) {
            this._enable = value;
            this.raisePropertyChanged('enable');
        }
    },
    
    get_visible : function() {
        return this._visible;
    },
    set_visible : function(value) {
        if (this._visible != value) {
            this._visible = value;
            this.raisePropertyChanged('visible');
        }
    },
    
    get_imageUrl : function() { 
        return this._parent; 
    },
    set_imageUrl : function(value) { 
        if (this._imageUrl != value) {
            this._imageUrl = value; 
            this.raisePropertyChanged("imageUrl");
        }
    },
    
    get_text : function() { 
        return this._text; 
    },
    set_text : function(value) { 
        if (this._text != value) {
            this._text = value; 
            this.raisePropertyChanged("text");
        }
    },
    
    get_toolTip : function() { 
        return this._text; 
    },
    set_toolTip : function(value) { 
        if (this._toolTip != value) {
            this._toolTip = value; 
            this.raisePropertyChanged("toolTip");
        }
    },    
    
    get_value : function() { 
        return this._value; 
    },
    set_value : function(value) { 
        if (this._value != value) {
            this._value = value; 
            this.raisePropertyChanged("value");
        }
    },
    
    get_target : function() { 
        return this._target; 
    },
    set_target : function(value) { 
        if (this._target != value) {
            this._target = value; 
            this.raisePropertyChanged("target");
        }
    },
    
    get_navigateUrl : function() { 
        return this._navigateUrl; 
    },
    set_navigateUrl : function(value) { 
        if (this._navigateUrl != value) {
            this._navigateUrl = value; 
            this.raisePropertyChanged("navigateUrl");
        }
    },
     
//    get_popOutImageUrl : function() { 
//        return this._popOutImageUrl; 
//    },    
//    set_popOutImageUrl : function(value) { 
//        if (this._popOutImageUrl != value) {
//            this._popOutImageUrl = value; 
//            this.raisePropertyChanged("popOutImageUrl");
//        }
//    },
    
    //静态菜单的图片
    get_staticPopOutImageUrl : function() { 
        return this._staticPopOutImageUrl; 
    },    
    set_staticPopOutImageUrl : function(value) { 
        if (this._staticPopOutImageUrl != value) {
            this._staticPopOutImageUrl = value; 
            this.raisePropertyChanged("staticPopOutImageUrl");
        }
    },
    
    get_dynamicPopOutImageUrl : function() { 
        return this._dynamicPopOutImageUrl; 
    },    
    set_dynamicPopOutImageUrl : function(value) { 
        if (this._dynamicPopOutImageUrl != value) {
            this._dynamicPopOutImageUrl = value; 
            this.raisePropertyChanged("dynamicPopOutImageUrl");
        }
    },
    
    get_currentActiveChildNode : function() {
        return this._currentActiveChildNode; 
    },
    set_currentActiveChildNode : function(value) { 
        if (this._currentActiveChildNode != value) {
            this._currentActiveChildNode = value; 
            this.raisePropertyChanged("currentActiveChildNode");
        }
    },
    
    get_separatorMode : function() {
        return this._separatorMode; 
    },
    set_separatorMode : function(value) { 
        if (this._separatorMode != value) {
            this._separatorMode = value; 
            this.raisePropertyChanged("separatorMode");
        }
    },
    
    get_selected : function() {
        return this._selected; 
    },
    //先清空再加载选中时的样式
    set_selected : function(value) { 
        if (this._selected != value) {
            this._selected = value;
            this.raisePropertyChanged("selected");
        }
        if (this._trItem)
        {
            this._removeItemCssClass(this._trItem.className);
            this._trItem.className = this._get_itemCssClass();
        }
    },
       
    get_hasDynamicChild : function()
    {
        return this.get_level() >= this._popupMenu.get_staticDisplayLevels();
    },
    
    get_isDynamicItem : function()
    {
         return this.get_level() > this._popupMenu.get_staticDisplayLevels();
    },
    
    get_DynamicRoot : function()
    {
        var root = this;
        while (root)
        {
            if (!root.get_isDynamicItem())
                return root;
                
            root = root.get_parent();
        }
        return null;
    },
    
    get_document : function()
    {
        if (this.get_isDynamicItem())
            return Sys.UI.DomElement.getParentWindow(this._parent.get_childNodeContainer()).document;
        else
            return document;
    },
      
    get_popupChildControl : function()
    {
        return this._popupChildControl;
    },
    /*********************************************/
    get_nodeID : function() { 
        return this._nodeID; 
    },
    set_nodeID : function(value) { 
        if (this._nodeID != value) {
            this._nodeID = value; 
            this.raisePropertyChanged("nodeID");
        }
    },
    
    initialize : function() {
        $HGRootNS.PopupMenuNode.callBaseMethod(this, 'initialize');
        //清空分割线其他属性项
        if(this._isSeparator)
        {
         this._text = "";
        }
        //if (!this.get_isDynamicItem())
            this.buildControl();

    },
    
    dispose : function()
    {
//         for (var i = 0; i < this._children.length; i++)
//        {
//         if(this._children[i]._trItem != null)
//         {
//          $HGDomEvent.removeHandlers(this._children[i]._trItem, this._itemEvents);
//         }
//        }

//$HGDomEvent.removeHandlers(this,this._itemEvents);

        var e = this.get_element();
        e.click = null;
        e.mouseover = null;
        this._popupMenu = null;
        this._popOutImage = null;
        this._trItem = null;
        this._tdContent = null;
        this._imageContainer = null;
        this._trChildContainer = null;
        this._popupChildControl = null;
        this._currentActiveChildNode = null;
        try
        {
            $HGRootNS.PopupMenuNode.callBaseMethod(this, 'dispose');
        }
        catch(e)
        {
        }
    },
    
    //创建菜单条目的内容
    buildControl : function()
    {
        var doc = $HGDomElement.get_currentDocument();
        $HGDomElement.set_currentDocument(this.get_document());
        this._buildContainer();
        this._buildControl();
        $HGDomElement.set_currentDocument(doc);
   },
    
////    buildChildControl : function()
////    {
////        if (!this._childControlsCreated)
////        {
////            for (var i = 0; i < this._children.length; i++)
////            {
////                var node = this._children[i];
////                this._childNodeContainer.appendChild(node.get_element());   
////                node.buildControl();  
////                if(this._children[i].get_hasChildNodes())
////                {
////    //              for(var grandchild = 0; grandchild < this._children[i]._children.length; grandchild++)
////    //              {
////                   //第１个孩子为隐藏，则不显示（孙子节点的visible属性）
////                   //处理将要显示的popup面板的下一级节点的visible状态
////                    $get("div").innerHTML = this._children[i]._children[0]._visible;
////                   if(!this._children[i]._children[0]._visible)
////                   Sys.UI.DomElement.setVisible(this._children[i]._popOutImage, false);      //"VISIBILITY: visible"    visibility "visible"
////    //               this._children[i]._popOutImage.cssText = 
////    //               this._children[i]._popOutImage.src = ""

////    //                  $get("div1").innerHTML = this._children[i]._popOutImage.visible;
////                      
////    //              }
////                }          
////            }
////            this._childControlsCreated = true;
////        }
////        //在show的时候检查子节点的子节点是否有被隐藏，如果有则显示字节点的时候不显示标识的图片
//////        $get("div").innerHTML = this._children[1]._popOutImage;
////        //this._childNodeContainer.appendChild(node.get_element());
////    },
    
    _buildContainer : function()
    {
        var elt = this.get_element();
        //如果是td则创建 table tbody 然后再创建菜单各项（横向则多个td并列）
        if (elt.tagName.toLowerCase() == "td")
        {
            var table = $HGDomElement.createElementFromTemplate({nodeName : "table"}, elt);

            var tbody = $HGDomElement.createElementFromTemplate({nodeName : "tbody"}, table);

            this._elementContainer = tbody;
        }
        else
        {
            this._elementContainer = elt;
        }
    },
    //设置选中状态的css
    _get_itemCssClass : function()
    {
        return this._selected ?
                this._popupMenu.get_selectedItemCssClass() :
                this._popupMenu.get_itemCssClass();

    },
    
    _get_itemHoverCssClass : function()
    {
        return this._popupMenu.get_hoverItemCssClass();
    },
    
    _buildControl : function()
    {
        //创建条目之前绘制分割线
//        if (this._separatorMode == $HGRootNS.PopupMenuItemSeparatorMode.Top)
//            this._buildSeparator();
        
        this._buildItem();                   
         
//        if (this._separatorMode == $HGRootNS.PopupMenuItemSeparatorMode.Bottom)
//            this._buildSeparator();    
   
//       if (this.get_hasChildNodes())
//        {
//            _ensureBuildChildContainer();
//        }
    },
    
    _get_indent : function()
    {
        return this.get_isDynamicItem() ? 0 : this._popupMenu.get_subMenuIndent() * (this.get_level() - 1);
    },
    
    _get_textIndent : function()
    {
        return this._popupMenu.get_textHeadWidth() + 
                (this._popupMenu.get_isImageIndent() ? 0 : this._get_indent());
    },
    
    //绘制分割线
    _buildSeparator : function()
    {
         var trSep = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TR"
             },
            this._elementContainer
        );

        var trContent = this._buildTrContent(trSep);
        
       this._buildTDHead(trContent, this._get_indent());
        
        var imageContainer = this._buildImageContainer(trContent);
       
        this._buildWidthTD(trContent, this._get_textIndent());
        
        var tdContent = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TD",
                properties :
                {
                   colSpan:2
                },
                cssClasses : [this._popupMenu.get_separatorCssClass()]                    
           },
            trContent
        );           
    },
    
    _buildTDHead : function(trContent, indent)
    {
        var headIndent = this._popupMenu.get_isImageIndent() ?
                indent : 0;
       this._buildWidthTD( trContent, headIndent);
    },
    
    _buildWidthTD : function(trContent, width)
    {       
         $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TD",
                properties :
                {
                   style : {width : width + "px"}
                }
            },
            trContent
        );
    },
    
    _buildImageContainer : function(trContent)
    {  
       //如果是分割线左边图片不添加样式
       var imageColCssClass = "";
       if(!this._isSeparator)
       {
         imageColCssClass = [this._popupMenu.get_imageColCssClass(),"popupMenu_Back"];
       }

        var td = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TD",
                cssClasses : imageColCssClass
           },
            trContent
        );
       
        return td;
    },
    /*build container for children node,first table*/
    _buildTrInTd : function(td)
    {
         var tbContent = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "Table",
                    properties :
                    {   
                        style :
                        {
                            width : "100%",
                            height : "100%"             
                        },
                        
                        cellPadding : 0,
                        cellSpacing : 0,
                        border : 0
                    }

                },
                td
            );
           
             var tbodyContent = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "TBody"
                },
                tbContent
            );
                   
            var trContent = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "TR"
                },
                tbodyContent
            );
            
            return trContent;
    },
    
    _buildTrContent : function(trItem)
    {       
       if (!this.get_isDynamicItem())
        {
            var tdItem = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "TD",
                    properties :
                    {
                        colSpan:4
                    }
                },
                trItem
            );
            
            var trContent = this._buildTrInTd(tdItem);
        }
        else
            var trContent = trItem;
            
        return trContent;
    },
    //创建菜单项的结构
    _buildItem : function()
    {  
       var separator = {};
       var trCss = "";   
       trEvent = null;
       if(this._isSeparator)
       {
        //如果是横向的第一级则没有分割线
        if(this._popupMenu.get_Orientation() == $HGRootNS.MenuOrientation.Horizontal && this.get_level()==1)
　　　　{
            separator = {display: "none"};
　　　　}
　　　　else
　　　　{
            separator = {height: "1px"};
　　　　}
        //"popupMenu_Default_Separator";//this._popupMenu.get_separatorCssClass();
       }
       else
       {      
       //如果不是隐藏项
       if(this._visible)
       {
       //如果是禁用项
       if(!this._enable)
       {
            separator = {color:"#ececec"};
            trCss = "popupMenu_Default_Item_Enable";
       }
       else
       {
            trCss = this._get_itemCssClass();
            trEvent = this._itemEvents;
        }
       }
       else
       {
            separator = {display: "none"};
       }
      }
       

       var trItem = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TR",
                properties :
                {
                    title : this._toolTip
                    ,style : separator   //改变菜单项的大小
                    
                },
                events : trEvent,
               cssClasses : [trCss]
            },
            this._elementContainer    //tbody
        );

        this._trItem = trItem;

        var trContent = this._buildTrContent(trItem);         
        
       this._buildTDHead(trContent, this._get_indent());
        
        var imageContainer = this._buildImageContainer(trContent);
        //imageContainer.style.height = "100%";
        this._imageContainer = imageContainer;
        //如果是分割线不添加左边图片
       if(!this._isSeparator)
       {
            var imageUrl = this._imageUrl || this._popupMenu.get_imageUrl();
            if (imageUrl)
            var image = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "img",
                    properties :
                    {
                       src : imageUrl
                    },
                    cssClasses : [this._popupMenu.get_imageCssClass()]
                },
                imageContainer
            );           
        }
                
        this._buildWidthTD(trContent, this._get_textIndent());
       //文本和右边图片之下的分割线
       var tdCss = "";
       if(!this._isSeparator)
       {
        tdCss = this._get_itemCssClass();
       }
       else
       {
       tdCss = "popupMenu_Default_Separator";
       }
       //菜单文本单元格
        var tdContent = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TD",
                properties :
                {
                   style : {whiteSpace: "nowrap"}
                },
                cssClasses : [tdCss]
            },
            trContent
        );
        this._tdContent = tdContent;     
        //菜单text
        var cursor;    
        if(this._enable)
       {
         cursor = "hand";
       }
       {
         cursor = "default";
       }
        var spanText =  $HGDomElement.createElementFromTemplate(
            {
                nodeName : "span",
                properties :
                {
                   innerText : this._text,
                   style : {cursor:cursor}                   
                }
            },
            tdContent
        );
        //菜单项右边的标识图标
        var tdPopupImage = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TD",
                properties :
                {
                   style : {width: "16px"},
                   align : "right"
                },
                cssClasses : [tdCss]        
            },
            trContent
        );
       if(!this._isSeparator) 
       {
        //优先加载 
        var outimg;
        if(this.get_level()<=this._popupMenu.get_staticDisplayLevels())
        {
         outimg = this._staticPopOutImageUrl || this._popupMenu.get_staticPopOutImageUrl();
        }
        else
        {
         outimg = this._dynamicPopOutImageUrl || this._popupMenu.get_dynamicPopOutImageUrl();
        }
        if(outimg == "" || outimg == null)
        {
         outimg = this._popupMenu.get_defaultPopOutImageUrl();
        }
 
         this._popOutImage = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "img",
                properties :
                {
                   src : outimg
                },
                visible : false
            },
            tdPopupImage
        );
        //再处理父节点的图片
        if(this.get_hasChildNodes())
        Sys.UI.DomElement.setVisible(this._popOutImage, true);
        var t;
        for(var i=0;i<this._children.length;i++) //if(this._children.length != 0)
        {
         t += this._children[i].get_text() //this._children[0].get_text();//this.get_firstChild().get_text();//this._children[0].get_element()._popOutImage;
        }
        if(this._children.length != 0)
        $get("div1").innerHTML = this._children[0]._visible;//this.get_hasChildNodes()//this._visible;
       }    
       
    },
    
    _removeChildContainer : function()
    {
        if (this._trChildContainer)
        {
            this.get_element().removeChild(this._trChildContainer);
            this._trChildContainer = null;
        }
        
        if (this._popupChildControl)
            this._popupChildControl = null;
    },
    
    _ensureBuildChildContainer : function()
    {       
        if (this.get_hasDynamicChild())
        {
           // if (!this._popupChildControl)
           // {
                var doc = $HGDomElement.get_currentDocument();
                $HGDomElement.set_currentDocument(this.get_document());
                this._buildDynamicChildNodeContainer();
                $HGDomElement.set_currentDocument(doc);
            //}
        }
        else
        {
            if (!this._trChildContainer)
                this._buildStaticChildNodeContainer();
        }        
    },
    
    _buildStaticChildNodeContainer : function()
    {
       var trContent = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TR"               
            },
            this.get_element()
        );
        this._trChildContainer = trContent;
        
         var tdContent = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "TD",
                properties :
                {
                    colSpan : 4
                }             
            },
            trContent
        );
        /*children node table*/
        if(true)
        {
        var childNodeContainer = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "Table",
                properties :
                {
                    width : "100%",
                    cellPadding : 0,
                    cellSpacing : 0,
                    border : 0
                }

            },
            tdContent
        );
        }
        else
        {
          var childNodeContainer = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "Table",
                properties :
                {
                    width : "100%",
                    cellPadding : 0,
                    cellSpacing : 0,
                    border : 0
                }

            },
            tdContent
        );
        }
        this._childNodeContainer = childNodeContainer;
    },
    
    //创建动态节点
    _buildDynamicChildNodeContainer : function()
    {   
//       $get("div").innerHTML = this._popOutImage._visible + "sss";


       //是分割线不创建图片,判断是否有图片
       if(this._popOutImage != null)
       {
        Sys.UI.DomElement.setVisible(this._popOutImage, true);
       } 
       //处理自身节点,鼠标移动到被隐藏图标的菜单项时，把状态恢复
        if(this.get_hasChildNodes())
       {  
       if(!this._children[0]._visible)
       {Sys.UI.DomElement.setVisible(this._popOutImage, false);}
        }
        //得到同一父节点
        var parentPopupControl = this.get_parent().get_popupChildControl ? this.get_parent().get_popupChildControl() : null;
        var position;
        if(this._popupMenu.get_Orientation() == $HGRootNS.MenuOrientation.Horizontal && this.get_level()==1)   //纵
        {
          position = $HGRootNS.PositioningMode.BottomRight; 
        }
        else
        {
          position = $HGRootNS.PositioningMode.RightTop;
        }

        if (!this._popupChildControl)
        {
            var popupControl = $create($HGRootNS.PopupControl, 
                {parent:parentPopupControl, positionElement:this._trItem, positioningMode:position}
                , {"beforeShow":this._showEvents}, null, null);
            
            this._popupChildControl = popupControl;
            this._initDynamicChildNodeContainer();
        }
    },
    
    _initDynamicChildNodeContainer : function()
    {
////        var popupBody = this._popupChildControl.get_popupBody();
//        var child = popupBody.childNodes[0];
//        if (child) popupBody.removeChild(child);
          var popBodyCssClass = this._popupMenu.get_popBodyCssClass();
         /*children node table*/
         if (!this._childNodeContainer)
         {
             var childNodeContainer = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "Table",
                    properties :
                    {  
                        cellPadding : 0,
                        cellSpacing : 0,
                        border : 0
                    },
                    cssClasses : [popBodyCssClass]
                },
                this._popupChildControl.get_popupBody(), null, this._popupChildControl.get_popupDocument()
            );
            
            this._childNodeContainer = childNodeContainer;
        }
    },
    
     
    createChildElement : function()
    {
        //创建popup
        this._ensureBuildChildContainer();
        var doc = Sys.UI.DomElement.getParentWindow(this.get_childNodeContainer()).document;
        var tbody = doc.createElement("tbody");
        return tbody;      
    },
    
//    appendChild : function(node)
//    {
//        $HGRootNS.PopupMenuNode.callBaseMethod(this, 'appendChild', [node]);
//    },    
   
    showPopup : function()
    {  
        if (this._popupChildControl)
        {
//            this._ensureBuildChildContainer();
//            //处理popup上节点的内容
//            this.buildChildControl();
            var hasVisibleChild = 0;
            
　　　　　　for(var i=0;i<this._children.length;i++)   
　　　　　　{
　　　　　　    var hasVisibleGrandchild = 0;
　　　　　　    
　　　　　　    //设置将要显示的节点为分割线的合理显示
　　　　　　    if(this._children[i]._isSeparator)
　　　　　　    {
　　　　　　        var preNode = false;
　　　　　　        var nextNode = false;
　　　　　　        //如果是最顶上和最后一个则隐藏分割线
　　　　　　        if(i==0 || i==this._children.length-1)
　　　　　　        {
　　　　　　            this._children[i]._trItem.style.display = "none";
　　　　　　        }
　　　　　　        else    //不是最后一项
　　　　　　        {   
　　　　　　            //如果下一条也是分割线，隐藏该分割线
　　　　　　            if((i < this._children.length) && this._children[i+1]._isSeparator)
　　　　　　            {
　　　　　　                this._children[i]._trItem.style.display = "none";
　　　　　　                break;
　　　　　　            }
　　　　　　            //寻找该分割线上面的项是否有可显示的项
　　　　　　            for(var pre=i-1;pre>0;pre--)
　　　　　　            {
　　　　　　                //正常显示的项
　　　　　　                if(this._children[pre]._visible && !this._children[pre]._isSeparator)
　　　　　　                {
　　　　　　                    preNode = true;
　　　　　　                    break;
　　　　　　                }
　　　　　　            }
　　　　　　            //寻找该分割线下面的项是否有可显示的项
　　　　　　            for(var next=i+1;next<this._children.length;next++)
　　　　　　            {
　　　　　　                if(this._children[pre]._visible && !this._children[pre]._isSeparator)
　　　　　　                {
　　　　　　                    nextNode = true;
　　　　　　                    break;
　　　　　　                }
　　　　　　            }
                        //如果前后都有没有可显示的项则隐藏当前的分割线
                        if(!preNode && !nextNode)
                        {
                            this._children[i]._trItem.style.display = "none";
                        }
　　　　　　        }
　　　　　　    }
　　　　　　    
　　　　　　    //如果将要显示的每个节点都为隐藏
　　　　　　    if(this._children[i]._visible)
　　　　　　    {
　　　　　　        //table里面
　　　　　　        hasVisibleChild++;
　　　　　　    }
　　　　　　    
　　　　　　    //将要显示的每个节点的子节点是否都为隐藏，若是则隐藏指示下一级的图片
　　　　　　    if(this._children[i].get_hasChildNodes())
　　　　　　    {
　　　　　　        for(var grandchild = 0; grandchild < this._children[i]._children.length; grandchild++)
　　　　　　        {
　　　　　　            if(this._children[i]._children[grandchild]._visible && !this._children[i]._children[grandchild]._isSeparator)
　　　　　　            {
　　　　　　                hasVisibleGrandchild++;
　　　　　　            }
　　　　　　        }
　　　　　　    }
　　　　　　    if(hasVisibleGrandchild==0)
　　　　　　    {
　　　　　　        Sys.UI.DomElement.setVisible(this._children[i]._popOutImage, false); 
　　　　　　    }
　　　　　　}
　　　　　　if(hasVisibleChild == 0)
　　　　　　{
　　　　　　    Sys.UI.DomElement.removeCssClass(this._popupChildControl.get_popupBody().childNodes[0], "popupMenu_Border");
　　　　　　}
            this._popupChildControl.show();
            $get('debug').innerHTML = this._children.length;

            //_isSeparator
       }  

    },
    
    _beforeShowCall : function(sender,e)
    {
     if(e.width < this._popupMenu.get_ItemFontWidth())
     {
       e.width = this._popupMenu.get_ItemFontWidth();
       this.get_childNodeContainer().style.width = this._popupMenu.get_ItemFontWidth();
     }

    },
    hidePopup : function()
    {
        if (this._popupChildControl)
        {
            this._popupChildControl.hide();
        }
    },
    
    _addItemCssClass : function(className)
    {
        Sys.UI.DomElement.addCssClass(this._imageContainer, className);
        Sys.UI.DomElement.addCssClass(this._trItem, className);
        Sys.UI.DomElement.addCssClass(this._tdContent, className);
    },
   
    _removeItemCssClass : function(className)
    {
        Sys.UI.DomElement.removeCssClass(this._imageContainer, className);
        Sys.UI.DomElement.removeCssClass(this._trItem, className);
        Sys.UI.DomElement.removeCssClass(this._tdContent, className);
    },
    
    setActive : function()
    {
        var cn = this._popupMenu.get_currentActiveChildNode();
        if (cn !== this)
        this._popupMenu.set_currentActiveChildNode(this);
            
        if (this != cn)
        {
            if (!this.get_isDynamicItem())
            {
                if (cn)
                {
                    var dRoot = cn.get_DynamicRoot();
                    dRoot.setInActive();
                    dRoot.hidePopup();   
                }
            }
            else
            {
                this.get_parent().enumerateChildren(this.setInActive);           
            }               

            this.showPopup();
            //添加鼠标移动过菜单项的样式
            var itemHoverCssClass = this._get_itemHoverCssClass();
            this._addItemCssClass(itemHoverCssClass);
       }        
    },
    
    setInActive : function()
    {
        this.hidePopup();
        var itemHoverCssClass = this._get_itemHoverCssClass();
        this._removeItemCssClass(itemHoverCssClass);
    },
    //鼠标移动事件
    _onItemMouseOver : function(e)
    {      
       this.setActive();

       if (this.get_hasDynamicChild())
        
       this.enumerateChildren(this.setInActive);
            
        this.raiseMover();
        e.stopPropagation();
        e.preventDefault();
    },

    _onItemMouseOut : function(e)
    {
//        this.setInActive();
//        e.stopPropagation();
//        e.preventDefault();
    },
    //点击事件
    _onItemClick : function(e)
    {
        if(this.raiseMclick(this,e))
        {
            if(this._popupMenu.get_MultiSelect())
            {
                //改变选中状态
                this._setNodeSelected(this._popupMenu._items,this.nodeID);
            }
        //设置样式
            this._popupMenu.set_selectedItem(this);
 
            if (this._navigateUrl)
            {
//            if (this._navigateUrl.startsWith("javascript:"))
//            {
//                eval(this_navigateUrl);
//            }
//            else
//                window.open(this._navigateUrl, this._target || this._popupMenu.get_target() || "_blank");
            this._popupMenu._link.href = this._navigateUrl;
            this._popupMenu._link.target = (this._target || this._popupMenu.get_target() || "_blank");
            this._popupMenu._link.click();
            }

            e.stopPropagation();
            e.preventDefault();
      }
    }
    //改变选中后item的数据
    ,_setNodeSelected : function(menuItems,nodeID)
    {
            if (menuItems)
            {
                for (var i = 0; i < menuItems.length; i++)
                {
                    if(nodeID==menuItems[i].nodeID)
                    {
                    //设置菜单数据结构的selected状态,viewstate保存其
                    if(this._selected)
                    {
                        menuItems[i].selected = false;
                    }
                    else
                    {
                        menuItems[i].selected = true;
                    }
                        break;
                    }

                    this._setNodeSelected(menuItems[i].childItems,nodeID);
                }

            }
    }
    /*************************************attach event*******************************************/   
    
    ,raiseMclick : function(item, eventElement)
    {
        var handlers = this._popupMenu.get_events().getHandler("mclick");
        var isContinue = true;
        if (handlers) 
        {
            var e = new Sys.EventArgs;
              
            e.item = item;
            e.cancel = false;
            e.eventElement = eventElement;

            handlers(this, e);

            if (e.cancel)
            {
                isContinue = false;
             }
        }
        
        return isContinue;
//        var eh = this.get_events().getHandler("menuItemClick");
//        if (eh) {
//            eh(this, args || Sys.EventArgs.Empty);
//        }
    }
    
    ,raiseMover : function()
    {
     var handlers = this._popupMenu.get_events().getHandler("mover");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
//        var eh = this.get_events().getHandler("menuItemClick");
//        if (eh) {
//            eh(this, args || Sys.EventArgs.Empty);
//        }
    }

}

$HGRootNS.PopupMenuNode.registerClass($HGRootNSName + '.PopupMenuNode', $HGRootNS.TreeNode);

/*enum*/
$HGRootNS.MenuOrientation = function() {
    throw Error.invalidOperation();
}
$HGRootNS.MenuOrientation.prototype = 
{
    Vertical: 0,
    Horizontal: 1
}
$HGRootNS.MenuOrientation.registerEnum($HGRootNSName + '.MenuOrientation');

$HGRootNS.PopupMenu = function(element)
{
    $HGRootNS.PopupMenu.initializeBase(this, [element]);
    //隐藏的link
    this._link = null;
    this._items = null;
    this._imageUrl = "";
    this._subMenuIndent = 10;
    this._staticDisplayLevels = 1;
    this._defaultPopOutImageUrl = null;

    this._staticPopOutImageUrl = null;
    this._dynamicPopOutImageUrl = null;
    this._target = "_blank";
    this._currentActiveChildNode = null;
    this._popBodyCssClass = "";
    this._cssClass = "";
    this._itemCssClass = "";
    this._hoverItemCssClass = "";
    this._selectedItemCssClass = "";
    this._separatorCssClass = "";
    this._textHeadWidth = 0;
    this._imageCssClass = "";
    this._imageColCssClass = "";
    this._isImageIndent = false;
    this._selectedItem = null;
    //
    //this._nodeID = "";
    //是否多选
    this._MultiSelect = false;
    this._ItemFontWidth = 100;
    this.counter = 0;
    //记录第一个出现的单选项
    this._singleSelectedCount = 0;
    this._selectedArray = new Array();
    //第一级的排列方向，默认为垂直
    this._Orientation = $HGRootNS.MenuOrientation.Vertical;
    this._popup = null;
    this._childNodesCreated = false;
    
    //event
//    this._documentEvents = Function.createDelegate(window.document.body, this._showPopup);
//    window.document.body.attachEvent('oncontextmenu', Function.createDelegate(this,this._showPopup));
    this._showEvents = Function.createDelegate(this, this._beforeShowCall);
}

$HGRootNS.PopupMenu.prototype = 
{
    get_Orientation : function() {
        return this._Orientation;
    },
    set_Orientation : function(value) {
        if (this._Orientation != value) {
            this._Orientation = value;
            this.raisePropertyChanged('Orientation');
        }
    },
    
    get_MultiSelect : function() {
        return this._MultiSelect;
    },
    set_MultiSelect : function(value) {
        if (this._MultiSelect != value) {
            this._MultiSelect = value;
            this.raisePropertyChanged('MultiSelect');
        }
    },
    
    get_ItemFontWidth : function() {
        return this._ItemFontWidth;
    },
    set_ItemFontWidth : function(value) {
        if (this._ItemFontWidth != value) {
            this._ItemFontWidth = value;
            this.raisePropertyChanged('ItemFontWidth');
        }
    },
    
    get_items : function() { 
        return this._items; 
    },
    set_items : function(value) { 
//        this._items = $HGCommon.convertToCamelObject(value);
        this._items = value;
        this.raisePropertyChanged("items");
    },
    
    get_imageUrl : function() { 
        return this._imageUrl; 
    },
    set_imageUrl : function(value) { 
        if (this._imageUrl != value) {
            this._imageUrl = value; 
            this.raisePropertyChanged("imageUrl");
        }
    },
    
    get_subMenuIndent : function() { 
        return this._subMenuIndent; 
    },
    set_subMenuIndent : function(value) { 
        if (this._subMenuIndent != value) {
            this._subMenuIndent = value; 
            this.raisePropertyChanged("subMenuIndent");
        }
    },
    
    get_staticDisplayLevels : function() { 
        return this._staticDisplayLevels; 
    },
    set_staticDisplayLevels : function(value) { 
        if (this._staticDisplayLevels != value) {
            this._staticDisplayLevels = value; 
            this.raisePropertyChanged("staticDisplayLevels");
        }
    },
    
    get_defaultPopOutImageUrl : function() { 
        return this._defaultPopOutImageUrl; 
    },    
    set_defaultPopOutImageUrl : function(value) { 
        if (this._defaultPopOutImageUrl != value) {
            this._defaultPopOutImageUrl = value; 
            this.raisePropertyChanged("defaultPopOutImageUrl");
        }
    },
    
    //静态菜但的图片
    get_staticPopOutImageUrl : function() { 
        return this._staticPopOutImageUrl; 
    },    
    set_staticPopOutImageUrl : function(value) { 
        if (this._staticPopOutImageUrl != value) {
            this._staticPopOutImageUrl = value; 
            this.raisePropertyChanged("staticPopOutImageUrl");
        }
    },
    
    get_dynamicPopOutImageUrl : function() { 
        return this._dynamicPopOutImageUrl; 
    },    
    set_dynamicPopOutImageUrl : function(value) { 
        if (this._dynamicPopOutImageUrl != value) {
            this._dynamicPopOutImageUrl = value; 
            this.raisePropertyChanged("dynamicPopOutImageUrl");
        }
    },
    
    get_target : function() { 
        return this._target; 
    },
    set_target : function(value) { 
        if (this._target != value) {
            this._target = value; 
            this.raisePropertyChanged("target");
        }
    },
    
    get_currentActiveChildNode : function() {
        return this._currentActiveChildNode; 
    },
    set_currentActiveChildNode : function(value) { 
        if (this._currentActiveChildNode != value) {
            this._currentActiveChildNode = value; 
            this.raisePropertyChanged("currentActiveChildNode");
        }
    },
    
    get_selectedItem : function() {
        return this._selectedItem; 
    },
    set_selectedItem : function(value) { 
        //单选记录之前选择的节点
        if (!this._MultiSelect && this._selectedItem != value) {
            if (this._selectedItem)
            {
                this._selectedItem.set_selected(false);
             }   
 
            this._selectedItem = value; 
        }
               //多选不记录
              //加载现在状态的样式（和当前的相反）
                  if(!value._selected)
                  {
                //this._selectedItem.set_selected(false);
                //当前node的selected
//                value._selected = false;
                    value.set_selected(true);
                   }
                  else
                  {
//                value._selected = true;
                    value.set_selected(false);

                   }

            this.raisePropertyChanged("selectedItem");

    },    
    
    get_cssClass : function() {
        return this._cssClass || " popupMenu_Default"; 
    },
    set_cssClass : function(value) { 
        if (this._cssClass != value) {
            this._cssClass = value; 
            this.raisePropertyChanged("cssClass");
        }
    },
    
    get_popBodyCssClass : function() {
        return this._popBodyCssClass || this.get_cssClass(); 
    },
    set_popBodyCssClass : function(value) { 
        if (this._popBodyCssClass != value) {
            this._popBodyCssClass = value; 
            this.raisePropertyChanged("popBodyCssClass");
        }
    },
    
    get_itemCssClass : function() {
        return this._itemCssClass || " popupMenu_Default_Item";
    },
    set_itemCssClass : function(value) { 
        if (this._itemCssClass != value) {
            this._itemCssClass = value;
            this.raisePropertyChanged("itemCssClass");
        }
    },
    
    get_hoverItemCssClass : function() {
        return this._hoverItemCssClass || " popupMenu_Default_ItemHover";
    },
    set_hoverItemCssClass : function(value) { 
        if (this._hoverItemCssClass != value) {
            this._hoverItemCssClass = value;
            this.raisePropertyChanged("hoverItemCssClass");
        }
    },
    
    get_selectedItemCssClass : function() {
        return this._selectedItemCssClass || " popupMenu_Default_ItemSelected";
    },
    set_selectedItemCssClass : function(value) { 
        if (this._selectedItemCssClass != value) {
            this._selectedItemCssClass = value;
            this.raisePropertyChanged("selectedItemCssClass");
        }
    },
    
    get_imageCssClass : function() {
        return this._imageCssClass || " popupMenu_Default_Image";
    },
    set_imageCssClass : function(value) { 
        if (this._imageCssClass != value) {
            this._imageCssClass = value;
            this.raisePropertyChanged("imageCssClass");
        }
    },
    
    get_imageColCssClass : function() {
        return this._imageColCssClass || " popupMenu_Default_ImageCol";
    },
    set_imageColCssClass : function(value) { 
        if (this._imageColCssClass != value) {
            this._imageColCssClass = value;
            this.raisePropertyChanged("imageColCssClass");
        }
    },
    
    get_separatorCssClass : function() {
        return this._separatorCssClass || " popupMenu_Default_Separator";
    },
    set_separatorCssClass : function(value) { 
        if (this._separatorCssClass != value) {
            this._separatorCssClass = value;
            this.raisePropertyChanged("separatorCssClass");
        }
    },
    
    get_isImageIndent : function() {
        return this._isImageIndent;
    },
    set_isImageIndent : function(value) { 
        if (this._isImageIndent != value) {
            this._isImageIndent = value;
            this.raisePropertyChanged("isImageIndent");
        }
    },
       
    get_textHeadWidth : function() {
        return this._textHeadWidth || 5;
    },
    set_textHeadWidth : function(value) { 
        if (this._textHeadWidth != value) {
            this._textHeadWidth = value;
            this.raisePropertyChanged("textHeadWidth");
        }
    },

    get_popupChildControl : function()
    {
        return this._popupChildControl;
    },
     
    initialize : function() {
        $HGRootNS.PopupMenu.callBaseMethod(this, 'initialize');
        //
//        this._popup = $create($HGRootNS.PopupControl, 
//            {parent:null, positionElement:null, positioningMode:$HGRootNS.PositioningMode.Absolute}
//            ,null, null, null);   
//            a.get_popupBody().innerHTML = "aaaaa";
//            a.show();

        //Sys.UI.DomElement.addCssClass(this.get_element(), this.get_cssClass());
        this._ensureBuildChildContainer();
        this._buildItems(this._items, this);
        //

        //创建隐藏的link,菜单项click时赋值并触发
        this._link = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "A",
                properties :
                {
                  style :
                  {
                   display: "none"
                  }       
                }   
            },
          this.get_element()
        );        
   },

    
    dispose : function() {    
        this._items = null;
        this._currentActiveChildNode = null;
        this._selectedItem = null;

        $HGRootNS.PopupMenu.callBaseMethod(this, 'dispose');
    },
//    //在sapn内,最外层的table，若为横向则添加tr Dom元素
//     _buildContainer : function()
//    {
//        var elt = this.get_element();
//        /*outer table what contain menu*/
//        //
//        
////       var table = $HGDomElement.createElementFromTemplate({
////            nodeName : "Table",
////            cssClasses : [""]
////        }, this._popup.get_popupBody(), null, this._popup.get_popupDocument());
//        
//        //
//       
//        var table = $HGDomElement.createElementFromTemplate(
//            {
//                nodeName : "Table",
//                properties :
//                {
//                    cellPadding : 0,
//                    cellSpacing : 0
////                    ,border : 0         
//                },
//             cssClasses : [this.get_cssClass()]       
//            },
//            elt
//        );
//       
//         if (this._Orientation == $HGRootNS.MenuOrientation.Vertical)   //纵
//         {
//         this.set_childNodeContainer(table);
//          }
//         else if (this._Orientation == $HGRootNS.MenuOrientation.Horizontal)   //横向
//         {  
//         var tbody = $HGDomElement.createElementFromTemplate({nodeName : "tbody"}, table);
//         var tr = $HGDomElement.createElementFromTemplate({nodeName : "tr"}, tbody);
//         this.set_childNodeContainer(tr);
//         }
//         
////        this._popup.get_popupBody().appendChild(this.get_childNodeContainer);
//    },
    
    _ensureBuildChildContainer : function()
    {       
        if (this.get_hasDynamicChild())
        {
            this._buildDynamicChildNodeContainer();      
        }
        else
        {
            if (!this._trChildContainer)
                this._buildStaticChildNodeContainer();
        }        
    },
    
    _buildStaticChildNodeContainer : function()
    {
       var elt = this.get_element();
      
        var table = $HGDomElement.createElementFromTemplate(
            {
                nodeName : "Table",
                properties :
                {
                    cellPadding : 0,
                    cellSpacing : 0
                },
             cssClasses : [this.get_cssClass()]       
            },
            elt
        );
       
        if (this._Orientation == $HGRootNS.MenuOrientation.Vertical)   //纵
         {
            this.set_childNodeContainer(table);
         }
        else if (this._Orientation == $HGRootNS.MenuOrientation.Horizontal)   //横向
         {  
            var tbody = $HGDomElement.createElementFromTemplate({nodeName : "tbody"}, table);
            var tr = $HGDomElement.createElementFromTemplate({nodeName : "tr"}, tbody);
            this.set_childNodeContainer(tr);
         }
    },
    
    //创建动态节点
    _buildDynamicChildNodeContainer : function()
    {   
       //是分割线不创建图片,判断是否有图片
       if(this._popOutImage != null)
       {
            Sys.UI.DomElement.setVisible(this._popOutImage, true);
       } 
       //处理自身节点,鼠标移动到被隐藏图标的菜单项时，把状态恢复
        if(this.get_hasChildNodes())
       {  
           if(!this._children[0]._visible)
           {
                Sys.UI.DomElement.setVisible(this._popOutImage, false);
           }
        }

        if (!this._popupChildControl)
        {
            var popupControl = $create($HGRootNS.PopupControl, {}
                , {"beforeShow":this._showEvents}, null, null);
            this._popupChildControl = popupControl;
            this._initDynamicChildNodeContainer();       
        }
        
    },
    
    _initDynamicChildNodeContainer : function()
    {
        var popupBody = this._popupChildControl.get_popupBody();
       // $HGDomElement.clearChildren(popupBody);
//        var child = popupBody.childNodes[0];
//        if (child) popupBody.removeChild(child);
          var popBodyCssClass = this._popBodyCssClass;
         /*children node table*/
         if (!this._childNodeContainer)
         {
             var childNodeContainer = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "Table",
                    properties :
                    {  
                        cellPadding : 0,
                        cellSpacing : 0,
                        border : 0
                    },
                    cssClasses : [popBodyCssClass,"popupMenu_Border"]
                },
                this._popupChildControl.get_popupBody(), null, this._popupChildControl.get_popupDocument()
            );
            
            this._childNodeContainer = childNodeContainer;
        }
        else
        {
            $HGDomElement.clearChildren(this._childNodeContainer);            
        }
    },
    
    get_hasDynamicChild : function()
    {
        return this._staticDisplayLevels == 0;
    },
    
    //加载数据
    _buildItems : function(items, parentNode)
    {
        if (items)
        {
            for (var i = 0; i < items.length; i++)
            {   
                var item = items[i];
                //改变数据
                //单选的时候
                if(!this._MultiSelect && this._singleSelectedCount<1 && item.selected)
                {
                    ++this._singleSelectedCount;
                    var menuNode = this.createNode(item, null, null, parentNode);
                    this._selectedItem = menuNode;
                }
                else if(!this._MultiSelect)   //全都为false
                {
                    item.selected = false;
                    var menuNode = this.createNode(item, null, null, parentNode);
                }
                //绘制
                else
                {
                    var menuNode = this.createNode(item, null, null, parentNode);
                }
               
                //有层级了再修改id值
                if(menuNode.get_level() != 1)
                {
                    menuNode.nodeID = menuNode.get_parent().nodeID+","+(i+1);

                    item.nodeID = menuNode.get_parent().nodeID+","+(i+1);
                }
                else   //node is root,first json element(i=0)
                {
                    menuNode.nodeID = i+1;

                    item.nodeID = i+1;
                }
                
                    parentNode.appendChild(menuNode);
                                
                    this._buildItems(item.childItems, menuNode);
                
            }
        }
    },
    
    //菜单项table里面的元素
     createChildElement : function()
    {
        var doc = Sys.UI.DomElement.getParentWindow(this.get_childNodeContainer()).document;  
        
        if (this._Orientation == $HGRootNS.MenuOrientation.Vertical)   //纵向
        {
            var tbody = doc.createElement("tbody"); 
            return tbody; 
        }
        else if (this._Orientation == $HGRootNS.MenuOrientation.Horizontal)   //横向
        {
            var td = doc.createElement("td"); 
            return td;
        }                  
    },

    ///properties:item数据;  parent:所属的已创建的节点
    createNode : function(properties, events, references, parent)
    {
        var elt = parent.createChildElement();
     
        //该item的popupMenu数据（item节点的全部数据）
        properties.popupMenu = this;
        properties.parent = parent;

        var node = $create($HGRootNS.PopupMenuNode, properties, events, references, elt);
     
        return node;
    }
    //自定义事件
    ,add_mclick : function(handler)
    {
        this.get_events().addHandler("mclick", handler);
    }
    ,remove_mclick : function(handler)
    {
        this.get_events().removeHandler("mclick", handler);
    }
    
    ,add_mover : function(handler)
    {
    this.get_events().addHandler("mover", handler);
    }
    ,remove_mover : function(handler)
    {
     this.get_events().removeHandler("mover", handler);
    }
    /**************************postdata************************************/
     ,loadClientState : function(value) {

        if(value)
        {
            var deItems = Sys.Serialization.JavaScriptSerializer.deserialize(value);

            if( deItems != null && deItems.length !=0 )
            {
                this._receiveArray(this._items,deItems,this.counter);
            }
          
        }
    }
    
    ,saveClientState : function()
    {

      if(this._items != null)
      {
         this._bulidArray(this._items);
      }

      return Sys.Serialization.JavaScriptSerializer.serialize(this._selectedArray);

    }
    
    /*recursion*/
    ,_bulidArray : function(items)
    {

            if (items)
            {
                for (var i = 0; i < items.length; i++)
                {
                    Array.add(this._selectedArray,new Array(items[i].nodeID, items[i].selected));//SetValue(this._Items[i].NodeID

                    this._bulidArray(items[i].childItems);
                }
            }
    }

    ,_receiveArray : function(items,deItems)
    { 
            
            if (items.length != 0)
            {
                for (var i = 0; i < items.length; i++)
                {
                    
                    items[i].nodeID = deItems[this.counter][0];
                    
//                    items[i].selected = deItems[this.counter][1];
                    ++this.counter;
                    this._receiveArray(items[i].childItems, deItems);

                }
            }
         
    },
        
    
    showPopupMenu : function(x, y)
    {
        this._popupChildControl.set_x(x);
        this._popupChildControl.set_y(y);
      
        this._popupChildControl.show();
    },   
    
    _beforeShowCall : function(sender,e)
    {
         if(e.width < this._ItemFontWidth)
         {
           e.width = this._ItemFontWidth;
           this.get_childNodeContainer().style.width = this._ItemFontWidth;
         }
    }

}
$HGRootNS.PopupMenu.registerClass($HGRootNSName + '.PopupMenu', $HGRootNS.TreeNode);
