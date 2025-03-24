using System;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;
using System.Collections.Generic;

namespace PegueFacilPay.Domain.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> GetByIdAsync(Guid id);
        Task<Wallet> GetByUserIdAsync(Guid userId);
        Task<bool> CreateAsync(Wallet wallet);
        Task<bool> UpdateAsync(Wallet wallet);
        Task<bool> UpdateBalanceAsync(Guid id, decimal newBalance);
        Task<bool> BlockAsync(Guid id);
        Task<bool> UnblockAsync(Guid id);
        Task<decimal> GetBalanceAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasSufficientFundsAsync(Guid id, decimal amount);
        Task<IEnumerable<Wallet>> GetAllAsync();
    }
} 