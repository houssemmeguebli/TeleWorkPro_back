using Microsoft.EntityFrameworkCore;
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
    public class ProjectManagerRepository : Repository<ProjectManager>, IProjectManagerRepository
    {
        private readonly TTProjectContextOld _context;

        public ProjectManagerRepository(TTProjectContextOld context) : base(context)
        {
            _context = context;
        }
        public async Task<(string firstName, string lastName)> GetUserByNameAsync(long userID)
        {
            var user = await _context.Users
                    .Where(u => u.Id == userID)
                    .Select(u => new { u.firstName, u.lastName })
                    .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return (user.firstName, user.lastName);


        }
        public async Task<IEnumerable<TTRequest>> GetRequestsByManagerIdAsync(long managerId)
        {
            var requests = await _context.Requests
                .Where(r => r.ProjectManagerId == managerId)
                .ToListAsync();

            if (requests == null || !requests.Any())
            {
                throw new Exception("No requests found for this employee.");
            }

            return requests;
        }
        public async Task<IEnumerable<TTRequest>> GetRequestsByProojectManagerIdAsync(long projectManagerId)
        {
            var requests = await _context.Requests
                .Where(r => r.ProjectManagerId == projectManagerId)
                .ToListAsync();

            if (requests == null || !requests.Any())
            {
                throw new Exception("No requests found for this employee.");
            }

            return requests;
        }



    }

}
