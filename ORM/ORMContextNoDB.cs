using System;
using System.Collections.Generic;
using System.Text;

using System.Web;
using System.Web.UI;


using ORM.render;
using ORM.exceptions;
using EM.Collections;
using EM.DB;
using System.Data;
//using System.Data.Common;
using System.Data.SqlClient;
using ORM.generator;
using ORM.db_store.persitence;

namespace ORM
{
    public class ORMContextNoDB: ORMContext
    {
        public ORMContextNoDB(): base()
        { }

        
        /////////////////////////////////////////////////////////////////////////////////
        /////////////// public interface
        /////////////////////////////////////////////////////////////////////////////////
        
        public override GenericDatabase db
        {
            get { return null; }
            set { throw new NotSupportedException("ORMContextNoDB objects do not support database assignement"); }            
        }
       
    }
}
