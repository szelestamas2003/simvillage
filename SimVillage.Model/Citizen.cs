using SimVillage.Model.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model
{
    public class Citizen
    {
        int Age;
        int Salary;
        EducationLevel EducationLevel;
        bool Pensioner;
        int Happiness;
        Building.Residental Home;
        Building.Building WorkPlace;


        public Citizen(int Age, Building.Residental Home)
        {
            this.Age = Age;
            this.Home = Home;
            Salary = 0;
            EducationLevel = EducationLevel.Basic;
            Pensioner = false;
            Happiness = calcHappiness();
        }
        public int calcHappiness()
        {
            Happiness = 0;
            int work_distance = City.calcDistance(Home, WorkPlace);
            work_distance = 15 - work_distance;
            Happiness += work_distance;
            Happiness += Salary / 10;
            if(EducationLevel == EducationLevel.Basic)
            {
                Happiness -= 5;
            }
            else if(EducationLevel == EducationLevel.Middle) 
            {
                Happiness += 5;
            }
            else
            {
                Happiness += 10;
            }
            return Happiness;

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
            WorkPlace = null;
        }
        public void SetEducation(EducationLevel level)
        {
            EducationLevel = level;
        }
        public void MoveOut()
        {
            WorkPlace = null;
            Home = null;
            Salary = 0;
        }
        public void SetWorkPlace(Building.Building work)
        {
            WorkPlace = work;
        }
        static public Citizen ReGen(Building.Residental home)
        {
            Random r = new Random();
            int age = r.Next(18,60);
            Citizen citizen = new Citizen(age, home);
            return citizen;
        }
        static public Citizen ReGen18(Building.Residental home)
        {
            Citizen citizen = new Citizen(18, home);
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
