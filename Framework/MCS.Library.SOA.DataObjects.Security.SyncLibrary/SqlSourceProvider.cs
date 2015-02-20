using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using System.Data;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    /// <summary>
    /// 表示SQL数据提供程序
    /// </summary>
    public class SqlSourceProvider : DataProviderBase
    {
        private string connectionName;
        private int batchSize;
        private string orderByColumn;
        private string queryString;
        private bool dataInitialized;
        private int numOfData; //记录总数
        private WrappedNameObjectCollection rowData = new WrappedNameObjectCollection();
        private int currentDataIndex;

        private int retrivedStartRow;
        private System.Data.DataTable dataTable;

        class WrappedNameObjectCollection : NameObjectCollection
        {
            public WrappedNameObjectCollection()
            {
                this.SetReadOnly(true);
            }

            internal void SetReadOnly(bool readOnly)
            {
                this.IsReadOnly = readOnly;
            }
        }

        public string ConnectionName
        {
            get { return connectionName; }
        }

        public SqlSourceProvider()
        {
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "SqlSourceProvider";

            if (string.IsNullOrEmpty(parameters["description"]))
            {
                parameters.Remove("description");
                parameters.Add("description", "SQL数据源提供程序");
            }

            base.Initialize(name, parameters);

            this.connectionName = parameters["connectionName"];
            if (string.IsNullOrEmpty(this.connectionName))
                throw new ProviderException("没有设置提供程序的配置的connectionName属性");

            string paraBatchSize = parameters["batchSize"];
            if (string.IsNullOrEmpty(paraBatchSize))
                this.batchSize = 0;
            else
            {
                int size = int.Parse(paraBatchSize);
                if (size < 0)
                    throw new ProviderException("batchSzie 的最小值是 0");
                this.batchSize = size;
            }

            this.orderByColumn = parameters["orderByColumnName"];
            this.queryString = parameters["queryString"];

            if (string.IsNullOrWhiteSpace(this.queryString))
                throw new ProviderException("queryString 属性不能为空");

            if (batchSize > 0 && string.IsNullOrWhiteSpace(this.orderByColumn))
                throw new ProviderException("当批次大小>0时，必须指定orderByColumnName属性且不得为空");

            parameters.Remove("connectionName");
            parameters.Remove("batchSize");
            parameters.Remove("orderByColumnName");
            parameters.Remove("queryString");

            if (parameters.Count > 0)
            {
                string key = parameters.GetKey(0);
                if (!string.IsNullOrEmpty(key))
                    throw new ProviderException(string.Format("不识别的属性： {0} ", key));
            }
        }

        public override void Reset()
        {
            this.dataInitialized = false;
        }

        public override bool MoveNext()
        {
            if (dataInitialized == false)
            {
                this.InitializeData();
            }

            int deltaIndex = currentDataIndex + 1;

            if (deltaIndex < numOfData)
            {
                if (IsDataRetrived(deltaIndex) == false)
                    RetriveData(deltaIndex);

                FillData(deltaIndex);

                currentDataIndex = deltaIndex;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FillData(int index)
        {
            if (this.dataTable != null && IsInRange(index))
            {
                int rowIdx = index + 1 - this.retrivedStartRow;
                this.rowData.SetReadOnly(false);
                this.rowData.Clear();
                foreach (DataColumn col in this.dataTable.Columns)
                {
                    this.rowData[col.ColumnName] = this.dataTable.Rows[rowIdx][col];
                }

                this.rowData.SetReadOnly(true);
            }
            else
            {
                throw new ProviderException("索引超出当前获取的记录行数");
            }
        }

        private void RetriveData(int index)
        {
            if (this.batchSize != 0)
            {
                int startRow = index + 1;
                int endRow = startRow + this.batchSize - 1;

                string sql = "SELECT * FROM (SELECT P.*,row_number() OVER (ORDER BY " + this.orderByColumn + " ASC) AS _RowNumber FROM (" + this.queryString + ") P) Q WHERE Q._RowNumber BETWEEN " + startRow + " AND " + endRow;

                this.dataTable = DbHelper.RunSqlReturnDS(sql, this.connectionName).Tables[0];

                if (this.dataTable.Rows.Count > 0)
                {
                    this.retrivedStartRow = (int)(long)dataTable.Rows[0]["_RowNumber"];
                }
                else
                {
                    throw new ProviderException("指定的索引未查询出任何数据");
                }

                this.dataTable.Columns.Remove("_RowNumber");
            }
            else
            {
                this.dataTable = DbHelper.RunSqlReturnDS(this.queryString, this.connectionName).Tables[0];
                this.retrivedStartRow = dataTable.Rows.Count > 0 ? 1 : 0;
            }
        }

        private bool IsDataRetrived(int index)
        {
            if (this.dataTable != null && IsInRange(index))
                return true;
            else
                return false;
        }

        private bool IsInRange(int index)
        {
            int rowNumber = index + 1;
            return this.retrivedStartRow <= rowNumber && this.dataTable.Rows.Count + this.retrivedStartRow > rowNumber;
        }

        private void InitializeData()
        {
            if (this.batchSize != 0)
            {
                string sql = "SELECT COUNT(1) FROM (" + this.queryString + ") Q";
                this.numOfData = (int)DbHelper.RunSqlReturnScalar(sql, this.connectionName);
                this.dataTable = null;
                this.retrivedStartRow = 0;
            }
            else
            {
                RetriveData(0);
                this.numOfData = this.dataTable.Rows.Count;
            }

            this.currentDataIndex = -1;
            this.rowData.SetReadOnly(false);
            this.rowData.Clear();
            this.rowData.SetReadOnly(true);
            this.dataInitialized = true;

        }

        public override NameObjectCollection CurrentData
        {
            get
            {
                if (this.dataInitialized && this.currentDataIndex >= 0 && this.currentDataIndex <= numOfData)
                    return this.rowData;
                else
                    return null;
            }
        }

        public override void Close()
        {
            this.Reset();
        }
    }
}
