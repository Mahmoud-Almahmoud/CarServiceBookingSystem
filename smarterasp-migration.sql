IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [CarBrands] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CarBrands] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [Token] nvarchar(max) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        [UserId] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [Services] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [DurationInMinutes] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Services] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [CarModels] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [BrandId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CarModels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarModels_CarBrands_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [CarBrands] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [CarYears] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [ModelId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CarYears] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarYears_CarModels_ModelId] FOREIGN KEY ([ModelId]) REFERENCES [CarModels] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [CarTrims] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [YearId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CarTrims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarTrims_CarYears_YearId] FOREIGN KEY ([YearId]) REFERENCES [CarYears] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [Cars] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [CarTrimId] int NOT NULL,
        [PlateNumber] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Cars] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cars_CarTrims_CarTrimId] FOREIGN KEY ([CarTrimId]) REFERENCES [CarTrims] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [Bookings] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [CarId] int NOT NULL,
        [ServiceId] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [LocationType] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bookings_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentIntentId] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_CarId] ON [Bookings] ([CarId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_ServiceId] ON [Bookings] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CarModels_BrandId] ON [CarModels] ([BrandId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Cars_CarTrimId] ON [Cars] ([CarTrimId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CarTrims_YearId] ON [CarTrims] ([YearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CarYears_ModelId] ON [CarYears] ([ModelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508095844_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508095844_InitialCreate', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Services] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Payments] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [CarYears] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Cars] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [CarModels] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    ALTER TABLE [Bookings] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154326_AddSoftDelete'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508154326_AddSoftDelete', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Services] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Services] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Services] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Services] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarYears] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarYears] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarYears] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarYears] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Cars] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Cars] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Cars] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Cars] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarModels] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarModels] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarModels] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarModels] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508154912_AddAuditFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508154912_AddAuditFields', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513113839_AddRefreshTokenReuseDetection'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [ReplacedByToken] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513113839_AddRefreshTokenReuseDetection'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [RevokedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513113839_AddRefreshTokenReuseDetection'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260513113839_AddRefreshTokenReuseDetection', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513114558_AddRefreshTokenSessionTracking'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [CreatedByIp] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513114558_AddRefreshTokenSessionTracking'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [Device] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513114558_AddRefreshTokenSessionTracking'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [RevocationReason] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513114558_AddRefreshTokenSessionTracking'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [RevokedByIp] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513114558_AddRefreshTokenSessionTracking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260513114558_AddRefreshTokenSessionTracking', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513172630_AddSecurityAuditLogs'
)
BEGIN
    CREATE TABLE [SecurityAuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [EventType] nvarchar(max) NOT NULL,
        [IpAddress] nvarchar(max) NULL,
        [Device] nvarchar(max) NULL,
        [Details] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_SecurityAuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513172630_AddSecurityAuditLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260513172630_AddSecurityAuditLogs', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519085702_addTrustedDevice'
)
BEGIN
    CREATE TABLE [TrustedDevices] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [TokenHash] nvarchar(max) NOT NULL,
        [DeviceName] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [UserAgent] nvarchar(max) NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_TrustedDevices] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519085702_addTrustedDevice'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519085702_addTrustedDevice', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519175448_AddGeoLocationToSecurityAuditLogs'
)
BEGIN
    ALTER TABLE [SecurityAuditLogs] ADD [City] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519175448_AddGeoLocationToSecurityAuditLogs'
)
BEGIN
    ALTER TABLE [SecurityAuditLogs] ADD [Country] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519175448_AddGeoLocationToSecurityAuditLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519175448_AddGeoLocationToSecurityAuditLogs', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520090402_AddDeviceFingerprintHash'
)
BEGIN
    ALTER TABLE [TrustedDevices] ADD [DeviceFingerprintHash] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520090402_AddDeviceFingerprintHash'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [DeviceFingerprintHash] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520090402_AddDeviceFingerprintHash'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520090402_AddDeviceFingerprintHash', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520094818_AddRefreshTokenFamilyTree'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [ParentTokenId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520094818_AddRefreshTokenFamilyTree'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [ReplacedByTokenId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520094818_AddRefreshTokenFamilyTree'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [TokenFamilyId] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520094818_AddRefreshTokenFamilyTree'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520094818_AddRefreshTokenFamilyTree', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521163551_AddApiKeys'
)
BEGIN
    CREATE TABLE [ApiKeys] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [KeyHash] nvarchar(450) NOT NULL,
        [Owner] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [ExpiresAt] datetime2 NULL,
        [LastUsedAt] datetime2 NULL,
        [LastUsedIp] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_ApiKeys] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521163551_AddApiKeys'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ApiKeys_KeyHash] ON [ApiKeys] ([KeyHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521163551_AddApiKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260521163551_AddApiKeys', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527083217_AddIdempotencyKeys'
)
BEGIN
    CREATE TABLE [IdempotencyKeys] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(200) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Endpoint] nvarchar(300) NOT NULL,
        [RequestHash] nvarchar(128) NOT NULL,
        [StatusCode] int NULL,
        [ResponseBody] nvarchar(max) NULL,
        [IsCompleted] bit NOT NULL,
        [CompletedAtUtc] datetime2 NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_IdempotencyKeys] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527083217_AddIdempotencyKeys'
)
BEGIN
    CREATE INDEX [IX_IdempotencyKeys_ExpiresAtUtc] ON [IdempotencyKeys] ([ExpiresAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527083217_AddIdempotencyKeys'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IdempotencyKeys_UserId_Key] ON [IdempotencyKeys] ([UserId], [Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527083217_AddIdempotencyKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527083217_AddIdempotencyKeys', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527193600_AddStripeWebhookEvents'
)
BEGIN
    CREATE TABLE [StripeWebhookEvents] (
        [Id] int NOT NULL IDENTITY,
        [StripeEventId] nvarchar(200) NOT NULL,
        [EventType] nvarchar(200) NOT NULL,
        [Processed] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [ProcessedAtUtc] datetime2 NULL,
        CONSTRAINT [PK_StripeWebhookEvents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527193600_AddStripeWebhookEvents'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StripeWebhookEvents_StripeEventId] ON [StripeWebhookEvents] ([StripeEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527193600_AddStripeWebhookEvents'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527193600_AddStripeWebhookEvents', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527204831_UpdatePaymentAddPaidAt'
)
BEGIN
    ALTER TABLE [Payments] ADD [PaidAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527204831_UpdatePaymentAddPaidAt'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527204831_UpdatePaymentAddPaidAt', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529094036_updateSecurityAuditLogEntity'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SecurityAuditLog]') AND [c].[name] = N'EventType');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [SecurityAuditLog] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [SecurityAuditLog] ALTER COLUMN [EventType] int NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529094036_updateSecurityAuditLogEntity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260529094036_updateSecurityAuditLogEntity', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    ALTER TABLE [Services] ADD [Description] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    ALTER TABLE [Services] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    ALTER TABLE [Cars] ADD [CarName] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE TABLE [ServicePriceRules] (
        [Id] int NOT NULL IDENTITY,
        [ServiceId] int NOT NULL,
        [CarBrandId] int NULL,
        [CarModelId] int NULL,
        [CarYearId] int NULL,
        [CarTrimId] int NULL,
        [Price] decimal(18,2) NOT NULL,
        [DurationMinutes] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ServicePriceRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServicePriceRules_CarBrands_CarBrandId] FOREIGN KEY ([CarBrandId]) REFERENCES [CarBrands] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServicePriceRules_CarModels_CarModelId] FOREIGN KEY ([CarModelId]) REFERENCES [CarModels] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServicePriceRules_CarTrims_CarTrimId] FOREIGN KEY ([CarTrimId]) REFERENCES [CarTrims] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServicePriceRules_CarYears_CarYearId] FOREIGN KEY ([CarYearId]) REFERENCES [CarYears] ([Id]),
        CONSTRAINT [FK_ServicePriceRules_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE INDEX [IX_ServicePriceRules_CarBrandId] ON [ServicePriceRules] ([CarBrandId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE INDEX [IX_ServicePriceRules_CarModelId] ON [ServicePriceRules] ([CarModelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE INDEX [IX_ServicePriceRules_CarTrimId] ON [ServicePriceRules] ([CarTrimId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE INDEX [IX_ServicePriceRules_CarYearId] ON [ServicePriceRules] ([CarYearId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE INDEX [IX_ServicePriceRules_ServiceId] ON [ServicePriceRules] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    CREATE INDEX [IX_ServicePriceRules_ServiceId_CarBrandId_CarModelId_CarTrimId_CarYearId_IsActive] ON [ServicePriceRules] ([ServiceId], [CarBrandId], [CarModelId], [CarTrimId], [CarYearId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530092436_updateCarsEntites'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260530092436_updateCarsEntites', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530111601_AddServiceAreaRules'
)
BEGIN
    CREATE TABLE [ServiceAreaRules] (
        [Id] int NOT NULL IDENTITY,
        [ServiceId] int NULL,
        [CountryCode] nvarchar(10) NOT NULL,
        [City] nvarchar(100) NULL,
        [IsAllowed] bit NOT NULL,
        [Priority] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ServiceAreaRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceAreaRules_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530111601_AddServiceAreaRules'
)
BEGIN
    CREATE INDEX [IX_ServiceAreaRules_Priority] ON [ServiceAreaRules] ([Priority]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530111601_AddServiceAreaRules'
)
BEGIN
    CREATE INDEX [IX_ServiceAreaRules_ServiceId_CountryCode_City_IsActive] ON [ServiceAreaRules] ([ServiceId], [CountryCode], [City], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530111601_AddServiceAreaRules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260530111601_AddServiceAreaRules', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CustomerCity] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CustomerCountryCode] nvarchar(10) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CustomerLatitude] decimal(10,7) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CustomerLongitude] decimal(10,7) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [DistanceKm] float(10) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [EstimatedTravelTimeMinutes] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [ServiceAreaRuleId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [ServicePrice] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [ServicePriceRuleId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [TotalPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [TravelFee] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530124826_AddBookingQuoteSnapshotFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260530124826_AddBookingQuoteSnapshotFields', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] DROP CONSTRAINT [FK_Payments_Bookings_BookingId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    EXEC sp_rename N'[StripeWebhookEvents].[ProcessedAtUtc]', N'UpdatedAt', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    EXEC sp_rename N'[StripeWebhookEvents].[CreatedAtUtc]', N'CreatedAt', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [ErrorMessage] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [Payload] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [ProcessedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [Status] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'PaymentIntentId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Payments] ALTER COLUMN [PaymentIntentId] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD [Currency] nvarchar(10) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD [DeclineCode] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD [FailureCode] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD [FailureReason] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD [StripeClientSecret] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD [UserId] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Payments_PaymentIntentId] ON [Payments] ([PaymentIntentId]) WHERE [PaymentIntentId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260530184413_UpdatePaymentsForBookingTotalPrice'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260530184413_UpdatePaymentsForBookingTotalPrice', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531071744_AddCustomerFullAdressInBooking'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CustomerFormattedAddress] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531071744_AddCustomerFullAdressInBooking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260531071744_AddCustomerFullAdressInBooking', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    ALTER TABLE [Bookings] ADD [ServiceBranchId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE TABLE [ServiceBranches] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [CountryCode] nvarchar(10) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [Latitude] decimal(10,7) NOT NULL,
        [Longitude] decimal(10,7) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ServiceBranches] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE TABLE [BranchServices] (
        [Id] int NOT NULL IDENTITY,
        [ServiceBranchId] int NOT NULL,
        [ServiceId] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_BranchServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchServices_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BranchServices_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE INDEX [IX_Bookings_ServiceBranchId] ON [Bookings] ([ServiceBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BranchServices_ServiceBranchId_ServiceId] ON [BranchServices] ([ServiceBranchId], [ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE INDEX [IX_BranchServices_ServiceId] ON [BranchServices] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE INDEX [IX_ServiceBranches_CountryCode_City_IsActive] ON [ServiceBranches] ([CountryCode], [City], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    CREATE INDEX [IX_ServiceBranches_Name] ON [ServiceBranches] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    ALTER TABLE [Bookings] ADD CONSTRAINT [FK_Bookings_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531091454_AddServiceBranches'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260531091454_AddServiceBranches', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531095522_AddBranchWorkingHours'
)
BEGIN
    CREATE TABLE [BranchWorkingHours] (
        [Id] int NOT NULL IDENTITY,
        [ServiceBranchId] int NOT NULL,
        [DayOfWeek] int NOT NULL,
        [OpenTime] time NOT NULL,
        [CloseTime] time NOT NULL,
        [IsClosed] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_BranchWorkingHours] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchWorkingHours_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531095522_AddBranchWorkingHours'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BranchWorkingHours_ServiceBranchId_DayOfWeek] ON [BranchWorkingHours] ([ServiceBranchId], [DayOfWeek]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531095522_AddBranchWorkingHours'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260531095522_AddBranchWorkingHours', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531103858_AddBranchClosures'
)
BEGIN
    CREATE TABLE [BranchClosures] (
        [Id] int NOT NULL IDENTITY,
        [ServiceBranchId] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [IsFullDay] bit NOT NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [Type] int NOT NULL,
        [Reason] nvarchar(300) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_BranchClosures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchClosures_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531103858_AddBranchClosures'
)
BEGIN
    CREATE INDEX [IX_BranchClosures_ServiceBranchId_StartDate_EndDate_IsActive] ON [BranchClosures] ([ServiceBranchId], [StartDate], [EndDate], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531103858_AddBranchClosures'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260531103858_AddBranchClosures', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531111536_AddBranchCapacityRules'
)
BEGIN
    CREATE TABLE [BranchCapacityRules] (
        [Id] int NOT NULL IDENTITY,
        [ServiceBranchId] int NOT NULL,
        [DayOfWeek] int NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [Capacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_BranchCapacityRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BranchCapacityRules_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531111536_AddBranchCapacityRules'
)
BEGIN
    CREATE INDEX [IX_BranchCapacityRules_ServiceBranchId_DayOfWeek_StartTime_EndTime_IsActive] ON [BranchCapacityRules] ([ServiceBranchId], [DayOfWeek], [StartTime], [EndTime], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531111536_AddBranchCapacityRules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260531111536_AddBranchCapacityRules', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    ALTER TABLE [Bookings] ADD [TechnicianId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE TABLE [Technicians] (
        [Id] int NOT NULL IDENTITY,
        [ServiceBranchId] int NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [Email] nvarchar(150) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Technicians] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Technicians_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE TABLE [TechnicianServices] (
        [Id] int NOT NULL IDENTITY,
        [TechnicianId] int NOT NULL,
        [ServiceId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TechnicianServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TechnicianServices_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TechnicianServices_Technicians_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [Technicians] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE TABLE [TechnicianUnavailableDates] (
        [Id] int NOT NULL IDENTITY,
        [TechnicianId] int NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [Reason] nvarchar(300) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TechnicianUnavailableDates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TechnicianUnavailableDates_Technicians_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [Technicians] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE TABLE [TechnicianWorkingHours] (
        [Id] int NOT NULL IDENTITY,
        [TechnicianId] int NOT NULL,
        [DayOfWeek] int NOT NULL,
        [OpenTime] time NOT NULL,
        [CloseTime] time NOT NULL,
        [IsClosed] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TechnicianWorkingHours] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TechnicianWorkingHours_Technicians_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [Technicians] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE INDEX [IX_Bookings_TechnicianId] ON [Bookings] ([TechnicianId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE INDEX [IX_Technicians_ServiceBranchId] ON [Technicians] ([ServiceBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE INDEX [IX_Technicians_ServiceBranchId_IsActive] ON [Technicians] ([ServiceBranchId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE INDEX [IX_TechnicianServices_ServiceId] ON [TechnicianServices] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TechnicianServices_TechnicianId_ServiceId] ON [TechnicianServices] ([TechnicianId], [ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE INDEX [IX_TechnicianUnavailableDates_TechnicianId_StartDate_EndDate_IsActive] ON [TechnicianUnavailableDates] ([TechnicianId], [StartDate], [EndDate], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TechnicianWorkingHours_TechnicianId_DayOfWeek] ON [TechnicianWorkingHours] ([TechnicianId], [DayOfWeek]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    ALTER TABLE [Bookings] ADD CONSTRAINT [FK_Bookings_Technicians_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [Technicians] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601054638_AddTechnicians'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260601054638_AddTechnicians', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602085844_AddBookingCancellationFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CancellationReason] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602085844_AddBookingCancellationFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CancelledAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602085844_AddBookingCancellationFields'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CancelledByUserId] nvarchar(450) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602085844_AddBookingCancellationFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260602085844_AddBookingCancellationFields', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602173502_AddPaymentRefundFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [RefundFailureReason] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602173502_AddPaymentRefundFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [RefundedAmount] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602173502_AddPaymentRefundFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [RefundedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602173502_AddPaymentRefundFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [StripeRefundId] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602173502_AddPaymentRefundFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260602173502_AddPaymentRefundFields', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604064443_AddCancellationPolicyRules'
)
BEGIN
    CREATE TABLE [CancellationPolicyRules] (
        [Id] int NOT NULL IDENTITY,
        [ServiceId] int NULL,
        [ServiceBranchId] int NULL,
        [HoursBeforeStart] int NOT NULL,
        [RefundPercentage] decimal(5,2) NOT NULL,
        [Description] nvarchar(300) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CancellationPolicyRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CancellationPolicyRules_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CancellationPolicyRules_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604064443_AddCancellationPolicyRules'
)
BEGIN
    CREATE INDEX [IX_CancellationPolicyRules_ServiceBranchId] ON [CancellationPolicyRules] ([ServiceBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604064443_AddCancellationPolicyRules'
)
BEGIN
    CREATE INDEX [IX_CancellationPolicyRules_ServiceId_ServiceBranchId_HoursBeforeStart_IsActive] ON [CancellationPolicyRules] ([ServiceId], [ServiceBranchId], [HoursBeforeStart], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604064443_AddCancellationPolicyRules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260604064443_AddCancellationPolicyRules', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    ALTER TABLE [Bookings] ADD [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    ALTER TABLE [Bookings] ADD [PromoCodeId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    ALTER TABLE [Bookings] ADD [PromoCodeSnapshot] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    ALTER TABLE [Bookings] ADD [SubtotalPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE TABLE [PromoCodes] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Description] nvarchar(300) NULL,
        [DiscountType] int NOT NULL,
        [DiscountValue] decimal(18,2) NOT NULL,
        [MaxDiscountAmount] decimal(18,2) NULL,
        [MinimumSubtotalAmount] decimal(18,2) NULL,
        [ServiceId] int NULL,
        [ServiceBranchId] int NULL,
        [StartsAt] datetime2 NULL,
        [ExpiresAt] datetime2 NULL,
        [MaxTotalRedemptions] int NULL,
        [MaxRedemptionsPerUser] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PromoCodes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromoCodes_ServiceBranches_ServiceBranchId] FOREIGN KEY ([ServiceBranchId]) REFERENCES [ServiceBranches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromoCodes_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE TABLE [PromoCodeRedemptions] (
        [Id] int NOT NULL IDENTITY,
        [PromoCodeId] int NOT NULL,
        [BookingId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PromoCodeRedemptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromoCodeRedemptions_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PromoCodeRedemptions_PromoCodes_PromoCodeId] FOREIGN KEY ([PromoCodeId]) REFERENCES [PromoCodes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE INDEX [IX_Bookings_PromoCodeId] ON [Bookings] ([PromoCodeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PromoCodeRedemptions_BookingId] ON [PromoCodeRedemptions] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE INDEX [IX_PromoCodeRedemptions_PromoCodeId] ON [PromoCodeRedemptions] ([PromoCodeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE INDEX [IX_PromoCodeRedemptions_PromoCodeId_UserId] ON [PromoCodeRedemptions] ([PromoCodeId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PromoCodes_Code] ON [PromoCodes] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE INDEX [IX_PromoCodes_ServiceBranchId] ON [PromoCodes] ([ServiceBranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    CREATE INDEX [IX_PromoCodes_ServiceId_ServiceBranchId_IsActive] ON [PromoCodes] ([ServiceId], [ServiceBranchId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    ALTER TABLE [Bookings] ADD CONSTRAINT [FK_Bookings_PromoCodes_PromoCodeId] FOREIGN KEY ([PromoCodeId]) REFERENCES [PromoCodes] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260604072908_AddPromoCodes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260604072908_AddPromoCodes', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605070633_AddBookingReviews'
)
BEGIN
    CREATE TABLE [BookingReviews] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(1000) NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_BookingReviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingReviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605070633_AddBookingReviews'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BookingReviews_BookingId] ON [BookingReviews] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605070633_AddBookingReviews'
)
BEGIN
    CREATE INDEX [IX_BookingReviews_Rating_IsVisible_CreatedAt] ON [BookingReviews] ([Rating], [IsVisible], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605070633_AddBookingReviews'
)
BEGIN
    CREATE INDEX [IX_BookingReviews_UserId] ON [BookingReviews] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605070633_AddBookingReviews'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605070633_AddBookingReviews', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianWorkingHours] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianWorkingHours] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianUnavailableDates] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianUnavailableDates] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianServices] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianServices] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Technicians] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Technicians] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Services] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Services] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServicePriceRules] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServicePriceRules] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceBranches] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceBranches] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceAreaRules] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceAreaRules] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodes] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodes] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodeRedemptions] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodeRedemptions] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Payments] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Payments] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [CreatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [DeletedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [UpdatedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ADD [UpdatedBy] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarYears] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarYears] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarTrims] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Cars] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Cars] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarModels] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarModels] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarBrands] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CancellationPolicyRules] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CancellationPolicyRules] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchWorkingHours] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchWorkingHours] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchServices] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchServices] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchClosures] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchClosures] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchCapacityRules] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchCapacityRules] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Bookings] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Bookings] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BookingReviews] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BookingReviews] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ApiKeys] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ApiKeys] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [TechnicianWorkingHours] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianWorkingHours] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianWorkingHours] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [TechnicianUnavailableDates] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianUnavailableDates] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianUnavailableDates] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [TechnicianServices] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianServices] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [TechnicianServices] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [Technicians] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Technicians] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Technicians] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [StripeWebhookEvents] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [Services] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Services] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Services] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [ServicePriceRules] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServicePriceRules] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServicePriceRules] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [ServiceBranches] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceBranches] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceBranches] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [ServiceAreaRules] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceAreaRules] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ServiceAreaRules] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [PromoCodes] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodes] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodes] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [PromoCodeRedemptions] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodeRedemptions] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [PromoCodeRedemptions] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [Payments] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Payments] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Payments] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [IdempotencyKeys] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [CarYears] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarYears] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarYears] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [CarTrims] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarTrims] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarTrims] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [Cars] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Cars] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Cars] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [CarModels] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarModels] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarModels] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [CarBrands] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarBrands] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CarBrands] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [CancellationPolicyRules] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CancellationPolicyRules] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [CancellationPolicyRules] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [BranchWorkingHours] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchWorkingHours] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchWorkingHours] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [BranchServices] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchServices] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchServices] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [BranchClosures] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchClosures] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchClosures] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [BranchCapacityRules] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchCapacityRules] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BranchCapacityRules] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [Bookings] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Bookings] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [Bookings] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [BookingReviews] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BookingReviews] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [BookingReviews] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    EXEC(N'ALTER TABLE [ApiKeys] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])')
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ApiKeys] ALTER COLUMN [PeriodStart] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    ALTER TABLE [ApiKeys] ALTER COLUMN [PeriodEnd] ADD HIDDEN
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema2 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [TechnicianWorkingHours] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema2 + '.[TechnicianWorkingHoursHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema3 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [TechnicianUnavailableDates] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema3 + '.[TechnicianUnavailableDatesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema4 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [TechnicianServices] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema4 + '.[TechnicianServicesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema5 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [Technicians] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema5 + '.[TechniciansHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema6 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [StripeWebhookEvents] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema6 + '.[StripeWebhookEventsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema7 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [Services] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema7 + '.[ServicesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema8 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [ServicePriceRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema8 + '.[ServicePriceRulesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema9 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [ServiceBranches] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema9 + '.[ServiceBranchesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema10 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [ServiceAreaRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema10 + '.[ServiceAreaRulesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema11 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [PromoCodes] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema11 + '.[PromoCodesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema12 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [PromoCodeRedemptions] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema12 + '.[PromoCodeRedemptionsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema13 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [Payments] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema13 + '.[PaymentsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema14 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [IdempotencyKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema14 + '.[IdempotencyKeysHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema15 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [CarYears] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema15 + '.[CarYearsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema16 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [CarTrims] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema16 + '.[CarTrimsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema17 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [Cars] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema17 + '.[CarsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema18 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [CarModels] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema18 + '.[CarModelsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema19 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [CarBrands] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema19 + '.[CarBrandsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema20 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [CancellationPolicyRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema20 + '.[CancellationPolicyRulesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema21 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [BranchWorkingHours] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema21 + '.[BranchWorkingHoursHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema22 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [BranchServices] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema22 + '.[BranchServicesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema23 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [BranchClosures] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema23 + '.[BranchClosuresHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema24 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [BranchCapacityRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema24 + '.[BranchCapacityRulesHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema25 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [Bookings] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema25 + '.[BookingsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema26 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [BookingReviews] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema26 + '.[BookingReviewsHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    DECLARE @historyTableSchema27 nvarchar(max) = QUOTENAME(SCHEMA_NAME())
    EXEC(N'ALTER TABLE [ApiKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ' + @historyTableSchema27 + '.[ApiKeyHistory]))')

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609110339_EnableTemporalTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609110339_EnableTemporalTables', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    IF SCHEMA_ID(N'security') IS NULL EXEC(N'CREATE SCHEMA [security];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER TABLE [ApiKeys] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [ApiKeys];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    IF SCHEMA_ID(N'auditing') IS NULL EXEC(N'CREATE SCHEMA [auditing];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [TechnicianWorkingHoursHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [TechnicianUnavailableDatesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [TechnicianServicesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [TechniciansHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [StripeWebhookEventsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [ServicesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [ServicePriceRulesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [ServiceBranchesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [ServiceAreaRulesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [PromoCodesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [PromoCodeRedemptionsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [PaymentsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [IdempotencyKeysHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [CarYearsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [CarTrimsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [CarsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [CarModelsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [CarBrandsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [CancellationPolicyRulesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [BranchWorkingHoursHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [BranchServicesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [BranchClosuresHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [BranchCapacityRulesHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [BookingsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER SCHEMA [auditing] TRANSFER [BookingReviewsHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    EXEC sp_rename N'[ApiKeyHistory]', N'ApiKeysHistory', 'OBJECT';
    ALTER SCHEMA [auditing] TRANSFER [ApiKeysHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    ALTER TABLE [security].[ApiKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[ApiKeysHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609111631_AddSchemaNamesToAudits'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609111631_AddSchemaNamesToAudits', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetRoleClaims] DROP CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserClaims] DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [PK_AspNetUserTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [PK_AspNetUserRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [PK_AspNetUserLogins];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetUserClaims] DROP CONSTRAINT [PK_AspNetUserClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetRoles] DROP CONSTRAINT [PK_AspNetRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [AspNetRoleClaims] DROP CONSTRAINT [PK_AspNetRoleClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[AspNetUserTokens]', N'UserTokens', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [UserTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[AspNetUserRoles]', N'UserRoles', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [UserRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[AspNetUserLogins]', N'UserLogins', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [UserLogins];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[AspNetUserClaims]', N'UserClaims', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [UserClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[AspNetRoles]', N'Roles', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [Roles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[AspNetRoleClaims]', N'RoleClaims', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [RoleClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[security].[UserRoles].[IX_AspNetUserRoles_RoleId]', N'IX_UserRoles_RoleId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[security].[UserLogins].[IX_AspNetUserLogins_UserId]', N'IX_UserLogins_UserId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[security].[UserClaims].[IX_AspNetUserClaims_UserId]', N'IX_UserClaims_UserId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    EXEC sp_rename N'[security].[RoleClaims].[IX_AspNetRoleClaims_RoleId]', N'IX_RoleClaims_RoleId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    IF SCHEMA_ID(N'audit') IS NULL EXEC(N'CREATE SCHEMA [audit];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER SCHEMA [audit] TRANSFER [auditing].[ApiKeysHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserTokens] ADD CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserRoles] ADD CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserLogins] ADD CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserClaims] ADD CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[Roles] ADD CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[RoleClaims] ADD CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[RoleClaims] ADD CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [security].[Roles] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserClaims] ADD CONSTRAINT [FK_UserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserLogins] ADD CONSTRAINT [FK_UserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserRoles] ADD CONSTRAINT [FK_UserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserRoles] ADD CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [security].[Roles] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    ALTER TABLE [security].[UserTokens] ADD CONSTRAINT [FK_UserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112546_ChangeASPNETTableNames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609112546_ChangeASPNETTableNames', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserClaims] DROP CONSTRAINT [FK_UserClaims_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserLogins] DROP CONSTRAINT [FK_UserLogins_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserRoles] DROP CONSTRAINT [FK_UserRoles_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserTokens] DROP CONSTRAINT [FK_UserTokens_AspNetUsers_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [PK_AspNetUsers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    EXEC sp_rename N'[AspNetUsers]', N'User', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [User];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[User] ADD CONSTRAINT [PK_User] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserClaims] ADD CONSTRAINT [FK_UserClaims_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[User] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserLogins] ADD CONSTRAINT [FK_UserLogins_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[User] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserRoles] ADD CONSTRAINT [FK_UserRoles_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[User] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    ALTER TABLE [security].[UserTokens] ADD CONSTRAINT [FK_UserTokens_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[User] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609112917_UpdateUsersTableName'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609112917_UpdateUsersTableName', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113204_UpdateRefreshTokenTableName'
)
BEGIN
    ALTER TABLE [RefreshTokens] DROP CONSTRAINT [PK_RefreshTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113204_UpdateRefreshTokenTableName'
)
BEGIN
    EXEC sp_rename N'[RefreshTokens]', N'RefreshToken', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [RefreshToken];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113204_UpdateRefreshTokenTableName'
)
BEGIN
    ALTER TABLE [security].[RefreshToken] ADD CONSTRAINT [PK_RefreshToken] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113204_UpdateRefreshTokenTableName'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609113204_UpdateRefreshTokenTableName', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    ALTER TABLE [TrustedDevices] DROP CONSTRAINT [PK_TrustedDevices];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    ALTER TABLE [SecurityAuditLogs] DROP CONSTRAINT [PK_SecurityAuditLogs];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    EXEC sp_rename N'[TrustedDevices]', N'TrustedDevice', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [TrustedDevice];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    EXEC sp_rename N'[SecurityAuditLogs]', N'SecurityAuditLog', 'OBJECT';
    ALTER SCHEMA [security] TRANSFER [SecurityAuditLog];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    ALTER TABLE [security].[TrustedDevice] ADD CONSTRAINT [PK_TrustedDevice] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    ALTER TABLE [security].[SecurityAuditLog] ADD CONSTRAINT [PK_SecurityAuditLog] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609113440_AddSecuritySchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609113440_AddSecuritySchema', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    IF SCHEMA_ID(N'booking') IS NULL EXEC(N'CREATE SCHEMA [booking];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    IF SCHEMA_ID(N'service') IS NULL EXEC(N'CREATE SCHEMA [service];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    IF SCHEMA_ID(N'finance') IS NULL EXEC(N'CREATE SCHEMA [finance];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    IF SCHEMA_ID(N'provider') IS NULL EXEC(N'CREATE SCHEMA [provider];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[UserTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[UserRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[UserLogins];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[UserClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[User];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[TrustedDevice];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [TechnicianWorkingHours] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [provider] TRANSFER [TechnicianWorkingHours];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [TechnicianUnavailableDates] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [provider] TRANSFER [TechnicianUnavailableDates];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [TechnicianServices] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [provider] TRANSFER [TechnicianServices];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [Technicians] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [provider] TRANSFER [Technicians];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [StripeWebhookEvents] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [finance] TRANSFER [StripeWebhookEvents];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [Services] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [Services];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [ServicePriceRules] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [ServicePriceRules];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [ServiceBranches] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [ServiceBranches];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [ServiceAreaRules] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [ServiceAreaRules];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[SecurityAuditLog];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[Roles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[RoleClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[RefreshToken];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [PromoCodes] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [finance] TRANSFER [PromoCodes];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [PromoCodeRedemptions] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [finance] TRANSFER [PromoCodeRedemptions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [Payments] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [finance] TRANSFER [Payments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [finance] TRANSFER [IdempotencyKeys];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [CarYears] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [CarYears];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [CarTrims] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [CarTrims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [Cars] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [Cars];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [CarModels] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [CarModels];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [CarBrands] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [CarBrands];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [CancellationPolicyRules] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [CancellationPolicyRules];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [BranchWorkingHours] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [BranchWorkingHours];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [BranchServices] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [BranchServices];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [BranchClosures] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [BranchClosures];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [BranchCapacityRules] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [service] TRANSFER [BranchCapacityRules];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [Bookings] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [booking] TRANSFER [Bookings];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [BookingReviews] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [booking] TRANSFER [BookingReviews];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [security].[ApiKeys] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER SCHEMA [dbo] TRANSFER [security].[ApiKeys];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [provider].[TechnicianWorkingHours] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[TechnicianWorkingHoursHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [provider].[TechnicianUnavailableDates] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[TechnicianUnavailableDatesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [provider].[TechnicianServices] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[TechnicianServicesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [provider].[Technicians] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[TechniciansHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [finance].[StripeWebhookEvents] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[StripeWebhookEventsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[Services] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[ServicesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[ServicePriceRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[ServicePriceRulesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[ServiceBranches] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[ServiceBranchesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[ServiceAreaRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[ServiceAreaRulesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [finance].[PromoCodes] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[PromoCodesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [finance].[PromoCodeRedemptions] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[PromoCodeRedemptionsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [finance].[Payments] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[PaymentsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [finance].[IdempotencyKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[IdempotencyKeysHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[CarYears] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CarYearsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[CarTrims] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CarTrimsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[Cars] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CarsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[CarModels] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CarModelsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[CarBrands] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CarBrandsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [dbo].[CancellationPolicyRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CancellationPolicyRulesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[BranchWorkingHours] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[BranchWorkingHoursHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[BranchServices] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[BranchServicesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[BranchClosures] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[BranchClosuresHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [service].[BranchCapacityRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[BranchCapacityRulesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [booking].[Bookings] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[BookingsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [booking].[BookingReviews] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[BookingReviewsHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    ALTER TABLE [dbo].[ApiKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [audit].[ApiKeysHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114352_ChangeSchemaNames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609114352_ChangeSchemaNames', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    IF SCHEMA_ID(N'security') IS NULL EXEC(N'CREATE SCHEMA [security];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[UserTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[UserRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[UserLogins];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[UserClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[User];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[TrustedDevice];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[SecurityAuditLog];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[Roles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[RoleClaims];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[RefreshToken];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER TABLE [finance].[IdempotencyKeys] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    DECLARE @defaultSchema28 nvarchar(max) = QUOTENAME(SCHEMA_NAME());
    EXEC(N'ALTER SCHEMA ' + @defaultSchema28 + N' TRANSFER [finance].[IdempotencyKeys];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER TABLE [dbo].[ApiKeys] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER SCHEMA [security] TRANSFER [dbo].[ApiKeys];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[IdempotencyKeysHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    ALTER TABLE [security].[ApiKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [audit].[ApiKeysHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609114644_ChangeSchemaNames1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609114644_ChangeSchemaNames1', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115041_ChangeSchemaNames2'
)
BEGIN
    ALTER TABLE [dbo].[CancellationPolicyRules] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115041_ChangeSchemaNames2'
)
BEGIN
    ALTER SCHEMA [booking] TRANSFER [dbo].[CancellationPolicyRules];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115041_ChangeSchemaNames2'
)
BEGIN
    ALTER TABLE [booking].[CancellationPolicyRules] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[CancellationPolicyRulesHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115041_ChangeSchemaNames2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609115041_ChangeSchemaNames2', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115501_ChangeSchemaNames3'
)
BEGIN
    ALTER TABLE [IdempotencyKeys] SET (SYSTEM_VERSIONING = OFF)

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115501_ChangeSchemaNames3'
)
BEGIN
    ALTER SCHEMA [finance] TRANSFER [IdempotencyKeys];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115501_ChangeSchemaNames3'
)
BEGIN
    ALTER TABLE [finance].[IdempotencyKeys] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [auditing].[IdempotencyKeysHistory]))

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609115501_ChangeSchemaNames3'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609115501_ChangeSchemaNames3', N'10.0.7');
END;

COMMIT;
GO

