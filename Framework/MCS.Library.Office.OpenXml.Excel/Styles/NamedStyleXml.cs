using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel.Styles;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class NamedStyleXmlWrapper : StyleXmlBaseWrapper
    {
        private WorkBookStylesWrapper _Styles;

        internal NamedStyleXmlWrapper(WorkBookStylesWrapper styles)
        {
            this._Styles = styles;
            this.BuildInId = int.MinValue;
        }

        internal NamedStyleXmlWrapper(int styleXfId, string name, WorkBookStylesWrapper styles) 
        {
            this.StyleXfId = styleXfId;
            this.Name = name;
             //todo:
            //this.BuildInId = GetXmlNodeInt(BuildInIdPath);
            this._Styles = styles;
            //this._Style = new StyleWrapper(styles, styles.NamedStylePropertyChange, -1, this.Name, this._StyleXfId);
        }

        internal override string Id
        {
            get
            {
                return Name;
            }
        }

        private int _StyleXfId = 0;
        private const string idPath = "@xfId";
        public int StyleXfId
        {
            get
            {
                return this._StyleXfId;
            }
            set
            {
                this._StyleXfId = value;
            }
        }

        private int _XfId = int.MinValue;
        internal int XfId
        {
            get
            {
                return this._XfId;
            }
            set
            {
                this._XfId = value;
            }
        }

        private const string BuildInIdPath = "@builtinId";
        public int BuildInId { get; set; }
        
        private const string namePath = "@name";
        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            internal set
            {
                this._Name = value;
            }
        }
    }
}
