using Abstractions;
using PaymentGatewayData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayApplication.ReadOperations
{
    public class ListOfAccounts
    {
        public class Validator : IValidator<Query>
        {
            private readonly Database _database;
            public Validator(Database database)
            {
                _database = database;
            }

            public bool Validate(Query input)
            {
                var person = input.PersonId.HasValue ?
                     _database.Persons.FirstOrDefault(x => x.PersonId == input.PersonId) :
                     _database.Persons.FirstOrDefault(x => x.Cnp == input.Cnp);

                return person != null;
            }
        }

        public class Query
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IReadOperation<Query, List<Model>>
        {
            private readonly Database _database;
            private readonly IValidator<Query> _validator;

            public QueryHandler(Database database, IValidator<Query> validator)
            {
                _database = database;
                _validator = validator;
            }
            public List<Model> PerformOperation(Query query)
            {
                var isValid = _validator.Validate(query);

                if (!isValid)
                {
                    throw new Exception("Person not found!");
                }

                //var person = query.PersonId.HasValue ?
                //    _database.Persons.FirstOrDefault(x => x.PersonId == query.PersonId) :
                //    _database.Persons.FirstOrDefault(x => x.Cnp == query.Cnp);

                //if (person == null)
                //{
                //    throw new Exception("Person not found!");
                //}

                var db = _database.Accounts.Where(x => x.PersonId == query.PersonId);
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    IbanCode = x.IbanCode,
                    Type = x.Type,
                    Status = x.Status,
                    Limit = x.Limit
                }).ToList();
                return result;

            }
        }

        public class Model
        {
            public int? AccountId { get; set; }
            public double Balance { get; set; }
            public string Currency { get; set; }
            public string IbanCode { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
            public double Limit { get; set; }
        }
    }
}
