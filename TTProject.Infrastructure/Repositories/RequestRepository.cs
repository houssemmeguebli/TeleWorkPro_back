using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;
using TTProject.Infrastructure.Data;

namespace TTProject.Infrastructure.Repositories
{
    public class RequestRepository : Repository<TTRequest>, IRequestRepository
    {
        public RequestRepository(TTProjectContextOld context) : base(context)
        {
        }
    }
}
