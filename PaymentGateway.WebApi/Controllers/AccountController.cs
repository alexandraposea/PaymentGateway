using Microsoft.AspNetCore.Mvc;
using PaymentGatewayApplication.ReadOperations;
using PaymentGatewayApplication.WriteOperations;
using PaymentGatewayPublishedLanguage.WriteSide;
using System.Collections.Generic;

namespace PaymentGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CreateAccountOperation _createAccountCommandHandler;
        private readonly ListOfAccounts.QueryHandler _queryHandler;

        public AccountController(CreateAccountOperation createAccountCommandHandler, ListOfAccounts.QueryHandler queryHandler)
        {
            _createAccountCommandHandler = createAccountCommandHandler;
            _queryHandler = queryHandler;
        }


        [HttpPost]
        [Route("Create")]
        public string CreateAccount(CreateAccountCommand command)
        {
            //CreateAccount request = new CreateAccount(new EventSender());
            _createAccountCommandHandler.PerformOperation(command);
            return "OK";
        }

        [HttpGet]
        [Route("ListOfAccounts")]
        public List<ListOfAccounts.Model> GetListOfAccounts([FromQuery] ListOfAccounts.Query query)
        {
            var result = _queryHandler.PerformOperation(query);
            return result;
        }
    }
}