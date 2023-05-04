namespace SimVillage.Model
{
    public class Finances
    {
        private List<Transaction> expenses = null!;
        private List<Transaction> incomes = null!;

        public List<Transaction> Expenses { get { return expenses; } }
        public List<Transaction> Incomes { get { return incomes; } }

        private int budget;
        private int ResidentTax = 10;
        private int StoreTax = 10;
        private int IndustrialTax = 10;


        public Finances(int budget)
        {
            this.budget = budget;
            expenses = new List<Transaction>();
            incomes = new List<Transaction>();
        }

        public void addExpenses(string name, int cost, string date)
        {
            expenses.Add(new Transaction(name, cost, date));
            budget -= cost;
        }

        public void setTax(ZoneType zone, int tax)
        {
            switch (zone)
            {
                case ZoneType.Residental:
                    ResidentTax = tax;
                    break;
                case ZoneType.Industrial:
                    IndustrialTax = tax;
                    break;
                case ZoneType.Store:
                    StoreTax = tax;
                    break;
            }
        } 

        public int getTax(ZoneType zone)
        {
            switch (zone)
            {
                case ZoneType.Residental:
                    return ResidentTax;
                case ZoneType.Industrial:
                    return IndustrialTax;
                case ZoneType.Store:
                    return StoreTax;
                default:
                    throw new ArgumentNullException();
            }
        }

        public void addIncome(string name, int cost, string date)
        {
            incomes.Add(new Transaction(name, cost, date));
            budget += cost;
        }

        public int getBudget() { return budget; }
    }
}