using System;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;

namespace PegueFacilPay.Application.Services
{
    public class DynamicFeeService
    {
        private const decimal BASE_FEE_PERCENTAGE = 2.5m; // 2.5%
        private const decimal MIN_FEE_PERCENTAGE = 1.0m;  // 1.0%
        private const decimal MAX_FEE_PERCENTAGE = 4.5m;  // 4.5%

        public async Task<decimal> CalculateDynamicFee(Transaction transaction, decimal riskScore)
        {
            // Base calculation considering transaction amount
            decimal feePercentage = BASE_FEE_PERCENTAGE;

            // Adjust based on transaction amount (volume discount)
            if (transaction.Amount >= 10000)
                feePercentage -= 0.5m;
            else if (transaction.Amount >= 5000)
                feePercentage -= 0.3m;
            else if (transaction.Amount >= 1000)
                feePercentage -= 0.1m;

            // Adjust based on risk score (0 to 1, where 1 is highest risk)
            feePercentage += (riskScore * 2.0m); // Can increase fee up to 2% based on risk

            // Ensure fee stays within acceptable range
            feePercentage = Math.Max(MIN_FEE_PERCENTAGE, Math.Min(MAX_FEE_PERCENTAGE, feePercentage));

            // Calculate final fee amount
            decimal feeAmount = (transaction.Amount * feePercentage) / 100;

            // Minimum fee of R$ 1,00
            return Math.Max(1.00m, feeAmount);
        }
    }
} 