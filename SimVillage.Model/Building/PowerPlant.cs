namespace SimVillage.Model.Building
{
    public class PowerPlant : Building
    {
        int GeneratedPower = 1000;
        public PowerPlant(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(0);
            Size = (2, 2);
            cost = 1000;
        }
        public int GetGeneratedPower() {  return GeneratedPower; }
        public void SetGeneratedPower(int value) {  GeneratedPower = value; }

        public override String ToString()
        {
            return "Generated Power: " + GeneratedPower + "\nMaintenance cost: " + cost / 100;
        }

    }
}
