using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Persistence;

public static class CarLookupSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.CarBrands.AnyAsync())
            return;

        var toyota = new CarBrand
        {
            Name = "Toyota",
            Models =
            [
                new CarModel
                {
                    Name = "Camry",
                    Years =
                    [
                        new CarYear
                        {
                            Year = 2023,
                            Trims =
                            [
                                new CarTrim { Name = "LE" },
                                new CarTrim { Name = "SE" },
                                new CarTrim { Name = "XSE" }
                            ]
                        }
                    ]
                },
                new CarModel
                {
                    Name = "Corolla",
                    Years =
                    [
                        new CarYear
                        {
                            Year = 2023,
                            Trims =
                            [
                                new CarTrim { Name = "L" },
                                new CarTrim { Name = "LE" },
                                new CarTrim { Name = "XSE" }
                            ]
                        }
                    ]
                }
            ]
        };

        var nissan = new CarBrand
        {
            Name = "Nissan",
            Models =
            [
                new CarModel
                {
                    Name = "Altima",
                    Years =
                    [
                        new CarYear
                        {
                            Year = 2023,
                            Trims =
                            [
                                new CarTrim { Name = "S" },
                                new CarTrim { Name = "SV" },
                                new CarTrim { Name = "SL" }
                            ]
                        }
                    ]
                }
            ]
        };

        var bmw = new CarBrand
        {
            Name = "BMW",
            Models =
            [
                new CarModel
                {
                    Name = "3 Series",
                    Years =
                    [
                        new CarYear
                        {
                            Year = 2023,
                            Trims =
                            [
                                new CarTrim { Name = "330i" },
                                new CarTrim { Name = "M340i" }
                            ]
                        }
                    ]
                }
            ]
        };

        await context.CarBrands.AddRangeAsync(toyota, nissan, bmw);
        await context.SaveChangesAsync();
    }
}