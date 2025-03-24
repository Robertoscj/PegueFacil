using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;
using PegueFacilPay.Domain.Interfaces;

namespace PegueFacilPay.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<Transaction> CreateDepositAsync(Guid userId, decimal amount, string description)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!user.IsActive)
                throw new InvalidOperationException("User is not active");

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            if (wallet.IsBlocked)
                throw new InvalidOperationException("Wallet is blocked");

            var transaction = new Transaction(userId, null, amount, TransactionType.Deposit, description);
            await _transactionRepository.CreateAsync(transaction);
            await _notificationService.NotifyTransactionCreatedAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> CreateWithdrawalAsync(Guid userId, decimal amount, string description)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!user.IsActive)
                throw new InvalidOperationException("User is not active");

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found");

            if (!wallet.CanWithdraw(amount))
                throw new InvalidOperationException("Insufficient funds or wallet is blocked");

            var transaction = new Transaction(userId, null, amount, TransactionType.Withdrawal, description);
            await _transactionRepository.CreateAsync(transaction);
            await _notificationService.NotifyTransactionCreatedAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> CreateTransferAsync(Guid senderId, Guid receiverId, decimal amount, string description)
        {
            if (senderId == receiverId)
                throw new ArgumentException("Sender and receiver cannot be the same");

            var sender = await _userRepository.GetByIdAsync(senderId);
            var receiver = await _userRepository.GetByIdAsync(receiverId);

            if (sender == null || receiver == null)
                throw new ArgumentException("Sender or receiver not found");

            if (!sender.IsActive || !receiver.IsActive)
                throw new InvalidOperationException("Sender or receiver is not active");

            var senderWallet = await _walletRepository.GetByUserIdAsync(senderId);
            var receiverWallet = await _walletRepository.GetByUserIdAsync(receiverId);

            if (senderWallet == null || receiverWallet == null)
                throw new InvalidOperationException("Sender or receiver wallet not found");

            if (!senderWallet.CanWithdraw(amount))
                throw new InvalidOperationException("Insufficient funds or wallet is blocked");

            if (receiverWallet.IsBlocked)
                throw new InvalidOperationException("Receiver wallet is blocked");

            var transaction = new Transaction(senderId, receiverId, amount, TransactionType.Transfer, description);
            await _transactionRepository.CreateAsync(transaction);
            await _notificationService.NotifyTransactionCreatedAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> CreatePaymentAsync(Guid payerId, Guid merchantId, decimal amount, string description)
        {
            if (payerId == merchantId)
                throw new ArgumentException("Payer and merchant cannot be the same");

            var payer = await _userRepository.GetByIdAsync(payerId);
            var merchant = await _userRepository.GetByIdAsync(merchantId);

            if (payer == null || merchant == null)
                throw new ArgumentException("Payer or merchant not found");

            if (!payer.IsActive || !merchant.IsActive)
                throw new InvalidOperationException("Payer or merchant is not active");

            var payerWallet = await _walletRepository.GetByUserIdAsync(payerId);
            var merchantWallet = await _walletRepository.GetByUserIdAsync(merchantId);

            if (payerWallet == null || merchantWallet == null)
                throw new InvalidOperationException("Payer or merchant wallet not found");

            if (!payerWallet.CanWithdraw(amount))
                throw new InvalidOperationException("Insufficient funds or wallet is blocked");

            if (merchantWallet.IsBlocked)
                throw new InvalidOperationException("Merchant wallet is blocked");

            var transaction = new Transaction(payerId, merchantId, amount, TransactionType.Payment, description);
            await _transactionRepository.CreateAsync(transaction);
            await _notificationService.NotifyTransactionCreatedAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> CreateRefundAsync(Guid originalTransactionId, string reason)
        {
            var originalTransaction = await _transactionRepository.GetByIdAsync(originalTransactionId);
            if (originalTransaction == null)
                throw new ArgumentException("Original transaction not found");

            if (originalTransaction.Type != TransactionType.Payment)
                throw new InvalidOperationException("Can only refund payment transactions");

            if (originalTransaction.Status != TransactionStatus.Completed)
                throw new InvalidOperationException("Can only refund completed transactions");

            var merchant = await _userRepository.GetByIdAsync(originalTransaction.ReceiverId.Value);
            var merchantWallet = await _walletRepository.GetByUserIdAsync(merchant.Id);

            if (!merchantWallet.CanWithdraw(originalTransaction.Amount))
                throw new InvalidOperationException("Merchant has insufficient funds for refund");

            var description = $"Refund for transaction {originalTransactionId} - {reason}";
            var refundTransaction = new Transaction(
                originalTransaction.ReceiverId.Value,
                originalTransaction.SenderId,
                originalTransaction.Amount,
                TransactionType.Refund,
                description);

            await _transactionRepository.CreateAsync(refundTransaction);
            await _notificationService.NotifyTransactionCreatedAsync(refundTransaction);

            return refundTransaction;
        }

        public async Task<bool> ProcessTransactionAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            if (transaction.Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not pending");

            try
            {
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        await ProcessDepositAsync(transaction);
                        break;
                    case TransactionType.Withdrawal:
                        await ProcessWithdrawalAsync(transaction);
                        break;
                    case TransactionType.Transfer:
                    case TransactionType.Payment:
                        await ProcessTransferAsync(transaction);
                        break;
                    case TransactionType.Refund:
                        await ProcessRefundAsync(transaction);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid transaction type");
                }

                transaction.Complete();
                await _transactionRepository.UpdateAsync(transaction);
                await _notificationService.NotifyTransactionCompletedAsync(transaction);
                return true;
            }
            catch (Exception ex)
            {
                transaction.Fail(ex.Message);
                await _transactionRepository.UpdateAsync(transaction);
                await _notificationService.NotifyTransactionFailedAsync(transaction, ex.Message);
                return false;
            }
        }

        private async Task ProcessDepositAsync(Transaction transaction)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(transaction.SenderId);
            wallet.Credit(transaction.Amount);
            await _walletRepository.UpdateAsync(wallet);
        }

        private async Task ProcessWithdrawalAsync(Transaction transaction)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(transaction.SenderId);
            wallet.Debit(transaction.Amount);
            await _walletRepository.UpdateAsync(wallet);
        }

        private async Task ProcessTransferAsync(Transaction transaction)
        {
            var senderWallet = await _walletRepository.GetByUserIdAsync(transaction.SenderId);
            var receiverWallet = await _walletRepository.GetByUserIdAsync(transaction.ReceiverId.Value);

            senderWallet.Debit(transaction.Amount);
            receiverWallet.Credit(transaction.Amount);

            await _walletRepository.UpdateAsync(senderWallet);
            await _walletRepository.UpdateAsync(receiverWallet);
        }

        private async Task ProcessRefundAsync(Transaction transaction)
        {
            var merchantWallet = await _walletRepository.GetByUserIdAsync(transaction.SenderId);
            var customerWallet = await _walletRepository.GetByUserIdAsync(transaction.ReceiverId.Value);

            merchantWallet.Debit(transaction.Amount);
            customerWallet.Credit(transaction.Amount);

            await _walletRepository.UpdateAsync(merchantWallet);
            await _walletRepository.UpdateAsync(customerWallet);
        }

        public async Task<bool> CancelTransactionAsync(Guid transactionId, string reason)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            transaction.Cancel(reason);
            await _transactionRepository.UpdateAsync(transaction);
            await _notificationService.NotifyTransactionCancelledAsync(transaction, reason);

            return true;
        }

        public async Task<decimal> GetBalanceAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                throw new ArgumentException("Wallet not found");

            return wallet.Balance;
        }

        public async Task<bool> HasSufficientFundsAsync(Guid userId, decimal amount)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                throw new ArgumentException("Wallet not found");

            return wallet.CanWithdraw(amount);
        }

        public async Task<Transaction> GetTransactionAsync(Guid transactionId)
        {
            return await _transactionRepository.GetByIdAsync(transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var sent = await _transactionRepository.GetBySenderIdAsync(userId);
            var received = await _transactionRepository.GetByReceiverIdAsync(userId);

            var allTransactions = new List<Transaction>();
            allTransactions.AddRange(sent);
            allTransactions.AddRange(received);

            if (startDate.HasValue)
                allTransactions = allTransactions.FindAll(t => t.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                allTransactions = allTransactions.FindAll(t => t.CreatedAt <= endDate.Value);

            return allTransactions;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status)
        {
            return await _transactionRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type)
        {
            return await _transactionRepository.GetByTypeAsync(type);
        }
    }
} 