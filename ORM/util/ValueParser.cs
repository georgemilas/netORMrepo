using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using System.IO;

namespace ORM.Util
{
    public class ValueParser
    {
        protected delegate object ValParser(string val, string msg);

        public int csvLineIndex;
        public EList<string> csvLine;
        public string csvLineString;
        public Dictionary<string, int> fileMap;

        public ValueParser(int ix, EList<string> csvLine, Dictionary<string, int> fileMap)
        {
            this.csvLineIndex = ix;
            this.csvLine = csvLine;
            this.fileMap = fileMap;
            this.csvLineString = CSV.toCsvLine(csvLine, ", ", delegate(object o) { string s = (string)o; return s != null ? s : ""; }, true);
        }

        public object parse(bool required, string msg, Type parseType) { return parse(required, msg, parseType, null); }
        /// <summary>
        /// parse value at column ix as parseType 
        ///     - if null or inexistent index and if defaultValue is given, return it even if required
        /// </summary>
        public object parse(bool required, string columnName, Type parseType, object defaultValue)
        {
            return parse(required, fileMap[columnName], columnName, parseType, defaultValue);
        }
        public object parse(bool required, int columnIndexInFile, string columnName, Type parseType, object defaultValue)
        {
            object parsed;
            string val;
            int ix = columnIndexInFile;

            try
            {
                val = csvLine[ix];
            }
            catch (ArgumentOutOfRangeException e)       //column does not exist in this line
            {
                if (!required)
                {
                    return null;
                }
                else
                {
                    if (defaultValue == null)
                    {
                        throw new InvalidDataException(string.Format("a value is required for {0} in the folowing line (line {2}:     {1})", columnName, csvLineString, csvLineIndex.ToString()));
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }

            ValParser parser = null;

            if (parseType == typeof(int)) { parser = this.int1; }
            if (parseType == typeof(string)) { parser = null; }
            if (parseType == typeof(decimal)) { parser = this.dec1; }
            if (parseType == typeof(DateTime)) { parser = this.datetime1; }
            if (parseType == typeof(bool)) { parser = this.bool1; }
            if (parseType == typeof(double)) { parser = this.double1; }

            if (required && (val == null || val.Trim() == ""))
            {
                if (defaultValue == null)
                {
                    throw new InvalidDataException(string.Format("a value is required for {0} in the folowing line (line {2}:     {1})", columnName, csvLineString, csvLineIndex.ToString()));
                }
                else
                {
                    return defaultValue;
                }
            }
            if (!required && (val == null || val.Trim() == ""))
            {
                return null;
            }
            if (val != null)
            {
                parsed = val.Trim();
                if (parser != null)
                {
                    parsed = parser(val.Trim(), columnName);
                }
                return parsed;
            }
            return null;
        }

        protected object dec1(string val, string msg)
        {
            try { return decimal.Parse(val); }
            catch { throw new InvalidDataException(string.Format(val + " ({0}) must be a decimal number in the folowing line (line {2}:     {1})", msg, csvLineString, csvLineIndex.ToString())); }
        }
        protected object int1(string val, string msg)
        {
            try { return int.Parse(val); }
            catch { throw new InvalidDataException(string.Format(val + " ({0}) must be an integer number in the folowing line (line {2}:     {1})", msg, csvLineString, csvLineIndex.ToString())); }
        }
        protected object datetime1(string val, string msg)
        {
            try { return DateTime.Parse(val); }
            catch { throw new InvalidDataException(string.Format(val + " ({0}) must be a valid date in the folowing line (line {2}:     {1})", msg, csvLineString, csvLineIndex.ToString())); }
        }
        protected object bool1(string val, string msg)
        {
            if (val.ToUpper() == "Y")
            {
                return true;
            }

            if (val.ToUpper() == "N")
            {
                return false;
            }

            try { return bool.Parse(val); }
            catch { throw new InvalidDataException(string.Format(val + " ({0}) must be a boolean in the folowing line (line {2}:     {1})", msg, csvLineString, csvLineIndex.ToString())); }
        }
        protected object double1(string val, string msg)
        {
            try { return double.Parse(val); }
            catch { throw new InvalidDataException(string.Format(val + " ({0}) must be a 'double' number in the folowing line (line {2}:     {1})", msg, csvLineString, csvLineIndex.ToString())); }
        }

    }
}
