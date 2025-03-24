using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Enums;
using PegueFacilPay.Domain.Interfaces;

namespace PegueFacilPay.Api.Controllers
{
    /// <summary>
    /// Controlador para gerenciamento de transações financeiras
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        /// <summary>
        /// Cria uma nova transação de depósito
        /// </summary>
        /// <param name="request">Os detalhes do depósito</param>
        /// <returns>A transação criada</returns>
        [HttpPost("deposit")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transaction>> CreateDeposit([FromBody] CreateDepositRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateDepositAsync(
                    request.UserId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                return HandleError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Cria uma nova transação de saque
        /// </summary>
        /// <param name="request">Os detalhes do saque</param>
        /// <returns>A transação criada</returns>
        [HttpPost("withdrawal")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transaction>> CreateWithdrawal([FromBody] CreateWithdrawalRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateWithdrawalAsync(
                    request.UserId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                return HandleError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Cria uma nova transação de transferência entre usuários
        /// </summary>
        /// <param name="request">Os detalhes da transferência</param>
        /// <returns>A transação criada</returns>
        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transaction>> CreateTransfer([FromBody] CreateTransferRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateTransferAsync(
                    request.SenderId,
                    request.ReceiverId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                return HandleError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Cria uma nova transação de pagamento
        /// </summary>
        /// <param name="request">Os detalhes do pagamento</param>
        /// <returns>A transação criada</returns>
        [HttpPost("payment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transaction>> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreatePaymentAsync(
                    request.PayerId,
                    request.MerchantId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                return HandleError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Cria uma nova transação de reembolso para uma transação existente
        /// </summary>
        /// <param name="request">Os detalhes do reembolso</param>
        /// <returns>A transação de reembolso criada</returns>
        [HttpPost("refund")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transaction>> CreateRefund([FromBody] CreateRefundRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateRefundAsync(
                    request.OriginalTransactionId,
                    request.Reason);

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                return HandleError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Processa uma transação pendente
        /// </summary>
        /// <param name="id">O ID da transação a ser processada</param>
        /// <returns>Verdadeiro se a transação foi processada com sucesso</returns>
        [HttpPost("{id}/process")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> ProcessTransaction(Guid id)
        {
            try
            {
                var result = await _transactionService.ProcessTransactionAsync(id);
                return HandleSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return HandleNotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Cancela uma transação pendente
        /// </summary>
        /// <param name="id">O ID da transação a ser cancelada</param>
        /// <param name="request">Os detalhes do cancelamento</param>
        /// <returns>Verdadeiro se a transação foi cancelada com sucesso</returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> CancelTransaction(Guid id, [FromBody] CancelTransactionRequest request)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(id, request.Reason);
                return HandleSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return HandleNotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return HandleError(ex.Message);
            }
        }

        /// <summary>
        /// Obtém uma transação pelo seu ID
        /// </summary>
        /// <param name="id">O ID da transação</param>
        /// <returns>Os detalhes da transação</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
        {
            var transaction = await _transactionService.GetTransactionAsync(id);
            if (transaction == null)
                return HandleNotFound("Transação não encontrada");

            return HandleSuccess(transaction);
        }

        /// <summary>
        /// Obtém todas as transações de um usuário
        /// </summary>
        /// <param name="userId">O ID do usuário</param>
        /// <param name="startDate">Data inicial opcional para filtro</param>
        /// <param name="endDate">Data final opcional para filtro</param>
        /// <returns>Lista de transações do usuário</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions(
            Guid userId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var transactions = await _transactionService.GetUserTransactionsAsync(userId, startDate, endDate);
            return HandleSuccess(transactions);
        }

        /// <summary>
        /// Obtém todas as transações com um status específico
        /// </summary>
        /// <param name="status">O status da transação para filtrar</param>
        /// <returns>Lista de transações com o status especificado</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByStatus(TransactionStatus status)
        {
            var transactions = await _transactionService.GetTransactionsByStatusAsync(status);
            return HandleSuccess(transactions);
        }

        /// <summary>
        /// Obtém todas as transações de um tipo específico
        /// </summary>
        /// <param name="type">O tipo de transação para filtrar</param>
        /// <returns>Lista de transações do tipo especificado</returns>
        [HttpGet("type/{type}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByType(TransactionType type)
        {
            var transactions = await _transactionService.GetTransactionsByTypeAsync(type);
            return HandleSuccess(transactions);
        }
    }

    public class CreateDepositRequest
    {
        /// <summary>
        /// O ID do usuário que está fazendo o depósito
        /// </summary>
        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public Guid UserId { get; set; }

        /// <summary>
        /// O valor do depósito
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Descrição do depósito
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public required string Description { get; set; }
    }

    public class CreateWithdrawalRequest
    {
        /// <summary>
        /// O ID do usuário que está fazendo o saque
        /// </summary>
        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public Guid UserId { get; set; }

        /// <summary>
        /// O valor do saque
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Descrição do saque
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public required string Description { get; set; }
    }

    public class CreateTransferRequest
    {
        /// <summary>
        /// O ID do usuário que está enviando o dinheiro
        /// </summary>
        [Required(ErrorMessage = "O ID do remetente é obrigatório")]
        public Guid SenderId { get; set; }

        /// <summary>
        /// O ID do usuário que está recebendo o dinheiro
        /// </summary>
        [Required(ErrorMessage = "O ID do destinatário é obrigatório")]
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// O valor da transferência
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Descrição da transferência
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public required string Description { get; set; }
    }

    public class CreatePaymentRequest
    {
        /// <summary>
        /// O ID do usuário que está fazendo o pagamento
        /// </summary>
        [Required]
        public Guid PayerId { get; set; }

        /// <summary>
        /// O ID do comerciante que está recebendo o pagamento
        /// </summary>
        [Required]
        public Guid MerchantId { get; set; }

        /// <summary>
        /// O valor do pagamento
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Descrição do pagamento
        /// </summary>
        [Required]
        [StringLength(500)]
        public required string Description { get; set; }
    }

    public class CreateRefundRequest
    {
        /// <summary>
        /// O ID da transação original para reembolsar
        /// </summary>
        [Required]
        public Guid OriginalTransactionId { get; set; }

        /// <summary>
        /// Motivo do reembolso
        /// </summary>
        [Required]
        [StringLength(500)]
        public required string Reason { get; set; }
    }

    public class CancelTransactionRequest
    {
        /// <summary>
        /// Motivo do cancelamento da transação
        /// </summary>
        [Required(ErrorMessage = "O motivo é obrigatório")]
        [StringLength(500, ErrorMessage = "O motivo deve ter no máximo 500 caracteres")]
        public required string Reason { get; set; }
    }
} 