using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ORM.db_store
{
    public class StoredProcParam
    {
        public string name;
        public string dbType;
        public bool hasDefault;
        public bool isNullable;
        public int position;
        public bool isOutputParam;
        public int characterOctetLength;
        public int characterMaxLength;
        public int numericPrecission;
        public int numericScale;

        //public StoredProcParam(string name, string dbType, bool hasDefault, bool isNullable, int position) :this(name, dbType, hasDefault, isNullable, position, false){ }
        public StoredProcParam(string name, string dbType, 
            bool hasDefault, bool isNullable, 
            int position, bool isOutputParam,
            int characterMaxLength, int characterOctetLength,
            int numericPrecission, int numericScale
            )
        {
            this.name = name;
            this.dbType = dbType;
            this.hasDefault = hasDefault;
            this.isNullable = isNullable;
            this.position = position;
            this.isOutputParam = isOutputParam;
            this.characterMaxLength = characterMaxLength;
            this.characterOctetLength = characterOctetLength;
            this.numericPrecission = numericPrecission;
            this.numericScale = numericScale;
        }        

    }
}
