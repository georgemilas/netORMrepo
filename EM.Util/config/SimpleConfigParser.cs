using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using EM.Collections;

namespace EM.Util
{
    /// <summary>
    /// Turn a file that looks like below into a Dictionary:
    /// --------------------------------------------------------- 
    /// [a key]= a value  #comments start with # 
    /// [key 2]=    #this is an empty value 
    /// [key 3]= a value may span on more   
    ///         then one line and may use macros @[a key] (embed values from other keys)
    /// ---------------------------------------------------------
    /// 
    /// instantiate : SimpleConfigParser.parse()
    ///               SimpleConfigParser.parse(filePath)
    /// </summary> 
    [Serializable]
    public class SimpleConfigParser : EDictionary<string, string>
    {
        public delegate void ConfigEventHandler(SimpleConfigParser cfg);
        public virtual event ConfigEventHandler OnSave;

        private static string COMMENT = "#";
        private static EDictionary<string, SimpleConfigParser> instance;  //sigleton map
        private EDictionary<string, ESet<string>> tagMacros; 
        protected string _filePath;


        public static string helpHeader = @"#
#  [a key]=     #this is an empty value and a comment
#  [key 2]= values may span more then one line 
#           and may use macros @[a key] (embed values from other keys)
#

";
        public bool writeFileHeader = false;    //write helpHeader to the begining of the file

        //--------------------------------------------------------------------------------------------------
        public SimpleConfigParser() {}   //empty config
        public SimpleConfigParser(string filePath, bool parseTheMacros)
        {
            if (filePath == null)
            {
                filePath = Environment.CurrentDirectory + "\\config.cfg";
            }
            this.filePath = filePath;
            this.construct(parseTheMacros);            
        }

        //--------------------------------------------------------------------------------------------------
        //sigleton factory
        public static SimpleConfigParser empty() { return new SimpleConfigParser(); }
        public static SimpleConfigParser parse() {return parse(null); }
        public static SimpleConfigParser parse(string filePath) {return parse(filePath, true, false); }
        public static SimpleConfigParser parse(string filePath, bool parseTheMacros, bool reloadConfig)
        {
            if (instance == null)
                {instance = new EDictionary<string, SimpleConfigParser>();}
            
            if (filePath == null)
            {
                filePath = Environment.CurrentDirectory + "\\config.cfg";
            }
            if (instance.get(filePath, null) == null || reloadConfig)
            { 
                instance[filePath] = new SimpleConfigParser(filePath, parseTheMacros); 
            }
            return instance[filePath];
        }

        public static SimpleConfigParser parse(StreamReader sr, bool parseTheMacros) 
        {
            var inst = new SimpleConfigParser();
            inst.construct(sr);
            if (parseTheMacros)
            {
                inst.parseMacros();
            }
            return inst;
        }

        public static SimpleConfigParser fromDictionary(IDictionary<string, string> d)
        {
            SimpleConfigParser p = new SimpleConfigParser();
            foreach (string key in d.Keys)
            {
                p.Add(key, d[key]);
            }
            return p;
        }
        //--------------------------------------------------------------------------------------
        public virtual string filePath
        {
            get { return this._filePath; }
            set { this._filePath = value; }
        }

        //--------------------------------------------------------------------------------------------------
        public virtual void construct(bool parseTheMacros)
        {
            StreamReader sr = File.OpenText(this.filePath);
            //Console.WriteLine(String.Format(StringUtil.CRLF + "-------------Reading config from {0}--------------" + StringUtil.CRLF, this.filePath));
            this.construct(sr);
            sr.Close();
            if (parseTheMacros)
            {
                this.parseMacros();
            }
        }
        protected virtual void construct(StreamReader sr)
        {
            Regex keyPtt = new Regex(@"^\[[\w\d_ -:@/\\]+\] *=", RegexOptions.IgnoreCase);
            Match keyMatch;
            String curKey = "";
            String input, tinput;
            string lineValue = "";

            while ((input = sr.ReadLine()) != null)
            {
                tinput = input.Trim();

                if (tinput.StartsWith(COMMENT)) continue;  //ignore comments

                keyMatch = keyPtt.Match(tinput);
                if (keyMatch.Success)  //new key tag
                {
                    if (curKey != "")
                    {   //finish up the previous key
                        this[curKey] = this[curKey].Trim();
                    }

                    lineValue = tinput.Replace(keyMatch.Value, "");
                    if (lineValue.IndexOf(COMMENT) >= 0)
                    {
                        lineValue = lineValue.Substring(0, lineValue.IndexOf(COMMENT) - 1);
                    }

                    //the tag is only the text (without paranteses [] and the = sign)
                    curKey = keyMatch.Value.Substring(1, keyMatch.Value.IndexOf("]") - 1);  
                    this[curKey] = lineValue;
                    continue;
                }

                //consider an empty line for the value of this key only if in the middle of the value
                if (curKey != "" && (tinput != "" || (tinput=="" && this[curKey] != "")))
                {
                    lineValue = input;
                    if (lineValue.IndexOf(COMMENT) >= 0)
                    {
                        lineValue = lineValue.Substring(0, lineValue.IndexOf(COMMENT) - 1);
                    }
                    if (this[curKey] != "" && lineValue != "")  //preserve line breaks (new lines)
                    {
                        this[curKey] += StringUtil.CRLF;
                    }
                    this[curKey] += lineValue;
                }
            }
            if (curKey != "")
            {   //finish up the last key
                this[curKey] = this[curKey].Trim();
            }
            
        }

