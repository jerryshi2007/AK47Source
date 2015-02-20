function onOpinionInputPropertyChange(elem) {
	// var elem = event.srcElement; // do NOT work under IE 7

	adjustInputHeight(elem);
}

function adjustInputHeight(elem) {

	elem.style.posHeight = (elem.scrollHeight > parseInt(elem.style.minHeight.replace('px', '')))
		? elem.scrollHeight : parseInt(elem.style.minHeight.replace('px', ''));
}

function appendElement(ddl, text, value) {
	var option = document.createElement("OPTION");

	option.appendChild(document.createTextNode(text));
	option.setAttribute("value", value);

	ddl.appendChild(option);
}

function onEditPredefinedOpinionClick(dialogID, userID, ddlistID, activityID) {
	var elem = event.srcElement;
	var result = $find(dialogID).showDialog(userID, activityID);

	if (result != null) {
		var ddlist = document.getElementById(ddlistID);

		if (ddlist && (ddlist.tagName == "SELECT")) {
			ddlist.options.length = 1;
			var arr = result.split(" ");

			for (var i = 0; i < arr.length; i++) {
				var text = arr[i];

				if (text.length > 0)
					appendElement(ddlist, text, text);
			}

			ddlist.options.selectedIndex = 0;
		} else {
			alert("常用意见已经修改，请刷新页面！");
		}
	}
}