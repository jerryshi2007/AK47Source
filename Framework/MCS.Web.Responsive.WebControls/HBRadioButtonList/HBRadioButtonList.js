
function initializeHBRadioButtonListControl(id, name, valueChangedFunName) {
	var inputs = document.getElementsByName(name);
	for (var i = 0; i < inputs.length; i++) {
		var input = inputs[i];
		if (input.tagName.toLowerCase() === "input") {
			if (input.attachEvent)
				input.attachEvent("onclick", function (e) { onHBRadioButtonListItemClick(e, id, valueChangedFunName); });
			else
				input.addEventListener("click", function (e) { onHBRadioButtonListItemClick(e, id, valueChangedFunName); });
		}
	}
}

function onHBRadioButtonListItemClick(e, id, valueChangedFunName) {
	var elem = document.getElementById(id);
	var selectedValueHidden = document.getElementById(elem.getAttribute("data-relativeValueHidden"));

	var element = e.target || window.event.srcElement;
	if (selectedValueHidden && element.value != selectedValueHidden.value) {
		var label = document.getElementById(elem.getAttribute("data-relativeLabel"));
		var selectedTextHidden = document.getElementById(elem.getAttribute("data-relativeHidden"));

		var selectedText = element.nextSibling.childNodes[0].nodeValue;

		if (label) {
			label.innerText = selectedText;
		}

		selectedTextHidden.value = selectedText;
		selectedValueHidden.value = element.value;

		if (valueChangedFunName) {
			if (typeof (window[valueChangedFunName]) == "function") {
				window[valueChangedFunName](element.value);
			}
			else {
				alert(valueChangedFunName + "()不是函数,该参数不可加()!");
			}
		}
	}
}
