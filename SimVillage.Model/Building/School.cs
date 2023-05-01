using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class School : Building
    {
        int MaxStudents;
        SchoolTypes Type;
        List<Citizen> Students;

        public School(List<Tile> tile, SchoolTypes type)
        {
            MaxStudents = 300;
            SetTiles(tile);
            SetPowerConsumption(50);
            Type = type;
            cost = 600;
            Students = new List<Citizen>();
        }

        public int GetMaxStudent()
        {
            return MaxStudents;
        }
        public int GetStudents()
        {
            return Students.Count;
        }
        public void SetStudents(Citizen student)
        {
            Students.Append(student);
        }
        public SchoolTypes GetSchoolType()
        {
            return Type;
        }
        public void GiveEducation()
        {
            foreach(Citizen student in Students)
            {
                if(Type == SchoolTypes.Elementary)
                {
                    student.SetEducation(EducationLevel.Middle);
                }
                else if(Type == SchoolTypes.University)
                {
                    student.SetEducation(EducationLevel.Higher);
                }
            }
        }

        public String ToString()
        {
            return "School type: " + Type + " Current students " + Students.Count + " Maximum students: " + MaxStudents + " Power consumption: " + PowerConsumption + " Maintenance cost: " + cost / 100;
        }


    }
}
