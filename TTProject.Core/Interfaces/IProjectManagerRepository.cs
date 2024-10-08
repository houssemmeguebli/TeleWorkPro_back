﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;

namespace TTProject.Core.Interfaces
{
    public interface IProjectManagerRepository : IRepository<ProjectManager>
    {
        Task<(string firstName, string lastName)> GetUserByNameAsync(long userID);
    }
}
