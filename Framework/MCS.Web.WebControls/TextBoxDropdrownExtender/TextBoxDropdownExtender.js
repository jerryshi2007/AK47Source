function onTextBoxDropdownExtenderDropdown() {
	var elem = event.srcElement;
	var textBoxID = elem.getAttribute("textBoxID");

	if (textBoxID) {
		showTextBoxDropdownExtenderDropdown(
			document.getElementById(elem.textBoxID || elem.getAttribute("textBoxID")),
			document.getElementById(elem.listBoxID || elem.getAttribute("listBoxID")),
            document.getElementById(elem.iframeID || elem.getAttribute("iframeID"))
			);
	}
}

function onTextBoxDropdownExtenderDropdownBlur() {
	var elem = event.srcElement;
	elem.style["display"] = "none";

}

function onTextBoxDropdownExtenderDropdownChange() {
	var elem = event.srcElement;

	if (elem.textBoxID) {
		document.getElementById(elem.textBoxID).value = elem.value;
	}
}

function onTextBoxDropdownExtenderDropdownClick() {
	var elem = event.srcElement;

	if (elem.textBoxID) {
		document.getElementById(elem.textBoxID).value = elem.value;
	}

	elem.style["display"] = "none";
}

function onTextBoxDropdownExtenderTextBoxKeyDown() {
	var elem = event.srcElement;

	if (event.altKey) {
		if (event.keyCode == 40) {
			var textBox = elem;
			var listBox = document.getElementById(elem.listBoxID);

			showTextBoxDropdownExtenderDropdown(textBox, listBox);
		}
	}
}

function onTextBoxDropdownExtenderDropdownKeyDown() {
	var elem = event.srcElement;
	var textBox = null;

	if (elem.textBoxID) {
		textBox = document.getElementById(elem.textBoxID);
	}

	if (textBox) {
		switch (event.keyCode) {
			case 13:
				textBox.value = elem.value;
				textBox.focus();

				elem.style["display"] = "none";
				event.keyCode = 0;
				event.returnValue = false;
				break;
			case 27:
				textBox.value = elem.value;
				textBox.focus();
				elem.style["display"] = "none";
				break;
		}
	}
}

function GetDialogContent() {
    var dialogContent = null;
    var containers = document.getElementsByTagName("div");
    for (var i = 0; i < containers.length; i++) {
        if(containers[i].className == "dialogContent"){
            dialogContent = containers[i];
            break;
        }
    }

    return dialogContent;
}

function showTextBoxDropdownExtenderDropdown(textBox, listBox, iframe) {
    var dialogContent = GetDialogContent();
//    if (dialogContent)
//        dialogContent.style.overflow = "visible";
    var textBoxBounds = $HGDomElement.getBounds(textBox);
    var value = textBox.value;
    var options = listBox.options;
    options.selectedIndex = -1;
    for (var i = 0; i < options.length; i++) {
        if (value === options[i].value) {
            options[i].selected = true;
            break;
        }
    }
	iframe.style.top = 0;
	iframe.style.left = 0;
	listBox.style.top = 0;
	listBox.style.left = 0;

	iframe.style.display = "inline";
	iframe.style.visibility = "hidden";

	listBox.style.display = "inline";

	var listBoxSize = $HGDomElement.getSize(listBox);
	var listBoxPosition = $HGDomElement.getBounds(listBox);
	listBox.style.width = textBoxBounds.width;

	var positionMode = GetPositionMode(textBox, listBox);
	var position = GetDisplayPosition(positionMode, textBox, listBox);


	listBox.style.left = position.x;
	listBox.style.top = position.y;

	listBox.focus();

	iframe.style.left = position.x;
	iframe.style.top = position.y;
	iframe.style.width = listBox.style.width;
//	if (dialogContent)
//	    dialogContent.style.overflow = "visible";

}

function get_documentWidth () {
		var w = document.documentElement.clientWidth;

		if (w == 0)
			w = document.body.offsetWidth;

		return w;
}

function get_documentHeight () {
		var h = document.documentElement.clientHeight;

		if (h == 0)
			h = document.body.offsetHeight;

		return h;
}

function GetPositionMode(textBox, listBox) {
    var positionMode = $HGRootNS.PositioningMode.BottomLeft;
    var textBoxBounds = $HGDomElement.getBounds(textBox);
    var listBoxSize = $HGDomElement.getSize(listBox);
    if ((textBoxBounds.y + textBoxBounds.height + listBoxSize.height) > get_documentHeight()) {
        positionMode = $HGRootNS.PositioningMode.TopLeft;
    }
    if ((textBoxBounds.x + listBoxSize.width) > get_documentWidth
    && (textBoxBounds.y + textBoxBounds.height + listBoxSize.height) < get_documentHeight()) {
        positionMode = $HGRootNS.PositioningMode.BottomRight;
    }
    if ((textBoxBounds.x + listBoxSize.width) > get_documentWidth()// $HGDomElement.getClientBounds(document.documentElement).width
    && (textBoxBounds.y + textBoxBounds.height + listBoxSize.height) > get_documentHeight()) {
        positionMode = $HGRootNS.PositioningMode.TopRight;
    }
    //$HGDomElement.getClientBounds(document.documentElement).height);
    return positionMode;
}

function GetDisplayPosition(positioningMode, textBox,listBox) {
    var position = {x:0,y:0};

	var textBoxBounds = GetAbsoluteLocationEx(textBox);//$HGDomElement.getBounds(textBox);
    var listBoxSize = $HGDomElement.getSize(listBox);

    switch (positioningMode) {
        case $HGRootNS.PositioningMode.TopLeft:
            position.x = textBoxBounds.x;
            position.y = textBoxBounds.y - listBoxSize.height;
            break;
        case $HGRootNS.PositioningMode.BottomLeft:
            position.x = textBoxBounds.x;
            position.y = textBoxBounds.y + textBoxBounds.height;
            break;
        case $HGRootNS.PositioningMode.BottomRight:
            position.x = (textBoxBounds.x + textBoxBounds.width) - listBoxSize.width;
            position.y = textBoxBounds.y + textBoxBounds.height;
            break;
        case $HGRootNS.PositioningMode.TopRight:
            position.x = (textBoxBounds.x + textBoxBounds.width) - listBoxSize.width;
            position.y = textBoxBounds.y - listBoxSize.height;
            break;

    }

    return position;
}

function GetAbsoluteLocationEx(element) 
{ 
    if ( arguments.length != 1 || element == null ) 
    { 
        return null; 
    } 
    var elmt = element; 
    var offsetTop = elmt.offsetTop; 
    var offsetLeft = elmt.offsetLeft; 
    var offsetWidth = elmt.offsetWidth; 
    var offsetHeight = elmt.offsetHeight; 
    while( elmt = elmt.offsetParent ) 
    { 
          // add this judge 
        if ( elmt.style.position == 'absolute' || elmt.style.position == 'relative'  
            || ( elmt.style.overflow != 'visible' && elmt.style.overflow != '' ) ) 
        { 
            break; 
        }  
        offsetTop += elmt.offsetTop; 
        offsetLeft += elmt.offsetLeft; 
    } 
    return { y: offsetTop, x: offsetLeft, 
        width: offsetWidth, height: offsetHeight }; 
} 
