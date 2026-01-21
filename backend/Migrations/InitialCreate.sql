-- Initial Database Migration Script for BadNews
-- This script creates all tables needed for the BadNews application

-- Users Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id] INT NOT NULL IDENTITY(1,1),
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NOT NULL,
        [Role] INT NOT NULL, -- 0 = Buyer, 1 = Messenger, 2 = Admin
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id])
    )
END

-- Messengers Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messengers')
BEGIN
    CREATE TABLE [dbo].[Messengers]
    (
        [Id] INT NOT NULL IDENTITY(1,1),
        [UserId] INT NOT NULL,
        [Rating] DECIMAL(3,2) NOT NULL DEFAULT 5.00,
        [TotalCalls] INT NOT NULL DEFAULT 0,
        [TotalEarnings] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [IsAvailable] BIT NOT NULL DEFAULT 1,
        [MaxCallsPerDay] INT NOT NULL DEFAULT 10,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Messengers] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Messengers_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    )
END

-- Orders Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE [dbo].[Orders]
    (
        [Id] INT NOT NULL IDENTITY(1,1),
        [BuyerId] INT NOT NULL,
        [MessengerId] INT,
        [RecipientPhone] NVARCHAR(20) NOT NULL,
        [RecipientName] NVARCHAR(100),
        [Message] NVARCHAR(500) NOT NULL,
        [MessageType] INT NOT NULL, -- 0 = Joke, 1 = Confession, 2 = Truth
        [Price] DECIMAL(10,2) NOT NULL,
        [IsAnonymous] BIT NOT NULL DEFAULT 0,
        [Status] INT NOT NULL DEFAULT 0, -- 0 = Pending, 1 = InProgress, 2 = Completed, 3 = Failed, 4 = Refunded
        [PaymentStatus] INT NOT NULL DEFAULT 0, -- 0 = Pending, 1 = Completed, 2 = Failed
        [Rating] INT,
        [Review] NVARCHAR(500),
        [CallAttempts] INT NOT NULL DEFAULT 0,
        [RecordingUrl] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Orders_Buyer] FOREIGN KEY ([BuyerId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Messenger] FOREIGN KEY ([MessengerId]) REFERENCES [dbo].[Messengers]([Id]) ON DELETE SET NULL
    )
END

-- Payments Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE [dbo].[Payments]
    (
        [Id] INT NOT NULL IDENTITY(1,1),
        [OrderId] INT NOT NULL,
        [BuyerId] INT NOT NULL,
        [Amount] DECIMAL(10,2) NOT NULL,
        [Currency] NVARCHAR(10) NOT NULL DEFAULT 'MXN',
        [MercadoPagoId] NVARCHAR(MAX),
        [Status] INT NOT NULL DEFAULT 0, -- 0 = Pending, 1 = Completed, 2 = Failed, 3 = Refunded
        [PaymentMethod] NVARCHAR(50),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Payments_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Payments_Buyer] FOREIGN KEY ([BuyerId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    )
END

-- CallAttempts Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CallAttempts')
BEGIN
    CREATE TABLE [dbo].[CallAttempts]
    (
        [Id] INT NOT NULL IDENTITY(1,1),
        [OrderId] INT NOT NULL,
        [TwilioCallSid] NVARCHAR(MAX),
        [PhoneNumber] NVARCHAR(20) NOT NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0 = Initiated, 1 = Ringing, 2 = Answered, 3 = Completed, 4 = Failed, 5 = Voicemail
        [Duration] INT DEFAULT 0,
        [RecordingUrl] NVARCHAR(MAX),
        [AttemptNumber] INT NOT NULL,
        [Error] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_CallAttempts] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_CallAttempts_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders]([Id]) ON DELETE CASCADE
    )
END

-- Create Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Users_Email' AND object_id = OBJECT_ID('Users'))
    CREATE NONCLUSTERED INDEX [IDX_Users_Email] ON [dbo].[Users] ([Email])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Orders_BuyerId' AND object_id = OBJECT_ID('Orders'))
    CREATE NONCLUSTERED INDEX [IDX_Orders_BuyerId] ON [dbo].[Orders] ([BuyerId])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Orders_MessengerId' AND object_id = OBJECT_ID('Orders'))
    CREATE NONCLUSTERED INDEX [IDX_Orders_MessengerId] ON [dbo].[Orders] ([MessengerId])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Orders_Status' AND object_id = OBJECT_ID('Orders'))
    CREATE NONCLUSTERED INDEX [IDX_Orders_Status] ON [dbo].[Orders] ([Status])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_CallAttempts_OrderId' AND object_id = OBJECT_ID('CallAttempts'))
    CREATE NONCLUSTERED INDEX [IDX_CallAttempts_OrderId] ON [dbo].[CallAttempts] ([OrderId])

PRINT 'Database migration completed successfully'
