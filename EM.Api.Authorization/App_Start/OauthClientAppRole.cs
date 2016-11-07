using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurePayroll.Api.Authorization
{
    public class OauthClientAppRole
    {
        public int clientId { get; set; }
        public int SecApplicationId { get; set; }
        public int RoleId { get; set; }
    }
}