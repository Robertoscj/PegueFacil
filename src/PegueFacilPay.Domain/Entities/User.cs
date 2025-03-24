using System;
using System.Collections.Generic;

namespace PegueFacilPay.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Document { get; private set; } // CPF/CNPJ
        public string PhoneNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public virtual Wallet Wallet { get; private set; }
        public virtual ICollection<Transaction> Transactions { get; private set; }

        protected User() { } // For ORM

        public User(string name, string email, string document, string phoneNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Document = document;
            PhoneNumber = phoneNumber;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Transactions = new List<Transaction>();
        }

        public void Update(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetWallet(Wallet wallet)
        {
            Wallet = wallet;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetId(Guid id)
        {
            Id = id;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDocument(string document)
        {
            Document = document;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = createdAt;
        }

        public void SetUpdatedAt(DateTime? updatedAt)
        {
            UpdatedAt = updatedAt;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 