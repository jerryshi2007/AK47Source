/// <reference path="jquery/jquery-2.0.3.js" />

/**
* 
* @Sniper (it_163@126.com)
* @date    2014-05-14 14:09:35
* @version $Id$
*/
$(function () {
    var sideBar = $(".sidebar");
    var sideBarMenu = $(".sidebar-menu");
    var minified = sideBar.parent().hasClass("sidebar-minified");
    var dropped = sideBar.hasClass("menu-dropped");
    var startX, startY;

    sideBarMenu.on("click", "li[role=dropdown-toggle]>a", function (e) {
        $(this.parentNode).toggleClass("open");
        e.preventDefault();
        return false;
    });
//    sideBarMenu.on("touchstart", function (e) {
//        e.preventDefault();
//        if (dropped) {
//            var t = e.originalEvent.touches[0];
//            startX = Number(t.pageX);
//            startY = Number(t.pageY);
//        }
//    }).on("touchmove", function (e) {
//        e.preventDefault();
//        if (dropped) {
//            var t = e.originalEvent.touches[0];
//            var thisX = Number(t.pageX);
//            var thisY = Number(t.pageY);

//            var offsetY = thisY - startY;
//            // 暂时不使用滑动
//        }
//    });

    $(".sidebar .menu-minifier").on("click", function () {
        sideBar.parent().toggleClass("sidebar-minified");
        minified = !minified;
    });

    $(".menu-toggler").click(function () {
        $(".sidebar").toggleClass("menu-dropped");
        dropped = !dropped;
    });
});
