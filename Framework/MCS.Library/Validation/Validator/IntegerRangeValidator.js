$HGRootNS.ValidatorManager.IntegerRangeValidator = function () {
    this.validate = function (cvalue, additionalData) {
        var sourcevalue = cvalue;
        var lowerBound = additionalData.lowerBound;
        var upperBound = additionalData.upperBound;

        sourcevalue = sourcevalue.toString().replace(/,/g, '');

        if (isNaN(sourcevalue * 1) == true) {
            return false;
        }
        if (sourcevalue * 1 < lowerBound * 1 || sourcevalue * 1 > upperBound * 1) {
            return false;
        }
        else {
            return true;
        }
    }
};
