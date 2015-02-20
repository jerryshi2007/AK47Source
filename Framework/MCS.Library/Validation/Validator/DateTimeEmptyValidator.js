//注意，此客户端类的名称必须与服务端ClientValidateMethodName相同,默认为类名,且必须包含validate方法
$HGRootNS.ValidatorManager.DateTimeEmptyValidator = function () {
    this.validate = function (cvalue) {
        var isValidate = false;
        isValidate = (isNaN(cvalue) == false);

        if (isValidate)
            isValidate = Date.isMinDate(cvalue) == false;
        return isValidate;
    }
};
