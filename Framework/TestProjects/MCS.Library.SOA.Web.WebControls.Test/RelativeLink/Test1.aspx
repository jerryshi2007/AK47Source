<%@ Page Language="C#" AutoEventWireup="true" 
CodeBehind="Test1.aspx.cs"
 Inherits="MCS.Library.SOA.Web.WebControls.Test.RelativeLink.Test1" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
    #div1  
    {
    	color:red; 
    	 padding:0px; 
    	  height: 300px;
        padding-right:20em;
        overflow-x:hidden;
    	overflow-y: auto;        
         position:fixed;
    	margin: 0px;
    	 left: 0px;
        right: 0px;      
          }
</style>
    <script type="text/javascript">
        window.onload = function ()
        {
//            var div1 = $get( "div1" );
//            div1.appendChild( $get( "RelativeLink1" ) );
        };
        function Button1_onclick() {
            var div = document.createElement("div");
            div.style.height = "500px";
            document.getElementById("div1").appendChild(div);
        }

        function Button2_onclick() {

            alert(document.getElementById("div1").clientWidth);
            alert(document.getElementById("div2").clientWidth);
        }

    </script>
</head>
<body>
    
    <form id="form1" runat="server">
    <div id="div1" style="border-style:solid; border:4px;  margin-left:100px;"><input id="Button1" type="button" value="button" onclick="return Button1_onclick()" />
    <input id="Button2" type="button" value="button" onclick="return Button2_onclick()" />
    <div id="div2" style="width:100%;"></div>
        在雨中漫步<br>
        蓝色街灯渐露<br>
        相对望<br>
        无声紧拥抱着<br>
   <%--     为了找往日<br>
        寻温馨的往日<br>
        消失了<br>
        任雨湿我面<br>
        难分水点泪痕<br>
        心更乱<br>
        愁丝绕千百段<br>
        骤变的态度<br>
        无心伤她说话<br>
        收不了<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        在雨中漫步<br>
        尝水中的味道<br>
        仿似是<br>
        情此刻的尽时<br>
        未了解结合<br>
        留低思忆片段<br>
        不经意<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        在雨中漫步<br>
        蓝色街灯渐露<br>
        相对望<br>
        无声紧拥抱着<br>
        为了找往日<br>
        寻温馨的往日<br>
        消失了<br>
        任雨湿我面<br>
        难分水点泪痕<br>
        心更乱<br>
        愁丝绕千百段<br>
        骤变的态度<br>
        无心伤她说话<br>
        收不了<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        在雨中漫步<br>
        尝水中的味道<br>
        仿似是<br>
        情此刻的尽时<br>
        未了解结合<br>
        留低思忆片段<br>
        不经意<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        在雨中漫步<br>
        尝水中的味道<br>
        仿似是<br>
        情此刻的尽时<br>
        未了解结合<br>
        留低思忆片段<br>
        不经意<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        冷雨夜我在你身边<br>
        盼望你会知<br>
        可知道我的心<br>
        比当初已改变<br>
        只牵强地相处<br>
        冷雨夜我不想归家<br>
        怕望你背影<br>
        只苦笑望雨点<br>
        须知要说清楚<br>
        可惜我没胆试<br>
        在雨中漫步<br>
        蓝色街灯渐露<br>
        相对望<br>
        无声紧拥抱着<br>
        为了找往日<br>
        寻温馨的往日<br>
        消失了<br>
        任雨湿我面<br>
        难分水点泪痕<br>
        心更乱<br>
        愁丝绕千百段<br>
        骤变的态度<br>
        无心伤她说话<br>--%>
        <div id="div3">aa</div>
        </div> 
        <cc1:RelativeLink ID="RelativeLink1" runat="server" RelativeLinkPosition="Right"  RelativeLinkStatus="Expanded" MoreLinkCategory="fffffff" AlwaysVerticalCenter="False" DockContainer="div1" ExtendContent="鸟儿飞过旷野。一批又一批成,群的鸟儿接连不断地飞了过去。" />        
    <!--TitleStyle='{"fontSize":"12px","border":"0px solid"}'-->
        
    </form>
</body>
</html>