        /// <summary>
        /// replace @[a-tag name_01] in the values of all the tags where it apears with the value of the tag it points to
        /// </summary>
        protected void parseMacros()
        {
            //TODO: is there a recursive search problem?
            Regex ptt = new Regex(@"@\[[\w\d_ -]+\]", RegexOptions.IgnoreCase);
            Match mptt;
            string searchKey;

            this.tagMacros = new EDictionary<string, ESet<string>>(); 
            //enumerate this list instead of main keys so we can alter the dictionary while enumerating keys
            List<string> keys = new List<string>();  
            foreach (string k in this.Keys)
            { keys.Add(k); }

            foreach (string k in keys)
            {
                mptt = ptt.Match(this[k]);
                while (mptt.Success)
                {
                    searchKey = mptt.Value.Substring(2, mptt.Value.Length - 3);
                    this[k] = this[k].Replace(mptt.Value, this[searchKey]);
                    this.tagMacros.setdefault(k, new ESet<string>()).Add(searchKey);
                    mptt = mptt.NextMatch();
                    //mptt = ptt.Match(this[k]);
                }
            }
        }

        /// <summary>
        /// - undo what parseMacros has done:
        ///   replace portion of the value in a tags with another @[a-tag name_01] if there is a tag that has as value the current portion
        /// </summary>
        protected bool unparseMacros()
        {
            if (this.tagMacros != null)
            {
                Dictionary<string, string> parsedValues = new Dictionary<string, string>();
                foreach (string k in this.tagMacros.Keys)
                {
                    parsedValues[k] = this[k];
                }

                foreach (string k in this.tagMacros.Keys)
                {
                    this.unparseMacro(k, parsedValues);
                }
                this.tagMacros = null;
                return true;    //reparse
            }
            return false;       //no need to reparse
        }

        protected void unparseMacro(string key, Dictionary<string, string> parsedValues)
        {
            foreach (string macro in this.tagMacros[key])
            {
                string parsedValue = this[macro];   //might be unparsed already
                if (this.tagMacros.ContainsKey(macro))
                {
                    parsedValue = parsedValues[macro];
                    unparseMacro(macro, parsedValues);
                }
                this[key] = this[key].Replace(parsedValue, string.Format("@[{0}]", macro));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{" + StringUtil.CRLF);
            foreach (string k in this.Keys)
            {
                sb.AppendFormat("[{0}] = '{1}'" + StringUtil.CRLF, k, this[k]);
            }
            sb.Append("}" + StringUtil.CRLF);
            return sb.ToString();
        }

        public virtual EDictionary<string, string> toDictionary()
        {
            EDictionary<string, string> d = new EDictionary<string, string>();
            foreach (string key in this.Keys)
            {
                d.Add(key, this[key]);
            }
            return d;
        }
        
        public virtual SimpleConfigParser copy()
        {
            SimpleConfigParser newCfg = SimpleConfigParser.fromDictionary(this.toDictionary());
            newCfg.filePath = this.filePath;
            newCfg.tagMacros = this.tagMacros;
            return newCfg;
        }
        
        public virtual void save()
        {
            this.saveAs(this.filePath);
        }

        public virtual void saveAs(string filePath)
        {
            if (OnSave != null)
            {
                OnSave(this);
            }

            StringBuilder sout = new StringBuilder();
            EList<string> existKeys = new EList<string>();

            if (File.Exists(filePath))
            {

                bool parseTheMacros = this.unparseMacros();

                StreamReader sr = File.OpenText(filePath);
                Regex keyPtt = new Regex(@"^\[[\w\d_ -]+\] *=", RegexOptions.IgnoreCase);
                Match keyMatch;
                String curKey = "";
                String input, tinput;
                while ((input = sr.ReadLine()) != null)
                {
                    tinput = input.Trim();

                    //preserve comments and blank lines
                    if (tinput.StartsWith(COMMENT) || tinput == "")
                    {
                        sout.Append(input + StringUtil.CRLF);
                        continue;
                    }

                    keyMatch = keyPtt.Match(tinput);
                    if (keyMatch.Success)  //new key tag
                    {
                        curKey = keyMatch.Value.Substring(1, keyMatch.Value.IndexOf("]") - 1);
                        sout.Append(keyMatch.Value + " " + this[curKey] + StringUtil.CRLF);
                        existKeys.Add(curKey);
                        continue;
                    }
                }
                sr.Close();

                if (parseTheMacros)
                {
                    this.parseMacros();
                }

            }
            
            if (this.writeFileHeader && !sout.ToString().Contains(SimpleConfigParser.helpHeader))
            {
                sout.Insert(0, SimpleConfigParser.helpHeader);
            }
            
            //write new keys that are not in the file already
            foreach (string key in this.Keys)
            {
                if (!existKeys.Contains(key))
                {
                    sout.Append(String.Format("[{0}]= {1}" + StringUtil.CRLF, key, this[key]));
                }
            }
            
            StreamWriter sw = new StreamWriter(filePath, false); //don't append, overwrite
            sw.Write(sout.ToString());
            sw.Close();

        }


     }

 


}
