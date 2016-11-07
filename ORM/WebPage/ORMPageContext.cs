using System;
using System.Data;
//using System.Data.Sql;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Text;
using EM.Collections;
//using System.IO;
//using System.Data.SqlClient;

namespace ORM.WebPage
{
    public class ORMPageContext : ORMContext
    {
        public Page page;
        //OrderedDictionary<string, string> fields 
        public static string getCSV(DataTable tb) { return getCSV(tb, null); }
        public static string getCSV(DataTable tb, OrderedDictionary<string, string> fields)
        {
            if (fields == null)
            {
                fields = new OrderedDictionary<string, string>();
                foreach (DataColumn c in tb.Columns)
                {
                    fields.Add(c.ColumnName, c.ColumnName);
                }
            }

            return CSV.toCsv(tb, fields);
            
        }
    }
   
}

