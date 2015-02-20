<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordConfirmationEditorTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyForm.PasswordConfirmationEditorTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .x-form-item
        {
            display: block;
            margin-bottom: 4px;
            zoom: 1;
            font: normal 12px tahoma,arial,helvetica,sans-serif;
        }
        .x-form-item label.x-form-item-label
        {
            display: block;
            float: left;
            width: 100px;
            padding: 3px;
            padding-left: 0;
            clear: left;
            z-index: 2;
            position: relative;
            cursor: default;
        }
        element.style
        {
            padding-left: 225px;
        }
        
        .x-form-text, textarea.x-form-field
        {
            background-color: white;
            border-color: #B5B8C8;
        }
        .x-form-clear-left
        {
            clear: left;
            height: 0;
            overflow: hidden;
            line-height: 0;
            font-size: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
    </div>
    <div>
        <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" Height="300px" />
    </div>
    <div>
      <asp:button runat="server" ID="UserPasswordPersister" Text="UserPasswordPersister"  OnClick="UserPasswordPersister_Click" />
    </div>
    <div>
        <div class="x-form-item " tabindex="-1" id="ext-gen26">
            <label for="SimpleForm1_NumberBox1" style="width: 220px;" class="x-form-item-label"
                id="ext-gen27">
                Number Box 1：
            </label>
            <div class="x-form-element" id="x-form-el-SimpleForm1_NumberBox1" style="padding-left: 225px">
                <input type="text" size="20" autocomplete="off" id="SimpleForm1_NumberBox1" name="SimpleForm1$NumberBox1"
                    class=" x-form-text x-form-field x-form-num-field" style="width: 335px;"></div>
            <div class="x-form-clear-left">
            </div>
        </div>
        <div class="x-form-item " tabindex="-1" id="Div1">
            <label for="SimpleForm1_NumberBox1" style="width: 220px;" class="x-form-item-label"
                id="Label1">
                Number Box 1：
            </label>
            <div class="x-form-element" id="Div2" style="padding-left: 225px">
                <input type="text" size="20" autocomplete="off" id="Text1" name="SimpleForm1$NumberBox1"
                    class=" x-form-text x-form-field x-form-num-field" style="width: 335px;"></div>
            <div class="x-form-clear-left">
            </div>
        </div>
    </div>
    </form>
    <script src="PasswordConfirmationEditor.js" type="text/javascript"></script>
</body>
</html>
