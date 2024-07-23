using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TTProject.Core.Entities
{
    public enum Role
    {
        ProjectManager, Employee
    }

    public class User : IdentityUser<long>
    {
    
        public string firstName { get; set; }
        public string lastName { get; set; }
  
        public Role role { get; set; }
        public string department { get; set; }

        public IList<TTRequest>? Requests { get; set; }
    }
}