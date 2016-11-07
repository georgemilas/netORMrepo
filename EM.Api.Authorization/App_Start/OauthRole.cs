using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurePayroll.Api.Authorization
{
    public class OauthRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }
}