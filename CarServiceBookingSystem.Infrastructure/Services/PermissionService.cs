using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Permissions;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using System.Reflection;

public sealed class PermissionService : IPermissionService
{
    public ApiResponse<List<PermissionGroupDto>> GetAll(string? group, string? search)
    {
        var groups = BuildPermissionGroups();

        if (!string.IsNullOrWhiteSpace(group))
        {
            groups = groups
                .Where(x => x.Group.Contains(group, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            groups = groups
                .Select(x => new PermissionGroupDto
                {
                    Group = x.Group,
                    Permissions = x.Permissions
                        .Where(p => p.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                })
                .Where(x => x.Permissions.Count > 0)
                .ToList();
        }

        return ApiResponse<List<PermissionGroupDto>>.Ok(groups);
    }

    private static List<PermissionGroupDto> BuildPermissionGroups()
    {
        var groups = typeof(Permissions)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .Select(group => new PermissionGroupDto
            {
                Group = group.Name,
                Permissions = group
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => f.GetRawConstantValue()?.ToString())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p!)
                    .OrderBy(p => p)
                    .ToList()
            })
            .Where(g => g.Permissions.Count > 0)
            .OrderBy(g => g.Group)
            .ToList();
        return groups;
    }
}