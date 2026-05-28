using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ISecurityAuditQueryService
{
    Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetLogsAsync(SecurityAuditLogRequest request);
    Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetMyLogsAsync(string userId, PagedRequest request);
}