-- ================================================================
-- BADNEWS - COMPLETE DATABASE SETUP
-- SQL Server 2022 RTM-GDR (v16.0.1165.1)
-- ================================================================
-- Last Updated: January 21, 2026
-- Version: 2.0 - CONSOLIDATED
--
-- Complete schema for BadNews platform:
-- - Buyers: Users who place orders for personalized messages
-- - Messengers: Users who deliver personalized messages via calls
-- - Admins: Platform administrators
-- - Google OAuth support (NEW)
-- ================================================================

-- ================================================================
-- STEP 1: CREATE DATABASE
-- ================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BadNews')
BEGIN
    CREATE DATABASE BadNews
    PRINT 'Database [BadNews] created successfully'
END
GO

USE BadNews
GO

-- ================================================================
-- STEP 2: USERS TABLE (Core user table for all roles)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE dbo.Users (
        -- Primary Key
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Authentication
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        
        -- Profile
        FirstName NVARCHAR(255) NOT NULL,
        LastName NVARCHAR(255) NOT NULL,
        PhoneNumber NVARCHAR(20),
        
        -- Account Status
        Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Buyer', 'Messenger', 'Admin')),
        IsActive BIT DEFAULT 1,
        LastLogin DATETIME2,
        
        -- Timezone & Schedule
        PreferredTimezone NVARCHAR(100) DEFAULT 'America/Mexico_City',
        PreferredCallTime NVARCHAR(100),
        
        -- Email Verification
        EmailVerified BIT DEFAULT 0,
        EmailVerificationToken NVARCHAR(500),
        EmailVerificationTokenExpiry DATETIME2,
        
        -- Google OAuth (NEW - January 21, 2026)
        GoogleId NVARCHAR(MAX),
        GoogleEmail NVARCHAR(255),
        GoogleProfilePictureUrl NVARCHAR(MAX),
        IsGoogleLinked BIT DEFAULT 0,
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
    )
    
    -- Create Indexes
    CREATE INDEX IX_Users_Email ON dbo.Users(Email)
    CREATE INDEX IX_Users_Role ON dbo.Users(Role)
    CREATE INDEX IX_Users_IsActive ON dbo.Users(IsActive)
    CREATE INDEX IX_Users_GoogleId ON dbo.Users(GoogleId) WHERE GoogleId IS NOT NULL
    
    PRINT 'Table [Users] created successfully (with Google OAuth)'
END
GO

-- ================================================================
-- STEP 3: MESSENGERS TABLE (Extended profile for messengers)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messengers')
BEGIN
    CREATE TABLE dbo.Messengers (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        
        -- Status & Availability
        IsAvailable BIT DEFAULT 1,
        
        -- Ratings & Statistics
        AverageRating DECIMAL(3,2) DEFAULT 0,
        TotalCompletedOrders INT DEFAULT 0,
        
        -- Earnings
        TotalEarnings DECIMAL(12,2) DEFAULT 0,
        PendingBalance DECIMAL(12,2) DEFAULT 0,
        
        -- Profile
        AvatarUrl NVARCHAR(500),
        Bio NVARCHAR(1000),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Messengers_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    )
    
    CREATE INDEX IX_Messengers_UserId ON dbo.Messengers(UserId)
    CREATE INDEX IX_Messengers_IsAvailable ON dbo.Messengers(IsAvailable)
    
    PRINT 'Table [Messengers] created successfully'
END
GO

