<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadFileToMossTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.UploadFileToMossTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dl>
            <dt>多附件上传</dt>
            <dd>
                <SOA:MaterialControl ID="MaterialControl1" MaterialUseMode="UploadFile" RootPathName="GenericProcess"
                    runat="server" />
            </dd>
        </dl>
        <dl>
            <dt>传统单附件上传</dt>
            <dd>
                <SOA:MaterialControl ID="MaterialControl2" MaterialUseMode="UploadFile" FileSelectMode="TraditionalSingle"
                    RootPathName="GenericProcess" runat="server" />
            </dd>
        </dl>
        <dl>
            <dt>在线编辑</dt>
            <dd>
                <SOA:MaterialControl ID="MaterialControl3" MaterialUseMode="SingleDraft" AutoOpenDocument="False"
                    EditDocumentInCurrentPage="False" TemplateUrl="~/MaterialControl/Templates/Test.xlsx"
                    RootPathName="GenericProcess" runat="server" />
            </dd>
        </dl>
    </div>
    <div>
        <asp:Button runat="server" ID="submit" OnClick="submit_OnClick" Text="Submit" />
    </div>
    </form>
</body>
</html>
