using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;

namespace TTProject.Application.Services
{
    public class ProjectManagerService : Service<ProjectManager>, IProjectManagerService
    {
        private readonly IProjectManagerRepository _projectManagerRepository;

        public ProjectManagerService(IProjectManagerRepository projectManagerRepository) : base(projectManagerRepository)
        {
            _projectManagerRepository = projectManagerRepository;
        }

        public async Task<(string firstName, string lastName)> GetUserByNameAsync(long userID)
        {
            return await _projectManagerRepository.GetUserByNameAsync(userID);
        }
    }
}
