namespace SimVillage.Model
{
    public class Finances
    {
        public List<Transaction> Expenses { get; set; }
        public List<Transaction> Incomes { get; set; }

        public int ResidentTax { get; set; } = 10;

        public int IndustrialTax { get; set; } = 10;

        public int StoreTax { get; set; } = 10;

        public int Budget { get; private set; }

        public int NegativeBudgetYears { get; set; } = 0;

        public Finances(int budget)
        {
            Budget = budget;
            Expenses = new List<Transaction>();
            Incomes = new List<Transaction>();
        }

        public void AddExpenses(string name, int cost, string date)
        {
            Expenses.Add(new Transaction(name, cost, date));
            Budget -= cost;
        }

        public void AddIncome(string name, int cost, string date)
        {
            Incomes.Add(new Transaction(name, cost, date));
            Budget += cost;
        }
    }
}