using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTProject.Core.Entities
{
    public class ProjectManager : User
    {
        public string projectName { get; set; }
        public IList<TTRequest>? Requests { get; set; }
    }
}
