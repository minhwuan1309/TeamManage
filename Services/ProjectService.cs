using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using TeamManage.Data;
using TeamManage.Models;
using TeamManage.Repositories.Interfaces;
using TeamManage.Services.Interfaces;

namespace TeamManage.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly ApplicationDbContext _context;

        public ProjectService(IProjectRepository projectRepository, AuthenticationStateProvider authProvider, ApplicationDbContext context)
        {
            _projectRepository = projectRepository;
            _authProvider = authProvider;
            _context = context;
        }

        public async Task<List<Project>> GetProjectsForCurrentUser()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _projectRepository.GetProjectsByUserAsync(userId);
        }

        public async Task<Project> CreateProject(Project project)
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (project.Members == null || !project.Members.Any())
            {
                project.Members = new List<ProjectMember>
                {
                    new ProjectMember
                    {
                        UserId = userId,
                        RoleInProject = UserRole.Dev,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
            }

            // Nếu có members, cập nhật CreatedAt, UpdatedAt
            foreach (var member in project.Members)
            {
                member.CreatedAt = DateTime.Now;
                member.UpdatedAt = DateTime.Now;
            }

            return await _projectRepository.AddProjectAsync(project);
        }

    }
}
