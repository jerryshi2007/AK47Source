<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AUCenter.Default" %>

<%@ Register Src="inc/Banner.ascx" TagName="Banner" TagPrefix="au" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>欢迎使用管理单元</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link rel="icon" href="favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="favicon.ico" type="image/x-icon" />
    <link href="styles/home.css" rel="stylesheet" type="text/css" />
</head>
<body class="au-full" style="overflow:auto">
    <form id="serverForm" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        <Services>
            <asp:ServiceReference Path="~/Services/CommonServices.asmx" />
        </Services>
    </asp:ScriptManager>
    <div class="pc-frame-header">
        <au:Banner ID="pcBanner" runat="server" ActiveMenuIndex="1" />
    </div>
    <div class="pc-frame-container">
        <%--<div class="pc-search-box-wrapper">
			<soa:DeluxeSearch ID="DeluxeSearch" runat="server" CssClass="deluxe-search deluxe-left"
				HasCategory="False" HasAdvanced="True" CustomSearchContainerControlID="advSearchPanel"
				OnConditionClick="onconditionClick" SearchFieldTemplate="CONTAINS(${DataField}$, ${Data}$)"
				SearchField="SearchContent" OnSearching="SearchButtonClick">
			</soa:DeluxeSearch>
			<soa:DataBindingControl runat="server" ID="searchBinding" AllowClientCollectData="True">
				<ItemBindings>
					<soa:DataBindingItem ControlID="sfCodeName" DataPropertyName="CodeName" />
					<soa:DataBindingItem ControlID="sfDisabled" DataPropertyName="AccountDisabled" ControlPropertyName="Checked"
						ClientIsHtmlElement="true" ClientPropName="checked" />
					<soa:DataBindingItem ControlID="sfSchemaType" DataPropertyName="SchemaType" />
				</ItemBindings>
			</soa:DataBindingControl>
			<div id="advSearchPanel" runat="server" style="display: none" class="pc-search-advpan">
				<asp:HiddenField runat="server" ID="sfAdvanced" Value="False" />
				<table border="0" cellpadding="0" cellspacing="0" class="pc-search-grid-duo">
					<tr>
						<td>
							<label for="sfCodeName" class="pc-label">
								代码名称</label><asp:TextBox runat="server" ID="sfCodeName" MaxLength="56" CssClass="pc-textbox" />(精确)
						</td>
						<td>
							<label for="sfDisabled" class="pc-label">
								已禁用</label><asp:CheckBox Text="" runat="server" ID="sfDisabled" />
						</td>
					</tr>
					<tr>
						<td>
							<label for="sdSchemaType" class="pc-label">
								类型</label><soa:HBDropDownList ID="sfSchemaType" runat="server" AppendDataBoundItems="True"
									DataSourceID="schemaTypeDataSource" SelectedText="(未指定)" DataMember="allTypes"
									DataTextField="Description" DataValueField="Name" CssClass="pc-textbox">
									<asp:ListItem Text="(未指定)" Value="" Selected="True"></asp:ListItem>
								</soa:HBDropDownList>
							<pc:SchemaDataSource runat="server" ID="schemaTypeDataSource" />
						</td>
						<td>
						</td>
					</tr>
				</table>
			</div>
		</div>--%>
        <div class="pc-container5">
            <asp:HiddenField ID="searchPerformed" runat="server" />
            <div class="pc-clear pc-center clearfix">
                <ul class="pc-metro-blocks">
                    <asp:Repeater runat="server" ID="categoryList" DataSourceID="ObjectDataSource1">
                        <ItemTemplate>
                            <li class="pc-metro-block-member pc-metro-orange" data-url='<%# this.ResolveClientUrl(Eval("ID","~/Explorer.aspx?id={0}")) %>'
                                onclick="metroClick(this);">
                                <div class="pc-metro-block-content">
                                    <%# Server.HtmlEncode ((string)Eval("Name")) %>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="GetCategories" TypeName="MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources.CategoryDataSource">
                    </asp:ObjectDataSource>
                    <li class="pc-metro-block-log pc-dw clearfix" data-url="LogList.aspx" onclick="metroClick(this);">
                        <div style="width: 64px; float: left">
                            <div class="pc-metro-icon">
                            </div>
                            <div class="pc-metro-block-content">
                                <div class="pc-metro-title">
                                    日志
                                </div>
                            </div>
                        </div>
                        <div style="padding-left: 0; text-align: left; overflow: hidden; height: 100%;">
                            <div class="pc-metro-logs-container">
                                <asp:Repeater runat="server" ID="logItems">
                                    <HeaderTemplate>
                                        <div class="pc-metro-logs" id="clientLogList">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="pc-metro-log even">
                                            <div class="pc-metro-log-title">
                                                <%#Eval("Subject")%></div>
                                            <div class="pc-metro-log-at">
                                                <%#Eval("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}")%></div>
                                        </div>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <div class="pc-metro-log">
                                            <div class="pc-metro-log-title">
                                                <%#Eval("Subject")%></div>
                                            <div class="pc-metro-log-at">
                                                <%#Eval("CreateTime", "{0:yyyy-MM-dd HH:mm:ss}")%></div>
                                        </div>
                                    </AlternatingItemTemplate>
                                    <FooterTemplate>
                                        </div>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        $pc.ui.listMenuBehavior("listMenu");
        function onconditionClick(sender, e) {
            var content = Sys.Serialization.JavaScriptSerializer.deserialize(e.ConditionContent);
            var bindingControl = $find("searchBinding");
            bindingControl.dataBind(content);
        }

        function metroClick(elem) {
            var url = $pc.getAttr(elem, "data-url");
            window.location.replace($pc.appRoot + url);
        }

        Sys.Application.add_load(function () {

        });

        (function () {
            var logList = document.getElementById("clientLogList");
            if (logList) {
                var tall = logList.clientHeight;
                var itemsCount = 0;
                for (var node = logList.firstChild; node; node = node.nextSibling) {
                    if (node.nodeType === 1 && $pc.hasClass(node, "pc-metro-log")) {
                        itemsCount++;
                    }
                }

                var marginTop = 0;
                var i = 0;
                if (itemsCount) {
                    window.setInterval(function () {
                        var startTop = tall * i;
                        i = (i + 1) % itemsCount;
                        finishTop = tall * i;
                        var delta = finishTop - startTop;

                        $pc.animation.circEaseOut(1000, function (p) {
                            logList.style.marginTop = (-(startTop + p * delta)) + "px";
                        }, function () {
                            logList.style.marginTop = (-finishTop) + "px";
                        });
                    }, 5000);
                }
            }
        })();
    </script>
</body>
</html>
