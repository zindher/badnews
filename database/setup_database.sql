-- ================================================================
-- BadNews Project - Database Setup Script
-- SQL Server 2019+
-- Created: January 21, 2026
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
DROP TABLE IF EXISTS dbo.Users;
*/

-- Step 3: Create Users Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE dbo.Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL CHECK (Role IN ('buyer', 'messenger', 'admin')),
        PhoneNumber NVARCHAR(20),
        AvatarUrl NVARCHAR(500),
        Bio NVARCHAR(1000),
        IsAvailable BIT DEFAULT 1,
        Rating DECIMAL(3,2) DEFAULT 0,
        CompletedOrders INT DEFAULT 0,
        TotalEarnings DECIMAL(10,2) DEFAULT 0,
        PendingEarnings DECIMAL(10,2) DEFAULT 0,
        LastLogin DATETIME2,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
    )
    CREATE INDEX IX_Users_Email ON dbo.Users(Email)
    CREATE INDEX IX_Users_Role ON dbo.Users(Role)
    PRINT 'Table Users created successfully'
END
GO

-- Step 4: Create Orders Table
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
        TotalPrice DECIMAL(10,2) NOT NULL,
        Status NVARCHAR(50) DEFAULT 'pending' CHECK (Status IN ('pending', 'accepted', 'in_progress', 'completed', 'cancelled', 'failed')),
        PaymentStatus NVARCHAR(50) DEFAULT 'pending' CHECK (PaymentStatus IN ('pending', 'completed', 'refunded')),
        Notes NVARCHAR(1000),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Orders_Buyer FOREIGN KEY (BuyerId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Orders_Messenger FOREIGN KEY (AcceptedMessengerId) REFERENCES dbo.Users(Id)
    )
    CREATE INDEX IX_Orders_BuyerId ON dbo.Orders(BuyerId)
    CREATE INDEX IX_Orders_MessengerId ON dbo.Orders(AcceptedMessengerId)
    CREATE INDEX IX_Orders_Status ON dbo.Orders(Status)
    CREATE INDEX IX_Orders_CreatedAt ON dbo.Orders(CreatedAt)
    PRINT 'Table Orders created successfully'
END
GO

-- Step 5: Create CallAttempts Table
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

-- Step 6: Create Payments Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE dbo.Payments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        BuyerId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(10,2) NOT NULL,
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

-- Step 7: Create Messages Table
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

-- Step 8: Create CallRetries Table
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

-- Step 9: Create Withdrawals Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Withdrawals')
BEGIN
    CREATE TABLE dbo.Withdrawals (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MessengerId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(10,2) NOT NULL,
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

-- Step 10: Create Disputes Table
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
DELETE FROM dbo.Users
DBCC CHECKIDENT ('Orders', RESEED, 0)
DBCC CHECKIDENT ('CallAttempts', RESEED, 0)
DBCC CHECKIDENT ('Payments', RESEED, 0)
GO

-- Insert test users
INSERT INTO dbo.Users (Email, PasswordHash, Name, Role, PhoneNumber, IsAvailable, Rating, CompletedOrders)
VALUES 
(
    'buyer@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S', -- password: Test@1234
    'Juan Buyer',
    'buyer',
    '+5215551234567',
    1,
    0,
    0
),
(
    'messenger@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S', -- password: Test@1234
    'María Messenger',
    'messenger',
    '+5215555678901',
    1,
    4.8,
    25
),
(
    'admin@example.com',
    '$2a$11$8qZzJw/fHl1pGyEXLJkVqO2kUPpqVvPHxNQvqF8/YpqZ7e4Jp5K.S', -- password: Test@1234
    'Admin User',
    'admin',
    '+5215559999999',
    1,
    0,
    0
)
GO

-- Insert test order
DECLARE @BuyerId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'buyer@example.com')
DECLARE @MessengerId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Users WHERE Email = 'messenger@example.com')

INSERT INTO dbo.Orders (BuyerId, AcceptedMessengerId, Message, RecipientName, RecipientPhone, Category, WordCount, EstimatedDuration, TotalPrice, Status, PaymentStatus)
VALUES 
(
    @BuyerId,
    @MessengerId,
    'Feliz cumpleaños hermana! Te deseo todo lo mejor en tu día especial. Que disfrutes muchísimo con la familia y los amigos. ¡Te queremos!',
    'María García',
    '+5215551111111',
    'birthday',
    50,
    2,
    499.99,
    'completed',
    'completed'
)
GO

PRINT ''
PRINT '=========================================='
PRINT 'Database setup completed successfully!'
PRINT '=========================================='
PRINT ''
PRINT 'Database: BadNews'
PRINT 'Tables created: 9'
PRINT 'Indexes created: Multiple for performance'
PRINT ''
PRINT 'Tables:'
PRINT '  ✓ Users'
PRINT '  ✓ Orders'
PRINT '  ✓ CallAttempts'
PRINT '  ✓ Payments'
PRINT '  ✓ Messages'
PRINT '  ✓ CallRetries'
PRINT '  ✓ Withdrawals'
PRINT '  ✓ Disputes'
PRINT ''
PRINT 'Next steps:'
PRINT '  1. Run Entity Framework migrations (if needed)'
PRINT '  2. Configure connection string in appsettings.json'
PRINT '  3. Start the backend application'
PRINT '=========================================='
*/

GO
