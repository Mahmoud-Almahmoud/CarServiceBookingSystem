using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CarServiceBookingSystem.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext CreateDbContext(string? userId = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var currentUserMock = new Mock<ICurrentUserService>();

        currentUserMock.Setup(x => x.UserId)
            .Returns(userId ?? "test-user-id");

        var context = new ApplicationDbContext(
            options,
            currentUserMock.Object);

        context.Database.EnsureCreated();

        return context;
    }
}