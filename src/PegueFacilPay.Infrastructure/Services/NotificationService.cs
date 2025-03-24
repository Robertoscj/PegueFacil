using System;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Interfaces;

namespace PegueFacilPay.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        public async Task NotifyTransactionCreatedAsync(Transaction transaction)
        {
            var title = "Nova Transação";
            var message = $"Uma nova transação foi criada: {transaction.Type} - R$ {transaction.Amount:N2}";
            await NotifyUserAsync(transaction.SenderId, title, message);

            if (transaction.ReceiverId.HasValue)
            {
                message = $"Você recebeu uma nova transação: {transaction.Type} - R$ {transaction.Amount:N2}";
                await NotifyUserAsync(transaction.ReceiverId.Value, title, message);
            }
        }

        public async Task NotifyTransactionCompletedAsync(Transaction transaction)
        {
            var title = "Transação Concluída";
            var message = $"Sua transação foi concluída com sucesso: {transaction.Type} - R$ {transaction.Amount:N2}";
            await NotifyUserAsync(transaction.SenderId, title, message);

            if (transaction.ReceiverId.HasValue)
            {
                message = $"Você recebeu uma transação: {transaction.Type} - R$ {transaction.Amount:N2}";
                await NotifyUserAsync(transaction.ReceiverId.Value, title, message);
            }
        }

        public async Task NotifyTransactionFailedAsync(Transaction transaction, string reason)
        {
            var title = "Falha na Transação";
            var message = $"Sua transação falhou: {transaction.Type} - R$ {transaction.Amount:N2}. Motivo: {reason}";
            await NotifyUserAsync(transaction.SenderId, title, message);

            if (transaction.ReceiverId.HasValue)
            {
                message = $"Uma transação para você falhou: {transaction.Type} - R$ {transaction.Amount:N2}. Motivo: {reason}";
                await NotifyUserAsync(transaction.ReceiverId.Value, title, message);
            }
        }

        public async Task NotifyTransactionCancelledAsync(Transaction transaction, string reason)
        {
            var title = "Transação Cancelada";
            var message = $"Sua transação foi cancelada: {transaction.Type} - R$ {transaction.Amount:N2}. Motivo: {reason}";
            await NotifyUserAsync(transaction.SenderId, title, message);

            if (transaction.ReceiverId.HasValue)
            {
                message = $"Uma transação para você foi cancelada: {transaction.Type} - R$ {transaction.Amount:N2}. Motivo: {reason}";
                await NotifyUserAsync(transaction.ReceiverId.Value, title, message);
            }
        }

        public async Task NotifyLowBalanceAsync(Wallet wallet, decimal threshold)
        {
            var title = "Saldo Baixo";
            var message = $"Seu saldo está abaixo do limite mínimo de R$ {threshold:N2}. Saldo atual: R$ {wallet.Balance:N2}";
            await NotifyUserAsync(wallet.UserId, title, message);
        }

        public async Task NotifyWalletBlockedAsync(Wallet wallet)
        {
            var title = "Carteira Bloqueada";
            var message = "Sua carteira foi bloqueada. Entre em contato com o suporte para mais informações.";
            await NotifyUserAsync(wallet.UserId, title, message);
        }

        public async Task NotifyWalletUnblockedAsync(Wallet wallet)
        {
            var title = "Carteira Desbloqueada";
            var message = "Sua carteira foi desbloqueada e está pronta para uso.";
            await NotifyUserAsync(wallet.UserId, title, message);
        }

        public async Task NotifyUserAsync(Guid userId, string title, string message)
        {
            // TODO: Implement actual notification delivery (email, SMS, push notification, etc.)
            await Task.CompletedTask;
        }
    }
} 