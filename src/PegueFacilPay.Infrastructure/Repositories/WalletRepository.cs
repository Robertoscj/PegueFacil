using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Interfaces;
using PegueFacilPay.Infrastructure.Data;

namespace PegueFacilPay.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly DatabaseConnection _db;

        public WalletRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public async Task<Wallet> GetByIdAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Wallets WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Wallet>(sql, new { Id = id });
        }

        public async Task<Wallet> GetByUserIdAsync(Guid userId)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Wallets WHERE UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Wallet>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Wallets";
            return await connection.QueryAsync<Wallet>(sql);
        }

        public async Task<bool> CreateAsync(Wallet wallet)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                INSERT INTO Wallets (Id, UserId, Balance, IsBlocked, CreatedAt, UpdatedAt) 
                VALUES (@Id, @UserId, @Balance, @IsBlocked, @CreatedAt, @UpdatedAt)";
            
            var result = await connection.ExecuteAsync(sql, wallet);
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Wallet wallet)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Wallets 
                SET Balance = @Balance, 
                    IsBlocked = @IsBlocked, 
                    UpdatedAt = @UpdatedAt 
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, wallet);
            return result > 0;
        }

        public async Task<bool> UpdateBalanceAsync(Guid id, decimal amount)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Wallets 
                SET Balance = Balance + @Amount,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, 
                new { 
                    Id = id, 
                    Amount = amount,
                    UpdatedAt = DateTime.UtcNow
                });
            return result > 0;
        }

        public async Task<bool> BlockWalletAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Wallets 
                SET IsBlocked = 1,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, 
                new { 
                    Id = id,
                    UpdatedAt = DateTime.UtcNow
                });
            return result > 0;
        }

        public async Task<bool> UnblockWalletAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Wallets 
                SET IsBlocked = 0,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, 
                new { 
                    Id = id,
                    UpdatedAt = DateTime.UtcNow
                });
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "DELETE FROM Wallets WHERE Id = @Id";
            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Wallets WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<decimal> GetBalanceAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT Balance FROM Wallets WHERE Id = @Id";
            return await connection.ExecuteScalarAsync<decimal>(sql, new { Id = id });
        }

        public async Task<bool> HasSufficientFundsAsync(Guid id, decimal amount)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT Balance FROM Wallets WHERE Id = @Id";
            var balance = await connection.ExecuteScalarAsync<decimal>(sql, new { Id = id });
            return balance >= amount;
        }

        public async Task<bool> BlockAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Wallets 
                SET IsBlocked = 1,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = id,
                UpdatedAt = DateTime.UtcNow
            });
            return result > 0;
        }

        public async Task<bool> UnblockAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Wallets 
                SET IsBlocked = 0,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = id,
                UpdatedAt = DateTime.UtcNow
            });
            return result > 0;
        }
    }
} 