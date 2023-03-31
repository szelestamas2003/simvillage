namespace SimVillage.Model.Building
{
    public class Forest : Building
    {
        int Age;
        public Forest() {
            Age = 0;
            PowerConsumption = 0;
            Occupied = false;
            Size = (1, 1);
            Cost = 30;
        }

        public Forest(int age)
        {
            Age = age;
        }

        public void AgeUp()
        {
            Age += 1;
        }
        public int GetAge() { return Age; }

        
    }
}
