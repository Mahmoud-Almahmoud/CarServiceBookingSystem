using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class AiServiceCatalogQuery : IAiServiceCatalogQuery
{
    private readonly ApplicationDbContext _context;

    public AiServiceCatalogQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AiServiceCatalogItemDto>> GetActiveServicesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .AsNoTracking()

            // Add this only if your Service entity has IsActive.
            // .Where(x => x.IsActive)

            .OrderBy(x => x.Name)
            .Select(x => new AiServiceCatalogItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,

                // Change this based on your real entity property.
                DurationInMinutes = x.DurationInMinutes,

                // If your Service entity does not have Description, use:
                // Description = null
                Description = x.Description
            })
            .ToListAsync(cancellationToken);
    }
}