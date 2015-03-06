using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Security.AUClient;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 包装了管理单元角色的资源描述类
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfAURoleResourceDescriptor : WfResourceDescriptor, IWfCreateActivityParamsGenerator
    {
        public static readonly string AdministrativeUnitParameterName = "AdministrativeUnit";

        public static readonly WfAURoleResourceDescriptor EmptyInstance = new WfAURoleResourceDescriptor();

        private WrappedAUSchemaRole _AUSchemaRole = null;

        public WfAURoleResourceDescriptor()
        {
        }

        public WfAURoleResourceDescriptor(string roleFullCodeName)
        {
            this.RoleFullCodeName = roleFullCodeName;
        }

        public WfAURoleResourceDescriptor(WrappedAUSchemaRole schemaRole)
        {
            schemaRole.NullCheck("schemaRole");

            this._AUSchemaRole = schemaRole;
        }

        public string RoleFullCodeName
        {
            get;
            private set;
        }

        public WrappedAUSchemaRole AUSchemaRole
        {
            get
            {
                if (this._AUSchemaRole == null)
                {
                    if (this.RoleFullCodeName.IsNotEmpty())
                        this.AUSchemaRole = WrappedAUSchemaRole.FromCodeName(this.RoleFullCodeName);
                }

                return this._AUSchemaRole;
            }
            set
            {
                this._AUSchemaRole = value;
            }
        }

        public void Fill(WfCreateActivityParamCollection capc, PropertyDefineCollection definedCollection)
        {
            if (this._AUSchemaRole != null)
            {
                this.AUSchemaRole.DoCurrentRoleAction(this.ProcessInstance, (role, auCodeName) =>
                    role.FillCreateActivityParams(capc, definedCollection)
                );
            }
        }

        public bool UseCreateActivityParams
        {
            get
            {
                bool result = false;

                if (this._AUSchemaRole != null)
                {
                    this.AUSchemaRole.DoCurrentRoleAction(this.ProcessInstance, (role, auCodeName) =>
                        result = role.MatrixType == WfMatrixType.ActivityMatrix
                    );
                }

                return result;
            }
        }

        protected internal override void FillUsers(OguDataCollection<IUser> users)
        {
            if (this._AUSchemaRole != null)
            {
                this._AUSchemaRole.FillUsers(this.ProcessInstance, users);
            }
        }

        protected override void ToXElement(XElement element)
        {
            //throw new NotImplementedException();
        }

        public ClientAURole GetCurrentAUObject()
        {
            ClientAURole result = null;

            if (this._AUSchemaRole != null)
            {
                string auCodeName = WrappedAUSchemaRole.GetCurrentAdministrativeUnitCodeName(this.ProcessInstance);

                result = this._AUSchemaRole.GetAURoleObject(auCodeName);
            }

            return result;
        }
    }
}
