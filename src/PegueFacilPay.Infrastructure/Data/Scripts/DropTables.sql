-- Drop indexes
DROP INDEX IF EXISTS IX_Users_Email ON Users;
DROP INDEX IF EXISTS IX_Users_Document ON Users;
DROP INDEX IF EXISTS IX_Transactions_SenderId ON Transactions;
DROP INDEX IF EXISTS IX_Transactions_ReceiverId ON Transactions;
DROP INDEX IF EXISTS IX_Transactions_Type ON Transactions;
DROP INDEX IF EXISTS IX_Transactions_Status ON Transactions;
DROP INDEX IF EXISTS IX_Transactions_CreatedAt ON Transactions;

-- Drop tables in correct order to handle foreign key constraints
DROP TABLE IF EXISTS Transactions;
DROP TABLE IF EXISTS Wallets;
DROP TABLE IF EXISTS Users; 