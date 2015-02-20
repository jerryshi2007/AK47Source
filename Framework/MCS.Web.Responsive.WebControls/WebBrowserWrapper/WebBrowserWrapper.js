//WebBrowserWrapper = function (element) {
//    this._onPrint = null;
//    this.webBrowserObj = null;
//}

//WebBrowserWrapper.prototype =
//{

//    //打印
//    WebBrowserWrapperPrint: function () {

//        this.GetWebBrowserObj().ExecWB(6, 1);
//    },

//    //打印预览
//    WebBrowserWrapperPrintPreview: function () {
//        this.GetWebBrowserObj().ExecWB(7, 1);
//    },
//    WebBrowserWrapperCloseWindow: function () {
//        this.GetWebBrowserObj().ExecWB(45, 1);
//    },
//    WebBrowserWrapperRefreshWindow: function myfunction() {
//        this.GetWebBrowserObj().ExecWB(22, 1);
//    },
//    GetWebBrowserObj: function () {
//        if (this.webBrowserObj) {
//            return this.webBrowserObj;
//        } else {
//            return document.getElementById("WebBrowserObj");
//        }
//    }

//}


//var WebBrowserWrapperInstance;

//(function () {
//    WebBrowserWrapperInstance = new WebBrowserWrapper();

//})();

(function () {
	WebBrowserWrapperFactory = {
		Create: function () {
			if (WebBrowserWrapper.Instance == null) {
				return new WebBrowserWrapper();
			} else {
				return WebBrowserWrapper.Instance;
			}
		}
	}

	var WebBrowserWrapper = function () {
		this.webBrowserObj = null;
		WebBrowserWrapper.Instance = this;
	}

	WebBrowserWrapper.prototype =
    {
    	//打印
    	print: function () {
    		this._getWebBrowserObj().ExecWB(6, 1);
    	},

    	//打印预览
    	preview: function () {
    		this._getWebBrowserObj().ExecWB(7, 1);
    	},

    	//关闭窗口
    	close: function () {
    		this._getWebBrowserObj().ExecWB(45, 1);
    	},

    	//刷新页面
    	refresh: function () {
    		this._getWebBrowserObj().ExecWB(22, 1);
    	},

    	_getWebBrowserObj: function () {
    		if (!this.webBrowserObj) {
    			this.webBrowserObj = document.getElementById("WebBrowserObj");
    		}
    		return this.webBrowserObj;
    	}
    }

})();
