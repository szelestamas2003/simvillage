namespace SimVillage.Model
{
    public class Finances
    {
        public List<Transaction> Expenses { get; set; }
        public List<Transaction> Incomes { get; set; }

        public int ResidentTax { get { return residentTax; } }

        public int IndustrialTax { get { return industrialTax; } }

        public int StoreTax { get { return storeTax; } }

        public int Budget { get { return budget; } }

        private int budget;
        private int residentTax = 10;
        private int storeTax = 10;
        private int industrialTax = 10;


        public Finances(int budget)
        {
            this.budget = budget;
            Expenses = new List<Transaction>();
            Incomes = new List<Transaction>();
        }

        public void addExpenses(string name, int cost, string date)
        {
            Expenses.Add(new Transaction(name, cost, date));
            budget -= cost;
        }

        public void setTax(ZoneType zone, int tax)
        {
            switch (zone)
            {
                case ZoneType.Residental:
                    residentTax = tax;
                    break;
                case ZoneType.Industrial:
                    industrialTax = tax;
                    break;
                case ZoneType.Store:
                    storeTax = tax;
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
                    return 0;
            }
        }

        public void addIncome(string name, int cost, string date)
        {
            Incomes.Add(new Transaction(name, cost, date));
            budget += cost;
        }

        public int getBudget() { return budget; }
    }
}