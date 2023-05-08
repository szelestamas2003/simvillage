using SimVillage.Model.Building;

namespace SimVillage.Model
{
    public class Citizen
    {
        public int Age { get; private set; }
        public int Salary { get; private set; }
        public EducationLevel EducationLevel { get; private set; }
        public bool Pensioner { get; private set; }
        public int Happiness { get; private set; }
        public int HadToMove { get; private set; } = 0;
        public Residental Home { get; private set; }
        public Building.Building WorkPlace { get; private set; } = null!;


        private Citizen(int Age, Residental Home)
        {
            this.Age = Age;
            this.Home = Home;
            Salary = 0;
            EducationLevel = EducationLevel.Basic;
            Pensioner = false;
        }
        public void SetHappiness(int happiness)
        {
            this.Happiness = happiness;
        }
        public int GetHappiness()
        {
            return Happiness;
        }
       

        public void PlusHadToMove()
        {
            HadToMove++;
        }

        public Residental GetHome() { return Home; }

        public void SetHome(Residental home)
        {
            Home = home;
        }

        public Building.Building GetWorkPlace() { return WorkPlace; }

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
        }
        public void SetEducation(EducationLevel level)
        {
            EducationLevel = level;
        }
        public void MoveOut()
        {
            WorkPlace = null!;
            Home = null!;
            Salary = 0;
        }
        public void SetWorkPlace(Building.Building work)
        {
            WorkPlace = work;
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
        public EducationLevel GetEducation()
        {
            return EducationLevel;
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
        public void SetSalary(int salary)
        {
            Salary = salary;
        }
        public int GetSalary() {
            return Salary;
        }
        public int GetHadToMove()
        {
            return HadToMove;
        }
    }
}
