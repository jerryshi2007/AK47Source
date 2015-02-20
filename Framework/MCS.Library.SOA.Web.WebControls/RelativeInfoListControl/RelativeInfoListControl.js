function onOpenMoreClick(hiddenInputId, methodType, actionUrl) {
    var tempForm = document.createElement("form");
    tempForm.action = actionUrl;
    tempForm.method = methodType;
    tempForm.target="_blank"
    document.body.appendChild(tempForm);
    var tempInput = document.createElement("input");
    tempInput.type = "hidden";
    tempInput.name = "jsonStr";
    tempInput.value = document.getElementById(hiddenInputId).value;
    tempForm.appendChild(tempInput);
    tempForm.submit();　
}