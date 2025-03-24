using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Interfaces;
using PegueFacilPay.Infrastructure.Data;

namespace PegueFacilPay.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnection _db;

        public UserRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                SELECT u.*, w.* 
                FROM Users u 
                LEFT JOIN Wallets w ON u.Id = w.UserId 
                WHERE u.Id = @Id";
            
            var users = await connection.QueryAsync<User, Wallet, User>(
                sql,
                (user, wallet) =>
                {
                    if (wallet != null)
                        user.SetWallet(wallet);
                    return user;
                },
                new { Id = id },
                splitOn: "Id");

            return users.FirstOrDefault();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Users WHERE Email = @Email";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User> GetByDocumentAsync(string document)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Users WHERE Document = @Document";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Document = document });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT * FROM Users";
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<bool> CreateAsync(User user)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                INSERT INTO Users (Id, Name, Email, Document, PhoneNumber, CreatedAt, UpdatedAt, IsActive) 
                VALUES (@Id, @Name, @Email, @Document, @PhoneNumber, @CreatedAt, @UpdatedAt, @IsActive)";
            
            var result = await connection.ExecuteAsync(sql, user);
            return result > 0;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using var connection = _db.CreateConnection();
            const string sql = @"
                UPDATE Users 
                SET Name = @Name, 
                    Email = @Email, 
                    PhoneNumber = @PhoneNumber, 
                    UpdatedAt = @UpdatedAt, 
                    IsActive = @IsActive 
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, user);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "DELETE FROM Users WHERE Id = @Id";
            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<bool> ExistsByDocumentAsync(string document)
        {
            using var connection = _db.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE Document = @Document";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Document = document });
            return count > 0;
        }
    }
} 