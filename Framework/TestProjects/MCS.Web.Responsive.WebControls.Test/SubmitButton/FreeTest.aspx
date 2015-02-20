<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FreeTest.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test.SubmitButton.FreeTest" %>

<%@ Register TagPrefix="mcsr" Namespace="MCS.Web.Responsive.WebControls" Assembly="MCS.Web.Responsive.WebControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" target="innerFrame">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <mcsr:SubmitButton runat="server" ID="submitWithProcess" Text="带进度的提交" PopupCaption="正在提交..."
            OnClick="submitWithProcess_Click" ProgressMode="BySteps" />
    </div>
    <div>
        超长的内容</div>
    <h1 class="title">
        Back navigation caching</h1>
    <div id="mainSection">
        <div class="clsServerSDKContent">
        </div>
        <p>
            Use back navigation caching in Internet Explorer&nbsp;11 to help users of your webpage
            return to previous pages more quickly.
        </p>
        <p>
            By default, pages are unloaded from memory when you navigate away from them. Beginning
            with IE11, webpages that meet specific conditions are cached when the user navigates
            away. If the user later returns to the page, it&#39;s restored from memory, rather
            than reloaded or reconstructed.</p>
        <p>
            In order to be cached, webpages must meet these conditions:</p>
        <p>
        </p>
        <ul>
            <li>Served using the HTTP: protocol (HTTPS pages are not cached for security reasons)</li>
            <li>Page has no <a href="http://msdn.microsoft.com/en-US/library/ie/ms536907(v=vs.85).aspx">
                <strong xmlns="http://www.w3.org/1999/xhtml">beforeunload</strong></a> event handlers
                defined </li>
            <li>All <a href="http://msdn.microsoft.com/en-US/library/ie/cc197055(v=vs.85).aspx">
                <strong xmlns="http://www.w3.org/1999/xhtml">load</strong></a> and <a href="http://msdn.microsoft.com/en-US/library/ie/dn255071(v=vs.85).aspx">
                    <strong xmlns="http://www.w3.org/1999/xhtml">pageshow</strong></a> events have
                completed</li>
            <li>The page doesn&#39;t contain any of the following:
                <ul>
                    <li>Pending <a href="http://msdn.microsoft.com/en-US/library/ie/hh772512(v=vs.85).aspx">
                        <strong xmlns="http://www.w3.org/1999/xhtml">indexedDB</strong></a> transactions</li>
                    <li>Open or active web socket connections</li>
                    <li>Running web workers</li>
                    <li>Microsoft ActiveX controls.</li>
                </ul>
            </li>
            <li>The F12 Developer tools window isn&#39;t open</li>
        </ul>
        <p>
            IE11 also supports <a href="http://msdn.microsoft.com/en-US/library/ie/dn255070(v=vs.85).aspx">
                <strong xmlns="http://www.w3.org/1999/xhtml">pagehide</strong></a> and <a href="http://msdn.microsoft.com/en-US/library/ie/dn255071(v=vs.85).aspx">
                    <strong xmlns="http://www.w3.org/1999/xhtml">pageshow</strong></a> events
            so you can meet these conditions more easily.</p>
        <p>
            When webpages are cached upon navigation, users are able to return to them more
            quickly. In addition, pages are returned to their previous states when restored
            from the cache.</p>
        <p>
            Review your website to identify pages you want cached before navigation and verify
            they meet these conditions.
        </p>
        <p>
            Use the <a href="http://msdn.microsoft.com/en-US/library/ie/dn255070(v=vs.85).aspx">
                <strong xmlns="http://www.w3.org/1999/xhtml">pagehide</strong></a> and <a href="http://msdn.microsoft.com/en-US/library/ie/dn255071(v=vs.85).aspx">
                    <strong xmlns="http://www.w3.org/1999/xhtml">pageshow</strong></a> events
            to manage the features that prevent your pages from being cached.
        </p>
        <h2>
            <a id="API_reference"></a><a id="api_reference0"></a><a id="API_REFERENCE1"></a>
            API reference</h2>
        <dl>
            <dd>
                <a href="http://msdn.microsoft.com/en-US/library/ie/dn255070(v=vs.85).aspx"><strong
                    xmlns="http://www.w3.org/1999/xhtml">pagehide</strong></a>
            </dd>
            <dd>
                <a href="http://msdn.microsoft.com/en-US/library/ie/dn255071(v=vs.85).aspx"><strong
                    xmlns="http://www.w3.org/1999/xhtml">pageshow</strong></a>
            </dd>
        </dl>
        <h2>
            <a id="Specification"></a><a id="specification0"></a><a id="SPECIFICATION1"></a>
            Specification</h2>
        <dl>
            <dd>
                <a href="http://go.microsoft.com/fwlink/p/?LinkID=203771">HTML5</a>, Section 5.6.10.1</dd>
        </dl>
        <p>
            &nbsp;</p>
    </div>
    <p>
    </p>
    <div>
        <iframe id="innerFrame" name="innerFrame"></iframe>
    </div>
    </form>
</body>
</html>
