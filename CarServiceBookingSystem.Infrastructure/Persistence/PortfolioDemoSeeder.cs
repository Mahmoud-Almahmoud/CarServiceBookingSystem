using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarServiceBookingSystem.Infrastructure.Persistence;

public static class PortfolioDemoSeeder
{
    private const string UaeCountryCode = "AE";

    private static readonly int[] SupportedYears =
        Enumerable.Range(2014, 13).ToArray(); // 2014 - 2026

    public static async Task SeedPortfolioAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting portfolio demo seed...");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        await SeedCarsAsync(context, logger, cancellationToken);
        await SeedServicesAsync(context, logger, cancellationToken);
        await SeedServicePriceRulesAsync(context, logger, cancellationToken);
        await SeedServiceAreaRulesAsync(context, logger, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Portfolio demo seed completed.");
    }

    private static async Task SeedCarsAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var brandModels = GetBrandModels();

        var existingBrandNames = await context.CarBrands
            .AsNoTracking()
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var existingBrandSet = existingBrandNames
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var brandsToAdd = brandModels.Keys
            .Where(name => !existingBrandSet.Contains(name))
            .Select(name => new CarBrand
            {
                Name = name
            })
            .ToList();

        if (brandsToAdd.Count > 0)
        {
            context.CarBrands.AddRange(brandsToAdd);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded {Count} car brands.", brandsToAdd.Count);
        }

        var brands = await context.CarBrands
            .AsNoTracking()
            .Where(x => brandModels.Keys.Contains(x.Name))
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        var brandIdByName = brands.ToDictionary(
            x => x.Name,
            x => x.Id,
            StringComparer.OrdinalIgnoreCase);

        var existingModels = await context.CarModels
            .AsNoTracking()
            .Select(x => new { x.BrandId, x.Name })
            .ToListAsync(cancellationToken);

        var existingModelSet = existingModels
            .Select(x => MakeKey(x.BrandId, x.Name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var modelsToAdd = new List<CarModel>();

        foreach (var brand in brandModels)
        {
            var brandId = brandIdByName[brand.Key];

            foreach (var modelName in brand.Value.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var key = MakeKey(brandId, modelName);

                if (existingModelSet.Contains(key))
                    continue;

                modelsToAdd.Add(new CarModel
                {
                    BrandId = brandId,
                    Name = modelName
                });

                existingModelSet.Add(key);
            }
        }

        if (modelsToAdd.Count > 0)
        {
            context.CarModels.AddRange(modelsToAdd);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded {Count} car models.", modelsToAdd.Count);
        }

        var brandIds = brandIdByName.Values.ToList();

        var models = await context.CarModels
            .AsNoTracking()
            .Where(x => brandIds.Contains(x.BrandId))
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.BrandId
            })
            .ToListAsync(cancellationToken);

        var modelIds = models.Select(x => x.Id).ToList();

        var existingYears = await context.CarYears
            .AsNoTracking()
            .Where(x => modelIds.Contains(x.ModelId))
            .Select(x => new { x.ModelId, x.Year })
            .ToListAsync(cancellationToken);

        var existingYearSet = existingYears
            .Select(x => MakeKey(x.ModelId, x.Year))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var yearsToAdd = new List<CarYear>();

        foreach (var model in models)
        {
            foreach (var year in SupportedYears)
            {
                var key = MakeKey(model.Id, year);

                if (existingYearSet.Contains(key))
                    continue;

                yearsToAdd.Add(new CarYear
                {
                    ModelId = model.Id,
                    Year = year
                });

                existingYearSet.Add(key);
            }
        }

        if (yearsToAdd.Count > 0)
        {
            context.CarYears.AddRange(yearsToAdd);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded {Count} car years.", yearsToAdd.Count);
        }

        var years = await context.CarYears
            .AsNoTracking()
            .Where(x => modelIds.Contains(x.ModelId))
            .Select(x => new
            {
                x.Id,
                x.Year,
                x.ModelId
            })
            .ToListAsync(cancellationToken);

        var yearIds = years.Select(x => x.Id).ToList();

        var existingTrims = await context.CarTrims
            .AsNoTracking()
            .Where(x => yearIds.Contains(x.YearId))
            .Select(x => new { x.YearId, x.Name })
            .ToListAsync(cancellationToken);

