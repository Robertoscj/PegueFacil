-- Seed test users
INSERT INTO Users (Id, Name, Email, Document, PhoneNumber, CreatedAt, UpdatedAt, IsActive)
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'John Doe', 'john@example.com', '12345678900', '+5511999999999', GETUTCDATE(), GETUTCDATE(), 1),
    ('22222222-2222-2222-2222-222222222222', 'Jane Smith', 'jane@example.com', '98765432100', '+5511988888888', GETUTCDATE(), GETUTCDATE(), 1),
    ('33333333-3333-3333-3333-333333333333', 'Acme Store', 'store@acme.com', '12345678000190', '+5511977777777', GETUTCDATE(), GETUTCDATE(), 1);

-- Seed wallets for test users
INSERT INTO Wallets (Id, UserId, Balance, IsBlocked, CreatedAt, UpdatedAt)
VALUES 
    ('AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA', '11111111-1111-1111-1111-111111111111', 1000.00, 0, GETUTCDATE(), GETUTCDATE()),
    ('BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB', '22222222-2222-2222-2222-222222222222', 500.00, 0, GETUTCDATE(), GETUTCDATE()),
    ('CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC', '33333333-3333-3333-3333-333333333333', 2000.00, 0, GETUTCDATE(), GETUTCDATE());

-- Seed some test transactions
DECLARE @Now DateTime2 = GETUTCDATE();

-- Deposit transaction
INSERT INTO Transactions (
    Id, Type, Status, Amount, Description,
    SenderId, ReceiverId, OriginalTransactionId,
    CreatedAt, UpdatedAt, CompletedAt, CancelledAt,
    CancellationReason, FailureReason
)
VALUES (
    'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD',
    1, -- Deposit
    2, -- Completed
    500.00,
    'Initial deposit',
    '11111111-1111-1111-1111-111111111111',
    '11111111-1111-1111-1111-111111111111',
    NULL,
    DATEADD(hour, -2, @Now),
    DATEADD(hour, -2, @Now),
    DATEADD(hour, -2, @Now),
    NULL, NULL, NULL
);

-- Transfer transaction
INSERT INTO Transactions (
    Id, Type, Status, Amount, Description,
    SenderId, ReceiverId, OriginalTransactionId,
    CreatedAt, UpdatedAt, CompletedAt, CancelledAt,
    CancellationReason, FailureReason
)
VALUES (
    'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE',
    3, -- Transfer
    2, -- Completed
    100.00,
    'Transfer to Jane',
    '11111111-1111-1111-1111-111111111111',
    '22222222-2222-2222-2222-222222222222',
    NULL,
    DATEADD(hour, -1, @Now),
    DATEADD(hour, -1, @Now),
    DATEADD(hour, -1, @Now),
    NULL, NULL, NULL
);

-- Payment transaction
INSERT INTO Transactions (
    Id, Type, Status, Amount, Description,
    SenderId, ReceiverId, OriginalTransactionId,
    CreatedAt, UpdatedAt, CompletedAt, CancelledAt,
    CancellationReason, FailureReason
)
VALUES (
    'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF',
    4, -- Payment
    2, -- Completed
    150.00,
    'Payment to Acme Store',
    '22222222-2222-2222-2222-222222222222',
    '33333333-3333-3333-3333-333333333333',
    NULL,
    @Now,
    @Now,
    @Now,
    NULL, NULL, NULL
); 