using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;

namespace TTProject.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<(string firstName, string lastName)> GetUserByNameAsync(long userID);
    }
}
