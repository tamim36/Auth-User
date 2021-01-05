using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class TokenSettings
    {
        public string TokenSecretKey { get; set; }
        public int JwtTokenExpires { get; set; }
        public int RefreshTokenExpires { get; set; }
        public int RefreshTokenTTL { get; set; }
    }
}
