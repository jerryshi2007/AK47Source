<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeluxeMenuTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DeluxeMenu.DeluxeMenuTest" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>菜单测试</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../css/form.css" rel="stylesheet" type="text/css" />
    <style>
        .box
        {
            margin-bottom: 20px;
            position: relative;
        }
        
        .blue-border
        {
            border-color: #00acec !important;
        }
        
        .box .box-header
        {
            font-size: 21px;
            font-weight: 200;
            line-height: 30px;
            padding: 10px 15px;
        }
        
        .box .box-header.red-background
        {
            color: white;
            background-color: #f34541 !important;
        }
        .box .box-content
        {
            padding: 10px;
            border: 1px solid #dddddd;
            background: white;
            display: block;
            -webkit-box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
            -moz-box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
        }
        
        hr
        {
            position: relative;
            margin-left: -15px;
            margin-right: -15px;
            border-top: 1px solid #eeeeee;
            border-bottom: 1px solid white;
        }
        hr.hr-normal
        {
            margin-left: 0;
            margin-right: 0;
        }
        
        .input-group-addon
        {
            cursor: pointer;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="box bordered-box blue-border box-nomargin">
        <div class="box-header red-background">
            <div class="title">
                DeluxeMenu
            </div>
        </div>
        <div class="box-content">
            <section>
                <p>
                    <strong>DeluxeMenu</strong>
                </p>
                <div>
                    <p>
                        使用此控件时，需提供一个容器元素和一个容器中的下拉按钮（否则此菜单将无法隐藏）。 将容器内任何一个元素指定data-toggle="dropdown"属性，即可自动作为下拉按钮。
                    </p>
                    <p>
                        容器元素可以是任何非行内元素，但必须是relative定位（可通过使用dropdown的class来实现）。
                    </p>
                    <p>
                        如果要让此菜单总是可见，请设置父元素的class含有“open”，并且不提供任何下拉按钮。
                    </p>
                </div>
                <hr class="hr-normal" />
                <div class="dropdown">
                    <button data-toggle="dropdown" type="button" class="btn btn-default">
                        bootstrip下拉
                    </button>
                    <res:DeluxeMenuStrip runat="server" ID="menuMainStrip" OnClientItemClick="clientItemClick">
                        <Items>
                            <res:DeluxeMenuStripItem Text="abc" href="#" />
                            <res:DeluxeMenuStripItem Text="def" Enabled="false" />
                        </Items>
                    </res:DeluxeMenuStrip>
                </div>
                <div>
                    <code>&lt;div class="dropdown"&gt;<br />
                        &nbsp;&nbsp;&lt;button data-toggle="dropdown" type="button" class="btn btn-default"&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;bootstrip默认的下拉<br />
                        &nbsp;&nbsp;&lt;/button&gt;<br />
                        &nbsp;&nbsp;&lt;res:DeluxeMenuStrip runat="server" ID="DeluxeMenuStrip1"&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&lt;Items&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;res:DeluxeMenuStripItem Text="abc" /&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;res:DeluxeMenuStripItem Text="def" /&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&lt;/Items&gt;<br />
                        &nbsp;&nbsp;&lt;/res:DeluxeMenuStrip&gt;<br />
                        &lt;/div&gt;<br />
                    </code>
                    <hr class="hr-normal" />
                    <p>
                        总是可见的菜单，设置父元素class含有“open”</p>
                    <p>
                        设置Static为true，则此菜单将出现在布局流中
                    </p>
                    <div class="dropdown open clearfix">
                        <res:DeluxeMenuStrip runat="server" ID="DeluxeMenuStrip1" Static="true">
                            <Items>
                                <res:DeluxeMenuStripItem Text="abc" Value="" href="http://www.google.com" />
                                <res:DeluxeMenuStripItem Text="def" Value="" />
                            </Items>
                        </res:DeluxeMenuStrip>
                    </div>
                    <code>&lt;div class="dropdown <strong>open</strong> clearfix"&gt;<br />
                        &nbsp;&nbsp;&lt;res:DeluxeMenuStrip runat="server" Static="true"&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&lt;Items&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;res:DeluxeMenuStripItem Text="abc" /&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;res:DeluxeMenuStripItem Text="def" /&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&lt;/Items&gt;<br />
                        &nbsp;&nbsp;&lt;/res:DeluxeMenuStrip&gt;<br />
                        &lt;/div&gt;<br />
                    </code>
                    <hr class="hr-normal" />
                    <p>
                        靠右对齐的菜单</p>
                    <div class="dropdown open clearfix">
                        <res:DeluxeMenuStrip runat="server" ID="DeluxeMenuStrip2" Static="true" Align="Right">
                            <Items>
                                <res:DeluxeMenuStripItem Text="abc" href="http://www.google.com" />
                                <res:DeluxeMenuStripItem Text="def" />
                            </Items>
                        </res:DeluxeMenuStrip>
                    </div>
                    <code>&lt;div class="dropdown open clearfix"&gt;<br />
                        &nbsp;&nbsp;&lt;res:DeluxeMenuStrip runat="server" ID="DeluxeMenuStrip1" Static="true"
                        Align="Right"&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&lt;Items&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;res:DeluxeMenuStripItem Text="abc" /&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;res:DeluxeMenuStripItem Text="def" /&gt;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&lt;/Items&gt;<br />
                        &nbsp;&nbsp;&lt;/res:DeluxeMenuStrip&gt;<br />
                        &lt;/div&gt;<br />
                    </code>
                </div>
            </section>
            <section>
                <hr class="hr-normal" />
                <p>
                    <strong>DeluxeMenuStrip</strong>
                </p>
                <div>
                    <div class="dropdown open clearfix">
                        <asp:Button ID="Button1" Text="服务器端加一项" runat="server" OnClick="AddSome" CssClass="btn btn-default" />
                        <res:DeluxeMenuStrip runat="server" ID="menu1" Static="true">
                            <Items>
                                <res:DeluxeMenuStripItem Text="aaa" data-target="http://www.google.com" href="http://www.google.com"
                                    role="menuitem" />
                                <res:DeluxeMenuStripItem Enabled="true" Text="-" Value="haha" />
                            </Items>
                        </res:DeluxeMenuStrip>
                    </div>
                </div>
            </section>
            <section>
                <hr class="hr-normal" />
                <div>
                    <p>
                        通过代码切换菜单状态，注意它会修改上层元素的位置。 如果因为z-index的问题导致遮挡，请尝试修改父元素的z-index样式。 注意由于可能受到单击事件影响，请在点击元素之后延时执行此方法，否则弹出的菜单可能立即收回。
                    </p>
                    <div>
                        <button type="button" id="butttonFollowMouse" class="btn btn-default">
                            设置在鼠标位置打开
                        </button>
                        <button type="button" id="buttonDefaultPosition" class="btn btn-default">
                            在默认位置打开
                        </button>
                    </div>
                    <div>
                    </div>
                    <div>
                        <div class="dropdown">
                            <div data-toggle="dropdown">
                            </div>
                            <res:DeluxeMenuStrip runat="server" ID="menuFree">
                                <Items>
                                    <res:DeluxeMenuStripItem Text="abc" Value="" />
                                    <res:DeluxeMenuStripItem Text="def" Value="" />
                                </Items>
                            </res:DeluxeMenuStrip>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#butttonFollowMouse").click(function (e) {
                window.setTimeout(function () {
                    var menu = $find("menuFree");
                    menu.setPosition(e.clientX, e.clientY, true);
                    menu.toggle();
                }, 1);
            });

            $("#buttonDefaultPosition").click(function (e) {
                window.setTimeout(function () {
                    var menu = $find("menuFree");
                    menu.toggle();
                    menu.resetPosition();
                }, 1);
            });
        });

        function clientItemClick(s, e) {
            alert("单击了元素" + e.target.innerHTML + " ，值为:" + e.target.dataset.value);
        }
    </script>
</body>
</html>
