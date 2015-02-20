/// <reference path="../jquery/jquery-2.0.3.js" />

$(document).ready(function () {
    (function () {
        var pageDatas = [
            { title: "日期时间选择", subTitle: "DateTimePicker", description: "日期和时间选择控件", baseUri: "/DateTimePicker/DateTimePickerTest.aspx" },
            { title: "菜单", subTitle: "DeluxeMenuStrip", description: "浮动菜单控件", baseUri: "/DeluxeMenu/DeluxeMenuTest.aspx" },
	        { title: "树控件", subTitle: "DeluxeTree", description: "树控件", baseUri: "/DeluxeTree/SimpleTreeTest.aspx" },
			{ title: "弹窗", subTitle: "ClientMsg", description: "客户端弹窗", baseUri: "/ModalBox/ClientMsgTest.aspx" },
			{ title: "表格", subTitle: "DeluxeGrid", description: "Grid控件", baseUri: "/DeluxeGrid/DeluxeGridForObjectDataSourceControl.aspx" }
        ];

        var navBar = $("#navbar-nav");
        $(pageDatas).each(function () {
            $('<li><a href="javascript:void(0);">' + this.title + " </a></li>").appendTo(navBar).data("paras", this);
        });

        navBar.on("click", "li", function () {
            var data = $(this).data("paras");
            $("li.active", navBar).removeClass("active");
            $(this).addClass("active");
            $("#slider h1").text(data.subTitle);
            $("#slider p").text(data.description);
            $("#main-content-inner").attr("src", data.baseUri);
        });

        $("li", navBar).first().click();

        function createA() {
            oGroup[0].innerHTML = "";
            for (var i = 0; i < arrLists.length; i++) {
                var oA = document.createElement('a');
                oGroup[0].appendChild(oA);
                oA.href = "#";
                oA.innerHTML = arrLists[i];
                oA.className = 'list-group-item';
            };
        };
        // iframe
        function dosome(text) {
            document.getElementById("getText").innerHTML = decodeURI(text);
        }
    })();
});
