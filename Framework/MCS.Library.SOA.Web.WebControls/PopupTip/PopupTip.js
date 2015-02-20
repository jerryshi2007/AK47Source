
$HGRootNS.PopupTip = function ( element )
{
    $HGRootNS.PopupTip.initializeBase( this, [element] );
    //local
    this.waitTimer = null;
    this.showTimer = null;
    this.popup = null;
    //fields
    this.timerPopWait = null;
    this.timerPopShow = null;
    this.reference = null;
    this.tipWidth = null;
    this.tipHeight = null;
    this.htmlContent = null;
    this.isPopup = null;
    this.fixedContainer = null;
    this.popupStyle = null;
    this.tipName = null;
    this.preloadContent = null;
    this.isSelfAdaption = null;
    this.popupPostion = null;
    //events
    this._onclickHandler = null;
    this._onmouseoutDelegate = null;
    this._onmouseoverDelegate = null;
    this._onmousemoveDelegate = null;
    this._applicationLoadDelegate = null;

};
$HGRootNS.PopupTip.prototype = {
    initialize: function ()
    {

        $HGRootNS.PopupTip.callBaseMethod( this, "initialize" );

        var element = $get( this.reference );
        var container = $get( this.fixedContainer );

        this._onmouseoutDelegate = Function.createDelegate( this, this._onmouseout );
        this._onmouseoverDelegate = Function.createDelegate( this, this._onmouseover );
        this._onmousemoveDelegate = Function.createDelegate( this, this._onmousemove );
        this._mouse$delegates = { "mouseout": this._onmouseoutDelegate, "mouseover": this._onmouseoverDelegate, "mousemove": this._onmousemoveDelegate };
        $addHandlers( element, this._mouse$delegates );

        this._applicationLoadDelegate = Function.createDelegate( this, this._applicationLoad );
        if ( this.preloadContent && container )
            Sys.Application.add_load( this._applicationLoadDelegate );
    },
    
    get_timerPopWait: function ()
    {
        return this.timerPopWait;
    },
    set_timerPopWait: function ( value )
    {
        this.timerPopWait = value;
    },
    get_timerPopShow: function ()
    {
        return this.timerPopShow;
    },
    set_timerPopShow: function ( value )
    {
        this.timerPopShow = value;
    },
    get_reference: function ()
    {
        return this.reference;
    },
    set_reference: function ( value )
    {
        this.reference = value;
    },
    get_tipWidth: function ()
    {
        return this.tipWidth;
    },
    set_tipWidth: function ( value )
    {
        this.tipWidth = value;
    },
    get_tipHeight: function ()
    {
        return this.tipHeight;
    },
    set_tipHeight: function ( value )
    {
        this.tipHeight = value;
    },
    get_htmlContent: function ()
    {
        return this.htmlContent;
    },
    set_htmlContent:function ( value )
    {
        this.htmlContent = value;
    },

    get_isPopup: function ()
    {
        return this.isPopup;
    },
    set_isPopup: function ( value )
    {
        this.isPopup = value;
    },

    get_fixedContainer: function ()
    {
        return this.fixedContainer;
    },
    set_fixedContainer: function ( value )
    {
        this.fixedContainer = value;
    },
    get_popupStyle: function ()
    {
        return this.popupStyle;
    },
    set_popupStyle: function ( value )
    {
        this.popupStyle = value;
    },
    get_tipName: function ()
    {
        return this.tipName;
    },
    set_tipName: function ( value )
    {
        this.tipName = value;
    },
    get_preloadContent: function ()
    {
        return this.preloadContent;
    },
    set_preloadContent: function ( value )
    {
        this.preloadContent = value;
    },
    get_isSelfAdaption: function ()
    {
        return this.isSelfAdaption;
    },
    set_isSelfAdaption: function ( value )
    {
        this.isSelfAdaption = value;
    },
    get_popupPostion: function ()
    {
        return this.popupPostion;
    },
    set_popupPostion: function ( value )
    {
        this.popupPostion = value;
    },
    _getStyle: function ( obj, attr )
    {

        return obj.currentStyle ? obj.currentStyle[attr] : getComputedStyle( obj, false )[attr];

    },

    _setStyle: function ( style, element )
    {
        for ( var p in style )
        {
            element.style[p] = style[p];
        }
    },

    _applicationLoad: function ()
    {
        var element = $get( this.fixedContainer );
        element.innerHTML = this.htmlContent;
    },

    _computedPopPostion: function ( innerhtml )
    {
        if ( this.isSelfAdaption )
        {
            var computedDiv = document.createElement( "div" );
            this._setStyle( {
                position: "absolute",
                visibility: "hidden",
                width: this.tipWidth + "px",
                zIndex: "-1000",
                border: "0"
            }, computedDiv );
            computedDiv.innerHTML = innerhtml;

            document.body.appendChild( computedDiv );

            var retValue = { "h": computedDiv.clientHeight, "w": computedDiv.clientWidth };

            document.body.removeChild( computedDiv );

            return retValue;
        } else
        {
            return { "h": this.tipHeight, "w": this.tipWidth };
        }
    },
    _setpostion: function ( e, tipSize )
    {
        var result = null;
        switch ( this.popupPostion )
        {
            case 0:
                result = {
                    "x": e.offsetX + e.target.getBoundingClientRect().left < 113 ? e.offsetX - e.target.getBoundingClientRect().left : e.offsetX - 113,
                    "y": -tipSize.h
                };
                break;
            case 1:
                result = {
                    "x": e.offsetX + e.target.getBoundingClientRect().left < 34 ? e.offsetX - e.target.getBoundingClientRect().left : e.offsetX - 34,
                    "y": $get( this.reference ).offsetHeight
                };
                break;
        }
        return result;
    },
    showtip: function ( e )
    {
        if ( this.isPopup )
        {
            var innerhtml;

            switch ( this.popupStyle )
            {
                case 0:
                    innerhtml = '<div class="bubble_tooltip"><div class="bubble_top"><span></span></div><div class="bubble_middle"><span class="bubble_tooltip_content">'
                    + this.htmlContent +
                        '</span></div><div class="bubble_bottom"></div></div>';

                    break;
                case 1:
                    innerhtml = '<div class="bubble_tooltip1"><div class="bubble_top"><span></span></div><div class="bubble_middle"><span class="bubble_tooltip_content">'
                    + this.htmlContent +
                        '</span></div><div class="bubble_bottom"></div></div>';
                    break;
                case 2:
                    innerhtml = '<div style="margin: 0px auto;margin-bottom:0px;border:1px solid #BBE1F1;background-color: #B2D3F5">'
                    + this.htmlContent + '</div>';
                    break;
                case 3:
                default:
                    innerhtml = this.htmlContent;
                    break;
                case 4:
                    innerhtml = '<div style="margin: 0px auto;margin-bottom:0px;border:1px solid #9BDF70;background-color: #C2ECA7">'
                    + this.htmlContent + '</div>';
                    break;
            }
            var tipSize = this._computedPopPostion( innerhtml );
            var tipPos = this._setpostion( e, tipSize );

            this.popup = $create( $HGRootNS.PopupControl,
                {
                    //style: { backgroundColor: "red" },
                    x: tipPos.x,
                    y: tipPos.y,
                    width: tipSize.w,
                    height: tipSize.h ,
                    positionElement: $get( this.reference ),
                    applyFilter: false,
                    usePublicPopupWindow: false
                }, null, null, null );

            var innerDiv = $HGDomElement.createElementFromTemplate( {
                nodeName: "div"
            }, this.popup.get_popupBody(), null, this.popup.get_popupDocument() );

            innerDiv.innerHTML = innerhtml;
            this.popup.show();
        } else
        {
            var element = $get( this.fixedContainer );
            element.innerHTML = this.htmlContent;
        }
    },

    hidetip: function ()
    {
        if ( this.isPopup )
        {

            if ( this.popup ) this.popup.hide();

        } else
        {
            var element = $get( this.fixedContainer );
            element.innerHTML = "";
        }
    },

    _onmouseover: function ( e )
    {
        var that = this;

        if ( this.timerPopWait == 0 )
        {
            this.showtip( e );
        } else
        {
            clearTimeout( this.waitTimer );
            clearTimeout( this.showTimer );

            this.waitTimer = setTimeout( 
                function ()
                {
                    that.showtip( e );
                }, that.timerPopWait
            );
        }
        if ( this.timerPopShow == -1 ) return;
        this.showTimer = setTimeout( function ()
        {
            that.hidetip();

        }, that.timerPopShow );
    },

    _onmouseout: function ()
    {
        clearTimeout( this.waitTimer );
        clearTimeout( this.showTimer );
        this.hidetip();
    },
    _onmousemove: function ()
    {

    },

    dispose: function ()
    {
        var element = this.get_element();

        $clearHandlers( element );

        if ( this._onmouseoutDelegate )
        {
            delete this._onmouseoutDelegate;
        }
        if ( this._onmouseoverDelegate )
        {
            delete this._onmouseoverDelegate;
        }
        if ( this._onmousemoveDelegate )
        {
            delete this._onmousemoveDelegate;
        }
        $HGRootNS.PopupTip.callBaseMethod( this, "dispose" );

    }
};
$HGRootNS.PopupTip.registerClass($HGRootNSName + ".PopupTip", $HGRootNS.ControlBase);