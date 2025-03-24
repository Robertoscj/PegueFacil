using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;
using PegueFacilPay.Domain.Interfaces;
using PegueFacilPay.Infrastructure.Data;

namespace PegueFacilPay.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DatabaseConnection _db;

        public TransactionRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public async Task<Transaction> GetByIdAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT * FROM Transactions 
                WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Transaction>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Transactions ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Transaction>(sql);
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            using var connection = _db.CreateConnection();
            var sql = @"
                SELECT * FROM Transactions 
                WHERE (SenderId = @UserId OR ReceiverId = @UserId)";

            if (startDate.HasValue)
                sql += " AND CreatedAt >= @StartDate";
            if (endDate.HasValue)
                sql += " AND CreatedAt <= @EndDate";

            sql += " ORDER BY CreatedAt DESC";

            return await connection.QueryAsync<Transaction>(sql, new 
            { 
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            });
        }

        public async Task<IEnumerable<Transaction>> GetByStatusAsync(TransactionStatus status)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT * FROM Transactions 
                WHERE Status = @Status 
                ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Transaction>(sql, new { Status = status });
        }

        public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT * FROM Transactions 
                WHERE Type = @Type 
                ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Transaction>(sql, new { Type = type });
        }

        public async Task<bool> CreateAsync(Transaction transaction)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                INSERT INTO Transactions (
                    Id, Type, Status, Amount, Description, 
                    SenderId, ReceiverId, OriginalTransactionId,
                    CreatedAt, UpdatedAt, CompletedAt, CancelledAt, 
                    CancellationReason, FailureReason
                ) VALUES (
                    @Id, @Type, @Status, @Amount, @Description,
                    @SenderId, @ReceiverId, @OriginalTransactionId,
                    @CreatedAt, @UpdatedAt, @CompletedAt, @CancelledAt,
                    @CancellationReason, @FailureReason
                )";
            
            var result = await connection.ExecuteAsync(sql, transaction);
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Transaction transaction)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Transactions 
                SET Status = @Status,
                    UpdatedAt = @UpdatedAt,
                    CompletedAt = @CompletedAt,
                    CancelledAt = @CancelledAt,
                    CancellationReason = @CancellationReason,
                    FailureReason = @FailureReason
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, transaction);
            return result > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, TransactionStatus status)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Transactions 
                SET Status = @Status,
                    UpdatedAt = @UpdatedAt,
                    CompletedAt = CASE 
                        WHEN @Status = @CompletedStatus THEN @UpdatedAt 
                        ELSE CompletedAt 
                    END
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = id,
                Status = status,
                UpdatedAt = DateTime.UtcNow,
                CompletedStatus = TransactionStatus.Completed
            });
            return result > 0;
        }

        public async Task<bool> CancelTransactionAsync(Guid id, string reason)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Transactions 
                SET Status = @Status,
                    UpdatedAt = @UpdatedAt,
                    CancelledAt = @UpdatedAt,
                    CancellationReason = @Reason
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = id,
                Status = TransactionStatus.Cancelled,
                UpdatedAt = DateTime.UtcNow,
                Reason = reason
            });
            return result > 0;
        }

        public async Task<bool> MarkTransactionFailedAsync(Guid id, string reason)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Transactions 
                SET Status = @Status,
                    UpdatedAt = @UpdatedAt,
                    FailureReason = @Reason
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = id,
                Status = TransactionStatus.Failed,
                UpdatedAt = DateTime.UtcNow,
                Reason = reason
            });
            return result > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Transactions WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<decimal> GetTotalTransactionAmountAsync(Guid userId, TransactionType type)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT COALESCE(SUM(Amount), 0)
                FROM Transactions 
                WHERE Type = @Type 
                AND (SenderId = @UserId OR ReceiverId = @UserId)
                AND Status = @CompletedStatus";
            
            return await connection.ExecuteScalarAsync<decimal>(sql, new 
            { 
                UserId = userId,
                Type = type,
                CompletedStatus = TransactionStatus.Completed
            });
        }

        public async Task<IEnumerable<Transaction>> GetBySenderIdAsync(Guid senderId)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT * FROM Transactions 
                WHERE SenderId = @SenderId 
                ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Transaction>(sql, new { SenderId = senderId });
        }

        public async Task<IEnumerable<Transaction>> GetByReceiverIdAsync(Guid receiverId)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT * FROM Transactions 
                WHERE ReceiverId = @ReceiverId 
                ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Transaction>(sql, new { ReceiverId = receiverId });
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT * FROM Transactions 
                WHERE CreatedAt BETWEEN @StartDate AND @EndDate 
                ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Transaction>(sql, new { StartDate = startDate, EndDate = endDate });
        }
    }
} 