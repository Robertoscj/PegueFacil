using System;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid SenderId { get; private set; }
        public Guid? ReceiverId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public virtual User Sender { get; private set; }
        public virtual User Receiver { get; private set; }
        public virtual Wallet SenderWallet { get; private set; }
        public virtual Wallet ReceiverWallet { get; private set; }

        protected Transaction() { } // For ORM

        public Transaction(
            Guid senderId,
            Guid? receiverId,
            decimal amount,
            TransactionType type,
            string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            if (type == TransactionType.Transfer && !receiverId.HasValue)
                throw new ArgumentException("Receiver is required for transfers");

            Id = Guid.NewGuid();
            SenderId = senderId;
            ReceiverId = receiverId;
            Amount = amount;
            Type = type;
            Status = TransactionStatus.Pending;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not pending");

            Status = TransactionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void Fail(string reason)
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not pending");

            Status = TransactionStatus.Failed;
            Description = $"{Description} | Failed: {reason}";
            CompletedAt = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not pending");

            Status = TransactionStatus.Cancelled;
            Description = $"{Description} | Cancelled: {reason}";
            CompletedAt = DateTime.UtcNow;
        }

        public void SetFee(decimal fee)
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not pending");

            Amount += fee;
        }

        public void Process()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not pending");

            if (SenderWallet == null)
                throw new InvalidOperationException("Sender wallet not set");

            if (Type == TransactionType.Transfer && ReceiverWallet == null)
                throw new InvalidOperationException("Receiver wallet not set");

            if (!SenderWallet.CanWithdraw(Amount))
                throw new InvalidOperationException("Insufficient funds or wallet is blocked");

            SenderWallet.Debit(Amount);

            if (Type == TransactionType.Transfer && ReceiverWallet != null)
                ReceiverWallet.Credit(Amount);

            Complete();
        }

        public void SetSenderWallet(Wallet wallet)
        {
            if (wallet.UserId != SenderId)
                throw new ArgumentException("Wallet does not belong to sender");

            SenderWallet = wallet;
        }

        public void SetReceiverWallet(Wallet wallet)
        {
            if (!ReceiverId.HasValue || wallet.UserId != ReceiverId.Value)
                throw new ArgumentException("Wallet does not belong to receiver");

            ReceiverWallet = wallet;
        }

        public void SetSender(User user)
        {
            if (user.Id != SenderId)
                throw new ArgumentException("User ID does not match sender ID");

            Sender = user;
        }

        public void SetReceiver(User user)
        {
            if (!ReceiverId.HasValue || user.Id != ReceiverId.Value)
                throw new ArgumentException("User ID does not match receiver ID");

            Receiver = user;
        }
    }
} 