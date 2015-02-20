function selectTag(showContent, selfObj) {
    // 操作标签
    var tag = document.getElementById("tags").getElementsByTagName("li");
    var taglength = tag.length;
    for (i = 0; i < taglength; i++) {
        tag[i].className = "";
    }
    selfObj.parentNode.className = "selectTag";
    // 操作内容
    for (i = 0; j = document.getElementById("tagContent" + i); i++) {
        j.style.display = "none";
    }
    document.getElementById(showContent).style.display = "block";

//    var grid = $find($("#" + showContent).attr("gridId"));
//    if (grid) {
//        var dataSource = grid.get_dataSource();
//        if (dataSource == null || dataSource.length == 0) {
//            setTimeout(function () {
//                grid.set_dataSource($find("DevelopmentLoanGrid").get_dataSource());
//            }, 1);
//        }
//    }

//}