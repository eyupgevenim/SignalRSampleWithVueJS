using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API
{
    public class AppSettings
    {
        public ConnectionStringsOptions ConnectionStrings { get; set; }
        public JwtOptions Jwt { get; set; }
    }

    public class ConnectionStringsOptions
    {
        public string DefaultConnection { get; set; }
    }

    public class JwtOptions
    {
        public string IssuerSigningKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int Expires { get; set; }
    }
}
