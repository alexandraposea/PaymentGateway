using PaymentGatewayModels;
using System;
using System.Collections.Generic;

namespace PaymentGatewayData
{
    public class Database
    {
        public List<Person> Persons = new List<Person>();
        public List<Transaction> Transactions = new List<Transaction>();
        public List<Product> Products = new List<Product>();
        public List<Account> Accounts = new List<Account>();
        public List<ProductXTransaction> ProductXTransactions = new List<ProductXTransaction>();

        private static Database _instance;
        public static Database GetInstance() { 
            if(_instance == null)
            {
                _instance = new Database();
            }
            return _instance;
        }

        public void SaveChanges()
        {
            Console.WriteLine("Save changes to database");
        }
    }
}
