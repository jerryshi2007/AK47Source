
function setHBDropDownListSelectedOption(select, value) {
	var result = null;

	if (select.tagName == "SELECT") {
		for (var i = 0; i < select.options.length; i++) {
			var opt = select.options.item(i);

			if (opt.value == value) {
				opt.selected = true;
				result = opt;
				break;
			}
		}
	}

	return result;
}

if (typeof ($HBDropDownList) === "undefined") {
	$HBDropDownList = {
		setValue: function (id, value) {
			var elem = document.getElementById(id);
			if (elem) {
				var selectedItem = setHBDropDownListSelectedOption(elem, value);
				if (selectedItem != null) {
					var selectedText = selectedItem.text;
					var selectedValue = selectedItem.value;

					var label = document.getElementById(elem.getAttribute("data-relativeLabel"));

					if (label) {
						label.innerHTML = "";
						var textNode = document.createTextNode(selectedText);
						label.appendChild(textNode);
					}

					var selectedTextHidden = document.getElementById(elem.getAttribute("data-relativeHidden"));
					var selectedValueHidden = document.getElementById(elem.getAttribute("data-relativeValueHidden"));

					if (selectedTextHidden) {
						selectedTextHidden.value = selectedText;
					}
					if (selectedValueHidden) {
						selectedValueHidden.value = selectedValue;
					}
				}
			}
		}
	};
}