using PaymentGatewayModels;
using System;
using System.Collections.Generic;

namespace PaymentGatewayData
{
    public class Database
    {
        public List<Person> Persons = new();
        public List<Transaction> Transactions = new();
        public List<Product> Products = new();
        public List<Account> Accounts = new();
        public List<ProductXTransaction> ProductXTransactions = new();

        public static void SaveChanges()
        {
            Console.WriteLine("Save changes to database");
        }
    }
}
