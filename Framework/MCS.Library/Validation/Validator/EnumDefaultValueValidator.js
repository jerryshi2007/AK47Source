$HGRootNS.ValidatorManager.EnumDefaultValueValidator = function () {
    this.validate = function (cvalue, additionalData) {
        var isValidate = false;
        if (cvalue) {
            isValidate = cvalue != additionalData.tag;
        }
        return isValidate;
    }
};
