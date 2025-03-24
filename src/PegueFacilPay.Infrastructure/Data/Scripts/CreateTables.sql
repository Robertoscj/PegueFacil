-- Create Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Document NVARCHAR(20) NOT NULL, -- CPF/CNPJ
    PhoneNumber NVARCHAR(20),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT UQ_Users_Document UNIQUE (Document)
);

-- Create Wallets table
CREATE TABLE Wallets (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    IsBlocked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_Wallets_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT UQ_Wallets_UserId UNIQUE (UserId),
    CONSTRAINT CK_Wallets_Balance CHECK (Balance >= 0)
);

-- Create Transactions table
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Type INT NOT NULL, -- Enum: Deposit = 1, Withdrawal = 2, Transfer = 3, Payment = 4, Refund = 5
    Status INT NOT NULL, -- Enum: Pending = 1, Completed = 2, Failed = 3, Cancelled = 4
    Amount DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(500),
    SenderId UNIQUEIDENTIFIER,
    ReceiverId UNIQUEIDENTIFIER,
    OriginalTransactionId UNIQUEIDENTIFIER, -- For refunds, references the original transaction
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CompletedAt DATETIME2,
    CancelledAt DATETIME2,
    CancellationReason NVARCHAR(500),
    FailureReason NVARCHAR(500),
    CONSTRAINT FK_Transactions_Users_Sender FOREIGN KEY (SenderId) REFERENCES Users(Id),
    CONSTRAINT FK_Transactions_Users_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id),
    CONSTRAINT FK_Transactions_Transactions FOREIGN KEY (OriginalTransactionId) REFERENCES Transactions(Id),
    CONSTRAINT CK_Transactions_Amount CHECK (Amount > 0)
);

-- Create indexes for better query performance
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Document ON Users(Document);
CREATE INDEX IX_Transactions_SenderId ON Transactions(SenderId);
CREATE INDEX IX_Transactions_ReceiverId ON Transactions(ReceiverId);
CREATE INDEX IX_Transactions_Type ON Transactions(Type);
CREATE INDEX IX_Transactions_Status ON Transactions(Status);
CREATE INDEX IX_Transactions_CreatedAt ON Transactions(CreatedAt); 