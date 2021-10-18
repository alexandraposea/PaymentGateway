using FluentValidation;
using MediatR;
using PaymentGateway.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Queries
{
    public class ListOfAccounts
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator(PaymentDbContext _dbContext)
            {
                RuleFor(q => q).Must(query =>
                {
                    var person = query.PersonId.HasValue ?
                        _dbContext.Persons.FirstOrDefault(x => x.PersonId == query.PersonId) :
                        _dbContext.Persons.FirstOrDefault(x => x.Cnp == query.Cnp);
                    return person != null;
                }).WithMessage("Customer not found");
            }
        }

        public class Validator2 : AbstractValidator<Query>
        {
            public Validator2()
            {
                RuleFor(q => q).Must(q =>
                {
                    return q.PersonId.HasValue || !string.IsNullOrEmpty(q.Cnp);
                }).WithMessage("Customer data is invalid");

                RuleFor(q => q.Cnp).Must(cnp =>
                {
                    if (string.IsNullOrEmpty(cnp))
                    {
                        return true;
                    }
                    return cnp.Length == 13;
                }).WithMessage("CNP doesn't have the required length");

                RuleFor(q => q.PersonId).Must(personId =>
                {
                    if (!personId.HasValue)
                    {
                        return true;
                    }
                    return personId.Value > 0;
                }).WithMessage("Person id must be positive");
            }
        }

        public class Query : IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly PaymentDbContext _dbContext;

            public QueryHandler(PaymentDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var person = request.PersonId.HasValue ?
                    _dbContext.Persons.FirstOrDefault(x => x.PersonId == request.PersonId) :
                    _dbContext.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

                var db = _dbContext.Accounts.Where(x => x.PersonId == request.PersonId);
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    IbanCode = x.IbanCode,
                    Type = x.Type,
                    Status = x.Status,
                    Limit = x.Limit
                }).ToList();
                return Task.FromResult(result);
            }
        }

        public class Model
        {
            public int? AccountId { get; set; }
            public decimal Balance { get; set; }
            public string Currency { get; set; }
            public string IbanCode { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
            public decimal Limit { get; set; }
        }
    }
}
