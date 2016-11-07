using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EM.Collections;

namespace EM.Logging
{
    public delegate string LogHeaderTemplate(); 
    public delegate string LogDetailTemplate(string appId, string logId, ILogLevel logLevel, string msg, string stackTrace); 

    public class LogTemplates
    {

        private static void test()
        {

        }

        public static string BasicTableStyleTemplateHeader()
        {
            return " date :: appId :: logLevel :: logId :: msg :: details ";
        }


        public static string TableStyleTemplateFull(string appId, string logId, ILogLevel logLevel, string msg, string details)
        {
            string dt = DateTime.Now.ToString("MM/dd HH:ss:mm");
            string lg = logLevel.ToString();
            StringBuilder sb = new StringBuilder();

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bts = encoding.GetBytes(details);

            var m = new MemoryStream(bts);

            StreamReader sr = new StreamReader(m);

            string input;
            int cnt = 1;
            while ((input = sr.ReadLine()) != null)
            {
                string line = input.Trim();
                if (line == "") continue;

                if (cnt == 1)
                {
                    sb.Append(dt + " :: " +
                              appId + " :: " +
                              lg + " :: " +
                              logId + " :: " +
                              msg + " :: " +
                              line);
                }
                else
                {
                    sb.Append(StringUtil.CRLF + dt + " :: " +
                              appId + " :: " +
                              lg + " :: " +
                              " " + " :: " +
                              " " + " :: " +
                              line);
                }
                cnt++;
            }
            m.Close();
          
            return sb.ToString();
        }
        
        public static string TableStyleTemplateFull2(string appId, string logId, ILogLevel logLevel, string msg, string details)
        {
            string dt = DateTime.Now.ToString();
            string lg = logLevel.ToString();
            StringBuilder sb = new StringBuilder();
            //string[] detailsLines = details.Split(new string[] { StringUtil.CRLF }, StringSplitOptions.RemoveEmptyEntries);
            string[] detailsLines = details.Split(new char[] { '\n' });
            int cnt = 1; 
            foreach (string line in detailsLines)
            {
                if (cnt == 1)
                {
                    sb.Append(dt + " :: " +
                              appId + " :: " +
                              lg + " :: " +
                              logId + " :: " +
                              msg + " :: " +
                              line);
                }
                else
                {
                    //sb.Append(StringUtil.CRLF + dt + " :: " +
                    sb.Append("\n" + dt + " :: " +
                              appId + " :: " +
                              lg + " :: " +
                              " " + " :: " +
                              " " + " :: " +
                              line);
                }
                cnt++;
            }
            return sb.ToString();
        }

        public static string BasicTableStyleTemplate(string appId, string logId, ILogLevel logLevel, string msg, string details)
        {
            return DateTime.Now.ToString() + " :: " +
                    appId + " :: " +
                    logLevel.ToString() + " :: " +
                    logId + " :: " +
                    msg + " :: " +
                    details;
        }

        public static string BasicEmailSubjectTemplate(string appId, string logId, ILogLevel logLevel, string msg, string details)
        {
            return appId + " " + logId + " - " + logLevel.ToString() + " message";
        }
        public static string BasicEmailBodyTemplate(string appId, string logId, ILogLevel logLevel, string msg, string details)
        {
            return "Application: " + appId + " (" + DateTime.Now.ToString() + ")" + StringUtil.CRLF +
                          logLevel + ": " + logId + StringUtil.CRLF +
                          "Message: " + msg + StringUtil.CRLF +
                          details + StringUtil.CRLF;
        }

        public static string BasicHTMLTemplate(string appId, string logId, ILogLevel logLevel, string msg, string details)
        {
            return "<div>" + DateTime.Now.ToShortTimeString() + " : " + appId + " : " +
                    logId + " : " +
                    logLevel.ToString() + " : " +
                    msg + " : " +
                    details +
                    "</div>";
        }

    }
}
