using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    /// <summary>
    /// 本地日志管理类
    /// </summary>
    public class LogManager
    {
        private string filePath = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicleLogFilePath">日志文件的物理路径</param>
        public LogManager(string physicleLogFilePath)
        {
            this.filePath = physicleLogFilePath;
        }

        /// <summary>
        /// 添加日志内容(如果日志文件不存在则自动创建，否则在原有文件内容尾部追加日志)
        /// </summary>
        /// <param name="logContent"></param>
        public void Write(string newLogContent)
        {
            byte[] log = Encoding.Default.GetBytes(newLogContent);

            FileOperator.AppendContentToFile(log, this.filePath);
        }

        /// <summary>
        /// 清除日志内容 
        /// </summary>
        public void ClearLog()
        {
            byte[] log = Encoding.Default.GetBytes(string.Empty);

            FileOperator.SaveFile(log, this.filePath, true);
        }

        /// <summary>
        /// 删除日志文件
        /// </summary>
        public void DeleteLog()
        {
            if (File.Exists(this.filePath))
            {
                File.Delete(this.filePath);
            }
        }
    }
}
