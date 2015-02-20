<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ObjectHistoryTimeline.aspx.cs"
	Inherits="PermissionCenter.ObjectHistoryTimeline" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head id="Head1" runat="server">
	<title>对象变更日志</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<base target="_self" />
	<link href="../scripts/timeline_2.3.0/timeline_js/timeline-bundle.css" rel="stylesheet"
		type="text/css" />
	<script type="text/javascript">
		Timeline_ajax_url = "/MCSWebApp/PermissionCenter/scripts/timeline_2.3.0/timeline_ajax/simile-ajax-api.js";
		Timeline_urlPrefix = '/MCSWebApp/PermissionCenter/scripts/timeline_2.3.0/timeline_js/';
		Timeline_parameters = 'bundle=true';
	</script>
	<script src="../scripts/timeline_2.3.0/timeline_js/timeline-api.js" type="text/javascript"></script>
</head>
<body onload="onLoad();" onresize="onResize();">
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" />
	<div class="pc-banner">
		<h1>
			检索变更历史 (对象类型<label id="schemaTypeLabel" runat="server"></label>)</h1>
	</div>
	<div class="pc-tabs-header">
		<ul>
			<li>
				<asp:HyperLink ID="lnkList" runat="server">列表</asp:HyperLink></li>
			<li class="pc-active">
				<asp:HyperLink ID="lnkTimeline" runat="server">时间轴</asp:HyperLink></li>
		</ul>
	</div>
	<div class="pc-container5">
		<div class="pc-center">
			<a href="javascript:void(0);" id="preEvent">上一个事件</a> <span id="curTimeDisp" style="padding: 2px 5px;">
			</span><a href="javascript:void(0);" id="nextEvent">下一个事件</a>
		</div>
		<div class="" id="timeline" style="height: 450px; border: 1px solid #aaa">
		</div>
	</div>
	<div style="display: none">
		<input type="hidden" id="objId" runat="server" />
	</div>
	<pc:Footer ID="footer" runat="server" />
	</form>
	<script type="text/javascript">
		var tl;
		$pc.ui.traceWindowWidth();
		var curDate = null;

		Timeline.GregorianDateLabeller.prototype.labelPrecise = function (date) {
			return date.localeFormat(""); //替换日期格式化方式
		}

		function onLoad() {
			var now = new Date().toGMTString();
			var url = $pc.appRoot + "Handlers/TimelineData.ashx?id=" + encodeURIComponent(document.getElementById("objId").value);

			var eventSource = new Timeline.DefaultEventSource();

			var bandInfos = [

     Timeline.createBandInfo({
     	eventSource: eventSource,
     	date: now,
     	width: "70%",
     	intervalUnit: Timeline.DateTime.DAY,
     	intervalPixels: 100
     }),
     Timeline.createBandInfo({
     	eventSource: eventSource,
     	date: now,
     	width: "30%",
     	intervalUnit: Timeline.DateTime.MONTH,
     	intervalPixels: 200
     })
   ];
			bandInfos[1].syncWith = 0;
			bandInfos[1].highlight = true;
			tl = Timeline.create(document.getElementById("timeline"), bandInfos);

			Timeline.loadXML(url, function (xml, url) {
				eventSource.loadXML(xml, url);
			});

			function shiftTimeLine(date) {
				//tl.getBand(0).setCenterVisibleDate(Timeline.DateTime.parseGregorianDateTime(date));
				tl.getBand(0).setCenterVisibleDate(date);
				$pc.setText("curTimeDisp", date.localeFormat(""));
			}

			function shiftLeft() {
				cdd();
				var d1, d2 = eventSource.getEarliestDate(); ;
				var it = eventSource.getAllEventIterator();
				while (it.hasNext()) {
					d1 = it.next().getStart();
					if (d1 < curDate && d1 > d2) {
						d2 = d1;
					}
				}
				curDate = d2;
				shiftTimeLine(d2);
				$pc.console.info(d2);
			}

			function shiftRight() {
				cdd();
				var d1, d2 = eventSource.getLatestDate();
				var it = eventSource.getAllEventIterator();
				while (it.hasNext()) {
					d1 = it.next().getStart();
					if (d1 > curDate && d1 < d2) {
						d2 = d1;
					}
				}
				curDate = d2;
				shiftTimeLine(d2);
				$pc.console.info(d2);
			}

			function cdd() {
				if (!curDate) {
					curDate = eventSource.getLatestDate();
				}
			}

			$pc.bindEvent(document.getElementById("nextEvent"), "click", function () {
				shiftRight();
			});

			$pc.bindEvent(document.getElementById("preEvent"), "click", function () {
				shiftLeft();
			});

			curDate = eventSource.getEarliestDate() || new Date();

			shiftTimeLine(curDate);

		}

		var resizeTimerID = null;
		function onResize() {
			if (resizeTimerID == null) {
				resizeTimerID = window.setTimeout(function () {
					resizeTimerID = null;
					tl.layout();
				}, 500);
			}
		}

	</script>
</body>
</html>
