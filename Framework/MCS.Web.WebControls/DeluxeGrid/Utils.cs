using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Threading;

namespace MCS.Web.WebControls
{
    internal class Utils
    {
        /// <summary>
        /// 页码显示模式
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        /// 页码显示模式
        /// </remarks>
        public static PagerCodeShowMode ParsePageCodeShowMode(object o)
        {
            return ParsePageCodeShowMode(o, new PagerCodeShowMode());
        }

        /// <summary>
        /// 页码显示模式
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// 页码显示模式
        /// </remarks>
        public static PagerCodeShowMode ParsePageCodeShowMode(object o, PagerCodeShowMode defaultValue)
        {
            if (o == null || o.ToString() == "")
            {
                return defaultValue;
            }
            try
            {
                return (PagerCodeShowMode)Enum.Parse(typeof(PagerCodeShowMode), o.ToString(), true);
            }
            catch
            {
                throw new FormatException("'" + o.ToString() + "' 类型改变失败");
            }
        }          

        /// <summary>
        /// 设置checkbox列的位置
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        ///  设置checkbox列的位置
        /// </remarks>
        public static RowPosition ParseRowPosition(object o)
        {
            return ParseRowPosition(o, new RowPosition());
        }

        /// <summary>
        /// 设置checkbox列的位置
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// 设置checkbox列的位置 
        /// </remarks>
        public static RowPosition ParseRowPosition(object o, RowPosition defaultValue)
        {
            if (o == null || o.ToString() == "")
            {
                return defaultValue;
            }
            try
            {
                return (RowPosition)Enum.Parse(typeof(RowPosition), o.ToString(), true);
            }
            catch
            {
                throw new FormatException("'" + o.ToString() + "' 类型改变失败");
            }
        }

        /// <summary>
        /// 颜色Parse方法
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// 颜色Parse方法
        /// </remarks>
        public static Color ParseColor(object o, Color defaultValue)
        {
            if (o == null || o.ToString() == "")
            {
                return defaultValue;
            }
            try
            {
                return ColorTranslator.FromHtml(o.ToString());
            }
            catch
            {
                throw new FormatException("'" + o.ToString() + "' can not be parsed as a color.");
            }
        }
    }
}
