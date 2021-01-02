using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses
{
    public class Tokens
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
