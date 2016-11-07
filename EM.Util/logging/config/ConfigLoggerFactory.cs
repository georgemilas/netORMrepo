using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using EM.Batch;
using System.Net;

namespace EM.Logging.Config
{
    /*
    <configSections>
	    <section name="loggersSection" type="EM.Logging.Config.LoggerSection, EUtil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"  />
	</configSections>
     
    <loggersSection>
		<loggers>
<!--
	level {  trace, debug, info, warning, error, fatal }
	RollingType { Daily, Weekly }
	RollingRemove { DayOld, WeekOld, MonthOld, ThreeMonthsOld, SixMonthOld, YearOld }
	BatchProvider { TimeBatchProvider, VolumeBatchProvider }
	batchTreshold { integer, for timeBatchProvider will be minutes and for volume is number of emails to combine }
-->
			<logger id="file" className="RollingFileLogger" level="debug" 
					rollingType="Daily" 
					rollingTypeRemove="MonthOld" 
					logFileName="PayrollEngine.log"
					></logger>
			<logger id="email" className="EmailLogger" level="debug" 
					emailAddress="george.milas@surepayroll.com"
					senderEmailAddress="tech.support@surepayroll.com"
					mailServerAddress="mail.surepayroll.com"
                    user="SMTPuserName" password="SMTPpassword"
					batchProvider="VolumeBatchProvider" batchThreshold="5"
					></logger>
			<logger id="console" className="ConsoleLogger" level="trace"></logger>
			<logger id="win" className="WindowsEventLogLogger" level="fatal"></logger>
		</loggers>
	</loggersSection>
    */
    public class ConfigLoggerFactory
    {
        public string appId { get; set; }
        protected ILogLevel level;

        public ConfigLoggerFactory(string appId)
        {
            this.appId = appId;
            this.level = new LogLevel(Level.DEBUG);
        }
        public ConfigLoggerFactory(string appId, ILogLevel level)
        {
            this.appId = appId;
            this.level = level;
        }

        public virtual T processConfigSection<T>() where T: Logger, new()
        {
            return processConfigSection<T>("loggersSection");
        }

        public virtual Logger processConfigSection()
        {
            return processConfigSection<Logger>("loggersSection");
        }

        public virtual T processConfigSection<T>(string sectionName) where T: Logger, new() 
        {
            T logger = new T();
            logger.appId = appId;
            logger.level = this.level;

            LoggerSection logSection = (LoggerSection)ConfigurationManager.GetSection(sectionName);
            foreach (LoggerElement log in logSection.loggers)
            {                
                BaseLogger thelog = getLogger(log, getTypedLoggerElement(log));
                if (thelog != null) 
                { 
                    logger.register(thelog); 
                }
            }

            return logger;
        }

        public virtual TypedLoggerElement getTypedLoggerElement(LoggerElement log)
        {
            TypedLoggerElement l = new TypedLoggerElement();

            string lv = log.level ?? level.ToString();
            l.level = new LogLevel(lv);
            l.logFileName = log.logFileName ?? appId + ".log";

            l.rollingType = !String.IsNullOrEmpty(log.rollingType) ?
                RollingTypeManager.instance.getEnum(log.rollingType) :
                RollingType.Daily;
            l.rollingTypeRemove = !String.IsNullOrEmpty(log.rollingTypeRemove) ?
                RollingTypeRemoveManager.instance.getEnum(log.rollingTypeRemove) :
                RollingTypeRemove.ThreeMonthsOld;
            
            l.senderEmailAddress = log.senderEmailAddress;
            l.mailServerAddress = log.mailServerAddress;
            l.emailAddress = log.emailAddress;
            l.user = log.user;
            l.password = log.password;

            l.batchThreshold = !String.IsNullOrEmpty(log.batchThreshold) ?
                int.Parse(log.batchThreshold) :
                10;
            
            if (log.batchProvider != null)
            {
                switch (log.batchProvider)
                {
                    case "VolumeBatchProvider":
                        l.batchProvider = new VolumeBatchProvider(l.batchThreshold);
                        break;
                    case "TimeBatchProvider":
                        l.batchProvider = new TimeBatchProvider(l.batchThreshold);
                        break;
                }
            }

            return l;
        }

        public virtual BaseLogger getLogger(LoggerElement rawLog, TypedLoggerElement log)
        {
            BaseLogger thelog = null;
            switch (rawLog.className)
            {
                case "RollingFileLogger":
                    thelog = new RollingFileLogger(appId, log.level, log.logFileName, log.rollingType, log.rollingTypeRemove);
                    break;
                case "FileLogger":
                    thelog = new FileLogger(appId, log.level);
                    break;
                case "WindowsEventLogLogger":
                    thelog = new WindowsEventLogLogger(appId, log.level);
                    break;
                case "ConsoleLogger":
                    thelog = new ConsoleLogger(appId, log.level);
                    break;
                case "EmailLogger":
                    EmailLogger l = new EmailLogger(appId, log.senderEmailAddress, log.emailAddress, log.mailServerAddress, log.level);
                    if (log.batchProvider != null)
                    {
                        l.batch = log.batchProvider;
                    }
                    if (!String.IsNullOrEmpty(log.user) && !String.IsNullOrEmpty(log.password))
                    {
                        l.mailer.SMTP_Authentication = new NetworkCredential(log.user, log.password);
                    }
                    thelog = l;
                    break;
            }

            return thelog;
        }

    }
}
