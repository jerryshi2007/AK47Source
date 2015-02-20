// -------------------------------------------------
// FileName	：	MaterialControl.js
// Remark	：	    附件控件
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    张梁	    20070620		创建
// -------------------------------------------------
 
$HGRootNS.MaterialControl = function(element)
{
	$HGRootNS.MaterialControl.initializeBase(this, [element]);
	
	this._displayText = null;
	this ._materialTableShowMode = null ;
	this._materialTable = null ;
	this._materialTablebody = null ;
	this._materialList = null ;	
	this._link = null ;
	
	this._popUpMaterialTableStyle = null ; 
	this._popUpMaterialTableCellStyle = null ; 
	this._popUpMaterialTableHeadCellStyle =null ;
	this._popUpMaterialTableCellInputStyle = null ;
	this._materialTableStyle = null ;
	this._materialTableCellStyle = null ;  
	this._popUpBodyStyle = null ;
	
	this._uploadFilePageUrl =null ;
	this._fileUrlRoot = null ;
	this._cssUrl = null ;
	this._hBWebHelperControl = null ;
	this._applicationName = null ;
	this._programName =null ;
	this._user = null ;
	this._editable = false ;
	this._allowUpload =    true  ;
	this._allowDelete =    true ;
	this._allowAdjustOrder =    true ;
	this._trackRevisions  =    null ;
	this._resourceID = null ;
	this._class = null ;
	this._wfActivity = null ;
	this._forbidUploadExpandName = null ;
	this._mainVersionIDs = null ;
	this._showAllVersion = null ;
	this._department = null ;
	
	this._btnEvents = 
		{
			click : Function.createDelegate(this, this._showUploadDialog)
		};
	 
		this._onBtnSave = 
	   {
		  click : Function.createDelegate(this, this._onSave)
	   };
}
 
