using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamManage.Data;
using TeamManage.Models;
using TeamManage.Models.DTO;

namespace TeamManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Dev,Tester")]
    public class ModuleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModuleController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetModulesByProject([FromQuery] int projectId)
        {
            var modules = await _context.Modules
                .Include(m=> m.AssignedUser)
                .Where(m => m.ProjectId == projectId && !m.IsDeleted)
                .ToListAsync();

            var result = modules.Select(m => new ModuleDTO
            {
                Id = m.Id,
                ProjectId = m.ProjectId,
                Name = m.Name,
                Status = m.Status,
                AssignedUserId = m.AssignedUserId,
                AssignedUserName = m.AssignedUser?.FullName,
                IsDeleted = m.IsDeleted,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });

            return Ok(modules);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetModule(int id)
        {
            var module = await _context.Modules
                    .Include(x => x.AssignedUser)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (module == null)
                return NotFound("Không tìm thấy module.");

            var result = new ModuleDTO
            {
                Id = module.Id,
                ProjectId = module.ProjectId,
                Name = module.Name,
                Status = module.Status,
                AssignedUserId = module.AssignedUserId,
                AssignedUserName = module.AssignedUser?.FullName,
                IsDeleted = module.IsDeleted,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };
            return Ok(module);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDTO moduleDTO)
        {
            var module = new Module
            {
                Name = moduleDTO.Name,
                ProjectId = moduleDTO.ProjectId,
                Status = moduleDTO.Status,
                AssignedUserId = moduleDTO.AssignedUserId,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return Ok(new {message ="Tạo module thành công", module});
            
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] ModuleDTO moduleDTO)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (module == null)
                return NotFound("Không tìm thấy module.");

            module.Name = moduleDTO.Name;
            module.Status = moduleDTO.Status;
            module.AssignedUserId = moduleDTO.AssignedUserId;
            module.UpdatedAt = DateTime.Now;

            _context.Modules.Update(module);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật module thành cÔng",
                module
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(x => x.Id == id);
            if (module == null)
                return NotFound("Không tìm thấy module.");

            module.IsDeleted = !module.IsDeleted;
            module.UpdatedAt = DateTime.Now;

            _context.Modules.Update(module);
            await _context.SaveChangesAsync();

            return Ok(module.IsDeleted
                ? "Đã xóa module."
                : "Đã khôi phục module.");
        }
    }
}
