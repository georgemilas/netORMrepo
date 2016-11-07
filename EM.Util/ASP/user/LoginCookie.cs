using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using EM.Collections;
using System.Security.Cryptography;
using System.Text;

namespace EM.ASP.user
{
    /// <summary>
    /// - a cookie that contains userName and userID from sd_sec.tbl_users and list of alowed actions
    ///   usualy specified by RightsManager
    /// - by default uses sliding expiration but if a timeout is not specified will alwais expire
    ///   otherwise if timeout is specified then will only expire if the user did not touch the page for the amout of time
    ///   and every time a user will touch the page the start time for timeout is reseted to the begining
    /// - the cookie must have userName set in order for the isLoggedIn to be true
    /// </summary>
    public class LoginCookie
    {
        protected HttpCookie _cookie;
        protected DateTime? _lastAccessTime;
        protected EList<string> _actions;
        protected string cookieName;
        protected bool _sliding;
        protected Page page;
        protected bool _isset;

        public LoginCookie(Page page, string cookieName)
        {
            this.cookieName = cookieName;
            this.page = page;
            this._isset = false;
        }

        public void setToResponse()
        {
            this.cookie["s"] = this.secret;

            if (!this._isset)
            {
                this.page.Response.Cookies.Add(this.cookie);
                this._isset = true;
            }
        }

        ////////////////////////////////////////////////////////////////////
        public HttpCookie cookie
        {
            get
            {
                if (_cookie == null)
                {
                    if (this.page.Request.Cookies[cookieName] == null)
                    {
                        _cookie = new HttpCookie(cookieName);
                        _cookie.Path = "/";
                    }
                    else
                    {
                        _cookie = this.page.Request.Cookies[cookieName];
                    }
                }
                return _cookie;
            }
        }

        public virtual bool slidingExpiration
        {
            get
            {
                if (this.cookie["slide"] == null) { return true; }      //by default is sliding
                return this.cookie["slide"] == "1" ? true : false;
            }
            set
            {
                this.cookie["slide"] = value ? "1" : "0";
            }

        }

        public void setCustomKey(string name, string value)
        {
            if (value == null)
            {
                this.cookie.Values.Remove(name);
            }
            else
            {
                this.cookie[name] = value;
            }
        }
        public string getCustomKey(string name) { return this.getCustomKey(name, null); }
        public string getCustomKey(string name, string defaultValue)
        {
            if (this.cookie[name] == null) { return defaultValue; }
            return this.cookie[name];
        }

        public virtual string userName
        {
            get
            {
                if (this.cookie["user"] == null) { return null; }
                return this.page.Server.UrlDecode(this.cookie["user"]);
            }
            set
            {
                if (value == null)
                {
                    this.cookie.Values.Remove("user");
                }
                else
                {
                    this.cookie["user"] = value;
                }
            }
        }
        public virtual int? userDBID
        {
            get
            {
                if (this.cookie["db"] == null) { return null; }
                return int.Parse(this.page.Server.UrlDecode(this.cookie["db"]));
            }
            set
            {
                if (value == null)
                {
                    this.cookie.Values.Remove("db");
                }
                else
                {
                    this.cookie["db"] = value.ToString();
                }
            }
        }
        public virtual EList<string> alowedActions
        {
            get
            {
                if (this._actions == null)
                {
                    if (this.cookie["a"] == null)
                    {
                        this._actions = new EList<string>();
                    }
                    else
                    {
                        string actions = this.decode(this.cookie["a"]);
                        this._actions = CSV.fromCsvLine(actions);
                    }

                }
                return this._actions;
            }
            set
            {
                this._actions = value;
                if (value == null)
                {
                    this.cookie.Values.Remove("a");
                }
                else
                {
                    this.cookie["a"] = this.encode(CSV.toCsvLine(value, true));
                }
            }
        }

        /// <summary>
        /// login cookie timeout in minutes, must be set (default is 0, so instant expire)
        /// </summary>
        public virtual int timeout
        {
            get
            {
                if (this.cookie["to"] == null) { return 0; }
                return int.Parse(this.cookie["to"]);
            }
            set
            {
                this.cookie["to"] = value.ToString();
            }
        }

        /*
        protected virtual string secretOld
        {
            get
            {
                if (this.cookie["s"] == null) { return null; }
                return this.page.Server.UrlDecode(this.cookie["s"]);
            }
        }
        */

        protected string decode(string str)
        {
            var e = new EM.Util.Encription("something 13$jhg+7345");
            return e.Decrypt(str);
        }
        protected string encode(string str)
        {
            //MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //byte[] res = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(str));
            //return UTF8Encoding.UTF8.GetString(res);
            var e = new EM.Util.Encription("something 13$jhg+7345");
            return e.Encrypt(str);
        }


        protected virtual string secret
        {
            get
            {
                string enc = this.encode(this.lastAccessTime.ToString() + this.userName + this.userDBID + this.alowedActions.ToString() + cookieName + "A Secrtet &*GT$^%+JHGF^6");
                //if (enc.Length > 15)
                //{
                //    enc = enc.Substring(0, 15);
                //}
                return this.page.Server.UrlEncode(enc);
            }
        }

        protected virtual string decodeSecret(string secret)
        {
            return this.decode(this.page.Server.UrlDecode(secret));                            
        }

        public virtual DateTime lastAccessTime
        {
            get
            {
                if (this._lastAccessTime == null)
                {
                    if (this.cookie["tm"] == null)
                    {
                        this._lastAccessTime = DateTime.Now;
                    }
                    else
                    {
                        this._lastAccessTime = DateTime.Parse(this.page.Server.UrlDecode(this.cookie["tm"]));
                    }
                }
                return (DateTime)this._lastAccessTime;
            }
            set
            {
                
                if (value == default(DateTime))
                {
                    this._lastAccessTime = null;
                    this.cookie.Values.Remove("tm");
                }
                else
                {
                    this._lastAccessTime = value;
                    this.cookie["tm"] = value.ToString();
                }
                this.setToResponse();
            }
        }


        public bool hasExpired()
        {
            TimeSpan elapsed = DateTime.Now - this.lastAccessTime;
            if (elapsed.TotalMinutes >= this.timeout)
            {
                this.logout();
                return true;
            }

            //secret
            if (this.cookie["s"] == null) { this.logout(); return true; }
            if (decodeSecret(this.cookie["s"]) != decodeSecret(this.secret)) { this.logout(); return true; }

            if (this.slidingExpiration)
            {
                this.lastAccessTime = DateTime.Now;
            }
            return false;
        }


        public bool isLoggedIn()
        {
            if (this.userName != null && this.userName.Trim() != "")
            {
                if (this.hasExpired())
                {
                    return false;
                }
                return true;
            }
            return false;
        }


        public void login(string userName, int? userDBID, EList<string> alowedActions)
        {
            this.userName = userName;
            this.userDBID = userDBID;
            this.alowedActions = alowedActions;
            this.timeout = this.page.Session.Timeout;
            this.setToResponse();
        }

        public void logout()
        {
            this.userName = null;
            this.userDBID = null;
            this.alowedActions = null;
            this.lastAccessTime = default(DateTime);
            this.timeout = 0;
            this.setToResponse();
            this.page.Session.Abandon();
        }


    }
}