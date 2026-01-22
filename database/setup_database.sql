-- ================================================================
-- BadNews Project - Database Setup Script
-- SQL Server 2019+
-- Created: January 21, 2026
-- Updated: January 21, 2026
-- 
-- Description:
-- Complete database schema for BadNews platform supporting:
-- - Buyers: Users who place orders for personalized messages
-- - Messengers: Users who deliver personalized messages via calls
-- - Admins: Platform administrators
--
-- Features:
-- - Three-tier role-based access control
-- - Order management with full lifecycle tracking
-- - Call retry system (3 attempts per day for 3 days)
-- - Payment processing integration (Mercado Pago)
-- - Messaging and dispute resolution
-- - Withdrawal management for messengers
-- ================================================================

-- Step 1: Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BadNews')
BEGIN
    CREATE DATABASE BadNews
    PRINT 'Database BadNews created successfully'
END
GO

USE BadNews
GO

-- Step 2: Drop existing tables (for development/reset)
-- Uncomment this section only if you want to reset the database
/*
DROP TABLE IF EXISTS dbo.Messages;
DROP TABLE IF EXISTS dbo.CallRetries;
DROP TABLE IF EXISTS dbo.Withdrawals;
DROP TABLE IF EXISTS dbo.Disputes;
DROP TABLE IF EXISTS dbo.Payments;
DROP TABLE IF EXISTS dbo.CallAttempts;
DROP TABLE IF EXISTS dbo.Orders;
DROP TABLE IF EXISTS dbo.Messengers;
DROP TABLE IF EXISTS dbo.Users;
DBCC CHECKIDENT ('Orders', RESEED, 0)
DBCC CHECKIDENT ('CallAttempts', RESEED, 0)
DBCC CHECKIDENT ('Payments', RESEED, 0)
DBCC CHECKIDENT ('CallRetries', RESEED, 0)
DBCC CHECKIDENT ('Withdrawals', RESEED, 0)
DBCC CHECKIDENT ('Disputes', RESEED, 0)
DBCC CHECKIDENT ('Messages', RESEED, 0)
GO
*/

-- Step 3: Create Users Table
-- This is the core table for all user types (Buyer, Messenger, Admin)
-- Role field determines the type of user and their permissions
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE dbo.Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        PhoneNumber NVARCHAR(20),
        FirstName NVARCHAR(255) NOT NULL,
        LastName NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Buyer', 'Messenger', 'Admin')),
        IsActive BIT DEFAULT 1,
        LastLogin DATETIME2,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        -- Timezone support
        PreferredTimezone NVARCHAR(100) DEFAULT 'America/Mexico_City',
        PreferredCallTime NVARCHAR(100),
        
        -- Email verification
        EmailVerified BIT DEFAULT 0,
        EmailVerificationToken NVARCHAR(500),
        EmailVerificationTokenExpiry DATETIME2
    )
    CREATE INDEX IX_Users_Email ON dbo.Users(Email)
    CREATE INDEX IX_Users_Role ON dbo.Users(Role)
    CREATE INDEX IX_Users_IsActive ON dbo.Users(IsActive)
    PRINT 'Table Users created successfully'
END
GO

-- Step 4: Create Messengers Table
-- Extended profile for messenger users (users with Role = 'Messenger')
-- This table stores messenger-specific data
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messengers')
BEGIN
    CREATE TABLE dbo.Messengers (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        IsAvailable BIT DEFAULT 1,
        AverageRating DECIMAL(3,2) DEFAULT 0,
        TotalCompletedOrders INT DEFAULT 0,
        TotalEarnings DECIMAL(12,2) DEFAULT 0,
        PendingBalance DECIMAL(12,2) DEFAULT 0,
        AvatarUrl NVARCHAR(500),
        Bio NVARCHAR(1000),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Messengers_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    )
    CREATE INDEX IX_Messengers_UserId ON dbo.Messengers(UserId)
    CREATE INDEX IX_Messengers_IsAvailable ON dbo.Messengers(IsAvailable)
    PRINT 'Table Messengers created successfully'
