using Core.SaaS.Inventory.Application.Interfaces;
using Core.SaaS.Inventory.Domain.Entities;
using Core.SaaS.Inventory.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.SaaS.Inventory.Application.Features.Products.Commands;
using Core.SaaS.Inventory.Application.Features.Products.Queries;

namespace Core.SaaS.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        // El controlador solo conoce a MediatR (El mesero principal)
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            // Solo le pasamos la "pregunta" a MediatR, él busca al lector correspondiente
            var products = await _mediator.Send(new GetProductsQuery());
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            // Magia CQRS: Le entregamos el "papel" a MediatR, y él se encarga de buscar al "cocinero"
            var productId = await _mediator.Send(command);

            return Ok(new { ProductId = productId, Message = "Producto creado con éxito usando CQRS" });
        }


        //endpoint del Stock
        [HttpPost("{id}/stock")]
        public async Task<IActionResult> AdjustStock(Guid id, [FromBody] AdjustStockRequest request)
        {
            // 1. Armamos el (Command) uniendo el ID de la URL y los datos del Body
            var command = new AdjustStockCommand
            {
                ProductId = id,
                Type = request.Type,
                Quantity = request.Quantity,
                Reason = request.Reason
            };

            // 2. Le pasamos el command a MediatR. Él buscará automáticamente al AdjustStockCommandHandler.
            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok(new { Message = "Movimiento de stock registrado con éxito." });
            }

            return BadRequest("No se pudo registrar el movimiento.");
        }

        // 3. Creamos un DTO (Data Transfer Object) auxiliar al final del archivo 
        // para definir exactamente qué esperamos recibir en el JSON del Body.
        public class AdjustStockRequest
        {
            public MovementType Type { get; set; }
            public int Quantity { get; set; }
            public string Reason { get; set; } = string.Empty;
        }
    }
}
