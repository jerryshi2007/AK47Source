<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadTestForIE10.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.TestPages.UploadTestForIE10" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="SOA" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
// <![CDATA[

        function btnUpload_onclick() {
            try {
                var localPath = fileInput.value;
                if (localPath == "") {
                    alert("请选择文件！");
                    return;
                }

                var componentHelperActiveX = document.getElementById("componentHelperActiveX");
                var stream = componentHelperActiveX.createObject("ADODB.Stream");
                stream.type = 1; //二进制方式
                //stream.mode = 3; //read write    
                stream.open();

                try {
                    _xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
                    //_xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                    stream.loadFromFile(localPath);

                    stream.position = 0;                        
                   
                    _xmlHttp.onreadystatechange = handleStateChange;
                    _xmlHttp.open("POST", window.location + "?fileName=" + escape(localPath), true);
                    //_xmlHttp.setRequestHeader("Content-length", stream.size);
                    //                    _xmlHttp.setRequestHeader("Content-type", "application/octet-stream");
                    //                    _xmlHttp.setRequestHeader("Content-length", 100);
                    //                    _xmlHttp.setRequestHeader("Connection", "close");

                   
                    _xmlHttp.send(stream.read(stream.size)); //Send the stream  
                }
                catch (ex) {
                    alert(ex);
                }
                finally {
                    stream.close();
                }
            }
            catch (e) {
                alert(e.message);
            }
        }

        function checkXmlHttpError(xmlHttp) {
            return ("状态码：" + xmlHttp.status + "\n" + "响应状态：" + xmlHttp.statusText);
        }

        function handleStateChange() {
            if (_xmlHttp.readyState == 4) {
                if (_xmlHttp.status >= 400) {
                    alert("发生错误：\r\n" + checkXmlHttpError(_xmlHttp));
                }
                else {
                    var message = getInformationFromResponse(_xmlHttp, "uploadInfo");
                    if (message != "") {
                        alert(message);
                    }
                }
            }            
        }

        function getInformationFromResponse(xmlHttp, headerName) {
            var headerInformation = xmlHttp.getResponseHeader(headerName);
            var message = "";

            if (headerInformation) {
                headerInformation = unescape(decodeURI(headerInformation));
                message = headerInformation;
            }
            return message;
        }

// ]]>
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <SOA:ComponentHelperWrapper ID="ComponentHelper1" runat="server"></SOA:ComponentHelperWrapper>
    
    </div>
    </form>
    <p>
        <input id="fileInput" type="file" /> <input id="btnUpload" type="button" 
            value="上传" onclick="return btnUpload_onclick()" /></p>
</body>
</html>
