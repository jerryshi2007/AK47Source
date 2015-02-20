$HGRootNS.DeluxeSearchClient = function (element) {

	$HGRootNS.DeluxeSearchClient.initializeBase(this, [element]);

	this.bindingControlID = null;

	this._applicationLoadDelegate = null;
};

$HGRootNS.DeluxeSearchClient.prototype =
 {
     initialize: function () {
         $HGRootNS.DeluxeSearchClient.callBaseMethod(this, "initialize");

         this._applicationLoadDelegate = Function.createDelegate(this, this._applicationLoad);

         Sys.Application.add_load(this._applicationLoadDelegate);
     },

     _convertObject: function (originalObj) {
         var result = {};

         for (var item in originalObj.List) {
             result[originalObj.List[item].DataField] = originalObj.List[item].Data;
         }

         return result;
     },

     _applicationLoad: function () {
         
         var args = window.dialogArguments;

         if ("undefined" != typeof (args)) {
             
             var result = Sys.Serialization.JavaScriptSerializer.deserialize(args);

             var data = this._convertObject(result);

             if (!this.isEmpty(data)) {

                 if ("" == this.bindingControlID) {

                     this.bindingControlID = this._getDataBindingControlID();
                 }

                 if ("" != this.bindingControlID) $find(this.bindingControlID).dataBind(data);                                   
                 
             }
         }
     },
     _getDataBindingControlID: function () {

         for (var controlID in Sys.Application._components) {

             var component = Sys.Application._components[controlID];

             if (component.get_serverControlType().indexOf("DataBindingControl") != -1) {

                 if ("undefined" != typeof (component)) {
                     return component.get_id();
                 }
             }
         }
         
         return "";
     },
     isEmpty: function (obj) {
         for (var name in obj) {
             return false;
         }

         return true;
     },

     dispose: function () {
         var element = this.get_element();

         $clearHandlers(element);

         $HGRootNS.DeluxeSearchClient.callBaseMethod(this, "dispose");
     }
 };

$HGRootNS.DeluxeSearchClient.registerClass($HGRootNSName + ".DeluxeSearchClient", $HGRootNS.ControlBase);