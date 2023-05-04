using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model
{
    public class Transaction
    {
        public Transaction(string name, int amount, string date)
        {
            Name = name;
            Amount = amount;
            Date = date;
        }

        public string Name { get; set; }
        public int Amount { get; set; }
        public string Date { get; set; }
    }
}
