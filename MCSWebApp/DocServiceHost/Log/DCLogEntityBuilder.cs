using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Logging;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services.Log
{
    public class DCLogEntityBuilder
    {
        List<string> fileOperation;

        Dictionary<string, string> operationDictionary;

        public DCLogEntityBuilder()
        {
            fileOperation = new List<string>();
            fileOperation.Add("DCMGetFileContent");
            fileOperation.Add("DCMSetFileFields");
            fileOperation.Add("DCMCheckIn");
            fileOperation.Add("DCMCheckOut");
            fileOperation.Add("DCMUndoCheckOut");
            fileOperation.Add("DCMGetVersionFileContent");

            operationDictionary = new Dictionary<string, string>();
            operationDictionary["DCMGetFileContent"] = "下载文件";
            operationDictionary["DCMSetFileFields"] = "更新文件属性";
            operationDictionary["DCMCheckIn"] = "签入";
            operationDictionary["DCMCheckOut"] = "签出";
            operationDictionary["DCMUndoCheckOut"] = "撤销签出";
            operationDictionary["DCMGetVersionFileContent"] = "下载历史版本";
            operationDictionary["DCMDelete"] = "删除";
            operationDictionary["DCMCreateFolder"] = "创建文件夹";
            operationDictionary["DCMSave"] = "上传文件";
        }



        public LogEntity BuildEntity(string operationName, string user, object[] parameters)
        {
            if (!operationDictionary.ContainsKey(operationName))
                return null;
            if (fileOperation.Contains(operationName))
                return new LogEntity(operationName) { Title = operationDictionary[operationName], Message = string.Format("操作员:{0},文件Id:{1}", user, parameters[0]) };
            if (operationName == "DCMDelete")
            {
                if (parameters[0] is DCTFolder)
                    return new LogEntity(operationName) { Title = operationDictionary[operationName], Message = string.Format("操作员:{0},文件夹Id:{1},文件夹Uri:{2}", user, (parameters[0] as DCTStorageObject).ID, (parameters[0] as DCTStorageObject).Uri) };
                if (parameters[0] is DCTFile)
                    return new LogEntity(operationName) { Title = operationDictionary[operationName], Message = string.Format("操作员:{0},文件Id:{1},文件Uri:{2}", user, (parameters[0] as DCTStorageObject).ID, (parameters[0] as DCTStorageObject).Uri) };
            }
            if (operationName == "DCMCreateFolder")
                return new LogEntity(operationName) { Title = operationDictionary[operationName], Message = string.Format("操作员:{0},文件夹Id:{1},文件夹Uri:{2},新建文件夹名称:{3}", user, (parameters[1] as DCTStorageObject).ID, (parameters[1] as DCTStorageObject).Uri, parameters[0]) };
            if (operationName == "DCMSave")
                return new LogEntity(operationName) { Title = operationDictionary[operationName], Message = string.Format("操作员:{0},文件夹Id:{1},文件夹Uri:{2},文件名称:{3}", user, (parameters[0] as DCTStorageObject).ID, (parameters[0] as DCTStorageObject).Uri, parameters[2]) };
            return null;
        }
    }
}