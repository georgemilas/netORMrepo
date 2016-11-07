using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using EM.Collections;
using System.Reflection;
using System.Globalization;
using System.Threading;


namespace EM.Logging
{
    public enum RollingType { Daily, Weekly }
    public enum RollingTypeRemove { DayOld, WeekOld, MonthOld, ThreeMonthsOld, SixMonthOld, YearOld }

    /// <summary>
    /// - log to a file named either appID_yyyy-mm-dd  OR  appID_yyyy-mm-wNr , based on the RollingType property
    /// </summary>
    public class RollingFileLogger : FileLogger
    {
        public RollingFileLogger(string appId, Level level) : this(appId, new LogLevel(level)) { }
        public RollingFileLogger(string appId, ILogLevel level) : this(appId, level, null, null, RollingType.Daily, RollingTypeRemove.YearOld) { }
        public RollingFileLogger(string appId, ILogLevel level, RollingType rollingType, RollingTypeRemove rollingTypeRemove) : this(appId, level, null, null, rollingType, rollingTypeRemove) { }
        public RollingFileLogger(string appId, ILogLevel level, string fileName, RollingType rollingType, RollingTypeRemove rollingTypeRemove): this(appId, level, null, fileName, rollingType, rollingTypeRemove) {  }
        public RollingFileLogger(string appId, ILogLevel level, string folderPath, string fileName, RollingType rollingType, RollingTypeRemove rollingTypeRemove)
            : base()
        {
            this.appId = appId;
            this.level = level;
            this.rollingType = rollingType;

            if (folderPath != null && folderPath.Trim() != "")
            {
                this.folderPath = folderPath;
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(Assembly.GetExecutingAssembly().Location);
                this.folderPath = di.Parent.FullName;                
            }
            if (fileName!= null) 
            {
                this.fileName = fileName;
            }

            this.rollingTypeRemove = rollingTypeRemove; // RollingTypeRemove.YearOld;
            this.createRollingFileIfNeeded();                        
        }

        private void createRollingFileIfNeeded()
        {
            //_filePath = getRollingFilePath(DateTime.Now);                    
            if (File.Exists(this.filePath))
            {
                DateTime created = File.GetCreationTime(this.filePath);
                double days = (DateTime.Now - created).TotalDays;
                bool doMove = false;
                if (this.rollingType == RollingType.Weekly && days >= 7)
                {
                    doMove = true;
                }
                else if (days >= 1)
                {
                    doMove = true;
                }
                if (doMove)
                {
                    string rollingFile = getRollingFilePath(created);
                    if (fs != null)
                    {
                        fs.Close();
                        fs = null;
                    }
                    File.SetCreationTime(this.filePath, DateTime.Now);
                    File.Move(this.filePath, rollingFile);
                    try 
                    { 
                        File.Delete(this.filePath);                        
                        Thread.Sleep(100);
                    } //just in case move will not delete it
                    catch { }
                }
            }
        }
        


        private string getRollingFilePath(DateTime date)
        {
            string name = this.fileName == null ? this.appId + ".log" : this.fileName;
            string ext = Path.GetExtension(name);
            name = Path.GetFileNameWithoutExtension(name);

            if (this.rollingType == RollingType.Weekly)
            {
                Calendar cal = CultureInfo.CurrentCulture.Calendar;
                int week = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);                
                return this.folderPath + "\\" + name + "_" + date.ToString("yyyy-MM-w") + week.ToString() + ext;
            }
            else
            {
                return this.folderPath + "\\" + name + "_" + date.ToString("yyyy-MM-dd") + ext;
            }
        }

        protected override void createFileIfNedded()
        {
            if (fs == null)
            {
                this.createRollingFileIfNeeded();
            }
            base.createFileIfNedded();            
        }


        private RollingType _rollingType;
        public RollingType rollingType
        {
            get { return this._rollingType; }
            set { this._rollingType = value; }
        }

