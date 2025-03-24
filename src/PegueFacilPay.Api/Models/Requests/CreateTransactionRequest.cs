using System;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Api.Models.Requests
{
    public class CreateTransactionRequest
    {
        public Guid SenderId { get; set; }
        public Guid? ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public required string Description { get; set; }
    }
} 