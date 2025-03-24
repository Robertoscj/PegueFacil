using System;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;

namespace PegueFacilPay.Domain.Interfaces
{
    public interface INotificationService
    {
        Task NotifyTransactionCreatedAsync(Transaction transaction);
        Task NotifyTransactionCompletedAsync(Transaction transaction);
        Task NotifyTransactionFailedAsync(Transaction transaction, string reason);
        Task NotifyTransactionCancelledAsync(Transaction transaction, string reason);
        Task NotifyLowBalanceAsync(Wallet wallet, decimal threshold);
        Task NotifyWalletBlockedAsync(Wallet wallet);
        Task NotifyWalletUnblockedAsync(Wallet wallet);
        Task NotifyUserAsync(Guid userId, string title, string message);
    }
} 