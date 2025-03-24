using System;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Application.DTOs
{
    public class TransactionResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string MerchantId { get; set; }
        public string CustomerId { get; set; }
        public decimal Fee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
} 