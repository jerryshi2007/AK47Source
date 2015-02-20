$HGRootNS.ValidatorManager.IOguObjectNullValidator = function () {
    this.validate = function (cvalue) {
        var isValidate = false;

        if (cvalue) {
            if (cvalue.length > 0 && cvalue[0].id) {
                isValidate = true;
            }
        }

        return isValidate;
    }
};
