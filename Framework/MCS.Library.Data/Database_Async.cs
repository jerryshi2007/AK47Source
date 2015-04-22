using MCS.Library.Core;
using MCS.Library.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data
{
    public partial class Database
    {
        #region 异步Command and Stored Procedure Mechanism
        /// <summary>
        /// 根据Command对象指向存储过程获取其所需的参数组
        /// </summary>
        internal async Task DiscoverParametersAsync(DbCommand command)
        {
            if (command != null && command.CommandType == CommandType.StoredProcedure)
            {
                using (DbContext context = await DbContext.GetContextAsync(this.name))
                {
                    command.Connection = context.Connection;

                    using (DbCommand discoveryCommand = await CreateCommandByCommandTypeAsync(command))
                    {
                        discoveryCommand.Connection = command.Connection;
                        this.DeriveParameters(discoveryCommand);

                        foreach (IDataParameter parameter in discoveryCommand.Parameters)
                        {
                            IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                            command.Parameters.Add(cloneParameter);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取执行指定存储过程需要的Command实例
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">对应的一组参数赋值</param>
        /// <returns>Command实例</returns>
        /// <remarks>获取执行指定存储过程需要的Command实例，此时要求其参数匹配齐全否则内部的参数检查将报出异常。</remarks>
        public async virtual Task<DbCommand> InitStoredProcedureCommandAsync(string storedProcedureName, params object[] parameterValues)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(storedProcedureName), "storedProcedureName");

            DbCommand command = await CreateCommandByCommandTypeAsync(CommandType.StoredProcedure, storedProcedureName);

            await cache.SetParametersAsync(command, this);

            ExceptionHelper.FalseThrow<InvalidOperationException>(this.SameNumberOfParametersAndValues(command, parameterValues), "parameterValues");

            this.AssignParameterValues(command, parameterValues);

            return command;
        }

        /// <summary>
        /// 创建一个存储过程Command对象
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <returns>制定的Command对象</returns>
        /// <remarks>创建一个存储过程Command对象，仅仅用于创建而不做任何参数检查处理。</remarks>
        public async virtual Task<DbCommand> CreateStoredProcedureCommandAsync(string storedProcedureName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(storedProcedureName), "storedProcedureName");

            return await CreateCommandByCommandTypeAsync(CommandType.StoredProcedure, storedProcedureName);
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行数据更新操作(DML)
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <returns>受影响的行数</returns>
        public async virtual Task<int> ExecuteNonQueryAsync(DbCommand command)
        {
            return await this.DoExecuteNonQueryAsync(command);
        }

        /// <summary>
        /// 执行数据更新操作(DML)
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>受影响的行数</returns>
        public async virtual Task<int> ExecuteNonQueryAsync(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = await this.InitStoredProcedureCommandAsync(storedProcedureName, parameterValues))
            {
                return await ExecuteNonQueryAsync(command);
            }
        }

        /// <summary>
        /// 执行数据更新操作(DML)
        /// </summary>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>受影响的行数</returns>
        public async virtual Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText)
        {
            using (DbCommand command = await this.CreateCommandByCommandTypeAsync(commandType, commandText))
            {
                return await this.ExecuteNonQueryAsync(command);
            }
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行Command返回单值
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <returns>单值</returns>
        public async virtual Task<object> ExecuteScalarAsync(DbCommand command)
        {
            return await this.DoExecuteScalarAsync(command);
        }

        /// <summary>
        /// 执行存储过程返回单值
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>单值</returns>
        public async virtual Task<object> ExecuteScalarAsync(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = await this.InitStoredProcedureCommandAsync(storedProcedureName, parameterValues))
            {
                return await this.ExecuteScalarAsync(command);
            }
        }

        /// <summary>
        /// 执行指定查询返回单值
        /// </summary>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">SQL语句或者SPName，与commandType匹配使用</param>
        /// <returns>单值</returns>
        public async virtual Task<object> ExecuteScalarAsync(CommandType commandType, string commandText)
        {
            using (DbCommand command = await this.CreateCommandByCommandTypeAsync(commandType, commandText))
            {
                return await this.ExecuteScalarAsync(command);
            }
        }
        #endregion

        #region 异步ExecuteReader
        /// <summary>
        /// 返回一个DataReader对象
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>对于存储过程方式返回DataReader需包括有返回查询结果的情况</item>
        ///     <item>需要外部应用显示关闭Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="command">命令实例</param>
        /// <returns>DataReader对象</returns>
        public async virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command)
        {
            return await DoExecuteReaderAsync(command);
        }

        /// <summary>
        /// 返回一个DataReader对象
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>对于存储过程方式返回DataReader需包括有返回查询结果的情况</item>
        ///     <item>需要外部应用显示关闭Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>DataReader对象</returns>
        public async Task<DbDataReader> ExecuteReaderAsync(string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = await this.InitStoredProcedureCommandAsync(storedProcedureName, parameterValues))
            {
                return await ExecuteReaderAsync(command);
            }
        }

        /// <summary>
        /// 返回一个DataReader对象
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>对于存储过程方式返回DataReader需包括有返回查询结果的情况</item>
        ///     <item>需要外部应用显示关闭Reader</item>
        /// </list>
        /// </remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <returns>DataReader对象</returns>
        public async Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string commandText)
        {
            using (DbCommand command = await this.CreateCommandByCommandTypeAsync(commandType, commandText))
            {
                return await ExecuteReaderAsync(command);
            }
        }
        #endregion

        #region LoadDataSet Async
        /// <summary>
        /// 向DataSet中填充SQL返回的结果
        /// </summary>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        public async virtual Task LoadDataSetAsync(CommandType commandType,
            string commandText,
            DataSet dataSet,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            using (DbCommand command = await this.CreateCommandByCommandTypeAsync(commandType, commandText))
            {
                await LoadDataSetAsync(command, dataSet, pageNo, pageSize, tableNames);
            }
        }

        /// <summary>
        /// 向DataSet中填充SQL返回的结果
        /// </summary>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        public async virtual Task LoadDataSetAsync(CommandType commandType,
            string commandText,
            DataSet dataSet,
            params string[] tableNames)
        {
            using (DbCommand command = await this.CreateCommandByCommandTypeAsync(commandType, commandText))
            {
                await this.LoadDataSetAsync(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 向DataSet中填充存储过程返回的结果
        /// </summary>
        /// <remarks>如果是Oracle查询，需要在定义存储过程的时候把REF CURSOR放在其他参数的后面</remarks>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableName">查询结果的DataTable名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        public async virtual Task LoadDataSetAsync(string storedProcedureName,
            DataSet dataSet,
            string tableName,
            params object[] parameterValues)
        {
            await this.LoadDataSetAsync(storedProcedureName, dataSet, new string[] { tableName }, parameterValues);
        }

        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <remarks>如果是Oracle查询，需要在定义存储过程的时候把REF CURSOR放在其他参数的后面</remarks>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        public async virtual Task LoadDataSetAsync(string storedProcedureName,
            DataSet dataSet,
            string[] tableNames,
            params object[] parameterValues)
        {
            using (DbCommand command = await this.InitStoredProcedureCommandAsync(storedProcedureName, parameterValues))
            {
                await this.LoadDataSetAsync(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        public async virtual Task LoadDataSetAsync(DbCommand command,
            DataSet dataSet,
            params string[] tableNames)
        {
            await this.LoadDataSetAsync(command, dataSet, 0, 0, tableNames);
        }

        /// <summary>
        /// 向DataSet中填充Command返回的结果
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        public async virtual Task LoadDataSetAsync(DbCommand command,
            DataSet dataSet,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            await this.DoLoadDataSetAsync(command, dataSet, pageNo, pageSize, tableNames);
        }
        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// 返回存储过程查询结果
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>查询结果</returns>
        public async virtual Task<DataSet> ExecuteDataSetAsync(string storedProcedureName, params object[] parameterValues)
        {
            return await this.ExecuteDataSetAsync(storedProcedureName, new string[] { SystemCreatedTableNameRoot }, parameterValues);
        }

        /// <summary>
        /// 返回存储过程查询结果
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="parameterValues">存储过程参数赋值</param>
        /// <returns>查询结果</returns>
        public async virtual Task<DataSet> ExecuteDataSetAsync(string storedProcedureName, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = await this.InitStoredProcedureCommandAsync(storedProcedureName, parameterValues))
            {
                return await this.ExecuteDataSetAsync(command, tableNames);
            }
        }

        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        public async virtual Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText, params string[] tableNames)
        {
            return await this.ExecuteDataSetAsync(commandType, commandText, 0, 0, tableNames);
        }

        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="command">Command实例</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        public async virtual Task<DataSet> ExecuteDataSetAsync(DbCommand command, params string[] tableNames)
        {
            return await this.ExecuteDataSetAsync(command, 0, 0, tableNames);
        }

        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="commandType">Command类型</param>
        /// <param name="commandText">Command命令内容</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        /// <remarks>该方法Oracle不支持， 如果需要查询返回多个DataTable请将其写成存储过程</remarks>
        public async virtual Task<DataSet> ExecuteDataSetAsync(CommandType commandType,
            string commandText,
            int pageNo,
            int pageSize,
            params string[] tableNames)
        {
            using (DbCommand command = await this.CreateCommandByCommandTypeAsync(commandType, commandText))
            {
                return await this.ExecuteDataSetAsync(command, pageNo, pageSize, tableNames);
            }
        }

        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="command">Command实例</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <returns>查询结果</returns>
        public async virtual Task<DataSet> ExecuteDataSetAsync(DbCommand command, int pageNo, int pageSize, params string[] tableNames)
        {
            DataSet dataSet = new DataSet();

            dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;

            await this.LoadDataSetAsync(command, dataSet, pageNo, pageSize, tableNames);

            return dataSet;
        }
        #endregion

        #region 异步Do系列方法
        /// <summary>
        /// 异步执行Command返回单值
        /// </summary>
        /// <param name="command">Command实例</param>
        /// <returns>单值</returns>
        private async Task<object> DoExecuteScalarAsync(DbCommand command)
        {
            ExceptionHelper.TrueThrow<ArgumentException>(command.CommandType == CommandType.TableDirect,
                Resource.ExecuteScalarNotSupportTableDirectException);

            using (DbContext context = await DbContext.GetContextAsync(this.name))
            {
                command.Connection = context.Connection;

                DoDbEvent(command, DbEventType.BeforeExecution);

                object returnValue = await command.ExecuteScalarAsync();

                if (command.CommandType != CommandType.Text)
                {
                    // 由于 SQL Server 在Stored Procedure和Function返回值处理方式上存在不同，因此增加了适应性修改
                    returnValue = (returnValue == null) ? command.Parameters[DefaultReturnValueParameterName].Value : returnValue;
                }

                DoDbEvent(command, DbEventType.AfterExecution);

                return returnValue;
            }
        }

        private async Task<int> DoExecuteNonQueryAsync(DbCommand command)
        {
            using (DbContext context = await DbContext.GetContextAsync(this.name))
            {
                command.Connection = context.Connection;

                DoDbEvent(command, DbEventType.BeforeExecution);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                DoDbEvent(command, DbEventType.AfterExecution);

                return rowsAffected;
            }
        }

        private async Task<DbDataReader> DoExecuteReaderAsync(DbCommand command)
        {
            if (null == command.Connection)
            {
                DbContext context = await DbContext.GetContextAsync(this.name, false);
                command.Connection = context.Connection;
            }

            if (ConnectionState.Open != command.Connection.State)
                command.Connection.Open();

            DoDbEvent(command, DbEventType.BeforeExecution);

            DbDataReader reader = await command.ExecuteReaderAsync();

            DoDbEvent(command, DbEventType.AfterExecution);

            return reader;
        }

        /// <summary>
        /// 异步向DataSet中填充Command返回的结果
        /// </summary>
        /// <param name="command">Command实例(要求此时Command对象的Connection已经设置并初始化)</param>
        /// <param name="dataSet">待填充的DataSet</param>
        /// <param name="tableNames">每个查询结果的DataTable名称</param>
        /// <param name="pageNo">要求返回数据所在的页码【以0开始】</param>
        /// <param name="pageSize">要求返回数据每一页数据量【如果为零则表示所有数据】</param>
        private async Task DoLoadDataSetAsync(DbCommand command, DataSet dataSet, int pageNo, int pageSize, string[] tableNames)
        {
            CheckTableNames(tableNames);

            using (DbContext context = await DbContext.GetContextAsync(this.name))
            {
                command.Connection = context.Connection;

                using (DbDataAdapter adapter = this.factory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;

                    AddTableMappingsInAdapter(adapter, tableNames);

                    DoDbEvent(command, DbEventType.BeforeExecution);

                    if (pageSize == 0)
                        await adapter.FillAsync(dataSet);
                    else
                    {
                        string srcTableName = tableNames.Length > 0 ? tableNames[0] : "Table";
                        await adapter.FillAsync(dataSet, pageNo * pageSize, pageSize, srcTableName);
                    }

                    DoDbEvent(command, DbEventType.AfterExecution);
                }
            }
        }
        #endregion

        #region 异步Protected Helper Methods
        /// <summary>
        /// 生成简单的DbCommand对象 
        /// </summary>
        /// <remarks>
        ///     相当于Provider Identpendent的new()出一个Command对象
        /// </remarks>
        /// <param name="commandText">Command执行语句</param>
        /// <param name="commandType">Command类型</param>
        /// <returns>Command对象</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected async Task<DbCommand> CreateCommandByCommandTypeAsync(CommandType commandType, string commandText)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(string.IsNullOrEmpty(commandText), "commandText");

            using (DbContext context = await DbContext.GetContextAsync(this.name))
            {
                DbCommand command = this.factory.CreateCommand();

                command.CommandTimeout = (int)context.CommandTimeout.TotalSeconds;
                command.CommandType = commandType;
                command.CommandText = commandText;
                command.Connection = context.Connection;
                command.Transaction = context.LocalTransaction;

                return command;
            }
        }

        /// <summary>
        /// 生成简单的DbCommand对象 
        /// <remarks>
        ///     相当于Provider Identpendent的new()出一个Command对象
        /// </remarks>
        /// </summary>
        /// <param name="originalCommand">需要执行的数据库语句</param>
        /// <returns>Command对象</returns>
        protected async Task<DbCommand> CreateCommandByCommandTypeAsync(IDbCommand originalCommand)
        {
            return await this.CreateCommandByCommandTypeAsync(originalCommand.CommandType, originalCommand.CommandText);
        }
        #endregion
    }
}
