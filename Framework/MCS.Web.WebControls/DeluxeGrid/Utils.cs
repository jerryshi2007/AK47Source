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
        /// ҳ����ʾģʽ
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        /// ҳ����ʾģʽ
        /// </remarks>
        public static PagerCodeShowMode ParsePageCodeShowMode(object o)
        {
            return ParsePageCodeShowMode(o, new PagerCodeShowMode());
        }

        /// <summary>
        /// ҳ����ʾģʽ
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// ҳ����ʾģʽ
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
                throw new FormatException("'" + o.ToString() + "' ���͸ı�ʧ��");
            }
        }          

        /// <summary>
        /// ����checkbox�е�λ��
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        ///  ����checkbox�е�λ��
        /// </remarks>
        public static RowPosition ParseRowPosition(object o)
        {
            return ParseRowPosition(o, new RowPosition());
        }

        /// <summary>
        /// ����checkbox�е�λ��
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// ����checkbox�е�λ�� 
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
                throw new FormatException("'" + o.ToString() + "' ���͸ı�ʧ��");
            }
        }

        /// <summary>
        /// ��ɫParse����
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// ��ɫParse����
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
