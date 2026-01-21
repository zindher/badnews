-- Migration: Add Retry Tracking and Email Fields
-- Purpose: Implement proper 3x3 retry logic with SMS/Email fallback
-- Date: 2026-01-21

-- Add new columns to Orders table
ALTER TABLE [dbo].[Orders] ADD
    [RecipientEmail] nvarchar(255) NULL,
    [RetryDay] int NOT NULL DEFAULT 0,
    [DailyAttempts] int NOT NULL DEFAULT 0,
    [FirstCallAttemptDate] datetime2 NULL,
    [FallbackSMSSent] bit NOT NULL DEFAULT 0,
    [FallbackEmailSent] bit NOT NULL DEFAULT 0;

-- Add comments for documentation
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Optional email for email fallback notification', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Orders', 
    @level2type = N'COLUMN', @level2name = N'RecipientEmail';

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Current retry day (0-2) for 3-day retry period', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Orders', 
    @level2type = N'COLUMN', @level2name = N'RetryDay';

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Number of attempts on current day (0-3)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Orders', 
    @level2type = N'COLUMN', @level2name = N'DailyAttempts';

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Date/time of first call attempt to track 3-day window', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Orders', 
    @level2type = N'COLUMN', @level2name = N'FirstCallAttemptDate';

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Whether SMS fallback notification was sent after 9 attempts', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Orders', 
    @level2type = N'COLUMN', @level2name = N'FallbackSMSSent';

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Whether email fallback notification was sent after 9 attempts', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Orders', 
    @level2type = N'COLUMN', @level2name = N'FallbackEmailSent';

-- Create indexes for better query performance
CREATE INDEX [IX_Orders_RetryDay_CallAttempts] ON [dbo].[Orders]([RetryDay], [CallAttempts]);
CREATE INDEX [IX_Orders_FirstCallAttemptDate] ON [dbo].[Orders]([FirstCallAttemptDate]);

-- Verify changes
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Orders' 
AND COLUMN_NAME IN ('RecipientEmail', 'RetryDay', 'DailyAttempts', 'FirstCallAttemptDate', 'FallbackSMSSent', 'FallbackEmailSent')
ORDER BY COLUMN_NAME;