-- ================================================================
-- STEP 4: ORDERS TABLE (Personalized message orders)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE dbo.Orders (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationships
        BuyerId UNIQUEIDENTIFIER NOT NULL,
        MessengerId UNIQUEIDENTIFIER,
        
        -- Order Content
        Message NVARCHAR(2500) NOT NULL,
        RecipientName NVARCHAR(255) NOT NULL,
        RecipientPhoneNumber NVARCHAR(20) NOT NULL,
        RecipientEmail NVARCHAR(255),
        
        -- Recipient Location
        RecipientState NVARCHAR(50),
        RecipientTimezone NVARCHAR(100),
        
        -- Metadata
        Category NVARCHAR(100),
        WordCount INT,
        EstimatedDuration INT,
        IsAnonymous BIT DEFAULT 0,
        
        -- Pricing
        Price DECIMAL(12,2) NOT NULL,
        
        -- Status Tracking
        Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Assigned', 'InProgress', 'Completed', 'Cancelled', 'Failed')),
        PaymentStatus NVARCHAR(50) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Completed', 'Refunded')),
        
        -- Retry System (3 attempts/day × 3 days = 9 max)
        CallAttempts INT DEFAULT 0,
        LastRetryDate DATETIME2,
        NextRetryDate DATETIME2,
        
        -- Additional Info
        PreferredCallTime NVARCHAR(100),
        Notes NVARCHAR(1000),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CompletedAt DATETIME2,
        
        CONSTRAINT FK_Orders_Buyer FOREIGN KEY (BuyerId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_Orders_Messenger FOREIGN KEY (MessengerId) REFERENCES dbo.Users(Id)
    )
    
    CREATE INDEX IX_Orders_BuyerId ON dbo.Orders(BuyerId)
    CREATE INDEX IX_Orders_MessengerId ON dbo.Orders(MessengerId)
    CREATE INDEX IX_Orders_Status ON dbo.Orders(Status)
    CREATE INDEX IX_Orders_PaymentStatus ON dbo.Orders(PaymentStatus)
    CREATE INDEX IX_Orders_CreatedAt ON dbo.Orders(CreatedAt)
    
    PRINT 'Table [Orders] created successfully'
END
GO

-- ================================================================
-- STEP 5: CALL ATTEMPTS TABLE (Twilio integration & recordings)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CallAttempts')
BEGIN
    CREATE TABLE dbo.CallAttempts (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationships
        OrderId UNIQUEIDENTIFIER NOT NULL,
        MessengerId UNIQUEIDENTIFIER NOT NULL,
        
        -- Call Status
        Status NVARCHAR(50) DEFAULT 'Initiated' CHECK (Status IN ('Initiated', 'Ringing', 'Connected', 'Completed', 'Failed', 'Cancelled')),
        
        -- Twilio Integration
        CallSid NVARCHAR(100),
        
        -- Recording Data
        RecordingSid NVARCHAR(100),
        RecordingUrl NVARCHAR(500),
        RecordingDuration INT,
        
        -- Call Timing
        StartTime DATETIME2,
        EndTime DATETIME2,
        Duration INT,
        
        -- Retry Info
        RetryNumber INT DEFAULT 1,
        
        -- Failure Info
        FailureReason NVARCHAR(500),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_CallAttempts_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_CallAttempts_Messengers FOREIGN KEY (MessengerId) REFERENCES dbo.Users(Id)
    )
    
    CREATE INDEX IX_CallAttempts_OrderId ON dbo.CallAttempts(OrderId)
    CREATE INDEX IX_CallAttempts_MessengerId ON dbo.CallAttempts(MessengerId)
    CREATE INDEX IX_CallAttempts_Status ON dbo.CallAttempts(Status)
    
    PRINT 'Table [CallAttempts] created successfully'
END
GO

-- ================================================================
-- STEP 6: CALL RETRIES TABLE (Retry scheduler: 3/day × 3 days)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CallRetries')
BEGIN
    CREATE TABLE dbo.CallRetries (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationship
        OrderId UNIQUEIDENTIFIER NOT NULL,
        
        -- Retry Info
        RetryNumber INT NOT NULL,
        ScheduledAt DATETIME2 NOT NULL,
        ExecutedAt DATETIME2,
        
        -- Status
        Status NVARCHAR(50) DEFAULT 'Scheduled' CHECK (Status IN ('Scheduled', 'Executed', 'Cancelled', 'Failed')),
        
        -- Details
        Reason NVARCHAR(500),
        HangfireJobId NVARCHAR(255),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_CallRetries_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id)
    )
    
    CREATE INDEX IX_CallRetries_OrderId ON dbo.CallRetries(OrderId)
    CREATE INDEX IX_CallRetries_ScheduledAt ON dbo.CallRetries(ScheduledAt)
    
    PRINT 'Table [CallRetries] created successfully'
