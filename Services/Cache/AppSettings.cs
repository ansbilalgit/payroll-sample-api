using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Cache
{
    public class AppSettings
    {

        #region Jwt (Json Web Token)
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireTime { get; set; }
        #endregion

    }
}