        private RollingTypeRemove _rollingTypeRemove;
        public RollingTypeRemove rollingTypeRemove
        {
            get { return this._rollingTypeRemove; }
            set 
            { 
                this._rollingTypeRemove = value;
                if (value != null)
                {
                    string name = this.fileName == null ? this.appId + ".log" : this.fileName;
                    string ext = Path.GetExtension(name);
                    name = Path.GetFileNameWithoutExtension(name);

                    DateTime now = DateTime.Now;
                    string[] files = Directory.GetFiles(this.folderPath);
                    Regex rg = new Regex(name + @"_(?<year>\d\d\d\d)-(?<month>\d\d)-((?<day>\d\d)|w(?<week>\d\d))?" + ext.Replace(".", @"\."));
                    foreach (string file in files)
                    {
                        Match fm = rg.Match(file);
                        if (fm.Success)
                        {
                            int year = int.Parse(fm.Groups["year"].Value);
                            int month = int.Parse(fm.Groups["month"].Value);
                            
                            DateTime logDate = new DateTime(year, month, 1);

                            if (fm.Groups["day"].Value != "")
                            {
                                int day = int.Parse(fm.Groups["day"].Value);
                                logDate = new DateTime(year, month, day);
                            }
                            else
                            {
                                int week = int.Parse(fm.Groups["week"].Value);
                                Calendar cal = CultureInfo.CurrentCulture.Calendar;
                                int week1 = cal.GetWeekOfYear(new DateTime(year, month, 1), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                                int day = 7;
                                if (week != week1)
                                {
                                    day = (Math.Abs(week - week1) * 7) + 7;
                                }
                                if (day > 30 || (month == 2 && day > 28))
                                {
                                    day = month == 2 ? 28 : 30;
                                }
                                logDate = new DateTime(year, month, day);
                            }

                            TimeSpan time = now - logDate;
                            switch (value)
                            {
                                case RollingTypeRemove.DayOld:
                                    if (time.TotalDays > 1) { File.Delete(file); }
                                    break;
                                case RollingTypeRemove.WeekOld:
                                    if (time.TotalDays > 7) { File.Delete(file); }
                                    break;
                                case RollingTypeRemove.MonthOld:
                                    if (time.TotalDays > 31) { File.Delete(file); }
                                    break;
                                case RollingTypeRemove.ThreeMonthsOld:
                                    if (time.TotalDays > 93) { File.Delete(file); }
                                    break;
                                case RollingTypeRemove.SixMonthOld:
                                    if (time.TotalDays > 186) { File.Delete(file); }
                                    break;
                                case RollingTypeRemove.YearOld:
                                    if (time.TotalDays > 365) { File.Delete(file); }
                                    break;
                            }
                        }
                    }

                }
            }
        }


        private string _folderPath;
        public string folderPath
        {
            get { return this._folderPath; }
            set { this._folderPath = value; }
        }

        private string _filePath;
        public override string filePath
        {
            get 
            {                
                if (_filePath == null)
                {
                    string name = this.fileName == null ? this.appId + ".log" : this.fileName;
                    _filePath = this.folderPath + "\\" + name;                                                                        
                }
                return _filePath;                
            }
            set
            {
                throw new NotSupportedException("Can not assing RollingFileLogger file path, it is calculated based on the folder, appID (or fileName if given) and rolling type.");
            }
        }

        public string fileName { get; set; }



       

        
    }




    public class RollingTypeManager : EnumManager<RollingType, string>
    {
        private Dictionary<string, RollingType> _map = new Dictionary<string, RollingType>()
                {
                    {"Daily", RollingType.Daily},    
                    {"Weekly", RollingType.Weekly}
                };

        protected override IDictionary<string, RollingType> valueMap { get { return _map; } }
        public static RollingTypeManager instance = new RollingTypeManager();
    }

    public class RollingTypeRemoveManager : EnumManager<RollingTypeRemove, string>
    {
        private Dictionary<string, RollingTypeRemove> _map = new Dictionary<string, RollingTypeRemove>()
                {
                    {"DayOld", RollingTypeRemove.DayOld},   
                    {"MonthOld", RollingTypeRemove.MonthOld},   
                    {"SixMonthOld", RollingTypeRemove.SixMonthOld},   
                    {"ThreeMonthsOld", RollingTypeRemove.ThreeMonthsOld},   
                    {"WeekOld", RollingTypeRemove.WeekOld},   
                    {"YearOld", RollingTypeRemove.YearOld}
                };

        protected override IDictionary<string, RollingTypeRemove> valueMap { get { return _map; } }
        public static RollingTypeRemoveManager instance = new RollingTypeRemoveManager();
    }


}
