using System;
using ORM.db_store;
using System.Collections.Generic;
using System.Text;

namespace ORM.generator
{
    public interface IStoredProcsGenerator
    {
        void addStoreProcInit(TableName table, StringBuilder s);
        void buildSPCountAll(TableName table);
        void buildSPDeleteByPK(TableName table, TableColumnsWrap cw, PKInfo pk);
        TableName buildSPDeleteByCustomFields(TableName table, TableColumnsWrap cw, DBConstraint constraint);
        void buildSPInsert(TableName table, TableColumnsWrap cw, PKInfo pk);
        void buildSPLoadByPK(TableName table, TableColumnsWrap cw, PKInfo pk);
        TableName buildSPLoadDBRelation(TableName table, List<string> loadFields, bool isSelect);
        void buildSPUpdateByPK(TableName table, TableColumnsWrap cw, PKInfo pk);
        void buildSPDropAllProcs();
    }
}
