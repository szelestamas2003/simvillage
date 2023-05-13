﻿namespace SimVillage.Model.Building
{
    public class School : Building
    {
        int MaxStudents;
        public SchoolTypes Type { get; private set; }
        public List<Citizen> Students { get; set; }

        public School(int x, int y, SchoolTypes type)
        {
            MaxStudents = 300;
            PowerConsumption = 50;
            if (type == SchoolTypes.Elementary)
                Size = (1, 2);
            else
                Size = (2, 2);
            Type = type;
            Cost = 600;
            X = x;
            Y = y;
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
            if (Students.Count < MaxStudents)
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
                    if (student.GetWorkPlace() != null)
                        student.SetSalary(1000);
                }
                else if(Type == SchoolTypes.University)
                {
                    student.SetEducation(EducationLevel.Higher);
                    if (student.GetWorkPlace() != null)
                        student.SetSalary(1500);
                }
            }
        }

        public override String ToString()
        {
            return "School type: " + Type + "\nCurrent students " + Students.Count + "\nMaximum students: " + MaxStudents + "\nPower consumption: " + PowerConsumption + "\nMaintenance cost: " + Cost / 100;
        }


    }
}
