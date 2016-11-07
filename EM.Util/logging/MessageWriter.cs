using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EM.Collections;

namespace EM.Logging
{
    /// <summary>
    /// A Logger that besides the loggers you register to it also coud write
    /// messages to a text box or something
    /// </summary>
    public class MessageWriter: Logger, IMessageWriter
    {
        public delegate void Writer(string txt);
        public Writer writer;
        public delegate void ColorSetter(Color color);
        public ColorSetter colorSetter;
        private EList<Color> prevColor; 

        public MessageWriter()
        {
            this.prevColor = new EList<Color>();
            this.prevColor.Add(Color.Black);
            this.level = new LogLevel(Level.DEBUG);
            this.appId = "";
        }

        
        private Color _color = Color.Black; 
        /// <summary>
        /// Set a collor for the follwing text to be written. 
        /// A colorSetter delegate must be provided for this to actualy work.
        /// </summary>
        public virtual Color color
        {
            get { return this._color; }
            set 
            {
                this.prevColor.Add(_color);
                this._color = value;
                if (this.colorSetter != null)
                {
                    this.colorSetter(value);
                }
            }
        }

        /// <summary>
        /// if there is no previous Color, initial color is set to be Color.Black
        /// </summary>
        public virtual void restorePreviousColor()
        {
            Color c = this.prevColor[0];
            if ( this.prevColor.Count > 1 )
            {
                c = this.prevColor.pop();                
            }
            this._color = c;
            if (this.colorSetter != null)
            {
                this.colorSetter(c);
            }            
        }

        private bool fromWriter = false;
        private object _lock = new object();
        private void doWriter(string msg, string msgNoCRLF, ILogLevel level) 
        {
            try
            {
                writer(msg);
            }
            finally
            {
                if (this.loggers.Count > 0)
                {
                    lock (_lock)
                    {
                        fromWriter = true;
                        try
                        {
                            base.write(appId, "", level, msgNoCRLF, null, "", null);
                        }
                        finally
                        {
                            fromWriter = false;
                        }
                    }
                }
            }
        }

        public virtual void Write(Color color, string txt, ILogLevel level) { this.Write(color, txt, level, null); }
        public virtual void Write(Color color, string txt, ILogLevel level, params object[] args)
        {
            this.color = color;
            this.Write(txt, level, args);
            this.restorePreviousColor();
        }

        public virtual void Write(string txt, ILogLevel level) { this.Write(txt, level, null); }
        public virtual void Write(string txt, ILogLevel level, params object[] args)
        {
            string msg = txt;
            if (args != null)
            {
                msg = String.Format(txt, args);
            }

            if (writer != null)
            {
                if (this.colorSetter != null)
                {
                    this.colorSetter(this.color);
                }
                doWriter(msg, msg, level);
            }
            //base.write(base.appId, "MessageWriter", base.level, msg, null, "", null);
        }

        public virtual void WriteLine(Color color, string txt, ILogLevel level) { this.WriteLine(color, txt, level, null); }
        public virtual void WriteLine(Color color, string txt, ILogLevel level, params object[] args)
        {
            this.color = color;
            this.WriteLine(txt, level, args);
            this.restorePreviousColor();
        }

        public virtual void WriteLine(string txt, ILogLevel level) { this.WriteLine(txt, level, null); }
        public virtual void WriteLine(string txt, ILogLevel level, params object[] args)
        {
            string msg = txt + StringUtil.CRLF;
            if (args != null)
            {
                msg = String.Format(txt + StringUtil.CRLF, args);
            }
            
            if (writer != null)
            {
                if (this.colorSetter != null)
                {
                    this.colorSetter(this.color);
                }
                doWriter(msg, msg.TrimEnd(), level);
            }
            //base.write(base.appId, "MessageWriter", base.level, msg, null, "", null);
        }

        public virtual void WriteException(Exception er)
        {
            var errlevel = new LogLevel(Level.ERROR);
            this.color = Color.Red;
            this.Write(StringUtil.CRLF, errlevel);
            this.Write(getDetails(er, ""), errlevel);
            this.Write(StringUtil.CRLF, errlevel);
            this.restorePreviousColor();
        }



        // will let individual registered loggers to hadle priority insteaf of
        // applying only if parameter logLevel's priority is bigger than the container's instance priority
        public override void write(string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails)
        {
            if (logLevel == null || this.level == null)
            {
                this.write(this.appId, logId, logLevel, msg, e, moreDetails);
                return;
            }
            if (logLevel.priority >= this.level.priority)
            {
                this.write(this.appId, logId, logLevel, msg, e, moreDetails);
            }
        }
        public override void write(string logId, ILogLevel logLevel, string msg, string moreDetails)
        {
            if (logLevel == null || this.level == null)
            {
                this.write(this.appId, logId, logLevel, msg, moreDetails);
                return;
            }
            if (logLevel.priority >= this.level.priority)
            {
                this.write(this.appId, logId, logLevel, msg, moreDetails);
            }
        }
        public override void write(ILogMessage message)
        {
            if (message.logLevel == null || this.level == null)
            {
                this.write(this.appId, message.logId, message.logLevel, message.msg, message.e, message.moreDetails, message.context);
                return;
            }
            if (message.logLevel.priority >= this.level.priority)
            {
                this.write(this.appId, message.logId, message.logLevel, message.msg, message.e, message.moreDetails, message.context);
            }
        }



        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (!fromWriter && logLevel.priority >= this.level.priority)
            {
                LogLevel warn = new LogLevel(Level.WARN);
                LogLevel debug = new LogLevel(Level.DEBUG);

                if (logLevel.priority >= warn.priority)
                {
                    this.color = Color.Red;
                    this.Write("{2}{1}: {0} {2}", logLevel, msg, logLevel.ToString().ToUpper() , StringUtil.CRLF);
                    string details = getDetails(e, moreDetails);
                    if (details != "")
                    {
                        this.WriteLine("DETAILS:" + StringUtil.CRLF + details + StringUtil.CRLF, logLevel);
                    }
                    this.restorePreviousColor();
                }
                else if (logLevel.priority == debug.priority)
                {
                    this.color = Color.DarkGray;
                    this.WriteLine(msg, logLevel);
                    string details = getDetails(e, moreDetails);
                    if (details != "")
                    {
                        this.WriteLine("DETAILS:" + StringUtil.CRLF + details + StringUtil.CRLF, logLevel);
                    }
                    this.restorePreviousColor();
                }
                else
                {
                    this.WriteLine(msg, logLevel);
                    string details = getDetails(e, moreDetails);
                    if (details != "")
                    {
                        this.WriteLine("DETAILS:" + StringUtil.CRLF + details + StringUtil.CRLF, logLevel);
                    }
                }                
            }            
        }


    }
}
