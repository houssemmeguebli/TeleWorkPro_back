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
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly TTProjectContext _context;

        public UserRepository(TTProjectContext context) : base(context)
        {
            _context = context;
        }
        public async Task<(string firstName, string lastName)> GetUserByNameAsync(long userID)
        {
            var user = await _context.Users
                    .Where(u => u.userId == userID)
                    .Select(u => new { u.firstName, u.lastName })
                    .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return (user.firstName, user.lastName);


        }



    }

}
