using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResultT<string>> CreateAsync(CreateUserRequestDto requestDto)
        {
          
            var roleExists = await _roleManager.RoleExistsAsync(requestDto.Role);
            if (!roleExists)
                return ResultT<string>.Failure($"Role '{requestDto.Role}' does not exist.", ErrorType.Validation);

            var existingUser = await _userManager.FindByNameAsync(requestDto.Email);
            if (existingUser != null)
                return ResultT<string>.Failure("Email is already registered.", ErrorType.Conflict);

            var user = new IdentityUser
            {
                UserName = requestDto.Email,
                Email = requestDto.Email,
                PhoneNumber = requestDto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, requestDto.Password);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                return ResultT<string>.Failure(errorMsg, ErrorType.Validation);
            }

            await _userManager.AddToRoleAsync(user, requestDto.Role);

            return ResultT<string>.Success(user.Id, "User created successfully.");
        }

        public async Task<ResultT<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellation)
        {
            var users = await _userManager.Users.ToListAsync(cancellation);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? string.Empty
                });
            }

            return ResultT<IEnumerable<UserDto>>.Success(userDtos);
        }

        public async Task<ResultT<UserDto>> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return ResultT<UserDto>.Failure("User not found.", ErrorType.NotFound);

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return ResultT<UserDto>.Success(userDto);
        }

        public async Task<Result> UpdateAsync(string id, UpdateUserRequestDto requestDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Result.Failure("User not found.", ErrorType.NotFound);

            var roleExists = await _roleManager.RoleExistsAsync(requestDto.Role);
            if (!roleExists)
                return Result.Failure($"Role '{requestDto.Role}' does not exist.", ErrorType.Validation);

            var emailUser = await _userManager.FindByNameAsync(requestDto.Email);
            if (emailUser != null && emailUser.Id != id)
                return Result.Failure("Email is already taken by another user.", ErrorType.Conflict);

            user.Email = requestDto.Email;
            user.UserName = requestDto.Email;
            user.PhoneNumber = requestDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(errorMsg, ErrorType.Validation);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, requestDto.Role);

            return Result.Success("User updated successfully.");
        }

        public async Task<Result> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Result.Failure("User not found.", ErrorType.NotFound);


            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Result.Failure("Failed to delete user.", ErrorType.Validation);

            return Result.Success("User deleted successfully.");
        }
    }

}
