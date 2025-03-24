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
    /// Controlador para gerenciamento de usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;

        public UserController(IUserRepository userRepository, IWalletRepository walletRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        /// <summary>
        /// Obtém todos os usuários cadastrados
        /// </summary>
        /// <returns>Lista de todos os usuários</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return HandleSuccess(users);
        }

        /// <summary>
        /// Obtém um usuário pelo seu ID
        /// </summary>
        /// <param name="id">O ID do usuário</param>
        /// <returns>Os detalhes do usuário</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return HandleNotFound("Usuário não encontrado");

            return HandleSuccess(user);
        }

        /// <summary>
        /// Obtém um usuário pelo seu e-mail
        /// </summary>
        /// <param name="email">O endereço de e-mail do usuário</param>
        /// <returns>Os detalhes do usuário</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return HandleNotFound("Usuário não encontrado");

            return HandleSuccess(user);
        }

        /// <summary>
        /// Obtém um usuário pelo seu documento (CPF/CNPJ)
        /// </summary>
        /// <param name="document">O número do documento do usuário</param>
        /// <returns>Os detalhes do usuário</returns>
        [HttpGet("document/{document}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetByDocument(string document)
        {
            var user = await _userRepository.GetByDocumentAsync(document);
            if (user == null)
                return HandleNotFound("Usuário não encontrado");

            return HandleSuccess(user);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="request">Os dados do usuário a ser criado</param>
        /// <returns>O usuário criado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Requisição inválida" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.ExistsByEmailAsync(request.Email))
                return BadRequest(new { message = "E-mail já cadastrado" });

            if (await _userRepository.ExistsByDocumentAsync(request.Document))
                return BadRequest(new { message = "Documento já cadastrado" });

            var user = new User(request.Name, request.Email, request.Document, request.PhoneNumber);
            var wallet = new Wallet(user.Id);

            var success = await _userRepository.CreateAsync(user);
            if (!success)
                return StatusCode(500, new { message = "Falha ao criar usuário" });

            success = await _walletRepository.CreateAsync(wallet);
            if (!success)
                return StatusCode(500, new { message = "Falha ao criar carteira" });

            user.SetWallet(wallet);
            await _userRepository.UpdateAsync(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">O ID do usuário a ser atualizado</param>
        /// <param name="request">Os novos dados do usuário</param>
        /// <returns>O usuário atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Requisição inválida" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            if (request.Email != user.Email && await _userRepository.ExistsByEmailAsync(request.Email))
                return BadRequest(new { message = "E-mail já cadastrado" });

            user.Update(request.Name, request.Email, request.PhoneNumber);
            var success = await _userRepository.UpdateAsync(user);

            if (!success)
                return StatusCode(500, new { message = "Falha ao atualizar usuário" });

            return Ok(user);
        }

        /// <summary>
        /// Desativa um usuário
        /// </summary>
        /// <param name="id">O ID do usuário a ser desativado</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            user.Deactivate();
            var success = await _userRepository.UpdateAsync(user);

            if (!success)
                return StatusCode(500, new { message = "Falha ao desativar usuário" });

            return Ok(new { message = "Usuário desativado com sucesso" });
        }

        /// <summary>
        /// Ativa um usuário
        /// </summary>
        /// <param name="id">O ID do usuário a ser ativado</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            user.Activate();
            var success = await _userRepository.UpdateAsync(user);

            if (!success)
                return StatusCode(500, new { message = "Falha ao ativar usuário" });

            return Ok(new { message = "Usuário ativado com sucesso" });
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _userRepository.ExistsAsync(id))
                return HandleNotFound();

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
    }

    public class CreateUserRequest
    {
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public required string Name { get; set; }

        /// <summary>
        /// Endereço de e-mail do usuário
        /// </summary>
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres")]
        public required string Email { get; set; }

        /// <summary>
        /// Número do documento (CPF/CNPJ) do usuário
        /// </summary>
        [Required(ErrorMessage = "O documento é obrigatório")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo 20 caracteres")]
        public required string Document { get; set; }

        /// <summary>
        /// Número de telefone do usuário
        /// </summary>
        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public required string PhoneNumber { get; set; }
    }

    public class UpdateUserRequest
    {
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public required string Name { get; set; }

        /// <summary>
        /// Endereço de e-mail do usuário
        /// </summary>
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres")]
        public required string Email { get; set; }

        /// <summary>
        /// Número de telefone do usuário
        /// </summary>
        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public required string PhoneNumber { get; set; }
    }
} 