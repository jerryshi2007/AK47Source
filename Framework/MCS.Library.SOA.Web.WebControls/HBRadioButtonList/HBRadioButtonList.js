function initializeHBRadioButtonListControl(id, name, valueChangedFunName) {
    var inputs = document.getElementsByName(name);
    for (var i = 0; i < inputs.length; i++) {
        var input = inputs[i];
        if (input.tagName.toLowerCase() === "input") {
            if (input.attachEvent)
                input.attachEvent("onclick", getHBRadioButtonListItemClickFunction(id, valueChangedFunName));
            else
                input.addEventListener("onclick", getHBRadioButtonListItemClickFunction(id, valueChangedFunName));
        }
    }
}

function onHBRadioButtonListItemClick(id, valueChangedFunName) {
    var elem = document.getElementById(id);
    var selectedValueHidden = document.getElementById(elem.relativeValueHidden);

    var element = window.event.srcElement;
    if (selectedValueHidden && element.value != selectedValueHidden.value) {
        var label = document.getElementById(elem.relativeLabel);
        var selectedTextHidden = document.getElementById(elem.relativeHidden);

        var selectedText = element.nextSibling.innerText;

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

function getHBRadioButtonListItemClickFunction(id, valueChangedFunName) {
    return function () {
        onHBRadioButtonListItemClick(id, valueChangedFunName);
    }
}