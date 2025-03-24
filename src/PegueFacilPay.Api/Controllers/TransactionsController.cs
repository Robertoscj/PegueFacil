using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PegueFacilPay.Api.Models.Requests;
using PegueFacilPay.Application.Services;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;
using PegueFacilPay.Domain.Interfaces;

namespace PegueFacilPay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly FeeService _feeService;

        public TransactionsController(
            ITransactionRepository transactionRepository,
            IUserRepository userRepository,
            IWalletRepository walletRepository,
            FeeService feeService)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _feeService = feeService ?? throw new ArgumentNullException(nameof(feeService));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request cannot be null" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sender = await _userRepository.GetByIdAsync(request.SenderId);
            if (sender == null)
                return NotFound(new { message = "Sender not found" });

            User? receiver = null;
            if (request.Type == TransactionType.Transfer)
            {
                if (!request.ReceiverId.HasValue)
                    return BadRequest(new { message = "Receiver ID is required for transfers" });

                receiver = await _userRepository.GetByIdAsync(request.ReceiverId.Value);
                if (receiver == null)
                    return NotFound(new { message = "Receiver not found" });
            }

            var senderWallet = await _walletRepository.GetByUserIdAsync(sender.Id);
            if (senderWallet == null)
                return NotFound(new { message = "Sender wallet not found" });

            Wallet? receiverWallet = null;
            if (receiver != null)
            {
                receiverWallet = await _walletRepository.GetByUserIdAsync(receiver.Id);
                if (receiverWallet == null)
                    return NotFound(new { message = "Receiver wallet not found" });
            }

            var transaction = new Transaction(
                sender.Id,
                receiver?.Id,
                request.Amount,
                request.Type,
                request.Description
            );

            transaction.SetSender(sender);
            transaction.SetSenderWallet(senderWallet);

            if (receiver != null && receiverWallet != null)
            {
                transaction.SetReceiver(receiver);
                transaction.SetReceiverWallet(receiverWallet);
            }

            var fee = await _feeService.CalculateFeeAsync(transaction);
            transaction.SetFee(fee);

            try
            {
                transaction.Process();

                var success = await _transactionRepository.CreateAsync(transaction);
                if (!success)
                    return StatusCode(500, new { message = "Failed to save transaction" });

                success = await _walletRepository.UpdateAsync(senderWallet);
                if (!success)
                    return StatusCode(500, new { message = "Failed to update sender wallet" });

                if (receiverWallet != null)
                {
                    success = await _walletRepository.UpdateAsync(receiverWallet);
                    if (!success)
                        return StatusCode(500, new { message = "Failed to update receiver wallet" });
                }

                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
            }
            catch (InvalidOperationException ex)
            {
                transaction.Fail(ex.Message);
                await _transactionRepository.CreateAsync(transaction);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelTransaction(Guid id, [FromBody] CancelTransactionRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request cannot be null" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            try
            {
                transaction.Cancel(request.Reason);
                var success = await _transactionRepository.UpdateAsync(transaction);

                if (!success)
                    return StatusCode(500, new { message = "Failed to update transaction" });

                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 