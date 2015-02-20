<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartAdministrativeUnitProcess.aspx.cs"
	Inherits="WfFormTemplate.Forms.StartAdministrativeUnitProcess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>启动管理单元的测试流程</title>
	<link href="../css/css.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
		function onInitializeDataClick() {
			return window.confirm("初始化数据会清除已存在的管理单元的数据，请问需要继续吗？");
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div class="t-layout">
		<div class="t-caption">
			启动管理单元的测试流程
		</div>
		<div class="t-prompt">
			<p>
				说明：管理单元的测试流程，会自动创建一个管理单元的测试流程，无需流程定义。
			</p>
			<p>
				在启动流程之前，如果没有进行过数据初始化，请点击下面的“初始化”按钮。需要注意的是，数据初始化会清除所有管理单元的数据。
                如果独立进行测试，请在全局连接串配置中，修改连接AdminUnit和AdminUnitTest，映射到一个临时的库，以免对其他开发人员造成麻烦。
			</p>
		</div>
		<div class="t-button-panel">
			<asp:Button AccessKey="N" runat="server" ID="btnOK" Text="下一步(N)" CssClass="formButton"
				OnClick="btnOK_Click" />
			<asp:Button AccessKey="I" runat="server" ID="initializeDataBtn" Text="初始化(I)" CssClass="formButton"
				OnClientClick="return onInitializeDataClick();" onclick="initializeDataBtn_Click" />
			<input accesskey="C" type="button" class="formButton" value="关闭(C)" onclick="window.close();" />
		</div>
	</div>
	</form>
</body>
</html>
