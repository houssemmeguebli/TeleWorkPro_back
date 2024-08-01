using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTProject.Core.Entities
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string phone { get; set; }
        public Role role { get; set; }
        public string department { get; set; }
        public string projectName { get; set; }
        public string position { get; set; }
        public Gender Gender { get; set; }

        public UserStatus UserStatus { get; set; }
        public DateTime dateOfbirth { get; set; }

    }
}
