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

namespace EM.ASP.user
{

    /// <summary>
    ///  - stores info about user: userName, userID, alowedActions, isAlowed(action), isLoggedIn etc.
    ///  - uses EUtil.ASP.user.LoginCookie internaly as engine during user session lifecicle
    ///    so after user is sucessfuly authenticated(), every time we instantiate a new UserPermission 
    ///    if data still exists in the cookie then the user is considered logged in
    /// </summary>
    public abstract class UserPermissions 
    {
        public Page page;
        public LoginCookie cookie;
        protected string _scriptName;
       
        public UserPermissions(Page page, string cookieName)            
        {
            this.page = page;
            this.cookie = new LoginCookie(this.page, cookieName);
            this.scriptName = this.page.Request.ServerVariables["URL"];
        }

        public UserPermissions(MasterPage page, string cookieName)
            : this(page.Page, cookieName)
        {
            //no page permission check is necesary for a MasterPage
            //you should only do handleLogin() without handlePermission()
        }

        /// <summary>
        /// will authenticate user and if success then must
        ///  call setIsAuthenticated(userName, userDBID, alowedActions)
        ///  so that from now on IsLogedIn() == true
        /// </summary>
        public abstract void authenticate(string userName, string password);  //check wholesale for an implementaion using our application server
        public abstract void doNotLoggedIn();


        protected void setIsAuthenticated(string userName, int? userDBID, EList<string> alowedActions)
        {
            this.cookie.login(userName, userDBID, alowedActions);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////

        public string scriptName
        {
            get { return this._scriptName; }
            set { this._scriptName = value; }
        }

        protected string passwordHash(string password)
        {
            string hash = "";
            foreach (char c in password)
            {
                hash += (char)((int)c + 7);
            }
            return hash;
        }

        public EList<string> alowedActions()
        {
            return cookie.alowedActions;
        }

        public bool isAlowed(string action)
        {
            return this.alowedActions().Contains(action);
        }

        /// <summary>
        /// is scriptName in the list of alowed actions?
        /// </summary>
        public bool isAlowed()
        {
            return this.alowedActions().Contains(this.scriptName);
        }

        public virtual string userName
        {
            get { return this.cookie.userName; }
        }

        public virtual int? userDBID
        {
            get { return this.cookie.userDBID; }
        }

        public void logout()
        {
            this.cookie.logout();
        }
        
        public bool isLoggedIn()
        {
            return this.cookie.isLoggedIn();
        }
        
        public void handleLogin()
        {
            if (!this.isLoggedIn())
            {
                this.doNotLoggedIn();
            }
        }

        



    }
}