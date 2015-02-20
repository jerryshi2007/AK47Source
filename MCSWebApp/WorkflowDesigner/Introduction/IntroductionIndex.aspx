<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntroductionIndex.aspx.cs"
	Inherits="WorkflowDesigner.Introduction.IntroductionIndex" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="StyleSheet1.css" rel="Stylesheet" type="text/css" />
	<style>
		.cat-content
		{
			padding: 5px 0px 5px 15px;
			line-height: 26px;
			overflow: hidden;
			border-top-color: rgb(234, 234, 234);
			border-top-width: 1px;
			border-top-style: solid;
		}
		.cat-content li
		{
			background-position: 5px -280px;
			padding-left: 15px;
		}
	</style>
</head>
<body>
	<div class="mainbody">
		<h3>
			流程设计器功能介绍</h3>
		<div class="cat-content">
			<ul>
				<li><a href="流程导入-导出.htm">流程模板导入/导出介绍</a></li>
				<li><a href="主流程和分支流程（子流程）.htm">分支流程（子流程）介绍</a></li>
			</ul>
		</div>
	</div>
</body>
</html>
