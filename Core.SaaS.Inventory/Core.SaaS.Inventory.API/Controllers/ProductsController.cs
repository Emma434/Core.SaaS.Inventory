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
    }
}
