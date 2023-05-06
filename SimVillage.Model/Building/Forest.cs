namespace SimVillage.Model.Building
{
    public class Forest : Building
    {
        int Age;
        int Radius = 3;
        public Forest(int x, int y)
        {
            Age = 0;
            Size = (1, 1);
            PowerConsumption = 3;
            Cost = 80;
            IsAccessible = true;
            X = x;
            Y = y;
        }

        public void SetAge(int age)
        {
            Age = age;
        }

        public int GetRadius() { return Radius; }

        public void AgeUp()
        {
            Age += 1;
        }
        public int GetAge() { return Age; }

        public override String ToString()
        {
            return "Age: " + Age + "\nPower consumption: " + PowerConsumption + "\nRadius: " + Radius;
        }
        
    }
}
