namespace SimVillage.Model.Building
{
    public class FireDepartment : Building
    {
        bool UnitAvailable;
        int Radius = 40;
        public FireDepartment(int x, int y)
        {
            PowerConsumption = 45;
            Size = (1, 1);
            Cost = 500;
            X = x;
            Y = y;
        }
        public bool IsAvailable() { return  UnitAvailable; }
        public void SendUnit()
        {
            UnitAvailable = false;
        }
        public void UnitArrive()
        {
            UnitAvailable = true;
        }

        public int GetRadius() { return Radius; }
        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nUnit available: " + UnitAvailable + "\nMaintenance cost: "+ Cost/100 + "\nRadius: "+ Radius;
        }
    }
}
