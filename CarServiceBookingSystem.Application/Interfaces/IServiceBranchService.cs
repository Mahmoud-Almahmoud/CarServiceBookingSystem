using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ServiceBranches;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IServiceBranchService
{
    Task<ApiResponse<PagedResponse<ServiceBranchResponse>>> GetBranchesAsync(
        ServiceBranchFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceBranchResponse>> GetBranchByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceBranchResponse>> CreateBranchAsync(
        CreateServiceBranchRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceBranchResponse>> UpdateBranchAsync(
        int id,
        UpdateServiceBranchRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<string>> DeleteBranchAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<BranchServiceResponse>>> GetBranchServicesAsync(
        int branchId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchServiceResponse>> AddBranchServiceAsync(
        int branchId,
        AddBranchServiceRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<string>> RemoveBranchServiceAsync(
        int branchId,
        int serviceId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceBranchSelectionResponse>> SelectNearestBranchAsync(
        int serviceId,
        decimal customerLatitude,
        decimal customerLongitude,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<BranchWorkingHourResponse>>> GetWorkingHoursAsync(
    int branchId,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<List<BranchWorkingHourResponse>>> UpdateWorkingHoursAsync(
        int branchId,
        UpdateBranchWorkingHoursRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceBranchSelectionResponse>> GetBranchForStoreBookingAsync(
        int branchId,
        int serviceId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<BranchClosureResponse>>> GetClosuresAsync(
    int branchId,
    BranchClosureFilterRequest request,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchClosureResponse>> GetClosureByIdAsync(
        int branchId,
        int closureId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchClosureResponse>> CreateClosureAsync(
        int branchId,
        CreateBranchClosureRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchClosureResponse>> UpdateClosureAsync(
        int branchId,
        int closureId,
        UpdateBranchClosureRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<string>> DeleteClosureAsync(
        int branchId,
        int closureId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<BranchCapacityRuleResponse>>> GetCapacityRulesAsync(
    int branchId,
    BranchCapacityRuleFilterRequest request,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchCapacityRuleResponse>> GetCapacityRuleByIdAsync(
        int branchId,
        int capacityRuleId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchCapacityRuleResponse>> CreateCapacityRuleAsync(
        int branchId,
        CreateBranchCapacityRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchCapacityRuleResponse>> UpdateCapacityRuleAsync(
        int branchId,
        int capacityRuleId,
        UpdateBranchCapacityRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<string>> DeleteCapacityRuleAsync(
        int branchId,
        int capacityRuleId,
        CancellationToken cancellationToken = default);

    Task<int> GetCapacityForSlotAsync(
        int branchId,
        DateTime slotStart,
        DateTime slotEnd,
        CancellationToken cancellationToken = default);
}