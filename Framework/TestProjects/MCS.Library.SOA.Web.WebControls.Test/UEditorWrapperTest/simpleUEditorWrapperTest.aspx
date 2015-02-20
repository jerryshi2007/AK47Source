<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="simpleUEditorWrapperTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UEditorWrapperTest.simpleUEditorWrapperTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>百度UEditor控件的包装控件的简单测试</title>
    <script type="text/javascript">
        function test() {
            var bridge = UEditorWrapperBridge.getInstance();
            var bridge1 = UEditorWrapperBridge.getInstance();
            bridge.OpenDialog();
        }
    </script>
	<style type="text/css">
		.test
		{ position: relative;}
	</style>
</head>
<body>
	<form id="serverForm" runat="server">
	<div style="width: 700px; position: relative;">
		<asp:Button runat="server" ID="PostButton" Text="Submit" OnClick="PostButton_Click" />
        <%--<input id="Button1" type="button" value="button" onclick="test()"/>--%>
		<SOA:UEditorWrapper runat="server" ID="editor" Width="800px" InitialData=""
        ReadOnly="false" AutoDownloadUploadImages="true" AutoUploadImages="true"  RootPathName="ImageUploadRootPath"
			CssClass="test" 
			Toolbars="|,Undo,Redo,|,
Bold,Italic,Underline,StrikeThrough,Superscript,Subscript,RemoveFormat,FormatMatch,|,
BlockQuote,|,PastePlain,|,ForeColor,BackColor,InsertOrderedList,InsertUnorderedList,|,CustomStyle,
Paragraph,RowSpacing,LineHeight,FontFamily,FontSize,|,
DirectionalityLtr,DirectionalityRtl,|,Indent,|,
JustifyLeft,JustifyCenter,JustifyRight,JustifyJustify,|,
Link,Unlink,Anchor,|,InsertFrame,PageBreak,HighlightCode,|,
Horizontal,Date,Time,Spechars,|,
InsertTable,DeleteTable,InsertParagraphBeforeTable,InsertRow,DeleteRow,InsertCol,DeleteCol,MergeCells,MergeRight,MergeDown,SplittoCells,SplittoRows,SplittoCols,|,
SelectAll,ClearDoc,SearchReplace,Print,Preview,CheckImage,WordImage,InsertImage,DownloadImage,|" />
<%--        <SOA:UEditorWrapper runat="server" ID="UEditorWrapper1" Width="800px" InitialData="<B>千山鸟飞绝，万径人踪灭</B>" 
            RootPathName="UploadRootPath" />
--%>
	</div>
<%--	<div>
        <textarea id="TextArea1" cols="20" rows="2" runat="server"></textarea>
		提交后的编辑结果
	</div>--%>
	<h2>编辑内容展示</h2>
	<div id="divContent" runat="server" style="border-width:1px; border-style:solid"></div>
	</form>
</body>
</html>
