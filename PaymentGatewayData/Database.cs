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

        private static Database _instance;
        public static Database GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Database();
            }
            return _instance;
        }

        public static void SaveChanges()
        {
            Console.WriteLine("Save changes to database");
        }
    }
}
