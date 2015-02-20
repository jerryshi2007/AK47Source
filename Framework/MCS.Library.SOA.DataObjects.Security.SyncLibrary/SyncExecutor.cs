using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Locks;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    public class SyncExecutor
    {
        public event EventHandler<SyncEventArgs> BeforeExecute;
        public event EventHandler<SyncEventArgs> AfterExecute;

        public virtual void Execute(string planName)
        {
            SyncSession session = SyncSession.CreateSession(planName);

            SyncEventArgs e = new SyncEventArgs(session);

            OnBeforeExecute(e);
            SCDataOperationLockContext.Current.DoAddLockAction(true, "向权限中心同步", () => InnerExecute(session));

            OnAfterExecute(e);

        }

        public virtual void Execute(string planName, DataProviderBase dataProvider)
        {
            SyncSession session = SyncSession.CreateSession(planName, dataProvider);

            SyncEventArgs e = new SyncEventArgs(session);

            OnBeforeExecute(e);
            SCDataOperationLockContext.Current.DoAddLockAction(true, "向权限中心同步", () => InnerExecute(session));

            OnAfterExecute(e);
        }

        private void InnerExecute(SyncSession session)
        {
            session.SourceProvider.Reset(); //重置
            session.WriteStartLog(session);
            session.NumerOfErrors = session.NumerOfUpdated = 0;

            bool success = false;
            try
            {
                if (session.BatchSize != 1)
                {
                    this.DoBatchExecute(session);
                }
                else
                {
                    this.DoOneByOneExecute(session);
                }

                success = true;
            }
            finally
            {
                session.SourceProvider.Close();
                session.WriteEndLog(session, success);
            }
        }

        private void DoOneByOneExecute(SyncSession session)
        {
            DataProviderBase provider = session.SourceProvider;
            provider.Reset();

            while (provider.MoveNext())
            {
                string key = (string)provider.CurrentData[session.Mappings.SourceKeyProperty];
                if (string.IsNullOrWhiteSpace(key))
                    throw new SyncException("数据源提供的对象的ID为空");

                SchemaObjectBase scObj = LoadSchemaObject(key);
                CompareAndChange(session, provider.CurrentData, session.Mappings.AllObjectValues, scObj);
            }
        }

        private void DoBatchExecute(SyncSession session)
        {
            DataProviderBase provider = session.SourceProvider;
            provider.Reset();

            List<NameObjectCollection> buffer = new List<NameObjectCollection>(session.BatchSize);
            int len;
            do
            {
                buffer.Clear();
                len = FetchToBuffer(buffer, provider, session.BatchSize);
                if (len > 0)
                {
                    string[] keys = new string[len];
                    for (int i = buffer.Count - 1; i >= 0; i--)
                    {
                        keys[i] = (string)buffer[i][session.Mappings.SourceKeyProperty];
                        if (string.IsNullOrWhiteSpace(keys[i]))
                            throw new SyncException("数据源提供的对象的ID为空");
                    }

                    SchemaObjectCollection scObjs = LoadSchemaObjects(keys);

                    foreach (NameObjectCollection item in buffer)
                    {
                        string id = (string)item[session.Mappings.SourceKeyProperty];
                        SchemaObjectBase scObj = scObjs[id];
                        if (scObjs != null)
                        {
                            CompareAndChange(session, item, session.Mappings.AllObjectValues, scObj);
                        }
                    }
                }

            } while (len > 0 && len != session.BatchSize);
        }

        private SchemaObjectBase LoadSchemaObject(string key)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("Status", (int)SchemaObjectStatus.Normal);
            where.AppendItem("ID", key);

            return PC.Adapters.SchemaObjectAdapter.Instance.Load(where).FirstOrDefault();
        }

        private SchemaObjectCollection LoadSchemaObjects(string[] keys)
        {
            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");
            inBuilder.AppendItem(keys);

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

            return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(where, inBuilder));
        }

        private int FetchToBuffer(List<NameObjectCollection> buffer, DataProviderBase dataSource, int size)
        {
            int count = 0;
            while (count < size && dataSource.MoveNext())
            {
                count++;
                NameObjectCollection data = new NameObjectCollection(dataSource.CurrentData);
                buffer.Add(data);
            }

            return count;
        }

        private void CompareAndChange(SyncSession session, NameObjectCollection currentData, PropertyMapping[] mappings, SchemaObjectBase scObj)
        {
            if (scObj != null && scObj.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal)
            {
                PropertyMapping mapping;
                bool hasChange = false;
                bool propertyChanged;

                for (int i = 0; i < mappings.Length; i++)
                {
                    mapping = mappings[i];
                    propertyChanged = HasPropertyChange(session, session.Comparers[mapping.ComparerKey], mapping, currentData, scObj);
                    hasChange |= propertyChanged;
                    if (propertyChanged)
                    {
                        MakeChange(session, session.Setters[mapping.SetterKey], mapping, currentData, scObj);
                    }
                }

                if (hasChange)
                {
                    //完成
                    try
                    {
                        PC.Adapters.SchemaObjectAdapter.Instance.Update(scObj);
                        session.NumerOfUpdated++;
                        session.WriteUpdateLog(scObj);

                    }
                    catch (Exception ex)
                    {
                        session.NumerOfErrors++;
                        session.WriteErrorLog(scObj, ex);
                    }
                }
            }
        }

        private void MakeChange(SyncSession session, IPropertySetter setter, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            setter.SetValue(session, mapping, srcValues, targetObj);
        }

        private bool HasPropertyChange(SyncSession session, IPropertyComparer comparer, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj)
        {
            return comparer.AreEqual(session, mapping, srcValues, targetObj) == false;
        }


        protected virtual void OnAfterExecute(SyncEventArgs e)
        {
            if (this.AfterExecute != null)
                this.AfterExecute(this, e);
        }


        protected virtual void OnBeforeExecute(SyncEventArgs e)
        {
            if (this.BeforeExecute != null)
                this.BeforeExecute(this, e);
        }
    }

    public class SyncEventArgs : EventArgs
    {
        private SyncSession session;

        public SyncSession Session
        {
            get { return session; }
        }

        public SyncEventArgs(SyncSession session)
        {
            this.session = session;
        }
    }
}
