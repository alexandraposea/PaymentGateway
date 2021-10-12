using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApplication.WriteOperations;
using PaymentGatewayPublishedLanguage.WriteSide;

namespace PaymentGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CreateAccountOperation _createAccountCommandHandler;
        public AccountController(CreateAccountOperation createAccountCommandHandler)
        {
            _createAccountCommandHandler = createAccountCommandHandler;
        }

        [HttpPost]
        [Route("Create")]
        public string CreateAccount(CreateAccountCommand command)
        {
            //CreateAccount request = new CreateAccount(new EventSender());
            _createAccountCommandHandler.PerformOperation(command);
            return "OK";
        }
    }
}