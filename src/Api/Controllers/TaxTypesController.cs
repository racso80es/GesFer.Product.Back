using GesFer.Product.Back.Application.Commands.TaxTypes;
using GesFer.Product.Back.Application.Common.Interfaces;
using GesFer.Product.Back.Application.DTOs.TaxTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GesFer.Product.Back.Api.Controllers;

/// <summary>
/// Controlador para la gestión de tipos de impuestos (Tax Types)
/// </summary>
[Route("api/tax-types")]
[ApiController]
[Authorize]
public class TaxTypesController : ControllerBase
{
    private readonly ICommandHandler<GetAllTaxTypesCommand, List<TaxTypeDto>> _getAllHandler;
    private readonly ICommandHandler<GetTaxTypeByIdCommand, TaxTypeDto?> _getByIdHandler;
    private readonly ICommandHandler<CreateTaxTypeCommand, Guid> _createHandler;
    private readonly ICommandHandler<UpdateTaxTypeCommand> _updateHandler;
    private readonly ICommandHandler<DeleteTaxTypeCommand> _deleteHandler;

    public TaxTypesController(
        ICommandHandler<GetAllTaxTypesCommand, List<TaxTypeDto>> getAllHandler,
        ICommandHandler<GetTaxTypeByIdCommand, TaxTypeDto?> getByIdHandler,
        ICommandHandler<CreateTaxTypeCommand, Guid> createHandler,
        ICommandHandler<UpdateTaxTypeCommand> updateHandler,
        ICommandHandler<DeleteTaxTypeCommand> deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    /// <summary>
    /// Obtiene todos los tipos de impuestos de la empresa del usuario autenticado
    /// </summary>
    /// <returns>Lista de tipos de impuestos</returns>
    [HttpGet]
    [ProducesResponseType<List<TaxTypeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTaxTypes(CancellationToken cancellationToken)
    {
        try
        {
            var companyId = this.GetCompanyId();
            var command = new GetAllTaxTypesCommand(companyId);
            var result = await _getAllHandler.HandleAsync(command, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un tipo de impuesto por ID
    /// </summary>
    /// <returns>El tipo de impuesto solicitado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType<TaxTypeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaxTypeById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var companyId = this.GetCompanyId();
            var command = new GetTaxTypeByIdCommand(id, companyId);
            var result = await _getByIdHandler.HandleAsync(command, cancellationToken);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de impuesto
    /// </summary>
    /// <returns>El ID del tipo de impuesto creado</returns>
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTaxType([FromBody] CreateTaxTypeDto request, CancellationToken cancellationToken)
    {
        try
        {
            var companyId = this.GetCompanyId();
            var command = new CreateTaxTypeCommand(request, companyId);
            var id = await _createHandler.HandleAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetTaxTypeById), new { id }, id);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un tipo de impuesto existente
    /// </summary>
    /// <returns>Sin contenido en caso de éxito</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTaxType(Guid id, [FromBody] UpdateTaxTypeDto request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest("ID mismatch");

        try
        {
            var companyId = this.GetCompanyId();
            var command = new UpdateTaxTypeCommand(request, companyId);
            await _updateHandler.HandleAsync(command, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { message = ex.Message })
                : BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un tipo de impuesto (soft delete)
    /// </summary>
    /// <returns>Sin contenido en caso de éxito</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTaxType(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var companyId = this.GetCompanyId();
            var command = new DeleteTaxTypeCommand(id, companyId);
            await _deleteHandler.HandleAsync(command, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
