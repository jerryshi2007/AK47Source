using MCS.Library.Core;
using MCS.Library.Data.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace MCS.Library.Data.Test.DBContextTest
{
    [TestClass]
    public class DbContextTest
    {
        private const string ConnectionName = "DataAccessTest";

        [TestMethod]
        public void CreateDbContextAsyncTest()
        {
            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Assert.AreEqual(ConnectionState.Open, context.Connection.State);
            }
        }

        [TestMethod]
        public void CascadeCreateDbContextAsyncTest()
        {
            Task.WaitAll(CascadeCreateDbContextAsync(), CascadeCreateDbContextAsync(), CascadeCreateDbContextAsync(), CascadeCreateDbContextAsync(), CascadeCreateDbContextAsync());
        }

        private static async Task CascadeCreateDbContextAsync()
        {
            using (DbContext contextOuter = await DbContext.GetContextAsync(ConnectionName))
            {
                CallContext.LogicalSetData("OriginalThreadID", Thread.CurrentThread.ManagedThreadId);

                //Console.WriteLine("Outer Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);
                int outerThreadID = Thread.CurrentThread.ManagedThreadId;

                using (DbContext contextInner = await DbContext.GetContextAsync(ConnectionName))
                {
                    object data = CallContext.LogicalGetData("OriginalThreadID");

                    if (data != null)
                        Console.WriteLine("Inner Thread ID: {0}, OuterThread ID: {1}, Get Original Thread ID: {2}",
                            Thread.CurrentThread.ManagedThreadId, outerThreadID, data);

                    //Assert.AreEqual(contextOuter.Connection, contextInner.Connection);
                }

                Assert.AreEqual(ConnectionState.Open, contextOuter.Connection.State);
            }
        }

        [TestMethod]
        public void CascadeTransactionScopeAndDbContextAsyncTest()
        {
            DbConnection connection = CascadeTransactionScopeAndDbContext().Result;

            Assert.AreEqual(ConnectionState.Closed, connection.State);
        }

        private static async Task<DbConnection> CascadeTransactionScopeAndDbContext()
        {
            DbConnection connection = null;

            using (TransactionScope scope = TransactionScopeFactory.CreateWithAsyncFlow())
            {
                using (DbContext context = await DbContext.GetContextAsync(ConnectionName))
                {
                    connection = context.Connection;
                }

                Assert.AreEqual(ConnectionState.Open, connection.State);

                scope.Complete();
            }

            return connection;
        }

        [TestMethod]
        public void ExecuteWaitForAsyncTest()
        {
            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                int count = database.ExecuteNonQueryAsync(CommandType.Text, "WAITFOR DELAY '000:00:01'").Result;

                Console.WriteLine(count);
            }
        }

        [TestMethod]
        public void ExecuteInsertUserAsyncTest()
        {
            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                int count = database.ExecuteNonQueryAsync(CommandType.Text,
                    PrepareUserInsert(UuidHelper.NewUuidString(), "沈峥", "Male")).Result;

                Console.WriteLine(count);

                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void ExecuteInsertTwoUsersAsyncTest()
        {
            StringBuilder strB = new StringBuilder();

            strB.Append(PrepareUserInsert(UuidHelper.NewUuidString(), "沈峥", "Male"));
            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
            strB.Append(PrepareUserInsert(UuidHelper.NewUuidString(), "沈嵘", "Male"));

            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    int count = database.ExecuteNonQueryAsync(CommandType.Text, strB.ToString()).Result;

                    scope.Complete();

                    Console.WriteLine(count);
                    Assert.AreEqual(2, count);
                }
            }
        }

        [TestMethod]
        public void ExecuteInsertTwoUsersNotCommitAsyncTest()
        {
            StringBuilder strB = new StringBuilder();

            string userID1 = UuidHelper.NewUuidString();

            string querySql = string.Format("SELECT COUNT(*) FROM Users WHERE UserID = {0}", TSqlBuilder.Instance.CheckUnicodeQuotationMark(userID1));

            strB.Append(PrepareUserInsert(userID1, "沈峥", "Male"));
            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
            strB.Append(PrepareUserInsert(UuidHelper.NewUuidString(), "沈嵘", "Male"));

            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                using (TransactionScope scope = TransactionScopeFactory.CreateWithAsyncFlow())
                {
                    int count = database.ExecuteNonQueryAsync(CommandType.Text, strB.ToString()).Result;
                    Assert.AreEqual(2, count);

                    int user1Count = (int)database.ExecuteScalarAsync(CommandType.Text, querySql).Result;
                    Console.WriteLine(user1Count);

                    Assert.AreEqual(1, user1Count);
                    //没有scope.complete
                }

                int user1CountOutTx = (int)database.ExecuteScalarAsync(CommandType.Text, querySql).Result;
                Console.WriteLine(user1CountOutTx);

                Assert.AreEqual(0, user1CountOutTx, "没有提交，返回0");
            }
        }

        [TestMethod]
        public void ExecuteDataSetAsyncTest()
        {
            ClearAllUserData();

            InsertUser(UuidHelper.NewUuidString(), "沈峥", "Male");
            InsertUser(UuidHelper.NewUuidString(), "沈嵘", "Male");

            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                DataSet dataSet = database.ExecuteDataSetAsync(CommandType.Text, "SELECT * FROM Users").Result;

                Console.WriteLine("Table Count: {0}, Row Count: {1}", dataSet.Tables.Count, dataSet.Tables[0].Rows.Count);
            }
        }

        private static void ClearAllUserData()
        {
            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                database.ExecuteNonQuery(CommandType.Text, "TRUNCATE TABLE Users");
            }
        }

        private static void InsertUser(string userID, string userName, string gender)
        {
            using (DbContext context = DbContext.GetContextAsync(ConnectionName).Result)
            {
                Database database = DatabaseFactory.Create(context);

                database.ExecuteNonQuery(CommandType.Text, PrepareUserInsert(userID, userName, gender));
            }
        }

        private static string PrepareUserInsert(string userID, string userName, string gender)
        {
            InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

            builder.AppendItem("UserID", userID);
            builder.AppendItem("UserName", userName);
            builder.AppendItem("Gender", gender);

            return string.Format("INSERT INTO Users{0}", builder.ToSqlString(TSqlBuilder.Instance));
        }
    }
}
