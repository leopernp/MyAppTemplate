using AutoMapper;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Enums;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models;
using DamayanFS.Contract.Models.ModuleType;
using DamayanFS.Contract.Models.Role;
using DamayanFS.Contract.Models.User;
using Microsoft.Extensions.Options;
using System.Net;

namespace DamayanFS.App.Services;

public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly IMapper _mapper;

    private readonly IAuthenticationService _authenticationService;
    private readonly IUserActivityLogService _userActivityLogService;

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IModuleTypeRepository _moduleTypeRepository;

    private readonly string _defaultPassword;

    public SettingsService(
        IOptions<InitialSystemSettings> options,
        ILogger<SettingsService> logger,
        IMapper mapper,
        IAuthenticationService authenticationService,
        IUserActivityLogService userActivityLogService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IModuleTypeRepository moduleTypeRepository)
    {
        _logger = logger;
        _mapper = mapper;

        _authenticationService = authenticationService;
        _userActivityLogService = userActivityLogService;

        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _moduleTypeRepository = moduleTypeRepository;

        _defaultPassword = options.Value.DefaultPassword;
    }

    #region User Operations

    public async Task<IEnumerable<UserDto>> InquireUsersAsync(bool? isActive = null, int? roleId = null) =>
        await _userRepository.InquireAsync(isActive, roleId);

    public async Task<UserDto?> GetUserByIdAsync(int id) =>
        await _userRepository.GetByIdAsync(id);

    public async Task<UserDto?> GetUserByUsernameAsync(string username) =>
        await _userRepository.GetByUsernameAsync(username);

    public async Task<UserDto?> GetUserByEmailAsync(string email) =>
        await _userRepository.GetByEmailAsync(email);

    public async Task<UserDto> SaveUserAsync(UserUpsertModel model, int? currentUserId, string? ipAddress = null, string? browser = null)
    {
        try
        {
            var userDto = _mapper.Map<UserDto>(model);

            var validation = await _userRepository.ValidateAsync(userDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for user {Username}: {Errors}",
                    model.Username, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            if (userDto.Id == 0)
            {
                userDto.Password = _authenticationService.HashPassword(_defaultPassword);
                var created = await _userRepository.CreateAsync(userDto, currentUserId);

                await _userActivityLogService.LogAsync(
                    UserActivityAction.UserCreated,
                    $"User {created.Username} was created by {created.CreatedByDisplayName ?? currentUserId?.ToString() ?? "system"}",
                    currentUserId,
                    ipAddress,
                    browser);

                return created;
            }
            else
            {
                // Profile updates don't touch passwords here!
                var updated = await _userRepository.UpdateAsync(userDto, currentUserId);

                await _userActivityLogService.LogAsync(
                    UserActivityAction.UserUpdated,
                    $"User {updated.Username} was updated by {updated.ModifiedByDisplayName ?? currentUserId?.ToString() ?? "system"}",
                    currentUserId,
                    ipAddress,
                    browser);

                return updated;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving user {Username}", model.Username);
            throw;
        }
    }

    public async Task DeleteUserAsync(int id, string? deletedUsername, int? currentUserId, string? ipAddress = null, string? browser = null)
    {
        await _userRepository.DeleteAsync(id);

        await _userActivityLogService.LogAsync(
            UserActivityAction.UserDeleted,
            $"User {deletedUsername ?? "Unknown"} (ID: {id}) was deleted by {currentUserId?.ToString() ?? "system"}",
            currentUserId,
            ipAddress,
            browser);
    }

    public async Task ResetPasswordAsync(int id, int? currentUserId, string? ipAddress = null, string? browser = null)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            throw new InvalidOperationException($"User with ID {id} was not found.");

        var hashed = _authenticationService.HashPassword(_defaultPassword);
        await _userRepository.UpdatePasswordAsync(id, hashed);

        await _userActivityLogService.LogAsync(
            UserActivityAction.PasswordReset,
            $"Password for user {user.Username} was reset to default by {currentUserId?.ToString() ?? "system"}",
            currentUserId,
            ipAddress,
            browser);
    }

    public async Task UnlockUserAsync(int userId, int? currentUserId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new InvalidOperationException($"User with ID {userId} was not found.");

            await _userRepository.UnlockAsync(userId);

            _logger.LogInformation(
                "User {UserId} was manually unlocked by {CurrentUserId}",
                userId, currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unlocking user {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Role Operations

    public async Task<IEnumerable<RoleDto>> InquireRolesAsync(bool? isActive = null) =>
        await _roleRepository.InquireAsync(isActive);

    public async Task<RoleDto?> GetRoleByIdAsync(int id) =>
        await _roleRepository.GetByIdAsync(id);

    public async Task<RoleDto?> GetRoleByNameAsync(string name) =>
        await _roleRepository.GetByNameAsync(name);

    public async Task<RoleDto> SaveRoleAsync(RoleUpsertModel model, int? currentUserId)
    {
        try
        {
            var roleDto = _mapper.Map<RoleDto>(model);

            var validation = await _roleRepository.ValidateAsync(roleDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for role {Name}: {Errors}",
                    model.Name, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            if (roleDto.Id == 0)
                return await _roleRepository.CreateAsync(roleDto, currentUserId);
            else
                return await _roleRepository.UpdateAsync(roleDto, currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving role {Name}", model.Name);
            throw;
        }
    }

    public async Task DeleteRoleAsync(int id) =>
        await _roleRepository.DeleteAsync(id);

    #endregion

    #region ModuleType Operations

    public async Task<IEnumerable<ModuleTypeDto>> InquireModuleTypesAsync(bool? isActive = null) =>
        await _moduleTypeRepository.InquireAsync(isActive);

    public async Task<ModuleTypeDto?> GetModuleTypeByIdAsync(int id) =>
        await _moduleTypeRepository.GetByIdAsync(id);

    public async Task<ModuleTypeDto> SaveModuleTypeAsync(ModuleTypeUpsertModel model, int? currentUserId)
    {
        try
        {
            var dto = _mapper.Map<ModuleTypeDto>(model);

            var validation = await _moduleTypeRepository.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for module type {Name}: {Errors}",
                    model.Name, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            if (dto.Id == 0)
                return await _moduleTypeRepository.CreateAsync(dto, currentUserId);
            else
                return await _moduleTypeRepository.UpdateAsync(dto, currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving module type {Name}", model.Name);
            throw;
        }
    }

    public async Task DeleteModuleTypeAsync(int id) =>
        await _moduleTypeRepository.DeleteAsync(id);

    #endregion
}
