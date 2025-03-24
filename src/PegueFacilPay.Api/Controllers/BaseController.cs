using Microsoft.AspNetCore.Mvc;

namespace PegueFacilPay.Api.Controllers
{
    /// <summary>
    /// Controlador base que fornece funcionalidades comuns para todos os controladores
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Manipula respostas de sucesso
        /// </summary>
        /// <param name="data">Os dados a serem retornados</param>
        /// <returns>Uma resposta HTTP 200 OK com os dados</returns>
        protected ActionResult<T> HandleSuccess<T>(T data)
        {
            return Ok(new { message = "Operação realizada com sucesso", data });
        }

        /// <summary>
        /// Manipula respostas de não encontrado
        /// </summary>
        /// <param name="message">A mensagem de erro opcional</param>
        /// <returns>Uma resposta HTTP 404 Not Found</returns>
        protected ActionResult HandleNotFound(string message = "Recurso não encontrado")
        {
            return NotFound(new { message });
        }

        /// <summary>
        /// Manipula respostas de erro do servidor
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 500 Internal Server Error</returns>
        protected ActionResult HandleServerError(string message = "Erro interno do servidor")
        {
            return StatusCode(500, new { message });
        }

        /// <summary>
        /// Manipula respostas de requisição inválida
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 400 Bad Request</returns>
        protected ActionResult HandleBadRequest(string message = "Requisição inválida")
        {
            return BadRequest(new { message });
        }

        /// <summary>
        /// Manipula respostas de erro
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 400 Bad Request</returns>
        protected ActionResult HandleError(string message)
        {
            return BadRequest(new { message });
        }

        /// <summary>
        /// Manipula respostas de criação
        /// </summary>
        /// <param name="routeName">O nome da rota</param>
        /// <param name="routeValues">Os valores da rota</param>
        /// <param name="data">Os dados criados</param>
        /// <returns>Uma resposta HTTP 201 Created</returns>
        protected ActionResult HandleCreated(string routeName, object routeValues, object data)
        {
            return CreatedAtRoute(routeName, routeValues, new { message = "Recurso criado com sucesso", data });
        }

        /// <summary>
        /// Manipula respostas de erro de validação
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 422 Unprocessable Entity</returns>
        protected ActionResult HandleValidationError(string message)
        {
            return UnprocessableEntity(new { message });
        }

        /// <summary>
        /// Manipula respostas de não autorizado
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 401 Unauthorized</returns>
        protected ActionResult HandleUnauthorized(string message = "Acesso não autorizado")
        {
            return Unauthorized(new { message });
        }

        /// <summary>
        /// Manipula respostas de acesso proibido
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 403 Forbidden</returns>
        protected ActionResult HandleForbidden(string message = "Acesso proibido")
        {
            return StatusCode(403, new { message });
        }

        /// <summary>
        /// Manipula respostas de conflito
        /// </summary>
        /// <param name="message">A mensagem de erro</param>
        /// <returns>Uma resposta HTTP 409 Conflict</returns>
        protected ActionResult HandleConflict(string message)
        {
            return Conflict(new { message });
        }
    }
} 