using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow.Importers
{
    /// <summary>
    /// 流程的导入器
    /// </summary>
    public static class WfProcessImporter
    {
        public const int MaxStep = 6;

        public static List<IWfProcessDescriptor> ImportProcessDescriptors(Stream stream, Action<string> notifyEveryStep = null)
        {
            stream.NullCheck("stream");

            List<IWfProcessDescriptor> result = null;

            using (Package package = ZipPackage.Open(stream))
            {
                notifyEveryStep.Notify("开始导入文件...\n");

                PackagePartCollection packageParts = package.GetParts();

                WfProcessImporterContext context = new WfProcessImporterContext(packageParts);

                notifyEveryStep.Notify(string.Format("	共发现{0}个矩阵定义，{1}个流程矩阵数据，{2}个流程模板文件...\n",
                    context.MatrixDefParts.Count, context.MatrixParts.Count, context.ProcessDescParts.Count));

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    SaveMatrix(context);
                    notifyEveryStep.Notify("	导入流程矩阵完成...\n");

                    result = SaveProcessDescriptor(context);
                    notifyEveryStep.Notify("	导入流程模板完成...\n");

                    scope.Complete();
                }

                notifyEveryStep.Notify("文件导入完成!");
            }

            return result;
        }

        private static List<IWfProcessDescriptor> SaveProcessDescriptor(WfProcessImporterContext context)
        {
            List<IWfProcessDescriptor> result = new List<IWfProcessDescriptor>();

            XElementFormatter formatter = new XElementFormatter();
            formatter.OutputShortType = false;

            foreach (PackagePart part in context.ProcessDescParts)
            {
                XDocument xmlDoc = XDocument.Load(part.GetStream());
                IWfProcessDescriptor processDesp = (IWfProcessDescriptor)formatter.Deserialize(xmlDoc.Root);

                WfProcessDescriptorManager.SaveDescriptor(processDesp);

                IUser user = null;

                if (DeluxePrincipal.IsAuthenticated)
                    user = DeluxeIdentity.CurrentUser;

                WfProcessDescriptorInfoAdapter.Instance.UpdateImportTime(processDesp.Key, user);

                result.Add(processDesp);
            }

            return result;
        }

        private static void SaveMatrix(WfProcessImporterContext context)
        {
            foreach (var part in context.MatrixParts)
            {
                WfPackageRelationMapping partMapInfo = context.MappingInfo[part.Uri.ToString()];
                WfMatrixDefinition matrixDef = context.MatrixDefinitions[partMapInfo.MatrixDefID];

                WfMatrix.ImportNewMatrixFromExcel2007(part.GetStream(), null, partMapInfo.ProcessDescriptionID, matrixDef);
            }
        }

        private static void Notify(this Action<string> notifyEveryStep, string info)
        {
            if (notifyEveryStep != null)
                notifyEveryStep(info);
        }
    }
}
