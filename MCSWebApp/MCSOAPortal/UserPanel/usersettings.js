

        var CountryEnum = [];

        function onDocumentLoad(sender, args) {

            InitBindData();
        }

        function InitBindData() {
        	var enumData = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenSource").value);
        	for (var i = 0; i < enumData.length; i++) {

        		CountryEnum.push({ "text": enumData[i].text, "value": enumData[i].value });
        	}
        }


        function onBindEditorDropdownList(sender, e) {
        	if (CountryEnum.length == 0) {
                InitBindData();
            }
            var propertyGrid = sender;
            switch (e.property.name) {

            	case "CountryCode":
            		e.enumDesc = CountryEnum;
            		break;
            }
        }
