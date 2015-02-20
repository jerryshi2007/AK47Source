// -------------------------------------------------
// Assembly	：	MCS.Web.Responsive.WebControls
// FileName	：	IPageEventArgs.cs
// Remark	：  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		马泽锋	    20070815		创建
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 翻页事件接口
    /// </summary>
    /// <remarks>
    ///  翻页事件接口
    /// </remarks>
    public interface IPageEventArgs
    {
        /// <summary>
        /// 获取绑定分页控件对应控件的翻页事件Args
        /// </summary>
        /// <param name="controlMode">绑定的控件模型</param>
        /// <param name="eventName">事件名</param>
        /// <param name="commandSource">数据源对象</param>
        /// <param name="newPageIndex">当前的页码</param>
        /// <returns></returns>
        /// <remarks>
        ///  获取绑定分页控件对应控件的翻页事件Args
        /// </remarks>
        object GetPageEventArgs(DataListControlType  controlMode, string eventName, object commandSource, int newPageIndex);

        /// <summary>
        /// 设置绑定对应控件的分页属性
        /// </summary>
        /// <param name="objControl">控件对象</param>
        /// <param name="controlMode">对象模型</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        /// <remarks>
        ///  设置绑定对应控件的分页属性
        /// </remarks>
        bool SetBoundControlPagerSetting(object objControl, DataListControlType  controlMode, int pageSize);
    }
}
