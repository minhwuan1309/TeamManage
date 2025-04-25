using TeamManage.Models;

namespace TeamManage.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<Project>> GetProjectsForCurrentUser();
        Task<Project> CreateProject(Project project);
    }
}
