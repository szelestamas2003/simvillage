namespace SimVillage.Model.Building
{
    public class PowerPlant : Building
    {
        int GeneratedPower = 1000;
        public PowerPlant()
        {
            PowerConsumption = 0;
            Size = (2, 2);
        }
        public int GetGeneratedPower() {  return GeneratedPower; }
        public void SetGeneratedPower(int value) {  GeneratedPower = value; }

    }
}
