using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using System.Data;
using MCS.Library.Data.Builder;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace PermissionCenter.DataSources
{
    public class ProcessTemplateSource : DataViewDataSourceQueryAdapterBase
    {
        private class Section
        {
            public int startIndex, endIndex;
            public Section next = null;

            public Section(int startIndex, int endIndex, Section next)
            {
                this.startIndex = startIndex;
                this.endIndex = endIndex;
                this.next = next;
            }
        }

        string id = null;
        string[] additionIds = null;

        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            base.OnBuildQueryCondition(qc);
            qc.SelectFields = "D.PROCESS_KEY, D.APPLICATION_NAME, D.PROGRAM_NAME, D.PROCESS_NAME, D.ENABLED, D.CREATE_TIME, D.CREATOR_ID,D.CREATOR_NAME, D.MODIFY_TIME, D.MODIFIER_ID, D.MODIFIER_NAME, D.IMPORT_TIME, D.IMPORT_USER_ID, D.IMPORT_USER_NAME";
            qc.FromClause = " WF.PROCESS_DESCRIPTOR_DIMENSIONS DIM INNER JOIN WF.PROCESS_DESCRIPTORS D ON DIM.PROCESS_KEY = D.PROCESS_KEY";

            qc.WhereClause = string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : qc.WhereClause + " AND ";

            if (id == null)
                throw new InvalidExpressionException("至少需要指定一个ID");

            if (additionIds == null || additionIds.Length == 0)
            {
                qc.WhereClause += "DIM.Data.exist('Process/descendant::Resource[@ID=''" + TSqlBuilder.Instance.CheckQuotationMark(id, false) + "'']') > 0";
            }
            else
            {
                string[] filters = new string[additionIds.Length + 1];
                filters[0] = MakeFilter(id);
                for (int i = 0; i < additionIds.Length; i++)
                    filters[i + 1] = MakeFilter(additionIds[i]);

                qc.WhereClause += BuildWhere(filters); ;
            }

            if (string.IsNullOrEmpty(qc.OrderByClause))
                qc.OrderByClause = "D.APPLICATION_NAME ASC";
        }

        private string BuildWhere(string[] filters)
        {
            Section firstSection = FilterSection(filters, 0);
            Section previous = firstSection;
            int nextIndex = previous.endIndex + 1;

            while (nextIndex < filters.Length)
            {
                previous = previous.next = FilterSection(filters, nextIndex);
                nextIndex = previous.endIndex + 1;
            }

            StringBuilder builder = new StringBuilder();

            previous = firstSection;

            builder.Append("(");

            do
            {
                builder.Append("(DIM.Data.exist('Process/descendant::Resource[");
                for (int i = previous.startIndex; i <= previous.endIndex; i++)
                {
                    if (i != previous.startIndex)
                        builder.Append(" or ");
                    builder.Append(filters[i]);
                }

                builder.Append("]') > 0)");

                if (previous.next != null)
                    builder.Append(" OR ");

            } while ((previous = previous.next) != null);

            builder.Append(")");

            return builder.ToString();
        }

        private Section FilterSection(string[] filters, int i)
        {
            Section section = new Section(i, i, null);
            int length = 128;
            int lastIndex = i;
            do
            {
                length -= filters[i].Length + 4;
                if (length > 0)
                    lastIndex = i;
                i++;
            } while (length > 0 && i < filters.Length);

            section.endIndex = lastIndex;

            return section;
        }

        private string MakeFilter(string id)
        {
            return "@ID=''" + TSqlBuilder.Instance.CheckQuotationMark(id, false) + "''";
        }

        protected override string GetConnectionName()
        {
            return "HB2008";
        }

        public DataView Query(string id, int startRowIndex, int maximumRows, string where, string orderBy, ref int totalCount)
        {
            this.id = id;

            return this.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
        }

        public DataView Query(string id, string[] additionIds, int startRowIndex, int maximumRows, string where, string orderBy, ref int totalCount)
        {
            this.id = id;
            this.additionIds = additionIds;
            return this.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
        }

        public int GetQueryCount(string id, string where, ref int totalCount)
        {
            return this.GetQueryCount(where, ref totalCount);
        }

        public int GetQueryCount(string id, string[] additionIds, string where, ref int totalCount)
        {
            return this.GetQueryCount(where, ref totalCount);
        }
    }
}