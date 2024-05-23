using EditorAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using EditorAPI.Data.Entities;
using Microsoft.AspNetCore.Authorization;


namespace EditorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string endpointSecret;

        public PaymentController(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            endpointSecret = configuration["Stripe:WebHookSecret"]?? throw new Exception("Stripe webhook secret is not set.");
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetPaymentLink(PaymentLinkDetails details)
        {
            var priceOptions = new PriceListOptions
            {
                Product = details.ProductId,
            };
            var priceService = new PriceService();
            StripeList<Price> prices;
            try
            {
                prices = priceService.List(priceOptions);
            }
            catch (StripeException ex)
            {

                return BadRequest(ex.Message);
            }
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = prices.Data[0].Id,
                    Quantity = details.Amount,
                  },
                },
                ClientReferenceId = User.Identity!.Name,
                Mode = "payment",
                SuccessUrl = details.ReturnLink,
            };
            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(
                new
                {
                    User = User.Identity.Name,
                    PaymentUrl = session.Url
                });
        }
        [HttpPost("Webhook")]
        public async Task<IActionResult> EventListenerAsync([FromBody] JsonElement stripeEventJson)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(stripeEventJson.ToString(),
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var sessionService = new SessionService();
                    var session = (Session)stripeEvent.Data.Object;

                    var user = await _userManager.FindByNameAsync(session.ClientReferenceId);
                    if (user != null)
                    {
                        var products = await sessionService.ListLineItemsAsync(session.Id);
                        var product = products.First();
                    
                        user.DocumentsLimit += (int)product.Quantity!;
                        await _userManager.UpdateAsync(user);
                    }
                }
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}