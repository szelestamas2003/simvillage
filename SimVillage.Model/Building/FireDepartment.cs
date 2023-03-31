namespace SimVillage.Model.Building
{
    public class FireDepartment : Building
    {
        bool UnitAvailable;

        public FireDepartment(int x, int y)
        {
            PowerConsumption = 45;
            X = x;
            Y = y;
            Cost = 30;
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
    }
}
