using System;
using System.Collections.Generic;
using System.Text;

using EM.Logging;
using System.Net.Mail;

namespace EM.Util
{
    /// <summary>
    /// - send emails
    ///     - errors are loggeg using it's logger members (at least a Console Logger) unless you register more loggers in it
    /// </summary>
    public class Mailer
    {
        
        public bool raiseErrors = false;
        public Logger logger;
        public string SMTP_Server;
        private System.Net.NetworkCredential _SMTP_Authentication;
        private int? SMTP_Port;
        
        public Mailer(string server)
        {
            this.logger = new Logger("EM.Mailer");
            this.SMTP_Server = server;
        }

        public Mailer(string server, int port) :this(server)
        {
            this.SMTP_Port = port;
        }

        /// <summary>
        /// authenticate for example to make relaying work 
        /// </summary>
        public System.Net.NetworkCredential SMTP_Authentication
        {
            get
            {
                return _SMTP_Authentication;
            }
            set
            {
                this._SMTP_Authentication = value;
            }
        }

        protected string addressFixup(string adr)
        {
            if (adr != null)
            {
                return adr.Replace(";", ",");
            }
            return adr;
        }

        public MailMessage getMail(string from, string to, string subject, string body) { return this.getMail(from, to, null, subject, body, null); }
        public MailMessage getMail(string from, string to, string bcc, string subject, string body) { return this.getMail(from, to, bcc, subject, body, null); }
        public MailMessage getMail(string from, string to, string subject, string body, IEnumerable<Attachment> attachements) { return this.getMail(from, to, null, subject, body, attachements); }
        public MailMessage getMail(string from, string to, string bcc, string subject, string body, IEnumerable<Attachment> attachements)
        {
            MailMessage mail;
            try
            {
                //because in outlook people are used to type ; to separate addresses but here we need comma
                string fromAdr = addressFixup(from);
                string toAdr = addressFixup(to); 

                mail = new MailMessage(fromAdr, toAdr, subject, body);
                if (bcc != null) 
                {
                    string bccAdr = addressFixup(bcc);
                    foreach (string adr in bccAdr.Split(','))
                    {
                        mail.Bcc.Add(new MailAddress(adr));
                    }
                } 
                if (attachements != null)
                {
                    foreach (Attachment atm in attachements)
                    {
                        mail.Attachments.Add(atm);
                    }
                }
                return mail;
            }
            catch (ArgumentException e)
            {
                logger.error("MAIL_FORMAT_ERROR", "Mail ArgumentException: " + e.Message, e);
                if (this.raiseErrors)
                {
                    throw e;
                }
                return null;
            }
            catch (Exception e)
            {
                logger.error("MAIL_FORMAT_ERROR", "Error in email: " + e.Message, e);
                if (this.raiseErrors)
                {
                    throw e;
                }
                return null;
            }
        }

        public bool send(MailMessage mail)
        {
            return send(mail, null);
        }
        public bool send(MailMessage mail, Action<SmtpClient> customization)        
        {
            try
            {
                SmtpClient s = new SmtpClient(SMTP_Server);
                if (this.SMTP_Port.HasValue) { s.Port = SMTP_Port.Value; }

                if (this.SMTP_Authentication != null)
                {
                    s.Credentials = this.SMTP_Authentication;         
                }
                if (customization != null)
                {
                    customization(s);
                }
                s.Send(mail);
                return true;
            }
            catch (SmtpException e)
            {
                if (e.ToString().ToLower().Contains("no mailbox here by that name"))
                {
                    logger.error("WRONG_EMAIL_ADDRESS", "Error sending email: the email address " + mail.To.ToString() + " because it does not exist ", e);                    
                }
                else
                {
                    logger.error("UNKNOWN_SMTP_ERROR", "Error sending email: " + e.Message , e);                    
                }
                if (this.raiseErrors)
                {
                    throw e;
                }
                return false;
            }
        }
        

    }
}
