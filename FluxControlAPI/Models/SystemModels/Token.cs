using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.SystemModels
{
    public class Token
    {
        public int Code { get; set; }
        public string Hash { get; set; }
        public DateTime Expires { get; set; }
        public User User { get; set; }
        
        public Token(User user)
        {
            this.Hash = Guid.NewGuid().ToString();
            this.User = user;
            this.Expires = DateTime.Now.AddHours(1);
        }
    }
}
