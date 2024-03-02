using SimVillage.Model.Building;

namespace SimVillage.Model
{
    public class Citizen
    {
        public int Age { get; private set; }
        public int Salary { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public bool Pensioner { get; set; }
        public int Happiness { get; set; }
        public int HadToMove { get; set; } = 0;
        public Residental Home { get; set; }
        public Building.Building? WorkPlace { get; set; } = null;
        public List<double> PaidTaxes { get; set; } = null!;
        public double Pension { get; set; }


        public Citizen(int Age, Residental Home)
        {
            this.Age = Age;
            this.Home = Home;
            Salary = 0;
            EducationLevel = EducationLevel.Basic;
            Pensioner = false;
            PaidTaxes = new List<double>();
        }

        public void PlusHadToMove()
        {
            HadToMove++;
        }

        public bool AgeUp()
        {
            Age++;
            if (Age >= 65)
            {
                if (!Pensioner)
                {
                    Retire();
                }
                else
                {
                    if (ChanceToPassAway())
                    {
                        MoveOut();
                        return false;
                    }
                }
            }
            return true;
        }
        public void Retire()
        {
            Pensioner = true;
            Salary = 0;
            WorkPlace = null!;
            Pension = PaidTaxes.Sum() / PaidTaxes.Count;
        }

        public void MoveOut()
        {
            WorkPlace = null!;
            Home = null!;
            Salary = 0;
        }

        static public Citizen ReGen(Residental home)
        {
            Random r = new Random();
            int age = r.Next(18,60);
            Citizen citizen = new Citizen(age, home);
            return citizen;
        }

        static public Citizen ReGen18()
        {
            Citizen citizen = new Citizen(18, null!);
            return citizen;
        }

        public bool ChanceToPassAway()
        {
            Random r = new Random();
            double chance = 100;
            for (int i = 65; i < Age; i++)
            {
                chance *= 0.95;
            }
            if (r.Next(0,100) > chance)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
