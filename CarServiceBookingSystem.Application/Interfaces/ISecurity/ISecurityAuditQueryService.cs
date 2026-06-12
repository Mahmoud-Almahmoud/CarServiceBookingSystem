using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.DTOs.Security;

namespace CarServiceBookingSystem.Application.Interfaces.ISecurity;

public interface ISecurityAuditQueryService
{
    Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetLogsAsync(SecurityAuditLogRequest request);
    Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetMyLogsAsync(string userId, MyActivityRequest request);
    Task<ApiResponse<List<SecurityAuditSummaryDto>>> GetSummaryAsync(int days);
    Task<ApiResponse<List<SecurityAuditTopCountryDto>>> GetTopCountriesAsync(int days, int take);
    Task<ApiResponse<List<SecurityAuditTopUserDto>>> GetTopUsersAsync(int days, int take);
    Task<ApiResponse<List<FailedLoginAnalyticsDto>>> GetFailedLoginsAsync(int days,int take);
    Task<ApiResponse<List<SuspiciousLoginAnalyticsDto>>> GetSuspiciousLoginsAsync(int days,int take);
    Task<ApiResponse<List<RiskyIpAnalyticsDto>>> GetRiskyIpsAsync(int days,int take);
    Task<ApiResponse<List<AccessDeniedAnalyticsDto>>> GetAccessDeniedAsync(int days,int take);
    Task<ApiResponse<List<SecurityAuditDailyTrendDto>>> GetDailyTrendAsync(int days);
    Task<ApiResponse<List<SecurityAuditHourlyRiskTrendDto>>> GetHourlyRiskTrendAsync(int hours);
    Task<ApiResponse<SecurityScoreDto>> GetSecurityScoreAsync();
}