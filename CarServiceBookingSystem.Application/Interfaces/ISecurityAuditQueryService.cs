using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ISecurityAuditQueryService
{
    Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetLogsAsync(PagedRequest request);
}