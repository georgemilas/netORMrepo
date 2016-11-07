using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;


namespace EM.Collections
{
    public static class CSV
    {
        /*
        public static string toCsvLine(FileInfo[] arr) { return toCsvLine(arr, ", "); }
        public static string toCsvLine(FileInfo[] arr, string separator)
        {
            return toCsvLine(arr, separator, delegate(Object fo)
                { return ((FileInfo)fo).FullName; });  
        }
        */

        public delegate string GetStringDelegate(Object obj);

        /// <summary>
        /// returns a string in a CSV format for the list (columns) given 
        /// </summary>
        public static string toCsvLine(IEnumerable e) { return toCsvLine(e, ",", null, false); }
        public static string toCsvLine(IEnumerable e, bool quoteString) { return toCsvLine(e, ",", null, quoteString); }

        public static string toCsvLine(IEnumerable e, string separator) { return toCsvLine(e, separator, null, false); }
        public static string toCsvLine(IEnumerable e, string separator, bool quoteStrings) { return toCsvLine(e, separator, null, quoteStrings); }

        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string toCsvLine(IEnumerable e, string separator, GetStringDelegate getStrDel) { return toCsvLine(e, separator, getStrDel, false); }
        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string toCsvLine(IEnumerable e, string separator, GetStringDelegate getStrDel, bool quoteStrings)
        {
            if (separator == null) { separator = ","; }
            StringBuilder bres = new StringBuilder("");
            foreach (Object s in e)
            {
                string ss;
                if (getStrDel != null) 
                { 
                    ss = getStrDel(s); 
                }
                else 
                {
                    if (s is string && quoteStrings) 
                    {
                        string mys = (string)s;
                        if (mys.Contains("\"" + separator))
                        {
                            mys = mys.Replace("\"" + separator, "\" " + separator);
                        }
                        ss = "\"" + mys + "\"";
                    }
                    else 
                    {
                        ss =  s != null ? s.ToString() : ""; 
                    }
                }
                bres.Append(ss + separator);
            }
            string res = bres.ToString();
            if (res.EndsWith(separator))  //remove trailing separator
            {
                res = res.Substring(0, res.Length - separator.Length);
            }
            return res;
        }

        public static string toCsvLine(Object e, GetStringDelegate getStrDel)
        {
            return getStrDel(e);
        }

        /// <summary>
        /// returns a string in a CSV format for all rows and columns in the table
        /// </summary>
        public static string toCsv(DataTable tb) 
        {
            OrderedDictionary<string, string> fields = new OrderedDictionary<string, string>();
            foreach (DataColumn c in tb.Columns)
            {
                fields.Add(c.ColumnName, c.ColumnName);
            }
            return CSV.toCsv(tb, fields); 
        }

        /// <summary>
        /// returns a string in a CSV format for all rows and only the specified columns in the dictionary of {column name in table: column label in the CSV}
        /// </summary>
        public static string toCsv(DataTable tb, OrderedDictionary<string, string> fields)
        {
            StringBuilder sb = new StringBuilder();
            if (fields == null)
            {
                
            }
            sb.AppendLine(CSV.toCsvLine(fields.Values));
            OrderedDictionary<string, string>.OrderedKeysCollection keys = fields.Keys;
            sb.Append(CSV.toCsv(tb, keys));
            return sb.ToString();
        }

        /// <summary>
        /// returns a string in a CSV format for all rows and only the specified columns
        /// </summary>
        public static string toCsv(DataTable tb, IEnumerable<string> columnNames)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow r in tb.Rows)
            {
                EList<object> rs = new EList<object>();
                foreach (string col in columnNames)
                {
                    rs.Add(r[col]);
                }
                sb.AppendLine(CSV.toCsvLine(rs, ",", true));
            }

