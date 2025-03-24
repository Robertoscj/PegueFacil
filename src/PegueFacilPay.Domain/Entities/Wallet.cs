using System;
using System.Collections.Generic;

namespace PegueFacilPay.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }
        public bool IsBlocked { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public virtual User User { get; private set; }
        public virtual ICollection<Transaction> Transactions { get; private set; }

        protected Wallet() { } // For ORM

        public Wallet(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Balance = 0;
            IsBlocked = false;
            CreatedAt = DateTime.UtcNow;
            Transactions = new List<Transaction>();
        }

        public bool CanWithdraw(decimal amount)
        {
            return !IsBlocked && Balance >= amount && amount > 0;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            if (IsBlocked)
                throw new InvalidOperationException("Wallet is blocked");

            Balance += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            if (IsBlocked)
                throw new InvalidOperationException("Wallet is blocked");

            if (!CanWithdraw(amount))
                throw new InvalidOperationException("Insufficient funds");

            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Block()
        {
            IsBlocked = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unblock()
        {
            IsBlocked = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 