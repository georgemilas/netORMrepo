using System;
using System.Collections.Generic;
using System.Text;

namespace EM.DB
{
    //sql server: "SELECT SCOPE_IDENTITY()";
    //sql server (beware for insert triggers that insert themselfs somewhere): "SELECT [fld_id] FROM [SD_WHLS].[tbl_cust] WHERE [fld_id] = @@IDENTITY"
    //sql server: select IDENT_CURRENT('tbl_cust')  -> return last inserted in this table even if not yours
    //postgres: select curval(tbl_cust_id_seq)
    //mysql: select LAST_INSERT_ID()

    public class SQLServerLastInserIDProvider : ILastInsertIDProvider 
    {
        private string _lastInsertID = "SELECT SCOPE_IDENTITY()";
        public string lastInsertID 
        {
            get { return this._lastInsertID; }
            set { this._lastInsertID = value; }
        }
    }
}
