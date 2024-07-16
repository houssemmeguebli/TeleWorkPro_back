using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;

namespace TTProject.Application.Services
{
   
    public class EmployeeService : Service<Employee>, IEmployeeService
    {
        private readonly IEmplyeeRepository _emplyeeRepository;

        public EmployeeService(IEmplyeeRepository repository) : base(repository)
        {
            _emplyeeRepository = repository;
        }
        public async Task<IEnumerable<TTRequest>> GetRequestsByEmployeeIdAsync(long employeeId)
        {
            return await _emplyeeRepository.GetRequestsByEmployeeIdAsync(employeeId);
        }
    }
}

