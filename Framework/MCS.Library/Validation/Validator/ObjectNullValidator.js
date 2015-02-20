$HGRootNS.ValidatorManager.ObjectNullValidator = function () {
    this.validate = function (cvalue) {
        var isValidate = false;
        if (cvalue) {
            isValidate = true;
        }
        return isValidate;
    }
};
