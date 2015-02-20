using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    public class FileOperator
    {
        /// <summary>
        /// 保存文件指文件系统
        /// </summary>
        /// <param name="content">文件内容</param>
        /// <param name="physicalSavePath">包含文件路径和文件名称的完整保存路径</param>
        /// <param name="overWrite">是否覆盖相同名称的文件</param>
        public static void SaveFile(byte[] content, string physicalSavePath, bool overWrite)
        {
            //获取文件路径和文件名称
            string dictionary = physicalSavePath.Substring(0, physicalSavePath.LastIndexOf(@"\"));
            string fileName = physicalSavePath.Substring(physicalSavePath.LastIndexOf(@"\") + 1);

            //保存至文件系统
            if (!Directory.Exists(dictionary))
            {
                Directory.CreateDirectory(dictionary);
            }

            if (!File.Exists(physicalSavePath) || (File.Exists(physicalSavePath) && overWrite))
            {
                FileStream fs = null;

                try
                {
                    fs = new FileStream(physicalSavePath, FileMode.Create);
                    fs.Write(content, 0, content.Length);
                    fs.Flush();
                }
                catch
                {

                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                        fs = null;
                    }
                }
            }
        }

        /// <summary>
        /// 保存文件指文件系统
        /// </summary>
        /// <param name="content">文件内容</param>
        /// <param name="physicalSavePath">包含文件路径和文件名称的完整保存路径</param>
        public static void SaveFile(byte[] content, string physicalSavePath)
        {
            FileOperator.SaveFile(content, physicalSavePath, true);
        }

        /// <summary>
        /// 添加内容至文件尾部，如果文件不存在则自动创建
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="physicalSavePath">文件的物理路径</param>
        public static void AppendContentToFile(byte[] content, string physicalSavePath)
        {
            //获取文件路径和文件名称
            string dictionary = physicalSavePath.Substring(0, physicalSavePath.LastIndexOf(@"\"));
            string fileName = physicalSavePath.Substring(physicalSavePath.LastIndexOf(@"\") + 1);

            //保存至文件系统
            if (!Directory.Exists(dictionary))
            {
                Directory.CreateDirectory(dictionary);
            }

            //保存至文件系统
            FileStream fs = null;

            try
            {
                fs = new FileStream(physicalSavePath, FileMode.Append, FileAccess.Write);
                fs.Write(content, 0, content.Length);
                fs.Flush();
            }
            catch
            {

            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="physicalSavePath">包含文件路径和文件名称的完整保存路径</param>
        public static void DeleteFile(string physicalSavePath)
        {
            if (File.Exists(physicalSavePath))
            {
                File.Delete(physicalSavePath);
            }
        }

        /// <summary>
        /// 指定的文件时否存在
        /// </summary>
        /// <param name="physicalSavePath">包含文件路径和文件名称的完整保存路径</param>
        /// <returns></returns>
        public static bool FileExists(string physicalSavePath)
        {
            return File.Exists(physicalSavePath);
        }

        /// <summary>
        /// 获取指定的文件信息
        /// </summary>
        /// <param name="physicalSavePath">包含文件路径和文件名称的完整保存路径</param>
        /// <returns></returns>
        public static FileInfo GetFile(string physicalSavePath)
        {
            return new FileInfo(physicalSavePath);
        }
    }
}
