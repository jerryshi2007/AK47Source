$HGRootNS.ValidatorManager.StringEmptyValidator = function () {
    this.validate = function (cvalue) {
        var isValidate = false;
        if (cvalue) {
            isValidate = cvalue.length > 0;
        }
        return isValidate;
    }
};