        var existingTrimSet = existingTrims
            .Select(x => MakeKey(x.YearId, x.Name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var modelById = models.ToDictionary(x => x.Id);

        var brandNameById = brands.ToDictionary(x => x.Id, x => x.Name);

        var trimsToAdd = new List<CarTrim>();

        foreach (var year in years)
        {
            var model = modelById[year.ModelId];
            var brandName = brandNameById[model.BrandId];

            foreach (var trimName in GetTrimsForBrand(brandName))
            {
                var key = MakeKey(year.Id, trimName);

                if (existingTrimSet.Contains(key))
                    continue;

                trimsToAdd.Add(new CarTrim
                {
                    YearId = year.Id,
                    Name = trimName
                });

                existingTrimSet.Add(key);
            }
        }

        if (trimsToAdd.Count > 0)
        {
            context.CarTrims.AddRange(trimsToAdd);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded {Count} car trims.", trimsToAdd.Count);
        }
    }

    private static async Task SeedServicesAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var serviceSeeds = GetServiceSeeds();

        var existingServiceNames = await context.Services
            .AsNoTracking()
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var existingSet = existingServiceNames
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var servicesToAdd = serviceSeeds
            .Where(x => !existingSet.Contains(x.Name))
            .Select(x => new Service
            {
                Name = x.Name,
                Description = x.Description,
                Price = x.BasePrice,
                DurationInMinutes = x.DurationMinutes,
                IsActive = true
            })
            .ToList();

        if (servicesToAdd.Count == 0)
            return;

        context.Services.AddRange(servicesToAdd);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} services.", servicesToAdd.Count);
    }

    private static async Task SeedServicePriceRulesAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var services = await context.Services
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                x.DurationInMinutes
            })
            .ToListAsync(cancellationToken);

        var brands = await context.CarBrands
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync(cancellationToken);

        var existingRules = await context.ServicePriceRules
            .AsNoTracking()
            .Where(x =>
                x.CarBrandId != null &&
                x.CarModelId == null &&
                x.CarYearId == null &&
                x.CarTrimId == null)
            .Select(x => new
            {
                x.ServiceId,
                x.CarBrandId
            })
            .ToListAsync(cancellationToken);

        var existingRuleSet = existingRules
            .Select(x => MakeKey(x.ServiceId, x.CarBrandId!.Value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var multipliers = GetBrandPriceMultipliers();

        var rulesToAdd = new List<ServicePriceRule>();

        foreach (var service in services)
        {
            foreach (var brand in brands)
            {
                var key = MakeKey(service.Id, brand.Id);

                if (existingRuleSet.Contains(key))
                    continue;

                var multiplier = multipliers.TryGetValue(brand.Name, out var value)
                    ? value
                    : 1.00m;

                var price = RoundPrice(service.Price * multiplier);
                var duration = service.DurationInMinutes + GetExtraDurationMinutes(multiplier);

                rulesToAdd.Add(new ServicePriceRule
                {
                    ServiceId = service.Id,
                    CarBrandId = brand.Id,
                    CarModelId = null,
                    CarYearId = null,
                    CarTrimId = null,
                    Price = price,
                    DurationMinutes = duration,
                    IsActive = true
                });

                existingRuleSet.Add(key);
            }
        }

        if (rulesToAdd.Count == 0)
            return;

        context.ServicePriceRules.AddRange(rulesToAdd);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} service price rules.", rulesToAdd.Count);
    }

    private static async Task SeedServiceAreaRulesAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var services = await context.Services
            .AsNoTracking()
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        var existingRules = await context.ServiceAreaRules
            .AsNoTracking()
            .Select(x => new
            {
                x.ServiceId,
                x.CountryCode,
                x.City
            })
            .ToListAsync(cancellationToken);

        var existingSet = existingRules
            .Select(x => MakeAreaKey(x.ServiceId, x.CountryCode, x.City))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var cities = new[]
        {
            "Abu Dhabi",
            "Dubai",
            "Sharjah",
            "Ajman",
            "Umm Al Quwain",
            "Ras Al Khaimah",
            "Fujairah",
            "Al Ain"
        };

        var rulesToAdd = new List<ServiceAreaRule>();

        var globalCountryKey = MakeAreaKey(null, UaeCountryCode, null);

        if (!existingSet.Contains(globalCountryKey))
        {
            rulesToAdd.Add(new ServiceAreaRule
            {
                ServiceId = null,
                CountryCode = UaeCountryCode,
                City = null,
                IsAllowed = true,
                Priority = 1,
                IsActive = true
            });

            existingSet.Add(globalCountryKey);
        }

        foreach (var city in cities)
        {
            var key = MakeAreaKey(null, UaeCountryCode, city);

            if (existingSet.Contains(key))
                continue;

            rulesToAdd.Add(new ServiceAreaRule
            {
                ServiceId = null,
                CountryCode = UaeCountryCode,
                City = city,
                IsAllowed = true,
                Priority = 10,
                IsActive = true
            });

            existingSet.Add(key);
        }

        var premiumServiceNames = new[]
        {
            "Ceramic Coating",
            "Paint Protection Film",
            "Full Interior Detailing",
            "Exterior Polishing",
            "Paint Correction"
        };

        var premiumServiceIds = services
            .Where(x => premiumServiceNames.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
            .Select(x => x.Id)
            .ToList();

        var premiumCities = new[]
        {
            "Abu Dhabi",
            "Dubai",
            "Sharjah",
            "Al Ain"
        };

        foreach (var serviceId in premiumServiceIds)
        {
            foreach (var city in premiumCities)
            {
                var key = MakeAreaKey(serviceId, UaeCountryCode, city);

                if (existingSet.Contains(key))
                    continue;

                rulesToAdd.Add(new ServiceAreaRule
                {
                    ServiceId = serviceId,
                    CountryCode = UaeCountryCode,
                    City = city,
                    IsAllowed = true,
                    Priority = 50,
                    IsActive = true
                });

                existingSet.Add(key);
            }
        }

        if (rulesToAdd.Count == 0)
            return;

        context.ServiceAreaRules.AddRange(rulesToAdd);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} service area rules.", rulesToAdd.Count);
    }

    private static Dictionary<string, string[]> GetBrandModels()
    {
        return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Toyota"] = new[]
            {
                "Yaris", "Corolla", "Camry", "Avalon", "C-HR", "RAV4", "Highlander",
                "Prado", "Land Cruiser", "Fortuner", "Hilux", "Supra", "Raize"
            },
            ["Nissan"] = new[]
            {
                "Sunny", "Sentra", "Altima", "Maxima", "Kicks", "Juke", "X-Trail",
                "Pathfinder", "Patrol", "Navara", "Armada", "370Z", "GT-R"
            },
            ["Honda"] = new[]
            {
                "City", "Civic", "Accord", "HR-V", "CR-V", "Pilot", "Odyssey", "Jazz"
            },
            ["Mitsubishi"] = new[]
            {
                "Attrage", "Lancer", "ASX", "Eclipse Cross", "Outlander", "Pajero",
                "Montero Sport", "L200"
            },
            ["Hyundai"] = new[]
            {
                "Accent", "Elantra", "Sonata", "Azera", "Creta", "Tucson", "Santa Fe",
                "Palisade", "Veloster", "Kona", "Ioniq 5"
            },
            ["Kia"] = new[]
            {
                "Rio", "Pegas", "Cerato", "K5", "Stinger", "Seltos", "Sportage",
                "Sorento", "Telluride", "Carnival", "EV6", "EV9"
            },
            ["Ford"] = new[]
            {
                "Figo", "Focus", "Fusion", "Taurus", "Mustang", "EcoSport", "Escape",
                "Edge", "Explorer", "Expedition", "Bronco", "Ranger", "F-150"
            },
            ["Chevrolet"] = new[]
            {
                "Spark", "Aveo", "Cruze", "Malibu", "Camaro", "Corvette", "Trax",
                "Captiva", "Trailblazer", "Tahoe", "Suburban", "Silverado"
            },
            ["GMC"] = new[]
            {
                "Terrain", "Acadia", "Yukon", "Yukon XL", "Sierra", "Canyon"
            },
            ["Jeep"] = new[]
            {
                "Renegade", "Compass", "Cherokee", "Grand Cherokee", "Wrangler",
                "Gladiator", "Wagoneer"
            },
            ["Dodge"] = new[]
            {
                "Charger", "Challenger", "Durango", "Journey"
            },
            ["Ram"] = new[]
            {
                "1500", "2500", "3500"
            },
            ["BMW"] = new[]
            {
                "1 Series", "2 Series", "3 Series", "4 Series", "5 Series", "7 Series",
                "X1", "X2", "X3", "X4", "X5", "X6", "X7", "Z4", "i4", "iX"
            },
            ["Mercedes-Benz"] = new[]
            {
                "A-Class", "C-Class", "E-Class", "S-Class", "CLA", "CLS", "GLA",
                "GLB", "GLC", "GLE", "GLS", "G-Class", "AMG GT", "EQE", "EQS"
            },
            ["Audi"] = new[]
            {
                "A3", "A4", "A5", "A6", "A7", "A8", "Q2", "Q3", "Q5", "Q7", "Q8",
                "TT", "R8", "e-tron", "RS Q8"
            },
            ["Volkswagen"] = new[]
            {
                "Polo", "Golf", "Jetta", "Passat", "Arteon", "T-Roc", "Tiguan",
                "Touareg", "Teramont", "ID.4"
            },
            ["Porsche"] = new[]
            {
                "718", "911", "Macan", "Cayenne", "Panamera", "Taycan"
            },
            ["Lexus"] = new[]
            {
                "IS", "ES", "GS", "LS", "UX", "NX", "RX", "GX", "LX", "LC"
            },
            ["Infiniti"] = new[]
            {
                "Q30", "Q50", "Q60", "QX30", "QX50", "QX55", "QX60", "QX70", "QX80"
            },
            ["Mazda"] = new[]
            {
                "Mazda2", "Mazda3", "Mazda6", "CX-3", "CX-30", "CX-5", "CX-9",
                "CX-60", "MX-5"
            },
            ["Subaru"] = new[]
            {
                "Impreza", "Legacy", "WRX", "BRZ", "Forester", "Outback", "XV", "Ascent"
            },
            ["Suzuki"] = new[]
            {
                "Swift", "Dzire", "Baleno", "Ciaz", "Ertiga", "Jimny", "Vitara",
                "Grand Vitara"
            },
            ["Renault"] = new[]
            {
                "Symbol", "Megane", "Talisman", "Duster", "Captur", "Koleos", "Arkana"
            },
            ["Peugeot"] = new[]
            {
                "208", "308", "508", "2008", "3008", "5008", "Partner"
            },
            ["Citroen"] = new[]
            {
                "C3", "C4", "C5 Aircross", "Berlingo"
            },
            ["Skoda"] = new[]
            {
                "Fabia", "Octavia", "Superb", "Kamiq", "Karoq", "Kodiaq"
            },
            ["Volvo"] = new[]
            {
                "S60", "S90", "V60", "XC40", "XC60", "XC90", "C40"
            },
            ["Land Rover"] = new[]
            {
                "Defender", "Discovery", "Discovery Sport", "Range Rover",
                "Range Rover Sport", "Range Rover Evoque", "Range Rover Velar"
            },
            ["Jaguar"] = new[]
            {
                "XE", "XF", "XJ", "E-Pace", "F-Pace", "F-Type", "I-Pace"
            },
            ["Mini"] = new[]
            {
                "Cooper", "Cooper S", "Countryman", "Clubman", "Paceman"
            },
            ["Tesla"] = new[]
            {
                "Model 3", "Model S", "Model X", "Model Y"
            },
            ["BYD"] = new[]
            {
                "Atto 3", "Dolphin", "Seal", "Han", "Tang", "Song Plus"
            },
            ["MG"] = new[]
            {
                "MG3", "MG5", "MG6", "ZS", "HS", "RX5", "RX8", "One", "Cyberster"
            },
            ["Changan"] = new[]
            {
                "Alsvin", "Eado", "CS35 Plus", "CS55 Plus", "CS75 Plus", "UNI-T", "UNI-K", "UNI-V"
            },
            ["Geely"] = new[]
            {
                "Emgrand", "Coolray", "Azkarra", "Tugella", "Monjaro", "Geometry C"
            },
            ["Jetour"] = new[]
            {
                "X70", "X70 Plus", "X90", "Dashing", "T2"
            },
            ["GAC"] = new[]
            {
                "GS3", "GS4", "GS8", "Empow", "Emkoo", "Aion Y", "Aion S"
            },
            ["Great Wall"] = new[]
            {
                "Wingle", "Poer", "Tank 300", "Tank 500", "Haval H6", "Haval Jolion"
            },
            ["Haval"] = new[]
            {
                "H6", "Jolion", "Dargo", "H9"
            },
            ["Chery"] = new[]
            {
                "Arrizo 5", "Arrizo 6", "Tiggo 2", "Tiggo 4", "Tiggo 7", "Tiggo 8"
            },
            ["Exeed"] = new[]
            {
                "LX", "TXL", "VX", "RX"
            },
            ["Hongqi"] = new[]
            {
                "H5", "H7", "H9", "HS5", "HS7", "E-HS9"
            },
            ["Genesis"] = new[]
            {
                "G70", "G80", "G90", "GV60", "GV70", "GV80"
            },
            ["Cadillac"] = new[]
            {
                "CT4", "CT5", "CT6", "XT4", "XT5", "XT6", "Escalade"
            },
            ["Lincoln"] = new[]
            {
                "Corsair", "Nautilus", "Aviator", "Navigator"
            },
            ["Alfa Romeo"] = new[]
            {
                "Giulia", "Stelvio", "Tonale"
            },
            ["Fiat"] = new[]
            {
                "500", "Tipo", "Panda", "Doblo"
            },
            ["Opel"] = new[]
            {
                "Corsa", "Astra", "Insignia", "Mokka", "Grandland"
            },
            ["Seat"] = new[]
            {
                "Ibiza", "Leon", "Ateca", "Tarraco"
            },
            ["Maserati"] = new[]
            {
                "Ghibli", "Quattroporte", "Levante", "Grecale", "GranTurismo", "MC20"
            },
            ["Ferrari"] = new[]
            {
                "Roma", "Portofino", "California", "F8", "296", "SF90", "812", "Purosangue"
            },
            ["Lamborghini"] = new[]
            {
                "Huracan", "Aventador", "Revuelto", "Urus", "Gallardo"
            },
            ["Bentley"] = new[]
            {
                "Continental GT", "Flying Spur", "Bentayga", "Mulsanne"
            },
            ["Rolls-Royce"] = new[]
            {
                "Ghost", "Phantom", "Wraith", "Dawn", "Cullinan", "Spectre"
            },
            ["Aston Martin"] = new[]
            {
                "Vantage", "DB11", "DB12", "DBX", "DBS"
            },
            ["McLaren"] = new[]
            {
                "570S", "600LT", "720S", "750S", "GT", "Artura"
            }
        };
    }

    private static IReadOnlyList<ServiceSeed> GetServiceSeeds()
    {
        return new List<ServiceSeed>
        {
            new("Oil & Filter Change", "Engine oil replacement with oil filter inspection and basic fluid check.", 180m, 45),
            new("Full Service", "Complete scheduled maintenance with fluids, filters, brakes, tyres, battery and computer check.", 650m, 180),
            new("Minor Service", "Quick maintenance package including oil, filter, tyre pressure and visual inspection.", 350m, 90),
            new("Major Service", "Advanced maintenance package including filters, spark plugs, brake inspection and full diagnostics.", 1200m, 300),
            new("Brake Pad Replacement", "Front or rear brake pad replacement with brake inspection.", 450m, 120),
            new("Brake Disc Skimming", "Brake disc resurfacing to reduce vibration and improve braking performance.", 300m, 90),
            new("Battery Replacement", "Battery testing and replacement with charging system inspection.", 380m, 45),
            new("AC Service", "Air conditioning gas refill, leak check and cooling performance inspection.", 320m, 75),
            new("AC Deep Cleaning", "Evaporator and vent cleaning with antibacterial treatment.", 450m, 120),
            new("Engine Diagnostics", "OBD scan, fault code reading and diagnostic report.", 220m, 60),
            new("Transmission Fluid Change", "Transmission fluid replacement and gearbox health check.", 650m, 150),
            new("Spark Plug Replacement", "Spark plug replacement with ignition system inspection.", 300m, 90),
            new("Wheel Alignment", "Computerized wheel alignment adjustment.", 180m, 45),
            new("Tyre Rotation", "Tyre rotation and tyre pressure adjustment.", 100m, 30),
            new("Wheel Balancing", "Wheel balancing to reduce vibration and tyre wear.", 160m, 45),
            new("Car Wash", "Exterior wash with quick interior vacuum.", 60m, 30),
            new("Full Interior Detailing", "Interior deep cleaning including seats, carpets, dashboard and sanitization.", 550m, 240),
            new("Exterior Polishing", "Exterior paint polishing and gloss enhancement.", 700m, 240),
            new("Paint Correction", "Multi-stage paint correction for swirl marks and surface defects.", 1400m, 420),
            new("Ceramic Coating", "Paint protection ceramic coating application.", 1800m, 480),
            new("Paint Protection Film", "Paint protection film installation for selected body panels.", 3500m, 720),
            new("Pre-Purchase Inspection", "Detailed inspection before buying a used car with report.", 450m, 120),
            new("Roadside Assistance Check", "Basic roadside inspection for battery, tyres, fluids and startup issues.", 250m, 60),
            new("EV Battery Health Check", "Electric vehicle battery and charging system diagnostic report.", 500m, 120)
        };
    }

    private static string[] GetTrimsForBrand(string brandName)
    {
        var sportLuxuryBrands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "BMW", "Mercedes-Benz", "Audi", "Porsche", "Jaguar", "Land Rover",
            "Maserati", "Ferrari", "Lamborghini", "Bentley", "Rolls-Royce",
            "Aston Martin", "McLaren", "Genesis", "Cadillac", "Lincoln"
        };

        var electricBrands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Tesla", "BYD"
        };

        if (electricBrands.Contains(brandName))
        {
            return new[]
            {
                "Standard Range",
                "Long Range",
                "Performance"
            };
        }

        if (sportLuxuryBrands.Contains(brandName))
        {
            return new[]
            {
                "Standard",
                "Premium",
                "Sport",
                "Performance"
            };
        }

        return new[]
        {
            "Base",
            "Mid Option",
            "Full Option",
            "Sport"
        };
    }

    private static Dictionary<string, decimal> GetBrandPriceMultipliers()
    {
        var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var brand in new[]
        {
            "Toyota", "Nissan", "Honda", "Mitsubishi", "Hyundai", "Kia", "Mazda",
            "Suzuki", "Renault", "Peugeot", "Citroen", "Skoda", "MG", "Changan",
            "Geely", "Jetour", "GAC", "Great Wall", "Haval", "Chery", "Exeed"
        })
        {
            result[brand] = 1.00m;
        }

        foreach (var brand in new[]
        {
            "Ford", "Chevrolet", "GMC", "Jeep", "Dodge", "Ram", "Volkswagen",
            "Subaru", "Volvo", "Mini", "Hongqi", "Genesis", "Lincoln", "Cadillac",
            "Alfa Romeo", "Fiat", "Opel", "Seat"
        })
        {
            result[brand] = 1.15m;
        }

        foreach (var brand in new[]
        {
            "Lexus", "Infiniti", "BMW", "Mercedes-Benz", "Audi", "Land Rover",
            "Jaguar", "Tesla", "BYD"
        })
        {
            result[brand] = 1.45m;
        }

        foreach (var brand in new[]
        {
            "Porsche", "Maserati", "Aston Martin"
        })
        {
            result[brand] = 1.75m;
        }

        foreach (var brand in new[]
        {
            "Ferrari", "Lamborghini", "Bentley", "Rolls-Royce", "McLaren"
        })
        {
            result[brand] = 2.25m;
        }

        return result;
    }

    private static int GetExtraDurationMinutes(decimal multiplier)
    {
        if (multiplier >= 2.00m)
            return 60;

        if (multiplier >= 1.70m)
            return 45;

        if (multiplier >= 1.40m)
            return 30;

        if (multiplier >= 1.10m)
            return 15;

        return 0;
    }

    private static decimal RoundPrice(decimal value)
    {
        return Math.Ceiling(value / 5m) * 5m;
    }

    private static string MakeKey(int id, string value)
    {
        return $"{id}|{value.Trim().ToLowerInvariant()}";
    }

    private static string MakeKey(int firstId, int secondId)
    {
        return $"{firstId}|{secondId}";
    }

    private static string MakeAreaKey(int? serviceId, string countryCode, string? city)
    {
        return $"{serviceId?.ToString() ?? "global"}|{countryCode.Trim().ToUpperInvariant()}|{city?.Trim().ToLowerInvariant() ?? "all"}";
    }

    private sealed record ServiceSeed(
        string Name,
        string Description,
        decimal BasePrice,
        int DurationMinutes);
}
