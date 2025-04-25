using TeamManage.Models;

namespace TeamManage.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetProjectsByUserAsync(string userId);
        Task<Project> AddProjectAsync(Project project);
    }
}
