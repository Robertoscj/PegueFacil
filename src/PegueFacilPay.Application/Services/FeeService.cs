using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;

namespace PegueFacilPay.Application.Services
{
    public class FeeService
    {
        public async Task<decimal> CalculateFeeAsync(Transaction transaction)
        {
            // This is a simplified fee calculation. In a real application,
            // this would consider various factors like:
            // - Transaction amount
            // - Transaction type
            // - User history
            // - Risk assessment
            // - Time of day
            // - etc.

            var baseFee = transaction.Type switch
            {
                TransactionType.Transfer => 2.00m,
                TransactionType.Deposit => 1.50m,
                TransactionType.Withdrawal => 3.00m,
                _ => 0m
            };

            var percentageFee = transaction.Amount * 0.01m; // 1% fee
            return baseFee + percentageFee;
        }
    }
} 