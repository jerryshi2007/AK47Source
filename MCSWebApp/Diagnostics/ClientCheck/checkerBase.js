AllCheckers = [];

Checker = function() {
	this._name = "";
	this._element = null;
	this._context = null;
}

Checker.registerChecker = function(checker) {
	AllCheckers.push(checker);
}

Checker.get_allCheckers = function() {
	return AllCheckers;
}

Checker.clearAllCheckers = function() {
	AllCheckers = [];
}

Checker.prototype = {
	get_name: function() {
		return this._name;
	},

	set_name: function(value) {
		this._name = value;
	},

	get_element: function() {
		return this._element;
	},

	set_element: function(value) {
		this._element = value;
	},

	get_context: function() {
		return this._context;
	},

	set_context: function(value) {
		this._context = value;
	},

	check: function() {
	},

	_pseudo: function() {
	}
}