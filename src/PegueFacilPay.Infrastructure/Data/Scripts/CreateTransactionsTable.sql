CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL,
    Status INT NOT NULL,
    PaymentMethod INT NOT NULL,
    MerchantId NVARCHAR(50) NOT NULL,
    CustomerId NVARCHAR(50) NOT NULL,
    Fee DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    ProcessedAt DATETIME2 NULL,
    
    INDEX IX_Transactions_MerchantId (MerchantId),
    INDEX IX_Transactions_CustomerId (CustomerId),
    INDEX IX_Transactions_Status (Status)
); 