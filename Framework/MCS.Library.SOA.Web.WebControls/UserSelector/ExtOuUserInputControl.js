$HBRootNS.ExtOuUserInputControl = function (element) {
    $HBRootNS.ExtOuUserInputControl.initializeBase(this, [element]);

    this._userTreeClientID = "";
    this._consignUserInputClientID = "";
    this._circulatorInputClientID = "";
    this._consignUserButtonClientID = "";
    this._circulatorButtonClientID = "";

    this._userTree = null;
    this._consignUserInput = null;
    this._circulatorInput = null;
    this._consignUserButton = null;
    this._circulatorButton = null;

    this._originalConsignUsers = null;
    this._originalCirculator = null;

    this._consignUserButton$Delegate = {
        click: Function.createDelegate(this, this._consignUserButtonClick)
    }

    this._circulatorButton$Delegate = {
        click: Function.createDelegate(this, this._circulatorButtonClick)
    }
}

$HBRootNS.ExtOuUserInputControl.prototype =
{
	initialize: function () {
		$HBRootNS.ExtOuUserInputControl.callBaseMethod(this, 'initialize');

		this._userTree = $find(this._userTreeClientID);
		this._consignUserInput = $find(this._consignUserInputClientID);
		this._circulatorInput = $find(this._circulatorInputClientID);

		this._consignUserButton = $get(this._consignUserButtonClientID);
		$addHandlers(this._consignUserButton, this._consignUserButton$Delegate);

		this._circulatorButton = $get(this._circulatorButtonClientID);

		if (this._circulatorButton != null)
			$addHandlers(this._circulatorButton, this._circulatorButton$Delegate);

		this._consignUserInput.set_selectedOuUserData(this._originalConsignUsers);
		this._consignUserInput.setInputAreaText();

		if (this._circulatorInput != null) {
			this._circulatorInput.set_selectedOuUserData(this._originalCirculator);
			this._circulatorInput.setInputAreaText();
		}
	},

	dispose: function () {

		if (this._consignUserButton != null)
			$HGDomEvent.removeHandlers(this._consignUserButton, this._consignUserButton$Delegate);

		if (this._circulatorButton != null)
			$HGDomEvent.removeHandlers(this._circulatorButton, this._circulatorButton$Delegate);

		$HBRootNS.ExtOuUserInputControl.callBaseMethod(this, 'dispose');
	},

	_consignUserButtonClick: function () {
		this._pushDataToInputControl(this._consignUserInput, this._userTree.get_selectedObjects());
		this._userTree.clearSelectedObjects();
	},

	_circulatorButtonClick: function () {
		this._pushDataToInputControl(this._circulatorInput, this._userTree.get_selectedObjects());
		this._userTree.clearSelectedObjects();
	},

	_pushDataToInputControl: function (input, data) {
		//debugger;
		var inputOriginalData = input.get_selectedOuUserData();

		if (input.get_multiSelect() == false) {
			inputOriginalData = [];
		}

		for (var i = 0; i < data.length; i++) {
			if (!input._checkUserInList(data[i].id))
				inputOriginalData.push(data[i]);
		}

		input.set_selectedOuUserData(inputOriginalData);
		input.setInputAreaText();
	},

	get_consignUsers: function () {
		//debugger;
		return this._consignUserInput.get_selectedOuUserData();
	},

	set_consignUsers: function (value) {
		if (this._consignUserInput)
			this._consignUserInput.set_selectedOuUserData(value);
		else
			this._originalConsignUsers = value;
	},

	get_circulators: function () {
		//debugger;
		var result = [];

		if (this._circulatorInput)
			result = this._circulatorInput.get_selectedOuUserData();

		return result;
	},

	set_circulators: function (value) {
		if (this._circulatorInput)
			this._circulatorInput.set_selectedOuUserData(value);
		else
			this._originalCirculator = value;
	},

	get_userTreeClientID: function () {
		return this._userTreeClientID;
	},

	set_userTreeClientID: function (value) {
		this._userTreeClientID = value;
	},

	get_consignUserInputClientID: function () {
		return this._consignUserInputClientID;
	},

	set_consignUserInputClientID: function (value) {
		this._consignUserInputClientID = value;
	},

	get_circulatorInputClientID: function () {
		return this._circulatorInputClientID;
	},

	set_circulatorInputClientID: function (value) {
		this._circulatorInputClientID = value;
	},

	get_consignUserButtonClientID: function () {
		return this._consignUserButtonClientID;
	},

	set_consignUserButtonClientID: function (value) {
		this._consignUserButtonClientID = value;
	},

	get_circulatorButtonClientID: function () {
		return this._circulatorButtonClientID;
	},

	set_circulatorButtonClientID: function (value) {
		this._circulatorButtonClientID = value;
	},

	//Not used
	RI: function () {
	}
}

$HBRootNS.ExtOuUserInputControl.registerClass($HBRootNSName + ".ExtOuUserInputControl", $HGRootNS.ControlBase);