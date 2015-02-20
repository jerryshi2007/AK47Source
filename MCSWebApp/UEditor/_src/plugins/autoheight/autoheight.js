///import core
///commands 当输入内容超过编辑器高度时，编辑器自动增高
///commandsName  AutoHeight
///commandsTitle  自动增高
/**
 * @description 自动伸展
 * @author zhanyi
 */
(function() {

    var domUtils = baidu.editor.dom.domUtils;

    baidu.editor.plugins['autoheight'] = function() {
        var me = this;
        //提供开关，就算加载也可以关闭
        me.autoHeightEnabled = me.options.autoHeightEnabled;
        
        var timer;
        var bakScroll;
        var bakOverflow,
            span,tmpNode;
       
        me.enableAutoHeight = function (){
            var iframe = me.iframe,
                doc = me.document;


            me.autoHeightEnabled = true;
            bakScroll = iframe.scroll;
            bakOverflow = doc.body.style.overflowY;
            iframe.scroll = 'no';
            //doc.body.style.overflowY = 'hidden';
            var lastHeight = 0,currentHeight;
            timer = setInterval(function(){
                if (me.queryCommandState('source') != 1) {
                        if(!span){
                            span = me.document.createElement('span');
                            span.style.cssText = 'margin:0;padding:0;border:0;clear:both;display:block;';
                            span.innerHTML ='.';

                        }
                        tmpNode = span.cloneNode(true);
                        me.body.appendChild(tmpNode);
                        currentHeight = Math.max(domUtils.getXY(tmpNode).y + tmpNode.offsetHeight,me.options.minFrameHeight);
                        if(!baidu.editor.browser.gecko || currentHeight - lastHeight != tmpNode.offsetHeight){
                            me.setHeight(currentHeight);
                            lastHeight = currentHeight;
                        }


                        domUtils.remove(tmpNode)
                    }
            },50);
            me.addListener('destroy',function(){
                clearInterval(timer)
            });
            me.fireEvent('autoheightchanged', me.autoHeightEnabled);
        };
        me.disableAutoHeight = function (){
            var iframe = me.iframe,
                doc = me.document;
            iframe.scroll = bakScroll;
            doc.body.style.overflowY = bakOverflow;
            clearInterval(timer);
            me.autoHeightEnabled = false;
            me.fireEvent('autoheightchanged', me.autoHeightEnabled);
        };
        me.addListener( 'ready', function() {
            
            if(me.autoHeightEnabled){
              
                me.enableAutoHeight();
               
            }

        });
    }

})();
