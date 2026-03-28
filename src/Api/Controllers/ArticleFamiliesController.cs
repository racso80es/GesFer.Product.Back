using GesFer.Product.Back.Application.Commands.ArticleFamilies;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.ArticleFamilies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GesFer.Product.Back.Api.Controllers;

[ApiController]
[Route("api/article-families")]
[Authorize]
public class ArticleFamiliesController : ControllerBase
{
    private readonly ICommandHandler<CreateArticleFamilyCommand, ArticleFamilyDto> _createHandler;
    private readonly ICommandHandler<UpdateArticleFamilyCommand, ArticleFamilyDto> _updateHandler;
    private readonly ICommandHandler<DeleteArticleFamilyCommand> _deleteHandler;
    private readonly ICommandHandler<GetArticleFamilyByIdCommand, ArticleFamilyDto?> _getByIdHandler;
    private readonly ICommandHandler<GetAllArticleFamiliesCommand, List<ArticleFamilyDto>> _getAllHandler;
    private readonly ILogger<ArticleFamiliesController> _logger;

    public ArticleFamiliesController(
        ICommandHandler<CreateArticleFamilyCommand, ArticleFamilyDto> createHandler,
        ICommandHandler<UpdateArticleFamilyCommand, ArticleFamilyDto> updateHandler,
        ICommandHandler<DeleteArticleFamilyCommand> deleteHandler,
        ICommandHandler<GetArticleFamilyByIdCommand, ArticleFamilyDto?> getByIdHandler,
        ICommandHandler<GetAllArticleFamiliesCommand, List<ArticleFamilyDto>> getAllHandler,
        ILogger<ArticleFamiliesController> logger)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _getAllHandler = getAllHandler;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las familias de artículos de la empresa del usuario autenticado
    /// </summary>
    /// <returns>Una lista de familias de artículos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ArticleFamilyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var companyId = this.GetCompanyId();
            var command = new GetAllArticleFamiliesCommand(companyId);
            var result = await _getAllHandler.HandleAsync(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener familias de artículos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una familia de artículos por su ID
    /// </summary>
    /// <param name="id">El ID de la familia de artículos</param>
    /// <returns>La familia de artículos solicitada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ArticleFamilyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var companyId = this.GetCompanyId();
            var command = new GetArticleFamilyByIdCommand(id, companyId);
            var result = await _getByIdHandler.HandleAsync(command);
            if (result == null)
                return NotFound(new { message = $"No se encontró la familia de artículos con ID {id}" });
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener familia de artículos {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva familia de artículos
    /// </summary>
    /// <param name="dto">Los datos de la familia de artículos a crear</param>
    /// <returns>La familia de artículos creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ArticleFamilyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateArticleFamilyDto dto)
    {
        try
        {
            dto.CompanyId = this.GetCompanyId();
            var command = new CreateArticleFamilyCommand(dto);
            var result = await _createHandler.HandleAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear familia de artículos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una familia de artículos existente
    /// </summary>
    /// <param name="id">El ID de la familia de artículos a actualizar</param>
    /// <param name="dto">Los nuevos datos de la familia de artículos</param>
    /// <returns>La familia de artículos actualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ArticleFamilyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleFamilyDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El ID del cuerpo no coincide con el de la URL." });
            var command = new UpdateArticleFamilyCommand(id, dto);
            var result = await _updateHandler.HandleAsync(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar familia de artículos {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una familia de artículos por su ID
    /// </summary>
    /// <param name="id">El ID de la familia de artículos a eliminar</param>
    /// <returns>Respuesta vacía si se elimina correctamente</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteArticleFamilyCommand(id);
            await _deleteHandler.HandleAsync(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar familia de artículos {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