$HGRootNS.MaterialControl.prototype = 
{
	initialize : function()
	{
		$HGRootNS.MaterialControl.callBaseMethod(this, 'initialize');
		this._buildControl();
	},
	
	dispose : function()
	{
	   if (this._link)
	   {
			$HGCommon.removeHandlers(this._link, this._btnEvents);
			this._link = null;
		}
		
		this._materialTablebody = null ; 
		this._materialTable = null ;
		this._materialList = null ; 
		this._popUpMaterialTableStyle = null ; 
		this._popUpMaterialTableCellStyle = null ; 
		this._popUpMaterialTableHeadCellStyle =null ;
		this._popUpMaterialTableCellInputStyle = null ;
		this._materialTableStyle = null ;
		this._materialTableCellStyle = null ;  
		this._popUpBodyStyle = null ;
		this._user  = null ; 
		this._wfActivity = null ;
		this._department = null ;
		
		$HGRootNS.MaterialControl.callBaseMethod(this, 'dispose');
	},
	
	_buildControl : function()
	{
			var element = this.get_element();
		  
			this._link = $HGCommon.createElementFromTemplate(
			{
					nodeName : "a",
					properties : { innerHTML :   this._displayText , target : "_blank" , className : "Link"},
					events : this._btnEvents
				},
				element
			);   
			 
		/*	var btn  = $HGCommon.createElementFromTemplate(
			{
					nodeName : "input",
					properties : { type : "button", value : "save" },
					events : this._onBtnSave
			},
				element
			);   
		*/
			this._buildMaterialTable(element) ; 
			this._showMaterialList() ;
	}, 
 
   _onSave : function()
   {  
	  this._invoke("ReinitMaterial",  [ Sys.Serialization.JavaScriptSerializer.serialize(this._materialList ) ]   ,        Function.createDelegate(this, this._invokeCallback));      
   } , 
  
	_invokeCallback : function(result)
	{
		alert(  result);
	},
	 
   _showMaterialList : function()
  {  
		var cntMaterial = 0 ;
		 
		  //InOneLine ,  clear all cells 
		  if ( this ._materialTableShowMode == 1 && this._materialTablebody.childNodes.length != 0    )
		  { 
				  cntMaterial =    this._materialTablebody.firstChild.childNodes.length ;
				   for( var i = 0 ; i <  cntMaterial ; i ++ )
				   {
						 this._materialTablebody.firstChild.deleteCell(0) ;
				   }
		  }
		  else
		  {       //MultiLine  clear all rows  
				   cntMaterial =    this._materialTablebody.childNodes.length ;                    
				   for( var i = 0 ; i< cntMaterial ; i ++ )
				   {    
						 this._materialTablebody.deleteRow( 0 ) ;
				   }
		  }

		for( var i = 0 ; i< this._materialList.length ; i ++ ) 
		{
			   this._addNewMatrerial("<a id='" +    this._materialList[i].ID + "' href='"  +  this._fileUrlRoot  + "/"
				+   this._materialList[i].FilePath +"'  target='_blank'>" +  this._materialList[i].Title+"</a>") ;
		}		
  } ,
  
	 _showUploadDialog :   function()
	{ 
	   var url =   this._uploadFilePageUrl  + "&PostFileToPage=" + escape( window.location.href ) ;
				
	   var materialList  = showModalDialog( url , this ,  "dialogWidth:500px; dialogHeight:700px;center:yes;help:no;resizable:no;scroll:yes;status:no"); 
  
	   if( materialList  !=null )  
	   {     
			this._materialList =   this._formatMaterialList(materialList) ;
			this._showMaterialList() ;
	   }
	},
	
	//格式化
	_formatMaterialList : function( materialList  )
	{
		var newMaterialList = new Array() ; 
		var material ;
		
	   for(var i = 0 ; i< materialList.length ; i ++)
	   {
						material =
						{
							 "ID": materialList[i].ID,
							 "ResourceID": materialList[i].ResourceID ,
							 "SortID":materialList[i].SortID,
							 "Class":materialList[i].Class ,
							 "Title":materialList[i].Title,
							 "PageQuantity": materialList[i].PageQuantity,
							 "FilePath": materialList[i].FilePath,
							 "OriginalName": materialList[i].OriginalName
						 } ; 
						 
						 newMaterialList.push( material ) ;
	   }
	   
	   return  newMaterialList ;
	}, 
	
	_setStyle : function( element , style )
	 { 
		 $HGCommon.setStyle( element ,  style ) ;     
	 },
	 
	   _buildMaterialTable : function(element  )
	  {
		 this._materialTable = $HGCommon.createElementFromTemplate(
				{
					nodeName : "TABLE"  
				},
				element
			);
		
		if( this._materialTableStyle == null    ) 
			this._materialTable.className =  "MaterialTableCss"  ;
		else
			this._setStyle( this._materialTable ,  this._materialTableStyle  ) ;
 
		this._materialTablebody = $HGCommon.createElementFromTemplate(
				{
					nodeName : "TBODY" 
				},
				this._materialTable
			);
	  },
  
		_addNewMatrerial : function(   fileName )
	   {
		 //InOneLine
		 if ( this ._materialTableShowMode == 1 && this._materialTablebody.childNodes.length !=  0 )
		{ 
			   this._buildMaterialTableCell(  this._materialTablebody.childNodes[0] , fileName ) ;           
			   return ;
		 }
		
		 //MultiLine 
		 this._buildMaterialTableCell( this._buildMaterialTableRow( this._materialTablebody ) , fileName   ) ;     
	   } ,
	 
	 _buildMaterialTableRow : function( materialTablebody )
	  {                    
		  var materialTableRow = $HGCommon.createElementFromTemplate(
				{
					nodeName : "TR"
				},
				materialTablebody
			);  
			
			return  materialTableRow ;
	  } ,

	  _buildMaterialTableCell : function( materialTableRow , cellContent )
	  {
			 var materialTableCell = $HGCommon.createElementFromTemplate(
				{
					nodeName : "TD" ,
					properties : { innerHTML : cellContent }   
				},
				materialTableRow
			);
		 
		if( this._materialTableCellStyle == null  ) 
			materialTableCell.className =  "MaterialTableCellCss"  ;
		else
			this._setStyle( materialTableCell ,  this._materialTableCellStyle  ) ;    
 
			return materialTableCell ;	
	  } , 
		 
	set_displayText : function(value)
	{
		this._displayText = value  ;
	},
	get_displayText : function()
	{
		return this._displayText;
	}  ,
	 
	set_materialTableShowMode : function(value)
	{
	   this._materialTableShowMode = value; 
	},
	get_materialTableShowMode : function()
	{
	  return this._materialTableShowMode;
	} ,
	
	set_materialTableStyle : function(value)
	{
	   this._materialTableStyle = value; 
	},
	get_materialTableStyle : function()
	{
		return this._materialTableStyle;
	},
   
	set_materialTableCellStyle : function(value)
	{   
	   this._materialTableCellStyle = value; 
	},
	get_materialTableCellStyle : function()
	{ 
		return this._materialTableCellStyle;
	},
	 
	  
	set_popUpMaterialTableStyle : function(value)
	{
	   this._popUpMaterialTableStyle= value; 
	},
	get_popUpMaterialTableStyle : function()
	{
	  return this._popUpMaterialTableStyle;
	} ,
	
	 set_popUpMaterialTableCellStyle : function(value)
	{
	   this._popUpMaterialTableCellStyle= value; 
	},
	get_popUpMaterialTableCellStyle : function()
	{
	  return this._popUpMaterialTableCellStyle;
	} ,
  
	set_popUpMaterialTableHeadCellStyle : function(value)
	{
	   this._popUpMaterialTableHeadCellStyle= value; 
	},
	get_popUpMaterialTableHeadCellStyle : function()
	{
	  return this._popUpMaterialTableHeadCellStyle;
	} ,
	 
	set_popUpMaterialTableCellInputStyle: function(value)
	{
	   this._popUpMaterialTableCellInputStyle = value; 
	},
	get_popUpMaterialTableCellInputStyle: function()
	{
	  return this._popUpMaterialTableCellInputStyle;
	},
	
	set_popUpBodyStyle: function(value)
	{
	   this._popUpBodyStyle = value; 
	},
	get_popUpBodyStyle: function()
	{
	  return this._popUpBodyStyle;
	} ,
	
	set_uploadFilePageUrl: function(value)
	{
	   this._uploadFilePageUrl = value; 
	},
	get_uploadFilePageUrl: function()
	{
	  return this._uploadFilePageUrl;
	}, 
	
	set_hBWebHelperControl: function(value)
	{
	   this._hBWebHelperControl = value; 
	},
	get_hBWebHelperControl: function()
	{
	  return this._hBWebHelperControl;
	},
	
	set_cssUrl: function(value)
	{
	   this._cssUrl = value; 
	},
	get_cssUrl: function()
	{
	  return this._cssUrl;
	},
	
	 set_materialList : function(value)
	{
	   this._materialList= value; 
	},
	get_materialList : function()
	{
	  return this._materialList;
	} ,
	
	set_fileUrlRoot : function(value)
	{    
	   this._fileUrlRoot= value; 
	},
	get_fileUrlRoot : function()
	{
	  return this._fileUrlRoot;
	} ,
	
	set_applicationName : function(value)
	{    
	   this._applicationName= value; 
	},
	get_applicationName : function()
	{
	  return this._applicationName;
	} ,
	 
	set_programName : function(value)
	{    
	   this._programName= value; 
	},
	get_programName : function()
	{
	  return this._programName ;
	}  ,
	
	set_user : function(value)
	{    
	   this._user= value; 
	},
	get_user : function()
	{
	  return this._user ;
	}  ,
	
	set_editable : function(value)
	{    
	   this._editable= value; 
	},
	get_editable : function()
	{
	  return this._editable ;
	}  ,
	 
	set_allowUpload : function(value)
	{    
	   this._allowUpload = value; 
	},
	get_allowUpload : function()
	{
	  return this._allowUpload ;
	}  ,
	
	set_allowDelete : function(value)
	{    
	   this._allowDelete= value; 
	},
	get_allowDelete : function()
	{
	  return this._allowDelete ;
	}  ,
	
	set_allowAdjustOrder : function(value)
	{    
	   this._allowAdjustOrder= value; 
	},
	get_allowAdjustOrder : function()
	{
	  return this._allowAdjustOrder ;
	}  ,
	
	set_trackRevisions : function(value)
	{    
	   this._trackRevisions = value; 
	},
	get_trackRevisions : function()
	{
	  return this._trackRevisions ;
	}  ,
	
	set_resourceID : function(value)
	{    
	   this._resourceID = value; 
	},
	get_resourceID : function()
	{
	  return this._resourceID ;
	} ,
	
	set_class : function(value)
	{    
	   this._class = value; 
	},
	get_class : function()
	{
	  return this._class ;
	} ,
	
	set_wfActivity : function(value)
	{    
	   this._wfActivity = value; 
	},
	get_wfActivity : function()
	{
	  return this._wfActivity ;
	} ,
	
	set_forbidUploadExpandName : function(value)
	{    
	   this._forbidUploadExpandName = value; 
	},
	get_forbidUploadExpandName : function()
	{
	  return this._forbidUploadExpandName ;
	} ,
	
	set_mainVersionIDs : function(value)
	{    
	   this._mainVersionIDs = value; 
	},
	get_mainVersionIDs : function()
	{
	  return this._mainVersionIDs ;
	} ,
	
	set_showAllVersion : function(value)
	{    
	   this._showAllVersion = value; 
	},
	get_showAllVersion : function()
	{
	  return this._showAllVersion ;
	} ,
	
	set_department : function(value)
	{    
	   this._department = value; 
	},
	get_department : function()
	{
	  return this._department ;
	} 
	
 }

$HGRootNS.MaterialControl.registerClass($HGRootNSName + ".MaterialControl", $HGRootNS.ControlBase);