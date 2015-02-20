<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test._default" %>

<html>
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<title>测试入口</title>
	<link rel="stylesheet" href="css/project.css">
	<script src="/scripts/project.js"></script>
</head>
<body>
	<ul id="getText">
	</ul>
	<nav class="navbar-default navbar-fixed-top bs-docs-nav" role="navigation">
        <div class="navbar-header">
        		<button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
		  <span class="sr-only">Toggle navigation</span>
		  <span class="icon-bar"></span>
		  <span class="icon-bar"></span>
		  <span class="icon-bar"></span>
		</button>

            <a class="navbar-brand" href="#">测试工程</a>
        </div>
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
            <ul id="navbar-nav" class="nav navbar-nav">
            </ul>
        </div>
    </nav>
	<!-- 导航条结束 -->
	<div id="slider" class="jumbotron slider">
		<h2>
			组件一</h2>
		<p>
			无数可复用的组件，包括图标，下拉菜单，导航，警告框，弹出框等更多功能。</p>
	</div>
	<div id="sidebar" class="sidebar">
		<div class="list-group">
		</div>
	</div>
	<div class="main-content">
		<iframe id="main-content-inner" style="border: 1px #ccc solid; width: 100%; height: 500px;"
			height="500px" src="/DateTimePicker/DateTimePickerTest.aspx"></iframe>
	</div>
</body>
</html>
