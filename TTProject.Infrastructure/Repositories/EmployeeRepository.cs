using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;
using TTProject.Infrastructure.Data;

namespace TTProject.Infrastructure.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmplyeeRepository 
    {
        private readonly TTProjectContextOld _context;

        public EmployeeRepository(TTProjectContextOld context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TTRequest>> GetRequestsByEmployeeIdAsync(long employeeId)
        {
            var requests = await _context.Requests
                .Where(r => r.EmployeeId == employeeId)
                .ToListAsync();

            if (requests == null || !requests.Any())
            {
                throw new Exception("No requests found for this employee.");
            }

            return requests; 
        }


    }
}
