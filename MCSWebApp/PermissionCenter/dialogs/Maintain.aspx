<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Maintain.aspx.cs" Inherits="PermissionCenter.Dialogs.Maintain"
	MasterPageFile="~/dialogs/MaintainMaster.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
	<ul class="pc-tabs-header3">
		<li class="pc-active"><a href="#">常 规 </a></li>
		<li><a href="ADSync.aspx">AD 同 步</a></li><li><a href="ADReverseSync.aspx">AD 反 向 同 步</a></li>
	</ul>
	<div class="pc-tabs-content3">
		<div class="pc-tabs-content pc-active" style="clear: none">
			<fieldset>
				<legend>快照数据</legend>
				<div>
					<div style="display: none">
						<asp:Button ID="btnGenSchemaTable" runat="server" OnClick="GenSchemaTable" /><asp:Button
							ID="btnGenSnap" runat="server" OnClick="GenSnapshot" />
					</div>
					<button type="button" class="pc-button" id="btnGenSchemaTrigger" title="当更改了配置文件后，请使用此功能重新生成Schema表。">
						生成Schema表
					</button>
					<button type="button" id="btnGen" class="pc-button" title="当查询数据与实际情况不一致时，使用此按钮重新生成快照表。生成失败时，可能影响用户登录，请联系DBA。">
						重新生成快照表<span id="iconGen" class="" style="vertical-align: middle">&nbsp;</span>
					</button>
					<span id="prompt1" class="pc-prompt"></span>
				</div>
				<div>
				</div>
			</fieldset>
			<fieldset>
				<legend>人员选择控件测试</legend>
				<soa:OuUserInputControl runat="server" ID="ou1" />
			</fieldset>
			<div runat="server" id="testPan" visible="false">
				<fieldset class="pc-profile-group">
					<legend>安全中心历史导入</legend>
					<div>
						<asp:Button CssClass="pc-button" Text="清除所有数据" runat="server" OnClick="ClearDataClick"
							ID="bt1" />
					</div>
					<div>
						<asp:Button CssClass="pc-button" Text="生成N个根组织" runat="server" OnClick="GenRoots"
							ID="bt2" />
						<br />
						<asp:Button ID="btnGenInitData" CssClass="pc-button" runat="server" OnClick="btnGenInitData_Click"
							Text="初始应用角色" />
					</div>
				</fieldset>
				<fieldset class="pc-profile-group">
					<legend>设置用户口令</legend>
					<label for="userCodeName">
						用户名</label>
					<asp:TextBox runat="server" ID="userCodeName" />
					<asp:Button Text="设置缺省口令" runat="server" CssClass="pc-button" OnClick="ResetPass"
						ID="bt3" />
					<span runat="server" style="color: #ff0000" id="prompt"></span>
				</fieldset>
				<fieldset class="pc-profile-group">
					<legend>冲洗</legend>
					<label for="timeToDeleteAfter">
						删除此时间之后的记录并恢复之前的记录</label><asp:TextBox runat="server" ID="timeToDeleteAfter" />
					<asp:Button Text="执行" runat="server" CssClass="pc-button" OnClick="DeleteAfterClick"
						ID="bt4" /><span runat="server" style="color: #ff0000" id="prompt2"></span>
				</fieldset>
			</div>
			<div class="pc-profile-groups">
				<asp:Panel runat="server" ID="factoryPan" Visible="false">
					<fieldset class="pc-profile-group">
						<legend>关系检测</legend>
						<div>
							<asp:Button ID="btnDetectConflict" runat="server" OnClick="DetectConflict" CssClass="pc-button"
								Text="检测" />
						</div>
						<div id="detectResult" runat="server" visible="false">
							<h1>
								检测结果</h1>
							<p>
								此处如果显示任何条目，表示数据有误。请联系DBA。
							</p>
							<div>
								<h3>
									父子关系中子对象不存在
								</h3>
								<asp:DataList runat="server" ID="viewList1">
									<HeaderTemplate>
										------------开始------------
									</HeaderTemplate>
									<ItemTemplate>
										<span class="pc-label">子对象ID</span>&nbsp;<span><%#Eval("ObjectID") %></span><span
											class="pc-label">父对象ID</span>&nbsp;<span><%#Eval("ParentID") %></span><span class="pc-label">子对象Schema</span>&nbsp;<span><%#Eval("ChildSchemaType") %></span><span
												class="pc-label">父对象Schema</span>&nbsp;<span><%#Eval("ParentSchemaType") %></span><span
													class="pc-label">全路径</span>&nbsp;<span><%#Eval("FullPath") %></span>
									</ItemTemplate>
									<FooterTemplate>
										------------结束------------
									</FooterTemplate>
								</asp:DataList>
							</div>
							<div>
								<h3>
									父子关系中父对象不存在
								</h3>
								<asp:DataList ID="viewList2" runat="server">
									<HeaderTemplate>
										------------开始------------
									</HeaderTemplate>
									<ItemTemplate>
										<span class="pc-label">子对象ID</span>&nbsp;<span><%#Eval("ObjectID") %></span><span
											class="pc-label">父对象ID</span>&nbsp;<span><%#Eval("ParentID") %></span><span class="pc-label">子对象Schema</span>&nbsp;<span><%#Eval("ChildSchemaType") %></span><span
												class="pc-label">父对象Schema</span><span><%#Eval("ParentSchemaType") %></span><span
													class="pc-label">全路径</span>&nbsp;<span><%#Eval("FullPath") %></span>
									</ItemTemplate>
									<FooterTemplate>
										------------结束------------
									</FooterTemplate>
								</asp:DataList>
							</div>
							<div>
								<h3>
									成员关系中容器对象不存在
								</h3>
								<asp:DataList ID="viewList3" runat="server">
									<HeaderTemplate>
										------------开始------------
									</HeaderTemplate>
									<ItemTemplate>
										<span class="pc-label">子对象ID</span>&nbsp;<span><%#Eval("MemberID") %></span><span
											class="pc-label">父对象ID</span>&nbsp;<span><%#Eval("ContainerID") %></span><span class="pc-label">子对象Schema</span>&nbsp;<span><%#Eval("MemberSchemaType") %></span><span
												class="pc-label">父对象Schema</span><span><%#Eval("ContainerSchemaType") %></span><span
													class="pc-label">版本开始时间</span>&nbsp;<span><%#Eval("VersionStartTime") %></span>
									</ItemTemplate>
									<FooterTemplate>
										------------结束------------
									</FooterTemplate>
								</asp:DataList>
							</div>
							<div>
								<h3>
									成员关系中成员对象不存在
								</h3>
								<asp:DataList ID="viewList4" runat="server">
									<HeaderTemplate>
										------------开始------------
									</HeaderTemplate>
									<ItemTemplate>
										<span class="pc-label">子对象ID</span>&nbsp;<span><%#Eval("MemberID") %></span><span
											class="pc-label">父对象ID</span>&nbsp;<span><%#Eval("ContainerID") %></span><span class="pc-label">子对象Schema</span>&nbsp;<span><%#Eval("MemberSchemaType") %></span><span
												class="pc-label">父对象Schema</span><span><%#Eval("ContainerSchemaType") %></span><span
													class="pc-label">版本开始时间</span>&nbsp;<span><%#Eval("VersionStartTime") %></span>
									</ItemTemplate>
									<FooterTemplate>
										------------结束------------
									</FooterTemplate>
								</asp:DataList>
							</div>
							<div>
								<h3>
									一个人存在多个主职
								</h3>
								<asp:DataList ID="viewList5" runat="server">
									<HeaderTemplate>
										------------开始------------
									</HeaderTemplate>
									<ItemTemplate>
										<span class="pc-label">人员代码名称</span>&nbsp;<span><%# Server.HtmlEncode((string) Eval("CodeName")) %></span>
										<span class="pc-label">人员名称</span>&nbsp;<span><%# Server.HtmlEncode((string)Eval("Name")) %></span>
										<span class="pc-label">父对象ID</span>&nbsp;<span><%#Eval("Parent_ID") %></span><span
											class="pc-label">子对象ID</span>&nbsp;<span><%#Eval("Object_ID") %></span><span class="pc-label">是否缺省组织</span><span><%#Eval("IsDefault") %></span><span
												class="pc-label">全路径</span>&nbsp;<span><%# Server.HtmlEncode((string)Eval("FullPath")) %></span>
									</ItemTemplate>
									<FooterTemplate>
										------------结束------------
									</FooterTemplate>
								</asp:DataList>
							</div>
						</div>
					</fieldset>
					<fieldset class="pc-profile-group">
						<legend>整理无效的秘书</legend>
						<div>
							<asp:Button ID="Button1" Text="清理" runat="server" OnClick="CleanSecretary" CssClass="pc-button" />
						</div>
					</fieldset>
				</asp:Panel>
			</div>
		</div>
	</div>
	<script type="text/javascript">
		$pc.bindEvent("btnGen", "click", function () {

			function restore() {
				$pc.removeClass("iconGen", "pc-icon-loader");
				$pc.removeClass("btnGen", "disabled");
				$pc.get("btnGen").disabled = false;
			}

			if (confirm('这可能会暂时影响其他用户使用，确认要继续吗？\r\n此操作可能需要较长时间，如果出现错误，请联系DBA。')) {
				$pc.addClass("iconGen", "pc-icon-loader");
				$pc.get("btnGen").disabled = true;
				$pc.addClass("btnGen", "disabled");
				$pc.setText("prompt1", "请稍候。生成快照期间，无法做其他操作。请耐心等待操作完成，期间请勿关闭此窗口，直到操作结束。");
				var img = new Image();
				img.onload = function () {
					restore();
					$pc.setText("prompt1", "生成快照结束");
				}

				img.onerror = function (e) {
					restore();
					$pc.setText("prompt1", "生成快照遇到错误。这种情况下，请联系DBA在数据库中执行生成快照，否则所有用户可能无法正常使用。");
				}

				img.src = "Maintain.aspx?action=genSnapshot&magic=" + new Date();
			}
		});

		$pc.bindEvent("btnGenSchemaTrigger", "click", function () {
			if (confirm("确定要重新生成吗？")) {
				document.getElementById('<%=btnGenSchemaTable.ClientID %>').click();
			}
		});
	
	</script>
</asp:Content>
