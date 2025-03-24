using System.ComponentModel.DataAnnotations;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Application.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public string MerchantId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public string Description { get; set; }
    }
} 