END
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE dbo.Orders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        BuyerId UNIQUEIDENTIFIER NOT NULL,
        AcceptedMessengerId UNIQUEIDENTIFIER,
        Message NVARCHAR(2500) NOT NULL,
        RecipientName NVARCHAR(255) NOT NULL,
        RecipientPhone NVARCHAR(20),
        Category NVARCHAR(100),
        WordCount INT,
        EstimatedDuration INT,
        TotalPrice DECIMAL(12,2) NOT NULL,
        Status NVARCHAR(50) DEFAULT 'pending' CHECK (Status IN ('pending', 'accepted', 'in_progress', 'completed', 'cancelled', 'failed')),
        PaymentStatus NVARCHAR(50) DEFAULT 'pending' CHECK (PaymentStatus IN ('pending', 'completed', 'refunded')),
        
        -- Retry tracking (3 retries per day, up to 3 days)
        CallAttempts INT DEFAULT 0,
        LastRetryDate DATETIME2,
        NextRetryDate DATETIME2,
        
        Notes NVARCHAR(1000),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CompletedAt DATETIME2,
        
        CONSTRAINT FK_Orders_Buyer FOREIGN KEY (BuyerId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Orders_Messenger FOREIGN KEY (AcceptedMessengerId) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_Orders_BuyerId ON dbo.Orders(BuyerId)
    CREATE INDEX IX_Orders_MessengerId ON dbo.Orders(AcceptedMessengerId)
    CREATE INDEX IX_Orders_Status ON dbo.Orders(Status)
    CREATE INDEX IX_Orders_PaymentStatus ON dbo.Orders(PaymentStatus)
    CREATE INDEX IX_Orders_CreatedAt ON dbo.Orders(CreatedAt)
    PRINT 'Table Orders created successfully'
END
GO

-- Step 6: Create CallAttempts Table
-- Tracks each call attempt made for an order, including Twilio integration
-- Supports call recording and retry tracking
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CallAttempts')
BEGIN
    CREATE TABLE dbo.CallAttempts (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        MessengerId UNIQUEIDENTIFIER NOT NULL,
        Status NVARCHAR(50) DEFAULT 'initiated' CHECK (Status IN ('initiated', 'ringing', 'connected', 'completed', 'failed', 'cancelled')),
        CallSid NVARCHAR(100),
        RecordingSid NVARCHAR(100),
        RecordingUrl NVARCHAR(500),
        RecordingDuration INT,
        StartTime DATETIME2,
        EndTime DATETIME2,
        Duration INT,
        RetryNumber INT DEFAULT 1,
        FailureReason NVARCHAR(500),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_CallAttempts_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_CallAttempts_Messenger FOREIGN KEY (MessengerId) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_CallAttempts_OrderId ON dbo.CallAttempts(OrderId)
    CREATE INDEX IX_CallAttempts_MessengerId ON dbo.CallAttempts(MessengerId)
    CREATE INDEX IX_CallAttempts_Status ON dbo.CallAttempts(Status)
    PRINT 'Table CallAttempts created successfully'
END
GO

-- Step 7: Create Payments Table
-- Tracks payment transactions for orders using Mercado Pago integration
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE dbo.Payments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        BuyerId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(12,2) NOT NULL,
        Currency NVARCHAR(10) DEFAULT 'MXN',
        Status NVARCHAR(50) DEFAULT 'pending' CHECK (Status IN ('pending', 'completed', 'failed', 'refunded')),
        ExternalPaymentId NVARCHAR(255),
        PaymentMethod NVARCHAR(50),
        PaymentMethodId NVARCHAR(255),
        TransactionId NVARCHAR(255),
        ErrorMessage NVARCHAR(500),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Payments_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Payments_Buyer FOREIGN KEY (BuyerId) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_Payments_OrderId ON dbo.Payments(OrderId)
    CREATE INDEX IX_Payments_Status ON dbo.Payments(Status)
    CREATE INDEX IX_Payments_CreatedAt ON dbo.Payments(CreatedAt)
    PRINT 'Table Payments created successfully'
END
GO

-- Step 8: Create Messages Table
-- In-app messaging between buyer and messenger for order coordination
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messages')
BEGIN
    CREATE TABLE dbo.Messages (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        SenderId UNIQUEIDENTIFIER NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        IsRead BIT DEFAULT 0,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Messages_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Messages_Sender FOREIGN KEY (SenderId) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_Messages_OrderId ON dbo.Messages(OrderId)
    CREATE INDEX IX_Messages_SenderId ON dbo.Messages(SenderId)
    PRINT 'Table Messages created successfully'
END
GO

-- Step 9: Create CallRetries Table
-- Schedules and tracks retry attempts (max 3 per day for 3 days = 9 total)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CallRetries')
BEGIN
    CREATE TABLE dbo.CallRetries (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        RetryNumber INT NOT NULL,
        ScheduledAt DATETIME2 NOT NULL,
        ExecutedAt DATETIME2,
        Status NVARCHAR(50) DEFAULT 'scheduled' CHECK (Status IN ('scheduled', 'executed', 'cancelled', 'failed')),
        Reason NVARCHAR(500),
        HangfireJobId NVARCHAR(255),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_CallRetries_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id)
    )
    CREATE INDEX IX_CallRetries_OrderId ON dbo.CallRetries(OrderId)
    CREATE INDEX IX_CallRetries_ScheduledAt ON dbo.CallRetries(ScheduledAt)
    PRINT 'Table CallRetries created successfully'
END
GO

-- Step 10: Create Withdrawals Table
-- Manages withdrawal requests from messengers to their bank accounts
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Withdrawals')
BEGIN
    CREATE TABLE dbo.Withdrawals (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MessengerId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(12,2) NOT NULL,
        Status NVARCHAR(50) DEFAULT 'pending' CHECK (Status IN ('pending', 'approved', 'rejected', 'processed')),
        RejectionReason NVARCHAR(500),
        BankAccount NVARCHAR(255),
        BankName NVARCHAR(255),
        TransactionId NVARCHAR(255),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        ProcessedAt DATETIME2,
        
        CONSTRAINT FK_Withdrawals_Messenger FOREIGN KEY (MessengerId) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_Withdrawals_MessengerId ON dbo.Withdrawals(MessengerId)
    CREATE INDEX IX_Withdrawals_Status ON dbo.Withdrawals(Status)
    PRINT 'Table Withdrawals created successfully'
END
GO

-- Step 11: Create Disputes Table
-- Handles disputes between buyers and messengers regarding orders
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Disputes')
BEGIN
    CREATE TABLE dbo.Disputes (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        ReportedById UNIQUEIDENTIFIER NOT NULL,
        Reason NVARCHAR(255) NOT NULL,
        Description NVARCHAR(2000),
        Status NVARCHAR(50) DEFAULT 'open' CHECK (Status IN ('open', 'investigating', 'resolved', 'closed')),
        Resolution NVARCHAR(2000),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        ResolvedAt DATETIME2,
        
        CONSTRAINT FK_Disputes_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Disputes_Reporter FOREIGN KEY (ReportedById) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_Disputes_OrderId ON dbo.Disputes(OrderId)
    CREATE INDEX IX_Disputes_Status ON dbo.Disputes(Status)
    PRINT 'Table Disputes created successfully'
END
GO

-- Step 11: Create Hangfire tables (for background jobs)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BadNews_Hangfire')
BEGIN
    CREATE DATABASE BadNews_Hangfire
    PRINT 'Hangfire database created successfully'
END
GO

-- ================================================================
-- OPTIONAL: Insert Sample Data for Development
-- ================================================================

-- Uncomment this section to insert test data

/*
-- Clear existing data
DELETE FROM dbo.Messages
DELETE FROM dbo.CallRetries
DELETE FROM dbo.Withdrawals
DELETE FROM dbo.Disputes
DELETE FROM dbo.Payments
DELETE FROM dbo.CallAttempts
DELETE FROM dbo.Orders
DELETE FROM dbo.Messengers
DELETE FROM dbo.Users
DBCC CHECKIDENT ('Orders', RESEED, 0)
DBCC CHECKIDENT ('CallAttempts', RESEED, 0)
DBCC CHECKIDENT ('Payments', RESEED, 0)
DBCC CHECKIDENT ('CallRetries', RESEED, 0)
DBCC CHECKIDENT ('Withdrawals', RESEED, 0)
DBCC CHECKIDENT ('Disputes', RESEED, 0)
DBCC CHECKIDENT ('Messages', RESEED, 0)
GO

-- Insert test users
-- Using BCrypt hash format (password: Test@1234)
INSERT INTO dbo.Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role, IsActive)
VALUES 
(
    'buyer@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S',
    'Juan',
    'Buyer',
    '+5215551234567',
    'Buyer',
    1
),
(
    'messenger1@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S',
    'María',
    'Messenger',
    '+5215555678901',
    'Messenger',
    1
),
(
    'messenger2@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S',
    'Carlos',
    'Messenger',
    '+5215559876543',
    'Messenger',
    1
),
(
    'admin@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S',
    'Admin',
    'User',
    '+5215559999999',
    'Admin',
    1
)
GO

-- Create messenger profiles for messenger users
DECLARE @Messenger1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'messenger1@example.com')
DECLARE @Messenger2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'messenger2@example.com')

INSERT INTO dbo.Messengers (UserId, IsAvailable, AverageRating, TotalCompletedOrders, TotalEarnings, PendingBalance)
VALUES 
(
    @Messenger1,
    1,
    4.8,
    25,
    12475.50,
    2500.00
),
(
    @Messenger2,
    1,
    4.5,
    18,
    8925.00,
    1800.00
)
GO

-- Create sample orders
DECLARE @BuyerId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'buyer@example.com')
DECLARE @Messenger1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'messenger1@example.com')
DECLARE @Messenger2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'messenger2@example.com')

INSERT INTO dbo.Orders (BuyerId, AcceptedMessengerId, Message, RecipientName, RecipientPhone, Category, WordCount, EstimatedDuration, TotalPrice, Status, PaymentStatus)
VALUES 
(
    @BuyerId,
    @Messenger1,
    'Feliz cumpleaños hermana! Te deseo todo lo mejor en tu día especial. Que disfrutes muchísimo con la familia y los amigos. ¡Te queremos!',
    'María García',
    '+5215551111111',
    'birthday',
    50,
    2,
    499.99,
    'completed',
    'completed'
),
(
    @BuyerId,
    @Messenger2,
    'Felicidades por tu nuevo trabajo! Sabía que lo lograrías. ¡Ahora a disfrutar de esta nueva etapa!',
    'Juan López',
    '+5215552222222',
    'congratulations',
    40,
    1,
    399.99,
    'completed',
    'completed'
),
(
    @BuyerId,
    NULL,
    'Te quiero mucho y quería decirlo personalmente. Eres lo mejor que me ha pasado.',
    'Ana Rodríguez',
    '+5215553333333',
    'love',
    35,
    1,
    349.99,
    'pending',
    'pending'
)
GO

-- Create sample payment
DECLARE @OrderId INT = (SELECT TOP 1 Id FROM dbo.Orders WHERE RecipientName = 'María García')
DECLARE @BuyerId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'buyer@example.com')

INSERT INTO dbo.Payments (OrderId, BuyerId, Amount, Status, ExternalPaymentId)
VALUES 
(
    @OrderId,
    @BuyerId,
    499.99,
    'completed',
    'MP-20260121-001'
)
GO

*/

-- Final summary message
PRINT ''
PRINT '=========================================='
PRINT 'Database setup completed successfully!'
PRINT '=========================================='
PRINT ''
PRINT 'Database: BadNews'
PRINT 'Tables created: 10'
PRINT 'Additional Hangfire database: BadNews_Hangfire'
PRINT ''
PRINT 'User Type Architecture:'
PRINT '  - BUYER: Places orders for personalized messages'
PRINT '  - MESSENGER: Records and delivers personalized messages (has Messengers profile)'
PRINT '  - ADMIN: Manages users, reviews disputes, handles system operations'
PRINT ''
PRINT 'Tables:'
PRINT '  ✓ Users (core user table for all roles)'
PRINT '  ✓ Messengers (extended profile for messengers only)'
PRINT '  ✓ Orders (personalized message orders)'
PRINT '  ✓ CallAttempts (Twilio integration & recordings)'
PRINT '  ✓ Payments (Mercado Pago transactions)'
PRINT '  ✓ Messages (order-related chat)'
PRINT '  ✓ CallRetries (retry scheduler: 3/day x 3 days)'
PRINT '  ✓ Withdrawals (messenger earnings withdrawals)'
PRINT '  ✓ Disputes (order disputes & resolutions)'
PRINT ''
PRINT 'Key Features Supported:'
PRINT '  ✓ Role-based access control (RBAC) for 3 user types'
PRINT '  ✓ Call recording with Twilio integration'
PRINT '  ✓ Automatic retry system (9 max attempts over 3 days)'
PRINT '  ✓ Payment processing with Mercado Pago'
PRINT '  ✓ Messenger earnings tracking & withdrawals'
PRINT '  ✓ Dispute resolution system'
PRINT '  ✓ Timezone & preferred call time support'
PRINT '  ✓ Email verification for security'
PRINT ''
PRINT 'Recommended Next Steps:'
PRINT '  1. Execute Entity Framework migrations'
PRINT '  2. Configure SQL Server connection string in appsettings.json'
PRINT '  3. Uncomment and run sample data insertion (at bottom of this script)'
PRINT '  4. Start the backend application'
PRINT '  5. Verify API endpoints with Swagger/Postman'
PRINT '=========================================='
GO
