<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LinkButtonTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.LinkButtonTest" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="HB" %>
<%@ Register assembly="MCS.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="cc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function onChange(calendar, e) {
            var value = calendar.get_value();
            $find("dateInput2").set_value(value);
        }
        window.showModalDialog(window.location.href, "");
    </script>
</head>
<body>
    <form id="form1" runat="server">

  
    <div>
		<HB:DataBindingControl runat="server" ID="bindingControl" IsValidateOnSubmit="true" 
			AutoBinding="true" ValidateUnbindProperties="false" AllowClientCollectData="false"  AutoValidate="true">
        			<ItemBindings>
<%--        	<HB:DataBindingItem ControlID="dateInput" DataPropertyName="DateInput" ControlPropertyName="Value"  AutoFormatOnBlur="false"
					ClientIsHtmlElement="false" ClientPropName="get_value" ClientSetPropName="set_value"  IsValidate="true"/>--%>
                  
                       <HB:DataBindingItem ControlID="OuUserInputControl1" DataPropertyName="User" ControlPropertyName="SelectedSingleData"
                 ClientPropName="get_selectedOuUserData" ClientSetPropName="set_selectedOuUserData" AutoFormatOnBlur="true" ClientIsHtmlElement="false"  IsValidate="true"/>
                   </ItemBindings>
        </HB:DataBindingControl>
    
    </div>  
    <cc2:DeluxeCalendar ID="dateInput" OnClientValueChanged="onChange" runat="server">
    </cc2:DeluxeCalendar>
     <cc2:DeluxeCalendar ID="dateInput2" runat="server">
    </cc2:DeluxeCalendar>
    <br />
    <HB:OuUserInputControl ID="OuUserInputControl1" runat="server" />
    <br />
    <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
    <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">LinkButton</asp:LinkButton>
    </form>
    <script type="text/javascript">        
        var Sys$WebForms$PageRequestManager$_doPostBack =  function(eventTarget, eventArgument) {
            
        var event = window.event;
        if (!event) {
            var caller = arguments.callee ? arguments.callee.caller : null;
            if (caller) {
                var recursionLimit = 30;
                while (caller.arguments.callee.caller && --recursionLimit) {
                    caller = caller.arguments.callee.caller;
                }
                event = (recursionLimit && caller.arguments.length) ? caller.arguments[0] : null;
            }
        }
        this._additionalInput = null;
        var form = this._form;
        if ((eventTarget === null) || (typeof(eventTarget) === "undefined") || (this._isCrossPost)) {
            this._postBackSettings = this._createPostBackSettings(false);
            this._isCrossPost = false;
        }
        else {
            var mpUniqueID = this._masterPageUniqueID;
            var clientID = this._uniqueIDToClientID(eventTarget);
            var postBackElement = document.getElementById(clientID);
            if (!postBackElement && mpUniqueID) {
                if (eventTarget.indexOf(mpUniqueID + "$") === 0) {
                    postBackElement = document.getElementById(clientID.substr(mpUniqueID.length + 1));
                }
            }
            if (!postBackElement) {
                if (Array.contains(this._asyncPostBackControlIDs, eventTarget)) {
                    this._postBackSettings = this._createPostBackSettings(true, null, eventTarget);
                }
                else {
                    if (Array.contains(this._postBackControlIDs, eventTarget)) {
                        this._postBackSettings = this._createPostBackSettings(false);
                    }
                    else {
                        var nearestUniqueIDMatch = this._findNearestElement(eventTarget);
                        if (nearestUniqueIDMatch) {
                            this._postBackSettings = this._getPostBackSettings(nearestUniqueIDMatch, eventTarget);
                        }
                        else {
                            if (mpUniqueID) {
                                mpUniqueID += "$";
                                if (eventTarget.indexOf(mpUniqueID) === 0) {
                                    nearestUniqueIDMatch = this._findNearestElement(eventTarget.substr(mpUniqueID.length));
                                }
                            }
                            if (nearestUniqueIDMatch) {
                                this._postBackSettings = this._getPostBackSettings(nearestUniqueIDMatch, eventTarget);
                            }
                            else {
                                var activeElement;
                                try {
                                    activeElement = event ? (event.target || event.srcElement) : null;
                                }
                                catch(ex) {
                                }
                                activeElement = activeElement || this._activeElement;
                                var causesPostback = /__doPostBack\(|WebForm_DoPostBackWithOptions\(/;
                                function testCausesPostBack(attr) {
                                    attr = attr ? attr.toString() : "";
                                    return (causesPostback.test(attr) &&
                                        (attr.indexOf("'" + eventTarget + "'") !== -1) || (attr.indexOf('"' + eventTarget + '"') !== -1));
                                }
                                if (activeElement && (
                                        (activeElement.name === eventTarget) ||
                                        testCausesPostBack(activeElement.href) ||
                                        testCausesPostBack(activeElement.onclick) ||
                                        testCausesPostBack(activeElement.onchange)
                                        )) {
                                    this._postBackSettings = this._getPostBackSettings(activeElement, eventTarget);
                                }
                                else {
                                    this._postBackSettings = this._createPostBackSettings(false);
                                }
                            }
                        }
                    }
                }
            }
            else {
                this._postBackSettings = this._getPostBackSettings(postBackElement, eventTarget);
            }
        }
        if (!this._postBackSettings.async) {
            for (i = 0, l = this._onSubmitStatements.length; i < l; i++) {
                if (!this._onSubmitStatements[i]()) {
                    continueSubmit = false;
                    break;
                }
            }
            form.onsubmit = this._onsubmit;
            this._originalDoPostBack(eventTarget, eventArgument);
            form.onsubmit = null;
            return;
        }
        form.__EVENTTARGET.value = eventTarget;
        form.__EVENTARGUMENT.value = eventArgument;
        this._onFormSubmit();
    }

    //Sys.WebForms.PageRequestManager.prototype._doPostBack = Sys$WebForms$PageRequestManager$_doPostBack;
    </script>
</body>
</html>
