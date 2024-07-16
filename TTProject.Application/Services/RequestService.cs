using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;

namespace TTProject.Application.Services
{
    public class RequestService : Service<TTRequest>, IRequestService
    {
        public RequestService(IRequestRepository repository) : base(repository)
        {
        }
    }
}
