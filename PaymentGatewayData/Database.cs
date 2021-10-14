using PaymentGateway.Models;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Data
{
    public class Database
    {
        public List<Person> Persons = new();
        public List<Transaction> Transactions = new();
        public List<Product> Products = new();
        public List<Account> Accounts = new();
        public List<ProductXTransaction> ProductXTransactions = new();

        public void SaveChanges()
        {
            Console.WriteLine("Save changes to database");
        }
    }
}
