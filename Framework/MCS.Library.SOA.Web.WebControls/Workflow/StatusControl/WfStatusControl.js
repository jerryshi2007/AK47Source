function onWfStatusLinkClick() {
	var link = event.srcElement;

	while (link.tagName != "A")
		link = link.parentElement;

	var firstLine = link.parentElement;
	var secondContainer = firstLine.nextSibling;

	if (secondContainer.style.display == "none")
		secondContainer.style.display = "inline";
	else
		secondContainer.style.display = "none";
}

function onWfTraceButtonClick() {
	event.returnValue = false;

	var elem = event.srcElement;

	while (!elem.resourceID)
		elem = elem.parentElement;

	var feature = elem.getAttribute("feature");
	var resourceID = elem.getAttribute("resourceID");

	window.open(elem.href, elem.target, feature);

	return false;
}