$HGRootNS.ValidatorManager.RegexValidator = function () {
    this.validate = function (cvalue, additionalData) {
        if (additionalData.isNumber === true) {
            cvalue = cvalue.toString().replace(/,/g, '');
        }

        var isValidate = false;
        var reg = new RegExp(additionalData.pattern);
        isValidate = reg.test(cvalue);

        return isValidate;
    }
};
