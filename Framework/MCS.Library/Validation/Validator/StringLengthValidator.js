$HGRootNS.ValidatorManager.StringLengthValidator = function () {
    this.validate = function (cvalue, additionalData) {
        var sourcevalue = cvalue.length;
        var lowerBound = additionalData.lowerBound;
        var upperBound = additionalData.upperBound;

        if (sourcevalue * 1 < lowerBound * 1 || sourcevalue * 1 > upperBound * 1) {
            return false;
        }
        else {
            return true;
        }
    }
};