            return sb.ToString();
        }


        /// <summary>
        /// returns a string in a CSV format for the list (rows) of list (columns)
        /// </summary>
        public static string toCsv(IEnumerable<IEnumerable> e) { return toCsv(e, ",", null, false); }
        public static string toCsv(IEnumerable<IEnumerable> e, bool quoteString) { return toCsv(e, ",", null, quoteString); }

        public static string toCsv(IEnumerable<IEnumerable> e, string separator) { return toCsv(e, separator, null, false); }
        public static string toCsv(IEnumerable<IEnumerable> e, string separator, bool quoteStrings) { return toCsv(e, separator, null, quoteStrings); }

        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list of lists otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string toCsv(IEnumerable<IEnumerable> e, string separator, GetStringDelegate getStrDel) { return toCsv(e, separator, getStrDel, false); }
        /// <summary>
        /// if getStrDel is given will use that to stringify each object in the list of lists otherwise if getStrDel is not given then will just use object's ToString() method
        /// </summary>
        public static string toCsv(IEnumerable<IEnumerable> e, string separator, GetStringDelegate getStrDel, bool quoteStrings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IEnumerable le in e)
            {
                sb.AppendLine(CSV.toCsvLine(le, separator, getStrDel, quoteStrings));
            }
            return sb.ToString();

            
        }

        public static string toCsv(IEnumerable e, GetStringDelegate getStrDel)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Object le in e)
            {
                sb.AppendLine(CSV.toCsvLine(le, getStrDel));
            }
            return sb.ToString();
        }

        public static string CalendarAppointmentstoCsv(IEnumerable e, GetStringDelegate getStrDel)
        {
            StringBuilder sb = new StringBuilder();
            //add the standard calender header line
            sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\"","Subject","Start Date","Start Time","End Date","End Time","All day event","Reminder on/off","Reminder Date","Reminder Time","Meeting Organizer","Required Attendees","Optional Attendees","Meeting Resources","Billing Information","Categories","Description","Location","Mileage","Priority","Private","Sensitivity","Show time as"));
            foreach (Object le in e)
            {
                sb.AppendLine(CSV.toCsvLine(le, getStrDel));
            }
            return sb.ToString();
        }


        public static EList<string> fromCsvLine(string line)
        {
            return fromCsvLine(line, new List<char> { ',', '\t' }, '"');
        }
        public static EList<string> fromCsvLine(string line, List<char> separators, char? quote)
        {
            var sep = new Dictionary<char, int>();
            foreach (var s in separators) { sep[s] = 0; }

            //string[] lline = line.Trim().Split(new char[] { ',' });
            EList<string> res = new EList<string>();

            string itm = null;
            bool inItemValue = false;   //consider quoted items
            int cnt = -1;
            foreach(char c in line)
            {
                cnt += 1;
                if (sep.ContainsKey(c))  //a separator (',' '\t')
                {
                    if (!inItemValue)
                    {
                        res.Add(itm);
                        itm = null;
                    }
                    else
                    {
                        itm = itm == null ? c.ToString() : itm + c.ToString();    //separator (comma, tab) is part of itm
                    }
                }
                else if (quote != null && c == quote)  // '"'
                {
                    if (inItemValue)
                    {
                        //only finish itm if next char is a separator (comma, tab) or EOL otherwise " is part of itm
                        if (cnt == line.Length - 1 || sep.ContainsKey(line[cnt + 1]))
                        {
                            inItemValue = false;
                        }
                        else
                        {
                            itm = itm == null ? c.ToString() : itm + c.ToString();
                        }
                    }
                    else
                    {
                        inItemValue = true;  //not already in a value so start a value
                    }
                }
                else if (c == '\0')
                {
                    //ignore
                }
                else
                {
                    itm = itm == null ? c.ToString() : itm + c.ToString();  
                }                
            }
            if (itm != null)
            {
                res.Add(itm);
            }
            return res;
        }

        /// <summary>
        /// then return a collection indexable using integers indexes
        /// - see also parseWithHeader
        /// </summary>
        public static EList<EList<string>> parse(string filePath) { return parse(File.OpenText(filePath)); }
        public static EList<EList<string>> parse(StreamReader sr) { return parse(sr, line => line, new List<char> { ',', '\t' }, '"'); }

        /// <summary>
        /// then return a collection indexable using integers indexes
        /// - see also parseWithHeader
        /// </summary>
        public static EList<T> parse<T>(string filePath, Func<EList<string>, T> objMaker) { return parse<T>(File.OpenText(filePath), objMaker); }
        public static EList<T> parse<T>(StreamReader sr, Func<EList<string>, T> objMaker) { return parse(sr, objMaker, new List<char> { ',', '\t' }, '"'); }

        public static EList<T> parse<T>(string filePath, Func<EList<string>, T> objMaker, List<char> separators, char? quote) { return parse<T>(File.OpenText(filePath), objMaker, separators, quote); }
        public static EList<T> parse<T>(StreamReader sr, Func<EList<string>, T> objMaker, List<char> separators, char? quote)
        {
            EList<T> res = new EList<T>();
            try
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() != "")
                    {
                        res.Add(objMaker(CSV.fromCsvLine(line, separators, quote)));  //let errors propagate
                    }
                }
            }
            finally
            {
                sr.Close();
            }
            return res;
        }

        public static EList<EDictionary<string, string>> parseWithHeader(EList<EList<string>> parsedCSV)
        {
            return parseWithHeader<EDictionary<string, string>>(parsedCSV, (line) =>
            {
                var r = new EDictionary<string, string>();
                for (int i = 0; i < line.Count; i++)
                {
                    r[parsedCSV[0][i]] = line[i];
                }
                return r;
            });
        }
        /// <summary>
        /// using a parsed collection and considering the first line in the file to be a header line
        /// then return a collection indexable using name instead on just integers indexes
        ///   ex:    csv[1]["invoice"], csv[2]["invoice"] instead of
        ///          csv[1][0], csv[2][0]    
        /// </summary>
        public static EList<T> parseWithHeader<T>(EList<EList<string>> parsedCSV, Func<EList<string>, T> objMaker) 
        {
            EList<T> res = new EList<T>();

            if (parsedCSV.Count < 1)
            {
                return new EList<T>();
            }

            if (parsedCSV.Count == 1)
            {
                EList<string> nulls = new EList<string>();
                for (int i = 0; i < parsedCSV[0].Count; i++)
                {
                    nulls.Add(null);
                }
                T d = objMaker(nulls);  //return an object with all values as null
                res.Add(d);
                
            }
            else
            {
                for (int i = 1; i < parsedCSV.Count; i++)
                {
                    T d = objMaker(parsedCSV[i]);
                    res.Add(d);
                }
            }

            return res;
        }


    }

}
