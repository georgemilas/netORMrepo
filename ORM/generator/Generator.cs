using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using EM.DB;
using EM.Collections;
using ORM;
using ORM.DBFields;
using System.Data;
using System.IO;
using ORM.db_store;
using System.Reflection;
using ORM.db_store.persitence;

namespace ORM.generator
{

    public class Generator : GeneratorBase
    {

        public Generator() : base() { }
        public Generator(GenericDatabase db): base(db) { }

        protected override void addStartClassFilePersist(TableName tn, StringBuilder s)
        {
            throw new NotSupportedException();
        }
        protected override void addConstructorPK(TableName table, PKInfo pk, TableColumnsWrap cw, StringBuilder s)
        {
            throw new NotSupportedException();
        }

        protected override void addDBOneToManyConstructors(TableName table, PKInfo pk, OrderedDictionary<TableName, ESet<DBRelation>> oneToMany, TableColumnsWrap cw, StringBuilder s)
        {
            throw new NotSupportedException();
        }
        protected override void addDBForeignKeysConstructors(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s)
        {
            throw new NotSupportedException();
        }
        protected override void addCustomSelectConstructors(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s, ESet<DBConstraint> customSelect, bool isView)
        {
            throw new NotImplementedException();
        }
        protected override void addUniqueConstraintConstructors(TableName table, EList<DBConstraint> constraints, TableColumnsWrap cw, StringBuilder s)
        {
            throw new NotSupportedException();
        }
        protected override void addCustomDeleteMethods(TableName table, FKInfo fk, TableColumnsWrap cw, StringBuilder s, ESet<DBConstraint> customDelete)
        {
            throw new NotImplementedException();
        }
    }
}
