using System;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Domain.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateDepositAsync(Guid userId, decimal amount, string description);
        Task<Transaction> CreateWithdrawalAsync(Guid userId, decimal amount, string description);
        Task<Transaction> CreateTransferAsync(Guid senderId, Guid receiverId, decimal amount, string description);
        Task<Transaction> CreatePaymentAsync(Guid payerId, Guid merchantId, decimal amount, string description);
        Task<Transaction> CreateRefundAsync(Guid originalTransactionId, string reason);
        
        Task<bool> ProcessTransactionAsync(Guid transactionId);
        Task<bool> CancelTransactionAsync(Guid transactionId, string reason);
        
        Task<decimal> GetBalanceAsync(Guid userId);
        Task<bool> HasSufficientFundsAsync(Guid userId, decimal amount);
        
        Task<Transaction> GetTransactionAsync(Guid transactionId);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status);
        Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type);
    }
} 