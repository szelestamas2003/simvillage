namespace SimVillage.Model.Building
{
    public class PowerPlant : Building
    {
        int GeneratedPower = 1000;
        public PowerPlant(int x, int y)
        {
            FireChance = 2;
            IsOnFire = false;
            Health = 250;
            PowerConsumption = 0;
            Size = (2, 2);
            Cost = 1000;
            X = x;
            Y = y;
        }
        public int GetGeneratedPower() {  return GeneratedPower; }

        public override String ToString()
        {
            return "Generated Power: " + GeneratedPower + "\nMaintenance cost: " + Cost / 100 + "\nHealth: " + Health + "\n";
        }

    }
}
