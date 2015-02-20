using CIIC.HSR.TSP.Resource.Common;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Common.Metrix;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using MCS.Library.WF.Contracts.Workflow.Builders;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 业务流程描述转换器
    /// </summary>
    public class WfMatrixDescriptorTransformation : IWfMatrixDescriptorTransformation
    {
        public static readonly WfMatrixDescriptorTransformation Instance = new WfMatrixDescriptorTransformation();

        private WfMatrixDescriptorTransformation()
        {
        }

        /// <summary>
        /// 将业务描述转换为流程引擎的流程描述
        /// </summary>
        /// <param name="process">业务流程描述</param>
        /// <returns>流程引擎流程描述</returns>
        public WfClientProcessDescriptor Transform(IWfMatrixProcess process)
        {
            WfClientProcessDescriptor clientDescriptor = null;
            WfCreateClientDynamicProcessParams paras = new WfCreateClientDynamicProcessParams();
            WfClientActivityMatrixResourceDescriptor resource = new WfClientActivityMatrixResourceDescriptor();
            paras.ActivityMatrix = resource;

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.Auto;
            string bizProcessData = JsonConvert.SerializeObject(process, jss);
            if (process.Properties.ContainsKey(Consts.ProcessSavedKey))
            {
                process.Properties[Consts.ProcessSavedKey] = bizProcessData;
            }
            else
            {
                process.Properties.Add(Consts.ProcessSavedKey, bizProcessData);
            }


            FillProcessInfo(process, paras);
            FillActivityInfo(process.Activities, resource);

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(paras);
            clientDescriptor = builder.Build(paras.Key, paras.Name);

            return clientDescriptor;
        }

        /// <summary>
        /// 从流程引擎中找回流程描述
        /// </summary>
        /// <param name="clientProcessDescriptor">流程Key</param>
        /// <returns>流程描述</returns>
        public IWfMatrixProcess TransformBack(WfClientProcessDescriptor clientProcessDescriptor)
        {
            string serializedInfo = clientProcessDescriptor.Properties[Consts.ProcessSavedKey].StringValue;

            if (string.IsNullOrEmpty(serializedInfo))
            {
                return null;
            }

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.Auto;

            return JsonConvert.DeserializeObject<WfMatrixProcess>(serializedInfo, jss);
        }
        /// <summary>
        /// 转换流程相关信息
        /// </summary>
        /// <param name="process">业务流程描述</param>
        /// <param name="processParams">流程参数构造器</param>
        private static void FillProcessInfo(IWfMatrixProcess process, WfCreateClientDynamicProcessParams processParams)
        {
            if (string.IsNullOrEmpty(process.Key))
            {
                throw new Exception(Message.EmptyProcessKeyIsNotAllowed);
            }

            processParams.ApplicationName = process.ApplicationName;
            processParams.AutoGenerateResourceUsers = true;
            processParams.Description = process.Description;
            processParams.Enabled = true;
            processParams.Key = process.Key;
            processParams.Name = process.Name;
            processParams.ProcessType = WfClientProcessType.Approval;
            processParams.ProgramName = process.ProgramName;
            processParams.Url = process.Url;

            if (process.Properties.ContainsKey(Consts.DefaultTaskTitle))
            {
                processParams.DefaultTaskTitle = process.Properties[Consts.DefaultTaskTitle].ToString();
            }

            if (process.Properties.ContainsKey(Consts.DefaultNotifyTaskTitle))
            {
                processParams.DefaultNotifyTaskTitle = process.Properties[Consts.DefaultNotifyTaskTitle].ToString();
            }

            if (process.Properties.ContainsKey(Consts.DefaultReturnValue))
            {
                processParams.DefaultReturnValue = (bool)process.Properties[Consts.DefaultReturnValue];
            }



            if (null != process.Properties)
            {
                var otherProperties = process.Properties.ToList().Where(p =>
                    !p.Key.Equals(Consts.DefaultTaskTitle, StringComparison.CurrentCultureIgnoreCase)
                    && !p.Key.Equals(Consts.DefaultNotifyTaskTitle, StringComparison.CurrentCultureIgnoreCase)
                    && !p.Key.Equals(Consts.DefaultReturnValue, StringComparison.CurrentCultureIgnoreCase)
                     );

                otherProperties.ToList().ForEach(p =>
                    processParams.Properties.AddOrSetValue(p.Key, p.Value)
                );
            }
        }
        /// <summary>
        /// 转换节点信息
        /// </summary>
        /// <param name="matrixActivity">业务节点描述</param>
        /// <param name="activityDescriptor">流程引擎节点描述</param>
        private void FillActivityInfo(IWfMatrixActivityCollection matrixActivity, WfClientActivityMatrixResourceDescriptor activityDescriptor)
        {
            #region 添加列定义
            WfClientRolePropertyDefinitionCollection propertiesDefinition = new WfClientRolePropertyDefinitionCollection();

            propertiesDefinition.Add(new WfClientRolePropertyDefinition()
            {
                Name = Consts.ActivitySN,
                SortOrder = 0,
                Description = Label.ActivitySN
            });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition()
            {
                Name = Consts.Operator,
                SortOrder = 1,
                Description = Label.Operator
            });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition()
            {
                Name = Consts.OperatorType,
                SortOrder = 2,
                Description = Label.OperatorType
            });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition()
            {
                Name = Consts.IsMergeable,
                SortOrder = 3,
                Description = Label.IsMergeable
            });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition()
            {
                Name = Consts.Condition,
                SortOrder = 4,
                Description = Label.Condition
            });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition()
            {
                Name = Consts.ActivityProperties,
                SortOrder = 5,
                Description = Label.ActivityProperties
            });

            activityDescriptor.PropertyDefinitions.CopyFrom(propertiesDefinition);
            #endregion
            #region 添加矩阵行数据
            WfClientRolePropertyRowCollection rows = new WfClientRolePropertyRowCollection();
            if (null != matrixActivity)
            {
                matrixActivity.ToList().ForEach(p =>
                {
                    p.Candidates.ToList().ForEach(c =>
                    {
                        WfClientRolePropertyRow mRow = new WfClientRolePropertyRow() { RowNumber = 1 };
                        mRow.IsMergeable();
                        mRow.Operator = c.ToExpression();
                        mRow.OperatorType = GetResourceType(c.ResourceType);

                        foreach (var dimension in propertiesDefinition)
                        {
                            WfClientRolePropertyValue mCell = new WfClientRolePropertyValue(dimension);

                            if (dimension.Name.Equals(Consts.Operator, StringComparison.CurrentCultureIgnoreCase))
                            {
                                mCell.Value = c.Candidate.Name;
                            }
                            else if (dimension.Name.Equals(Consts.OperatorType, StringComparison.CurrentCultureIgnoreCase))
                            {
                                mCell.Value = mRow.OperatorType.ToString();
                            }
                            else if (dimension.Name.Equals(Consts.ActivitySN, StringComparison.CurrentCultureIgnoreCase))
                            {
                                mCell.Value = 1.ToString();
                            }
                            else if (dimension.Name.Equals(Consts.Condition, StringComparison.CurrentCultureIgnoreCase))
                            {
                                mCell.Value = p.Expression.ToExpression();
                            }
                            else if (dimension.Name.Equals(Consts.IsMergeable, StringComparison.CurrentCultureIgnoreCase))
                            {
                                mCell.Value = true.ToString();
                            }
                            else if (dimension.Name.Equals(Consts.ActivityProperties, StringComparison.CurrentCultureIgnoreCase))
                            {
                                mCell.Value = JsonConvert.SerializeObject(p.Properties);
                            }
                            mRow.Values.Add(mCell);
                        }

                        rows.Add(mRow);
                    });
                });

                activityDescriptor.Rows.CopyFrom(rows);
            }
            #endregion
        }
        /// <summary>
        /// 获取资源类型
        /// </summary>
        /// <param name="resourceType">资源类型编码</param>
        /// <returns>流程引擎类型</returns>
        private WfClientRoleOperatorType GetResourceType(string resourceType)
        {
            WfClientRoleOperatorType roleType = WfClientRoleOperatorType.User;
            if (!string.Equals(resourceType, ResourceType.User))
            {
                roleType = WfClientRoleOperatorType.Role;
            }
            return roleType;
        }
    }
}
