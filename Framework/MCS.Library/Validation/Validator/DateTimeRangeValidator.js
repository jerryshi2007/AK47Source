$HGRootNS.ValidatorManager.DateTimeRangeValidator = function () {
    this.validate = function (cvalue, additionalData) {
        var isValidate = false;
        var sourcevalue = cvalue;
        var lowerBound = additionalData.lowerBound;
        var upperBound = additionalData.upperBound;

        if (sourcevalue * 1 < lowerBound * 1 || sourcevalue * 1 > upperBound * 1) {
            isValidate = false;
        }
        else {
            isValidate = true;
        }

        return isValidate;
    }
};
