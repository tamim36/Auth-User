using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Users
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool isExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !isExpired;
    }
}
