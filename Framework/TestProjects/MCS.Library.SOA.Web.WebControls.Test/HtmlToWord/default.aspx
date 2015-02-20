<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.HtmlToWord._default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Once upon a time in the west</title>
	<script type="text/javascript">
		function onExportClick() {
			var url = document.getElementById("exportUrl").value;

			window.open(url);
		}
	</script>
</head>
<body style="margin-top: 8px">
	<form id="serverForm" runat="server">
	<div>
		<h2>
			Once upon a time in the west</h2>
		<div>
			<input type="button" id="exportBtn" runat="server" value="Export to word..." onclick="onExportClick();" />
			<input type="hidden" id="exportUrl" runat="server" />
		</div>
		<div>
			<img src="./images/album.jpg" alt="Once upon a time in the west" />
			<p>
				导演：瑟吉欧·莱昂 Sergio Leone</p>
			<p>
				主演：亨利·方达 Henry Fonda，查尔斯·布朗森 Charles Bronson，歌迪亚·卡汀娜 Claudia Cardinale</p>
			<p>
				作曲：艾里奥·莫里康 Ennio Morricone</p>
			<img src="./images/academyAward.jpg" alt="Once upon a time in the west" />
		</div>
		<div>
			<p>
				意大利西部片宗师瑟吉欧·莱昂的传世经典之作，继与克林伊斯威特合作「镖客三系列」之后，瑟吉欧·莱昂这部近三个小时的长篇钜作，被称之为影史上最伟大的西部片。讲的是西部片的老主题：以美国立国早期修筑一条西部铁路为背景而展开的争夺，杀戮和复仇往事，引出了一个个牛仔，枪手，镖客，阴谋家......
			</p>
			<p>
				对剧情我并不太感兴趣，我的目的是想弄明白这部曲子倒底在描写什么，诉说什么，我反反复观复看这部片子，透过那些枪战，拼斗的表象，慢慢地，我似乎渐渐理解了它的深刻主题．片中女主人公从美国东南海岸城市新奥尔良长途跋涉来到西部的一个小镇，准备去往一个陌生的荘园完婚并开始一种新生活，她一下火车就遇到无人接站的尴尬（影片开始已经交代她的未婚丈夫一家已遭杀戮，但她还未知情），她带着疑惑的心情问路并登上一辆四轮马车急急奔向未卜的目的．这段配乐从她一下火车就缓缓开始，并逐渐显露出如梦如诗般的人声伴唱.在她问路以后开始出发时，随着摄影镜头急速从地面抬升到屋顶而出现一个高潮，管弦乐队以全声响演奏出高昂激励的旋律，展示出女主人公对未来生活的无限憧憬和企盼，使观众如身临其境并产生强烈的共鸣，接着出现一段马车疾驶经过沸腾的建筑工地的镜头，工人们正在热火朝天的架设铁路，配合着生气勃勃的建设场景，由圆号引出的旋律唤起人们对社会发展的美好期望，随着四轮马车向着亚利桑那大峡谷的深处渐渐远去，音乐也由强转弱，给人们留下新的悬念。
			</p>
			<p>
				音乐是一种艺术语言，每个人都可以从同一首乐曲中展开自己的想像，得出自己的领悟．我不敢说自己的体会是准确的，但我可以肯定的说，每当我听到这首乐曲时（而且和莫里康其它的许多作品一样，这首乐曲也为许多乐队重新编缉并以各种不同的乐器，配音所演奏,从而有很多不同的版本.），我都会被唤起一种复杂的情感，有时会静静地聆听那轻似耳语般的诉说，有时会得到一种像一轮红日喷薄而起的壮丽感受．无论是旧情，柔情，热情，激情....总是会给自己带来一份思绪,一份憧憬，一份力量．有时，音乐已经结束，可你却还久久不肯挪动身驱，不愿意离开那音乐所带你投入的精神时空．这也许就是莫里康音乐的魅力所在吧.我常常把这首乐曲介绍给我的一些朋友，现在,利用这个机会介绍给你,我也希望你去欣赏它，喜欢它．也许你会得到更多的品味和感受。
			</p>
			<p>
				在使得莱昂内莫里康内的代表作《西部往事》成为不朽名作的诸多因素中，音乐所起的作用再大不过了。“往事主题”具有的穿透力和强大感染力，为一般影片配乐不能相提并论。那辽阔宽广、舒缓动人、含义深刻的旋律是无法用语言来形容的....更难忘那牵动着人类共有的怀旧情结，仿佛飘过了无限时空的女高音无言哼唱....“住事主题”是莫里康内的代表作，又是世界电影音乐的颠峰之作。在用各种文字书写的有关这位作曲家的介绍或评论文章中，它是人们必然要涉及的一段音乐，同时又是上至影评家、下到普通观众以及无数音乐爱好者永远怀着赞叹和敬畏之情去聆听、去谈论的一段音乐。其迷人的音色、流畅的旋律和隽永的风格，对世界各国的许多影片产生了影响。
			</p>
		</div>
	</div>
	</form>
</body>
</html>
