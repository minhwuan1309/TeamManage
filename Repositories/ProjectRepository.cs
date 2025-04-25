using Microsoft.EntityFrameworkCore;
using TeamManage.Data;
using TeamManage.Models;
using TeamManage.Repositories.Interfaces;

namespace TeamManage.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Project>> GetProjectsByUserAsync(string userId)
        {
            return await _context.Projects
                .Where(p => !p.IsDeleted && p.Members.Any(m => m.UserId == userId))
                .ToListAsync();
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }
    }
}
