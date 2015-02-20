<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContinueDownloading.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.DownloadFile.ContinueDownloading" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Continue Downloading</title>
    <script type="text/javascript">
        function onDownloadClick() {
            var urls = [
                { url: 'http://km.sinooceanland.com/sites/ServiceBiz/Service11/4/1/RL-YGFW-V1.0-BZ.001_劳动合同期限标准.docx', fileName: '劳动合同期限标准.docx' },
                { url: 'http://km.sinooceanland.com/sites/ServiceBiz/Service11/4/1/RL-YGFW-V1.0-BZ.002_入职引导人确定标准.docx', fileName: '入职引导人确定标准.doc' }
            ];

            var shell = new ActiveXObject("Shell.Application");
            var folder = shell.BrowseForFolder(0, '', 64, 0x11);

            if (!folder) return;

            //启动个进度条
            for (var i in urls) {
                try {
                    $HBRootNS.fileIO.downloadFile(urls[i].url, folder.Self.Path + '\\' + urls[i].fileName, 'post', true);
                } catch (e) {
                    //出错的记录下
                }
            }
            //结束个进度条
            //提示个完成信息或者错误信息
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <SOA:ComponentHelperWrapper runat="server" ID="componentHelper" />
    </div>
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>
        <input type="button" onclick="onDownloadClick()" value="Download..." />
    </div>
    </form>
</body>
</html>
