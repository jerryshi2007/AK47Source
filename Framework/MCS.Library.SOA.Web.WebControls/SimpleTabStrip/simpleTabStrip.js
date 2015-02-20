function tabStripClientValidate(source, arguments)
{
	var strip = document.all(source.tabStripControlID);
	var hiddenData = document.all(source.hiddenDataControlID);

	if (strip.selectedItem)
		hiddenData.value = strip.selectedItem.key;
}