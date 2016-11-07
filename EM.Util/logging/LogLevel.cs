using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace EM.Logging
{

    class LevelManager : EnumManager<Level, string>
    {
        private OrderedDictionary<string, Level> _map = new OrderedDictionary<string, Level>()
                    {
                        {"debug", Level.DEBUG}, 
                        {"error", Level.ERROR},
                        {"fatal", Level.FATAL}, 
                        {"info", Level.INFO},
                        {"trace", Level.TRACE},
                        {"warning", Level.WARN}
                    };
        public override Level defaultEnum { get { return Level.ERROR; } }
        protected override IDictionary<string, Level> valueMap { get { return _map; } }
        public static LevelManager instance = new LevelManager();
    }


    //string instead of int so class instead of enum
    public class LogLevel: ILogLevel, IComparable, IComparable<ILogLevel>     
    {
        public LogLevel(Level level)
        {
            this.level = level;
        }

        /// <summary>
        /// - one of:  debug, info, warning, error
        /// - if null then error
        /// </summary>
        public LogLevel(string level)
        {
            if (level == null)
            {
                level = "error";
            }
            this.level = LevelManager.instance.getEnum(level.Trim().ToLower());            
        }

        
        private Level _level;
        public virtual Level level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        public virtual int priority
        {
            get
            {
                return (int)this.level;
            }
        }

        public override string ToString()
        {
            return LevelManager.instance.getValue(this.level);
        }

        #region IComparable<ILogLevel> Members

        public int CompareTo(ILogLevel other)
        {
            return this.level.CompareTo(other.level);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this.CompareTo((ILogLevel)obj);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return this.level.GetHashCode();
        }


    }

}
