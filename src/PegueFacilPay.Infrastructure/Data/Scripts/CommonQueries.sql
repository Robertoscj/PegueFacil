-- Get user details with wallet balance
SELECT 
    u.*,
    w.Balance,
    w.IsBlocked as WalletBlocked
FROM Users u
LEFT JOIN Wallets w ON u.Id = w.UserId
WHERE u.Id = @UserId;

-- Get user's transaction history with details
SELECT 
    t.*,
    sender.Name as SenderName,
    receiver.Name as ReceiverName
FROM Transactions t
LEFT JOIN Users sender ON t.SenderId = sender.Id
LEFT JOIN Users receiver ON t.ReceiverId = receiver.Id
WHERE t.SenderId = @UserId OR t.ReceiverId = @UserId
ORDER BY t.CreatedAt DESC;

-- Get transaction summary by type for a user
SELECT 
    t.Type,
    COUNT(*) as TransactionCount,
    SUM(t.Amount) as TotalAmount
FROM Transactions t
WHERE (t.SenderId = @UserId OR t.ReceiverId = @UserId)
    AND t.Status = 2 -- Completed
GROUP BY t.Type;

-- Get daily transaction summary
SELECT 
    CAST(t.CreatedAt as DATE) as TransactionDate,
    COUNT(*) as TransactionCount,
    SUM(t.Amount) as TotalAmount
FROM Transactions t
WHERE t.Status = 2 -- Completed
    AND t.CreatedAt >= DATEADD(day, -30, GETUTCDATE())
GROUP BY CAST(t.CreatedAt as DATE)
ORDER BY TransactionDate DESC;

-- Get pending transactions
SELECT 
    t.*,
    sender.Name as SenderName,
    receiver.Name as ReceiverName
FROM Transactions t
LEFT JOIN Users sender ON t.SenderId = sender.Id
LEFT JOIN Users receiver ON t.ReceiverId = receiver.Id
WHERE t.Status = 1 -- Pending
ORDER BY t.CreatedAt;

-- Get blocked wallets
SELECT 
    w.*,
    u.Name,
    u.Email,
    u.Document
FROM Wallets w
INNER JOIN Users u ON w.UserId = u.Id
WHERE w.IsBlocked = 1;

-- Get users with low balance (less than 100)
SELECT 
    u.Name,
    u.Email,
    w.Balance
FROM Users u
INNER JOIN Wallets w ON u.Id = w.UserId
WHERE w.Balance < 100
ORDER BY w.Balance;

-- Get transaction statistics
SELECT
    COUNT(*) as TotalTransactions,
    SUM(Amount) as TotalAmount,
    AVG(Amount) as AverageAmount,
    MIN(Amount) as MinAmount,
    MAX(Amount) as MaxAmount
FROM Transactions
WHERE Status = 2 -- Completed
    AND CreatedAt >= DATEADD(day, -30, GETUTCDATE());

-- Get refund history
SELECT 
    t.*,
    ot.Amount as OriginalAmount,
    ot.CreatedAt as OriginalTransactionDate
FROM Transactions t
INNER JOIN Transactions ot ON t.OriginalTransactionId = ot.Id
WHERE t.Type = 5 -- Refund
ORDER BY t.CreatedAt DESC;

-- Get user activity summary
SELECT 
    u.Id,
    u.Name,
    u.Email,
    w.Balance,
    (SELECT COUNT(*) FROM Transactions t WHERE t.SenderId = u.Id) as TransactionsSent,
    (SELECT COUNT(*) FROM Transactions t WHERE t.ReceiverId = u.Id) as TransactionsReceived,
    (SELECT MAX(CreatedAt) FROM Transactions t WHERE t.SenderId = u.Id OR t.ReceiverId = u.Id) as LastActivity
FROM Users u
LEFT JOIN Wallets w ON u.Id = w.UserId
WHERE u.IsActive = 1; 