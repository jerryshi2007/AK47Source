(function () {
    Array.prototype._defaultcomparingfn = function (obj, value) {
        if (obj == value) return true;
        return false;
    };

    Array.prototype.has = function (val, fnComparing) {
        if (fnComparing == undefined) {
            fnComparing = this._defaultcomparingfn;
        }

        var i;
        for (i = 0; i < this.length; i++) {
            if (fnComparing(this[i], val)) return true;
        }
        return false;
    };

    Array.prototype.get = function (val, fnComparing) {
        if (fnComparing == undefined) {
            fnComparing = this._defaultcomparingfn;
        }

        var i;
        for (i = 0; i < this.length; i++) {
            if (fnComparing(this[i], val)) return this[i];
        }
        return undefined;
    };

    /*移除符合条件的第一个元素*/
    Array.prototype.remove = function (val, fnComparing) {
        if (fnComparing == undefined) {
            fnComparing = this._defaultcomparingfn;
        }

        var i;
        for (i = 0; i < this.length; i++) {
            if (fnComparing(this[i], val)) {
                this.splice(i, 1);
                return;
            }
        }
    };

})();