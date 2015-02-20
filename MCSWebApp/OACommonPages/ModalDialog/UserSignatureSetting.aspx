<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserSignatureSetting.aspx.cs" Inherits="MCS.OA.CommonPages.ModalDialog.UserSignatureSetting" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="SOA" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>用户签名设置</title>
    <base target="_self"/>
    	<script type="text/javascript">
    	    function onConfirm() {
    	        Sys.Application.add_load(function () {
    	            var content = $find("editor").get_content();    	            
    	            window.returnValue = escape(content);
    	            top.close();
    	        });
    	    }
	</script>  
</head>
<body>
    <form id="form1" runat="server" target="_innerFrame">

    	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">请设置签名</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="text-align:center; vertical-align:top">
            	<div class="dialogContent" id="dialogContent" runat="server" style=" vertical-align:middle; padding:0 5px 0 2px;
				  text-align:left;">
					<!--Put your dialog content here... -->
                    <SOA:UEditorWrapper runat="server" ID="editor" Width="100%" InitialData="" RootPathName="ImageUploadRootPath" 
        ReadOnly="false" AutoDownloadUploadImages="true" AutoUploadImages="true" 
                        Toolbars="|,Undo,Redo,|,
Bold,Italic,Underline,StrikeThrough,Superscript,Subscript,RemoveFormat,FormatMatch,|,
BlockQuote,|,PastePlain,|,ForeColor,BackColor,InsertOrderedList,InsertUnorderedList,|,CustomStyle,
Paragraph,RowSpacing,LineHeight,FontFamily,FontSize,|,
DirectionalityLtr,DirectionalityRtl,|,Indent,|,
JustifyLeft,JustifyCenter,JustifyRight,JustifyJustify,|,
Link,Unlink,Anchor,|,InsertFrame,PageBreak,HighlightCode,|,
Horizontal,Date,Time,Spechars,|,
InsertTable,DeleteTable,InsertParagraphBeforeTable,InsertRow,DeleteRow,InsertCol,DeleteCol,MergeCells,MergeRight,MergeDown,SplittoCells,SplittoRows,SplittoCols,|,
SelectAll,ClearDoc,SearchReplace,Print,Preview,CheckImage,WordImage,DownloadImage,InsertImage,|,
Source" />

				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
                            <asp:Button runat="server" ID="btnConfirm" Text="确定(0)" class="formButton" OnClick="btnConfirm_Click" accesskey="O"/>
						</td>
						<td style="text-align: center;">
							<input type="button" onclick="top.close();" class="formButton"  value="取消(C)" id="Button1" accesskey="C"/>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>

    </form>
     <iframe style="display: none" id="innerFrame" name="_innerFrame"></iframe>
</body>
</html>
