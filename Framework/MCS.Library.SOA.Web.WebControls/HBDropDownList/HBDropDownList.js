function onHBDropDownListChange() {
	var elem = event.srcElement;

	if (event.propertyName == 'value') {
	    window.setTimeout(function () {
	        var selectedItem = findHBDropDownListOption(elem);
	        if (selectedItem != null) {
	            var selectedText = selectedItem.text;
	            var selectedValue = selectedItem.value;

	            var label = document.getElementById(elem.relativeLabel);

	            if (label) {
	                label.innerText = selectedText;
	            }

	            var selectedTextHidden = document.getElementById(elem.relativeHidden);
	            var selectedValueHidden = document.getElementById(elem.relativeValueHidden);

	            if (selectedTextHidden) {
	                selectedTextHidden.value = selectedText;
	            }
	            if (selectedValueHidden) {
	                selectedValueHidden.value = selectedValue;
	            }
	        }
	    }, 0);
	}
}

function attachHBDropDownListEvents(elem) {
    if (elem.attachEvent)
        elem.attachEvent("onpropertychange", onHBDropDownListChange);
    else
        elem.addEventListener("onpropertychange", onHBDropDownListChange, false);
}

function findHBDropDownListOption(select) {
    var result = null;

	if (select.tagName == "SELECT") {
		for (var i = 0; i < select.options.length; i++) {
			var opt = select.options.item(i);

			if (opt.value == select.value) {
				result = opt;
				break;
			}
		}
	}

	return result;
}