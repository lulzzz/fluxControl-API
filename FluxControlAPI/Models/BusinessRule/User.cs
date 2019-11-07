using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataModels.BusinessRule
{
    public enum UserType
    {
        System = 0,
        Transparency = 1,
        Operator = 2,
        Manager = 3,
        Administrator = 4
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserType Type { get; set; }
        public string Email { get; set; }
        public int Registration { get; set; }
        public string Password { get; set; }
    }
}
