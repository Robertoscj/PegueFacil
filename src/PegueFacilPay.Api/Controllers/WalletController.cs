using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PegueFacilPay.Domain.Entities;
using PegueFacilPay.Domain.Interfaces;

namespace PegueFacilPay.Api.Controllers
{
    /// <summary>
    /// Controlador para gerenciamento de carteiras
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : BaseController
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionService _transactionService;
        private readonly INotificationService _notificationService;

        public WalletController(
            IWalletRepository walletRepository,
            ITransactionService transactionService,
            INotificationService notificationService)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        /// Obtém todas as carteiras
        /// </summary>
        /// <returns>Lista de todas as carteiras</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetAll()
        {
            var wallets = await _walletRepository.GetAllAsync();
            return Ok(new { message = "Carteiras obtidas com sucesso", data = wallets });
        }

        /// <summary>
        /// Obtém uma carteira pelo seu ID
        /// </summary>
        /// <param name="id">O ID da carteira</param>
        /// <returns>Os detalhes da carteira</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Wallet>> GetById(Guid id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return NotFound(new { message = "Carteira não encontrada" });

            return Ok(new { message = "Carteira obtida com sucesso", data = wallet });
        }

        /// <summary>
        /// Obtém uma carteira pelo ID do usuário
        /// </summary>
        /// <param name="userId">O ID do usuário</param>
        /// <returns>Os detalhes da carteira</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Wallet>> GetByUserId(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return NotFound(new { message = "Carteira não encontrada" });

            return Ok(new { message = "Carteira obtida com sucesso", data = wallet });
        }

        /// <summary>
        /// Obtém o saldo de uma carteira
        /// </summary>
        /// <param name="id">O ID da carteira</param>
        /// <returns>O saldo da carteira</returns>
        [HttpGet("{id}/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<decimal>> GetBalance(Guid id)
        {
            if (!await _walletRepository.ExistsAsync(id))
                return NotFound(new { message = "Carteira não encontrada" });

            var balance = await _walletRepository.GetBalanceAsync(id);
            return Ok(new { message = "Saldo obtido com sucesso", data = balance });
        }

        /// <summary>
        /// Bloqueia uma carteira
        /// </summary>
        /// <param name="id">O ID da carteira a ser bloqueada</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("{id}/block")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BlockWallet(Guid id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return NotFound(new { message = "Carteira não encontrada" });

            wallet.Block();
            var success = await _walletRepository.UpdateAsync(wallet);

            if (!success)
                return StatusCode(500, new { message = "Falha ao bloquear carteira" });

            return Ok(new { message = "Carteira bloqueada com sucesso" });
        }

        /// <summary>
        /// Desbloqueia uma carteira
        /// </summary>
        /// <param name="id">O ID da carteira a ser desbloqueada</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("{id}/unblock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnblockWallet(Guid id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return NotFound(new { message = "Carteira não encontrada" });

            wallet.Unblock();
            var success = await _walletRepository.UpdateAsync(wallet);

            if (!success)
                return StatusCode(500, new { message = "Falha ao desbloquear carteira" });

            return Ok(new { message = "Carteira desbloqueada com sucesso" });
        }

        /// <summary>
        /// Verifica se uma carteira tem saldo suficiente
        /// </summary>
        /// <param name="id">O ID da carteira</param>
        /// <param name="amount">O valor a ser verificado</param>
        /// <returns>Verdadeiro se a carteira tem saldo suficiente</returns>
        [HttpGet("{id}/has-funds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> HasSufficientFunds(Guid id, [FromQuery] decimal amount)
        {
            if (!await _walletRepository.ExistsAsync(id))
                return NotFound(new { message = "Carteira não encontrada" });

            var hasFunds = await _walletRepository.HasSufficientFundsAsync(id, amount);
            return Ok(new { message = "Verificação realizada com sucesso", data = hasFunds });
        }

        /// <summary>
        /// Verifica se uma carteira tem saldo baixo e envia notificação se necessário
        /// </summary>
        /// <param name="id">O ID da carteira</param>
        /// <param name="request">Os detalhes da verificação de saldo baixo</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("{id}/check-low-balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CheckLowBalance(Guid id, [FromBody] CheckLowBalanceRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Requisição inválida" });

            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return NotFound(new { message = "Carteira não encontrada" });

            if (wallet.Balance < request.Threshold)
            {
                await _notificationService.NotifyLowBalanceAsync(wallet, request.Threshold);
                return Ok(new { message = "Notificação de saldo baixo enviada" });
            }

            return Ok(new { message = "Saldo está acima do limite" });
        }
    }

    /// <summary>
    /// Modelo de requisição para verificação de saldo baixo
    /// </summary>
    public class CheckLowBalanceRequest
    {
        /// <summary>
        /// O limite de saldo que ativa a notificação
        /// </summary>
        [Required(ErrorMessage = "O limite é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O limite deve ser maior ou igual a 0")]
        public decimal Threshold { get; set; }
    }
} 