namespace SimVillage.Model
{
    public class Finances
    {
        private List<(string, int, DateTime)> expenses = null!;
        private List<(string, int, DateTime)> incomes = null!;

        private int budget;
        private int ResidentTax = 10;
        private int StoreTax = 10;
        private int IndustrialTax = 10;


        public Finances(int budget)
        {
            this.budget = budget;
            expenses = new List<(string, int, DateTime)>();
            incomes = new List<(string, int, DateTime)>();
        }

        public void addExpenses(string name, int cost, DateTime date)
        {
            expenses.Add((name, cost, date));
            budget -= cost;
        }

        public int getTax(ZoneType zone)
        {
            if(zone  == ZoneType.Industrial) return IndustrialTax;
            else return StoreTax;
        }

        public void addIncome(string name, int cost, DateTime date)
        {
            incomes.Add((name, cost, date));
            budget += cost;
        }

        public int getBudget() { return budget; }
    }
}