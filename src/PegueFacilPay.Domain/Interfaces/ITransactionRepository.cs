using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(Guid id);
        Task<IEnumerable<Transaction>> GetBySenderIdAsync(Guid senderId);
        Task<IEnumerable<Transaction>> GetByReceiverIdAsync(Guid receiverId);
        Task<IEnumerable<Transaction>> GetByStatusAsync(TransactionStatus status);
        Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type);
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> CreateAsync(Transaction transaction);
        Task<bool> UpdateAsync(Transaction transaction);
        Task<bool> UpdateStatusAsync(Guid id, TransactionStatus status);
        Task<bool> ExistsAsync(Guid id);
    }
} 