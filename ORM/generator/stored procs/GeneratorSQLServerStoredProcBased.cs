using System;
using System.Collections.Generic;
using System.Text;
using ORM.db_store.persitence;

namespace ORM.generator
{
    public class GeneratorSQLServerStoredProcBased: GeneratorStoredProcBased
    {
        private SQLServerStoredProcsGenerator spg;

        public GeneratorSQLServerStoredProcBased(GenericDatabase db)
            : base(db)
        {
            this.spg = new SQLServerStoredProcsGenerator();
        }


        public override IStoredProcsGenerator gsp
        {
            get 
            {
                this.spg.customizeFromOtherGenerator(this);
                return this.spg; 
            }
        }

    }
}