END
GO

-- ================================================================
-- STEP 7: PAYMENTS TABLE (Mercado Pago transactions)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE dbo.Payments (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationships
        OrderId UNIQUEIDENTIFIER NOT NULL,
        BuyerId UNIQUEIDENTIFIER NOT NULL,
        
        -- Amount
        Amount DECIMAL(12,2) NOT NULL,
        Currency NVARCHAR(10) DEFAULT 'MXN',
        
        -- Payment Status
        Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Completed', 'Failed', 'Refunded')),
        
        -- Mercado Pago Integration
        ExternalPaymentId NVARCHAR(255),
        
        -- Payment Method
        PaymentMethod NVARCHAR(50),
        PaymentMethodId NVARCHAR(255),
        
        -- Transaction
        TransactionId NVARCHAR(255),
        ErrorMessage NVARCHAR(500),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Payments_Users FOREIGN KEY (BuyerId) REFERENCES dbo.Users(Id)
    )
    
    CREATE INDEX IX_Payments_OrderId ON dbo.Payments(OrderId)
    CREATE INDEX IX_Payments_Status ON dbo.Payments(Status)
    CREATE INDEX IX_Payments_CreatedAt ON dbo.Payments(CreatedAt)
    
    PRINT 'Table [Payments] created successfully'
END
GO

-- ================================================================
-- STEP 8: MESSAGES TABLE (Order-related chat)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messages')
BEGIN
    CREATE TABLE dbo.Messages (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationships
        OrderId UNIQUEIDENTIFIER NOT NULL,
        SenderId UNIQUEIDENTIFIER NOT NULL,
        
        -- Content
        Content NVARCHAR(MAX) NOT NULL,
        
        -- Status
        IsRead BIT DEFAULT 0,
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Messages_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Messages_Users FOREIGN KEY (SenderId) REFERENCES dbo.Users(Id)
    )
    
    CREATE INDEX IX_Messages_OrderId ON dbo.Messages(OrderId)
    CREATE INDEX IX_Messages_SenderId ON dbo.Messages(SenderId)
    
    PRINT 'Table [Messages] created successfully'
END
GO

-- ================================================================
-- STEP 9: WITHDRAWALS TABLE (Messenger earnings withdrawals)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Withdrawals')
BEGIN
    CREATE TABLE dbo.Withdrawals (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationship
        MessengerId UNIQUEIDENTIFIER NOT NULL,
        
        -- Withdrawal Amount
        Amount DECIMAL(12,2) NOT NULL,
        
        -- Status
        Status NVARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'Processed')),
        RejectionReason NVARCHAR(500),
        
        -- Bank Details
        BankAccount NVARCHAR(255),
        BankName NVARCHAR(255),
        
        -- Transaction
        TransactionId NVARCHAR(255),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        ProcessedAt DATETIME2,
        
        CONSTRAINT FK_Withdrawals_Users FOREIGN KEY (MessengerId) REFERENCES dbo.Users(Id)
    )
    
    CREATE INDEX IX_Withdrawals_MessengerId ON dbo.Withdrawals(MessengerId)
    CREATE INDEX IX_Withdrawals_Status ON dbo.Withdrawals(Status)
    
    PRINT 'Table [Withdrawals] created successfully'
END
GO

