function onOpenFormButtonClick() {
	event.returnValue = false;

	var elem = event.srcElement;

	while (!elem.resourceID)
		elem = elem.parentElement;

	var feature = elem.getAttribute("feature");
	var resourceID = elem.getAttribute("resourceID");

	window.open(elem.href, elem.target, feature);

	return false;
}