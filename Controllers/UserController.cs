﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamManage.Data;
using TeamManage.Models.DTO;

namespace TeamManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _user;
        public UserController(UserManager<ApplicationUser> user) => _user = user;
        [HttpGet("debug")]
        public IActionResult DebugClaims()
        {
            return Ok(new
            {
                Username = User.Identity?.Name,
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Roles = User.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value)
            });
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            var users = _user.Users
                .Where(u => !u.IsDeleted && u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Role,
                    u.Avatar,
                    u.IsActive,
                    u.IsDeleted
                });
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = _user.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Role,
                    u.Avatar,
                    u.IsActive,
                    u.IsDeleted
                }).FirstOrDefault();

            if (user == null)
                return NotFound("Người dùng không tồn tại hoặc đã bị xóa.");

            return Ok(user);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            if(string.IsNullOrWhiteSpace(userDTO.Password))
                return BadRequest("Mật khẩu là bắt buộc");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = userDTO.FullName,
                Email = userDTO.Email,
                UserName = userDTO.Email.Split('@')[0],
                Phone = userDTO.Phone,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, userDTO.Password),
                Avatar = userDTO.Avatar,
                Role = userDTO.Role,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var result = await _user.CreateAsync(user, userDTO.Password);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _user.AddToRoleAsync(user, userDTO.Role.ToString());
            return Ok(new { message = "Tạo tài khoản thành công", user });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDTO userDTO)
        {
            var user = await _user.FindByIdAsync(id);
            if(user == null || user.IsDeleted)
                return NotFound("Người dùng không tồn tại hoặc đã bị xóa.");

            if (!string.IsNullOrWhiteSpace(userDTO.FullName))
                user.FullName = userDTO.FullName;
            if (!string.IsNullOrWhiteSpace(userDTO.Phone))
                user.Phone = userDTO.Phone;
            if (!string.IsNullOrWhiteSpace(userDTO.Avatar))
                user.Avatar = userDTO.Avatar;
            if (userDTO.Role != null)
                user.Role = userDTO.Role;

            user.UpdatedAt = DateTime.Now;

            var result = await _user.UpdateAsync(user);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Cập nhật tài khoản thành công", result });
        }

        [HttpPut("toggle-block/{id}")]
        public async Task<IActionResult> ToggleBlockUser(string id)
        {
            var user = await _user.FindByIdAsync(id);
            if(user == null)
                return NotFound("Không tìm thấy người dùng.");
            
            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;

            var result = await _user.UpdateAsync(user);
            return result.Succeeded
                ? Ok(user.IsActive ? "Đã mở khóa tài khoản" : "Đã khóa tài khoản")
                : BadRequest(result.Errors);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _user.FindByIdAsync(id);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            user.IsDeleted = !user.IsDeleted;
            user.UpdatedAt = DateTime.Now;

            var result = await _user.UpdateAsync(user);

            return result.Succeeded
                ? Ok(user.IsDeleted
                    ? "Đã ẩn tài khoản người dùng."
                    : "Đã khôi phục tài khoản người dùng.")
                : BadRequest(result.Errors);
                }
    }
}