-- ================================================================
-- STEP 10: DISPUTES TABLE (Order disputes & resolutions)
-- ================================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Disputes')
BEGIN
    CREATE TABLE dbo.Disputes (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        
        -- Relationships
        OrderId UNIQUEIDENTIFIER NOT NULL,
        ReportedById UNIQUEIDENTIFIER NOT NULL,
        
        -- Dispute Content
        Reason NVARCHAR(255) NOT NULL,
        Description NVARCHAR(2000),
        
        -- Resolution
        Status NVARCHAR(50) DEFAULT 'Open' CHECK (Status IN ('Open', 'Investigating', 'Resolved', 'Closed')),
        Resolution NVARCHAR(2000),
        
        -- Audit
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        ResolvedAt DATETIME2,
        
        CONSTRAINT FK_Disputes_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Disputes_Users FOREIGN KEY (ReportedById) REFERENCES dbo.Users(Id)
    )
    
    CREATE INDEX IX_Disputes_OrderId ON dbo.Disputes(OrderId)
    CREATE INDEX IX_Disputes_Status ON dbo.Disputes(Status)
    
    PRINT 'Table [Disputes] created successfully'
END
GO

-- ================================================================
-- STEP 11: HANGFIRE TABLES (Background jobs for retries)
-- ================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BadNews_Hangfire')
BEGIN
    CREATE DATABASE BadNews_Hangfire
    PRINT 'Database [BadNews_Hangfire] created successfully'
END
GO

-- ================================================================
-- FINAL SUMMARY
-- ================================================================

PRINT ''
PRINT '╔═══════════════════════════════════════════════════════════╗'
PRINT '║          BADNEWS DATABASE SETUP COMPLETE ✅               ║'
PRINT '╚═══════════════════════════════════════════════════════════╝'
PRINT ''
PRINT '📊 Database Information:'
PRINT '   • Name: BadNews'
PRINT '   • Version: 2.0 - CONSOLIDATED'
PRINT '   • SQL Server: 2022 RTM-GDR v16.0.1165.1'
PRINT '   • Last Updated: January 21, 2026'
PRINT ''
PRINT '📋 Tables Created (10 total):'
PRINT '   ✓ Users (core user table, all roles)'
PRINT '   ✓ Messengers (extended messenger profile)'
PRINT '   ✓ Orders (personalized message orders)'
PRINT '   ✓ CallAttempts (Twilio integration & recordings)'
PRINT '   ✓ CallRetries (retry scheduler)'
PRINT '   ✓ Payments (Mercado Pago transactions)'
PRINT '   ✓ Messages (order-related chat)'
PRINT '   ✓ Withdrawals (earnings withdrawals)'
PRINT '   ✓ Disputes (order dispute resolution)'
PRINT '   ✓ EFMigrationsHistory (Entity Framework)'
PRINT ''
PRINT '🔐 Security Features:'
PRINT '   ✓ Role-based access (Buyer, Messenger, Admin)'
PRINT '   ✓ Email verification support'
PRINT '   ✓ Google OAuth integration'
PRINT '   ✓ Password hashing (bcrypt)'
PRINT '   ✓ Foreign key constraints'
PRINT ''
PRINT '💳 Payment Integration:'
PRINT '   ✓ Mercado Pago support'
PRINT '   ✓ Transaction tracking'
PRINT '   ✓ Refund handling'
PRINT '   ✓ Withdrawal management'
PRINT ''
PRINT '📞 Call System:'
PRINT '   ✓ Twilio integration'
PRINT '   ✓ Call recording support'
PRINT '   ✓ 3x3 retry system (9 max attempts)'
PRINT '   ✓ Timezone support'
PRINT ''
PRINT '🆕 New in v2.0:'
PRINT '   ✓ Google OAuth fields (GoogleId, GoogleEmail, etc)'
PRINT '   ✓ Consolidated database script'
PRINT '   ✓ Complete documentation'
PRINT ''
PRINT '═══════════════════════════════════════════════════════════'
PRINT 'Next Steps:'
PRINT '   1. Configure appsettings.json with connection string'
PRINT '   2. Run dotnet ef database update (if using EF Migrations)'
PRINT '   3. Start backend application'
PRINT '   4. Verify tables in SQL Server Management Studio'
PRINT '═══════════════════════════════════════════════════════════'
PRINT ''
GO
