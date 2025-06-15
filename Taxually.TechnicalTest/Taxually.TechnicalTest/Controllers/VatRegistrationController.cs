using Microsoft.AspNetCore.Mvc;
using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taxually.TechnicalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VatRegistrationController : ControllerBase
    {
        private readonly RegistrationResolver _resolver;

        public VatRegistrationController(RegistrationResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// Registers a company for a VAT number in a given country
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] VatRegistrationRequest request, CancellationToken ct)
        {
            if (request is null)
                return BadRequest("Request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.Country))
                return BadRequest("Country code is required.");
            try
            {
                var handler = _resolver.Resolve(request.Country);
                await handler.RegisterAsync(request, ct);
                return Ok("Registration request has been successfully processed.");
            }
            catch (NotSupportedException ex)
            {
                return BadRequest($"Country not supported: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Invalid operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }


}
