using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// 定义页面或控件输出ContentType的关键字
    /// </summary>
    public enum ResponseContentTypeKey
    {
        #region Define Content
        /// <summary>
        /// MSWord类型
        /// </summary>
        WORD,

        /// <summary>
        /// MSExcel类型
        /// </summary>
        EXCEL,

        /// <summary>
        /// HTML类型
        /// </summary>
        HTML,

        /// <summary>
        /// BMP图片
        /// </summary>
        BMP,

        /// <summary>
        /// GIF图片
        /// </summary>
        GIF,

        /// <summary>
        /// GD文件
        /// </summary>
        GD,

        /// <summary>
        /// GW，GW2文件
        /// </summary>
        GW,

        /// <summary>
        /// SPD文件
        /// </summary>
        SPD,

        /// <summary>
        /// SEP文件
        /// </summary>
        SEP,

        /// <summary>
        /// RM文件
        /// </summary>
        RM,

        /// <summary>
        /// RMVB文件
        /// </summary>
        RMVB,

        /// <summary>
        /// RA文件
        /// </summary>
        RA,

        /// <summary>
        /// SEDS92文件
        /// </summary>
        SEDS92,

        /// <summary>
        /// JPEG文件
        /// </summary>
        JPEG,

        /// <summary>
        /// MP3文件
        /// </summary>
        MP3,

        /// <summary>
        /// MPEG文件
        /// </summary>
        MPEG,

        /// <summary>
        /// MHTML文件
        /// </summary>
        MHTML,

        /// <summary>
        /// PDF文件
        /// </summary>
        PDF,

        /// <summary>
        /// PNG文件
        /// </summary>
        PNG,

        /// <summary>
        /// PPT文件
        /// </summary>
        PPT,

        /// <summary>
        /// PostScript文件
        /// </summary>
        PostScript,

        /// <summary>
        /// Text文件
        /// </summary>
        Text,

        /// <summary>
        /// ///文件
        /// </summary>
        Tiff,

        /// <summary>
        /// Visio文件
        /// </summary>
        Visio,

        /// <summary>
        /// WMD文件
        /// </summary>
        WMD,

        /// <summary>
        /// WMP文件
        /// </summary>
        WMP,

        /// <summary>
        /// WMA文件
        /// </summary>
        WMA,

        /// <summary>
        /// WMZ文件
        /// </summary>
        WMZ,

        /// <summary>
        /// WMV文件
        /// </summary>
        WMV,

        /// <summary>
        /// AVI文件
        /// </summary>
        AVI,

        /// <summary>
        /// ICO文件
        /// </summary>
        ICO,

        /// <summary>
        /// XML文件
        /// </summary>
        XML,

        /// <summary>
        /// BIN文件
        /// </summary>
        BIN
        #endregion
    }

    /// <summary>
    /// HttpResponse 返回流 客户端打开类型
    /// </summary>
    public enum ResponseDispositionType
    {
        #region Define Conent
        /// <summary>
        /// 未定义
        /// </summary>
        Undefine = 0,

        /// <summary>
        /// 在浏览器内打开
        /// </summary>
        InnerBrowser = 1,

        /// <summary>
        /// 不提示用户直接打开文件
        /// </summary>
        Inline = 2,

        /// <summary>
        /// 提示用户，用户选择打开文件、保存文件或取消
        /// </summary>
        Attachment = 3
        #endregion
    }
}
