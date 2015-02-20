// -------------------------------------------------
// Assembly	：	MCS.Web.Responsive.WebControls
// FileName	：	IPagerBoundControlType.cs
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
    /// 页码绑定类型接口类
    /// </summary>
    /// <remarks>
    ///  页码绑定类型接口类
    /// </remarks>
    public interface IPagerBoundControlType
    {
        /// <summary>
        /// 获取当前绑定的控件的状态对象
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        /// <remarks>
        ///  获取当前绑定的控件的状态对象
        /// </remarks>
        PagerBoundControlStatus GetPagerBoundControl(Type controlType); 
    }
}
